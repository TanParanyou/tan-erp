---
status: accepted
---

# Firebase ยืนยันตัวตนและ Backend เป็นเจ้าของ RBAC

Firebase Authentication ยืนยัน Identity เท่านั้น ส่วน Role, Permission, Organization Membership, Scope และ Maker–Checker อยู่ใน PostgreSQL และตรวจโดย Backend การแยกนี้ทำให้สิทธิ์ธุรกิจเปลี่ยนและ Audit ได้โดยไม่ผูกกับ Token Claims ที่อาจเก่า แม้ต้องมีขั้นตอน Map Firebase UID ไปยัง Internal User และจัดการ Cache Invalidation อย่างระมัดระวัง
