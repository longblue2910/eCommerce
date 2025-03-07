using Application.Commands.Role;
using Application.Queries.Role;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/roles")]
//[Authorization("Admin")]
public class RolesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
    {
        var roleId = await _mediator.Send(command);
        return Ok(roleId);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        await _mediator.Send(new DeleteRoleCommand(id));
        return NoContent();
    }

    [HttpPost("{roleId}/permissions")]
    public async Task<IActionResult> AssignPermission(Guid roleId, [FromBody] Guid permissionId)
    {
        await _mediator.Send(new AssignPermissionToRoleCommand(roleId, permissionId));
        return NoContent();
    }

    [HttpDelete("{roleId}/permissions/{permissionId}")]
    public async Task<IActionResult> RemovePermission(Guid roleId, Guid permissionId)
    {
        await _mediator.Send(new RemovePermissionFromRoleCommand(roleId, permissionId));
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles([FromQuery] string keyword, [FromQuery] bool? isActive,
                                              [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetRolesQuery(keyword, isActive, pageNumber, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
