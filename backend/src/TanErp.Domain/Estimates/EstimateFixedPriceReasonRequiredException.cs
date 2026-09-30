namespace TanErp.Domain.Estimates;

public sealed class EstimateFixedPriceReasonRequiredException : Exception
{
    public EstimateFixedPriceReasonRequiredException()
        : base("A reason code is required when using a fixed selling price.")
    {
    }
}
