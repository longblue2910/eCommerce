using Application.Common.Interfaces;
using Infrastructure.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtProvider(JwtSettings jwtSettings) : IJwtProvider
{
    private readonly JwtSettings _jwtSettings = jwtSettings;

    public (string Token, DateTime Expiration) GenerateResetToken(Guid userId)
    {
        var expireTime = DateTime.Now.AddMinutes(_jwtSettings.ResetTokenExpiryMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Expiration, expireTime.ToString("MMM ddd dd yyyy HH:mm:ss tt")),
            new Claim("ResetPassword", "true") // ✅ Đánh dấu token chỉ dùng cho reset password
        };

        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        var token = new JwtSecurityToken(issuer: _jwtSettings.Issuer,
                                         audience: _jwtSettings.Audience,
                                         claims: claims,
                                         expires: new DateTimeOffset(expireTime).DateTime,
                                         signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                                         SecurityAlgorithms.HmacSha256)
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expireTime);
    }


    public (string Token, DateTime Expiration) GenerateToken(Guid userId, string username, string email, List<string> roles)
    {
        var expireTime = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);
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

        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        var token = new JwtSecurityToken(issuer: _jwtSettings.Issuer,
                                         audience: _jwtSettings.Audience,
                                         claims: claims,
                                         expires: new DateTimeOffset(expireTime).DateTime,
                                         signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                                         SecurityAlgorithms.HmacSha256)
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expireTime);
    }

    public Guid? ValidateResetToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            return Guid.TryParse(userId, out var parsedId) ? parsedId : null;
        }
        catch
        {
            return null;
        }
    }
}
