using TanErp.Domain.Items;
using Xunit;

namespace TanErp.UnitTests.Items;

public class ItemTests
{
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();
    private readonly Guid _baseUnitId = Guid.NewGuid();
    private readonly Guid _actorId = Guid.NewGuid();

    [Fact]
    public void CreateDraft_WithValidInputs_InitializesDraftItem()
    {
        var itemId = Guid.NewGuid();
        var name = LocalizedText.Create("ไม้อัด 18 มม.", "18mm Plywood");
        var now = DateTimeOffset.UtcNow;

        var item = Item.CreateDraft(
            itemId,
            _orgId,
            "MAT-PLY-18",
            ItemType.Material,
            _categoryId,
            null,
            name,
            null,
            _baseUnitId,
            ItemAvailabilityMode.AllBranches,
            new ItemCapabilities(CanSell: false, CanCost: true, CanPurchase: true, CanStock: true, CanProduce: false),
            null,
            null,
            _actorId,
            now);

        Assert.Equal(itemId, item.Id);
        Assert.Equal(_orgId, item.OrganizationId);
        Assert.Equal("MAT-PLY-18", item.Code);
        Assert.Equal("MAT-PLY-18", item.NormalizedCode);
        Assert.Equal(ItemType.Material, item.ItemType);
        Assert.Equal(_categoryId, item.CategoryId);
        Assert.Null(item.BrandId);
        Assert.Equal("ไม้อัด 18 มม.", item.Name.Thai);
        Assert.Equal("18mm Plywood", item.Name.English);
        Assert.Equal(ItemAvailabilityMode.AllBranches, item.AvailabilityMode);
        Assert.True(item.Capabilities.CanCost);
        Assert.False(item.Capabilities.CanSell);
        Assert.Equal(ItemStatus.Draft, item.Status);
        Assert.False(item.ActivatedOnce);
        Assert.NotEqual(Guid.Empty, item.RowVersion);
    }

    [Fact]
    public void Activate_WhenAllRequirementsMet_TransitionsToActive()
    {
        var item = CreateStandardDraftItem();
        var now = DateTimeOffset.UtcNow;

        item.Activate(_actorId, now, hasActiveSelectedBranch: false);

        Assert.Equal(ItemStatus.Active, item.Status);
        Assert.True(item.ActivatedOnce);
        Assert.Equal(_actorId, item.ActivatedByUserId);
        Assert.Equal(now, item.ActivatedAtUtc);
    }

    [Fact]
    public void Activate_WhenAvailabilityIsSelectedBranches_RequiresActiveSelectedBranch()
    {
        var itemId = Guid.NewGuid();
        var name = LocalizedText.Create("ไม้อัด 18 มม.", "18mm Plywood");
        var item = Item.CreateDraft(
            itemId,
            _orgId,
            "MAT-PLY-18",
            ItemType.Material,
            _categoryId,
            null,
            name,
            null,
            _baseUnitId,
            ItemAvailabilityMode.SelectedBranches,
            new ItemCapabilities(CanSell: false, CanCost: true, CanPurchase: true, CanStock: true, CanProduce: false),
            null,
            null,
            _actorId,
            DateTimeOffset.UtcNow);

        var ex = Assert.Throws<ItemValidationException>(() =>
            item.Activate(_actorId, DateTimeOffset.UtcNow, hasActiveSelectedBranch: false));

        Assert.Equal("ITEM_BRANCH_REQUIRED", ex.Code);
    }

    [Fact]
    public void Activate_WhenNoCapabilitiesTrue_ThrowsItemValidationException()
    {
        var itemId = Guid.NewGuid();
        var name = LocalizedText.Create("ไม้อัด 18 มม.", "18mm Plywood");
        var item = Item.CreateDraft(
            itemId,
            _orgId,
            "MAT-PLY-18",
            ItemType.Material,
            _categoryId,
            null,
            name,
            null,
            _baseUnitId,
            ItemAvailabilityMode.AllBranches,
            new ItemCapabilities(CanSell: false, CanCost: false, CanPurchase: false, CanStock: false, CanProduce: false),
            null,
            null,
            _actorId,
            DateTimeOffset.UtcNow);

        var ex = Assert.Throws<ItemValidationException>(() =>
            item.Activate(_actorId, DateTimeOffset.UtcNow, hasActiveSelectedBranch: false));

        Assert.Equal("ITEM_CAPABILITY_REQUIRED", ex.Code);
    }

    [Fact]
    public void UpdateDetails_WhenActivatedOnce_ThrowsIfCodeChanged()
    {
        var item = CreateStandardDraftItem();
        item.Activate(_actorId, DateTimeOffset.UtcNow, hasActiveSelectedBranch: false);

        var ex = Assert.Throws<ItemValidationException>(() =>
            item.UpdateDetails(
                "MAT-PLY-99", // Different code
                ItemType.Material,
                _categoryId,
                null,
                item.Name,
                null,
                _baseUnitId,
                ItemAvailabilityMode.AllBranches,
                item.Capabilities,
                null,
                null,
                _actorId,
                DateTimeOffset.UtcNow));

        Assert.Equal("ITEM_CODE_IMMUTABLE", ex.Code);
    }

    [Fact]
    public void Deactivate_WithValidReason_TransitionsToInactive()
    {
        var item = CreateStandardDraftItem();
        item.Activate(_actorId, DateTimeOffset.UtcNow, hasActiveSelectedBranch: false);

        var now = DateTimeOffset.UtcNow;
        item.Deactivate(_actorId, now, "DISCONTINUED", "ยกเลิกการผลิตจากโรงงาน");

        Assert.Equal(ItemStatus.Inactive, item.Status);
        Assert.Equal(_actorId, item.InactiveByUserId);
        Assert.Equal(now, item.InactiveAtUtc);
        Assert.Equal("DISCONTINUED", item.InactiveReasonCode);
        Assert.Equal("ยกเลิกการผลิตจากโรงงาน", item.InactiveReason);
    }

    [Fact]
    public void Deactivate_WithoutReasonCode_ThrowsItemValidationException()
    {
        var item = CreateStandardDraftItem();
        item.Activate(_actorId, DateTimeOffset.UtcNow, hasActiveSelectedBranch: false);

        var ex = Assert.Throws<ItemValidationException>(() =>
            item.Deactivate(_actorId, DateTimeOffset.UtcNow, "", null));

        Assert.Equal("ITEM_FIELD_REQUIRED", ex.Code);
    }

    private Item CreateStandardDraftItem()
    {
        return Item.CreateDraft(
            Guid.NewGuid(),
            _orgId,
            "MAT-PLY-18",
            ItemType.Material,
            _categoryId,
            null,
            LocalizedText.Create("ไม้อัด 18 มม.", "18mm Plywood"),
            null,
            _baseUnitId,
            ItemAvailabilityMode.AllBranches,
            new ItemCapabilities(CanSell: false, CanCost: true, CanPurchase: true, CanStock: true, CanProduce: false),
            null,
            null,
            _actorId,
            DateTimeOffset.UtcNow);
    }
}
