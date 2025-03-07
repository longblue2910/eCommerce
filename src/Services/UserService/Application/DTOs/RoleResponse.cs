namespace Application.DTOs;

public class RoleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public List<string> Permissions { get; set; }
}
