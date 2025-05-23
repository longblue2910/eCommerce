﻿namespace Application.DTOs.Auth;

public class TokenResponse(string accessToken, string refreshToken, DateTime expiration)
{
    public string AccessToken { get; set; } = accessToken;
    public string RefreshToken { get; set; } = refreshToken;
    public DateTime Expiration { get; set; } = expiration;
}
