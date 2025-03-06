using Domain.Entities;

namespace Application.DTOs;

public class UserResponse
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public List<Role> Roles { get; private set; } = [];
    public string FullName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Address { get; private set; }
    public string ProfilePictureUrl { get; private set; }
} 
