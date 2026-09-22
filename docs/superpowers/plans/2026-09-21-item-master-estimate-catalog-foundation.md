# Item Master Estimate Catalog Foundation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** สร้าง Item Master ขั้นต่ำระดับ Production ที่รองรับ Localized JSONB, หลายสาขา, Versioned Cost, Audit และภาพแบบ Private File พร้อมเชื่อมข้อมูลจริงเข้ากับ `estimate-item-catalog-modal.tsx` และตรึง Item/Cost Snapshot ใน Official Estimate

**Architecture:** Item เป็น Aggregate ระดับ Organization; Branch Availability, Item Image และ Cost Record เป็น Relation แยกที่บังคับ Same-organization Scope ฝั่ง Backend Catalog Read Model รับ Branch Context แล้วคืน Structured Projection พร้อม Published Cost และ Verified Primary Image ส่วน Estimate Command จะ Resolve/Validate ราคาใหม่ก่อนบันทึก Snapshot จึงไม่เชื่อราคาใน Browser

**Tech Stack:** .NET 10, ASP.NET Core, EF Core 10, PostgreSQL/Npgsql, xUnit/Testcontainers, Next.js 16, React 19, TypeScript strict, TanStack Query, Zod, Vitest/Testing Library, OpenAPI

## Global Constraints

- ขอบเขตนี้สิ้นสุดที่ Item/Cost/Image Catalog → Estimate Cost Component → Calculation Snapshot; Procurement, Inventory, Production, MRP, Import และ Unit Conversion Chain ไม่อยู่ใน Slice นี้
- ภาษาไทยเป็นค่าเริ่มต้น; UI copy ใหม่ทุกข้อความต้องมีทั้ง `frontend/src/messages/th.json` และ `frontend/src/messages/en.json`
- ห้ามใช้ `any`, `as any`, `@ts-ignore`, arbitrary fallback หรือ Frontend `array.find` เพื่อประกอบความสัมพันธ์จาก Foreign Key
- Frontend ใช้ TanStack Query สำหรับ Server State และใช้ Tailwind semantic tokens; รักษา Atelier Architectural Navy Sharp และ `border-radius: 0px`
- Reuse `AuditEvent`, `IFileStorageProvider`, File Upload Session, `FileClient`, Problem Details, Request Context และ Permission infrastructure เดิม
- EF Core เป็นเจ้าของ Write/Transaction; Catalog/Cost Resolver ใช้ EF projection ก่อน และใช้ Parameterized Dapper Read Model เฉพาะเมื่อ Query Plan พิสูจน์ว่าจำเป็น
- File Selection ใช้ Deferred Upload; Binary อยู่ใน Private Storage และ Item เก็บเฉพาะ Verified File relation
- ทุก Resource lookup บังคับ Organization/Branch Scope ฝั่ง Backend; Resource นอก Scope คืน 404
- ทุก Write ที่แก้ State ต้องเขียน Audit Event ใน Transaction เดียวกันและใช้ optimistic concurrency
- ห้ามแก้ Published Cost หรือ Historical Estimate Snapshot ย้อนหลัง
- เอกสารตัดสินใจหลักคือ ADR 0008, ADR 0012, ADR 0013, Item Master API Contract และ Item Master Data Contract

---

## Slice Boundary

### Included

- Hierarchical Item Category, Brand, Alias และ Unit ขั้นต่ำที่ Item อ้างได้
- Item create/update/activate/deactivate พร้อม Localized JSONB และ Typed Capabilities
- `allBranches|selectedBranches` พร้อม Branch Availability
- Item Image attach/update/reorder/set-primary/detach ผ่าน Verified File
- Organization Default และ Branch-scoped Cost Record แบบ Draft → Submitted → Approved → Published
- Estimate Catalog search/facets/cursor pagination สำหรับ Active `canCost=true` Item
- Base Unit, THB, Organization Default/Branch Override และ Primary Image
- Estimate Cost Component reference/snapshot และ server-side price revalidation
- Audit, permission, concurrency, OpenAPI, Thai/English UI และ automated tests

### Deferred

- Item import, bulk edit และ spreadsheet workflow
- Unit conversion chain, quantity-break management UI และ automatic currency conversion
- Supplier CRUD/contract integration และ Supplier filter ใน Catalog, inventory balance, warehouse, BOM, production และ MRP
- Public CDN, image editing/cropping และ asset transformation service; Slice นี้ใช้ authorized file content/derived thumbnail ที่ File Service รองรับ
- Full Item Master management workspace; Slice นี้ทำ API และ form/list ขั้นต่ำที่จำเป็นต่อการสร้างข้อมูล Catalog จริง

---

### Task 1: Item Domain Model and Localized JSONB Persistence

**Files:**
- Create: `backend/src/TanErp.Domain/Items/LocalizedText.cs`
- Create: `backend/src/TanErp.Domain/Items/ItemConstants.cs`
- Create: `backend/src/TanErp.Domain/Items/Item.cs`
- Create: `backend/src/TanErp.Domain/Items/ItemCategory.cs`
- Create: `backend/src/TanErp.Domain/Items/ItemBrand.cs`
- Create: `backend/src/TanErp.Domain/Items/ItemAlias.cs`
- Create: `backend/src/TanErp.Domain/Items/UnitOfMeasure.cs`
- Create: `backend/src/TanErp.Domain/Items/ItemBranchAvailability.cs`
- Create: `backend/tests/TanErp.UnitTests/Items/ItemTests.cs`
- Create: `backend/tests/TanErp.UnitTests/Items/LocalizedTextTests.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemCategoryConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemBrandConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemAliasConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/UnitOfMeasureConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemBranchAvailabilityConfiguration.cs`
- Modify: `backend/src/TanErp.Application/Common/Abstractions/IApplicationDbContext.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/ItemPersistenceTests.cs`

**Interfaces:**
- Produces: `LocalizedText.Create(string th, string? en)`, `Item.CreateDraft(...)`, `Item.UpdateDetails(...)`, `Item.Activate(...)`, `Item.Deactivate(...)`
- Produces: `ItemAvailabilityMode.AllBranches|SelectedBranches` และ `ItemBranchAvailability.Create(...)`
- Produces: `DbSet<Item> Items`, `DbSet<ItemCategory> ItemCategories`, `DbSet<ItemBrand> ItemBrands`, `DbSet<ItemAlias> ItemAliases`, `DbSet<UnitOfMeasure> Units`, `DbSet<ItemBranchAvailability> ItemBranchAvailabilities`

- [ ] **Step 1: Write failing unit tests for localized text and lifecycle invariants**

```csharp
[Fact]
public void Activate_requires_thai_name_base_unit_capability_and_selected_branch()
{
    var item = Item.CreateDraft(ItemTestData.ValidDraft with
    {
        Name = LocalizedText.Create("", null),
        AvailabilityMode = ItemAvailabilityMode.SelectedBranches
    });

    var error = Assert.Throws<ItemValidationException>(() =>
        item.Activate(Guid.NewGuid(), DateTimeOffset.UtcNow, hasActiveSelectedBranch: false));

    Assert.Equal("ITEM_FIELD_REQUIRED", error.Code);
}
```

Cover allowed locale keys through `LocalizedText`, immutable code after first activation, at least one capability, deactivation reason, selected-branch gate, category cycle, optional Brand for Labor/Service, normalized Alias uniqueness and row-version change.

- [ ] **Step 2: Run focused tests and confirm the expected failure**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~Items"`

Expected: FAIL because Item domain types do not exist.

- [ ] **Step 3: Implement the domain types with explicit invariants**

```csharp
public sealed record LocalizedText(string Thai, string? English)
{
    public static LocalizedText Create(string thai, string? english) =>
        new(NormalizeRequired(thai, 250), NormalizeOptional(english, 250));
}

public static class ItemAvailabilityMode
{
    public const string AllBranches = "all_branches";
    public const string SelectedBranches = "selected_branches";
}
```

Expose behavior methods rather than public setters. Keep capabilities as typed booleans and attributes as a validated dictionary whose keys/values are bounded strings and cannot use reserved keys such as `price`, `status`, `permission`, `currency` or `unitCost`.

- [ ] **Step 4: Map PostgreSQL JSONB and same-organization relationships**

Reuse one `LocalizedText` value object/EF converter for Item, Category, Brand, Alias and Unit. Add check constraints for object/type/Thai activation gate, unique `(organization_id, normalized_code)`, unique `(organization_id,id)` principal keys, expression indexes for localized names, category parent cycle guard, Alias normalized indexes and composite FKs for Category, Brand, Unit and Branch Availability.

- [ ] **Step 5: Add persistence integration tests**

Verify JSON round-trip for every localized master, duplicate normalized code/Alias rejection, category cycle rejection, Brand/selected Branch from another organization rejection, allowed same code in a second organization and concurrency conflict.

- [ ] **Step 6: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~Items"`

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemPersistence"`

Commit: `feat(items): add localized item and branch domain model`

---

### Task 2: Item Schema Migration and Audit Hardening

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddItemMasterCatalogFoundation.cs` (ชื่อจริงใช้ timestamp ที่ EF Core สร้าง)
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs`
- Modify: `backend/src/TanErp.Domain/Common/AuditEvent.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/AuditEventConfiguration.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/ItemSchemaConstraintTests.cs`

**Interfaces:**
- Consumes: Domain mappings from Task 1
- Produces: `item_master` schema and append-only-compatible audit indexes
- Produces: optional audit context `branchId`, `actorMembershipId`, `requestId`, `rowVersionBefore`, `rowVersionAfter`, `reason`

- [ ] **Step 1: Write constraint tests against PostgreSQL**

Test invalid JSON shape, missing Thai name at activation, unknown locale key, duplicate primary natural keys, cross-organization FK, invalid availability period and direct update of immutable audit identity fields.

- [ ] **Step 2: Generate the migration**

Run: `dotnet ef migrations add AddItemMasterCatalogFoundation --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api`

Expected: migration creates `item_master.items`, `item_categories`, `units`, `item_branch_availabilities` plus constraints/indexes and audit columns/indexes.

- [ ] **Step 3: Review generated SQL before applying**

Run: `dotnet ef migrations script --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api`

Confirm there is no table drop, no public file URL column, no `current_cost`, and every organization-owned FK proves same-organization scope.

- [ ] **Step 4: Extend the shared audit entity without duplicating an Item audit table**

Add nullable context fields while preserving existing constructors through an overload or parameter object. Add indexes:

```text
(organization_id, resource_type, resource_id, occurred_at_utc DESC)
(organization_id, occurred_at_utc DESC)
```

- [ ] **Step 5: Run integration tests and commit**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemSchemaConstraint"`

Commit: `feat(items): add catalog schema and audit constraints`

---

### Task 3: Item Commands, Permissions, and Minimal Management API

**Files:**
- Create: `backend/src/TanErp.Application/Items/IItemStore.cs`
- Create: `backend/src/TanErp.Application/Items/ItemProjections.cs`
- Create: `backend/src/TanErp.Application/Items/CreateItem/CreateItemCommand.cs`
- Create: `backend/src/TanErp.Application/Items/CreateItem/CreateItemHandler.cs`
- Create: `backend/src/TanErp.Application/Items/UpdateItem/UpdateItemCommand.cs`
- Create: `backend/src/TanErp.Application/Items/UpdateItem/UpdateItemHandler.cs`
- Create: `backend/src/TanErp.Application/Items/ChangeItemStatus/ChangeItemStatusHandlers.cs`
- Create: `backend/src/TanErp.Application/Items/UpdateBranchAvailability/UpdateBranchAvailabilityHandler.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/ItemStore.cs`
- Create: `backend/src/TanErp.Api/Contracts/Items/ItemContracts.cs`
- Create: `backend/src/TanErp.Api/Contracts/Items/ItemTaxonomyContracts.cs`
- Create: `backend/src/TanErp.Api/Controllers/ItemsController.cs`
- Create: `backend/src/TanErp.Api/Controllers/ItemTaxonomyController.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Modify: `backend/src/TanErp.Domain/IdentityAccess/Permission.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/ItemEndpointsTests.cs`

**Interfaces:**
- Produces: `POST /api/v1/items`, `GET /api/v1/items/{id}`, `PATCH /api/v1/items/{id}`, activate/deactivate and branch-availability endpoints
- Produces: permissions `items.read`, `items.create`, `items.update`, `items.activate`, `items.deactivate`, `items.manage-taxonomy`, `items.manage-branches`

- [ ] **Step 1: Write API tests for permission, scope, ETag and audit**

```csharp
[Fact]
public async Task Update_branch_availability_rejects_cross_organization_branch()
{
    var response = await Client.PutAsJsonAsync(
        $"/api/v1/items/{_itemId}/branch-availability",
        new { mode = "selectedBranches", branchIds = new[] { _otherOrganizationBranchId } });

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}
```

Also cover Category parent cycle, Brand CRUD, Alias search identity, create, duplicate code, activate gates, stale ETag, deactivate reason, unauthorized permission and one audit row per successful mutation.

- [ ] **Step 2: Define a narrow store interface**

```csharp
public interface IItemStore
{
    Task<Result<ItemProjection>> CreateAsync(CreateItemData data, RequestAccessContext access, CancellationToken ct);
    Task<ItemProjection?> GetAsync(Guid organizationId, Guid itemId, CancellationToken ct);
    Task<Result<ItemProjection>> UpdateAsync(UpdateItemData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemProjection>> SetBranchAvailabilityAsync(SetItemBranchAvailabilityData data, RequestAccessContext access, CancellationToken ct);
}
```

- [ ] **Step 3: Implement handlers and store transactions**

Resolve access with membership-derived organization. Write Item/Branch state and `AuditEvent` together. Map duplicate key/concurrency violations to stable codes `ITEM_CODE_CONFLICT` and `ITEM_VERSION_CONFLICT`; never compare exception message text in Frontend.

- [ ] **Step 4: Add controller contracts and OpenAPI output**

Use structured `name`, `description`, category, brand, aliases, base unit, capabilities and availability projection. Return ETag on read/write and RFC Problem Details on failures.

- [ ] **Step 5: Run API tests and commit**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemEndpoints"`

Commit: `feat(items): add scoped item management api`

---

### Task 4: Production Item Image Relation over the Existing File Service

**Files:**
- Create: `backend/src/TanErp.Domain/Items/ItemImage.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemImageConfiguration.cs`
- Create: `backend/src/TanErp.Application/Items/Images/IItemImageStore.cs`
- Create: `backend/src/TanErp.Application/Items/Images/ItemImageHandlers.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/ItemImageStore.cs`
- Create: `backend/src/TanErp.Api/Contracts/Items/ItemImageContracts.cs`
- Modify: `backend/src/TanErp.Api/Controllers/ItemsController.cs`
- Modify: `backend/src/TanErp.Domain/Files/FileConstants.cs`
- Modify: `backend/src/TanErp.Domain/Files/UploadedFile.cs`
- Modify: `backend/src/TanErp.Infrastructure/Files/FileParentAccessResolver.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/UploadedFileConfiguration.cs`
- Test: `backend/tests/TanErp.UnitTests/Items/ItemImageTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/ItemImageEndpointsTests.cs`

**Interfaces:**
- Produces: File parent type `item`
- Produces: `POST/PATCH/DELETE /api/v1/items/{itemId}/images...`
- Produces: `ItemImageProjection(Guid Id, Guid FileId, string Role, bool IsPrimary, int DisplayOrder, LocalizedTextProjection AltText, LocalizedTextProjection? Caption, Guid RowVersion)`

- [ ] **Step 1: Write failing tests for file-parent and primary-image invariants**

Cover Verified file only, same organization, exact `parentType=item` and `parentId`, one active primary, stable reorder, stale ETag, unauthorized download and detach without immediate binary deletion.

- [ ] **Step 2: Extend File Service instead of creating another uploader**

Add `FileParentTypes.Item`, resolve `items.read` for read and `items.manage-images` for upload/write, and retain the existing 10 MB JPEG/PNG/WebP allowlist/magic-number verification path.

- [ ] **Step 3: Add safe file metadata fields**

Persist `content_sha256`, image width/height, `scan_status` and `verified_at_utc`; storage paths remain server-owned. Do not expose storage path or signed URL in Item API responses.

- [ ] **Step 4: Implement atomic image commands**

Attaching, changing primary, reordering and detaching must update relation + audit atomically. A primary change clears the previous primary inside the same transaction before inserting/updating the new one, while the partial unique index remains the final race guard.

- [ ] **Step 5: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~ItemImage"`

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemImage"`

Commit: `feat(items): attach verified private item images`

---

### Task 5: Versioned Cost Workflow and Deterministic Resolver

**Files:**
- Create: `backend/src/TanErp.Domain/Items/CostRecord.cs`
- Create: `backend/src/TanErp.Domain/Items/CostRecordReview.cs`
- Create: `backend/src/TanErp.Domain/Items/CostSource.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CostRecordConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CostRecordReviewConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CostSourceConfiguration.cs`
- Create: `backend/src/TanErp.Application/Items/Costs/ICostRecordStore.cs`
- Create: `backend/src/TanErp.Application/Items/Costs/ICostResolver.cs`
- Create: `backend/src/TanErp.Application/Items/Costs/CostRecordHandlers.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/CostRecordStore.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/CostResolver.cs`
- Create: `backend/src/TanErp.Api/Controllers/CostRecordsController.cs`
- Modify: `backend/src/TanErp.Api/Controllers/ItemsController.cs`
- Test: `backend/tests/TanErp.UnitTests/Items/CostRecordTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/CostResolverTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/CostRecordEndpointsTests.cs`

**Interfaces:**
- Produces: `ResolvedCostProjection` with record ID/version, source, amount, currency, unit, scope, effective period and policy version
- Produces: Draft → Submitted → Approved/Returned → Published/Superseded/Disabled transitions

- [ ] **Step 1: Write lifecycle and resolver tests**

Cover maker cannot approve own record, only Published resolves, branch wins organization default, latest effective-from wins, overlap is rejected, a tie returns `ITEM_COST_AMBIGUOUS`, missing cost returns `ITEM_COST_NOT_FOUND`, and historical published records remain immutable.

- [ ] **Step 2: Implement cost entities and database constraints**

Use decimal columns from the Data Contract, same-organization FKs, immutable financial fields after publish, append-only reviews and a transaction/exclusion guard against overlapping published natural keys.

- [ ] **Step 3: Implement the deterministic resolver**

```csharp
public sealed record ResolveCostRequest(
    Guid OrganizationId,
    Guid BranchId,
    Guid ItemId,
    Guid UnitId,
    string Currency,
    decimal Quantity,
    DateTimeOffset EffectiveAtUtc,
    string PolicyVersion);

public interface ICostResolver
{
    Task<Result<ResolvedCostProjection>> ResolveAsync(ResolveCostRequest request, CancellationToken ct);
}
```

Apply the ordering from Governance and return ambiguity instead of picking an arbitrary row.

- [ ] **Step 4: Implement command endpoints and atomic audit**

Create, submit, review, publish and disable endpoints must use permissions, idempotency where specified, ETag and audit in the same transaction.

- [ ] **Step 5: Run focused tests and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~CostRecord"`

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~Cost"`

Commit: `feat(items): add governed versioned cost resolver`

---

### Task 6: Estimate Catalog Read Model API

**Files:**
- Create: `backend/src/TanErp.Application/Items/Catalog/EstimateCatalogQuery.cs`
- Create: `backend/src/TanErp.Application/Items/Catalog/EstimateCatalogProjection.cs`
- Create: `backend/src/TanErp.Application/Items/Catalog/IEstimateCatalogReader.cs`
- Create: `backend/src/TanErp.Application/Items/Catalog/SearchEstimateCatalogHandler.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/EstimateCatalogReader.cs`
- Create: `backend/src/TanErp.Api/Contracts/Items/EstimateCatalogResponse.cs`
- Create: `backend/src/TanErp.Api/Controllers/EstimateCatalogController.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/EstimateCatalogEndpointsTests.cs`

**Interfaces:**
- Produces: `GET /api/v1/estimate-catalog/items`
- Produces: items, facets and cursor `pageInfo` matching `item-master-api-contract.md`

- [ ] **Step 1: Write contract tests for branch-scoped catalog behavior**

Seed two organizations, two branches, Active/Inactive items, all/selected availability, organization/branch costs and verified/unverified images. Assert no cross-organization leak, no inactive/unavailable item, branch override precedence, primary verified image only and stable cursor ordering.

- [ ] **Step 2: Implement one structured server projection**

Return category/parent-category/brand/base-unit/image/cost as nested objects from the query. Join Active Alias in server-side search without replacing Canonical Name. Do not return raw foreign keys for Frontend lookup and do not load all rows before filtering/pagination.

- [ ] **Step 3: Enforce query limits**

Normalize search, require Branch ID, clamp `pageSize` to 1–100, cap filter/attribute counts, use stable `(normalized_code,id)` cursor and cancel database work when the request aborts.

- [ ] **Step 4: Register endpoint and update OpenAPI**

Run the API OpenAPI generation path and verify the response schema has no `currentCost`, storage path or internal evidence details.

- [ ] **Step 5: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateCatalog"`

Commit: `feat(items): expose branch-aware estimate catalog`

---

### Task 7: Estimate Cost Component Item Reference and Immutable Snapshot

**Files:**
- Modify: `backend/src/TanErp.Domain/Estimates/EstimateCostComponent.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/EstimateCostComponentConfiguration.cs`
- Modify: `backend/src/TanErp.Application/Estimates/IEstimateStore.cs`
- Modify: `backend/src/TanErp.Application/Estimates/UpdateEstimateDraft/UpdateEstimateDraftHandler.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Estimates/UpdateEstimateDraftRequest.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Estimates/EstimateDetailResponse.cs`
- Modify: `backend/tests/TanErp.UnitTests/Estimates/EstimateTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddEstimateItemCostSnapshots.cs` (ชื่อจริงใช้ timestamp ที่ EF Core สร้าง)

**Interfaces:**
- Consumes: `ICostResolver` from Task 5
- Consumes: Catalog identity from Task 6
- Produces: nullable Item/Cost identity plus immutable snapshot fields on Estimate Cost Component

- [ ] **Step 1: Write failing tests for catalog and manual components**

Assert a catalog component requires `itemId` + `costRecordId`, re-resolves against Estimate Branch/Quantity/Unit/EffectiveAt, rejects stale/wrong records with `ITEM_COST_VERSION_CONFLICT`, stores the resolved snapshot, and keeps manual components valid only with provisional reason rules.

- [ ] **Step 2: Extend the request DTO without trusting browser price**

```csharp
public sealed record EstimateCostComponentDraftDto(
    Guid? Id,
    Guid? ItemId,
    Guid? CostRecordId,
    string Type,
    string Description,
    decimal Quantity,
    string UnitCode,
    decimal? ManualUnitCost,
    string? Currency,
    string? ProvisionalReason,
    int SortOrder);
```

For catalog rows ignore/reject client-provided cost authority and use the resolver result; for manual rows require `ManualUnitCost` and reason according to Governance.

- [ ] **Step 3: Persist explicit reference and snapshot columns**

Add nullable `item_id`, `cost_record_id` and the snapshot columns from the Data Contract. Preserve existing rows as manual components during migration and do not backfill invented Item IDs.

- [ ] **Step 4: Keep snapshots immutable across later master changes**

When quantity/unit/item identity changes, resolve a new snapshot. When only Estimate selling data changes, retain the stored Item/Cost snapshot. Calculation reads the stored validated snapshot and never queries an unversioned current price.

- [ ] **Step 5: Run Estimate regression tests and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~Estimates"`

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateEndpoints"`

Commit: `feat(estimates): snapshot catalog item costs`

---

### Task 8: Frontend Catalog API Adapter and TanStack Query Hook

**Files:**
- Create: `frontend/src/features/estimates/types/estimate-catalog.ts`
- Create: `frontend/src/features/estimates/api/estimate-catalog-client.ts`
- Create: `frontend/src/features/estimates/api/estimate-catalog-client.test.ts`
- Create: `frontend/src/features/estimates/hooks/use-estimate-catalog.ts`
- Modify: `frontend/src/generated/api/tan-erp.v1.ts`
- Modify: `frontend/src/features/estimates/hooks/useCatalogFilter.ts`
- Modify: `frontend/src/lib/api/file-client.ts`

**Interfaces:**
- Consumes: OpenAPI `EstimateCatalogResponse`
- Produces: `useEstimateCatalog({ branchId, search, filters, cursor })`
- Produces: authenticated object URL lifecycle for private Item thumbnails through `FileClient.getFileBlob`

- [ ] **Step 1: Write client mapping/error tests**

Test exact structured mapping, decimal-string parsing at display boundary, empty `description.en`, no arbitrary localized fallback, AbortSignal propagation and RFC Problem Details mapping.

- [ ] **Step 2: Generate types and define a narrow UI model**

Run: `cd frontend && npm run generate:api`

```ts
export interface EstimateCatalogItem {
  id: string;
  code: string;
  name: { th: string; en: string | null };
  description: { th: string | null; en: string | null };
  itemType: string;
  category: { id: string; name: { th: string; en: string | null } };
  brand: { id: string; name: { th: string; en: string | null } } | null;
  baseUnit: { id: string; code: string; symbol: string };
  primaryImage: { fileId: string; altText: { th: string; en: string | null } } | null;
  resolvedCost: { costRecordId: string; version: number; amount: string; currency: string; unitCode: string; scope: string };
}
```

- [ ] **Step 3: Replace local-array filtering with query parameters**

Keep ephemeral UI state such as selected IDs locally, but derive rows/facets/page info from TanStack Query. Debounce search in the hook/component, preserve previous page data during cursor navigation and show explicit loading/empty/error states. Expose Category and Brand facets; omit Supplier filter until Supplier Master has an accepted contract.

- [ ] **Step 4: Implement private image loading without persistent bearer URLs**

Fetch blobs with authentication, create object URLs only in memory and revoke them on replacement/unmount. A failed image renders the existing local placeholder and accessible alt text; it must not retry indefinitely.

- [ ] **Step 5: Run frontend focused tests and commit**

Run: `cd frontend && npm test -- --run src/features/estimates/api/estimate-catalog-client.test.ts`

Commit: `feat(estimates): load catalog from item api`

---

### Task 9: Connect the Existing Catalog Modal and Estimate Draft Payload

**Files:**
- Modify: `frontend/src/features/estimates/components/estimate-item-catalog-modal.tsx`
- Modify: `frontend/src/features/estimates/components/estimate-item-catalog-modal.test.tsx`
- Modify: `frontend/src/features/estimates/components/catalog/catalog-filter-sidebar.tsx`
- Modify: `frontend/src/features/estimates/components/catalog/catalog-items-table.tsx`
- Modify: `frontend/src/features/estimates/components/catalog/catalog-item-detail-drawer.tsx`
- Modify: `frontend/src/components/catalog/CatalogItemDetailDrawer.tsx`
- Modify: `frontend/src/features/estimates/components/estimate-cost-component-table.tsx`
- Modify: `frontend/src/features/estimates/schemas/estimate-workspace-schema.ts`
- Modify: `frontend/src/features/estimates/components/estimate-workspace-drawer.tsx`
- Modify: `frontend/src/lib/api/api-client.ts`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`
- Delete after replacement: `frontend/src/features/estimates/constants/estimate-catalog-items.ts`
- Delete after replacement: `frontend/src/features/estimates/constants/estimate-catalog-items.test.ts`

**Interfaces:**
- Consumes: `useEstimateCatalog` and `EstimateCatalogItem`
- Produces: selected rows carrying `itemId`, `costRecordId`, quantity and unit code into Estimate Draft

- [ ] **Step 1: Rewrite modal tests around mocked server state**

Cover initial loading, API error/retry, empty search, cursor next/previous, filters/facets, multi-select across one page, detail drawer, authenticated thumbnail fallback, branch change clearing invalid selection and keyboard focus restoration.

- [ ] **Step 2: Preserve the existing modal composition while replacing the data source**

Keep the modal, table, filter sidebar and detail drawer components reusable. Remove imports of `ESTIMATE_CATALOG_ITEMS`; accept `branchId` from Estimate context and call the query hook. Keep Category/Brand/Attributes filters backed by server facets and remove the static Supplier filter from this Slice.

- [ ] **Step 3: Map selected items to authoritative identities**

```ts
const component = {
  itemId: item.id,
  costRecordId: item.resolvedCost.costRecordId,
  type: item.itemType,
  description: item.name.th,
  quantity: "1.0000",
  unitCode: item.resolvedCost.unitCode,
  manualUnitCost: null,
  provisionalReason: null,
};
```

Display amount from the Catalog projection, but treat the saved response from Backend as authoritative after draft update.

- [ ] **Step 4: Add Thai and English messages**

Add translations for loading, no result, unavailable branch, price changed, image unavailable, retry, pagination and selected count. Do not hardcode visible copy in TSX.

- [ ] **Step 5: Run component tests and commit**

Run: `cd frontend && npm test -- --run src/features/estimates/components/estimate-item-catalog-modal.test.tsx`

Run: `cd frontend && npm run typecheck`

Commit: `feat(estimates): connect item catalog modal to backend`

---

### Task 10: End-to-End Security, Verification, and Documentation Evidence

**Files:**
- Create: `backend/tests/TanErp.IntegrationTests/Api/ItemCatalogEstimateFlowTests.cs`
- Create: `frontend/e2e/estimate-item-catalog.spec.ts`
- Create: `docs/05-engineering/item-master-estimate-catalog-verification.md`
- Modify: `docs/README.md`
- Modify only if implementation diverges: `docs/03-contracts/item-master-api-contract.md`
- Modify only if implementation diverges: `docs/04-data/item-master-data-contract.md`

**Interfaces:**
- Consumes: all previous tasks
- Produces: reproducible evidence for UAT-ITEM-019 through UAT-ITEM-026 and completion gates

- [ ] **Step 1: Add a backend end-to-end integration scenario**

Create Category parent/child, Brand and Alias; create an Item with Thai/English JSONB, selected branch, verified primary image, organization default plus branch cost; find it by Alias and Brand filter; publish through a distinct checker; query Catalog; save it into an Estimate; publish a newer cost; assert the historical Estimate snapshot remains unchanged.

- [ ] **Step 2: Add security regression cases**

Test cross-organization Item/Branch/File/Cost IDs, missing permissions, unverified file, stale ETag, reused idempotency key with different payload, oversized search/filter input and client-tampered price.

- [ ] **Step 3: Add browser flow coverage**

At desktop and 320px viewport, open Estimate Catalog, search, filter, select an item, verify private image/placeholder, insert into BOQ, save, reload and confirm snapshot values. Verify keyboard operation, visible focus and no important content clipping at 200% zoom.

- [ ] **Step 4: Run all completion gates**

Run: `dotnet build backend/TanErp.sln`

Run: `dotnet test backend/TanErp.sln`

Run: `cd frontend && npm run verify`

Run: `cd frontend && npm run test:e2e -- estimate-item-catalog.spec.ts`

Expected: all commands exit 0. Record exact command, timestamp, commit SHA and result in the verification document; do not claim completion if any gate was skipped or failed.

- [ ] **Step 5: Verify documentation integrity**

Run: `rg -n "nameTh|nameEn|descriptionTh|descriptionEn|currentCost|ESTIMATE_CATALOG_ITEMS" docs/01-business/item-master-* docs/03-contracts/item-master-api-contract.md docs/04-data/item-master-data-contract.md frontend/src/features/estimates`

Expected: legacy names remain only where explicitly documenting migration/history; static catalog is absent from production code.

Run: `git diff --check`

- [ ] **Step 6: Commit verification evidence**

Commit: `test(items): verify estimate catalog foundation`

---

## Delivery Checkpoints

1. **Checkpoint A — Safe Master Data:** Tasks 1–4 pass; Item/Branch/Image data can be maintained securely but Catalog is not yet enabled for Estimate users.
2. **Checkpoint B — Authoritative Pricing:** Task 5 passes; only Published deterministic costs are selectable.
3. **Checkpoint C — End-to-End Estimate Flow:** Tasks 6–9 pass; Catalog modal uses API data and Estimate stores immutable snapshots.
4. **Checkpoint D — Production Evidence:** Task 10 passes; security, responsive UX and full verification evidence are recorded.

## Rollout and Backout

- Deploy additive migrations before API/Frontend; existing Estimate components remain manual because new reference fields are nullable.
- Enable Catalog API and modal through one server-owned feature flag per Organization until Checkpoint D evidence is approved.
- Backout disables the feature flag and restores manual entry; do not drop Item/Cost/Image tables or erase snapshots.
- Existing Estimate rows remain valid as manual components; no invented Item mapping or destructive data migration is allowed.

## Definition of Done

- `estimate-item-catalog-modal.tsx` ไม่มี Production dependency ต่อ static catalog
- Item เดียวใช้หลายสาขาได้โดยไม่ duplicate และ Branch Cost แยกจาก Availability
- Category hierarchy, Brand และ Alias เป็น scoped relational masters; Supplier Integration ถูก defer อย่างชัดเจน
- Localized text ถูกเก็บเป็น constrained JSONB และค้นหาได้ด้วย server-side indexed query
- Item Image ใช้ Verified private file, Parent Invariant, one-primary constraint และ Audit
- Catalog คืนเฉพาะ Active/authorized/branch-available Item พร้อม Published deterministic cost
- Estimate บันทึก Item/Cost identity และ immutable snapshot; ราคา Client ไม่ใช่ Authority
- Audit ครบทุก mutation สำคัญและไม่มี secret/binary/signed URL
- OpenAPI, Backend tests, Frontend tests, build, lint, typecheck และ E2E ผ่านพร้อม Verification Record
