using Application.DTOs.Auth;
using MediatR;

namespace Application.Commands.Auth;

public record RefreshTokenCommand(string RefreshToken) : IRequest<TokenResponse>;
