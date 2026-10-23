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

    /// <summary>Where the one-time code is sent (G-05). Never returned to the customer or written to the audit trail.</summary>
    public string? RecipientEmail { get; private set; }

    /// <summary>E.164 phone for the SMS channel, typed by staff (G-05). Same confidentiality rules as <see cref="RecipientEmail"/>.</summary>
    public string? RecipientPhone { get; private set; }

    /// <summary>Set after too many wrong codes; only a newly issued link clears it.</summary>
    public DateTimeOffset? OtpLockedAtUtc { get; private set; }

    protected QuotationAcceptanceLink() { }

    private static readonly System.Text.RegularExpressions.Regex PhonePattern = new(@"^\+[1-9]\d{7,14}$", System.Text.RegularExpressions.RegexOptions.Compiled);

    /// <summary>Drops the separators people type (space, dash, dot, parentheses); null for blank input.</summary>
    public static string? NormalizePhone(string? raw) =>
        string.IsNullOrWhiteSpace(raw) ? null : new string(raw.Where(c => c is not (' ' or '-' or '.' or '(' or ')')).ToArray());

    public QuotationAcceptanceLink(Guid id, Guid organizationId, Guid quotationId, string tokenHash, string? signerHint, DateTimeOffset expiresAtUtc, Guid createdByUserId, DateTimeOffset now, string? recipientEmail = null, string? recipientPhone = null) : base(id)
    {
        if (organizationId == Guid.Empty || quotationId == Guid.Empty || createdByUserId == Guid.Empty) throw new ArgumentException("Organization, quotation and actor are required.");
        if (string.IsNullOrWhiteSpace(tokenHash) || tokenHash.Length != 64) throw new ArgumentException("A SHA-256 token hash is required.", nameof(tokenHash));
        if (expiresAtUtc <= now || expiresAtUtc > now.AddDays(MaxLifetimeDays))
        {
            throw new AcceptanceLinkException("ACCEPTANCE_LINK_LIFETIME_INVALID", $"A link must expire within {MaxLifetimeDays} days and not in the past.");
        }

        var hint = string.IsNullOrWhiteSpace(signerHint) ? null : signerHint.Trim();
        if (hint is { Length: > 200 }) throw new AcceptanceLinkException("ACCEPTANCE_LINK_LIFETIME_INVALID", "The signer hint cannot exceed 200 characters.");

        var email = string.IsNullOrWhiteSpace(recipientEmail) ? null : recipientEmail.Trim();
        if (email is not null && (email.Length > 254 || !System.Net.Mail.MailAddress.TryCreate(email, out var parsed) || parsed.Address != email))
        {
            throw new AcceptanceLinkException("ACCEPTANCE_LINK_LIFETIME_INVALID", "The recipient email is not valid.");
        }

        var phone = NormalizePhone(recipientPhone);
        if (phone is not null && !PhonePattern.IsMatch(phone)) throw new AcceptanceLinkException("ACCEPTANCE_LINK_LIFETIME_INVALID", "The recipient phone must be in international format, e.g. +66812345678.");

        OrganizationId = organizationId;
        QuotationId = quotationId;
        RecipientEmail = email;
        RecipientPhone = phone;
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

    public void LockOtp(DateTimeOffset now) => OtpLockedAtUtc ??= now.ToUniversalTime();

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

    /// <summary>SHA-256 of the quotation PDF the customer attested to; null when the quotation had no stored PDF (issued before G-04).</summary>
    public string? DocumentSha256 { get; private set; }

    /// <summary>How the signer's access was verified (G-05), e.g. "email"; null when the link did not require OTP. The code itself is never stored here.</summary>
    public string? OtpChannel { get; private set; }
    public DateTimeOffset? OtpVerifiedAtUtc { get; private set; }
    public Guid? OtpChallengeId { get; private set; }

    protected QuotationAcceptanceEvidence() { }

    public QuotationAcceptanceEvidence(
        Guid id, Guid organizationId, Guid linkId, Guid quotationId, string signerName, string? signerRole, string consentVersion,
        string? signatureImage, string? signatureHash, string clientAddressHash, string? userAgent, DateTimeOffset now, string? documentSha256 = null,
        string? otpChannel = null, DateTimeOffset? otpVerifiedAtUtc = null, Guid? otpChallengeId = null) : base(id)
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
        DocumentSha256 = string.IsNullOrWhiteSpace(documentSha256) ? null : documentSha256.Trim();
        AcceptedAtUtc = now.ToUniversalTime();
        OtpChannel = otpChannel;
        OtpVerifiedAtUtc = otpVerifiedAtUtc?.ToUniversalTime();
        OtpChallengeId = otpChallengeId;
        OtpChannel = otpChannel;
        OtpVerifiedAtUtc = otpVerifiedAtUtc?.ToUniversalTime();
        OtpChallengeId = otpChallengeId;
    }
}

public static class OtpChannels
{
    /// <summary>Add new channels here and a matching sender; the DB column is bounded by a CHECK constraint (extend it in a migration).</summary>
    public const string Email = "email";
    public const string Sms = "sms";

    public static readonly IReadOnlyList<string> All = [Email, Sms];
}

/// <summary>
/// One one-time code issued for an acceptance link. Only an HMAC of the code is kept. After a correct code the challenge becomes a
/// short-lived "verification" (token hash) that the accept call must present; accepting consumes it.
/// </summary>
public class AcceptanceOtpChallenge : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid LinkId { get; private set; }
    public string Channel { get; private set; } = OtpChannels.Email;
    public string CodeHash { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public int FailedAttempts { get; private set; }
    public DateTimeOffset? VerifiedAtUtc { get; private set; }
    public string? VerificationTokenHash { get; private set; }
    public DateTimeOffset? ConsumedAtUtc { get; private set; }

    protected AcceptanceOtpChallenge() { }

    public AcceptanceOtpChallenge(Guid id, Guid organizationId, Guid linkId, string channel, string codeHash, DateTimeOffset now, DateTimeOffset expiresAtUtc) : base(id)
    {
        OrganizationId = organizationId;
        LinkId = linkId;
        Channel = channel;
        CodeHash = codeHash;
        CreatedAtUtc = now.ToUniversalTime();
        ExpiresAtUtc = expiresAtUtc.ToUniversalTime();
    }

    public void MarkVerified(string verificationTokenHash, DateTimeOffset now)
    {
        VerifiedAtUtc = now.ToUniversalTime();
        VerificationTokenHash = verificationTokenHash;
    }

    public void Consume(DateTimeOffset now) => ConsumedAtUtc = now.ToUniversalTime();
}
