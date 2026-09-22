namespace TanErp.Domain.Files;

public static class UploadedFileStatus
{
    public const string Verified = "verified";
}

public static class FileScanStatus
{
    public const string Pending = "pending";
    public const string ContentVerified = "content_verified";
    public const string Clean = "clean";
    public const string Infected = "infected";
}

public static class AllowedMediaTypes
{
    public const string WebP = "image/webp";
    public const string Jpeg = "image/jpeg";
    public const string Png = "image/png";

    private static readonly HashSet<string> Allowed = [WebP, Jpeg, Png];

    public static bool IsAllowed(string mediaType) =>
        Allowed.Contains(mediaType.Trim().ToLowerInvariant());
}
