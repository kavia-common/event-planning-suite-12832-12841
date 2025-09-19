using EventPlanner.Domain.Models;

namespace EventPlanner.Domain.Repositories;

/// PUBLIC_INTERFACE
/// <summary>
/// Contract for persisting and retrieving users.
/// </summary>
public interface IUserRepository
{
    User? GetByEmail(string email);
    User? GetById(Guid id);
    User Add(User user);
}
