# File Security and Work Images Remediation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** ทำให้ upload, complete, preview และ bind รูปเป็น parent-bound, tenant-safe และ retry-safe พร้อมรองรับ Opportunity gallery หลายหน้า.

**Architecture:** เพิ่ม persisted `FileUploadSession` + slots ที่ระบุ `parentType`, `parentId`, organization, actor, expiry และ declared metadata. Application ใช้ `IFileParentAccessResolver` กลางแปลง parent type เป็น permission/scope check; preview รับเฉพาะ `fileId` และ authorize ผ่าน parent references. Work Images ใช้ opaque keyset cursor และ frontend เก็บ verified items ใน retry intent.

**Tech Stack:** ASP.NET Core, EF Core/PostgreSQL, OpenAPI, Next.js/TypeScript, TanStack Query, Vitest, Playwright.

## Global Constraints

- Parent types รอบนี้มีค่า exact: `opportunity`, `customer`, `site`; unknown value คืน `FILE_PARENT_TYPE_INVALID` 422. Existing resource ใช้ `parentId`; Customer/Site ที่กำลังสร้างใช้ `creationIntentId`; ต้องมีค่าอย่างใดอย่างหนึ่งเพียงค่าเดียว.
- Session TTL = 15 นาที, สูงสุด 20 files/session, 10 MiB/file และ 100 MiB/session; server ตรวจ declared metadata และ detected media type.
- Preview ไม่มี anonymous/storage-path route; out-of-scope และ unknown file ตอบ 404 เพื่อไม่เปิดเผยการมีอยู่.
- File bind ต้อง match `(organizationId, parentType, parentId, status=verified)`; file หนึ่ง bind ได้เฉพาะ parent intent เดิม.
- Cursor sort exact `(createdAtUtc DESC, id DESC)`; cursor เป็น Base64Url JSON `{createdAtUtc,id}` และ malformed cursor คืน `OPPORTUNITY_WORK_IMAGE_CURSOR_INVALID` 400.
- เพิ่ม tests ใหม่ 6 จุดเท่านั้นใน remediation นี้: security 3, pagination 1, retry 1, authorized journey 1.

---

## Exact Contract

```http
POST /api/v1/files/upload-sessions
Authorization: Bearer <token>
X-Membership-Id: <uuid>
Idempotency-Key: <16-128 chars>
Content-Type: application/json

{
  "parentType": "opportunity",
  "parentId": "<uuid>",
  "creationIntentId": null,
  "files": [{"filename":"site-01.webp","mediaType":"image/webp","fileSizeBytes":245120}]
}
```

Customer/Site create ใช้รูปแบบเดียวกันแต่ส่ง `parentId: null`, `creationIntentId: "<client-generated uuid>"`; create Customer/Site request ต้องส่ง `fileUploadIntentId` ค่าเดียวกันเพื่อ consume verified files. Retry ของ business create ต้อง reuse intent เดิม.

Success `201` returns `{sessionId,expiresAtUtc,slots:[{slotId,filename,mediaType,fileSizeBytes}]}`. Same key+same payload replays exactly; same key+different payload returns `409 IDEMPOTENCY_KEY_REUSED`.

```http
POST /api/v1/files/upload-sessions/{sessionId}/complete
Authorization: Bearer <token>
X-Membership-Id: <uuid>
Content-Type: multipart/form-data
```

Each part carries declared `slotId`; success `200` returns verified files. Missing/expired/consumed/wrong actor or metadata mismatch returns `409 FILE_UPLOAD_SESSION_INVALID`; wrong parent scope returns 404.

```http
GET /api/v1/files/{fileId}/content
Authorization: Bearer <token>
X-Membership-Id: <uuid>
```

Success streams authorized content with `Cache-Control: private, no-store`; route never accepts storage paths.

```http
GET /api/v1/opportunities/{id}/work-images?stage=<stage>&limit=25&cursor=<opaque>
```

Success `200`:

```json
{"items":[{"id":"uuid","fileId":"uuid","contentUrl":"/api/v1/files/uuid/content","stageAtAttach":"surveying","caption":null,"displayOrder":0,"createdAtUtc":"2026-09-20T00:00:00Z","createdBy":{"id":"uuid","displayName":"Name"}}],"nextCursor":null}
```

## Task 1: Persist parent-bound upload sessions — tests 1–2

**Files:**
- Create: `backend/src/TanErp.Domain/Files/FileUploadSession.cs`
- Create: `backend/src/TanErp.Domain/Files/FileUploadSlot.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/FileUploadSessionConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/FileUploadSlotConfiguration.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Migrations/<timestamp>_SecureFileUploadSessions.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Files/FileUploadContracts.cs`
- Modify: `backend/src/TanErp.Application/Files/CreateUploadSession/CreateUploadSessionCommand.cs`
- Modify: `backend/src/TanErp.Application/Files/CompleteUploadSession/CompleteUploadSessionCommand.cs`
- Modify: `backend/src/TanErp.Application/Files/IFileStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Files/FileStore.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/FileUploadSessionTests.cs`

**Interfaces:**
- Consumes: `IClock`, `IdempotencyRecord`, authenticated request context.
- Produces: `CreateSessionAsync(...)`, `GetCompletableSessionAsync(...)`, `CompleteSessionAsync(...)` and persisted session/slot invariants.

- [ ] Add failing test `CreateSession_ReplaySameIntent_ReturnsSamePersistedSlots` asserting one session, stable slots and same expiry.
- [ ] Add failing test `CompleteSession_WithoutMatchingPersistedSlots_ReturnsConflict` covering invented session ID and mismatched slot metadata in one parameterized test.
- [ ] Introduce exact store signatures:

```csharp
Task<Result<FileUploadSession>> CreateSessionAsync(
    RequestAccessContext access,
    string parentType,
    Guid? parentId,
    Guid? creationIntentId,
    IReadOnlyList<FileSlotInput> files,
    string keyHash,
    string payloadHash,
    CancellationToken cancellationToken);

Task<Result<FileUploadSession>> GetCompletableSessionAsync(
    Guid organizationId,
    Guid actorUserId,
    Guid sessionId,
    CancellationToken cancellationToken);
```

- [ ] Store session and slots atomically with idempotency record; complete validates expiry, actor, organization, unused state, count, names, sizes and types before saving binaries, then marks session consumed in the same EF transaction.
- [ ] Run:

```bash
dotnet test backend/TanErp.slnx --filter "CreateSession_ReplaySameIntent_ReturnsSamePersistedSlots|CompleteSession_WithoutMatchingPersistedSlots_ReturnsConflict"
```

- [ ] Proposed commit: `fix(files): persist parent-bound upload sessions`

## Task 2: Central parent authorization and protected content — tests 3–4

**Files:**
- Create: `backend/src/TanErp.Application/Files/IFileParentAccessResolver.cs`
- Create: `backend/src/TanErp.Infrastructure/Files/FileParentAccessResolver.cs`
- Create: `backend/src/TanErp.Application/Files/GetFileContent/GetFileContentQuery.cs`
- Create: `backend/src/TanErp.Application/Files/GetFileContent/GetFileContentHandler.cs`
- Modify: `backend/src/TanErp.Api/Controllers/FilesController.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Modify: `backend/src/TanErp.Api/Resources/Errors.resx`
- Modify: `backend/src/TanErp.Api/Resources/Errors.en.resx`
- Test: `backend/tests/TanErp.IntegrationTests/Api/FileUploadSessionTests.cs`

**Interfaces:**
- Consumes: session parent intent and current parent tables.
- Produces: `ResolveAsync(RequestAccessContext,string,Guid,FileAccessOperation,...)` and authenticated `GET /api/v1/files/{fileId}/content`.

- [ ] Add failing test `GetFileContent_AnonymousOrWrongOrganization_DoesNotDiscloseFile`; assert anonymous 401 and wrong organization 404.
- [ ] Add failing test `GetFileContent_AuthorizedParentReader_StreamsVerifiedImage`; assert bytes, media type and private cache header.
- [ ] Implement resolver mapping:

```csharp
public enum FileAccessOperation { Upload, Read, Bind }

Task<Result<FileParentAccess>> ResolveAsync(
    RequestAccessContext access,
    string parentType,
    Guid parentId,
    FileAccessOperation operation,
    CancellationToken cancellationToken);
```

`opportunity` maps read/update permissions to an organization-scoped Opportunity. Existing `customer`/`site` map to resource permissions; creation intent maps to `customers.create`/`sites.create`, same organization and actor. Return 404 for missing/out-of-scope parent.
- [ ] Delete catch-all storage-path serving behavior and move metadata/storage lookup out of controller into handler; controller only reads context and maps Result.
- [ ] Run both named tests, then `dotnet test backend/TanErp.slnx --filter FileUploadSessionTests`.
- [ ] Proposed commit: `fix(files): authorize parent-scoped file content`

## Task 3: Enforce exact parent on Customer, Site and Opportunity — test 5

**Files:**
- Modify: `backend/src/TanErp.Application/Crm/Customers/CreateCustomer/CreateCustomerHandler.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Crm/Customers/CreateCustomerRequest.cs`
- Modify: `backend/src/TanErp.Application/Crm/Customers/CreateCustomer/CreateCustomerCommand.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Crm/SiteStore.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Crm/Sites/CreateSiteRequest.cs`
- Modify: `backend/src/TanErp.Application/Crm/Sites/CreateSite/CreateSiteCommand.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/CustomerConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/SiteImageConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Configurations/OpportunityWorkImageConfiguration.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/CustomerEndpointsTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Api/FileUploadSessionTests.cs`

**Interfaces:**
- Consumes: `IFileParentAccessResolver` and persisted file parent intent.
- Produces: one reusable `ValidateVerifiedFilesForParentAsync(...)` path used by all three resource owners.

- [ ] Add parameterized failing test `BindFile_WrongOrganizationWrongParentOrUnverified_IsRejected` with Customer, Site and Opportunity cases; assert no resource/reference/audit is persisted.
- [ ] Validate all file IDs as a set before mutating aggregate; existing Opportunity requires exact parent ID. Customer/Site create requires exact `fileUploadIntentId`, organization and actor, then atomically consumes that intent when the newly generated resource ID is persisted. Add composite FK/index only where it strengthens the invariant without cross-aggregate cascade.
- [ ] Run the named test and existing Customer/Site/Opportunity attachment tests.
- [ ] Proposed commit: `fix(files): enforce verified parent ownership on bind`

## Task 4: Cursor gallery and retry intent — test 6

**Files:**
- Modify: `backend/src/TanErp.Application/Crm/Opportunities/IOpportunityStore.cs`
- Modify: `backend/src/TanErp.Application/Crm/Opportunities/WorkImages/OpportunityWorkImageProjections.cs`
- Modify: `backend/src/TanErp.Application/Crm/Opportunities/WorkImages/ListWorkImages/ListWorkImagesHandler.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Crm/OpportunityStore.cs`
- Modify: `backend/src/TanErp.Api/Contracts/Crm/Opportunities/OpportunityWorkImageContracts.cs`
- Modify: `backend/src/TanErp.Api/Controllers/OpportunitiesController.cs`
- Modify: `frontend/src/features/opportunities/api/opportunity-queries.ts`
- Modify: `frontend/src/features/opportunities/components/opportunity-work-image-attach-modal.tsx`
- Modify: `frontend/src/features/opportunities/components/opportunity-work-images-section.tsx`
- Modify: `frontend/src/components/forms/MultiImagePicker.tsx`
- Modify: `frontend/src/messages/th.json`
- Modify: `frontend/src/messages/en.json`
- Test: `frontend/src/features/opportunities/components/opportunity-work-image-attach-modal.test.tsx`
- Test: `backend/tests/TanErp.IntegrationTests/Api/OpportunitySiteEndpointsTests.cs`

**Interfaces:**
- Consumes: protected `contentUrl`, `WorkImagePage(items,nextCursor)`.
- Produces: `useInfiniteOpportunityWorkImages` and stable local upload intent `{localId,file,caption,status,fileId?,errorCode?}`.

- [ ] Add backend half of test 6: `ListWorkImages_MoreThanLimit_ReturnsStableNextPage`; seed 26 equal-time-safe rows, request 25 then cursor, assert all IDs exactly once.
- [ ] Add frontend half of test 6: `retry uploads only failed images and reuses verified file ids`; first completion partially fails, retry must not call upload for verified items and bind once only after all are verified.
- [ ] Change store return type to:

```csharp
public sealed record OpportunityWorkImagePageProjection(
    IReadOnlyList<OpportunityWorkImageProjection> Items,
    string? NextCursor);
```

- [ ] Replace timestamp/`Math.random` keys with one `crypto.randomUUID()` per stable session/attach intent; clear only on success, cancel-before-submit or parent/version change.
- [ ] Move all hardcoded upload error copy to paired translation keys and render per-image status with `aria-live="polite"`.
- [ ] Run both named tests plus existing work-image component/API tests.
- [ ] Proposed commit: `fix(opportunities): make work image retry and paging deterministic`

## Task 5: Contract generation and journey proof

**Files:**
- Modify: `docs/03-contracts/crm-site-survey-api-contract.md`
- Modify: `docs/05-engineering/crm-site-survey-uat-scenarios.md`
- Modify: `docs/05-engineering/opportunity-work-images-verification.md`
- Modify: `contracts/openapi/tan-erp.v1.json`
- Regenerate: `frontend/src/generated/api/tan-erp.v1.ts`
- Modify: `frontend/src/lib/api/api-client.ts`
- Modify: `frontend/src/lib/api/file-client.ts`
- Modify: `frontend/e2e/opportunity-work-images.spec.ts`

**Interfaces:**
- Consumes: exact contracts from this plan.
- Produces: generated types and one authorized browser journey.

- [ ] Update authoritative contract before generating OpenAPI; request required fields must remain required in TypeScript.
- [ ] Update the existing E2E to attach three images, simulate one retry without duplicate upload, open authorized content and load a second gallery page.
- [ ] Run targeted six-test set, `check:api`, then the E2E once. Record exact SHA/results without calling the slice Production-ready.
- [ ] Proposed commit: `test(files): verify secure multi-image journey`

## Definition of Success

- All six named remediation tests pass and existing attachment regressions remain green.
- No `[AllowAnonymous]` or storage-path file route remains.
- No upload can complete without its persisted session and exact slots; no resource can bind a wrong-parent/wrong-org/unverified file.
- Retry does not upload verified images again; gallery traverses all pages deterministically.
- OpenAPI generated types require `parentType`, exactly one of `parentId`/`creationIntentId`, session and slot fields; Customer/Site create types require `fileUploadIntentId` when images are present; Thai/English error/UI keys are paired.

## Deferred Scope

- Public links, CDN/object storage, virus scanning, advanced EXIF policy, retention worker, hard delete and cross-parent file reuse.
- Reorder after save, annotation/cropping and bulk download.
