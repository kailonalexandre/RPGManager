using Microsoft.EntityFrameworkCore;
using RpgManager.Domain.Entities;

namespace RpgManager.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignMember> CampaignMembers => Set<CampaignMember>();
    public DbSet<CampaignNote> CampaignNotes => Set<CampaignNote>();
    public DbSet<Npc> Npcs => Set<Npc>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterSkill> CharacterSkills => Set<CharacterSkill>();
    public DbSet<CharacterAttack> CharacterAttacks => Set<CharacterAttack>();
    public DbSet<CharacterCondition> CharacterConditions => Set<CharacterCondition>();
    public DbSet<CharacterNote> CharacterNotes => Set<CharacterNote>();
    public DbSet<CharacterInventoryItem> CharacterInventoryItems => Set<CharacterInventoryItem>();
    public DbSet<CharacterAsset> CharacterAssets => Set<CharacterAsset>();
    public DbSet<CharacterSpell> CharacterSpells => Set<CharacterSpell>();
    public DbSet<CharacterSpellSlot> CharacterSpellSlots => Set<CharacterSpellSlot>();
    public DbSet<CharacterFeature> CharacterFeatures => Set<CharacterFeature>();
    public DbSet<Spell> Spells => Set<Spell>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<Race> Races => Set<Race>();
    public DbSet<CharacterClass> CharacterClasses => Set<CharacterClass>();
    public DbSet<Background> Backgrounds => Set<Background>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);

            entity.Property(user => user.Name)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(user => user.Email)
                .HasMaxLength(180)
                .IsRequired();

            entity.HasIndex(user => user.Email)
                .IsUnique();

            entity.Property(user => user.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(user => user.Profile)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(user => user.AvatarUrl)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.ToTable("campaigns");
            entity.HasKey(campaign => campaign.Id);

            entity.Property(campaign => campaign.Name)
                .HasMaxLength(140)
                .IsRequired();

            entity.Property(campaign => campaign.Description)
                .HasMaxLength(1200)
                .IsRequired();

            entity.Property(campaign => campaign.System)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(campaign => campaign.CoverImageUrl)
                .HasMaxLength(500);

            entity.Property(campaign => campaign.InviteCode)
                .HasMaxLength(16)
                .IsRequired();

            entity.HasIndex(campaign => campaign.InviteCode)
                .IsUnique();

            entity.HasOne(campaign => campaign.CreatedByUser)
                .WithMany()
                .HasForeignKey(campaign => campaign.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CampaignMember>(entity =>
        {
            entity.ToTable("campaign_members");
            entity.HasKey(member => member.Id);

            entity.Property(member => member.Role)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(member => new { member.CampaignId, member.UserId })
                .IsUnique();

            entity.HasOne(member => member.Campaign)
                .WithMany(campaign => campaign.Members)
                .HasForeignKey(member => member.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(member => member.User)
                .WithMany()
                .HasForeignKey(member => member.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CampaignNote>(entity =>
        {
            entity.ToTable("campaign_notes");
            entity.HasKey(note => note.Id);

            entity.Property(note => note.Title)
                .HasMaxLength(180)
                .IsRequired();

            entity.Property(note => note.ContentMarkdown)
                .HasMaxLength(20000)
                .IsRequired();

            entity.Property(note => note.Tags)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(note => note.Visibility)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(note => note.LinkedEntityType)
                .HasMaxLength(80);

            entity.Property(note => note.ExternalProvider)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(note => note.ExternalId)
                .HasMaxLength(180);

            entity.HasIndex(note => note.CampaignId);
            entity.HasIndex(note => note.OwnerUserId);
            entity.HasIndex(note => note.Visibility);
            entity.HasIndex(note => note.Title);
            entity.HasIndex(note => note.LinkedEntityType);
            entity.HasIndex(note => note.LinkedEntityId);

            entity.HasOne(note => note.Campaign)
                .WithMany()
                .HasForeignKey(note => note.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(note => note.OwnerUser)
                .WithMany()
                .HasForeignKey(note => note.OwnerUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Npc>(entity =>
        {
            entity.ToTable("npcs");
            entity.HasKey(npc => npc.Id);

            entity.Property(npc => npc.Name).HasMaxLength(180).IsRequired();
            entity.Property(npc => npc.Alias).HasMaxLength(180).IsRequired();
            entity.Property(npc => npc.Race).HasMaxLength(120).IsRequired();
            entity.Property(npc => npc.Occupation).HasMaxLength(180).IsRequired();
            entity.Property(npc => npc.Location).HasMaxLength(180).IsRequired();
            entity.Property(npc => npc.Faction).HasMaxLength(180).IsRequired();
            entity.Property(npc => npc.Personality).HasMaxLength(2000).IsRequired();
            entity.Property(npc => npc.Appearance).HasMaxLength(2000).IsRequired();
            entity.Property(npc => npc.Motivation).HasMaxLength(2000).IsRequired();
            entity.Property(npc => npc.Secrets).HasMaxLength(4000).IsRequired();
            entity.Property(npc => npc.Notes).HasMaxLength(4000).IsRequired();
            entity.Property(npc => npc.StatBlockJson).HasMaxLength(8000).IsRequired();
            entity.Property(npc => npc.Tags).HasMaxLength(500).IsRequired();

            entity.Property(npc => npc.Visibility)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.HasIndex(npc => npc.CampaignId);
            entity.HasIndex(npc => npc.Name);
            entity.HasIndex(npc => npc.Location);
            entity.HasIndex(npc => npc.Faction);
            entity.HasIndex(npc => npc.Visibility);
            entity.HasIndex(npc => npc.CreatedByUserId);

            entity.HasOne(npc => npc.Campaign)
                .WithMany()
                .HasForeignKey(npc => npc.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(npc => npc.CreatedByUser)
                .WithMany()
                .HasForeignKey(npc => npc.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.ToTable("characters");
            entity.HasKey(character => character.Id);

            entity.Property(character => character.Name)
                .HasMaxLength(140)
                .IsRequired();

            entity.Property(character => character.Nickname).HasMaxLength(140);
            entity.Property(character => character.AvatarUrl).HasMaxLength(500);
            entity.Property(character => character.TokenImageUrl).HasMaxLength(500);
            entity.Property(character => character.Species).HasMaxLength(120).IsRequired();
            entity.Property(character => character.MainClass).HasMaxLength(120).IsRequired();
            entity.Property(character => character.Subclass).HasMaxLength(120).IsRequired();
            entity.Property(character => character.Background).HasMaxLength(120).IsRequired();
            entity.Property(character => character.Alignment).HasMaxLength(80).IsRequired();
            entity.Property(character => character.TotalHitDice).HasMaxLength(80).IsRequired();
            entity.Property(character => character.AvailableHitDice).HasMaxLength(80).IsRequired();
            entity.Property(character => character.PhysicalDescription).HasMaxLength(2000).IsRequired();
            entity.Property(character => character.PersonalityTraits).HasMaxLength(1200).IsRequired();
            entity.Property(character => character.Ideals).HasMaxLength(1200).IsRequired();
            entity.Property(character => character.Bonds).HasMaxLength(1200).IsRequired();
            entity.Property(character => character.Flaws).HasMaxLength(1200).IsRequired();
            entity.Property(character => character.Backstory).HasMaxLength(4000).IsRequired();
            entity.Property(character => character.QuickNotes).HasMaxLength(2000).IsRequired();

            entity.HasIndex(character => character.UserId);
            entity.HasIndex(character => character.CampaignId);
            entity.HasIndex(character => character.RaceId);
            entity.HasIndex(character => character.ClassId);
            entity.HasIndex(character => character.BackgroundId);

            entity.HasOne(character => character.User)
                .WithMany()
                .HasForeignKey(character => character.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(character => character.Campaign)
                .WithMany()
                .HasForeignKey(character => character.CampaignId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(character => character.Race)
                .WithMany()
                .HasForeignKey(character => character.RaceId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(character => character.Class)
                .WithMany()
                .HasForeignKey(character => character.ClassId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(character => character.BackgroundOption)
                .WithMany()
                .HasForeignKey(character => character.BackgroundId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<CharacterSkill>(entity =>
        {
            entity.ToTable("character_skills");
            entity.HasKey(skill => skill.Id);

            entity.Property(skill => skill.SkillType)
                .HasConversion<string>()
                .HasMaxLength(60)
                .IsRequired();

            entity.Property(skill => skill.BaseAttribute)
                .HasConversion<string>()
                .HasMaxLength(60)
                .IsRequired();

            entity.HasIndex(skill => new { skill.CharacterId, skill.SkillType })
                .IsUnique();

            entity.HasOne(skill => skill.Character)
                .WithMany(character => character.Skills)
                .HasForeignKey(skill => skill.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CharacterAttack>(entity =>
        {
            entity.ToTable("character_attacks");
            entity.HasKey(attack => attack.Id);

            entity.Property(attack => attack.Name)
                .HasMaxLength(140)
                .IsRequired();

            entity.Property(attack => attack.Damage)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(attack => attack.DamageType)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(attack => attack.Range)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(attack => attack.UsesAttribute)
                .HasConversion<string>()
                .HasMaxLength(60);

            entity.Property(attack => attack.Notes)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasIndex(attack => attack.CharacterId);

            entity.HasOne(attack => attack.Character)
                .WithMany(character => character.Attacks)
                .HasForeignKey(attack => attack.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CharacterCondition>(entity =>
        {
            entity.ToTable("character_conditions");
            entity.HasKey(condition => condition.Id);

            entity.Property(condition => condition.ConditionType)
                .HasConversion<string>()
                .HasMaxLength(60)
                .IsRequired();

            entity.Property(condition => condition.Name)
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(condition => condition.Description)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(condition => condition.Notes)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasIndex(condition => new { condition.CharacterId, condition.ConditionType })
                .IsUnique();

            entity.HasOne(condition => condition.Character)
                .WithMany(character => character.Conditions)
                .HasForeignKey(condition => condition.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CharacterNote>(entity =>
        {
            entity.ToTable("character_notes");
            entity.HasKey(note => note.Id);

            entity.Property(note => note.Title)
                .HasMaxLength(180)
                .IsRequired();

            entity.Property(note => note.Content)
                .HasMaxLength(10000)
                .IsRequired();

            entity.Property(note => note.Category)
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(note => note.Tags)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasIndex(note => note.CharacterId);
            entity.HasIndex(note => note.Category);

            entity.HasOne(note => note.Character)
                .WithMany(character => character.Notes)
                .HasForeignKey(note => note.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CharacterInventoryItem>(entity =>
        {
            entity.ToTable("character_inventory_items");
            entity.HasKey(item => item.Id);

            entity.Property(item => item.Name)
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(item => item.Description)
                .HasMaxLength(1500)
                .IsRequired();

            entity.Property(item => item.Weight)
                .HasPrecision(10, 2);

            entity.Property(item => item.Value)
                .HasPrecision(10, 2);

            entity.Property(item => item.ItemType)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(item => item.Notes)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasIndex(item => item.CharacterId);
            entity.HasIndex(item => item.ItemType);

            entity.HasOne(item => item.Character)
                .WithMany(character => character.InventoryItems)
                .HasForeignKey(item => item.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CharacterAsset>(entity =>
        {
            entity.ToTable("character_assets");
            entity.HasKey(asset => asset.Id);

            entity.Property(asset => asset.FileName)
                .HasMaxLength(220)
                .IsRequired();

            entity.Property(asset => asset.FileUrl)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(asset => asset.FileType)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(asset => asset.AssetType)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.HasIndex(asset => asset.CharacterId);
            entity.HasIndex(asset => asset.AssetType);

            entity.HasOne(asset => asset.Character)
                .WithMany(character => character.Assets)
                .HasForeignKey(asset => asset.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CharacterSpell>(entity =>
        {
            entity.ToTable("character_spells");
            entity.HasKey(characterSpell => characterSpell.Id);

            entity.Property(characterSpell => characterSpell.Notes)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasIndex(characterSpell => new { characterSpell.CharacterId, characterSpell.SpellId })
                .IsUnique();

            entity.HasIndex(characterSpell => characterSpell.SpellId);

            entity.HasOne(characterSpell => characterSpell.Character)
                .WithMany(character => character.Spells)
                .HasForeignKey(characterSpell => characterSpell.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(characterSpell => characterSpell.Spell)
                .WithMany()
                .HasForeignKey(characterSpell => characterSpell.SpellId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CharacterSpellSlot>(entity =>
        {
            entity.ToTable("character_spell_slots");
            entity.HasKey(slot => slot.Id);

            entity.HasIndex(slot => new { slot.CharacterId, slot.SpellLevel })
                .IsUnique();

            entity.HasOne(slot => slot.Character)
                .WithMany(character => character.SpellSlots)
                .HasForeignKey(slot => slot.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CharacterFeature>(entity =>
        {
            entity.ToTable("character_features");
            entity.HasKey(feature => feature.Id);

            entity.Property(feature => feature.CustomName)
                .HasMaxLength(180)
                .IsRequired();

            entity.Property(feature => feature.CustomDescription)
                .HasMaxLength(10000)
                .IsRequired();

            entity.Property(feature => feature.RecoveryType)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(feature => feature.Notes)
                .HasMaxLength(1000)
                .IsRequired();

            entity.HasIndex(feature => feature.CharacterId);
            entity.HasIndex(feature => feature.FeatureId);

            entity.HasOne(feature => feature.Character)
                .WithMany(character => character.Features)
                .HasForeignKey(feature => feature.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(feature => feature.Feature)
                .WithMany()
                .HasForeignKey(feature => feature.FeatureId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Spell>(entity =>
        {
            entity.ToTable("spells");
            entity.HasKey(spell => spell.Id);

            entity.Property(spell => spell.Name)
                .HasMaxLength(180)
                .IsRequired();

            entity.Property(spell => spell.EnglishName).HasMaxLength(180).IsRequired();
            entity.Property(spell => spell.School).HasMaxLength(80).IsRequired();
            entity.Property(spell => spell.CastingTime).HasMaxLength(120).IsRequired();
            entity.Property(spell => spell.Range).HasMaxLength(120).IsRequired();
            entity.Property(spell => spell.Components).HasMaxLength(120).IsRequired();
            entity.Property(spell => spell.Material).HasMaxLength(500).IsRequired();
            entity.Property(spell => spell.Duration).HasMaxLength(120).IsRequired();
            entity.Property(spell => spell.Description).HasMaxLength(8000).IsRequired();
            entity.Property(spell => spell.HigherLevelDescription).HasMaxLength(4000).IsRequired();
            entity.Property(spell => spell.AvailableClasses).HasMaxLength(500).IsRequired();
            entity.Property(spell => spell.Source).HasMaxLength(160).IsRequired();
            entity.Property(spell => spell.ExternalSource).HasMaxLength(80);
            entity.Property(spell => spell.ExternalId).HasMaxLength(180);
            entity.Property(spell => spell.Slug).HasMaxLength(180);
            entity.Property(spell => spell.RulesVersion).HasMaxLength(80);
            entity.Property(spell => spell.Language).HasMaxLength(20).HasDefaultValue("pt-BR").IsRequired();

            entity.Property(spell => spell.Visibility)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.HasIndex(spell => spell.Name);
            entity.HasIndex(spell => spell.Level);
            entity.HasIndex(spell => spell.School);
            entity.HasIndex(spell => spell.Visibility);
            entity.HasIndex(spell => spell.CampaignId);
            entity.HasIndex(spell => spell.CreatedByUserId);
            entity.HasIndex(spell => new { spell.ExternalSource, spell.ExternalId })
                .IsUnique()
                .HasFilter("\"ExternalSource\" IS NOT NULL AND \"ExternalId\" IS NOT NULL");
            entity.HasIndex(spell => spell.IsImported);
            entity.HasIndex(spell => spell.IsSrd);

            entity.HasOne(spell => spell.CreatedByUser)
                .WithMany()
                .HasForeignKey(spell => spell.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(spell => spell.Campaign)
                .WithMany()
                .HasForeignKey(spell => spell.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.ToTable("features");
            entity.HasKey(feature => feature.Id);

            entity.Property(feature => feature.Name)
                .HasMaxLength(180)
                .IsRequired();

            entity.Property(feature => feature.Type)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(feature => feature.Description)
                .HasMaxLength(10000)
                .IsRequired();

            entity.Property(feature => feature.Source)
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(feature => feature.Prerequisites)
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(feature => feature.Visibility)
                .HasConversion<string>()
                .HasMaxLength(40)
                .IsRequired();

            entity.HasIndex(feature => feature.Name);
            entity.HasIndex(feature => feature.Type);
            entity.HasIndex(feature => feature.Source);
            entity.HasIndex(feature => feature.Visibility);
            entity.HasIndex(feature => feature.CampaignId);
            entity.HasIndex(feature => feature.CreatedByUserId);

            entity.HasOne(feature => feature.CreatedByUser)
                .WithMany()
                .HasForeignKey(feature => feature.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(feature => feature.Campaign)
                .WithMany()
                .HasForeignKey(feature => feature.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Race>(entity =>
        {
            entity.ToTable("races");
            entity.HasKey(race => race.Id);
            entity.Property(race => race.Name).HasMaxLength(180).IsRequired();
            entity.Property(race => race.Description).HasMaxLength(4000).IsRequired();
            entity.Property(race => race.Source).HasMaxLength(160).IsRequired();
            entity.HasIndex(race => race.Name);
            entity.HasIndex(race => race.CreatedByUserId);
            entity.HasOne(race => race.CreatedByUser)
                .WithMany()
                .HasForeignKey(race => race.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CharacterClass>(entity =>
        {
            entity.ToTable("character_classes");
            entity.HasKey(characterClass => characterClass.Id);
            entity.Property(characterClass => characterClass.Name).HasMaxLength(180).IsRequired();
            entity.Property(characterClass => characterClass.Description).HasMaxLength(4000).IsRequired();
            entity.Property(characterClass => characterClass.Source).HasMaxLength(160).IsRequired();
            entity.HasIndex(characterClass => characterClass.Name);
            entity.HasIndex(characterClass => characterClass.CreatedByUserId);
            entity.HasOne(characterClass => characterClass.CreatedByUser)
                .WithMany()
                .HasForeignKey(characterClass => characterClass.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Background>(entity =>
        {
            entity.ToTable("backgrounds");
            entity.HasKey(background => background.Id);
            entity.Property(background => background.Name).HasMaxLength(180).IsRequired();
            entity.Property(background => background.Description).HasMaxLength(4000).IsRequired();
            entity.Property(background => background.Source).HasMaxLength(160).IsRequired();
            entity.HasIndex(background => background.Name);
            entity.HasIndex(background => background.CreatedByUserId);
            entity.HasOne(background => background.CreatedByUser)
                .WithMany()
                .HasForeignKey(background => background.CreatedByUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
