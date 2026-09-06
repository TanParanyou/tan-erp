# Foundation Login and Current User Remediation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ปิด findings จาก code review ของ branch `feat/foundation-login-current-user` ให้ Foundation Login → Current User → ERP shell ผ่าน security, contract, CI, localization และ accessibility gates โดยไม่เพิ่ม business module ใหม่

**Architecture:** ป้องกัน tenant boundary ที่ PostgreSQL ด้วย composite foreign keys, ทำ API failure ให้ fail closed ผ่าน configuration validation และ global RFC 9457 handler, และผูกการล้าง TanStack Query cache กับ provider instance จริง ฝั่ง Frontend จะคงไว้เฉพาะ primitives ที่ Foundation slice ใช้งาน พร้อมทดสอบพฤติกรรมผ่าน integration, component และ Playwright tests

**Tech Stack:** .NET SDK `10.0.400`, ASP.NET Core 10, EF Core `10.0.11`, PostgreSQL 17, Firebase Admin .NET `3.6.0`, Next.js `16.3.4`, React `19.2.8`, TypeScript, TanStack Query `5.102.8`, Vitest และ Playwright `1.63.0`

## Global Constraints

- ทำงานเฉพาะ branch `feat/foundation-login-current-user` และ diff เทียบ `main`; ห้ามเพิ่ม Customer, CRM, Survey runtime, Estimate, Item, Commercial, Procurement, Inventory, Production หรือ MRP behavior
- อ่าน `AGENTS.md`, `design.md`, `CONTEXT.md`, `docs/README.md` และ scoped `AGENTS.md` ก่อนแก้ไฟล์
- ห้ามใช้ `any`, `as any` หรือ `@ts-ignore`; ใช้ explicit types หรือ `unknown` พร้อม type guard
- ข้อความ UI และ Problem Details ต้องมีคู่ภาษา `th`/`en`; ภาษาไทยเป็นค่าเริ่มต้น
- PostgreSQL เป็น security authority ของ Organization และ RBAC; Frontend guard มีไว้เพื่อ UX เท่านั้น
- ห้ามเก็บ connection string, credential, token หรือ secret ใน source, test fixture หรือ log
- ใช้ TDD: เพิ่ม failing test, ยืนยันว่า fail ด้วยสาเหตุที่ต้องการ, แก้ขั้นต่ำ แล้วรัน test ให้ผ่าน
- ใช้ migration ใหม่ชื่อ `EnforceOrganizationBoundaries`; ห้ามแก้ migration ที่ commit แล้ว
- ทุก commit ใช้รูปแบบ `type(scope): description` และแตะเฉพาะไฟล์ของ task นั้น
- ก่อนประกาศเสร็จต้องผ่าน `git diff --check`, fixture, Backend build/test, OpenAPI drift, Frontend lint/typecheck/test/build และ Playwright E2E

---

## File Map

- `backend/src/TanErp.Domain/IdentityAccess/MembershipRole.cs` — เก็บ `OrganizationId` ของ assignment เพื่อสร้าง composite tenant constraints
- `backend/src/TanErp.Infrastructure/Persistence/Configurations/{Branch,Membership,Role,MembershipRole}Configuration.cs` — alternate keys และ composite foreign keys
- `backend/src/TanErp.Infrastructure/Persistence/Migrations/*EnforceOrganizationBoundaries*` — schema change สำหรับ tenant integrity
- `backend/tests/TanErp.IntegrationTests/Persistence/FoundationMigrationTests.cs` — negative cross-organization insert tests และ rollback/reapply
- `backend/src/TanErp.Api/ErrorHandling/ApiExceptionHandler.cs` — global unexpected-exception mapping
- `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs` — สร้าง localized RFC 9457 payload จาก stable code
- `backend/src/TanErp.Api/Program.cs` — fail-closed configuration และ exception middleware registration
- `backend/src/TanErp.Infrastructure/Identity/FirebaseTokenVerifier.cs` — preserve cancellation และไม่ log exception detail
- `frontend/src/lib/auth/auth-session.ts` — บังคับรับ QueryClient instance จริง
- `frontend/src/features/auth/components/access-gate.tsx` และ `frontend/src/components/layout/erp-shell.tsx` — ส่ง provider QueryClient เข้า auth lifecycle
- `frontend/src/features/auth/components/login-form.tsx` — allowlisted Firebase error mapping เท่านั้น
- `frontend/src/components/ui/Input.tsx` — keyboard-accessible password reveal button ขนาด 44px
- `frontend/src/app/layout.tsx` — document language จาก validated request locale
- `.github/workflows/verify.yml` และ `frontend/playwright.config.ts` — E2E server/base URL เดียวกัน
- `docs/05-engineering/foundation-login-verification.md` — หลักฐานจากคำสั่งจริงหลัง remediation

---

### Task 1: Enforce Organization Boundaries at the Database

**Files:**
- Modify: `backend/src/TanErp.Domain/IdentityAccess/MembershipRole.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/BranchConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/MembershipConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/RoleConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/MembershipRoleConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/CurrentUserEndpointTests.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Persistence/FoundationMigrationTests.cs`
- Create (EF-generated): `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_EnforceOrganizationBoundaries.cs`
- Create (EF-generated): `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_EnforceOrganizationBoundaries.Designer.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs`

**Interfaces:**
- Consumes: `Membership.OrganizationId`, `Branch.OrganizationId`, `Role.OrganizationId`
- Produces: `MembershipRole(Guid membershipId, Guid roleId, Guid organizationId, DateTimeOffset? assignedAtUtc = null)` และ database-enforced same-organization relationships

- [ ] **Step 1: Replace the existing positive-only cross-organization test with negative inserts**

เพิ่ม assertion สองกรณีใน `FoundationMigrationTests.CrossOrganization_And_Rollback_Tests` หลังบันทึกข้อมูล Org A/Org B ที่ถูกต้องแล้ว:

```csharp
var crossBranchMembership = new Membership(
    Guid.NewGuid(), orgA.Id, branchB.Id, userA.Id);
db.Memberships.Add(crossBranchMembership);
await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
db.ChangeTracker.Clear();

var crossRoleAssignment = new MembershipRole(
    memberA.Id, roleB.Id, orgA.Id);
db.MembershipRoles.Add(crossRoleAssignment);
await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
db.ChangeTracker.Clear();
```

- [ ] **Step 2: Run the migration test and confirm the current model accepts invalid data**

Run:

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter FullyQualifiedName~FoundationMigrationTests.CrossOrganization_And_Rollback_Tests
```

Expected: FAIL เพราะอย่างน้อยหนึ่ง `Assert.ThrowsAsync<DbUpdateException>` ไม่ได้รับ exception

- [ ] **Step 3: Add OrganizationId to MembershipRole**

เปลี่ยน entity ให้ assignment ระบุ tenant อย่างชัดเจน:

```csharp
public Guid OrganizationId { get; private set; }

public MembershipRole(
    Guid membershipId,
    Guid roleId,
    Guid organizationId,
    DateTimeOffset? assignedAtUtc = null)
{
    if (membershipId == Guid.Empty)
        throw new ArgumentException("Membership ID cannot be empty.", nameof(membershipId));
    if (roleId == Guid.Empty)
        throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));
    if (organizationId == Guid.Empty)
        throw new ArgumentException("Organization ID cannot be empty.", nameof(organizationId));

    MembershipId = membershipId;
    RoleId = roleId;
    OrganizationId = organizationId;
    AssignedAtUtc = assignedAtUtc ?? DateTimeOffset.UtcNow;
}
```

แก้ทุก call site ให้ส่ง organization ID ของ membership เช่น:

```csharp
new MembershipRole(membership.Id, role.Id, organization.Id)
```

- [ ] **Step 4: Configure composite alternate keys and foreign keys**

เพิ่ม alternate keys:

```csharp
// BranchConfiguration
builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

// MembershipConfiguration
builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });

// RoleConfiguration
builder.HasAlternateKey(x => new { x.Id, x.OrganizationId });
```

เปลี่ยน Branch relationship ใน `MembershipConfiguration`:

```csharp
builder.HasOne(x => x.Branch)
    .WithMany(x => x.Memberships)
    .HasForeignKey(x => new { x.BranchId, x.OrganizationId })
    .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
    .OnDelete(DeleteBehavior.Restrict);
```

กำหนด `MembershipRoleConfiguration` ให้ tenant เดียวกันทั้งสองด้าน:

```csharp
builder.Property(x => x.OrganizationId)
    .HasColumnName("organization_id")
    .IsRequired();

builder.HasOne(x => x.Membership)
    .WithMany(x => x.MembershipRoles)
    .HasForeignKey(x => new { x.MembershipId, x.OrganizationId })
    .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
    .OnDelete(DeleteBehavior.Cascade);

builder.HasOne(x => x.Role)
    .WithMany(x => x.MembershipRoles)
    .HasForeignKey(x => new { x.RoleId, x.OrganizationId })
    .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
    .OnDelete(DeleteBehavior.Cascade);
```

ลบ relationship แบบ `MembershipId`-only และ `RoleId`-only ที่ซ้ำจาก configuration อื่น เพื่อให้ EF มี relationship เดียวต่อ navigation

- [ ] **Step 5: Generate and inspect the corrective migration**

Run:

```bash
test -n "$ConnectionStrings__Database"
dotnet ef migrations add EnforceOrganizationBoundaries --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
```

Expected migration:

- เพิ่ม `organization_id NOT NULL` ใน `identity_access.membership_roles`
- เพิ่ม unique/alternate constraints สำหรับ `(id, organization_id)` บน branches, memberships และ roles
- เพิ่ม composite FKs `(branch_id, organization_id)`, `(membership_id, organization_id)` และ `(role_id, organization_id)`
- `Down` คืน schema เดิมได้ครบ

- [ ] **Step 6: Run persistence and endpoint tests**

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~FoundationMigrationTests|FullyQualifiedName~CurrentUserEndpointTests"
```

Expected: PASS และ invalid cross-organization inserts ถูก PostgreSQL ปฏิเสธ

- [ ] **Step 7: Commit tenant integrity changes**

```bash
git add backend/src/TanErp.Domain/IdentityAccess/MembershipRole.cs backend/src/TanErp.Infrastructure/Persistence backend/tests/TanErp.IntegrationTests
git commit -m "fix(auth): enforce organization boundaries"
```

---

### Task 2: Fail Closed on Configuration and Preserve Cancellation

**Files:**
- Modify: `backend/src/TanErp.Api/Program.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/AppDbContextFactory.cs`
- Modify: `backend/src/TanErp.Infrastructure/Identity/FirebaseTokenVerifier.cs`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/CurrentUserEndpointTests.cs`
- Modify: `docs/05-engineering/foundation-login-runbook.md`
- Modify: `backend/README.md`

**Interfaces:**
- Consumes: `ConnectionStrings__Database`, `Firebase__ProjectId`, request `CancellationToken`
- Produces: startup failure when required configuration is absent; cancellation remains cancellation rather than becoming HTTP 401

- [ ] **Step 1: Add focused tests for missing configuration and cancellation**

เพิ่ม unit seam โดยให้ `FirebaseTokenVerifier.VerifyTokenAsync` rethrow cancellation และเพิ่ม test ที่ส่ง canceled token:

```csharp
using var cancellation = new CancellationTokenSource();
cancellation.Cancel();

await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
    verifier.VerifyTokenAsync("test-token", cancellation.Token));
```

เพิ่ม startup test ที่ลบ `ConnectionStrings:Database` และยืนยันว่า factory/startup โยน `InvalidOperationException` พร้อมข้อความชื่อ key แต่ไม่มี credential

- [ ] **Step 2: Run the focused tests and confirm they fail**

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~Configuration|FullyQualifiedName~Cancellation"
```

Expected: FAIL เพราะปัจจุบันใช้ fallback credential และ catch cancellation เป็น token invalid

- [ ] **Step 3: Require connection configuration in Program and design-time factory**

ใช้ fail-fast lookup ใน `Program.cs`:

```csharp
var connectionString = builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException(
        "Required configuration 'ConnectionStrings:Database' is missing.");
```

ใน `AppDbContextFactory` อ่าน environment variable เดียวกัน:

```csharp
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Database")
    ?? throw new InvalidOperationException(
        "Required environment variable 'ConnectionStrings__Database' is missing.");

optionsBuilder.UseNpgsql(connectionString);
```

- [ ] **Step 4: Narrow Firebase failures without logging exception details**

เปลี่ยน catch order:

```csharp
catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
{
    throw;
}
catch (FirebaseAuthException)
{
    return null;
}
```

ห้าม log token, `ex.Message`, project credential หรือ personal data; unexpected infrastructure exception ต้องปล่อยให้ global handler ของ Task 3 จัดการ

- [ ] **Step 5: Replace credential-bearing documentation examples**

ให้ runbook และ Backend README ใช้ placeholder ที่ผู้ใช้ต้องกำหนดเอง:

```bash
test -n "$ConnectionStrings__Database"
dotnet run --project backend/src/TanErp.Api
```

- [ ] **Step 6: Run Backend tests and secret scan**

```bash
dotnet test backend/TanErp.slnx
git grep -nE 'Password=postgres|Username=postgres' -- ':!docker-compose.yml' ':!backend/tests/**'
```

Expected: tests PASS และ grep ไม่พบ production/runtime fallback หรือ documentation credential

- [ ] **Step 7: Commit configuration hardening**

```bash
git add backend/src/TanErp.Api/Program.cs backend/src/TanErp.Infrastructure docs/05-engineering/foundation-login-runbook.md backend/README.md backend/tests
git commit -m "fix(config): fail closed on missing credentials"
```

---

### Task 3: Guarantee Localized RFC 9457 Responses

**Files:**
- Create: `backend/src/TanErp.Api/ErrorHandling/ApiExceptionHandler.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Modify: `backend/src/TanErp.Api/Controllers/CurrentUserController.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Modify: `backend/src/TanErp.Api/Resources/Errors.resx`
- Modify: `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Modify: `backend/tests/TanErp.IntegrationTests/Api/CurrentUserEndpointTests.cs`

**Interfaces:**
- Consumes: stable error code และ current request culture
- Produces: `ProblemDetailsMapper.CreateProblem(string code, HttpContext context)` และ global 500 payload code `INTERNAL_SERVER_ERROR`

- [ ] **Step 1: Strengthen localization assertions**

ขยาย theory ให้ตรวจทั้ง title และ detail:

```csharp
[Theory]
[InlineData("th", "จำเป็นต้องมีสมาชิกภาพ", "ไม่พบสมาชิกภาพที่ใช้งานอยู่")]
[InlineData("en", "Active Membership", "no active organization membership")]
[InlineData("fr", "จำเป็นต้องมีสมาชิกภาพ", "ไม่พบสมาชิกภาพที่ใช้งานอยู่")]
public async Task GetCurrentUser_Localization_HonorsAcceptLanguageHeader(
    string locale,
    string expectedTitle,
    string expectedDetail)
{
    // Existing request setup
    Assert.Contains(expectedTitle, problem.Title);
    Assert.Contains(expectedDetail, problem.Detail);
}
```

เพิ่ม test factory ที่แทน `ICurrentUserReader` ด้วย implementation ซึ่งโยน `InvalidOperationException` และ assert 500 body:

```csharp
Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
Assert.Equal("INTERNAL_SERVER_ERROR", problem.Code);
Assert.False(string.IsNullOrWhiteSpace(problem.TraceId));
Assert.DoesNotContain("InvalidOperationException", body);
```

- [ ] **Step 2: Run the endpoint tests and confirm detail/500 tests fail**

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter FullyQualifiedName~CurrentUserEndpointTests
```

Expected: localization detail ยังเป็นอังกฤษ และ unexpected exception ยังไม่ผ่าน required contract

- [ ] **Step 3: Make ProblemDetailsMapper code-driven only**

ลบ `customDetail` และแยก object creation:

```csharp
public static ApiProblemDetails CreateProblem(string code, HttpContext context)
{
    var status = GetStatus(code);
    return new ApiProblemDetails
    {
        Type = $"https://tan-erp.local/problems/{code.ToLowerInvariant().Replace('_', '-')}",
        Title = ResourceManager.GetString($"{code}_TITLE") ?? code,
        Detail = ResourceManager.GetString($"{code}_DETAIL") ?? code,
        Status = status,
        Code = code,
        TraceId = Activity.Current?.Id ?? context.TraceIdentifier,
        Instance = context.Request.Path
    };
}

public static ObjectResult CreateProblemResult(string code, HttpContext context) =>
    new(CreateProblem(code, context))
    {
        StatusCode = GetStatus(code),
        ContentTypes = { "application/problem+json" }
    };
```

Controller ต้องเรียกเพียง:

```csharp
return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
```

- [ ] **Step 4: Add the global exception handler**

สร้าง `ApiExceptionHandler`:

```csharp
public sealed class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = ProblemDetailsMapper.CreateProblem(
            "INTERNAL_SERVER_ERROR", httpContext);
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(
            problem,
            cancellationToken: cancellationToken);
        return true;
    }
}
```

ลงทะเบียนก่อน `builder.Build()` และวาง middleware หลัง localization:

```csharp
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
app.UseExceptionHandler();
```

- [ ] **Step 5: Add bilingual internal-error resources**

เพิ่ม keys เดียวกันในทั้งสองไฟล์:

```text
INTERNAL_SERVER_ERROR_TITLE
INTERNAL_SERVER_ERROR_DETAIL
```

ข้อความไทยต้องไม่เผย exception detail และไฟล์อังกฤษต้องมีความหมายเทียบเท่า

- [ ] **Step 6: Run API integration and OpenAPI contract tests**

```bash
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~CurrentUserEndpointTests|FullyQualifiedName~OpenApiContractTests"
```

Expected: 401/403/500 เป็น `application/problem+json`, localized และมี stable `code`/`traceId`

- [ ] **Step 7: Commit the error contract**

```bash
git add backend/src/TanErp.Api backend/tests/TanErp.IntegrationTests/Api
git commit -m "fix(api): enforce localized problem details"
```

---

### Task 4: Clear the Active TanStack Query Cache on Session Changes

**Files:**
- Modify: `frontend/src/lib/query/query-client.ts`
- Modify: `frontend/src/lib/auth/auth-session.ts`
- Modify: `frontend/src/features/auth/components/access-gate.tsx`
- Modify: `frontend/src/components/layout/erp-shell.tsx`
- Modify: `frontend/src/lib/auth/auth-session.test.ts`
- Modify: `frontend/src/features/auth/components/access-gate.test.tsx`
- Modify: `frontend/src/components/layout/erp-shell.test.tsx`

**Interfaces:**
- Consumes: `QueryClient` จาก nearest `QueryClientProvider`
- Produces: `signOutSession(client: QueryClient): Promise<void>` และ `subscribeToAuthChanges(client, callback): () => void` โดยไม่มี singleton fallback

- [ ] **Step 1: Add tests that reject an implicit singleton**

แก้ unit tests ให้ function ต้องรับ client และเพิ่ม component assertions ว่า provider client ถูกส่งไป:

```tsx
const providerClient = new QueryClient();
providerClient.setQueryData(["currentUser", "uid-a", "th"], { user: { id: "a" } });

render(
  <QueryClientProvider client={providerClient}>
    <AccessGate>{() => <div>Protected</div>}</AccessGate>
  </QueryClientProvider>
);

// Trigger logout/user change, then:
expect(providerClient.getQueryData(["currentUser", "uid-a", "th"])).toBeUndefined();
```

- [ ] **Step 2: Run auth and shell tests and confirm the regression test fails**

```bash
npm --prefix frontend test -- src/lib/auth/auth-session.test.ts src/features/auth/components/access-gate.test.tsx src/components/layout/erp-shell.test.tsx
```

Expected: FAIL เพราะ active provider cache ยังไม่ถูก clear

- [ ] **Step 3: Remove the exported singleton and require explicit QueryClient**

เก็บเฉพาะ factory ใน `query-client.ts` และเปลี่ยน signatures:

```ts
export async function signOutSession(client: QueryClient): Promise<void> {
  await client.cancelQueries();
  client.clear();
  await firebaseSignOut(auth);
}

export function subscribeToAuthChanges(
  client: QueryClient,
  callback: (user: User | null) => void
): () => void {
  // Existing UID transition logic using client.clear()
}
```

- [ ] **Step 4: Pass the provider instance from components**

ใน `AccessGate` และ `ErpShell`:

```tsx
const queryClient = useQueryClient();
```

แล้วใช้:

```tsx
subscribeToAuthChanges(queryClient, setFirebaseUser);
await signOutSession(queryClient);
```

เพิ่ม `queryClient` ใน effect dependency arrays ที่เกี่ยวข้อง

- [ ] **Step 5: Run the focused and full Frontend tests**

```bash
npm --prefix frontend test -- src/lib/auth/auth-session.test.ts src/features/auth/components/access-gate.test.tsx src/components/layout/erp-shell.test.tsx
npm --prefix frontend run typecheck
```

Expected: PASS และไม่มี exported `queryClient` singleton เหลืออยู่

- [ ] **Step 6: Commit session isolation**

```bash
git add frontend/src/lib/auth frontend/src/lib/query frontend/src/features/auth frontend/src/components/layout
git commit -m "fix(auth): clear active query cache on logout"
```

---

### Task 5: Localize Login Failures and Fix Foundation Accessibility

**Files:**
- Modify: `frontend/src/features/auth/components/login-form.tsx`
- Modify: `frontend/src/features/auth/components/login-form.test.tsx`
- Modify: `frontend/src/components/ui/Input.tsx`
- Modify: `frontend/src/components/ui/Button.test.tsx`
- Modify: `frontend/src/app/layout.tsx`
- Modify: `frontend/e2e/auth.spec.ts`

**Interfaces:**
- Consumes: Firebase error as `unknown`, next-intl request locale
- Produces: `getFirebaseAuthErrorKey(error: unknown): "invalidCredentials" | "signInFailed"`; keyboard-accessible password control; correct `<html lang>`

- [ ] **Step 1: Add failing tests for unknown errors and keyboard access**

เพิ่ม LoginForm test:

```tsx
vi.mocked(signInWithEmail).mockRejectedValueOnce(
  new Error("Firebase internal project detail")
);
// Submit valid values
expect(await screen.findByRole("alert")).toHaveTextContent(
  "ไม่สามารถเข้าสู่ระบบได้ กรุณาลองใหม่อีกครั้ง"
);
expect(screen.queryByText(/Firebase internal project detail/)).toBeNull();
```

เพิ่ม Input assertion:

```tsx
const reveal = screen.getByRole("button", { name: "แสดงรหัสผ่าน" });
expect(reveal.tabIndex).toBe(0);
expect(reveal).toHaveStyle({ minWidth: "44px", minHeight: "44px" });
```

เพิ่ม E2E assertions หลังเข้า `/th/login` และหลังเปลี่ยน `/en`:

```ts
await expect(page.locator("html")).toHaveAttribute("lang", "th");
// after switching locale
await expect(page.locator("html")).toHaveAttribute("lang", "en");
```

- [ ] **Step 2: Run tests and confirm current behavior fails**

```bash
npm --prefix frontend test -- src/features/auth/components/login-form.test.tsx src/components/ui/Button.test.tsx
```

Expected: raw error ถูกแสดง และ password reveal ไม่อยู่ใน keyboard tab order

- [ ] **Step 3: Narrow Firebase errors through an allowlist**

ใช้ `FirebaseError` โดยไม่ assert arbitrary object:

```ts
import { FirebaseError } from "firebase/app";

function getFirebaseAuthErrorKey(
  error: unknown
): "invalidCredentials" | "signInFailed" {
  if (!(error instanceof FirebaseError)) return "signInFailed";

  return [
    "auth/invalid-credential",
    "auth/user-not-found",
    "auth/wrong-password",
  ].includes(error.code)
    ? "invalidCredentials"
    : "signInFailed";
}
```

catch ต้องเรียก `setErrorMessage(t(getFirebaseAuthErrorKey(err)))` เท่านั้น

- [ ] **Step 4: Restore keyboard focus and 44px target**

ลบ `tabIndex={-1}` และใช้ขนาดขั้นต่ำ:

```tsx
style={{
  position: "absolute",
  right: 0,
  minWidth: "44px",
  minHeight: "44px",
  display: "inline-flex",
  alignItems: "center",
  justifyContent: "center",
  background: "transparent",
  border: 0,
  cursor: "pointer",
}}
```

- [ ] **Step 5: Derive document language from next-intl**

เปลี่ยน root layout เป็น async และใช้ locale ที่ request config ตรวจแล้ว:

```tsx
import { getLocale } from "next-intl/server";

export default async function RootLayout({ children }: RootLayoutProps) {
  const locale = await getLocale();
  return (
    <html lang={locale}>
      <body><AppProviders>{children}</AppProviders></body>
    </html>
  );
}
```

- [ ] **Step 6: Run component, type and E2E tests**

```bash
npm --prefix frontend run typecheck
npm --prefix frontend test -- src/features/auth/components/login-form.test.tsx
npm --prefix frontend run test:e2e
```

Expected: raw SDK message ไม่ปรากฏ, reveal button ใช้ keyboard ได้ และ `html[lang]` ตรงกับ route

- [ ] **Step 7: Commit localization and accessibility fixes**

```bash
git add frontend/src/features/auth frontend/src/components/ui/Input.tsx frontend/src/app/layout.tsx frontend/e2e/auth.spec.ts
git commit -m "fix(auth): localize errors and restore accessibility"
```

---

### Task 6: Make CI Exercise the Real E2E Stack

**Files:**
- Modify: `.github/workflows/verify.yml`
- Modify: `frontend/playwright.config.ts`
- Modify: `docs/05-engineering/foundation-login-runbook.md`

**Interfaces:**
- Consumes: `PLAYWRIGHT_TEST_BASE_URL`
- Produces: CI and local E2E use an explicit, identical Frontend port

- [ ] **Step 1: Add an explicit CI base URL and server port**

ใช้ port 3005 ตาม Makefile/local convention:

```yaml
- name: Start Frontend Server (Background)
  env:
    NEXT_PUBLIC_API_BASE_URL: "http://localhost:5000"
  run: |
    npm --prefix frontend run dev -- -p 3005 &
    until curl --fail --silent http://localhost:3005/th/login > /dev/null; do
      sleep 1
    done

- name: Run Playwright E2E Acceptance Journey
  env:
    PLAYWRIGHT_TEST_BASE_URL: "http://localhost:3005"
  run: npm --prefix frontend run test:e2e
```

- [ ] **Step 2: Bound the server wait loop**

แทน infinite loop ด้วย 60 attempts และ fail พร้อม diagnostic:

```bash
for attempt in $(seq 1 60); do
  curl --fail --silent http://localhost:3005/th/login > /dev/null && exit 0
  sleep 1
done
echo "Frontend did not become ready on port 3005"
exit 1
```

- [ ] **Step 3: Verify the workflow and local command use the same URL**

```bash
rg -n '3000|3005|PLAYWRIGHT_TEST_BASE_URL' .github/workflows/verify.yml frontend/playwright.config.ts docs/05-engineering/foundation-login-runbook.md Makefile
```

Expected: Foundation E2E path ใช้ 3005 สอดคล้องกันทั้งหมด; 3000 ไม่มีอยู่ใน E2E setup

- [ ] **Step 4: Run the real local acceptance journey**

เริ่ม PostgreSQL, Firebase emulator, API และ Frontend ตาม runbook แล้วรัน:

```bash
PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e
```

Expected: Playwright journey ผ่านโดยไม่ mock `/api/v1/me`

- [ ] **Step 5: Commit CI correction**

```bash
git add .github/workflows/verify.yml frontend/playwright.config.ts docs/05-engineering/foundation-login-runbook.md
git commit -m "fix(ci): align foundation e2e server port"
```

---

### Task 7: Remove Deferred Business and Shared Framework Scope

**Files:**
- Delete: `frontend/src/components/common/FormActionBar.tsx`
- Delete: `frontend/src/components/common/PageHeader.tsx`
- Modify: `frontend/src/components/common/index.ts`
- Delete: `frontend/src/components/ui/Checkbox.tsx`
- Delete: `frontend/src/components/ui/ConfirmModal.tsx`
- Delete: `frontend/src/components/ui/DataTable.tsx`
- Delete: `frontend/src/components/ui/Drawer.tsx`
- Delete: `frontend/src/components/ui/EmptyState.tsx`
- Delete: `frontend/src/components/ui/Modal.tsx`
- Delete: `frontend/src/components/ui/Select.tsx`
- Delete: `frontend/src/components/ui/StatusBadge.tsx`
- Delete: `frontend/src/components/ui/Switch.tsx`
- Delete: `frontend/src/components/ui/Textarea.tsx`
- Modify: `frontend/src/components/ui/index.ts`
- Delete: `frontend/src/hooks/useBodyScrollLock.ts`
- Delete: `frontend/src/hooks/useConfirm.tsx`
- Delete: `frontend/src/hooks/useDataTableState.ts`
- Delete: `frontend/src/hooks/useDebounce.ts`
- Delete: `frontend/src/hooks/useDisclosure.ts`
- Delete: `frontend/src/hooks/useDisclosure.test.ts`
- Delete: `frontend/src/hooks/useFormDirtyWarning.ts`
- Delete: `frontend/src/hooks/useRowSelection.ts`
- Delete: `frontend/src/hooks/useRowSelection.test.ts`
- Delete: `frontend/src/hooks/index.ts`
- Delete: `frontend/src/lib/utils/deferred-upload.ts`
- Delete: `frontend/src/lib/utils/deferred-upload.test.ts`
- Delete: `frontend/src/lib/utils/formatters.ts`
- Delete: `frontend/src/lib/utils/formatters.test.ts`
- Delete: `frontend/src/lib/utils/line-items.ts`
- Delete: `frontend/src/lib/utils/problem-details.ts`
- Modify: `frontend/src/lib/utils/index.ts`
- Modify: `docs/frontend/shared-components-guide.md`

**Interfaces:**
- Consumes: actual Foundation imports
- Produces: shared surface จำกัดที่ `Button`, `Input`, `MonoSpinner`, `Icons` และ `cn`; ไม่มี pricing/VAT หรือ future-form behavior

- [ ] **Step 1: Prove the deferred files have no Foundation consumer**

```bash
rg -n 'DataTable|Drawer|ConfirmModal|FormActionBar|PageHeader|useDataTableState|useConfirm|line-items|deferred-upload|formatters' frontend/src --glob '!**/index.ts' --glob '!**/*.test.*'
```

Expected: พบเฉพาะ imports ระหว่าง deferred files ด้วยกัน ไม่มี import จาก `features/auth`, `components/layout` หรือ route ของ Foundation

- [ ] **Step 2: Delete the unused future framework files**

ลบเฉพาะรายการใน Files ด้านบน ห้ามลบ `Button.tsx`, `Input.tsx`, `MonoSpinner.tsx`, `components/common/Icons.tsx`, `lib/utils/cn.ts` หรือ tests ที่รองรับ Foundation

- [ ] **Step 3: Reduce barrel exports to actual Foundation primitives**

`components/ui/index.ts`:

```ts
export * from "./Button";
export * from "./Input";
export * from "./MonoSpinner";
```

`components/common/index.ts`:

```ts
export * from "./Icons";
```

`lib/utils/index.ts`:

```ts
export * from "./cn";
```

- [ ] **Step 4: Remove deferred component claims from documentation**

แก้ `docs/frontend/shared-components-guide.md` ให้กล่าวเฉพาะ primitives ที่ Foundation ใช้งานจริง และเชื่อมไปยัง plan นี้สำหรับรายการที่ถอนออก ห้ามเก็บตัวอย่าง Customer, Item, Approval, Drawer หรือ financial calculation เป็นของที่พร้อมใช้แล้ว

- [ ] **Step 5: Run dead-import, type and test checks**

```bash
rg -n 'DataTable|Drawer|ConfirmModal|line-items|deferred-upload' frontend/src
npm --prefix frontend run typecheck
npm --prefix frontend test
npm --prefix frontend run build
```

Expected: `rg` ไม่พบ symbol ที่ถอน, typecheck/tests/build PASS

- [ ] **Step 6: Commit scope reduction**

```bash
git add -A frontend/src frontend/src/components frontend/src/hooks frontend/src/lib/utils docs/frontend/shared-components-guide.md
git commit -m "refactor(frontend): trim foundation shared surface"
```

---

### Task 8: Re-run the Gate and Correct Verification Evidence

**Files:**
- Modify: `docs/05-engineering/foundation-login-verification.md`
- Modify: `docs/README.md` only if the plan/verification index is missing

**Interfaces:**
- Consumes: actual command outputs and resolved dependency versions
- Produces: a truthful verification record for commit under review

- [ ] **Step 1: Verify required runtimes before claiming results**

```bash
dotnet --version
node --version
npm --version
```

Expected: .NET `10.0.400`, Node `v24.20.x`; if unavailable, stop and record the gate as blocked rather than passed

- [ ] **Step 2: Run whitespace, fixture and Backend gates**

```bash
git diff --check main...HEAD
npm run test:fixtures
dotnet restore backend/TanErp.slnx
dotnet build backend/TanErp.slnx --no-restore
dotnet test backend/TanErp.slnx --no-build
```

Expected: every command exits 0

- [ ] **Step 3: Run Frontend and contract gates**

```bash
npm --prefix frontend run check:api
npm --prefix frontend run lint
npm --prefix frontend run typecheck
npm --prefix frontend run test
npm --prefix frontend run build
```

Expected: every command exits 0; generated OpenAPI TypeScript has no diff

- [ ] **Step 4: Run the unmocked acceptance journey**

```bash
PLAYWRIGHT_TEST_BASE_URL=http://localhost:3005 npm --prefix frontend run test:e2e
```

Expected: login, `/api/v1/me`, locale switch, logout/cache clear และ no-membership recovery ผ่านกับ Firebase emulator + API + PostgreSQL จริง

- [ ] **Step 5: Record exact evidence instead of expected values**

แก้ verification record ให้มี:

- tested commit SHA จาก `git rev-parse HEAD`
- command, exit code, test count และเวลารันจริง
- Firebase Admin .NET target/actual `3.6.0`
- ผล negative cross-organization tests
- ผล localized 401/403/500 tests
- ผล logout cache isolation และ `html[lang]`/keyboard/44px assertions
- สถานะ `Blocked` หาก Backend หรือ E2E ยังไม่ได้รันจริง; ใช้ `Verified & Accepted` เฉพาะเมื่อทุก gate ผ่าน

- [ ] **Step 6: Check links and documentation diff**

```bash
git diff --check main...HEAD
git diff -- docs/05-engineering/foundation-login-verification.md docs/README.md
```

Expected: ไม่มี trailing whitespace และ plan/verification links ถูกต้อง

- [ ] **Step 7: Commit verified evidence**

```bash
git add docs/05-engineering/foundation-login-verification.md docs/README.md
git commit -m "docs(verification): record foundation remediation gate"
```

---

## Final Review Checklist

- [ ] PostgreSQL ปฏิเสธ Membership → Branch และ Membership → Role ที่ข้าม Organization
- [ ] `/api/v1/me` ไม่คืน permission ข้าม tenant และยัง filter active state ตาม Foundation contract
- [ ] ไม่มี runtime/design-time connection string fallback หรือ credential-bearing log
- [ ] cancellation ไม่ถูกแปลงเป็น 401
- [ ] expected และ unexpected API errors เป็น localized RFC 9457 พร้อม stable `code` และ `traceId`
- [ ] logout, invalid session และ UID switch ล้าง QueryClient instance ที่ Provider ใช้จริงก่อน render ผู้ใช้ถัดไป
- [ ] Login ไม่แสดง raw Firebase SDK message
- [ ] password reveal ใช้ keyboard ได้และมี target อย่างน้อย 44px
- [ ] `/th` ใช้ `<html lang="th">` และ `/en` ใช้ `<html lang="en">`
- [ ] CI เปิดและทดสอบ Frontend บน port เดียวกัน พร้อม bounded readiness wait
- [ ] ไม่มี future business calculation หรือ unused form/table framework ใน Foundation slice
- [ ] verification record ตรงกับ dependency และ command output จริง
- [ ] `git status --short` ไม่มี generated/build artifacts หรือ secret

## Self-Review Notes

- **Spec coverage:** Task 1 ปิด tenant boundary; Tasks 2–3 ปิด config/error/security contract; Tasks 4–5 ปิด auth isolation/localization/accessibility; Task 6 ปิด CI/E2E; Task 7 คืน scope; Task 8 ยืนยัน gate และหลักฐาน
- **Completeness scan:** wildcard ของชื่อ migration เป็น artifact ที่ EF Core ตั้ง timestamp ให้อัตโนมัติจากคำสั่งใน Task 1; ทุก implementation step มีคำสั่งหรือตัวอย่างที่ตรวจสอบได้
- **Type consistency:** `MembershipRole` รับ `organizationId` แบบ `Guid`; auth functions รับ `QueryClient` แบบบังคับ; Problem Details ใช้ stable code `INTERNAL_SERVER_ERROR`; E2E ใช้ port `3005` ทุกจุด
- **Known execution prerequisite:** เครื่องที่ใช้ review ปัจจุบันมี .NET SDK `9.0.300`; ต้องใช้ SDK `10.0.400` ก่อน Task 1–3 และ Task 8 จึงจะยืนยัน Backend gate ได้
