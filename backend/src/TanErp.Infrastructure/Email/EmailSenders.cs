using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using TanErp.Application.Common.Abstractions;

namespace TanErp.Infrastructure.Email;

/// <summary>Section <c>Email</c>. Password comes from env/user-secrets only; never commit it.</summary>
public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public bool Enabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseStartTls { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;

    public void Validate()
    {
        if (!Enabled) return;
        if (string.IsNullOrWhiteSpace(Host) || string.IsNullOrWhiteSpace(FromAddress)) throw new InvalidOperationException("Email:Host and Email:FromAddress are required when Email:Enabled is true.");
    }
}

/// <summary>System.Net.Mail SMTP sender (no extra package). shortcut: SmtpClient is basic; move to MailKit if OAuth or connection pooling is needed.</summary>
public sealed class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(EmailOptions options, ILogger<SmtpEmailSender> logger)
    {
        _options = options;
        _logger = logger;
    }

    public bool IsEnabled => true;

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        try
        {
            using var mail = new MailMessage { From = new MailAddress(_options.FromAddress, _options.FromName), Subject = message.Subject, Body = message.TextBody };
            mail.To.Add(message.To);
            using var client = new SmtpClient(_options.Host, _options.Port) { EnableSsl = _options.UseStartTls };
            if (!string.IsNullOrEmpty(_options.Username)) client.Credentials = new NetworkCredential(_options.Username, _options.Password);
            await client.SendMailAsync(mail, ct);
        }
        catch (Exception ex) when (ex is SmtpException or InvalidOperationException or FormatException)
        {
            // Log the failure kind only: the message body (an OTP) and recipient are never written.
            _logger.LogError("Email delivery failed ({ExceptionType}).", ex.GetType().Name);
            throw;
        }
    }
}

/// <summary>Used when Email:Enabled is false. Logs that a mail would have gone out, never its content.</summary>
public sealed class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public bool IsEnabled => false;

    public Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        _logger.LogInformation("Email sending is disabled; a message was not sent.");
        return Task.CompletedTask;
    }
}
