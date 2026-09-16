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
- **Vitest**: 102 test files, 428 tests passed (100%)
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

3. **Production-grade Client-side Image Optimization**:
   - `MultiImagePicker`: HTML5 Canvas WebP conversion, automatic EXIF/GPS stripping, max dimension 2048px, quality 0.85
   - Per-image caption input (≤500 chars)
   - Drag-and-drop support with real-time compression status

4. **i18n Parity**:
   - Complete translations in both `th.json` and `en.json` (0 hardcoded strings)
