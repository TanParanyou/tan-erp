---
status: accepted
---

# Organization/Branch Administration: ปิดใช้แทนลบ, นิยาม "เอกสารเปิด" ตาม status รายตาราง, Branch Code แก้ไม่ได้

เอกสารจากทุกโมดูล (Estimate, Quotation, PO, Billing, Work Order, Project, Installation ฯลฯ) อ้าง `branch_id`. ก่อน G-03a ไม่มีทางแก้ Organization profile หรือสร้าง/ปิดสาขาโดยไม่แก้ฐานข้อมูล.

**การตัดสินใจ 1 — ปิดใช้ ไม่ลบ:** Branch ไม่มี hard delete (FK จาก 15+ ตารางเป็น `Restrict`). ปิดสาขาได้เมื่อไม่มี "เอกสารเปิด" (Estimate ไม่ใช่ quoted/cancelled, Quotation issued, PO draft..partially_received, Billing issued/partially_paid, Work Order draft..in_progress, Project planned..ready_for_handover, Installation ไม่ terminal, Site Survey scheduled/in_progress, Opportunity ไม่ใช่ won/lost/cancelled, Quick Estimate ไม่ใช่ converted, MRP run ที่มี recommendation proposed) ไม่มี Warehouse active ไม่มี Membership active และไม่ใช่สาขา active สุดท้าย. `GoodsReceipt`/`StockDocument` เป็นเอกสาร posted ไม่มี lifecycle เปิดจึงไม่นับ. ผู้ตรวจ (`BranchDependencyInspector`) เป็นรหัสตัวเดียวที่ใช้ทั้ง endpoint `deactivation-check` และ guard จริงใน transaction เดียวกับการปิด เพื่อให้ UI กับ Backend ไม่เห็นต่างกัน. เมื่อโมดูลใหม่เพิ่ม `branch_id` ต้องเพิ่มรายการในผู้ตรวจ — test `BranchDependencyInspectorCoverageTests` ไล่ entity ที่มี `BranchId` แล้วล้มเมื่อไม่อยู่ในรายการ "นับ" หรือ "ยกเว้น" อย่างชัดเจน.

**การตัดสินใจ 2 — `Branch.Code` แก้ไม่ได้หลังสร้าง:** ถูกใช้ใน projection ของ Estimate/Quotation และ preview เลขที่เอกสาร. แก้ได้เฉพาะชื่อ th/en, เลขสาขาภาษี, ที่อยู่, โทรศัพท์.

**การตัดสินใจ 3 — เลขผู้เสียภาษี/เลขสาขาภาษีเก็บเป็นข้อมูลธรรมดา:** ตรวจรูปแบบ (13 หลัก+checksum / 5 หลัก) แต่ไม่เรียกกรมสรรพากร. G-15 (ใบกำกับภาษี) เป็นผู้บังคับว่าต้องมี.

**การตัดสินใจ 4 — โลโก้เลื่อนไป G-04:** ต้องลงทะเบียน owner type `organization` ใน owner registry (ADR 0017) พร้อมกฎอ่านฝั่ง server สำหรับ PDF ซึ่ง G-04 เป็นผู้ใช้รายแรก; ทำก่อนจะเป็นฟิลด์ไร้ผู้ใช้.

**การตัดสินใจ 5 — store แยก:** `IOrganizationAdministrationStore` แยกจาก `IdentityAdministrationStore` (700+ บรรทัด). Helper transaction ที่ซ้ำ (`RunAsync`) ถูก extract เป็น `SerializableTransactionRunner` เพื่อให้ G-03b/G-03c ใช้ซ้ำ.

ข้อเสียที่ยอมรับ: ช่องว่างแข่งขัน — เอกสารใหม่ที่สร้างพร้อมการปิดสาขาอาจหลุดการนับ (การสร้างเอกสารไม่ล็อกแถวสาขา). ลดความเสี่ยงด้วยการปิดสาขาเป็น action ที่หายากและทำภายใต้ transaction `Serializable` + advisory lock ต่อ Organization; ไม่เพิ่มการล็อกสาขาในทุกเส้นทางสร้างเอกสาร (blast radius ใหญ่). ถ้า Operations ต้องการรับประกันเด็ดขาด ให้เปิด slice เพิ่ม `branch.is_active` check ใน document creation.
