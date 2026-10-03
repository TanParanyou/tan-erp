using TanErp.Domain.Common;

namespace TanErp.Domain.Commercial;

public static class AcceptanceLinkStatus
{
    public const string Active = "active";
    public const string Revoked = "revoked";
    public const string Accepted = "accepted";
}

public class AcceptanceLinkException : Exception
{
    public string Code { get; }

    public AcceptanceLinkException(string code, string message) : base(message)
    {
        Code = code;
    }
}

/// <summary>
/// A time-limited link that lets a customer representative review and accept one specific quotation without an ERP login.
/// Only the SHA-256 of the token is stored; the token itself is shown to the creator once.
/// </summary>
public class QuotationAcceptanceLink : Entity
{
    public const int MaxLifetimeDays = 30;

    public Guid OrganizationId { get; private set; }
    public Guid QuotationId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public string? SignerHint { get; private set; }
    public string Status { get; private set; } = AcceptanceLinkStatus.Active;
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }
    public Guid? RevokedByUserId { get; private set; }
    public DateTimeOffset? AcceptedAtUtc { get; private set; }

    protected QuotationAcceptanceLink() { }

    public QuotationAcceptanceLink(Guid id, Guid organizationId, Guid quotationId, string tokenHash, string? signerHint, DateTimeOffset expiresAtUtc, Guid createdByUserId, DateTimeOffset now) : base(id)
    {
        if (organizationId == Guid.Empty || quotationId == Guid.Empty || createdByUserId == Guid.Empty) throw new ArgumentException("Organization, quotation and actor are required.");
        if (string.IsNullOrWhiteSpace(tokenHash) || tokenHash.Length != 64) throw new ArgumentException("A SHA-256 token hash is required.", nameof(tokenHash));
        if (expiresAtUtc <= now || expiresAtUtc > now.AddDays(MaxLifetimeDays))
        {
            throw new AcceptanceLinkException("ACCEPTANCE_LINK_LIFETIME_INVALID", $"A link must expire within {MaxLifetimeDays} days and not in the past.");
        }

        var hint = string.IsNullOrWhiteSpace(signerHint) ? null : signerHint.Trim();
        if (hint is { Length: > 200 }) throw new AcceptanceLinkException("ACCEPTANCE_LINK_LIFETIME_INVALID", "The signer hint cannot exceed 200 characters.");

        OrganizationId = organizationId;
        QuotationId = quotationId;
        TokenHash = tokenHash;
        SignerHint = hint;
        ExpiresAtUtc = expiresAtUtc.ToUniversalTime();
        CreatedByUserId = createdByUserId;
        CreatedAtUtc = now.ToUniversalTime();
    }

    public bool IsUsable(DateTimeOffset now) => Status == AcceptanceLinkStatus.Active && ExpiresAtUtc > now;

    public void Revoke(Guid actorUserId, DateTimeOffset now)
    {
        if (Status != AcceptanceLinkStatus.Active)
        {
            throw new AcceptanceLinkException("ACCEPTANCE_LINK_INVALID_STATE", $"Only an active link can be revoked; current status is '{Status}'.");
        }

        Status = AcceptanceLinkStatus.Revoked;
        RevokedByUserId = actorUserId;
        RevokedAtUtc = now.ToUniversalTime();
    }

    public void MarkAccepted(DateTimeOffset now)
    {
        if (Status != AcceptanceLinkStatus.Active)
        {
            throw new AcceptanceLinkException("ACCEPTANCE_LINK_INVALID_STATE", $"Only an active link can be accepted; current status is '{Status}'.");
        }

        Status = AcceptanceLinkStatus.Accepted;
        AcceptedAtUtc = now.ToUniversalTime();
    }
}

/// <summary>What the customer representative declared and when, kept next to the link so the acceptance can be audited later.</summary>
public class QuotationAcceptanceEvidence : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid LinkId { get; private set; }
    public Guid QuotationId { get; private set; }
    public string SignerName { get; private set; } = string.Empty;
    public string? SignerRole { get; private set; }
    public string ConsentVersion { get; private set; } = string.Empty;
    public string? SignatureImage { get; private set; }
    public string? SignatureHash { get; private set; }
    public string ClientAddressHash { get; private set; } = string.Empty;
    public string? UserAgent { get; private set; }
    public DateTimeOffset AcceptedAtUtc { get; private set; }

    protected QuotationAcceptanceEvidence() { }

    public QuotationAcceptanceEvidence(
        Guid id, Guid organizationId, Guid linkId, Guid quotationId, string signerName, string? signerRole, string consentVersion,
        string? signatureImage, string? signatureHash, string clientAddressHash, string? userAgent, DateTimeOffset now) : base(id)
    {
        OrganizationId = organizationId;
        LinkId = linkId;
        QuotationId = quotationId;
        SignerName = signerName;
        SignerRole = signerRole;
        ConsentVersion = consentVersion;
        SignatureImage = signatureImage;
        SignatureHash = signatureHash;
        ClientAddressHash = clientAddressHash;
        UserAgent = userAgent;
        AcceptedAtUtc = now.ToUniversalTime();
    }
}
