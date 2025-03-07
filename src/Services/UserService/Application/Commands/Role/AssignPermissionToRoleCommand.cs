using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;
using SharedKernel.Exceptions;

namespace Application.Commands.Role;

// Command: Assign Permission to Role
public record AssignPermissionToRoleCommand(Guid RoleId, Guid PermissionId) : IRequest;

public class AssignPermissionToRoleHandler(IRoleRepository roleRepository, IPermissionRepository permissionRepository, IUnitOfWork unitOfWork) : IRequestHandler<AssignPermissionToRoleCommand>
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IPermissionRepository _permissionRepository = permissionRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId) ?? throw new NotFoundException("Role not found");
        var permission = await _permissionRepository.GetByIdAsync(request.PermissionId) ?? throw new NotFoundException("Permission not found");
        role.AddPermission(permission);
        await _unitOfWork.CommitAsync();
    }
}