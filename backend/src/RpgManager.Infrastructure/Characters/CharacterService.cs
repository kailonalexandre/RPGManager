using Microsoft.EntityFrameworkCore;
using RpgManager.Application.Characters;
using RpgManager.Application.Common;
using RpgManager.Application.Storage;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.Characters;

public sealed class CharacterService(AppDbContext dbContext, IFileStorageService fileStorageService) : ICharacterService
{
    private static readonly IReadOnlyList<SkillDefinition> SkillDefinitions =
    [
        new(SkillType.Acrobatics, "Acrobacia", AbilityType.Dexterity),
        new(SkillType.AnimalHandling, "Lidar com Animais", AbilityType.Wisdom),
        new(SkillType.Arcana, "Arcanismo", AbilityType.Intelligence),
        new(SkillType.Athletics, "Atletismo", AbilityType.Strength),
        new(SkillType.Deception, "Enganação", AbilityType.Charisma),
        new(SkillType.History, "História", AbilityType.Intelligence),
        new(SkillType.Insight, "Intuição", AbilityType.Wisdom),
        new(SkillType.Intimidation, "Intimidação", AbilityType.Charisma),
        new(SkillType.Investigation, "Investigação", AbilityType.Intelligence),
        new(SkillType.Medicine, "Medicina", AbilityType.Wisdom),
        new(SkillType.Nature, "Natureza", AbilityType.Intelligence),
        new(SkillType.Perception, "Percepção", AbilityType.Wisdom),
        new(SkillType.Performance, "Atuação", AbilityType.Charisma),
        new(SkillType.Persuasion, "Persuasão", AbilityType.Charisma),
        new(SkillType.Religion, "Religião", AbilityType.Intelligence),
        new(SkillType.SleightOfHand, "Prestidigitação", AbilityType.Dexterity),
        new(SkillType.Stealth, "Furtividade", AbilityType.Dexterity),
        new(SkillType.Survival, "Sobrevivência", AbilityType.Wisdom)
    ];

    private static readonly IReadOnlyList<ConditionDefinition> ConditionDefinitions =
    [
        new(ConditionType.Blinded, "Cego", "Visão comprometida. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Charmed, "Enfeitiçado", "Influência social ou mágica ativa. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Deafened, "Surdo", "Audição comprometida. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Frightened, "Amedrontado", "Medo ativo. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Grappled, "Agarrado", "Movimento limitado por agarrão. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Incapacitated, "Incapacitado", "Ações limitadas. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Invisible, "Invisível", "Difícil de perceber visualmente. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Paralyzed, "Paralisado", "Movimento severamente limitado. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Petrified, "Petrificado", "Corpo transformado ou imobilizado. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Poisoned, "Envenenado", "Veneno ou toxina ativa. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Prone, "Caído", "Personagem no chão. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Restrained, "Contido", "Movimento restringido. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Stunned, "Atordoado", "Reações e ações prejudicadas. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Unconscious, "Inconsciente", "Sem consciência. Ajuste efeitos conforme sua mesa."),
        new(ConditionType.Exhaustion, "Exaustão", "Cansaço acumulado. Ajuste níveis e efeitos conforme sua mesa.")
    ];

    public async Task<IReadOnlyList<CharacterSummaryResponse>> GetVisibleAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var masterCampaignIds = dbContext.CampaignMembers
            .Where(member => member.UserId == userId && member.Role == CampaignRole.Master)
            .Select(member => member.CampaignId);

        return await dbContext.Characters
            .AsNoTracking()
            .Include(character => character.Campaign)
            .Where(character => character.UserId == userId
                || (character.CampaignId.HasValue && masterCampaignIds.Contains(character.CampaignId.Value)))
            .OrderBy(character => character.Name)
            .Select(character => new CharacterSummaryResponse(
                character.Id,
                character.UserId,
                character.CampaignId,
                character.Campaign == null ? null : character.Campaign.Name,
                character.Name,
                character.Nickname,
                character.AvatarUrl,
                character.TotalLevel,
                character.Species,
                character.MainClass,
                character.Subclass,
                character.ArmorClass,
                character.CurrentHitPoints,
                character.MaxHitPoints,
                character.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult<CharacterResponse>> GetByIdAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<CharacterResponse>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        return ServiceResult<CharacterResponse>.Success(ToResponse(character, userId));
    }

    public async Task<ServiceResult<CharacterResponse>> CreateAsync(
        Guid userId,
        CharacterRequest request,
        CancellationToken cancellationToken)
    {
        var validationError = await ValidateRequestAsync(userId, request, cancellationToken);
        if (validationError is not null)
        {
            return ServiceResult<CharacterResponse>.Failure(validationError.Message, validationError.Type);
        }

        var character = new Character
        {
            UserId = userId
        };

        ApplyRequest(character, request);
        dbContext.Characters.Add(character);
        await dbContext.SaveChangesAsync(cancellationToken);

        var created = await GetCharacterAsync(character.Id, cancellationToken);
        return ServiceResult<CharacterResponse>.Success(ToResponse(created!, userId));
    }

    public async Task<ServiceResult<CharacterResponse>> UpdateAsync(
        Guid userId,
        Guid characterId,
        CharacterRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var validationError = await ValidateRequestAsync(userId, request, cancellationToken);
        if (validationError is not null)
        {
            return ServiceResult<CharacterResponse>.Failure(validationError.Message, validationError.Type);
        }

        ApplyRequest(character, request);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterResponse>.Success(ToResponse(character, userId));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await dbContext.Characters.SingleOrDefaultAsync(item => item.Id == characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<bool>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<bool>.Failure("Você só pode excluir seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        dbContext.Characters.Remove(character);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<IReadOnlyList<AbilityScoreResponse>>> GetAttributesAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<AbilityScoreResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<AbilityScoreResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        return ServiceResult<IReadOnlyList<AbilityScoreResponse>>.Success(ToAbilityScores(character));
    }

    public async Task<ServiceResult<IReadOnlyList<AbilityScoreResponse>>> UpdateAttributesAsync(
        Guid userId,
        Guid characterId,
        AbilityScoreRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<AbilityScoreResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<IReadOnlyList<AbilityScoreResponse>>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var validationError = ValidateScores(request);
        if (validationError is not null)
        {
            return ServiceResult<IReadOnlyList<AbilityScoreResponse>>.Failure(validationError);
        }

        character.Strength = request.Strength;
        character.Dexterity = request.Dexterity;
        character.Constitution = request.Constitution;
        character.Intelligence = request.Intelligence;
        character.Wisdom = request.Wisdom;
        character.Charisma = request.Charisma;
        character.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<IReadOnlyList<AbilityScoreResponse>>.Success(ToAbilityScores(character));
    }

    public async Task<ServiceResult<IReadOnlyList<SavingThrowResponse>>> GetSavingThrowsAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<SavingThrowResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<SavingThrowResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        return ServiceResult<IReadOnlyList<SavingThrowResponse>>.Success(ToSavingThrows(character));
    }

    public async Task<ServiceResult<IReadOnlyList<SavingThrowResponse>>> UpdateSavingThrowsAsync(
        Guid userId,
        Guid characterId,
        IReadOnlyList<SavingThrowRequest> request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<SavingThrowResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<IReadOnlyList<SavingThrowResponse>>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        foreach (var savingThrow in request)
        {
            ApplySavingThrow(character, savingThrow);
        }

        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<SavingThrowResponse>>.Success(ToSavingThrows(character));
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterSkillResponse>>> GetSkillsAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterWithSkillsAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterSkillResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CharacterSkillResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        await EnsureSkillsAsync(character, cancellationToken);
        return ServiceResult<IReadOnlyList<CharacterSkillResponse>>.Success(ToSkills(character));
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterSkillResponse>>> UpdateSkillsAsync(
        Guid userId,
        Guid characterId,
        IReadOnlyList<CharacterSkillRequest> request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterWithSkillsAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterSkillResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<IReadOnlyList<CharacterSkillResponse>>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        await EnsureSkillsAsync(character, cancellationToken);

        foreach (var requestSkill in request)
        {
            var definition = GetSkillDefinition(requestSkill.SkillType);
            var skill = character.Skills.Single(item => item.SkillType == requestSkill.SkillType);
            skill.BaseAttribute = definition.BaseAttribute;
            skill.IsProficient = requestSkill.IsProficient;
            skill.IsExpertise = requestSkill.IsExpertise;
            skill.CustomBonus = requestSkill.CustomBonus;
        }

        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<CharacterSkillResponse>>.Success(ToSkills(character));
    }

    public async Task<ServiceResult<CharacterCombatResponse>> GetCombatAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterCombatResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<CharacterCombatResponse>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        return ServiceResult<CharacterCombatResponse>.Success(ToCombat(character));
    }

    public async Task<ServiceResult<CharacterCombatResponse>> UpdateCombatAsync(
        Guid userId,
        Guid characterId,
        CharacterCombatRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterCombatResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterCombatResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var validationError = ValidateCombat(request);
        if (validationError is not null)
        {
            return ServiceResult<CharacterCombatResponse>.Failure(validationError);
        }

        character.ArmorClass = request.ArmorClass;
        character.Initiative = request.Initiative;
        character.Speed = request.Speed;
        character.MaxHitPoints = request.MaxHitPoints;
        character.CurrentHitPoints = request.CurrentHitPoints;
        character.TemporaryHitPoints = request.TemporaryHitPoints;
        character.TotalHitDice = NormalizeRequired(request.TotalHitDice);
        character.AvailableHitDice = NormalizeRequired(request.AvailableHitDice);
        character.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<CharacterCombatResponse>.Success(ToCombat(character));
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterAttackResponse>>> GetAttacksAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterAttackResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CharacterAttackResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        var attacks = await dbContext.CharacterAttacks
            .AsNoTracking()
            .Where(attack => attack.CharacterId == characterId)
            .OrderBy(attack => attack.Name)
            .Select(attack => ToAttackResponse(attack))
            .ToListAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<CharacterAttackResponse>>.Success(attacks);
    }

    public async Task<ServiceResult<CharacterAttackResponse>> CreateAttackAsync(
        Guid userId,
        Guid characterId,
        CharacterAttackRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterAttackResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterAttackResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var validationError = ValidateAttack(request);
        if (validationError is not null)
        {
            return ServiceResult<CharacterAttackResponse>.Failure(validationError);
        }

        var attack = new CharacterAttack
        {
            CharacterId = characterId
        };
        ApplyAttack(attack, request);

        dbContext.CharacterAttacks.Add(attack);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterAttackResponse>.Success(ToAttackResponse(attack));
    }

    public async Task<ServiceResult<CharacterAttackResponse>> UpdateAttackAsync(
        Guid userId,
        Guid characterId,
        Guid attackId,
        CharacterAttackRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterAttackResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterAttackResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var attack = await dbContext.CharacterAttacks
            .SingleOrDefaultAsync(item => item.Id == attackId && item.CharacterId == characterId, cancellationToken);
        if (attack is null)
        {
            return ServiceResult<CharacterAttackResponse>.Failure("Ataque não encontrado.", ServiceErrorType.NotFound);
        }

        var validationError = ValidateAttack(request);
        if (validationError is not null)
        {
            return ServiceResult<CharacterAttackResponse>.Failure(validationError);
        }

        ApplyAttack(attack, request);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterAttackResponse>.Success(ToAttackResponse(attack));
    }

    public async Task<ServiceResult<bool>> DeleteAttackAsync(
        Guid userId,
        Guid characterId,
        Guid attackId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<bool>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<bool>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var attack = await dbContext.CharacterAttacks
            .SingleOrDefaultAsync(item => item.Id == attackId && item.CharacterId == characterId, cancellationToken);
        if (attack is null)
        {
            return ServiceResult<bool>.Failure("Ataque não encontrado.", ServiceErrorType.NotFound);
        }

        dbContext.CharacterAttacks.Remove(attack);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterConditionResponse>>> GetConditionsAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterWithConditionsAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterConditionResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CharacterConditionResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        await EnsureConditionsAsync(character, cancellationToken);
        return ServiceResult<IReadOnlyList<CharacterConditionResponse>>.Success(ToConditions(character));
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterConditionResponse>>> UpdateConditionsAsync(
        Guid userId,
        Guid characterId,
        IReadOnlyList<CharacterConditionRequest> request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterWithConditionsAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterConditionResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<IReadOnlyList<CharacterConditionResponse>>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        await EnsureConditionsAsync(character, cancellationToken);

        foreach (var requestCondition in request)
        {
            var definition = GetConditionDefinition(requestCondition.ConditionType);
            var condition = character.Conditions.Single(item => item.ConditionType == requestCondition.ConditionType);
            condition.Name = NormalizeRequired(requestCondition.Name);
            condition.Description = NormalizeRequired(requestCondition.Description);
            condition.IsActive = requestCondition.IsActive;
            condition.Notes = NormalizeRequired(requestCondition.Notes);

            if (string.IsNullOrWhiteSpace(condition.Name))
            {
                condition.Name = definition.Name;
            }

            if (string.IsNullOrWhiteSpace(condition.Description))
            {
                condition.Description = definition.Description;
            }
        }

        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<CharacterConditionResponse>>.Success(ToConditions(character));
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterNoteResponse>>> GetNotesAsync(
        Guid userId,
        Guid characterId,
        string? search,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterNoteResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        var isOwner = character.UserId == userId;
        var isMasterViewer = !isOwner && await CanViewAsync(userId, character, cancellationToken);
        if (!isOwner && !isMasterViewer)
        {
            return ServiceResult<IReadOnlyList<CharacterNoteResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        var query = dbContext.CharacterNotes
            .AsNoTracking()
            .Where(note => note.CharacterId == characterId);

        if (!isOwner)
        {
            query = query.Where(note => note.IsVisibleToMaster && !note.IsPrivate);
        }

        var normalizedSearch = NormalizeOptional(search);
        if (normalizedSearch is not null)
        {
            var term = $"%{normalizedSearch}%";
            query = query.Where(note =>
                EF.Functions.ILike(note.Title, term)
                || EF.Functions.ILike(note.Content, term)
                || EF.Functions.ILike(note.Category, term)
                || EF.Functions.ILike(note.Tags, term));
        }

        var notes = await query
            .OrderByDescending(note => note.UpdatedAt ?? note.CreatedAt)
            .ThenBy(note => note.Title)
            .Select(note => ToNoteResponse(note, isOwner))
            .ToListAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<CharacterNoteResponse>>.Success(notes);
    }

    public async Task<ServiceResult<CharacterNoteResponse>> GetNoteByIdAsync(
        Guid userId,
        Guid characterId,
        Guid noteId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterNoteResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        var note = await dbContext.CharacterNotes
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == noteId && item.CharacterId == characterId, cancellationToken);
        if (note is null)
        {
            return ServiceResult<CharacterNoteResponse>.Failure("Nota não encontrada.", ServiceErrorType.NotFound);
        }

        var isOwner = character.UserId == userId;
        if (!isOwner)
        {
            var canMasterView = note.IsVisibleToMaster && !note.IsPrivate && await CanViewAsync(userId, character, cancellationToken);
            if (!canMasterView)
            {
                return ServiceResult<CharacterNoteResponse>.Failure("Você não pode visualizar esta nota.", ServiceErrorType.Forbidden);
            }
        }

        return ServiceResult<CharacterNoteResponse>.Success(ToNoteResponse(note, isOwner));
    }

    public async Task<ServiceResult<CharacterNoteResponse>> CreateNoteAsync(
        Guid userId,
        Guid characterId,
        CharacterNoteRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterNoteResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterNoteResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var validationError = ValidateNote(request);
        if (validationError is not null)
        {
            return ServiceResult<CharacterNoteResponse>.Failure(validationError);
        }

        var note = new CharacterNote
        {
            CharacterId = characterId
        };
        ApplyNote(note, request);

        dbContext.CharacterNotes.Add(note);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterNoteResponse>.Success(ToNoteResponse(note, true));
    }

    public async Task<ServiceResult<CharacterNoteResponse>> UpdateNoteAsync(
        Guid userId,
        Guid characterId,
        Guid noteId,
        CharacterNoteRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterNoteResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterNoteResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var note = await dbContext.CharacterNotes
            .SingleOrDefaultAsync(item => item.Id == noteId && item.CharacterId == characterId, cancellationToken);
        if (note is null)
        {
            return ServiceResult<CharacterNoteResponse>.Failure("Nota não encontrada.", ServiceErrorType.NotFound);
        }

        var validationError = ValidateNote(request);
        if (validationError is not null)
        {
            return ServiceResult<CharacterNoteResponse>.Failure(validationError);
        }

        ApplyNote(note, request);
        note.UpdatedAt = DateTime.UtcNow;
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterNoteResponse>.Success(ToNoteResponse(note, true));
    }

    public async Task<ServiceResult<bool>> DeleteNoteAsync(
        Guid userId,
        Guid characterId,
        Guid noteId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<bool>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<bool>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var note = await dbContext.CharacterNotes
            .SingleOrDefaultAsync(item => item.Id == noteId && item.CharacterId == characterId, cancellationToken);
        if (note is null)
        {
            return ServiceResult<bool>.Failure("Nota não encontrada.", ServiceErrorType.NotFound);
        }

        dbContext.CharacterNotes.Remove(note);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterInventoryItemResponse>>> GetInventoryAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterInventoryItemResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CharacterInventoryItemResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        var canEdit = character.UserId == userId;
        var items = await dbContext.CharacterInventoryItems
            .AsNoTracking()
            .Where(item => item.CharacterId == characterId)
            .OrderBy(item => item.ItemType)
            .ThenBy(item => item.Name)
            .Select(item => ToInventoryItemResponse(item, canEdit))
            .ToListAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<CharacterInventoryItemResponse>>.Success(items);
    }

    public async Task<ServiceResult<CharacterInventoryItemResponse>> CreateInventoryItemAsync(
        Guid userId,
        Guid characterId,
        CharacterInventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterInventoryItemResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterInventoryItemResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var validationError = ValidateInventoryItem(request);
        if (validationError is not null)
        {
            return ServiceResult<CharacterInventoryItemResponse>.Failure(validationError);
        }

        var item = new CharacterInventoryItem
        {
            CharacterId = characterId
        };
        ApplyInventoryItem(item, request);

        dbContext.CharacterInventoryItems.Add(item);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterInventoryItemResponse>.Success(ToInventoryItemResponse(item, true));
    }

    public async Task<ServiceResult<CharacterInventoryItemResponse>> UpdateInventoryItemAsync(
        Guid userId,
        Guid characterId,
        Guid itemId,
        CharacterInventoryItemRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterInventoryItemResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterInventoryItemResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var item = await dbContext.CharacterInventoryItems
            .SingleOrDefaultAsync(entry => entry.Id == itemId && entry.CharacterId == characterId, cancellationToken);
        if (item is null)
        {
            return ServiceResult<CharacterInventoryItemResponse>.Failure("Item não encontrado.", ServiceErrorType.NotFound);
        }

        var validationError = ValidateInventoryItem(request);
        if (validationError is not null)
        {
            return ServiceResult<CharacterInventoryItemResponse>.Failure(validationError);
        }

        ApplyInventoryItem(item, request);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterInventoryItemResponse>.Success(ToInventoryItemResponse(item, true));
    }

    public async Task<ServiceResult<bool>> DeleteInventoryItemAsync(
        Guid userId,
        Guid characterId,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<bool>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<bool>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var item = await dbContext.CharacterInventoryItems
            .SingleOrDefaultAsync(entry => entry.Id == itemId && entry.CharacterId == characterId, cancellationToken);
        if (item is null)
        {
            return ServiceResult<bool>.Failure("Item não encontrado.", ServiceErrorType.NotFound);
        }

        dbContext.CharacterInventoryItems.Remove(item);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<CharacterCurrencyResponse>> GetCurrencyAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterCurrencyResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<CharacterCurrencyResponse>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        return ServiceResult<CharacterCurrencyResponse>.Success(ToCurrency(character, character.UserId == userId));
    }

    public async Task<ServiceResult<CharacterCurrencyResponse>> UpdateCurrencyAsync(
        Guid userId,
        Guid characterId,
        CharacterCurrencyRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterCurrencyResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterCurrencyResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var validationError = ValidateCurrency(request);
        if (validationError is not null)
        {
            return ServiceResult<CharacterCurrencyResponse>.Failure(validationError);
        }

        character.Copper = request.Copper;
        character.Silver = request.Silver;
        character.Electrum = request.Electrum;
        character.Gold = request.Gold;
        character.Platinum = request.Platinum;
        character.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<CharacterCurrencyResponse>.Success(ToCurrency(character, true));
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterAssetResponse>>> GetAssetsAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterAssetResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CharacterAssetResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        var canEdit = character.UserId == userId;
        var assets = await dbContext.CharacterAssets
            .AsNoTracking()
            .Where(asset => asset.CharacterId == characterId)
            .OrderByDescending(asset => asset.UploadedAt)
            .Select(asset => ToAssetResponse(asset, canEdit))
            .ToListAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<CharacterAssetResponse>>.Success(assets);
    }

    public async Task<ServiceResult<CharacterAssetResponse>> UploadAssetAsync(
        Guid userId,
        Guid characterId,
        Stream fileStream,
        string originalFileName,
        string contentType,
        AssetType assetType,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterAssetResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterAssetResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        if (assetType == AssetType.Document)
        {
            return ServiceResult<CharacterAssetResponse>.Failure("Documentos ainda não são aceitos.");
        }

        var storedFile = await SaveFileSafelyAsync(fileStream, originalFileName, contentType, cancellationToken);
        if (!storedFile.Succeeded)
        {
            return ServiceResult<CharacterAssetResponse>.Failure(storedFile.Error ?? "Erro ao salvar arquivo.");
        }

        var asset = new CharacterAsset
        {
            CharacterId = characterId,
            FileName = storedFile.Data!.FileName,
            FileUrl = storedFile.Data.FileUrl,
            FileType = storedFile.Data.ContentType,
            AssetType = assetType
        };

        dbContext.CharacterAssets.Add(asset);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterAssetResponse>.Success(ToAssetResponse(asset, true));
    }

    public async Task<ServiceResult<bool>> DeleteAssetAsync(
        Guid userId,
        Guid characterId,
        Guid assetId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<bool>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<bool>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var asset = await dbContext.CharacterAssets
            .SingleOrDefaultAsync(item => item.Id == assetId && item.CharacterId == characterId, cancellationToken);
        if (asset is null)
        {
            return ServiceResult<bool>.Failure("Arquivo não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.AvatarUrl == asset.FileUrl)
        {
            character.AvatarUrl = null;
        }

        if (character.TokenImageUrl == asset.FileUrl)
        {
            character.TokenImageUrl = null;
        }

        dbContext.CharacterAssets.Remove(asset);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        await fileStorageService.DeleteAsync(asset.FileUrl, cancellationToken);

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<CharacterResponse>> UploadAvatarAsync(
        Guid userId,
        Guid characterId,
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        return await UploadPrimaryImageAsync(userId, characterId, fileStream, originalFileName, contentType, AssetType.Avatar, cancellationToken);
    }

    public async Task<ServiceResult<CharacterResponse>> UploadTokenAsync(
        Guid userId,
        Guid characterId,
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        return await UploadPrimaryImageAsync(userId, characterId, fileStream, originalFileName, contentType, AssetType.Token, cancellationToken);
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterSpellResponse>>> GetSpellsAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterSpellResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CharacterSpellResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        var canEdit = character.UserId == userId;
        var spells = await dbContext.CharacterSpells
            .AsNoTracking()
            .Include(characterSpell => characterSpell.Spell)
            .Where(characterSpell => characterSpell.CharacterId == characterId)
            .OrderBy(characterSpell => characterSpell.Spell.Level)
            .ThenBy(characterSpell => characterSpell.Spell.Name)
            .Select(characterSpell => ToCharacterSpellResponse(characterSpell, canEdit))
            .ToListAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<CharacterSpellResponse>>.Success(spells);
    }

    public async Task<ServiceResult<CharacterSpellResponse>> AddSpellAsync(
        Guid userId,
        Guid characterId,
        CharacterSpellRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterSpellResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterSpellResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var spell = await dbContext.Spells
            .Include(item => item.Campaign)
            .SingleOrDefaultAsync(item => item.Id == request.SpellId, cancellationToken);
        if (spell is null)
        {
            return ServiceResult<CharacterSpellResponse>.Failure("Magia não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await CanViewSpellAsync(userId, spell, cancellationToken))
        {
            return ServiceResult<CharacterSpellResponse>.Failure("Você não pode usar esta magia.", ServiceErrorType.Forbidden);
        }

        var alreadyAdded = await dbContext.CharacterSpells.AnyAsync(item =>
            item.CharacterId == characterId && item.SpellId == request.SpellId,
            cancellationToken);
        if (alreadyAdded)
        {
            return ServiceResult<CharacterSpellResponse>.Failure("Magia já adicionada ao personagem.", ServiceErrorType.Conflict);
        }

        var characterSpell = new CharacterSpell
        {
            CharacterId = characterId,
            SpellId = request.SpellId
        };
        ApplyCharacterSpell(characterSpell, request);

        dbContext.CharacterSpells.Add(characterSpell);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        characterSpell.Spell = spell;
        return ServiceResult<CharacterSpellResponse>.Success(ToCharacterSpellResponse(characterSpell, true));
    }

    public async Task<ServiceResult<CharacterSpellResponse>> UpdateSpellAsync(
        Guid userId,
        Guid characterId,
        Guid characterSpellId,
        CharacterSpellUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterSpellResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterSpellResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var characterSpell = await dbContext.CharacterSpells
            .Include(item => item.Spell)
            .SingleOrDefaultAsync(item => item.Id == characterSpellId && item.CharacterId == characterId, cancellationToken);
        if (characterSpell is null)
        {
            return ServiceResult<CharacterSpellResponse>.Failure("Magia do personagem não encontrada.", ServiceErrorType.NotFound);
        }

        ApplyCharacterSpell(characterSpell, request);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterSpellResponse>.Success(ToCharacterSpellResponse(characterSpell, true));
    }

    public async Task<ServiceResult<bool>> DeleteSpellAsync(
        Guid userId,
        Guid characterId,
        Guid characterSpellId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<bool>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<bool>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var characterSpell = await dbContext.CharacterSpells
            .SingleOrDefaultAsync(item => item.Id == characterSpellId && item.CharacterId == characterId, cancellationToken);
        if (characterSpell is null)
        {
            return ServiceResult<bool>.Failure("Magia do personagem não encontrada.", ServiceErrorType.NotFound);
        }

        dbContext.CharacterSpells.Remove(characterSpell);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>> GetSpellSlotsAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        var canEdit = character.UserId == userId;
        var existingSlots = await dbContext.CharacterSpellSlots
            .AsNoTracking()
            .Where(slot => slot.CharacterId == characterId)
            .OrderBy(slot => slot.SpellLevel)
            .ToListAsync(cancellationToken);

        var responses = Enumerable.Range(1, 9)
            .Select(level =>
            {
                var slot = existingSlots.SingleOrDefault(item => item.SpellLevel == level);
                return slot is null
                    ? new CharacterSpellSlotResponse(Guid.Empty, characterId, level, 0, 0, canEdit)
                    : ToSpellSlotResponse(slot, canEdit);
            })
            .ToList();

        return ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>.Success(responses);
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>> UpdateSpellSlotsAsync(
        Guid userId,
        Guid characterId,
        IReadOnlyList<CharacterSpellSlotRequest> request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var validationError = ValidateSpellSlots(request);
        if (validationError is not null)
        {
            return ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>.Failure(validationError);
        }

        var existingSlots = await dbContext.CharacterSpellSlots
            .Where(slot => slot.CharacterId == characterId)
            .ToListAsync(cancellationToken);

        foreach (var slotRequest in request.GroupBy(item => item.SpellLevel).Select(group => group.Last()))
        {
            var slot = existingSlots.SingleOrDefault(item => item.SpellLevel == slotRequest.SpellLevel);
            if (slot is null)
            {
                slot = new CharacterSpellSlot
                {
                    CharacterId = characterId,
                    SpellLevel = slotRequest.SpellLevel
                };
                dbContext.CharacterSpellSlots.Add(slot);
                existingSlots.Add(slot);
            }

            slot.TotalSlots = slotRequest.TotalSlots;
            slot.UsedSlots = slotRequest.UsedSlots;
        }

        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var responses = existingSlots
            .OrderBy(slot => slot.SpellLevel)
            .Select(slot => ToSpellSlotResponse(slot, true))
            .ToList();

        return ServiceResult<IReadOnlyList<CharacterSpellSlotResponse>>.Success(responses);
    }

    public async Task<ServiceResult<IReadOnlyList<CharacterFeatureResponse>>> GetFeaturesAsync(
        Guid userId,
        Guid characterId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<IReadOnlyList<CharacterFeatureResponse>>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanViewAsync(userId, character, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CharacterFeatureResponse>>.Failure("Você não pode visualizar este personagem.", ServiceErrorType.Forbidden);
        }

        var canEdit = character.UserId == userId;
        var features = await dbContext.CharacterFeatures
            .AsNoTracking()
            .Include(characterFeature => characterFeature.Feature)
            .Where(characterFeature => characterFeature.CharacterId == characterId)
            .OrderBy(characterFeature => characterFeature.Feature == null ? characterFeature.CustomName : characterFeature.Feature.Name)
            .Select(characterFeature => ToCharacterFeatureResponse(characterFeature, canEdit))
            .ToListAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<CharacterFeatureResponse>>.Success(features);
    }

    public async Task<ServiceResult<CharacterFeatureResponse>> AddFeatureAsync(
        Guid userId,
        Guid characterId,
        CharacterFeatureRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterFeatureResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterFeatureResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var validationError = await ValidateCharacterFeatureAsync(userId, request, cancellationToken);
        if (validationError is not null)
        {
            return ServiceResult<CharacterFeatureResponse>.Failure(validationError.Message, validationError.Type);
        }

        var characterFeature = new CharacterFeature
        {
            CharacterId = characterId
        };
        ApplyCharacterFeature(characterFeature, request);

        dbContext.CharacterFeatures.Add(characterFeature);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var created = await dbContext.CharacterFeatures
            .Include(item => item.Feature)
            .SingleAsync(item => item.Id == characterFeature.Id, cancellationToken);

        return ServiceResult<CharacterFeatureResponse>.Success(ToCharacterFeatureResponse(created, true));
    }

    public async Task<ServiceResult<CharacterFeatureResponse>> UpdateFeatureAsync(
        Guid userId,
        Guid characterId,
        Guid characterFeatureId,
        CharacterFeatureRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterFeatureResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterFeatureResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var characterFeature = await dbContext.CharacterFeatures
            .Include(item => item.Feature)
            .SingleOrDefaultAsync(item => item.Id == characterFeatureId && item.CharacterId == characterId, cancellationToken);
        if (characterFeature is null)
        {
            return ServiceResult<CharacterFeatureResponse>.Failure("Talento/característica do personagem não encontrado.", ServiceErrorType.NotFound);
        }

        var validationError = await ValidateCharacterFeatureAsync(userId, request, cancellationToken);
        if (validationError is not null)
        {
            return ServiceResult<CharacterFeatureResponse>.Failure(validationError.Message, validationError.Type);
        }

        ApplyCharacterFeature(characterFeature, request);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        if (characterFeature.FeatureId.HasValue && characterFeature.Feature?.Id != characterFeature.FeatureId.Value)
        {
            await dbContext.Entry(characterFeature).Reference(item => item.Feature).LoadAsync(cancellationToken);
        }

        return ServiceResult<CharacterFeatureResponse>.Success(ToCharacterFeatureResponse(characterFeature, true));
    }

    public async Task<ServiceResult<bool>> DeleteFeatureAsync(
        Guid userId,
        Guid characterId,
        Guid characterFeatureId,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<bool>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<bool>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var characterFeature = await dbContext.CharacterFeatures
            .SingleOrDefaultAsync(item => item.Id == characterFeatureId && item.CharacterId == characterId, cancellationToken);
        if (characterFeature is null)
        {
            return ServiceResult<bool>.Failure("Talento/característica do personagem não encontrado.", ServiceErrorType.NotFound);
        }

        dbContext.CharacterFeatures.Remove(characterFeature);
        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<CharacterRestResponse>> ShortRestAsync(
        Guid userId,
        Guid characterId,
        CharacterRestRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterRestResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterRestResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var features = await dbContext.CharacterFeatures
            .Include(feature => feature.Feature)
            .Where(feature => feature.CharacterId == characterId)
            .ToListAsync(cancellationToken);

        foreach (var feature in features.Where(feature => feature.RecoveryType == RecoveryType.ShortRest))
        {
            feature.CurrentUses = feature.MaxUses;
        }

        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterRestResponse>.Success(await ToRestResponseAsync(character, userId, cancellationToken));
    }

    public async Task<ServiceResult<CharacterRestResponse>> LongRestAsync(
        Guid userId,
        Guid characterId,
        CharacterRestRequest request,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterRestResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterRestResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var features = await dbContext.CharacterFeatures
            .Include(feature => feature.Feature)
            .Where(feature => feature.CharacterId == characterId)
            .ToListAsync(cancellationToken);
        foreach (var feature in features.Where(feature =>
            feature.RecoveryType is RecoveryType.ShortRest or RecoveryType.LongRest))
        {
            feature.CurrentUses = feature.MaxUses;
        }

        var slots = await dbContext.CharacterSpellSlots
            .Where(slot => slot.CharacterId == characterId)
            .ToListAsync(cancellationToken);
        foreach (var slot in slots)
        {
            slot.UsedSlots = 0;
        }

        if (request.RestoreHitPoints)
        {
            character.CurrentHitPoints = character.MaxHitPoints;
            character.TemporaryHitPoints = 0;
        }

        if (request.RestoreHitDice)
        {
            character.AvailableHitDice = character.TotalHitDice;
        }

        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterRestResponse>.Success(await ToRestResponseAsync(character, userId, cancellationToken));
    }

    private async Task<Character?> GetCharacterAsync(Guid characterId, CancellationToken cancellationToken)
    {
        return await dbContext.Characters
            .Include(character => character.Campaign)
            .SingleOrDefaultAsync(character => character.Id == characterId, cancellationToken);
    }

    private async Task<Character?> GetCharacterWithSkillsAsync(Guid characterId, CancellationToken cancellationToken)
    {
        return await dbContext.Characters
            .Include(character => character.Campaign)
            .Include(character => character.Skills)
            .SingleOrDefaultAsync(character => character.Id == characterId, cancellationToken);
    }

    private async Task<Character?> GetCharacterWithConditionsAsync(Guid characterId, CancellationToken cancellationToken)
    {
        return await dbContext.Characters
            .Include(character => character.Campaign)
            .Include(character => character.Conditions)
            .SingleOrDefaultAsync(character => character.Id == characterId, cancellationToken);
    }

    private async Task<bool> CanViewAsync(Guid userId, Character character, CancellationToken cancellationToken)
    {
        if (character.UserId == userId)
        {
            return true;
        }

        if (!character.CampaignId.HasValue)
        {
            return false;
        }

        return await dbContext.CampaignMembers.AnyAsync(
            member => member.CampaignId == character.CampaignId.Value
                && member.UserId == userId
                && member.Role == CampaignRole.Master,
            cancellationToken);
    }

    private async Task<bool> CanViewSpellAsync(Guid userId, Spell spell, CancellationToken cancellationToken)
    {
        if (spell.Visibility == SpellVisibility.LocalPublic)
        {
            return true;
        }

        if (spell.Visibility == SpellVisibility.Private)
        {
            return spell.CreatedByUserId == userId;
        }

        if (!spell.CampaignId.HasValue)
        {
            return false;
        }

        return await dbContext.CampaignMembers.AnyAsync(member =>
            member.CampaignId == spell.CampaignId.Value && member.UserId == userId,
            cancellationToken);
    }

    private async Task<bool> CanViewFeatureAsync(Guid userId, Feature feature, CancellationToken cancellationToken)
    {
        if (feature.Visibility == SpellVisibility.LocalPublic)
        {
            return true;
        }

        if (feature.Visibility == SpellVisibility.Private)
        {
            return feature.CreatedByUserId == userId;
        }

        if (!feature.CampaignId.HasValue)
        {
            return false;
        }

        return await dbContext.CampaignMembers.AnyAsync(member =>
            member.CampaignId == feature.CampaignId.Value && member.UserId == userId,
            cancellationToken);
    }

    private async Task<ValidationError?> ValidateRequestAsync(
        Guid userId,
        CharacterRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new ValidationError("Nome é obrigatório.");
        }

        if (request.Name.Trim().Length > 140)
        {
            return new ValidationError("Nome deve ter no máximo 140 caracteres.");
        }

        if (request.CampaignId.HasValue)
        {
            var isMember = await dbContext.CampaignMembers.AnyAsync(
                member => member.CampaignId == request.CampaignId.Value && member.UserId == userId,
                cancellationToken);

            if (!isMember)
            {
                return new ValidationError("Usuário precisa ser membro da campanha.", ServiceErrorType.Forbidden);
            }
        }

        if (request.TotalLevel < 1 || request.TotalLevel > 20)
        {
            return new ValidationError("Nível total deve ficar entre 1 e 20.");
        }

        if (request.ProficiencyBonus < 0 || request.ArmorClass < 0 || request.Speed < 0)
        {
            return new ValidationError("Bônus de proficiência, CA e deslocamento não podem ser negativos.");
        }

        if (request.MaxHitPoints < 0
            || request.CurrentHitPoints < 0
            || request.TemporaryHitPoints < 0
            || request.Experience < 0)
        {
            return new ValidationError("Vida, vida temporária e experiência não podem ser negativas.");
        }

        return null;
    }

    private static void ApplyRequest(Character character, CharacterRequest request)
    {
        character.CampaignId = request.CampaignId;
        character.Name = request.Name.Trim();
        character.Nickname = NormalizeOptional(request.Nickname);
        character.AvatarUrl = NormalizeOptional(request.AvatarUrl);
        character.TokenImageUrl = NormalizeOptional(request.TokenImageUrl);
        character.TotalLevel = request.TotalLevel;
        character.Species = NormalizeRequired(request.Species);
        character.MainClass = NormalizeRequired(request.MainClass);
        character.Subclass = NormalizeRequired(request.Subclass);
        character.Background = NormalizeRequired(request.Background);
        character.Alignment = NormalizeRequired(request.Alignment);
        character.Experience = request.Experience;
        character.Inspiration = request.Inspiration;
        character.ProficiencyBonus = request.ProficiencyBonus;
        character.ArmorClass = request.ArmorClass;
        character.Initiative = request.Initiative;
        character.Speed = request.Speed;
        character.MaxHitPoints = request.MaxHitPoints;
        character.CurrentHitPoints = request.CurrentHitPoints;
        character.TemporaryHitPoints = request.TemporaryHitPoints;
        character.TotalHitDice = NormalizeRequired(request.TotalHitDice);
        character.AvailableHitDice = NormalizeRequired(request.AvailableHitDice);
        character.PhysicalDescription = NormalizeRequired(request.PhysicalDescription);
        character.PersonalityTraits = NormalizeRequired(request.PersonalityTraits);
        character.Ideals = NormalizeRequired(request.Ideals);
        character.Bonds = NormalizeRequired(request.Bonds);
        character.Flaws = NormalizeRequired(request.Flaws);
        character.Backstory = NormalizeRequired(request.Backstory);
        character.QuickNotes = NormalizeRequired(request.QuickNotes);
    }

    private static CharacterResponse ToResponse(Character character, Guid currentUserId)
    {
        return new CharacterResponse(
            character.Id,
            character.UserId,
            character.CampaignId,
            character.Campaign?.Name,
            character.Name,
            character.Nickname,
            character.AvatarUrl,
            character.TokenImageUrl,
            character.TotalLevel,
            character.Species,
            character.MainClass,
            character.Subclass,
            character.Background,
            character.Alignment,
            character.Experience,
            character.Inspiration,
            character.ProficiencyBonus,
            character.ArmorClass,
            character.Initiative,
            character.Speed,
            character.MaxHitPoints,
            character.CurrentHitPoints,
            character.TemporaryHitPoints,
            character.TotalHitDice,
            character.AvailableHitDice,
            character.PhysicalDescription,
            character.PersonalityTraits,
            character.Ideals,
            character.Bonds,
            character.Flaws,
            character.Backstory,
            character.QuickNotes,
            character.CreatedAt,
            character.UpdatedAt,
            character.UserId == currentUserId);
    }

    private static IReadOnlyList<AbilityScoreResponse> ToAbilityScores(Character character)
        =>
        [
            ToAbilityScore(AbilityType.Strength, character.Strength),
            ToAbilityScore(AbilityType.Dexterity, character.Dexterity),
            ToAbilityScore(AbilityType.Constitution, character.Constitution),
            ToAbilityScore(AbilityType.Intelligence, character.Intelligence),
            ToAbilityScore(AbilityType.Wisdom, character.Wisdom),
            ToAbilityScore(AbilityType.Charisma, character.Charisma)
        ];

    private static IReadOnlyList<SavingThrowResponse> ToSavingThrows(Character character)
        =>
        [
            ToSavingThrow(AbilityType.Strength, character.Strength, character.StrengthSaveProficient, character.StrengthSaveCustomBonus, character.ProficiencyBonus),
            ToSavingThrow(AbilityType.Dexterity, character.Dexterity, character.DexteritySaveProficient, character.DexteritySaveCustomBonus, character.ProficiencyBonus),
            ToSavingThrow(AbilityType.Constitution, character.Constitution, character.ConstitutionSaveProficient, character.ConstitutionSaveCustomBonus, character.ProficiencyBonus),
            ToSavingThrow(AbilityType.Intelligence, character.Intelligence, character.IntelligenceSaveProficient, character.IntelligenceSaveCustomBonus, character.ProficiencyBonus),
            ToSavingThrow(AbilityType.Wisdom, character.Wisdom, character.WisdomSaveProficient, character.WisdomSaveCustomBonus, character.ProficiencyBonus),
            ToSavingThrow(AbilityType.Charisma, character.Charisma, character.CharismaSaveProficient, character.CharismaSaveCustomBonus, character.ProficiencyBonus)
        ];

    private static AbilityScoreResponse ToAbilityScore(AbilityType attribute, int score)
        => new(attribute, GetAbilityLabel(attribute), score, CalculateModifier(score));

    private static SavingThrowResponse ToSavingThrow(
        AbilityType attribute,
        int score,
        bool isProficient,
        int customBonus,
        int proficiencyBonus)
    {
        var modifier = CalculateModifier(score);
        var finalValue = modifier + customBonus + (isProficient ? proficiencyBonus : 0);
        return new SavingThrowResponse(attribute, GetAbilityLabel(attribute), modifier, isProficient, customBonus, finalValue);
    }

    private static IReadOnlyList<CharacterSkillResponse> ToSkills(Character character)
    {
        return SkillDefinitions
            .Select(definition =>
            {
                var skill = character.Skills.Single(item => item.SkillType == definition.SkillType);
                return ToSkillResponse(character, skill, definition);
            })
            .ToList();
    }

    private static CharacterSkillResponse ToSkillResponse(
        Character character,
        CharacterSkill skill,
        SkillDefinition definition)
    {
        var abilityModifier = CalculateModifier(GetAbilityScore(character, definition.BaseAttribute));
        var proficiencyBonus = skill.IsProficient ? character.ProficiencyBonus : 0;
        var expertiseBonus = skill.IsExpertise ? character.ProficiencyBonus : 0;
        var finalValue = abilityModifier + proficiencyBonus + expertiseBonus + skill.CustomBonus;

        return new CharacterSkillResponse(
            skill.Id,
            skill.SkillType,
            definition.Label,
            definition.BaseAttribute,
            GetAbilityLabel(definition.BaseAttribute),
            skill.IsProficient,
            skill.IsExpertise,
            skill.CustomBonus,
            finalValue);
    }

    private static CharacterCombatResponse ToCombat(Character character)
        => new(
            character.ArmorClass,
            character.Initiative,
            character.Speed,
            character.MaxHitPoints,
            character.CurrentHitPoints,
            character.TemporaryHitPoints,
            character.TotalHitDice,
            character.AvailableHitDice);

    private static CharacterAttackResponse ToAttackResponse(CharacterAttack attack)
        => new(
            attack.Id,
            attack.Name,
            attack.AttackBonus,
            attack.Damage,
            attack.DamageType,
            attack.Range,
            attack.UsesAttribute,
            attack.UsesAttribute.HasValue ? GetAbilityLabel(attack.UsesAttribute.Value) : null,
            attack.Notes);

    private static IReadOnlyList<CharacterConditionResponse> ToConditions(Character character)
    {
        return ConditionDefinitions
            .Select(definition =>
            {
                var condition = character.Conditions.Single(item => item.ConditionType == definition.ConditionType);
                return ToConditionResponse(condition);
            })
            .ToList();
    }

    private static CharacterConditionResponse ToConditionResponse(CharacterCondition condition)
        => new(
            condition.Id,
            condition.ConditionType,
            condition.Name,
            condition.Description,
            condition.IsActive,
            condition.Notes);

    private static CharacterNoteResponse ToNoteResponse(CharacterNote note, bool canEdit)
        => new(
            note.Id,
            note.CharacterId,
            note.Title,
            note.Content,
            note.Category,
            note.Tags,
            note.IsPrivate,
            note.IsVisibleToMaster,
            note.CreatedAt,
            note.UpdatedAt,
            canEdit);

    private static CharacterAssetResponse ToAssetResponse(CharacterAsset asset, bool canEdit)
        => new(
            asset.Id,
            asset.CharacterId,
            asset.FileName,
            asset.FileUrl,
            asset.FileType,
            asset.AssetType,
            asset.UploadedAt,
            canEdit);

    private static CharacterSpellResponse ToCharacterSpellResponse(CharacterSpell characterSpell, bool canEdit)
        => new(
            characterSpell.Id,
            characterSpell.CharacterId,
            characterSpell.SpellId,
            characterSpell.Spell.Name,
            characterSpell.Spell.EnglishName,
            characterSpell.Spell.Level,
            characterSpell.Spell.School,
            characterSpell.Spell.CastingTime,
            characterSpell.Spell.Range,
            characterSpell.Spell.Components,
            characterSpell.Spell.Material,
            characterSpell.Spell.Duration,
            characterSpell.Spell.IsConcentration,
            characterSpell.Spell.IsRitual,
            characterSpell.Spell.Description,
            characterSpell.Spell.HigherLevelDescription,
            characterSpell.Spell.AvailableClasses,
            characterSpell.Spell.Source,
            characterSpell.Spell.IsHomebrew,
            characterSpell.IsKnown,
            characterSpell.IsPrepared,
            characterSpell.IsFavorite,
            characterSpell.Notes,
            canEdit);

    private static CharacterSpellSlotResponse ToSpellSlotResponse(CharacterSpellSlot slot, bool canEdit)
        => new(
            slot.Id,
            slot.CharacterId,
            slot.SpellLevel,
            slot.TotalSlots,
            slot.UsedSlots,
            canEdit);

    private static CharacterFeatureResponse ToCharacterFeatureResponse(CharacterFeature characterFeature, bool canEdit)
    {
        var feature = characterFeature.Feature;
        var name = string.IsNullOrWhiteSpace(characterFeature.CustomName)
            ? feature?.Name ?? string.Empty
            : characterFeature.CustomName;
        var description = string.IsNullOrWhiteSpace(characterFeature.CustomDescription)
            ? feature?.Description ?? string.Empty
            : characterFeature.CustomDescription;

        return new CharacterFeatureResponse(
            characterFeature.Id,
            characterFeature.CharacterId,
            characterFeature.FeatureId,
            name,
            feature?.Type,
            feature is null ? null : GetFeatureTypeLabel(feature.Type),
            description,
            feature?.Source ?? string.Empty,
            feature?.Prerequisites ?? string.Empty,
            feature?.IsHomebrew ?? true,
            characterFeature.CustomName,
            characterFeature.CustomDescription,
            characterFeature.MaxUses,
            characterFeature.CurrentUses,
            characterFeature.RecoveryType,
            GetRecoveryTypeLabel(characterFeature.RecoveryType),
            characterFeature.Notes,
            canEdit);
    }

    private async Task<CharacterRestResponse> ToRestResponseAsync(
        Character character,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var features = await dbContext.CharacterFeatures
            .AsNoTracking()
            .Include(feature => feature.Feature)
            .Where(feature => feature.CharacterId == character.Id)
            .OrderBy(feature => feature.Feature == null ? feature.CustomName : feature.Feature.Name)
            .Select(feature => ToCharacterFeatureResponse(feature, true))
            .ToListAsync(cancellationToken);

        var slots = await dbContext.CharacterSpellSlots
            .AsNoTracking()
            .Where(slot => slot.CharacterId == character.Id)
            .OrderBy(slot => slot.SpellLevel)
            .Select(slot => ToSpellSlotResponse(slot, true))
            .ToListAsync(cancellationToken);

        return new CharacterRestResponse(ToResponse(character, userId), features, slots);
    }

    private static CharacterInventoryItemResponse ToInventoryItemResponse(CharacterInventoryItem item, bool canEdit)
        => new(
            item.Id,
            item.CharacterId,
            item.Name,
            item.Description,
            item.Quantity,
            item.Weight,
            item.Value,
            item.ItemType,
            GetItemTypeLabel(item.ItemType),
            item.Equipped,
            item.Attuned,
            item.Notes,
            item.Quantity * item.Weight,
            canEdit);

    private static CharacterCurrencyResponse ToCurrency(Character character, bool canEdit)
        => new(
            character.Copper,
            character.Silver,
            character.Electrum,
            character.Gold,
            character.Platinum,
            canEdit);

    private async Task<ServiceResult<CharacterResponse>> UploadPrimaryImageAsync(
        Guid userId,
        Guid characterId,
        Stream fileStream,
        string originalFileName,
        string contentType,
        AssetType assetType,
        CancellationToken cancellationToken)
    {
        var character = await GetCharacterAsync(characterId, cancellationToken);
        if (character is null)
        {
            return ServiceResult<CharacterResponse>.Failure("Personagem não encontrado.", ServiceErrorType.NotFound);
        }

        if (character.UserId != userId)
        {
            return ServiceResult<CharacterResponse>.Failure("Você só pode editar seus próprios personagens.", ServiceErrorType.Forbidden);
        }

        var storedFile = await SaveFileSafelyAsync(fileStream, originalFileName, contentType, cancellationToken);
        if (!storedFile.Succeeded)
        {
            return ServiceResult<CharacterResponse>.Failure(storedFile.Error ?? "Erro ao salvar arquivo.");
        }

        var asset = new CharacterAsset
        {
            CharacterId = characterId,
            FileName = storedFile.Data!.FileName,
            FileUrl = storedFile.Data.FileUrl,
            FileType = storedFile.Data.ContentType,
            AssetType = assetType
        };

        dbContext.CharacterAssets.Add(asset);
        if (assetType == AssetType.Avatar)
        {
            character.AvatarUrl = asset.FileUrl;
        }
        else
        {
            character.TokenImageUrl = asset.FileUrl;
        }

        character.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CharacterResponse>.Success(ToResponse(character, userId));
    }

    private async Task<ServiceResult<StoredFile>> SaveFileSafelyAsync(
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        try
        {
            return ServiceResult<StoredFile>.Success(
                await fileStorageService.SaveAsync(fileStream, originalFileName, contentType, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            return ServiceResult<StoredFile>.Failure(exception.Message);
        }
    }

    private static int CalculateModifier(int score)
        => (int)Math.Floor((score - 10) / 2.0);

    private static int GetAbilityScore(Character character, AbilityType attribute)
        => attribute switch
        {
            AbilityType.Strength => character.Strength,
            AbilityType.Dexterity => character.Dexterity,
            AbilityType.Constitution => character.Constitution,
            AbilityType.Intelligence => character.Intelligence,
            AbilityType.Wisdom => character.Wisdom,
            AbilityType.Charisma => character.Charisma,
            _ => 10
        };

    private static string GetAbilityLabel(AbilityType attribute)
        => attribute switch
        {
            AbilityType.Strength => "Força",
            AbilityType.Dexterity => "Destreza",
            AbilityType.Constitution => "Constituição",
            AbilityType.Intelligence => "Inteligência",
            AbilityType.Wisdom => "Sabedoria",
            AbilityType.Charisma => "Carisma",
            _ => attribute.ToString()
        };

    private static string GetItemTypeLabel(ItemType itemType)
        => itemType switch
        {
            ItemType.Weapon => "Arma",
            ItemType.Armor => "Armadura",
            ItemType.Consumable => "Consumível",
            ItemType.Tool => "Ferramenta",
            ItemType.MagicItem => "Item mágico",
            ItemType.Treasure => "Tesouro",
            ItemType.Other => "Outro",
            _ => itemType.ToString()
        };

    private static string GetFeatureTypeLabel(FeatureType featureType)
        => featureType switch
        {
            FeatureType.Feat => "Talento",
            FeatureType.Class => "Classe",
            FeatureType.Subclass => "Subclasse",
            FeatureType.Species => "Espécie",
            FeatureType.Background => "Antecedente",
            FeatureType.Homebrew => "Homebrew",
            _ => featureType.ToString()
        };

    private static string GetRecoveryTypeLabel(RecoveryType recoveryType)
        => recoveryType switch
        {
            RecoveryType.Manual => "Manual",
            RecoveryType.ShortRest => "Descanso curto",
            RecoveryType.LongRest => "Descanso longo",
            _ => recoveryType.ToString()
        };

    private static string? ValidateScores(AbilityScoreRequest request)
    {
        var scores = new[]
        {
            request.Strength,
            request.Dexterity,
            request.Constitution,
            request.Intelligence,
            request.Wisdom,
            request.Charisma
        };

        return scores.Any(score => score < 1 || score > 30)
            ? "Atributos devem ficar entre 1 e 30."
            : null;
    }

    private static string? ValidateCombat(CharacterCombatRequest request)
    {
        if (request.ArmorClass < 0 || request.Speed < 0)
        {
            return "CA e deslocamento não podem ser negativos.";
        }

        if (request.MaxHitPoints < 0 || request.CurrentHitPoints < 0 || request.TemporaryHitPoints < 0)
        {
            return "Pontos de vida não podem ser negativos.";
        }

        if (request.TotalHitDice.Length > 80 || request.AvailableHitDice.Length > 80)
        {
            return "Dados de vida devem ter no máximo 80 caracteres.";
        }

        return null;
    }

    private static string? ValidateAttack(CharacterAttackRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "Nome do ataque é obrigatório.";
        }

        if (request.Name.Trim().Length > 140)
        {
            return "Nome do ataque deve ter no máximo 140 caracteres.";
        }

        if ((request.Damage?.Length ?? 0) > 120
            || (request.DamageType?.Length ?? 0) > 80
            || (request.Range?.Length ?? 0) > 80
            || (request.Notes?.Length ?? 0) > 1000)
        {
            return "Campos do ataque excedem o limite permitido.";
        }

        return null;
    }

    private static string? ValidateNote(CharacterNoteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return "Título da nota é obrigatório.";
        }

        if (request.Title.Trim().Length > 180
            || (request.Category?.Length ?? 0) > 80
            || (request.Tags?.Length ?? 0) > 500
            || (request.Content?.Length ?? 0) > 10000)
        {
            return "Campos da nota excedem o limite permitido.";
        }

        return null;
    }

    private static string? ValidateInventoryItem(CharacterInventoryItemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "Nome do item é obrigatório.";
        }

        if (request.Name.Trim().Length > 160
            || (request.Description?.Length ?? 0) > 1500
            || (request.Notes?.Length ?? 0) > 1000)
        {
            return "Campos do item excedem o limite permitido.";
        }

        if (request.Quantity < 0 || request.Weight < 0 || request.Value < 0)
        {
            return "Quantidade, peso e valor não podem ser negativos.";
        }

        return null;
    }

    private static string? ValidateCurrency(CharacterCurrencyRequest request)
    {
        return request.Copper < 0
            || request.Silver < 0
            || request.Electrum < 0
            || request.Gold < 0
            || request.Platinum < 0
            ? "Moedas não podem ser negativas."
            : null;
    }

    private static string? ValidateSpellSlots(IReadOnlyList<CharacterSpellSlotRequest> request)
    {
        foreach (var slot in request)
        {
            if (slot.SpellLevel is < 1 or > 9)
            {
                return "Nível de slot deve ficar entre 1 e 9.";
            }

            if (slot.TotalSlots < 0 || slot.UsedSlots < 0)
            {
                return "Slots totais e usados não podem ser negativos.";
            }

            if (slot.UsedSlots > slot.TotalSlots)
            {
                return "Slots usados não podem passar dos slots totais.";
            }
        }

        return null;
    }

    private async Task<ValidationError?> ValidateCharacterFeatureAsync(
        Guid userId,
        CharacterFeatureRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.FeatureId.HasValue && string.IsNullOrWhiteSpace(request.CustomName))
        {
            return new ValidationError("Nome é obrigatório para característica manual.");
        }

        if ((request.CustomName?.Length ?? 0) > 180
            || (request.CustomDescription?.Length ?? 0) > 10000
            || (request.Notes?.Length ?? 0) > 1000)
        {
            return new ValidationError("Campos da característica excedem o limite permitido.");
        }

        if (request.MaxUses < 0 || request.CurrentUses < 0)
        {
            return new ValidationError("Usos não podem ser negativos.");
        }

        if (request.MaxUses == 0 && request.CurrentUses > 0)
        {
            return new ValidationError("Usos atuais exigem usos máximos maiores que zero.");
        }

        if (request.MaxUses > 0 && request.CurrentUses > request.MaxUses)
        {
            return new ValidationError("Usos atuais não podem passar dos usos máximos.");
        }

        if (request.FeatureId.HasValue)
        {
            var feature = await dbContext.Features
                .SingleOrDefaultAsync(item => item.Id == request.FeatureId.Value, cancellationToken);
            if (feature is null)
            {
                return new ValidationError("Talento/característica não encontrado.", ServiceErrorType.NotFound);
            }

            if (!await CanViewFeatureAsync(userId, feature, cancellationToken))
            {
                return new ValidationError("Você não pode usar este talento/característica.", ServiceErrorType.Forbidden);
            }
        }

        return null;
    }

    private static void ApplyNote(CharacterNote note, CharacterNoteRequest request)
    {
        note.Title = request.Title.Trim();
        note.Content = NormalizeRequired(request.Content);
        note.Category = string.IsNullOrWhiteSpace(request.Category) ? "Outros" : request.Category.Trim();
        note.Tags = NormalizeRequired(request.Tags);
        note.IsPrivate = request.IsPrivate;
        note.IsVisibleToMaster = request.IsPrivate ? false : request.IsVisibleToMaster;
    }

    private static void ApplyInventoryItem(CharacterInventoryItem item, CharacterInventoryItemRequest request)
    {
        item.Name = request.Name.Trim();
        item.Description = NormalizeRequired(request.Description);
        item.Quantity = request.Quantity;
        item.Weight = request.Weight;
        item.Value = request.Value;
        item.ItemType = request.ItemType;
        item.Equipped = request.Equipped;
        item.Attuned = request.Attuned;
        item.Notes = NormalizeRequired(request.Notes);
    }

    private static void ApplyCharacterSpell(CharacterSpell characterSpell, CharacterSpellRequest request)
    {
        characterSpell.IsKnown = request.IsKnown;
        characterSpell.IsPrepared = request.IsPrepared;
        characterSpell.IsFavorite = request.IsFavorite;
        characterSpell.Notes = NormalizeRequired(request.Notes);
    }

    private static void ApplyCharacterSpell(CharacterSpell characterSpell, CharacterSpellUpdateRequest request)
    {
        characterSpell.IsKnown = request.IsKnown;
        characterSpell.IsPrepared = request.IsPrepared;
        characterSpell.IsFavorite = request.IsFavorite;
        characterSpell.Notes = NormalizeRequired(request.Notes);
    }

    private static void ApplyCharacterFeature(CharacterFeature characterFeature, CharacterFeatureRequest request)
    {
        characterFeature.FeatureId = request.FeatureId;
        characterFeature.CustomName = NormalizeRequired(request.CustomName);
        characterFeature.CustomDescription = NormalizeRequired(request.CustomDescription);
        characterFeature.MaxUses = request.MaxUses;
        characterFeature.CurrentUses = request.CurrentUses;
        characterFeature.RecoveryType = request.RecoveryType;
        characterFeature.Notes = NormalizeRequired(request.Notes);
    }

    private static void ApplyAttack(CharacterAttack attack, CharacterAttackRequest request)
    {
        attack.Name = request.Name.Trim();
        attack.AttackBonus = request.AttackBonus;
        attack.Damage = NormalizeRequired(request.Damage);
        attack.DamageType = NormalizeRequired(request.DamageType);
        attack.Range = NormalizeRequired(request.Range);
        attack.UsesAttribute = request.UsesAttribute;
        attack.Notes = NormalizeRequired(request.Notes);
    }

    private static void ApplySavingThrow(Character character, SavingThrowRequest request)
    {
        switch (request.Attribute)
        {
            case AbilityType.Strength:
                character.StrengthSaveProficient = request.IsProficient;
                character.StrengthSaveCustomBonus = request.CustomBonus;
                break;
            case AbilityType.Dexterity:
                character.DexteritySaveProficient = request.IsProficient;
                character.DexteritySaveCustomBonus = request.CustomBonus;
                break;
            case AbilityType.Constitution:
                character.ConstitutionSaveProficient = request.IsProficient;
                character.ConstitutionSaveCustomBonus = request.CustomBonus;
                break;
            case AbilityType.Intelligence:
                character.IntelligenceSaveProficient = request.IsProficient;
                character.IntelligenceSaveCustomBonus = request.CustomBonus;
                break;
            case AbilityType.Wisdom:
                character.WisdomSaveProficient = request.IsProficient;
                character.WisdomSaveCustomBonus = request.CustomBonus;
                break;
            case AbilityType.Charisma:
                character.CharismaSaveProficient = request.IsProficient;
                character.CharismaSaveCustomBonus = request.CustomBonus;
                break;
            default:
                break;
        }
    }

    private async Task EnsureSkillsAsync(Character character, CancellationToken cancellationToken)
    {
        var missingDefinitions = SkillDefinitions
            .Where(definition => character.Skills.All(skill => skill.SkillType != definition.SkillType))
            .ToList();

        if (missingDefinitions.Count == 0)
        {
            return;
        }

        var newSkills = missingDefinitions
            .Select(definition => new CharacterSkill
            {
                CharacterId = character.Id,
                SkillType = definition.SkillType,
                BaseAttribute = definition.BaseAttribute
            })
            .ToList();

        dbContext.CharacterSkills.AddRange(newSkills);
        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var skill in newSkills)
        {
            if (character.Skills.All(existing => existing.SkillType != skill.SkillType))
            {
                character.Skills.Add(skill);
            }
        }
    }

    private async Task EnsureConditionsAsync(Character character, CancellationToken cancellationToken)
    {
        var missingDefinitions = ConditionDefinitions
            .Where(definition => character.Conditions.All(condition => condition.ConditionType != definition.ConditionType))
            .ToList();

        if (missingDefinitions.Count == 0)
        {
            return;
        }

        var newConditions = missingDefinitions
            .Select(definition => new CharacterCondition
            {
                CharacterId = character.Id,
                ConditionType = definition.ConditionType,
                Name = definition.Name,
                Description = definition.Description
            })
            .ToList();

        dbContext.CharacterConditions.AddRange(newConditions);
        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var condition in newConditions)
        {
            if (character.Conditions.All(existing => existing.ConditionType != condition.ConditionType))
            {
                character.Conditions.Add(condition);
            }
        }
    }

    private static SkillDefinition GetSkillDefinition(SkillType skillType)
        => SkillDefinitions.Single(definition => definition.SkillType == skillType);

    private static ConditionDefinition GetConditionDefinition(ConditionType conditionType)
        => ConditionDefinitions.Single(definition => definition.ConditionType == conditionType);

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string NormalizeRequired(string value)
        => value?.Trim() ?? string.Empty;

    private sealed record ValidationError(
        string Message,
        ServiceErrorType Type = ServiceErrorType.Validation);

    private sealed record SkillDefinition(
        SkillType SkillType,
        string Label,
        AbilityType BaseAttribute);

    private sealed record ConditionDefinition(
        ConditionType ConditionType,
        string Name,
        string Description);
}
