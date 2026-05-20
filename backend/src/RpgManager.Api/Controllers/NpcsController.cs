using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgManager.Application.Common;
using RpgManager.Application.Npcs;
using RpgManager.Domain.Enums;

namespace RpgManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/campaigns/{campaignId:guid}/npcs")]
public sealed class NpcsController(INpcService npcService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NpcResponse>>> Get(
        Guid campaignId,
        [FromQuery] string? search,
        [FromQuery] string? tag,
        [FromQuery] string? location,
        [FromQuery] string? faction,
        [FromQuery] bool? isImportant,
        [FromQuery] bool? isAlive,
        [FromQuery] Visibility? visibility,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var query = new NpcQuery(search, tag, location, faction, isImportant, isAlive, visibility);
        var result = await npcService.GetAsync(userId, campaignId, query, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{npcId:guid}")]
    public async Task<ActionResult<NpcResponse>> GetById(
        Guid campaignId,
        Guid npcId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await npcService.GetByIdAsync(userId, campaignId, npcId, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<NpcResponse>> Create(
        Guid campaignId,
        NpcRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await npcService.CreateAsync(userId, campaignId, request, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { campaignId, npcId = result.Data!.Id }, result.Data)
            : ToActionResult(result);
    }

    [HttpPut("{npcId:guid}")]
    public async Task<ActionResult<NpcResponse>> Update(
        Guid campaignId,
        Guid npcId,
        NpcRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await npcService.UpdateAsync(userId, campaignId, npcId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{npcId:guid}")]
    public async Task<IActionResult> Delete(
        Guid campaignId,
        Guid npcId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await npcService.DeleteAsync(userId, campaignId, npcId, cancellationToken);
        return result.Succeeded ? NoContent() : ToActionResult(result);
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
