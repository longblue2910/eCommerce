using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;

namespace Application.Commands.User;

public record RemoveRoleFromUserCommand(Guid UserId, Guid RoleId) : IRequest<bool>;

public class RemoveRoleFromUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveRoleFromUserCommand, bool>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) ?? throw new Exception("User not found");

        user.RemoveRole(request.RoleId.ToString());
        await _unitOfWork.CommitAsync();
        return true;
    }
}