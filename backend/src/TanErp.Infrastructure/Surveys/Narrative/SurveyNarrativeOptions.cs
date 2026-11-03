namespace TanErp.Infrastructure.Surveys.Narrative;

/// <summary>
/// Configuration section "SurveyNarrative". Disabled unless Provider is "gemini", ApiKey and Model are
/// set, and DataTermsAcknowledged is true. Free-tier terms of external providers may allow them to use submitted content to
/// improve their products: use free tiers with synthetic/TEST_ONLY data only.
/// </summary>
public sealed class SurveyNarrativeOptions
{
    public string Provider { get; set; } = "none";

    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Must be set to true by an operator who has read the provider's data-use terms; without it the
    /// feature stays disabled even when a key is present.
    /// </summary>
    public bool DataTermsAcknowledged { get; set; }
}
