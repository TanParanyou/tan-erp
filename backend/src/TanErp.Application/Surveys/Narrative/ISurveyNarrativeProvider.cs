using TanErp.Application.Common.Results;

namespace TanErp.Application.Surveys.Narrative;

/// <summary>
/// Optional text-polishing service for survey summaries. Disabled unless explicitly configured;
/// callers must send only redacted text (see <see cref="SurveyNarrativeRedactor"/>).
/// </summary>
public interface ISurveyNarrativeProvider
{
    bool IsEnabled { get; }

    string Name { get; }

    Task<Result<string>> PolishAsync(string text, string language, CancellationToken cancellationToken);
}
