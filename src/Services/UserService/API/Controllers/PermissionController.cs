using Application.Permissions.Commands;
using Application.Queries.Permissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/permissions")]
[ApiController]
[Authorization("Admin")]
public class PermissionController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Lấy danh sách quyền
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await _mediator.Send(new GetPermissionsQuery());
        return Ok(permissions);
    }

    /// <summary>
    /// Thêm quyền mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionCommand command)
    {
        var permissionId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPermissions), new { id = permissionId }, permissionId);
    }
}
