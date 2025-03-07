namespace Application.Common.Interfaces;

public interface IJwtProvider
{
    (string Token, DateTime Expiration) GenerateToken(Guid userId, string username, string email, List<string> roles);
    public (string Token, DateTime Expiration) GenerateResetToken(Guid userId);

    public Guid? ValidateResetToken(string token);

}
