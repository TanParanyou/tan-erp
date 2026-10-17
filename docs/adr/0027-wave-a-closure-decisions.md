---
status: accepted
---

# Wave A Closure: 1 User = 1 Organization, Audit read/redaction, และ go-live bootstrap

> **ที่มาของการตัดสินใจ:** ตัดสินเมื่อ 2026-10-17 โดยเจ้าของระบบ (owner delegation: "เลือกตัวเลือกยอดนิยมและทำให้ปรับได้ภายหลัง") **ไม่ใช่การยืนยันจาก Security/Legal** — Retention และนโยบาย PII ต้องให้ Security/Legal ยืนยันอีกครั้งก่อน Production.
> **สถานะ implementation:** Implemented (no tests written, none run) — ไม่ได้ตรวจในเบราว์เซอร์; ยังไม่ผ่าน UAT. ที่มาของตัวเลือก: [G-21 Open Decisions](../superpowers/plans/2026-10-12-g21-open-decisions.md).

## D-A — User หนึ่งคนอยู่ได้ Organization เดียว (ตัวเลือก A1)

กฎ: User หนึ่งคนมี Membership ได้เฉพาะ Organization เดียว (หลายสาขาได้). ทุกเส้นทางที่สร้าง/ผูก Membership ให้ User ที่มีอยู่แล้วต้องถาม `UserOrganizationPolicy.MayHoldMembershipIn` (`backend/src/TanErp.Domain/IdentityAccess/UserOrganizationPolicy.cs`) และถ้าไม่ผ่านต้องคืน `409 USER_EMAIL_ALREADY_EXISTS` เหมือนกรณีอีเมลซ้ำปกติ (ไม่บอกว่าอยู่ Organization ใด). ตรวจแล้ว: เส้นทางสร้าง Membership ใน production มีเส้นเดียวคือ `IdentityAdministrationStore.CreateUserAsync` (และ `TestOnlyDataSeeder`); `FirebaseIdentityLinker` และ `UpdateMembership` ไม่สร้าง Membership ใหม่.

**ปรับภายหลัง (ไป A2 หลาย Organization):** แก้เมธอดเดียวใน `UserOrganizationPolicy` (ให้คืน true) แล้วทำงานตามหัวข้อ "A2" ของ Open Decisions: แยก `User.IsActive` (ระบบ) ออกจากสถานะต่อ Membership, flow เชิญผู้ใช้เดิม + consent, org switcher, ซ่อนอีเมลข้ามองค์กร, test ข้ามองค์กร. ไม่มี migration สำหรับการเปลี่ยนกฎนี้.

## D-B — Audit: อ่านได้ (B1) + ปิดบัง PII ที่จุดเดียว (B3); Retention เลื่อน (B2)

- **B1:** `GET /api/v1/audit-events` (`audit.read`, ขอบเขต Organization, ใหม่ → เก่า) คืน `actor { id, name }` ประกอบจาก Backend. สัญญา: [Audit API Contract](../03-contracts/audit-api-contract.md). FE มีหน้าอ่านอย่างเดียวที่ `/settings/audit`.
- **B3:** `AuditRedactor` (`backend/src/TanErp.Domain/Common/AuditRedactor.cs`) ถูกเรียกใน constructor ของ `AuditEvent` — จุดเดียวที่ทุกการเขียน Audit ผ่าน; ค่าของ key ใน deny-list (ตัวพิมพ์เล็ก/ใหญ่ไม่สำคัญ, ซ้อนลึกได้) กลายเป็น `"[redacted]"`. ค่าเริ่มต้น: `email, phone, mobile, address, taxId, nationalId, displayName, firstName, lastName, fullName`; ปรับได้ที่ `Audit:RedactedKeys` ใน appsettings. ตอนอ่านก็ผ่าน redactor อีกรอบเพื่อครอบแถวเก่าที่เขียนก่อนมี redactor (แถวในฐานข้อมูลไม่ถูกแก้). รายละเอียด: [Audit Redaction](../04-data/audit-redaction.md).
- **B2 (เลื่อน):** ไม่มี purge/worker. เพิ่มคีย์ `Audit:RetentionDays` (ค่าเริ่มต้น `null` = เก็บตลอด) เป็น **reserved** — ยังไม่มีโค้ดอ่าน. นโยบายเริ่มต้นที่แนะนำ: เอกสารการเงิน/ภาษี 7 ปี, access log 1 ปี — **รอ Legal ยืนยัน**.

**ปรับภายหลัง:** เพิ่ม/ลบ key ที่ปิดบังด้วย config (ไม่ต้อง deploy โค้ด); ถ้าต้อง allowlist ต่อ action (B3 แบบเข้ม) ให้ขยาย `AuditRedactor` ที่เดียว; ทำ purge โดยเปิด slice ใหม่ที่ใช้ pattern `MembershipExpiryReminderWorker` และอ่าน `Audit:RetentionDays` พร้อม migration ดัชนี/partition.

## D-C — เตรียมความพร้อม go-live ของ G-03

1. **Production ไม่มี seeding ของ Permission/สิทธิ์ใด ๆ มาก่อน** (seed มีเฉพาะ Test environment). เพิ่ม `GoLiveAccessSeeder.EnsureSystemAdministratorPermissionsAsync` แบบ opt-in (`AccessBootstrap:Enabled=true`, ค่าเริ่มต้นปิด) ซึ่งสร้างแถว Permission ที่ขาด และมอบ 11 คีย์ (organizations.read/manage, branches.manage, users.read/manage, memberships.manage, roles.assign/assign-approval/manage, approval-limits.manage, audit.read) ให้ทุก Role ที่ `is_system` และ active ที่ scope Organization ของ Role นั้น; เติมเฉพาะแถวที่ขาด ไม่ลบ ไม่แก้. ไม่ตัดสินสิทธิ์ด้วยชื่อ Role. การตั้ง `is_system` ยังต้องทำโดย bootstrap/SQL (ไม่มี API).
2. **Approval limits ยัง fail-closed** แต่มี seeder แบบ opt-in `ApprovalLimits:Seed` (ว่าง = ไม่ทำอะไร): รายการ `{ documentType, roleCode, maxAmount, currency }`; จับคู่ Role ตามชื่อ (case-insensitive) ในทุก Organization; สร้างวงเงิน org-wide เฉพาะที่ยังไม่มี (active หรือไม่ก็ตาม), ไม่เขียนทับ, เขียน Audit `approval-limits.seeded` (ผู้กระทำ = สมาชิกเก่าสุดของ Organization). รายการที่ไม่ถูกต้อง/ไม่พบ Role ถูกข้ามและ log. ตารางตัวอย่างอยู่ใน runbook และ **เป็นค่าตัวอย่างที่ Finance ต้องยืนยัน**.
3. Runbook ลำดับขั้นตอน: [Wave A Go-Live Runbook](../05-engineering/wave-a-go-live-runbook.md).

**ปรับภายหลัง:** ปิด seeder ด้วยการตั้ง `AccessBootstrap:Enabled=false` และลบรายการใน `ApprovalLimits:Seed`; วงเงินที่สร้างแล้วแก้ผ่านหน้า `/settings/approval-limits` ตามปกติ.

## ผลกระทบ

- Audit `changes` ไม่มีข้อมูลส่วนบุคคลตาม deny-list อีก; หากบางเหตุการณ์ต้องการค่าเหล่านั้นจริงต้องปรับ config ไม่ใช่เขียนไว้ใน payload.
- `audit.read` ถูกเพิ่มใน Test seed และ FE `PERMISSIONS`.
- ไม่มี migration; มี OpenAPI เพิ่ม 1 endpoint.
