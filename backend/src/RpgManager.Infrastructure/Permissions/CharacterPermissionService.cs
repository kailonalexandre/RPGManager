using Microsoft.EntityFrameworkCore;
using RpgManager.Application.Permissions;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.Permissions;

public sealed class CharacterPermissionService(AppDbContext dbContext) : ICharacterPermissionService
{
    public async Task<bool> CanViewCharacterAsync(Guid characterId, Guid userId, CancellationToken cancellationToken)
    {
        var character = await dbContext.Characters
            .AsNoTracking()
            .Where(item => item.Id == characterId)
            .Select(item => new { item.UserId, item.CampaignId })
            .SingleOrDefaultAsync(cancellationToken);

        if (character is null)
        {
            return false;
        }

        if (character.UserId == userId)
        {
            return true;
        }

        if (!character.CampaignId.HasValue)
        {
            return false;
        }

        return await dbContext.CampaignMembers.AnyAsync(
            member => member.CampaignId == character.CampaignId.Value &&
                member.UserId == userId &&
                member.Role == CampaignRole.Master,
            cancellationToken);
    }

    public Task<bool> CanEditCharacterAsync(Guid characterId, Guid userId, CancellationToken cancellationToken)
        => dbContext.Characters.AnyAsync(
            character => character.Id == characterId && character.UserId == userId,
            cancellationToken);

    public Task<bool> CanDeleteCharacterAsync(Guid characterId, Guid userId, CancellationToken cancellationToken)
        => CanEditCharacterAsync(characterId, userId, cancellationToken);

    public async Task<bool> CanViewNoteAsync(Guid noteId, Guid userId, CancellationToken cancellationToken)
    {
        var note = await dbContext.CharacterNotes
            .AsNoTracking()
            .Where(item => item.Id == noteId)
            .Select(item => new
            {
                item.IsPrivate,
                item.IsVisibleToMaster,
                CharacterUserId = item.Character.UserId,
                item.Character.CampaignId
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (note is null)
        {
            return false;
        }

        if (note.CharacterUserId == userId)
        {
            return true;
        }

        if (note.IsPrivate || !note.IsVisibleToMaster || !note.CampaignId.HasValue)
        {
            return false;
        }

        return await dbContext.CampaignMembers.AnyAsync(
            member => member.CampaignId == note.CampaignId.Value &&
                member.UserId == userId &&
                member.Role == CampaignRole.Master,
            cancellationToken);
    }

    public Task<bool> CanEditNoteAsync(Guid noteId, Guid userId, CancellationToken cancellationToken)
        => dbContext.CharacterNotes.AnyAsync(
            note => note.Id == noteId && note.Character.UserId == userId,
            cancellationToken);
}
