using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TanErp.Application.Common.Abstractions;

namespace TanErp.Infrastructure.Email;

/// <summary>
/// Section <c>Sms</c> (G-05). Supports <c>thaibulksms</c>, <c>twilio</c>, and <c>webhook</c> providers.
/// Disabled by default (<see cref="Enabled"/> = <c>false</c>).
/// </summary>
public sealed class SmsOptions
{
    public const string SectionName = "Sms";

    public bool Enabled { get; set; }
    public string Provider { get; set; } = "thaibulksms";
    public string? EndpointUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? ApiSecret { get; set; }
    public string SenderName { get; set; } = "TAN-ERP";

    public void Validate()
    {
        if (!Enabled) return;

        var provider = string.IsNullOrWhiteSpace(Provider) ? "thaibulksms" : Provider.Trim().ToLowerInvariant();
        if (provider is "thaibulksms" or "twilio")
        {
            if (string.IsNullOrWhiteSpace(ApiKey) || string.IsNullOrWhiteSpace(ApiSecret))
            {
                throw new InvalidOperationException($"Sms:ApiKey and Sms:ApiSecret are required when Sms:Enabled is true for provider '{provider}'.");
            }
        }
        else if (provider == "webhook")
        {
            if (string.IsNullOrWhiteSpace(EndpointUrl))
            {
                throw new InvalidOperationException("Sms:EndpointUrl is required when Sms:Enabled is true for provider 'webhook'.");
            }
        }
        else
        {
            throw new InvalidOperationException($"Unsupported SMS provider '{Provider}'. Supported providers: thaibulksms, twilio, webhook.");
        }
    }
}

/// <summary>HTTP SMS gateway sender supporting ThaiBulkSMS API v2, Twilio Programmable Messaging, and generic JSON Webhook (G-05).</summary>
public sealed class HttpSmsSender : ISmsSender
{
    public const string HttpClientName = "SmsGateway";

    private readonly SmsOptions _options;
    private readonly Func<HttpClient>? _httpClientFactory;
    private readonly HttpClient? _directHttpClient;
    private readonly ILogger<HttpSmsSender>? _logger;

    public HttpSmsSender(
        SmsOptions options,
        HttpClient? httpClient = null,
        ILogger<HttpSmsSender>? logger = null,
        Func<HttpClient>? httpClientFactory = null)
    {
        _options = options ?? new SmsOptions();
        _directHttpClient = httpClient;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public bool IsEnabled => _options.Enabled;

    public async Task SendAsync(SmsMessage message, CancellationToken ct = default)
    {
        if (!_options.Enabled)
        {
            return;
        }

        var provider = string.IsNullOrWhiteSpace(_options.Provider)
            ? "thaibulksms"
            : _options.Provider.Trim().ToLowerInvariant();
        var senderName = string.IsNullOrWhiteSpace(_options.SenderName) ? "TAN-ERP" : _options.SenderName.Trim();

        HttpRequestMessage request;
        if (provider == "thaibulksms")
        {
            var url = string.IsNullOrWhiteSpace(_options.EndpointUrl)
                ? "https://api-v2.thaibulksms.com/sms"
                : _options.EndpointUrl.Trim();
            request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["msisdn"] = message.To,
                    ["message"] = message.Text,
                    ["sender"] = senderName
                })
            };
            var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ApiKey}:{_options.ApiSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
        }
        else if (provider == "twilio")
        {
            var url = string.IsNullOrWhiteSpace(_options.EndpointUrl)
                ? $"https://api.twilio.com/2010-04-01/Accounts/{_options.ApiKey}/Messages.json"
                : _options.EndpointUrl.Trim();
            request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["To"] = message.To,
                    ["From"] = senderName,
                    ["Body"] = message.Text
                })
            };
            var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ApiKey}:{_options.ApiSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
        }
        else
        {
            var url = _options.EndpointUrl?.Trim()
                ?? throw new InvalidOperationException("Sms:EndpointUrl is required for webhook SMS provider.");
            var json = JsonSerializer.Serialize(new
            {
                to = message.To,
                message = message.Text,
                sender = senderName
            });
            request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            if (!string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                request.Headers.TryAddWithoutValidation("X-Api-Key", _options.ApiKey.Trim());
            }
        }

        using (request)
        {
            try
            {
                var client = _directHttpClient ?? _httpClientFactory?.Invoke() ?? new HttpClient();
                using var response = await client.SendAsync(request, ct);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or InvalidOperationException)
            {
                // Log only the failure kind; never log the OTP body or recipient phone number.
                _logger?.LogError("SMS delivery failed via provider '{Provider}' ({ExceptionType}).", provider, ex.GetType().Name);
                throw;
            }
        }
    }
}

/// <summary>Used when <c>Sms:Enabled</c> is false. Logs that a text would have gone out, never its content or recipient.</summary>
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
