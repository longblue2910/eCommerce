using MediatR;

namespace Application.Commands.User;

public record ChangePasswordCommand(Guid UserId, string NewPassword) : IRequest<bool>;
