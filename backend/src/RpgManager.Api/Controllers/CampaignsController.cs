using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgManager.Application.Campaigns;
using RpgManager.Application.Common;

namespace RpgManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CampaignsController(ICampaignService campaignService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CampaignSummaryResponse>>> GetMine(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var campaigns = await campaignService.GetMyCampaignsAsync(userId, cancellationToken);
        return Ok(campaigns);
    }

    [HttpPost]
    public async Task<ActionResult<CampaignResponse>> Create(
        CampaignRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignService.CreateAsync(userId, request, cancellationToken);

        if (!result.Succeeded)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CampaignResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignService.GetByIdAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CampaignResponse>> Update(
        Guid id,
        CampaignRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignService.UpdateAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignService.DeleteAsync(userId, id, cancellationToken);
        return result.Succeeded ? NoContent() : ToActionResult(result);
    }

    [HttpPost("join")]
    public async Task<ActionResult<CampaignResponse>> Join(
        JoinCampaignRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignService.JoinAsync(userId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPost("{id:guid}/invite/regenerate")]
    public async Task<ActionResult<CampaignResponse>> RegenerateInvite(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignService.RegenerateInviteAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<IReadOnlyList<CampaignMemberResponse>>> GetMembers(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignService.GetMembersAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/characters")]
    public async Task<ActionResult<IReadOnlyList<CampaignCharacterSummaryResponse>>> GetCharacters(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignService.GetCharactersAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/master-dashboard")]
    public async Task<ActionResult<CampaignMasterDashboardResponse>> GetMasterDashboard(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await campaignService.GetMasterDashboardAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
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
