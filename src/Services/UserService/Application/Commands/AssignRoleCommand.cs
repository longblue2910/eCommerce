using MediatR;

namespace Application.Commands;

public record AssignRoleCommand(Guid UserId, List<string> RoleNames) : IRequest<bool>;
