using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using RpgManager.Application.Storage;

namespace RpgManager.Infrastructure.Storage;

public sealed class LocalFileStorageService(
    IOptions<LocalFileStorageOptions> options,
    IHostEnvironment environment) : IFileStorageService
{
    private static readonly IReadOnlyDictionary<string, string> AllowedExtensions = new Dictionary<string, string>
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp"
    };

    public async Task<StoredFile> SaveAsync(
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        var storageOptions = options.Value;
        if (fileStream.Length > storageOptions.MaxBytes)
        {
            throw new InvalidOperationException("Arquivo excede o tamanho máximo permitido.");
        }

        var extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        if (!AllowedExtensions.TryGetValue(extension, out var expectedContentType))
        {
            throw new InvalidOperationException("Tipo de arquivo não permitido.");
        }

        if (!string.Equals(contentType, expectedContentType, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Tipo de arquivo inválido.");
        }

        var rootPath = GetRootPath(storageOptions);
        Directory.CreateDirectory(rootPath);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(rootPath, fileName);

        await using var output = File.Create(filePath);
        fileStream.Position = 0;
        await fileStream.CopyToAsync(output, cancellationToken);

        var publicBasePath = storageOptions.PublicBasePath.TrimEnd('/');
        return new StoredFile(fileName, $"{publicBasePath}/{fileName}", expectedContentType);
    }

    public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            return Task.CompletedTask;
        }

        var storageOptions = options.Value;
        var publicBasePath = storageOptions.PublicBasePath.TrimEnd('/');
        if (!fileUrl.StartsWith($"{publicBasePath}/", StringComparison.OrdinalIgnoreCase))
        {
            return Task.CompletedTask;
        }

        var fileName = Path.GetFileName(fileUrl);
        var filePath = Path.Combine(GetRootPath(storageOptions), fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    private string GetRootPath(LocalFileStorageOptions storageOptions)
        => Path.IsPathRooted(storageOptions.RootPath)
            ? Path.GetFullPath(storageOptions.RootPath)
            : Path.GetFullPath(Path.Combine(environment.ContentRootPath, storageOptions.RootPath));
}
