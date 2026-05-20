using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgManager.Application.CharacterOptions;
using RpgManager.Application.Common;

namespace RpgManager.Api.Controllers;

[Authorize]
[ApiController]
public sealed class CharacterOptionsController(ICharacterOptionService characterOptionService) : ControllerBase
{
    [HttpGet("api/races")]
    public async Task<ActionResult<IReadOnlyList<RaceResponse>>> GetRaces(CancellationToken cancellationToken)
        => Ok(await characterOptionService.GetRacesAsync(cancellationToken));

    [HttpPost("api/races")]
    public async Task<ActionResult<RaceResponse>> CreateRace(
        RaceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await characterOptionService.CreateRaceAsync(GetCurrentUserId(), request, cancellationToken);
        return result.Succeeded ? CreatedAtAction(nameof(GetRaces), result.Data) : ToActionResult(result);
    }

    [HttpGet("api/classes")]
    public async Task<ActionResult<IReadOnlyList<CharacterClassResponse>>> GetClasses(CancellationToken cancellationToken)
        => Ok(await characterOptionService.GetClassesAsync(cancellationToken));

    [HttpPost("api/classes")]
    public async Task<ActionResult<CharacterClassResponse>> CreateClass(
        CharacterClassRequest request,
        CancellationToken cancellationToken)
    {
        var result = await characterOptionService.CreateClassAsync(GetCurrentUserId(), request, cancellationToken);
        return result.Succeeded ? CreatedAtAction(nameof(GetClasses), result.Data) : ToActionResult(result);
    }

    [HttpGet("api/backgrounds")]
    public async Task<ActionResult<IReadOnlyList<BackgroundResponse>>> GetBackgrounds(CancellationToken cancellationToken)
        => Ok(await characterOptionService.GetBackgroundsAsync(cancellationToken));

    [HttpPost("api/backgrounds")]
    public async Task<ActionResult<BackgroundResponse>> CreateBackground(
        BackgroundRequest request,
        CancellationToken cancellationToken)
    {
        var result = await characterOptionService.CreateBackgroundAsync(GetCurrentUserId(), request, cancellationToken);
        return result.Succeeded ? CreatedAtAction(nameof(GetBackgrounds), result.Data) : ToActionResult(result);
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
