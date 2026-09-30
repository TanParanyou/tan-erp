namespace TanErp.Domain.Estimates;

public sealed record EstimateDiscount(string Type, decimal Value, string? ReasonCode)
{
    public const string None = "none";
    public const string Percent = "percent";
    public const string FixedAmount = "fixed-amount";

    public static EstimateDiscount LegacyFixedAmount(decimal amount) => new(FixedAmount, amount, null);

    public decimal ResolveAmount(decimal sellingBeforeDiscount, MidpointRounding rounding)
    {
        if (Value < 0m)
            throw new ArgumentOutOfRangeException(nameof(Value), "Discount value cannot be negative.");

        var amount = Type switch
        {
            None when Value == 0m => 0m,
            None => throw new ArgumentException("A no-discount selection must have a zero value.", nameof(Value)),
            Percent when Value <= 1m => decimal.Round(sellingBeforeDiscount * Value, 2, rounding),
            Percent => throw new ArgumentOutOfRangeException(nameof(Value), "Discount rate must be between zero and one."),
            FixedAmount => decimal.Round(Value, 2, rounding),
            _ => throw new ArgumentException("Discount type is not supported.", nameof(Type))
        };

        if (amount > sellingBeforeDiscount)
            throw new ArgumentOutOfRangeException(nameof(Value), "Discount cannot exceed the selling amount.");
        if (amount > 0m && string.IsNullOrWhiteSpace(ReasonCode))
            throw new EstimateDiscountReasonRequiredException();

        return amount;
    }
}

public sealed class EstimateDiscountReasonRequiredException : Exception
{
    public EstimateDiscountReasonRequiredException() : base("A reason code is required for a non-zero discount.") { }
}
