namespace TanErp.Domain.Surveys;

public class SurveyDomainException : Exception
{
    public SurveyDomainException(string message) : base(message) { }
    public SurveyDomainException(string message, Exception innerException) : base(message, innerException) { }
}

public sealed class SurveyVersionException : SurveyDomainException
{
    public SurveyVersionException() : base("Site survey row version mismatch.") { }
}

public sealed class SurveyTransitionException : SurveyDomainException
{
    public string CurrentStatus { get; }
    public string TargetStatus { get; }

    public SurveyTransitionException(string currentStatus, string targetStatus)
        : base($"Cannot transition site survey from '{currentStatus}' to '{targetStatus}'.")
    {
        CurrentStatus = currentStatus;
        TargetStatus = targetStatus;
    }
}

public sealed class SurveyInvalidStateException : SurveyDomainException
{
    public string CurrentStatus { get; }

    public SurveyInvalidStateException(string currentStatus, string message)
        : base(message)
    {
        CurrentStatus = currentStatus;
    }
}

public sealed class SurveyReadinessException : SurveyDomainException
{
    public string MissingRequirement { get; }

    public SurveyReadinessException(string missingRequirement, string message)
        : base(message)
    {
        MissingRequirement = missingRequirement;
    }
}

