using System.Linq.Expressions;

namespace SharedKernel.Common;

public abstract class Specification<T>
{
    public Expression<Func<T, bool>> Criteria { get; protected set; } = _ => true;

    public Specification() { }

    public Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }
}
