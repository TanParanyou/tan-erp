# tan-erp Documentation

เอกสารชุดนี้เป็นแหล่งอ้างอิงหลักก่อนเริ่มพัฒนาระบบจริง เนื้อหาจัดตามเป้าหมายของผู้อ่าน: ทำความเข้าใจธุรกิจ, อ้างอิงสัญญาระบบ, ทำงานตามมาตรฐาน และดูแลระบบ Production

## ทางลัดตามผู้อ่าน

- ผู้บริหาร/เจ้าของงาน: [Product Vision](00-overview/product-vision.md) → [Scope](00-overview/scope-and-non-goals.md) → [Roadmap](00-overview/implementation-roadmap.md)
- ฝ่ายประเมินราคา: [End-to-End Flow](01-business/end-to-end-business-flow.md) → [Quick Estimate Flow](01-business/quick-estimate-flow.md) → [Estimation Flow](01-business/estimation-flow.md) → [Approval Matrix](01-business/approval-matrix.md)
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
| กระบวนการประเมินราคา | [Estimation Flow](01-business/estimation-flow.md) |
| สูตรประเมินราคา | [Estimation Calculation](01-business/estimation-calculation-rules.md) |
| คำศัพท์ธุรกิจ | [CONTEXT.md](../CONTEXT.md) |
| ขอบเขต Module | [Module Boundaries](02-architecture/module-boundaries.md) |
| Backend | [Backend Architecture](02-architecture/backend-architecture.md) |
| Frontend | [Frontend Architecture](02-architecture/frontend-architecture.md) |
| API | [API Conventions](03-contracts/api-conventions.md) |
| Error หลายภาษา | [Error Contract](03-contracts/error-contract.md) |
| RBAC | [RBAC](03-contracts/rbac.md) |
| รายการ Permission | [Permission Catalog](03-contracts/permission-catalog.md) |
| Database | [Database Standards](04-data/database-standards.md) |
| Conceptual Data Model | [Conceptual Data Model](04-data/conceptual-data-model.md) |
| Raw SQL | [Raw SQL Policy](04-data/raw-sql-policy.md) |
| Testing | [Testing Strategy](05-engineering/testing-strategy.md) |
| เกณฑ์ส่งมอบ | [Definition of Done](05-engineering/definition-of-done.md) |
| Production Operations | [Operations](06-operations/observability.md) |
| ความพร้อมก่อน Release | [Release Readiness](06-operations/release-readiness.md) |

## สถานะเอกสาร

- `Accepted` หมายถึงแนวทางที่อนุมัติและใช้เป็นมาตรฐาน
- `Draft` หมายถึงต้องยืนยันกับผู้ใช้งานธุรกิจก่อนนำไปสร้างระบบ
- `Future` หมายถึงเตรียมทางขยายไว้ แต่ยังไม่อยู่ใน Release ปัจจุบัน

ข้อมูลธุรกิจที่ยังไม่มีจากลูกค้าจะเขียนเป็น **สมมติฐานที่ต้องยืนยัน (Validation Question)** แทนการเดาเป็นข้อเท็จจริง
