using Microsoft.Extensions.Logging;
using TanErp.Application.Common.Abstractions;

namespace TanErp.Infrastructure.Email;

/// <summary>
/// Section <c>Sms</c>, reserved for a future provider (G-05). Nothing reads <see cref="Provider"/> yet: to plug one in, implement
/// <see cref="ISmsSender"/>, register it when <see cref="Enabled"/> is true, and set <c>AcceptanceOtp:Channels:Sms=true</c>.
/// </summary>
public sealed class SmsOptions
{
    public const string SectionName = "Sms";

    public bool Enabled { get; set; }
    public string Provider { get; set; } = string.Empty;
}

/// <summary>The only SMS sender today. Logs that a text would have gone out, never its content or recipient.</summary>
public sealed class LoggingSmsSender : ISmsSender
{
    private readonly ILogger<LoggingSmsSender> _logger;

    public LoggingSmsSender(ILogger<LoggingSmsSender> logger)
    {
        _logger = logger;
    }

    public bool IsEnabled => false;

    public Task SendAsync(SmsMessage message, CancellationToken ct = default)
    {
        _logger.LogInformation("SMS sending is not configured; a message was not sent.");
        return Task.CompletedTask;
    }
}
