using TanErp.Application.Estimates;

namespace TanErp.Application.Estimates.UpdateEstimateDraft;

public sealed record UpdateEstimateDraftCommand(
    string FirebaseUid,
    Guid MembershipId,
    Guid EstimateId,
    Guid RevisionId,
    Guid ExpectedRevisionVersion,
    IReadOnlyList<EstimateSectionDraftDto> Sections);
