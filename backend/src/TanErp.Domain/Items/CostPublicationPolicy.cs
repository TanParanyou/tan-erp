namespace TanErp.Domain.Items;

public static class CostPublicationPolicy
{
    public static bool Overlaps(CostRecord candidate, CostRecord published)
    {
        if (candidate.OrganizationId != published.OrganizationId || candidate.ItemId != published.ItemId
            || candidate.UnitId != published.UnitId || candidate.Currency != published.Currency
            || candidate.Scope != published.Scope || candidate.BranchId != published.BranchId)
            return false;

        var effectivePeriodsOverlap = (!candidate.EffectiveToUtc.HasValue || published.EffectiveFromUtc <= candidate.EffectiveToUtc.Value)
            && (!published.EffectiveToUtc.HasValue || candidate.EffectiveFromUtc <= published.EffectiveToUtc.Value);
        var quantityBandsOverlap = (!candidate.MaximumQuantity.HasValue || published.MinimumQuantity <= candidate.MaximumQuantity.Value)
            && (!published.MaximumQuantity.HasValue || candidate.MinimumQuantity <= published.MaximumQuantity.Value);

        return effectivePeriodsOverlap && quantityBandsOverlap;
    }

    public static bool CanSupersede(CostRecord candidate, CostRecord published) =>
        candidate.EffectiveFromUtc > published.EffectiveFromUtc
        && candidate.MinimumQuantity == published.MinimumQuantity
        && candidate.MaximumQuantity == published.MaximumQuantity
        && (!published.EffectiveToUtc.HasValue
            ? !candidate.EffectiveToUtc.HasValue
            : !candidate.EffectiveToUtc.HasValue || candidate.EffectiveToUtc >= published.EffectiveToUtc);
}
