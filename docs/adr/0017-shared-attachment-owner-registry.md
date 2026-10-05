---
status: accepted
---

# ไฟล์แนบและลายเซ็นกลางด้วย Polymorphic Link + Owner Registry ในโค้ด

Service (G-18), Quick Estimate (G-09), Procurement (G-11) และ Survey (G-07) ต้องแนบรูป/หลักฐานและเก็บลายเซ็นแบบเดียวกัน. โมดูลเดิมแต่ละตัวทำตารางผูกไฟล์ของตัวเอง (`opportunity_work_images`, `site_survey_evidence`, `item_images`) ซึ่งซ้ำซ้อนและทำให้กฎ scope/permission ต่างกันไปทีละที่. เราเลือกตารางกลาง `files.attachment_links` และ `files.signature_captures` ที่อ้าง owner ด้วย `(owner_type, owner_id)` โดย **owner type เป็น whitelist ในโค้ด** (`AttachmentOwnerTypes` + `AttachmentOwnerRegistry`) ไม่ใช่ string อิสระ และ permission/สถานะที่แก้ได้/การค้นหา owner พร้อมบังคับ Organization-Branch scope ถูก resolve ต่อ owner type จึงไม่มีทางเข้าถึงไฟล์ของ owner ที่ตนไม่มีสิทธิ์ผ่านช่องทางกลางนี้.

ข้อเสียที่ยอมรับ: ฐานข้อมูลบังคับ foreign key ไปยัง owner ไม่ได้ (polymorphic) จึงต้องตรวจในแอปทุกครั้ง และ DB check constraint ตรวจได้เพียงรูปแบบของ `owner_type` ไม่ใช่รายชื่อ (เพื่อไม่ต้องแก้ migration ทุกครั้งที่ลงทะเบียน owner ใหม่); การลงทะเบียน owner ใหม่ต้องแก้สามจุดพร้อมกัน (whitelist, registry descriptor, scope reader) โดยมี test ตรวจความครบคู่. ไฟล์ยังเป็นรูปภาพ (JPEG/PNG/WebP ≤ 10 MB) ตาม Files module เดิม — เอกสาร PDF ของ Procurement ต้องขยาย Files module แยกต่างหาก.

ลายเซ็นเก็บเป็นภาพ PNG (ไฟล์) + SHA-256 ของ bytes + ชื่อผู้ลงนาม + เวอร์ชันถ้อยคำยินยอม + เวลาเซิร์ฟเวอร์ ใช้กฎร่วมกับ External Acceptance (CP-07) แต่ไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตามกฎหมาย. ถ้า Legal กำหนดระดับสูงกว่านี้ ต้องเปิด slice ใหม่แล้วอ้างอิง ADR นี้.
