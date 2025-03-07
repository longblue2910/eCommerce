using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;
using SharedKernel.Exceptions;

namespace Application.Commands.Auth;

public class UpdateProfileCommand : IRequest
{
    public Guid UserId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string ProfilePictureUrl { get; set; }
}

public class UpdateProfileHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateProfileCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new NotFoundException("User not found.");

        user.UpdateProfile(request.FullName, request.PhoneNumber, request.Address, request.ProfilePictureUrl);

        await _unitOfWork.CommitAsync();

    }
}
