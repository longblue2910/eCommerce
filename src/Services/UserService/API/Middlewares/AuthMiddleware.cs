namespace API.Middlewares;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Claims;

public class AuthMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        // Lấy endpoint hiện tại
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        if (endpoint == null)
        {
            await _next(context);
            return;
        }

        Console.WriteLine($"Token Received: {context.Request.Headers["Authorization"]}");

        // Kiểm tra có attribute [Authorize] không
        var authorizeAttribute = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
        if (authorizeAttribute != null)
        {
            var user = context.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            // Kiểm tra Role (nếu có)
            var requiredRoles = authorizeAttribute.Roles?.Split(',');
            if (requiredRoles != null && requiredRoles.Length > 0)
            {
                var userRoles = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                if (!userRoles.Any(role => requiredRoles.Contains(role)))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden");
                    return;
                }
            }
        }

        await _next(context);
    }
}