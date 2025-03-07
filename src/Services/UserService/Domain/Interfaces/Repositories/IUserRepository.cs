using Domain.Entities;
using Domain.Specifications;
using SharedKernel.Common;

namespace Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);

    Task<User> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    void Update(User user);
    void Delete(User user);
    Task<bool> AssignRolesToUserAsync(Guid userId, List<Role> roles);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<User?> GetByUsernameAsync(string username);
    Task<PagedResult<User>> GetUsersAsync(Specification<User> specification, int page, int pageSize);

    Task<int> CountUsersAsync(UserSpecification spec);
}
