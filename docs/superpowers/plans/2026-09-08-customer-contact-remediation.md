# Customer + Contact Vertical Slice Remediation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ปิด findings จาก code review ของ Customer + Contact Vertical Slice ให้ Create/List/Detail ผ่าน authorization, contract, cache isolation, localization และ evidence gates ก่อนเริ่ม Slice ถัดไป

**Architecture:** แก้จาก security boundary ออกมาหา UI: เพิ่มสถานะ active ให้ Permission และบังคับ active Branch/Permission ใน resolver, จากนั้นบังคับ Create ให้มีสิทธิ์ทั้งสองรายการ ฝั่ง Frontend ใช้ generated OpenAPI types เป็น source of truth, business query keys ที่รวม Membership/locale/filter, schema ที่ตรง Domain และ create intent ที่จัดการ idempotency/duplicate result อย่างชัดเจน ปิดท้ายด้วย acceptance tests และเขียน verification record จากผลรันจริงเท่านั้น

**Tech Stack:** .NET SDK `10.0.400`, ASP.NET Core/EF Core `10.0.11`, PostgreSQL `17-alpine`, Next.js `16.3.4`, React `19.2.8`, TypeScript `7.0.2`, TanStack Query `5.102.8`, React Hook Form `7.87.0`, Zod `4.5.4`, Vitest `5.0.0`, Playwright `1.63.0`

## Global Constraints

- Fixed point ของ remediation คือ `origin/main`; แก้เฉพาะ findings ของ Customer + Contact Vertical Slice และ regression ที่เกิดจากการแก้เหล่านี้
- ห้ามเพิ่ม Update, Activate/Deactivate Customer workflow, Address, Site, Opportunity, Duplicate Review workflow, Export, Merge หรือ Hard Delete
- Create ต้องมี `customers.create` และ `customer-contacts.manage`; List/Detail ต้องมี `customers.read`; Backend เป็น security authority
- Effective permission ระยะนี้รับเฉพาะ scope `organization`; User, Membership, Organization, Branch, Role และ Permission ที่เกี่ยวข้องต้อง active
- ค่า canonical คือ Customer Type `person|organization`, Customer Status `draft|active|inactive`, Locale `th|en` และ Contact Channel `phone|email|line|other`
- Contact ต้องมี phone หรือ email อย่างน้อยหนึ่งค่า; email ที่มีค่าต้องผ่าน format validation
- `Idempotency-Key` คงเดิมเฉพาะ retry ของ create intent/payload เดิม; เมื่อแก้ field หลัง failed response ต้องสร้าง key ใหม่
- Duplicate candidate ต้อง mask PII, จำกัดใน Organization เดียว, ไม่ auto-merge และต้องมองเห็นได้หลัง Create
- Frontend API request/response/query parameter types ต้อง derive จาก `frontend/src/generated/api/tan-erp.v1.ts`; ห้ามเขียน DTO ซ้ำ
- Business query keys ขึ้นต้นด้วย `business` และรวม Membership, locale และ filter ที่มีผล; ก่อนเปลี่ยน Membership ต้อง cancel และ remove business queries เก่า
- UI text ทั้งหมดใช้ `frontend/src/messages/th.json` และ `en.json` ด้วย key parity; ห้าม `any`, `as any` และ `@ts-ignore`
- ใช้ TDD ทุก Task: ยืนยัน RED ด้วยสาเหตุเป้าหมาย, แก้ขั้นต่ำ, ยืนยัน GREEN แล้ว commit แบบ `type(scope): description`
- ห้ามแก้ migration `20260907161632_CustomerContactSlice` ที่ commit แล้ว; schema correction ต้องเป็น migration ใหม่
- ห้ามปรับ `customer-contact-verification.md` เป็น Passed จนทุก automated gate และ manual UX gate ที่กำหนดใน Task 6 ผ่านจริง

---

## Planned File Map

### Backend authorization and persistence

- Modify `backend/src/TanErp.Domain/IdentityAccess/Permission.cs` — เพิ่ม explicit activation lifecycle
- Modify `backend/src/TanErp.Infrastructure/Persistence/Configurations/PermissionConfiguration.cs` — map `is_active`
- Modify `backend/src/TanErp.Infrastructure/Persistence/RequestAccessResolver.cs` — require active Branch และ Permission
- Modify `backend/src/TanErp.Infrastructure/Persistence/CurrentUserReader.cs` — ไม่ project Permission ที่ inactive ไปยัง Frontend
- Modify `backend/src/TanErp.Application/Crm/Customers/CreateCustomer/CreateCustomerHandler.cs` — require contact-management permission ก่อนสร้าง Contact
- Create EF migration `*_EnforceCustomerAccessActivation.cs` และ designer — เพิ่ม `identity_access.permissions.is_active`
- Modify `backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs`
- Modify `backend/tests/TanErp.UnitTests/Crm/Customers/CreateCustomerHandlerTests.cs`
- Modify `backend/tests/TanErp.IntegrationTests/Persistence/RequestAccessResolverTests.cs`
- Modify `backend/tests/TanErp.IntegrationTests/Api/CurrentUserEndpointTests.cs`

### Frontend contract, state and UI

- Modify `frontend/src/lib/api/api-client.ts` และ `.test.ts` — derive list query parameters จาก OpenAPI
- Modify `frontend/src/lib/membership/selected-membership-context.tsx`
- Create `frontend/src/lib/membership/selected-membership-context.test.tsx`
- Modify `frontend/src/features/customers/api/customer-queries.ts`
- Create `frontend/src/features/customers/api/customer-queries.test.tsx`
- Modify `frontend/src/features/customers/schemas/customer-form-schema.ts`
- Create `frontend/src/features/customers/schemas/customer-form-schema.test.ts`
- Modify `frontend/src/features/customers/components/customer-editor.tsx`
- Modify `frontend/src/features/customers/components/customer-list.tsx`
- Modify `frontend/src/features/customers/components/customer-detail.tsx`
- Create `frontend/src/features/customers/components/duplicate-candidate-card.tsx`
- Create `frontend/src/features/customers/components/customer-editor.test.tsx`
- Create `frontend/src/features/customers/components/customer-list.test.tsx`
- Create `frontend/src/features/customers/components/customer-detail.test.tsx`
- Modify `frontend/src/messages/th.json` และ `frontend/src/messages/en.json`
- Modify `frontend/src/lib/permissions/can.ts` — whitespace-only gate fix

### Acceptance evidence

- Modify `frontend/e2e/customer-contact.spec.ts`
- Modify `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`
- Modify `backend/tests/TanErp.IntegrationTests/Persistence/CustomerContactMigrationTests.cs`
- Modify `docs/05-engineering/customer-contact-verification.md`
- Modify `docs/README.md`

---

### Task 1: Close the Backend Authorization Boundary

**Files:**
- Modify: `backend/src/TanErp.Domain/IdentityAccess/Permission.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/PermissionConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/RequestAccessResolver.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/CurrentUserReader.cs`
- Modify: `backend/src/TanErp.Application/Crm/Customers/CreateCustomer/CreateCustomerHandler.cs`
- Modify: `backend/tests/TanErp.UnitTests/Crm/Customers/CreateCustomerHandlerTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Persistence/RequestAccessResolverTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/CurrentUserEndpointTests.cs`
- Create (EF-generated): `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_EnforceCustomerAccessActivation.cs`
- Create (EF-generated): `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_EnforceCustomerAccessActivation.Designer.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs`

**Interfaces:**
- Consumes: Firebase UID, exact Membership ID, permission key, current UTC time
- Produces: `Permission.IsActive`, `Permission.Activate()`, `Permission.Deactivate()` และ fail-closed `RequestAccessContext`

- [ ] **Step 1: Add failing handler test for missing contact permission**

เพิ่ม test ที่ grant เฉพาะ `customers.create` แล้ว assert ว่า store ไม่ถูกเรียก:

```csharp
[Fact]
public async Task Handle_WhenMissingContactManagePermission_ReturnsPermissionDeniedWithoutWriting()
{
    var resolver = new FakeRequestAccessResolver();
    resolver.GrantedPermissions.Add("customers.create");
    var store = new FakeCustomerCreationStore();
    var handler = new CreateCustomerHandler(resolver, store, new FakeClock());

    var result = await handler.Handle(new CreateCustomerCommand(
        "uid-1", Guid.NewGuid(), "key-1234567890123456",
        "organization", "บริษัท ก", null, "th",
        new CreatePrimaryContact("นาย ก", null, "0812345678", null, "phone"),
        "trace-1"));

    Assert.True(result.IsFailure);
    Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    Assert.Null(store.LastRequest);
}
```

- [ ] **Step 2: Add failing resolver tests for inactive Permission and Branch**

เพิ่มสอง resolver tests โดย deactivate entity แล้วเรียก resolver ด้วย permission ที่เคย grant:

```csharp
permission.Deactivate();
branch.Deactivate();
await _db.SaveChangesAsync();

var result = await _resolver.ResolveAsync(firebaseUid, membershipId, permissionKey);
Assert.True(result.IsFailure);
Assert.Equal("PERMISSION_DENIED", result.Error.Code);
```

แยก fixture ต่อ test เพื่อให้ inactive Permission และ inactive Branch พิสูจน์คนละเงื่อนไข และ assert ว่า Membership เดิมยัง active

เพิ่ม Current User endpoint test ที่ deactivate Permission แล้ว assert ว่า `GET /api/v1/me` ยังสำเร็จ แต่ response ไม่มี permission key ดังกล่าว เพื่อให้ UX guard ใช้ effective permissions ชุดเดียวกับ Backend

- [ ] **Step 3: Run focused tests and confirm RED**

```bash
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter FullyQualifiedName~CreateCustomerHandlerTests
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter FullyQualifiedName~RequestAccessResolverTests
```

Expected: handler test พบ write ทั้งที่ขาด permission; resolver tests ยัง compile ไม่ผ่านหรือ authorize entity ที่ inactive

- [ ] **Step 4: Add Permission activation lifecycle and mapping**

เพิ่ม property/method โดย constructor เดิมยัง default active เพื่อไม่ทำลาย seed call sites:

```csharp
public bool IsActive { get; private set; }

public Permission(Guid id, string key, string? description = null, bool isActive = true)
    : base(id)
{
    if (string.IsNullOrWhiteSpace(key))
        throw new ArgumentException("Permission key cannot be blank.", nameof(key));

    Key = key.Trim();
    Description = description;
    IsActive = isActive;
}

public void Activate() => IsActive = true;
public void Deactivate() => IsActive = false;
```

Map `is_active` เป็น required และ generate migration `EnforceCustomerAccessActivation` ที่เพิ่ม boolean `NOT NULL DEFAULT true`

- [ ] **Step 5: Make the resolver and Create handler fail closed**

เพิ่ม predicates:

```csharp
.Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
.Where(m => m.BranchId == null || m.Branch!.IsActive)
```

และใน RolePermission query:

```csharp
.Where(rp => rp.Permission!.IsActive
    && rp.Permission.Key == permissionKey
    && rp.Scope == PermissionScope.Organization
    && rp.ScopeId == m.OrganizationId)
```

เพิ่ม `.Where(rp => rp.Permission!.IsActive)` ใน `CurrentUserReader` ก่อน project permission key ด้วย

เปลี่ยน Create handler ให้หยุดทันที:

```csharp
if (manageContactResult.IsFailure)
    return Result<CreateCustomerResult>.Failure(manageContactResult.Error);

const bool includeContactPii = true;
```

- [ ] **Step 6: Run tests, inspect migration and commit**

```bash
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter FullyQualifiedName~CreateCustomerHandlerTests
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter FullyQualifiedName~RequestAccessResolverTests
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter FullyQualifiedName~CurrentUserEndpointTests
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter FullyQualifiedName~FoundationMigrationTests
git diff --check
```

Expected: PASS; migration แตะเฉพาะ `identity_access.permissions.is_active`; inactive Branch/Permission และ missing contact permission ถูกปฏิเสธ

```bash
git add backend/src/TanErp.Domain/IdentityAccess/Permission.cs backend/src/TanErp.Application/Crm/Customers/CreateCustomer backend/src/TanErp.Infrastructure/Persistence backend/tests/TanErp.UnitTests/Crm/Customers backend/tests/TanErp.IntegrationTests/Persistence
git commit -m "fix(auth): close customer access boundary"
```

---

### Task 2: Align Customer Contracts and Form Validation

**Files:**
- Modify: `frontend/src/lib/api/api-client.ts`
- Modify: `frontend/src/lib/api/api-client.test.ts`
- Modify: `frontend/src/features/customers/schemas/customer-form-schema.ts`
- Create: `frontend/src/features/customers/schemas/customer-form-schema.test.ts`
- Modify: `frontend/src/features/customers/components/customer-editor.tsx`
- Modify: `frontend/src/features/customers/components/customer-list.tsx`
- Modify: `frontend/src/features/customers/components/customer-detail.tsx`
- Create: `frontend/src/features/customers/components/customer-list.test.tsx`
- Create: `frontend/src/features/customers/components/customer-detail.test.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`

**Interfaces:**
- Consumes: generated `paths`/`components`, translation function `(key: string) => string`
- Produces: `ListCustomersParams` จาก OpenAPI, `createCustomerFormSchema(tValidation)` และ UI ที่รู้จัก `organization|person` กับ `draft|active|inactive`

- [ ] **Step 1: Write failing contract and schema tests**

ครอบคลุมค่าต่อไปนี้:

```ts
expect(schema.safeParse(validOrganizationWithPhone).success).toBe(true);
expect(schema.safeParse(validPersonWithEmailOnly).success).toBe(true);
expect(schema.safeParse({ ...validOrganizationWithPhone, customerType: "corporate" }).success).toBe(false);
expect(schema.safeParse(contactWithoutPhoneAndEmail).error?.issues[0].message)
  .toBe("validation.phoneOrEmailRequired");
```

แก้ API client test payload จาก `corporate` เป็น `organization` และเพิ่ม compile-time assignment:

```ts
const params: ListCustomersParams = { search: "บริษัท", status: "draft", limit: 10 };
```

- [ ] **Step 2: Write failing component tests for canonical labels and draft status**

List/detail fixtures ใช้ `customerType: "organization"`, `status: "draft"` แล้ว assert ป้าย “นิติบุคคล” และ “ฉบับร่าง”; เพิ่ม person/active case แยกเพื่อไม่ให้ fallback ternary ซ่อนค่าที่ไม่รู้จัก

- [ ] **Step 3: Run frontend customer tests and confirm RED**

```bash
npm --prefix frontend run test -- src/features/customers src/lib/api/api-client.test.ts
```

Expected: email-only, canonical type และ draft label tests FAIL

- [ ] **Step 4: Derive API parameters and implement localized schema factory**

```ts
import type { components, paths } from "@/generated/api/tan-erp.v1";

export type ListCustomersParams = NonNullable<
  paths["/api/v1/customers"]["get"]["parameters"]["query"]
>;

type ValidationTranslator = (key: "required" | "invalidEmail" | "phoneOrEmailRequired") => string;

export const createCustomerFormSchema = (t: ValidationTranslator) =>
  z.object({
    customerType: z.enum(["organization", "person"]),
    displayNameTh: z.string().trim().min(1, t("required")),
    displayNameEn: z.string().trim().optional().or(z.literal("")),
    preferredLocale: z.enum(["th", "en"]),
    primaryContact: z.object({
      name: z.string().trim().min(1, t("required")),
      roleTitle: z.string().trim().optional().or(z.literal("")),
      phone: z.string().trim().optional().or(z.literal("")),
      email: z.string().trim().email(t("invalidEmail")).optional().or(z.literal("")),
      preferredChannel: z.enum(["phone", "email", "line", "other"]),
    }).refine((value) => Boolean(value.phone || value.email), {
      message: t("phoneOrEmailRequired"),
      path: ["phone"],
    }),
  });
```

สร้าง schema ด้วย `useTranslations("common.validation")`; ลบ `required` จาก phone input และให้ error เชื่อมกับ phone control

- [ ] **Step 5: Render Customer Type and Status without fallback misclassification**

ใช้ exhaustive maps:

```ts
const customerTypeKey = { organization: "organization", person: "person" } as const;
const customerStatusKey = { draft: "draft", active: "active", inactive: "inactive" } as const;
```

เพิ่ม message keys `customers.organization`, `customers.person` และใช้ `common.status.draft|active|inactive`; unknown server value แสดง localized `common.feedback.operationFailed` แทนการจัดเป็น person/inactive

- [ ] **Step 6: Run tests and commit**

```bash
npm --prefix frontend run test -- src/features/customers src/lib/api/api-client.test.ts
npm --prefix frontend run typecheck
npm --prefix frontend run lint
```

Expected: PASS และ payload ทุก test ใช้ `organization|person`

```bash
git add frontend/src/lib/api frontend/src/features/customers frontend/src/messages
git commit -m "fix(customers): align form with api contract"
```

---

### Task 3: Make Create Intent, Retry and Duplicate Results Correct

**Files:**
- Modify: `frontend/src/features/customers/components/customer-editor.tsx`
- Create: `frontend/src/features/customers/components/customer-editor.test.tsx`
- Create: `frontend/src/features/customers/components/duplicate-candidate-card.tsx`
- Modify: `frontend/src/features/customers/components/customer-detail.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`

**Interfaces:**
- Consumes: `apiClient.createCustomer`, generated `CustomerResponse`, selected Membership
- Produces: one idempotency key per payload intent และ reusable `DuplicateCandidateCard`

- [ ] **Step 1: Write failing idempotency lifecycle tests**

Mock `crypto.randomUUID()` ให้คืน `key-1`, `key-2`; ทดสอบว่า valid double click ยิง network ครั้งเดียว, retry โดยไม่แก้ field ใช้ `key-1`, และเมื่อ failed response แล้วแก้ field retry ใช้ `key-2`

- [ ] **Step 2: Write failing duplicate response test**

ให้ Create ตอบ `duplicateCandidates` หนึ่งรายการ แล้ว assert ว่า masked candidate ปรากฏ, ไม่ auto-merge, และมี link ไป Customer ที่สร้าง ส่วน response ที่ไม่มี candidate ต้อง navigate ไป detail ตามปกติ

- [ ] **Step 3: Run editor tests and confirm RED**

```bash
npm --prefix frontend run test -- src/features/customers/components/customer-editor.test.tsx
```

Expected: key rotation และ duplicate presentation tests FAIL

- [ ] **Step 4: Implement explicit create-intent state**

```ts
const idempotencyKeyRef = useRef<string | null>(null);
const failedSubmissionRef = useRef(false);
const [createdCustomerId, setCreatedCustomerId] = useState<string | null>(null);

const handleFormChange = () => {
  if (failedSubmissionRef.current) {
    idempotencyKeyRef.current = null;
    failedSubmissionRef.current = false;
  }
};
```

ผูก `onChange={handleFormChange}` ที่ `<form>`; ใน valid submit ใช้ `idempotencyKeyRef.current ??= crypto.randomUUID()` ก่อน request และ set `failedSubmissionRef.current = true` ใน catch เท่านั้น

- [ ] **Step 5: Preserve duplicate result instead of discarding it**

หลัง Create สำเร็จ:

```ts
if (created.duplicateCandidates?.length) {
  setDuplicateCandidates(created.duplicateCandidates);
  setCreatedCustomerId(created.id);
  return;
}

router.push(`/${locale}/customers/${created.id}`);
```

`DuplicateCandidateCard` รับ `{ candidates, createdCustomerHref? }`; เมื่อ Create พบ candidate ให้แสดง localized link “ดูข้อมูลลูกค้าที่สร้างแล้ว” ส่วน Detail reuse card เดียวกัน ห้ามแสดง raw phone/email ที่ไม่ mask

- [ ] **Step 6: Run tests and commit**

```bash
npm --prefix frontend run test -- src/features/customers/components/customer-editor.test.tsx src/features/customers/components/customer-detail.test.tsx
npm --prefix frontend run typecheck
```

Expected: PASS; same intent ใช้ key เดิม, changed intent ใช้ key ใหม่, duplicate candidate มองเห็นได้

```bash
git add frontend/src/features/customers/components frontend/src/messages
git commit -m "fix(customers): preserve create intent results"
```

---

### Task 4: Isolate Business Queries Across Memberships

**Files:**
- Modify: `frontend/src/lib/membership/selected-membership-context.tsx`
- Create: `frontend/src/lib/membership/selected-membership-context.test.tsx`
- Modify: `frontend/src/features/customers/api/customer-queries.ts`
- Create: `frontend/src/features/customers/api/customer-queries.test.tsx`

**Interfaces:**
- Consumes: `QueryClient`, Membership DTOs, locale และ generated list parameters
- Produces: `setSelectedMembershipId(id): Promise<void>` และ business query key factories

- [ ] **Step 1: Write failing Membership switch tests**

Seed QueryClient ด้วย keys ของ Membership A/B, สร้าง pending fetch ของ A, สลับไป B แล้ว assert ลำดับ: cancel business queries → remove business queries → expose B; assert current-user/auth query ไม่ถูกลบ

- [ ] **Step 2: Write failing query-key tests**

```ts
expect(customerListQueryKey("membership-a", "th", { status: "draft", limit: 10 }))
  .not.toEqual(customerListQueryKey("membership-a", "th", { status: "draft", limit: 25 }));
expect(customerDetailQueryKey("membership-a", "th", "customer-1"))
  .not.toEqual(customerDetailQueryKey("membership-b", "th", "customer-1"));
expect(customerDetailQueryKey("membership-a", "th", "customer-1"))
  .not.toEqual(customerDetailQueryKey("membership-a", "en", "customer-1"));
```

- [ ] **Step 3: Run tests and confirm RED**

```bash
npm --prefix frontend run test -- src/lib/membership src/features/customers/api
```

Expected: current key omits limit/locale และ Membership switch ไม่ cancel/remove cache

- [ ] **Step 4: Derive valid selection without Effect**

เก็บเฉพาะ explicit selection:

```ts
const [requestedId, setRequestedId] = useState<string | null>(null);
const selectedMembership =
  memberships.find((membership) => membership.id === requestedId) ?? memberships[0] ?? null;
```

ลบ `useEffect`; ปฏิเสธ `newId` ที่ไม่มีใน memberships และไม่เปลี่ยน state เมื่อเลือก ID เดิม

- [ ] **Step 5: Add business key namespace and ordered cache switch**

```ts
export const customerListQueryKey = (
  membershipId: string | null | undefined,
  locale: "th" | "en",
  params?: ListCustomersParams,
) => ["business", membershipId, locale, "customers", "list",
  params?.search ?? null, params?.status ?? null, params?.limit ?? 25] as const;
```

ใช้ detail key รูป `['business', membershipId, locale, 'customers', 'detail', customerId]` และสลับ Membership ด้วย:

```ts
await queryClient.cancelQueries({ queryKey: ["business"] });
queryClient.removeQueries({ queryKey: ["business"] });
setRequestedId(newId);
```

- [ ] **Step 6: Run tests and commit**

```bash
npm --prefix frontend run test -- src/lib/membership src/features/customers/api
npm --prefix frontend run typecheck
npm --prefix frontend run lint
```

Expected: PASS และไม่มี `useEffect` ใน selected-membership context

```bash
git add frontend/src/lib/membership frontend/src/features/customers/api
git commit -m "fix(frontend): isolate membership query state"
```

---

### Task 5: Complete Customer Localization and Reuse Cleanup

**Files:**
- Modify: `frontend/src/features/customers/components/customer-editor.tsx`
- Modify: `frontend/src/features/customers/components/customer-list.tsx`
- Modify: `frontend/src/features/customers/components/customer-detail.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`
- Modify: `frontend/src/lib/permissions/can.ts`

**Interfaces:**
- Consumes: `next-intl` customer/common namespaces และ shared `DuplicateCandidateCard`
- Produces: Customer UI ที่ไม่มี user-visible hardcoded copy และผ่าน `git diff --check`

- [ ] **Step 1: Add failing localization assertions**

ใน component tests render error states ทั้ง `th` และ `en`; assert translated list/detail/auth/membership/save errors, `telLabel`, `emailLabel`, `localeThai`, `localeEnglish` และ duplicate link โดยไม่พบข้อความ fallback ภาษาอังกฤษที่ hardcode

- [ ] **Step 2: Run component tests and confirm RED**

```bash
npm --prefix frontend run test -- src/features/customers/components
```

Expected: FAIL ที่ hardcoded error/label ปัจจุบัน

- [ ] **Step 3: Add paired message keys and replace literals**

เพิ่ม key tree เดียวกันทั้งสองภาษา:

```text
customers.errors.loadList
customers.errors.loadDetail
customers.errors.authenticationRequired
customers.errors.membershipRequired
customers.errors.saveUnexpected
customers.telLabel
customers.emailLabel
customers.localeThai
customers.localeEnglish
customers.viewCreatedCustomer
common.validation.phoneOrEmailRequired
```

ใช้ key เหล่านี้แทน `Failed to load customers`, `Customer not found`, `No authentication token available`, `No active membership selected`, `Tel`, `Email`, `ไทย (Thai)` และ `English`

- [ ] **Step 4: Remove duplicate markup and whitespace**

ให้ editor/detail import `DuplicateCandidateCard`; ลบ warning-card markup เดิมทั้งสองชุด และลบ trailing whitespace ที่ `frontend/src/lib/permissions/can.ts:8`

- [ ] **Step 5: Run localization and static gates, then commit**

```bash
npm --prefix frontend run test -- src/features/customers
npm --prefix frontend run lint
npm --prefix frontend run typecheck
git diff --check
```

Expected: PASS; th/en key trees parity และ diff check ไม่มี output

```bash
git add frontend/src/features/customers frontend/src/messages frontend/src/lib/permissions/can.ts
git commit -m "fix(customers): localize customer experience"
```

---

### Task 6: Rebuild the Quality-Gate Evidence

**Files:**
- Modify: `frontend/e2e/customer-contact.spec.ts`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Persistence/CustomerContactMigrationTests.cs`
- Modify: `docs/05-engineering/customer-contact-verification.md`
- Modify: `docs/README.md`

**Interfaces:**
- Consumes: real API/PostgreSQL/Firebase emulator test environment และ corrected Customer UI
- Produces: executable UAT evidence และ truthful `Passed Quality Gate` หรือ `Pending` record

- [ ] **Step 1: Expand Playwright acceptance journey**

แยก tests ให้พิสูจน์: required validation, email-only Create, one POST on double click, response `201` + `ETag`, detail refresh, search, draft/type labels และ duplicate-candidate path ใช้ `TEST_ONLY` data เท่านั้น

- [ ] **Step 2: Add direct security acceptance cases**

Integration tests ต้อง assert:

```text
customers.create without customer-contacts.manage -> 403 and no customer row
inactive Permission -> 403
inactive Branch -> 403
Org B membership requesting Org A customer -> 404 and no PII
read-only user -> masked contact
same idempotency key + same payload -> same customer ID
same idempotency key + changed payload -> 409 IDEMPOTENCY_KEY_REUSED
```

- [ ] **Step 3: Verify migration rehearsal and actual schema facts**

รัน rollback/reapply ทั้ง Foundation และ `EnforceCustomerAccessActivation`; query `information_schema` เพื่อ assert `crm.customers`, `crm.customer_contacts`, `audit.idempotency_records`, `permissions.is_active` และ canonical columns `normalized_phone`/`normalized_email`

- [ ] **Step 4: Run every automated gate from a .NET 10 environment**

```bash
dotnet --version
dotnet build backend/TanErp.slnx
dotnet test backend/TanErp.slnx
npm --prefix frontend run verify
npm run test:fixtures
PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e
git diff --check
rg -n "\bany\b|as any|@ts-ignore|BEGIN PRIVATE KEY|firebase-admin" frontend/src frontend/e2e
```

Expected: SDK `10.0.400`; ทุก command exit `0`; forbidden-pattern scan ไม่มี code violation; E2E ใช้ API/PostgreSQL/Firebase emulator จริง

- [ ] **Step 5: Perform manual UX checks**

ตรวจภาษาไทย/อังกฤษ, keyboard-only, focus visible, error association, 320px, tablet, desktop, zoom 200%, 44px targets, reduced motion, zero-radius และ Minimal Mono Loading บันทึก pass/fail พร้อมวันที่จริง

- [ ] **Step 6: Rewrite the verification record from observed evidence**

แก้ schema จาก `common` เป็น `audit`; ลบคำกล่าวอ้าง CHECK/hash columns ที่ไม่มีจริงและระบุ `normalized_phone`/`normalized_email` ตาม implementation บันทึก command, runtime, counts, exit codes, UAT IDs และ known limitations จากผล Step 4–5 เท่านั้น ถ้ามี gate ใดไม่ผ่านให้สถานะคงเป็น `Pending` พร้อมเหตุผล ห้ามคัดลอกตัวเลขเดิม

- [ ] **Step 7: Update documentation index and commit evidence**

เพิ่มลิงก์ remediation plan และ verification record ใน `docs/README.md` แล้วตรวจ internal links

```bash
git add frontend/e2e backend/tests/TanErp.IntegrationTests docs/05-engineering/customer-contact-verification.md docs/README.md
git commit -m "test(crm): verify remediated customer slice"
```

---

## Completion Gate

แผนนี้สำเร็จเมื่อครบทุกข้อ:

- Create ที่ขาด permission ใด permission หนึ่งได้ `403` และไม่มี write/audit/idempotency record
- inactive Branch, Role หรือ Permission authorize Business API ไม่ได้
- UI ส่งเฉพาะ `organization|person`, รองรับ email-only และแสดง `draft` ถูกต้อง
- Membership switch cancel/remove business queries ก่อน expose context ใหม่ และ key รวม Membership/locale/filter
- Retry payload เดิมใช้ Idempotency-Key เดิม; field change หลัง failure ใช้ keyใหม่
- Duplicate candidate ที่ mask แล้วมองเห็นได้และไม่ถูก auto-merge
- ไม่มี hardcoded Customer UI copy, duplicated warning card, generated contract duplication หรือ `git diff --check` error
- Backend build/test, Frontend verify, fixtures, Playwright และ manual UX checks ผ่านจริง
- `customer-contact-verification.md` ตรงกับ migration/code/test output และจึงค่อยระบุ `Passed Quality Gate`
