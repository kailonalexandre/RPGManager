using RpgManager.Application.Common;
using RpgManager.Domain.Enums;

namespace RpgManager.Application.Characters;

public interface ICharacterService
{
    Task<IReadOnlyList<CharacterSummaryResponse>> GetVisibleAsync(Guid userId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterResponse>> GetByIdAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterResponse>> CreateAsync(Guid userId, CharacterRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterResponse>> UpdateAsync(Guid userId, Guid characterId, CharacterRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<AbilityScoreResponse>>> GetAttributesAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<AbilityScoreResponse>>> UpdateAttributesAsync(Guid userId, Guid characterId, AbilityScoreRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<SavingThrowResponse>>> GetSavingThrowsAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<SavingThrowResponse>>> UpdateSavingThrowsAsync(Guid userId, Guid characterId, IReadOnlyList<SavingThrowRequest> request, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterSkillResponse>>> GetSkillsAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterSkillResponse>>> UpdateSkillsAsync(Guid userId, Guid characterId, IReadOnlyList<CharacterSkillRequest> request, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterCombatResponse>> GetCombatAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterCombatResponse>> UpdateCombatAsync(Guid userId, Guid characterId, CharacterCombatRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterAttackResponse>>> GetAttacksAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterAttackResponse>> CreateAttackAsync(Guid userId, Guid characterId, CharacterAttackRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterAttackResponse>> UpdateAttackAsync(Guid userId, Guid characterId, Guid attackId, CharacterAttackRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteAttackAsync(Guid userId, Guid characterId, Guid attackId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterConditionResponse>>> GetConditionsAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterConditionResponse>>> UpdateConditionsAsync(Guid userId, Guid characterId, IReadOnlyList<CharacterConditionRequest> request, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterNoteResponse>>> GetNotesAsync(Guid userId, Guid characterId, string? search, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterNoteResponse>> GetNoteByIdAsync(Guid userId, Guid characterId, Guid noteId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterNoteResponse>> CreateNoteAsync(Guid userId, Guid characterId, CharacterNoteRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterNoteResponse>> UpdateNoteAsync(Guid userId, Guid characterId, Guid noteId, CharacterNoteRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteNoteAsync(Guid userId, Guid characterId, Guid noteId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterInventoryItemResponse>>> GetInventoryAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterInventoryItemResponse>> CreateInventoryItemAsync(Guid userId, Guid characterId, CharacterInventoryItemRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterInventoryItemResponse>> UpdateInventoryItemAsync(Guid userId, Guid characterId, Guid itemId, CharacterInventoryItemRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteInventoryItemAsync(Guid userId, Guid characterId, Guid itemId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterCurrencyResponse>> GetCurrencyAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterCurrencyResponse>> UpdateCurrencyAsync(Guid userId, Guid characterId, CharacterCurrencyRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterAssetResponse>>> GetAssetsAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterAssetResponse>> UploadAssetAsync(Guid userId, Guid characterId, Stream fileStream, string originalFileName, string contentType, AssetType assetType, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteAssetAsync(Guid userId, Guid characterId, Guid assetId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterResponse>> UploadAvatarAsync(Guid userId, Guid characterId, Stream fileStream, string originalFileName, string contentType, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterResponse>> UploadTokenAsync(Guid userId, Guid characterId, Stream fileStream, string originalFileName, string contentType, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterSpellResponse>>> GetSpellsAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterSpellResponse>> AddSpellAsync(Guid userId, Guid characterId, CharacterSpellRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterSpellResponse>> UpdateSpellAsync(Guid userId, Guid characterId, Guid characterSpellId, CharacterSpellUpdateRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteSpellAsync(Guid userId, Guid characterId, Guid characterSpellId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>> GetSpellSlotsAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>> UpdateSpellSlotsAsync(Guid userId, Guid characterId, IReadOnlyList<CharacterSpellSlotRequest> request, CancellationToken cancellationToken);
    Task<ServiceResult<IReadOnlyList<CharacterFeatureResponse>>> GetFeaturesAsync(Guid userId, Guid characterId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterFeatureResponse>> AddFeatureAsync(Guid userId, Guid characterId, CharacterFeatureRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterFeatureResponse>> UpdateFeatureAsync(Guid userId, Guid characterId, Guid characterFeatureId, CharacterFeatureRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<bool>> DeleteFeatureAsync(Guid userId, Guid characterId, Guid characterFeatureId, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterRestResponse>> ShortRestAsync(Guid userId, Guid characterId, CharacterRestRequest request, CancellationToken cancellationToken);
    Task<ServiceResult<CharacterRestResponse>> LongRestAsync(Guid userId, Guid characterId, CharacterRestRequest request, CancellationToken cancellationToken);
}
