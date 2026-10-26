# Photo Area Measurement (วัดพื้นที่จากรูปถ่าย) — Implementation Plan

- Status: Approved by user 2026-10-26 (chat): "ทำโครงสร้างแบบโปรดักชัน ให้ยืดหยุ่น เขียนแผนพร้อมแก้บัค"
- Scope: Field Survey area items only (`frontend/src/features/surveys/components/field/`)
- Tests: no new tests (per user standing rule); keep existing tests compiling/valid. Gates: `dotnet build`, `npx tsc --noEmit`, `npm run lint`, `npm run build`.

## Goal

ช่างหน้างานวัดพื้นที่ (ตร.ม.) จากรูปถ่าย 1 รูป โดยวางวัตถุอ้างอิงที่รู้ขนาด (ค่าเริ่มต้น A4) บนระนาบเดียวกับพื้นที่ที่วัด แตะ 4 มุมวัตถุอ้างอิง แล้วแตะมุมพื้นที่ (รูปหลายเหลี่ยม ≥3 จุด) ระบบแก้ perspective ด้วย homography และคำนวณพื้นที่ แล้วเติมค่าเข้า Measurement Pad เดิม

## Decisions

1. **Reference = planar rectangle of known size.** Presets `a4` (210×297 mm), `a3` (297×420 mm), `custom` (user enters width/height mm). Long image side pair maps to the long reference dimension.
2. **Homography (DLT, 4 point pairs)** maps image px → plane mm. Solves tilt; valid only for the reference's plane.
3. **Polygon, not rectangle.** Any vertex count ≥ 3; area via shoelace in plane mm. Rectangle = 4 vertices.
4. **No new storage of geometry.** The annotated photo (polygon + reference drawn on it) is the evidence. Persisted value stays one `area` measurement in `sqm`.
5. **New capture method `photo_estimated`** (Domain constant + DB check constraint + migration) so estimators can tell photo estimates from tape measurements. API contract field is already a free `string` — no OpenAPI change.
6. **Manual override wins.** Editing gross width/length by hand clears the photo measurement (value becomes `measured` again).
7. **Bug fix (in scope):** Field Survey save sends gross area (w×l) and drops deductions. Must send net area = gross − deductions, rounded 2 dp, floor 0.

## Known limits (show as hint text in UI)

- Accuracy ~2–5 %, only on a flat surface that contains the reference; use for estimating, confirm before quotation.
- Field Survey photos (all of them, not only this feature) are still local blob URLs and are not uploaded on save — existing gap, out of scope (follow-up).

## Task 1 — Backend: `photo_estimated` capture method

Files:
- `backend/src/TanErp.Domain/Surveys/SurveyValues.cs` — add `public const string PhotoEstimated = "photo_estimated";` to `CaptureMethod` and to `All`.
- `backend/src/TanErp.Infrastructure/Persistence/Configurations/SiteSurveyMeasurementConfiguration.cs` — check constraint becomes `capture_method IN ('measured', 'customer_provided', 'derived', 'photo_estimated')`.
- New EF migration `AddPhotoEstimatedCaptureMethod` generated with `dotnet ef migrations add` (drop + re-add the check constraint; Down restores the old list). Do NOT touch the existing uncommitted migration `20261026012153_AddUserPreferredLocale`.
- Grep backend for other capture-method whitelists/validators (FluentValidation, Dapper SQL, seeders) and update consistently.

Done when `dotnet build` (backend solution) succeeds.

## Task 2 — Frontend core: geometry + schema + net-area bug fix

1. `frontend/src/lib/geometry/plane-measurement.ts` (new, global reuse — pure functions, no React):
   - `type Point2D = { x: number; y: number }`
   - `computeHomography(src: readonly Point2D[4], dst: readonly Point2D[4]): Homography | null` (8×8 Gaussian elimination with partial pivoting; `null` when singular/degenerate)
   - `applyHomography(h, p): Point2D`
   - `orderQuadClockwise(points): Point2D[]` — sort by angle around centroid, start at top-left (min x+y)
   - `polygonAreaAbs(points): number` (shoelace)
   - `isSimplePolygon(points): boolean` (no self-intersection of non-adjacent edges)
   - `measurePlanePolygon({ referenceQuadPx, referenceSizeMm: { widthMm, heightMm }, polygonPx }): PlaneMeasurementResult | null` → `{ areaSqm, widthMm, lengthMm, vertexCount }` where width/length = average opposite edges when 4 vertices, else world bounding box; maps long image side pair to long reference side.
2. `frontend/src/features/surveys/schemas/field-survey-schema.ts`:
   - `photoMeasurementSchema = z.object({ grossAreaSqm: z.number().min(0), vertexCount: z.number().int().min(3), referenceKind: z.enum(["a4","a3","custom"]), referenceWidthMm: z.number().positive(), referenceHeightMm: z.number().positive() })`
   - `areaDimensionsSchema` gains `photoMeasurement: photoMeasurementSchema.nullable().optional()`
   - export `resolveGrossAreaSqm(d: AreaDimensions): number` (photo value when present else w×l/1e6) and `resolveNetAreaSqm(d: AreaDimensions): number` (reuse `calculateNetAreaSqm` logic; keep `calculateNetAreaSqm` signature unchanged — existing tests use it).
3. Bug fix in `field-survey-workspace.tsx`: the save mapper (~line 308) and the item summary (~line 772) use `resolveNetAreaSqm`; `captureMethod` = `"photo_estimated"` when `areaDimensions.photoMeasurement` is set, else `"measured"`. `field-survey-measurement-pad.tsx` gross/net display uses the resolvers; manual width/length edits set `photoMeasurement: null`.

## Task 3 — Frontend UI: Photo measure modal

New `frontend/src/features/surveys/components/field/field-survey-photo-area-measure.tsx` (modal via `@/components/ui/Modal`, full-screen on mobile):

Steps (single screen, step indicator):
1. **Photo** — reuse the existing file input pattern (`accept="image/*" capture="environment"`) so the native camera opens on mobile; desktop picks a file.
2. **Reference** — `Select` preset (A4/A3/custom; custom shows two number `Input`s, mm). Tap 4 corners.
3. **Area** — tap ≥3 vertices; "close polygon" button; undo-last-point; clear.
4. Live result panel: area m² (2 dp), width/length mm, vertex count; validation alerts (`Alert`): reference not 4 points, degenerate homography, self-intersecting polygon.
5. **Confirm** → `onApply({ photoMeasurement, grossWidthMm, grossLengthMm, annotatedPhoto: FieldSurveyPhoto })`.

Interaction requirements:
- Pointer events on an SVG overlay sized to the displayed image; convert to natural-image px via the rendered rect. `touch-action: none` on the overlay.
- Points draggable after placement; magnifier loupe (square, 0 radius, ~2.5×, crosshair) shown while a pointer is down, placed in the corner away from the finger.
- Reference quad drawn in one colour, area polygon in Navy with translucent fill; vertex handles ≥ 44px hit area.
- Annotated photo: draw image + overlays + area label on a canvas at natural resolution (max long side 2048 px), `toBlob("image/jpeg", 0.85)`, `URL.createObjectURL`, photo tag `measurement_sketch`, caption = formatted area.
- Revoke object URLs that are discarded; disable confirm while rendering (double-submit protection).
- Atelier rules: 0 radius, SVG stroke icons from `@/components/common/Icons` (add a new icon there only if none fits), Tailwind tokens, `Button`/`Tooltip`/`Input`/`Select`/`Alert` from direct sub-path imports, every input has i18n placeholder.

Wiring (`field-survey-measurement-pad.tsx` + `field-survey-workspace.tsx`):
- Icon-only `Button size="icon"` + `Tooltip` beside the area width/length inputs opens the modal.
- On apply: `onAreaChange({ ...areaDimensions, grossWidthMm, grossLengthMm, photoMeasurement })` and append the annotated photo to the item's photos.
- Show a small `Badge` "ประเมินจากรูป / Photo estimate" in the pad when `photoMeasurement` is set.

Desk view: `survey-measurement-row.tsx` (or the area card that lists measurements) shows a capture-method `Badge` when `captureMethod === "photo_estimated"`, label via i18n.

i18n: all strings in both `frontend/src/messages/th.json` and `en.json` (namespace `fieldSurvey.photoMeasure.*`, capture-method label in the surveys namespace used by the desk view).

## Verification

- Backend: `dotnet build` (solution).
- Frontend: `npx tsc --noEmit`, `npm run lint`, `npm run build`.
- Geometry sanity check by the reviewer with a throwaway script (not a committed test): identity homography and a tilted A4 case reproduce known areas.
- No tests written or run (per user).
