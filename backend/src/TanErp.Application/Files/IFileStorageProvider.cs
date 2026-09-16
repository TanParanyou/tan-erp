namespace TanErp.Application.Files;

/// <summary>
/// Storage provider abstraction — decouples file binary storage from business logic.
/// Concrete implementations: LocalFileStorageProvider (dev/test), S3FileStorageProvider (prod).
/// </summary>
public interface IFileStorageProvider
{
    /// <summary>
    /// Saves the binary content and returns the storage path.
    /// </summary>
    Task<string> SaveAsync(
        Guid organizationId,
        string sessionId,
        string filename,
        Stream content,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a URL or relative path suitable for serving the file.
    /// </summary>
    string GetServingUrl(string storagePath);

    /// <summary>
    /// Deletes the file binary. Idempotent — does not throw if not found.
    /// </summary>
    Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default);
}
