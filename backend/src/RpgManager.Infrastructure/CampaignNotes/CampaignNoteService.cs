using Microsoft.EntityFrameworkCore;
using RpgManager.Application.CampaignNotes;
using RpgManager.Application.Common;
using RpgManager.Application.Permissions;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.CampaignNotes;

public sealed class CampaignNoteService(
    AppDbContext dbContext,
    ICampaignPermissionService campaignPermissionService) : ICampaignNoteService
{
    public async Task<ServiceResult<IReadOnlyList<CampaignNoteResponse>>> GetAsync(
        Guid userId,
        Guid campaignId,
        CampaignNoteQuery query,
        CancellationToken cancellationToken)
    {
        if (!await campaignPermissionService.CanViewCampaignAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CampaignNoteResponse>>.Failure(
                "Você não participa desta campanha.",
                ServiceErrorType.Forbidden);
        }

        var isMaster = await campaignPermissionService.IsCampaignMasterAsync(campaignId, userId, cancellationToken);
        var notesQuery = dbContext.CampaignNotes
            .AsNoTracking()
            .Include(note => note.OwnerUser)
            .Where(note => note.CampaignId == campaignId);

        notesQuery = ApplyVisibility(notesQuery, userId, isMaster);
        notesQuery = ApplyFilters(notesQuery, query);

        var notes = await notesQuery
            .OrderByDescending(note => note.UpdatedAt ?? note.CreatedAt)
            .ThenBy(note => note.Title)
            .ToListAsync(cancellationToken);

        return ServiceResult<IReadOnlyList<CampaignNoteResponse>>.Success(
            notes.Select(note => ToResponse(note, userId, isMaster)).ToList());
    }

    public async Task<ServiceResult<CampaignNoteResponse>> GetByIdAsync(
        Guid userId,
        Guid campaignId,
        Guid noteId,
        CancellationToken cancellationToken)
    {
        var note = await GetNoteAsync(campaignId, noteId, cancellationToken);
        if (note is null)
        {
            return ServiceResult<CampaignNoteResponse>.Failure("Nota não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await campaignPermissionService.CanViewCampaignAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<CampaignNoteResponse>.Failure(
                "Você não participa desta campanha.",
                ServiceErrorType.Forbidden);
        }

        var isMaster = await campaignPermissionService.IsCampaignMasterAsync(campaignId, userId, cancellationToken);
        if (!CanView(note, userId, isMaster))
        {
            return ServiceResult<CampaignNoteResponse>.Failure("Você não pode visualizar esta nota.", ServiceErrorType.Forbidden);
        }

        return ServiceResult<CampaignNoteResponse>.Success(ToResponse(note, userId, isMaster));
    }

    public async Task<ServiceResult<CampaignNoteResponse>> CreateAsync(
        Guid userId,
        Guid campaignId,
        CampaignNoteRequest request,
        CancellationToken cancellationToken)
    {
        var role = await campaignPermissionService.GetCampaignRoleAsync(campaignId, userId, cancellationToken);
        if (role is null)
        {
            return ServiceResult<CampaignNoteResponse>.Failure(
                "Você não participa desta campanha.",
                ServiceErrorType.Forbidden);
        }

        var visibility = request.Visibility ?? await campaignPermissionService.GetDefaultVisibilityAsync(
            campaignId,
            userId,
            cancellationToken);

        if (visibility is null)
        {
            return ServiceResult<CampaignNoteResponse>.Failure("Visibilidade inválida.");
        }

        var validationError = Validate(request, visibility.Value, role.Value);
        if (validationError is not null)
        {
            return ServiceResult<CampaignNoteResponse>.Failure(validationError);
        }

        var note = new CampaignNote
        {
            CampaignId = campaignId,
            OwnerUserId = userId
        };

        ApplyRequest(note, request, visibility.Value);
        dbContext.CampaignNotes.Add(note);
        await dbContext.SaveChangesAsync(cancellationToken);

        var created = await GetNoteAsync(campaignId, note.Id, cancellationToken);
        return ServiceResult<CampaignNoteResponse>.Success(ToResponse(created!, userId, role == CampaignRole.Master));
    }

    public async Task<ServiceResult<CampaignNoteResponse>> UpdateAsync(
        Guid userId,
        Guid campaignId,
        Guid noteId,
        CampaignNoteRequest request,
        CancellationToken cancellationToken)
    {
        var note = await GetNoteAsync(campaignId, noteId, cancellationToken);
        if (note is null)
        {
            return ServiceResult<CampaignNoteResponse>.Failure("Nota não encontrada.", ServiceErrorType.NotFound);
        }

        var role = await campaignPermissionService.GetCampaignRoleAsync(campaignId, userId, cancellationToken);
        if (role is null || !CanEdit(note, userId, role.Value))
        {
            return ServiceResult<CampaignNoteResponse>.Failure("Você não pode editar esta nota.", ServiceErrorType.Forbidden);
        }

        var visibility = request.Visibility ?? note.Visibility;
        var validationError = Validate(request, visibility, role.Value);
        if (validationError is not null)
        {
            return ServiceResult<CampaignNoteResponse>.Failure(validationError);
        }

        ApplyRequest(note, request, visibility);
        note.UpdatedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ServiceResult<CampaignNoteResponse>.Success(ToResponse(note, userId, role == CampaignRole.Master));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid userId,
        Guid campaignId,
        Guid noteId,
        CancellationToken cancellationToken)
    {
        var note = await GetNoteAsync(campaignId, noteId, cancellationToken);
        if (note is null)
        {
            return ServiceResult<bool>.Failure("Nota não encontrada.", ServiceErrorType.NotFound);
        }

        var role = await campaignPermissionService.GetCampaignRoleAsync(campaignId, userId, cancellationToken);
        if (role is null || !CanEdit(note, userId, role.Value))
        {
            return ServiceResult<bool>.Failure("Você não pode excluir esta nota.", ServiceErrorType.Forbidden);
        }

        dbContext.CampaignNotes.Remove(note);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true);
    }

    private async Task<CampaignNote?> GetNoteAsync(
        Guid campaignId,
        Guid noteId,
        CancellationToken cancellationToken)
        => await dbContext.CampaignNotes
            .Include(note => note.OwnerUser)
            .SingleOrDefaultAsync(note => note.CampaignId == campaignId && note.Id == noteId, cancellationToken);

    private static IQueryable<CampaignNote> ApplyVisibility(
        IQueryable<CampaignNote> query,
        Guid userId,
        bool isMaster)
        => isMaster
            ? query
            : query.Where(note => note.OwnerUserId == userId || note.Visibility == Visibility.PublicToPlayers);

    private static IQueryable<CampaignNote> ApplyFilters(
        IQueryable<CampaignNote> query,
        CampaignNoteQuery filters)
    {
        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.Trim().ToLower();
            query = query.Where(note =>
                note.Title.ToLower().Contains(search) ||
                note.ContentMarkdown.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filters.Tag))
        {
            var tag = filters.Tag.Trim().ToLower();
            query = query.Where(note => note.Tags.ToLower().Contains(tag));
        }

        if (filters.Visibility.HasValue)
        {
            query = query.Where(note => note.Visibility == filters.Visibility.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.LinkedEntityType))
        {
            var linkedEntityType = filters.LinkedEntityType.Trim();
            query = query.Where(note => note.LinkedEntityType == linkedEntityType);
        }

        if (filters.LinkedEntityId.HasValue)
        {
            query = query.Where(note => note.LinkedEntityId == filters.LinkedEntityId.Value);
        }

        return query;
    }

    private static bool CanView(CampaignNote note, Guid userId, bool isMaster)
        => isMaster || note.OwnerUserId == userId || note.Visibility == Visibility.PublicToPlayers;

    private static bool CanEdit(CampaignNote note, Guid userId, CampaignRole role)
        => role == CampaignRole.Master || note.OwnerUserId == userId;

    private static string? Validate(CampaignNoteRequest request, Visibility visibility, CampaignRole role)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return "Título é obrigatório.";
        }

        if (request.Title.Trim().Length > 180)
        {
            return "Título deve ter no máximo 180 caracteres.";
        }

        if (request.ContentMarkdown.Length > 20000)
        {
            return "Conteúdo deve ter no máximo 20000 caracteres.";
        }

        if (request.Tags.Length > 500)
        {
            return "Tags devem ter no máximo 500 caracteres.";
        }

        if (request.LinkedEntityType?.Length > 80)
        {
            return "Tipo de entidade vinculada deve ter no máximo 80 caracteres.";
        }

        if (request.ExternalId?.Length > 180)
        {
            return "Id externo deve ter no máximo 180 caracteres.";
        }

        if (role == CampaignRole.Player &&
            visibility is not (Visibility.Private or Visibility.PlayerOnly))
        {
            return "Jogadores só podem criar ou manter notas privadas.";
        }

        return null;
    }

    private static void ApplyRequest(CampaignNote note, CampaignNoteRequest request, Visibility visibility)
    {
        note.Title = request.Title.Trim();
        note.ContentMarkdown = request.ContentMarkdown.Trim();
        note.Tags = request.Tags.Trim();
        note.Visibility = visibility;
        note.LinkedEntityType = NormalizeOptional(request.LinkedEntityType);
        note.LinkedEntityId = request.LinkedEntityId;
        note.ExternalProvider = request.ExternalProvider;
        note.ExternalId = NormalizeOptional(request.ExternalId);
    }

    private static CampaignNoteResponse ToResponse(CampaignNote note, Guid userId, bool isMaster)
        => new(
            note.Id,
            note.CampaignId,
            note.OwnerUserId,
            note.OwnerUser.Name,
            note.Title,
            note.ContentMarkdown,
            note.Tags,
            note.Visibility,
            note.LinkedEntityType,
            note.LinkedEntityId,
            note.ExternalProvider,
            note.ExternalId,
            note.CreatedAt,
            note.UpdatedAt,
            isMaster || note.OwnerUserId == userId);

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
