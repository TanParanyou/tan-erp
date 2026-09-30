namespace TanErp.Domain.Items;

public static class UnitRoundingMode
{
    public const string HalfUp = "half_up";
    public const string HalfEven = "half_even";
    public const string Up = "up";
    public const string Down = "down";
    public const string Ceiling = "ceiling";
    public const string Floor = "floor";

    public static string Default => HalfUp;

    public static bool TryNormalize(string? value, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(value)) return false;

        var key = value.Trim().Replace("_", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        normalized = key.ToLowerInvariant() switch
        {
            "halfup" => HalfUp,
            "halfeven" => HalfEven,
            "up" => Up,
            "down" => Down,
            "ceiling" => Ceiling,
            "floor" => Floor,
            _ => string.Empty,
        };
        return normalized.Length > 0;
    }

    public static decimal Round(decimal value, int decimalScale, string mode)
    {
        if (decimalScale is < 0 or > 6) throw new ArgumentOutOfRangeException(nameof(decimalScale));
        if (!TryNormalize(mode, out var normalized)) throw new ArgumentOutOfRangeException(nameof(mode));

        var factor = (decimal)Math.Pow(10, decimalScale);
        var scaledValue = value * factor;
        var rounded = normalized switch
        {
            HalfUp => decimal.Round(scaledValue, 0, MidpointRounding.AwayFromZero),
            HalfEven => decimal.Round(scaledValue, 0, MidpointRounding.ToEven),
            Up => Math.Sign(scaledValue) * decimal.Ceiling(Math.Abs(scaledValue)),
            Down => decimal.Truncate(scaledValue),
            Ceiling => decimal.Ceiling(scaledValue),
            Floor => decimal.Floor(scaledValue),
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
        return rounded / factor;
    }
}
