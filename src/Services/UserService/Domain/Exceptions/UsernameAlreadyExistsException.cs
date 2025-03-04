namespace Domain.Exceptions;

public class UsernameAlreadyExistsException(string username) : Exception($"The username '{username}' is already taken.")
{
}