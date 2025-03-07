using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id);
    Task<List<Permission>> GetAllAsync();
    Task<List<Permission>> GetByNamesAsync(List<string> permissionNames);
    Task AddAsync(Permission permission);
    void Delete(Permission permission);
}
