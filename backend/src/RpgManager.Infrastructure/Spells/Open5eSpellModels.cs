using System.Text.Json.Serialization;

namespace RpgManager.Infrastructure.Spells;

public sealed record Open5eSpellPage(
    int Count,
    string? Next,
    string? Previous,
    IReadOnlyList<Open5eSpellItem> Results);

public sealed record Open5eSpellItem(
    string Key,
    Open5eDocument? Document,
    Open5eSchool? School,
    IReadOnlyList<Open5eClass>? Classes,
    string Name,
    [property: JsonPropertyName("desc")] string Description,
    int Level,
    [property: JsonPropertyName("higher_level")] string? HigherLevel,
    [property: JsonPropertyName("range_text")] string? RangeText,
    bool Ritual,
    [property: JsonPropertyName("casting_time")] string? CastingTime,
    bool Verbal,
    bool Somatic,
    bool Material,
    [property: JsonPropertyName("material_specified")] string? MaterialSpecified,
    string? Duration,
    bool Concentration);

public sealed record Open5eDocument(
    string Name,
    string Key,
    [property: JsonPropertyName("display_name")] string DisplayName,
    [property: JsonPropertyName("gamesystem")] Open5eGameSystem? GameSystem);

public sealed record Open5eGameSystem(string Name, string Key);

public sealed record Open5eSchool(string Name, string Key);

public sealed record Open5eClass(string Name, string Key);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(Open5eSpellPage))]
internal sealed partial class Open5eJsonContext : JsonSerializerContext;
