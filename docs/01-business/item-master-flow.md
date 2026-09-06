# Item Master Flow (กระบวนการข้อมูลสินค้า บริการ และต้นทุน)

**สถานะ:** Accepted Direction — Production Baseline ก่อนเชื่อม Official Estimate

เอกสารนี้เป็น Flow หลักของ Item Master, Unit และ Cost Record รายการ Field อยู่ที่ [Item Master Field Catalog](item-master-field-catalog.md) และการกำกับดูแลอยู่ที่ [Item Master Governance](item-master-governance.md)

## เป้าหมาย

ทำให้ผู้ประเมินเลือก Item, Unit และต้นทุนที่มีที่มา/ช่วงเวลามีผลได้อย่างสม่ำเสมอ พร้อมเก็บ Snapshot ใน Estimate และยังขยายไป Procurement, Inventory, Production และ MRP ได้ในอนาคต

## Item Flow

```text
Create Draft
  → classify Item Type and Category
  → assign Base Unit and Capabilities
  → validate
  → Activate
  → use in Estimate/Cost Record
  → update descriptive data or Deactivate
```

```text
Draft → Active → Inactive
```

- Draft ยังถูกเลือกใน Estimate ใหม่ไม่ได้
- Active ใช้ได้ตาม Capability และ Scope
- Inactive ห้ามเลือกใหม่ แต่ Snapshot/ประวัติเดิมยังอ่านได้
- Code เปลี่ยนไม่ได้หลัง Active ครั้งแรก
- Item ที่เคยถูกอ้างห้าม Hard Delete

## Cost Flow

```text
Create Cost Draft + Source Evidence
  → validate Item/Unit/Currency/Quantity/Effective Period
  → Submit → Approve/Return → Publish
  → resolve deterministically for Estimate
  → Supersede or Disable
```

```text
Draft → Submitted → Approved → Published → Superseded
  ▲          │                              └────→ Disabled
  └──Return──┘
```

Published Cost Record แก้ย้อนหลังไม่ได้ Version ใหม่ใช้ Effective Period ใหม่ Estimate เก็บ Cost Snapshot จึงไม่เปลี่ยนเมื่อ Master Data เปลี่ยน

## Standard Sequence

1. สร้าง Category/Unit ที่จำเป็น
2. สร้าง Item Draft พร้อม Type, Base Unit และ Capability
3. Activate Item เมื่อ Field Gate ผ่าน
4. สร้าง Cost Source และ Cost Record ตาม Unit/Currency/Branch/Quantity Break
5. Submit และผ่าน Maker–Checker ก่อน Publish
6. Official Estimate ขอ Resolve Cost ด้วย Item + Branch + Unit + Currency + Quantity + Estimate Date
7. ระบบคืน Cost Record เดียวพร้อม Version/Source หรือ Stable Error
8. Estimate บันทึก Snapshot ที่ใช้จริง

## Cost Resolution Flow

```text
Published and effective?
  No → ITEM_COST_NOT_FOUND
  Yes → Item/Unit/Currency/Quantity match?
    No → ITEM_COST_NOT_FOUND
    Yes → Branch-specific exists?
      Yes → prefer Branch scope
      No → use Organization scope
        → apply Source Priority
        → newest effectiveFrom
        → still tied? ITEM_COST_AMBIGUOUS
```

ระบบห้ามเลือกต้นทุนจาก “รายการแรกที่พบ” หากไม่สามารถตัดสินได้แน่นอนต้อง Block

## Import Flow

```text
Upload CSV/XLSX
  → Parse as data only
  → Preview Create/Update/Skip/Error
  → Fix or Commit atomically
  → Audit Import Batch and affected records
```

Import ไม่รัน Macro, Formula หรือ External Link และไม่ Commit เฉพาะบางแถวโดยค่าเริ่มต้น

## Action Matrix

| Action | State | Permission | Guard | Result |
| --- | --- | --- | --- | --- |
| Create Item | — | `items.create` | Organization Scope | Draft |
| Patch Item | Draft/Active | `items.update` | ETag; immutable Code rule | ETag ใหม่ |
| Activate Item | Draft/Inactive | `items.activate` | Required Field/Unit/Capability | Active |
| Deactivate Item | Active | `items.deactivate` | Reason + no destructive cascade | Inactive |
| Create Cost | Active Item | `cost-records.create` | Unit/Currency/Source/Scope | Cost Draft |
| Submit Cost | Draft/Returned | `cost-records.submit` | Field/Period complete | Submitted |
| Review Cost | Submitted | `cost-records.approve` | Authority + Maker–Checker | Approved/Returned |
| Publish Cost | Approved | `cost-records.publish` | No overlap + Idempotency | Published |
| Disable Cost | Published | `cost-records.disable` | Reason + Authority | Disabled |
| Commit Import | Validated Batch | `item-imports.commit` | No Error + Idempotency | Atomic changes |

## ตัวอย่างสั้น

`TEST_ONLY`: Item `MAT-PLY-18` เป็น Material, Base Unit `sheet`, ใช้คิดต้นทุนและจัดซื้อได้ Cost Record สาขากรุงเทพมีผลตามวันที่และ Quantity Break เมื่อ Estimate ต้องการ `6.0000 sheet` ระบบเลือก Branch Cost ที่ตรง หากมี Published Record เท่ากันสองรายการให้คืน `ITEM_COST_AMBIGUOUS` ไม่สุ่มราคา

## เอกสารที่เกี่ยวข้อง

- [Item Master Field Catalog](item-master-field-catalog.md)
- [Item Master Governance](item-master-governance.md)
- [Official Estimate Field Catalog](official-estimate-field-catalog.md)
- [Item Master API Contract](../03-contracts/item-master-api-contract.md)
- [Item Master Data Contract](../04-data/item-master-data-contract.md)
