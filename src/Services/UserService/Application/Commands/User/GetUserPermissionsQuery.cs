using Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Commands.User;

public record GetUserPermissionsQuery(Guid UserId) : IRequest<List<string>>;

public class GetUserPermissionsQueryHandler(IUserRepository userRepository, ILogger<GetUserPermissionsQueryHandler> logger) : IRequestHandler<GetUserPermissionsQuery, List<string>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger = logger;

    public async Task<List<string>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return [];

        var permissions = user.Roles.SelectMany(r => r.Permissions).Select(p => p.Name).Distinct().ToList();

        _logger.LogInformation("User {UserId} has {Count} permissions.", request.UserId, permissions.Count);

        return permissions;
    }
}