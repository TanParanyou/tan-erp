# Master Data (ข้อมูลหลัก)

**สถานะ:** Accepted Direction สำหรับ Item/Unit/Cost; Master Data อื่นยังเป็น Draft

Master Data ที่ Phase Estimation ต้องมีอย่างน้อย:

- Item Category และ Item
- Unit of Measure
- Material/Service/Labor Type
- Cost Source และ Versioned Cost Record พร้อม Effective Period
- Tax Code และ Currency ที่อนุญาต
- Document Numbering Rule
- Organization และ Branch

Item, Unit, Cost Source และ Versioned Cost Record ใช้รายละเอียดจาก [Item Master Flow](../01-business/item-master-flow.md), [Field Catalog](../01-business/item-master-field-catalog.md) และ [Governance](../01-business/item-master-governance.md) เป็นแหล่งอ้างอิงหลัก Pricing Template และ Reference Rate เป็น Versioned Master Data ที่มีกระบวนการอนุมัติเพิ่มเติมตาม [Pricing Template Governance](../01-business/pricing-template-governance.md)

## Data Quality Rules

- Code ต้องไม่ซ้ำภายใน Organization ตามขอบเขตที่กำหนด
- รายการที่เคยถูกใช้ให้เปลี่ยนเป็น Inactive แทนการลบ
- Item ไม่มี `currentCost`; Cost เป็น Versioned Record ที่ต้องมี Source, Unit, Currency, Effective Period และ Lifecycle
- Unit Conversion ต้องระบุ From, To, Factor และเงื่อนไขการปัดเศษ
- การ Import ต้องมี Preview, Validation Result และ Error Report ก่อน Commit

## ตัวอย่าง Item

```text
Code: MAT-PLY-18
Name TH: ไม้อัดยาง 18 มม.
Type: Material
Base Unit: Sheet
Status: Active
```

Cost Example: แยกเป็น Cost Record `1,250 THB/sheet`, Supplier Quote, Effective `2026-09-01`, Branch/Organization Scope และสถานะ Published

ข้อมูลและตัวเลขนี้เป็น `TEST_ONLY` ไม่ใช่ราคาจริงสำหรับใช้งาน Logical Schema อยู่ที่ [Item Master Data Contract](item-master-data-contract.md)
