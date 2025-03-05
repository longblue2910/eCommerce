namespace Application.Common.Interfaces;

public interface IJwtProvider
{
    (string Token, DateTime Expiration) GenerateToken(Guid userId, string username, string email, List<string> roles);
}
