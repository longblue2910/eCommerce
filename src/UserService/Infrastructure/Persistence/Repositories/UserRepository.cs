using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
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
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> AssignRolesToUserAsync(Guid userId, List<Role> roles)
    {
        // Mở transaction
        using var transaction = await _context.Database.BeginTransactionAsync();

        var user = await _context.Users.Include(u => u.Roles)
                                         .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return false;

        foreach (var role in roles)
        {
            user.AssignRole(role);
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }
}