using Microsoft.Extensions.Configuration;
using TanErp.Application.Files;

namespace TanErp.Infrastructure.Files;

/// <summary>
/// Stores file binaries on the local filesystem under Storage:BasePath.
/// Directory structure: {BasePath}/{organizationId}/{sessionId}/{filename}
/// Suitable for development and self-hosted deployments.
/// Replace with S3FileStorageProvider for cloud deployments.
/// </summary>
public class LocalFileStorageProvider : IFileStorageProvider
{
    private readonly string _basePath;

    public LocalFileStorageProvider(IConfiguration configuration)
    {
        var basePath = configuration["Storage:BasePath"];
        if (string.IsNullOrWhiteSpace(basePath))
        {
            // Default to a subdirectory next to the app in development
            basePath = Path.Combine(AppContext.BaseDirectory, "local-storage");
        }
        _basePath = basePath;
    }

    public async Task<string> SaveAsync(
        Guid organizationId,
        string sessionId,
        string filename,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        var sanitizedFilename = SanitizeFilename(filename);
        var relativePath = Path.Combine(
            organizationId.ToString("D"),
            sessionId,
            sanitizedFilename);

        var fullPath = Path.Combine(_basePath, relativePath);
        var directory = Path.GetDirectoryName(fullPath)!;

        Directory.CreateDirectory(directory);

        await using var fileStream = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await content.CopyToAsync(fileStream, cancellationToken);

        // Normalize path separators for cross-platform consistency
        return relativePath.Replace('\\', '/');
    }

    public string GetServingUrl(string storagePath) =>
        $"/api/v1/files/{storagePath}";

    public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, storagePath.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        return Task.CompletedTask;
    }

    private static string SanitizeFilename(string filename)
    {
        // Replace any directory traversal characters and reserved chars
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", filename.Split(invalid));

        // Prefix with a short timestamp + entropy to avoid name collision within a session
        var prefix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString("x8");
        return $"{prefix}_{sanitized}";
    }
}
