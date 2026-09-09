# Opportunity + Site Vertical Slice Remediation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ปิด findings จาก code review ของ Customer Activation + Opportunity + Site Vertical Slice ให้ idempotency ปลอดภัยเมื่อ retry พร้อมกัน, UI ตรง contract, ไม่มี explicit `any`, acceptance coverage ครบ และ verification record สะท้อนผลรันจริงเท่านั้น

**Architecture:** แก้ correctness ก่อน evidence: Frontend ผูก activation retry กับ stable intent และแสดง Branch/Owner จาก shared selected-membership context; Application รวม SHA-256 hashing ไว้จุดเดียว; Infrastructure ใช้ `IClock` เดิมและกู้ผลของ idempotency winner หลัง PostgreSQL unique race. จากนั้นเพิ่ม deterministic concurrency tests, browser/API acceptance assertions และเขียน verification record ใหม่จากคำสั่งที่รันจริงหลัง remediation เท่านั้น

**Tech Stack:** .NET SDK `10.0.400`, ASP.NET Core/EF Core `10.0.11`, Npgsql EF Core `10.0.3`, PostgreSQL `17-alpine`, Next.js `16.3.4`, React `19.2.8`, TypeScript `7.0.2`, TanStack Query `5.102.8`, Vitest `5.0.0`, Playwright `1.63.0`

## Global Constraints

- Fixed point ของการแก้คือ commit `808ddde`; แก้เฉพาะ findings ในเอกสารนี้และ regression ที่เกิดจากการแก้
- ขอบเขตยังเป็น Customer Draft → Active, Create/List Site และ Create/List/Get Opportunity Draft; lifecycle หรือ CRUD อื่นยัง deferred
- รักษา HTTP contract เดิม: stale activation = `409 CUSTOMER_VERSION_CONFLICT`, missing/invalid `If-Match` = `428 IF_MATCH_REQUIRED`
- `POST activate`, `POST site` และ `POST opportunity` ต้องใช้ key เดิมกับ intent/payload เดิม; concurrent retry ต้องคืน resource เดิม ไม่คืน 500 และไม่สร้าง audit/resource ซ้ำ
- Resource, idempotency record และ audit event ต้อง commit atomic ด้วย EF Core transaction; ไม่มี raw SQL write และไม่มี Generic Repository
- ใช้ `IClock` ที่มีอยู่แล้วเป็นเวลาเดียวของ use case; production ใช้ `SystemClock`, tests ใช้ fixed clock
- Frontend ใช้ generated OpenAPI types, `unknown` + narrowing และ typed test fixtures; ไม่มี `any`, `as any` หรือ `@ts-ignore`
- UI text ใช้ i18n และเพิ่ม `th`/`en` เป็นคู่เสมอ; Thai เป็นค่าเริ่มต้น
- ใช้ TDD: เพิ่ม test ที่ fail ด้วยเหตุผลของ finding, ยืนยัน RED, แก้ขั้นต่ำ, ยืนยัน GREEN แล้ว commit หนึ่งครั้งต่อ Task
- Verification status เป็น Passed ได้เมื่อ command, runtime, commit SHA, test count และ exit code มาจากรอบหลัง remediation จริงเท่านั้น
- **Global reuse proposal:** เพิ่ม `Sha256Hex.Compute(string): string` ใน Application Common และขยาย `SelectedMembershipContext` ให้คืน `currentUser`; ทั้งสองจุดมี consumer ซ้ำชัดเจนและไม่สร้าง generic CRM abstraction

---

## Planned File Map

### Shared/Application

- Create `backend/src/TanErp.Application/Common/Security/Sha256Hex.cs` — deterministic lowercase SHA-256 hex helper
- Create `backend/tests/TanErp.UnitTests/Common/Security/Sha256HexTests.cs` — canonical hash tests
- Modify three create/activate handlers — consume the shared hash helper
- Modify `frontend/src/lib/membership/selected-membership-context.tsx` — expose generated `CurrentUserResponse` alongside selected membership
- Modify its test — prove current user remains stable when membership switches

### Backend Persistence

- Modify `CustomerLifecycleStore.cs`, `SiteStore.cs`, `OpportunityStore.cs` — inject `IClock`
- Modify `SiteStore.cs`, `OpportunityStore.cs` — recover the committed winner after PostgreSQL `23505`
- Modify Site/Opportunity persistence tests — fixed timestamps and deterministic concurrent retry coverage

### Frontend

- Modify Customer detail + test — stable activation intent and correct `409` handling
- Modify Opportunity editor + test — read-only Branch and Owner context
- Modify affected Customer/Site/Opportunity tests — remove explicit `any`
- Modify `Icons.tsx` — remove trailing blank line

### Acceptance and Evidence

- Modify `frontend/e2e/opportunity-site.spec.ts` — complete required happy/negative/UX assertions
- Modify `docs/05-engineering/opportunity-site-verification.md` — replace premature pass claims with post-remediation evidence

---

### Task 1: Make Customer Activation a Stable Retry Intent

**Files:**
- Modify: `frontend/src/features/customers/components/customer-detail.tsx:24-103`
- Modify: `frontend/src/features/customers/components/customer-detail.test.tsx`

**Interfaces:**
- Consumes: `CustomerResponse.id`, `CustomerResponse.rowVersion`, `ApiError.code`
- Produces: one idempotency key per `customerId|rowVersion|activate` intent and localized handling for `409 CUSTOMER_VERSION_CONFLICT`

- [ ] **Step 1: Replace untyped Customer test fixtures before adding behavior**

Import `CustomerResponse` and `MembershipDto`, then type mutable fixtures without widening:

```ts
import type { CustomerResponse } from "@/lib/api/api-client";
import type { MembershipDto } from "@/lib/permissions/can";

const mockMembership = {
  id: "membership-a",
  permissions: [{ key: "customers.activate", scope: "organization" }],
} satisfies MembershipDto;

let currentMembership: MembershipDto | null = mockMembership;
let currentCustomerData: CustomerResponse = organizationDraftCustomer;
```

- [ ] **Step 2: Write failing activation retry and conflict tests**

Mock `crypto.randomUUID()` as `activation-key-1`, reject the first call with a network `Error`, submit again without changing `customer.rowVersion`, and assert both calls carry the same `idempotencyKey`. Then update the fixture to a new `rowVersion`, rerender, submit, and assert a new key is used.

Replace the current 412 test with:

```ts
mockActivateCustomer.mockRejectedValueOnce(new ApiError({
  status: 409,
  code: "CUSTOMER_VERSION_CONFLICT",
  message: "Customer row version conflict.",
}));
```

Assert `customers.errors.activateConflict` appears and a generic API title does not.

- [ ] **Step 3: Run focused tests and confirm RED**

```bash
npm --prefix frontend run test -- src/features/customers/components/customer-detail.test.tsx
```

Expected: retry test sees two different timestamp keys; conflict test does not select the localized branch.

- [ ] **Step 4: Implement stable intent and exact error narrowing**

Use a ref keyed by the canonical activation payload:

```ts
interface ActivationIntent {
  signature: string;
  key: string;
}

const activationIntentRef = React.useRef<ActivationIntent | null>(null);

const signature = `${customer.id}|${customer.rowVersion ?? ""}|activate`;
if (activationIntentRef.current?.signature !== signature) {
  activationIntentRef.current = { signature, key: crypto.randomUUID() };
}
```

Send `activationIntentRef.current.key`; retain it after failure, and clear it only after a successful response. Narrow conflict with both status and code:

```ts
if (
  err instanceof ApiError &&
  err.status === 409 &&
  err.code === "CUSTOMER_VERSION_CONFLICT"
) {
  toast.error(t("errors.activateConflict"));
}
```

- [ ] **Step 5: Run tests and commit**

```bash
npm --prefix frontend run test -- src/features/customers/components/customer-detail.test.tsx
npm --prefix frontend run typecheck
git diff --check
```

Expected: tests pass; same intent reuses a key, changed row version rotates it, and 409 renders localized conflict.

```bash
git add frontend/src/features/customers/components/customer-detail.tsx frontend/src/features/customers/components/customer-detail.test.tsx
git commit -m "fix(customers): preserve activation retry intent"
```

---

### Task 2: Show Server-Derived Owner Context in the Opportunity Form

**Files:**
- Modify: `frontend/src/lib/membership/selected-membership-context.tsx`
- Modify: `frontend/src/lib/membership/selected-membership-context.test.tsx`
- Modify: `frontend/src/features/opportunities/components/opportunity-editor.tsx:30-214`
- Modify: `frontend/src/features/opportunities/components/opportunity-editor.test.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`

**Interfaces:**
- Consumes: generated `CurrentUserResponse.user`, selected Membership Branch
- Produces: `useSelectedMembership()` result containing `currentUser` and a read-only Branch/Owner context block

- [ ] **Step 1: Write failing context and editor tests**

Update `Probe` to read `currentUser.user` and assert the display name/email remain unchanged after switching Membership A → B. In the editor test, provide:

```ts
const currentUser = {
  user: {
    id: "10000000-0000-0000-0000-000000000010",
    displayName: "คุณเจ้าของโอกาส",
    email: "owner@example.test",
  },
  memberships: [mockMembershipWithBranch],
} satisfies CurrentUserResponse;
```

Assert the form shows both `สาขา: สาขาใหญ่ (กรุงเทพ)` and `ผู้รับผิดชอบ: คุณเจ้าของโอกาส` as non-editable text.

- [ ] **Step 2: Run focused tests and confirm RED**

```bash
npm --prefix frontend run test -- src/lib/membership/selected-membership-context.test.tsx src/features/opportunities/components/opportunity-editor.test.tsx
```

Expected: context has no `currentUser`; Owner label is absent.

- [ ] **Step 3: Extend the existing shared context**

```ts
interface SelectedMembershipContextType {
  currentUser: CurrentUserResponse;
  selectedMembership: MembershipDto | null;
  memberships: MembershipDto[];
  setSelectedMembershipId: (id: string) => Promise<void>;
}
```

Pass the same generated response supplied to `SelectedMembershipProvider`. Consume it in the editor:

```ts
const { currentUser, selectedMembership } = useSelectedMembership();
const ownerLabel = currentUser.user?.displayName || currentUser.user?.email || "-";
```

Render a semantic read-only context block (`<dl><dt><dd>`) containing translated `branchLabel` and `ownerLabel`. Do not add Branch/Owner to the POST payload.

- [ ] **Step 4: Keep translations paired**

Reuse the existing `opportunities.branchLabel` and `opportunities.ownerLabel` keys. If accessible helper copy is added, create the same key path in both `th.json` and `en.json` and assert parity in the existing localization test.

- [ ] **Step 5: Run tests and commit**

```bash
npm --prefix frontend run test -- src/lib/membership/selected-membership-context.test.tsx src/features/opportunities/components/opportunity-editor.test.tsx
npm --prefix frontend run typecheck
npm --prefix frontend run lint
```

Expected: Branch/Owner render from context and create payload remains unchanged.

```bash
git add frontend/src/lib/membership frontend/src/features/opportunities/components/opportunity-editor.tsx frontend/src/features/opportunities/components/opportunity-editor.test.tsx frontend/src/messages
git commit -m "fix(opportunities): show derived owner context"
```

---

### Task 3: Centralize Deterministic SHA-256 Hashing

**Files:**
- Create: `backend/src/TanErp.Application/Common/Security/Sha256Hex.cs`
- Create: `backend/tests/TanErp.UnitTests/Common/Security/Sha256HexTests.cs`
- Modify: `backend/src/TanErp.Application/Crm/Customers/ActivateCustomer/ActivateCustomerHandler.cs`
- Modify: `backend/src/TanErp.Application/Crm/Sites/CreateSite/CreateSiteHandler.cs`
- Modify: `backend/src/TanErp.Application/Crm/Opportunities/CreateOpportunity/CreateOpportunityHandler.cs`

**Interfaces:**
- Consumes: UTF-8 string
- Produces: `Sha256Hex.Compute(string value): string` returning 64 lowercase hexadecimal characters

- [ ] **Step 1: Write the failing shared hash tests**

```csharp
[Theory]
[InlineData("", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")]
[InlineData("abc", "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
public void Compute_ReturnsLowercaseSha256(string value, string expected)
{
    Assert.Equal(expected, Sha256Hex.Compute(value));
}
```

- [ ] **Step 2: Run test and confirm RED**

```bash
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter FullyQualifiedName~Sha256HexTests
```

Expected: compile fails because `Sha256Hex` does not exist.

- [ ] **Step 3: Implement one shared helper and replace all private copies**

```csharp
using System.Security.Cryptography;
using System.Text;

namespace TanErp.Application.Common.Security;

public static class Sha256Hex
{
    public static string Compute(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
```

Replace `ComputeSha256`/`ComputeSha256Hex` calls with `Sha256Hex.Compute` and remove handler-local cryptography imports/methods. Do not move payload canonicalization into the helper.

- [ ] **Step 4: Run handler and helper tests, then commit**

```bash
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~Sha256HexTests|FullyQualifiedName~ActivateCustomerHandlerTests|FullyQualifiedName~SiteHandlerTests|FullyQualifiedName~OpportunityHandlerTests"
git diff --check
```

Expected: hashes and existing handler payload hashes remain identical.

```bash
git add backend/src/TanErp.Application/Common/Security backend/src/TanErp.Application/Crm backend/tests/TanErp.UnitTests/Common/Security
git commit -m "refactor(application): centralize idempotency hashing"
```

---

### Task 4: Make Persistence Time Deterministic

**Files:**
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Crm/CustomerLifecycleStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Crm/SiteStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Persistence/SiteStoreTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Persistence/OpportunityStoreTests.cs`

**Interfaces:**
- Consumes: existing `IClock.UtcNow`
- Produces: deterministic timestamps for resource creation, idempotency and audit records

- [ ] **Step 1: Add fixed-clock assertions that fail against wall-clock time**

Use a local typed clock in each relevant test class:

```csharp
private sealed class FixedClock : IClock
{
    public DateTimeOffset UtcNow { get; } =
        DateTimeOffset.Parse("2026-09-09T04:30:00Z");
}
```

Construct `SiteStore` and `OpportunityStore` with the fixed clock. In `CustomerEndpointsTests`, replace the production registration inside `ConfigureServices`:

```csharp
services.RemoveAll<IClock>();
services.AddSingleton<IClock>(new FixedClock());
```

Assert created resource `CreatedAtUtc`, `IdempotencyRecord.CreatedAtUtc` and `AuditEvent.OccurredAtUtc` equal the fixed value.

- [ ] **Step 2: Run focused persistence/API tests and confirm RED**

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~SiteStoreTests|FullyQualifiedName~OpportunityStoreTests|FullyQualifiedName~CustomerEndpointsTests"
```

Expected: constructors do not accept `IClock`, or timestamps differ from the fixed value.

- [ ] **Step 3: Inject and use `IClock`**

Apply this constructor shape to all three stores:

```csharp
private readonly AppDbContext _db;
private readonly IClock _clock;

public SiteStore(AppDbContext db, IClock clock)
{
    _db = db;
    _clock = clock;
}
```

Use `_clock.UtcNow` once per mutation and reuse that value for resource, idempotency and audit timestamps. `Program.cs` already registers `IClock`; do not add a second registration.

- [ ] **Step 4: Run tests and commit**

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~SiteStoreTests|FullyQualifiedName~OpportunityStoreTests|FullyQualifiedName~CustomerEndpointsTests"
dotnet build backend/TanErp.slnx
```

Expected: all asserted timestamps equal the fixed clock and production DI resolves all stores.

```bash
git add backend/src/TanErp.Infrastructure/Persistence/Crm backend/tests/TanErp.IntegrationTests
git commit -m "refactor(persistence): use controlled crm clock"
```

---

### Task 5: Recover Site and Opportunity Concurrent Idempotency Winners

**Files:**
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Crm/SiteStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Persistence/SiteStoreTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Persistence/OpportunityStoreTests.cs`

**Interfaces:**
- Consumes: PostgreSQL unique violation `SqlState == "23505"`, `(organizationId, operation, keyHash)` idempotency identity
- Produces: both concurrent callers receive the same projection; exactly one resource, idempotency row and audit event persist

- [ ] **Step 1: Add a deterministic concurrent SaveChanges coordinator for tests**

Inside each persistence test file, create two independent `AppDbContext` instances over the same Testcontainer database. Attach one shared `SaveChangesInterceptor` containing a two-participant `Barrier`; enable the barrier only after fixture seeding so both mutation calls reach their first `SaveChangesAsync` before either continues.

```csharp
private sealed class CoordinatedSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly Barrier _barrier = new(2);
    public bool Enabled { get; set; }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (Enabled)
        {
            await Task.Run(() => _barrier.SignalAndWait(cancellationToken), cancellationToken);
        }
        return result;
    }
}
```

- [ ] **Step 2: Write RED concurrent replay tests**

For Site and Opportunity, issue `Task.WhenAll` through two stores using the same command, `keyHash` and `payloadHash`. Assert:

```csharp
Assert.All(results, result => Assert.True(result.IsSuccess));
Assert.Single(results.Select(result => result.Value!.Id).Distinct());
Assert.Equal(1, await verificationDb.IdempotencyRecords.CountAsync(
    row => row.OrganizationId == orgId && row.Operation == operation && row.KeyHash == keyHash));
Assert.Equal(1, await verificationDb.AuditEvents.CountAsync(
    row => row.OrganizationId == orgId && row.Action == expectedAuditAction));
```

Also assert exactly one Site/Opportunity with the returned ID. Add a separate concurrent same-key/different-payload case: one succeeds, the other returns `IDEMPOTENCY_KEY_REUSED`; neither path returns 500.

- [ ] **Step 3: Run focused tests and confirm RED**

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~Create_Concurrent|FullyQualifiedName~Create_ConcurrentDifferentPayload"
```

Expected: one caller throws `DbUpdateException` from PostgreSQL unique constraint.

- [ ] **Step 4: Catch only the idempotency unique race and reload the winner**

Extract the initial replay lookup in `SiteStore` into this typed method, and call it both before validation and after a unique race:

```csharp
private async Task<Result<SiteProjection>?> TryLoadReplayAsync(
    Guid organizationId,
    string operation,
    string keyHash,
    string payloadHash,
    CancellationToken cancellationToken)
{
    var record = await _db.IdempotencyRecords
        .AsNoTracking()
        .SingleOrDefaultAsync(row =>
            row.OrganizationId == organizationId &&
            row.Operation == operation &&
            row.KeyHash == keyHash,
            cancellationToken);

    if (record is null) return null;
    if (record.PayloadHash != payloadHash)
        return Result<SiteProjection>.Failure(new Error(
            "IDEMPOTENCY_KEY_REUSED",
            "The idempotency key has already been used with a different payload."));

    if (!Guid.TryParse(record.ResourceId, out var siteId)) return null;
    var site = await _db.Sites.AsNoTracking().SingleOrDefaultAsync(
        candidate => candidate.Id == siteId && candidate.OrganizationId == organizationId,
        cancellationToken);
    return site is null ? null : Result<SiteProjection>.Success(ToProjection(site));
}
```

Add this separately typed method to `OpportunityStore`:

```csharp
private async Task<Result<OpportunityProjection>?> TryLoadReplayAsync(
    Guid organizationId,
    string operation,
    string keyHash,
    string payloadHash,
    CancellationToken cancellationToken)
{
    var record = await _db.IdempotencyRecords
        .AsNoTracking()
        .SingleOrDefaultAsync(row =>
            row.OrganizationId == organizationId &&
            row.Operation == operation &&
            row.KeyHash == keyHash,
            cancellationToken);

    if (record is null) return null;
    if (record.PayloadHash != payloadHash)
        return Result<OpportunityProjection>.Failure(new Error(
            "IDEMPOTENCY_KEY_REUSED",
            "The idempotency key has already been used with a different payload."));

    if (!Guid.TryParse(record.ResourceId, out var opportunityId)) return null;
    var opportunity = await _db.Opportunities.AsNoTracking().SingleOrDefaultAsync(
        candidate => candidate.Id == opportunityId && candidate.OrganizationId == organizationId,
        cancellationToken);
    return opportunity is null
        ? null
        : Result<OpportunityProjection>.Success(ToProjection(opportunity));
}
```

Then wrap each store's save/commit:

```csharp
try
{
    await _db.SaveChangesAsync(cancellationToken);
    await tx.CommitAsync(cancellationToken);
}
catch (DbUpdateException ex) when (
    ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
{
    await tx.RollbackAsync(cancellationToken);
    _db.ChangeTracker.Clear();
    var replay = await TryLoadReplayAsync(
        orgId, operation, keyHash, payloadHash, cancellationToken);
    if (replay is not null) return replay;
    throw;
}
```

The Site catch returns `Result<SiteProjection>` and the Opportunity catch returns `Result<OpportunityProjection>` through their respective typed helper. Absence of an idempotency winner rethrows the original exception so unrelated unique violations remain visible.

- [ ] **Step 5: Run persistence and endpoint regression tests, then commit**

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~SiteStoreTests|FullyQualifiedName~OpportunityStoreTests|FullyQualifiedName~OpportunitySiteEndpointsTests"
git diff --check
```

Expected: concurrent callers converge on one resource; sequential replay and different-payload conflict remain unchanged.

```bash
git add backend/src/TanErp.Infrastructure/Persistence/Crm/SiteStore.cs backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs backend/tests/TanErp.IntegrationTests/Persistence
git commit -m "fix(crm): recover concurrent idempotency winners"
```

---

### Task 6: Remove Explicit `any` and Make the Policy Scan Precise

**Files:**
- Modify: `frontend/src/features/customers/components/customer-detail.test.tsx`
- Modify: `frontend/src/features/sites/components/site-editor.test.tsx`
- Modify: `frontend/src/features/sites/components/site-list.test.tsx`
- Modify: `frontend/src/features/sites/schemas/site-form-schema.test.ts`
- Modify: `frontend/src/features/opportunities/components/opportunity-editor.test.tsx`
- Modify: `frontend/src/features/opportunities/components/opportunity-list.test.tsx`
- Modify: `frontend/src/features/opportunities/components/opportunity-detail.test.tsx`
- Modify: `frontend/src/components/common/Icons.tsx`

**Interfaces:**
- Consumes: generated DTOs and actual hook return types
- Produces: tests with no explicit `any`; a policy command that ignores valid HTML `step="any"` and `expect.any(...)`

- [ ] **Step 1: Replace mutable fixture `any` with generated types**

Use `MembershipDto | null`, `CustomerResponse`, `SiteResponse`, `OpportunityResponse` and `CurrentUserResponse`. Type deferred promises precisely:

```ts
let resolvePromise: ((value: SiteResponse) => void) | undefined;
const pending = new Promise<SiteResponse>((resolve) => {
  resolvePromise = resolve;
});
```

For mocked hooks, use their real return type rather than `any`:

```ts
const loadingResult = {
  data: undefined,
  isLoading: true,
  isError: false,
  error: null,
  refetch: vi.fn(),
  fetchNextPage: vi.fn(),
  hasNextPage: false,
  isFetchingNextPage: false,
} as ReturnType<typeof useOpportunityList>;
```

Use `satisfies OpportunityResponse`/`satisfies SiteListResponse` for API fixtures. A narrow assertion to the real hook return type is allowed; `as any` and double-casting through `any` are not.

- [ ] **Step 2: Type the schema translator**

Replace `mockT as any` with the exact schema factory translator parameter:

```ts
const mockT: Parameters<typeof createSiteFormSchema>[0] = (key) => key;
const schema = createSiteFormSchema(mockT);
```

- [ ] **Step 3: Remove the whitespace failure**

Delete the extra blank line after the closing brace in `Icons.tsx` so the file ends with exactly one newline.

- [ ] **Step 4: Run precise scans and tests**

```bash
rg -n '(:\s*any\b|\bas\s+any\b|@ts-ignore)' frontend/src frontend/e2e --glob '*.{ts,tsx}'
npm --prefix frontend run test
npm --prefix frontend run typecheck
git diff --check 808ddde..HEAD
```

Expected: `rg` returns exit `1` with no matches; 70+ test files pass; typecheck and diff check exit `0`. Do not use the old broad `\bany\b` scan because it reports valid `step="any"`, `expect.any(File)` and English comments.

- [ ] **Step 5: Commit typed cleanup**

```bash
git add frontend/src/features frontend/src/components/common/Icons.tsx
git commit -m "test(frontend): remove explicit any from crm tests"
```

Do not update the verification status in this commit; Task 8 owns final evidence.

---

### Task 7: Complete Acceptance, Security and UX Coverage

**Files:**
- Modify: `frontend/e2e/opportunity-site.spec.ts`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs`
- Create: `backend/tests/TanErp.IntegrationTests/Support/InMemoryLoggerProvider.cs`

**Interfaces:**
- Consumes: deterministic Org A/Org B/no-Branch fixtures and the complete UI journey
- Produces: executable evidence for retry, derived context, tenant isolation, stale ETag, privacy, keyboard, 320px and 200% zoom

- [ ] **Step 1: Strengthen the happy-path E2E request assertions**

Change `signIn` to return the authenticated API context captured from the real `/api/v1/me` exchange:

```ts
interface SignedInContext {
  apiOrigin: string;
  authorization: string;
  membershipId: string;
  branchId: string | null;
  userId: string;
}

async function signIn(page: Page, email = "foundation-user@example.test"):
  Promise<SignedInContext> {
  await page.goto("/th/login");
  await page.getByLabel("อีเมล").fill(email);
  await page.locator("input#password").fill("TestPassword123!");
  const meRequest = page.waitForRequest(
    (request) => request.url().includes("/api/v1/me") && request.method() === "GET",
  );
  const meResponse = page.waitForResponse(
    (response) => response.url().includes("/api/v1/me") && response.status() === 200,
  );
  await page.getByRole("button", { name: "เข้าสู่ระบบ" }).click();
  const [request, response] = await Promise.all([meRequest, meResponse]);
  const body = await response.json() as CurrentUserResponse;
  const membership = body.memberships?.[0];
  if (!body.user?.id || !membership?.id) throw new Error("Incomplete test identity fixture");
  return {
    apiOrigin: new URL(response.url()).origin,
    authorization: request.headers()["authorization"] ?? "",
    membershipId: membership.id,
    branchId: membership.branch?.id ?? null,
    userId: body.user.id,
  };
}
```

Import `CurrentUserResponse` from the generated API client and assert the captured authorization header is non-empty before using it for direct negative API requests.

For activate/Site/Opportunity request listeners, inspect `request.headers()` and assert:

```ts
expect(headers["x-membership-id"]).toMatch(UUID_PATTERN);
expect(headers["idempotency-key"]?.length).toBeGreaterThanOrEqual(16);
expect(headers["if-match"]).toBe(`"${draftCustomer.rowVersion}"`);
```

On Opportunity response, assert `branchId` and `ownerUserId` equal the signed-in `/api/v1/me` selected membership/user, while the observed POST body has none of `organizationId`, `branchId`, `ownerUserId`, `code`, `stage` or `rowVersion`.

- [ ] **Step 2: Prove double-submit and retry behavior in the browser**

Count matching POST requests. Double-click the submit action while the first response is pending and assert exactly one request. For retry, route the first request and abort it after recording `Idempotency-Key`; submit the unchanged form again and assert the second request carries the same key. Change one payload field, submit again, and assert the key rotates.

- [ ] **Step 3: Add browser negative and accessibility journeys**

Use the signed-in context for direct requests so API negatives exercise the same Firebase session as the UI:

```ts
async function postApi(
  page: Page,
  context: SignedInContext,
  path: string,
  body: unknown,
  extraHeaders: Record<string, string> = {},
) {
  return page.request.post(`${context.apiOrigin}${path}`, {
    data: body,
    headers: {
      authorization: context.authorization,
      "x-membership-id": context.membershipId,
      "idempotency-key": crypto.randomUUID(),
      ...extraHeaders,
    },
  });
}
```

Read the response as `ProblemDetails`, then assert both HTTP status and stable `code`.

Add named tests for:

- Draft Customer: Site/Opportunity create CTA unavailable and direct Site API returns `409 CUSTOMER_INVALID_STATE`.
- Org B: Org A Customer/Site IDs return `404 RESOURCE_NOT_FOUND` without exposing which ID exists.
- Membership without Branch: form shows the localized block before POST; direct Opportunity API returns `422 ACTIVE_BRANCH_REQUIRED`.
- Stale ETag: activation returns `409 CUSTOMER_VERSION_CONFLICT` and UI shows localized conflict copy.
- Keyboard: tab order reaches open modal → cancel → confirm, focus is trapped in the dialog, Escape closes, and focus returns to the trigger.
- Responsive: run the create journey at viewport `320x800`; then set `document.documentElement.style.zoom = "200%"` and assert no horizontal document overflow and all submit controls remain reachable.

Use deterministic test-only IDs/emails from the Test seeder; do not embed tokens, passwords beyond the existing emulator fixture, or production data.

- [ ] **Step 4: Close privacy assertions at the API boundary**

Existing endpoint tests already cover several authorization cases; add only missing assertions. Create a Site with unique sentinel strings in address/access note, then inspect Problem Details and matching `AuditEvent.ChangesJson`:

```csharp
Assert.DoesNotContain(addressSentinel, problemJson, StringComparison.Ordinal);
Assert.DoesNotContain(accessNoteSentinel, problemJson, StringComparison.Ordinal);
Assert.DoesNotContain(addressSentinel, audit.ChangesJson, StringComparison.Ordinal);
Assert.DoesNotContain(accessNoteSentinel, audit.ChangesJson, StringComparison.Ordinal);
Assert.Contains("addressLine1", audit.ChangesJson, StringComparison.Ordinal);
Assert.Contains("accessNote", audit.ChangesJson, StringComparison.Ordinal);
```

Create a minimal in-memory logger provider and register the same singleton instance through `builder.ConfigureLogging(logging => logging.AddProvider(logProvider))`:

```csharp
public sealed class InMemoryLoggerProvider : ILoggerProvider
{
    public ConcurrentQueue<string> Messages { get; } = new();
    public ILogger CreateLogger(string categoryName) => new Sink(Messages);
    public void Dispose() { }

    private sealed class Sink(ConcurrentQueue<string> messages) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter) =>
            messages.Enqueue(formatter(state, exception));
    }
}
```

After the request completes, join `Messages` and assert both sentinels are absent. The assertions cover rendered log text; audit assertions continue to require field names but no values.

- [ ] **Step 5: Run focused gates and confirm all journeys execute**

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~CustomerEndpointsTests|FullyQualifiedName~OpportunitySiteEndpointsTests"
PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e -- opportunity-site.spec.ts
```

Expected: no skipped tests; every named negative/UX case executes against PostgreSQL 17, Firebase emulator, Backend `:5005` and Frontend `:3005`.

- [ ] **Step 6: Commit acceptance coverage**

```bash
git add frontend/e2e/opportunity-site.spec.ts backend/tests/TanErp.IntegrationTests/Api backend/tests/TanErp.IntegrationTests/Support
git commit -m "test(crm): complete opportunity site acceptance coverage"
```

---

### Task 8: Re-run All Gates and Publish Honest Evidence

**Files:**
- Modify: `docs/05-engineering/opportunity-site-verification.md`

**Interfaces:**
- Consumes: completed remediation commits and full verification stack
- Produces: auditable post-remediation status with no unsupported pass claim

- [ ] **Step 1: Capture the exact tested revision and runtimes**

```bash
git rev-parse HEAD
dotnet --info
node --version
npm --version
docker compose ps
```

Record only emitted values. Required runtime is .NET SDK `10.0.400`; stop and keep status `Blocked` if the target SDK/runtime or required services are unavailable.

- [ ] **Step 2: Run all repository gates**

```bash
npm run verify
PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e
rg -n '(:\s*any\b|\bas\s+any\b|@ts-ignore)' frontend/src frontend/e2e --glob '*.{ts,tsx}'
git diff --check 808ddde..HEAD
```

Expected: `npm run verify` and Playwright exit `0`; policy scan exits `1` with no matches; diff check exits `0`.

- [ ] **Step 3: Reconcile the verification document with raw output**

Replace prior counts with the new exact suite/file/test counts. Include:

- tested commit SHA and current branch;
- actual .NET, Node, PostgreSQL, Firebase emulator, Next.js and Playwright versions;
- each command and exit code;
- pass/fail/skip counts;
- mapping from UAT requirements to exact test names/files;
- explicit evidence for concurrent replay, stale ETag, Branch/Owner derivation, tenant isolation, no-Branch, privacy, keyboard, 320px and 200% zoom;
- remaining deferred scope from the original plan.

Set status `Passed` only if every required command and assertion passed in this run. Otherwise set `Blocked` or `Failed`, list the exact failed command, and retain no “100%” wording.

- [ ] **Step 4: Check documentation and policy consistency**

```bash
rg -n "Passed Quality Gate 100%|ไม่พบ Type any|Playwright Test Count|Tested Base Commit SHA" docs/05-engineering/opportunity-site-verification.md
git diff --check -- docs
```

Expected: no stale pre-remediation claim remains; evidence names the post-remediation SHA and actual counts.

- [ ] **Step 5: Commit evidence**

```bash
git add docs/05-engineering/opportunity-site-verification.md
git commit -m "docs(crm): publish remediation verification evidence"
```

---

## Definition of Success

1. Activation retry ที่ payload เดิมใช้ key เดิม; row version ใหม่สร้าง intent/key ใหม่; `409 CUSTOMER_VERSION_CONFLICT` แสดงข้อความ localized
2. Site/Opportunity concurrent retry คืน resource เดียวกันทั้งสอง caller พร้อม resource/idempotency/audit อย่างละหนึ่งรายการ
3. Store ทั้งสามใช้ `IClock`; deterministic tests ยืนยันเวลาของ resource/idempotency/audit
4. Handler ทั้งสามใช้ `Sha256Hex.Compute`; ไม่มี SHA-256 implementation ซ้ำใน CRM handlers
5. Opportunity create form แสดง selected Branch และ current User/Owner แบบ read-only โดย payload ยังไม่มี server-derived fields
6. ไม่มี `any`, `as any`, `@ts-ignore` ใน `frontend/src` หรือ `frontend/e2e`; TypeScript, Vitest, build และ diff check ผ่าน
7. Acceptance tests ยืนยัน headers, one POST on double-click, retry key, derived Branch/Owner, inactive Customer, Org isolation, no-Branch, stale ETag และ privacy
8. Keyboard, 320px และ zoom 200% ถูกทดสอบจริงโดยไม่มี skipped test
9. Backend build/test, Frontend verify, fixture tests และ real E2E ผ่านบน runtime ที่กำหนด
10. Verification record ใช้ผลหลัง remediation และไม่มีข้อความ Passed หากหลักฐานไม่ครบ

## Explicitly Deferred

```text
Customer update/deactivate and additional contacts
Site edit/deactivate, geocoding and Customer Address resource
Opportunity edit, qualify, stage transition/history, close and reopen
Owner reassignment and Branch/Own permission scope
Survey appointment, Site Survey identity/revision and evidence upload
Official Estimate, Project, Procurement, Inventory, Production and MRP
```
