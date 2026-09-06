# Foundation Login and Current User Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** เปลี่ยน `tan-erp` เข้าสู่ Application Implementation และส่งมอบ Foundation Vertical Slice แรกที่ผู้ใช้ Login ผ่าน Firebase แล้วเห็น Organization, Branch และ Effective Permissions ที่ Backend อ่านจาก PostgreSQL ผ่าน `GET /api/v1/me`

**Architecture:** ใช้ Monorepo ที่มี Backend แบบ Clean Architecture 4 Projects และ Frontend แบบ Next.js App Router แยกตาม Feature. Firebase เป็น Identity Provider เท่านั้น ส่วน PostgreSQL เป็น Source of Truth ของ User Mapping, Membership, Role, Permission และ Scope; OpenAPI ที่ Backend สร้างเป็นต้นทางของ Type ฝั่ง Frontend.

**Tech Stack:** .NET 10 LTS (SDK 10.0.400, runtime/ASP.NET Core/EF Core 10.0.11), PostgreSQL 17, Firebase Admin .NET 3.6.0, Next.js 16.3.4, React/React DOM 19.2.8, Node.js 24.20.0 LTS, TypeScript 7.0.2, TanStack Query 5.102.8, Zod 4.5.4, Firebase JS 12.18.0, Vitest 5.0.0 และ Playwright 1.63.0

## Global Constraints

- Product ต้องเรียกว่า **Project ERP** หรือ **tan-erp**; MRP เป็น Future Module ภายใน ERP
- ภาษาไทย (`th`) เป็นค่าเริ่มต้นและรองรับภาษาอังกฤษ (`en`)
- ขอบเขตนี้จบที่ Login → Current User → ERP shell; ยังไม่สร้าง Customer, Opportunity, Site, Survey, Estimate, Item หรือ Module อื่น
- Backend ใช้ Clean Architecture 4 Projects: `TanErp.Domain`, `TanErp.Application`, `TanErp.Infrastructure`, `TanErp.Api`
- Firebase ยืนยัน Identity เท่านั้น; Role, Permission และ Scope มาจาก PostgreSQL ทุก Request
- EF Core เป็น Write/Transaction path; Slice นี้ยังไม่มีเหตุผลให้ใช้ Dapper/Raw SQL
- API Error ใช้ RFC 9457 Problem Details พร้อม stable `code`, `traceId` และข้อความไทย/อังกฤษ
- OpenAPI ที่ Backend สร้างต้อง generate Type ฝั่ง Frontend; ห้ามแก้ไฟล์ใต้ `frontend/src/generated/api/` ด้วยมือ
- Test data ต้องเป็นข้อมูลสังเคราะห์และใช้คำว่า `TEST_ONLY`; ห้าม Seed ลง Production โดยอัตโนมัติ
- `SURVEY-BASELINE-v1` เป็น system-owned test fixture สำหรับ readiness gate เท่านั้น ไม่ใช่ Business Template ที่อนุมัติแล้ว
- UI ใช้ semantic HTML, visible focus, control สูงอย่างน้อย 44px, form error ที่สัมพันธ์กับ field, `aria-live` สำหรับสถานะสำคัญ และรองรับ `prefers-reduced-motion`
- ห้าม Commit secret, Firebase service-account JSON, token, `.env` จริง หรือข้อมูลลูกค้าจริง
- ทุก Task ใช้ TDD: เขียน test ที่ fail → ยืนยัน failure → เขียน implementation ขั้นต่ำ → ยืนยัน pass → commit

---

## Planned File Map

```text
tan-erp/
├── AGENTS.md
├── README.md
├── .editorconfig
├── .gitattributes
├── .gitignore
├── .nvmrc
├── package.json
├── backend/
│   ├── AGENTS.md
│   ├── README.md
│   ├── TanErp.slnx
│   ├── global.json
│   ├── Directory.Build.props
│   ├── Directory.Packages.props
│   ├── src/
│   │   ├── TanErp.Domain/
│   │   │   ├── Common/
│   │   │   ├── IdentityAccess/
│   │   │   └── Organization/
│   │   ├── TanErp.Application/
│   │   │   ├── Common/
│   │   │   └── IdentityAccess/CurrentUser/GetCurrentUser/
│   │   ├── TanErp.Infrastructure/
│   │   │   ├── Identity/
│   │   │   └── Persistence/
│   │   └── TanErp.Api/
│   │       ├── Authentication/
│   │       ├── Contracts/IdentityAccess/
│   │       ├── ErrorHandling/
│   │       ├── OpenApi/
│   │       ├── Resources/
│   │       └── Program.cs
│   └── tests/
│       ├── TanErp.UnitTests/
│       ├── TanErp.IntegrationTests/
│       └── TanErp.ArchitectureTests/
├── frontend/
│   ├── AGENTS.md
│   ├── README.md
│   ├── package.json
│   ├── next.config.ts
│   ├── playwright.config.ts
│   ├── src/
│   │   ├── app/[locale]/
│   │   ├── components/layout/
│   │   ├── features/auth/
│   │   ├── generated/api/
│   │   ├── lib/api/
│   │   ├── lib/auth/
│   │   ├── lib/i18n/
│   │   ├── lib/query/
│   │   ├── messages/
│   │   └── providers/
│   └── e2e/auth.spec.ts
├── fixtures/
│   ├── survey-baseline-v1.schema.json
│   └── SURVEY-BASELINE-v1.json
├── deploy/
│   ├── compose.yml
│   └── .env.example
├── contracts/openapi/tan-erp.v1.json
└── .github/workflows/verify.yml
```

### Task 1: Approve the phase, runtime ADR and Current User contract

**Files:**
- Modify: `AGENTS.md`
- Modify: `docs/00-overview/implementation-roadmap.md`
- Modify: `docs/03-contracts/api-conventions.md`
- Modify: `docs/03-contracts/error-contract.md`
- Create: `docs/adr/0010-foundation-application-runtime.md`
- Modify: `docs/adr/README.md`

**Interfaces:**
- Produces: approved Application Implementation phase, pinned runtime decision and exact HTTP contract consumed by Tasks 5–7

- [ ] **Step 1: Preserve unrelated working-tree changes**

Run:

```bash
git status --short
git diff -- docs/00-overview/implementation-roadmap.md docs/02-architecture/backend-architecture.md docs/02-architecture/frontend-architecture.md
```

Expected: retain all existing uncommitted documentation edits; later patches may extend the roadmap but must not discard those changes.

- [ ] **Step 2: Record the explicit phase change**

Change `AGENTS.md` current phase to:

```markdown
## Current phase

This repository is in **Application Implementation**. The authorized implementation boundary is the Foundation Login and Current User vertical slice documented in `docs/superpowers/plans/2026-09-06-foundation-login-current-user.md`. Do not add CRM, Survey, Estimation, Item, Commercial, Project, Procurement, Inventory, Production, or MRP behavior without a separately approved implementation task.
```

Update the Foundation row in `docs/00-overview/implementation-roadmap.md` to state that the Login/Current User slice is authorized, while later rows and modules remain gated.

- [ ] **Step 3: Record ADR 0010**

Create an Accepted ADR with these exact decisions:

```text
Backend SDK: .NET SDK 10.0.400
Backend target: net10.0
Runtime/ASP.NET Core/EF Core patch line: 10.0.11
Frontend runtime: Node.js 24.20.0 LTS
Framework: Next.js 16.3.4 + React 19.2.8
Database: PostgreSQL 17
Identity adapters: Firebase Admin .NET 3.6.0 and Firebase JS 12.18.0
```

The ADR must cite the official .NET support/download pages, Node release page, Next.js release source, React version page and Firebase release notes. State why Node 26 Current and .NET 11 Preview are rejected for the first production foundation.

- [ ] **Step 4: Define `GET /api/v1/me` before implementation**

Add this success contract to `docs/03-contracts/api-conventions.md`:

```json
{
  "user": {
    "id": "019a3cf8-96f0-7c9f-b207-93aa818f4a10",
    "displayName": "ผู้ใช้ TEST_ONLY",
    "email": "foundation-user@example.test"
  },
  "memberships": [
    {
      "id": "019a3cf8-96f0-7c9f-b207-93aa818f4a11",
      "organization": {
        "id": "019a3cf8-96f0-7c9f-b207-93aa818f4a12",
        "name": "TEST_ONLY Project ERP"
      },
      "branch": {
        "id": "019a3cf8-96f0-7c9f-b207-93aa818f4a13",
        "name": "สาขาทดสอบ"
      },
      "permissions": [
        {
          "key": "organizations.read",
          "scope": "organization",
          "scopeId": "019a3cf8-96f0-7c9f-b207-93aa818f4a12"
        }
      ]
    }
  ]
}
```

Contract rules: return only active user/memberships/organization/branch/role-permission links; sort memberships by Organization name then Membership ID and permissions by key/scope/scopeId; do not return Firebase UID, Role names or Custom Claims.

- [ ] **Step 5: Define authentication error cases**

Add these stable codes to `docs/03-contracts/error-contract.md`:

| Code | HTTP | Meaning |
| --- | ---: | --- |
| `AUTHENTICATION_REQUIRED` | 401 | Bearer token is missing |
| `AUTHENTICATION_INVALID` | 401 | Token cannot be verified, has expired, or belongs to another Firebase project |
| `USER_ACCESS_DISABLED` | 403 | Internal User is disabled |
| `ACTIVE_MEMBERSHIP_REQUIRED` | 403 | Identity is valid but no active Membership is available |
| `PERMISSION_DENIED` | 403 | Active Membership exists but the requested permission is absent |

Include Thai and English example Problem Details; stable `code`, HTTP status and `type` must be identical across locales.

- [ ] **Step 6: Verify and commit the governing contract**

Run:

```bash
rg -n "Application Implementation|GET /api/v1/me|ACTIVE_MEMBERSHIP_REQUIRED|10.0.400|24.20.0" AGENTS.md docs
git diff --check
```

Expected: all five concepts are present and Markdown has no whitespace errors.

```bash
git add AGENTS.md docs/00-overview/implementation-roadmap.md docs/03-contracts/api-conventions.md docs/03-contracts/error-contract.md docs/adr/0010-foundation-application-runtime.md docs/adr/README.md
git commit -m "docs: authorize foundation implementation"
```

### Task 2: Establish root governance and deterministic fixtures

**Files:**
- Create: `.editorconfig`
- Create: `.gitattributes`
- Create: `.gitignore`
- Create: `.nvmrc`
- Create: `package.json`
- Create: `package-lock.json`
- Modify: `README.md`
- Create: `fixtures/survey-baseline-v1.schema.json`
- Create: `fixtures/SURVEY-BASELINE-v1.json`
- Create: `fixtures/survey-baseline.test.mjs`

**Interfaces:**
- Produces: reproducible root commands, secret/build exclusions and validated system-owned Survey baseline fixture

- [ ] **Step 1: Write the failing Survey fixture test**

Create a Node test that validates the fixture with Ajv and then asserts:

```js
assert.equal(fixture.templateCode, "SURVEY-BASELINE-v1");
assert.equal(fixture.owner, "system");
assert.equal(fixture.classification, "TEST_ONLY");
assert.equal(fixture.status, "baseline");
assert.deepEqual(fixture.requiredSections, ["site", "measurement", "checklist", "evidence"]);
assert.equal(fixture.businessApproved, false);
```

Run `node --test fixtures/survey-baseline.test.mjs`.

Expected: FAIL because the schema and fixture do not exist yet.

- [ ] **Step 2: Create the schema and fixture**

The JSON Schema must set `additionalProperties: false`, require all six fields asserted above, constrain `templateCode` to `SURVEY-BASELINE-v1`, `owner` to `system`, `classification` to `TEST_ONLY`, `status` to `baseline`, and `businessApproved` to `false`.

The fixture contains no customer name, phone, email, address, coordinate, photograph or production identifier.

- [ ] **Step 3: Add root runtime and hygiene files**

Create `.nvmrc` with `24.20.0`. `.gitignore` must include:

```gitignore
.DS_Store
.env
.env.*
!.env.example
**/appsettings.*.Local.json
**/bin/
**/obj/
**/node_modules/
**/.next/
**/coverage/
**/playwright-report/
**/test-results/
**/service-account*.json
```

Create a private root `package.json` with Ajv `8.17.1` as an exact dev dependency and these scripts:

```json
{
  "name": "tan-erp",
  "private": true,
  "engines": { "node": "24.20.x" },
  "devDependencies": { "ajv": "8.17.1" },
  "scripts": {
    "test:fixtures": "node --test fixtures/survey-baseline.test.mjs",
    "verify:backend": "dotnet restore backend/TanErp.slnx && dotnet build backend/TanErp.slnx --no-restore && dotnet test backend/TanErp.slnx --no-build",
    "verify:frontend": "npm --prefix frontend run verify",
    "verify": "npm run test:fixtures && npm run verify:backend && npm run verify:frontend"
  }
}
```

Run `npm install --package-lock-only` and commit the resulting root lockfile.

- [ ] **Step 4: Document safe configuration**

Update root `README.md` with prerequisites, `npm run verify`, local startup order and these ownership statements: Firebase public web configuration is not an authorization source; service-account credentials remain outside the repository; PostgreSQL contains no production/customer seed.

- [ ] **Step 5: Verify and commit root foundation**

Run:

```bash
node --test fixtures/survey-baseline.test.mjs
git check-ignore .env frontend/.next backend/src/TanErp.Api/bin/Debug/net10.0/TanErp.Api.dll service-account.json
git diff --check
```

Expected: fixture test passes and all four sensitive/generated paths are ignored.

```bash
git add .editorconfig .gitattributes .gitignore .nvmrc package.json package-lock.json README.md fixtures
git commit -m "chore: establish application foundation"
```

### Task 3: Scaffold the backend and enforce Clean Architecture

**Files:**
- Create: `backend/global.json`
- Create: `backend/TanErp.slnx`
- Create: `backend/Directory.Build.props`
- Create: `backend/Directory.Packages.props`
- Create: `backend/AGENTS.md`
- Create: `backend/README.md`
- Create: `backend/src/TanErp.Domain/TanErp.Domain.csproj`
- Create: `backend/src/TanErp.Application/TanErp.Application.csproj`
- Create: `backend/src/TanErp.Infrastructure/TanErp.Infrastructure.csproj`
- Create: `backend/src/TanErp.Api/TanErp.Api.csproj`
- Create: `backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj`
- Create: `backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj`
- Create: `backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj`
- Test: `backend/tests/TanErp.ArchitectureTests/LayerDependencyTests.cs`

**Interfaces:**
- Produces: buildable `net10.0` solution with dependency direction `Api → Application ← Infrastructure` and `Application → Domain ← Infrastructure`

- [ ] **Step 1: Pass the SDK gate**

Run:

```bash
dotnet --list-sdks
node --version
```

Expected before proceeding: .NET SDK `10.0.400` is installed and Node reports `v24.20.0`. The planning host currently reports .NET `9.0.300` and Node `v26.0.0`; install/select the pinned versions before scaffolding so generated files match CI.

- [ ] **Step 2: Scaffold exactly four production projects and three test projects**

Use `dotnet new sln --format slnx`, `dotnet new classlib`, `dotnet new webapi --use-controllers` and `dotnet new xunit`. Remove template `WeatherForecast` and `Class1` files. Add all projects to `TanErp.slnx`.

- [ ] **Step 3: Add only allowed project references**

```text
TanErp.Application -> TanErp.Domain
TanErp.Infrastructure -> TanErp.Application, TanErp.Domain
TanErp.Api -> TanErp.Application, TanErp.Infrastructure
TanErp.UnitTests -> TanErp.Application, TanErp.Domain
TanErp.IntegrationTests -> TanErp.Api, TanErp.Infrastructure
TanErp.ArchitectureTests -> all four production projects
```

- [ ] **Step 4: Pin backend packages centrally**

Set `TargetFramework` to `net10.0`, nullable and implicit usings on, warnings as errors, deterministic builds and central package management. Pin:

```xml
<PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.11" />
<PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.11" />
<PackageVersion Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.11" />
<PackageVersion Include="Microsoft.Extensions.ApiDescription.Server" Version="10.0.11" />
<PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.3" />
<PackageVersion Include="FirebaseAdmin" Version="3.6.0" />
<PackageVersion Include="Swashbuckle.AspNetCore" Version="10.2.3" />
<PackageVersion Include="Testcontainers.PostgreSql" Version="4.14.0" />
<PackageVersion Include="TngTech.ArchUnitNET.xUnit" Version="0.13.4" />
<PackageVersion Include="xunit" Version="2.9.3" />
```

- [ ] **Step 5: Write failing architecture tests**

Assert these rules using ArchUnitNET:

```csharp
Types().That().ResideInAssembly(DomainAssembly)
    .Should().NotDependOnAny(ApplicationAssembly, InfrastructureAssembly, ApiAssembly);

Types().That().ResideInAssembly(ApplicationAssembly)
    .Should().NotDependOnAny(InfrastructureAssembly, ApiAssembly);

Types().That().ResideInAssembly(InfrastructureAssembly)
    .Should().NotDependOnAny(ApiAssembly);
```

Run `dotnet test backend/tests/TanErp.ArchitectureTests` and confirm failure until references and namespaces are corrected.

- [ ] **Step 6: Add backend guidance and verify**

`backend/AGENTS.md` must require feature folders, thin controllers, EF Core writes, no Generic Repository, no Role-name checks, no secret logging and localized Problem Details. `backend/README.md` must list restore/build/test/migration commands and environment variable names without values.

Run:

```bash
dotnet restore backend/TanErp.slnx
dotnet build backend/TanErp.slnx --no-restore
dotnet test backend/TanErp.slnx --no-build
git diff --check
```

Expected: build and architecture tests pass.

```bash
git add backend
git commit -m "feat: scaffold clean backend"
```

### Task 4: Create the identity, membership and audit database baseline

**Files:**
- Create: `backend/src/TanErp.Domain/Common/Entity.cs`
- Create: `backend/src/TanErp.Domain/Organization/Organization.cs`
- Create: `backend/src/TanErp.Domain/Organization/Branch.cs`
- Create: `backend/src/TanErp.Domain/Organization/Membership.cs`
- Create: `backend/src/TanErp.Domain/IdentityAccess/User.cs`
- Create: `backend/src/TanErp.Domain/IdentityAccess/Role.cs`
- Create: `backend/src/TanErp.Domain/IdentityAccess/Permission.cs`
- Create: `backend/src/TanErp.Domain/IdentityAccess/MembershipRole.cs`
- Create: `backend/src/TanErp.Domain/IdentityAccess/RolePermission.cs`
- Create: `backend/src/TanErp.Domain/IdentityAccess/PermissionScope.cs`
- Create: `backend/src/TanErp.Domain/Common/AuditEvent.cs`
- Create: `backend/src/TanErp.Application/Common/Abstractions/IApplicationDbContext.cs`
- Create: `backend/src/TanErp.Application/Common/Abstractions/IClock.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/*.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_FoundationIdentityAccess.cs`
- Test: `backend/tests/TanErp.UnitTests/IdentityAccess/PermissionScopeTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/FoundationMigrationTests.cs`

**Interfaces:**
- Produces: relational source of truth for `User → Membership → Role → Permission + Scope` and append-only Audit Event storage

- [ ] **Step 1: Write failing domain tests**

Test exact valid scopes `organization`, `branch`, `project`, `own`; reject unknown values and reject a blank permission key. Test `Membership.IsActiveAt(instant)` against disabled user, inactive organization, inactive branch, start and end timestamps.

Run the focused tests and confirm they fail because domain types are absent.

- [ ] **Step 2: Implement minimal framework-free domain types**

Use `Guid` identifiers, `DateTimeOffset` UTC instants and private collections. Required public behavior:

```csharp
public bool Membership.IsActiveAt(DateTimeOffset instant);
public static PermissionScope PermissionScope.Create(string value);
public static Permission Permission.Create(string key);
```

No Domain file may import EF Core, Firebase, ASP.NET Core or Npgsql.

- [ ] **Step 3: Write failing migration integration tests**

Using PostgreSQL 17 Testcontainers, assert migration from zero creates:

```text
organization.organizations
organization.branches
identity_access.users
organization.memberships
identity_access.roles
identity_access.permissions
identity_access.membership_roles
identity_access.role_permissions
audit.audit_events
```

Assert unique constraints on `users.firebase_uid`, `(organization_id, branch_code)`, `(organization_id, normalized_role_name)` and permission key; assert foreign keys prevent a Branch or Role from crossing Organization.

- [ ] **Step 4: Implement EF Core mappings and migration**

Map names to snake_case and use `timestamptz`. Store Permission and Scope relationally, not JSONB. `audit_events` must contain `id`, `organization_id`, `actor_user_id`, `action`, `resource_type`, `resource_id`, `occurred_at_utc`, `trace_id` and JSONB `changes`; revoke application update/delete paths for Audit Event by exposing only `Add` through an Application port.

- [ ] **Step 5: Add cross-organization and rollback tests**

Seed two `TEST_ONLY` organizations inside the test transaction. Assert joins for Organization A never return Membership, Role or Permission assignment from Organization B. Migrate to zero, migrate forward again and assert the same model snapshot applies.

- [ ] **Step 6: Verify and commit persistence baseline**

Run:

```bash
dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~IdentityAccess"
dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~FoundationMigration"
dotnet test backend/TanErp.slnx
git diff --check
```

Expected: domain, migration, isolation and rollback tests pass against PostgreSQL 17.

```bash
git add backend/src/TanErp.Domain backend/src/TanErp.Application/Common backend/src/TanErp.Infrastructure/Persistence backend/tests
git commit -m "feat: add identity access persistence"
```

### Task 5: Implement Firebase verification, Current User use case and OpenAPI

**Files:**
- Create: `backend/src/TanErp.Application/Common/Results/Error.cs`
- Create: `backend/src/TanErp.Application/Common/Results/Result.cs`
- Create: `backend/src/TanErp.Application/Common/Abstractions/IVerifiedIdentity.cs`
- Create: `backend/src/TanErp.Application/Common/Abstractions/ICurrentUserReader.cs`
- Create: `backend/src/TanErp.Application/IdentityAccess/CurrentUser/GetCurrentUser/GetCurrentUserQuery.cs`
- Create: `backend/src/TanErp.Application/IdentityAccess/CurrentUser/GetCurrentUser/GetCurrentUserHandler.cs`
- Create: `backend/src/TanErp.Application/IdentityAccess/CurrentUser/GetCurrentUser/GetCurrentUserResult.cs`
- Create: `backend/src/TanErp.Infrastructure/Identity/FirebaseTokenVerifier.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/CurrentUserReader.cs`
- Create: `backend/src/TanErp.Api/Authentication/FirebaseAuthenticationHandler.cs`
- Create: `backend/src/TanErp.Api/Contracts/IdentityAccess/CurrentUserResponse.cs`
- Create: `backend/src/TanErp.Api/Controllers/CurrentUserController.cs`
- Create: `backend/src/TanErp.Api/ErrorHandling/ApiProblemDetails.cs`
- Create: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Create: `backend/src/TanErp.Api/Resources/Errors.resx`
- Create: `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Create: `backend/src/TanErp.Api/OpenApi/OpenApiConfiguration.cs`
- Test: `backend/tests/TanErp.UnitTests/IdentityAccess/GetCurrentUserHandlerTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/CurrentUserEndpointTests.cs`
- Create: `contracts/openapi/tan-erp.v1.json`

**Interfaces:**
- Consumes: `GET /api/v1/me` contract from Task 1 and tables from Task 4
- Produces: `IVerifiedIdentity.FirebaseUid`, `ICurrentUserReader.GetAsync(firebaseUid, cancellationToken)`, `GetCurrentUserHandler.Handle(query, cancellationToken)` and generated `CurrentUserResponse`

- [ ] **Step 1: Write failing use-case tests**

Cover four cases:

```text
known active user + active membership -> success with deterministic permissions
known disabled user -> USER_ACCESS_DISABLED
unknown Firebase UID -> ACTIVE_MEMBERSHIP_REQUIRED
known user without active membership -> ACTIVE_MEMBERSHIP_REQUIRED
```

Assert duplicate permissions from multiple Roles collapse by `(key, scope, scopeId)`.

- [ ] **Step 2: Implement the query and EF reader**

The Application result uses records only and exposes no EF/Firebase types. The reader uses `AsNoTracking`, selects only contract fields, filters active rows and orders server-side. It must never accept Organization ID, Role or Permission from the browser.

- [ ] **Step 3: Write failing authentication and contract tests**

Using `WebApplicationFactory`, assert:

```text
no Authorization header -> 401 AUTHENTICATION_REQUIRED
malformed/expired/wrong-project token -> 401 AUTHENTICATION_INVALID
valid token, disabled User -> 403 USER_ACCESS_DISABLED
valid token, no Membership -> 403 ACTIVE_MEMBERSHIP_REQUIRED
valid token, active Membership -> 200 exact documented response
```

Repeat one error with `Accept-Language: th`, `en` and `fr`; `fr` must fall back to Thai. Every failure has `application/problem+json`, `code`, `traceId` and no exception/SQL/token text.

- [ ] **Step 4: Implement Firebase authentication fail-closed**

`FirebaseTokenVerifier` calls Firebase Admin token verification and maps only verified UID into a ClaimsPrincipal. Configure Firebase project ID from `Firebase__ProjectId` and ADC/`GOOGLE_APPLICATION_CREDENTIALS`; never read Role/Permission custom claims. Support `FIREBASE_AUTH_EMULATOR_HOST` only outside Production.

- [ ] **Step 5: Implement Current User endpoint and localized errors**

Controller signature:

```csharp
[HttpGet("/api/v1/me")]
[Authorize]
[ProducesResponseType<CurrentUserResponse>(StatusCodes.Status200OK)]
public async Task<ActionResult<CurrentUserResponse>> Get(CancellationToken cancellationToken)
```

Map expected `Result` failures to RFC 9457 and leave unexpected exceptions to the global exception handler. Configure supported cultures exactly `th` and `en`, default `th`.

- [ ] **Step 6: Generate and verify OpenAPI**

Generate `contracts/openapi/tan-erp.v1.json` from the running/test API. Add a snapshot test that asserts path `/api/v1/me`, operation responses `200/401/403`, bearer security scheme and schemas `CurrentUserResponse`/`ApiProblemDetails` exist.

- [ ] **Step 7: Verify and commit the backend slice**

Run:

```bash
dotnet test backend/tests/TanErp.UnitTests --filter "FullyQualifiedName~GetCurrentUser"
dotnet test backend/tests/TanErp.IntegrationTests --filter "FullyQualifiedName~CurrentUserEndpoint"
dotnet test backend/TanErp.slnx
git diff --check
```

Expected: use-case, authentication, localization and OpenAPI contract tests pass.

```bash
git add backend contracts/openapi/tan-erp.v1.json
git commit -m "feat: expose current user context"
```

### Task 6: Scaffold the frontend and generate the API boundary

**Files:**
- Create: `frontend/package.json`
- Create: `frontend/package-lock.json`
- Create: `frontend/next.config.ts`
- Create: `frontend/tsconfig.json`
- Create: `frontend/eslint.config.mjs`
- Create: `frontend/vitest.config.ts`
- Create: `frontend/AGENTS.md`
- Create: `frontend/README.md`
- Create: `frontend/src/generated/api/tan-erp.v1.ts`
- Create: `frontend/src/lib/api/problem-details.ts`
- Create: `frontend/src/lib/api/api-error.ts`
- Create: `frontend/src/lib/api/api-client.ts`
- Test: `frontend/src/lib/api/api-client.test.ts`

**Interfaces:**
- Consumes: `contracts/openapi/tan-erp.v1.json`
- Produces: generated `paths["/api/v1/me"]`, typed `ApiError`, `ApiClient.getCurrentUser(token, locale, signal)`

- [ ] **Step 1: Scaffold with pinned packages**

Create a private frontend package with Node engine `24.20.x` and exact dependencies:

```json
{
  "next": "16.3.4",
  "react": "19.2.8",
  "react-dom": "19.2.8",
  "@tanstack/react-query": "5.102.8",
  "firebase": "12.18.0",
  "zod": "4.5.4"
}
```

Pin dev dependencies `typescript` 7.0.2, `vitest` 5.0.0, `@testing-library/react` 16.3.3, `@playwright/test` 1.63.0, `openapi-typescript` 7.13.0 and `eslint` 10.10.0. Use App Router, TypeScript, `src/` and plain CSS; do not install a UI component library or Tailwind for this slice.

- [ ] **Step 2: Add generated-type drift protection**

Add scripts:

```json
{
  "generate:api": "openapi-typescript ../contracts/openapi/tan-erp.v1.json -o src/generated/api/tan-erp.v1.ts",
  "check:api": "npm run generate:api && git diff --exit-code -- src/generated/api/tan-erp.v1.ts",
  "lint": "eslint .",
  "typecheck": "tsc --noEmit",
  "test": "vitest run",
  "build": "next build",
  "verify": "npm run check:api && npm run lint && npm run typecheck && npm run test && npm run build"
}
```

Run `npm run generate:api` and commit the generated file.

- [ ] **Step 3: Write failing API client tests**

Assert `getCurrentUser` sends:

```text
GET /api/v1/me
Authorization: Bearer <token>
Accept-Language: th | en
```

Assert successful JSON is returned typed; Problem Details becomes `ApiError(status, code, traceId, errors)`; non-JSON 500 becomes safe `UNKNOWN_ERROR`; `AbortSignal` cancellation stays distinguishable from server failure; raw response bodies are never used as display messages.

- [ ] **Step 4: Implement the central API client**

Use native `fetch` inside `lib/api` only. Components and feature hooks must not create API URLs or call `fetch`. Reject missing tokens before network access. Base URL comes from `NEXT_PUBLIC_API_BASE_URL` and must not contain credentials.

- [ ] **Step 5: Add frontend guidance and verify**

`frontend/AGENTS.md` must enforce generated contracts, central API calls, TanStack Query ownership of server state, no Effect for derived state, Thai/English key parity, semantic controls and accessible error/loading states.

Run:

```bash
npm --prefix frontend run generate:api
npm --prefix frontend run lint
npm --prefix frontend run typecheck
npm --prefix frontend run test
npm --prefix frontend run build
git diff --check
```

Expected: generated types are current and the empty app/client foundation passes all checks.

```bash
git add frontend
git commit -m "feat: scaffold typed frontend"
```

### Task 7: Implement Firebase Login and the accessible ERP shell

**Files:**
- Create: `frontend/src/lib/auth/firebase-client.ts`
- Create: `frontend/src/lib/auth/auth-session.ts`
- Create: `frontend/src/lib/query/query-client.ts`
- Create: `frontend/src/lib/i18n/locales.ts`
- Create: `frontend/src/providers/app-providers.tsx`
- Create: `frontend/src/features/auth/api/current-user-query.ts`
- Create: `frontend/src/features/auth/components/login-form.tsx`
- Create: `frontend/src/features/auth/components/access-gate.tsx`
- Create: `frontend/src/features/auth/index.ts`
- Create: `frontend/src/components/layout/erp-shell.tsx`
- Create: `frontend/src/app/[locale]/layout.tsx`
- Create: `frontend/src/app/[locale]/(auth)/login/page.tsx`
- Create: `frontend/src/app/[locale]/(erp)/page.tsx`
- Create: `frontend/src/app/globals.css`
- Create: `frontend/src/messages/th.json`
- Create: `frontend/src/messages/en.json`
- Test: `frontend/src/features/auth/components/login-form.test.tsx`
- Test: `frontend/src/features/auth/components/access-gate.test.tsx`
- Test: `frontend/src/lib/auth/auth-session.test.ts`

**Interfaces:**
- Consumes: Firebase JS session and `ApiClient.getCurrentUser`
- Produces: Login form, `useCurrentUser`, logout cache cleanup and ERP shell showing trusted backend context

- [ ] **Step 1: Write failing login component tests**

Test email/password controls have visible labels, submit button is keyboard reachable and at least 44px high, invalid fields set `aria-invalid` with `aria-describedby`, pending form sets `aria-busy`, duplicate submit is blocked and authentication errors appear in an `aria-live="polite"` region.

Email/password is the Local/Test Firebase Emulator path for this slice; the message catalog must not claim that a production sign-in provider has been approved.

- [ ] **Step 2: Implement Firebase client boundary**

Initialize the browser SDK once from these public variables:

```text
NEXT_PUBLIC_FIREBASE_API_KEY
NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN
NEXT_PUBLIC_FIREBASE_PROJECT_ID
NEXT_PUBLIC_FIREBASE_APP_ID
```

Call `connectAuthEmulator` only when `NEXT_PUBLIC_FIREBASE_AUTH_EMULATOR_URL` is set and `NODE_ENV !== "production"`. Keep token retrieval and `signOut` inside `lib/auth`.

- [ ] **Step 3: Write failing access-gate tests**

Cover these states independently:

```text
Firebase session loading -> Thai status text + aria-busy
no Firebase session -> redirect to /th/login
Current User loading -> status skeleton with accessible text
401 AUTHENTICATION_INVALID -> sign out, clear cache, return to login
403 ACTIVE_MEMBERSHIP_REQUIRED -> no-membership state
403 USER_ACCESS_DISABLED -> disabled-user state
500/503/network -> retry button and traceId when present
200 -> ERP shell with user, Organization, Branch and permissions from API
```

- [ ] **Step 4: Implement query/session lifecycle without derived-state Effects**

Use TanStack Query key `['current-user', firebaseUid]`. Enable the query only when a Firebase user/token exists. On logout or user change: cancel current-user/business queries, remove them from cache, then call Firebase `signOut`; no previous Organization name may render for the next user.

- [ ] **Step 5: Implement localized routes and ERP shell**

Support `/th/login`, `/en/login`, `/th` and `/en`; unknown locale returns not found. Thai is the default redirect. Keep all keys synchronized in `th.json` and `en.json`.

Use `design.md` tokens: Navy `#0B3056`, square corners (`border-radius: 0`), system fonts and local SVG stroke icons only. The shell must render real Current User data and no fake dashboard metrics.

- [ ] **Step 6: Add accessibility and behavior tests**

Test visible focus, logical heading levels, native buttons/links, 44px targets, error association, no color-only status, and reduced-motion CSS. Avoid redundant ARIA when native semantics already provide the role.

- [ ] **Step 7: Verify and commit the frontend slice**

Run:

```bash
npm --prefix frontend run lint
npm --prefix frontend run typecheck
npm --prefix frontend run test
npm --prefix frontend run build
git diff --check
```

Expected: login, access state, cache cleanup, localization and production build checks pass.

```bash
git add frontend
git commit -m "feat: add login and erp shell"
```

### Task 8: Add safe local runtime and end-to-end acceptance

**Files:**
- Create: `deploy/compose.yml`
- Create: `deploy/.env.example`
- Create: `firebase.json`
- Create: `.firebaserc.example`
- Create: `frontend/.env.example`
- Create: `backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs`
- Create: `frontend/e2e/auth.spec.ts`
- Create: `frontend/playwright.config.ts`
- Modify: `backend/README.md`
- Modify: `frontend/README.md`

**Interfaces:**
- Produces: PostgreSQL 17 + Firebase Auth Emulator local stack and browser proof of the complete slice

- [ ] **Step 1: Create safe local configuration**

`deploy/compose.yml` starts PostgreSQL 17 with health check and a named volume. `.env.example` uses only local values. Document these backend variables:

```text
ConnectionStrings__Database
Firebase__ProjectId=tan-erp-test-only
FIREBASE_AUTH_EMULATOR_HOST=127.0.0.1:9099
SeedTestData=true
```

Production startup must reject `SeedTestData=true` and `FIREBASE_AUTH_EMULATOR_HOST`.

- [ ] **Step 2: Add an environment-locked synthetic seeder**

`TestOnlyDataSeeder` runs through EF Core only when environment equals `Test` and `SeedTestData=true`. Seed the UUIDs and values from Task 1 plus Firebase UID `foundation-user-test-only`, permission `organizations.read` and no customer/business records. The seeder must be idempotent.

- [ ] **Step 3: Configure Firebase Auth Emulator**

Configure Auth emulator port `9099`; do not enable Firestore, Realtime Database or Storage. Add a setup command that creates `foundation-user@example.test` with a documented TEST_ONLY password in the emulator only. No real Firebase project or service-account key is needed for local/E2E.

- [ ] **Step 4: Write the failing Playwright journey**

The E2E test must:

```text
open /th/login
sign in through Firebase Auth Emulator
wait for GET /api/v1/me
assert TEST_ONLY Project ERP and สาขาทดสอบ are visible
switch to /en and assert English shell labels while data remains unchanged
log out
assert login is visible and previous Organization text is absent
sign in as a valid Firebase user with no Membership
assert the no-membership recovery state is visible
```

- [ ] **Step 5: Run the complete local acceptance**

Run PostgreSQL and Firebase Auth Emulator, apply migration explicitly, start API and frontend, then run Playwright. Capture exact process commands in both READMEs; stop processes without deleting the PostgreSQL volume.

Expected: browser journey passes with real HTTP, Firebase emulator token verification and PostgreSQL membership resolution; no API response is mocked.

- [ ] **Step 6: Commit local runtime and E2E**

```bash
git add deploy firebase.json .firebaserc.example frontend/.env.example frontend/e2e frontend/playwright.config.ts backend/src/TanErp.Infrastructure/Persistence/TestOnlyDataSeeder.cs backend/README.md frontend/README.md
git commit -m "test: verify foundation login journey"
```

### Task 9: Add CI gates and record the verified baseline

**Files:**
- Create: `.github/workflows/verify.yml`
- Create: `docs/05-engineering/foundation-login-runbook.md`
- Create: `docs/05-engineering/foundation-login-verification.md`
- Modify: `docs/README.md`
- Modify: `README.md`

**Interfaces:**
- Consumes: all commands and artifacts from Tasks 1–8
- Produces: repeatable CI/operations evidence and the gate for the next approved vertical slice

- [ ] **Step 1: Add least-privilege CI**

Configure `permissions: contents: read`, Ubuntu runner, .NET SDK `10.0.400`, Node `24.20.0`, PostgreSQL 17 service and Firebase Auth Emulator. CI jobs must run:

```text
git diff --check
fixture schema/readiness test
dotnet restore/build/test
OpenAPI generation and committed-contract diff check
npm ci/lint/typecheck/test/build
Playwright E2E
secret scan
```

Pin third-party GitHub Actions to reviewed commit SHAs during implementation; record the tag and SHA in the workflow comment.

- [ ] **Step 2: Add explicit security policy checks**

Fail CI if tracked files match service-account JSON/private key markers, if frontend imports Firebase Admin, if API/Application/Domain contains raw SQL, or if source code checks Role display names. Permit public `NEXT_PUBLIC_FIREBASE_*` identifiers but not credential files or tokens.

- [ ] **Step 3: Write the runbook**

Document local start, migration apply/rollback rehearsal, Firebase Emulator startup, test-only seed reset, health/readiness checks, trace-ID troubleshooting, token-safe logging and forward-fix policy. State that Production login-provider configuration and Membership administration UI remain outside this slice.

- [ ] **Step 4: Run final verification from clean dependencies**

Run:

```bash
npm ci
npm run test:fixtures
dotnet restore backend/TanErp.slnx --force-evaluate
dotnet build backend/TanErp.slnx --no-restore
dotnet test backend/TanErp.slnx --no-build
npm --prefix frontend ci
npm --prefix frontend run verify
npm --prefix frontend run test:e2e
git diff --check
git status --short
```

Expected: every command exits `0`; no secret, build artifact, generated drift or unexpected file is present.

- [ ] **Step 5: Record evidence, limitations and next gate**

`foundation-login-verification.md` records runtime versions, migration name, test totals, commands, exit codes and verification date. List these explicit deferrals:

```text
Membership/Role administration and audit mutation UI
Customer + Contact
Opportunity + Site
Ready Site Survey Revision
Official Estimate Draft
Item/Cost, Calculation and Approval
all modules listed as Explicitly Deferred by Development Ready Gate
```

The next implementation task may start Customer + Contact only after its own contract, permission/scope, audit and UAT acceptance are approved.

- [ ] **Step 6: Commit the verified baseline**

```bash
git add .github/workflows/verify.yml README.md docs/README.md docs/05-engineering/foundation-login-runbook.md docs/05-engineering/foundation-login-verification.md
git commit -m "ci: gate foundation login slice"
```

## Completion Gate

Foundation Login and Current User ถือว่าเสร็จเมื่อมีหลักฐานสดว่าทุกข้อผ่าน:

- Repository phase ระบุ Application Implementation และจำกัดขอบเขต Slice ชัดเจน
- Runtime/Dependency ADR เป็น Accepted และใช้รุ่นเดียวกับ local/CI
- Backend มีสี่ Production Projects และ Architecture Tests ผ่าน
- Migration จากศูนย์และ rollback/reapply ผ่าน PostgreSQL 17
- Firebase Token ถูกตรวจโดย Backend และไม่มี Business Permission มาจาก Token/Browser
- `GET /api/v1/me` ตรงเอกสารและ OpenAPI ที่ commit ไว้
- 401/403 และข้อความไทย/อังกฤษใช้ stable code เดียวกันพร้อม Trace ID
- Cross-organization test และ active-state filtering ผ่าน
- Frontend Type ถูก generate จาก OpenAPI และไม่มี contract drift
- Login, loading, expired session, disabled user, no membership, service error, retry และ logout-cache-clear states ผ่าน test
- Keyboard, accessible names, field errors, live announcements, 44px targets, contrast/focus และ reduced motion ผ่าน
- `SURVEY-BASELINE-v1` ผ่าน schema/readiness test และยังระบุ `TEST_ONLY`, `system`, `businessApproved: false`
- E2E ใช้ Firebase Auth Emulator + API + PostgreSQL จริงโดยไม่ mock API response
- CI ผ่าน build, static analysis, architecture, unit, integration, contract, E2E และ secret scan
- Runbook และ verification record แยกสิ่งที่พิสูจน์แล้วออกจากงาน Deferred ชัดเจน

## Self-Review Notes

- **Spec coverage:** ครบ Before First Application Commit, Roadmap Foundation login, Clean Architecture, OpenAPI, migration, architecture test, synthetic fixture, system-owned Survey baseline, CI, localization, accessibility, observability และ secret handling.
- **Scope control:** ไม่มี Customer, Opportunity, Site, Survey runtime table, Estimate, Item, Dapper query หรือ fake dashboard ใน Slice นี้.
- **Type consistency:** Endpoint, response names, stable error codes, query key, Firebase UID mapping และ generated OpenAPI path ใช้ชื่อเดียวกันทุก Task.
- **Working-tree safety:** Task 1 เริ่มด้วยการตรวจและรักษา documentation edits ที่มีอยู่; แผนไม่อนุญาตให้ reset หรือ overwrite งานเดิม.
