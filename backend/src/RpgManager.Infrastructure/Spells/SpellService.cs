using Microsoft.EntityFrameworkCore;
using RpgManager.Application.Common;
using RpgManager.Application.Spells;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.Spells;

public sealed class SpellService(AppDbContext dbContext) : ISpellService
{
    public async Task<PagedResponse<SpellResponse>> GetVisibleAsync(
        Guid userId,
        SpellFilters filters,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, filters.Page);
        var pageSize = Math.Clamp(filters.PageSize, 1, 50);

        var query = VisibleQuery(userId).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filters.Name))
        {
            var name = filters.Name.Trim().ToLower();
            query = query.Where(spell =>
                spell.Name.ToLower().Contains(name) ||
                spell.EnglishName.ToLower().Contains(name));
        }

        if (filters.Level.HasValue)
        {
            query = query.Where(spell => spell.Level == filters.Level.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.School))
        {
            var school = filters.School.Trim().ToLower();
            query = query.Where(spell => spell.School.ToLower().Contains(school));
        }

        if (!string.IsNullOrWhiteSpace(filters.Class))
        {
            var className = filters.Class.Trim().ToLower();
            query = query.Where(spell => spell.AvailableClasses.ToLower().Contains(className));
        }

        if (filters.IsConcentration.HasValue)
        {
            query = query.Where(spell => spell.IsConcentration == filters.IsConcentration.Value);
        }

        if (filters.IsRitual.HasValue)
        {
            query = query.Where(spell => spell.IsRitual == filters.IsRitual.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Source))
        {
            var source = filters.Source.Trim().ToLower();
            query = query.Where(spell => spell.Source.ToLower().Contains(source));
        }

        if (filters.IsHomebrew.HasValue)
        {
            query = query.Where(spell => spell.IsHomebrew == filters.IsHomebrew.Value);
        }

        if (filters.Visibility.HasValue)
        {
            query = query.Where(spell => spell.Visibility == filters.Visibility.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var masterCampaignIds = await dbContext.CampaignMembers
            .AsNoTracking()
            .Where(member => member.UserId == userId && member.Role == CampaignRole.Master)
            .Select(member => member.CampaignId)
            .ToListAsync(cancellationToken);

        var spells = await query
            .OrderBy(spell => spell.Level)
            .ThenBy(spell => spell.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var responses = spells
            .Select(spell => ToResponse(spell, userId, masterCampaignIds))
            .ToList();

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        return new PagedResponse<SpellResponse>(responses, page, pageSize, totalItems, totalPages);
    }

    public async Task<ServiceResult<SpellResponse>> GetByIdAsync(
        Guid userId,
        Guid spellId,
        CancellationToken cancellationToken)
    {
        var spell = await GetVisibleSpellAsync(userId, spellId, cancellationToken);
        if (spell is null)
        {
            return ServiceResult<SpellResponse>.Failure("Magia não encontrada.", ServiceErrorType.NotFound);
        }

        return ServiceResult<SpellResponse>.Success(await ToResponseAsync(spell, userId, cancellationToken));
    }

    public async Task<ServiceResult<SpellResponse>> CreateAsync(
        Guid userId,
        SpellRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await ValidateAsync(userId, request, cancellationToken);
        if (validation is not null)
        {
            return ServiceResult<SpellResponse>.Failure(validation.Error, validation.ErrorType);
        }

        var spell = new Spell
        {
            CreatedByUserId = userId
        };

        Apply(spell, request);
        dbContext.Spells.Add(spell);
        await dbContext.SaveChangesAsync(cancellationToken);

        var created = await GetSpellWithRelationsAsync(spell.Id, cancellationToken);
        return ServiceResult<SpellResponse>.Success(await ToResponseAsync(created!, userId, cancellationToken));
    }

    public async Task<ServiceResult<SpellResponse>> UpdateAsync(
        Guid userId,
        Guid spellId,
        SpellRequest request,
        CancellationToken cancellationToken)
    {
        var spell = await GetSpellWithRelationsAsync(spellId, cancellationToken);
        if (spell is null)
        {
            return ServiceResult<SpellResponse>.Failure("Magia não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await CanEditAsync(userId, spell, cancellationToken))
        {
            return ServiceResult<SpellResponse>.Failure("Você não pode editar esta magia.", ServiceErrorType.Forbidden);
        }

        var validation = await ValidateAsync(userId, request, cancellationToken);
        if (validation is not null)
        {
            return ServiceResult<SpellResponse>.Failure(validation.Error, validation.ErrorType);
        }

        Apply(spell, request);
        spell.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var updated = await GetSpellWithRelationsAsync(spell.Id, cancellationToken);
        return ServiceResult<SpellResponse>.Success(await ToResponseAsync(updated!, userId, cancellationToken));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid userId,
        Guid spellId,
        CancellationToken cancellationToken)
    {
        var spell = await GetSpellWithRelationsAsync(spellId, cancellationToken);
        if (spell is null)
        {
            return ServiceResult<bool>.Failure("Magia não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await CanEditAsync(userId, spell, cancellationToken))
        {
            return ServiceResult<bool>.Failure("Você não pode excluir esta magia.", ServiceErrorType.Forbidden);
        }

        dbContext.Spells.Remove(spell);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true);
    }

    private IQueryable<Spell> VisibleQuery(Guid userId)
    {
        return dbContext.Spells
            .Include(spell => spell.CreatedByUser)
            .Include(spell => spell.Campaign)
            .Where(spell =>
                spell.Visibility == SpellVisibility.LocalPublic ||
                (spell.Visibility == SpellVisibility.Private && spell.CreatedByUserId == userId) ||
                (spell.Visibility == SpellVisibility.Campaign &&
                    spell.CampaignId.HasValue &&
                    dbContext.CampaignMembers.Any(member =>
                        member.CampaignId == spell.CampaignId.Value &&
                        member.UserId == userId)));
    }

    private async Task<Spell?> GetVisibleSpellAsync(Guid userId, Guid spellId, CancellationToken cancellationToken)
    {
        return await VisibleQuery(userId)
            .SingleOrDefaultAsync(spell => spell.Id == spellId, cancellationToken);
    }

    private async Task<Spell?> GetSpellWithRelationsAsync(Guid spellId, CancellationToken cancellationToken)
    {
        return await dbContext.Spells
            .Include(spell => spell.CreatedByUser)
            .Include(spell => spell.Campaign)
            .SingleOrDefaultAsync(spell => spell.Id == spellId, cancellationToken);
    }

    private async Task<bool> CanEditAsync(Guid userId, Spell spell, CancellationToken cancellationToken)
    {
        if (spell.Visibility == SpellVisibility.Private)
        {
            return spell.CreatedByUserId == userId;
        }

        if (spell.Visibility == SpellVisibility.Campaign && spell.CampaignId.HasValue)
        {
            return await IsCampaignMasterAsync(userId, spell.CampaignId.Value, cancellationToken);
        }

        return spell.CreatedByUserId == userId;
    }

    private async Task<ValidationResult?> ValidateAsync(Guid userId, SpellRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new ValidationResult("Nome é obrigatório.");
        }

        if (request.Name.Trim().Length > 180)
        {
            return new ValidationResult("Nome deve ter no máximo 180 caracteres.");
        }

        if (request.EnglishName.Trim().Length > 180)
        {
            return new ValidationResult("Nome em inglês deve ter no máximo 180 caracteres.");
        }

        if (request.Level is < 0 or > 9)
        {
            return new ValidationResult("Nível deve estar entre 0 e 9.");
        }

        if (string.IsNullOrWhiteSpace(request.School))
        {
            return new ValidationResult("Escola é obrigatória.");
        }

        if (request.Visibility == SpellVisibility.Campaign)
        {
            if (!request.CampaignId.HasValue)
            {
                return new ValidationResult("Campanha é obrigatória para magia de campanha.");
            }

            if (!await IsCampaignMasterAsync(userId, request.CampaignId.Value, cancellationToken))
            {
                return new ValidationResult("Apenas Mestre pode criar magia de campanha.", ServiceErrorType.Forbidden);
            }
        }

        if (request.Visibility != SpellVisibility.Campaign && request.CampaignId.HasValue)
        {
            return new ValidationResult("Campanha só pode ser vinculada em visibilidade Campaign.");
        }

        return null;
    }

    private async Task<bool> IsCampaignMasterAsync(Guid userId, Guid campaignId, CancellationToken cancellationToken)
    {
        return await dbContext.CampaignMembers.AnyAsync(member =>
            member.CampaignId == campaignId &&
            member.UserId == userId &&
            member.Role == CampaignRole.Master,
            cancellationToken);
    }

    private static void Apply(Spell spell, SpellRequest request)
    {
        spell.Name = request.Name.Trim();
        spell.EnglishName = Normalize(request.EnglishName);
        spell.Level = request.Level;
        spell.School = request.School.Trim();
        spell.CastingTime = Normalize(request.CastingTime);
        spell.Range = Normalize(request.Range);
        spell.Components = Normalize(request.Components);
        spell.Material = Normalize(request.Material);
        spell.Duration = Normalize(request.Duration);
        spell.IsConcentration = request.IsConcentration;
        spell.IsRitual = request.IsRitual;
        spell.Description = Normalize(request.Description);
        spell.HigherLevelDescription = Normalize(request.HigherLevelDescription);
        spell.AvailableClasses = Normalize(request.AvailableClasses);
        spell.Source = Normalize(request.Source);
        spell.IsHomebrew = request.IsHomebrew;
        spell.Visibility = request.Visibility;
        spell.CampaignId = request.Visibility == SpellVisibility.Campaign ? request.CampaignId : null;
    }

    private async Task<SpellResponse> ToResponseAsync(Spell spell, Guid userId, CancellationToken cancellationToken)
    {
        var masterCampaignIds = await dbContext.CampaignMembers
            .AsNoTracking()
            .Where(member => member.UserId == userId && member.Role == CampaignRole.Master)
            .Select(member => member.CampaignId)
            .ToListAsync(cancellationToken);

        return ToResponse(spell, userId, masterCampaignIds);
    }

    private static SpellResponse ToResponse(Spell spell, Guid userId, IReadOnlyCollection<Guid> masterCampaignIds)
    {
        var canEdit = spell.Visibility switch
        {
            SpellVisibility.Private => spell.CreatedByUserId == userId,
            SpellVisibility.Campaign => spell.CampaignId.HasValue && masterCampaignIds.Contains(spell.CampaignId.Value),
            SpellVisibility.LocalPublic => spell.CreatedByUserId == userId,
            _ => false
        };

        return new SpellResponse(
            spell.Id,
            spell.Name,
            spell.EnglishName,
            spell.Level,
            spell.School,
            spell.CastingTime,
            spell.Range,
            spell.Components,
            spell.Material,
            spell.Duration,
            spell.IsConcentration,
            spell.IsRitual,
            spell.Description,
            spell.HigherLevelDescription,
            spell.AvailableClasses,
            spell.Source,
            spell.IsHomebrew,
            spell.CreatedByUserId,
            spell.CreatedByUser.Name,
            spell.Visibility,
            spell.CampaignId,
            spell.Campaign?.Name,
            spell.CreatedAt,
            spell.UpdatedAt,
            canEdit);
    }

    private static string Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private sealed record ValidationResult(
        string Error,
        ServiceErrorType ErrorType = ServiceErrorType.Validation);
}
