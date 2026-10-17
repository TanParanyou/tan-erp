namespace TanErp.Application.QuickEstimates;

public sealed record BranchOverrideInput(decimal ReferenceRate, decimal MinimumCharge, DateOnly EffectiveFrom, DateOnly EffectiveTo, string? ReasonCode, string? ReasonDetail, string? SourceReference);

public sealed record BranchOverrideProjection(
    Guid Id, Guid TemplateId, Guid BranchId, string BranchCode, string Status, decimal ReferenceRate, decimal MinimumCharge, DateOnly EffectiveFrom, DateOnly EffectiveTo,
    string ReasonCode, string ReasonDetail, string? SourceReference, QePerson RequestedBy, DateTimeOffset RequestedAtUtc, QePerson? DecidedBy, DateTimeOffset? DecidedAtUtc, string? DecisionNote, Guid RowVersion);
