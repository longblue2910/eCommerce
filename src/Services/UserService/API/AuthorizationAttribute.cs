using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizationAttribute(params string[] roles) : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles = roles;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Kiểm tra nếu action có [AllowAnonymous] thì bỏ qua middleware
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .FirstOrDefault();

        if (allowAnonymous != null) return;

        var authHeader = context.HttpContext.Request.Headers["Authorization"];
        Console.WriteLine($"Authorization Header: {authHeader}");

        var user = context.HttpContext.User;

        // 🛑 Kiểm tra user có đăng nhập không
        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new JsonResult(new { message = "Unauthorized." })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return;
        }

        // ✅ Kiểm tra quyền truy cập theo role
        if (_roles.Length > 0)
        {
            var userRoles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role) // Kiểm tra cả hai kiểu claim
                .Select(c => c.Value)
                .Distinct()
                .ToList();

            Console.WriteLine("User Roles: " + string.Join(", ", userRoles));

            if (!userRoles.Any(role => _roles.Contains(role)))
            {
                context.Result = new JsonResult(new { message = "Forbidden." })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }
        }
    }
}