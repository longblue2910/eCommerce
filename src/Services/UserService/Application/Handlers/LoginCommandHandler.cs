using Application.Commands;
using Application.Commands.Auth;
using Application.Common.Interfaces;
using Application.DTOs.Auth;
using Application.Exceptions;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;

public class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<LoginCommand, TokenResponse>,
                                                      IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<TokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        // Tạo Access Token
        var (accessToken, expiration) = _jwtProvider.GenerateToken(user.Id, user.Username, user.Email, user.Roles.Select(r => r.Name).ToList());

        // Tạo Refresh Token
        var refreshTokenString = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken(
           Guid.NewGuid(),
           user.Id,
           refreshTokenString,
           DateTime.Now.AddDays(14),
           ""
        );
        // Lưu Refresh Token vào database
        await _refreshTokenRepository.AddAsync(refreshToken);

        await _unitOfWork.CommitAsync();

        return new TokenResponse(accessToken, refreshTokenString, expiration);
    }

    public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var rToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (rToken == null || rToken.ExpiryDate < DateTime.Now)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = await _userRepository.GetByIdAsync(rToken.UserId);

        var (accessToken, expiration) = _jwtProvider.GenerateToken(user.Id, user.Username, user.Email, user.Roles.Select(r => r.Name).ToList());
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Cập nhật Refresh Token mới
        rToken.Replace(newRefreshToken);

        rToken.Revoke();
        await _unitOfWork.CommitAsync();

        return new TokenResponse(accessToken, newRefreshToken, expiration);

    }
}
