using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgManager.Application.Common;
using RpgManager.Application.Spells;

namespace RpgManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class SpellsController(ISpellService spellService, ISpellImportService spellImportService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<SpellResponse>>> GetVisible(
        [FromQuery] SpellFilters filters,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var spells = await spellService.GetVisibleAsync(userId, filters, cancellationToken);
        return Ok(spells);
    }

    [HttpPost]
    public async Task<ActionResult<SpellResponse>> Create(SpellRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await spellService.CreateAsync(userId, request, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SpellResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await spellService.GetByIdAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SpellResponse>> Update(
        Guid id,
        SpellRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await spellService.UpdateAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await spellService.DeleteAsync(userId, id, cancellationToken);
        return result.Succeeded ? NoContent() : ToActionResult(result);
    }

    [Authorize(Roles = "GameMaster")]
    [HttpPost("import/open5e")]
    public async Task<ActionResult<SpellImportResponse>> ImportOpen5e(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await spellImportService.ImportOpen5eAsync(userId, cancellationToken);
        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdValue, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("Token inválido.");
    }

    private ActionResult ToActionResult<T>(ServiceResult<T> result)
    {
        var error = new { message = result.Error };

        return result.ErrorType switch
        {
            ServiceErrorType.NotFound => NotFound(error),
            ServiceErrorType.Forbidden => Forbid(),
            ServiceErrorType.Conflict => Conflict(error),
            _ => BadRequest(error)
        };
    }
}
