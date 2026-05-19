namespace RpgManager.Application.Storage;

public sealed record StoredFile(string FileName, string FileUrl, string ContentType);

public interface IFileStorageService
{
    Task<StoredFile> SaveAsync(
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken);

    Task DeleteAsync(string fileUrl, CancellationToken cancellationToken);
}
