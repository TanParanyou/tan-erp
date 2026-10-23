namespace TanErp.Application.Common.Abstractions;

public sealed record EmailMessage(string To, string Subject, string TextBody);

/// <summary>
/// Outbound email port. <see cref="IsEnabled"/> is false for the logging no-op used in dev/test; callers that depend on delivery
/// (e.g. acceptance OTP) must treat a disabled sender as "feature unavailable" instead of pretending a mail was sent.
/// </summary>
public interface IEmailSender
{
    bool IsEnabled { get; }

    Task SendAsync(EmailMessage message, CancellationToken ct = default);
}
