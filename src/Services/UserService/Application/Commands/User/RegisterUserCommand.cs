using MediatR;

namespace Application.Commands.User;

public record RegisterUserCommand(
    string Username,
    string Email,
    string Password,
    string FullName,
    string PhoneNumber
) : IRequest<Guid>;
