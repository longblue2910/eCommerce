using Application.Users.Dtos;
using MediatR;

namespace Application.Queries.User;

public record GetUsersByRoleQuery(string RoleName) : IRequest<List<UserResponse>>;
