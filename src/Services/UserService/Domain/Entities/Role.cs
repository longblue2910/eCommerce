using SharedKernel.Common;

namespace Domain.Entities;

public class Role(Guid id, string name) : Entity<Guid>(id)
{
    public string Name { get; private set; } = name;
    public List<Permission> Permissions { get; private set; } = [];

    public void AddPermission(Permission permission)
    {
        if (!Permissions.Any(p => p.Name == permission.Name))
        {
            Permissions.Add(permission);
        }
    }

    public void RemovePermission(Guid permissionId)
    {
        var permission = Permissions.FirstOrDefault(p => p.Id == permissionId);
        if (permission != null)
        {
            Permissions.Remove(permission);
        }
    }

    public bool HasPermission(string permissionName) => Permissions.Any(p => p.Name == permissionName);
}
