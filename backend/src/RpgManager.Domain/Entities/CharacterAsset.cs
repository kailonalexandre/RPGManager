using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class CharacterAsset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public AssetType AssetType { get; set; } = AssetType.Gallery;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
