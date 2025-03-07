using SharedKernel.Common;

namespace Domain.Entities;

public class Permission(Guid id, string name, string description) : Entity<Guid>(id)
{
    public string Name { get; private set; } = name;
    public string Description { get; private set; } = description;
}
