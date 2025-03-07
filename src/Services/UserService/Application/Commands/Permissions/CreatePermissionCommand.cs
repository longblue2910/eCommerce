using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Permissions.Commands;

public record CreatePermissionCommand(string Name, string Description) : IRequest<Guid>;

public class CreatePermissionCommandHandler(IPermissionRepository permissionRepository)
    : IRequestHandler<CreatePermissionCommand, Guid>
{
    private readonly IPermissionRepository _permissionRepository = permissionRepository;

    public async Task<Guid> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = new Permission(Guid.NewGuid(), request.Name, request.Description);
        await _permissionRepository.AddAsync(permission);
        return permission.Id;
    }
}
