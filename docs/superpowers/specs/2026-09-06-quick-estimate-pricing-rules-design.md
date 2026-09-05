# Quick Estimate Pricing Rules Design

## 1. สถานะเอกสาร

- วันที่ออกแบบ: 6 กันยายน 2026
- สถานะ: Accepted from Workshop — พร้อมจัดทำ Implementation Plan ด้านเอกสารหลังผู้ใช้ตรวจฉบับนี้
- ระบบ: `tan-erp` Project ERP
- ขอบเขต: สูตรและกฎธุรกิจของ Quick Estimate สำหรับ Built-in, ผ้าม่าน และ Wallpaper
- เอกสารที่เกี่ยวข้อง: [Quick Estimate Workflow Design](2026-09-06-quick-estimate-workflow-design.md)

## 2. ผู้อ่านและเป้าหมาย

เอกสารนี้เป็น Explanation และ Reference สำหรับเจ้าของธุรกิจ ฝ่ายประเมินราคา ผู้ดูแล Pricing Template และทีมพัฒนา เพื่อให้ทุกฝ่ายเข้าใจตรงกันว่า Quick Estimate คำนวณช่วงราคาอย่างไร รายการใดแชร์ได้ และกฎใดปรับได้โดยไม่เปลี่ยน Core System

เอกสารนี้ไม่กำหนดราคาจริง วงเงินจริง เปอร์เซ็นต์กำไร หรือจำนวนวันใช้งานแทนลูกค้า ค่าเหล่านี้ต้องผ่าน Calibration และ Limited Pilot ก่อนเปิดใช้จริง

## 3. การตัดสินใจจาก Workshop

| เรื่อง | แนวทางที่อนุมัติ |
| --- | --- |
| Work Type แรก | Built-in, ผ้าม่าน และ Wallpaper |
| Pricing Method | Hybrid: Quick Estimate ใช้ Rate/Factor; Official Estimate แตกต้นทุนจริง |
| Price Range | Base Range จาก Template และขยายตาม Risk Modifier |
| Measurement | Template เป็นเจ้าของหน่วยและ Measurement Rule ของแต่ละ Work Type |
| Material Grade | ตัวเลือกตาม Template; Custom ต้องมีข้อมูลเพิ่มและผ่าน Review |
| Complexity | ระบบเสนอระดับจาก Checklist; Override ต้องมีเหตุผล |
| Rate Ownership | Organization มีราคาหลัก; Branch Override ได้ตามสิทธิ์และมีวันหมดอายุ |
| Validity | กำหนดตาม Template และไม่เกินอายุของ Reference Rate ที่ใช้ |
| Evidence | Required Checklist แยกตาม Template |
| Share Review | Risk-based Review พร้อม Maker–Checker |

## 4. หลักการออกแบบ

- Quick Estimate ต้องทำได้เร็วบนมือถือ แต่ผลลัพธ์ต้องอธิบายและคำนวณย้อนหลังได้
- สูตรจริงอยู่ใน Pricing Template Version ไม่ฝังค่าทางธุรกิจไว้ใน Application Code
- Template ประกอบจาก Operation ที่ระบบรองรับและตรวจสอบได้ ไม่อนุญาต Raw Script หรือ Formula อิสระ
- เงิน อัตรา ปริมาณ และ Factor ใช้ Decimal ตาม Precision Policy ห้ามใช้ Floating Point
- ทุกผลคำนวณเก็บ Input, Rate, Factor, Rule Version และ Output เป็น Snapshot
- Price Range เป็นข้อมูลเบื้องต้น ไม่ใช่ Approved Selling Price หรือ Quotation
- Official Estimate ต้องคำนวณต้นทุนและราคาขายใหม่ตามกฎทางการ

## 5. Calculation Model (แบบจำลองการคำนวณ)

### 5.1 สูตรกลาง

```text
Billable Quantity = Measurement Rule(Input)
Line Base Amount = Billable Quantity × Reference Rate
Line Adjusted Amount = Line Base Amount × Grade Factor × Complexity Factor
                      + Line Add-ons

Adjusted Amount = Σ Line Adjusted Amount + Document Add-ons
Estimated Amount Before Adjustment = max(Adjusted Amount, Minimum Charge)
Net Estimated Amount = Apply Published Promotion/Adjustment,
                       otherwise Estimated Amount Before Adjustment

Risk Rate = min(Max Range Rate, Base Range Rate + Σ Risk Modifiers)
Raw Net Lower Bound = Net Estimated Amount × (1 - Risk Rate)
Raw Net Upper Bound = Net Estimated Amount × (1 + Risk Rate)

Raw Display Bounds = Apply Tax Display Policy(Raw Net Bounds)
Displayed Lower Bound = Round Down(Raw Display Lower Bound, Rounding Step)
Displayed Upper Bound = Round Up(Raw Display Upper Bound, Rounding Step)
```

`Minimum Charge` เป็นยอดขั้นต่ำ ไม่ใช่ค่าที่บวกเพิ่มทุกครั้ง ส่วน `Add-on` เป็นค่าที่เพิ่มจากงานหรือเงื่อนไขเฉพาะ เช่น รื้อถอน ติดตั้งที่สูง หรือทำงานนอกเวลา `Published Promotion/Adjustment` เป็นกฎที่อนุมัติและอยู่ใน Effective Period เท่านั้น หากไม่มีให้ใช้ยอดก่อน Adjustment โดยตรง

### 5.2 Guardrails (ข้อควบคุม)

- Quantity, Rate, Factor และ Price Range ต้องเป็นค่าบวกภายในช่วงที่ Template อนุญาต
- Unit ของ Input, Measurement Rule และ Reference Rate ต้องเข้ากัน
- Factor ทุกตัวต้องมี Code, ชื่อ, เหตุผล และช่วงค่าที่อนุญาต
- หากไม่มี Rate ที่ใช้ได้ ห้ามคำนวณหรือแชร์จากค่าที่ผู้ใช้เดา
- Custom Material ที่มี Provisional Rate ซึ่งอนุมัติไว้คำนวณได้ แต่ต้อง `PendingReview`
- การแก้ Rate, Factor หรือ Result ด้วย Manual Override ต้องเก็บค่าก่อน/หลัง เหตุผล และผู้แก้

## 6. Measurement Rules ตามประเภทงาน

ทุกประเภทงานใช้โครงสร้าง Measurement Line กลาง แต่ Template กำหนด Field, Unit, Formula และ Validation ที่ต่างกัน

### 6.1 Built-in

ข้อมูลขั้นต่ำ:

- Room/Area และชนิดชิ้นงาน
- Width, Height, Depth และ Quantity ตามชนิดชิ้นงาน
- หน่วยคิดเงิน เช่น Linear Meter, Square Meter หรือ Piece ซึ่ง Template เป็นผู้กำหนด
- Material Grade และตัวเลือกบาน/ลิ้นชักที่มีผลต่อราคา
- Complexity Checklist เช่น งานโค้ง งานระบบซ่อน รื้อถอน หรือพื้นที่เข้าถึงยาก

ตัวอย่างย่อ:

```text
ตู้เสื้อผ้า → กว้าง 3 เมตร → สูง 2.6 เมตร
→ Premium → มีลิ้นชัก → ติดตั้งปกติ → Price Range
```

### 6.2 ผ้าม่าน

ข้อมูลขั้นต่ำ:

- จุดติดตั้ง, Width, Height และจำนวนช่องเปิด
- Curtain Type และ Fullness Ratio ตาม Preset
- Fabric Grade, Lining และ Track/Rod Type
- Installation Height และเงื่อนไขติดตั้ง

Measurement Rule อาจคำนวณปริมาณผ้าจากขนาดช่องเปิด อัตราจีบ จำนวนผืน และค่าเผื่อ โดยแต่ละ Template ต้องระบุ Unit และวิธีปัดปริมาณชัดเจน

### 6.3 Wallpaper

ข้อมูลขั้นต่ำ:

- Room/Wall และ Width/Height หรือ Gross Area
- Opening ที่อนุญาตให้หักตาม Policy
- Roll Coverage หรือ Square Meter Rate ของวัสดุ
- Waste Factor, Surface Condition และ Removal Option

หากคิดเป็นม้วน ระบบต้องปัดจำนวนม้วนขึ้นหลังใช้ Coverage และ Waste Rule ห้ามปัดราคาก่อนปัดปริมาณ

## 7. Material Grade และ Complexity

### 7.1 Material Grade

- แต่ละ Template กำหนด Grade Option ของตนเอง เช่น Standard และ Premium
- แต่ละ Grade เชื่อมกับ Factor หรือ Rate Set ที่มี Version
- `Custom` ไม่ใช้ Factor กลางโดยอัตโนมัติ ต้องมีรายละเอียดวัสดุและ Rate ที่ตรวจสอบได้
- เปลี่ยน Grade หลังแชร์ถือเป็น Material Change และต้องสร้าง Quick Estimate Version ใหม่

### 7.2 Complexity

ระบบคำนวณ Complexity จาก Checklist แล้วเสนอระดับและ Factor ให้ผู้ใช้ยืนยัน ตัวอย่างปัจจัย:

- เข้าถึงหรือติดตั้งยาก
- งานโค้ง งานเข้ามุม หรืองานรูปทรงเฉพาะ
- รื้อถอนหรือแก้สภาพพื้นที่เดิม
- ระบบไฟ/ประปาซ่อนหรือจุดเชื่อมต่อพิเศษ
- ทำงานนอกเวลา หรือมีข้อจำกัดอาคาร

การเลือก Factor ที่ต่างจากผล Checklist เป็น Manual Override และต้องระบุเหตุผล พร้อมส่ง Review ตาม Share Policy

## 8. Price Range และ Risk Modifier

Template กำหนด `BaseRangeRate`, `MaxRangeRate` และรายการ Risk Modifier ที่รองรับ ระบบเพิ่ม Risk Rate เมื่อพบเงื่อนไข เช่น:

- ข้อมูลเสริมยังไม่ครบ แต่ไม่ใช่ Required Field
- วัสดุ Custom หรือ Provisional Rate
- หน้างานซับซ้อนหรือมีงานรื้อถอน
- Measurement Confidence ต่ำ
- Reference Rate ใกล้หมดอายุ

ผู้ใช้ทั่วไปลด Risk Rate ให้แคบกว่าค่าที่ระบบคำนวณไม่ได้ การ Override ต้องใช้ Permission เฉพาะ มีเหตุผล และผ่าน Review

## 9. Share Policy

ระบบประเมินผลเป็นสามระดับ:

### 9.1 `Blocked`

- Required Input, Assumption, Exclusion หรือ Evidence ไม่ครบ
- Template/Rate หมดอายุ ถูก Disabled หรือยังไม่ยืนยันจาก Server
- Price Range ไม่เป็นค่าบวกหรือคำนวณไม่สำเร็จ
- ไม่มี Customer Contact/ช่องทางผู้รับที่ตรวจสอบได้
- ผู้ใช้ไม่มี Permission หรือ Resource อยู่นอก Scope

รายการ Blocked บันทึก Draft ได้ แต่แชร์และ Convert ไม่ได้

### 9.2 `PendingReview`

- ใช้ Custom Material หรือ Provisional Rate
- มี Manual Override
- Risk Rate กว้างเกินเพดานแชร์โดยตรง
- Upper Bound เกิน Authority Limit ของ Field Estimator
- พบ Risk Trigger ที่ Policy บังคับตรวจ
- Template Version อยู่ในสถานะ `Calibration`

Reviewer ต้องไม่ใช่ผู้จัดทำเมื่อ Maker–Checker มีผล ระบบรองรับ Reviewer สำรอง, SLA และ Escalation ที่ตั้งค่าได้

### 9.3 `Shareable`

- Template อยู่ในสถานะ `Active`
- Required Input และ Evidence ครบ
- ผลคำนวณอยู่ใน Direct-share Limit
- ไม่มี Manual Override หรือ Risk Trigger ที่ต้องตรวจ
- ผู้ใช้มี `quick-estimates.share` ภายใน Scope

Permission ไม่สามารถข้าม Share Policy ได้

## 10. Pricing Template Lifecycle

```text
Draft → Calibration → Active → Superseded
                         └────→ Disabled
```

| สถานะ | ความหมาย |
| --- | --- |
| `Draft` | กำลังจัด Rule/Rate และยังใช้ประเมินไม่ได้ |
| `Calibration` | ใช้ทดลองกับกรณีควบคุมหรือ Pilot; ทุกการแชร์ต้อง Review |
| `Active` | ใช้คำนวณและแชร์ตาม Risk-based Policy ได้ |
| `Superseded` | มี Version ใหม่แทน แต่ Snapshot เก่ายังอ่านและคำนวณย้อนหลังได้ |
| `Disabled` | หยุดใช้ทันทีเพราะความเสี่ยงหรือข้อมูลผิด; ห้ามคำนวณ/แชร์ใหม่ |

Template Version ต้องเก็บ Work Type, Measurement Rule, Unit, Rate Source, Factor, Add-on, Minimum Charge, Range, Rounding, Required Field/Evidence, Assumption, Exclusion, Validity, Effective Period, Creator และ Approver

เมื่อ Rate, Formula, Unit หรือ Policy ที่มีผลต่อราคาเปลี่ยน ต้องสร้าง Version ใหม่ ห้ามแก้ Version ที่ Published แล้วแบบย้อนหลัง

Organization เป็นเจ้าของ Standard Rate ส่วน Branch Override ได้เมื่อมี Permission, Reason, Effective Period และ Approval ตาม Policy

## 11. Validity, Rounding, Tax และ Discount

### 11.1 Validity

- Validity Period กำหนดตาม Template
- `validUntil` ต้องไม่เกินวันหมดอายุที่สั้นที่สุดของ Template หรือ Reference Rate ที่ใช้
- รายการหมดอายุต้อง Recalculate เป็น Version ใหม่ก่อนแชร์หรือ Convert

### 11.2 Rounding

- การคำนวณภายในใช้ Decimal และเก็บ Money ตาม Currency Precision
- Customer-facing Range ปัดออกด้านนอกตาม `RoundingStep`
- Lower Bound ปัดลงและ Upper Bound ปัดขึ้น เพื่อไม่ทำให้ช่วงแคบกว่าผลจริง

ตัวอย่าง:

```text
Raw Range: 82,340–96,720 บาท
Rounding Step: 1,000 บาท
Displayed Range: 82,000–97,000 บาท
```

### 11.3 Tax

- เก็บ Net, Tax และ Gross แยกกัน
- Organization กำหนด Tax Display Policy ว่า Customer Summary แสดงรวม VAT หรือยังไม่รวม VAT
- Customer Summary ต้องระบุสถานะ VAT ชัดเจนทุกครั้ง
- Tax Rate และ Effective Date ที่ใช้ต้องอยู่ใน Snapshot

### 11.4 Discount

- Quick Estimate ไม่เปิด Free-form Discount เป็นค่าเริ่มต้น
- Promotion/Adjustment ใช้ได้เมื่อ Published, อยู่ใน Effective Period และผ่าน Policy
- Manual Discount หรือ Selling Price Override ต้องมีเหตุผลและ Review
- Official Estimate คำนวณ Discount และ Tax ใหม่ตามกฎทางการ

## 12. Customer Summary, Versioning และ Retry

Preliminary Summary ต้องแสดง Price Range, Tax Status, Scope, Assumptions, Exclusions, Created Date, Valid Until, Reference Number, Version และข้อความว่าไม่ใช่ Quotation

ห้ามแสดง Cost, Margin, Internal Note, Approval Detail, Permission หรือข้อมูลของลูกค้ารายอื่น

- ก่อนแชร์ ผู้ใช้แก้ Draft เดิมได้
- หลังแชร์ การเปลี่ยน Measurement, Template, Grade, Complexity, Price Range, Assumption หรือ Exclusion ต้องสร้าง Version ใหม่
- Retry เนื้อหาเดิมหลัง Network/Delivery Error ไม่สร้าง Version ใหม่
- Share Attempt เก็บ Source Version, Channel, Result และเวลาส่ง โดยไม่เก็บ PII เกินจำเป็น
- Conversion เชื่อม Source Version กับ Official Estimate Draft และทำงานแบบ Idempotent

## 13. Logical Components (ส่วนรับผิดชอบของระบบ)

| Component | หน้าที่ | ขึ้นต่อ |
| --- | --- | --- |
| Pricing Template | เก็บ Published Version, Rate, Factor และ Checklist | Organization/Branch policy |
| Measurement Engine | Validate Input และสร้าง Billable Quantity | Template Version |
| Quick Pricing Calculator | คำนวณ Amount, Risk Rate และ Range | Measurement result และ Rate snapshot |
| Risk & Share Policy | คืน `Blocked`, `PendingReview` หรือ `Shareable` พร้อมเหตุผล | Calculation, Evidence, Permission และ Authority |
| Customer Summary Projector | สร้าง Customer-safe payload | Approved/Shareable Version |
| Official Estimate Converter | Convert Snapshot แบบ Idempotent | Quick Estimate Version และ Estimation contract |

```text
Field Input → Template Validation → Measurement Engine
    → Quick Pricing Calculator → Risk & Share Policy
        ├─ Blocked → แก้ข้อมูล
        ├─ PendingReview → Reviewer
        └─ Shareable → Customer Summary → Official Estimate Draft
```

แต่ละ Component ต้องมี Input/Output Contract ที่ทดสอบแยกได้ การเปลี่ยนสูตรภายใน Template ต้องไม่เปลี่ยน Contract ของผู้ใช้ผลลัพธ์

## 14. Error Handling

- Field/Unit ผิด: ระบุ Field และรักษาข้อมูลอื่นไว้
- Formula หรือ Template ไม่สมบูรณ์: ห้าม Publish
- Template/Rate หมดอายุ: บังคับเลือก Version ที่ใช้ได้และ Recalculate
- Permission/Scope เปลี่ยน: หยุด Share/Convert และรักษา Draft ที่ปลอดภัย
- Concurrent Edit: แจ้ง Version Conflict และให้ Reload/Compare
- Share ล้มเหลว: Retry Source Version เดิมโดยไม่สร้าง Version ใหม่
- Convert ซ้ำด้วย Idempotency Key เดิม: คืน Official Estimate เดิม

API ใช้ Stable Error Code และ RFC 9457 Problem Details ตาม Error Contract กลาง

## 15. Audit และข้อมูลที่ต้องเก็บ

ทุก Calculation Snapshot ต้องเชื่อมกับ:

- Quick Estimate ID และ Version
- Pricing Template ID/Version และ Rate Source Version
- Measurement Input, Unit และ Billable Quantity
- Grade/Complexity Factor และ Risk Modifier
- Add-on, Minimum Charge, Tax, Rounding และผลลัพธ์ก่อน/หลังปัด
- User, Organization, Branch, occurredAtUtc และ Trace ID
- Manual Override พร้อมค่าก่อน/หลังและเหตุผล

Audit Event และ Business Snapshot เป็นคนละหน้าที่: Audit บอกว่าใครทำอะไรเมื่อใด ส่วน Snapshot บอกว่า Version นั้นคำนวณจากอะไร

## 16. Testing และ Rollout

### 16.1 Template Validation

ก่อน Publish ต้องตรวจ:

- Field, Unit และ Formula reference ถูกต้อง
- Test Case ปกติ ขอบเขตต่ำ/สูง และข้อมูลผิด
- Grade, Complexity, Add-on, Minimum Charge และ Risk Modifier
- Outward Rounding, Tax Display และ Validity
- Blocked/PendingReview/Shareable decision cases

### 16.2 Rollout

```text
Template Calibration → Limited Pilot → Active
```

1. `Calibration`: ใช้กรณีตัวอย่างและให้ผู้ประเมินตรวจทุกผลลัพธ์
2. `Limited Pilot`: ใช้กับงานจริงกลุ่มเล็ก ทุกการแชร์ยังต้อง Review และเปรียบเทียบกับ Official Estimate/Quotation
3. `Active`: งานความเสี่ยงต่ำแชร์ได้ทันที งานเสี่ยงยังใช้ Maker–Checker

หากราคาทางการหลุด Quick Estimate Range ต้องบันทึก Root Cause เช่น Measurement, Rate, Grade, Complexity, Scope Change หรือ Site Condition เพื่อนำไปปรับ Template Version ถัดไป

## 17. Metrics และเกณฑ์ผ่าน Pilot

เก็บ Metrics ต่อ Work Type/Template Version:

- เวลาจากเริ่มกรอกจนเห็น Price Range
- Completion และ Abandonment rate
- Review rate และสาเหตุที่ต้อง Review
- ความต่างจาก Official Estimate และ Quotation
- Out-of-range rate พร้อม Root Cause
- Conversion rate และ Field ที่ทำให้ผู้ใช้ติดขัด

ก่อนเปลี่ยน Template เป็น `Active` เจ้าของธุรกิจต้องอนุมัติ Direct-share Limit, Base/Max Range, Validity, SLA และ Accuracy Target จากข้อมูล Pilot ไม่กำหนดตัวเลขเหล่านี้จากการคาดเดา

## 18. Acceptance Criteria

1. Template ทั้งสาม Work Type สร้าง Billable Quantity และ Price Range จาก Input ขั้นต่ำได้
2. Calculation Snapshot สามารถคำนวณซ้ำแล้วได้ผลเดิม
3. Unit mismatch, invalid rate และ incomplete evidence ถูก Block ก่อนแชร์
4. Calibration, Custom, Override และรายการเกิน Authority ถูกส่ง Review
5. Customer Summary ไม่มีข้อมูลภายในและระบุ Tax/Validity/Disclaimer ชัดเจน
6. การเปลี่ยนสาระสำคัญหลังแชร์สร้าง Version ใหม่
7. Retry และ Conversion ไม่สร้างข้อมูลซ้ำ
8. Published Template แก้ย้อนหลังไม่ได้และ Version เก่ายังอ่านได้
9. Test Matrix ครอบคลุมสูตร ขอบเขต Policy, Permission, Scope และ Error Path
10. Template เปลี่ยนเป็น Active ได้เมื่อเจ้าของธุรกิจอนุมัติผล Pilot และค่าควบคุมแล้ว

## 19. Policy Values ที่ต้องได้จาก Pilot

รายการต่อไปนี้เป็น Configurable Business Values ไม่ใช่ช่องว่างของการออกแบบ:

- Reference Rate และ Rate Review Cycle ต่อ Work Type/Branch
- Grade/Complexity Factor และ Add-on Amount
- Base/Max Range และ Risk Modifier
- Rounding Step และ Minimum Charge
- Validity Period และ Rate Expiry rule
- Direct-share Authority Limit และ Reviewer routing
- Calibration sample criteria, Review SLA และ Escalation
- Accuracy Target และ Out-of-range tolerance

ค่าเหล่านี้ต้องมี Owner, Effective Period, Approval และ Version ก่อน Production
