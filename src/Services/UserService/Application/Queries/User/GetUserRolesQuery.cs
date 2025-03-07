﻿using Domain.Entities;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Queries.User;

public record GetUserRolesQuery(Guid UserId) : IRequest<List<Role>>;

public class GetUserRolesQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserRolesQuery, List<Role>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<List<Role>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        return user?.Roles ?? [];
    }
}