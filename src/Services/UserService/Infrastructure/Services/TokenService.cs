using Application.Common.Interfaces;
using System.Security.Cryptography;

namespace Infrastructure.Services;

public class TokenService : ITokenService
{
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}