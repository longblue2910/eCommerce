using Application.Commands.User;
using Application.Queries.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Đăng ký user
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = userId }, new { Id = userId });
    }

    /// <summary>
    /// Get by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        return user is not null ? Ok(user) : NotFound();
    }

    /// <summary>
    /// Xóa role khỏi user
    /// </summary>
    [HttpDelete("{roleId}")]
    public async Task<IActionResult> RemoveRole(Guid userId, Guid roleId)
    {
        var result = await _mediator.Send(new RemoveRoleFromUserCommand(userId, roleId));
        return result ? Ok() : BadRequest();
    }

    /// <summary>
    /// Lấy danh sách roles của user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetUserRoles(Guid userId)
    {
        var roles = await _mediator.Send(new GetUserRolesQuery(userId));
        return Ok(roles);
    }

    /// <summary>
    /// Lấy danh sách quyền của user
    /// </summary>
    [HttpGet("{userId}/permissions")]
    public async Task<IActionResult> GetUserPermissions(Guid userId)
    {
        var permissions = await _mediator.Send(new GetUserPermissionsQuery(userId));
        return Ok(permissions);
    }

    /// <summary>
    /// Kiểm tra user có quyền cụ thể không
    /// </summary>
    [HttpPost("{userId}/check-permission")]
    public async Task<IActionResult> CheckUserPermission(Guid userId, [FromBody] string permission)
    {
        var hasPermission = await _mediator.Send(new CheckUserPermissionCommand(userId, permission));
        return Ok(new { UserId = userId, Permission = permission, HasPermission = hasPermission });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers(
    [FromQuery] string username,
    [FromQuery] string email,
    [FromQuery] string fullName,
    [FromQuery] string phoneNumber,
    [FromQuery] bool? isActive,
    [FromQuery] DateTime? createdFrom,
    [FromQuery] DateTime? createdTo,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new SearchUsersQuery(
            username, email, fullName, phoneNumber, isActive, createdFrom, createdTo, page, pageSize
        ));

        return Ok(result);
    }

}