namespace TanErp.Application.Commercial;

public sealed record QuotationLifecycleCaller(string FirebaseUid, Guid MembershipId, string TraceId);

public sealed record QuotationPersonRef(Guid Id, string DisplayName);

/// <summary>One quotation of an estimate, including how it relates to the others.</summary>
public sealed record QuotationHistoryItem(
    Guid Id,
    string Number,
    string Status,
    decimal TotalAmount,
    Guid EstimateRevisionId,
    int EstimateRevisionNo,
    DateTimeOffset IssuedAtUtc,
    DateTimeOffset? AcceptedAtUtc,
    Guid? SupersedesQuotationId,
    string? SupersedesNumber,
    Guid? SupersededByQuotationId,
    string? SupersededByNumber,
    string? AmendmentReason,
    DateTimeOffset? VoidedAtUtc,
    QuotationPersonRef? VoidedBy,
    string? VoidReason,
    Guid RowVersion);

public sealed record QuotationHistoryProjection(Guid EstimateId, IReadOnlyList<QuotationHistoryItem> Items);
