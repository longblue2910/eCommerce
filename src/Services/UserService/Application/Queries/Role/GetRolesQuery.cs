using Application.DTOs;
using Domain.Interfaces.Repositories;
using MediatR;
using SharedKernel.Common;

namespace Application.Queries.Role;

public class GetRolesQuery(string keyword, bool? isActive, int pageNumber, int pageSize) : IRequest<PagedResult<RoleResponse>>
{
    public string Keyword { get; set; } = keyword;
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
}


public class GetRolesHandler : IRequestHandler<GetRolesQuery, PagedResult<RoleResponse>>
{
    private readonly IRoleRepository _roleRepository;

    public GetRolesHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<PagedResult<RoleResponse>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var (roles, totalRecords) = await _roleRepository.GetRolesAsync(
            request.Keyword, request.PageNumber, request.PageSize, cancellationToken);

        var roleResponses = roles.Select(r => new RoleResponse
        {
            Id = r.Id,
            Name = r.Name,
            Permissions = r.Permissions.Select(p => p.Name).ToList()
        }).ToList();

        return new PagedResult<RoleResponse>(roleResponses, totalRecords, request.PageNumber, request.PageSize);
    }
}
