using Application.Commands;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Handlers;
public class UserCommandHandler(IUserRepository userRepository, IRoleRepository roleRepository, IPasswordHasher passwordHasher) :
    IRequestHandler<RegisterUserCommand, Guid>,
    IRequestHandler<ChangePasswordCommand, bool>,
    IRequestHandler<AssignRoleCommand, bool>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        var user = new User(Guid.NewGuid(), request.Username, request.Email, hashedPassword);
        await _userRepository.AddAsync(user);
        return user.Id;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {

        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return false;

        user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
        return true;
    }

    public async Task<bool> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null) return false;

        var roles = await _roleRepository.GetByNamesAsync(request.RoleNames);
        if (roles.Count != request.RoleNames.Count) return false; // Kiểm tra có role nào không tồn tại

        foreach (var role in roles)
        {
            user.AssignRole(role);
        }

        return true;
    }
}
