namespace Domain.Exceptions;

public class InvalidEmailException(string email) : Exception($"The email '{email}' is not valid.")
{
}
