# Scope and Non-goals (ขอบเขตและสิ่งที่ยังไม่ทำ)

**สถานะ:** Accepted สำหรับทิศทาง; รายละเอียดธุรกิจเป็น Draft จนผ่าน Workshop

## Phase 1: Estimation Foundation

- Organization, Branch, User, Role, Permission และ Scope
- Customer, Contact, Address, Site และ Opportunity ขั้นพื้นฐาน
- Item Master, หน่วยนับ, หมวดหมู่ และ Versioned Cost Record
- Versioned Site Survey, Measurement, Checklist และ Evidence Reference
- Estimate, Work Item, Revision, Versioned Calculation/Tax Policy, Margin/Markup, Discount และ Customer-safe Output
- Versioned Approval Policy, Maker–Checker, Audit Trail และ Quotation
- ภาษาไทย/อังกฤษและ Error Contract กลาง

## Phase ถัดไป

- Quick Estimate สำหรับ Built-in, ผ้าม่าน และ Wallpaper เป็น Optional Module หลัง Official Estimate Foundation พร้อม
- Project Planning และ Budget Control
- Procurement และ Supplier
- Inventory/Warehouse
- Production และ MRP
- Installation, Handover, Warranty และ After-sales
- Billing, Payment และการเชื่อมระบบบัญชี

## Non-goals ระยะแรก

- ไม่สร้างระบบบัญชีเต็มรูปแบบ
- ไม่ทำ Microservices หรือ Event Sourcing
- ไม่สร้าง MRP ก่อนข้อมูล Item, BOM, Inventory และ Production พร้อม
- ไม่ให้ Firebase เป็นแหล่งเก็บ Role/Permission
- ไม่ทำ Workflow Builder ที่ผู้ใช้ปรับได้ทุกอย่าง
- ไม่ทำ Offline-first synchronization หลายอุปกรณ์ใน Release แรก
- ไม่ออก Quotation ที่อนุมัติแล้วจาก Quick Estimate ณ หน้างาน
- ไม่ใช้ AI ประเมินราคาจากภาพเพียงอย่างเดียวโดยไม่มี Template และการตรวจของคน
- ไม่คัดลอก Source Code จากโครงการอื่นทั้งชุด

## สมมติฐานที่ต้องยืนยัน

- Default Margin/Markup และ Overhead ต่อ Work Type; Baseline รองรับทั้งสองวิธี
- ลำดับ ผู้ดำรง Authority และวงเงินจริง; ระหว่างรอใช้ Production Bootstrap แบบ Fail-closed
- Tax Rate/Display Policy, หัก ณ ที่จ่าย และรูปแบบเลขเอกสาร
- หน่วยนับ การแปลงหน่วย และอายุราคาวัสดุ
- Customer-facing Field/Description ที่อนุญาตใน Quotation
