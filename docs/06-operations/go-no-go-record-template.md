# Go/No-go Record Template (แบบบันทึกการตัดสินใจปล่อยระบบ)

**สถานะ:** Template — สร้างสำเนาหนึ่งฉบับต่อ release candidate (เช่น `docs/06-operations/records/go-no-go-<version>.md`) ตาม [Release Readiness](release-readiness.md#gono-go-record)

กติกา: กรอกข้อมูลและลิงก์หลักฐานจริงของ **candidate เดียวกัน** (commit SHA, schema และ artifact เดียวกัน) ก่อนให้ผู้มีอำนาจตัดสินใจ. ช่องที่ยังไม่มีผลให้ใส่ `ค้าง` พร้อมเจ้าของงานและวันที่คาดว่าจะเสร็จ; ห้ามเติมผลที่ไม่ได้รันจริง. ผลรวมเป็น `Pending decision` จนกว่าทุกช่องมีหลักฐานหรือ deferral ที่ผู้มีอำนาจอนุมัติ. ผล `Go` เป็นการตัดสินใจของเจ้าของงาน/ผู้มีอำนาจ ไม่ใช่ของผู้พัฒนาหรือเครื่องมืออัตโนมัติ.

สถานะที่ใช้: `Pass` (มีหลักฐานของ candidate นี้), `ค้าง` (ยังไม่มี), `Deferred` (เลื่อนโดยมีผู้อนุมัติและผลกระทบ), `N/A` (ไม่อยู่ใน scope ของ release นี้ พร้อมเหตุผล)

## 1. Release candidate

| ข้อมูล | ค่า |
| --- | --- |
| วันที่/เวลา | |
| Environment | |
| Version / Change log | |
| Commit SHA (backend / frontend) | |
| Schema / migration version | |
| Immutable artifact ID (build ครั้งเดียว, promote ต่อ) | |
| Scope ที่รวม (CP/Slice) | |
| Scope ที่เลื่อน (พร้อมเหตุผล) | |
| ผู้ประสานงาน release | |

## 2. Product / UAT

| รายการ | หลักฐาน (ลิงก์) | สถานะ | เจ้าของ |
| --- | --- | --- | --- |
| UAT ด้วย role จริง (Estimator, Approver, Sales, Administrator ฯลฯ) และข้อมูลที่ลบข้อมูลอ่อนไหวแล้ว | | ค้าง | |
| Pilot data และ policy ที่ Business/Finance อนุมัติ (ราคา, ภาษี, authority) — ห้ามใช้ข้อมูล `TEST_ONLY` | | ค้าง | |
| Customer-safe Quotation Document: allowlist/เงื่อนไข/branding ได้รับการอนุมัติ, ตรวจไฟล์ที่พิมพ์/PDF จริง (UAT-EST-013) | | ค้าง | Sales + Finance |
| Verification ของ critical slice | [Commercial](../05-engineering/commercial-quotation-verification.md), [Estimate](../05-engineering/official-estimate-verification.md), [Identity Admin](../05-engineering/identity-administration-verification.md), [Customer](../05-engineering/customer-completion-verification.md), [Site Survey](../05-engineering/site-survey-verification.md), [Item Master](../05-engineering/item-master-estimate-catalog-verification.md) | ตรวจว่าตรง candidate | |
| Known limitations และ support owner แจ้งผู้ใช้แล้ว | | ค้าง | |

## 3. Engineering / Security

| รายการ | หลักฐาน (ลิงก์/ผลดิบ) | สถานะ | เจ้าของ |
| --- | --- | --- | --- |
| `dotnet build` และ `dotnet test` (Architecture / Integration / Unit) | | ค้าง | |
| Frontend `check:api`, `lint`, `tsc`, `test`, `build` และ Playwright journeys | | ค้าง | |
| Security scan / dependency audit | | ค้าง | Security |
| RBAC / organization scope / maker–checker ทดสอบหลาย role และหลายองค์กร | | ค้าง | Security |
| Security review การผูกบัญชีด้วยอีเมลที่ยืนยันแล้ว ([contract](../03-contracts/identity-administration-api-contract.md)) | | ค้าง | Security |
| Secret, ข้อมูลส่วนบุคคล และ file access review | | ค้าง | Security |
| Accessibility: 200% zoom, keyboard ครบ, screen reader, 320px, print layout | | ค้าง | QA |
| Defect ที่ยังเปิด (ระดับ, ผลกระทบ, ผู้รับผิดชอบ) | | ค้าง | |

## 4. Data / Operations

| รายการ | หลักฐาน (ลิงก์/ผลดิบ) | สถานะ | เจ้าของ |
| --- | --- | --- | --- |
| Migration rehearsal บนสำเนา sanitized legacy data (หรือเหตุผล/ผู้รับรองการเริ่มด้วยฐานใหม่) | | ค้าง | Data owner |
| Backup สำเร็จ และ Restore drill ผ่านบน staging (รวม file bytes, identity/config/secrets) | [Backup/Restore](backup-and-restore.md) | ค้าง | Operations |
| RPO / RTO ที่ Business อนุมัติ และ backup schedule ตามนั้น | | ค้าง | Business + Operations |
| Role/Permission ของ Production และผู้ดูแลคนแรก (bootstrap) รวม Permission ใหม่ `users.*`, `memberships.manage`, `roles.assign*`, `quotations.read` | | ค้าง | เจ้าของระบบ |
| Administrator recovery ซ้อมบน staging ([runbook](administrator-recovery.md)) | | ค้าง | Operations |
| Health (liveness/readiness), logs, metrics, traces, alerts ตาม [Observability](observability.md) | | ค้าง | Operations |
| Hosting / domain / HTTPS / secrets / capacity / rate limit / dependency failure | | ค้าง | Operations |
| Staging rehearsal ตาม [Staging Rehearsal Checklist](staging-rehearsal-checklist.md) | | ค้าง | Operations |
| On-call, contact และ incident channel ([Incident Response](incident-response.md)) | | ค้าง | Operations |

## 5. Risk / Decision

| ข้อมูล | ค่า |
| --- | --- |
| Remaining risk และ deferral (ผลกระทบ + ผู้รับผิดชอบ) | |
| เงื่อนไขหยุด rollout (stop conditions) | |
| Rollback หรือ forward-fix (เลือกและเหตุผล, ขั้นตอน) | |
| ผู้มีอำนาจตัดสินใจ | |
| วันที่/เวลาตัดสินใจ | |
| **ผล** | `Pending decision` / `Go` / `No-go` |
