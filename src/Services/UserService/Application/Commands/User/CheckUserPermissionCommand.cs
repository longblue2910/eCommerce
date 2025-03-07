using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.User;

public record CheckUserPermissionCommand(Guid UserId, string PermissionName) : IRequest<bool>;

public class CheckUserPermissionCommandHandler(IUserRepository userRepository, ILogger<CheckUserPermissionCommandHandler> logger) :
    IRequestHandler<CheckUserPermissionCommand, bool>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<CheckUserPermissionCommandHandler> _logger = logger;

    public async Task<bool> Handle(CheckUserPermissionCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return false;

        bool hasPermission = user.Roles.Any(r => r.Permissions.Any(p => p.Name == request.PermissionName));

        _logger.LogInformation("User {UserId} has permission {PermissionName}: {Result}", request.UserId, request.PermissionName, hasPermission);

        return hasPermission;
    }
}