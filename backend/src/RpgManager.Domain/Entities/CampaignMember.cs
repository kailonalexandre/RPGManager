using RpgManager.Domain.Enums;

namespace RpgManager.Domain.Entities;

public sealed class CampaignMember
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public CampaignRole Role { get; set; } = CampaignRole.Player;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
