# Engineering Verification: Site Survey Vertical Slice (Slice 4)

This document records the verification results for **Site Survey Vertical Slice (Slice 4)** in `tan-erp`, covering survey appointment scheduling, surveying stage transitions, survey workspace with areas/measurements/notes, survey revision readiness gates (Mark Ready) with snapshot hashing, and automatic progression to the estimating stage with `EstimateCard`.

## 1. Scope & Implementation Summary

- **Appointment Scheduling:** Modal dialog with site selection, surveyor autocomplete, and schedule times. Schedules a survey and automatically transitions the Opportunity from `qualified` to `surveying`.
- **Survey Workspace:** Interactive drawer (`SurveyWorkspaceDrawer`) for recording:
  - Visit timestamp and survey scope summary
  - Area catalog with batch standard template presets or custom areas
  - Measurement rows with positive values, unit codes (`mm`, `cm`, `m`, `sqm`), and capture methods
  - Site assumptions, constraints, and missing detail tags
- **Revision Readiness Gate:**
  - Strict server-side invariant checks: requires at least 1 area and all areas must have at least 1 positive measurement.
  - Generates an immutable snapshot hash (SHA-256) upon `Mark Ready`.
  - Automatically transitions the parent Opportunity from `surveying` to `estimating`.
  - Disables further draft editing on the revision (Read-only view) and activates the `EstimateCard` for cost estimation.
- **Strict Architecture Compliance:**
  - Zero `any` or `@ts-ignore` (strict TypeScript narrowings).
  - All UI strings localized in both Thai (`th`) and English (`en`).
  - Atelier Architectural Navy Sharp design principles (`border-radius: 0px !important`, Solid Navy `#0B3056`, Pure SVG icons).

---

## 2. Verification Gates & Test Results

### 2.1 Backend Tests (.NET 10)
```bash
/Users/syaco/.dotnet/dotnet test backend/TanErp.slnx --filter "FullyQualifiedName~SiteSurvey"
```
**Result:** Passed 100% (13/13 tests green)
- `CreateSiteSurveyAppointmentHandlerTests` (Appointment scheduling & stage transition to `surveying`)
- `UpdateSiteSurveyDraftHandlerTests` (Draft areas, measurements, assumptions, constraints)
- `MarkSiteSurveyRevisionReadyHandlerTests` (Readiness gate validation, snapshot hashing, transition to `estimating`)
- Integration tests in `TanErp.IntegrationTests.Api.SiteSurveyEndpointsTests`

### 2.2 Frontend Unit & Integration Tests (Vitest)
```bash
npm --prefix frontend run test
```
**Result:** Passed 100% (110 test files, 474 tests green)
- `survey-components.test.tsx`
- `survey-workspace-schema.test.ts`
- `survey-queries.test.tsx`

### 2.3 Frontend Strict Typecheck & Lint
```bash
npm --prefix frontend run typecheck
npm --prefix frontend run lint
```
**Result:** 0 errors, 0 warnings (Strict TypeScript mode passed)

### 2.4 Frontend Production Build
```bash
npm --prefix frontend run build
```
**Result:** Next.js production build succeeded with standalone output.

### 2.5 End-to-End Test (Playwright)
```bash
npx playwright test e2e/site-survey.spec.ts
```
**Result:** Passed 100% (1/1 test green, ~7s)
Complete user journey verified:
1. Login with test foundation user
2. Create and activate customer with primary site
3. Create opportunity, meet Q-gate invariants, and qualify to `qualified`
4. Schedule survey appointment -> Opportunity transitions to `surveying` & displays `SurveyCard`
5. Open Survey Workspace drawer -> Fill scope summary -> Add area -> Record measurement -> Save draft
6. Mark survey revision ready with confirmation modal -> Snapshot hash generated
7. Opportunity transitions to `estimating` -> Revision marked `ready` -> `EstimateCard` displayed with Create Estimate action.

---

## 3. Review & Sign-off

- **Implementation Plan:** `docs/superpowers/plans/2026-09-13-opportunity-work-images-vertical-slice.md` / `task.md`
- **Verification Date:** 2026-09-18
- **Status:** Verified and Complete. Ready for branch consolidation.
