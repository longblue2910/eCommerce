using Application.Commands;
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
    IRefreshTokenRepository refreshTokenRepository) : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user is null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        // 🔥 Tạo Access Token
        var (accessToken, expiration) = _jwtProvider.GenerateToken(user.Id, user.Username, user.Email, user.Roles.Select(r => r.Name).ToList());

        // 🔥 Tạo Refresh Token
        var refreshTokenString = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken(
           Guid.NewGuid(),
           user.Id,
           refreshTokenString,
           DateTime.UtcNow.AddDays(7),
           request.IpAddress
        );
        // Lưu Refresh Token vào database
        await _refreshTokenRepository.AddAsync(refreshToken);

        await _unitOfWork.CommitAsync();

        return new LoginResponseDto(accessToken, refreshTokenString, expiration);
    }
}
