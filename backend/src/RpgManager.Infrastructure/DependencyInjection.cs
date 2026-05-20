using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RpgManager.Application.Auth;
using RpgManager.Application.Campaigns;
using RpgManager.Application.Characters;
using RpgManager.Application.CampaignNotes;
using RpgManager.Application.CharacterOptions;
using RpgManager.Application.Npcs;
using RpgManager.Infrastructure.Auth;
using RpgManager.Infrastructure.Campaigns;
using RpgManager.Infrastructure.CampaignNotes;
using RpgManager.Infrastructure.CharacterOptions;
using RpgManager.Infrastructure.Characters;
using RpgManager.Infrastructure.Npcs;
using RpgManager.Infrastructure.Data;
using RpgManager.Application.Storage;
using RpgManager.Infrastructure.Storage;
using RpgManager.Application.Spells;
using RpgManager.Infrastructure.Spells;
using RpgManager.Application.Features;
using RpgManager.Infrastructure.Features;
using RpgManager.Application.Dice;
using RpgManager.Infrastructure.Dice;
using RpgManager.Application.Permissions;
using RpgManager.Infrastructure.Permissions;

namespace RpgManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<LocalFileStorageOptions>(configuration.GetSection(LocalFileStorageOptions.SectionName));
        services.Configure<Open5eSpellImportOptions>(configuration.GetSection(Open5eSpellImportOptions.SectionName));

        var connectionString = DatabaseConnectionStringFactory.FromConfiguration(
            configuration["DATABASE_URL"],
            configuration.GetConnectionString("DefaultConnection"));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICampaignPermissionService, CampaignPermissionService>();
        services.AddScoped<ICharacterPermissionService, CharacterPermissionService>();
        services.AddScoped<IContentVisibilityService, ContentVisibilityService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<ICampaignNoteService, CampaignNoteService>();
        services.AddScoped<ICharacterOptionService, CharacterOptionService>();
        services.AddScoped<INpcService, NpcService>();
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddScoped<ISpellService, SpellService>();
        services.AddScoped<ISpellImportService, SpellImportService>();
        services.AddScoped<IFeatureService, FeatureService>();
        services.AddScoped<IDiceService, DiceService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddHttpClient<IOpen5eSpellClient, Open5eSpellClient>((serviceProvider, client) =>
        {
            var options = serviceProvider
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<Open5eSpellImportOptions>>()
                .Value;
            client.Timeout = TimeSpan.FromSeconds(Math.Max(5, options.TimeoutSeconds));
        });

        return services;
    }
}
