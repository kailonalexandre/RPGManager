using RpgManager.Domain.Entities;
using RpgManager.Domain.Enums;

namespace RpgManager.Infrastructure.Spells;

public static class Open5eSpellMapper
{
    public const string ExternalSource = "Open5e";

    public static void Apply(Spell spell, Open5eSpellItem item, Guid userId, DateTime now)
    {
        var sourceName = Normalize(item.Document?.DisplayName) ?? Normalize(item.Document?.Name) ?? "SRD";
        var documentKey = Normalize(item.Document?.Key) ?? string.Empty;

        spell.Name = Normalize(item.Name) ?? "Unnamed Spell";
        spell.EnglishName = spell.Name;
        spell.Level = Math.Clamp(item.Level, 0, 9);
        spell.School = MapSchool(item.School?.Name ?? item.School?.Key);
        spell.CastingTime = MapCastingTime(item.CastingTime);
        spell.Range = Normalize(item.RangeText) ?? string.Empty;
        spell.Components = MapComponents(item);
        spell.Material = item.Material ? Normalize(item.MaterialSpecified) ?? string.Empty : string.Empty;
        spell.Duration = Normalize(item.Duration) ?? string.Empty;
        spell.IsConcentration = item.Concentration || spell.Duration.Contains("concentration", StringComparison.OrdinalIgnoreCase);
        spell.IsRitual = item.Ritual;
        spell.Description = Normalize(item.Description) ?? string.Empty;
        spell.HigherLevelDescription = Normalize(item.HigherLevel) ?? string.Empty;
        spell.AvailableClasses = MapClasses(item.Classes);
        spell.Source = $"{sourceName} / Open5e";
        spell.IsHomebrew = false;
        spell.ExternalSource = ExternalSource;
        spell.ExternalId = item.Key;
        spell.Slug = item.Key;
        spell.RulesVersion = MapRulesVersion(item.Document?.GameSystem?.Name, item.Document?.GameSystem?.Key);
        spell.IsImported = true;
        spell.IsSrd = documentKey.StartsWith("srd-", StringComparison.OrdinalIgnoreCase);
        spell.Language = "en";
        spell.TranslationMissing = true;
        spell.Visibility = SpellVisibility.LocalPublic;
        spell.CampaignId = null;
        spell.ImportedAt = now;

        if (spell.CreatedByUserId == Guid.Empty)
        {
            spell.CreatedByUserId = userId;
        }
    }

    public static string MapSchool(string? school)
    {
        return Normalize(school)?.ToLowerInvariant() switch
        {
            "abjuration" => "Abjuração",
            "conjuration" => "Conjuração",
            "divination" => "Adivinhação",
            "enchantment" => "Encantamento",
            "evocation" => "Evocação",
            "illusion" => "Ilusão",
            "necromancy" => "Necromancia",
            "transmutation" => "Transmutação",
            _ => Normalize(school) ?? "Desconhecida"
        };
    }

    public static string MapComponents(Open5eSpellItem item)
    {
        var components = new List<string>();
        if (item.Verbal)
        {
            components.Add("V");
        }

        if (item.Somatic)
        {
            components.Add("S");
        }

        if (item.Material)
        {
            components.Add("M");
        }

        return string.Join(", ", components);
    }

    public static string MapClasses(IReadOnlyList<Open5eClass>? classes)
    {
        if (classes is null || classes.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(", ", classes
            .Select(item => MapClass(item.Name))
            .Where(item => item.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item));
    }

    public static string MapClass(string? className)
    {
        return Normalize(className)?.ToLowerInvariant() switch
        {
            "artificer" => "Artífice",
            "bard" => "Bardo",
            "cleric" => "Clérigo",
            "druid" => "Druida",
            "paladin" => "Paladino",
            "ranger" => "Patrulheiro",
            "sorcerer" => "Feiticeiro",
            "warlock" => "Bruxo",
            "wizard" => "Mago",
            _ => Normalize(className) ?? string.Empty
        };
    }

    private static string MapCastingTime(string? castingTime)
    {
        return Normalize(castingTime)?.ToLowerInvariant() switch
        {
            "action" => "1 ação",
            "bonus action" => "1 ação bônus",
            "reaction" => "1 reação",
            "minute" => "1 minuto",
            "hour" => "1 hora",
            _ => Normalize(castingTime) ?? string.Empty
        };
    }

    private static string MapRulesVersion(string? name, string? key)
    {
        if ((key ?? string.Empty).Contains("2024", StringComparison.OrdinalIgnoreCase) ||
            (name ?? string.Empty).Contains("2024", StringComparison.OrdinalIgnoreCase))
        {
            return "D&D 2024";
        }

        return Normalize(name) ?? Normalize(key) ?? "D&D 2014";
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
