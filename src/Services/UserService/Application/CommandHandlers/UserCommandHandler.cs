using Application.Commands.User;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using MediatR;
using SharedKernel.Exceptions;

namespace Application.Handlers;
public class UserCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository, IPasswordHasher passwordHasher, IUnitOfWork unitOfWork) :
    IRequestHandler<RegisterUserCommand, Guid>,
    IRequestHandler<ChangePasswordCommand, bool>,
    IRequestHandler<AssignRoleCommand, bool>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        string hashedPassword = _passwordHasher.HashPassword(request.Password);

        if (await _userRepository.ExistsByUsernameAsync(request.Username))
            throw new UsernameAlreadyExistsException(request.Username);

        if (await _userRepository.GetByEmailAsync(request.Email) != null)
            throw new EmailAlreadyExistsException(request.Email);

        if (!PasswordIsStrong(request.Password))
            throw new WeakPasswordException();

        var user = User.RegisterUser(
            id: Guid.NewGuid(),
            username: request.Username,
            email: request.Email,
            passwordHash: hashedPassword,
            fullName: request.FullName,
            phoneNumber: request.PhoneNumber
        );

        await _userRepository.AddAsync(user);
        await _unitOfWork.CommitAsync();

        return user.Id;
    }

    private bool PasswordIsStrong(string password) =>
        password.Length >= 8;


    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) 
            ?? throw new NotFoundException($"User id {request.UserId} does not exist !");

        user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<bool> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId) ?? throw new Exception("User not found");

        var roles = await _roleRepository.GetByIdsAsync(request.RoleIds);
        if (roles.Count != request.RoleIds.Count) throw new Exception("Some roles not found");

        user.SetRoles(roles); // Xóa hết roles cũ và gán roles mới
        await _unitOfWork.CommitAsync();
        return true;
    }
}
