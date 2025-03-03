using SharedKernel.Common;

namespace Domain.Entities;

public class RefreshToken(Guid id, Guid userId, string token, DateTime expiryDate) : Entity<Guid>(id)
{
    public Guid UserId { get; private set; } = userId;
    public string Token { get; private set; } = token;
    public DateTime ExpiryDate { get; private set; } = expiryDate;
    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
}
