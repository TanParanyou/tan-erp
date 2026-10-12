---
status: accepted
---

# Role Management: system role ด้วย flag, maker–checker เมื่อเพิ่มอำนาจอนุมัติให้ Role ที่มีผู้ถือ, ปิด Role ต้องไม่มีผู้ถือ, ขยาย Approval Permission Set

ก่อน G-03b Role ถูกสร้างได้เฉพาะจาก seed/DB และแก้ไม่ได้จากระบบ. G-03b เพิ่มการสร้าง/แก้/ปิดใช้ Role จาก Permission catalog ที่มีอยู่ โดยรักษา invariant ของ CP-02 (anti-escalation, maker–checker, last administrator).

**การตัดสินใจ 1 — System Role คือคอลัมน์ `is_system`:** Role ระบบแก้ชื่อ/คำอธิบาย/grant และปิด-เปิดไม่ได้ (`409 ROLE_SYSTEM_IMMUTABLE`) ไม่มี API ที่ตั้ง flag นี้ (seed/bootstrap เท่านั้น). ไม่ตรวจชื่อ Role เพื่อตัดสินสิทธิ์ (`backend/AGENTS.md` ข้อ 5); ชื่อขึ้นต้น `system.` ถูกสงวนตอนสร้างเพื่อไม่ให้สับสนเท่านั้น.

**การตัดสินใจ 2 — anti-escalation ตรวจเฉพาะ grant ที่เพิ่ม:** ผู้แก้ต้องถือ permission ของ grant ที่เพิ่มด้วย `AdministrationPolicy.Covers` เดิม (Scope เท่ากันหรือกว้างกว่า). การถอน grant ทำได้เสมอ. การลบ `users.manage` ออกจาก Role ที่เป็นแหล่งผู้ดูแลคนสุดท้ายถูกปฏิเสธด้วย `LAST_ADMINISTRATOR_REQUIRED` (ใช้ query ผู้ดูแลชุดเดียวกับ CP-02).

**การตัดสินใจ 3 — maker–checker ที่จุดที่ CP-02 ยังไม่คุม:** CP-02 คุมการ *มอบ* Role ที่มี approval permission. ช่องที่เหลือคือการ *เพิ่ม* approval permission ให้ Role ที่มีคนถืออยู่ ซึ่งจะข้ามประตูการมอบ. จึงกำหนดว่า การแก้ที่เพิ่ม grant ให้ Role ที่มีผู้ถือ ≥ 1 และ Role หลังแก้ `RequiresApproval` ต้องผ่าน `RoleChangeRequest` (ผู้ขอ ≠ ผู้ตัดสิน, ผู้ตัดสินถือ `roles.assign-approval` และ `Covers` grant ที่เพิ่ม, ตรวจ `row_version` ว่า Role ไม่เปลี่ยนตั้งแต่ขอ). การสร้าง Role ใหม่และการแก้ Role ที่ไม่มีผู้ถือมีผลทันทีเพราะการมอบยังผ่าน `RoleAssignmentRequest`. กฎ "เพิ่ม grant ใด ๆ" (ไม่เฉพาะ approval key) ให้ Role ที่ RequiresApproval และมีผู้ถือเป็นแบบเข้มงวดโดยตั้งใจ.

**การตัดสินใจ 4 — ปิด Role ต้องไม่มีผู้ถือ:** Role ที่ปิดใช้ถูกกรองออกจากการคำนวณสิทธิ์ทุกจุด (`RequestAccessResolver`) ดังนั้นการปิด Role ที่มีผู้ถือ = ถอนสิทธิ์เงียบ ๆ. ปิดได้เมื่อไม่มีแถว `membership_roles` ใด (รวม Membership inactive) ไม่มี `RoleAssignmentRequest` pending และไม่มี `RoleChangeRequest` pending (`ROLE_IN_USE` / `ROLE_CHANGE_REQUEST_PENDING`). ไม่ถอน Role ให้อัตโนมัติ; ผู้ดูแลถอนที่หน้าผู้ใช้ (มี audit เดิม).

**การตัดสินใจ 5 — ขยาย `ApprovalPermissionKeys` เป็น 6 ตัว:** `estimates.approve`, `cost-records.approve`, `cost-records.publish` (เดิม) + `purchase-orders.approve`, `projects.change-orders.approve`, `mrp.approve`. ผลกระทบ: การมอบ Role ที่มีคีย์ใหม่ต้อง maker–checker (พฤติกรรม CP-02 เปลี่ยน). ไม่ใส่ `boms.approve`, `pricing-templates.approve`, `quick-estimates.review` (ไม่ใช่อำนาจอนุมัติเงิน/สัญญา) — test `ApprovalPermissionCoverageTests` กวาดคีย์ `*.approve/publish/review` ใน seeder แล้วบังคับให้ทุกคีย์อยู่ใน set หรือใน "reviewed-not-approval" อย่างชัดเจน.

**ความเสี่ยงคงเหลือ (R1):** `roles.manage` และ `roles.assign-approval` เป็น meta-privilege แต่ไม่อยู่ใน set จึงเพิ่มให้ Role ที่มีผู้ถือได้โดยไม่ต้องมีผู้ตรวจ (ผู้แก้ยังต้องถือสิทธิ์นั้นเองตาม anti-escalation). เพิ่มเข้า set ได้ภายหลังด้วยการแก้ set + test ล็อก.

**ความเสี่ยงคงเหลือ (R2):** ไม่มี notification แจ้งผู้ตรวจ (ผู้ตรวจต้องเปิดคิว `settings/role-requests`).

ข้อเสียที่ยอมรับ: `RoleChangeRequest` ทำซ้ำ logic `Try*` ของ `RoleAssignmentRequest` (~20 บรรทัด) เพื่อไม่แตะ entity ของ CP-02.
