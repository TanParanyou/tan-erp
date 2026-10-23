namespace TanErp.Application.Common.Abstractions;

public sealed record SmsMessage(string To, string Text);

/// <summary>
/// Outbound SMS port, channel-agnostic like <see cref="IEmailSender"/>. No provider exists yet: the only implementation is a logging no-op
/// whose <see cref="IsEnabled"/> is false, so callers (acceptance OTP) report the SMS channel as unavailable instead of pretending a text was sent.
/// </summary>
public interface ISmsSender
{
    bool IsEnabled { get; }

    Task SendAsync(SmsMessage message, CancellationToken ct = default);
}
