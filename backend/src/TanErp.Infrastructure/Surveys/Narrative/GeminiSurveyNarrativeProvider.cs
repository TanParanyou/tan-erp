using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TanErp.Application.Common.Results;
using TanErp.Application.Surveys.Narrative;

namespace TanErp.Infrastructure.Surveys.Narrative;

public sealed class GeminiSurveyNarrativeProvider : ISurveyNarrativeProvider
{
    private const int MaxOutputLength = 1800;

    private readonly HttpClient _http;
    private readonly SurveyNarrativeOptions _options;

    public GeminiSurveyNarrativeProvider(HttpClient http, IOptions<SurveyNarrativeOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public string Name => "gemini";

    public bool IsEnabled =>
        string.Equals(_options.Provider, "gemini", StringComparison.OrdinalIgnoreCase)
        && _options.DataTermsAcknowledged
        && !string.IsNullOrWhiteSpace(_options.ApiKey)
        && !string.IsNullOrWhiteSpace(_options.Model);

    public async Task<Result<string>> PolishAsync(string text, string language, CancellationToken cancellationToken)
    {
        var languageName = language == "en" ? "English" : "Thai";
        var prompt =
            $"Rewrite the following site-survey summary in {languageName} so it reads clearly and concisely. " +
            "Keep every number, unit and item name exactly as written, do not add facts, and keep the same list structure. " +
            "Return only the rewritten text.\n\n" + text;

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Uri.EscapeDataString(_options.Model)}:generateContent";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("x-goog-api-key", _options.ApiKey);
        request.Content = JsonContent.Create(new { contents = new[] { new { parts = new[] { new { text = prompt } } } } });

        try
        {
            using var response = await _http.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return Failure();
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var polished = document.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString()
                ?.Trim();

            if (string.IsNullOrEmpty(polished))
            {
                return Failure();
            }

            return Result<string>.Success(polished.Length > MaxOutputLength ? polished[..MaxOutputLength] : polished);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException or KeyNotFoundException or InvalidOperationException or IndexOutOfRangeException)
        {
            // Never log the request or response: they contain survey text.
            return Failure();
        }
    }

    private static Result<string> Failure() =>
        Result<string>.Failure(new Error("SURVEY_NARRATIVE_PROVIDER_FAILED", "The narrative provider did not return a usable result."));
}
