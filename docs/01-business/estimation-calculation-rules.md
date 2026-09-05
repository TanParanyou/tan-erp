# Estimation Calculation Rules (กฎคำนวณราคา)

**สถานะ:** Draft — สูตรสุดท้ายต้องยืนยันกับฝ่ายประเมินราคาและบัญชี

## ลำดับคำนวณที่แนะนำ

```text
Direct Cost = Material + Labor + Subcontract + Other Direct Cost
Allocated Overhead = ผลรวมค่าใช้จ่ายที่จัดสรรตามกฎองค์กร
Total Cost = Direct Cost + Allocated Overhead
Selling Price Before Discount = ผลจาก Pricing Method ที่เลือก
Net Before Tax = Selling Price Before Discount - Discount
Tax = Net Before Tax × Tax Rate
Grand Total = Net Before Tax + Tax
```

## Markup และ Margin ไม่เหมือนกัน

- `Markup % = (Selling Price - Cost) ÷ Cost × 100`
- `Margin % = (Selling Price - Cost) ÷ Selling Price × 100`

ตัวอย่างต้นทุน 100 บาท:

- Markup 25% → ราคาขาย 125 บาท → Margin 20%
- Margin 25% → ราคาขาย 133.33 บาท → Markup 33.33%

ระบบต้องเก็บว่าใช้วิธีใด ห้ามใช้คำสองคำนี้แทนกัน

## Precision และ Rounding

- คำนวณด้วย Decimal ตาม Precision ที่กำหนด
- Quantity, Unit Cost, Rate และ Money อาจใช้ Precision ต่างกัน
- กำหนดว่าปัดระดับ Line, Section, Subtotal, Tax หรือ Document Total
- แสดงค่าที่ปัดแล้วตรงกับค่าที่ใช้สร้างเอกสารและ Audit

## Override

การ Override Unit Cost, Selling Price, Discount, Tax หรือ Margin ต้องเก็บค่าก่อนแก้ ค่าหลังแก้ เหตุผล ผู้แก้ และเวลาที่แก้ และอาจ Trigger Approval ใหม่ตาม Matrix

## Validation Questions

- Pricing Method เริ่มต้นต่อ Organization/หมวดงานคืออะไร
- Overhead จัดสรรเป็นเปอร์เซ็นต์ ยอดคงที่ หรือตาม Driver
- Discount ใช้ระดับ Line/Section/Document และเรียงก่อนหรือหลัง Tax
- VAT รวมในราคาหรือบวกเพิ่ม และมี Withholding Tax ใน Flow ใด
- Currency เดียวหรือหลาย Currency และใช้อัตราแลกเปลี่ยนวันใด
