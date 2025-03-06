using Application.Common.Interfaces;
using Infrastructure.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtProvider(JwtSettings jwtSettings) : IJwtProvider
{
    private readonly JwtSettings _jwtSettings = jwtSettings;

    public (string Token, DateTime Expiration) GenerateToken(Guid userId, string username, string email, List<string> roles)
    {
        var expireTime = DateTime.Now.AddHours(3);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Expiration, expireTime.ToString("MMM ddd dd yyyy HH:mm:ss tt")),

            // 🔹 Thêm cả `ClaimTypes.Role` và `"role"` để đảm bảo hoạt động
            .. roles.Select(role => new Claim(ClaimTypes.Role, role)),
            .. roles.Select(role => new Claim("role", role)),
        ];

        var key = Encoding.ASCII.GetBytes(_jwtSettings.IssuerSigningKey);
        var token = new JwtSecurityToken(issuer: _jwtSettings.ValidIssuer,
                                         audience: _jwtSettings.ValidAudience,
                                         claims: claims,
                                         notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                                         expires: new DateTimeOffset(expireTime).DateTime,
                                         signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                                         SecurityAlgorithms.HmacSha256)
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expireTime);

    }
}
