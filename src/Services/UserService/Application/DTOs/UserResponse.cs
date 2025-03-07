namespace Application.Users.Dtos;

public class UserResponse
{
    public Guid Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string FullName { get; init; }
    public string PhoneNumber { get; init; }
    public string Address { get; init; }
    public string ProfilePictureUrl { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public List<string> Roles { get; init; }
}
