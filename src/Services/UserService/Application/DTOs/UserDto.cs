namespace Application.DTOs;

public record UserDto(Guid Id, string Username, string Email, bool IsActive, List<string> Roles);
