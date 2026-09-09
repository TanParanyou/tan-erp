# Customer Activation + Opportunity + Site Vertical Slice Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ให้ผู้ใช้ที่มีสิทธิ์ Activate Customer Draft แล้วสร้าง Site ที่พร้อมใช้งานและ Opportunity Draft ซึ่งผูก Customer, Branch, Owner และ Primary Site ใน Organization เดียวกันได้ครบ UI → API → PostgreSQL → Audit

**Architecture:** ขยาย CRM ด้วยสาม mutation ที่ต่อกันเป็น tracer journey เดียว: idempotent Customer activation, idempotent Site creation และ idempotent Opportunity creation โดย Backend derive Organization, Branch และ Owner จาก Active Membership และตรวจ cross-resource scope ใหม่ทุก request. Site และ Opportunity เป็น aggregate แยกจาก Customer; EF Core ถือครอง writes/transactions ส่วน Frontend ใช้ generated OpenAPI types, central `ApiClient`, TanStack Query และหน้า form แบบ Single Dynamic Route `[id]`.

**Tech Stack:** .NET SDK `10.0.400`, ASP.NET Core/EF Core `10.0.11`, Npgsql EF Core `10.0.3`, PostgreSQL `17-alpine`, Next.js `16.3.4`, React `19.2.8`, TypeScript `7.0.2`, TanStack Query `5.102.8`, Zod `4.5.4`, React Hook Form `7.87.0`, Vitest `5.0.0`, Playwright `1.63.0`

## Global Constraints

- เริ่ม implementation หลังผู้ใช้อนุมัติแผนนี้และ Task 1 ล็อก exact contract แล้วเท่านั้น
- ก่อนเริ่ม Task 2 ต้องบันทึกผล Playwright 7/7 ของ Customer + Contact ลง `docs/05-engineering/customer-contact-verification.md`; ห้ามอ้าง external result โดยไม่มี command, commit SHA และผลจริง
- Slice นี้รวม Customer Draft → Active, Create/List Site, Create/List/Get Opportunity Draft เท่านั้น; ไม่รวม Customer edit/deactivate, Site edit/deactivate, Opportunity qualify/stage transition, Survey, Address resource, additional Contact, Estimate หรือ Project
- Customer activation เป็น prerequisite ที่ตัดออกไม่ได้ เพราะ Accepted Flow ห้ามสร้าง Opportunity จาก Customer ที่ไม่ Active
- Backend derive `OrganizationId` จาก `X-Membership-Id`; Slice นี้ derive `BranchId` จาก selected Membership และ `OwnerUserId` จาก authenticated User ห้ามรับสาม field นี้จาก Create Opportunity payload
- Selected Membership ต้องมี Active Branch เพื่อสร้าง Opportunity; Membership ที่ไม่มี Branch คืน `ACTIVE_BRANCH_REQUIRED` และไม่ fallback ไป Branch อื่น
- Permission scope ที่รองรับยังเป็น `organization` เท่านั้น; `branch`, `project` และ `own` fail-closed จนมี visibility contract เฉพาะ
- Permissions: activate ใช้ `customers.activate`; read/create Site ใช้ `sites.read`/`sites.manage`; read/create Opportunity ใช้ `opportunities.read`/`opportunities.create`
- `POST activate`, `POST site` และ `POST opportunity` ต้องมี `Idempotency-Key` 16–128 characters; key เดิม + payload เดิมคืน resource เดิม, payload ต่างคืน `409 IDEMPOTENCY_KEY_REUSED`
- Customer activation ต้องมี `If-Match: "<rowVersion>"`; header หาย/ผิดรูปแบบคืน `428 IF_MATCH_REQUIRED`, version เก่าคืน `409 CUSTOMER_VERSION_CONFLICT`
- Customer activation เขียน `customer.activated`; Site creation เขียน `site.created`; Opportunity creationเขียน `opportunity.created`; resource + idempotency + audit ต้อง commit atomic
- Resource ต่าง Organization หรือ Customer/Site relationship ไม่ตรงกันคืน `404 RESOURCE_NOT_FOUND` โดยไม่เปิดเผยว่า ID ใดมีอยู่จริง
- Site address/location/access note เป็น Personal Data: ไม่อยู่ใน structured log, Problem Details, idempotency payload text หรือ audit changes; audit เก็บเฉพาะ changed field names/IDs
- Site ใน Slice นี้สร้างเป็น `active` เพราะ create form บังคับ field gate A ครบ: label, structured address และ active Customer; Draft Site/edit flow เลื่อนไป slice ภายหลัง
- Opportunity สร้างเป็น `draft`; `scopeSummary`, `workTypes`, Primary Site และ next action เก็บได้ แต่การตรวจ Q gate และการสร้าง stage history เริ่มใน Opportunity Lifecycle slice ถัดไป
- Customer code ใช้ของเดิม; Site code เป็น `SITE-` + 12 hex ตัวแรกของ UUID uppercase; Opportunity code เป็น `OPP-` + 12 hex ตัวแรก
- UI text ใหม่ทั้งหมดต้องมี key parity ใน `frontend/src/messages/th.json` และ `en.json`; Backend errors อยู่ใน `.resx` ไทย/อังกฤษ
- ห้ามใช้ `any`, `as any`, `@ts-ignore`, handwritten frontend API DTO, component `fetch`, Generic Repository, role-name check หรือ raw SQL write
- Reuse first: ใช้ `Button`, `Input`, `MonoSpinner`, selected membership, permission helper, business query-key prefix และ shared `ApiClient` เดิม
- Global reuse proposal ที่อนุมัติพร้อมแผนนี้: เพิ่ม `ConfirmationModal` ใน `frontend/src/components/ui/` สำหรับ lifecycle action; ยังไม่สร้าง generic CRM form/table abstraction จนมี use case ซ้ำจริง
- Form ใช้ React Hook Form + Zod + `Controller`, Single Dynamic Route `[id]`, double-submit protection, dirty-state warning, semantic controls, `aria-live="polite"`, 44px targets, no skeleton และ Atelier Architectural Navy Sharp
- ทุก Task ใช้ TDD และจบด้วย commit `type(scope): description`; ห้ามรวม formatting/refactor ที่ไม่เกี่ยวข้อง

---

## Acceptance Journey

```text
Firebase Login
  → เลือก Active Membership ที่มี Active Branch
  → เปิด Customer Draft และ Activate ด้วย If-Match + Idempotency-Key
  → Customer กลายเป็น Active พร้อม ETag ใหม่
  → สร้าง Active Site ใต้ Customer เดิม
  → สร้าง Opportunity Draft โดย Backend derive Branch + Owner
  → เปิด Opportunity detail และค้นเจอใน list
  → PostgreSQL/Audit ยืนยัน Organization และ cross-resource scope
```

Journey ต้องพิสูจน์เพิ่มว่า inactive Customer สร้าง Site/Opportunity ไม่ได้, Org B ใช้ Customer/Site ของ Org A ไม่ได้, Membership ไม่มี Branch สร้าง Opportunity ไม่ได้, retry ไม่สร้างซ้ำ และ Site address ไม่รั่วใน log/error/audit

## Exact Slice Contract

### Activate Customer

```http
POST /api/v1/customers/{customerId}/activate
X-Membership-Id: <membership UUID>
Idempotency-Key: <opaque 16-128 characters>
If-Match: "<customer rowVersion UUID>"
```

Request ไม่มี body. Response `200` ใช้ `CustomerResponse` เดิมและส่ง ETag ใหม่

### Create Site

```http
POST /api/v1/customers/{customerId}/sites
X-Membership-Id: <membership UUID>
Idempotency-Key: <opaque 16-128 characters>
Content-Type: application/json
```

```json
{
  "label": "คอนโดสุขุมวิท TEST_ONLY",
  "addressLine1": "99/9 ถนนสุขุมวิท TEST_ONLY",
  "subdistrict": "คลองตันเหนือ",
  "district": "วัฒนา",
  "province": "กรุงเทพมหานคร",
  "postalCode": "10110",
  "countryCode": "TH",
  "latitude": null,
  "longitude": null,
  "accessNote": "ติดต่อเจ้าหน้าที่ก่อนขึ้นอาคาร TEST_ONLY"
}
```

Response `201`:

```json
{
  "id": "019a3cf8-96f0-7c9f-b207-93aa818f4c10",
  "customerId": "019a3cf8-96f0-7c9f-b207-93aa818f4b10",
  "code": "SITE-019A3CF896F0",
  "label": "คอนโดสุขุมวิท TEST_ONLY",
  "addressLine1": "99/9 ถนนสุขุมวิท TEST_ONLY",
  "subdistrict": "คลองตันเหนือ",
  "district": "วัฒนา",
  "province": "กรุงเทพมหานคร",
  "postalCode": "10110",
  "countryCode": "TH",
  "latitude": null,
  "longitude": null,
  "accessNote": "ติดต่อเจ้าหน้าที่ก่อนขึ้นอาคาร TEST_ONLY",
  "status": "active",
  "rowVersion": "019a3cf8-96f0-7c9f-b207-93aa818f4c11",
  "createdAtUtc": "2026-09-08T12:00:00Z"
}
```

`GET /api/v1/customers/{customerId}/sites` คืน `{ "items": SiteResponse[] }` เรียง `normalizedLabel ASC, id ASC`; ไม่มี pagination ใน slice นี้

### Create and Read Opportunity

```http
POST /api/v1/opportunities
X-Membership-Id: <membership UUID>
Idempotency-Key: <opaque 16-128 characters>
Content-Type: application/json
```

```json
{
  "customerId": "019a3cf8-96f0-7c9f-b207-93aa818f4b10",
  "primarySiteId": "019a3cf8-96f0-7c9f-b207-93aa818f4c10",
  "title": "Built-in ห้องนอนใหญ่ TEST_ONLY",
  "scopeSummary": "สำรวจและประเมินตู้เสื้อผ้า TEST_ONLY",
  "workTypes": ["built-in"],
  "sourceCode": null,
  "expectedBudget": 250000.00,
  "currencyCode": "THB",
  "targetDecisionDate": "2026-10-15",
  "nextActionAtUtc": "2026-09-10T03:00:00Z",
  "nextActionNote": "นัดยืนยันเวลา TEST_ONLY"
}
```

Request ห้ามมี `organizationId`, `branchId`, `ownerUserId`, `code`, `stage`, `rowVersion` หรือ actor ID. Response `201`:

```json
{
  "id": "019a3cf8-96f0-7c9f-b207-93aa818f4d10",
  "code": "OPP-019A3CF896F0",
  "customerId": "019a3cf8-96f0-7c9f-b207-93aa818f4b10",
  "primarySiteId": "019a3cf8-96f0-7c9f-b207-93aa818f4c10",
  "branchId": "019a3cf8-96f0-7c9f-b207-93aa818f4a13",
  "ownerUserId": "019a3cf8-96f0-7c9f-b207-93aa818f4a10",
  "title": "Built-in ห้องนอนใหญ่ TEST_ONLY",
  "scopeSummary": "สำรวจและประเมินตู้เสื้อผ้า TEST_ONLY",
  "workTypes": ["built-in"],
  "sourceCode": null,
  "expectedBudget": 250000.00,
  "currencyCode": "THB",
  "targetDecisionDate": "2026-10-15",
  "nextActionAtUtc": "2026-09-10T03:00:00Z",
  "nextActionNote": "นัดยืนยันเวลา TEST_ONLY",
  "stage": "draft",
  "rowVersion": "019a3cf8-96f0-7c9f-b207-93aa818f4d11",
  "createdAtUtc": "2026-09-08T12:00:00Z"
}
```

`GET /api/v1/opportunities/{id}` คืน shape เดียวกันพร้อม ETag. `GET /api/v1/opportunities?search=&customerId=&stage=draft&limit=25&cursor=` คืน `{ items, nextCursor }`; allowed limit `1..100`, stable sort `nextActionAtUtc ASC NULLS LAST, id ASC`

## Planned File Map

### Documentation

- Modify `AGENTS.md` — authorize the approved Customer Activation + Opportunity + Site implementation boundary
- Modify `docs/03-contracts/crm-site-survey-api-contract.md` — exact Slice 2 HTTP shapes, derived context and deferred boundary
- Modify `docs/03-contracts/error-contract.md` — `IF_MATCH_REQUIRED`, `ACTIVE_BRANCH_REQUIRED`, `SITE_FIELD_REQUIRED`
- Modify `docs/05-engineering/crm-site-survey-uat-scenarios.md` — executable Slice 2 subset
- Modify `docs/05-engineering/customer-contact-verification.md` — record external E2E evidence before gate transition
- Create `docs/05-engineering/opportunity-site-verification.md` — final evidence
- Modify `docs/README.md` — link plan and verification record

### Backend

- Modify `backend/src/TanErp.Domain/Crm/Customers/Customer.cs` — activation transition and version guard
- Modify `backend/src/TanErp.Domain/Crm/Customers/CustomerValues.cs` — activation outcome
- Create `backend/src/TanErp.Domain/Crm/Sites/Site.cs`, `SiteValues.cs`
- Create `backend/src/TanErp.Domain/Crm/Opportunities/Opportunity.cs`, `OpportunityValues.cs`
- Create `backend/src/TanErp.Application/Crm/Customers/ActivateCustomer/*`
- Create `backend/src/TanErp.Application/Crm/Customers/ICustomerLifecycleStore.cs`
- Create `backend/src/TanErp.Application/Crm/Sites/CreateSite/*`, `ListSites/*`, `ISiteStore.cs`, `SiteProjection.cs`
- Create `backend/src/TanErp.Application/Crm/Opportunities/CreateOpportunity/*`, `ListOpportunities/*`, `GetOpportunity/*`, `IOpportunityStore.cs`, `OpportunityProjection.cs`, `OpportunityCursor.cs`
- Create `backend/src/TanErp.Infrastructure/Persistence/Crm/CustomerLifecycleStore.cs`, `SiteStore.cs`, `OpportunityStore.cs`
- Create `backend/src/TanErp.Infrastructure/Persistence/Configurations/SiteConfiguration.cs`, `OpportunityConfiguration.cs`
- Modify `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`, `TestOnlyDataSeeder.cs`
- Create EF migration `*_OpportunitySiteSlice.cs` + designer; modify snapshot
- Create API request/response contracts and `SitesController.cs`, `OpportunitiesController.cs`; modify `CustomersController.cs`, `RequestContextReader.cs`, error mapper/resources and `Program.cs`
- Regenerate `contracts/openapi/tan-erp.v1.json`

### Frontend

- Modify `frontend/src/lib/api/api-client.ts` — conditional request option and generated Site/Opportunity methods
- Create `frontend/src/features/sites/` — queries, schema, Site editor/list components
- Create `frontend/src/features/opportunities/` — queries, schema, list/editor/detail and canonical label helpers
- Create `frontend/src/components/ui/ConfirmationModal.tsx` + test
- Create `frontend/src/app/[locale]/(erp)/customers/[customerId]/sites/[id]/page.tsx`
- Create `frontend/src/app/[locale]/(erp)/opportunities/page.tsx`
- Create `frontend/src/app/[locale]/(erp)/opportunities/[id]/page.tsx`
- Modify Customer detail, ERP shell, icons, th/en messages and generated API file
- Create/modify component/query/schema tests and `frontend/e2e/opportunity-site.spec.ts`

---

### Task 1: Lock Slice 2 Contract and Gate

**Files:**
- Modify: `AGENTS.md`
- Modify: `docs/03-contracts/crm-site-survey-api-contract.md`
- Modify: `docs/03-contracts/error-contract.md`
- Modify: `docs/05-engineering/crm-site-survey-uat-scenarios.md`
- Modify: `docs/05-engineering/customer-contact-verification.md`
- Modify: `docs/README.md`

**Interfaces:**
- Consumes: Accepted CRM Flow, Field Catalog, Governance, Data Contract and Customer verification evidence
- Produces: exact Slice 2 contract shown in this plan and executable exit criteria

- [x] **Step 1: Record the completed Customer E2E evidence**

Add the exact command, tested commit SHA, Playwright count, pass/fail count and exit code supplied by Antigravity to `customer-contact-verification.md`. If any value is unavailable, stop before Task 2 and request the missing evidence.

- [x] **Step 2: Authorize the approved implementation boundary**

Update `AGENTS.md` Current phase so the authorized implementation boundary points to this approved plan. Preserve the existing product boundaries and state explicitly that lifecycle transitions beyond Customer activation remain deferred.

- [x] **Step 3: Add the exact activate/site/opportunity contract**

Copy the request/response shapes and derived-field rules from `Exact Slice Contract` into the authoritative API contract. State that Opportunity `branchId` and `ownerUserId` are server-derived in Slice 2 even though the broader baseline example allows assignment in a later slice.

- [x] **Step 4: Add stable errors**

```text
IF_MATCH_REQUIRED       428  If-Match missing or not a quoted UUID
ACTIVE_BRANCH_REQUIRED  422  selected Membership has no Active Branch
SITE_FIELD_REQUIRED     422  Site field/address/coordinate pair invalid
```

Retain `CUSTOMER_INVALID_STATE`, `CUSTOMER_VERSION_CONFLICT`, `OPPORTUNITY_FIELD_REQUIRED`, `RESOURCE_NOT_FOUND` and `IDEMPOTENCY_KEY_REUSED`.

- [x] **Step 5: Add Slice 2 UAT subset**

Require `UAT-CRM-001` activation remainder, `UAT-CRM-004` create-Draft subset only, Site creation portion of `UAT-SRV-001`, `UAT-SEC-001`, `UAT-SEC-002`, `UAT-UX-001`, `UAT-I18N-001`, idempotency retry and stale ETag cases. Explicitly defer Qualify/Stage History and Survey Appointment.

- [x] **Step 6: Verify and commit**

Run: `rg -n "Customer Activation|ACTIVE_BRANCH_REQUIRED|IF_MATCH_REQUIRED|Opportunity \+ Site Slice 2" docs`

Run: `git diff --check -- docs`

Expected: all exact rules are discoverable and diff check exits `0`.

```bash
git add AGENTS.md docs/03-contracts/crm-site-survey-api-contract.md docs/03-contracts/error-contract.md docs/05-engineering/crm-site-survey-uat-scenarios.md docs/05-engineering/customer-contact-verification.md docs/README.md
git commit -m "docs(crm): approve opportunity site slice contract"
```

### Task 2: Implement Idempotent Customer Activation

**Files:**
- Modify: `backend/src/TanErp.Domain/Crm/Customers/Customer.cs`
- Modify: `backend/src/TanErp.Domain/Crm/Customers/CustomerValues.cs`
- Modify: `backend/src/TanErp.Domain/Crm/Customers/CustomerContact.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/ICustomerLifecycleStore.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/ActivateCustomer/ActivateCustomerCommand.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/ActivateCustomer/ActivateCustomerHandler.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Crm/CustomerLifecycleStore.cs`
- Modify: `backend/src/TanErp.Api/RequestContext/RequestContextReader.cs`
- Modify: `backend/src/TanErp.Api/Controllers/CustomersController.cs`
- Test: `backend/tests/TanErp.UnitTests/Crm/Customers/CustomerTests.cs`
- Test: `backend/tests/TanErp.UnitTests/Crm/Customers/ActivateCustomerHandlerTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`

**Interfaces:**
- Consumes: `customers.activate`, customer ID, Membership context, Idempotency-Key and quoted UUID If-Match
- Produces: `ActivateCustomerHandler.Handle(ActivateCustomerCommand, CancellationToken)` → `Result<CustomerProjection>`

- [x] **Step 1: Write RED domain tests**

```csharp
var originalVersion = customer.RowVersion;
var outcome = customer.Activate(originalVersion);
Assert.Equal(CustomerActivationOutcome.Activated, outcome);
Assert.Equal(CustomerStatus.Active, customer.Status);
Assert.NotEqual(originalVersion, customer.RowVersion);
Assert.Equal(CustomerActivationOutcome.VersionConflict, customer.Activate(originalVersion));
```

Also assert a second activation with the current version returns `InvalidState`.

- [x] **Step 2: Run domain tests and confirm RED**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "CustomerTests"`

Expected: FAIL because `Activate` and `CustomerActivationOutcome` do not exist.

- [x] **Step 3: Add the domain transition**

```csharp
public enum CustomerActivationOutcome { Activated, VersionConflict, InvalidState }

public CustomerActivationOutcome Activate(Guid expectedRowVersion)
{
    if (RowVersion != expectedRowVersion) return CustomerActivationOutcome.VersionConflict;
    if (Status != CustomerStatus.Draft) return CustomerActivationOutcome.InvalidState;
    if (_contacts.Count(c => c.IsPrimary && c.Status == ContactStatus.Active) != 1)
        return CustomerActivationOutcome.InvalidState;
    Status = CustomerStatus.Active;
    RowVersion = Guid.NewGuid();
    return CustomerActivationOutcome.Activated;
}
```

Add `ContactStatus` to `CustomerValues.cs` with canonical `active|inactive`, then replace the literal `"active"` defaults and assignments in `CustomerContact` with `ContactStatus.Active` before adding the activation method.

- [x] **Step 4: Write RED handler/store/API tests**

Cover permission denied without write, missing/invalid If-Match → `428 IF_MATCH_REQUIRED`, stale version → `409 CUSTOMER_VERSION_CONFLICT`, inactive/foreign customer → `404`, success → `200` + new ETag + audit, replay → same resource/version, reused key different version → `409`.

- [x] **Step 5: Implement the lifecycle port and handler**

```csharp
public sealed record ActivateCustomerCommand(
    string FirebaseUid, Guid MembershipId, Guid CustomerId,
    Guid ExpectedRowVersion, string IdempotencyKey, string TraceId);

public interface ICustomerLifecycleStore
{
    Task<Result<CustomerProjection>> ActivateAsync(
        RequestAccessContext access, Guid customerId, Guid expectedRowVersion,
        string keyHash, string payloadHash, string traceId,
        CancellationToken cancellationToken = default);
}
```

Handler resolves `customers.activate`, hashes key and canonical payload `customerId|expectedRowVersion|activate`, then calls the store once.

- [x] **Step 6: Implement atomic store and thin endpoint**

Store checks idempotency inside EF transaction before version/state checks, loads Customer by `(organizationId,id)`, invokes `Activate`, adds `customer.activated` audit with `{"changedFields":["status"]}`, commits and catches `DbUpdateConcurrencyException` as `CUSTOMER_VERSION_CONFLICT`. Controller returns mapped Problem Details or Customer response with new ETag.

- [x] **Step 7: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "CustomerTests|ActivateCustomerHandlerTests"`

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter CustomerEndpointsTests`

Expected: all activation cases pass.

```bash
git add backend/src/TanErp.Domain/Crm/Customers backend/src/TanErp.Application/Crm/Customers backend/src/TanErp.Infrastructure/Persistence/Crm/CustomerLifecycleStore.cs backend/src/TanErp.Api/RequestContext/RequestContextReader.cs backend/src/TanErp.Api/Controllers/CustomersController.cs backend/tests
git commit -m "feat(crm): activate customer with concurrency guard"
```

### Task 3: Add Site and Opportunity Domain Models

**Files:**
- Create: `backend/src/TanErp.Domain/Crm/Sites/Site.cs`
- Create: `backend/src/TanErp.Domain/Crm/Sites/SiteValues.cs`
- Create: `backend/src/TanErp.Domain/Crm/Opportunities/Opportunity.cs`
- Create: `backend/src/TanErp.Domain/Crm/Opportunities/OpportunityValues.cs`
- Test: `backend/tests/TanErp.UnitTests/Crm/Sites/SiteTests.cs`
- Test: `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityTests.cs`

**Interfaces:**
- Produces: `Site.CreateActive(...)`, `Opportunity.CreateDraft(...)`, canonical value sets and generated codes

- [x] **Step 1: Write RED Site tests**

Assert label/address/country normalization, paired latitude/longitude requirement, ranges `-90..90`/`-180..180`, active status, generated code and row version. Assert blank required address and one-sided coordinate throw `ArgumentException`.

- [x] **Step 2: Write RED Opportunity tests**

Assert Draft stage, unique work types, allowed work types `built-in|interior|curtain|wallpaper|exterior|other`, positive expected budget with three-letter currency, generated code and row version. Assert blank title, invalid work type and non-positive budget throw.

- [x] **Step 3: Run and confirm RED**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "SiteTests|OpportunityTests"`

Expected: FAIL because the new domain types do not exist.

- [x] **Step 4: Implement exact factories**

```csharp
public static Site CreateActive(
    Guid id, Guid organizationId, Guid customerId, Guid createdByUserId,
    string label, SiteAddressInput address, decimal? latitude,
    decimal? longitude, string? accessNote, DateTimeOffset now);

public static Opportunity CreateDraft(
    Guid id, Guid organizationId, Guid branchId, Guid customerId,
    Guid? primarySiteId, Guid ownerUserId, Guid createdByUserId,
    string title, string? scopeSummary, IReadOnlyCollection<string> workTypes,
    string? sourceCode, decimal? expectedBudget, string? currencyCode,
    DateOnly? targetDecisionDate, DateTimeOffset? nextActionAtUtc,
    string? nextActionNote, DateTimeOffset now);
```

Use value constants in Domain; do not depend on EF, ASP.NET or Application types.

- [x] **Step 5: Run and commit**

Run the same filtered tests; expected PASS.

```bash
git add backend/src/TanErp.Domain/Crm/Sites backend/src/TanErp.Domain/Crm/Opportunities backend/tests/TanErp.UnitTests/Crm/Sites backend/tests/TanErp.UnitTests/Crm/Opportunities
git commit -m "feat(crm): add site and opportunity aggregates"
```

### Task 4: Persist Site and Opportunity with Database Guards

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/SiteConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/OpportunityConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_OpportunitySiteSlice.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_OpportunitySiteSlice.Designer.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/OpportunitySiteMigrationTests.cs`

**Interfaces:**
- Consumes: Domain `Site` and `Opportunity`
- Produces: `crm.sites`, `crm.opportunities` with composite isolation constraints

- [x] **Step 1: Write RED migration tests**

Verify clean apply, rollback/reapply, exact snake_case columns, unique `(organization_id,code)`, Site FK `(customer_id,organization_id)`, Opportunity FKs for customer/branch and primary Site `(primary_site_id,customer_id,organization_id)`, concurrency tokens and indexes.

- [x] **Step 2: Run and confirm RED**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter OpportunitySiteMigrationTests`

Expected: FAIL because tables/configurations are absent.

- [x] **Step 3: Add DbSets and configurations**

Add `DbSet<Site> Sites` and `DbSet<Opportunity> Opportunities`. Configure max lengths from Field Catalog, `numeric(12,2)` budget, `numeric(9,6)` latitude, `numeric(10,6)` longitude, `date` target date, PostgreSQL `text[]` work types, check constraints for status/stage/coordinates/budget and `DeleteBehavior.Restrict`.

- [x] **Step 4: Generate migration**

Run:

```bash
dotnet ef migrations add OpportunitySiteSlice --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations
```

Inspect generated migration; do not edit older migrations.

- [x] **Step 5: Run migration tests and commit**

Run the filtered tests; expected PASS including cross-organization constraint rejection.

```bash
git add backend/src/TanErp.Infrastructure/Persistence backend/tests/TanErp.IntegrationTests/Persistence/OpportunitySiteMigrationTests.cs docs/superpowers/plans/2026-09-08-opportunity-site-vertical-slice.md
git commit -m "feat(crm): persist opportunity site slice"
```

### Task 5: Implement Idempotent Site Create and Scoped List

**Files:**
- Create: `backend/src/TanErp.Application/Crm/Sites/SiteProjection.cs`
- Create: `backend/src/TanErp.Application/Crm/Sites/ISiteStore.cs`
- Create: `backend/src/TanErp.Application/Crm/Sites/CreateSite/CreateSiteCommand.cs`
- Create: `backend/src/TanErp.Application/Crm/Sites/CreateSite/CreateSiteHandler.cs`
- Create: `backend/src/TanErp.Application/Crm/Sites/ListSites/ListSitesQuery.cs`
- Create: `backend/src/TanErp.Application/Crm/Sites/ListSites/ListSitesHandler.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Crm/SiteStore.cs`
- Test: `backend/tests/TanErp.UnitTests/Crm/Sites/SiteHandlerTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/SiteStoreTests.cs`

**Interfaces:**
- Produces: `CreateSiteHandler.Handle(...)` and `ListSitesHandler.Handle(...)`

- [ ] **Step 1: Write RED handler tests**

Cover `sites.manage`/`sites.read`, foreign/inactive Customer, invalid fields, create success, exact replay and changed-payload conflict. Assert the store is not called after permission or validation failure.

- [ ] **Step 2: Define exact contracts**

```csharp
public sealed record CreateSiteCommand(
    string FirebaseUid, Guid MembershipId, Guid CustomerId, string IdempotencyKey,
    string Label, string AddressLine1, string Subdistrict, string District,
    string Province, string PostalCode, string CountryCode,
    decimal? Latitude, decimal? Longitude, string? AccessNote, string TraceId);

public interface ISiteStore
{
    Task<Result<SiteProjection>> CreateAsync(
        RequestAccessContext access, CreateSiteCommand command,
        string keyHash, string payloadHash, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SiteProjection>?> ListByCustomerAsync(
        Guid organizationId, Guid customerId, CancellationToken cancellationToken = default);
}
```

Null list means Customer missing/out of scope; empty collection means valid Customer with no Site.

- [ ] **Step 3: Implement handler and atomic EF store**

Create checks Active Customer in selected Organization, adds Site, idempotency record and `site.created` audit in one transaction. Canonical payload uses normalized fields/hashes but never logs or stores raw payload in idempotency/audit.

- [ ] **Step 4: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter SiteHandlerTests`

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter SiteStoreTests`

Expected: all pass.

```bash
git add backend/src/TanErp.Application/Crm/Sites backend/src/TanErp.Infrastructure/Persistence/Crm/SiteStore.cs backend/tests/TanErp.UnitTests/Crm/Sites backend/tests/TanErp.IntegrationTests/Persistence/SiteStoreTests.cs
git commit -m "feat(crm): create scoped customer sites"
```

### Task 6: Implement Opportunity Create, List and Detail

**Files:**
- Create: `backend/src/TanErp.Application/Crm/Opportunities/OpportunityProjection.cs`
- Create: `backend/src/TanErp.Application/Crm/Opportunities/OpportunityCursor.cs`
- Create: `backend/src/TanErp.Application/Crm/Opportunities/IOpportunityStore.cs`
- Create: `backend/src/TanErp.Application/Crm/Opportunities/CreateOpportunity/*`
- Create: `backend/src/TanErp.Application/Crm/Opportunities/ListOpportunities/*`
- Create: `backend/src/TanErp.Application/Crm/Opportunities/GetOpportunity/*`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs`
- Test: `backend/tests/TanErp.UnitTests/Crm/Opportunities/OpportunityHandlerTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/OpportunityStoreTests.cs`

**Interfaces:**
- Consumes: selected Membership branch, authenticated user, active Customer, optional same-customer Active Site
- Produces: create/list/get projections with organization-scoped cursor reads

- [ ] **Step 1: Write RED tests**

Cover permissions, no branch → `ACTIVE_BRANCH_REQUIRED`, inactive Customer → `CUSTOMER_INVALID_STATE`, foreign IDs → `RESOURCE_NOT_FOUND`, Site belongs to another Customer → `RESOURCE_NOT_FOUND`, invalid business fields → `OPPORTUNITY_FIELD_REQUIRED`, create/replay/conflict, cursor stability and tenant isolation.

- [ ] **Step 2: Define store interface**

```csharp
public sealed record OpportunityListFilter(
    string? Search, Guid? CustomerId, string? Stage, int Limit = 25, string? Cursor = null);

public interface IOpportunityStore
{
    Task<Result<OpportunityProjection>> CreateAsync(
        RequestAccessContext access, CreateOpportunityCommand command,
        string keyHash, string payloadHash, CancellationToken cancellationToken = default);
    Task<OpportunityPage> ListAsync(
        Guid organizationId, OpportunityListFilter filter,
        CancellationToken cancellationToken = default);
    Task<OpportunityProjection?> GetAsync(
        Guid organizationId, Guid opportunityId,
        CancellationToken cancellationToken = default);
}
```

- [ ] **Step 3: Implement handlers**

Create handler derives branch from `access.BranchId` and owner from `access.ActorUserId`; it never trusts body values. List/get resolve `opportunities.read`; create resolves `opportunities.create`. Validate cursor before store call.

- [ ] **Step 4: Implement EF store**

Validate Active Customer, Active Branch, actor's Active Membership and optional Active Site in the same Organization/Customer. Persist Opportunity + idempotency + `opportunity.created` audit atomically. List uses EF keyset pagination ordered `next_action_at_utc ASC NULLS LAST, id ASC` and projects only allowlisted fields.

- [ ] **Step 5: Run and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter OpportunityHandlerTests`

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter OpportunityStoreTests`

Expected: all pass.

```bash
git add backend/src/TanErp.Application/Crm/Opportunities backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs backend/tests/TanErp.UnitTests/Crm/Opportunities backend/tests/TanErp.IntegrationTests/Persistence/OpportunityStoreTests.cs
git commit -m "feat(crm): create and query opportunities"
```

### Task 7: Publish HTTP, Permissions and OpenAPI

**Files:**
- Create: `backend/src/TanErp.Api/Contracts/Crm/Sites/CreateSiteRequest.cs`
- Create: `backend/src/TanErp.Api/Contracts/Crm/Sites/SiteResponse.cs`
- Create: `backend/src/TanErp.Api/Contracts/Crm/Opportunities/CreateOpportunityRequest.cs`
- Create: `backend/src/TanErp.Api/Contracts/Crm/Opportunities/OpportunityResponse.cs`
- Create: `backend/src/TanErp.Api/Contracts/Crm/Opportunities/OpportunityListResponse.cs`
- Create: `backend/src/TanErp.Api/Controllers/SitesController.cs`
- Create: `backend/src/TanErp.Api/Controllers/OpportunitiesController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Modify: `backend/src/TanErp.Api/Resources/Errors.resx`
- Modify: `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs`
- Modify: `contracts/openapi/tan-erp.v1.json`
- Test: `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/OpenApiContractTests.cs`

**Interfaces:**
- Produces: exact HTTP paths and generated-schema source consumed by Frontend

- [ ] **Step 1: Write RED endpoint tests**

Test every success status/location/ETag and stable failure code. Include Org A/B isolation, inactive Customer, no active branch, missing permissions, exact replay, changed payload and assert address/access note never appears in Problem Details or audit JSON.

- [ ] **Step 2: Add request/response records and controllers**

Controllers only parse request context, map generated HTTP records to commands, invoke handlers, set Location/ETag and map failures via `ProblemDetailsMapper`. Do not put cross-resource rules in controllers.

- [ ] **Step 3: Register services and localized errors**

Register lifecycle/site/opportunity handlers and stores. Add Thai/English title/detail pairs for all new stable codes and map `IF_MATCH_REQUIRED` to 428, `ACTIVE_BRANCH_REQUIRED`/`SITE_FIELD_REQUIRED` to 422.

- [ ] **Step 4: Seed test permissions safely**

Extend find-or-create permission list with `customers.activate`, `sites.read`, `sites.manage`, `opportunities.read`, `opportunities.create` and backfill Org A/B test roles. Do not seed Site/Opportunity in production paths.

- [ ] **Step 5: Regenerate OpenAPI**

Use the existing OpenAPI export workflow documented in `frontend/README.md`; inspect diff for only Slice 2 endpoints/schemas and status responses.

- [ ] **Step 6: Run and commit**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "OpportunitySiteEndpointsTests|OpenApiContractTests"`

Expected: all endpoint and contract tests pass.

```bash
git add backend/src/TanErp.Api backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs backend/tests/TanErp.IntegrationTests/Api contracts/openapi/tan-erp.v1.json
git commit -m "feat(api): publish opportunity site contract"
```

### Task 8: Extend the Typed Frontend API and Query Layer

**Files:**
- Modify: `frontend/src/generated/api/tan-erp.v1.ts`
- Modify: `frontend/src/lib/api/api-client.ts`
- Modify: `frontend/src/lib/api/api-client.test.ts`
- Create: `frontend/src/features/sites/api/site-queries.ts`
- Create: `frontend/src/features/sites/api/site-queries.test.tsx`
- Create: `frontend/src/features/opportunities/api/opportunity-queries.ts`
- Create: `frontend/src/features/opportunities/api/opportunity-queries.test.tsx`

**Interfaces:**
- Consumes: generated `components`/`paths` only
- Produces: API methods and business query keys scoped by Membership + locale + filters

- [ ] **Step 1: Regenerate generated TypeScript**

Run: `npm --prefix frontend run generate:api`

Do not edit `tan-erp.v1.ts` manually.

- [ ] **Step 2: Write RED API client tests**

Assert activate sends quoted `If-Match`, Membership and idempotency headers; Site/Opportunity create send derived-field-free bodies; query params are encoded; non-2xx uses Problem Details.

- [ ] **Step 3: Extend request options and methods**

```ts
export interface RequestOptions {
  token: string;
  membershipId?: string;
  idempotencyKey?: string;
  ifMatch?: string;
  locale?: "th" | "en";
  signal?: AbortSignal;
}
```

Export all DTO aliases from `components["schemas"]` and query params from `paths[...]`; add `activateCustomer`, `createSite`, `listCustomerSites`, `createOpportunity`, `listOpportunities`, `getOpportunity`.

- [ ] **Step 4: Add query keys/hooks**

All keys start `['business', membershipId, locale, ...]`. Mutations invalidate only affected customer/site/opportunity keys; Membership switching remains owned by selected-membership context.

- [ ] **Step 5: Run and commit**

Run: `npm --prefix frontend run test -- --run src/lib/api/api-client.test.ts src/features/sites/api/site-queries.test.tsx src/features/opportunities/api/opportunity-queries.test.tsx`

Expected: all pass.

```bash
git add frontend/src/generated/api/tan-erp.v1.ts frontend/src/lib/api frontend/src/features/sites/api frontend/src/features/opportunities/api
git commit -m "feat(frontend): add opportunity site data layer"
```

### Task 9: Add Customer Activation and Site Create Flow

**Files:**
- Create: `frontend/src/components/ui/ConfirmationModal.tsx`
- Create: `frontend/src/components/ui/ConfirmationModal.test.tsx`
- Modify: `frontend/src/components/ui/index.ts`
- Modify: `frontend/src/features/customers/components/customer-detail.tsx`
- Modify: `frontend/src/features/customers/components/customer-detail.test.tsx`
- Create: `frontend/src/features/sites/schemas/site-form-schema.ts`
- Create: `frontend/src/features/sites/schemas/site-form-schema.test.ts`
- Create: `frontend/src/features/sites/components/site-editor.tsx`
- Create: `frontend/src/features/sites/components/site-editor.test.tsx`
- Create: `frontend/src/features/sites/components/site-list.tsx`
- Create: `frontend/src/app/[locale]/(erp)/customers/[customerId]/sites/[id]/page.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`

**Interfaces:**
- Consumes: Customer ETag/status, activation mutation, Site methods, `customers.activate`, `sites.read`, `sites.manage`
- Produces: activation confirmation and `/customers/{customerId}/sites/create` flow

- [ ] **Step 1: Write RED modal and activation tests**

Assert focus enters modal and returns to trigger, Escape closes only when idle, confirm is disabled/loading during request, Customer Draft shows action only with permission, stale ETag shows localized conflict and success refreshes Customer ETag/status.

- [ ] **Step 2: Implement shared ConfirmationModal**

Use semantic dialog, labelled title/description, focus trap, `aria-modal="true"`, 44px controls and no external package. Activation copy must explain that Active Customer can start Site/Opportunity work.

- [ ] **Step 3: Write RED Site schema/editor tests**

Assert all structured fields, postal code, uppercase two-letter country, paired coordinates/ranges, submit locking, idempotency retry behavior, generic localized errors and navigation to Customer detail after success.

- [ ] **Step 4: Implement Site page**

Use Single Dynamic Route `[id]`; accept only `id=create` in this slice and call `notFound()` for arbitrary IDs until Site detail/edit is authorized. Use `FormProvider` + `Controller`; disable all inputs during valid submit; no file upload.

- [ ] **Step 5: Add Site list to Customer detail**

Show Minimal Mono loading, empty/error/data states, address only with `sites.read`, and create CTA only when Customer is Active plus `sites.manage`. Do not nest button inside link.

- [ ] **Step 6: Add th/en keys and run tests**

Run: `npm --prefix frontend run test -- --run src/components/ui/ConfirmationModal.test.tsx src/features/customers/components/customer-detail.test.tsx src/features/sites`

Expected: tests pass and message key parity remains exact.

- [ ] **Step 7: Commit**

```bash
git add frontend/src/components/ui frontend/src/features/customers frontend/src/features/sites 'frontend/src/app/[locale]/(erp)/customers/[customerId]/sites/[id]/page.tsx' frontend/src/messages
git commit -m "feat(crm): activate customers and create sites"
```

### Task 10: Build Opportunity List, Create and Detail Screens

**Files:**
- Create: `frontend/src/features/opportunities/schemas/opportunity-form-schema.ts`
- Create: `frontend/src/features/opportunities/schemas/opportunity-form-schema.test.ts`
- Create: `frontend/src/features/opportunities/components/opportunity-list.tsx`
- Create: `frontend/src/features/opportunities/components/opportunity-list.test.tsx`
- Create: `frontend/src/features/opportunities/components/opportunity-editor.tsx`
- Create: `frontend/src/features/opportunities/components/opportunity-editor.test.tsx`
- Create: `frontend/src/features/opportunities/components/opportunity-detail.tsx`
- Create: `frontend/src/features/opportunities/components/opportunity-detail.test.tsx`
- Create: `frontend/src/features/opportunities/opportunity-labels.ts`
- Create: `frontend/src/app/[locale]/(erp)/opportunities/page.tsx`
- Create: `frontend/src/app/[locale]/(erp)/opportunities/[id]/page.tsx`
- Modify: `frontend/src/components/layout/erp-shell.tsx`
- Modify: `frontend/src/components/layout/erp-shell.test.tsx`
- Modify: `frontend/src/components/common/Icons.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`

**Interfaces:**
- Consumes: Opportunity query/mutation hooks, Active Customer list, Customer Sites, selected Membership Branch and current User
- Produces: permission-aware navigation and `/opportunities`, `/opportunities/create`, `/opportunities/{id}`

- [ ] **Step 1: Write RED schema tests**

Assert title required/max 250, canonical work types, positive budget + required THB when budget exists, target date format, next action date/note pairing and optional primary Site belonging to selected Customer.

- [ ] **Step 2: Write RED component tests**

Cover loading/empty/error/data, localized draft label, create CTA permissions, no-Branch block, customer selection restricted to Active, site options refresh when Customer changes, double submit, retry key reuse/rotation and detail rendering.

- [ ] **Step 3: Implement list and dynamic route**

`[id]` handles `create|add` and UUID detail. List supports search/stage filter and keyset Load More. Detail displays Customer/Site IDs as navigation links only after their scoped resources are loaded; unknown enum uses localized operation failure.

- [ ] **Step 4: Implement form**

Use `FormProvider`, `Controller`, generated request type, current selected Membership Branch label and current User label as read-only context. Payload includes only fields from the exact contract. When Customer changes, clear `primarySiteId` synchronously in the change handler and load that Customer's Sites via TanStack Query; do not use `useEffect` for derived state.

- [ ] **Step 5: Add shell navigation and translations**

Show Opportunity navigation only with `opportunities.read`; close mobile drawer and preserve focus behavior. Add a pure SVG stroke icon and complete th/en message parity.

- [ ] **Step 6: Run and commit**

Run: `npm --prefix frontend run test -- --run src/features/opportunities src/components/layout/erp-shell.test.tsx`

Expected: all pass.

```bash
git add frontend/src/features/opportunities 'frontend/src/app/[locale]/(erp)/opportunities' frontend/src/components/layout/erp-shell.tsx frontend/src/components/layout/erp-shell.test.tsx frontend/src/components/common/Icons.tsx frontend/src/messages
git commit -m "feat(crm): add opportunity workspace"
```

### Task 11: Prove the Slice and Publish Evidence

**Files:**
- Create: `frontend/e2e/opportunity-site.spec.ts`
- Create: `docs/05-engineering/opportunity-site-verification.md`
- Modify: `docs/README.md`

**Interfaces:**
- Consumes: completed Slice 2 application and test stack
- Produces: executable gate evidence and explicit next boundary

- [ ] **Step 1: Add Playwright acceptance journey**

Test login → Draft Customer → activate → Site create → Opportunity create → detail → list/search. Assert request headers, status/ETag, server-derived Branch/Owner, Draft stage, Thai labels, one POST on double click and retry behavior.

- [ ] **Step 2: Add negative/security journeys**

Assert inactive Customer rejected, Org B cannot use Org A Customer/Site, selected Membership without Branch is blocked before POST and Backend still rejects direct API, stale Customer ETag conflicts, address/access note absent from error/audit/log fixtures, keyboard flow and 320px/200% zoom.

- [ ] **Step 3: Run Backend gates**

Run: `dotnet build backend/TanErp.slnx`

Run: `dotnet test backend/TanErp.slnx`

Expected: exit `0`, zero failed tests.

- [ ] **Step 4: Run Frontend and fixtures gates**

Run: `npm --prefix frontend run verify`

Run: `npm run test:fixtures`

Expected: API drift, lint, typecheck, Vitest, production build and fixture tests pass.

- [ ] **Step 5: Run real E2E**

With PostgreSQL 17, Firebase emulator, Backend `:5005` and Frontend `:3005` running:

Run: `PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e`

Expected: auth, Customer and Opportunity/Site journeys all pass; test discovery alone is not sufficient.

- [ ] **Step 6: Run policy scans**

Run: `git diff --check`

Run: `rg -n "\bany\b|as any|@ts-ignore|BEGIN PRIVATE KEY|firebase-admin" frontend/src frontend/e2e`

Review matches manually; expected no forbidden TypeScript or committed secret.

- [ ] **Step 7: Write verification record**

Record runtime versions, migration name, exact commands/counts/exit codes, UAT IDs, tenant/isolation evidence, responsive/accessibility checks and known deferred scope. Set status Passed only when Steps 3–6 have real evidence.

- [ ] **Step 8: Commit evidence**

```bash
git add frontend/e2e/opportunity-site.spec.ts docs/05-engineering/opportunity-site-verification.md docs/README.md
git commit -m "test(crm): verify opportunity site slice"
```

## Definition of Success

1. Customer Draft activate ได้เฉพาะ permission/scope ที่ถูกต้องด้วย If-Match + idempotency และได้ ETag ใหม่
2. Active Site ถูกสร้างใต้ Active Customer พร้อม structured address และไม่มี Personal Data รั่วใน audit/log/error
3. Opportunity Draft ถูกสร้างจาก Active Customer โดย Branch/Owner derive จาก Active Membership และ Primary Site อยู่ Customer เดียวกัน
4. Retry intent เดิมไม่สร้างซ้ำ; key เดิม payload ใหม่ได้ `409`; stale activation version ได้ `409`
5. List/detail จำกัด Organization เดียว; cross-organization และ mismatched Site/Customer คืน `404`
6. OpenAPI และ generated frontend types ไม่มี drift; Frontend ไม่มี handwritten API DTO
7. Customer activation, Site create/list และ Opportunity create/list/detail ใช้งานได้ทั้ง th/en, keyboard, 320px และ zoom 200%
8. Backend build/test, Frontend verify, fixture tests, real E2E และ diff/policy scans ผ่านทั้งหมด
9. Verification record มีผลจริงครบและ Next Gate ยังคงล็อก Opportunity lifecycle + Site Survey

## Explicitly Deferred

```text
Customer update/deactivate and additional contacts
Site edit/deactivate, geocoding and Customer Address resource
Opportunity edit, qualify, stage transition/history, close and reopen
Owner reassignment and Branch/Own permission scope
Survey appointment, Site Survey identity/revision and evidence upload
Fuzzy duplicate review/merge, export and retention/redaction workflows
Official Estimate, Project, Procurement, Inventory, Production and MRP
```
