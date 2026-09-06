---
status: accepted
---

# Site Survey ใช้ Immutable Ready Revisions

Site Survey แยก Identity ออกจาก Revision โดย Draft แก้ได้ด้วย ETag แต่ Ready Revision ล็อก Measurement, Checklist, Evidence Manifest และ Snapshot Hash โดย Lifecycle Metadata เปลี่ยนได้ผ่าน Use Case; เมื่อวัดใหม่ต้อง Clone เป็น Revision ใหม่และ Official Estimate อ้าง Revision ID ที่แน่นอน วิธีนี้รักษาหลักฐานและทำให้ประมาณการย้อนหลังไม่เปลี่ยน แลกกับการจัดการ Revision/Superseded State และพื้นที่เก็บข้อมูลเพิ่มขึ้น
