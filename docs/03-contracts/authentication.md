# Authentication (การยืนยันตัวตน)

**สถานะ:** Accepted Direction

Firebase Authentication ใช้ยืนยันว่า “ผู้ใช้คือใคร” ส่วน tan-erp Backend ผูก Firebase UID เข้ากับ User/Membership ภายใน เพื่อทราบ Organization, Branch, Role และ Scope

## Flow

1. ผู้ใช้เข้าสู่ระบบผ่าน Firebase Client SDK
2. Frontend รับ ID Token และส่งเป็น Bearer Token
3. Backend ตรวจ Signature, Issuer, Audience, Expiry และสถานะผู้ใช้
4. Backend Map `firebase_uid` ไปยัง Internal User
5. Backend สร้าง Trusted Authorization Context จาก PostgreSQL
6. Request จึงผ่าน RBAC และ Resource Scope

## กฎ

- ห้ามรับ Role หรือ Permission จาก Browser เป็นหลักฐาน
- Firebase Custom Claims ไม่เป็น Source of Truth ของ Business Permission
- ผู้ใช้ที่ยืนยันตัวตนได้แต่ไม่มี Active Membership ต้องเข้า ERP ไม่ได้
- Disable User/Membership ต้องมีผลตาม Cache Invalidation Policy ที่กำหนด
- Token และข้อมูลลับห้ามอยู่ใน Log หรือ URL
