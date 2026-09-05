# Master Data (ข้อมูลหลัก)

**สถานะ:** Draft

Master Data ที่ Phase Estimation ต้องมีอย่างน้อย:

- Item Category และ Item
- Unit of Measure
- Material/Service/Labor Type
- Reference Cost และ Effective Date
- Tax Code และ Currency ที่อนุญาต
- Document Numbering Rule
- Organization และ Branch

## Data Quality Rules

- Code ต้องไม่ซ้ำภายใน Organization ตามขอบเขตที่กำหนด
- รายการที่เคยถูกใช้ให้เปลี่ยนเป็น Inactive แทนการลบ
- Cost ต้องมีแหล่งที่มา วันที่มีผล และผู้บันทึก
- Unit Conversion ต้องระบุ From, To, Factor และเงื่อนไขการปัดเศษ
- การ Import ต้องมี Preview, Validation Result และ Error Report ก่อน Commit

## ตัวอย่าง Item

```text
Code: MAT-PLY-18
Name TH: ไม้อัดยาง 18 มม.
Type: Material
Base Unit: Sheet
Reference Cost: 1,250 THB
Effective Date: 2026-09-01
Status: Active
```

ตัวเลขนี้เป็นตัวอย่างรูปแบบข้อมูล ไม่ใช่ราคาจริงสำหรับใช้งาน
