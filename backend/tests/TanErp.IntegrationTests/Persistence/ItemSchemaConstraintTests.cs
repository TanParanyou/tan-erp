using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Estimates;
using TanErp.Domain.Files;
using TanErp.Domain.Items;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class ItemSchemaConstraintTests : IAsyncLifetime
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
    public async Task Migration_PreservesElevenLegacyUploadedFilesAsPending()
    {
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync("20260921151336_ScopeFileUploadIdempotencyByActor");

        var itemMasterDidNotExistBeforeMigration = await db.Database.SqlQueryRaw<bool>(@"
            SELECT NOT EXISTS (
                SELECT 1 FROM information_schema.tables
                WHERE table_schema = 'item_master' AND table_name = 'items') AS ""Value""")
            .SingleAsync();
        Assert.True(itemMasterDidNotExistBeforeMigration);

        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdAtUtc = DateTimeOffset.UtcNow;
        var legacyFiles = Enumerable.Range(1, 11)
            .Select(index => new
            {
                Id = Guid.NewGuid(),
                StoragePath = $"/legacy/image-{index}.jpg",
                OriginalFilename = $"image-{index}.jpg",
                UploadSessionId = $"legacy-session-{index}",
            })
            .ToArray();

        foreach (var legacyFile in legacyFiles)
        {
            await db.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO files.uploaded_files
                    (id, organization_id, storage_path, original_filename, media_type,
                     file_size_bytes, upload_session_id, status, uploaded_by_user_id, created_at_utc)
                VALUES
                    ({legacyFile.Id}, {organizationId}, {legacyFile.StoragePath}, {legacyFile.OriginalFilename}, 'image/jpeg',
                     1024, {legacyFile.UploadSessionId}, 'verified', {userId}, {createdAtUtc})");
        }

        await db.Database.MigrateAsync();

        var itemMasterTablesExist = await db.Database.SqlQueryRaw<bool>(@"
            SELECT EXISTS (
                SELECT 1 FROM information_schema.tables
                WHERE table_schema = 'item_master' AND table_name = 'items')
                AND EXISTS (
                SELECT 1 FROM information_schema.tables
                WHERE table_schema = 'item_master' AND table_name = 'item_barcodes') AS ""Value""")
            .SingleAsync();
        Assert.True(itemMasterTablesExist);

        var migratedFiles = await db.UploadedFiles.AsNoTracking()
            .Where(file => legacyFiles.Select(legacyFile => legacyFile.Id).Contains(file.Id))
            .ToDictionaryAsync(file => file.Id);
        Assert.Equal(legacyFiles.Length, migratedFiles.Count);
        foreach (var legacyFile in legacyFiles)
        {
            Assert.True(migratedFiles.TryGetValue(legacyFile.Id, out var migratedFile));
            Assert.Equal(legacyFile.StoragePath, migratedFile!.StoragePath);
            Assert.Equal(legacyFile.OriginalFilename, migratedFile.OriginalFilename);
            Assert.Equal(FileScanStatus.Pending, migratedFile.ScanStatus);
            Assert.Null(migratedFile.ContentSha256);
            Assert.Null(migratedFile.VerifiedAtUtc);
        }

        var hasNoDefault = await db.Database.SqlQueryRaw<bool>(@"
            SELECT column_default IS NULL AS ""Value""
            FROM information_schema.columns
            WHERE table_schema = 'files' AND table_name = 'uploaded_files' AND column_name = 'scan_status'")
            .SingleAsync();
        Assert.True(hasNoDefault);
    }

    [Fact]
    public async Task CostRecord_EvidenceFileFromAnotherOrganization_ThrowsDbUpdateException()
    {
        await using var db = CreateDbContext();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        var orgAId = TestOnlyDataSeeder.TestOrgId;
        var orgBId = TestOnlyDataSeeder.TestOrgBId;

        var catA = new ItemCategory(Guid.NewGuid(), orgAId, "CAT-A", LocalizedText.Create("หมวด A", null), null, null, ItemType.All, 0, Guid.NewGuid(), DateTimeOffset.UtcNow);
        var unitA = new UnitOfMeasure(Guid.NewGuid(), orgAId, "UNIT-A", LocalizedText.Create("หน่วย A", null), "A", "count", 0, "half_up", Guid.NewGuid(), DateTimeOffset.UtcNow);
        db.ItemCategories.Add(catA);
        db.Units.Add(unitA);

        var itemA = Item.CreateDraft(
            Guid.NewGuid(),
            orgAId,
            "ITEM-A",
            ItemType.Material,
            catA.Id,
            null,
            LocalizedText.Create("สินค้า A", null),
            null,
            unitA.Id,
            ItemAvailabilityMode.AllBranches,
            ItemCapabilities.DefaultMaterial,
            null,
            null,
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);
        db.Items.Add(itemA);

        // UploadedFile belongs to Org B!
        var fileInOrgB = new UploadedFile(
            Guid.NewGuid(),
            orgBId,
            "/storage/file-org-b.pdf",
            "file.pdf",
            "application/pdf",
            1024,
            "session-b",
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);
        db.UploadedFiles.Add(fileInOrgB);

        await db.SaveChangesAsync();

        // Create CostRecord in Org A referencing file in Org B
        var costRecord = CostRecord.CreateDraft(
            Guid.NewGuid(),
            orgAId,
            itemA.Id,
            CostScopeType.Organization,
            null,
            unitA.Id,
            "THB",
            100m,
            0m,
            null,
            DateTimeOffset.UtcNow,
            null,
            1,
            null,
            null,
            "Test quote",
            fileInOrgB.Id, // CROSS-ORG FILE
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);

        db.CostRecords.Add(costRecord);

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        Assert.NotNull(ex.InnerException);
        Assert.IsType<PostgresException>(ex.InnerException);
    }

    [Fact]
    public async Task EstimateCostComponent_ItemFromAnotherOrganization_ThrowsDbUpdateException()
    {
        await using var db = CreateDbContext();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        var orgAId = TestOnlyDataSeeder.TestOrgId;
        var orgBId = TestOnlyDataSeeder.TestOrgBId;
        var branchAId = TestOnlyDataSeeder.TestBranchId;
        var userAId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var catB = new ItemCategory(Guid.NewGuid(), orgBId, "CAT-B", LocalizedText.Create("หมวด B", null), null, null, ItemType.All, 0, Guid.NewGuid(), now);
        var unitB = new UnitOfMeasure(Guid.NewGuid(), orgBId, "UNIT-B", LocalizedText.Create("หน่วย B", null), "B", "count", 0, "half_up", Guid.NewGuid(), now);
        db.ItemCategories.Add(catB);
        db.Units.Add(unitB);

        var itemB = Item.CreateDraft(
            Guid.NewGuid(),
            orgBId,
            "ITEM-B",
            ItemType.Material,
            catB.Id,
            null,
            LocalizedText.Create("สินค้า B", null),
            null,
            unitB.Id,
            ItemAvailabilityMode.AllBranches,
            ItemCapabilities.DefaultMaterial,
            null,
            null,
            Guid.NewGuid(),
            now);
        db.Items.Add(itemB);

        var costRecordB = CostRecord.CreateDraft(
            Guid.NewGuid(),
            orgBId,
            itemB.Id,
            CostScopeType.Organization,
            null,
            unitB.Id,
            "THB",
            100m,
            0m,
            null,
            now,
            null,
            1,
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            now);
        db.CostRecords.Add(costRecordB);

        // Estimate in Org A
        var customer = Customer.CreateDraft(
            Guid.NewGuid(), orgAId, userAId, CustomerType.Person, "ลูกค้า Org A", null, "th",
            new PrimaryContactInput("สมชาย", null, "0811111111", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), orgAId, branchAId, customer.Id, null, userAId, userAId,
            "ดีล Org A", "ขอบเขต", new[] { "built-in" }, null, 100000m, "THB",
            null, null, null, now);
        db.Opportunities.Add(opp);

        var estimate = Estimate.CreateDraft(
            Guid.NewGuid(),
            orgAId,
            branchAId,
            customer.Id,
            opp.Id,
            "EST-001");
        db.Estimates.Add(estimate);

        var section = new EstimateSection(Guid.NewGuid(), orgAId, estimate.CurrentRevision!.Id, "SEC-01", "Section A", "Section A EN");
        var workItem = new EstimateWorkItem(
            Guid.NewGuid(),
            orgAId,
            section.Id,
            "WI-01",
            "Work Item A",
            "Work Item A EN",
            1m,
            "PCS",
            SellingRuleType.Margin,
            0.2m);
        db.EstimateSections.Add(section);
        db.EstimateWorkItems.Add(workItem);
        await db.SaveChangesAsync();

        // Create CostComponent in Org A referencing Item B (from Org B)
        var comp = new EstimateCostComponent(
            Guid.NewGuid(),
            orgAId,
            workItem.Id,
            CostComponentType.Material,
            "Comp A",
            1m,
            "PCS",
            100m);

        comp.SetCatalogCostSnapshot(
            itemId: itemB.Id, // CROSS-ORG ITEM
            costRecordId: costRecordB.Id,
            costRecordVersion: 1,
            itemCodeSnapshot: "ITEM-B",
            itemNameSnapshot: LocalizedText.Create("สินค้า B", null),
            unitSnapshot: "PCS",
            unitCostSnapshot: 100m,
            currencySnapshot: "THB",
            costScopeSnapshot: "organization",
            costEffectiveFromUtc: now,
            costPolicyVersion: null,
            resolvedAtUtc: now);

        db.EstimateCostComponents.Add(comp);

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        Assert.NotNull(ex.InnerException);
        Assert.IsType<PostgresException>(ex.InnerException);
    }

    [Fact]
    public async Task EstimateCostComponent_CostRecordFromAnotherOrganization_ThrowsDbUpdateException()
    {
        await using var db = CreateDbContext();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        var orgAId = TestOnlyDataSeeder.TestOrgId;
        var orgBId = TestOnlyDataSeeder.TestOrgBId;
        var branchAId = TestOnlyDataSeeder.TestBranchId;
        var userAId = TestOnlyDataSeeder.TestUserId;
        var now = DateTimeOffset.UtcNow;

        var catA = new ItemCategory(Guid.NewGuid(), orgAId, "CAT-A", LocalizedText.Create("หมวด A", null), null, null, ItemType.All, 0, Guid.NewGuid(), now);
        var unitA = new UnitOfMeasure(Guid.NewGuid(), orgAId, "UNIT-A", LocalizedText.Create("หน่วย A", null), "A", "count", 0, "half_up", Guid.NewGuid(), now);
        db.ItemCategories.Add(catA);
        db.Units.Add(unitA);

        var itemA = Item.CreateDraft(
            Guid.NewGuid(),
            orgAId,
            "ITEM-A",
            ItemType.Material,
            catA.Id,
            null,
            LocalizedText.Create("สินค้า A", null),
            null,
            unitA.Id,
            ItemAvailabilityMode.AllBranches,
            ItemCapabilities.DefaultMaterial,
            null,
            null,
            Guid.NewGuid(),
            now);
        db.Items.Add(itemA);

        var catB = new ItemCategory(Guid.NewGuid(), orgBId, "CAT-B", LocalizedText.Create("หมวด B", null), null, null, ItemType.All, 0, Guid.NewGuid(), now);
        var unitB = new UnitOfMeasure(Guid.NewGuid(), orgBId, "UNIT-B", LocalizedText.Create("หน่วย B", null), "B", "count", 0, "half_up", Guid.NewGuid(), now);
        db.ItemCategories.Add(catB);
        db.Units.Add(unitB);

        var itemB = Item.CreateDraft(
            Guid.NewGuid(),
            orgBId,
            "ITEM-B",
            ItemType.Material,
            catB.Id,
            null,
            LocalizedText.Create("สินค้า B", null),
            null,
            unitB.Id,
            ItemAvailabilityMode.AllBranches,
            ItemCapabilities.DefaultMaterial,
            null,
            null,
            Guid.NewGuid(),
            now);
        db.Items.Add(itemB);

        // CostRecord in Org B
        var costRecordB = CostRecord.CreateDraft(
            Guid.NewGuid(),
            orgBId,
            itemB.Id,
            CostScopeType.Organization,
            null,
            unitB.Id,
            "THB",
            150m,
            0m,
            null,
            now,
            null,
            1,
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            now);
        db.CostRecords.Add(costRecordB);

        // Estimate in Org A
        var customer = Customer.CreateDraft(
            Guid.NewGuid(), orgAId, userAId, CustomerType.Person, "ลูกค้า Org A", null, "th",
            new PrimaryContactInput("สมชาย", null, "0811111111", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), orgAId, branchAId, customer.Id, null, userAId, userAId,
            "ดีล Org A", "ขอบเขต", new[] { "built-in" }, null, 100000m, "THB",
            null, null, null, now);
        db.Opportunities.Add(opp);

        var estimate = Estimate.CreateDraft(
            Guid.NewGuid(),
            orgAId,
            branchAId,
            customer.Id,
            opp.Id,
            "EST-002");
        db.Estimates.Add(estimate);

        var section = new EstimateSection(Guid.NewGuid(), orgAId, estimate.CurrentRevision!.Id, "SEC-01", "Section A", "Section A EN");
        var workItem = new EstimateWorkItem(
            Guid.NewGuid(),
            orgAId,
            section.Id,
            "WI-01",
            "Work Item A",
            "Work Item A EN",
            1m,
            "PCS",
            SellingRuleType.Margin,
            0.2m);
        db.EstimateSections.Add(section);
        db.EstimateWorkItems.Add(workItem);
        await db.SaveChangesAsync();

        var comp = new EstimateCostComponent(
            Guid.NewGuid(),
            orgAId,
            workItem.Id,
            CostComponentType.Material,
            "Comp A",
            1m,
            "PCS",
            150m);

        // Valid Item A (in Org A), but invalid CostRecord B (in Org B)!
        comp.SetCatalogCostSnapshot(
            itemId: itemA.Id,
            costRecordId: costRecordB.Id, // CROSS-ORG COST RECORD
            costRecordVersion: 1,
            itemCodeSnapshot: "ITEM-A",
            itemNameSnapshot: LocalizedText.Create("สินค้า A", null),
            unitSnapshot: "PCS",
            unitCostSnapshot: 150m,
            currencySnapshot: "THB",
            costScopeSnapshot: "organization",
            costEffectiveFromUtc: now,
            costPolicyVersion: null,
            resolvedAtUtc: now);

        db.EstimateCostComponents.Add(comp);

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        Assert.NotNull(ex.InnerException);
        Assert.IsType<PostgresException>(ex.InnerException);
    }

    [Fact]
    public async Task Item_MalformedLocalizedJson_ThrowsPostgresException()
    {
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();

        var org = new Organization(Guid.NewGuid(), "Org", true, DateTimeOffset.UtcNow);
        db.Organizations.Add(org);
        var cat = new ItemCategory(Guid.NewGuid(), org.Id, "CAT", LocalizedText.Create("หมวด", null), null, null, ItemType.All, 0, Guid.NewGuid(), DateTimeOffset.UtcNow);
        var unit = new UnitOfMeasure(Guid.NewGuid(), org.Id, "UNIT", LocalizedText.Create("หน่วย", null), "U", "count", 0, "half_up", Guid.NewGuid(), DateTimeOffset.UtcNow);
        db.ItemCategories.Add(cat);
        db.Units.Add(unit);
        await db.SaveChangesAsync();

        var itemId = Guid.NewGuid();
        var conn = (NpgsqlConnection)db.Database.GetDbConnection();
        await conn.OpenAsync();

        // 1. Array instead of object
        var cmd1 = new NpgsqlCommand(
            "INSERT INTO item_master.items (id, organization_id, code, normalized_code, item_type, category_id, base_unit_id, availability_mode, status, activated_once, row_version, created_at_utc, created_by_user_id, updated_at_utc, updated_by_user_id, can_sell, can_cost, can_purchase, can_stock, can_produce, name) " +
            $"VALUES ('{itemId}', '{org.Id}', 'ITM-BAD', 'ITM-BAD', 'material', '{cat.Id}', '{unit.Id}', 'all_branches', 'draft', false, '{Guid.NewGuid()}', now(), '{Guid.NewGuid()}', now(), '{Guid.NewGuid()}', true, true, true, false, false, '[\"invalid\", \"array\"]'::jsonb)",
            conn);

        var ex1 = await Assert.ThrowsAsync<PostgresException>(() => cmd1.ExecuteNonQueryAsync());
        Assert.Contains("CK_items_name_shape", ex1.ConstraintName ?? ex1.Message);

        // 2. Extra unexpected keys
        var itemId2 = Guid.NewGuid();
        var cmd2 = new NpgsqlCommand(
            "INSERT INTO item_master.items (id, organization_id, code, normalized_code, item_type, category_id, base_unit_id, availability_mode, status, activated_once, row_version, created_at_utc, created_by_user_id, updated_at_utc, updated_by_user_id, can_sell, can_cost, can_purchase, can_stock, can_produce, name) " +
            $"VALUES ('{itemId2}', '{org.Id}', 'ITM-BAD-2', 'ITM-BAD-2', 'material', '{cat.Id}', '{unit.Id}', 'all_branches', 'draft', false, '{Guid.NewGuid()}', now(), '{Guid.NewGuid()}', now(), '{Guid.NewGuid()}', true, true, true, false, false, '{{\"th\": \"ไทย\", \"en\": \"Eng\", \"jp\": \"Nihon\"}}'::jsonb)",
            conn);

        var ex2 = await Assert.ThrowsAsync<PostgresException>(() => cmd2.ExecuteNonQueryAsync());
        Assert.Contains("CK_items_name_shape", ex2.ConstraintName ?? ex2.Message);
    }

    [Fact]
    public async Task ItemAlias_DuplicateNormalizedThOrEn_ForSameItem_ThrowsDbUpdateException()
    {
        await using var db = CreateDbContext();
        await db.Database.MigrateAsync();

        var org = new Organization(Guid.NewGuid(), "Org", true, DateTimeOffset.UtcNow);
        db.Organizations.Add(org);
        var cat = new ItemCategory(Guid.NewGuid(), org.Id, "CAT", LocalizedText.Create("หมวด", null), null, null, ItemType.All, 0, Guid.NewGuid(), DateTimeOffset.UtcNow);
        var unit = new UnitOfMeasure(Guid.NewGuid(), org.Id, "UNIT", LocalizedText.Create("หน่วย", null), "U", "count", 0, "half_up", Guid.NewGuid(), DateTimeOffset.UtcNow);
        db.ItemCategories.Add(cat);
        db.Units.Add(unit);

        var item = Item.CreateDraft(
            Guid.NewGuid(),
            org.Id,
            "ITM-ALIAS",
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
        await db.SaveChangesAsync();

        // 1. Add Alias with English "plywood"
        var alias1 = new ItemAlias(
            Guid.NewGuid(),
            org.Id,
            item.Id,
            LocalizedText.Create("ไม้อัด", "plywood"),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);
        db.ItemAliases.Add(alias1);
        await db.SaveChangesAsync();

        // 2. Try to add another Alias for the SAME item with the same English "plywood" but different Thai "ไม้แบบ"
        var alias2 = new ItemAlias(
            Guid.NewGuid(),
            org.Id,
            item.Id,
            LocalizedText.Create("ไม้แบบ", "plywood"),
            Guid.NewGuid(),
            DateTimeOffset.UtcNow);
        db.ItemAliases.Add(alias2);

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        Assert.NotNull(ex.InnerException);
        Assert.IsType<PostgresException>(ex.InnerException);
    }

    [Fact]
    public async Task ItemUnitConversion_EnforcesFactorSelfLoopAndOrganizationForeignKeys()
    {
        await using (var db = CreateDbContext())
        {
            await TestOnlyDataSeeder.SeedAsync(db, "Test", true);
            var now = DateTimeOffset.UtcNow;
            var orgAId = TestOnlyDataSeeder.TestOrgId;
            var orgBId = TestOnlyDataSeeder.TestOrgBId;
            var actorId = TestOnlyDataSeeder.TestUserId;
            var categoryA = new ItemCategory(Guid.NewGuid(), orgAId, "CONV-CAT-A", LocalizedText.Create("หมวดแปลง A", null), null, null, ItemType.All, 0, actorId, now);
            var categoryB = new ItemCategory(Guid.NewGuid(), orgBId, "CONV-CAT-B", LocalizedText.Create("หมวดแปลง B", null), null, null, ItemType.All, 0, actorId, now);
            var baseUnit = new UnitOfMeasure(Guid.NewGuid(), orgAId, "CONV-BASE-A", LocalizedText.Create("ชิ้น", null), "ea", "count", 0, "half_up", actorId, now);
            var sourceUnit = new UnitOfMeasure(Guid.NewGuid(), orgAId, "CONV-BOX-A", LocalizedText.Create("กล่อง", null), "box", "count", 0, "half_up", actorId, now);
            var foreignUnit = new UnitOfMeasure(Guid.NewGuid(), orgBId, "CONV-FOREIGN-B", LocalizedText.Create("กล่อง B", null), "box", "count", 0, "half_up", actorId, now);
            var item = Item.CreateDraft(Guid.NewGuid(), orgAId, "CONV-ITEM-A", ItemType.Material, categoryA.Id, null,
                LocalizedText.Create("สินค้าแปลงหน่วย", null), null, baseUnit.Id, ItemAvailabilityMode.AllBranches,
                ItemCapabilities.DefaultMaterial, null, null, actorId, now);
            var itemB = Item.CreateDraft(Guid.NewGuid(), orgBId, "CONV-ITEM-B", ItemType.Material, categoryB.Id, null,
                LocalizedText.Create("สินค้าองค์กร B", null), null, foreignUnit.Id, ItemAvailabilityMode.AllBranches,
                ItemCapabilities.DefaultMaterial, null, null, actorId, now);
            db.ItemCategories.AddRange(categoryA, categoryB);
            db.Units.AddRange(baseUnit, sourceUnit, foreignUnit);
            db.Items.AddRange(item, itemB);
            await db.SaveChangesAsync();

            var conversion = new ItemUnitConversion(Guid.NewGuid(), orgAId, item.Id, sourceUnit.Id, baseUnit.Id,
                12m, DateOnly.FromDateTime(now.UtcDateTime), null, "Box contains twelve", actorId, now);
            db.ItemUnitConversions.Add(conversion);
            await db.SaveChangesAsync();

            var barcode = new ItemBarcode(Guid.NewGuid(), orgAId, item.Id, "internal", "CONV-UNIT-A",
                sourceUnit.Id, 12m, "case", false, actorId, now);
            db.ItemBarcodes.Add(barcode);
            await db.SaveChangesAsync();
            db.ItemBarcodes.Add(new ItemBarcode(Guid.NewGuid(), orgBId, itemB.Id, "internal", "CONV-UNIT-A",
                foreignUnit.Id, 12m, "case", false, actorId, now));
            await db.SaveChangesAsync();
            await Assert.ThrowsAsync<PostgresException>(() => db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE item_master.item_barcodes SET quantity_in_base_unit = 0 WHERE id = {barcode.Id}"));

            await Assert.ThrowsAsync<PostgresException>(() => db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE item_master.item_unit_conversions SET factor = 0 WHERE id = {conversion.Id}"));
            await Assert.ThrowsAsync<PostgresException>(() => db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE item_master.item_unit_conversions SET from_unit_id = to_unit_id WHERE id = {conversion.Id}"));
        }

        await using (var db = CreateDbContext())
        {
            var orgAId = TestOnlyDataSeeder.TestOrgId;
            var item = await db.Items.AsNoTracking().SingleAsync(x => x.Code == "CONV-ITEM-A");
            var foreignUnitId = await db.Units.AsNoTracking().Where(x => x.OrganizationId == TestOnlyDataSeeder.TestOrgBId && x.Code == "CONV-FOREIGN-B")
                .Select(x => x.Id).SingleAsync();
            var crossOrganization = new ItemUnitConversion(Guid.NewGuid(), orgAId, item.Id, foreignUnitId, item.BaseUnitId,
                12m, DateOnly.FromDateTime(DateTime.UtcNow), null, "Cross-organization test", TestOnlyDataSeeder.TestUserId, DateTimeOffset.UtcNow);
            db.ItemUnitConversions.Add(crossOrganization);
            var crossOrganizationBarcode = new ItemBarcode(Guid.NewGuid(), orgAId, item.Id, "internal", "CONV-FOREIGN-A",
                foreignUnitId, 12m, "case", false, TestOnlyDataSeeder.TestUserId, DateTimeOffset.UtcNow);
            db.ItemBarcodes.Add(crossOrganizationBarcode);
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }

        await using (var db = CreateDbContext())
        {
            var itemFromOrgB = await db.Items.AsNoTracking().SingleAsync(x => x.Code == "CONV-ITEM-B");
            db.ItemBarcodes.Add(new ItemBarcode(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, itemFromOrgB.Id, "internal", "CONV-FOREIGN-ITEM",
                itemFromOrgB.BaseUnitId, 1m, "each", false, TestOnlyDataSeeder.TestUserId, DateTimeOffset.UtcNow));
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }

        await using (var db = CreateDbContext())
        {
            var item = await db.Items.AsNoTracking().SingleAsync(x => x.Code == "CONV-ITEM-A");
            db.ItemBarcodes.Add(new ItemBarcode(Guid.NewGuid(), item.OrganizationId, item.Id, "internal", "CONV-UNIT-A",
                item.BaseUnitId, 1m, "each", false, TestOnlyDataSeeder.TestUserId, DateTimeOffset.UtcNow));
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }

        var primaryBarcodeId = Guid.NewGuid();
        await using (var db = CreateDbContext())
        {
            var item = await db.Items.AsNoTracking().SingleAsync(x => x.Code == "CONV-ITEM-A");
            db.ItemBarcodes.Add(new ItemBarcode(primaryBarcodeId, item.OrganizationId, item.Id, "internal", "PRIMARY-ONE",
                item.BaseUnitId, 1m, "case", true, TestOnlyDataSeeder.TestUserId, DateTimeOffset.UtcNow));
            await db.SaveChangesAsync();
        }

        await using (var db = CreateDbContext())
        {
            var item = await db.Items.AsNoTracking().SingleAsync(x => x.Code == "CONV-ITEM-A");
            db.ItemBarcodes.Add(new ItemBarcode(Guid.NewGuid(), item.OrganizationId, item.Id, "internal", "PRIMARY-TWO",
                item.BaseUnitId, 1m, "case", true, TestOnlyDataSeeder.TestUserId, DateTimeOffset.UtcNow));
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
    }
}
