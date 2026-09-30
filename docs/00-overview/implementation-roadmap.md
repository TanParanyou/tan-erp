# Implementation Roadmap (แผนพัฒนา)

**สถานะ:** Accepted เป็นลำดับการพัฒนา; สถานะ implementation และหลักฐาน verification ล่าสุด ณ 2026-09-30

เอกสารนี้เป็นแหล่งหลักสำหรับเจ้าของงานและทีมพัฒนาในการดูว่าอะไรมี implementation แล้ว อะไรยังค้าง และควรทำลำดับใด. รายละเอียดงานย่อยอยู่ในแผนของแต่ละ slice และ [ERP Completion Plan](../superpowers/plans/2026-09-29-erp-completion-master-plan.md); กฎธุรกิจและขอบเขตผลิตภัณฑ์อยู่ใน [Scope](scope-and-non-goals.md) และ [Requirements Catalog](requirements-catalog.md).

## ระยะการพัฒนา

| ระยะ | เป้าหมาย | ผลส่งมอบหลัก | Gate ก่อนผ่าน |
| --- | --- | --- | --- |
| 0 Documentation | ทำความเข้าใจตรงกัน | Glossary, Flow, Architecture, Contracts, Portal | เจ้าของงานยืนยัน Flow และคำศัพท์ |
| 1 Foundation | ระบบปลอดภัยและวางฐานถูก | Identity, Organization, RBAC, Audit, Localization และหน้าผู้ดูแล | Test สิทธิ์และข้ามองค์กร; ตรวจการถอนสิทธิ์และ Audit |
| 2 Estimation MVP | ประเมินราคาได้ครบวงจร | Customer, Survey, Item, Estimate, Revision, Approval | UAT เคสจริงและเทียบผลคำนวณ; ยืนยันนโยบายราคาและอนุมัติ |
| 3 Commercial | ส่งข้อเสนอและติดตามผล | Quotation, Customer-safe Document, Acceptance, Project Handover | เอกสารขายย้อนกลับ Approved Estimate ได้และไม่เปิดเผยต้นทุน |
| 4 Project Control | คุมแผนและงบ | Project, Budget, Change Order, Progress | Baseline และต้นทุน/สถานะตรวจย้อนหลังได้ |
| 5 Supply & Production | จัดซื้อ คลัง ผลิต | Supplier, PO, Inventory, BOM, Production, MRP | Master Data, หน่วยนับ, Stock Accuracy และแผนผลิตพร้อม |
| 6 Finance & Service | ปิดวงจรธุรกิจ | Installation, Handover, Billing Integration, Payment Status, Warranty | ตรวจรับงาน, Reconciliation และ Support Runbook ผ่าน |

ทุกระยะส่งมอบเป็น Vertical Slice ที่ผู้ใช้ทดลองได้. ระยะในตารางไม่ใช่สถานะว่าเสร็จแล้ว และไม่ใช่การอนุมัติ implementation ของทุกโมดูล.

## สถานะ Implementation ปัจจุบัน

คำว่า **มี implementation** หมายถึงพบโค้ดรองรับบน `main` ณ 2026-09-30; การแก้หลังหลักฐานที่อ้างต้องตรวจ diff และรัน gates ใหม่. ผลทดสอบด้านล่างเป็นหลักฐานที่บันทึกไว้ตามวันที่ของแต่ละเอกสาร ไม่ใช่การรันใหม่โดย Roadmap และไม่เท่ากับ Production sign-off.

ผลตรวจรอบปรับเอกสาร 2026-09-29 อยู่ใน [ERP Completion Plan — ผลตรวจ](../superpowers/plans/2026-09-29-erp-completion-master-plan.md#ผลตรวจเอกสารและ-regression-gates-2026-09-29). ผลรันล่าสุด 2026-09-30 หลังแก้ Survey hash ยืนยัน backend solution build และ full test suite ผ่าน (Integration 289/289, Unit 288/288, Architecture 3/3); ดู [Site Survey Verification — Snapshot hash follow-up](../05-engineering/site-survey-verification.md#4-snapshot-hash-follow-up-2026-09-30). ผลนี้ไม่แทน UAT หรือ Production sign-off.

| ส่วน | Capability ที่มี implementation | ข้อค้าง/ข้อจำกัด | แหล่งหลักฐาน |
| --- | --- | --- | --- |
| Foundation | Login, Current User, Membership/Permission/Scope, Organization boundary และ Audit foundation | หน้าจัดการ Organization/Branch/Membership/Role ครบวงจรยังต้องมี slice แยก | [Foundation Verification](../05-engineering/foundation-login-verification.md), [UsersController](../../backend/src/TanErp.Api/Controllers/UsersController.cs), [BranchesController](../../backend/src/TanErp.Api/Controllers/BranchesController.cs) |
| Customer/Contact/Address/Site | สร้าง/แก้โปรไฟล์, ข้อมูลภาษีและเครดิต, จัดการ Contact/Address/Primary, ปิด/เปิดใช้ Customer, แก้/ปิดใช้ Site และ Billing Snapshot ตอนออก Quotation | เครดิตเป็น master data; ยังไม่บังคับ credit exposure/ลูกหนี้. Full backend suite ผ่าน 2026-09-30; UAT จริงยังคงเป็น release gate | [Customer Completion Verification](../05-engineering/customer-completion-verification.md), [Full backend gate](../05-engineering/official-estimate-verification.md#full-backend-gate-refresh-2026-09-30) |
| Opportunity | Qualification, Controlled Stage Transition, Owner/Outcome/History และ Work Images | ตรวจรับกับบทบาทจริงร่วมกับ Customer/Survey/Estimate; งานชนะการขายยังไม่สร้าง Project | [Completion Plan](../superpowers/plans/2026-09-12-opportunity-module-completion-master-plan.md), [Hardening Verification](../05-engineering/opportunity-hardening-verification.md), [Work Images Verification](../05-engineering/opportunity-work-images-verification.md) |
| Site Survey | นัดหมาย, พื้นที่/การวัด/Notes, Mark Ready พร้อม Immutable Hash และ progression ไป Estimating; hash `v2` ของ baseline รวมค่าการวัดแล้ว | Checklist/Evidence, Published Versioned Template และ New/Void Revision ยังไม่ครบ; hash `v2` ยังไม่ครอบคลุมฟิลด์อนาคตและต้องมี version ใหม่เมื่อเพิ่มฟิลด์ | [Site Survey Verification](../05-engineering/site-survey-verification.md), [Survey API Contract](../03-contracts/crm-site-survey-api-contract.md), [SiteSurveysController](../../backend/src/TanErp.Api/Controllers/SiteSurveysController.cs) |
| Item Master/Estimate Catalog | Item/SKU/Barcode, Category/Brand/Unit/Conversion/Tax, Cost Source, Versioned Cost/Maker–Checker, รูปภาพส่วนตัวและ authoritative BOQ cost snapshot | Import Preview/Validate/Atomic Commit ยังไม่พบ endpoint ในโค้ดปัจจุบัน; pilot prices, historical migration และ tablet/200% zoom ยังต้องตรวจรับ | [Item Verification](../05-engineering/item-master-estimate-catalog-verification.md), [Maintenance Plan](../superpowers/plans/2026-09-23-estimate-master-data-maintenance.md), [ItemsController](../../backend/src/TanErp.Api/Controllers/ItemsController.cs) |
| Official Estimate | BOQ workspace, Versioned Calculation/Tax Policy, Snapshot, Discount, Submit/Return/Independent Approve, Review Queue, New Revision และ Cancel | UAT-EST-003 API/UI mapping and tests verified; manual UAT and conversion snapshot deferred. UAT-EST-006 custom work item has Item Master link snapshots or bounded custom reason fields, readiness gates and a test-profile approval trigger; Estimate API passed 40/40 and Item Catalog Estimate flow passed 1/1 on PostgreSQL. Browser journey passed 1/1 against disposable DB on 2026-09-30. Authorized UAT and production policy sign-off remain open. UAT-EST-012 cross-Organization/Branch read/update/calculate/approve matrix passes 40/40; UAT-EST-013 customer document evidence remains deferred | [Estimate Completion Plan](../superpowers/plans/2026-09-27-official-estimate-completion.md), [Estimate Verification](../05-engineering/official-estimate-verification.md), [UAT baseline](../05-engineering/official-estimate-uat-scenarios.md) |
| Commercial/Document Numbering | Issue จาก Approved Estimate, Acceptance, Proposed/Won progression, idempotency และตั้งค่าเลขที่เอกสาร | Customer Preview/PDF/Printing/Signatures, Amendment/Void, External Acceptance และ Project Handover ยังเป็นงานต่อไป | [Commercial Verification](../05-engineering/commercial-quotation-verification.md), [Estimate API Contract](../03-contracts/official-estimate-api-contract.md) |
| Project/Supply/Production/Finance/Service | มีทิศทางและ Module Boundary | ยังไม่มี implementation slice ของโมดูลเหล่านี้; งานย่อยและคำถามก่อนเริ่มอยู่ในแผนรวม | [Module Boundaries](../02-architecture/module-boundaries.md), [ERP Completion Plan](../superpowers/plans/2026-09-29-erp-completion-master-plan.md) |
| Quick Estimate | มี Business/API/Data/Template contracts และแผนเอกสาร | Optional Future Module; ยังไม่ใช่ runtime module | [Documentation Plan](../superpowers/plans/2026-09-06-quick-estimate-documentation-plan.md), [Quick Estimate API](../03-contracts/quick-estimate-api-contract.md) |

## ลำดับงานถัดไป

| ลำดับ | ผลลัพธ์ที่ต้องได้ | แผน/งาน | Dependency และเงื่อนไข |
| --- | --- | --- | --- |
| 1 | Baseline ปัจจุบันตรงกับโค้ดและ UAT | ปิดข้อค้างใน [Estimate Completion Task 7](../superpowers/plans/2026-09-27-official-estimate-completion.md#task-7--release-verification-and-business-sign-off); reconcile Survey/Item baseline ตามแผนรวม | แยก defect, requirement ที่ยังไม่ implement และงานเลื่อนอย่างชัดเจน; ห้ามเปลี่ยน Expected Result เพื่อให้เทสต์ผ่าน |
| 2 | ดูแลผู้ใช้/สิทธิ์และเตรียมข้อมูลจริงได้ | CP-02 Identity/Organization Administration และ CP-03 Master Data/Survey Completion | ยืนยันสิทธิ์ผู้ดูแลและกฎข้อมูลก่อน implementation; Import อาจเลื่อนได้หาก pilot ทำผ่านหน้าดูแลเดิมได้ |
| 3 | ลูกค้าได้รับเอกสารที่ใช้งานได้ | CP-04 Customer-safe Quotation Document | Approved Estimate/Snapshots/Billing Readiness พร้อม; Business ยืนยัน field allowlist และเงื่อนไขเอกสาร |
| 4 | ตรวจรับและตัดสินใจเปิดใช้ release ที่ระบุขอบเขตชัด | CP-05 Release/UAT/Operations | ทำควบคู่ได้ตั้งแต่ต้น; Go/No-go ต้องอ้างหลักฐานของ code/schema/artifact ชุดเดียวกัน |
| 5 | จัดการข้อเสนอหลังออก และเชื่อมงานขายไปงานจริง | CP-06 Quotation Lifecycle, CP-07 External Acceptance, CP-08 Project Handover | เปิดเป็น slice แยก; Project Handover ใช้ Acceptance ภายในที่มีอยู่ได้โดยไม่ต้องรอ Portal |
| 6 | คุมโครงการและงบ | CP-09 Project Control | Project Baseline พร้อม; ยืนยัน Change Order/Actual Cost ownership |
| 7 | จัดซื้อ → รับ → คลัง → ผลิต/MRP | CP-10 Procurement, CP-11 Inventory, CP-12 Production, CP-13 MRP | ทำตาม dependency ของแต่ละ slice; ไม่เริ่ม MRP ก่อน BOM/Stock/Production Plan พร้อม |
| 8 | ส่งมอบ ติดตามการเงิน และบริการหลังขาย | CP-14 Installation/Service, CP-15 Finance Integration | ผูก Project/Commercial/Supply contracts; Installation ไม่ต้องรอ MRP หากกฎงานอนุญาต |
| ทางเลือก | ประเมินช่วงราคาหน้างาน | CP-16 Quick Estimate | หลัง Official Estimate Foundation พร้อม; ไม่เป็น prerequisite ของ Project/Supply |

CP หมายถึงรายการใน [ERP Completion Plan](../superpowers/plans/2026-09-29-erp-completion-master-plan.md). ลำดับนี้เป็นข้อเสนอจัดงาน ต้องกำหนดผู้รับผิดชอบและอนุมัติ scope ของ slice ใหม่ก่อน implementation; ไม่กำหนดวันส่งมอบหรือค่าธุรกิจที่ยังไม่ได้ยืนยัน.

## Gates ก่อนประกาศพร้อมใช้งาน

ใช้ [Definition of Done](../05-engineering/definition-of-done.md) สำหรับแต่ละ feature และ [Release Readiness](../06-operations/release-readiness.md) สำหรับ release. Full backend integration หลังแก้ Survey hash ผ่านแล้วตามหลักฐานด้านบน; หาก code/schema เปลี่ยนอีกต้องตรวจใหม่. หลักฐานที่ยังค้างต้องคงสถานะเปิดจนมีผลตรวจจริง ได้แก่ migration จาก sanitized legacy copy, authorized-role UAT, pilot data, Business/Finance/Security policy sign-off, accessibility และ Backup/Restore/Operations.

หากเสนอเลื่อน requirement ให้บันทึก ID, ผลกระทบ, ขอบเขต release, ผู้ตัดสินใจและหลักฐานไว้ในแผนของ slice และ Go/No-go Record. การมี fixture TEST_ONLY ไม่แทนข้อมูลหรือราคาที่ธุรกิจอนุมัติ.
