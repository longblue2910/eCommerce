using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role> GetByIdAsync(Guid id);
    Task<List<Role>> GetAllAsync();
    Task AddAsync(Role role);
    void Delete(Role role);

    Task<Role> GetByNameAsync(string roleName);
    Task<List<Role>> GetByNamesAsync(List<string> roleNames); // Thêm phương thức này
}
