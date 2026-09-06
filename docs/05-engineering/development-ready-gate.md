# Development Ready Gate (เกณฑ์พร้อมเริ่มพัฒนา)

**สถานะ:** Accepted — ใช้ตัดสินเริ่ม Application Implementation

Project ERP พร้อมออกจาก Documentation Foundation เพื่อเริ่มพัฒนา **เฉพาะ Vertical Slice ที่ผ่าน Gate** ไม่ต้องรอเอกสาร Procurement, Inventory, Production หรือ MRP

## Mandatory Gate

| Concern | หลักฐานที่ต้องมี | สถานะปัจจุบัน |
| --- | --- | --- |
| Product scope/terms | Scope, Module Boundary, `CONTEXT.md` | พร้อม |
| Identity/RBAC/Error | Firebase Identity Boundary, PostgreSQL RBAC, Permission/Error Contract | พร้อม |
| CRM/Survey source | Flow, Field, Governance, API, Data, UAT | พร้อมด้านเอกสาร |
| Item/Cost source | Flow, Field, Governance, API, Data, UAT | พร้อมด้านเอกสาร |
| Official Estimate | Flow, Field, Calculation, Approval, API, Data, UAT | พร้อมด้านเอกสาร |
| Architecture | Clean Architecture 4 Projects, Feature folders, EF Core writes, Dapper reads | พร้อมด้านเอกสาร |
| UX baseline | Desktop/Tablet/Mobile, states, accessibility, localization | พร้อมด้านเอกสาร |
| Engineering/Operations | Testing, migration, audit, observability, backup, release readiness | พร้อมด้านเอกสาร |

คำว่า “พร้อมด้านเอกสาร” หมายถึงเริ่ม Scaffold/Prototype/Vertical Slice ได้ ไม่ได้หมายถึง Production Sign-off หรือข้อมูลธุรกิจจริงครบ

## Before First Application Commit

- ผู้ใช้ต้องอนุมัติเปลี่ยน Repository Phase จาก Documentation Foundation เป็น Application Implementation
- เลือก Runtime/Version ที่รองรับ ณ วันเริ่มงานและบันทึกใน Architecture/ADR โดยตรวจจาก Official Documentation
- กำหนด Local/Development Configuration โดยไม่มี Secret ใน Source
- สร้าง OpenAPI-first Boundary, Database Migration Baseline และ Architecture Tests
- กำหนด Synthetic Fixture ที่ติด `TEST_ONLY`; ห้ามคัดลอกข้อมูลลูกค้าจริง
- กำหนด System-owned `SURVEY-BASELINE-v1` Fixture/Seed ที่ผ่าน Schema/Readiness Test; ห้ามถือว่าเป็น Business Template จริงจน Survey Owner ยืนยัน
- สร้าง CI Gate ขั้นต่ำ: build, static analysis, architecture, unit/integration/contract และ secret scan

## First Vertical Slice Boundary

เริ่มได้เฉพาะเส้นทาง:

```text
Firebase Login
  → PostgreSQL Membership/RBAC
  → Create Customer + Contact
  → Create Opportunity + Site
  → Create/Ready Site Survey Revision
  → Create Official Estimate Draft
```

Item/Cost และ Calculation/Approval เชื่อมต่อเป็น Slice ถัดไปหลัง Foundation นี้ทำงานครบจาก UI → API → Database → Audit ห้ามสร้างทุก Module/ทุก Table พร้อมกัน

## Explicitly Deferred

- Quick Estimate
- Customer auto-merge และ configurable CRM workflow builder
- Offline-first multi-device survey merge
- Quotation delivery/acceptance automation
- Project, Procurement, Inventory, Production และ MRP
- Generic Form/Rules Engine และ Microservices

## Ready Decision

เมื่อ Mandatory Gate และ Before First Application Commit ผ่าน ให้ Business Owner + Technical Owner เปลี่ยน Phase ใน `AGENTS.md`/authoritative roadmap ผ่านงานที่อนุมัติโดยชัดเจน จากนั้นจึงสร้าง Application Code งานเริ่ม Dev ห้ามแก้ Phase แบบแฝงในงานเอกสารทั่วไป
