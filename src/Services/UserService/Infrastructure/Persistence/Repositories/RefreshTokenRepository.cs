using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    private readonly AppDbContext _context = context;

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken); 

    }

    public void Remove(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Remove(refreshToken);
    }
}
