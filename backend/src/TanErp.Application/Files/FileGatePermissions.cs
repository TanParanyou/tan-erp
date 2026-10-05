using TanErp.Application.Attachments;

namespace TanErp.Application.Files;

/// <summary>
/// First-gate permission lists for the Files module (an actor needs at least one). The parent access resolver still
/// checks the exact permission for the specific parent. Owner types registered for shared attachments extend both lists.
/// </summary>
public static class FileGatePermissions
{
    private static readonly string[] UploadBase =
    [
        "opportunities.update",
        "customers.update",
        "customers.create",
        "sites.update",
        "sites.create",
        "items.manage-images",
        "items.update"
    ];

    private static readonly string[] ReadBase =
    [
        "opportunities.read",
        "opportunities.update",
        "customers.read",
        "customers.update",
        "customers.create",
        "sites.read",
        "sites.update",
        "sites.create",
        "items.read",
        "items.manage-images",
        "items.update"
    ];

    public static IReadOnlyCollection<string> Upload { get; } = [.. UploadBase, .. AttachmentOwnerRegistry.ManagePermissions];

    public static IReadOnlyCollection<string> Read { get; } =
        [.. ReadBase, .. AttachmentOwnerRegistry.ReadPermissions, .. AttachmentOwnerRegistry.ManagePermissions];
}
