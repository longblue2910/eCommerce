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


    public User(Guid id, string username, string email, string passwordHash)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.");

        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;

        if (!EmailIsValid(email))
            throw new InvalidEmailException(email);

        AddDomainEvent(new UserRegisteredEvent(Id, Username, Email));
    }



    private bool EmailIsValid(string email) =>
        email.Contains("@") && email.Contains(".");


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
