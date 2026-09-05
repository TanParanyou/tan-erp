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
