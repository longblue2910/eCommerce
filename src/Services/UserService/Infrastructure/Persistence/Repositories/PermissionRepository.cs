using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PermissionRepository(AppDbContext context) : IPermissionRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Permission?> GetByIdAsync(Guid id)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Permission>> GetAllAsync()
    {
        return await _context.Permissions.ToListAsync();
    }

    public async Task<List<Permission>> GetByNamesAsync(List<string> permissionNames)
    {
        return await _context.Permissions
            .Where(p => permissionNames.Contains(p.Name))
            .ToListAsync();
    }

    public async Task AddAsync(Permission permission)
    {
        await _context.Permissions.AddAsync(permission);
    }

    public void Delete(Permission permission)
    {
        _context.Permissions.Remove(permission);
    }
}
