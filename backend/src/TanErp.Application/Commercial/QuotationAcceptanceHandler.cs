using System.Security.Cryptography;
using System.Text;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;

namespace TanErp.Application.Commercial;

/// <summary>
/// External acceptance. Internal staff create and revoke links; the customer uses the token without an ERP account.
/// Every public failure that could reveal whether a token exists (unknown, expired, revoked, replaced) returns the same code.
/// </summary>
public partial class QuotationAcceptanceHandler
{
    public const int DefaultLifetimeDays = 7;
    public const string UnavailableCode = "ACCEPTANCE_LINK_UNAVAILABLE";

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IQuotationAcceptanceStore _store;
    private readonly IEstimateStore _estimates;
    private readonly IClock _clock;
    private readonly IQuotationDocumentStore _documents;
    private readonly AcceptanceOtpOptions _otp;
    private readonly IAcceptanceOtpStore _otpStore;
    private readonly IEmailSender _email;

    public QuotationAcceptanceHandler(
        IRequestAccessResolver accessResolver, IQuotationAcceptanceStore store, IEstimateStore estimates, IClock clock, IQuotationDocumentStore documents,
        AcceptanceOtpOptions otp, IAcceptanceOtpStore otpStore, IEmailSender email)
    {
        _otp = otp;
        _otpStore = otpStore;
        _email = email;
        _accessResolver = accessResolver;
        _store = store;
        _estimates = estimates;
        _clock = clock;
        _documents = documents;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    public static string HashToken(string token) => Sha256Hex.Compute(token);

    private static string NewToken() => Base64Url(RandomNumberGenerator.GetBytes(32));

    private static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    // ----- internal ----------------------------------------------------------------------------------

    public async Task<Result<CreatedAcceptanceLink>> CreateLinkAsync(QuotationLifecycleCaller caller, Guid quotationId, int? lifetimeDays, string? signerHint, string? recipientEmail, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "quotations.share", ct);
        if (access.IsFailure) return Result<CreatedAcceptanceLink>.Failure(access.Error);
        var days = lifetimeDays ?? DefaultLifetimeDays;
        if (days is < 1 or > 30) return Fail<CreatedAcceptanceLink>("ACCEPTANCE_LINK_LIFETIME_INVALID", "A link must live between 1 and 30 days.");
        if (signerHint is { Length: > 200 }) return Fail<CreatedAcceptanceLink>("ACCEPTANCE_LINK_LIFETIME_INVALID", "The signer hint cannot exceed 200 characters.");

        if (_otp.Required && string.IsNullOrWhiteSpace(recipientEmail))
        {
            return Fail<CreatedAcceptanceLink>("ACCEPTANCE_RECIPIENT_EMAIL_REQUIRED", "A recipient email is required because acceptance needs a one-time code.");
        }

        var token = NewToken();
        var created = await _store.CreateLinkAsync(access.Value!, quotationId, HashToken(token), signerHint, recipientEmail, days, caller.TraceId, ct);
        return created.IsFailure ? Result<CreatedAcceptanceLink>.Failure(created.Error) : Result<CreatedAcceptanceLink>.Success(new CreatedAcceptanceLink(created.Value!, token));
    }

    public async Task<Result<AcceptanceLinkProjection>> RevokeLinkAsync(QuotationLifecycleCaller caller, Guid linkId, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "quotations.share", ct);
        if (access.IsFailure) return Result<AcceptanceLinkProjection>.Failure(access.Error);
        return await _store.RevokeLinkAsync(access.Value!, linkId, caller.TraceId, ct);
    }

    public async Task<Result<IReadOnlyList<AcceptanceLinkProjection>>> ListLinksAsync(QuotationLifecycleCaller caller, Guid quotationId, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "quotations.read", ct);
        if (access.IsFailure) return Result<IReadOnlyList<AcceptanceLinkProjection>>.Failure(access.Error);
        var links = await _store.ListLinksAsync(access.Value!.OrganizationId, quotationId, ct);
        return links is null ? Fail<IReadOnlyList<AcceptanceLinkProjection>>("RESOURCE_NOT_FOUND", "Quotation not found.") : Result<IReadOnlyList<AcceptanceLinkProjection>>.Success(links);
    }

    // ----- public ------------------------------------------------------------------------------------

    private async Task<AcceptanceContext?> UsableContextAsync(string? token, bool recoverPartialAcceptance, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token) || token.Length is < 20 or > 128) return null;
        var context = await _store.FindContextAsync(HashToken(token.Trim()), ct);
        if (context is null) return null;

        // An accepted link stays viewable to the token holder (idempotent refresh); everything else must still be live.
        if (context.LinkStatus == "accepted") return context;
        if (context.LinkStatus != "active" || context.ExpiresAtUtc <= _clock.UtcNow) return null;
        // After a crash between accepting and recording the evidence, the quotation is already accepted through this very link.
        // Submitting again lets the keyed accept replay and finish the record.
        if (context.QuotationStatus == "issued" || (recoverPartialAcceptance && context.QuotationStatus == "accepted")) return context;
        return null;
    }

    public async Task<Result<PublicAcceptanceView>> GetPublicViewAsync(string? token, string? locale, CancellationToken ct = default)
    {
        var context = await UsableContextAsync(token, false, ct);
        if (context is null) return Fail<PublicAcceptanceView>(UnavailableCode, "This acceptance link is not available.");

        var document = await _estimates.GetQuotationDocumentAsync(context.OrganizationId, context.EstimateId, locale ?? "th", ct);
        if (document.IsFailure) return Fail<PublicAcceptanceView>(UnavailableCode, "This acceptance link is not available.");
        if (document.Value!.Number != context.QuotationNumber) return Fail<PublicAcceptanceView>(UnavailableCode, "This acceptance link is not available.");

        var pdf = await _documents.FindSummaryAsync(context.OrganizationId, context.QuotationId, ct);
        return Result<PublicAcceptanceView>.Success(new PublicAcceptanceView(
            context.LinkStatus, context.ExpiresAtUtc, context.SignerHint, AcceptanceConsent.CurrentVersion, context.AcceptedAtUtc, document.Value,
            pdf is null ? null : new PublicQuotationPdf(pdf.Sha256, pdf.ByteCount), _otp.Required));
    }

    /// <summary>The exact stored PDF of the quotation this link is for. Same unusable-link rules (and the same 404) as the view.</summary>
    public async Task<Result<QuotationDocumentFile>> GetPublicDocumentAsync(string? token, CancellationToken ct = default)
    {
        var context = await UsableContextAsync(token, false, ct);
        var file = context is null ? null : await _documents.OpenForAcceptanceAsync(context.OrganizationId, context.QuotationId, ct);
        return file is null ? Fail<QuotationDocumentFile>(UnavailableCode, "This acceptance link is not available.") : Result<QuotationDocumentFile>.Success(file);
    }

    public async Task<Result<PublicAcceptanceResult>> AcceptAsync(string? token, AcceptanceSubmission? submission, ClientInfo client, string traceId, CancellationToken ct = default)
    {
        var context = await UsableContextAsync(token, true, ct);
        if (context is null) return Fail<PublicAcceptanceResult>(UnavailableCode, "This acceptance link is not available.");

        // A second submission on an accepted link just shows the original result.
        if (context.LinkStatus == "accepted" && context.Evidence is not null && context.AcceptedAtUtc.HasValue)
        {
            return Result<PublicAcceptanceResult>.Success(new PublicAcceptanceResult("accepted", context.AcceptedAtUtc.Value, context.QuotationNumber, context.Evidence));
        }

        if (submission is null || !SignatureEvidenceRules.TryNormalizeSigner(submission.SignerName, submission.SignerRole, out var name, out var role))
        {
            return Fail<PublicAcceptanceResult>("ACCEPTANCE_SUBMISSION_INVALID", "A signer name of 2-200 characters is required.");
        }

        if (!submission.ConsentAccepted || submission.ConsentVersion != AcceptanceConsent.CurrentVersion)
        {
            return Fail<PublicAcceptanceResult>("ACCEPTANCE_CONSENT_REQUIRED", "The current consent statement must be accepted.");
        }

        string? signatureHash = null;
        if (!string.IsNullOrEmpty(submission.SignatureImage))
        {
            if (submission.SignatureImage.Length > AcceptanceConsent.MaxSignatureImageChars || !SignatureEvidenceRules.IsPngBase64(submission.SignatureImage))
            {
                return Fail<PublicAcceptanceResult>("ACCEPTANCE_SUBMISSION_INVALID", "The signature image must be a PNG of limited size.");
            }

            signatureHash = SignatureEvidenceRules.HashSubmittedImage(submission.SignatureImage);
        }

        var otp = await CheckVerificationForAcceptAsync(context, submission.OtpVerificationToken, ct);
        if (otp.IsFailure) return Result<PublicAcceptanceResult>.Failure(otp.Error);

        // The customer attests to the exact PDF they were shown: its hash must equal the stored one, and "no PDF" (null) only matches
        // a quotation that has none. A PDF that appeared after the page was loaded is a conflict: reload and review again.
        var storedPdf = await _documents.FindSummaryAsync(context.OrganizationId, context.QuotationId, ct);
        if (!string.Equals(submission.DocumentSha256, storedPdf?.Sha256, StringComparison.Ordinal))
        {
            return Fail<PublicAcceptanceResult>("ACCEPTANCE_CONFLICT", "The document changed while you were reviewing it. Please reload and try again.");
        }

        // The accept transaction is keyed by the link, so a retry after a partial failure replays instead of accepting twice.
        var accepted = await _estimates.AcceptQuotationAsync(
            context.OrganizationId, context.EstimateId, context.OpportunityRowVersion,
            $"Accepted by customer representative via acceptance link ({name}).",
            context.CreatedByUserId, Sha256Hex.Compute($"external-accept:{context.LinkId}"), Sha256Hex.Compute($"external-accept:{context.LinkId}:{context.QuotationId}"),
            traceId, ct, context.QuotationId);
        if (accepted.IsFailure)
        {
            return accepted.Error.Code is "OPPORTUNITY_VERSION_CONFLICT" or "ESTIMATE_VERSION_CONFLICT"
                ? Fail<PublicAcceptanceResult>("ACCEPTANCE_CONFLICT", "The document changed while you were reviewing it. Please reload and try again.")
                : Fail<PublicAcceptanceResult>(UnavailableCode, "This acceptance link is not available.");
        }

        var addressHash = Sha256Hex.Compute($"{client.RemoteAddress}|{context.LinkId}");
        var recorded = await _store.RecordAcceptanceAsync(
            context, submission with { SignerName = name, SignerRole = role },
            signatureHash ?? string.Empty, addressHash, client.UserAgent is { Length: > 200 } ua ? ua[..200] : client.UserAgent, traceId, otp.Value, ct);
        if (recorded.IsFailure) return Result<PublicAcceptanceResult>.Failure(recorded.Error);

        return Result<PublicAcceptanceResult>.Success(new PublicAcceptanceResult("accepted", recorded.Value!.AcceptedAtUtc, context.QuotationNumber, recorded.Value));
    }
}
