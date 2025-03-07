using MediatR;

namespace Application.Commands.User;

public record AssignRoleCommand(Guid UserId, List<Guid> RoleIds) : IRequest<bool>;
