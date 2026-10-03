namespace TanErp.Domain.Estimates;

public sealed class EstimatePolicyUnavailableException : Exception
{
    public EstimatePolicyUnavailableException(string message) : base(message) { }
}
