---
status: accepted
---

# Site Survey ใช้ Immutable Ready Revisions

Site Survey แยก Identity ออกจาก Revision โดย Draft แก้ได้ด้วย ETag แต่ Ready Revision ล็อก Measurement, Checklist, Evidence Manifest และ Snapshot Hash โดย Lifecycle Metadata เปลี่ยนได้ผ่าน Use Case; เมื่อวัดใหม่ต้อง Clone เป็น Revision ใหม่และ Official Estimate อ้าง Revision ID ที่แน่นอน วิธีนี้รักษาหลักฐานและทำให้ประมาณการย้อนหลังไม่เปลี่ยน แลกกับการจัดการ Revision/Superseded State และพื้นที่เก็บข้อมูลเพิ่มขึ้น

Hash ของ Ready Revision ต้องระบุรุ่นอัลกอริทึมในค่าที่เก็บเพื่อไม่ให้หลักฐานรุ่นใหม่ถูกตีความเป็นสูตรเดิม. Baseline `v2:<sha256>` ใช้ข้อมูล Draft ที่มี implementation จริงรวมค่าการวัดและหมายเหตุ; hash SHA-256 เปล่า 64 ตัวอักษรที่สร้างก่อน `v2` ยังคงอยู่กับ Revision/Estimate เดิมและไม่คำนวณทับ. เมื่อต่อ Checklist, Evidence หรือข้อมูลอื่นตาม Snapshot Contract ต้องออกรุ่น hash ใหม่และกำหนดวิธีอ่านรุ่นเก่าใน release/migration plan.
