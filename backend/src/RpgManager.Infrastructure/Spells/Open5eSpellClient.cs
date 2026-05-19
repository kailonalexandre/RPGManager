using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RpgManager.Infrastructure.Spells;

public interface IOpen5eSpellClient
{
    Task<Open5eSpellPage?> GetPageAsync(string url, CancellationToken cancellationToken);
}

public sealed class Open5eSpellClient(
    HttpClient httpClient,
    ILogger<Open5eSpellClient> logger) : IOpen5eSpellClient
{
    public async Task<Open5eSpellPage?> GetPageAsync(string url, CancellationToken cancellationToken)
    {
        logger.LogInformation("Consultando Open5e: {Url}", url);

        using var response = await httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Open5e respondeu {StatusCode} para {Url}", response.StatusCode, url);
            return null;
        }

        return await response.Content.ReadFromJsonAsync(Open5eJsonContext.Default.Open5eSpellPage, cancellationToken);
    }
}
