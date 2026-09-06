# Audit and Revision (ประวัติและฉบับแก้ไข)

**สถานะ:** Accepted Principle

## Audit Trail

บันทึกอย่างน้อย:

- ใครทำ (`userId`)
- ทำอะไร (`action`)
- กับ Resource ใด (`resourceType`, `resourceId`)
- เมื่อใด (`occurredAtUtc`)
- จากช่องทางใด (`traceId`, client/application)
- เหตุผล เมื่อเป็น Override, Reject, Cancel หรือสิทธิ์สำคัญ

Audit Log ต้อง Append-only สำหรับผู้ใช้ทั่วไปและหลีกเลี่ยงการเก็บ Secret หรือ Payload ส่วนบุคคลทั้งหมดโดยไม่จำเป็น

## Business Revision

Estimate Revision คือ Snapshot ทางธุรกิจ ไม่ใช่ Audit Log:

- Revision มีเลขลำดับและสถานะของตน
- Revision ที่ Approved/Quoted แล้วไม่ถูกเขียนทับ
- Revision ใหม่ระบุ Parent Revision และเหตุผล
- Quotation อ้างอิง Revision ที่ใช้สร้างจริง
- การเปรียบเทียบ Revision แสดงรายการเพิ่ม ลบ และเปลี่ยนค่า

Audit ตอบว่า “ใครทำอะไรเมื่อใด” ส่วน Revision ตอบว่า “เนื้อหาแต่ละฉบับคืออะไร” จึงต้องมีทั้งสองอย่าง

## Quick Estimate Version และ Official Estimate Revision

สองคำนี้มีหน้าที่ต่างกันและห้ามใช้แทนกัน:

| Record | ใช้เมื่อ | สิ่งที่ Snapshot | กฎหลังเผยแพร่ |
| --- | --- | --- | --- |
| Quick Estimate Version | ประเมินช่วงราคาหน้างานและออก Preliminary Summary | Template Version, Input, Factor, Price Range, Assumptions, Exclusions และข้อมูลที่ลูกค้าเห็น | เมื่อแชร์แล้ว การเปลี่ยนเนื้อหาสำคัญต้องสร้าง Version ใหม่ |
| Official Estimate Revision | คำนวณต้นทุนและราคาขายทางการเพื่ออนุมัติและออก Quotation | Work Item, Cost Component, Calculation Rule, Tax, Discount และ Approval context | เมื่อ Approved/Quoted แล้วห้ามแก้ ต้องสร้าง Revision ใหม่ |

การ Convert ต้องบันทึก `sourceQuickEstimateId` และ `sourceQuickEstimateVersion` บน Official Estimate Draft พร้อม Snapshot ของข้อมูลที่รับมา Price Range เป็น Reference เท่านั้น ไม่ใช่ Approved Selling Price

Official Estimate ต้องบันทึก Calculation/Tax/Approval Policy Version และ Snapshot Hash ที่ใช้จริง การเปลี่ยน Published Policy ห้ามแก้ผลของ Revision เดิม การ Cancel Submitted ต้องปิด Open Approval Route และบันทึกผู้สั่ง เหตุผล Permission/Authority และเวลาใน Transaction เดียว

## Quick Estimate Audit Events

อย่างน้อยต้องบันทึก:

- `quick-estimate.created`, `calculated`, `reviewed`, `shared`, `versioned`, `converted`, `closed` และ `expired`
- `pricing-template.created`, `submitted`, `approved`, `calibrated`, `activated`, `superseded` และ `disabled`
- Manual Override พร้อมค่าก่อน/หลังและเหตุผล
- Preliminary Summary share attempt, ผลสำเร็จ/ล้มเหลว, ช่องทาง และ Source Version โดยไม่เก็บข้อมูลผู้รับเกินจำเป็น
- Conversion result, Official Estimate ID และ Idempotency Key fingerprint ที่ปลอดภัย

Audit Event บอกเหตุการณ์ ส่วน Quick Estimate Version และ Official Estimate Revision เป็น Business Snapshot ที่ใช้อ่านเนื้อหา ณ เวลานั้น

## Calculation Snapshot ขั้นต่ำ

- Pricing Template/Rate Source ID และ Version
- Measurement Input, Unit และ Billable Quantity
- Grade/Complexity Factor, Risk Modifier และ Checklist Result
- Add-on, Minimum Charge และ Published Promotion/Adjustment
- Net, Tax, Gross, Validity และ Tax Display Policy
- Raw/Displayed Bounds และ Rounding Step
- Formula/Rule Version และผล Share Decision พร้อม Reason Codes

Calculation Snapshot ต้อง Immutable หลังสร้าง Version และต้องเพียงพอให้คำนวณซ้ำได้ผลเดิม แม้ Pricing Template ปัจจุบันเปลี่ยนแล้ว

Entity, Snapshot Envelope และ Immutability Constraint อยู่ที่ [Quick Estimate Data Contract](quick-estimate-data-contract.md)

Entity และ Constraint ของ Official Estimate Revision อยู่ที่ [Official Estimate Data Contract](official-estimate-data-contract.md)
