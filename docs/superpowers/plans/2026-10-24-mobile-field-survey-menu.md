# Mobile Field Survey Menu & Workspace Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement the dedicated "Field Survey / สำรวจหน้างาน" menu (`/surveys/field`) and mobile-first on-site survey workspace with system catalog material selection, multi-type & adaptive measurements, per-item photo/sketch attachments, and survey-to-BOQ handoff, complying 100% with `AGENTS.md` and `design.md`.

**Architecture:** A streamlined Next.js route (`/surveys/field`) integrated into `SidebarNav` under CRM, communicating directly with existing `OpportunitiesController`, `SiteSurveysController`, `EstimatesController`, and `ItemsController` backend endpoints. Data flows from field intake (Lead + Mood/Before photos) to mobile room/item measurement (converting to standard mm/sqm and catalog SKUs), locking into immutable survey snapshot hashes, and auto-seeding Estimate BOQ sections/items for consultant pricing.

**Tech Stack:** Next.js 16 App Router, React 19, React Hook Form, Zod, TanStack Query, Tailwind CSS (Atelier Architectural Navy Sharp, 0px radius, `#0B3056`), Vitest, TypeScript (Strict).

**Spec:** [CONTEXT.md](file:///Users/syaco/Documents/development/tan-erp/CONTEXT.md), [design.md](file:///Users/syaco/Documents/development/tan-erp/design.md), [AGENTS.md](file:///Users/syaco/Documents/development/tan-erp/AGENTS.md), and [streamlined-renovation-flow-mockup.html](file:///Users/syaco/.gemini/antigravity/brain/09cb0ac2-9c04-49f8-a0a4-2ec636bf2e09/streamlined-renovation-flow-mockup.html).

## Global Constraints

- Strict TypeScript: No `any`, `as any`, or `@ts-ignore` under any circumstance. Use strict types, interfaces, or `unknown` with narrow guards.
- No Hardcoded strings: All UI labels and messages must be translated in both `messages/th.json` and `messages/en.json`.
- Visual Theme: Atelier Architectural Navy Sharp (`border-radius: 0px !important`, Solid Navy `#0B3056`, 44px+ touch targets, Pure SVG stroke icons, no emojis in code).
- Reuse First: Use existing central components (`CustomerAutocomplete`, `useCustomerSiteList`, `useMeasurementMetadata`, `useAreaPresets`, `useBoxDimensions`, `useEstimateCatalog`, `Button`, `Input`, `Select`, `Badge`, `Alert`).
- Minimal Blast Radius: Modify only necessary files; verify via full test suite before claiming completion.

## Review Focus

1. Permission Guarding: Users without `opportunities.read` or `surveys.create` must not see the menu item or be able to mutate survey data.
2. Conversion Accuracy: Measurement values entered in cm or m must correctly convert to mm (`convertMeasurementToMm`) without rounding errors or NaN.
3. Catalog SKU Resolution: If an Item Master item is selected, its SKU, name, and resolved cost must link to the item rather than free-text.
4. Photo Attachment Integrity: Photos attached to specific items must register with their purpose (`site_location`, `sketch_laser`, `material_swatch`) and render in lightbox without breaking when detached.
5. Form DOM Persistence: Stepper and tabs must keep form inputs registered without unmounting (using CSS `hidden`).

---

### Task 1: Navigation Item & i18n Dictionary Setup

**Files:**
- Modify: `frontend/src/components/layout/SidebarNav.tsx:110-150`
- Modify: `frontend/messages/th.json`
- Modify: `frontend/messages/en.json`
- Test: `frontend/src/components/layout/SidebarNav.test.tsx`

**Interfaces:**
- Consumes: `can(activeMembership, "opportunities.read") || can(activeMembership, "surveys.create")`
- Produces: Navigation item `field-surveys` linking to `/${locale}/surveys/field` with icon and active route detection.

- [ ] **Step 1: Write test for new sidebar menu item**
  Add unit test in `SidebarNav.test.tsx` asserting that when the user has opportunity or survey permissions, the "field-surveys" menu item renders with href `/[locale]/surveys/field`.

- [ ] **Step 2: Run test to verify failure**
  Run: `npm run test --prefix frontend -- src/components/layout/SidebarNav.test.tsx`
  Expected: FAIL (menu item not yet present)

- [ ] **Step 3: Add translations and update SidebarNav.tsx**
  Add `shell.fieldSurveys` in `frontend/messages/th.json` ("สำรวจหน้างาน") and `frontend/messages/en.json` ("Field Survey").
  Add `field-surveys` item to `crmItems` in `SidebarNav.tsx`.

- [ ] **Step 4: Run test to verify pass**
  Run: `npm run test --prefix frontend -- src/components/layout/SidebarNav.test.tsx`
  Expected: PASS

- [ ] **Step 5: Commit**
  ```bash
  git add frontend/src/components/layout/SidebarNav.tsx frontend/messages/th.json frontend/messages/en.json frontend/src/components/layout/SidebarNav.test.tsx
  git commit -m "feat(navigation): add field survey menu item in sidebar"
  ```

---

### Task 2: Field Survey Zod Schemas & Types

**Files:**
- Create: `frontend/src/features/surveys/schemas/field-survey-schema.ts`
- Test: `frontend/src/features/surveys/schemas/field-survey-schema.test.ts`

**Interfaces:**
- Produces: `FieldSurveyItemFormData`, `FieldSurveyMeasurementData`, `FieldSurveyEvidenceData`, `FieldSurveyFormData` types and `fieldSurveyFormSchema`.

- [ ] **Step 1: Write schema validation unit tests**
  Write tests in `field-survey-schema.test.ts` testing valid and invalid payloads (missing item name, negative dimensions, invalid SKU, empty customer).

- [ ] **Step 2: Run test to verify failure**
  Run: `npm run test --prefix frontend -- src/features/surveys/schemas/field-survey-schema.test.ts`
  Expected: FAIL (module not found)

- [ ] **Step 3: Implement `field-survey-schema.ts`**
  Implement Zod schemas strictly without `any`:
  - `fieldSurveyMeasurementSchema`: `type`, `value`, `unit`, `note`
  - `fieldSurveyItemSchema`: `id`, `name`, `category`, `measureMode`, `primaryDims`, `extraMeasurements`, `coreSku`, `finishSku`, `hardwareSku`, `customNote`, `photos`, `qty`, `unit`, `unitCost`, `sellingPrice`
  - `fieldSurveyFormSchema`: `customerId`, `siteId`, `title`, `scheduledDate`, `items`, `checklist`

- [ ] **Step 4: Run test to verify pass**
  Run: `npm run test --prefix frontend -- src/features/surveys/schemas/field-survey-schema.test.ts`
  Expected: PASS

- [ ] **Step 5: Commit**
  ```bash
  git add frontend/src/features/surveys/schemas/field-survey-schema.ts frontend/src/features/surveys/schemas/field-survey-schema.test.ts
  git commit -m "feat(surveys): add field survey form schema and validation"
  ```

---

### Task 3: Item Master Catalog & Spec Picker Component

**Files:**
- Create: `frontend/src/features/surveys/components/field/field-survey-catalog-picker.tsx`
- Test: `frontend/src/features/surveys/components/field/field-survey-catalog-picker.test.tsx`

**Interfaces:**
- Consumes: `useEstimateCatalog` hook or catalog categories, props `{ itemIndex: number, disabled?: boolean }`
- Produces: 3-tier catalog select (Core Material SKU, Surface/Finish SKU, Hardware SKU) + custom note input, emitting updates to React Hook Form.

- [ ] **Step 1: Write component unit test**
  Test in `field-survey-catalog-picker.test.tsx` asserting that selecting an SKU updates form values and triggers change handler.

- [ ] **Step 2: Run test to verify failure**
  Run: `npm run test --prefix frontend -- src/features/surveys/components/field/field-survey-catalog-picker.test.tsx`
  Expected: FAIL

- [ ] **Step 3: Implement `FieldSurveyCatalogPicker`**
  Implement the component using Atelier Architectural Navy Sharp styling (`min-h-[44px]`, 0px border radius, clear SKU badge).

- [ ] **Step 4: Run test to verify pass**
  Run: `npm run test --prefix frontend -- src/features/surveys/components/field/field-survey-catalog-picker.test.tsx`
  Expected: PASS

- [ ] **Step 5: Commit**
  ```bash
  git add frontend/src/features/surveys/components/field/field-survey-catalog-picker.tsx frontend/src/features/surveys/components/field/field-survey-catalog-picker.test.tsx
  git commit -m "feat(surveys): add catalog spec picker component for field survey"
  ```

---

### Task 4: Multi-Type & Custom Measurement Pad Component

**Files:**
- Create: `frontend/src/features/surveys/components/field/field-survey-measurement-pad.tsx`
- Test: `frontend/src/features/surveys/components/field/field-survey-measurement-pad.test.tsx`

**Interfaces:**
- Consumes: `useMeasurementMetadata`, `convertMeasurementToMm`, props `{ itemIndex: number, isReady: boolean }`
- Produces: Adaptive measurement input (W×D×H with mm conversion display, area sqm calculation, opening deduction) and custom measurement modal ("+ เพิ่มระยะวัดเฉพาะจุด").

- [ ] **Step 1: Write component unit test**
  Test in `field-survey-measurement-pad.test.tsx` verifying primary dimension inputs, mm live calculation, and adding a custom offset/clearance measurement.

- [ ] **Step 2: Run test to verify failure**
  Run: `npm run test --prefix frontend -- src/features/surveys/components/field/field-survey-measurement-pad.test.tsx`
  Expected: FAIL

- [ ] **Step 3: Implement `FieldSurveyMeasurementPad`**
  Implement component following `design.md` mobile standards (large numeric touch inputs, clear conversion tags, zero radius).

- [ ] **Step 4: Run test to verify pass**
  Run: `npm run test --prefix frontend -- src/features/surveys/components/field/field-survey-measurement-pad.test.tsx`
  Expected: PASS

- [ ] **Step 5: Commit**
  ```bash
  git add frontend/src/features/surveys/components/field/field-survey-measurement-pad.tsx frontend/src/features/surveys/components/field/field-survey-measurement-pad.test.tsx
  git commit -m "feat(surveys): add adaptive measurement pad with mm conversion"
  ```

---

### Task 5: Per-Item Photo & Sketch Attachment Manager

**Files:**
- Create: `frontend/src/features/surveys/components/field/field-survey-item-photo-manager.tsx`
- Test: `frontend/src/features/surveys/components/field/field-survey-item-photo-manager.test.tsx`

**Interfaces:**
- Consumes: `useGalleryLightbox`, props `{ itemIndex: number, disabled?: boolean }`
- Produces: Photo strip per item with purpose badges (`site_location`, `sketch_laser`, `material_swatch`) and "+ ถ่ายรูป/แนบภาพจุดนี้" modal.

- [ ] **Step 1: Write component unit test**
  Test in `field-survey-item-photo-manager.test.tsx` verifying photo attachment, tagging, caption editing, and removal.

- [ ] **Step 2: Run test to verify failure**
  Run: `npm run test --prefix frontend -- src/features/surveys/components/field/field-survey-item-photo-manager.test.tsx`
  Expected: FAIL

- [ ] **Step 3: Implement `FieldSurveyItemPhotoManager`**
  Implement component with thumbnail cards, tag chips, and modal selector.

- [ ] **Step 4: Run test to verify pass**
  Run: `npm run test --prefix frontend -- src/features/surveys/components/field/field-survey-item-photo-manager.test.tsx`
  Expected: PASS

- [ ] **Step 5: Commit**
  ```bash
  git add frontend/src/features/surveys/components/field/field-survey-item-photo-manager.tsx frontend/src/features/surveys/components/field/field-survey-item-photo-manager.test.tsx
  git commit -m "feat(surveys): add per-item photo and sketch attachment manager"
  ```

---

### Task 6: Field Survey Page Route & Workspace Component

**Files:**
- Create: `frontend/src/app/[locale]/(erp)/surveys/field/page.tsx`
- Create: `frontend/src/features/surveys/components/field/field-survey-workspace.tsx`
- Test: `frontend/src/features/surveys/components/field/field-survey-workspace.test.tsx`

**Interfaces:**
- Consumes: React Hook Form, TanStack Query mutations (`createOpportunity`, `createSiteSurvey`, `createEstimate`, `calculateEstimate`), PermissionGuard
- Produces: Full page at `/[locale]/surveys/field` with 4-step mobile/desktop adaptive workflow.

- [ ] **Step 1: Write page and workspace integration tests**
  Test step navigation (Step 1 -> 2 -> 3 -> 4), data continuity (values in Step 2 reflected in Step 3 BOQ and Step 4 Quotation), and form submission.

- [ ] **Step 2: Run test to verify failure**
  Run: `npm run test --prefix frontend -- src/features/surveys/components/field/field-survey-workspace.test.tsx`
  Expected: FAIL

- [ ] **Step 3: Implement page and `FieldSurveyWorkspace`**
  Implement page route with PermissionGuard (`OPPORTUNITIES_READ` & `SURVEYS_CREATE`) and `FieldSurveyWorkspace` tying together all subcomponents.

- [ ] **Step 4: Run test to verify pass**
  Run: `npm run test --prefix frontend -- src/features/surveys/components/field/field-survey-workspace.test.tsx`
  Expected: PASS

- [ ] **Step 5: Commit**
  ```bash
  git add frontend/src/app/[locale]/(erp)/surveys/field/page.tsx frontend/src/features/surveys/components/field/field-survey-workspace.tsx frontend/src/features/surveys/components/field/field-survey-workspace.test.tsx
  git commit -m "feat(surveys): implement mobile field survey page and workspace orchestrator"
  ```

---

### Task 7: Full Verification Gates & Clean Architecture Sign-Off

**Files:**
- All changed files

- [ ] **Step 1: Run frontend API check**
  Run: `npm run check:api --prefix frontend`

- [ ] **Step 2: Run frontend lint & typecheck**
  Run: `npm run lint --prefix frontend`
  Run: `npm run typecheck --prefix frontend`

- [ ] **Step 3: Run full frontend test suite**
  Run: `npm run test --prefix frontend`

- [ ] **Step 4: Run frontend production build**
  Run: `npm run build --prefix frontend`

- [ ] **Step 5: Run backend build and test suite**
  Run: `dotnet build backend`
  Run: `dotnet test backend`

- [ ] **Step 6: Final clean commit and report**
  Ensure all gates pass with zero errors, zero warnings.
