using TanErp.Domain.Attachments;
using TanErp.Domain.QuickEstimates;
using TanErp.Domain.Service;
using TanErp.Domain.Surveys;

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
            SignStates: new HashSet<string>(StringComparer.Ordinal) { InstallationStatus.ReadyForHandover }),
        // Survey area evidence: editable only while the revision is a draft on an area-evidence template; never signed.
        new AttachmentOwnerDescriptor(
            AttachmentOwnerTypes.SurveyArea,
            ReadPermission: "surveys.read",
            ManagePermission: "surveys.update",
            SignPermission: "surveys.update",
            ManageStates: new HashSet<string>(StringComparer.Ordinal) { SurveyRevisionStatus.Draft },
            SignStates: new HashSet<string>(StringComparer.Ordinal)),
        // Quick estimates have no signatures: SignPermission mirrors the manage permission and SignStates is empty, so a capture is refused with ATTACHMENT_OWNER_LOCKED.
        new AttachmentOwnerDescriptor(
            AttachmentOwnerTypes.QuickEstimate,
            ReadPermission: "quick-estimates.read",
            ManagePermission: "quick-estimates.update",
            SignPermission: "quick-estimates.update",
            ManageStates: new HashSet<string>(StringComparer.Ordinal) { QuickEstimateStatus.Draft, QuickEstimateStatus.Calculated, QuickEstimateStatus.Returned },
            SignStates: new HashSet<string>(StringComparer.Ordinal)),
        new AttachmentOwnerDescriptor(
            AttachmentOwnerTypes.ServiceRequest,
            ReadPermission: "service-requests.read",
            ManagePermission: "service-requests.manage",
            SignPermission: "service-requests.manage",
            ManageStates: new HashSet<string>(StringComparer.Ordinal) { ServiceRequestStatus.Open, ServiceRequestStatus.Scheduled, ServiceRequestStatus.InProgress, ServiceRequestStatus.WaitingCustomer, ServiceRequestStatus.Resolved },
            SignStates: new HashSet<string>(StringComparer.Ordinal) { ServiceRequestStatus.Resolved })
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
