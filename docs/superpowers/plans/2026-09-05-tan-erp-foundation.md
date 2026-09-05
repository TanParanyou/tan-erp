# tan-erp Foundation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** วางรากฐาน `tan-erp` ให้พร้อมเริ่มพัฒนา Feature โดยมีโครงสร้าง FE/BE, กฎบังคับ, เอกสารอ้างอิง, Error Contract, Localization, RBAC, EF Core, Dapper/Raw SQL, Testing, CI และ Local Deployment ที่ตรวจสอบได้

**Architecture:** ใช้ Monorepo โดย Backend เป็น Clean Architecture 4 Projects และแบ่งงานด้วย Feature Folders; Frontend เป็น Next.js App Router และแยก Business Feature ใต้ `src/features`. EF Core เป็น Write Path หลัก ส่วน Dapper/Raw SQL เป็น Read Path ที่ควบคุมผ่าน Typed Interface; Backend เป็น Authority ของ RBAC และส่ง Error ตาม RFC 9457 Problem Details

**Tech Stack:** .NET 10 LTS, ASP.NET Core 10.0.11, EF Core 10.0.11, Npgsql EF Core Provider 10.0.3, Dapper 2.1.79, PostgreSQL 17, Next.js 16.3.3, React 19, TypeScript, TanStack Query, Zod, Vitest, Playwright, Docker-compatible Compose และ GitHub Actions

## Global Constraints

- Repository คือ `/Users/syaco/Documents/development/tan-erp`
- ระบบนี้คือ Project ERP; MRP เป็น Future Module ไม่ใช่ชื่อระบบ
- ภาษาไทยเป็นค่าเริ่มต้นและรองรับภาษาอังกฤษ
- Frontend ห้ามเชื่อม PostgreSQL, Firebase Admin SDK หรือ File Storage โดยตรง
- Domain ห้ามอ้างอิง ASP.NET Core, EF Core, Dapper, Firebase หรือ Infrastructure
- Application ห้ามอ้างอิง Infrastructure หรือ API
- EF Core ใช้สำหรับ Write และ Transaction; Dapper/Raw SQL ใช้สำหรับ Read/Report ที่มีเหตุผลชัดเจน
- Raw SQL ต้องอยู่ใน Infrastructure, ใช้ Parameter, ระบุคอลัมน์แทน `SELECT *`, บังคับ `organization_id` และมี Integration Test
- Backend เป็น Authority ของ Authentication Context, RBAC และ Resource Scope
- Error ใช้ RFC 9457 Problem Details, Stable Code, `.resx` Localization และ Trace ID
- System Error Translation ห้ามเก็บใน JSONB และห้ามมี Runtime Editor ใน Release แรก
- ห้ามใช้ Generic Repository, Service Base Class, MediatR, Event Sourcing หรือ Microservices ใน Foundation
- Secret ห้าม Commit; ใช้ Environment Variables, User Secrets หรือ ignored local configuration
- ทุก Task ต้องผ่าน Verification ของตนก่อน Commit

---

## Planned File Map

```text
tan-erp/
|-- AGENTS.md
|-- README.md
|-- CONTEXT.md
|-- .editorconfig
|-- .gitattributes
|-- .gitignore
|-- .nvmrc
|-- global.json
|-- package.json
|-- frontend/
|   |-- AGENTS.md
|   |-- package.json
|   |-- e2e/
|   `-- src/
|       |-- app/[locale]/
|       |-- components/
|       |-- features/
|       |-- generated/api/
|       |-- lib/
|       |-- messages/
|       `-- providers/
|-- backend/
|   |-- AGENTS.md
|   |-- TanErp.slnx
|   |-- Directory.Build.props
|   |-- Directory.Packages.props
|   |-- src/
|   |   |-- TanErp.Api/
|   |   |-- TanErp.Application/
|   |   |-- TanErp.Domain/
|   |   `-- TanErp.Infrastructure/
|   `-- tests/
|       |-- TanErp.UnitTests/
|       |-- TanErp.IntegrationTests/
|       `-- TanErp.ArchitectureTests/
|-- docs/
|   |-- README.md
|   |-- ARCHITECTURE.md
|   |-- FRONTEND.md
|   |-- BACKEND.md
|   |-- API.md
|   |-- ERROR_HANDLING.md
|   |-- AUTHORIZATION.md
|   |-- DATABASE.md
|   |-- RAW_SQL.md
|   |-- SECURITY.md
|   |-- TESTING.md
|   |-- DEPLOYMENT.md
|   |-- OPERATIONS.md
|   |-- CONTRIBUTING.md
|   `-- adr/
|       |-- 0001-use-clean-architecture.md
|       |-- 0002-use-ef-core-and-dapper.md
|       `-- 0003-use-backend-authoritative-rbac.md
|-- deploy/
|   |-- compose.yml
|   |-- Caddyfile
|   `-- .env.example
`-- .github/workflows/verify.yml
```

### Task 1: Repository hygiene and root governance

**Files:**
- Create: `.gitignore`
- Create: `.gitattributes`
- Create: `.editorconfig`
- Create: `.nvmrc`
- Create: `global.json`
- Create: `AGENTS.md`
- Create: `README.md`
- Create: `CONTEXT.md`
- Create: `package.json`

**Interfaces:**
- Consumes: `docs/superpowers/specs/2026-09-05-tan-erp-backend-architecture-design.md`
- Produces: Repository-wide runtime versions, commands, terminology, source-of-truth map and Definition of Done

- [ ] **Step 1: Confirm repository state without removing user files**

Run:

```bash
git status --short
git log --oneline -5
rg --files --hidden -g '!.git/**' | sort
```

Expected: the architecture specification is tracked; `.DS_Store` files may be untracked and must not be staged.

- [ ] **Step 2: Add repository hygiene files**

Create `.gitignore` with exact categories: `.DS_Store`, `.env`, `.env.*`, `!.env.example`, `appsettings.*.Local.json`, `secrets/`, `bin/`, `obj/`, `.next/`, `node_modules/`, `coverage/`, `playwright-report/`, `test-results/`, IDE folders and local container volumes.

Create `.gitattributes` with `* text=auto eol=lf` and binary declarations for common images, fonts and PDF files.

Create `.editorconfig` using UTF-8, LF and final newline; use four spaces for C# and two spaces for JSON, YAML, JavaScript, TypeScript and Markdown.

- [ ] **Step 3: Pin runtime policy**

Create `global.json`:

```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestFeature",
    "allowPrerelease": false
  }
}
```

Create `.nvmrc` containing `24` followed by a newline.

- [ ] **Step 4: Define root commands**

Create a private root `package.json` with scripts:

```json
{
  "name": "tan-erp",
  "private": true,
  "scripts": {
    "verify": "npm run verify:backend && npm run verify:frontend",
    "verify:backend": "dotnet restore backend/TanErp.slnx && dotnet build backend/TanErp.slnx --no-restore && dotnet test backend/TanErp.slnx --no-build",
    "verify:frontend": "npm --prefix frontend run verify"
  }
}
```

Run `npm install --package-lock-only` to create the root lockfile without runtime dependencies.

- [ ] **Step 5: Write root governance and vocabulary**

`AGENTS.md` must define read scope, Thai communication, FE/BE boundaries, typed API contracts, append-only migrations, raw SQL rules, backend-authoritative RBAC, localized Problem Details, secret handling, test commands and Definition of Done.

`CONTEXT.md` must define only canonical business terms: Project ERP, Item Master, Estimate, Quotation, Organization, Branch, Project Scope, Role, Permission, MRP and Revision. It must not contain framework or database decisions.

`README.md` must identify the product, current Foundation scope, target architecture, prerequisites and links to `docs/README.md` and the approved specification.

- [ ] **Step 6: Verify and commit repository governance**

Run:

```bash
rg -n "MRP|Project ERP|Raw SQL|RBAC|Problem Details" AGENTS.md README.md CONTEXT.md
git diff --check
git status --short
```

Expected: `.DS_Store` is ignored; only intended governance files appear.

```bash
git add .gitignore .gitattributes .editorconfig .nvmrc global.json package.json package-lock.json AGENTS.md README.md CONTEXT.md
git commit -m "chore: establish repository governance"
```

### Task 2: Create the documentation system and architecture decisions

**Files:**
- Create: `docs/README.md`
- Create: `docs/ARCHITECTURE.md`
- Create: `docs/FRONTEND.md`
- Create: `docs/BACKEND.md`
- Create: `docs/API.md`
- Create: `docs/ERROR_HANDLING.md`
- Create: `docs/AUTHORIZATION.md`
- Create: `docs/DATABASE.md`
- Create: `docs/RAW_SQL.md`
- Create: `docs/SECURITY.md`
- Create: `docs/TESTING.md`
- Create: `docs/DEPLOYMENT.md`
- Create: `docs/OPERATIONS.md`
- Create: `docs/CONTRIBUTING.md`
- Create: `docs/adr/0001-use-clean-architecture.md`
- Create: `docs/adr/0002-use-ef-core-and-dapper.md`
- Create: `docs/adr/0003-use-backend-authoritative-rbac.md`

**Interfaces:**
- Consumes: Global constraints and approved backend specification
- Produces: One source of truth per architecture, development and operating concern

- [ ] **Step 1: Create the documentation index**

`docs/README.md` must map each concern to exactly one authoritative document and explain that ADRs record hard-to-reverse decisions while guides describe the current operating rules.

- [ ] **Step 2: Write architecture and development guides**

Document the approved FE/BE tree and dependency direction in `ARCHITECTURE.md`. Put frontend route/feature/state rules in `FRONTEND.md`; put Clean Architecture project responsibilities and Feature Folder conventions in `BACKEND.md`; put route naming, OpenAPI synchronization and versioning in `API.md`.

- [ ] **Step 3: Write data, SQL and error guides**

`DATABASE.md` must require PostgreSQL, UTC timestamps, decimal money, append-only EF migrations, restore testing and Organization scope.

`RAW_SQL.md` must contain the accepted EF-write/Dapper-read decision, allowed use cases, parameterization, `organization_id`, deterministic pagination, SQL file placement, transaction sharing and PostgreSQL integration-test requirements.

`ERROR_HANDLING.md` must define RFC 9457 fields `type`, `title`, `status`, `code`, `detail`, `traceId`, `errors`; HTTP status mapping; `.resx` localization through `Accept-Language`; Thai fallback; and the prohibition on JSONB system-error translations.

- [ ] **Step 4: Write authorization and security guides**

`AUTHORIZATION.md` must define Firebase as identity only, PostgreSQL RBAC, `resource.action` permission keys, Organization/Branch/Project/Own scopes, Maker–Checker, 401/403/404 behavior and audit requirements.

`SECURITY.md` must cover trust boundaries, secret storage, sensitive logging, file authorization, least privilege, dependency scanning and incident trace IDs.

- [ ] **Step 5: Write verification and operating guides**

`TESTING.md` must map Unit, Integration, Architecture, Contract and End-to-End tests to their purposes. `DEPLOYMENT.md` must describe local Compose and the production Linux/Caddy direction. `OPERATIONS.md` must define health/readiness, backup, restore drill, migration, rollback and support ownership. `CONTRIBUTING.md` must define branch, commit, review, migration and Definition of Done rules.

- [ ] **Step 6: Record three ADRs**

Each ADR must contain Status, Context, Decision, Consequences and Rejected Alternatives. Decisions are: Clean Architecture 4 Projects; EF Core writes plus Dapper reads; and Backend-authoritative RBAC with scoped permissions.

- [ ] **Step 7: Verify documentation consistency and commit**

Run:

```bash
rg -n "T[B]D|T[O]DO|PLACEH[O]LDER" docs --glob '*.md'
rg -n "Controller.*Service.*Repository|GenericRepository|System Error.*JSONB" docs --glob '*.md'
git diff --check
```

Expected: no placeholders; old Controller-Service-Repository architecture is not stated as the selected pattern; JSONB appears only in a prohibition or unrelated business-data context.

```bash
git add docs
git commit -m "docs: establish engineering handbook"
```

### Task 3: Scaffold the backend Clean Architecture solution

**Files:**
- Create: `backend/AGENTS.md`
- Create: `backend/TanErp.slnx`
- Create: `backend/Directory.Build.props`
- Create: `backend/Directory.Packages.props`
- Create: `backend/src/TanErp.Api/*`
- Create: `backend/src/TanErp.Application/*`
- Create: `backend/src/TanErp.Domain/*`
- Create: `backend/src/TanErp.Infrastructure/*`
- Create: `backend/tests/TanErp.UnitTests/*`
- Create: `backend/tests/TanErp.IntegrationTests/*`
- Create: `backend/tests/TanErp.ArchitectureTests/*`

**Interfaces:**
- Consumes: Runtime and dependency rules from Tasks 1–2
- Produces: Buildable `.NET 10` solution with the dependency direction Domain <- Application <- Infrastructure/Api

- [ ] **Step 1: Pass the .NET environment gate**

Run `dotnet --list-sdks` and require a stable `10.0.x` SDK. The planning host currently reports `9.0.300`; stop this task until .NET 10 is installed rather than lowering the target framework.

- [ ] **Step 2: Scaffold the solution and seven projects**

Use `dotnet new` to create `TanErp.slnx`, four production projects and three xUnit test projects. Delete template WeatherForecast code. Add all projects to the solution.

- [ ] **Step 3: Add only approved project references**

Add Application -> Domain; Infrastructure -> Application + Domain; Api -> Application + Infrastructure; tests -> their subject projects. No reverse reference is allowed.

- [ ] **Step 4: Pin backend packages centrally**

Create `Directory.Packages.props` with central package management and exact versions:

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.11" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.11" />
    <PackageVersion Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.11" />
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.3" />
    <PackageVersion Include="Dapper" Version="2.1.79" />
    <PackageVersion Include="Testcontainers.PostgreSql" Version="4.14.0" />
  </ItemGroup>
</Project>
```

`Testcontainers.PostgreSql 4.14.0` targets a framework compatible with .NET 10 and is pinned with the other dependencies for reproducible restore.

- [ ] **Step 5: Enable compiler guardrails**

`Directory.Build.props` must enable nullable references, implicit usings, warnings as errors, deterministic builds, analyzers and invariant globalization disabled because Thai/English localization is required.

- [ ] **Step 6: Write backend-specific AGENTS.md**

Require reading `docs/BACKEND.md`, `DATABASE.md`, `RAW_SQL.md`, `ERROR_HANDLING.md`, `AUTHORIZATION.md` and `TESTING.md` according to task scope. Include exact restore/build/test commands and prohibit cross-layer references, direct SQL outside Infrastructure, hard-coded Role checks and secrets.

- [ ] **Step 7: Verify and commit backend scaffolding**

Run:

```bash
dotnet restore backend/TanErp.slnx
dotnet build backend/TanErp.slnx --no-restore
dotnet test backend/TanErp.slnx --no-build
git diff --check
```

Expected: all projects build and template tests pass.

```bash
git add backend
git commit -m "feat: scaffold clean backend"
```

### Task 4: Implement backend cross-cutting contracts and enforcement

**Files:**
- Create: `backend/src/TanErp.Application/Common/Results/Error.cs`
- Create: `backend/src/TanErp.Application/Common/Results/Result.cs`
- Create: `backend/src/TanErp.Application/Common/Abstractions/IApplicationDbContext.cs`
- Create: `backend/src/TanErp.Application/Common/Abstractions/IDbConnectionFactory.cs`
- Create: `backend/src/TanErp.Application/Common/Abstractions/ICurrentUser.cs`
- Create: `backend/src/TanErp.Application/Common/Authorization/IPermissionChecker.cs`
- Create: `backend/src/TanErp.Application/Common/Authorization/PermissionNames.cs`
- Create: `backend/src/TanErp.Api/ErrorHandling/GlobalExceptionHandler.cs`
- Create: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`
- Create: `backend/src/TanErp.Api/Authorization/PermissionRequirement.cs`
- Create: `backend/src/TanErp.Api/Authorization/PermissionAuthorizationHandler.cs`
- Create: `backend/src/TanErp.Api/Resources/Errors.resx`
- Create: `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Test: `backend/tests/TanErp.UnitTests/Common/ResultTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/ErrorContractTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/AuthorizationTests.cs`

**Interfaces:**
- Produces: `Error(Code, ResourceKey, Type, Parameters)`, `Result<T>`, `IDbConnectionFactory.OpenConnectionAsync`, `IPermissionChecker.HasPermissionAsync` and RFC 9457 API responses

- [ ] **Step 1: Write failing Result and error-mapping tests**

Test that a failed `Result<T>` cannot expose a value; validation maps to 400, conflict to 409, business rule to 422 and unexpected exception to 500; every response includes stable `code` and `traceId`.

- [ ] **Step 2: Implement minimal Result types and Problem Details mapping**

Expected failures flow through `Result<T>`. `GlobalExceptionHandler` handles unexpected exceptions only and never returns exception messages or stack traces in Production.

- [ ] **Step 3: Write failing localization tests**

Call one protected test endpoint with `Accept-Language: th` and `Accept-Language: en`. Assert identical status/code and different localized title/detail. Assert an unsupported language falls back to Thai.

- [ ] **Step 4: Implement `.resx` localization**

Configure request localization for exactly `th` and `en`, default `th`. Resource keys must be stable and code-owned; do not read translations from PostgreSQL.

- [ ] **Step 5: Write failing RBAC behavior tests**

Assert unauthenticated returns 401 `AUTHENTICATION_REQUIRED`; authenticated without permission returns 403 `PERMISSION_DENIED`; matching permission succeeds; cross-organization resource lookup returns 404.

- [ ] **Step 6: Implement fail-closed scoped authorization**

Use ASP.NET Core policies backed by `PermissionRequirement` and `IPermissionChecker`. Permission names use `resource.action`; controllers must not check Role names. The initial checker may use a test implementation only inside the test host; Production registration must fail startup until a real provider is configured.

- [ ] **Step 7: Add architecture tests**

Use assembly metadata to verify dependency direction. Add source-based assertions that `TanErp.Api` contains no `DbContext`, `Dapper`, `NpgsqlConnection` or raw SQL, and Domain/Application do not reference Infrastructure packages.

- [ ] **Step 8: Verify and commit cross-cutting contracts**

Run backend build and all tests. Expected: Thai/English error tests, status mapping and RBAC fail-closed tests pass.

```bash
git add backend
git commit -m "feat: enforce api and authorization contracts"
```

### Task 5: Establish PostgreSQL, EF Core and Raw SQL foundations

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/DesignTimeDbContextFactory.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/NpgsqlConnectionFactory.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Queries/System/SystemQueries.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Queries/System/Sql/GetDatabaseStatus.sql`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/*_InitializeFoundation.cs`
- Create: `backend/src/TanErp.Infrastructure/DependencyInjection.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/DatabaseFoundationTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/RawSqlPolicyTests.cs`

**Interfaces:**
- Consumes: `IApplicationDbContext` and `IDbConnectionFactory` from Task 4
- Produces: PostgreSQL-backed AppDbContext, shared Npgsql connection factory and typed Dapper database-status query

- [ ] **Step 1: Write failing PostgreSQL integration tests**

Start PostgreSQL 17 with Testcontainers. Assert migration succeeds on an empty database, `SaveChangesAsync` works and Dapper returns the expected typed database status. Add a policy test that scans business-query `.sql` files and requires an `@OrganizationId` parameter while explicitly exempting queries under `Queries/System`.

- [ ] **Step 2: Implement AppDbContext and design-time factory**

Use Npgsql, snake_case naming and UTC-only DateTime policy. Keep migrations in Infrastructure. Do not enable automatic migration during normal application startup.

- [ ] **Step 3: Implement the typed Raw SQL path**

`SystemQueries` loads `GetDatabaseStatus.sql` as an embedded resource, opens a connection through `IDbConnectionFactory`, executes a parameterized Dapper query and maps to a sealed record. No SQL string reaches Application or API.

- [ ] **Step 4: Create the foundation migration**

Create schema `system` and table `system.outbox_messages` with `id`, `occurred_at_utc`, `type`, `payload` JSONB, `processed_at_utc`, `error` and `retry_count`. The JSONB payload is event data and is unrelated to the prohibited System Error translation catalog. Do not create Item, Estimate or Quotation tables in this task.

- [ ] **Step 5: Verify rollback and repeatability**

Apply migration, run tests, roll back to zero in the isolated test database, reapply and rerun. Expected: identical schema and passing tests.

- [ ] **Step 6: Verify and commit persistence foundation**

Run backend restore, build and tests plus `git diff --check`.

```bash
git add backend
git commit -m "feat: establish data access foundation"
```

### Task 6: Scaffold the frontend and shared client contracts

**Files:**
- Create: `frontend/AGENTS.md`
- Create: `frontend/src/app/[locale]/*`
- Create: `frontend/src/features/auth/*`
- Create: `frontend/src/features/item-master/*` directories without business screens
- Create: `frontend/src/lib/api/api-client.ts`
- Create: `frontend/src/lib/api/api-error.ts`
- Create: `frontend/src/lib/api/problem-details.ts`
- Create: `frontend/src/lib/permissions/*`
- Create: `frontend/src/messages/th.json`
- Create: `frontend/src/messages/en.json`
- Create: `frontend/src/providers/query-provider.tsx`
- Test: `frontend/src/lib/api/problem-details.test.ts`
- Test: `frontend/src/lib/permissions/permission.test.ts`

**Interfaces:**
- Consumes: RFC 9457 error contract, `Accept-Language` and permission names from backend documentation
- Produces: Central API client, typed `ApiProblem`, locale propagation and UX-only permission guard

- [ ] **Step 1: Pass the Node environment gate and scaffold**

Require Node 24 LTS from `.nvmrc`. Use `create-next-app@16.3.3` with TypeScript, App Router, `src/`, ESLint and Tailwind. Do not copy WAT-PROFILE source files.

- [ ] **Step 2: Install and configure frontend foundations**

Add TanStack Query, Zod, React Hook Form, Vitest, Testing Library and Playwright with lockfile updates. Scripts must include `lint`, `typecheck`, `test`, `test:e2e`, `build` and `verify`.

- [ ] **Step 3: Write failing Problem Details tests**

Assert the parser preserves status/code/traceId, maps field errors, returns a safe `UNKNOWN_ERROR` for non-JSON responses and never displays raw response bodies for 500 errors.

- [ ] **Step 4: Implement the central API client**

All feature APIs use one client that attaches Firebase ID token when available, sends selected `Accept-Language`, parses Problem Details and throws typed `ApiError`. The client must not contain business-specific behavior.

- [ ] **Step 5: Implement locale and permission foundations**

Create `th` and `en` message catalogs, Thai default route and permission helpers. Permission UI may hide/disable controls but documentation and tests must state that Backend remains authoritative.

- [ ] **Step 6: Create empty feature boundaries**

Create only index/readme boundary files for `auth` and `item-master`; do not add sample records, fake dashboards or unapproved ERP workflows.

- [ ] **Step 7: Write frontend AGENTS.md and verify**

Require feature-local API/components/schemas/types, no direct fetch inside components, TanStack Query for server state, generated API types not edited manually, Thai/English keys synchronized and accessibility states for forms.

Run lint, typecheck, unit tests and production build.

```bash
git add frontend
git commit -m "feat: scaffold frontend foundation"
```

### Task 7: Add local deployment, CI and operational checks

**Files:**
- Create: `deploy/compose.yml`
- Create: `deploy/.env.example`
- Create: `deploy/Caddyfile`
- Create: `.github/workflows/verify.yml`
- Modify: `docs/DEPLOYMENT.md`
- Modify: `docs/OPERATIONS.md`

**Interfaces:**
- Consumes: Backend/frontend build and health contracts
- Produces: Reproducible local PostgreSQL environment, CI verification and documented production topology

- [ ] **Step 1: Create a safe local Compose stack**

Define PostgreSQL 17 with named volume, health check and credentials loaded from an ignored `.env`. `.env.example` contains non-secret local placeholders only. Do not include Firebase credentials.

- [ ] **Step 2: Add Caddy development routing documentation**

Provide a Caddyfile that routes `/api/*` to Backend and other paths to Frontend, preserving request headers and Trace Context. Keep HTTPS production requirements documented separately from local HTTP convenience.

- [ ] **Step 3: Create CI verification**

Use .NET 10 and Node 24. Run backend restore/build/test, frontend `npm ci`/verify, `git diff --check`, secret scan and migration-on-empty-database test. Workflow permissions are `contents: read` unless a later workflow requires more.

- [ ] **Step 4: Verify local stack and commit**

Run Compose config validation, start PostgreSQL, wait for health, run backend integration tests, stop containers without deleting the named volume, then run full root verification.

```bash
git add deploy .github docs/DEPLOYMENT.md docs/OPERATIONS.md
git commit -m "ci: add deployment and verification baseline"
```

### Task 8: Foundation acceptance and baseline record

**Files:**
- Create: `docs/BASELINE.md`
- Modify: `docs/README.md`
- Modify: `README.md`

**Interfaces:**
- Consumes: All previous tasks
- Produces: Evidence-backed Foundation baseline and the entry gate for the first Item Master vertical slice

- [ ] **Step 1: Run clean backend verification**

Run restore, build and all tests using .NET 10. Record SDK version, test totals and exit codes.

- [ ] **Step 2: Run clean frontend verification**

Run `npm ci`, lint, typecheck, unit tests and production build using Node 24. Record runtime versions and results.

- [ ] **Step 3: Verify security and architecture rules**

Search for committed secrets, direct frontend database/Firebase Admin access, raw SQL outside Infrastructure, hard-coded Role checks, `SELECT *`, JSONB system-error catalogs and reverse project references. Expected: zero violations.

- [ ] **Step 4: Exercise runtime contracts**

Verify health/readiness, Thai and English Problem Details, 401/403/404 authorization behavior, Trace ID propagation, PostgreSQL migration and Dapper database query against the local stack.

- [ ] **Step 5: Write the baseline record**

`docs/BASELINE.md` records versions, commands, test totals, known operational limits and the exact next scope: Item Master create/search vertical slice. It must distinguish verified behavior from deferred business functionality.

- [ ] **Step 6: Final repository verification and commit**

Run:

```bash
npm run verify
git diff --check
git status --short
git log --oneline -10
```

Expected: all verification commands exit `0`; working tree contains no untracked build output or secret.

```bash
git add README.md docs/README.md docs/BASELINE.md
git commit -m "docs: record foundation baseline"
```

## Completion Gate

Foundation ถือว่าเสร็จเมื่อ:

- Root, frontend และ backend `AGENTS.md` สอดคล้องกัน
- เอกสารทุกเรื่องมี Source of Truth เดียวและไม่มี Placeholder
- Backend dependency direction ผ่าน Architecture Tests
- EF Core migration และ Dapper typed query ผ่าน PostgreSQL Integration Tests
- Raw SQL Policy และ Organization Scope ถูกบังคับด้วย Test
- Problem Details แสดงไทย/อังกฤษด้วย Code เดียวกัน
- RBAC ปฏิเสธแบบ fail-closed และผ่าน 401/403/404 tests
- Frontend API client จัดการ locale, token และ Problem Details จากจุดกลาง
- Backend/frontend build และ test ผ่าน CI
- Local stack เริ่มและหยุดได้ตามเอกสาร
- ไม่มี Secret, `.DS_Store` หรือ Build Artifact ใน Commit
