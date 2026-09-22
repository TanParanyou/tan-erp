---
status: accepted
---

# Item ใช้ Typed Capabilities และ Versioned Cost Records

Item Master แยกตัวตน/ประเภท/Capability/หน่วยฐานออกจาก Cost Record ที่มี Source, Scope, Effective Period และ Lifecycle ของตนเอง โดยใช้ Relational Typed Fields แทน EAV/Generic JSONB และเก็บ Cost Snapshot ใน Estimate; แนวทางนี้ทำให้ค้นหาและบังคับ Constraint ได้ชัด รองรับ Branch Override/Quantity Break และเปลี่ยนราคาในอนาคตโดยไม่แก้ประวัติ แลกกับจำนวน Table/Workflow ที่มากขึ้นและต้องมี Deterministic Resolver กับ Maker–Checker ก่อน Publish การใช้ JSONB แบบมีรูปทรงคงที่สำหรับข้อความหลายภาษาเป็นข้อยกเว้นที่กำหนดใน ADR 0012 และไม่เปลี่ยน Relational Core
