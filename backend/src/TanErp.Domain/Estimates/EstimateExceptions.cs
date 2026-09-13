namespace TanErp.Domain.Estimates;

public class EstimateNotFoundException : Exception
{
    public EstimateNotFoundException(Guid estimateId)
        : base($"Estimate '{estimateId}' was not found.")
    {
    }
}

public class EstimateInvalidStateException : Exception
{
    public EstimateInvalidStateException(string message)
        : base(message)
    {
    }
}

public class EstimateCalculationException : Exception
{
    public EstimateCalculationException(string message)
        : base(message)
    {
    }
}
