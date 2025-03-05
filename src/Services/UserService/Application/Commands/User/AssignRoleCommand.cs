using MediatR;

namespace Application.Commands.User;

public record AssignRoleCommand(Guid UserId, List<string> RoleNames) : IRequest<bool>;
