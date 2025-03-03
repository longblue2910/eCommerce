using Domain.Entities;

namespace Domain.Repositories;

public interface IUserRoleRepository
{
    Task<List<Role>> GetRolesByUserIdAsync(Guid userId);
    Task AssignRoleToUserAsync(Guid userId, Guid roleId);
    Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
}
