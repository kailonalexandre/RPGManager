using Npgsql;

namespace RpgManager.Infrastructure.Data;

public static class DatabaseConnectionStringFactory
{
    public static string FromConfiguration(string? databaseUrl, string? defaultConnectionString)
    {
        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            return FromDatabaseUrl(databaseUrl);
        }

        if (!string.IsNullOrWhiteSpace(defaultConnectionString))
        {
            return defaultConnectionString;
        }

        throw new InvalidOperationException("Database connection is missing. Set DATABASE_URL or ConnectionStrings__DefaultConnection.");
    }

    public static string FromEnvironment()
    {
        return FromConfiguration(
            Environment.GetEnvironmentVariable("DATABASE_URL"),
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"));
    }

    private static string FromDatabaseUrl(string databaseUrl)
    {
        if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException("DATABASE_URL must be a valid PostgreSQL URL.");
        }

        if (uri.Scheme is not ("postgres" or "postgresql"))
        {
            throw new InvalidOperationException("DATABASE_URL must use the postgres:// or postgresql:// scheme.");
        }

        var userInfo = uri.UserInfo.Split(':', 2);
        var query = ParseQuery(uri.Query);
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Database = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/')),
            Username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty,
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            SslMode = ResolveSslMode(query)
        };

        if (!uri.IsDefaultPort)
        {
            builder.Port = uri.Port;
        }

        return builder.ConnectionString;
    }

    private static SslMode ResolveSslMode(Dictionary<string, string> query)
    {
        if (!query.TryGetValue("sslmode", out var sslMode))
        {
            return SslMode.Require;
        }

        return sslMode.ToLowerInvariant() switch
        {
            "disable" => SslMode.Disable,
            "prefer" => SslMode.Prefer,
            "require" => SslMode.Require,
            "verify-ca" => SslMode.VerifyCA,
            "verify-full" => SslMode.VerifyFull,
            _ => SslMode.Require
        };
    }

    private static Dictionary<string, string> ParseQuery(string query)
    {
        return query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part => part.Split('=', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(
                parts => Uri.UnescapeDataString(parts[0]).ToLowerInvariant(),
                parts => Uri.UnescapeDataString(parts[1]));
    }
}
