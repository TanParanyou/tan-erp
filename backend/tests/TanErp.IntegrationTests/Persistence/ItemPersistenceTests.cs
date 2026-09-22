using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Domain.Files;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class ItemPersistenceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }

    private AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Migration_AppliesSuccessfully_AndEnforcesSameOrganizationInvariants()
    {
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();

        var orgA = new Organization(Guid.NewGuid(), "Org A", true, DateTimeOffset.UtcNow);
        var orgB = new Organization(Guid.NewGuid(), "Org B", true, DateTimeOffset.UtcNow);
        db.Organizations.AddRange(orgA, orgB);

        var branchA = new Branch(Guid.NewGuid(), orgA.Id, "BKK", "Bangkok Branch", true, DateTimeOffset.UtcNow);
        var branchB = new Branch(Guid.NewGuid(), orgB.Id, "CNX", "Chiang Mai Branch", true, DateTimeOffset.UtcNow);
        db.Branches.AddRange(branchA, branchB);

        var categoryA = new ItemCategory(
            Guid.NewGuid(),
            orgA.Id,
            "WOOD",
            LocalizedText.Create("ไม้", "Wood"),
            null,
            null,
            ItemType.All,
            0,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        var brandA = new ItemBrand(
            Guid.NewGuid(),
            orgA.Id,
            "VANA",
            LocalizedText.Create("วนชัย", "Vanachai"),
            null,
            0,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        var unitA = new UnitOfMeasure(
            Guid.NewGuid(),
            orgA.Id,
            "SHEET",
            LocalizedText.Create("แผ่น", "Sheet"),
            "แผ่น",
            "count",
            0,
            "half_up",
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        db.ItemCategories.Add(categoryA);
        db.ItemBrands.Add(brandA);
        db.Units.Add(unitA);
        await db.SaveChangesAsync();

        // 1. Create valid Item in Org A
        var item = Item.CreateDraft(
            Guid.NewGuid(),
            orgA.Id,
            "MAT-PLY-18",
            ItemType.Material,
            categoryA.Id,
            brandA.Id,
            LocalizedText.Create("ไม้อัด 18 มม.", "18mm Plywood"),
            LocalizedText.CreateOptional("คำอธิบายไม้อัด", "Description"),
            unitA.Id,
            ItemAvailabilityMode.AllBranches,
            ItemCapabilities.DefaultMaterial,
            new Dictionary<string, string> { ["thickness"] = "18mm" },
            1,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        db.Items.Add(item);
        await db.SaveChangesAsync();

        // 2. Query back and verify JSONB roundtrip
        var loaded = await db.Items
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Brand)
            .Include(i => i.BaseUnit)
            .FirstOrDefaultAsync(i => i.Id == item.Id);

        Assert.NotNull(loaded);
        Assert.Equal("ไม้อัด 18 มม.", loaded.Name.Thai);
        Assert.Equal("18mm Plywood", loaded.Name.English);
        Assert.NotNull(loaded.Description);
        Assert.Equal("คำอธิบายไม้อัด", loaded.Description.Thai);
        Assert.NotNull(loaded.Attributes);
        Assert.Equal("18mm", loaded.Attributes["thickness"]);
        Assert.True(loaded.Capabilities.CanCost);
        Assert.False(loaded.Capabilities.CanSell);
    }

    [Fact]
    public async Task Category_SelfParentCycle_ThrowsException()
    {
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();

        var org = new Organization(Guid.NewGuid(), "Org", true, DateTimeOffset.UtcNow);
        db.Organizations.Add(org);
        await db.SaveChangesAsync();

        var catId = Guid.NewGuid();
        var ex = Assert.Throws<ItemValidationException>(() =>
            new ItemCategory(
                catId,
                org.Id,
                "CAT-01",
                LocalizedText.Create("หมวด", "Category"),
                null,
                catId, // self parent
                ItemType.All,
                0,
                Guid.NewGuid(),
                DateTimeOffset.UtcNow));

        Assert.Equal("ITEM_CATEGORY_CYCLE", ex.Code);
    }

    [Fact]
    public async Task CrossOrganization_BranchAvailability_IsRejectedByForeignKey()
    {
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();

        var orgA = new Organization(Guid.NewGuid(), "Org A", true, DateTimeOffset.UtcNow);
        var orgB = new Organization(Guid.NewGuid(), "Org B", true, DateTimeOffset.UtcNow);
        db.Organizations.AddRange(orgA, orgB);

        var branchB = new Branch(Guid.NewGuid(), orgB.Id, "CNX", "Chiang Mai Branch", true, DateTimeOffset.UtcNow);
        db.Branches.Add(branchB);

        var catA = new ItemCategory(Guid.NewGuid(), orgA.Id, "CAT", LocalizedText.Create("หมวด", null), null, null, ItemType.All, 0, Guid.NewGuid(), DateTimeOffset.UtcNow);
        var unitA = new UnitOfMeasure(Guid.NewGuid(), orgA.Id, "PCS", LocalizedText.Create("ชิ้น", null), "ชิ้น", "count", 0, "half_up", Guid.NewGuid(), DateTimeOffset.UtcNow);
        db.ItemCategories.Add(catA);
        db.Units.Add(unitA);

        var itemA = Item.CreateDraft(
            Guid.NewGuid(),
            orgA.Id,
            "ITEM-A",
            ItemType.Material,
            catA.Id,
            null,
            LocalizedText.Create("สินค้า A", null),
            null,
            unitA.Id,
            ItemAvailabilityMode.SelectedBranches,
            ItemCapabilities.DefaultMaterial,
            null,
            null,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);
        db.Items.Add(itemA);
        await db.SaveChangesAsync();

        // Try to attach BranchB (OrgB) to ItemA (OrgA) with same organizationId as item (OrgA)
        var avail = new ItemBranchAvailability(
            Guid.NewGuid(),
            orgA.Id,
            itemA.Id,
            branchB.Id, // Branch from Org B!
            null,
            null,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        db.ItemBranchAvailabilities.Add(avail);

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task ItemImage_OnlyOneActivePrimaryAllowed_EnforcedByPartialUniqueIndex()
    {
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();

        var org = new Organization(Guid.NewGuid(), "Org", true, DateTimeOffset.UtcNow);
        db.Organizations.Add(org);

        var cat = new ItemCategory(Guid.NewGuid(), org.Id, "CAT", LocalizedText.Create("หมวด", null), null, null, ItemType.All, 0, Guid.NewGuid(), DateTimeOffset.UtcNow);
        var unit = new UnitOfMeasure(Guid.NewGuid(), org.Id, "PCS", LocalizedText.Create("ชิ้น", null), "ชิ้น", "count", 0, "half_up", Guid.NewGuid(), DateTimeOffset.UtcNow);
        db.ItemCategories.Add(cat);
        db.Units.Add(unit);

        var item = Item.CreateDraft(
            Guid.NewGuid(),
            org.Id,
            "ITEM-01",
            ItemType.Material,
            cat.Id,
            null,
            LocalizedText.Create("สินค้า", null),
            null,
            unit.Id,
            ItemAvailabilityMode.AllBranches,
            ItemCapabilities.DefaultMaterial,
            null,
            null,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);
        db.Items.Add(item);

        var file1 = new UploadedFile(
            Guid.NewGuid(),
            org.Id,
            "/storage/file1.jpg",
            "file1.jpg",
            "image/jpeg",
            1024,
            "session-1",
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        var file2 = new UploadedFile(
            Guid.NewGuid(),
            org.Id,
            "/storage/file2.jpg",
            "file2.jpg",
            "image/jpeg",
            2048,
            "session-2",
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        db.UploadedFiles.AddRange(file1, file2);
        await db.SaveChangesAsync();

        var img1 = new ItemImage(
            Guid.NewGuid(),
            org.Id,
            item.Id,
            file1.Id,
            ItemImageRole.Primary,
            isPrimary: true,
            displayOrder: 0,
            LocalizedText.Create("ภาพหลัก 1", null),
            null,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);
        db.ItemImages.Add(img1);
        await db.SaveChangesAsync();

        // Attempting to add a SECOND active primary image for the same item
        var img2 = new ItemImage(
            Guid.NewGuid(),
            org.Id,
            item.Id,
            file2.Id,
            ItemImageRole.Primary,
            isPrimary: true,
            displayOrder: 1,
            LocalizedText.Create("ภาพหลัก 2", null),
            null,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);
        db.ItemImages.Add(img2);

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }
}
