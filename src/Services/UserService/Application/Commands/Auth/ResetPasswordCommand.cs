using Application.Common.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;
using SharedKernel.Exceptions;

namespace Application.Commands.Auth;

public class ResetPasswordCommand : IRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class ResetPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IUnitOfWork unitOfWork) : IRequestHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // 🔹 Giải mã token để lấy userId
        var userId = _jwtProvider.ValidateResetToken(request.Token)
                    ?? throw new BadRequestException("Token không hợp lệ hoặc đã hết hạn.");

        // 🔹 Tìm user trong database
        var user = await _userRepository.GetByIdAsync(userId) 
            ?? throw new NotFoundException("User not found.");

        // 🔹 Hash mật khẩu mới
        user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));

        await _unitOfWork.CommitAsync();

    }
}
