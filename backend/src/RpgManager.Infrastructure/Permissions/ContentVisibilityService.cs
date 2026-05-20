using Microsoft.EntityFrameworkCore;
using RpgManager.Application.Permissions;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.Permissions;

public sealed class ContentVisibilityService(AppDbContext dbContext) : IContentVisibilityService
{
    public async Task<bool> CanViewContentAsync(Guid contentId, Guid userId, CancellationToken cancellationToken)
        => await CanViewSpellAsync(contentId, userId, cancellationToken) ||
            await CanViewFeatureAsync(contentId, userId, cancellationToken);

    public async Task<bool> CanEditContentAsync(Guid contentId, Guid userId, CancellationToken cancellationToken)
        => await CanEditSpellAsync(contentId, userId, cancellationToken) ||
            await CanEditFeatureAsync(contentId, userId, cancellationToken);

    public async Task<bool> CanViewSpellAsync(Guid spellId, Guid userId, CancellationToken cancellationToken)
    {
        var spell = await dbContext.Spells
            .AsNoTracking()
            .Where(item => item.Id == spellId)
            .Select(item => new { item.CreatedByUserId, item.Visibility, item.CampaignId })
            .SingleOrDefaultAsync(cancellationToken);

        return spell is not null && await CanViewAsync(
            spell.CreatedByUserId,
            spell.Visibility,
            spell.CampaignId,
            userId,
            cancellationToken);
    }

    public async Task<bool> CanEditSpellAsync(Guid spellId, Guid userId, CancellationToken cancellationToken)
    {
        var spell = await dbContext.Spells
            .AsNoTracking()
            .Where(item => item.Id == spellId)
            .Select(item => new { item.CreatedByUserId, item.Visibility, item.CampaignId })
            .SingleOrDefaultAsync(cancellationToken);

        return spell is not null && await CanEditAsync(
            spell.CreatedByUserId,
            spell.Visibility,
            spell.CampaignId,
            userId,
            cancellationToken);
    }

    public async Task<bool> CanViewFeatureAsync(Guid featureId, Guid userId, CancellationToken cancellationToken)
    {
        var feature = await dbContext.Features
            .AsNoTracking()
            .Where(item => item.Id == featureId)
            .Select(item => new { item.CreatedByUserId, item.Visibility, item.CampaignId })
            .SingleOrDefaultAsync(cancellationToken);

        return feature is not null && await CanViewAsync(
            feature.CreatedByUserId,
            feature.Visibility,
            feature.CampaignId,
            userId,
            cancellationToken);
    }

    public async Task<bool> CanEditFeatureAsync(Guid featureId, Guid userId, CancellationToken cancellationToken)
    {
        var feature = await dbContext.Features
            .AsNoTracking()
            .Where(item => item.Id == featureId)
            .Select(item => new { item.CreatedByUserId, item.Visibility, item.CampaignId })
            .SingleOrDefaultAsync(cancellationToken);

        return feature is not null && await CanEditAsync(
            feature.CreatedByUserId,
            feature.Visibility,
            feature.CampaignId,
            userId,
            cancellationToken);
    }

    private async Task<bool> CanViewAsync(
        Guid createdByUserId,
        SpellVisibility visibility,
        Guid? campaignId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (visibility == SpellVisibility.LocalPublic)
        {
            return true;
        }

        if (visibility == SpellVisibility.Private)
        {
            return createdByUserId == userId;
        }

        return campaignId.HasValue &&
            await dbContext.CampaignMembers.AnyAsync(
                member => member.CampaignId == campaignId.Value && member.UserId == userId,
                cancellationToken);
    }

    private async Task<bool> CanEditAsync(
        Guid createdByUserId,
        SpellVisibility visibility,
        Guid? campaignId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (visibility == SpellVisibility.Private || visibility == SpellVisibility.LocalPublic)
        {
            return createdByUserId == userId;
        }

        return campaignId.HasValue &&
            await dbContext.CampaignMembers.AnyAsync(
                member => member.CampaignId == campaignId.Value &&
                    member.UserId == userId &&
                    member.Role == CampaignRole.Master,
                cancellationToken);
    }
}
