using TanErp.Application.Estimates.GetQuotationDocument;

namespace TanErp.Application.Commercial;

/// <summary>Current wording version the customer consents to. Evidence records which version was shown.</summary>
public static class AcceptanceConsent
{
    public const string CurrentVersion = "2026-10-v1";
    public const int MaxSignatureImageChars = 150_000;
}

public sealed record AcceptanceEvidenceSummary(string SignerName, string? SignerRole, string ConsentVersion, bool HasSignatureImage, string? SignatureHash, DateTimeOffset AcceptedAtUtc);

public sealed record AcceptanceLinkProjection(
    Guid Id,
    Guid QuotationId,
    string QuotationNumber,
    string Status,
    string QuotationStatus,
    bool IsUsable,
    string? SignerHint,
    DateTimeOffset ExpiresAtUtc,
    DateTimeOffset CreatedAtUtc,
    QuotationPersonRef CreatedBy,
    DateTimeOffset? RevokedAtUtc,
    DateTimeOffset? AcceptedAtUtc,
    AcceptanceEvidenceSummary? Evidence);

/// <summary>A freshly created link. The token exists only in this result.</summary>
public sealed record CreatedAcceptanceLink(AcceptanceLinkProjection Link, string Token);

public sealed record AcceptanceContext(
    Guid LinkId,
    Guid OrganizationId,
    Guid QuotationId,
    string QuotationNumber,
    string QuotationStatus,
    Guid EstimateId,
    Guid OpportunityRowVersion,
    Guid CreatedByUserId,
    string LinkStatus,
    DateTimeOffset ExpiresAtUtc,
    string? SignerHint,
    DateTimeOffset? AcceptedAtUtc,
    AcceptanceEvidenceSummary? Evidence);

public sealed record PublicAcceptanceView(
    string Status,
    DateTimeOffset ExpiresAtUtc,
    string? SignerHint,
    string ConsentVersion,
    DateTimeOffset? AcceptedAtUtc,
    QuotationDocumentProjection Document);

public sealed record AcceptanceSubmission(
    string? SignerName,
    string? SignerRole,
    bool ConsentAccepted,
    string? ConsentVersion,
    string? SignatureImage);

public sealed record ClientInfo(string? RemoteAddress, string? UserAgent);

public sealed record PublicAcceptanceResult(string Status, DateTimeOffset AcceptedAtUtc, string QuotationNumber, AcceptanceEvidenceSummary Evidence);
