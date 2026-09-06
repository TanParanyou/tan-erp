using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace TanErp.Api.ErrorHandling;

public class ApiProblemDetails : ProblemDetails
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("traceId")]
    public string TraceId { get; set; } = string.Empty;

    [JsonPropertyName("errors")]
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}
