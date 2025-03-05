using Application.DTOs.Auth;
using MediatR;

namespace Application.Commands;

public record LoginCommand(string Username, string Password, string IpAddress) : IRequest<LoginResponseDto>;
