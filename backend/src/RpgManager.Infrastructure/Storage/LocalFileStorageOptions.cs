namespace RpgManager.Infrastructure.Storage;

public sealed class LocalFileStorageOptions
{
    public const string SectionName = "LocalFileStorage";

    public string RootPath { get; set; } = "wwwroot/uploads";
    public string PublicBasePath { get; set; } = "/uploads";
    public long MaxBytes { get; set; } = 5 * 1024 * 1024;
}
