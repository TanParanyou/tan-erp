using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Surveys.Narrative.PolishSurveyNarrative;

public sealed record PolishSurveyNarrativeCommand(string Text, string Language, string FirebaseUid, Guid MembershipId);

public sealed record PolishSurveyNarrativeResult(string Text, string Provider);

public class PolishSurveyNarrativeHandler
{
    /// <summary>Matches the scope_summary column limit so polished text always fits.</summary>
    public const int MaxTextLength = 2000;

    private static readonly HashSet<string> SupportedLanguages = new(StringComparer.Ordinal) { "th", "en" };

    private readonly IRequestAccessResolver _accessResolver;
    private readonly ISurveyNarrativeProvider _provider;

    public PolishSurveyNarrativeHandler(IRequestAccessResolver accessResolver, ISurveyNarrativeProvider provider)
    {
        _accessResolver = accessResolver;
        _provider = provider;
    }

    public async Task<Result<PolishSurveyNarrativeResult>> Handle(
        PolishSurveyNarrativeCommand command,
        CancellationToken cancellationToken = default)
    {
        var accessResult = await _accessResolver.ResolveAsync(
            command.FirebaseUid,
            command.MembershipId,
            "surveys.update",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return Result<PolishSurveyNarrativeResult>.Failure(accessResult.Error);
        }

        if (!_provider.IsEnabled)
        {
            return Result<PolishSurveyNarrativeResult>.Failure(
                new Error("SURVEY_NARRATIVE_DISABLED", "Survey narrative assist is not enabled."));
        }

        var text = command.Text?.Trim();
        if (string.IsNullOrEmpty(text) || text.Length > MaxTextLength || !SupportedLanguages.Contains(command.Language))
        {
            return Result<PolishSurveyNarrativeResult>.Failure(
                new Error("SURVEY_FIELD_REQUIRED", $"Text of 1-{MaxTextLength} characters and a supported language (th, en) are required."));
        }

        var polished = await _provider.PolishAsync(SurveyNarrativeRedactor.Redact(text), command.Language, cancellationToken);
        if (polished.IsFailure)
        {
            return Result<PolishSurveyNarrativeResult>.Failure(polished.Error);
        }

        return Result<PolishSurveyNarrativeResult>.Success(new PolishSurveyNarrativeResult(polished.Value!, _provider.Name));
    }
}
