using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RpgManager.Application.Spells;
using RpgManager.Domain.Entities;
using RpgManager.Infrastructure.Data;

namespace RpgManager.Infrastructure.Spells;

public sealed class SpellImportService(
    AppDbContext dbContext,
    IOpen5eSpellClient client,
    IOptions<Open5eSpellImportOptions> options,
    ILogger<SpellImportService> logger) : ISpellImportService
{
    private readonly Open5eSpellImportOptions _options = options.Value;

    public async Task<SpellImportResponse> ImportOpen5eAsync(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Iniciando importação de magias Open5e.");

        var created = 0;
        var updated = 0;
        var skipped = 0;
        var errors = new List<string>();
        var pageCount = 0;
        var url = BuildInitialUrl();

        while (!string.IsNullOrWhiteSpace(url))
        {
            pageCount++;
            if (pageCount > Math.Max(1, _options.MaxPages))
            {
                errors.Add("Importação interrompida: limite máximo de páginas atingido.");
                break;
            }

            Open5eSpellPage? page;
            try
            {
                page = await client.GetPageAsync(url, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Erro HTTP ao consultar Open5e.");
                errors.Add($"Erro HTTP na página {pageCount}: {exception.Message}");
                break;
            }

            if (page is null)
            {
                errors.Add($"Open5e não retornou página {pageCount}.");
                break;
            }

            logger.LogInformation("Open5e página {Page} recebida com {Count} magias.", pageCount, page.Results.Count);

            foreach (var item in page.Results)
            {
                try
                {
                    var result = await ImportItemAsync(userId, item, cancellationToken);
                    switch (result)
                    {
                        case ImportItemResult.Created:
                            created++;
                            logger.LogInformation("Magia criada: {Key} - {Name}", item.Key, item.Name);
                            break;
                        case ImportItemResult.Updated:
                            updated++;
                            logger.LogInformation("Magia atualizada: {Key} - {Name}", item.Key, item.Name);
                            break;
                        case ImportItemResult.Skipped:
                            skipped++;
                            logger.LogInformation("Magia ignorada: {Key} - {Name}", item.Key, item.Name);
                            break;
                    }
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    logger.LogError(exception, "Erro ao mapear/importar magia {Key}", item.Key);
                    errors.Add($"{item.Key}: {exception.Message}");
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            url = page.Next;
        }

        logger.LogInformation(
            "Importação Open5e finalizada. Criadas: {Created}. Atualizadas: {Updated}. Ignoradas: {Skipped}. Erros: {Errors}.",
            created,
            updated,
            skipped,
            errors.Count);

        return new SpellImportResponse(created, updated, skipped, errors);
    }

    private async Task<ImportItemResult> ImportItemAsync(
        Guid userId,
        Open5eSpellItem item,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(item.Key) || string.IsNullOrWhiteSpace(item.Name))
        {
            return ImportItemResult.Skipped;
        }

        var existingImported = await dbContext.Spells
            .SingleOrDefaultAsync(spell =>
                spell.ExternalSource == Open5eSpellMapper.ExternalSource &&
                spell.ExternalId == item.Key,
                cancellationToken);

        var now = DateTime.UtcNow;
        if (existingImported is not null)
        {
            if (!existingImported.IsImported)
            {
                return ImportItemResult.Skipped;
            }

            Open5eSpellMapper.Apply(existingImported, item, userId, now);
            existingImported.UpdatedAt = now;
            return ImportItemResult.Updated;
        }

        var hasManualConflict = await dbContext.Spells.AnyAsync(spell =>
            spell.IsHomebrew &&
            spell.Level == item.Level &&
            (spell.Name.ToLower() == item.Name.ToLower() || spell.EnglishName.ToLower() == item.Name.ToLower()),
            cancellationToken);

        if (hasManualConflict)
        {
            return ImportItemResult.Skipped;
        }

        var spell = new Spell();
        Open5eSpellMapper.Apply(spell, item, userId, now);
        dbContext.Spells.Add(spell);
        return ImportItemResult.Created;
    }

    private string BuildInitialUrl()
    {
        var pageSize = Math.Clamp(_options.PageSize, 1, 200);
        var documentKeys = Uri.EscapeDataString(_options.DocumentKeys);
        var separator = _options.SpellsUrl.Contains('?') ? '&' : '?';
        return $"{_options.SpellsUrl}{separator}document__key__in={documentKeys}&limit={pageSize}";
    }

    private enum ImportItemResult
    {
        Created,
        Updated,
        Skipped
    }
}
