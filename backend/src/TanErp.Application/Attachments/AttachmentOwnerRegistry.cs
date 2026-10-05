using TanErp.Domain.Attachments;
using TanErp.Domain.Service;

namespace TanErp.Application.Attachments;

/// <summary>
/// What a registered owner type requires. Permissions are existing keys of the owner's own module; no attachment-specific keys exist.
/// </summary>
public sealed record AttachmentOwnerDescriptor(
    string OwnerType,
    string ReadPermission,
    string ManagePermission,
    string SignPermission,
    IReadOnlySet<string> ManageStates,
    IReadOnlySet<string> SignStates);

/// <summary>Code-defined registry. Must stay in step with AttachmentOwnerTypes (enforced by tests).</summary>
public static class AttachmentOwnerRegistry
{
    private static readonly IReadOnlyDictionary<string, AttachmentOwnerDescriptor> Descriptors = new[]
    {
        new AttachmentOwnerDescriptor(
            AttachmentOwnerTypes.InstallationJob,
            ReadPermission: "installations.read",
            ManagePermission: "installations.operate",
            SignPermission: "installations.handover",
            ManageStates: new HashSet<string>(StringComparer.Ordinal) { InstallationStatus.Planned, InstallationStatus.InProgress, InstallationStatus.ReadyForHandover },
            SignStates: new HashSet<string>(StringComparer.Ordinal) { InstallationStatus.ReadyForHandover })
    }.ToDictionary(d => d.OwnerType, StringComparer.Ordinal);

    public static IReadOnlyCollection<string> OwnerTypes { get; } = Descriptors.Keys.ToArray();

    /// <summary>Distinct permissions that allow reading attachments of any registered owner.</summary>
    public static IReadOnlyCollection<string> ReadPermissions { get; } = Descriptors.Values.Select(d => d.ReadPermission).Distinct(StringComparer.Ordinal).ToArray();

    /// <summary>Distinct permissions that allow uploading to (managing or signing for) any registered owner.</summary>
    public static IReadOnlyCollection<string> ManagePermissions { get; } = Descriptors.Values
        .SelectMany(d => new[] { d.ManagePermission, d.SignPermission })
        .Distinct(StringComparer.Ordinal)
        .ToArray();

    /// <summary>Exact (already normalized) owner type lookup.</summary>
    public static AttachmentOwnerDescriptor? Find(string? ownerType) =>
        ownerType is not null && Descriptors.TryGetValue(ownerType, out var descriptor) ? descriptor : null;
}
