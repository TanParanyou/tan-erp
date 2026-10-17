namespace TanErp.Domain.Estimates;

public static class UnitConversionModes
{
    public const string None = "none";
    public const string Snapshot = "snapshot";

    public static bool IsValid(string? value) => value is None or Snapshot;
}

public static class UnitConversionScopes
{
    public const string Item = "item";
    public const string Shared = "shared";

    public static bool IsValid(string? value) => value is Item or Shared;
}

/// <summary>
/// The unit conversion frozen on one catalog cost component when its draft was saved: 1 FromUnit = Factor ToUnit, where ToUnit is the
/// item's base unit (the unit the cost record is published in). It has no mutator: a later Item Master change never alters a saved estimate.
/// </summary>
public sealed class EstimateConversionSnapshot
{
    public Guid ConversionId { get; }
    public string Scope { get; }
    public string FromUnit { get; }
    public string ToUnit { get; }
    public decimal Factor { get; }

    private EstimateConversionSnapshot(Guid conversionId, string scope, string fromUnit, string toUnit, decimal factor)
    {
        ConversionId = conversionId;
        Scope = scope;
        FromUnit = fromUnit;
        ToUnit = toUnit;
        Factor = factor;
    }

    public static EstimateConversionSnapshot Create(Guid conversionId, string scope, string fromUnit, string toUnit, decimal factor)
    {
        if (conversionId == Guid.Empty)
            throw new ArgumentException("Conversion ID cannot be empty.", nameof(conversionId));
        if (!UnitConversionScopes.IsValid(scope))
            throw new ArgumentException($"Invalid conversion scope: '{scope}'.", nameof(scope));
        if (string.IsNullOrWhiteSpace(fromUnit) || string.IsNullOrWhiteSpace(toUnit) || fromUnit.Trim().Length > 32 || toUnit.Trim().Length > 32)
            throw new ArgumentException("Conversion unit codes are required and limited to 32 characters.");
        if (string.Equals(fromUnit.Trim(), toUnit.Trim(), StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("A conversion cannot map a unit to itself.", nameof(toUnit));
        if (factor <= 0m || factor >= 1_000_000_000_000m || decimal.Round(factor, 6) != factor)
            throw new ArgumentOutOfRangeException(nameof(factor), "Conversion factor must be positive with at most six decimal places.");

        return new EstimateConversionSnapshot(conversionId, scope, fromUnit.Trim(), toUnit.Trim(), factor);
    }
}
