namespace TanErp.Api.Contracts.Commercial;

public sealed record QuotationReasonRequest(string? Reason);

public sealed record QuotationPersonResponse(Guid Id, string DisplayName);

public sealed record QuotationHistoryItemResponse(
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
    QuotationPersonResponse? VoidedBy,
    string? VoidReason,
    Guid RowVersion);

public sealed record QuotationHistoryResponse(Guid EstimateId, IReadOnlyList<QuotationHistoryItemResponse> Items);
