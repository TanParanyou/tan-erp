---
status: accepted
---

# Branch Rate Override ของแม่แบบราคา และ Conversion ที่สร้างรายการขอบเขต

Quick Estimate (CP-16) แปลงเป็นร่าง Official Estimate เปล่า ไม่มีราคาเฉพาะสาขา และแนบหลักฐานไม่ได้. ADR นี้บันทึก 3 การตัดสินใจของ G-09.

**Conversion:** แม่แบบไม่มี mapping ไป Item/ต้นทุน จึงแปลง **เส้นวัด** ใน calculation snapshot เป็น Work Item ขอบเขต (ปริมาณคิดราคา + หน่วยของแม่แบบ) ไม่มี cost และไม่คัดลอกจำนวนเงิน. เติมเฉพาะร่างที่ว่างจริง (ไม่ทับงานเดิม, retry ปลอดภัย). ไม่ backfill conversion เดิม.

**Branch Rate Override:** เอนทิตีใหม่ `BranchRateOverride` (`quick_estimate.branch_rate_overrides`) ผูก template version เดียวและสาขาเดียว แทนเฉพาะ `referenceRate`/`minimumCharge` ในช่วงวันที่ ≤ 366 วัน โดยไม่แก้ template (ชุดย่อยของ `pricing-template-governance.md` หัวข้อ Branch Rate Override). ผู้ขอ (`pricing-templates.override-rate`) ≠ ผู้ตัดสิน (`pricing-templates.approve`) บังคับทั้งใน domain และ DB check. ไม่ใช้ `RoleChangeRequest` (ผูกกับ Role/สิทธิ์) และไม่ทำ template ที่มี `BranchId` (กระทบ unique index/effective list ทั้งโมดูล). ข้ามสาขาไม่ได้: request ไม่รับ `branchId`, นอกสาขา = 404, คำนวณค้นด้วยสาขาของใบประเมิน. Override อยู่ใน snapshot และ `InputHash` เฉพาะเมื่อมี.

**Evidence:** ลงทะเบียน owner type `quick-estimate` ใน Shared Attachment (ADR 0017) ด้วย permission เดิมของโมดูล.

ข้อเสียที่ยอมรับ: `pricing-templates.approve` อนุมัติราคาเฉพาะสาขาโดยไม่เป็น approval-class (ADR 0020) เพราะผลคือช่วงราคาเบื้องต้น; override ไม่ข้าม template version; endpoint Quick Estimate เดิมยังไม่ตรวจ branch.
