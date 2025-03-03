using Domain.Events.User;
using SharedKernel.Common;

namespace Domain.Entities;

public class User : AggregateRoot<Guid>
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }
    public List<Role> Roles { get; private set; } = new();


    public User(Guid id, string username, string email, string passwordHash)
        : base(id)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;

        AddDomainEvent(new UserRegisteredEvent(Id, Username, Email));
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        AddDomainEvent(new UserPasswordChangedEvent(Id));
    }

    public void Deactivate()
    {
        IsActive = false;
        AddDomainEvent(new UserDeactivatedEvent(Id));
    }

    public void AssignRole(Role role)
    {
        Roles.Add(role);
        AddDomainEvent(new UserRoleAssignedEvent(Id, role.Name));
    }
}
