namespace TanErp.Domain.Files;

public static class UploadedFileStatus
{
    public const string Verified = "verified";
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
