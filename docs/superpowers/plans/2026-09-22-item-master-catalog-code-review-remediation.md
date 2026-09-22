# Item Master Catalog Code Review Remediation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** แก้ข้อค้นพบจาก code review ของ Item Master Estimate Catalog ให้ migration deploy ได้จริง, ปิดช่องโหว่ file/tenant boundary, ทำให้ราคาและ Catalog deterministic และคืน frontend flow ที่ผ่าน contract, i18n และ verification ก่อนทำ Completion Plan ต่อ

**Architecture:** ใช้ File Service, Request Access และ Cost Resolver เดิมเป็น authority เพียงจุดเดียว เพิ่ม composite organization key ที่ฐานข้อมูลและ validate branch ก่อนอ่าน Catalog จากนั้นรวม Catalog เป็น bounded server projection และให้ Estimate เรียก resolver เดียวกัน Frontend ใช้ generated contracts, locale-aware projection และ private file content โดยไม่เชื่อราคา client

**Tech Stack:** .NET SDK 10.0.400, ASP.NET Core 10, EF Core 10, PostgreSQL/Npgsql, xUnit/Testcontainers, Next.js 16, React 19, TypeScript strict, TanStack Query, Vitest, Playwright, OpenAPI

## Global Constraints

- แผนนี้แก้ diff ของ working tree จาก merge-base `50cdf1771333ccfd742ac51bb40117ce876cc241`; รักษาการแก้ไขเดิมและห้าม reset/checkout ทับ
- แผนนี้เป็น prerequisite ของ `2026-09-22-item-master-estimate-catalog-completion.md`; หลัง remediation ผ่านจึงกลับไปทำ requirement ที่ยังเหลือใน Completion Plan
- ใช้ SDK `10.0.400` และ `backend/TanErp.slnx`; ห้าม downgrade runtime/package หรือ suppress `PendingModelChangesWarning`
- ทุก resource lookup ใช้ Organization/Branch จาก membership; ID นอก scope ตอบ 404
- ทุก organization-owned FK ใช้ composite `(resource_id, organization_id)` เมื่อ relation มี OrganizationId ทั้งสองฝั่ง
- `ICostResolver` เป็น single authority สำหรับ cost precedence; ห้ามเขียน query resolve ราคาแยกใน Estimate/Catalog
- Published Cost และ Estimate snapshot immutable; browser price เป็น display-only
- File upload ตรวจจำนวน byte จริงก่อนเขียน storage; `content_verified` ไม่เท่ากับ malware status `clean`
- UI copy ใหม่ต้องมี `th` และ `en`; rendering ใช้ locale ที่ร้องขอโดยไม่ fallback มั่ว
- ห้ามใช้ `any`, `as any`, `@ts-ignore`, frontend relation lookup ด้วย `array.find` หรือ hardcoded visible UI copy
- ทุก task ใช้ TDD และจบด้วย focused tests, `git diff --check` และ commit ที่ไม่รวม unrelated changes
- ห้าม claim complete ก่อน backend build/tests, frontend verify และ focused E2E ผ่านจริง

---

## Review Findings Covered

| Finding | Remediation task |
|---|---|
| Actual upload size bypass and missing SHA-256 | Task 1 |
| Pending model changes / missing migration | Task 2 |
| Loose Evidence File, Item Image and Estimate Cost FKs | Task 2 |
| JSONB constraints and Alias uniqueness mismatch | Task 2 |
| Unvalidated Catalog branch context | Task 3 |
| Wrong quantity-tier precedence and duplicated resolver | Task 3 |
| `other` versus `other_direct` contract collision | Task 3 |
| Invalid cursor returns 200; N+1; incomplete filters/facets/search | Task 4 |
| Missing Item/Image/Cost governed write APIs | Tasks 5–7 |
| Stale static modal tests, hardcoded Thai, private image/conflict UX gaps | Task 8 |
| Missing security/E2E/release evidence | Task 9 |

---

### Task 1: Close the Actual-Byte File Upload Bypass

**Files:**
- Modify: `backend/src/TanErp.Application/Files/CompleteUploadSession/CompleteUploadSessionHandler.cs`
- Modify: `backend/src/TanErp.Domain/Files/UploadedFile.cs`
- Create: `backend/tests/TanErp.UnitTests/Files/CompleteUploadSessionHandlerTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/FileUploadSessionTests.cs`

**Interfaces:**
- Consumes: `FileCompletionInput.Content`, declared slot metadata, 10 MB limit and `IFileStorageProvider`
- Produces: verified byte buffer and lowercase 64-character `ContentSha256`

- [ ] **Step 1: Write failing tests for actual length and hashing**

```csharp
[Fact]
public async Task Complete_rejects_content_larger_than_declared_slot_size()
{
    var declared = ValidPngBytes();
    var submitted = declared.Concat(new byte[1024]).ToArray();
    var command = CreateCommand(declaredSize: declared.Length, submitted);

    var result = await Handler.Handle(command, CancellationToken.None);

    Assert.True(result.IsFailure);
    Assert.Equal("FILE_SIZE_MISMATCH", result.Error.Code);
    Assert.Empty(Storage.SavedPaths);
}
```

Also test a stream exceeding 10 MB despite a small declaration, truncated content, deterministic SHA-256, magic-number mismatch and cancellation cleanup of already-saved paths.

- [ ] **Step 2: Run tests and confirm the security failure**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~CompleteUploadSessionHandler"`

Expected: actual-length cases fail because the current handler trusts `FileSizeBytes` and copies the remainder unbounded.

- [ ] **Step 3: Bound, verify and hash the actual stream before storage**

Read at most `MaxFileSizeBytes + 1` into a temporary stream, including the already-read header. Reject immediately when actual bytes exceed the limit or differ from the slot declaration. Only after validation compute SHA-256, reset position and call `SaveAsync`:

```csharp
await using var verified = new MemoryStream(capacity: checked((int)fileInput.FileSizeBytes));
await verified.WriteAsync(headerBuffer.AsMemory(0, bytesRead), cancellationToken);
await CopyWithLimitAsync(fileInput.Content, verified, MaxFileSizeBytes + 1, cancellationToken);

if (verified.Length > MaxFileSizeBytes)
    return await RollbackAndFail(savedPaths, new Error("FILE_TOO_LARGE", "The uploaded file exceeds 10 MB."), cancellationToken);
if (verified.Length != fileInput.FileSizeBytes)
    return await RollbackAndFail(savedPaths, new Error("FILE_SIZE_MISMATCH", "Uploaded byte count does not match the declared size."), cancellationToken);

var sha256 = Convert.ToHexString(SHA256.HashData(verified.ToArray())).ToLowerInvariant();
verified.Position = 0;
```

Pass `sha256`, `FileScanStatus.ContentVerified` and `VerifiedAtUtc` to `UploadedFile`; do not mark malware status `clean` without a scanner.

- [ ] **Step 4: Run focused security tests**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~CompleteUploadSessionHandler"`

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemFileUploadSecurity"`

Expected: oversize/mismatch writes zero binaries and valid uploads persist the expected hash.

- [ ] **Step 5: Commit**

```bash
git add backend/src/TanErp.Application/Files backend/src/TanErp.Domain/Files backend/tests/TanErp.UnitTests/Files backend/tests/TanErp.IntegrationTests/Api
git commit -m "fix(files): verify actual upload bytes before storage"
```

---

### Task 2: Restore Database Tenant Integrity and Migration Parity

**Files:**
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/UploadedFileConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemImageConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CostSourceConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/EstimateCostComponentConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/ItemAliasConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_HardenItemCatalogTenantAndJsonConstraints.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Persistence/ItemSchemaConstraintTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Persistence/ItemPersistenceTests.cs`

**Interfaces:**
- Consumes: organization-owned UploadedFile, CostRecord, Item and EstimateCostComponent
- Produces: migration/model parity and composite tenant-safe FKs

- [ ] **Step 1: Add failing DB constraint tests**

```csharp
[Fact]
public async Task Estimate_component_rejects_cost_record_from_another_organization()
{
    Db.EstimateCostComponents.Add(CreateCatalogComponent(OrgA, ItemA, CostRecordFromOrgB));
    await Assert.ThrowsAsync<DbUpdateException>(() => Db.SaveChangesAsync());
}
```

Cover cross-org Item Image file, Cost Source evidence file and Estimate Cost Record; malformed localized JSON, unknown locale key, missing Thai name on active Item, duplicate Thai/English aliases for the same Item, and the same alias reused by a different Item when the data contract permits it.

- [ ] **Step 2: Run the current persistence suite and preserve the pending-model failure**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemPersistence|FullyQualifiedName~ItemSchemaConstraint"`

Expected before migration: EF throws `PendingModelChangesWarning` or new constraint tests fail.

- [ ] **Step 3: Align mappings with tenant and JSON contracts**

Use alternate keys `(Id, OrganizationId)` on UploadedFile and CostRecord. Configure ItemImage, CostSource and EstimateCostComponent with composite FKs. Add PostgreSQL checks that localized fields are JSON objects, contain only `th`/`en`, and active Item `name->>'th'` is non-empty. Change alias uniqueness to `(organization_id, item_id, normalized_thai)` and a filtered English equivalent.

- [ ] **Step 4: Generate one additive remediation migration**

Run:

```bash
dotnet ef migrations add HardenItemCatalogTenantAndJsonConstraints --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
dotnet ef migrations script --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
```

Review that the migration adds `content_sha256`, dimensions, scan status and verified time; replaces loose FKs with composite FKs; adjusts alias indexes; adds JSON checks; and does not drop Item/Cost/Image/Estimate data.

- [ ] **Step 5: Verify both migration paths**

Run integration tests against a fresh Testcontainer, then against a database migrated to `AddEstimateItemCostSnapshots` before applying the remediation migration.

Expected: `Database.MigrateAsync()` succeeds, no pending model changes remain and all tenant/JSON cases pass.

- [ ] **Step 6: Commit**

```bash
git add backend/src/TanErp.Infrastructure/Persistence/Configurations backend/src/TanErp.Infrastructure/Persistence/Migrations backend/tests/TanErp.IntegrationTests/Persistence
git commit -m "fix(items): enforce tenant-safe catalog schema"
```

---

### Task 3: Make Branch Access and Cost Resolution Authoritative

**Files:**
- Modify: `backend/src/TanErp.Application/Items/Costs/ICostResolver.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Items/CostResolver.cs`
- Modify: `backend/src/TanErp.Application/Items/Catalog/SearchEstimateCatalogHandler.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`
- Create: `backend/src/TanErp.Application/Items/ItemTypeCostComponentMapper.cs`
- Modify: `frontend/src/features/estimates/api/estimate-catalog-client.ts`
- Modify: `frontend/src/features/estimates/types/estimate-catalog.ts`
- Create: `backend/tests/TanErp.IntegrationTests/Persistence/CostResolverTests.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Api/EstimateCatalogAccessTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`

**Interfaces:**
- Consumes: membership-derived organization, requested Branch, quantity and effective time
- Produces: validated `BranchAccessContext`, one deterministic `ResolvedCostProjection` and valid Estimate cost-component type

- [ ] **Step 1: Add failing branch, quantity-tier and Item type tests**

Test that cross-org/nonexistent branch returns 404; organization-scoped permission may use only a branch belonging to the organization; branch-scoped permission may use only its authorized branch; quantity 10 selects minimum quantity 10 over minimum quantity 1; equal full precedence returns `ITEM_COST_AMBIGUOUS`; Item type `other` maps to `other_direct` and all other types map explicitly.

- [ ] **Step 2: Validate Branch before invoking Catalog reader**

Introduce an Application abstraction backed by Infrastructure:

```csharp
public interface IItemBranchAccessReader
{
    Task<Result<Guid>> RequireReadableBranchAsync(
        RequestAccessContext access,
        Guid branchId,
        CancellationToken cancellationToken);
}
```

The result returns the validated Branch ID. Missing/cross-org/out-of-scope branches use `RESOURCE_NOT_FOUND` or `PERMISSION_DENIED` according to existing RBAC disclosure rules.

- [ ] **Step 3: Fix exact cost precedence**

Order by branch match, `EffectiveFromUtc`, then `MinimumQuantity`, all descending. Load enough candidates to detect equality across those three business keys. Remove ID as a winner; ID may be used only after a unique winner is already established for stable materialization.

- [ ] **Step 4: Reuse `ICostResolver` in Estimate draft updates**

Inject `ICostResolver` into `EstimateStore` or move catalog-component construction into an Application service that owns both transaction and resolver call. Compare the selected `costRecordId` with the resolved ID; mismatch returns `ITEM_COST_VERSION_CONFLICT`. Delete the duplicate precedence query from `EstimateStore`.

- [ ] **Step 5: Add explicit Item type mapping at the backend contract boundary**

```csharp
public static string ToCostComponentType(string itemType) => itemType switch
{
    ItemType.Material => CostComponentType.Material,
    ItemType.Labor => CostComponentType.Labor,
    ItemType.Service => CostComponentType.Service,
    ItemType.Subcontract => CostComponentType.Subcontract,
    ItemType.Other => CostComponentType.OtherDirect,
    _ => throw new ItemValidationException("Unsupported Item type.")
};
```

Return this typed value in the Catalog projection so the frontend does not duplicate domain mapping.

- [ ] **Step 6: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~CostResolver|FullyQualifiedName~EstimateCatalogAccess|FullyQualifiedName~EstimateEndpoints"`

Commit: `fix(items): centralize branch-scoped cost resolution`

---

### Task 4: Repair Catalog Contract, Search, Pagination, and Query Shape

**Files:**
- Modify: `backend/src/TanErp.Application/Items/Catalog/EstimateCatalogQuery.cs`
- Modify: `backend/src/TanErp.Application/Items/Catalog/EstimateCatalogProjection.cs`
- Modify: `backend/src/TanErp.Application/Items/Catalog/SearchEstimateCatalogHandler.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Items/EstimateCatalogReader.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Items/EstimateCatalogResponse.cs`
- Modify: `backend/src/TanErp.Api/Controllers/EstimateCatalogController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Modify: `docs/03-contracts/item-master-api-contract.md`
- Create: `backend/tests/TanErp.IntegrationTests/Api/EstimateCatalogEndpointsTests.cs`

**Interfaces:**
- Consumes: validated Branch and deterministic published cost resolver policy
- Produces: server-side filters/facets and stable keyset page with constant query count

- [ ] **Step 1: Add failing Catalog contract tests**

Cover malformed cursor returning `ITEM_CATALOG_CURSOR_INVALID`; search by code, Thai name, English name and active alias; category/brand/item type filters; only eligible priced items counting toward page size; correct `hasNextPage`; branch override; ambiguous/no-cost exclusion; facets derived from the filtered authorized set; and SQL command count independent of page size.

- [ ] **Step 2: Resolve list-contract naming before implementation**

Align the Item API contract with repository list standards using query `limit` and response `pagination`. Because the endpoint is not released, update backend, OpenAPI and frontend atomically rather than carrying dual parameter names. Preserve keyset fields `nextCursor` and `hasNextPage`.

- [ ] **Step 3: Move cursor decoding to a strict Application codec**

```csharp
public interface IEstimateCatalogCursorCodec
{
    Result<EstimateCatalogCursor> Decode(string value);
    string Encode(EstimateCatalogCursor value);
}
```

Reject invalid Base64/JSON/empty code/empty UUID before querying and map the error through localized RFC Problem Details.

- [ ] **Step 4: Build one bounded server projection**

Filter Item eligibility and winning Published cost before `Take(limit + 1)`. Project Category, Brand, Unit, cost and primary image in one EF query. If EF translation cannot keep query count constant, use one parameterized SQL read model and capture `EXPLAIN (ANALYZE, BUFFERS)` evidence in the verification document.

- [ ] **Step 5: Implement localized search and facets**

Search normalized code, `name->>'th'`, `name->>'en'`, normalized Thai alias and normalized English alias. Apply category/brand/item type/attributes before computing facets. Do not load masters into frontend for `find` joins.

- [ ] **Step 6: Run tests, regenerate OpenAPI and commit**

Run: `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateCatalogEndpoints|FullyQualifiedName~OpenApiContract"`

Commit: `fix(items): make catalog query deterministic and bounded`

---

### Task 5: Complete Item, Taxonomy, and Branch Availability Commands

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
- Create: `backend/tests/TanErp.IntegrationTests/Api/ItemEndpointsTests.cs`

**Interfaces:**
- Consumes: tenant-safe Item schema and Request Access
- Produces: ETag-protected Item/Taxonomy/Branch Availability APIs with atomic audit

- [ ] **Step 1: Write failing API tests**

Cover create/get/update, duplicate code, immutable activated code, activation gates, deactivation reason, stale ETag, permission denial, category cycle, Alias uniqueness, cross-org Category/Brand/Unit/Branch returning 404 and exactly one AuditEvent per successful mutation.

- [ ] **Step 2: Define and implement the store contract**

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

Each store method owns one EF transaction, calls Domain behavior and writes AuditEvent before one `SaveChangesAsync`.

- [ ] **Step 3: Add typed contracts and routes**

Provide create/get/patch/activate/deactivate/branch-availability plus Category/Brand/Unit/Alias routes. Return structured relations and quoted ETag; map duplicate/cycle/version errors to stable Problem Details.

- [ ] **Step 4: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemEndpoints"`

Commit: `feat(items): add audited item management commands`

---

### Task 6: Complete Private Item Image Commands

**Files:**
- Create: `backend/src/TanErp.Application/Items/Images/IItemImageStore.cs`
- Create: `backend/src/TanErp.Application/Items/Images/ItemImageHandlers.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/ItemImageStore.cs`
- Create: `backend/src/TanErp.Api/Contracts/Items/ItemImageContracts.cs`
- Modify: `backend/src/TanErp.Api/Controllers/ItemsController.cs`
- Modify: `backend/src/TanErp.Infrastructure/Files/FileParentAccessResolver.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Api/ItemImageEndpointsTests.cs`

**Interfaces:**
- Consumes: `FileParentTypes.Item`, Verified File and `items.manage-images`
- Produces: attach/reorder/set-primary/detach APIs without public file URLs

- [ ] **Step 1: Write failing image security tests**

Test wrong organization, wrong parent type/ID, unverified file, stale ETag, unauthorized content, deterministic reorder, one primary and detach preserving binary retention.

- [ ] **Step 2: Implement exact Item parent access**

Require an existing organization-scoped Item. Use `items.read` for file content and `items.manage-images` for upload/write. Revalidate parent access during upload completion.

- [ ] **Step 3: Implement atomic image operations**

```csharp
public interface IItemImageStore
{
    Task<Result<ItemImageProjection>> AttachAsync(AttachItemImageData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<IReadOnlyList<ItemImageProjection>>> ReorderAsync(ReorderItemImagesData data, RequestAccessContext access, CancellationToken ct);
    Task<Result<ItemImageProjection>> SetPrimaryAsync(SetPrimaryItemImageData data, RequestAccessContext access, CancellationToken ct);
    Task<Result> DetachAsync(DetachItemImageData data, RequestAccessContext access, CancellationToken ct);
}
```

Clear old primary and set new primary in one transaction; write one AuditEvent per command.

- [ ] **Step 4: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ItemImageEndpoints|FullyQualifiedName~FileUploadSession"`

Commit: `feat(items): add verified private item images`

---

### Task 7: Complete Governed Cost Commands and Reviews

**Files:**
- Create: `backend/src/TanErp.Domain/Items/CostRecordReview.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CostRecordReviewConfiguration.cs`
- Create: `backend/src/TanErp.Application/Items/Costs/ICostRecordStore.cs`
- Create: `backend/src/TanErp.Application/Items/Costs/CostRecordHandlers.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Items/CostRecordStore.cs`
- Create: `backend/src/TanErp.Api/Contracts/Items/CostRecordContracts.cs`
- Create: `backend/src/TanErp.Api/Controllers/CostRecordsController.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Api/CostRecordEndpointsTests.cs`

**Interfaces:**
- Consumes: corrected `ICostResolver` and tenant-safe Cost schema
- Produces: Draft → Submitted → Approved/Returned → Published/Superseded/Disabled commands

- [ ] **Step 1: Write failing governance tests**

Cover creator/editor self-approval, append-only review reason, stale ETag, idempotent create/publish, overlapping Published ranges, disable reason, immutable Published fields, permissions and atomic AuditEvent.

- [ ] **Step 2: Implement append-only review and locked publish guard**

`CostRecordReview` stores action, reason, actor user/membership, occurred time and row versions. Publish locks matching organization/item/unit/currency/scope/branch candidates and rejects effective/quantity overlap before state change.

- [ ] **Step 3: Add command endpoints**

Expose create/update/submit/approve/return/publish/disable with `item-costs.create/submit/approve/publish/disable`, ETag, idempotency and localized Problem Details.

- [ ] **Step 4: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~CostRecordEndpoints|FullyQualifiedName~CostResolver"`

Commit: `feat(items): add governed cost commands and reviews`

---

### Task 8: Repair Frontend Contract, Locale, Images, Conflict UX, and Tests

**Files:**
- Modify: `frontend/src/features/estimates/api/estimate-catalog-client.ts`
- Modify: `frontend/src/features/estimates/hooks/use-estimate-catalog.ts`
- Create: `frontend/src/features/estimates/hooks/use-private-item-image.ts`
- Modify: `frontend/src/features/estimates/components/estimate-item-catalog-modal.tsx`
- Modify: `frontend/src/features/estimates/components/estimate-cost-component-table.tsx`
- Modify: `frontend/src/features/estimates/components/estimate-workspace-drawer.tsx`
- Rewrite: `frontend/src/features/estimates/components/estimate-item-catalog-modal.test.tsx`
- Create: `frontend/src/features/estimates/api/estimate-catalog-client.test.ts`
- Delete: `frontend/src/features/estimates/constants/estimate-catalog-items.ts`
- Delete: `frontend/src/features/estimates/constants/estimate-catalog-items.test.ts`
- Delete if unused: `frontend/src/features/estimates/hooks/useCatalogFilter.ts`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`
- Modify: `frontend/src/generated/api/tan-erp.v1.ts`

**Interfaces:**
- Consumes: regenerated Catalog response with typed component type, locale data, pagination and private `fileId`
- Produces: locale-aware, private-image-safe, server-state modal and authoritative save recovery

- [ ] **Step 1: Regenerate types and write strict adapter tests**

Run: `cd frontend && npm run generate:api`

Test required fields, typed component type, decimal strings, facets and pagination. Missing contract fields throw `EstimateCatalogContractError`; do not invent fallback values.

- [ ] **Step 2: Replace stale static tests with mocked server-state tests**

Mock `useEstimateCatalog`; supply required `branchId`; test pending/error/retry/empty/results, search, filters, next/back cursor, multi-page selection, confirm-disabled state and selected count. Remove assertions for supplier/static drawer behavior outside this slice.

- [ ] **Step 3: Render localized text from the active locale**

Introduce one helper that returns Thai for `th`, English for `en` when present, otherwise the explicit empty-state marker from i18n. Apply it to Item, Category, Brand, alt text and descriptions; do not hardcode `.th` in UI rendering.

- [ ] **Step 4: Implement revocable private thumbnails**

Fetch authorized file content with the existing API client, create an object URL and revoke it on file change/unmount. Render localized accessible alt text or translated placeholder; never store a bearer/signed URL.

- [ ] **Step 5: Handle authoritative price conflict and stable ordering**

On `ITEM_COST_VERSION_CONFLICT`, invalidate Catalog query and require re-selection. After save, replace local values with server response. For multi-insert assign `sortOrder = fields.length + selectedIndex + 1`; use resolved cost currency and typed component type from the contract.

- [ ] **Step 6: Apply modal design/accessibility standards**

Use `size="md"` for footer actions, preserve focus trap/return focus, 44px controls, semantic table headers and keyboard selection. Add every visible message to both locale files.

- [ ] **Step 7: Run frontend gates and commit**

Run:

```bash
cd frontend
npm test -- --run src/features/estimates/api/estimate-catalog-client.test.ts src/features/estimates/components/estimate-item-catalog-modal.test.tsx
npm run lint
npm run typecheck
npm run build
```

Commit: `fix(estimates): harden item catalog frontend flow`

---

### Task 9: Prove the Remediation and Resume the Completion Plan

**Files:**
- Create: `backend/tests/TanErp.IntegrationTests/Api/ItemCatalogEstimateFlowTests.cs`
- Create: `frontend/e2e/estimate-item-catalog.spec.ts`
- Create: `docs/05-engineering/item-master-estimate-catalog-verification.md`
- Modify: `docs/05-engineering/item-master-uat-scenarios.md`
- Modify: `docs/README.md`
- Modify: `docs/superpowers/plans/2026-09-22-item-master-estimate-catalog-completion.md`

**Interfaces:**
- Consumes: Tasks 1–8
- Produces: reproducible evidence that all review findings are closed

- [x] **Step 1: Add one end-to-end backend security scenario**

Create taxonomy, selected-branch Item, item-parent Verified image and organization/branch costs; publish with a distinct checker; query Catalog; tamper client unit cost; save Estimate; publish a newer cost; assert authoritative historical snapshot remains unchanged.

- [x] **Step 2: Add the review regression matrix**

Record automated cases for actual-byte overflow, truncated upload, cross-org Branch/File/Cost IDs, wrong file parent, invalid cursor, equal-precedence cost ambiguity, quantity tier, Item type `other`, stale ETag, maker self-approval and unauthorized private content.

- [x] **Step 3: Run browser coverage**

At desktop and 320px viewport test Thai/English rendering, server search/filter, private image/placeholder, multi-select ordering, save/reload, price conflict recovery, keyboard focus and 200% zoom.

- [x] **Step 4: Run the full completion gates**

```bash
dotnet --version
dotnet build backend/TanErp.slnx
dotnet test backend/TanErp.slnx
cd frontend && npm ci
npm run verify
npm run test:e2e -- estimate-item-catalog.spec.ts
cd ..
git diff --check
```

Expected: SDK `10.0.400`, zero pending model changes and every command exits 0. Record UTC timestamp, commit SHA, test counts and exact result.

- [x] **Step 5: Update plan status from evidence**

Mark remediation checkboxes only where evidence exists. In the Completion Plan, mark corresponding requirements complete and leave unrelated remaining tasks unchecked. Link the verification document from `docs/README.md`.

- [x] **Step 6: Commit**

```bash
git add backend/tests/TanErp.IntegrationTests/Api frontend/e2e docs
git commit -m "test(items): verify catalog review remediation"
```

---

## Delivery Gates

1. **Security Hotfix Gate:** Task 1 passes; storage never receives unverified-length content.
2. **Schema Gate:** Task 2 passes on fresh and upgrade databases with no pending model changes.
3. **Authority Gate:** Tasks 3–4 pass; Branch and Cost behavior has one server authority and Catalog is bounded.
4. **Workflow Gate:** Tasks 5–8 pass; write surfaces and frontend behavior satisfy contracts.
5. **Evidence Gate:** Task 9 passes; every review finding is mapped to automated evidence.

## Definition of Done

- Actual uploaded bytes, declared slot bytes and 10 MB limit are all enforced before storage; SHA-256 is persisted.
- All Item/File/Cost/Estimate relations prove same-organization scope at database level.
- Fresh and upgraded databases migrate without `PendingModelChangesWarning`.
- Branch ID is validated against organization and permission scope before Catalog read.
- One `ICostResolver` implements branch/effective/minimum-quantity precedence and ambiguity behavior.
- Catalog rejects invalid cursors, has constant query count, paginates eligible items and supports required localized search/filters/facets.
- Item type `other` saves as Estimate type `other_direct` through an explicit typed mapping.
- Item/Image/Cost mutations have permissions, ETag, audit, file-parent invariants and maker–checker controls.
- Frontend renders active locale, uses private thumbnails, handles price conflicts explicitly and has no production static Catalog dependency.
- Backend build/tests, frontend verify, focused E2E and diff check pass with recorded evidence.
