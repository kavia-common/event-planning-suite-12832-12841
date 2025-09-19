using EventPlanner.Domain.Models;
using EventPlanner.Domain.Repositories;

namespace EventPlanner.Application.Services;

/// PUBLIC_INTERFACE
/// <summary>
/// User profile operations.
/// </summary>
public interface IUserService
{
    User? GetById(Guid id);
}

public class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users) => _users = users;

    public User? GetById(Guid id) => _users.GetById(id);
}
