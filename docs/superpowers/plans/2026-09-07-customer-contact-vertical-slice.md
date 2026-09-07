# Customer + Contact Vertical Slice Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ให้ผู้ใช้ที่ผ่าน Firebase Login และมี Membership/Permission ที่ถูกต้องค้นหา เปิดดู และสร้าง Customer Draft พร้อม Primary Contact ผ่าน UI → API → PostgreSQL → Audit ได้โดยไม่รั่วข้อมูลข้าม Organization

**Architecture:** เพิ่ม CRM Customer aggregate ภายใน Clean Architecture สี่ Project โดยให้ EF Core ถือครอง write/transaction และใช้ Application feature folders สำหรับ Create/List/Get Detail แยกกัน ทุก Business Request ระบุ `X-Membership-Id`; Backend resolve User, Membership, Organization และ Effective Permission จาก PostgreSQL ใหม่ทุก Request และไม่รับ `organizationId` จาก payload ฝั่ง Client Frontend ใช้ ERP layout เดิม, generated OpenAPI types, central API client, TanStack Query และ React Hook Form + Zod

**Tech Stack:** .NET SDK `10.0.400`, ASP.NET Core/EF Core `10.0.11`, Npgsql EF Core `10.0.3`, PostgreSQL `17-alpine`, Next.js `16.3.4`, React `19.2.8`, TypeScript `7.0.2`, TanStack Query `5.102.8`, Zod `4.5.4`, React Hook Form `7.87.0`, `@hookform/resolvers` `5.9.1`, Vitest `5.0.0`, Playwright `1.63.0`

## Global Constraints

- เริ่ม Application Code ได้หลังผู้ใช้อนุมัติแผนนี้และ Contract changes ใน Task 1 เท่านั้น
- Foundation Login verification ต้องคงสถานะ `Passed Quality Gate`; หาก Regression ให้หยุด Slice นี้และแก้ Foundation ก่อน
- Scope นี้รวมเฉพาะ Customer List, Customer Detail และ Create Customer Draft + Primary Contact ไม่รวม Update, Activate/Deactivate, Address, Site, Opportunity, Duplicate Review workflow, Export, Merge หรือ Hard Delete
- Slice แรกยอมรับเฉพาะ Effective Permission แบบ `organization`; `branch`, `project` และ `own` ต้อง Fail-closed จนมี Contract เรื่อง Customer ownership/visibility ที่อนุมัติแล้ว
- Client ส่ง `X-Membership-Id`; Backend derive `OrganizationId`, `BranchId` และ `ActorUserId` จาก Active Membership เท่านั้น ห้ามรับค่าเหล่านี้จาก Create payload
- Create ต้องมี `customers.create` และ `customer-contacts.manage`; List/Detail ต้องมี `customers.read`; Contact PII แสดงเต็มเฉพาะเมื่อมี `customer-contacts.manage` มิฉะนั้นส่งเฉพาะค่าที่ mask แล้ว
- `POST /api/v1/customers` ต้องรองรับ `Idempotency-Key`; key เดิม + payload เดิมคืน Customer เดิม ส่วน key เดิม + payload ต่างคืน `409 IDEMPOTENCY_KEY_REUSED`
- Customer และ Primary Contact ต้องบันทึกพร้อม Audit Events `customer.created` และ `contact.created` ใน Transaction เดียวกัน
- Audit/structured log ห้ามเก็บ phone, email, tax identifier, Authorization token หรือ raw idempotency key
- Customer code สร้างจาก UUID ฝั่ง Server ในรูป `CUS-` + 12 ตัวแรกของ UUID แบบ uppercase และบังคับ unique `(organization_id, code)` ที่ PostgreSQL
- Duplicate candidate ระยะแรกใช้ exact match หลัง normalize เฉพาะ `displayNameTh`, phone หรือ email ภายใน Organization เดียวกัน; ไม่ใช้ fuzzy matching และไม่ auto-merge
- UI text ใหม่ทั้งหมดอยู่ใน `frontend/src/messages/th.json` และ `en.json` ด้วย key parity; Backend error text อยู่ใน `.resx` ไทย/อังกฤษ
- ห้ามใช้ `any`, `as any`, `@ts-ignore`, manually authored frontend API DTO, direct component `fetch`, Generic Repository, role-name check หรือ raw SQL write
- Reuse proposal ที่อนุมัติพร้อมแผนนี้: เพิ่ม shared request primitive ใน `ApiClient`, shared selected-membership context และ shared permission helper; Customer-specific list/form ยังอยู่ใน `features/customers` จนมี Slice ที่สองพิสูจน์ว่าควรยกเป็น Global UI
- UI ใช้ Atelier Architectural Navy Sharp: `border-radius: 0`, Solid Navy `#0B3056`, pure SVG stroke icons, visible focus, semantic controls, 44px minimum target, no skeleton และรองรับ 320px/zoom 200%
- Form แสดง validation หลัง blur หรือ submit, clear error เมื่อแก้ถูก, เชื่อม `aria-invalid`/`aria-describedby`, ไม่ disable submit ก่อน validation และ disable ทันทีหลัง valid submit เริ่มทำงาน
- ทุก Task ใช้ TDD, commit เฉพาะไฟล์ของ Task และข้อความ `type(scope): description`

## Acceptance Journey

```text
Firebase Login
  → GET /api/v1/me
  → เลือก Active Membership
  → GET /api/v1/customers (empty state)
  → เปิด /customers/create
  → POST Customer Draft + Primary Contact พร้อม Idempotency-Key
  → PostgreSQL commit Customer + Contact + 2 Audit Events แบบ atomic
  → ไป /customers/{id}
  → GET detail จาก Organization เดิมและเห็น generated code/ETag
```

Journey ต้องพิสูจน์เพิ่มว่า Membership ต่าง Organization เปิด resource เดิมไม่ได้ (`404`), ผู้ไม่มี Permission ได้ `403`, request ซ้ำไม่สร้างข้อมูลซ้ำ, Contact ที่ไม่มีสิทธิ์ manage ถูก mask และ log ไม่มี PII

## Planned File Map

### Authoritative documents

- Modify `docs/03-contracts/crm-site-survey-api-contract.md` — ล็อก headers, payloads, projections, pagination และ Slice boundary
- Modify `docs/03-contracts/error-contract.md` — เพิ่ม stable Customer/context errors ที่ใช้จริง
- Modify `docs/05-engineering/crm-site-survey-uat-scenarios.md` — เพิ่ม Slice-1 acceptance subset และ pass criteria
- Modify `docs/05-engineering/foundation-login-verification.md` — บันทึกการอนุมัติ Next Gate โดยไม่แก้ผล Foundation เดิม
- Modify `docs/README.md` — ลิงก์แผนและ verification record ของ Slice

### Backend

- Create `backend/src/TanErp.Domain/Crm/Customers/Customer.cs` — Customer aggregate root และ Draft invariants
- Create `backend/src/TanErp.Domain/Crm/Customers/CustomerContact.cs` — Contact child entity และ contact-channel invariant
- Create `backend/src/TanErp.Domain/Crm/Customers/CustomerValues.cs` — constant sets และ normalization functions ของ Slice
- Create `backend/src/TanErp.Domain/Common/IdempotencyRecord.cs` — reusable idempotent mutation record ที่เก็บ hash/resource ID เท่านั้น
- Create `backend/src/TanErp.Application/Common/Abstractions/IRequestAccessResolver.cs` — port สำหรับ resolve actor/membership/permission
- Create `backend/src/TanErp.Application/Common/Models/RequestAccessContext.cs` — trusted access context ที่ได้จาก resolver
- Create `backend/src/TanErp.Application/Crm/Customers/ICustomerCreationStore.cs` — use-case-specific atomic write port โดยไม่เปิดเผย EF Core type
- Create `backend/src/TanErp.Application/Crm/Customers/ICustomerReadStore.cs` — scoped list/detail projection port โดยไม่เปิดเผย EF Core type
- Create `backend/src/TanErp.Application/Crm/Customers/CreateCustomer/*` — idempotent atomic create use case
- Create `backend/src/TanErp.Application/Crm/Customers/ListCustomers/*` — cursor list query/result
- Create `backend/src/TanErp.Application/Crm/Customers/GetCustomer/*` — scoped detail query/result
- Create `backend/src/TanErp.Infrastructure/Persistence/RequestAccessResolver.cs` — active membership + effective permission query
- Create `backend/src/TanErp.Infrastructure/Persistence/Crm/CustomerCreationStore.cs` — EF transaction, idempotency and audit write implementation
- Create `backend/src/TanErp.Infrastructure/Persistence/Crm/CustomerReadStore.cs` — EF list/detail projection implementation
- Create `backend/src/TanErp.Infrastructure/Persistence/Configurations/CustomerConfiguration.cs`
- Create `backend/src/TanErp.Infrastructure/Persistence/Configurations/CustomerContactConfiguration.cs`
- Create `backend/src/TanErp.Infrastructure/Persistence/Configurations/IdempotencyRecordConfiguration.cs`
- Modify `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs` — register new DbSets
- Modify `backend/src/TanErp.Infrastructure/Persistence/Migrations/` — add EF-generated `CustomerContactSlice` migration, designer and snapshot changes
- Modify `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs` — seed three permissions only, no Customer production seed
- Create `backend/src/TanErp.Api/Contracts/Crm/Customers/*.cs` — HTTP-only request/response contracts
- Create `backend/src/TanErp.Api/Controllers/CustomersController.cs` — thin endpoints
- Create `backend/src/TanErp.Api/RequestContext/RequestContextReader.cs` — parse authenticated UID, membership and idempotency headers
- Modify `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs` และ `.resx` — map Customer/context errors
- Modify `backend/src/TanErp.Api/Program.cs` — register resolver/handlers

### Frontend

- Modify `frontend/package.json` และ `frontend/package-lock.json` — add exact form dependencies
- Create `frontend/src/app/[locale]/(erp)/layout.tsx` — compose AccessGate, selected membership and reusable shell once
- Modify `frontend/src/app/[locale]/(erp)/page.tsx` — home content only
- Modify `frontend/src/components/layout/erp-shell.tsx` — accept `children`, membership selector and Customer navigation
- Create `frontend/src/lib/membership/selected-membership-context.tsx` — selected Membership state and cache isolation
- Create `frontend/src/lib/permissions/can.ts` — UI-only effective permission helper
- Modify `frontend/src/lib/api/api-client.ts` — shared typed request primitive + Customer methods
- Modify `frontend/src/components/ui/Input.tsx` — correct ARIA linkage for all forms
- Create `frontend/src/features/customers/api/customer-queries.ts`
- Create `frontend/src/features/customers/schemas/customer-form-schema.ts`
- Create `frontend/src/features/customers/components/customer-list.tsx`
- Create `frontend/src/features/customers/components/customer-editor.tsx`
- Create `frontend/src/features/customers/components/customer-detail.tsx`
- Create `frontend/src/features/customers/index.ts`
- Create `frontend/src/app/[locale]/(erp)/customers/page.tsx`
- Create `frontend/src/app/[locale]/(erp)/customers/[id]/page.tsx`
- Modify `frontend/src/messages/th.json` และ `en.json`
- Modify `frontend/src/styles/erp-theme.css`
- Regenerate `contracts/openapi/tan-erp.v1.json` และ `frontend/src/generated/api/tan-erp.v1.ts`; generated TypeScript ห้ามแก้ด้วยมือ

### Tests and evidence

- Create `backend/tests/TanErp.UnitTests/Crm/Customers/CustomerTests.cs`
- Create `backend/tests/TanErp.UnitTests/Crm/Customers/CreateCustomerHandlerTests.cs`
- Create `backend/tests/TanErp.IntegrationTests/Persistence/CustomerContactMigrationTests.cs`
- Create `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`
- Create `frontend/src/lib/permissions/can.test.ts`
- Create `frontend/src/features/customers/api/customer-queries.test.tsx`
- Create `frontend/src/features/customers/components/customer-list.test.tsx`
- Create `frontend/src/features/customers/components/customer-editor.test.tsx`
- Create `frontend/src/features/customers/components/customer-detail.test.tsx`
- Create `frontend/e2e/customer-contact.spec.ts`
- Create `docs/05-engineering/customer-contact-verification.md`

---

### Task 1: Approve the Slice Contract and Scope Boundary

**Files:**
- Modify: `docs/03-contracts/crm-site-survey-api-contract.md`
- Modify: `docs/03-contracts/error-contract.md`
- Modify: `docs/05-engineering/crm-site-survey-uat-scenarios.md`
- Modify: `docs/05-engineering/foundation-login-verification.md`
- Modify: `docs/README.md`

**Interfaces:**
- Consumes: accepted CRM Flow, Field Catalog, Governance, Data Contract และ Foundation `GET /api/v1/me`
- Produces: exact HTTP contract สำหรับ `GET /api/v1/customers`, `GET /api/v1/customers/{id}`, `POST /api/v1/customers`; `X-Membership-Id`; stable errors; Slice-1 UAT IDs

- [ ] **Step 1: Add the approved Slice-1 boundary to the API contract**

เพิ่มหัวข้อ `Customer + Contact Slice 1` ที่ระบุข้อความต่อไปนี้โดยไม่แก้ Endpoint อื่น:

```text
Required business header: X-Membership-Id: <membership UUID>
Required create header: Idempotency-Key: <opaque 16-128 characters>
Supported permission scope in Slice 1: organization only
List sort: normalizedDisplayName ASC, id ASC
List defaults: limit=25; allowed range 1..100
Create result: Customer status=draft, one active primaryContact, ETag="<rowVersion>"
Duplicate signal: exact normalized name/phone/email inside the selected Organization only
```

กำหนด request body ให้มีเฉพาะ `customerType`, `displayNameTh`, `displayNameEn`, `preferredLocale` และ `primaryContact.{name,roleTitle,phone,email,preferredChannel}`; ห้ามมี `organizationId`, `branchId`, `status`, `code`, `rowVersion` หรือ actor ID

- [ ] **Step 2: Add exact response shapes**

List response ใช้โครงนี้:

```json
{
  "items": [
    {
      "id": "019a3cf8-96f0-7c9f-b207-93aa818f4b10",
      "code": "CUS-019A3CF896F0",
      "customerType": "organization",
      "displayNameTh": "บริษัท ตัวอย่าง จำกัด TEST_ONLY",
      "displayNameEn": null,
      "preferredLocale": "th",
      "status": "draft",
      "primaryContact": {
        "name": "คุณตัวอย่าง TEST_ONLY",
        "phone": "+66******123",
        "email": "t***@example.test",
        "isMasked": true
      }
    }
  ],
  "nextCursor": null
}
```

Create response ใช้ Customer shape เดียวกับ Detail เพิ่ม `duplicateCandidates` และส่ง `ETag` response header; เมื่อมี `customer-contacts.manage` ให้ `primaryContact.isMasked=false` และคืนค่าเต็ม

- [ ] **Step 3: Add stable errors and HTTP mapping**

เพิ่มรายการต่อไปนี้ใน `error-contract.md` และกำหนดข้อความ `.resx` ที่จะใช้ใน Task 7:

```text
MEMBERSHIP_CONTEXT_REQUIRED   400  ไม่มี X-Membership-Id หรือรูปแบบไม่ใช่ UUID
CUSTOMER_CURSOR_INVALID       400  cursor ถอดรหัสหรือ validate ไม่ได้
RESOURCE_NOT_FOUND            404  ไม่พบ Customer หรืออยู่นอก Organization scope
CONTACT_FIELD_REQUIRED        422  ต้องมี phone หรือ email อย่างน้อยหนึ่งค่า
CUSTOMER_FIELD_REQUIRED       422  customerType/displayNameTh/preferredLocale ไม่ผ่านกฎ
IDEMPOTENCY_KEY_REUSED        409  key เดิมถูกใช้กับ payload คนละชุด
```

- [ ] **Step 4: Define the executable UAT subset**

เพิ่ม `Customer + Contact Slice 1 Exit Criteria` โดยบังคับผ่าน `UAT-CRM-001` เฉพาะ Create Draft/ค้นหา, `UAT-CRM-002`, `UAT-SEC-001`, `UAT-SEC-002`, `UAT-UX-001`, `UAT-I18N-001` และกรณี retry Idempotency; ระบุว่า Activate portion ของ `UAT-CRM-001` อยู่ Slice ถัดไป

- [ ] **Step 5: Verify documentation and commit**

Run:

```bash
rg -n "X-Membership-Id|Idempotency-Key|organization only|MEMBERSHIP_CONTEXT_REQUIRED|Customer \+ Contact Slice 1" docs
git diff --check -- docs
```

Expected: พบ contract/gate ครบ, ไม่มีข้อความค้างให้ตัดสินภายหลัง และ `git diff --check` exit `0`

```bash
git add docs/03-contracts/crm-site-survey-api-contract.md docs/03-contracts/error-contract.md docs/05-engineering/crm-site-survey-uat-scenarios.md docs/05-engineering/foundation-login-verification.md docs/README.md
git commit -m "docs(crm): approve customer contact slice contract"
```

### Task 2: Add the Reusable Request Access Boundary

**Files:**
- Create: `backend/src/TanErp.Application/Common/Models/RequestAccessContext.cs`
- Create: `backend/src/TanErp.Application/Common/Abstractions/IRequestAccessResolver.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/RequestAccessResolver.cs`
- Create: `backend/src/TanErp.Api/RequestContext/RequestContextReader.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/RequestAccessResolverTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`

**Interfaces:**
- Consumes: Firebase UID claim, `X-Membership-Id`, active Membership/Organization/User และ RolePermission tables
- Produces: `Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string permissionKey, CancellationToken)`

- [ ] **Step 1: Write failing resolver tests**

ทดสอบ exact cases: active Organization permission สำเร็จ; missing permission คืน `PERMISSION_DENIED`; inactive membership คืน `ACTIVE_MEMBERSHIP_REQUIRED`; membership ของ UID อื่นคืน `ACTIVE_MEMBERSHIP_REQUIRED`; branch/own scope คืน `PERMISSION_DENIED`

```csharp
var result = await resolver.ResolveAsync("uid-active", membershipId, "customers.read", cancellationToken);
Assert.True(result.IsSuccess);
Assert.Equal(userId, result.Value!.ActorUserId);
Assert.Equal(organizationId, result.Value.OrganizationId);
Assert.Equal(membershipId, result.Value.MembershipId);
```

- [ ] **Step 2: Run tests and confirm RED**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter RequestAccessResolverTests`

Expected: FAIL เพราะ interface/resolver ยังไม่มี

- [ ] **Step 3: Add the trusted access model and resolver port**

```csharp
public sealed record RequestAccessContext(
    Guid ActorUserId,
    Guid MembershipId,
    Guid OrganizationId,
    Guid? BranchId,
    string PermissionKey,
    string PermissionScope);

public interface IRequestAccessResolver
{
    Task<Result<RequestAccessContext>> ResolveAsync(
        string firebaseUid,
        Guid membershipId,
        string permissionKey,
        CancellationToken cancellationToken = default);
}
```

- [ ] **Step 4: Implement the Infrastructure resolver**

Query active User → exact Membership → active Organization/Branch → active Roles/Permissions; require `RolePermission.Scope == "organization"` and `ScopeId == Membership.OrganizationId`; project only the six fields in `RequestAccessContext` and never return role names

```csharp
var now = _clock.UtcNow;
var context = await _db.Memberships
    .AsNoTracking()
    .Where(m => m.Id == membershipId && m.User!.FirebaseUid == firebaseUid)
    .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
    .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
    .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
    .SelectMany(m => m.MembershipRoles
        .Where(mr => mr.Role!.IsActive)
        .SelectMany(mr => mr.Role!.RolePermissions
            .Where(rp => rp.Permission!.IsActive
                && rp.Permission.Key == permissionKey
                && rp.Scope == PermissionScope.Organization
                && rp.ScopeId == m.OrganizationId)
            .Select(rp => new RequestAccessContext(
                m.UserId, m.Id, m.OrganizationId, m.BranchId,
                rp.Permission!.Key, rp.Scope))))
    .FirstOrDefaultAsync(cancellationToken);
```

ถ้าไม่พบ ให้ query แยกเฉพาะ active membership เพื่อแยก `ACTIVE_MEMBERSHIP_REQUIRED` จาก `PERMISSION_DENIED` โดยไม่เปิดเผย Membership ของ UID อื่น

- [ ] **Step 5: Add one HTTP header parser**

`RequestContextReader` ต้องคืน `MEMBERSHIP_CONTEXT_REQUIRED` เมื่อ header ไม่มี/ซ้ำ/ไม่ใช่ UUID และอ่าน Firebase UID จาก `firebase_uid` หรือ `NameIdentifier`; raw header ห้ามถูก log

```csharp
public sealed record AuthenticatedRequest(string FirebaseUid, Guid MembershipId);
public sealed record IdempotentRequest(string FirebaseUid, Guid MembershipId, string IdempotencyKey);
```

- [ ] **Step 6: Register, run tests, and commit**

Run:

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter RequestAccessResolverTests
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter MembershipContext
```

Expected: PASS; missing/invalid header เป็น 400 Problem Details และ wrong membership ไม่เปิดเผย Organization

```bash
git add backend/src backend/tests/TanErp.UnitTests backend/tests/TanErp.IntegrationTests
git commit -m "feat(authz): add membership request context"
```

### Task 3: Build the Customer Aggregate Test-first

**Files:**
- Create: `backend/src/TanErp.Domain/Crm/Customers/CustomerValues.cs`
- Create: `backend/src/TanErp.Domain/Crm/Customers/Customer.cs`
- Create: `backend/src/TanErp.Domain/Crm/Customers/CustomerContact.cs`
- Create: `backend/src/TanErp.Domain/Common/IdempotencyRecord.cs`
- Test: `backend/tests/TanErp.UnitTests/Crm/Customers/CustomerTests.cs`

**Interfaces:**
- Consumes: Organization/Actor IDs, `IClock` timestamp and validated create fields
- Produces: `Customer.CreateDraft(...)`, exactly one `PrimaryContact`, normalization keys, `RowVersion`, server-generated code

- [ ] **Step 1: Write failing aggregate tests**

ครอบคลุม: valid organization/person customer; blank Thai name; invalid locale/type/channel; contact ไม่มี phone/email; trim/normalize; generated `CUS-XXXXXXXXXXXX`; one primary contact; PII ไม่ปรากฏใน `ToString()`

```csharp
var customer = Customer.CreateDraft(
    customerId,
    organizationId,
    actorUserId,
    "organization",
    "  บริษัท ตัวอย่าง จำกัด TEST_ONLY  ",
    null,
    "th",
    new PrimaryContactInput("คุณตัวอย่าง TEST_ONLY", null, "+66 81 234 5678", null, "phone"),
    now);

Assert.Equal("draft", customer.Status);
Assert.Equal("CUS-" + customerId.ToString("N")[..12].ToUpperInvariant(), customer.Code);
Assert.Single(customer.Contacts);
Assert.True(customer.Contacts.Single().IsPrimary);
Assert.Equal("66812345678", customer.Contacts.Single().NormalizedPhone);
```

- [ ] **Step 2: Run and confirm RED**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter CustomerTests`

Expected: FAIL เพราะ CRM domain types ยังไม่มี

- [ ] **Step 3: Implement minimal domain invariants**

ใช้ constant sets `person|organization`, `draft|active|inactive`, `th|en`, `phone|email|line|other`; normalize whitespace ด้วยการ split/join, phone เก็บ digits สำหรับ match และ email ใช้ `Trim().ToLowerInvariant()`; Contact ต้องมี phone/email อย่างน้อยหนึ่งค่า

```csharp
public static Customer CreateDraft(
    Guid id,
    Guid organizationId,
    Guid actorUserId,
    string customerType,
    string displayNameTh,
    string? displayNameEn,
    string preferredLocale,
    PrimaryContactInput primaryContact,
    DateTimeOffset now)
```

สร้าง `CustomerContact` ภายใน factory เท่านั้นใน Slice นี้; `RowVersion` เป็น Guid ใหม่และมีไว้ส่ง ETag แม้ Update จะยัง Deferred

- [ ] **Step 4: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter CustomerTests`

Expected: PASS

```bash
git add backend/src/TanErp.Domain backend/tests/TanErp.UnitTests/Crm
git commit -m "feat(crm): add customer contact aggregate"
```

### Task 4: Persist Customer, Contact, Idempotency and Database Guards

**Files:**
- Modify: `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CustomerConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CustomerContactConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/IdempotencyRecordConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Migrations/` (EF-generated `CustomerContactSlice` migration and designer)
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/CustomerContactMigrationTests.cs`

**Interfaces:**
- Consumes: Customer aggregate and existing Organization/User/Audit tables
- Produces: `crm.customers`, `crm.customer_contacts`, `audit.idempotency_records` with DB-enforced tenant/contact/idempotency constraints; no EF Core type crosses into Application

- [ ] **Step 1: Write failing PostgreSQL migration tests**

ทดสอบ migration จาก zero, rollback/reapply และ negative inserts:

```text
unique customers (organization_id, code)
customer organization FK
contact composite FK (customer_id, organization_id)
contact CHECK phone/email อย่างน้อยหนึ่งค่า
partial unique primary contact WHERE is_primary AND status='active'
unique idempotency (organization_id, operation, key_hash)
idempotency payload_hash/resource_id ไม่ว่าง
```

- [ ] **Step 2: Run and confirm RED**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter CustomerContactMigrationTests`

Expected: FAIL เพราะ tables/configurations ยังไม่มี

- [ ] **Step 3: Configure exact relational mapping**

สร้าง schema `crm`; ใช้ snake_case columns; จำกัดความยาวตาม Field Catalog; index `(organization_id,status,normalized_display_name,id)`; Customer alternate key `(id,organization_id)`; contact FK ใช้ `(customer_id,organization_id)`; delete behavior `Restrict`; map `row_version` เป็น UUID concurrency token

- [ ] **Step 4: Generate the EF migration**

Run:

```bash
dotnet ef migrations add CustomerContactSlice --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations
```

Expected: migration มีเฉพาะสาม tables/indexes/constraints ของ Task นี้และ snapshot diff สอดคล้องกัน

- [ ] **Step 5: Run migration tests and commit**

Run:

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter CustomerContactMigrationTests
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
```

Expected: PASS รวม cross-organization negative cases และ rollback/reapply

```bash
git add backend/src/TanErp.Infrastructure backend/tests/TanErp.IntegrationTests/Persistence
git commit -m "feat(crm): persist customer contact slice"
```

### Task 5: Implement Idempotent Atomic Customer Creation

**Files:**
- Create: `backend/src/TanErp.Application/Crm/Customers/CreateCustomer/CreateCustomerCommand.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/CreateCustomer/CreateCustomerHandler.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/CreateCustomer/CreateCustomerResult.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/CustomerProjection.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/ICustomerCreationStore.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Crm/CustomerCreationStore.cs`
- Test: `backend/tests/TanErp.UnitTests/Crm/Customers/CreateCustomerHandlerTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`

**Interfaces:**
- Consumes: `IRequestAccessResolver`, `ICustomerCreationStore`, `IClock`, Firebase UID, Membership ID, Idempotency Key and create fields
- Produces: `Result<CreateCustomerResult>` with Customer, primary contact, masked duplicate candidates and ETag source token

กำหนด persistence port ให้รับข้อมูลที่ผ่าน validation แล้วและ encapsulate EF transaction/race recovery ไว้ใน Infrastructure:

```csharp
public sealed record PersistCustomerCreation(
    Customer Customer,
    string Operation,
    string IdempotencyKeyHash,
    string PayloadHash,
    IReadOnlyList<AuditEvent> AuditEvents);

public sealed record PersistCustomerCreationResult(
    CustomerProjection Customer,
    IReadOnlyList<DuplicateCustomerProjection> DuplicateCandidates,
    bool WasReplayed);

public interface ICustomerCreationStore
{
    Task<Result<PersistCustomerCreationResult>> CreateAsync(
        PersistCustomerCreation request,
        CancellationToken cancellationToken = default);
}
```

- [ ] **Step 1: Write failing handler tests**

ทดสอบ permission pair, validation, atomic audit, PII-safe audit, exact duplicate match, same-key retry และ changed-payload conflict

```csharp
var command = new CreateCustomerCommand(
    "uid-active", membershipId, "request-key-0001",
    "organization", "บริษัท ตัวอย่าง จำกัด TEST_ONLY", null, "th",
    new CreatePrimaryContact("คุณตัวอย่าง TEST_ONLY", null, "+66812345678", null, "phone"),
    traceId);
var result = await handler.Handle(command, cancellationToken);
Assert.True(result.IsSuccess);
Assert.Equal("draft", result.Value!.Customer.Status);
```

- [ ] **Step 2: Run and confirm RED**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter CreateCustomerHandlerTests`

Expected: FAIL เพราะ command/handler ยังไม่มี

- [ ] **Step 3: Implement deterministic request hashing**

Canonicalize field order and normalized values, hash payload and raw idempotency key แยกกันด้วย SHA-256 hex; persist only hashes Raw key, phone and email ห้ามอยู่ใน `IdempotencyRecord` หรือ log

- [ ] **Step 4: Implement the transaction flow**

```text
Resolve customers.create at organization scope
Resolve customer-contacts.manage for same membership
Validate command and normalize values
Call ICustomerCreationStore once; Infrastructure opens an EF transaction
Lookup idempotency by organization + operation + keyHash
  same payloadHash → reload and return existing Customer
  different payloadHash → IDEMPOTENCY_KEY_REUSED
Find duplicate candidates inside organization only
Create Customer aggregate + IdempotencyRecord
Add customer.created + contact.created AuditEvent with IDs and changed field names only
SaveChangesAsync once and commit; unique-key race reloads the winning record and compares payloadHash
Return created projection and duplicate candidates
```

`ChangesJson` ใช้ payload คงที่ต่อไปนี้และห้ามเพิ่มค่าของ field:

```json
{"changedFields":["customerType","displayNameTh","displayNameEn","preferredLocale","primaryContact"]}
```

- [ ] **Step 5: Prove atomicity and retry behavior against PostgreSQL**

Integration test บังคับ constraint failure หลังสร้าง object แล้วตรวจว่า Customer, Contact, IdempotencyRecord และ AuditEvent ไม่เหลือบางส่วน; ส่ง key/payload เดิมสองครั้งต้องมี Customer หนึ่งรายการ, Contact หนึ่งรายการ, Audit สองรายการ

- [ ] **Step 6: Run tests and commit**

Run:

```bash
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter CreateCustomerHandlerTests
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter CreateCustomer
```

Expected: PASS และ test assertions ยืนยันว่า serialized log/audit ไม่มี phone/email

```bash
git add backend/src/TanErp.Application/Crm backend/tests/TanErp.UnitTests/Crm backend/tests/TanErp.IntegrationTests/Api
git commit -m "feat(crm): create customer with primary contact"
```

### Task 6: Add Scoped List and Detail Queries

**Files:**
- Create: `backend/src/TanErp.Application/Crm/Customers/ListCustomers/ListCustomersQuery.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/ListCustomers/ListCustomersHandler.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/ListCustomers/ListCustomersResult.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/GetCustomer/GetCustomerQuery.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/GetCustomer/GetCustomerHandler.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/GetCustomer/GetCustomerResult.cs`
- Create: `backend/src/TanErp.Application/Crm/Customers/ICustomerReadStore.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Crm/CustomerReadStore.cs`
- Test: `backend/tests/TanErp.UnitTests/Crm/Customers/CustomerQueryHandlerTests.cs`

**Interfaces:**
- Consumes: `customers.read`, optional `customer-contacts.manage`, selected Membership, search/status/limit/cursor and `ICustomerReadStore`
- Produces: deterministic page and scoped detail; full or masked contact projection

```csharp
public interface ICustomerReadStore
{
    Task<CustomerPage> ListAsync(
        Guid organizationId,
        CustomerListFilter filter,
        bool includeContactPii,
        CancellationToken cancellationToken = default);

    Task<CustomerProjection?> GetAsync(
        Guid organizationId,
        Guid customerId,
        bool includeContactPii,
        CancellationToken cancellationToken = default);
}
```

- [ ] **Step 1: Write failing query tests**

ครอบคลุม empty list, normalized Thai search, status filter, stable tie-breaker, next cursor, malformed cursor, Organization isolation, resource outside scope 404 และ masked/full contact projection

- [ ] **Step 2: Run and confirm RED**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter CustomerQueryHandlerTests`

Expected: FAIL เพราะ query handlers ยังไม่มี

- [ ] **Step 3: Implement opaque cursor and stable query**

Cursor encode UTF-8 JSON `{normalizedDisplayName,id}` เป็น Base64Url; decode failure คืน `CUSTOMER_CURSOR_INVALID`; query ต้องเริ่มด้วย `OrganizationId == access.OrganizationId`, sort `NormalizedDisplayName`, `Id`, take `limit + 1`

- [ ] **Step 4: Implement masking without mutating stored values**

Phone แสดง country/prefix ที่จำเป็นและ 3 ตัวท้าย, email แสดงอักษรแรก + `***` + domain; ถ้าไม่มี `customer-contacts.manage` ค่า raw อาจถูกอ่านภายใน Infrastructure เพื่อ mask เท่านั้น แต่ห้ามออกจาก `ICustomerReadStore`, log หรือ Application projection

- [ ] **Step 5: Run tests and commit**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter CustomerQueryHandlerTests`

Expected: PASS

```bash
git add backend/src/TanErp.Application/Crm backend/tests/TanErp.UnitTests/Crm
git commit -m "feat(crm): query scoped customers"
```

### Task 7: Publish the Customer HTTP and OpenAPI Contract

**Files:**
- Create: `backend/src/TanErp.Api/Contracts/Crm/Customers/CreateCustomerRequest.cs`
- Create: `backend/src/TanErp.Api/Contracts/Crm/Customers/CustomerResponse.cs`
- Create: `backend/src/TanErp.Api/Contracts/Crm/Customers/CustomerListResponse.cs`
- Create: `backend/src/TanErp.Api/Controllers/CustomersController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Modify: `backend/src/TanErp.Api/Resources/Errors.resx`
- Modify: `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Modify: `contracts/openapi/tan-erp.v1.json`
- Test: `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/OpenApiContractTests.cs`

**Interfaces:**
- Consumes: Task 2 request context and Tasks 5–6 handlers
- Produces: documented endpoints and generated schemas with RFC 9457 errors

- [ ] **Step 1: Write failing endpoint tests**

ทดสอบ 201 + Location + ETag, 200 list/detail, 400 missing membership/idempotency, 401 token, 403 permission, 404 foreign Organization, 409 reused key, 422 field validation, th/en/fallback-th messages และ no PII in Problem Details

- [ ] **Step 2: Run and confirm RED**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter CustomerEndpointsTests`

Expected: FAIL เพราะ controller/contracts ยังไม่มี

- [ ] **Step 3: Add thin controller actions**

Controller ทำเพียง parse route/query/header, สร้าง command/query, เรียก handler และ map Result; ห้าม query DbContext หรือเช็ก role name

```csharp
[HttpPost("/api/v1/customers")]
[Authorize]
[ProducesResponseType<CustomerResponse>(StatusCodes.Status201Created)]
public async Task<ActionResult<CustomerResponse>> Create(
    [FromBody] CreateCustomerRequest request,
    CancellationToken cancellationToken)
```

ส่ง `Location: /api/v1/customers/{id}` และ `ETag: "{rowVersion}"`

- [ ] **Step 4: Add localized mappings**

เพิ่ม exact status mappings จาก Task 1; error resource ไทย/อังกฤษต้องมี key `_TITLE` และ `_DETAIL` ครบคู่ และ unknown code ยังคง fallback `INTERNAL_SERVER_ERROR`

- [ ] **Step 5: Generate and freeze OpenAPI**

Run:

```bash
dotnet build backend/TanErp.slnx
npm --prefix frontend run generate:api
npm --prefix frontend run check:api
```

Expected: OpenAPI JSON มีสาม endpoints, headers, request/response/error schemas และ generated TS เปลี่ยนจาก generator เท่านั้น

- [ ] **Step 6: Run tests and commit**

Run:

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "CustomerEndpointsTests|OpenApiContractTests"
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
git diff --check
```

Expected: PASS / exit `0`

```bash
git add backend/src/TanErp.Api backend/tests/TanErp.IntegrationTests contracts/openapi frontend/src/generated/api
git commit -m "feat(api): publish customer contact endpoints"
```

### Task 8: Seed Test Permissions and Prove Tenant Isolation

**Files:**
- Modify: `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Persistence/CustomerContactMigrationTests.cs`

**Interfaces:**
- Consumes: `customers.read`, `customers.create`, `customer-contacts.manage`
- Produces: deterministic two-Organization `TEST_ONLY` acceptance fixture; no production Customer seed

- [ ] **Step 1: Add deterministic permission fixtures**

Seed permission catalog entries only when environment is `Test`; assign Organization-scope permissions to the existing test role and create a second Organization/User/Membership with separate IDs

- [ ] **Step 2: Add adversarial integration cases**

```text
Org A list never returns Org B Customer
Org A GET Org B customerId returns RESOURCE_NOT_FOUND
Org A duplicate search never reports Org B contact/name
RolePermission branch/own does not authorize Slice 1
users with customers.read but no customer-contacts.manage receive masked projection
logs and Problem Details do not contain seeded phone/email
```

- [ ] **Step 3: Run and commit**

Run:

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "CustomerEndpointsTests|CustomerContactMigrationTests"
```

Expected: PASS

```bash
git add backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs backend/tests/TanErp.IntegrationTests
git commit -m "test(crm): prove customer tenant isolation"
```

### Task 9: Add Frontend Membership and API Reuse Seams

**Files:**
- Modify: `frontend/package.json`
- Modify: `frontend/package-lock.json`
- Create: `frontend/src/lib/membership/selected-membership-context.tsx`
- Create: `frontend/src/lib/permissions/can.ts`
- Create: `frontend/src/lib/permissions/can.test.ts`
- Modify: `frontend/src/lib/api/api-client.ts`
- Modify: `frontend/src/lib/api/api-client.test.ts`
- Modify: `frontend/src/components/ui/Input.tsx`
- Create: `frontend/src/components/ui/Input.test.tsx`

**Interfaces:**
- Consumes: generated `CurrentUserResponse` memberships/permissions, Firebase token, locale
- Produces: `useSelectedMembership()`, `can(permissionKey)`, reusable `ApiClient.request<T>()` with membership/idempotency headers

- [ ] **Step 1: Install exact form dependencies**

Run:

```bash
npm --prefix frontend install --save-exact react-hook-form@7.87.0 @hookform/resolvers@5.9.1
```

Expected: package and lock files contain exact versions; no unrelated dependency upgrades

- [ ] **Step 2: Write failing context, permission and client tests**

ทดสอบ default selected membership เป็นรายการแรก, explicit switch, business query cache removal on switch, permission organization scope, branch/own fail-closed, request headers และ Problem Details parsing

```ts
expect(can(membership, "customers.read")).toBe(true);
expect(can(branchScopedMembership, "customers.read")).toBe(false);
```

- [ ] **Step 3: Implement selected membership context**

Provider รับ memberships จาก AccessGate, เก็บเฉพาะ selected membership ID, derive object ระหว่าง render และเมื่อต้อง switch ให้ cancel/remove queries whose key begins `business`; ห้าม copy server membership object เข้า local state

- [ ] **Step 4: Deepen the central API client**

สร้าง private `request<T>` ที่แนบ Authorization, Accept-Language, Accept, optional `X-Membership-Id`, optional `Idempotency-Key`; GET Customer methods และ POST Customer method ใช้ generated schemas เท่านั้น; `fetch` ต้องอยู่ใน ApiClient ไฟล์เดียว

- [ ] **Step 5: Correct shared Input accessibility**

`Input` ต้องตั้ง `aria-invalid={Boolean(error)}`, `aria-describedby` ให้ชี้ error/helper ID, `name` ส่งผ่านจาก Controller, `autocomplete="name|tel|email"` กำหนดจาก form field และ error แสดงหลัง touched/submit จาก React Hook Form ไม่แสดงทันทีตอน mount

- [ ] **Step 6: Run tests and commit**

Run:

```bash
npm --prefix frontend run test -- src/lib/api/api-client.test.ts src/lib/permissions/can.test.ts src/components/ui/Input.test.tsx
npm --prefix frontend run typecheck
```

Expected: PASS

```bash
git add frontend/package.json frontend/package-lock.json frontend/src/lib frontend/src/components/ui/Input.tsx frontend/src/components/ui/Input.test.tsx
git commit -m "feat(frontend): add business request context"
```

### Task 10: Compose the ERP Layout and Customer Navigation

**Files:**
- Create: `frontend/src/app/[locale]/(erp)/layout.tsx`
- Modify: `frontend/src/app/[locale]/(erp)/page.tsx`
- Modify: `frontend/src/components/layout/erp-shell.tsx`
- Modify: `frontend/src/components/layout/erp-shell.test.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`

**Interfaces:**
- Consumes: AccessGate and selected membership provider
- Produces: one shared authenticated ERP shell with `children` slot and permission-aware Customer link

- [ ] **Step 1: Write failing shell/layout tests**

ทดสอบ Customer nav แสดงเมื่อ `can(customers.read)`, ไม่แสดงเมื่อไม่มีสิทธิ์, selector แสดงเมื่อ memberships มากกว่าหนึ่ง, switch คืน focus และ child content render ภายใน `<main>`

- [ ] **Step 2: Refactor by composition**

`(erp)/layout.tsx` validate locale แล้ว compose:

```tsx
<AccessGate>
  {(currentUser) => (
    <SelectedMembershipProvider memberships={currentUser.memberships ?? []}>
      <ErpShell currentUser={currentUser}>{children}</ErpShell>
    </SelectedMembershipProvider>
  )}
</AccessGate>
```

หน้า home คืนเฉพาะ dashboard welcome content; `ErpShell` รับ `children: React.ReactNode`; nav text และ accessible labels มาจาก `shell` message keys ทั้ง th/en

- [ ] **Step 3: Run tests and commit**

Run:

```bash
npm --prefix frontend run test -- src/components/layout/erp-shell.test.tsx
npm --prefix frontend run typecheck
```

Expected: PASS

```bash
git add frontend/src/app frontend/src/components/layout frontend/src/messages
git commit -m "feat(shell): add customer workspace navigation"
```

### Task 11: Build the Customer Query Layer and List Page

**Files:**
- Create: `frontend/src/features/customers/api/customer-queries.ts`
- Create: `frontend/src/features/customers/api/customer-queries.test.tsx`
- Create: `frontend/src/features/customers/components/customer-list.tsx`
- Create: `frontend/src/features/customers/components/customer-list.test.tsx`
- Create: `frontend/src/features/customers/index.ts`
- Create: `frontend/src/app/[locale]/(erp)/customers/page.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`
- Modify: `frontend/src/styles/erp-theme.css`

**Interfaces:**
- Consumes: `apiClient.listCustomers`, selected membership, locale and generated response type
- Produces: Customer list route with loading/empty/error/data states and Create action

- [ ] **Step 1: Write failing query/list tests**

ทดสอบ query key `['business',membershipId,'customers',filters,locale]`, AbortSignal forwarding, no request without membership, loading MonoSpinner, empty explanation, retryable error + trace ID, table data, masked indicator และ permission-hidden Create button

- [ ] **Step 2: Implement query hooks**

ใช้ `useQuery`; filter values อยู่ URL search params; server data ห้าม copy เข้า state; search form เป็น semantic GET; debounce ไม่จำเป็นใน Slice นี้—submit แล้วเปลี่ยน URL เท่านั้น

- [ ] **Step 3: Implement responsive list UI**

Desktop/tablet ใช้ semantic table, mobile ใช้ CSS responsive row presentation จาก markup เดียว; rows/links สูงอย่างน้อย 44px; no rounded cards; Empty/Error มี `aria-live="polite"`; Create link ไป `/${locale}/customers/create`

- [ ] **Step 4: Run tests and commit**

Run:

```bash
npm --prefix frontend run test -- src/features/customers
npm --prefix frontend run lint
npm --prefix frontend run typecheck
```

Expected: PASS

```bash
git add frontend/src/features/customers frontend/src/app/'[locale]'/'(erp)'/customers frontend/src/messages frontend/src/styles/erp-theme.css
git commit -m "feat(customers): add customer list"
```

### Task 12: Build the Single Dynamic Create/Detail Route

**Files:**
- Create: `frontend/src/features/customers/schemas/customer-form-schema.ts`
- Create: `frontend/src/features/customers/components/customer-editor.tsx`
- Create: `frontend/src/features/customers/components/customer-editor.test.tsx`
- Create: `frontend/src/features/customers/components/customer-detail.tsx`
- Create: `frontend/src/features/customers/components/customer-detail.test.tsx`
- Create: `frontend/src/app/[locale]/(erp)/customers/[id]/page.tsx`
- Modify: `frontend/src/features/customers/api/customer-queries.ts`
- Modify: `frontend/src/features/customers/index.ts`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`
- Modify: `frontend/src/styles/erp-theme.css`

**Interfaces:**
- Consumes: route ID (`create` or UUID), React Hook Form, Zod, generated create request, selected membership and Customer queries
- Produces: accessible Create flow and read-only Detail view; Update remains unavailable

- [ ] **Step 1: Write failing localized schema tests**

สร้าง schema factory `createCustomerFormSchema(t)` และทดสอบ type, Thai name, locale, contact name, phone-or-email refinement, email format; inferred type ต้องมาจาก `z.infer`

- [ ] **Step 2: Write failing editor/detail component tests**

ทดสอบ labels, required indicators, blur/submit timing, focus first invalid control, `aria-invalid`, phone/email autocomplete/inputMode, submit lock, one network call on double click, idempotency key stable across retry, navigation to detail, detail loading/error/404/data และไม่มี Update/Delete controls

- [ ] **Step 3: Implement the schema and form composition**

ใช้ `FormProvider` + `Controller` กับทุก Input/custom radio; customerType มี 2 options จึงใช้ radio; preferredLocale มี 2 options จึงใช้ radio; form เป็น one-column บน mobile และ two-column เมื่อพื้นที่พอ

```ts
const methods = useForm<CustomerFormValues>({
  resolver: zodResolver(createCustomerFormSchema(tValidation)),
  mode: "onBlur",
  reValidateMode: "onChange",
  defaultValues: {
    customerType: "organization",
    displayNameTh: "",
    displayNameEn: "",
    preferredLocale: "th",
    primaryContact: { name: "", roleTitle: "", phone: "", email: "", preferredChannel: "phone" },
  },
});
```

- [ ] **Step 4: Implement idempotent submission**

สร้าง idempotency key ตอน valid submit ครั้งแรกด้วย `crypto.randomUUID()` และเก็บใน ref เพื่อให้ retry intent เดิมใช้ key เดิม; เมื่อผู้ใช้แก้ field หลัง failed response ให้ clear ref เพื่อสร้าง intent ใหม่; button ใช้ `isLoading={isSubmitting || mutation.isPending}` และ disable หลัง validation ผ่านเท่านั้น

- [ ] **Step 5: Implement route mode resolution**

`id === "create"` render editor; UUID render detail; ค่าอื่นเรียก `notFound()` หน้า Detail ใช้ Minimal Mono Loading และ query key รวม membership/locale/customer ID

- [ ] **Step 6: Verify translations, tests and commit**

Run:

```bash
npm --prefix frontend run test -- src/features/customers
npm --prefix frontend run typecheck
npm --prefix frontend run lint
```

Expected: PASS; th/en message key trees เท่ากัน และไม่มี `any`, `as any`, `@ts-ignore`

```bash
git add frontend/src/features/customers frontend/src/app/'[locale]'/'(erp)'/customers/'[id]' frontend/src/messages frontend/src/styles/erp-theme.css
git commit -m "feat(customers): add create and detail workflow"
```

### Task 13: Run the Unmocked Acceptance Journey and Publish Evidence

**Files:**
- Create: `frontend/e2e/customer-contact.spec.ts`
- Create: `docs/05-engineering/customer-contact-verification.md`
- Modify: `docs/README.md`

**Interfaces:**
- Consumes: PostgreSQL, Firebase emulator, API, Frontend and `TEST_ONLY` two-Organization fixture
- Produces: evidence-backed Slice completion record and next-slice gate

- [ ] **Step 1: Write the Playwright journey**

ทดสอบ Login → Customer empty/list → Create form → validation → create → detail → refresh → search; capture response assertions for 201/ETag and verify double click creates one record ใช้เฉพาะ `TEST_ONLY` values

- [ ] **Step 2: Add direct security acceptance calls**

ใน integration/E2E setup ใช้ Org B membership request Customer ID ของ Org A แล้ว assert 404 + no PII; user read-only sees masked contact; same idempotency key/payload returns same ID; changed payload returns 409

- [ ] **Step 3: Run all verification gates**

Run:

```bash
dotnet build backend/TanErp.slnx
dotnet test backend/TanErp.slnx
npm --prefix frontend run verify
npm run test:fixtures
PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e
git diff --check
```

Expected: ทุกคำสั่ง exit `0`; generated OpenAPI ไม่มี drift; E2E ใช้ API/PostgreSQL/Firebase emulator จริง

- [ ] **Step 4: Perform manual UX checks**

ตรวจไทย/อังกฤษ, keyboard-only, visible focus, 320px, tablet, desktop, zoom 200%, screen-reader labels, no color-only errors, 44px targets, submit lock และไม่มี Skeleton/rounded UI

- [ ] **Step 5: Scan for forbidden patterns and sensitive data**

Run:

```bash
rg -n "\bany\b|as any|@ts-ignore|BEGIN PRIVATE KEY|firebase-admin" frontend/src frontend/e2e
rg -n "phone|email|Authorization|Idempotency-Key" backend/src/TanErp.Api backend/src/TanErp.Application backend/src/TanErp.Infrastructure
```

Expected: ไม่มี forbidden TypeScript; ผล PII scan ทุกตำแหน่งเป็น contract/field handling ที่ไม่ log raw values

- [ ] **Step 6: Write the verification record**

บันทึก runtime จริง, commands, counts, exit codes, migration name, UAT IDs, security evidence, known deferred scope และระบุ Next Gate ว่า Opportunity + Site ยังเริ่มไม่ได้จนมีแผน/contract ของตนเอง ห้ามเขียนว่า Passed หาก Step 3–5 ยังไม่ผ่านจริง

- [ ] **Step 7: Commit evidence**

```bash
git add frontend/e2e/customer-contact.spec.ts docs/05-engineering/customer-contact-verification.md docs/README.md
git commit -m "test(crm): verify customer contact slice"
```

## Definition of Success

แผนนี้สำเร็จเมื่อทุกข้อเป็นจริงพร้อมกัน:

1. ผู้ใช้ Login แล้วเลือก Active Membership และเปิด Customer list ได้ตาม `customers.read`
2. ผู้มี `customers.create` + `customer-contacts.manage` สร้าง Customer Draft + Primary Contact ได้จาก UI
3. Customer, Contact, IdempotencyRecord และสอง Audit Events commit แบบ atomic
4. Retry intent เดิมไม่สร้างข้อมูลซ้ำ; reused key กับ payload ใหม่ได้ `409`
5. List/detail จำกัด Organization ที่ derive จาก Membership; cross-organization ได้ `404`
6. Contact PII ถูก mask เมื่อไม่มี permission และไม่ปรากฏใน log, audit หรือ Problem Details
7. OpenAPI → generated frontend types ไม่มี drift; Frontend ไม่มี handwritten API DTO
8. Loading/empty/error/validation/success states ใช้งานได้ทั้ง th/en, keyboard, 320px และ zoom 200%
9. Backend build/test, Frontend verify, fixture tests, real E2E และ `git diff --check` ผ่านทั้งหมด
10. Verification record มีหลักฐานคำสั่งและผลจริง โดย Scope ถัดไปยังคงถูก Gate

## Explicitly Deferred

```text
Customer update and ETag conflict UI
Customer activate/deactivate
Additional contacts and contact editing
Address, Site and Opportunity
Branch/Own Customer visibility model
Fuzzy duplicate scoring and duplicate review/merge
Tax/private identifiers, encryption, export, retention and redaction workflows
Bulk operations, Dapper search optimization and background jobs
```
