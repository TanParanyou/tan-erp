using TanErp.Application.QuickEstimates;

namespace TanErp.Api.Contracts.QuickEstimates;

public sealed record BranchRateOverrideRequest(decimal ReferenceRate, decimal MinimumCharge, DateOnly EffectiveFrom, DateOnly EffectiveTo, string? ReasonCode, string? ReasonDetail, string? SourceReference);
public sealed record BranchRateOverrideDecisionRequest(string? Decision, string? Note);
public sealed record BranchRateOverrideRevokeRequest(string? Reason);
public sealed record BranchRateOverrideResponse(
    Guid Id, Guid TemplateId, Guid BranchId, string BranchCode, string Status, decimal ReferenceRate, decimal MinimumCharge, DateOnly EffectiveFrom, DateOnly EffectiveTo,
    string ReasonCode, string ReasonDetail, string? SourceReference, QePersonResponse RequestedBy, DateTimeOffset RequestedAtUtc, QePersonResponse? DecidedBy, DateTimeOffset? DecidedAtUtc, string? DecisionNote, Guid RowVersion);
public sealed record BranchRateOverrideListResponse(IReadOnlyList<BranchRateOverrideResponse> Items);

internal static class BranchRateOverrideMapper
{
    public static BranchRateOverrideResponse To(BranchOverrideProjection x) => new(x.Id, x.TemplateId, x.BranchId, x.BranchCode, x.Status, x.ReferenceRate, x.MinimumCharge, x.EffectiveFrom, x.EffectiveTo,
        x.ReasonCode, x.ReasonDetail, x.SourceReference, QuickEstimateMapper.To(x.RequestedBy), x.RequestedAtUtc, x.DecidedBy is null ? null : QuickEstimateMapper.To(x.DecidedBy), x.DecidedAtUtc, x.DecisionNote, x.RowVersion);
}
