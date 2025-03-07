using Domain.Events.User;
using Domain.Exceptions;
using SharedKernel.Common;

namespace Domain.Entities;

public class User : AggregateRoot<Guid>
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }
    public List<Role> Roles { get; private set; } = [];
    public string FullName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Address { get; private set; }
    public string ProfilePictureUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // 🔥 Thêm danh sách RefreshToken
    public List<RefreshToken> RefreshTokens { get; private set; } = [];

    public User(Guid id, string username, string email, string passwordHash, string fullName, string phoneNumber)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.");

        if (!EmailIsValid(email))
            throw new InvalidEmailException(email);

        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = string.Empty;
        ProfilePictureUrl = string.Empty;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public static User RegisterUser(Guid id, string username, string email, string passwordHash, string fullName, string phoneNumber)
    {
        var user = new User(id, username, email, passwordHash, fullName, phoneNumber);
        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Username, user.Email));
        return user;
    }

    public void UpdateProfile(string fullName, string phoneNumber, string address, string profilePictureUrl)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = address;
        ProfilePictureUrl = profilePictureUrl;
    }

    public void Login()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        AddDomainEvent(new UserPasswordChangedEvent(Id));
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    public void AssignRole(Role role)
    {
        if (!Roles.Any(r => r.Name == role.Name))
        {
            Roles.Add(role);
            AddDomainEvent(new UserRoleAssignedEvent(Id, role.Name));
        }
    }

    public void RemoveRole(string roleName)
    {
        var role = Roles.FirstOrDefault(r => r.Name == roleName);
        if (role != null)
        {
            Roles.Remove(role);
        }
    }

    public void SetRoles(List<Role> roles)
    {
        Roles.Clear(); // Xóa hết roles cũ
        Roles.AddRange(roles); // Gán roles mới
    }

    public bool HasRole(string roleName) => Roles.Any(r => r.Name == roleName);

    // 🔥 Thêm phương thức quản lý Refresh Token
    public void AddRefreshToken(string token, DateTime expiryDate, string ipAddress)
    {
        var refreshToken = new RefreshToken(Guid.NewGuid(), Id, token, expiryDate, ipAddress);
        RefreshTokens.Add(refreshToken);
    }


    private bool EmailIsValid(string email) => email.Contains("@") && email.Contains(".");

    public bool HasPermission(string permissionName) =>
        Roles.Any(role => role.HasPermission(permissionName));

}
