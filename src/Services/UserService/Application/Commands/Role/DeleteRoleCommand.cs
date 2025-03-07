using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;
using SharedKernel.Exceptions;

namespace Application.Commands.Role;

// Command: Delete Role
public record DeleteRoleCommand(Guid RoleId) : IRequest;

public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.RoleId) ?? throw new NotFoundException("Role not found");
        _roleRepository.Delete(role);
        await _unitOfWork.CommitAsync();
    }
}