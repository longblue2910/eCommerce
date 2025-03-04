namespace SharedKernel.Exceptions;

// 400 Bad Request
public class BadRequestException(string message) : BaseException(message, 400);

// 401 Unauthorized
public class UnauthorizedException(string message) : BaseException(message, 401);

// 403 Forbidden
public class ForbiddenException(string message) : BaseException(message, 403);

// 404 Not Found
public class NotFoundException(string message) : BaseException(message, 404);
