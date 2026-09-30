namespace TanErp.Domain.Estimates;

public sealed class EstimateIdempotencyKeyReusedException : Exception
{
    public EstimateIdempotencyKeyReusedException()
        : base("The idempotency key has already been used with a different calculation request.")
    {
    }
}
