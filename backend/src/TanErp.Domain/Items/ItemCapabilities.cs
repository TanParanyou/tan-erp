namespace TanErp.Domain.Items;

public sealed record ItemCapabilities(
    bool CanSell,
    bool CanCost,
    bool CanPurchase,
    bool CanStock,
    bool CanProduce)
{
    public bool HasAnyActive() => CanSell || CanCost || CanPurchase || CanStock || CanProduce;

    public static ItemCapabilities DefaultMaterial => new(CanSell: false, CanCost: true, CanPurchase: true, CanStock: true, CanProduce: false);
    public static ItemCapabilities DefaultService => new(CanSell: true, CanCost: true, CanPurchase: false, CanStock: false, CanProduce: false);
}
