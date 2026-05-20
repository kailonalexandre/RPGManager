using Microsoft.EntityFrameworkCore;
using RpgManager.Application.CharacterOptions;
using RpgManager.Application.Common;
using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.CharacterOptions;

public sealed class CharacterOptionService(AppDbContext dbContext) : ICharacterOptionService
{
    public async Task<IReadOnlyList<RaceResponse>> GetRacesAsync(CancellationToken cancellationToken)
        => await dbContext.Races
            .AsNoTracking()
            .OrderBy(race => race.Name)
            .Select(race => ToResponse(race))
            .ToListAsync(cancellationToken);

    public async Task<ServiceResult<RaceResponse>> CreateRaceAsync(
        Guid userId,
        RaceRequest request,
        CancellationToken cancellationToken)
    {
        if (!await CanCreateOptionsAsync(userId, cancellationToken))
        {
            return ServiceResult<RaceResponse>.Failure("Apenas Mestre pode cadastrar opções de personagem.", ServiceErrorType.Forbidden);
        }

        var validationError = ValidateNameSource(request.Name, request.Source);
        if (validationError is not null)
        {
            return ServiceResult<RaceResponse>.Failure(validationError);
        }

        var race = new Race
        {
            Name = request.Name.Trim(),
            Description = Normalize(request.Description),
            Source = Normalize(request.Source),
            IsHomebrew = request.IsHomebrew,
            CreatedByUserId = userId
        };

        dbContext.Races.Add(race);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<RaceResponse>.Success(ToResponse(race));
    }

    public async Task<IReadOnlyList<CharacterClassResponse>> GetClassesAsync(CancellationToken cancellationToken)
        => await dbContext.CharacterClasses
            .AsNoTracking()
            .OrderBy(characterClass => characterClass.Name)
            .Select(characterClass => ToResponse(characterClass))
            .ToListAsync(cancellationToken);

    public async Task<ServiceResult<CharacterClassResponse>> CreateClassAsync(
        Guid userId,
        CharacterClassRequest request,
        CancellationToken cancellationToken)
    {
        if (!await CanCreateOptionsAsync(userId, cancellationToken))
        {
            return ServiceResult<CharacterClassResponse>.Failure("Apenas Mestre pode cadastrar opções de personagem.", ServiceErrorType.Forbidden);
        }

        var validationError = ValidateNameSource(request.Name, request.Source);
        if (validationError is not null)
        {
            return ServiceResult<CharacterClassResponse>.Failure(validationError);
        }

        if (request.HitDie is < 1 or > 20)
        {
            return ServiceResult<CharacterClassResponse>.Failure("Dado de vida deve ficar entre 1 e 20.");
        }

        var characterClass = new CharacterClass
        {
            Name = request.Name.Trim(),
            HitDie = request.HitDie,
            Description = Normalize(request.Description),
            Source = Normalize(request.Source),
            IsHomebrew = request.IsHomebrew,
            CreatedByUserId = userId
        };

        dbContext.CharacterClasses.Add(characterClass);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<CharacterClassResponse>.Success(ToResponse(characterClass));
    }

    public async Task<IReadOnlyList<BackgroundResponse>> GetBackgroundsAsync(CancellationToken cancellationToken)
        => await dbContext.Backgrounds
            .AsNoTracking()
            .OrderBy(background => background.Name)
            .Select(background => ToResponse(background))
            .ToListAsync(cancellationToken);

    public async Task<ServiceResult<BackgroundResponse>> CreateBackgroundAsync(
        Guid userId,
        BackgroundRequest request,
        CancellationToken cancellationToken)
    {
        if (!await CanCreateOptionsAsync(userId, cancellationToken))
        {
            return ServiceResult<BackgroundResponse>.Failure("Apenas Mestre pode cadastrar opções de personagem.", ServiceErrorType.Forbidden);
        }

        var validationError = ValidateNameSource(request.Name, request.Source);
        if (validationError is not null)
        {
            return ServiceResult<BackgroundResponse>.Failure(validationError);
        }

        var background = new Background
        {
            Name = request.Name.Trim(),
            Description = Normalize(request.Description),
            Source = Normalize(request.Source),
            IsHomebrew = request.IsHomebrew,
            CreatedByUserId = userId
        };

        dbContext.Backgrounds.Add(background);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ServiceResult<BackgroundResponse>.Success(ToResponse(background));
    }

    private static string? ValidateNameSource(string name, string source)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Nome é obrigatório.";
        }

        if (name.Trim().Length > 180 || source.Length > 160)
        {
            return "Nome ou fonte excede o tamanho máximo.";
        }

        return null;
    }

    private static RaceResponse ToResponse(Race race)
        => new(race.Id, race.Name, race.Description, race.Source, race.IsHomebrew, race.CreatedByUserId, race.CreatedAt, race.UpdatedAt);

    private static CharacterClassResponse ToResponse(CharacterClass characterClass)
        => new(characterClass.Id, characterClass.Name, characterClass.HitDie, characterClass.Description, characterClass.Source, characterClass.IsHomebrew, characterClass.CreatedByUserId, characterClass.CreatedAt, characterClass.UpdatedAt);

    private static BackgroundResponse ToResponse(Background background)
        => new(background.Id, background.Name, background.Description, background.Source, background.IsHomebrew, background.CreatedByUserId, background.CreatedAt, background.UpdatedAt);

    private static string Normalize(string value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

    private Task<bool> CanCreateOptionsAsync(Guid userId, CancellationToken cancellationToken)
        => dbContext.Users.AnyAsync(
            user => user.Id == userId && user.Profile == UserProfile.GameMaster,
            cancellationToken);
}
