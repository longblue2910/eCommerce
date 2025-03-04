using SharedKernel.Exceptions;

namespace Domain.Exceptions;

public class WeakPasswordException : BadRequestException
{
    public WeakPasswordException()
        : base("The provided password is too weak.") { }
}
