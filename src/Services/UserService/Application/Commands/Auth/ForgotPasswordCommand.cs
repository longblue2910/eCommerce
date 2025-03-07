using Application.Common.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;
using SharedKernel.Email;
using SharedKernel.Exceptions;

namespace Application.Commands.Auth;

public class ForgotPasswordCommand : IRequest
{
    public string Email { get; set; }
}

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IJwtProvider _tokenGenerator;

    public ForgotPasswordHandler(IUserRepository userRepository, IEmailService emailService, IJwtProvider tokenGenerator)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _tokenGenerator = tokenGenerator;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email)
            ?? throw new NotFoundException("User not found.");

        var resetToken = _tokenGenerator.GenerateResetToken(user.Id);
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken.Token);

    }
}
