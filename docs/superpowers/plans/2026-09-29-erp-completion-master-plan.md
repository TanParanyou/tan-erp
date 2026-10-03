# ERP Completion Master Plan (แผนรวมปิดช่องว่าง Project ERP)

**สถานะ:** Draft สำหรับงานใหม่; งานปิด slice เดิมใช้ขอบเขตที่อนุมัติในแผนเดิม
**วันที่:** 2026-09-29 (ผล verification ปรับปรุง 2026-09-30)
**ผู้อ่าน:** เจ้าของงาน, Business/Finance/Security, ทีมพัฒนาและ Operations
**เป้าหมาย:** ให้ทุกส่วนที่ยังขาดมีลำดับงาน, dependency, ผู้รับผิดชอบตามหน้าที่, deliverables และเกณฑ์ตรวจรับ โดยไม่ทำซ้ำแผนเดิมหรือกำหนดกฎธุรกิจแทนผู้อนุมัติ

## 1. แหล่งหลักและขอบเขตแผน

- สถานะ implementation และหลักฐาน: [Implementation Roadmap](../../00-overview/implementation-roadmap.md)
- ขอบเขตผลิตภัณฑ์: [Scope](../../00-overview/scope-and-non-goals.md)
- Requirement IDs: [Requirements Catalog](../../00-overview/requirements-catalog.md)
- Ownership: [Module Boundaries](../../02-architecture/module-boundaries.md)
- คำศัพท์: [CONTEXT.md](../../../CONTEXT.md)
- มาตรฐานพัฒนาและ reuse: [AGENTS.md](../../../AGENTS.md), [Design System](../../../design.md), [Shared Components](../../frontend/shared-components-guide.md)
- เกณฑ์ส่งมอบ: [Definition of Done](../../05-engineering/definition-of-done.md), [Release Readiness](../../06-operations/release-readiness.md)

แผนนี้เป็นแหล่งหลักของ **งานเตรียมและปิดช่องว่างข้ามโมดูล**. กฎธุรกิจ, API และข้อมูลยังใช้เอกสารเจ้าของเรื่องเดิม. เมื่อสร้างแผน implementation แยก ให้เปลี่ยนรายการ CP ที่เกี่ยวข้องเป็นลิงก์ไปแผนนั้นและคงเฉพาะ dependency/สถานะไว้ที่นี่.

งานรอบนี้ปรับเอกสารและแผนงานเท่านั้น. รายการ Draft/Future ด้านล่างยังไม่ใช่การอนุมัติสร้าง API, schema, UI หรือ deploy. Business Flow และ Architecture Flow เดิมไม่เปลี่ยน จึงไม่แก้ Portal JSON/schema ในรอบนี้; เมื่อ task ใหม่เปลี่ยน flow ต้องปรับ JSON ที่เกี่ยวข้องพร้อมเอกสารหลัก.

## 2. วิธีเปิดงานและบันทึกหลักฐาน

แต่ละ CP ต้องมี task ระบุ scope และผู้รับผิดชอบจริงก่อน implementation. ชื่อหน้าที่ในแผนยังไม่ใช่บุคคลที่รับรองแล้ว. ห้ามใช้ค่าจาก TEST_ONLY เป็น policy/price/authority ของ Production.

ก่อนเริ่ม slice ใหม่ ให้ส่งมอบชุดต่อไปนี้ภายใน task เดียวกัน:

1. Requirement IDs และ business scenarios ที่จะส่งมอบ พร้อมเรื่องที่เลื่อนและผู้ตัดสินใจ
2. แผนสำรวจ reuse ของ component/hook/service/contract; เสนอ Global Reuse ก่อนสร้างส่วนกลางใหม่ตาม AGENTS.md
3. Business invariants, state transitions, field gates, permission/scope/authority และ privacy allowlist ที่ผู้มีอำนาจยืนยัน
4. API/OpenAPI/error contracts, data/transaction/migration design และ UI ไทย/อังกฤษตาม Design System
5. Acceptance matrix ครอบคลุม success, invalid state, permission/scope, concurrency, retry/idempotency และ immutable history ตามความเสี่ยง
6. คำสั่งตรวจสอบและบันทึกผลที่ระบุ code/schema/artifact, environment, วันที่, exit code, coverage และข้อจำกัด

ใช้เอกสารหลักตาม docs/README.md ก่อนสร้างเอกสารใหม่. หากเป็น concern ใหม่ให้ลงทะเบียนใน docs/README.md; การตัดสินใจ trade-off ที่ถาวรให้บันทึก ADR. นโยบาย, วงเงิน, สถานะหรือ Permission ใหม่ที่ยังไม่มี contract เป็น **Validation Question** จนได้รับคำตอบ.

## 3. รายการงานและ dependency

| ID | งาน | สถานะ ณ วันจัดแผน | Dependency | เจ้าของตามหน้าที่ |
| --- | --- | --- | --- | --- |
| CP-01 | ปิดช่องว่าง Official Estimate | มีแผนเดิม; baseline/release gates ยังเปิด | โค้ด Customer/Survey/Item/Estimate ปัจจุบัน | Estimation + Business/Finance/Security |
| CP-02 | Identity/Organization Administration | Draft slice ใหม่ | Foundation/RBAC | System Administrator + Security |
| CP-03 | Master Data/Survey Baseline Completion | มีแผนเดิมบางส่วน; Import/Survey extension เป็น Draft | Customer/Item/Survey contracts | Data Steward + Cost Owner + Survey Lead |
| CP-04 | Customer-safe Quotation Document | Draft slice ใหม่ | Approved Estimate + Billing Snapshot + Numbering | Sales + Finance |
| CP-05 | Release/UAT/Operations | Checklist มีแล้ว; ยังต้องหลักฐาน release | ทุก critical slice ใน release ที่เลือก | Business/Finance/Security + Operations |
| CP-06 | Quotation Amendment/Void | Implemented 2026-10-04 (กฎที่ทีมพัฒนาเลือก; accepted ถูกล็อก, void ไม่ย้อน Opportunity) | Quotation/Snapshot contracts; CP-04 สำหรับเอกสาร | Sales + Finance |
| CP-07 | External Customer Acceptance/Signatures | Draft slice ใหม่ | CP-04 + authentication/access policy | Sales + Security + Legal/Business |
| CP-08 | Won → Project Handover | Implemented 2026-10-04 (ค่าเริ่มต้นที่ทีมพัฒนาเลือก รอ Sales/Project Owner ยืนยัน) | Accepted Quotation; CP-06 rules เมื่อเปิดใช้ | Sales + Project Owner |
| CP-09 | Project Budget/Plan/Change Order/Progress | Implemented 2026-10-04 (กฎที่ทีมพัฒนาเลือก; ไม่มี Actual Cost/WBS) | CP-08 | Project Manager + Finance |
| CP-10 | Supplier/Procurement | Implemented 2026-10-04 (Supplier → PO → Receipt; กฎที่ทีมพัฒนาเลือก; ไม่มี PR/Return) | Item/Unit + CP-09 demand/budget | Procurement + Finance |
| CP-11 | Inventory/Warehouse | Implemented 2026-10-04 (กฎที่ทีมพัฒนาเลือก; ไม่มี lot/serial/location/ปิดงวด) | CP-10 receipt contract + Item/Unit | Warehouse + Procurement |
| CP-12 | BOM/Production | Implemented 2026-10-04 (กฎที่ทีมพัฒนาเลือก; ไม่มี routing/capacity/subcontracting/ค่าแรง) | CP-09 scope + CP-11 stock | Engineering/Production |
| CP-13 | MRP | Implemented 2026-10-04 (กฎที่ทีมพัฒนาเลือก; lot-for-lot, ไม่มี safety stock/MOQ/ปฏิทิน) | CP-10 lead time + CP-11 stock + CP-12 BOM/plan | Production Planner + Procurement |
| CP-14 | Installation/Handover/Warranty/Service | Future slice | CP-08/09; CP-12 หากมีงานผลิต | Installation + Customer Service |
| CP-15 | Billing/Payment/Accounting Integration | Future slice | Customer billing + Commercial/Project; Supply เมื่อรวมยอดจัดซื้อ | Finance/Accounting |
| CP-16 | Quick Estimate | Optional Future; มีแผนเอกสารแล้ว | Official Estimate Foundation + approved template/share policies | Sales/Estimator + Cost Owner |

CP-02/03/05 เตรียมคู่ขนานกับ CP-01 ได้ตามขอบเขตที่อนุมัติ. CP-08 ใช้ customer acceptance ภายในที่มีอยู่ได้ ไม่ต้องรอ CP-07. CP-14 และ CP-15 เปิดแยกได้เมื่อ upstream contract ที่ต้องใช้พร้อม โดยไม่ต้องรอ MRP. Import ไม่ใช่ gate บังคับของ pilot ถ้าข้อมูลเติมและตรวจผ่าน workflow เดิมได้และมีบันทึกข้อจำกัด.

## 4. งานย่อยและเกณฑ์ตรวจรับ

### CP-01 — Official Estimate Completion

ใช้ [Official Estimate Completion Plan](2026-09-27-official-estimate-completion.md) เป็นแผน implementation หลัก โดยเฉพาะ Task 7. ใช้ [Approval Matrix](../../01-business/approval-matrix.md) และ [UAT baseline](../../05-engineering/official-estimate-uat-scenarios.md) เป็นกฎและหลักฐาน ไม่คัดลอกขั้นตอนเดิมมาทำแผนซ้ำ.

- [x] UAT-EST-003 implementation: API gate `ESTIMATE_UNIT_INVALID` สำหรับ Cost Component ที่ไม่ตรงหน่วยฐาน Catalog Item, bilingual Problem Details และ UI mapping มี regression tests แล้ว; Estimate API class ผ่าน 40/40 บน disposable PostgreSQL. Manual UAT ยังเปิดอยู่. Item/Shared Unit Conversion ใน Estimate ยัง deferred จนกว่าจะเก็บ immutable conversion snapshot ได้.
- [x] UAT-EST-006 implementation: sequential lifecycle/profile `TEST_ONLY-TH-EST-V1` รวม custom-work-item trigger ผ่าน Estimate API 40/40; Item Catalog Estimate flow ผ่าน 1/1 บน PostgreSQL พร้อม structured Item Master snapshot. คง manual UAT และนโยบายจริง/ผู้อนุมัติ sign-off ไว้เป็น release gates; Production ยังคง Bootstrap checker
- [x] UAT-EST-012: PostgreSQL API matrix verifies read/update/calculate/approve from another Organization and another Branch returns non-disclosing 404; denied writes preserve Estimate/revision/snapshot/approval state. Authorized-user UAT remains a release gate.
- [x] UAT-EST-013 API boundary: บันทึกฟิลด์ของ `QuotationResponse`, integration regression ยืนยัน serialized field allowlist, ยืนยันว่าไม่มี Preview/Export/Document endpoint ใน slice ปัจจุบัน และส่งงาน customer output/UAT ไป CP-04; ดู [API contract](../../03-contracts/official-estimate-api-contract.md#issue-quotation) และ [UAT baseline](../../05-engineering/official-estimate-uat-scenarios.md#uat-est-013--customer-safe-output). Document UAT ยัง deferred.
- [ ] ผูก pilot data, historical quotation audit/migration และ UAT/policy sign-off ที่แผนเดิมยังเปิดไว้เข้ากับ CP-05

**ผลส่งมอบ/เกณฑ์จบ:** ข้อค้างแต่ละ UAT ID มี implementation+หลักฐานตรง Expected Result หรือบันทึก deferral ที่ระบุผู้ตัดสินใจและผลกระทบ; ไม่มีการรับรอง completion จากจำนวน tests เพียงอย่างเดียว.

### CP-02 — Identity/Organization Administration

ฐาน Login/Current User/RBAC มีแล้ว; ยังต้องหน้าจัดการผู้ใช้และสิทธิ์ครบวงจร. อ้าง [RBAC](../../03-contracts/rbac.md), [Permissions](../../03-contracts/permission-catalog.md), [Authentication](../../03-contracts/authentication.md) และ Foundation schema เดิม.

- [x] ยืนยันวิธีเพิ่มผู้ใช้/เชื่อม Firebase identity, Membership, Role Assignment; ใครมีสิทธิ์จัดการใคร และต้องมีผู้ตรวจแยกเมื่อใด (ตัดสินแล้ว 2026-10-03; ดู [CP-02 plan](2026-10-03-identity-organization-administration.md). Organization/Branch CRUD และ Approval Authority matrix ยังนอกขอบเขตรอบแรก)
- [x] กำหนด administration contracts และ handlers โดย PostgreSQL เป็นเจ้าของสิทธิ์; reuse Request Context/Scope/Audit และ transaction/concurrency เดิม ([contract](../../03-contracts/identity-administration-api-contract.md))
- [x] ทำ List/Form/Confirmation ไทย/อังกฤษพร้อม conflict/retry และคำอธิบายผลกระทบการถอนสิทธิ์; ไม่มี DB write จาก FE (ยังไม่ได้ตรวจ 200% zoom/keyboard/screen-reader)
- [x] ทดสอบ cross-organization, privilege escalation, ถอน role/membership แล้ว request ถัดไปใช้สิทธิ์ไม่ได้ (automated; logout/cache isolation ของ FE ยังไม่มี test เฉพาะ)

**ผลส่งมอบ/เกณฑ์จบ:** ผู้ดูแลที่ได้รับอนุญาตทำและถอน assignment ผ่าน UI/API ได้ พร้อม Audit; ผู้ไม่มีสิทธิ์เปลี่ยนไม่ได้. กฎป้องกันการสูญเสียผู้ดูแลทั้งหมดและการกู้สิทธิ์ต้องยืนยันก่อนออก contract.

### CP-03 — Master Data/Survey Baseline Completion

Reuse [Item Completion Plan](2026-09-22-item-master-estimate-catalog-completion.md), [Maintenance Plan](2026-09-23-estimate-master-data-maintenance.md), [Item Governance](../../01-business/item-master-governance.md) และ [CRM/Survey Governance](../../01-business/crm-site-survey-governance.md). Customer/Site lifecycle ที่มีแล้วใช้ [Customer Verification](../../05-engineering/customer-completion-verification.md); ไม่สร้างซ้ำ.

**Baseline audit 2026-09-30:** ตารางนี้อ้าง code/API ที่มีจริงและ [Survey Verification](../../05-engineering/site-survey-verification.md), [Item Verification](../../05-engineering/item-master-estimate-catalog-verification.md); `partial` ไม่ใช่ผ่าน UAT.

| Requirement | สถานะ code | งานที่ยังต้องปิด | หลักฐาน API/Data/UI/UAT |
| --- | --- | --- | --- |
| FR-ITEM-001–004 | Implemented: Item lifecycle, versioned Cost/Maker–Checker และ deterministic branch-aware resolver | ตรวจ pilot data/ราคา/สิทธิ์กับผู้ใช้จริง | [Item API](../../03-contracts/item-master-api-contract.md), [Item Data](../../04-data/item-master-data-contract.md), [Item Verification](../../05-engineering/item-master-estimate-catalog-verification.md), [Item UAT](../../05-engineering/item-master-uat-scenarios.md) |
| FR-ITEM-005 | Implemented ใน Item Master: exact/item-specific conversion และ guard มิติ/วงรอบ | ถ้าจะใช้หน่วยที่แปลงใน Estimate ต้องตรึง conversion snapshot ก่อน | [Item API](../../03-contracts/item-master-api-contract.md), [Item Data](../../04-data/item-master-data-contract.md), [Item Verification](../../05-engineering/item-master-estimate-catalog-verification.md), [Item UAT](../../05-engineering/item-master-uat-scenarios.md) |
| FR-ITEM-006 | Implemented Phase 1 (2026-10-04): CSV `createOnly` Preview/Validate/Atomic Commit + UI; Draft only ไม่แตะ Cost | ยืนยัน format/duplicate rule กับ Data Steward; `upsert`/Cost/Batch/File Service เป็น Future | [Item API](../../03-contracts/item-master-api-contract.md), [Item Data](../../04-data/item-master-data-contract.md), [Item Verification](../../05-engineering/item-master-estimate-catalog-verification.md), [Item UAT](../../05-engineering/item-master-uat-scenarios.md) ระบุการบำรุงข้อมูลผ่าน workflow ปัจจุบัน; Import ยังไม่มี runtime evidence |
| FR-ITEM-007 | Partial: Estimate ตรึง Cost Record และ Calculation/Tax Policy | Conversion snapshot ยังไม่มี; คง base-unit gate จนกว่าจะมี snapshot | [Estimate API](../../03-contracts/official-estimate-api-contract.md), [Estimate Data](../../04-data/official-estimate-data-contract.md), [Estimate Verification](../../05-engineering/official-estimate-verification.md), [Estimate UAT](../../05-engineering/official-estimate-uat-scenarios.md) |
| FR-SRV-001 | Implemented baseline (2026-10-04): visit/scope, Area/Measurement, Assumption/Constraint, Checklist และ Evidence ผูก Revision | ยืนยัน Checklist/จำนวนหลักฐานกับ Survey Owner; evidence ต่อ Area; UAT | [Survey API](../../03-contracts/crm-site-survey-api-contract.md), [Survey Data](../../04-data/crm-site-survey-data-contract.md), [Survey Verification](../../05-engineering/site-survey-verification.md), [Survey UAT](../../05-engineering/crm-site-survey-uat-scenarios.md) |
| FR-SRV-002 | Partial: Draft version check, Ready immutability, Clone/Void Revision และ supersede เมื่อ Ready ใหม่ มี backend แล้ว และ UI (2026-10-04; รัน test เฉพาะ survey แล้ว ยังไม่รัน full gate) | รัน gate เต็ม และ UAT | [Survey API](../../03-contracts/crm-site-survey-api-contract.md), [Survey Data](../../04-data/crm-site-survey-data-contract.md), [Survey Verification](../../05-engineering/site-survey-verification.md), [Survey UAT](../../05-engineering/crm-site-survey-uat-scenarios.md) |
| FR-SRV-003 | Implemented baseline (2026-10-04): template registry `v1/v2` แบบ code, `GET survey-template-versions`, Ready gate checklist/evidence แบบ fail-closed | Template แบบ DB/effective period และ Business-approved template ยังไม่มี | [Survey API](../../03-contracts/crm-site-survey-api-contract.md), [Survey Data](../../04-data/crm-site-survey-data-contract.md), [Survey Verification](../../05-engineering/site-survey-verification.md), [Survey UAT](../../05-engineering/crm-site-survey-uat-scenarios.md) |
| FR-SRV-004 | Implemented: Estimate ตรึง Ready Revision ID/hash; `v2` สำหรับ template v1 และ `v3` (รวม Checklist/Evidence manifest) สำหรับ template v2 | hash version ใหม่เมื่อเพิ่ม labels/actor-time; UAT reference/history ยังเปิด | [Survey Data](../../04-data/crm-site-survey-data-contract.md), [Estimate Data](../../04-data/official-estimate-data-contract.md), [Survey Verification](../../05-engineering/site-survey-verification.md), [Estimate UAT](../../05-engineering/official-estimate-uat-scenarios.md) |

- [x] Baseline audit FR-ITEM-001–007 และ FR-SRV-001–004: ตารางด้านบนผูกสถานะ code กับ API/Data/UI/UAT evidence; `partial` และ `missing` ยังคงเป็นงานเปิด ไม่ถือว่าผ่าน UAT.
- [x] (Phase 1 เสร็จ 2026-10-04 ด้วย default ที่ทีมพัฒนาเลือก; ส่วน upsert/Cost/Batch ยังเป็น Future) เปิด Import slice ตาม FR-ITEM-006: ยืนยัน file format/size/duplicate rule; Preview/Validate แสดง error ต่อแถว; Commit แบบ atomic/idempotent โดยไม่ auto-publish Cost; สร้าง permission/contract/migration และ test rollback ก่อนใช้จริง
- [x] (เสร็จ 2026-10-04 ด้วย default ที่ทีมพัฒนาเลือก รอ Survey Owner ยืนยัน) เปิด Survey extension ตามช่องว่างที่ยืนยัน: Checklist/Evidence/Versioned Readiness Template, New/Void Revision และการเข้าถึงไฟล์; ยืนยัน business effect ต่อ Opportunity/Estimate ก่อนสร้าง routes
- [x] (2026-10-04: integration test ยืนยัน Revision เดิม/hash ไม่เปลี่ยนเมื่อมี Ready ใหม่หรือ Void, Estimate ยังอ้าง Revision เดิม, audit `referencedByEstimate`, ไฟล์ไม่ verified ถูกปฏิเสธ) ตรวจ hash/reference invariants: Ready Revision เดิมและ Estimate ที่อ้างไว้ไม่เปลี่ยนตาม revision ใหม่; ไฟล์ยังไม่ผ่าน readiness ต้องไม่ถูกใช้เป็นหลักฐานที่อนุมัติ
- [ ] เตรียม pilot Item/Unit/Conversion/Published Cost/Evidence ผ่านหน้าดูแลและผู้ตรวจอิสระ; ราคาจริงอยู่ในระบบที่อนุญาต ไม่เก็บใน repo

**ผลส่งมอบ/เกณฑ์จบ:** baseline แต่ละ requirement มี coverage/disposition ที่ตรวจได้; Import/Survey extension ที่เลือกส่งมอบมี end-to-end journey และ failure/concurrency/scope tests. การเพิ่ม Conversion ไม่ใช่หลักฐานว่า Estimate ทุกกรณีตรวจ Unit Compatibility แล้ว (ยังเป็น CP-01).

### CP-04 — Customer-safe Quotation Document

อ้าง FR-QUO-001/002, UAT-EST-013 และ [Estimate Field Catalog](../../01-business/official-estimate-field-catalog.md). Reuse immutable calculation/approval/customer billing snapshots และ numbering; ไม่ใช้ข้อมูล master ปัจจุบันสร้างเอกสารย้อนหลัง.

- [ ] ยืนยัน field allowlist ภาษา/รายละเอียดงาน, ยอดและส่วนลด/ภาษี, validity/payment/delivery terms, branding, page size และวิธีจัดเก็บ rendered document/version/hash (ใช้ Proposed default TEST_ONLY ใน [CP-04 plan](2026-10-03-customer-safe-quotation-document.md); ยังรอ Business)
- [x] กำหนด server projection ของรายการ Quotation และเอกสาร (implemented 2026-10-03, proposed allowlist); ใช้ approved snapshot totals โดยไม่คำนวณยอดใหม่บน FE และไม่ส่ง cost/margin/internal note/approval detail
- [ ] (บางส่วน: Preview + Browser Print ไทย/อังกฤษ ตาม [ADR 0016](../../adr/0016-browser-print-for-quotation-pdf.md); ยังไม่มี server-side PDF/artifact/hash และยังไม่ตรวจ long-description layout จริง) ทำ Preview/Export/PDF/Printing ไทย/อังกฤษพร้อม pagination, font/asset ownership และ long-description layout; เสนอ reuse ของ document renderer/export ก่อนสร้างของใหม่
- [ ] (บางส่วน: payload allowlist, cross-scope, billing snapshot ณ วันออกมี automated test; ไฟล์จริง, keyboard/320px/200% zoom และ print layout ยังไม่ตรวจ) ทดสอบ payload และไฟล์จริงด้วย allowlist, cross-scope access, ข้อมูล billing ณ วันออก, re-render ฉบับเก่าเมื่อ master เปลี่ยน, keyboard/320px/200% zoom และ print layout

**ผลส่งมอบ/เกณฑ์จบ:** ลูกค้าได้รับเอกสารที่ Business ยืนยัน ซึ่งย้อนกลับ quotation/revision/snapshot ได้และไม่เผยข้อมูลภายใน; UAT-EST-013 ตรวจทั้ง response และ artifact. Signatures ที่ต้องมีการยืนยันตัวตน/หลักฐานการลงนามอยู่ CP-07.

### CP-05 — Release/UAT/Operations

ใช้ [Release Readiness](../../06-operations/release-readiness.md) เป็น checklist หลัก. Operations ใช้ [Environments](../../06-operations/environments.md), [Deployment](../../06-operations/deployment.md), [Observability](../../06-operations/observability.md), [Backup/Restore](../../06-operations/backup-and-restore.md) และ [Incident Response](../../06-operations/incident-response.md).

- [x] Full backend code gate: หลังแก้ Survey hash `dotnet build backend/TanErp.slnx --no-restore -m:1` ผ่าน 0 warnings/errors; `dotnet test backend/TanErp.slnx --no-build --no-restore -m:1` ผ่าน Integration 289/289, Unit 288/288 และ Architecture 3/3 เมื่ออนุญาต Docker Desktop socket; ดู [Site Survey Verification](../../05-engineering/site-survey-verification.md#4-snapshot-hash-follow-up-2026-09-30). ระบุ release scope/code/schema/artifact และ known limitations สำหรับ release จริงยังค้าง.
- [x] Full backend code gate passed on 2026-09-30: build passed with 0 warnings/errors; Integration 289/289, Unit 288/288 and Architecture 3/3. A later recheck of the same code passed 288/289 integration tests; one test failed during PostgreSQL fixture startup with an SSL response error, then passed on two isolated reruns (1/1 each). Root cause of the full-suite startup error remains unconfirmed. See [Site Survey Verification](../../05-engineering/site-survey-verification.md#5-full-suite-recheck-2026-09-30). Release scope/code/schema/artifact and known limitations for an actual release remain open.
- [x] Latest full backend code gate after Opportunity search and Estimate review concurrency fixes (2026-09-30): build passed with 0 warnings/errors; Integration 292/292, Unit 288/288 and Architecture 3/3 passed with exit 0 and clean TRX counters. Frontend lint/typecheck/API parity, Vitest 608/608, default production build and Chromium Estimate journey 1/1 passed. See [Estimate Verification](../../05-engineering/official-estimate-verification.md#review-decision-concurrency-follow-up-2026-09-30). Earlier non-green/canceled runs remain historical evidence in [Opportunity Verification](../../05-engineering/opportunity-hardening-verification.md#4-case-insensitive-search-recheck-2026-09-30); the earlier SSL startup cause remains unconfirmed. Production release gates below remain open.
- [ ] ทดสอบ migration บน sanitized legacy copy และ historical quotations/files/costs; หาก release เป็นฐานใหม่ให้เจ้าของข้อมูลรับรองเงื่อนไขนั้น ห้ามถือ empty-database rehearsal แทน upgrade rehearsal
- [ ] Business/Finance ยืนยัน pilot prices, policies/authority/tax และ authorized-role UAT; Security ตรวจ scope/PII/private files และ permission revocation
- [ ] ตรวจ tablet/200% zoom/full keyboard/screen-reader/ไทย–อังกฤษ และ output/print ที่อยู่ใน release; technical E2E ไม่แทน business UAT
- [ ] ระบุ hosting/domain/secrets/HTTPS, health/logs/traces/alerts, rate/dependency failure, capacity และ support owner; ยืนยัน RPO/RTO ก่อนจัด Backup Schedule และทำ Restore Drill จริง
- [x] Local database-only restore rehearsal (2026-09-30, code `d77410e`): กู้ลง PostgreSQL แยกและตรวจ schema, counts/hashes ครบ 58 ตาราง/11,084 แถว, migration history 41 รายการ และ document counters ตรงกัน; ต้นทางไม่เปลี่ยนและลบปลายทางชั่วคราวแล้ว. ดู [Backup/Restore evidence](../../06-operations/backup-and-restore.md#local-database-restore-rehearsal--2026-09-30). File bytes, identity/config/secrets, staging recovery และ Business Owner/RPO/RTO approval ยังเปิด; ไม่ถือว่าผ่าน Production restore gate.
- [ ] ทำ staging rehearsal, immutable artifact promotion, rollback/forward-fix decision และ Go/No-go Record โดยผู้มีอำนาจ; เก็บผลและข้อจำกัดในเอกสาร verification/runbook เจ้าของเรื่อง
- [x] เตรียมเอกสารสำหรับข้อบน: [Go/No-go Record Template](../../06-operations/go-no-go-record-template.md) และ [Staging Rehearsal Checklist](../../06-operations/staging-rehearsal-checklist.md) (Draft; ยังไม่ได้ซ้อมจริง). พบช่องว่างที่ต้องปิดก่อนซ้อม: liveness/readiness endpoint (PR #13), CI `verify.yml` ที่ไม่เคยรันสำเร็จ (PR #12), ยังไม่มี build/publish ของ immutable artifact และยังไม่ได้เลือก Hosting

**ผลส่งมอบ/เกณฑ์จบ:** ทุกข้อที่เกี่ยวข้องใน Release Readiness มีหลักฐานของ release เดียวกันหรือ disposition ที่อนุมัติ; full backend code gate ผ่านแล้ว แต่ migration จาก sanitized legacy data, pilot, UAT, Accessibility และ Operations/Go-No-Go ยังคงเปิด.

### CP-06 — Quotation Amendment/Void

- [x] Sales/Finance ยืนยันสถานะที่แก้/ยกเลิกได้, ผลต่อ Acceptance/Opportunity/Project และเลขที่เอกสาร; นิยาม amendment แยกจาก Estimate Revision
- [x] ออกแบบ immutable history/linked replacement พร้อมเหตุผล/authority/expected versions/idempotency; คง snapshots เดิมและป้องกัน concurrent accept/amend/void
- [x] ทำ API/UI พร้อม safety confirmation/loading และ audit แบบไม่เผยข้อมูลอ่อนไหว; เอกสารใหม่ใช้ renderer ของ CP-04
- [x] ทดสอบย้อนหลังฉบับเก่า, replay, race conditions, denied state/scope และการส่งต่อที่เกิดขึ้นแล้ว

**ผลส่งมอบ/เกณฑ์จบ:** ทุก lifecycle transition ตรงกฎที่ยืนยัน และ downstream ใช้เอกสารฉบับที่ถูกต้องโดยไม่แก้ประวัติ. ไม่กำหนดว่า Void ต้องย้อน Won อัตโนมัติจน Business ยืนยัน.

### CP-07 — External Acceptance/Signatures

- [ ] ยืนยันผู้มีอำนาจแทนลูกค้า, identity/link lifetime/revocation, consent, ลายเซ็นและหลักฐานที่ต้องเก็บ พร้อม retention/legal review
- [ ] สร้าง public/customer projection contract ที่แยกจาก internal ERP; ป้องกัน token replay/leakage, enumeration และ rate abuse ตาม threat model
- [ ] ทำ customer journey ดูเอกสาร/ยอมรับ/ลงนาม และ internal audit โดย reuse acceptance transaction/idempotency; reject เอกสาร superseded/void ตาม CP-06
- [ ] ทดสอบ expired/revoked link, double accept, race กับ amendment/void, unauthorized signer และทุก response/artifact ไม่มีข้อมูลต้นทุน

**ผลส่งมอบ/เกณฑ์จบ:** Acceptance ผูกกับ quotation version และผู้ยืนยัน/หลักฐานที่ตรวจย้อนหลังได้; ไม่ถือภาพลายเซ็นเพียงอย่างเดียวเป็นนโยบายการลงนามที่ธุรกิจอนุมัติ.

### CP-08 — Won → Project Handover

อ้าง FR-PRJ-001 และ [Project Lifecycle](../../01-business/project-lifecycle.md).

- [x] ยืนยันจำนวน Project ต่อ Quotation, การแบ่งงาน, Project Owner/Branch, วันเริ่มและข้อมูลขั้นต่ำของ Planned Project
- [x] ออกแบบ Project baseline จาก accepted quotation/approved estimate/ready survey พร้อม revision/hash refs; ไม่ผูกงบกับ master price ที่เปลี่ยนได้
- [x] สร้าง handover command/transaction และ UI review โดย reuse numbering/audit/idempotency; การสร้าง Project ไม่เรียก Procurement/Production โดยอัตโนมัติหากไม่ได้อนุมัติ flow
- [x] ทดสอบ duplicate handover, stale version, out-of-scope, failed transaction และ baseline ที่คงเดิมเมื่อมีฉบับใหม่

**ผลส่งมอบ/เกณฑ์จบ:** งาน Won ที่เข้าเงื่อนไขส่งต่อเป็น Planned Project ได้ตาม cardinality ที่ยืนยัน พร้อม immutable source baseline และผู้รับงาน.

### CP-09 — Project Control

- [x] ยืนยัน Work Breakdown, milestone/dependency/calendar, baseline budget, commitments/actuals, approval authority และ ownership ของต้นทุนก่อนออก contract
- [x] แบ่ง slice เป็น Planned → Active พร้อมแผน/งบ, Progress, Change Order และ lifecycle/Hold/Complete ตาม Project Lifecycle ที่ยืนยัน
- [x] ทำ budget revisions และ approved change orders โดยคง baseline/history; แยกการแก้ scope หลังขายออกจาก Estimate ก่อนขาย
- [x] ทดสอบ over-budget controls ที่ยืนยัน, progress rollup, concurrent change, permission และ trace จาก Project กลับ Commercial

**ผลส่งมอบ/เกณฑ์จบ:** Project หนึ่งงานคุมแผน/งบ/ความคืบหน้าและเปลี่ยน scope ผ่าน workflow ที่อนุมัติได้; ยังไม่สร้างรายงาน actual cost จากข้อมูลที่ไม่มีเจ้าของ.

### CP-10 — Supplier/Procurement

- [x] ยืนยัน Supplier identity/terms, requisition/PO approval, sourcing/pricing, receive/return/cancel rules และ budget commitment กับ Project/Finance
- [x] แบ่ง slice Supplier → Purchase Request → PO → Receipt โดย reuse Item/Unit/numbering/approval foundations และแยก Supplier จาก Cost Source
- [x] ออก contracts/transactions สำหรับ PO quantity/value และ receipt handoff ไป Inventory โดยไม่เขียน stock tables ของอีก module โดยตรง
- [x] ทดสอบ partial/over receipt ตามกฎที่ยืนยัน, replay, concurrent receive/cancel, scope และ trace Project → PO → Receipt

**ผลส่งมอบ/เกณฑ์จบ:** จัดซื้อและรับหนึ่งรายการครบ workflow พร้อมหลักฐานและความรับผิดชอบงบ; costing ไม่เดาจากราคา TEST_ONLY หรือ Cost Source แทน Supplier.

### CP-11 — Inventory/Warehouse

- [x] ยืนยัน Warehouse/location, base unit, valuation method, negative stock, lot/serial, reservation และ stock count/adjustment rules
- [x] ออกแบบ movement ledger และ balance/reservation projection; freeze valuation/conversion evidence ตามนโยบายที่ยืนยัน
- [x] ทำ receive/issue/transfer/reserve/count-adjustment เป็น slice ตามลำดับที่เลือก โดยรับข้อมูลจาก Procurement/Project contracts
- [x] ทดสอบ concurrent reservation/issue, atomic transfer, duplicate movement, backdate/period rules และ balance reconciliation

**ผลส่งมอบ/เกณฑ์จบ:** สต็อกตรวจจาก movement ถึงเอกสารต้นทางได้; Stock Accuracy/valuation ผ่าน Finance/Warehouse UAT ก่อนเป็น input ของ MRP.

### CP-12 — BOM/Production

- [x] ยืนยัน BOM revision, unit/yield/scrap, routing/capacity, subcontracting และ production states; แยก Estimate BOQ จาก production BOM
- [x] ทำ BOM approval/revision, Work Order/plan และ material reservation/issue/return/completion contracts กับ Inventory
- [x] ทำ production UI และ snapshot ของ BOM/plan ต่อ order; รักษาประวัติเมื่อ BOM ใหม่มีผล
- [x] ทดสอบ BOM cycle/dimension, yield/scrap rules, duplicate completion, material shortage, partial completion และ actual input/output reconciliation

**ผลส่งมอบ/เกณฑ์จบ:** ผลิตหนึ่ง Work Order จาก approved BOM ผ่านเบิก/คืน/รับผลผลิตได้ พร้อม trace ต้นทุนและ stock movement.

### CP-13 — MRP

- [x] Production/Procurement ยืนยัน demand sources, planning horizon, lead time/calendar, safety stock, lot sizing และ treatment ของ open PO/reservations
- [x] ทำ deterministic planning run บน versioned input snapshot ของ BOM/demand/stock/open supply โดยไม่ใช้ live data ปะปนระหว่าง run
- [x] แสดง shortage, recommendation และเหตุผลย้อนกลับ input; เปลี่ยน recommendation เป็น request/order เฉพาะ workflow ที่อนุมัติ
- [x] ทดสอบ multi-level BOM, unit conversion, timing/lead time, repeated run และเปลี่ยน stock ระหว่าง planning

**ผลส่งมอบ/เกณฑ์จบ:** input snapshot เดิมให้ material plan เดิม และผู้วางแผนยืนยัน shortage/recommendation จากเคสจริง; ไม่เปิด MRP ก่อน CP-10/11/12 prerequisites พร้อม.

### CP-14 — Installation/Handover/Warranty/Service

- [ ] ยืนยัน installation scope/crew/checklist, defects/punch list, customer handover evidence และวันเริ่ม/เงื่อนไข warranty
- [ ] แบ่ง slice installation appointment/progress → acceptance/handover → warranty/service request โดยอ้าง Project และ output ของ Production เมื่อเกี่ยวข้อง
- [ ] ทำ attachments/readiness/approval ด้วย shared file access และ confirmation; ผูกการปิด Project กับ handover gates ที่ Business ยืนยัน
- [ ] ทดสอบ incomplete checklist, disputed acceptance, reopened defect, warranty eligibility, cross-scope/private evidence และ history

**ผลส่งมอบ/เกณฑ์จบ:** งานหนึ่งโครงการส่งมอบและเปิดบริการหลังขายจากหลักฐานจริงได้; ไม่กำหนด warranty duration หรือเงื่อนไขชดเชยแทนธุรกิจ.

### CP-15 — Finance Integration

- [ ] Finance ยืนยัน billing milestone/deposit/retention/tax/withholding, credit exposure rules, payment allocation และระบบบัญชีปลายทาง/เจ้าของ ledger
- [ ] แบ่ง slice billing reference → payment status/reconciliation → accounting connector; ไม่สร้างระบบบัญชีเต็มรูปแบบตาม Scope
- [ ] ออก immutable financial reference และ connector retry/deduplication/reconciliation contracts; ยอดจาก source document ที่อนุมัติ ไม่คำนวณจาก master ปัจจุบัน
- [ ] ทดสอบ duplicate callback/export, partial/overpayment ตามกฎ, reconciliation mismatch, dependency outage และ correction history

**ผลส่งมอบ/เกณฑ์จบ:** ข้อมูล Billing/Payment ตรวจย้อนกลับเอกสารขาย/จัดซื้อและเทียบกับระบบบัญชีได้; Customer credit terms ที่มีแล้วไม่เท่ากับ credit exposure control.

### CP-16 — Quick Estimate

ใช้ [Documentation Plan](2026-09-06-quick-estimate-documentation-plan.md), [Mobile Contracts Plan](2026-09-06-quick-estimate-mobile-contracts.md), [Pricing Rules](../../01-business/quick-estimate-pricing-rules.md), [Template Governance](../../01-business/pricing-template-governance.md), [API](../../03-contracts/quick-estimate-api-contract.md) และ [Data](../../04-data/quick-estimate-data-contract.md) ก่อนเปิดแผน runtime implementation แยก.

- [ ] ยืนยันประเภทงาน/ข้อมูลขั้นต่ำ/Template/Reference Rate/Share Policy และผู้รับรอง; published versions ต้อง immutable
- [ ] ทำ mobile capture/calculation snapshot/price range และ preliminary customer-safe summary; แยกจาก Official Estimate/Quotation
- [ ] กำหนด autosave/recovery/concurrency และ convert version → Official Estimate Draft แบบ idempotent; ไม่อ้างว่ารองรับ offline-first หลายอุปกรณ์
- [ ] ทดสอบ deterministic range/outward rounding, stale template/rate, blocked sharing, conflict recovery และ convert replay

**ผลส่งมอบ/เกณฑ์จบ:** ผู้ประเมินให้ช่วงราคาเบื้องต้นที่ตรวจจาก template/version ได้และส่งต่อ Draft ได้; ไม่ออก Quotation ที่อนุมัติจาก Quick Estimate โดยข้าม Official Estimate.

## 5. หลักฐานการปรับเอกสารรอบนี้

รอบนี้ต้องตรวจลิงก์ของเอกสารที่แก้, CP-01–16/dependency coverage, JSON syntax/schema เดิม และ diff ของไฟล์ที่แตะ. Build/test ตาม AGENTS.md ให้บันทึกผลจริงและข้อจำกัดแยกจากสถานะ feature. การแก้ Markdown ไม่เปลี่ยน Portal UI/Flow จึงไม่ใช่หลักฐานใหม่ของ responsive/keyboard/print behavior.

เอกสารที่ปรับ: AGENTS.md, README.md, docs/README.md, Scope, Implementation Roadmap และแผนรวมฉบับนี้. Checklist CP ยังคงเปิดจนลงมือและตรวจรับงานนั้นจริง.

### ผลตรวจเอกสารและ regression gates (2026-09-29)

| คำสั่ง/การตรวจ | ผลจริง |
| --- | --- |
| ตรวจ local links/heading anchors ของ Markdown ทั้งหกไฟล์ด้วย Python | ผ่าน 221 ลิงก์; ไม่พบ path/heading ที่หาย |
| ตรวจ CP-01–16 และทุกหัวข้อมีงานย่อย/เกณฑ์จบ | ผ่านครบ 16 รายการ |
| ตรวจ whitespace ของหกไฟล์และ git diff --check ของไฟล์ tracked ที่แก้ | ผ่าน |
| AJV draft 2020-12: Portal data/schema, JSON parse และ locale key parity | ผ่าน Portal 7 ไฟล์, OpenAPI JSON และ th/en 1,633 key paths |
| npm run test:fixtures | ผ่าน 1/1 |
| dotnet build backend/TanErp.slnx --no-restore -m:1 /nodeReuse:false | ผ่าน; 0 warnings/errors |
| dotnet test backend/TanErp.slnx --no-build --no-restore -m:1 /nodeReuse:false -- xUnit.ParallelizeTestCollections=false | Exit 1. Sandbox ครั้งแรกเปิด VSTest local socket ไม่ได้; rerun เมื่ออนุญาต socket แล้ว Architecture 3/3 และ Unit 284/284 ผ่าน. Integration 7/285 ผ่าน อีก 278 รายการเริ่ม fixture ไม่ได้ด้วย DockerUnavailableException ที่ Docker endpoints ทั้งสอง ไม่ใช่หลักฐานผ่าน full integration |
| cd frontend && npm run lint | ผ่าน; exit 0 |
| cd frontend && npm run build | Exit 1; Turbopack CSS helper เปิด local port ใน sandbox ไม่ได้ |
| cd frontend && npm run build -- --webpack | ผ่าน; exit 0 รวม TypeScript/static generation; มี warning เดิมเรื่อง multiple lockfiles |

ผลนี้ตรวจการแก้เอกสารและ regression commands ของ working tree ณ เวลารัน ไม่ใช่ business UAT/Production sign-off และไม่ปิด CP-01/05. รอบนี้ไม่มีการแก้ application code, contracts, database หรือ Portal UI/Flow. Full IntegrationTests ต้องรันใหม่ใน environment ที่ Docker พร้อม.

### Full backend gate rerun (2026-09-30)

- Docker Desktop was running; the sandboxed attempt could not access its socket, so the authorized test command ran with Docker socket access.
- `dotnet build backend/TanErp.slnx --no-restore -m:1`: passed, 0 warnings/errors.
- `dotnet test backend/TanErp.slnx --no-restore -m:1`: passed IntegrationTests 289/289 (8m37s), UnitTests 287/287, and ArchitectureTests 3/3.
- Official Estimate browser journey passed Chromium 1/1 in 57 seconds against the Test API, Firebase Auth Emulator, and disposable PostgreSQL after verifying Playwright's base URL and API target. Authorized UAT and release sign-off remain open.
- This supersedes the 2026-09-29 environment-blocked test result above. It closes the current backend code-test gate, not UAT, release policy approval, sanitized migration rehearsal, pilot data, or CP-01/CP-05 overall.
