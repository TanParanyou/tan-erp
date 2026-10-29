namespace TanErp.Application.Estimates.CalculationPolicies;

public sealed record CreateCalculationPolicyCommand(
    Guid? BranchId,
    string PolicyCode,
    int Version,
    string OverheadMethod,
    decimal OverheadValue,
    string RoundingMode,
    string UnitConversionMode,
    DateTimeOffset EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc);
