using ForumBackend.Contracts.Role;
using ForumBackend.Filters.Role;
using ForumBackend.Services.Role;
using Microsoft.AspNetCore.Mvc;

namespace ForumBackend.Controllers;

[Controller]
[Route("api/[controller]")]
public class RoleController(IRoleService roleService, ILogger<RoleController> logger) : ControllerBase
{
    [RoleExceptionFilter]
    [HttpGet("{name}")]
    public async Task<ActionResult<RoleResponseContract>> GetByUid([FromRoute] string name,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting role with name: {name}.", name);

        var role = await roleService.GetByNameAsync(name, cancellationToken);

        if (role is null)
        {
            logger.LogWarning("role with name {name} was not found.", name);
            return NotFound();
        }

        logger.LogInformation("role received with name: {name}.", name);

        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleResponseContract>> Create(
        [FromBody] CreateRoleRequestContract request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating role with name {name}.", request.Name);

        var createdRole = await roleService.CreateAsync(request, cancellationToken);

        logger.LogInformation("Created role with name: {name}", createdRole.Name);

        return CreatedAtAction(nameof(GetByUid), new { name = createdRole.Name }, createdRole);
    }
    

    [RoleExceptionFilter]
    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete([FromRoute] string name, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting role with name: {name}.", name);

        var deleted = await roleService.DeleteAsync(name, cancellationToken);

        if (!deleted)
        {
            logger.LogWarning("role with name {name} was not found for deletion.", name);
            return NotFound();
        }

        logger.LogInformation("role with name: {name} deleted.", name);

        return NoContent();
    }
}