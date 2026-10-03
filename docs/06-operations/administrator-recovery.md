# Administrator Recovery Runbook (กู้สิทธิ์ผู้ดูแลที่หายไป)

**สถานะ:** Draft — ต้องให้เจ้าของระบบและ Security ยืนยันก่อนใช้ใน Production. ใช้เมื่อ Organization ไม่เหลือผู้ดูแลที่ active (เช่น ผู้ดูแลคนเดียวลาออกก่อนระบบกันไว้ หรือข้อมูลถูกแก้นอกระบบ) ซึ่งหน้า/API จัดการผู้ใช้แก้เองไม่ได้ เพราะทุกการเปลี่ยนต้องมีผู้ถือ `users.manage` อยู่

ปกติระบบกันไม่ให้เกิดเหตุนี้: การถอน Role, ปิด Membership หรือปิดผู้ใช้ที่จะทำให้ไม่เหลือผู้ดูแล active (ผู้ใช้ที่ยังไม่เคยล็อกอินไม่นับ) ถูกปฏิเสธด้วย `LAST_ADMINISTRATOR_REQUIRED` — ดู [Identity Administration API Contract](../03-contracts/identity-administration-api-contract.md).

## ใครทำได้

ผู้มีอำนาจเข้าถึง PostgreSQL ของสภาพแวดล้อมนั้น (Operations/DBA) และได้รับอนุมัติเป็นลายลักษณ์อักษรจากเจ้าของธุรกิจของ Organization. ต้องมีผู้ตรวจคนที่สองเห็นคำสั่งก่อนรัน. ห้ามทำโดยลำพังหรือโดยไม่มีบันทึกเหตุผล.

## ก่อนเริ่ม

1. บันทึกเหตุผล ผู้ขอ ผู้อนุมัติ เวลา และ Organization ที่เกี่ยวข้อง (Incident/Change record)
2. Backup ฐานข้อมูลหรือยืนยันว่ามี Backup ล่าสุดที่กู้ได้ ([Backup/Restore](backup-and-restore.md))
3. ระบุ Membership เป้าหมายที่เป็นผู้ใช้จริง **ที่เคยล็อกอินแล้ว** (`firebase_uid` ไม่ว่าง) และ Role ที่ถือ `users.manage` ระดับ organization ของ Organization นั้น

## ตรวจสถานะ (อ่านอย่างเดียว)

```sql
-- ผู้ดูแลที่ active ตามกฎของระบบ: ต้องไม่ว่างอย่างน้อย 1 แถว ถ้าว่างคือเข้าเงื่อนไขกู้สิทธิ์
SELECT m.id AS membership_id, u.display_name
FROM organization.memberships m
JOIN identity_access.users u ON u.id = m.user_id
JOIN organization.organizations o ON o.id = m.organization_id
WHERE m.organization_id = :organization_id
  AND m.is_active AND u.is_active AND o.is_active AND u.firebase_uid IS NOT NULL
  AND (m.starts_at_utc IS NULL OR m.starts_at_utc <= now())
  AND (m.expires_at_utc IS NULL OR m.expires_at_utc > now())
  AND EXISTS (
    SELECT 1
    FROM identity_access.membership_roles mr
    JOIN identity_access.roles r ON r.id = mr.role_id AND r.is_active
    JOIN identity_access.role_permissions rp ON rp.role_id = r.id AND rp.scope = 'organization' AND rp.scope_id = m.organization_id
    JOIN identity_access.permissions p ON p.id = rp.permission_id AND p.is_active AND p.key = 'users.manage'
    WHERE mr.membership_id = m.id);
```

> Query ตรวจสถานะนี้รันผ่านบนฐานข้อมูล local ที่ migrate ถึง `AddIdentityAdministration` (2026-10-03). ชื่อตารางอาจเปลี่ยนตาม migration ถัดไป จึงต้องตรวจกับ schema ปัจจุบันและซ้อมบน staging ที่กู้จาก Backup ก่อนเสมอ; คำสั่งแก้ข้อมูลในหัวข้อ "ขั้นตอนกู้" ยังไม่ได้ซ้อมรัน

## ขั้นตอนกู้

ทำใน transaction เดียว, ตรวจผลก่อน `COMMIT`:

1. เปิด transaction (`BEGIN`)
2. ถ้า Membership เป้าหมายถูกปิด ให้เปิด (`is_active = true`) และเปลี่ยน `row_version` เป็นค่าใหม่ (`gen_random_uuid()`)
3. เพิ่ม `membership_roles` ให้ Membership เป้าหมายกับ Role ที่ถือ `users.manage` ระดับ organization (ถ้ายังไม่มี) และเปลี่ยน `row_version` ของ Membership
4. เพิ่ม `audit.audit_events` หนึ่งแถว: `action = 'users.recovery-grant'`, `resource_type = 'Membership'`, `resource_id` = id ของ Membership, `reason` = เลขที่ Incident/Change และผู้อนุมัติ, `actor_user_id` = ผู้ปฏิบัติการ (หรือ user ระบบที่ตกลงกัน)
5. รัน query ตรวจสถานะด้านบนซ้ำ ต้องได้อย่างน้อย 1 แถว แล้ว `COMMIT`

## หลังกู้

1. ให้ผู้ใช้ที่ได้สิทธิ์ล็อกอินและตรวจว่าเปิดหน้าจัดการผู้ใช้ได้ (สิทธิ์อ่านสดจากฐานข้อมูล ไม่ต้องรอ cache)
2. ให้ผู้ดูแลเพิ่มผู้ดูแลสำรองอย่างน้อยหนึ่งคนผ่านหน้าจัดการผู้ใช้ (ใช้ Role ที่ผู้ตรวจอิสระอนุมัติได้)
3. ปิด Incident พร้อมแนบ Audit event และผลตรวจ

## ข้อห้าม

- ห้ามแก้ `firebase_uid` ของผู้ใช้ที่ผูกแล้ว (ระบบไม่ผูกซ้ำโดยออกแบบ เพื่อกัน account takeover)
- ห้ามสร้างผู้ใช้ผูก UID ที่ยังไม่ผ่านการยืนยันตัวตนจริง
- ห้ามข้าม Audit
