using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventPlanner.Application.DTOs;
using EventPlanner.Domain.Models;
using EventPlanner.Domain.Repositories;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EventPlanner.Application.Services;

/// PUBLIC_INTERFACE
/// <summary>
/// Issues JWT tokens and manages user registration and verification.
/// </summary>
public interface IAuthService
{
    AuthResponse Login(LoginRequest req);
    User Register(RegisterRequest req);
    string HashPassword(string password, string salt);
    bool Verify(string password, string salt, string hash);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository users, IConfiguration config)
    {
        _users = users;
        _config = config;
    }

    public AuthResponse Login(LoginRequest req)
    {
        var user = _users.GetByEmail(req.Email);
        if (user == null) throw new UnauthorizedAccessException("Invalid credentials.");
        var parts = user.PasswordHash.Split(':');
        if (parts.Length != 2 || !Verify(req.Password, parts[0], parts[1]))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var (token, expires) = IssueToken(user);
        return new AuthResponse { Token = token, ExpiresAtUtc = expires };
    }

    public User Register(RegisterRequest req)
    {
        if (_users.GetByEmail(req.Email) != null)
            throw new InvalidOperationException("Email already registered.");
        var salt = Guid.NewGuid().ToString("N");
        var hash = HashPassword(req.Password, salt);
        var user = new User
        {
            Email = req.Email,
            FullName = req.FullName,
            PasswordHash = $"{salt}:{hash}"
        };
        return _users.Add(user);
    }

    private (string token, DateTime expires) IssueToken(User user)
    {
        var issuer = _config["JWT__Issuer"] ?? "event-planner";
        var audience = _config["JWT__Audience"] ?? "event-planner-clients";
        var key = _config["JWT__Key"] ?? "super-secret-development-key-change-me";
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(4);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwt, expires);
    }

    public string HashPassword(string password, string salt)
    {
        var saltBytes = Encoding.UTF8.GetBytes(salt);
        var derived = KeyDerivation.Pbkdf2(password, saltBytes, KeyDerivationPrf.HMACSHA256, 10000, 32);
        return Convert.ToBase64String(derived);
    }

    public bool Verify(string password, string salt, string hash) => HashPassword(password, salt) == hash;
}
