using SharedKernel.Exceptions;

namespace Domain.Exceptions;

public class InvalidEmailException(string email) : BadRequestException($"The email '{email}' is not valid.")
{
}
