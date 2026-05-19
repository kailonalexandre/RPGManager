using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgManager.Application.Common;
using RpgManager.Application.Features;
using RpgManager.Application.Spells;

namespace RpgManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class FeaturesController(IFeatureService featureService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<FeatureResponse>>> GetVisible(
        [FromQuery] FeatureFilters filters,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var features = await featureService.GetVisibleAsync(userId, filters, cancellationToken);
        return Ok(features);
    }

    [HttpPost]
    public async Task<ActionResult<FeatureResponse>> Create(FeatureRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await featureService.CreateAsync(userId, request, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : ToActionResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FeatureResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await featureService.GetByIdAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FeatureResponse>> Update(
        Guid id,
        FeatureRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await featureService.UpdateAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await featureService.DeleteAsync(userId, id, cancellationToken);
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
