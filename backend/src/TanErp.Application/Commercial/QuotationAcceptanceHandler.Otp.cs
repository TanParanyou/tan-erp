using System.Security.Cryptography;
using System.Text;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Domain.Commercial;

namespace TanErp.Application.Commercial;

// G-05: one-time code over email before a customer may accept. See docs/adr/0029-external-acceptance-otp.md.
public partial class QuotationAcceptanceHandler
{
    private async Task<Result<AcceptanceContext>> OtpContextAsync(string? token, CancellationToken ct)
    {
        // Unknown/expired/revoked/accepted links keep the single 404 answer; "OTP not in use" is a separate, non-secret answer.
        var context = await UsableContextAsync(token, false, ct);
        if (context is null || context.LinkStatus != AcceptanceLinkStatus.Active) return Fail<AcceptanceContext>(UnavailableCode, "This acceptance link is not available.");
        if (!_otp.Required || !_email.IsEnabled) return Fail<AcceptanceContext>("ACCEPTANCE_OTP_UNAVAILABLE", "Verification codes are not available right now.");
        if (context.OtpLocked) return Fail<AcceptanceContext>("ACCEPTANCE_OTP_LOCKED", "Too many incorrect codes. Ask the sender for a new link.");
        return Result<AcceptanceContext>.Success(context);
    }

    private string HashCode(Guid linkId, string code) =>
        Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(_otp.HmacKey), Encoding.UTF8.GetBytes($"{linkId:N}:{code}"))).ToLowerInvariant();

    /// <summary>Always the same answer for a usable link, whether or not an email is on file, so the address is never revealed.</summary>
    public async Task<Result<OtpRequestResult>> RequestOtpAsync(string? token, string traceId, CancellationToken ct = default)
    {
        var ctx = await OtpContextAsync(token, ct);
        if (ctx.IsFailure) return Result<OtpRequestResult>.Failure(ctx.Error);
        var context = ctx.Value!;

        var now = _clock.UtcNow;
        var stats = await _otpStore.GetSendStatsAsync(context.LinkId, now.AddHours(-1), ct);
        if (stats.SendsInWindow >= _otp.MaxSendsPerHour || (stats.LastSentAtUtc is { } last && last.AddSeconds(_otp.ResendCooldownSeconds) > now))
        {
            return Fail<OtpRequestResult>("ACCEPTANCE_OTP_RATE_LIMITED", "Please wait before requesting another code.");
        }

        var code = RandomNumberGenerator.GetInt32(0, (int)Math.Pow(10, _otp.CodeLength)).ToString($"D{_otp.CodeLength}");
        await _otpStore.CreateChallengeAsync(context, OtpChannels.Email, HashCode(context.LinkId, code), now, now.AddMinutes(_otp.ExpiryMinutes), traceId, ct);

        if (!string.IsNullOrEmpty(context.RecipientEmail))
        {
            try
            {
                await _email.SendAsync(new EmailMessage(context.RecipientEmail, OtpEmailTemplate.Subject(context.QuotationNumber), OtpEmailTemplate.Body(context.QuotationNumber, code, _otp.ExpiryMinutes)), ct);
            }
            catch (Exception) when (!ct.IsCancellationRequested)
            {
                return Fail<OtpRequestResult>("ACCEPTANCE_OTP_UNAVAILABLE", "Verification codes are not available right now.");
            }
        }

        return Result<OtpRequestResult>.Success(new OtpRequestResult(_otp.ResendCooldownSeconds, _otp.ExpiryMinutes * 60));
    }

    public async Task<Result<OtpVerifyResult>> VerifyOtpAsync(string? token, string? code, string traceId, CancellationToken ct = default)
    {
        var ctx = await OtpContextAsync(token, ct);
        if (ctx.IsFailure) return Result<OtpVerifyResult>.Failure(ctx.Error);
        var context = ctx.Value!;

        var now = _clock.UtcNow;
        var challenge = await _otpStore.FindActiveChallengeAsync(context.LinkId, now, ct);
        // No live challenge covers never-requested, expired and already-used alike.
        if (challenge is null) return Fail<OtpVerifyResult>("ACCEPTANCE_OTP_EXPIRED", "The code has expired. Request a new one.");

        var trimmed = code?.Trim() ?? string.Empty;
        if (trimmed.Length != _otp.CodeLength || !trimmed.All(char.IsAsciiDigit)) return Fail<OtpVerifyResult>("ACCEPTANCE_OTP_INVALID", "The code is not correct.");

        var matches = CryptographicOperations.FixedTimeEquals(Encoding.ASCII.GetBytes(HashCode(context.LinkId, trimmed)), Encoding.ASCII.GetBytes(challenge.CodeHash));
        if (!matches)
        {
            var locked = await _otpStore.RegisterFailureAsync(context, challenge.Id, _otp.MaxAttempts, now, traceId, ct);
            return locked
                ? Fail<OtpVerifyResult>("ACCEPTANCE_OTP_LOCKED", "Too many incorrect codes. Ask the sender for a new link.")
                : Fail<OtpVerifyResult>("ACCEPTANCE_OTP_INVALID", "The code is not correct.");
        }

        var verificationToken = NewToken();
        await _otpStore.MarkVerifiedAsync(context, challenge.Id, HashToken(verificationToken), now, traceId, ct);
        return Result<OtpVerifyResult>.Success(new OtpVerifyResult(verificationToken, now.AddMinutes(_otp.VerificationMinutes)));
    }

    /// <summary>Null evidence when OTP is off; otherwise the accept call must carry the token returned by verify.</summary>
    private async Task<Result<OtpEvidence?>> CheckVerificationForAcceptAsync(AcceptanceContext context, string? verificationToken, CancellationToken ct)
    {
        if (!_otp.Required) return Result<OtpEvidence?>.Success(null);
        if (context.OtpLocked) return Fail<OtpEvidence?>("ACCEPTANCE_OTP_LOCKED", "Too many incorrect codes. Ask the sender for a new link.");
        if (string.IsNullOrWhiteSpace(verificationToken)) return Fail<OtpEvidence?>("ACCEPTANCE_OTP_REQUIRED", "A verification code is required before accepting.");

        var record = await _otpStore.FindVerificationAsync(context.LinkId, HashToken(verificationToken.Trim()), ct);
        if (record is null) return Fail<OtpEvidence?>("ACCEPTANCE_OTP_INVALID", "The verification is not valid. Request a new code.");
        if (record.VerifiedAtUtc.AddMinutes(_otp.VerificationMinutes) <= _clock.UtcNow) return Fail<OtpEvidence?>("ACCEPTANCE_OTP_EXPIRED", "The verification has expired. Request a new code.");
        return Result<OtpEvidence?>.Success(new OtpEvidence(OtpChannels.Email, record.VerifiedAtUtc, record.ChallengeId));
    }
}

/// <summary>Bilingual (Thai first) plain-text mail. The code is the only secret in it.</summary>
internal static class OtpEmailTemplate
{
    public static string Subject(string quotationNumber) => $"รหัสยืนยันการอนุมัติใบเสนอราคา {quotationNumber} / Quotation acceptance code";

    public static string Body(string quotationNumber, string code, int minutes) =>
        $"รหัสยืนยันสำหรับใบเสนอราคา {quotationNumber}: {code}\nรหัสมีอายุ {minutes} นาที อย่าบอกรหัสนี้กับผู้อื่น\n\n" +
        $"Your verification code for quotation {quotationNumber}: {code}\nIt expires in {minutes} minutes. Do not share it with anyone.";
}
