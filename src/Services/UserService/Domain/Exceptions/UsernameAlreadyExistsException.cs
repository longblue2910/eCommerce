using SharedKernel.Exceptions;

namespace Domain.Exceptions;

public class UsernameAlreadyExistsException(string username) : BadRequestException($"The username '{username}' is already taken.")
{
}

public class EmailAlreadyExistsException(string email) : BadRequestException($"The email '{email}' is already taken.")
{
}