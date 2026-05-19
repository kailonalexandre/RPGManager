using Microsoft.EntityFrameworkCore;
using RpgManager.Application.Common;
using RpgManager.Application.Features;
using RpgManager.Application.Spells;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.Features;

public sealed class FeatureService(AppDbContext dbContext) : IFeatureService
{
    public async Task<PagedResponse<FeatureResponse>> GetVisibleAsync(
        Guid userId,
        FeatureFilters filters,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, filters.Page);
        var pageSize = Math.Clamp(filters.PageSize, 1, 50);
        var query = VisibleQuery(userId).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filters.Name))
        {
            var name = filters.Name.Trim().ToLower();
            query = query.Where(feature => feature.Name.ToLower().Contains(name));
        }

        if (filters.Type.HasValue)
        {
            query = query.Where(feature => feature.Type == filters.Type.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.Source))
        {
            var source = filters.Source.Trim().ToLower();
            query = query.Where(feature => feature.Source.ToLower().Contains(source));
        }

        if (filters.IsHomebrew.HasValue)
        {
            query = query.Where(feature => feature.IsHomebrew == filters.IsHomebrew.Value);
        }

        if (filters.Visibility.HasValue)
        {
            query = query.Where(feature => feature.Visibility == filters.Visibility.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var masterCampaignIds = await GetMasterCampaignIdsAsync(userId, cancellationToken);
        var features = await query
            .OrderBy(feature => feature.Type)
            .ThenBy(feature => feature.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var responses = features
            .Select(feature => ToResponse(feature, userId, masterCampaignIds))
            .ToList();

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        return new PagedResponse<FeatureResponse>(responses, page, pageSize, totalItems, totalPages);
    }

    public async Task<ServiceResult<FeatureResponse>> GetByIdAsync(
        Guid userId,
        Guid featureId,
        CancellationToken cancellationToken)
    {
        var feature = await VisibleQuery(userId)
            .SingleOrDefaultAsync(item => item.Id == featureId, cancellationToken);
        if (feature is null)
        {
            return ServiceResult<FeatureResponse>.Failure("Talento/característica não encontrado.", ServiceErrorType.NotFound);
        }

        return ServiceResult<FeatureResponse>.Success(await ToResponseAsync(feature, userId, cancellationToken));
    }

    public async Task<ServiceResult<FeatureResponse>> CreateAsync(
        Guid userId,
        FeatureRequest request,
        CancellationToken cancellationToken)
    {
        var validation = await ValidateAsync(userId, request, cancellationToken);
        if (validation is not null)
        {
            return ServiceResult<FeatureResponse>.Failure(validation.Error, validation.ErrorType);
        }

        var feature = new Feature
        {
            CreatedByUserId = userId
        };
        Apply(feature, request);

        dbContext.Features.Add(feature);
        await dbContext.SaveChangesAsync(cancellationToken);

        var created = await GetFeatureWithRelationsAsync(feature.Id, cancellationToken);
        return ServiceResult<FeatureResponse>.Success(await ToResponseAsync(created!, userId, cancellationToken));
    }

    public async Task<ServiceResult<FeatureResponse>> UpdateAsync(
        Guid userId,
        Guid featureId,
        FeatureRequest request,
        CancellationToken cancellationToken)
    {
        var feature = await GetFeatureWithRelationsAsync(featureId, cancellationToken);
        if (feature is null)
        {
            return ServiceResult<FeatureResponse>.Failure("Talento/característica não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanEditAsync(userId, feature, cancellationToken))
        {
            return ServiceResult<FeatureResponse>.Failure("Você não pode editar este conteúdo.", ServiceErrorType.Forbidden);
        }

        var validation = await ValidateAsync(userId, request, cancellationToken);
        if (validation is not null)
        {
            return ServiceResult<FeatureResponse>.Failure(validation.Error, validation.ErrorType);
        }

        Apply(feature, request);
        feature.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var updated = await GetFeatureWithRelationsAsync(feature.Id, cancellationToken);
        return ServiceResult<FeatureResponse>.Success(await ToResponseAsync(updated!, userId, cancellationToken));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid userId,
        Guid featureId,
        CancellationToken cancellationToken)
    {
        var feature = await GetFeatureWithRelationsAsync(featureId, cancellationToken);
        if (feature is null)
        {
            return ServiceResult<bool>.Failure("Talento/característica não encontrado.", ServiceErrorType.NotFound);
        }

        if (!await CanEditAsync(userId, feature, cancellationToken))
        {
            return ServiceResult<bool>.Failure("Você não pode excluir este conteúdo.", ServiceErrorType.Forbidden);
        }

        dbContext.Features.Remove(feature);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true);
    }

    private IQueryable<Feature> VisibleQuery(Guid userId)
    {
        return dbContext.Features
            .Include(feature => feature.CreatedByUser)
            .Include(feature => feature.Campaign)
            .Where(feature =>
                feature.Visibility == SpellVisibility.LocalPublic ||
                (feature.Visibility == SpellVisibility.Private && feature.CreatedByUserId == userId) ||
                (feature.Visibility == SpellVisibility.Campaign &&
                    feature.CampaignId.HasValue &&
                    dbContext.CampaignMembers.Any(member =>
                        member.CampaignId == feature.CampaignId.Value &&
                        member.UserId == userId)));
    }

    private async Task<Feature?> GetFeatureWithRelationsAsync(Guid featureId, CancellationToken cancellationToken)
    {
        return await dbContext.Features
            .Include(feature => feature.CreatedByUser)
            .Include(feature => feature.Campaign)
            .SingleOrDefaultAsync(feature => feature.Id == featureId, cancellationToken);
    }

    private async Task<bool> CanEditAsync(Guid userId, Feature feature, CancellationToken cancellationToken)
    {
        if (feature.Visibility == SpellVisibility.Private)
        {
            return feature.CreatedByUserId == userId;
        }

        if (feature.Visibility == SpellVisibility.Campaign && feature.CampaignId.HasValue)
        {
            return await IsCampaignMasterAsync(userId, feature.CampaignId.Value, cancellationToken);
        }

        return feature.CreatedByUserId == userId;
    }

    private async Task<ValidationResult?> ValidateAsync(Guid userId, FeatureRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new ValidationResult("Nome é obrigatório.");
        }

        if (request.Name.Trim().Length > 180)
        {
            return new ValidationResult("Nome deve ter no máximo 180 caracteres.");
        }

        if ((request.Description?.Length ?? 0) > 10000 ||
            (request.Source?.Length ?? 0) > 160 ||
            (request.Prerequisites?.Length ?? 0) > 1000)
        {
            return new ValidationResult("Campos excedem o limite permitido.");
        }

        if (request.Visibility == SpellVisibility.Campaign)
        {
            if (!request.CampaignId.HasValue)
            {
                return new ValidationResult("Campanha é obrigatória para conteúdo de campanha.");
            }

            if (!await IsCampaignMasterAsync(userId, request.CampaignId.Value, cancellationToken))
            {
                return new ValidationResult("Apenas Mestre pode criar conteúdo de campanha.", ServiceErrorType.Forbidden);
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

    private async Task<IReadOnlyList<Guid>> GetMasterCampaignIdsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.CampaignMembers
            .AsNoTracking()
            .Where(member => member.UserId == userId && member.Role == CampaignRole.Master)
            .Select(member => member.CampaignId)
            .ToListAsync(cancellationToken);
    }

    private async Task<FeatureResponse> ToResponseAsync(Feature feature, Guid userId, CancellationToken cancellationToken)
    {
        var masterCampaignIds = await GetMasterCampaignIdsAsync(userId, cancellationToken);
        return ToResponse(feature, userId, masterCampaignIds);
    }

    private static FeatureResponse ToResponse(Feature feature, Guid userId, IReadOnlyCollection<Guid> masterCampaignIds)
    {
        var canEdit = feature.Visibility switch
        {
            SpellVisibility.Private => feature.CreatedByUserId == userId,
            SpellVisibility.Campaign => feature.CampaignId.HasValue && masterCampaignIds.Contains(feature.CampaignId.Value),
            SpellVisibility.LocalPublic => feature.CreatedByUserId == userId,
            _ => false
        };

        return new FeatureResponse(
            feature.Id,
            feature.Name,
            feature.Type,
            feature.Description,
            feature.Source,
            feature.Prerequisites,
            feature.IsHomebrew,
            feature.CreatedByUserId,
            feature.CreatedByUser.Name,
            feature.Visibility,
            feature.CampaignId,
            feature.Campaign?.Name,
            feature.CreatedAt,
            feature.UpdatedAt,
            canEdit);
    }

    private static void Apply(Feature feature, FeatureRequest request)
    {
        feature.Name = request.Name.Trim();
        feature.Type = request.Type;
        feature.Description = Normalize(request.Description);
        feature.Source = Normalize(request.Source);
        feature.Prerequisites = Normalize(request.Prerequisites);
        feature.IsHomebrew = request.IsHomebrew;
        feature.Visibility = request.Visibility;
        feature.CampaignId = request.Visibility == SpellVisibility.Campaign ? request.CampaignId : null;
    }

    private static string Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private sealed record ValidationResult(
        string Error,
        ServiceErrorType ErrorType = ServiceErrorType.Validation);
}
