using System.Security.Cryptography;
using System.Text;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;

namespace TanErp.Application.Commercial;

/// <summary>
/// External acceptance. Internal staff create and revoke links; the customer uses the token without an ERP account.
/// Every public failure that could reveal whether a token exists (unknown, expired, revoked, replaced) returns the same code.
/// </summary>
public class QuotationAcceptanceHandler
{
    public const int DefaultLifetimeDays = 7;
    public const string UnavailableCode = "ACCEPTANCE_LINK_UNAVAILABLE";

    private readonly IRequestAccessResolver _accessResolver;
    private readonly IQuotationAcceptanceStore _store;
    private readonly IEstimateStore _estimates;
    private readonly IClock _clock;

    public QuotationAcceptanceHandler(IRequestAccessResolver accessResolver, IQuotationAcceptanceStore store, IEstimateStore estimates, IClock clock)
    {
        _accessResolver = accessResolver;
        _store = store;
        _estimates = estimates;
        _clock = clock;
    }

    private static Result<T> Fail<T>(string code, string message) => Result<T>.Failure(new Error(code, message));

    public static string HashToken(string token) => Sha256Hex.Compute(token);

    private static string NewToken() => Base64Url(RandomNumberGenerator.GetBytes(32));

    private static string Base64Url(byte[] bytes) => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    // ----- internal ----------------------------------------------------------------------------------

    public async Task<Result<CreatedAcceptanceLink>> CreateLinkAsync(QuotationLifecycleCaller caller, Guid quotationId, int? lifetimeDays, string? signerHint, CancellationToken ct = default)
    {
        var access = await _accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "quotations.share", ct);
        if (access.IsFailure) return Result<CreatedAcceptanceLink>.Failure(access.Error);
        var days = lifetimeDays ?? DefaultLifetimeDays;
        if (days is < 1 or > 30) return Fail<CreatedAcceptanceLink>("ACCEPTANCE_LINK_LIFETIME_INVALID", "A link must live between 1 and 30 days.");
        if (signerHint is { Length: > 200 }) return Fail<CreatedAcceptanceLink>("ACCEPTANCE_LINK_LIFETIME_INVALID", "The signer hint cannot exceed 200 characters.");

        var token = NewToken();
        var created = await _store.CreateLinkAsync(access.Value!, quotationId, HashToken(token), signerHint, days, caller.TraceId, ct);
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

        return Result<PublicAcceptanceView>.Success(new PublicAcceptanceView(context.LinkStatus, context.ExpiresAtUtc, context.SignerHint, AcceptanceConsent.CurrentVersion, context.AcceptedAtUtc, document.Value));
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

        var name = submission?.SignerName?.Trim();
        if (submission is null || string.IsNullOrWhiteSpace(name) || name.Length is < 2 or > 200 || submission.SignerRole is { Length: > 100 })
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
            if (submission.SignatureImage.Length > AcceptanceConsent.MaxSignatureImageChars || !IsPngBase64(submission.SignatureImage))
            {
                return Fail<PublicAcceptanceResult>("ACCEPTANCE_SUBMISSION_INVALID", "The signature image must be a PNG of limited size.");
            }

            signatureHash = Sha256Hex.Compute(submission.SignatureImage);
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
            context, submission with { SignerName = name, SignerRole = string.IsNullOrWhiteSpace(submission.SignerRole) ? null : submission.SignerRole.Trim() },
            signatureHash ?? string.Empty, addressHash, client.UserAgent is { Length: > 200 } ua ? ua[..200] : client.UserAgent, traceId, ct);
        if (recorded.IsFailure) return Result<PublicAcceptanceResult>.Failure(recorded.Error);

        return Result<PublicAcceptanceResult>.Success(new PublicAcceptanceResult("accepted", recorded.Value!.AcceptedAtUtc, context.QuotationNumber, recorded.Value));
    }

    private static bool IsPngBase64(string value)
    {
        // data URLs are accepted; the payload must decode and start with the PNG signature.
        var payload = value.StartsWith("data:image/png;base64,", StringComparison.Ordinal) ? value["data:image/png;base64,".Length..] : value;
        try
        {
            var bytes = Convert.FromBase64String(payload);
            return bytes.Length > 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
