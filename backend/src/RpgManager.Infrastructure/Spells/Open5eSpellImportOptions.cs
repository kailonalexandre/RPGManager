namespace RpgManager.Infrastructure.Spells;

public sealed class Open5eSpellImportOptions
{
    public const string SectionName = "Open5e";

    public string SpellsUrl { get; set; } = "https://api.open5e.com/v2/spells/";
    public string DocumentKeys { get; set; } = "srd-2014,srd-2024";
    public int PageSize { get; set; } = 100;
    public int MaxPages { get; set; } = 100;
    public int TimeoutSeconds { get; set; } = 30;
}
