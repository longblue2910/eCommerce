using Domain.Interfaces.Repositories;
using Domain.Interfaces;
using MediatR;
using SharedKernel.Exceptions;

namespace Application.Commands.Role;

public record RemovePermissionFromRoleCommand(Guid RoleId, Guid PermissionId) : IRequest;

public class RemovePermissionFromRoleHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork) : IRequestHandler<RemovePermissionFromRoleCommand>
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId) ?? throw new NotFoundException("Role not found");

        role.RemovePermission(request.PermissionId);
        await _unitOfWork.CommitAsync();
    }
}
