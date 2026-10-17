using System.Text.Json;
using System.Text.Json.Nodes;

namespace TanErp.Domain.Common;

/// <summary>
/// The single redaction point for audit payloads (ADR 0027 D-B/B3). <see cref="AuditEvent"/> runs every ChangesJson through
/// <see cref="Redact"/>, so no call site needs its own PII handling. Keys are matched case-insensitively at any depth.
/// The deny-list is configurable (appsettings Audit:RedactedKeys) through <see cref="Configure"/> at startup.
/// </summary>
public static class AuditRedactor
{
    public const string Placeholder = "[redacted]";

    public static readonly IReadOnlyList<string> DefaultKeys = new[]
    {
        "email", "phone", "mobile", "address", "taxId", "nationalId", "displayName", "firstName", "lastName", "fullName"
    };

    private static HashSet<string> _keys = new(DefaultKeys, StringComparer.OrdinalIgnoreCase);

    /// <summary>Replaces the deny-list. A null or empty list restores the default.</summary>
    public static void Configure(IEnumerable<string>? keys)
    {
        var list = keys?.Where(k => !string.IsNullOrWhiteSpace(k)).Select(k => k.Trim()).ToList();
        _keys = new HashSet<string>(list is { Count: > 0 } ? list : DefaultKeys, StringComparer.OrdinalIgnoreCase);
    }

    public static string Redact(string changesJson)
    {
        JsonNode? root;
        try
        {
            root = JsonNode.Parse(changesJson);
        }
        catch (JsonException)
        {
            return changesJson;
        }

        if (root is null) return changesJson;
        Walk(root, _keys);
        return root.ToJsonString();
    }

    private static void Walk(JsonNode node, HashSet<string> keys)
    {
        switch (node)
        {
            case JsonObject obj:
                foreach (var name in obj.Select(p => p.Key).ToList())
                {
                    if (keys.Contains(name)) obj[name] = Placeholder;
                    else if (obj[name] is { } child) Walk(child, keys);
                }
                break;
            case JsonArray array:
                foreach (var item in array)
                {
                    if (item is not null) Walk(item, keys);
                }
                break;
        }
    }
}
