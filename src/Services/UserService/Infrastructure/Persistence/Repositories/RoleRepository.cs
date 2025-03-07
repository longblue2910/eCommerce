using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RoleRepository(AppDbContext context) : IRoleRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }

    public async Task<List<Role>> GetByNamesAsync(List<string> roleNames)
    {
        return await _context.Roles
            .Where(r => roleNames.Contains(r.Name))
            .ToListAsync();
    }

    public async Task AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
    }

    public void Delete(Role role)
    {
        _context.Roles.Remove(role);
    }

    public async Task<List<Role>> GetByIdsAsync(List<Guid> ids)
    {
        return await _context.Roles.Where(r => ids.Contains(r.Id)).ToListAsync();
    }
}
