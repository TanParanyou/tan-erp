using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TanErp.Application.Finance;

namespace TanErp.Infrastructure.Persistence.Finance;

public sealed class AccountingConnectorOptions
{
    public const string SectionName = "AccountingConnector";

    public bool Enabled { get; init; } = false;
    public string Provider { get; init; } = "local";
    public string? EndpointUrl { get; init; }
    public string? ApiKey { get; init; }
    public string? ClientId { get; init; }
    public string? ClientSecret { get; init; }
    public int TimeoutSeconds { get; init; } = 15;
}

/// <summary>
/// Configurable accounting connector supporting local ledger simulation, PEAK Account, FlowAccount, and signed HTTP Webhooks (G-17).
/// Disabled by default (<see cref="AccountingConnectorOptions.Enabled"/> = <c>false</c>) so outbox messages are never marked as sent
/// unless an administrator explicitly enables and configures an accounting provider.
/// </summary>
public sealed class ConfigurableAccountingConnector : IAccountingConnector
{
    public const string HttpClientName = "AccountingConnector";

    private readonly AccountingConnectorOptions _options;
    private readonly Func<HttpClient>? _httpClientFactory;
    private readonly HttpClient? _directHttpClient;
    private readonly ILogger<ConfigurableAccountingConnector>? _logger;

    public ConfigurableAccountingConnector(
        AccountingConnectorOptions options,
        HttpClient? httpClient = null,
        ILogger<ConfigurableAccountingConnector>? logger = null,
        Func<HttpClient>? httpClientFactory = null)
    {
        _options = options ?? new AccountingConnectorOptions();
        _directHttpClient = httpClient;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ConnectorResult> SendAsync(AccountingEnvelope envelope, CancellationToken ct = default)
    {
        if (!_options.Enabled)
        {
            return new ConnectorResult(false, null, "CONNECTOR_NOT_CONFIGURED");
        }

        var provider = (_options.Provider ?? "local").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(provider))
        {
            provider = "local";
        }

        if (provider == "local")
        {
            var suffix = string.IsNullOrEmpty(envelope.DedupeKey)
                ? "00000000"
                : envelope.DedupeKey[..Math.Min(8, envelope.DedupeKey.Length)];
            return new ConnectorResult(true, $"LOCAL-{envelope.ResourceNumber}-{suffix}", null);
        }

        if (string.IsNullOrWhiteSpace(_options.EndpointUrl))
        {
            return new ConnectorResult(false, null, "CONNECTOR_ENDPOINT_MISSING");
        }

        var occurredDate = (envelope.OccurredOn == default
            ? DateOnly.FromDateTime(DateTime.UtcNow)
            : envelope.OccurredOn).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        object payloadObj = provider switch
        {
            "peak" => new
            {
                provider = "peak",
                documentCode = envelope.ResourceNumber,
                eventKind = envelope.Kind,
                issuedDate = occurredDate,
                amount = envelope.Amount,
                dedupeKey = envelope.DedupeKey,
                data = envelope.PayloadJson
            },
            "flowaccount" => new
            {
                provider = "flowaccount",
                referenceDocument = envelope.ResourceNumber,
                category = envelope.Kind,
                publishedOn = occurredDate,
                grandTotal = envelope.Amount,
                dedupeKey = envelope.DedupeKey,
                rawPayload = envelope.PayloadJson
            },
            _ => new
            {
                messageId = envelope.MessageId,
                organizationId = envelope.OrganizationId,
                kind = envelope.Kind,
                resourceId = envelope.ResourceId,
                resourceNumber = envelope.ResourceNumber,
                amount = envelope.Amount,
                occurredOn = occurredDate,
                dedupeKey = envelope.DedupeKey,
                payloadJson = envelope.PayloadJson
            }
        };

        var jsonBody = JsonSerializer.Serialize(payloadObj);
        using var request = new HttpRequestMessage(HttpMethod.Post, _options.EndpointUrl.Trim())
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };

        request.Headers.TryAddWithoutValidation("Idempotency-Key", envelope.DedupeKey);

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            var apiKey = _options.ApiKey.Trim();
            if (provider == "peak")
            {
                request.Headers.TryAddWithoutValidation("Client-Token", apiKey);
            }
            else if (provider == "flowaccount")
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
            else
            {
                request.Headers.TryAddWithoutValidation("X-Api-Key", apiKey);
            }
        }

        if (!string.IsNullOrWhiteSpace(_options.ClientId))
        {
            request.Headers.TryAddWithoutValidation("X-Client-Id", _options.ClientId.Trim());
        }

        if (!string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            var signatureHex = ComputeHmacSha256Hex(_options.ClientSecret, jsonBody);
            request.Headers.TryAddWithoutValidation("X-TanErp-Signature", $"sha256={signatureHex}");
        }

        try
        {
            var client = _directHttpClient ?? _httpClientFactory?.Invoke() ?? new HttpClient();
            using var response = await client.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode)
            {
                return new ConnectorResult(false, null, $"CONNECTOR_HTTP_{(int)response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync(ct);
            var externalRef = ExtractExternalReference(responseBody) ?? $"{provider.ToUpperInvariant()}-{envelope.ResourceNumber}";
            return new ConnectorResult(true, externalRef, null);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException && !ct.IsCancellationRequested)
        {
            _logger?.LogWarning("Accounting connector HTTP delivery failed ({ExceptionType}).", ex.GetType().Name);
            return new ConnectorResult(false, null, "CONNECTOR_NETWORK_ERROR");
        }
    }

    public static string ComputeHmacSha256Hex(string secret, string payload)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var hash = HMACSHA256.HashData(keyBytes, payloadBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string? ExtractExternalReference(string? responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody)) return null;
        try
        {
            using var doc = JsonDocument.Parse(responseBody);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return null;

            foreach (var propName in new[] { "externalRef", "documentId", "id" })
            {
                if (doc.RootElement.TryGetProperty(propName, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.String)
                    {
                        var val = prop.GetString();
                        if (!string.IsNullOrWhiteSpace(val)) return val.Trim();
                    }
                    else if (prop.ValueKind == JsonValueKind.Number)
                    {
                        return prop.GetRawText();
                    }
                }
            }
        }
        catch (JsonException)
        {
            // Ignore non-JSON response body and use fallback external reference.
        }

        return null;
    }
}
