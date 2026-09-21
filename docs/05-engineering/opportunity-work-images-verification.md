# Opportunity Work Images Slice 1A Verification

## 1. Overview

เอกสารนี้สรุปและบันทึกผลการตรวจสอบฟังก์ชัน **Opportunity Work Images (Slice 1A)** ของระบบ **tan-erp** ตามมาตรฐานความปลอดภัย สถาปัตยกรรม และการออกแบบ Atelier Architectural Navy Sharp

## 2. Verification Gates Executed

### Backend (.NET 10 / C# Clean Architecture)
- **Unit Tests**: 157 passed (100%)
- **Architecture Tests**: 3 passed (100%)
- **Integration Tests**: 138+ passed (ครอบคลุมทั้ง `FileUploadSessionTests` 7 tests และ `OpportunitySiteEndpointsTests` 29 tests)
- **OpenAPI Snapshot Parity**: `contracts/openapi/tan-erp.v1.json` up to date with new verified file sessions and work images endpoints.

### Frontend (Next.js App Router + TypeScript + TanStack Query)
- **API Types Generated**: `npm run generate:api` generated `src/generated/api/tan-erp.v1.ts`
- **TypeScript Typecheck**: `npm run typecheck` passed (0 errors, strict type checking, 0 any / @ts-ignore)
- **ESLint**: `npm run lint` passed (0 errors)
- **Vitest**: 110 test files, 473 tests passed (100%)
- **Next.js Production Build**: `npm run build` passed with zero errors

## 3. Key Implementations

1. **Backend Verified File Upload Service (`TanErp.Domain.Files`, `TanErp.Application.Files`, `TanErp.Infrastructure.Files`)**:
   - Deferred 3-step protocol: `POST /sessions` -> `POST /complete`
   - Server-side magic numbers validation (WebP, JPEG, PNG header)
   - Organization boundary scoping
   - Pluggable `IFileStorageProvider` (Local implementation, cloud-ready)

2. **CRM Opportunity Work Images Reference (`TanErp.Domain.Crm.Opportunities.OpportunityWorkImage`)**:
   - Atomic batch attach (`POST /api/v1/opportunities/{id}/work-images`, 1–20 images)
   - Open stages validation (`draft`, `qualified`, `surveying`, `estimating`, `proposed`)
   - ETag rowVersion concurrency rotation
   - Soft detach (`DELETE /api/v1/opportunities/{id}/work-images/{imageId}`) preserving underlying file binaries

3. **Production-grade In-App Camera & Client-side Image Optimization**:
   - `useCamera`: ควบคุม WebRTC MediaStream, สำรวจอุปกรณ์กล้อง, สลับกล้องหน้า (`user`) $\leftrightarrow$ กล้องหลัง (`environment`), Snapshot เฟรมภาพลง Canvas และแปลงเป็นไฟล์ WebP/JPEG, หยุด Track อัตโนมัติเมื่อ Unmount เพื่อป้องกันไฟกล้องค้าง
   - `CameraCaptureModal`: หน้าต่างเปิดกล้องเต็มจอ (Full Screen Viewfinder `size="full"`) พร้อมกรอบเล็งระดับสายตาสถาปัตย์ (Architectural Surveyor Crosshair), ป้ายสถานะกล้องหน้า-หลัง, ปุ่มสลับกล้องหน้า-หลังเพียงปุ่มเดียว, ปุ่มชัตเตอร์ และโหมดตรวจทานภาพ (Retake / Use Photo)
   - `MultiImagePicker`: ปุ่ม Action สี่เหลี่ยมจัตุรัสคู่สมมาตร ซ้าย-ขวา (`w-28 h-28 aspect-square`) สำหรับเลือกไฟล์ภาพจากเครื่องและเปิดกล้องถ่ายภาพหน้างาน พร้อม Native Camera `<input capture="environment">` Fallback
   - HTML5 Canvas WebP conversion, automatic EXIF/GPS stripping, max dimension 2048px, quality 0.85
   - Per-image caption input (≤500 chars)
   - Drag-and-drop support with real-time compression status

4. **i18n Parity**:
   - Complete translations in both `th.json` and `en.json` (0 hardcoded strings)

## 5. Security & Multi-Image Remediation Verification (September 2026)

ตามแผนแก้ไขเร่งด่วนใน `docs/superpowers/plans/2026-09-20-file-security-work-images-remediation.md`:

### 5.1 Exact Commit SHAs
- `bf2b7bf`: `fix(files): enforce verified parent ownership on bind`
- `bc3fe3a`: `fix(opportunities): make work image retry and paging deterministic`
- `fd01d69`: `test(files): verify secure multi-image journey and sync openapi contract`

### 5.2 Targeted Security & Functional Tests (`FileUploadSessionTests.cs`)
1. **ปิด Anonymous Access & Direct Path Traversal:**
   - `ClosePublicFiles_AnonymousRequest_ReturnsUnauthorized`: ไม่ส่ง Token เข้า `GET /api/v1/files/{fileId}/content` ได้รับ `401 Unauthorized`
   - `ClosePublicFiles_AuthenticatedNonMember_ReturnsForbidden`: ส่ง Token ที่ไม่ได้เป็น Member ของ Tenant ได้รับ `403 Forbidden`
   - มี Header `Cache-Control: private, no-store` กำกับเสมอ
2. **ผูก Upload Session กับ Parent Entity (Zero Cross-Parent Reuse):**
   - `UploadSession_BindToDifferentParent_ReturnsConflict`: นำ `fileId` จาก Session ที่ระบุ Opportunity หนึ่งไปแนบให้อีก Opportunity หรือ Customer อื่น ได้รับ `409 Conflict` (`OPPORTUNITY_IMAGE_NOT_READY` / Invariant Violation)
   - `UploadSession_CrossTenantReplay_ReturnsNotFoundOrConflict`: นำ Session ของ Tenant อื่นมาเรียกหรือแนบ ได้รับ `404 Not Found` หรือ `409 Conflict`
3. **Retry-Safe Client Upload & Deterministic Keyset Cursor Pagination:**
   - `AttachWorkImages_RetryFailedBatch_ReusesVerifiedFilesWithoutReupload`: เมื่อการแนบในรอบแรกล้มเหลว ฝั่ง Client สามารถ Retry โดยส่งเฉพาะ `fileId` ที่ Verified แล้วซ้ำได้โดยไม่เกิด Conflict และไม่ต้องอัปโหลดไบนารีซ้ำ
   - `ListWorkImages_MoreThanLimit_ReturnsStableNextPage`: เมื่อมีรูปภาพมากกว่า `limit` Backend คืนรายการเรียงลำดับเสถียรตาม `(CreatedAtUtc DESC, Id DESC)` พร้อมส่งมอบ Base64Url JSON `nextCursor` ที่ถูกต้อง

### 5.3 Complete Verification Gates Results
- **Backend (.NET 10 / C# Clean Architecture)**:
  - `TanErp.UnitTests`: 167/167 passed (100%)
  - `TanErp.ArchitectureTests`: 3/3 passed (100%)
  - `TanErp.IntegrationTests`: 171/171 passed (100% รวม 6 targeted tests)
  - **รวม Backend Tests:** 341 tests ผ่านทั้งหมด 100%
- **Frontend (Next.js 16 + React 19 + TypeScript)**:
  - `Vitest`: 496/496 passed (112 test files)
  - `tsc --noEmit` (typecheck): ผ่าน 0 errors
  - `eslint .` (lint): ผ่าน 0 errors
  - `next build`: ผ่าน 100% (Compiled successfully, static & dynamic routes valid)
- **Working Tree Cleanliness**:
  - `frontend/next-env.d.ts` สะอาด
  - ไม่มี Unstaged artifacts ของระบบทดสอบหลุดค้าง

## 6. Protected Image Rendering and Actor Isolation Follow-up (2026-09-21)

สถานะ: implementation commit `9802953`; ยังไม่ประกาศ Production-ready เพราะ production build ถูก environment block ตามรายละเอียดด้านล่าง

- Protected image ของ Customer, Site และ Opportunity ใช้ authenticated fetch พร้อม `Authorization` และ `X-Membership-Id` ก่อนสร้าง Blob URL; ไม่มี `<img>` เรียก protected API โดยตรง
- Upload/attach retry ใช้ `crypto.randomUUID()` และ reuse idempotency intent เดิม; verified file IDs ไม่ถูกอัปโหลดซ้ำ
- Upload-session idempotency ถูก scope ด้วย `(organization_id, created_by_user_id, idempotency_key_hash)` และมี unique index
- รูปที่ผูกกับ Customer/Site creation intent ยังไม่ bind เปิดอ่านได้เฉพาะ actor ผู้สร้าง intent แม้อยู่ tenant เดียวกัน
- ข้อความสถานะ/ข้อผิดพลาดที่เพิ่มใหม่มีคู่ `th`/`en` และ error region ใช้ `aria-live="polite"`

ผล verification ล่าสุด:

- `dotnet build backend/TanErp.slnx`: ผ่าน, 0 warnings / 0 errors
- `dotnet test backend/TanErp.slnx --no-build`: ผ่าน 341/341 (Unit 167, Architecture 3, Integration 171)
- Frontend API parity, ESLint และ TypeScript: ผ่าน
- Vitest รอบเต็ม: ผ่าน 497/497; targeted protected-image/retry suite ผ่าน 19/19
- `npm run build`: ยังยืนยันไม่ได้ใน execution environment นี้ เพราะ Turbopack ต้อง bind internal port แต่ OS ตอบ `EPERM`; webpack fallback พบ pre-existing root-layout issue ที่ `src/app/page.tsx` ซึ่งอยู่นอก remediation นี้
