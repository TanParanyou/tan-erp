using TanErp.Api.Contracts.Estimates;

namespace TanErp.Api.Contracts.Commercial;

public sealed record CreateAcceptanceLinkRequest(int? LifetimeDays, string? SignerHint, string? RecipientEmail = null, string? RecipientPhone = null);

/// <summary>Identifies the quotation PDF offered to the customer; the file itself is served by the public document endpoint.</summary>
public sealed record PublicQuotationPdfResponse(string Sha256, long ByteCount);

public sealed record AcceptanceEvidenceResponse(string SignerName, string? SignerRole, string ConsentVersion, bool HasSignatureImage, string? SignatureHash, DateTimeOffset AcceptedAtUtc);

/// <summary>Both fields are fixed numbers from configuration, identical for every link.</summary>
public sealed record PublicOtpRequestResponse(int ResendAfterSeconds, int ExpiresInSeconds);

public sealed record PublicOtpVerifyRequest(string? Code);

public sealed record PublicOtpVerifyResponse(string VerificationToken, DateTimeOffset ExpiresAtUtc);

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
    QuotationDocumentResponse Document,
    PublicQuotationPdfResponse? Pdf,
    bool OtpRequired,
    IReadOnlyList<string> OtpChannels);

public sealed record PublicOtpRequest(string? Channel);

public sealed record PublicAcceptRequest(string? SignerName, string? SignerRole, bool ConsentAccepted, string? ConsentVersion, string? SignatureImage, string? DocumentSha256 = null, string? OtpVerificationToken = null);

public sealed record PublicAcceptanceResponse(string Status, DateTimeOffset AcceptedAtUtc, string QuotationNumber, AcceptanceEvidenceResponse Evidence);
