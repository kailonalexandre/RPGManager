using Microsoft.EntityFrameworkCore;
using RpgManager.Application.Common;
using RpgManager.Application.Npcs;
using RpgManager.Application.Permissions;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.Npcs;

public sealed class NpcService(
    AppDbContext dbContext,
    ICampaignPermissionService campaignPermissionService) : INpcService
{
    public async Task<ServiceResult<IReadOnlyList<NpcResponse>>> GetAsync(
        Guid userId,
        Guid campaignId,
        NpcQuery query,
        CancellationToken cancellationToken)
    {
        if (!await campaignPermissionService.CanViewCampaignAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<NpcResponse>>.Failure(
                "Você não participa desta campanha.",
                ServiceErrorType.Forbidden);
        }

        var isMaster = await campaignPermissionService.IsCampaignMasterAsync(campaignId, userId, cancellationToken);
        var npcsQuery = dbContext.Npcs
            .AsNoTracking()
            .Include(npc => npc.CreatedByUser)
            .Where(npc => npc.CampaignId == campaignId);

        if (!isMaster)
        {
            npcsQuery = npcsQuery.Where(npc => npc.Visibility == Visibility.PublicToPlayers);
        }

        npcsQuery = ApplyFilters(npcsQuery, query);

        var npcs = await npcsQuery
            .OrderByDescending(npc => npc.IsImportant)
            .ThenBy(npc => npc.Name)
            .ToListAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<NpcResponse>>.Success(
            npcs.Select(npc => ToResponse(npc, isMaster)).ToList());
    }

    public async Task<ServiceResult<NpcResponse>> GetByIdAsync(
        Guid userId,
        Guid campaignId,
        Guid npcId,
        CancellationToken cancellationToken)
    {
        if (!await campaignPermissionService.CanViewCampaignAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<NpcResponse>.Failure(
                "Você não participa desta campanha.",
                ServiceErrorType.Forbidden);
        }

        var npc = await GetNpcAsync(campaignId, npcId, cancellationToken);
        if (npc is null)
        {
            return ServiceResult<NpcResponse>.Failure("NPC não encontrado.", ServiceErrorType.NotFound);
        }

        var isMaster = await campaignPermissionService.IsCampaignMasterAsync(campaignId, userId, cancellationToken);
        if (!isMaster && npc.Visibility != Visibility.PublicToPlayers)
        {
            return ServiceResult<NpcResponse>.Failure("Você não pode visualizar este NPC.", ServiceErrorType.Forbidden);
        }

        return ServiceResult<NpcResponse>.Success(ToResponse(npc, isMaster));
    }

    public async Task<ServiceResult<NpcResponse>> CreateAsync(
        Guid userId,
        Guid campaignId,
        NpcRequest request,
        CancellationToken cancellationToken)
    {
        if (!await campaignPermissionService.IsCampaignMasterAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<NpcResponse>.Failure("Apenas Mestre pode criar NPCs.", ServiceErrorType.Forbidden);
        }

        var validationError = Validate(request);
        if (validationError is not null)
        {
            return ServiceResult<NpcResponse>.Failure(validationError);
        }

        var npc = new Npc
        {
            CampaignId = campaignId,
            CreatedByUserId = userId
        };

        ApplyRequest(npc, request);
        dbContext.Npcs.Add(npc);
        await dbContext.SaveChangesAsync(cancellationToken);

        var created = await GetNpcAsync(campaignId, npc.Id, cancellationToken);
        return ServiceResult<NpcResponse>.Success(ToResponse(created!, isMaster: true));
    }

    public async Task<ServiceResult<NpcResponse>> UpdateAsync(
        Guid userId,
        Guid campaignId,
        Guid npcId,
        NpcRequest request,
        CancellationToken cancellationToken)
    {
        if (!await campaignPermissionService.IsCampaignMasterAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<NpcResponse>.Failure("Apenas Mestre pode editar NPCs.", ServiceErrorType.Forbidden);
        }

        var npc = await GetNpcAsync(campaignId, npcId, cancellationToken);
        if (npc is null)
        {
            return ServiceResult<NpcResponse>.Failure("NPC não encontrado.", ServiceErrorType.NotFound);
        }

        var validationError = Validate(request);
        if (validationError is not null)
        {
            return ServiceResult<NpcResponse>.Failure(validationError);
        }

        ApplyRequest(npc, request);
        npc.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<NpcResponse>.Success(ToResponse(npc, isMaster: true));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid userId,
        Guid campaignId,
        Guid npcId,
        CancellationToken cancellationToken)
    {
        if (!await campaignPermissionService.IsCampaignMasterAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<bool>.Failure("Apenas Mestre pode excluir NPCs.", ServiceErrorType.Forbidden);
        }

        var npc = await GetNpcAsync(campaignId, npcId, cancellationToken);
        if (npc is null)
        {
            return ServiceResult<bool>.Failure("NPC não encontrado.", ServiceErrorType.NotFound);
        }

        dbContext.Npcs.Remove(npc);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true);
    }

    private async Task<Npc?> GetNpcAsync(Guid campaignId, Guid npcId, CancellationToken cancellationToken)
        => await dbContext.Npcs
            .Include(npc => npc.CreatedByUser)
            .SingleOrDefaultAsync(npc => npc.CampaignId == campaignId && npc.Id == npcId, cancellationToken);

    private static IQueryable<Npc> ApplyFilters(IQueryable<Npc> query, NpcQuery filters)
    {
        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim().ToLower();
            query = query.Where(npc =>
                npc.Name.ToLower().Contains(search) ||
                npc.Alias.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filters.Tag))
        {
            var tag = filters.Tag.Trim().ToLower();
            query = query.Where(npc => npc.Tags.ToLower().Contains(tag));
        }

        if (!string.IsNullOrWhiteSpace(filters.Location))
        {
            var location = filters.Location.Trim().ToLower();
            query = query.Where(npc => npc.Location.ToLower().Contains(location));
        }

        if (!string.IsNullOrWhiteSpace(filters.Faction))
        {
            var faction = filters.Faction.Trim().ToLower();
            query = query.Where(npc => npc.Faction.ToLower().Contains(faction));
        }

        if (filters.IsImportant.HasValue)
        {
            query = query.Where(npc => npc.IsImportant == filters.IsImportant.Value);
        }

        if (filters.IsAlive.HasValue)
        {
            query = query.Where(npc => npc.IsAlive == filters.IsAlive.Value);
        }

        if (filters.Visibility.HasValue)
        {
            query = query.Where(npc => npc.Visibility == filters.Visibility.Value);
        }

        return query;
    }

    private static string? Validate(NpcRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "Nome é obrigatório.";
        }

        if (request.Name.Trim().Length > 180)
        {
            return "Nome deve ter no máximo 180 caracteres.";
        }

        if (request.Alias.Length > 180 ||
            request.Race.Length > 120 ||
            request.Occupation.Length > 180 ||
            request.Location.Length > 180 ||
            request.Faction.Length > 180)
        {
            return "Campos curtos do NPC excedem o tamanho máximo.";
        }

        if (request.Personality.Length > 2000 ||
            request.Appearance.Length > 2000 ||
            request.Motivation.Length > 2000 ||
            request.Secrets.Length > 4000 ||
            request.Notes.Length > 4000 ||
            request.StatBlockJson.Length > 8000 ||
            request.Tags.Length > 500)
        {
            return "Campos longos do NPC excedem o tamanho máximo.";
        }

        return null;
    }

    private static void ApplyRequest(Npc npc, NpcRequest request)
    {
        npc.Name = request.Name.Trim();
        npc.Alias = request.Alias.Trim();
        npc.Race = request.Race.Trim();
        npc.Occupation = request.Occupation.Trim();
        npc.Location = request.Location.Trim();
        npc.Faction = request.Faction.Trim();
        npc.Personality = request.Personality.Trim();
        npc.Appearance = request.Appearance.Trim();
        npc.Motivation = request.Motivation.Trim();
        npc.Secrets = request.Secrets.Trim();
        npc.Notes = request.Notes.Trim();
        npc.StatBlockJson = request.StatBlockJson.Trim();
        npc.Tags = request.Tags.Trim();
        npc.IsImportant = request.IsImportant;
        npc.IsAlive = request.IsAlive;
        npc.Visibility = request.Visibility;
    }

    private static NpcResponse ToResponse(Npc npc, bool isMaster)
        => new(
            npc.Id,
            npc.CampaignId,
            npc.CreatedByUserId,
            npc.CreatedByUser.Name,
            npc.Name,
            npc.Alias,
            npc.Race,
            npc.Occupation,
            npc.Location,
            npc.Faction,
            npc.Personality,
            npc.Appearance,
            npc.Motivation,
            isMaster ? npc.Secrets : null,
            npc.Notes,
            npc.StatBlockJson,
            npc.Tags,
            npc.IsImportant,
            npc.IsAlive,
            npc.Visibility,
            npc.CreatedAt,
            npc.UpdatedAt,
            isMaster);
}
