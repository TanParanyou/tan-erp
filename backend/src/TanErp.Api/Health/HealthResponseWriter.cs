using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TanErp.Api.Health;

/// <summary>Writes only status names (never descriptions, data or exceptions) so anonymous probes learn nothing sensitive.</summary>
public static class HealthResponseWriter
{
    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        context.Response.Headers.CacheControl = "no-store";

        var payload = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.ToDictionary(entry => entry.Key, entry => entry.Value.Status.ToString())
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
