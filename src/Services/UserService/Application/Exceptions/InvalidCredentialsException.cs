using SharedKernel.Exceptions;

namespace Application.Exceptions;


public class InvalidCredentialsException : UnauthorizedException
{
    public InvalidCredentialsException()
        : base("Invalid username or password.")
    {
    }
}
