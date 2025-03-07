using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Common;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.Where(u => u.Username == username).Include(x => x.Roles).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    public void Delete(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task<bool> AssignRolesToUserAsync(Guid userId, List<Role> roles)
    {
        var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        foreach (var role in roles)
        {
            user.AssignRole(role);
        }
        
        return true;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Users.AnyAsync(x => x.Username == username);
    }

    public async Task<PagedResult<User>> GetUsersAsync(Specification<User> specification, int page, int pageSize)
    {
        var query = _context.Users.Where(specification.Criteria);

        int totalCount = await query.CountAsync(); // 🔥 Tổng số bản ghi
        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<User>(users, totalCount, page, pageSize);
    }

    public async Task<int> CountUsersAsync(UserSpecification spec)
    {
        return await _context.Users.Where(spec.Criteria).CountAsync();
    }

}