using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using RpgManager.Application.Campaigns;
using RpgManager.Application.Common;
using RpgManager.Application.Permissions;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.Campaigns;

public sealed class CampaignService(
    AppDbContext dbContext,
    ICampaignPermissionService campaignPermissionService) : ICampaignService
{
    private const string InviteAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public async Task<IReadOnlyList<CampaignSummaryResponse>> GetMyCampaignsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.CampaignMembers
            .AsNoTracking()
            .Where(member => member.UserId == userId)
            .OrderByDescending(member => member.Campaign.CreatedAt)
            .Select(member => new CampaignSummaryResponse(
                member.Campaign.Id,
                member.Campaign.Name,
                member.Campaign.Description,
                member.Campaign.System,
                member.Campaign.CoverImageUrl,
                member.Campaign.CreatedAt,
                member.Role,
                member.Campaign.Members.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceResult<CampaignResponse>> GetByIdAsync(
        Guid userId,
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        var campaign = await GetCampaignWithMembersAsync(campaignId, cancellationToken);
        if (campaign is null)
        {
            return ServiceResult<CampaignResponse>.Failure("Campanha não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await campaignPermissionService.CanViewCampaignAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<CampaignResponse>.Failure("Você não participa desta campanha.", ServiceErrorType.Forbidden);
        }

        return ServiceResult<CampaignResponse>.Success(ToResponse(campaign, userId));
    }

    public async Task<ServiceResult<CampaignResponse>> CreateAsync(
        Guid userId,
        CampaignRequest request,
        CancellationToken cancellationToken)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return ServiceResult<CampaignResponse>.Failure(validationError);
        }

        var campaign = new Campaign
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            System = request.System.Trim(),
            CoverImageUrl = NormalizeOptional(request.CoverImageUrl),
            InviteCode = await GenerateUniqueInviteCodeAsync(cancellationToken),
            CreatedByUserId = userId,
            Members =
            [
                new CampaignMember
                {
                    UserId = userId,
                    Role = CampaignRole.Master
                }
            ]
        };

        dbContext.Campaigns.Add(campaign);
        await dbContext.SaveChangesAsync(cancellationToken);

        var created = await GetCampaignWithMembersAsync(campaign.Id, cancellationToken);
        return ServiceResult<CampaignResponse>.Success(ToResponse(created!, userId));
    }

    public async Task<ServiceResult<CampaignResponse>> UpdateAsync(
        Guid userId,
        Guid campaignId,
        CampaignRequest request,
        CancellationToken cancellationToken)
    {
        var validationError = Validate(request);
        if (validationError is not null)
        {
            return ServiceResult<CampaignResponse>.Failure(validationError);
        }

        var campaign = await GetCampaignWithMembersAsync(campaignId, cancellationToken);
        if (campaign is null)
        {
            return ServiceResult<CampaignResponse>.Failure("Campanha não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await campaignPermissionService.CanEditCampaignAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<CampaignResponse>.Failure("Apenas Mestre pode editar campanha.", ServiceErrorType.Forbidden);
        }

        campaign.Name = request.Name.Trim();
        campaign.Description = request.Description.Trim();
        campaign.System = request.System.Trim();
        campaign.CoverImageUrl = NormalizeOptional(request.CoverImageUrl);
        campaign.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<CampaignResponse>.Success(ToResponse(campaign, userId));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(
        Guid userId,
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        var campaign = await GetCampaignWithMembersAsync(campaignId, cancellationToken);
        if (campaign is null)
        {
            return ServiceResult<bool>.Failure("Campanha não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await campaignPermissionService.CanEditCampaignAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<bool>.Failure("Apenas Mestre pode excluir campanha.", ServiceErrorType.Forbidden);
        }

        dbContext.Campaigns.Remove(campaign);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<CampaignResponse>> JoinAsync(
        Guid userId,
        JoinCampaignRequest request,
        CancellationToken cancellationToken)
    {
        var inviteCode = request.InviteCode.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(inviteCode))
        {
            return ServiceResult<CampaignResponse>.Failure("Código de convite é obrigatório.");
        }

        var campaign = await dbContext.Campaigns
            .Include(item => item.Members)
                .ThenInclude(member => member.User)
            .SingleOrDefaultAsync(item => item.InviteCode == inviteCode, cancellationToken);

        if (campaign is null)
        {
            return ServiceResult<CampaignResponse>.Failure("Convite inválido.", ServiceErrorType.NotFound);
        }

        if (IsMember(campaign, userId))
        {
            return ServiceResult<CampaignResponse>.Failure("Usuário já participa desta campanha.", ServiceErrorType.Conflict);
        }

        dbContext.CampaignMembers.Add(new CampaignMember
        {
            CampaignId = campaign.Id,
            UserId = userId,
            Role = CampaignRole.Player
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        var joined = await GetCampaignWithMembersAsync(campaign.Id, cancellationToken);
        return ServiceResult<CampaignResponse>.Success(ToResponse(joined!, userId));
    }

    public async Task<ServiceResult<CampaignResponse>> RegenerateInviteAsync(
        Guid userId,
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        var campaign = await GetCampaignWithMembersAsync(campaignId, cancellationToken);
        if (campaign is null)
        {
            return ServiceResult<CampaignResponse>.Failure("Campanha não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await campaignPermissionService.CanEditCampaignAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<CampaignResponse>.Failure("Apenas Mestre pode regenerar convite.", ServiceErrorType.Forbidden);
        }

        campaign.InviteCode = await GenerateUniqueInviteCodeAsync(cancellationToken);
        campaign.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<CampaignResponse>.Success(ToResponse(campaign, userId));
    }

    public async Task<ServiceResult<IReadOnlyList<CampaignMemberResponse>>> GetMembersAsync(
        Guid userId,
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        var campaign = await GetCampaignWithMembersAsync(campaignId, cancellationToken);
        if (campaign is null)
        {
            return ServiceResult<IReadOnlyList<CampaignMemberResponse>>.Failure("Campanha não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await campaignPermissionService.IsCampaignMasterAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CampaignMemberResponse>>.Failure("Apenas Mestre pode visualizar membros administrativos.", ServiceErrorType.Forbidden);
        }

        return ServiceResult<IReadOnlyList<CampaignMemberResponse>>.Success(ToMemberResponses(campaign.Members));
    }

    public async Task<ServiceResult<IReadOnlyList<CampaignCharacterSummaryResponse>>> GetCharactersAsync(
        Guid userId,
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        var campaign = await GetCampaignWithMembersAsync(campaignId, cancellationToken);
        if (campaign is null)
        {
            return ServiceResult<IReadOnlyList<CampaignCharacterSummaryResponse>>.Failure("Campanha não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await campaignPermissionService.CanViewCampaignAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<IReadOnlyList<CampaignCharacterSummaryResponse>>.Failure("Você não participa desta campanha.", ServiceErrorType.Forbidden);
        }

        var isMaster = await campaignPermissionService.IsCampaignMasterAsync(campaignId, userId, cancellationToken);
        var characters = await GetCampaignCharactersAsync(campaignId, isMaster ? null : userId, cancellationToken);
        return ServiceResult<IReadOnlyList<CampaignCharacterSummaryResponse>>.Success(characters);
    }

    public async Task<ServiceResult<CampaignMasterDashboardResponse>> GetMasterDashboardAsync(
        Guid userId,
        Guid campaignId,
        CancellationToken cancellationToken)
    {
        var campaign = await GetCampaignWithMembersAsync(campaignId, cancellationToken);
        if (campaign is null)
        {
            return ServiceResult<CampaignMasterDashboardResponse>.Failure("Campanha não encontrada.", ServiceErrorType.NotFound);
        }

        if (!await campaignPermissionService.IsCampaignMasterAsync(campaignId, userId, cancellationToken))
        {
            return ServiceResult<CampaignMasterDashboardResponse>.Failure("Apenas Mestre pode acessar painel completo.", ServiceErrorType.Forbidden);
        }

        var characters = await GetCampaignCharactersAsync(campaignId, null, cancellationToken);
        var notes = await dbContext.CharacterNotes
            .AsNoTracking()
            .Include(note => note.Character)
            .Where(note =>
                note.Character.CampaignId == campaignId &&
                note.IsVisibleToMaster &&
                !note.IsPrivate)
            .OrderByDescending(note => note.UpdatedAt ?? note.CreatedAt)
            .ThenBy(note => note.Title)
            .Select(note => new CampaignMasterNoteResponse(
                note.Id,
                note.CharacterId,
                note.Character.Name,
                note.Title,
                note.Content,
                note.Category,
                note.Tags,
                note.CreatedAt,
                note.UpdatedAt))
            .ToListAsync(cancellationToken);

        var dashboard = new CampaignMasterDashboardResponse(
            campaign.Id,
            campaign.Name,
            ToMemberResponses(campaign.Members),
            characters,
            notes);

        return ServiceResult<CampaignMasterDashboardResponse>.Success(dashboard);
    }

    private async Task<Campaign?> GetCampaignWithMembersAsync(Guid campaignId, CancellationToken cancellationToken)
    {
        return await dbContext.Campaigns
            .Include(campaign => campaign.Members)
                .ThenInclude(member => member.User)
            .SingleOrDefaultAsync(campaign => campaign.Id == campaignId, cancellationToken);
    }

    private async Task<IReadOnlyList<CampaignCharacterSummaryResponse>> GetCampaignCharactersAsync(
        Guid campaignId,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        var characters = await dbContext.Characters
            .AsNoTracking()
            .Include(character => character.User)
            .Include(character => character.Skills)
            .Where(character => character.CampaignId == campaignId &&
                (!userId.HasValue || character.UserId == userId.Value))
            .OrderBy(character => character.Name)
            .ToListAsync(cancellationToken);

        return characters
            .Select(character => new CampaignCharacterSummaryResponse(
                character.Id,
                character.UserId,
                character.User.Name,
                character.Name,
                character.MainClass,
                character.TotalLevel,
                character.CurrentHitPoints,
                character.MaxHitPoints,
                character.ArmorClass,
                CalculatePassivePerception(character)))
            .ToList();
    }

    private async Task<string> GenerateUniqueInviteCodeAsync(CancellationToken cancellationToken)
    {
        string code;
        do
        {
            code = GenerateInviteCode();
        }
        while (await dbContext.Campaigns.AnyAsync(campaign => campaign.InviteCode == code, cancellationToken));

        return code;
    }

    private static string GenerateInviteCode()
    {
        Span<char> chars = stackalloc char[8];
        for (var index = 0; index < chars.Length; index++)
        {
            chars[index] = InviteAlphabet[RandomNumberGenerator.GetInt32(InviteAlphabet.Length)];
        }

        return new string(chars);
    }

    private static bool IsMember(Campaign campaign, Guid userId)
        => campaign.Members.Any(member => member.UserId == userId);

    private static bool IsMaster(Campaign campaign, Guid userId)
        => campaign.Members.Any(member => member.UserId == userId && member.Role == CampaignRole.Master);

    private static string? Validate(CampaignRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "Nome é obrigatório.";
        }

        if (request.Name.Trim().Length > 140)
        {
            return "Nome deve ter no máximo 140 caracteres.";
        }

        if (request.Description.Trim().Length > 1200)
        {
            return "Descrição deve ter no máximo 1200 caracteres.";
        }

        if (string.IsNullOrWhiteSpace(request.System))
        {
            return "Sistema é obrigatório.";
        }

        if (request.System.Trim().Length > 80)
        {
            return "Sistema deve ter no máximo 80 caracteres.";
        }

        return null;
    }

    private static CampaignResponse ToResponse(Campaign campaign, Guid currentUserId)
    {
        var currentUserRole = campaign.Members
            .Single(member => member.UserId == currentUserId)
            .Role;

        var isMaster = currentUserRole == CampaignRole.Master;
        var members = ToMemberResponses(campaign.Members, includeAdministrativeInfo: isMaster);

        return new CampaignResponse(
            campaign.Id,
            campaign.Name,
            campaign.Description,
            campaign.System,
            campaign.CoverImageUrl,
            isMaster ? campaign.InviteCode : string.Empty,
            campaign.CreatedByUserId,
            campaign.CreatedAt,
            campaign.UpdatedAt,
            currentUserRole,
            members);
    }

    private static List<CampaignMemberResponse> ToMemberResponses(
        IEnumerable<CampaignMember> members,
        bool includeAdministrativeInfo = true)
    {
        return members
            .OrderBy(member => member.Role == CampaignRole.Master ? 0 : 1)
            .ThenBy(member => member.User.Name)
            .Select(member => new CampaignMemberResponse(
                member.Id,
                member.UserId,
                member.User.Name,
                includeAdministrativeInfo ? member.User.Email : string.Empty,
                member.Role,
                includeAdministrativeInfo ? member.JoinedAt : default))
            .ToList();
    }

    private static int CalculatePassivePerception(Character character)
    {
        var wisdomModifier = (int)Math.Floor((character.Wisdom - 10) / 2.0);
        var perception = character.Skills.SingleOrDefault(skill => skill.SkillType == SkillType.Perception);
        if (perception is null)
        {
            return 10 + wisdomModifier;
        }

        var proficiency = perception.IsProficient ? character.ProficiencyBonus : 0;
        var expertise = perception.IsExpertise ? character.ProficiencyBonus : 0;
        return 10 + wisdomModifier + proficiency + expertise + perception.CustomBonus;
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
