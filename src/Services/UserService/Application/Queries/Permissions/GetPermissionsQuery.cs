using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Queries.Permissions;

public record GetPermissionsQuery : IRequest<List<Permission>>;

public class GetPermissionsQueryHandler(IPermissionRepository permissionRepository)
    : IRequestHandler<GetPermissionsQuery, List<Permission>>
{
    private readonly IPermissionRepository _permissionRepository = permissionRepository;

    public async Task<List<Permission>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        return await _permissionRepository.GetAllAsync();
    }
}