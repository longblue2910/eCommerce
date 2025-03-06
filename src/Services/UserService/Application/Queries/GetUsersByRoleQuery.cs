using Application.DTOs;
using MediatR;

namespace Application.Queries;

public record GetUsersByRoleQuery(string RoleName) : IRequest<List<UserResponse>>;
