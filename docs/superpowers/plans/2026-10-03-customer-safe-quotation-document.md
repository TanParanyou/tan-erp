# CP-04 Customer-safe Quotation Document — Implementation Plan

**สถานะ:** Draft for execution — ค่าธุรกิจทุกข้อด้านล่างเป็น **Proposed default (TEST_ONLY)** จนกว่า Sales + Finance ยืนยัน; ห้ามถือว่า Business sign-off แล้ว
**แหล่งอ้างอิงหลัก:** [ERP Completion Plan CP-04](2026-09-29-erp-completion-master-plan.md), [Official Estimate Field Catalog](../../01-business/official-estimate-field-catalog.md), [UAT-EST-013](../../05-engineering/official-estimate-uat-scenarios.md), [Estimate API Contract](../../03-contracts/official-estimate-api-contract.md#issue-quotation)

## เป้าหมาย

ผู้ใช้ที่มีสิทธิ์เปิด Preview ของ Quotation ที่ออกแล้ว (ไทย/อังกฤษ) และสั่งพิมพ์/บันทึกเป็น PDF ผ่าน browser ได้ โดยเอกสารมาจาก server projection ที่ใช้ immutable snapshots เท่านั้น และไม่มี cost/margin/internal note/approval detail.

## ขอบเขต (In scope)

- Backend: endpoint อ่าน Quotation Document projection (read-only)
- Frontend: หน้า Preview + print stylesheet (A4), สลับภาษา th/en
- Tests: allowlist payload, cross-scope, snapshot immutability, FE render/print
- Docs: อัปเดต contract, UAT-EST-013, verification doc, roadmap/CP-04 checklist

## นอกขอบเขต (Out of scope — ห้ามทำ)

- Server-side PDF rendering / เก็บไฟล์ rendered / version-hash ของไฟล์ (ต้องมี ADR และการตัดสินใจเรื่อง library + Thai font ก่อน; แผนนี้ใช้ browser print เป็น PDF path ชั่วคราว)
- Signatures / External Acceptance (CP-07), Amendment/Void (CP-06)
- แก้ logic การคำนวณ, Issue/Accept flow, schema ของ Quotation, หรือ migration ใหม่ (ถ้าจำเป็นต้องมี migration ให้หยุดและรายงาน)
- แก้ไฟล์ที่ไม่เกี่ยวข้อง (Minimal Blast Radius)

## Proposed defaults (TEST_ONLY จนกว่า Business ยืนยัน)

**Customer-visible allowlist** (ตาม field catalog ที่ Visibility = Customer):
- Header: `number`, `issuedAtUtc` (แสดงเป็นวันที่ตาม Branch time zone ถ้ามีข้อมูลใน snapshot; ถ้าไม่มีให้ใช้ UTC date และบันทึกข้อจำกัด), `currency`, `customerReference`, `validityDays` + วันหมดอายุที่คำนวณจาก issued date
- Customer: ชื่อ/ที่อยู่/เลขผู้เสียภาษีจาก **Customer Billing Snapshot ณ วันออก** (ห้ามอ่าน master ปัจจุบัน)
- Sections: `code`, `nameTh/nameEn`, `description`, section total
- Work Items: `code`, `descriptionTh/descriptionEn`, `quantity`, `unitCode`, `scopeNote`, selling unit price, line total
- Totals: subtotal, discount, tax, grand total (ค่าจาก approved calculation snapshot เท่านั้น)
- Footer: ข้อความเงื่อนไข (payment/delivery terms) — **ไม่มีข้อมูลต้นทางในระบบ** ให้เว้นเป็น section ที่ไม่แสดง และบันทึกเป็น open question ห้ามประดิษฐ์ข้อความ

**ห้ามอยู่ใน payload เด็ดขาด:** unit cost, total cost, margin/markup, `sellingRuleType/Value/ReasonCode`, `internalNote`, `overrideReason*`, cost record/source ids, approval trigger/threshold/route/reviewer, `itemId`, snapshot hashes, row versions, user ids.

## Pre-flight (ทำก่อนแก้โค้ด และรายงานผล)

1. อ่าน `AGENTS.md`, `design.md`, `.agents/skills/building-erp-apis/SKILL.md`
2. อ่านโค้ดจริง: `backend/src/TanErp.Domain/Commercial/Quotation.cs`, `Application/Estimates/IssueQuotation/*`, `EstimateProjections.cs`, `EstimateStore.cs` (ส่วน IssueQuotation), `EstimatesController.cs`, `EstimateCalculationSnapshot.cs` และ shape ของ `CalculationSnapshotJson` ว่ามี per-line selling price/line total หรือไม่
3. ตรวจ `docs/03-contracts/permission-catalog.md` และ `frontend/src/lib/permissions/permissions.ts` ว่ามี permission สำหรับอ่านเอกสาร Quotation หรือไม่ — **ถ้ามี `quotations.issue` เท่านั้น ให้ reuse ตัวที่มีอยู่และบันทึกข้อเสนอ permission ใหม่ในรายงาน อย่าเพิ่ม permission ใหม่เอง**
4. ตรวจว่า calculation snapshot มีข้อมูลต่อรายการเพียงพอหรือไม่ ถ้าไม่พอ (ต้อง recompute หรือ join master ปัจจุบัน) **หยุดและรายงาน** ห้ามคำนวณใหม่ใน projection
5. ถ้ามี reusable document/print/export utility อยู่แล้ว (เช่น `estimate-export`) ให้ reuse; ถ้าจะสร้างของกลางใหม่ ให้เสนอใน report ก่อน (กฎ Global Reuse)

## Task 1 — Backend projection

- Endpoint: `GET /api/v1/estimates/{estimateId}/quotation/document?locale=th|en` (ปรับ route ได้ถ้า convention เดิมต่างออกไป ให้ยึด convention เดิมและบอกเหตุผล)
- Handler ใน `Application/Estimates/GetQuotationDocument/` ตามรูปแบบ handler เดิม; Controller บางๆ ไม่มี business logic
- Response เป็น typed record ที่ map จาก allowlist เท่านั้น (explicit property mapping — ห้ามส่ง entity/JsonDocument ดิบของ snapshot ออกไป)
- Scope: ตรวจ Organization/Branch ผ่าน `IRequestAccessResolver` เดิม; cross-scope/ไม่พบ → 404 แบบไม่เปิดเผย (เหมือน UAT-EST-012)
- ต้องมี Quotation ที่ออกแล้วเท่านั้น; ยังไม่ออก → 404/409 ตาม Error code convention ใน `docs/03-contracts/` (เพิ่ม error code + ข้อความ th/en ใน `Errors.resx`/`Errors.en.resx` ถ้าจำเป็น)
- ข้อมูลทุกส่วนมาจาก snapshot ของ revision ที่ผูกกับ Quotation + `CustomerBillingSnapshotJson` ห้ามอ่าน Customer/Item master ปัจจุบัน ห้ามคำนวณยอดใหม่
- ถ้า locale=en แต่ `descriptionEn` ว่าง: **ห้าม fallback เงียบๆ** ให้คืนค่า null และให้ response มี flag ระดับ document ที่บอกว่าข้อความภาษาอังกฤษไม่ครบ (หรือคืน 422 พร้อม error code ถ้า convention เดิมเหมาะกว่า) — บันทึกการตัดสินใจ
- อัปเดต OpenAPI / generated client: ดูวิธี regenerate `frontend/src/generated/api/tan-erp.v1.ts` จาก script ที่มีใน repo (API parity check) อย่าแก้ไฟล์ generated ด้วยมือ

## Task 2 — Frontend Preview/Print

- Feature อยู่ใน `frontend/src/features/estimates/` (หรือ `features/quotations/` ถ้าเห็นว่าเหมาะกว่า ให้ reuse query/client pattern เดิมใน `estimate-queries.ts`)
- ใช้ TanStack Query ดึง document; ห้าม fetch ใน `useEffect`; ใช้ API client กลาง; ใช้ Minimal Mono Loading (ห้าม Skeleton)
- Component แสดงเอกสาร: render เฉพาะค่าจาก payload (ห้ามคำนวณ/รวมยอดบน FE, ห้าม fallback chain `a || b`; ค่าว่างแสดง `-`)
- ปุ่ม "พิมพ์/บันทึก PDF" เรียก `window.print()`; เพิ่ม print stylesheet (A4, ซ่อน navigation/ปุ่ม, หลีกเลี่ยง row แตกกลางหน้า, long description wrap) ตาม `design.md` — Tailwind-first, ไม่มี border-radius, ไม่ใช้ external font/CDN
- สลับภาษา th/en ของ **เนื้อหาเอกสาร** แยกจากภาษา UI (ใช้ `locale` query ของ endpoint) และข้อความ label ทั้งหมดผ่าน i18n: เพิ่มคีย์ใน `frontend/src/messages/th.json` และ `en.json` ครบคู่
- จุดเข้า: ปุ่ม "ดูเอกสาร" ใน Estimate card/HUD เมื่อมี Quotation ที่ออกแล้ว (เปิดตาม permission เดียวกับ backend); แก้ไฟล์เดิมเฉพาะจุดที่จำเป็น
- ห้ามใช้ `any`, `as any`, `@ts-ignore`

## Task 3 — Tests (ต้องเขียนก่อน/พร้อมโค้ด และต้องรันจริง)

Backend integration (PostgreSQL, ตาม pattern `QuotationEndpointsTests.cs`):
1. serialized property-name allowlist ของ document response ตรงชุดที่กำหนด + negative scan ทั้ง JSON หา forbidden keys/ค่า (เช่น ใส่ internalNote/overrideReason/cost ที่รู้ค่าไว้ใน fixture แล้วยืนยันว่าสตริงเหล่านั้นไม่ปรากฏใน body)
2. Organization อื่น / Branch อื่น → 404 ไม่เปิดเผย
3. ยังไม่ออก Quotation → ตอบตาม error convention
4. **Immutability:** เปลี่ยนชื่อ/ที่อยู่ Customer และชื่อ Item ใน master หลังออก Quotation แล้วเรียก document ซ้ำ → ได้ข้อมูล ณ วันออกเหมือนเดิม
5. ยอดรวมใน document ตรงกับ `grandTotal` ของ Quotation และ calculation snapshot
6. locale=en เมื่อ `descriptionEn` ขาด → พฤติกรรมตามที่ตัดสินใจ (ไม่ fallback เงียบ)
7. ไม่มีสิทธิ์ → 403 `PERMISSION_DENIED`

Frontend (Vitest + Testing Library): render จาก payload, ค่าว่างแสดง `-`, ไม่มี element ของ forbidden fields, ปุ่ม print เรียก `window.print`, th/en keys ครบ

## Task 4 — Docs (ต้องทำให้ตรงกับสิ่งที่ทำจริงเท่านั้น)

- `docs/03-contracts/official-estimate-api-contract.md`: เพิ่มหัวข้อ endpoint + allowlist + forbidden list + error codes
- `docs/05-engineering/official-estimate-uat-scenarios.md`: ปรับ UAT-EST-013 ให้ตรงสถานะจริง (automated evidence เพิ่ม; **Business/Finance allowlist sign-off และ UAT เอกสารจริงยังเปิด** อย่าเขียนว่าผ่าน UAT)
- `docs/05-engineering/commercial-quotation-verification.md`: เพิ่ม section ผลทดสอบจริง (คำสั่ง, จำนวน pass/fail)
- `docs/superpowers/plans/2026-09-29-erp-completion-master-plan.md`: ติ๊กเฉพาะข้อ CP-04 ที่ทำเสร็จจริง ข้อที่เหลือ (Business ยืนยัน allowlist/terms/branding, server-side PDF + hash, 320px/200% zoom UAT) คงไว้เป็นเปิด
- `docs/00-overview/implementation-roadmap.md`: อัปเดตแถว Commercial ให้ตรง
- ถ้าเลือกใช้ browser print แทน server-side PDF ให้เขียน ADR สั้นๆ ใน `docs/adr/` (ตามรูปแบบ ADR ที่มี) ระบุ trade-off และเงื่อนไขที่ต้องยกระดับเป็น server-side PDF

## Verification gates (รันจริงทั้งหมด รายงานผลดิบ)

```bash
dotnet build backend/TanErp.slnx --no-restore -m:1
dotnet test backend/TanErp.slnx --no-build --no-restore -m:1
cd frontend && npm run lint && npm run build
cd frontend && npx vitest run
```

(เช็ค `frontend/package.json` สำหรับชื่อ script typecheck / API parity และรันด้วย) Backend integration ต้องใช้ Docker/PostgreSQL; ถ้ารันไม่ได้ให้รายงานตรงๆ ห้ามอ้างว่าผ่าน

## กฎการทำงาน

- ห้าม commit / push / แก้ git config; ปล่อยการเปลี่ยนแปลงไว้ใน working tree ให้ผู้ตรวจสอบ
- ห้ามแก้ Expected Result ของ test เพื่อให้ผ่าน ถ้า test เดิมล้มให้รายงานสาเหตุ
- ถ้าเจอสิ่งที่แผนไม่ครอบคลุมหรือขัดกับโค้ดจริง ให้หยุดและรายงานแทนการเดา
- รายงานสุดท้ายต้องมี: ไฟล์ที่แก้/เพิ่มทั้งหมด, การตัดสินใจที่ต่างจากแผน (พร้อมเหตุผล), ผล gates ดิบ, open questions สำหรับ Business, สิ่งที่ไม่ได้ทำ

## CP-04 Release readiness (production)

โค้ดของ slice นี้ผ่าน automated gates ตาม [Verification §4](../../05-engineering/commercial-quotation-verification.md#4-customer-safe-quotation-document-cp-04-2026-10-03) แต่ **ยังไม่พร้อมประกาศใช้ Production** จนกว่าข้อด้านล่างมีผู้รับผิดชอบยืนยันพร้อมหลักฐานของ release เดียวกัน (ดู [Release Readiness](../../06-operations/release-readiness.md)):

| # | เงื่อนไข | ผู้รับผิดชอบ | สถานะ |
| --- | --- | --- | --- |
| 1 | Sales + Finance ยืนยัน field allowlist, ชื่อฟิลด์ที่ลูกค้าเห็น, ภาษา, ส่วนลด/ภาษี และข้อความท้ายเอกสาร | Sales + Finance | เปิด |
| 2 | ตัดสินใจ `customerReference`, `validityDays`, `scopeNote`, payment/delivery terms, branding, page size (ต้องมี Domain + migration หากรับ) | Sales + Finance | เปิด (ตัดสินใจรอ Business) |
| 3 | มอบ `quotations.read` ให้ Role ที่ต้องดู/พิมพ์เอกสารใน Production ก่อนเปิดใช้ (ระบบ seed ให้เฉพาะ test) | เจ้าของ Role Bootstrap / Security | เปิด |
| 4 | UAT-EST-013 กับ artifact จริง: พิมพ์/บันทึก PDF จาก browser ที่ใช้งานจริง, ตรวจ pagination และคำอธิบายยาว | Sales / Quotation Issuer | เปิด |
| 5 | Accessibility: 200% zoom, keyboard ทั้งหน้า, screen reader | QA / Accessibility | เปิด (ตรวจ 320px และ overflow อัตโนมัติผ่านแล้ว) |
| 6 | ตัดสินใจว่าต้องเก็บไฟล์ + hash ของเอกสารที่ส่งลูกค้าก่อนเปิดใช้หรือไม่ ([ADR 0016](../../adr/0016-browser-print-for-quotation-pdf.md)) | Business + Security | เปิด |
| 7 | Release gates ของระบบ: migration จาก sanitized legacy data, pilot data, staging rehearsal, RPO/RTO, Go/No-go | ตาม CP-05 | เปิด |

ไม่มี schema migration ใน slice นี้ จึงไม่มีขั้นตอน migrate/rollback เฉพาะ; rollback ทำได้โดย deploy artifact เดิม (endpoint และหน้าใหม่เป็น additive).
