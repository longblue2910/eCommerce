using SharedKernel.Exceptions;

namespace Domain.Exceptions;

public class UsernameAlreadyExistsException(string username) : BadRequestException($"The username '{username}' is already taken.")
{
}