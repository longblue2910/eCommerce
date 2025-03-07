using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Commands.Role;

public record CreateRoleCommand(string Name) : IRequest<Guid>;

public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Guid>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = new Domain.Entities.Role(Guid.NewGuid(),request.Name);
        await _roleRepository.AddAsync(role);
        await _unitOfWork.CommitAsync();
        return role.Id;
    }
}
