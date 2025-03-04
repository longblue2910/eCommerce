using MediatR;

namespace Application.Commands;

public record ChangePasswordCommand(Guid UserId, string NewPassword) : IRequest<bool>;
