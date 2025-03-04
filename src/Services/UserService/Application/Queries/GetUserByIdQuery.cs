using Application.DTOs;
using MediatR;

namespace Application.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>;
