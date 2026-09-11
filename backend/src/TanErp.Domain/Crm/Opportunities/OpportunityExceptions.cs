namespace TanErp.Domain.Crm.Opportunities;

public class OpportunityDomainException : Exception
{
    public OpportunityDomainException(string message) : base(message) { }
    public OpportunityDomainException(string message, Exception innerException) : base(message, innerException) { }
}

public sealed class OpportunityVersionException : OpportunityDomainException
{
    public OpportunityVersionException() : base("Opportunity row version mismatch.") { }
}

public sealed class OpportunityTransitionException : OpportunityDomainException
{
    public string CurrentStage { get; }
    public string TargetStage { get; }

    public OpportunityTransitionException(string currentStage, string targetStage)
        : base($"Cannot transition opportunity from '{currentStage}' to '{targetStage}'.")
    {
        CurrentStage = currentStage;
        TargetStage = targetStage;
    }
}

public sealed class OpportunityQualificationException : OpportunityDomainException
{
    public string MissingField { get; }

    public OpportunityQualificationException(string missingField)
        : base($"Opportunity qualification failed due to missing or invalid field: '{missingField}'.")
    {
        MissingField = missingField;
    }
}
