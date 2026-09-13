# tan-erp Documentation

เอกสารชุดนี้เป็นแหล่งอ้างอิงหลักก่อนเริ่มพัฒนาระบบจริง เนื้อหาจัดตามเป้าหมายของผู้อ่าน: ทำความเข้าใจธุรกิจ, อ้างอิงสัญญาระบบ, ทำงานตามมาตรฐาน และดูแลระบบ Production

## ทางลัดตามผู้อ่าน

- ผู้บริหาร/เจ้าของงาน: [Product Vision](00-overview/product-vision.md) → [Scope](00-overview/scope-and-non-goals.md) → [Roadmap](00-overview/implementation-roadmap.md)
- ฝ่ายประเมินราคา: [Estimation Flow](01-business/estimation-flow.md) → [Field Catalog](01-business/official-estimate-field-catalog.md) → [Calculation Rules](01-business/estimation-calculation-rules.md) → [Approval Matrix](01-business/approval-matrix.md)
- ทีมพัฒนา Official Estimate: [Responsive Wireframe](01-business/official-estimate-responsive-wireframe.md) → [Field Catalog](01-business/official-estimate-field-catalog.md) → [API Contract](03-contracts/official-estimate-api-contract.md) → [Data Contract](04-data/official-estimate-data-contract.md) → [UAT](05-engineering/official-estimate-uat-scenarios.md)
- ทีมดูแล/พัฒนา Item Master: [Flow](01-business/item-master-flow.md) → [Field Catalog](01-business/item-master-field-catalog.md) → [Governance](01-business/item-master-governance.md) → [Responsive Wireframe](01-business/item-master-responsive-wireframe.md) → [API Contract](03-contracts/item-master-api-contract.md) → [Data Contract](04-data/item-master-data-contract.md) → [UAT](05-engineering/item-master-uat-scenarios.md)
- ทีมขาย/สำรวจ/พัฒนา CRM: [Flow](01-business/crm-site-survey-flow.md) → [Field Catalog](01-business/crm-site-survey-field-catalog.md) → [Governance](01-business/crm-site-survey-governance.md) → [Responsive Wireframe](01-business/crm-site-survey-responsive-wireframe.md) → [API Contract](03-contracts/crm-site-survey-api-contract.md) → [Data Contract](04-data/crm-site-survey-data-contract.md) → [UAT](05-engineering/crm-site-survey-uat-scenarios.md)
- ทีมออกแบบ/พัฒนา Quick Estimate: [Mobile Wireframe](01-business/quick-estimate-mobile-wireframe.md) → [API Contract](03-contracts/quick-estimate-api-contract.md) → [Data Contract](04-data/quick-estimate-data-contract.md)
- นักพัฒนา: [System Context](02-architecture/system-context.md) → [Module Boundaries](02-architecture/module-boundaries.md) → [Contracts](03-contracts/api-conventions.md)
- ผู้ดูแลระบบ: [Environments](06-operations/environments.md) → [Observability](06-operations/observability.md) → [Backup](06-operations/backup-and-restore.md)
- ทุกคนที่อยากเห็นภาพ: [Documentation Portal](portal/README.md)

## แหล่งอ้างอิงหลัก

| เรื่อง | เอกสารหลัก |
| --- | --- |
| เป้าหมายผลิตภัณฑ์ | [Product Vision](00-overview/product-vision.md) |
| ขอบเขต Release | [Scope and Non-goals](00-overview/scope-and-non-goals.md) |
| ลำดับการพัฒนา | [Implementation Roadmap](00-overview/implementation-roadmap.md) |
| รายการ Requirement | [Requirements Catalog](00-overview/requirements-catalog.md) |
| ราคาประเมินเบื้องต้นหน้างาน | [Quick Estimate Flow](01-business/quick-estimate-flow.md) |
| สูตร Quick Estimate และ Pricing Template | [Quick Estimate Pricing Rules](01-business/quick-estimate-pricing-rules.md) |
| Field, ตัวอย่าง และ Test Case ของ Quick Estimate | [Quick Estimate Template Catalog](01-business/quick-estimate-template-catalog.md) |
| การสร้าง อนุมัติ เปิดใช้ และเปลี่ยน Pricing Template/Rate | [Pricing Template Governance](01-business/pricing-template-governance.md) |
| Mobile Wireframe ของ Quick Estimate | [Quick Estimate Mobile Wireframe](01-business/quick-estimate-mobile-wireframe.md) |
| กระบวนการประเมินราคา | [Estimation Flow](01-business/estimation-flow.md) |
| Responsive Wireframe ของ Official Estimate | [Official Estimate Responsive Wireframe](01-business/official-estimate-responsive-wireframe.md) |
| Field และ Required Gate ของ Official Estimate | [Official Estimate Field Catalog](01-business/official-estimate-field-catalog.md) |
| สูตรประเมินราคา | [Estimation Calculation](01-business/estimation-calculation-rules.md) |
| กฎและ Route การอนุมัติ | [Approval Matrix](01-business/approval-matrix.md) |
| Flow ของ Item/Unit/Cost | [Item Master Flow](01-business/item-master-flow.md) |
| Field และ Gate ของ Item/Unit/Cost/Import | [Item Master Field Catalog](01-business/item-master-field-catalog.md) |
| Lifecycle, Cost Resolution และ Import Control | [Item Master Governance](01-business/item-master-governance.md) |
| Responsive UX ของ Item Master | [Item Master Responsive Wireframe](01-business/item-master-responsive-wireframe.md) |
| Flow ของ Customer/Opportunity/Site Survey | [CRM and Site Survey Flow](01-business/crm-site-survey-flow.md) |
| Field และ Gate ของ CRM/Survey | [CRM and Site Survey Field Catalog](01-business/crm-site-survey-field-catalog.md) |
| ระยะการเก็บข้อมูลลูกค้า (Customer Phases) | [Customer Field Expansion Phases](01-business/customer-field-expansion-phases.md) |
| Lifecycle, Privacy และ Survey Readiness | [CRM and Site Survey Governance](01-business/crm-site-survey-governance.md) |
| Responsive UX ของ CRM/Survey | [CRM and Site Survey Responsive Wireframe](01-business/crm-site-survey-responsive-wireframe.md) |
| คำศัพท์ธุรกิจ | [CONTEXT.md](../CONTEXT.md) |
| ขอบเขต Module | [Module Boundaries](02-architecture/module-boundaries.md) |
| Backend | [Backend Architecture](02-architecture/backend-architecture.md) |
| Frontend | [Frontend Architecture](02-architecture/frontend-architecture.md) |
| UI/UX Design System | [Design System (Atelier Navy Sharp)](../design.md) |
| มาตรฐานฟอร์ม ERP | [Building ERP Forms](../.agents/skills/building-erp-forms/SKILL.md) |
| คอมโพเนนต์และ Hooks กลาง | [Shared Components Guide](frontend/shared-components-guide.md) |
| คู่มือ Agent และกฎการพัฒนา | [AGENTS.md](../AGENTS.md) |
| API | [API Conventions](03-contracts/api-conventions.md) |
| API ของ Quick Estimate | [Quick Estimate API Contract](03-contracts/quick-estimate-api-contract.md) |
| API ของ Official Estimate | [Official Estimate API Contract](03-contracts/official-estimate-api-contract.md) |
| API ของ Item/Unit/Cost | [Item Master API Contract](03-contracts/item-master-api-contract.md) |
| API ของ Customer/Opportunity/Site Survey | [CRM and Site Survey API Contract](03-contracts/crm-site-survey-api-contract.md) |
| Error หลายภาษา | [Error Contract](03-contracts/error-contract.md) |
| RBAC | [RBAC](03-contracts/rbac.md) |
| รายการ Permission | [Permission Catalog](03-contracts/permission-catalog.md) |
| Database | [Database Standards](04-data/database-standards.md) |
| Conceptual Data Model | [Conceptual Data Model](04-data/conceptual-data-model.md) |
| ข้อมูลและ JSONB ของ Quick Estimate | [Quick Estimate Data Contract](04-data/quick-estimate-data-contract.md) |
| ข้อมูล BOQ/Revision ของ Official Estimate | [Official Estimate Data Contract](04-data/official-estimate-data-contract.md) |
| ข้อมูล Item/Unit/Versioned Cost | [Item Master Data Contract](04-data/item-master-data-contract.md) |
| ข้อมูล Customer/Opportunity/Survey Revision | [CRM and Site Survey Data Contract](04-data/crm-site-survey-data-contract.md) |
| Raw SQL | [Raw SQL Policy](04-data/raw-sql-policy.md) |
| Testing | [Testing Strategy](05-engineering/testing-strategy.md) |
| สถานการณ์ UAT ของ Official Estimate | [Official Estimate UAT](05-engineering/official-estimate-uat-scenarios.md) |
| สถานการณ์ UAT ของ Item Master | [Item Master UAT](05-engineering/item-master-uat-scenarios.md) |
| สถานการณ์ UAT ของ CRM/Site Survey | [CRM and Site Survey UAT](05-engineering/crm-site-survey-uat-scenarios.md) |
| เกณฑ์พร้อมเริ่ม Application Development | [Development Ready Gate](05-engineering/development-ready-gate.md) |
| เกณฑ์ส่งมอบ | [Definition of Done](05-engineering/definition-of-done.md) |
| Production Operations | [Operations](06-operations/observability.md) |
| ความพร้อมก่อน Release | [Release Readiness](06-operations/release-readiness.md) |
| เหตุผลเลือก Versioned Policy/Snapshot | [ADR 0007](adr/0007-versioned-official-estimate-policies.md) |
| เหตุผลเลือก Typed Item/Versioned Cost | [ADR 0008](adr/0008-typed-items-versioned-cost-records.md) |
| เหตุผลเลือก Immutable Ready Survey Revision | [ADR 0009](adr/0009-versioned-site-survey-revisions.md) |
| เหตุผลเลือก Foundation Application Runtime | [ADR 0010](adr/0010-foundation-application-runtime.md) |
| แผนแก้ไข Foundation Login จาก Code Review | [Foundation Login Remediation Plan](superpowers/plans/2026-09-06-foundation-login-current-user-remediation.md) |
| คู่มือปฏิบัติการ Foundation Login | [Foundation Login Runbook](05-engineering/foundation-login-runbook.md) |
| บันทึกผลการตรวจสอบ Foundation Login | [Foundation Login Verification](05-engineering/foundation-login-verification.md) |
| แผนพัฒนา Customer + Contact Vertical Slice | [Customer + Contact Vertical Slice Plan](superpowers/plans/2026-09-07-customer-contact-vertical-slice.md) |
| แผนแก้ไข Customer + Contact จาก Code Review | [Customer + Contact Remediation Plan](superpowers/plans/2026-09-08-customer-contact-remediation.md) |
| บันทึกผลการตรวจสอบ Customer + Contact Slice | [Customer + Contact Verification](05-engineering/customer-contact-verification.md) |
| แผนพัฒนา Customer Activation + Opportunity + Site Slice | [Opportunity + Site Vertical Slice Plan](superpowers/plans/2026-09-08-opportunity-site-vertical-slice.md) |
| แผนแก้ไข Opportunity + Site จาก Code Review | [Opportunity + Site Remediation Plan](superpowers/plans/2026-09-09-opportunity-site-remediation.md) |
| บันทึกผลการตรวจสอบ Opportunity + Site Slice | [Opportunity + Site Verification](05-engineering/opportunity-site-verification.md) |
| แผนพัฒนา Opportunity Qualification Vertical Slice | [Opportunity Qualification Vertical Slice Plan](superpowers/plans/2026-09-10-opportunity-qualification-vertical-slice.md) |
| บันทึกผลการตรวจสอบ Opportunity Qualification Slice | [Opportunity Qualification Verification](05-engineering/opportunity-qualification-verification.md) |
| แผนรวมการพัฒนา Opportunity Module | [Opportunity Module Completion Master Plan](superpowers/plans/2026-09-12-opportunity-module-completion-master-plan.md) |
| แผนแนบภาพงานจริงใน Opportunity ตาม Stage | [Opportunity Work Images Vertical Slice Plan](superpowers/plans/2026-09-13-opportunity-work-images-vertical-slice.md) |
| บันทึกผลการตรวจสอบ Opportunity Draft Q-Gate Slice | [Opportunity Draft Q-Gate Verification](05-engineering/opportunity-draft-q-gate-verification.md) |
| บันทึกผลการตรวจสอบ Opportunity Outcome Slice | [Opportunity Outcome Verification](05-engineering/opportunity-outcome-verification.md) |
| บันทึกผลการตรวจสอบ Opportunity Hardening & Readiness | [Opportunity Hardening Verification](05-engineering/opportunity-hardening-verification.md) |
| คำสั่งและ Workflow การพัฒนา (Makefile) | [Quickstart with Make](../README.md#การเริ่มต้นระบบอย่างรวดเร็วด้วย-make-quickstart-with-make) |

## สถานะเอกสาร

- `Accepted` หมายถึงแนวทางที่อนุมัติและใช้เป็นมาตรฐาน
- `Draft` หมายถึงต้องยืนยันกับผู้ใช้งานธุรกิจก่อนนำไปสร้างระบบ
- `Future` หมายถึงเตรียมทางขยายไว้ แต่ยังไม่อยู่ใน Release ปัจจุบัน

ข้อมูลธุรกิจที่ยังไม่มีจากลูกค้าจะเขียนเป็น **สมมติฐานที่ต้องยืนยัน (Validation Question)** แทนการเดาเป็นข้อเท็จจริง
