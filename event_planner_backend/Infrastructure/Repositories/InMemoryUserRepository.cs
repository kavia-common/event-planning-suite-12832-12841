using EventPlanner.Domain.Models;
using EventPlanner.Domain.Repositories;

namespace EventPlanner.Infrastructure.Repositories;

/// <summary>
/// Simple in-memory user repository for development and demos.
/// </summary>
public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public InMemoryUserRepository()
    {
        // Seed a demo user: email: demo@ocean.pro, password: Demo@123 (hash to be set by AuthService on register)
        _users.Add(new User
        {
            Email = "demo@ocean.pro",
            FullName = "Ocean Pro Demo",
            PasswordHash = "" // blank until registered via API in-memory
        });
    }

    public User Add(User user)
    {
        _users.Add(user);
        return user;
    }

    public User? GetByEmail(string email) => _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    public User? GetById(Guid id) => _users.FirstOrDefault(u => u.Id == id);
}
