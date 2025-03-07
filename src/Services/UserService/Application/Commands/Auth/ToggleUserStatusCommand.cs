using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;
using SharedKernel.Exceptions;

namespace Application.Commands.Auth;

public class ToggleUserStatusCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
}

public class ToggleUserStatusHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<ToggleUserStatusCommand, bool>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    

    public async Task<bool> Handle(ToggleUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException("User not found");
        user.ToggleStatus();

        await _unitOfWork.CommitAsync();
        return user.IsActive;
    }
}
