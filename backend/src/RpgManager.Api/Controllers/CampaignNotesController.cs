using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgManager.Application.CampaignNotes;
using RpgManager.Application.Common;
using RpgManager.Domain.Enums;

namespace RpgManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/campaigns/{campaignId:guid}/notes")]
public sealed class CampaignNotesController(ICampaignNoteService campaignNoteService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CampaignNoteResponse>>> Get(
        Guid campaignId,
        [FromQuery] string? search,
        [FromQuery] string? tag,
        [FromQuery] Visibility? visibility,
        [FromQuery] string? linkedEntityType,
        [FromQuery] Guid? linkedEntityId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var query = new CampaignNoteQuery(search, tag, visibility, linkedEntityType, linkedEntityId);
        var result = await campaignNoteService.GetAsync(userId, campaignId, query, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{noteId:guid}")]
    public async Task<ActionResult<CampaignNoteResponse>> GetById(
        Guid campaignId,
        Guid noteId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignNoteService.GetByIdAsync(userId, campaignId, noteId, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CampaignNoteResponse>> Create(
        Guid campaignId,
        CampaignNoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignNoteService.CreateAsync(userId, campaignId, request, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { campaignId, noteId = result.Data!.Id }, result.Data)
            : ToActionResult(result);
    }

    [HttpPut("{noteId:guid}")]
    public async Task<ActionResult<CampaignNoteResponse>> Update(
        Guid campaignId,
        Guid noteId,
        CampaignNoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignNoteService.UpdateAsync(userId, campaignId, noteId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{noteId:guid}")]
    public async Task<IActionResult> Delete(
        Guid campaignId,
        Guid noteId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignNoteService.DeleteAsync(userId, campaignId, noteId, cancellationToken);
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
