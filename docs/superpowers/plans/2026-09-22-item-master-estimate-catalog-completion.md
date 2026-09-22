# Item Master Estimate Catalog Completion Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ปิดงานค้างจาก Item Master Estimate Catalog Foundation ให้เป็น vertical slice ที่ใช้งานจริงได้ตั้งแต่ดูแล Item/Taxonomy/Cost/Image จนเลือก Catalog เข้า Official Estimate พร้อม immutable snapshot, audit และหลักฐาน verification ระดับ production

**Architecture:** รักษา Item เป็น aggregate ระดับ Organization และแยก Branch Availability, private Item Image และ versioned Cost เป็น relation ที่พิสูจน์ same-organization ด้วย composite foreign key ทุก mutation ผ่าน Application handler/store และเขียน AuditEvent ใน transaction เดียวกัน Catalog API คืน structured projection ที่ resolve ราคาและรูปไว้แล้ว ส่วน Estimate server จะ resolve ราคาใหม่และบันทึก immutable snapshot โดยไม่เชื่อราคาจาก browser

**Tech Stack:** .NET SDK 10.0.400, ASP.NET Core 10, EF Core 10, PostgreSQL/Npgsql, xUnit/Testcontainers, Next.js 16, React 19, TypeScript strict, TanStack Query, Vitest/Testing Library, Playwright, OpenAPI

> **Code-review gate:** ปิดข้อค้นพบใน [Item Master Catalog Code Review Remediation Plan](2026-09-22-item-master-catalog-code-review-remediation.md) ก่อนเดินงาน completion ต่อ เพื่อไม่ต่อยอดจาก migration, tenant boundary หรือ pricing behavior ที่ยังไม่ผ่าน review

## Global Constraints

- แผนนี้ต่อจาก `2026-09-21-item-master-estimate-catalog-foundation.md`; ห้ามสร้าง schema หรือ implementation คู่ขนานกับของที่มีแล้ว
- ขอบเขตจบที่ Item/Cost/Image Catalog → Estimate Cost Component → Calculation Snapshot; Procurement, Inventory, Production, MRP, Import, Supplier CRUD และ Unit Conversion Chain ยัง defer
- ใช้ .NET SDK `10.0.400` จาก `backend/global.json`; ห้าม downgrade target framework หรือ package เพื่อหลบปัญหา build
- UI copy ใหม่ทุกข้อความต้องอยู่ใน `frontend/src/messages/th.json` และ `frontend/src/messages/en.json`; ภาษาไทยเป็นค่าเริ่มต้น
- ห้ามใช้ `any`, `as any`, `@ts-ignore`, arbitrary fallback หรือ frontend `array.find` เพื่อประกอบ relation จาก foreign key
- Frontend ใช้ TanStack Query เป็นเจ้าของ server state, Tailwind semantic tokens, zero radius และ SVG icons ตาม `design.md`
- Reuse `AuditEvent`, Request Access, RFC 7807 Problem Details, File Upload Session, `IFileStorageProvider` และ authorized file-content endpoint เดิม
- ทุก lookup ต้อง scope ด้วย Organization และ Branch ที่ได้จาก membership; ID นอก scope ตอบ 404 เพื่อไม่เปิดเผยการมีอยู่ของ resource
- ทุก mutation ต้องใช้ optimistic concurrency และบันทึก AuditEvent ใน transaction เดียวกัน; Published Cost และ Estimate snapshot ห้ามแก้ย้อนหลัง
- File upload ต้อง deferred จน submit, binary อยู่ private storage, API ห้ามคืน storage path หรือ bearer/signed URL อายุยาว
- ใช้ EF Core สำหรับ write/transaction; Catalog read เริ่มด้วย EF projection และเปลี่ยนเป็น parameterized SQL เฉพาะเมื่อมี query-plan evidence
- ห้าม claim ว่าเสร็จก่อน `dotnet build`, `dotnet test`, `npm run verify` และ focused Playwright ผ่านจริง

---

## Current Baseline (2026-09-22)

มีแล้วและต้อง reuse:

- Domain/mapping/migrations สำหรับ Item, Category, Brand, Alias, Unit, Branch Availability, Item Image, Cost Source และ Cost Record
- `ICostResolver`/`CostResolver`, `IEstimateCatalogReader`/`EstimateCatalogReader` และ `GET /api/v1/estimate-catalog/items`
- Item/Cost identity และ snapshot columns ใน `EstimateCostComponent` พร้อม server-side cost revalidation ใน `EstimateStore`
- OpenAPI contract, generated TypeScript types, Catalog API adapter, TanStack Query hook และ modal ที่อ่านข้อมูลจาก server
- Unit tests สำหรับ Item/Image/Cost และ persistence tests เบื้องต้น

งานค้างที่แผนนี้เป็นเจ้าของ:

- ตรวจและทำ migration ล่าสุดให้ผ่านทั้งฐานข้อมูลว่างและฐานข้อมูลที่มี migration เดิม
- Item/Taxonomy/Branch Availability management API พร้อม ETag, permission และ audit
- Item Image file-parent invariant, metadata และ atomic image commands
- Cost workflow API, reviews, overlap protection และ audit
- Catalog query hardening, facets, valid cursor และ single structured projection
- Estimate snapshot regression/integration tests และ price-conflict UX
- Frontend private thumbnail, server-state component tests, static catalog removal และ E2E
- verification record และ rollout/backout evidence

---

### Task 1: Stabilize the Existing Schema and Migration Chain

**Files:**
- Modify: `backend/src/TanErp.Domain/Files/FileConstants.cs`
- Modify: `backend/src/TanErp.Domain/Files/FileUploadSession.cs`
- Modify: `backend/src/TanErp.Domain/Files/UploadedFile.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/UploadedFileConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemImageConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CostSourceConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_HardenItemFileMetadata.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Persistence/ItemSchemaConstraintTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Persistence/ItemPersistenceTests.cs`

**Interfaces:**
- Consumes: existing `UploadedFile`, Item schema migrations and `AppDbContext`
- Produces: `FileParentTypes.Item`, same-organization file FKs and safe metadata fields `ContentSha256`, `ImageWidth`, `ImageHeight`, `ScanStatus`, `VerifiedAtUtc`

- [ ] **Step 1: Add failing PostgreSQL constraint tests**

```csharp
[Fact]
public async Task Item_image_rejects_verified_file_from_another_organization()
{
    var image = ItemImage.Create(Guid.NewGuid(), OrgA, ItemA, FileFromOrgB,
        "primary", true, 0, LocalizedText.Create("ภาพหลัก", "Primary image", 250),
        null, UserA, DateTimeOffset.UtcNow);

    Db.ItemImages.Add(image);
    await Assert.ThrowsAsync<DbUpdateException>(() => Db.SaveChangesAsync());
}
```

Also assert invalid localized JSON shape, duplicate normalized item/alias code, selected-branch cross-organization FK, invalid cost ranges, more than one primary image and invalid SHA-256 length are rejected by PostgreSQL rather than UI validation alone.

- [ ] **Step 2: Run the focused tests and preserve the failure output**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemSchemaConstraint|FullyQualifiedName~ItemPersistence"`

Expected: new cross-organization file/metadata cases fail before the final mapping and migration are complete.

- [ ] **Step 3: Finish safe file metadata capture**

Update `CompleteUploadSessionHandler` to buffer at most 10 MB, verify actual byte count, compute SHA-256 from the verified bytes, then rewind before calling storage:

```csharp
await using var verifiedContent = new MemoryStream(capacity: checked((int)fileInput.FileSizeBytes));
await verifiedContent.WriteAsync(headerBuffer.AsMemory(0, bytesRead), cancellationToken);
await fileInput.Content.CopyToAsync(verifiedContent, cancellationToken);
if (verifiedContent.Length != fileInput.FileSizeBytes)
    return await RollbackAndFail(savedPaths, new Error("FILE_SIZE_MISMATCH", "Uploaded byte count does not match the declared size."), cancellationToken);

var contentSha256 = Convert.ToHexString(SHA256.HashData(verifiedContent.ToArray())).ToLowerInvariant();
verifiedContent.Position = 0;
```

Set `ScanStatus=content_verified` only for magic-number/content validation; reserve `clean` for a real malware scanner result. Image width/height remain nullable unless deterministically parsed from the verified header.

- [ ] **Step 4: Generate and review one additive migration**

Run: `dotnet ef migrations add HardenItemFileMetadata --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api`

Run: `dotnet ef migrations script --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api`

Expected: only additive columns, constraints, alternate key/indexes and same-organization FKs; no table drop, no public URL column and no rewrite of historical Estimate snapshots.

- [ ] **Step 5: Verify empty and upgrade migration paths**

Run focused integration tests once against a fresh Testcontainer and once by applying migrations from the migration immediately before `AddItemMasterCatalogFoundation` through the latest migration.

Expected: both database paths reach the same model and all constraint tests pass.

- [ ] **Step 6: Commit the schema checkpoint**

```bash
git add backend/src/TanErp.Domain/Files backend/src/TanErp.Application/Files backend/src/TanErp.Infrastructure/Persistence backend/tests/TanErp.IntegrationTests/Persistence
git commit -m "feat(items): harden catalog schema and file metadata"
```

---

### Task 2: Complete Item, Taxonomy, and Branch Availability Management API

**Files:**
- Create: `backend/src/TanErp.Application/Items/IItemStore.cs`
- Create: `backend/src/TanErp.Application/Items/ItemProjections.cs`
- Create: `backend/src/TanErp.Application/Items/Commands/ItemCommandHandlers.cs`
- Create: `backend/src/TanErp.Application/Items/Taxonomy/IItemTaxonomyStore.cs`
- Create: `backend/src/TanErp.Application/Items/Taxonomy/ItemTaxonomyHandlers.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/ItemStore.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/ItemTaxonomyStore.cs`
- Create: `backend/src/TanErp.Api/Contracts/Items/ItemContracts.cs`
- Create: `backend/src/TanErp.Api/Contracts/Items/ItemTaxonomyContracts.cs`
- Create: `backend/src/TanErp.Api/Controllers/ItemsController.cs`
- Create: `backend/src/TanErp.Api/Controllers/ItemTaxonomyController.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Api/ItemEndpointsTests.cs`

**Interfaces:**
- Consumes: Item domain behavior, `IRequestAccessResolver`, `AuditEvent`, `IClock`
- Produces: create/get/patch/activate/deactivate/branch-availability endpoints and Category/Brand/Unit/Alias endpoints

- [ ] **Step 1: Write failing API tests for scope, ETag, lifecycle and audit**

```csharp
[Fact]
public async Task Patch_with_stale_etag_returns_item_version_conflict()
{
    using var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/items/{ItemId}");
    request.Headers.TryAddWithoutValidation("If-Match", $"\"{Guid.NewGuid()}\"");
    request.Content = JsonContent.Create(ValidUpdateRequest());

    var response = await Client.SendAsync(request);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    await AssertProblemCodeAsync(response, "ITEM_VERSION_CONFLICT");
}
```

Cover permission denial, duplicate code, missing Thai name, immutable code after first activation, category cycle, selected branch from another organization returning 404, deactivation reason, alias uniqueness and exactly one audit event per successful mutation.

- [ ] **Step 2: Define the narrow Application contracts**

```csharp
public interface IItemStore
{
    Task<Result<ItemProjection>> CreateAsync(CreateItemData data, RequestAccessContext access, CancellationToken ct);
    Task<ItemProjection?> GetAsync(Guid organizationId, Guid itemId, CancellationToken ct);
    Task<Result<ItemProjection>> UpdateAsync(UpdateItemData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemProjection>> ChangeStatusAsync(ChangeItemStatusData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemProjection>> SetBranchAvailabilityAsync(SetItemBranchAvailabilityData data, RequestAccessContext access, CancellationToken ct);
}
```

`ItemProjection` must include structured Category, Brand, BaseUnit, Aliases, Capabilities and Branch Availability; do not expose only UUIDs that force frontend joins.

- [ ] **Step 3: Implement transaction-owned stores and handlers**

Resolve permissions `items.read/create/update/activate/deactivate/manage-taxonomy/manage-branches`. Store methods load organization-scoped entities, call domain behavior, add `AuditEvent` with branch/membership/request/version/reason context and save once inside the same transaction.

- [ ] **Step 4: Add controllers and stable Problem Details mappings**

Provide:

```text
POST   /api/v1/items
GET    /api/v1/items/{id}
PATCH  /api/v1/items/{id}
POST   /api/v1/items/{id}/activate
POST   /api/v1/items/{id}/deactivate
PUT    /api/v1/items/{id}/branch-availability
GET/POST/PATCH /api/v1/item-categories
GET/POST/PATCH /api/v1/item-brands
GET/POST/PATCH /api/v1/units
GET/POST/DELETE /api/v1/items/{id}/aliases
```

Reads and successful writes return quoted ETag. Map duplicate/concurrency/validation errors to `ITEM_CODE_CONFLICT`, `ITEM_ALIAS_CONFLICT`, `ITEM_VERSION_CONFLICT`, `ITEM_CATEGORY_CYCLE` and `ITEM_FIELD_REQUIRED`.

- [ ] **Step 5: Regenerate OpenAPI and run focused tests**

Run: `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemEndpoints|FullyQualifiedName~OpenApiContract"`

Expected: all tests pass and `contracts/openapi/tan-erp.v1.json` contains every route above.

- [ ] **Step 6: Commit the management API checkpoint**

```bash
git add backend/src/TanErp.Application/Items backend/src/TanErp.Infrastructure/Persistence/Items backend/src/TanErp.Api backend/tests/TanErp.IntegrationTests/Api contracts/openapi
git commit -m "feat(items): complete scoped item management api"
```

---

### Task 3: Complete Private Item Image Commands

**Files:**
- Create: `backend/src/TanErp.Application/Items/Images/IItemImageStore.cs`
- Create: `backend/src/TanErp.Application/Items/Images/ItemImageHandlers.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/ItemImageStore.cs`
- Create: `backend/src/TanErp.Api/Contracts/Items/ItemImageContracts.cs`
- Modify: `backend/src/TanErp.Api/Controllers/ItemsController.cs`
- Modify: `backend/src/TanErp.Infrastructure/Files/FileParentAccessResolver.cs`
- Modify: `backend/src/TanErp.Application/Files/CompleteUploadSession/CompleteUploadSessionHandler.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Api/ItemImageEndpointsTests.cs`

**Interfaces:**
- Consumes: `FileParentTypes.Item`, verified uploaded files and `items.read`/`items.manage-images`
- Produces: attach/update/reorder/set-primary/detach commands and private `fileId` projection

- [ ] **Step 1: Add failing file-parent and image-command tests**

```csharp
[Fact]
public async Task Attach_rejects_file_uploaded_for_a_different_parent()
{
    var response = await Client.PostAsJsonAsync($"/api/v1/items/{ItemId}/images", new
    {
        fileId = FileUploadedForOpportunity,
        role = "primary",
        isPrimary = true,
        displayOrder = 0,
        altText = new { th = "ภาพหลัก", en = "Primary image" }
    });

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
}
```

Also cover unverified/cross-organization file, wrong `parentType`/`parentId`, stale ETag, unauthorized content read, deterministic reorder, one active primary and detach preserving the binary for retention cleanup.

- [ ] **Step 2: Extend file access for Item without adding another uploader**

Add an Item branch to `FileParentAccessResolver`: `items.read` for content read and `items.manage-images` for upload/write; require an existing `parentId`, matching organization and active membership. Add `items.manage-images` to `ResolveAnyAsync` in upload completion.

- [ ] **Step 3: Implement atomic image store operations**

```csharp
public interface IItemImageStore
{
    Task<Result<ItemImageProjection>> AttachAsync(AttachItemImageData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<IReadOnlyList<ItemImageProjection>>> ReorderAsync(ReorderItemImagesData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemImageProjection>> SetPrimaryAsync(SetPrimaryItemImageData data, RequestAccessContext access, CancellationToken ct);
    Task<Result> DetachAsync(DetachItemImageData data, RequestAccessContext access, CancellationToken ct);
}
```

When setting primary, clear the old primary and set the new primary in one transaction; keep the partial unique index as the race guard. Every operation writes one audit event with row versions.

- [ ] **Step 4: Add routes and verify private response shape**

Provide `POST /images`, `PATCH /images/{imageId}`, `PUT /images/reorder`, `PUT /images/{imageId}/primary`, and `DELETE /images/{imageId}` under `/api/v1/items/{itemId}`. Responses include `fileId`, alt/caption and row version only—never storage path or persistent bearer URL.

- [ ] **Step 5: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~ItemImage"`

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemImageEndpoints"`

Commit: `feat(items): complete private item image workflow`

---

### Task 4: Complete Governed Versioned Cost Workflow

**Files:**
- Create: `backend/src/TanErp.Domain/Items/CostRecordReview.cs`
- Modify: `backend/src/TanErp.Domain/Items/CostRecord.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CostRecordReviewConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CostRecordConfiguration.cs`
- Create: `backend/src/TanErp.Application/Items/Costs/ICostRecordStore.cs`
- Create: `backend/src/TanErp.Application/Items/Costs/CostRecordHandlers.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/CostRecordStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Items/CostResolver.cs`
- Create: `backend/src/TanErp.Api/Contracts/Items/CostRecordContracts.cs`
- Create: `backend/src/TanErp.Api/Controllers/CostRecordsController.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_CompleteVersionedCostWorkflow.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Persistence/CostResolverTests.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Api/CostRecordEndpointsTests.cs`

**Interfaces:**
- Consumes: existing `CostRecord`, `CostSource`, `ICostResolver`
- Produces: Draft → Submitted → Approved/Returned → Published/Superseded/Disabled with append-only reviews

- [ ] **Step 1: Write failing lifecycle, resolver and overlap tests**

```csharp
[Fact]
public async Task Resolver_returns_ambiguous_when_two_candidates_have_equal_precedence()
{
    await SeedPublishedCostsAsync(sameBranch: true, sameEffectiveFrom: true, sameQuantityBand: true);

    var result = await Resolver.ResolveAsync(ValidRequest(), CancellationToken.None);

    Assert.True(result.IsFailure);
    Assert.Equal("ITEM_COST_AMBIGUOUS", result.Error.Code);
}
```

Cover maker/editor self-approval, only Published resolving, branch override over organization default, latest effective-from, quantity bands, overlap rejection, no cost, immutable Published rows, return reason and disable/supersede reason.

- [ ] **Step 2: Add append-only reviews and publish overlap guard**

`CostRecordReview` records action, reason, actor membership/user, timestamp and before/after row version. Publishing must transactionally reject another Published record with the same organization/item/unit/currency/scope/branch and overlapping effective/quantity ranges using a locked query or PostgreSQL exclusion constraint.

- [ ] **Step 3: Harden deterministic resolution**

Keep this precedence exact:

```text
1. branch scope before organization scope
2. greatest EffectiveFromUtc not after requested time
3. greatest MinimumQuantity containing requested quantity
4. exactly one winner; equal-precedence ties return ITEM_COST_AMBIGUOUS
```

Do not use record ID as a silent business tie-breaker.

- [ ] **Step 4: Implement command API with permission separation**

Provide create/update/submit/approve/return/publish/disable routes. Use `item-costs.create/submit/approve/publish/disable`, ETag, idempotency for create/publish and AuditEvent in the same transaction. A creator or last financial editor cannot approve their own record.

- [ ] **Step 5: Run focused tests, regenerate OpenAPI and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~CostRecord"`

Run: `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~CostResolver|FullyQualifiedName~CostRecordEndpoints|FullyQualifiedName~OpenApiContract"`

Commit: `feat(items): complete governed versioned cost workflow`

---

### Task 5: Harden the Estimate Catalog Read Model

**Files:**
- Modify: `backend/src/TanErp.Application/Items/Catalog/EstimateCatalogQuery.cs`
- Modify: `backend/src/TanErp.Application/Items/Catalog/EstimateCatalogProjection.cs`
- Modify: `backend/src/TanErp.Application/Items/Catalog/SearchEstimateCatalogHandler.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Items/EstimateCatalogReader.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Items/EstimateCatalogResponse.cs`
- Modify: `backend/src/TanErp.Api/Controllers/EstimateCatalogController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Api/EstimateCatalogEndpointsTests.cs`

**Interfaces:**
- Consumes: Active/canCost Item, Branch Availability, Published Cost and primary Item Image
- Produces: branch-scoped search/facets/keyset page with a deterministic resolved cost per item

- [ ] **Step 1: Add failing contract/security/query-count tests**

Test search by code, Thai/English localized name and alias; category/brand/item-type filters; invalid cursor; page size bounds; selected-branch dates; missing permission; cross-organization branch; unpublished/ambiguous/missing cost exclusion and primary private image projection.

- [ ] **Step 2: Validate the cursor in Application before querying**

Expose one strict cursor codec. Invalid Base64, invalid JSON, empty code or empty UUID returns `ITEM_CATALOG_CURSOR_INVALID` Problem Details instead of a successful empty page.

- [ ] **Step 3: Replace per-row cost/image queries with one bounded projection**

Build one EF query that selects Item, Category, Brand, Unit, winning cost and primary image. If EF cannot translate the deterministic winner without client evaluation, use one parameterized SQL read model and document the query plan. The number of SQL commands must remain constant as page size changes.

- [ ] **Step 4: Add server-side facets and indexed localized search**

Accept structured `categoryId`, `brandId`, `itemType`, `search`, `pageSize`, `cursor` and required `branchId`. Query JSONB Thai/English fields and aliases on the server; add expression/GIN indexes only when supported by an integration test and reviewed migration SQL.

- [ ] **Step 5: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateCatalogEndpoints|FullyQualifiedName~ItemPersistence"`

Commit: `perf(items): harden estimate catalog projection`

---

### Task 6: Close Estimate Snapshot and Price-Revalidation Regression Coverage

**Files:**
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`
- Modify: `backend/src/TanErp.Application/Estimates/UpdateEstimateDraft/UpdateEstimateDraftHandler.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Estimates/UpdateEstimateDraftRequest.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Estimates/EstimateDetailResponse.cs`
- Modify: `backend/tests/TanErp.UnitTests/Estimates/EstimateTests.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Api/ItemCatalogEstimateFlowTests.cs`

**Interfaces:**
- Consumes: `itemId`, `costRecordId`, quantity and server-side `ICostResolver`
- Produces: authoritative snapshot or stable `ITEM_COST_VERSION_CONFLICT`

- [ ] **Step 1: Add failing integration tests for tampering and historical immutability**

```csharp
[Fact]
public async Task Saving_catalog_component_ignores_client_price_and_freezes_server_snapshot()
{
    var response = await UpdateEstimateAsync(itemId: ItemId, costRecordId: PublishedCostId, clientUnitCost: 0.01m);
    response.EnsureSuccessStatusCode();

    var component = await ReadSavedComponentAsync(response);
    Assert.Equal(PublishedAmount, component.UnitCost);
    Assert.Equal(PublishedCostId, component.CostRecordId);
    Assert.Equal(ItemCode, component.ItemCodeSnapshot);
}
```

Also publish a newer cost after save and assert the existing Estimate snapshot/total remains unchanged; saving with a stale selected cost returns `ITEM_COST_VERSION_CONFLICT`; manual components with both IDs null remain supported.

- [ ] **Step 2: Make snapshot writes complete and immutable**

For catalog components populate item code/name, cost ID/version/scope/effective time/policy/resolved time and authoritative amount in one constructor call. Update paths may change quantity and re-resolve a new snapshot only while the Estimate Revision is Draft; quoted revisions remain immutable.

- [ ] **Step 3: Return the saved server projection to the frontend**

Ensure response contracts expose both nullable identities and snapshot fields, and the frontend replaces its optimistic row with the saved response rather than retaining the displayed Catalog price.

- [ ] **Step 4: Run Estimate regressions and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~Estimate"`

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemCatalogEstimateFlow|FullyQualifiedName~EstimateEndpoints"`

Commit: `feat(estimates): finalize immutable item cost snapshots`

---

### Task 7: Finish the Frontend Catalog, Private Thumbnails, and Tests

**Files:**
- Modify: `frontend/src/features/estimates/api/estimate-catalog-client.ts`
- Modify: `frontend/src/features/estimates/hooks/use-estimate-catalog.ts`
- Create: `frontend/src/features/estimates/hooks/use-private-item-image.ts`
- Modify: `frontend/src/features/estimates/components/estimate-item-catalog-modal.tsx`
- Modify: `frontend/src/features/estimates/components/estimate-cost-component-table.tsx`
- Modify: `frontend/src/features/estimates/components/estimate-workspace-drawer.tsx`
- Modify: `frontend/src/features/estimates/components/estimate-item-catalog-modal.test.tsx`
- Create: `frontend/src/features/estimates/api/estimate-catalog-client.test.ts`
- Delete: `frontend/src/features/estimates/constants/estimate-catalog-items.ts`
- Delete: `frontend/src/features/estimates/constants/estimate-catalog-items.test.ts`
- Delete if no remaining import: `frontend/src/features/estimates/hooks/useCatalogFilter.ts`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`
- Modify: `frontend/src/generated/api/tan-erp.v1.ts`

**Interfaces:**
- Consumes: generated Catalog/Estimate/File contracts
- Produces: server-backed modal with branch context, private image lifecycle and conflict recovery

- [ ] **Step 1: Regenerate types and add strict adapter tests**

Run: `cd frontend && npm run generate:api`

Test missing required IDs/structured fields as `EstimateCatalogContractError`; do not invent values or chain fallbacks. Map decimal amounts as strings until explicit numeric calculation boundaries.

- [ ] **Step 2: Rewrite modal tests around mocked TanStack Query state**

Cover loading, retryable error, empty result, Thai/English text, debounced server search, filters, cursor next/back, multi-selection across pages, selected count and disabled insert while submitting.

- [ ] **Step 3: Implement revocable private thumbnails**

`usePrivateItemImage(fileId)` fetches authorized `/api/v1/files/{fileId}/content`, creates an object URL, revokes it on replacement/unmount and returns typed loading/error state. Do not place access tokens or persistent signed URLs in `<img src>`.

- [ ] **Step 4: Handle authoritative save and price conflict**

On successful draft save replace local component values with response values. For `ITEM_COST_VERSION_CONFLICT`, invalidate Catalog query, show translated price-changed copy and require the user to reselect; never silently substitute a new price.

- [ ] **Step 5: Remove the production static catalog and verify i18n/accessibility**

Run: `rg -n "ESTIMATE_CATALOG_ITEMS|useCatalogFilter" frontend/src --glob '!**/*.test.*'`

Expected: no result. Ensure dialog focus trap/return focus, labeled search, semantic table headers, 44px controls, visible focus and translated loading/empty/error/image-unavailable/pagination/selected/price-changed messages in both locale files.

- [ ] **Step 6: Run focused frontend gates and commit**

Run: `cd frontend && npm test -- --run src/features/estimates/api/estimate-catalog-client.test.ts src/features/estimates/components/estimate-item-catalog-modal.test.tsx`

Run: `cd frontend && npm run lint && npm run typecheck && npm run build`

Commit: `feat(estimates): finish secure item catalog experience`

---

### Task 8: End-to-End Security, Documentation, and Release Evidence

**Files:**
- Create: `frontend/e2e/estimate-item-catalog.spec.ts`
- Create: `docs/05-engineering/item-master-estimate-catalog-verification.md`
- Modify: `docs/05-engineering/item-master-uat-scenarios.md`
- Modify: `docs/README.md`
- Modify only on verified contract divergence: `docs/03-contracts/item-master-api-contract.md`
- Modify only on verified schema divergence: `docs/04-data/item-master-data-contract.md`

**Interfaces:**
- Consumes: all Tasks 1–7
- Produces: reproducible security/UAT evidence and rollout approval gate

- [ ] **Step 1: Add one complete backend scenario**

Create Category hierarchy, Brand, Alias, Unit, selected-branch Item and verified primary image; create organization and branch costs; publish using a distinct checker; search by alias/filter; select branch price; save Estimate; publish a newer cost; assert historical snapshot unchanged.

- [ ] **Step 2: Add explicit security regression matrix**

Test cross-organization Item/Branch/File/Cost IDs, wrong file parent, missing permissions, maker self-approval, unverified file, stale ETag, reused idempotency key with changed payload, malformed cursor, oversized search, client-tampered price and unauthorized private image read.

- [ ] **Step 3: Add desktop and narrow viewport browser coverage**

At desktop and 320px viewport: open Estimate Catalog, search/filter, load private thumbnail or accessible placeholder, select item, insert, save, reload and assert snapshot values. Exercise keyboard-only operation, return focus after dialog close and inspect 200% zoom for clipped critical controls.

- [ ] **Step 4: Run every completion gate from a clean dependency install**

```bash
dotnet --version
dotnet build backend/TanErp.slnx
dotnet test backend/TanErp.slnx
cd frontend && npm ci
npm run verify
npm run test:e2e -- estimate-item-catalog.spec.ts
git diff --check
```

Expected: SDK reports `10.0.400` and every command exits 0. Record command, UTC timestamp, commit SHA, test counts and result; a skipped or failed command keeps the feature incomplete.

- [ ] **Step 5: Review migration, query and security evidence**

Record migration SQL review, Catalog query count/query plan, storage privacy check, tenant/branch isolation results, audit transaction evidence and the fact that no secret, binary, storage path or persistent signed URL appears in API/OpenAPI/frontend state.

- [ ] **Step 6: Document rollout and backout**

Roll out additive migrations before API/frontend and enable the Catalog with one organization-owned feature flag. Backout disables the flag and returns to manual Estimate components; never drop Item/Cost/Image tables or delete Estimate snapshots.

- [ ] **Step 7: Commit final evidence**

```bash
git add frontend/e2e docs/05-engineering docs/README.md
git commit -m "test(items): verify estimate catalog completion"
```

---

## Execution Order and Review Gates

1. **Gate A — Schema Safety:** Task 1 passes on fresh and upgraded PostgreSQL databases.
2. **Gate B — Governed Master Data:** Tasks 2–4 pass; all mutations are scoped, audited and concurrency-safe.
3. **Gate C — Authoritative Estimate Flow:** Tasks 5–7 pass; Catalog is server-backed and Estimate stores authoritative immutable snapshots.
4. **Gate D — Production Evidence:** Task 8 passes; full build/test/security/browser evidence is recorded.

Do not start the next gate while the previous gate has a failing test, unresolved migration issue or undocumented contract change.

## Definition of Done

- Item, Category, Brand, Unit, Alias and Branch Availability are maintainable through scoped APIs with ETag and audit.
- Item Image uses a Verified private file with exact parent invariant, same-organization FK, one-primary constraint and authorized content access.
- Versioned Cost has maker–checker, append-only reviews, deterministic resolution and overlap protection.
- Catalog search/facets/cursor run server-side without N+1 queries and return only Active, branch-available, costable Items with one Published cost.
- `estimate-item-catalog-modal.tsx` has no production dependency on static catalog and supports private thumbnail/loading/error/empty/pagination/conflict states in Thai and English.
- Estimate ignores client price, re-resolves on save and retains Item/Cost/calculation snapshot after master data changes.
- Cross-organization, cross-branch, wrong-file-parent, permission, stale-version and tampering tests pass.
- OpenAPI and generated TypeScript types match.
- `dotnet build`, `dotnet test`, `npm run verify`, focused Playwright and `git diff --check` pass and are recorded in the verification document.
