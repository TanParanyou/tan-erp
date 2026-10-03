using TanErp.Api.Contracts.Estimates;

namespace TanErp.Api.Contracts.Commercial;

public sealed record CreateAcceptanceLinkRequest(int? LifetimeDays, string? SignerHint);

public sealed record AcceptanceEvidenceResponse(string SignerName, string? SignerRole, string ConsentVersion, bool HasSignatureImage, string? SignatureHash, DateTimeOffset AcceptedAtUtc);

public sealed record AcceptanceLinkResponse(
    Guid Id,
    Guid QuotationId,
    string QuotationNumber,
    string Status,
    string QuotationStatus,
    bool IsUsable,
    string? SignerHint,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset CreatedAtUtc,
    QuotationPersonResponse CreatedBy,
    DateTimeOffset? RevokedAtUtc,
    DateTimeOffset? AcceptedAtUtc,
    AcceptanceEvidenceResponse? Evidence);

public sealed record AcceptanceLinkListResponse(IReadOnlyList<AcceptanceLinkResponse> Items);

/// <summary>The token appears only here, once, when the link is created.</summary>
public sealed record CreatedAcceptanceLinkResponse(AcceptanceLinkResponse Link, string Token, string PublicPath);

public sealed record PublicAcceptanceViewResponse(
    string Status,
    DateTimeOffset ExpiresAtUtc,
    string? SignerHint,
    string ConsentVersion,
    DateTimeOffset? AcceptedAtUtc,
    QuotationDocumentResponse Document);

public sealed record PublicAcceptRequest(string? SignerName, string? SignerRole, bool ConsentAccepted, string? ConsentVersion, string? SignatureImage);

public sealed record PublicAcceptanceResponse(string Status, DateTimeOffset AcceptedAtUtc, string QuotationNumber, AcceptanceEvidenceResponse Evidence);
