using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> GetByTokenAsync(string token);
    Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);
    Task AddAsync(RefreshToken refreshToken);
    void Remove(RefreshToken refreshToken);
}
