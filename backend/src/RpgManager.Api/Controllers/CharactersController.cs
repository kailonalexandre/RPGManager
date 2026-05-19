using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RpgManager.Application.Characters;
using RpgManager.Application.Common;
using RpgManager.Domain.Enums;

namespace RpgManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class CharactersController(ICharacterService characterService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CharacterSummaryResponse>>> GetVisible(
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var characters = await characterService.GetVisibleAsync(userId, cancellationToken);
        return Ok(characters);
    }

    [HttpPost]
    public async Task<ActionResult<CharacterResponse>> Create(
        CharacterRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.CreateAsync(userId, request, cancellationToken);

        if (!result.Succeeded)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CharacterResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetByIdAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CharacterResponse>> Update(
        Guid id,
        CharacterRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.DeleteAsync(userId, id, cancellationToken);
        return result.Succeeded ? NoContent() : ToActionResult(result);
    }

    [HttpGet("{id:guid}/attributes")]
    public async Task<ActionResult<IReadOnlyList<AbilityScoreResponse>>> GetAttributes(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetAttributesAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}/attributes")]
    public async Task<ActionResult<IReadOnlyList<AbilityScoreResponse>>> UpdateAttributes(
        Guid id,
        AbilityScoreRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateAttributesAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/saving-throws")]
    public async Task<ActionResult<IReadOnlyList<SavingThrowResponse>>> GetSavingThrows(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetSavingThrowsAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}/saving-throws")]
    public async Task<ActionResult<IReadOnlyList<SavingThrowResponse>>> UpdateSavingThrows(
        Guid id,
        IReadOnlyList<SavingThrowRequest> request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateSavingThrowsAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/skills")]
    public async Task<ActionResult<IReadOnlyList<CharacterSkillResponse>>> GetSkills(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetSkillsAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}/skills")]
    public async Task<ActionResult<IReadOnlyList<CharacterSkillResponse>>> UpdateSkills(
        Guid id,
        IReadOnlyList<CharacterSkillRequest> request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateSkillsAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/combat")]
    public async Task<ActionResult<CharacterCombatResponse>> GetCombat(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetCombatAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}/combat")]
    public async Task<ActionResult<CharacterCombatResponse>> UpdateCombat(
        Guid id,
        CharacterCombatRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateCombatAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/attacks")]
    public async Task<ActionResult<IReadOnlyList<CharacterAttackResponse>>> GetAttacks(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetAttacksAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPost("{id:guid}/attacks")]
    public async Task<ActionResult<CharacterAttackResponse>> CreateAttack(
        Guid id,
        CharacterAttackRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.CreateAttackAsync(userId, id, request, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetAttacks), new { id }, result.Data)
            : ToActionResult(result);
    }

    [HttpPut("{id:guid}/attacks/{attackId:guid}")]
    public async Task<ActionResult<CharacterAttackResponse>> UpdateAttack(
        Guid id,
        Guid attackId,
        CharacterAttackRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateAttackAsync(userId, id, attackId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{id:guid}/attacks/{attackId:guid}")]
    public async Task<IActionResult> DeleteAttack(Guid id, Guid attackId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.DeleteAttackAsync(userId, id, attackId, cancellationToken);
        return result.Succeeded ? NoContent() : ToActionResult(result);
    }

    [HttpGet("{id:guid}/conditions")]
    public async Task<ActionResult<IReadOnlyList<CharacterConditionResponse>>> GetConditions(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetConditionsAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}/conditions")]
    public async Task<ActionResult<IReadOnlyList<CharacterConditionResponse>>> UpdateConditions(
        Guid id,
        IReadOnlyList<CharacterConditionRequest> request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateConditionsAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/notes")]
    public async Task<ActionResult<IReadOnlyList<CharacterNoteResponse>>> GetNotes(
        Guid id,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetNotesAsync(userId, id, search, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPost("{id:guid}/notes")]
    public async Task<ActionResult<CharacterNoteResponse>> CreateNote(
        Guid id,
        CharacterNoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.CreateNoteAsync(userId, id, request, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetNoteById), new { id, noteId = result.Data!.Id }, result.Data)
            : ToActionResult(result);
    }

    [HttpGet("{id:guid}/notes/{noteId:guid}")]
    public async Task<ActionResult<CharacterNoteResponse>> GetNoteById(
        Guid id,
        Guid noteId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetNoteByIdAsync(userId, id, noteId, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}/notes/{noteId:guid}")]
    public async Task<ActionResult<CharacterNoteResponse>> UpdateNote(
        Guid id,
        Guid noteId,
        CharacterNoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateNoteAsync(userId, id, noteId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{id:guid}/notes/{noteId:guid}")]
    public async Task<IActionResult> DeleteNote(Guid id, Guid noteId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.DeleteNoteAsync(userId, id, noteId, cancellationToken);
        return result.Succeeded ? NoContent() : ToActionResult(result);
    }

    [HttpGet("{id:guid}/inventory")]
    public async Task<ActionResult<IReadOnlyList<CharacterInventoryItemResponse>>> GetInventory(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetInventoryAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPost("{id:guid}/inventory")]
    public async Task<ActionResult<CharacterInventoryItemResponse>> CreateInventoryItem(
        Guid id,
        CharacterInventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.CreateInventoryItemAsync(userId, id, request, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetInventory), new { id }, result.Data)
            : ToActionResult(result);
    }

    [HttpPut("{id:guid}/inventory/{itemId:guid}")]
    public async Task<ActionResult<CharacterInventoryItemResponse>> UpdateInventoryItem(
        Guid id,
        Guid itemId,
        CharacterInventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateInventoryItemAsync(userId, id, itemId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{id:guid}/inventory/{itemId:guid}")]
    public async Task<IActionResult> DeleteInventoryItem(Guid id, Guid itemId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.DeleteInventoryItemAsync(userId, id, itemId, cancellationToken);
        return result.Succeeded ? NoContent() : ToActionResult(result);
    }

    [HttpGet("{id:guid}/currency")]
    public async Task<ActionResult<CharacterCurrencyResponse>> GetCurrency(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetCurrencyAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}/currency")]
    public async Task<ActionResult<CharacterCurrencyResponse>> UpdateCurrency(
        Guid id,
        CharacterCurrencyRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateCurrencyAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/spells")]
    public async Task<ActionResult<IReadOnlyList<CharacterSpellResponse>>> GetSpells(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetSpellsAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPost("{id:guid}/spells")]
    public async Task<ActionResult<CharacterSpellResponse>> AddSpell(
        Guid id,
        CharacterSpellRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.AddSpellAsync(userId, id, request, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetSpells), new { id }, result.Data)
            : ToActionResult(result);
    }

    [HttpPut("{id:guid}/spells/{characterSpellId:guid}")]
    public async Task<ActionResult<CharacterSpellResponse>> UpdateSpell(
        Guid id,
        Guid characterSpellId,
        CharacterSpellUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateSpellAsync(userId, id, characterSpellId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{id:guid}/spells/{characterSpellId:guid}")]
    public async Task<IActionResult> DeleteSpell(Guid id, Guid characterSpellId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.DeleteSpellAsync(userId, id, characterSpellId, cancellationToken);
        return result.Succeeded ? NoContent() : ToActionResult(result);
    }

    [HttpGet("{id:guid}/spell-slots")]
    public async Task<ActionResult<IReadOnlyList<CharacterSpellSlotResponse>>> GetSpellSlots(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetSpellSlotsAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}/spell-slots")]
    public async Task<ActionResult<IReadOnlyList<CharacterSpellSlotResponse>>> UpdateSpellSlots(
        Guid id,
        IReadOnlyList<CharacterSpellSlotRequest> request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateSpellSlotsAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/features")]
    public async Task<ActionResult<IReadOnlyList<CharacterFeatureResponse>>> GetFeatures(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetFeaturesAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPost("{id:guid}/features")]
    public async Task<ActionResult<CharacterFeatureResponse>> AddFeature(
        Guid id,
        CharacterFeatureRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.AddFeatureAsync(userId, id, request, cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetFeatures), new { id }, result.Data)
            : ToActionResult(result);
    }

    [HttpPut("{id:guid}/features/{characterFeatureId:guid}")]
    public async Task<ActionResult<CharacterFeatureResponse>> UpdateFeature(
        Guid id,
        Guid characterFeatureId,
        CharacterFeatureRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.UpdateFeatureAsync(userId, id, characterFeatureId, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpDelete("{id:guid}/features/{characterFeatureId:guid}")]
    public async Task<IActionResult> DeleteFeature(Guid id, Guid characterFeatureId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.DeleteFeatureAsync(userId, id, characterFeatureId, cancellationToken);
        return result.Succeeded ? NoContent() : ToActionResult(result);
    }

    [HttpPost("{id:guid}/short-rest")]
    public async Task<ActionResult<CharacterRestResponse>> ShortRest(
        Guid id,
        CharacterRestRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.ShortRestAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPost("{id:guid}/long-rest")]
    public async Task<ActionResult<CharacterRestResponse>> LongRest(
        Guid id,
        CharacterRestRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.LongRestAsync(userId, id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpGet("{id:guid}/assets")]
    public async Task<ActionResult<IReadOnlyList<CharacterAssetResponse>>> GetAssets(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.GetAssetsAsync(userId, id, cancellationToken);
        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPost("{id:guid}/assets")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<CharacterAssetResponse>> UploadAsset(
        Guid id,
        [FromForm] IFormFile file,
        [FromForm] AssetType assetType,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest(new { message = "Arquivo é obrigatório." });
        }

        var userId = GetCurrentUserId();
        await using var stream = file.OpenReadStream();
        var result = await characterService.UploadAssetAsync(
            userId,
            id,
            stream,
            file.FileName,
            file.ContentType,
            assetType,
            cancellationToken);

        return result.Succeeded
            ? CreatedAtAction(nameof(GetAssets), new { id }, result.Data)
            : ToActionResult(result);
    }

    [HttpDelete("{id:guid}/assets/{assetId:guid}")]
    public async Task<IActionResult> DeleteAsset(Guid id, Guid assetId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await characterService.DeleteAssetAsync(userId, id, assetId, cancellationToken);
        return result.Succeeded ? NoContent() : ToActionResult(result);
    }

    [HttpPut("{id:guid}/avatar")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<CharacterResponse>> UploadAvatar(
        Guid id,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest(new { message = "Arquivo é obrigatório." });
        }

        var userId = GetCurrentUserId();
        await using var stream = file.OpenReadStream();
        var result = await characterService.UploadAvatarAsync(
            userId,
            id,
            stream,
            file.FileName,
            file.ContentType,
            cancellationToken);

        return result.Succeeded ? Ok(result.Data) : ToActionResult(result);
    }

    [HttpPut("{id:guid}/token")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<CharacterResponse>> UploadToken(
        Guid id,
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
        {
            return BadRequest(new { message = "Arquivo é obrigatório." });
        }

        var userId = GetCurrentUserId();
        await using var stream = file.OpenReadStream();
        var result = await characterService.UploadTokenAsync(
            userId,
            id,
            stream,
            file.FileName,
            file.ContentType,
            cancellationToken);

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
