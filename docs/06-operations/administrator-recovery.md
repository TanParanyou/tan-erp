# Administrator Recovery Runbook (กู้สิทธิ์ผู้ดูแลที่หายไป)

**สถานะ:** Rehearsed บนสำเนา local (2026-10-03) — ต้องซ้อมบน staging และให้เจ้าของระบบ/Security ยืนยันก่อนใช้ใน Production. ใช้เมื่อ Organization ไม่เหลือผู้ดูแลที่ active (เช่น ผู้ดูแลคนเดียวลาออกก่อนระบบกันไว้ หรือข้อมูลถูกแก้นอกระบบ) ซึ่งหน้า/API จัดการผู้ใช้แก้เองไม่ได้ เพราะทุกการเปลี่ยนต้องมีผู้ถือ `users.manage` อยู่

ปกติระบบกันไม่ให้เกิดเหตุนี้: การถอน Role, ปิด Membership หรือปิดผู้ใช้ที่จะทำให้ไม่เหลือผู้ดูแล active (ผู้ใช้ที่ยังไม่เคยล็อกอินไม่นับ) ถูกปฏิเสธด้วย `LAST_ADMINISTRATOR_REQUIRED` — ดู [Identity Administration API Contract](../03-contracts/identity-administration-api-contract.md).

## ใครทำได้

ผู้มีอำนาจเข้าถึง PostgreSQL ของสภาพแวดล้อมนั้น (Operations/DBA) และได้รับอนุมัติเป็นลายลักษณ์อักษรจากเจ้าของธุรกิจของ Organization. ต้องมีผู้ตรวจคนที่สองเห็นคำสั่งก่อนรัน. ห้ามทำโดยลำพังหรือโดยไม่มีบันทึกเหตุผล.

## ก่อนเริ่ม

1. บันทึกเหตุผล ผู้ขอ ผู้อนุมัติ เวลา และ Organization ที่เกี่ยวข้อง (Incident/Change record)
2. Backup ฐานข้อมูลหรือยืนยันว่ามี Backup ล่าสุดที่กู้ได้ ([Backup/Restore](backup-and-restore.md))
3. ระบุ Membership เป้าหมายที่เป็นผู้ใช้จริง **ที่เคยล็อกอินแล้ว** (`firebase_uid` ไม่ว่าง) และ Role ที่ถือ `users.manage` ระดับ organization ของ Organization นั้น (ดู id จาก `identity_access.roles`)

## ตรวจสถานะ (อ่านอย่างเดียว)

ใช้ [administrator-status.sql](../../scripts/ops/administrator-status.sql) ซึ่งคืนผู้ดูแลที่ active ตามกฎของระบบ (ผู้ใช้ที่ยังไม่เคยล็อกอินไม่นับ). ถ้าได้ 0 แถวคือเข้าเงื่อนไขกู้สิทธิ์

```bash
psql "$DATABASE_URL" -v organization_id=<organization-uuid> -f scripts/ops/administrator-status.sql
```

## ขั้นตอนกู้

ใช้ [administrator-recover.sql](../../scripts/ops/administrator-recover.sql) ซึ่งเปิด transaction แล้ว **ไม่ COMMIT เอง** ให้ตรวจผลก่อน:

1. รันสคริปต์กู้ในเซสชัน `psql` เดียวกับขั้นตรวจสถานะ โดยส่งตัวแปรทั้งหมด (ดูตัวอย่าง) สคริปต์จะ
   - หยุดทันทีด้วย error ถ้าเป้าหมายไม่ใช่สมาชิก active ที่เคยล็อกอินแล้วขององค์กรนี้ (ผู้ใช้ pending, ไม่พบ, หรือองค์กรอื่น) โดยไม่มีอะไรถูกบันทึก
   - เปิด Membership เป้าหมาย ล้างช่วงเวลาที่หมดอายุ/ยังไม่เริ่ม และเปลี่ยน `row_version`
   - มอบ Role ผู้ดูแลหากยังไม่มี (รันซ้ำได้ ไม่ซ้ำแถว)
   - เพิ่ม audit event `users.recovery-grant` พร้อมเหตุผล
2. รัน `administrator-status.sql` ต่อในเซสชันเดิม ต้องเห็นเป้าหมายอย่างน้อย 1 แถว
3. พิมพ์ `COMMIT;` เมื่อถูกต้อง หรือ `ROLLBACK;` หากไม่ใช่

```bash
psql "$DATABASE_URL" \
  -v organization_id=<organization-uuid> -v target_membership_id=<membership-uuid> \
  -v admin_role_id=<role-with-users.manage-uuid> -v actor_user_id=<existing-users.id> \
  -v "reason=<incident-id> approved by <approver>" \
  -f scripts/ops/administrator-recover.sql
```

`actor_user_id` ต้องเป็น `users.id` ที่มีอยู่จริง (ผู้ปฏิบัติการ หรือผู้ถูกกู้เมื่อไม่มีบัญชีของผู้ปฏิบัติการ).

## หลักฐานการซ้อม

ซ้อมเมื่อ 2026-10-03 บนสำเนาฐานข้อมูล local (`CREATE DATABASE ... TEMPLATE`) ที่ migrate ถึง `AddIdentityAdministration`: ปิดผู้ดูแลคนเดียว (status 0 แถว) → สคริปต์ปฏิเสธเป้าหมายที่ไม่มีอยู่และผู้ใช้ pending (ไม่มี audit/role ถูกบันทึก) → กู้ให้ผู้ใช้ที่ล็อกอินแล้วสำเร็จ (status 1 แถว, audit 1 แถว) → รันซ้ำไม่เพิ่มแถว role → รัน backend กับสำเนานั้น ผู้ดูแลเดิม `GET /api/v1/me` = 403 และผู้ถูกกู้ `GET /api/v1/admin/users` = 200. **ยังไม่ได้ซ้อมบน staging ที่กู้จาก Backup จริง** และชื่อตารางอาจเปลี่ยนตาม migration ถัดไป จึงต้องซ้อมซ้ำก่อนใช้ใน production.

## หลังกู้

1. ให้ผู้ใช้ที่ได้สิทธิ์ล็อกอินและตรวจว่าเปิดหน้าจัดการผู้ใช้ได้ (สิทธิ์อ่านสดจากฐานข้อมูล ไม่ต้องรอ cache)
2. ให้ผู้ดูแลเพิ่มผู้ดูแลสำรองอย่างน้อยหนึ่งคนผ่านหน้าจัดการผู้ใช้ (ใช้ Role ที่ผู้ตรวจอิสระอนุมัติได้)
3. ปิด Incident พร้อมแนบ Audit event และผลตรวจ

## ข้อห้าม

- ห้ามแก้ `firebase_uid` ของผู้ใช้ที่ผูกแล้ว (ระบบไม่ผูกซ้ำโดยออกแบบ เพื่อกัน account takeover)
- ห้ามสร้างผู้ใช้ผูก UID ที่ยังไม่ผ่านการยืนยันตัวตนจริง
- ห้ามข้าม Audit
