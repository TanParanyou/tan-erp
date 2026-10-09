using TanErp.Application.Common.Results;

namespace TanErp.Application.Notifications;

/// <summary>
/// Payload hygiene. The allowlist (exactly the fields the type declares) is the real guard; the forbidden-fragment check is a second
/// net that also stops a future type from declaring a field that looks like a figure or a contact detail.
/// Error messages name keys, never values.
/// </summary>
public static class NotificationPayloadRules
{
    public const int MaxValueLength = 200;

    private static readonly string[] ForbiddenKeyFragments =
    {
        "cost", "price", "margin", "amount", "total", "budget", "discount", "tax", "salary",
        "email", "phone", "address", "token", "password", "secret"
    };

    public static bool IsForbiddenKey(string key) =>
        ForbiddenKeyFragments.Any(fragment => key.Contains(fragment, StringComparison.OrdinalIgnoreCase));

    public static Result<IReadOnlyDictionary<string, string>> Validate(NotificationTypeDescriptor descriptor, IReadOnlyDictionary<string, string>? payload)
    {
        if (payload is null) return Fail("The payload is required.");

        foreach (var key in payload.Keys)
        {
            if (IsForbiddenKey(key)) return Fail($"The payload field '{key}' is not allowed in a notification.");
            if (!descriptor.RequiredFields.Contains(key)) return Fail($"The payload field '{key}' is not declared for '{descriptor.Type}'.");
        }

        var missing = descriptor.RequiredFields.Where(field => !payload.ContainsKey(field)).OrderBy(f => f, StringComparer.Ordinal).ToArray();
        if (missing.Length > 0) return Fail($"The payload is missing: {string.Join(", ", missing)}.");

        var cleaned = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var (key, value) in payload)
        {
            var trimmed = value?.Trim() ?? string.Empty;
            if (trimmed.Length is 0 || trimmed.Length > MaxValueLength) return Fail($"The payload field '{key}' must be 1-{MaxValueLength} characters.");
            cleaned[key] = trimmed;
        }

        return Result<IReadOnlyDictionary<string, string>>.Success(cleaned);
    }

    private static Result<IReadOnlyDictionary<string, string>> Fail(string message) =>
        Result<IReadOnlyDictionary<string, string>>.Failure(new Error("NOTIFICATION_PAYLOAD_INVALID", message));
}
