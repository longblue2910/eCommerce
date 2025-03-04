using SharedKernel.Common;

namespace Domain.Entities;

public class Role(Guid id, string name) : Entity<Guid>(id)
{
    public string Name { get; private set; } = name;
}
