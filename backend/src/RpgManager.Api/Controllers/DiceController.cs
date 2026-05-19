using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgManager.Application.Common;
using RpgManager.Application.Dice;

namespace RpgManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class DiceController(IDiceService diceService) : ControllerBase
{
    [HttpPost("roll")]
    public ActionResult<DiceRollResponse> Roll(DiceRollRequest request)
    {
        var result = diceService.Roll(request);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
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
