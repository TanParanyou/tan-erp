# Quick Estimate Pricing Rules (กฎคำนวณราคาประเมินเบื้องต้น)

**สถานะ:** Accepted — โครงสร้างสูตรและกฎใช้เป็นมาตรฐาน ส่วนค่าราคา วงเงิน และ SLA ต้องอนุมัติจาก Pilot ก่อน Production

## เป้าหมายและขอบเขต

เอกสารนี้เป็นแหล่งอ้างอิงหลักสำหรับการคำนวณ Quick Estimate ของงาน Built-in, ผ้าม่าน และ Wallpaper เพื่อให้หน้างานทำราคาได้เร็ว ตรวจสอบย้อนหลังได้ และไม่ทำให้ราคาประเมินเบื้องต้นกลายเป็น Quotation

Quick Estimate ใช้ Rate/Factor สำหรับความเร็ว ส่วน [Official Estimate](estimation-flow.md) ต้องแตกต้นทุนและคำนวณราคาทางการใหม่ตาม [Estimation Calculation Rules](estimation-calculation-rules.md)

## สูตรกลาง

```text
Billable Quantity = Measurement Rule(Input)
Line Base Amount = Billable Quantity × Reference Rate
Line Adjusted Amount = Line Base Amount × Grade Factor × Complexity Factor
                      + Line Add-ons

Adjusted Amount = Σ Line Adjusted Amount + Document Add-ons
Amount Before Adjustment = max(Adjusted Amount, Minimum Charge)
Net Estimated Amount = Apply Published Promotion/Adjustment,
                       otherwise Amount Before Adjustment

Risk Rate = min(Max Range Rate, Base Range Rate + Σ Risk Modifiers)
Raw Net Lower Bound = Net Estimated Amount × (1 - Risk Rate)
Raw Net Upper Bound = Net Estimated Amount × (1 + Risk Rate)

Raw Display Bounds = Apply Tax Display Policy(Raw Net Bounds)
Displayed Lower Bound = Round Down(Raw Display Lower Bound, Rounding Step)
Displayed Upper Bound = Round Up(Raw Display Upper Bound, Rounding Step)
```

`Minimum Charge` เป็นยอดขั้นต่ำ ไม่ใช่ค่าที่บวกเพิ่ม ส่วน Promotion/Adjustment ต้องเผยแพร่ มีช่วงเวลามีผล และผ่านการอนุมัติแล้ว

## กฎคำนวณบังคับ

- Quantity, Rate, Factor และผลราคาเป็น Decimal และต้องอยู่ในช่วงที่ Template อนุญาต
- Unit ของ Input, Measurement Rule และ Reference Rate ต้องเข้ากัน
- Factor ทุกตัวมี Code, ชื่อ เหตุผล และขอบเขตค่าที่อนุญาต
- ไม่มี Rate ที่ใช้ได้ต้อง Block ห้ามให้ผู้ใช้เดาราคาแล้วแชร์
- Custom Material คำนวณได้เมื่อมี Provisional Rate ที่อนุมัติ แต่ต้องส่ง Review
- Manual Override เก็บค่าก่อน/หลัง เหตุผล ผู้แก้ และส่ง Review
- Calculation Snapshot ต้องคำนวณซ้ำแล้วได้ผลเดิม
- Official Estimate ห้ามใช้ค่ากลางของ Price Range เป็น Approved Selling Price

## Measurement ตามประเภทงาน

| Work Type | ข้อมูลขั้นต่ำ | ตัวเลือกที่มีผลต่อราคา |
| --- | --- | --- |
| Built-in | Room/Area, ชนิดชิ้นงาน, Width, Height, Depth, Quantity และหน่วยตาม Template | Material Grade, บาน/ลิ้นชัก, งานโค้ง, งานระบบ, รื้อถอน และการเข้าถึง |
| ผ้าม่าน | จุดติดตั้ง, Width, Height, จำนวนช่องเปิด, Curtain Type และ Fullness Ratio | Fabric Grade, Lining, Track/Rod และ Installation Height |
| Wallpaper | Room/Wall, Width/Height หรือ Gross Area และวิธีหัก Opening | Roll Coverage/Area Rate, Waste Factor, Surface Condition และ Removal Option |

Template แสดงเฉพาะ Field ที่ต้องใช้ หาก Wallpaper คิดเป็นม้วน ต้องปัดจำนวนม้วนขึ้นหลังคำนวณ Coverage/Waste แล้วจึงคำนวณราคา

## Material Grade และ Complexity

- Material Grade แยกตาม Template เช่น Standard/Premium และเชื่อมกับ Factor หรือ Rate Set ที่มี Version
- `Custom` ต้องมีรายละเอียดและ Rate ที่ตรวจสอบได้ ไม่ใช้ Factor กลางอัตโนมัติ
- Complexity มาจาก Checklist เช่น เข้าถึงยาก งานโค้ง รื้อถอน ระบบซ่อน หรืองานนอกเวลา
- ระบบเสนอ Complexity Factor จาก Checklist ให้ผู้ใช้ยืนยัน
- การเลือก Factor ต่างจากผล Checklist เป็น Manual Override

## Price Range และ Risk

Template กำหนด `BaseRangeRate`, `MaxRangeRate` และ Risk Modifier ระบบขยายช่วงราคาเมื่อพบข้อมูลเสริมไม่ครบ วัสดุ Custom, Provisional Rate, Measurement Confidence ต่ำ งานซับซ้อน หรือ Rate ใกล้หมดอายุ

ผู้ใช้ทั่วไปลดช่วงให้แคบกว่าที่ระบบคำนวณไม่ได้ Override ต้องมี Permission เหตุผล และ Review

## Share Decision

| ผล | เงื่อนไขหลัก | สิ่งที่ทำได้ |
| --- | --- | --- |
| `Blocked` | ข้อมูล/หลักฐานบังคับไม่ครบ, Unit/Rate ผิด, Template ใช้ไม่ได้, ราคาไม่ถูกต้อง, ไม่มีผู้รับ หรือไม่มีสิทธิ์ | เก็บ Draft และแก้ข้อมูล; ห้าม Share/Convert |
| `PendingReview` | Custom/Provisional Rate, Manual Override, ช่วงกว้าง, เกิน Authority, Risk Trigger หรือ Template อยู่ใน Calibration | ส่ง Reviewer ตาม Maker–Checker |
| `Shareable` | Active Template, ข้อมูลครบ, อยู่ใน Direct-share Limit, ไม่มี Risk Trigger และมีสิทธิ์ | สร้างและส่ง Preliminary Summary |

Permission ไม่สามารถข้าม Share Policy ได้ รายละเอียดผู้ตรวจและเหตุผลอยู่ที่ [Approval Matrix](approval-matrix.md)

Permission Key และ Scope ที่เป็น Contract อยู่ที่ [Permission Catalog](../03-contracts/permission-catalog.md)

## Pricing Template Lifecycle

```text
Draft → Calibration → Active → Superseded
                         └────→ Disabled
```

| สถานะ | ใช้เมื่อ |
| --- | --- |
| `Draft` | กำลังจัดกฎและยังใช้ประเมินไม่ได้ |
| `Calibration` | ทดลองกับกรณีควบคุมหรือ Pilot; ทุกการแชร์ต้อง Review |
| `Active` | ใช้คำนวณและแชร์ตาม Risk-based Policy |
| `Superseded` | มี Version ใหม่แทน แต่ Snapshot เก่ายังอ่านได้ |
| `Disabled` | หยุดใช้ทันที; ห้ามคำนวณหรือแชร์ใหม่ |

Template Version ต้องเก็บ Work Type, Measurement Rule, Unit, Rate Source, Factor, Add-on, Minimum Charge, Range, Rounding, Required Field/Evidence, Assumption, Exclusion, Validity, Effective Period, Creator และ Approver

เมื่อ Rate, Formula, Unit หรือ Policy ที่มีผลต่อราคาเปลี่ยน ต้องสร้าง Version ใหม่ Organization เป็นเจ้าของ Standard Rate และ Branch Override ได้เมื่อมี Permission, Reason, Effective Period และ Approval

## Validity, Rounding, Tax และ Discount

- `validUntil` กำหนดตาม Template และไม่เกินวันหมดอายุที่สั้นที่สุดของ Template/Rate ที่ใช้
- Customer-facing Range ปัดออกด้านนอก: Lower ปัดลงและ Upper ปัดขึ้นตาม `RoundingStep`
- เก็บ Net, Tax และ Gross แยกกัน พร้อม Tax Rate/Effective Date ใน Snapshot
- Organization กำหนด Tax Display Policy และ Customer Summary ต้องระบุชัดว่ารวมหรือยังไม่รวม VAT
- Quick Estimate ไม่เปิด Free-form Discount เป็นค่าเริ่มต้น
- Promotion ใช้ได้เมื่อ Published และอยู่ใน Effective Period; Manual Discount ต้อง Review

ตัวอย่างการปัด:

```text
Raw Range: 82,340–96,720 บาท
Rounding Step: 1,000 บาท
Displayed Range: 82,000–97,000 บาท
```

## Snapshot และ Audit

Calculation Snapshot เก็บ Quick Estimate Version, Template/Rate Version, Input/Unit, Billable Quantity, Grade/Complexity Factor, Risk Modifier, Add-on, Minimum Charge, Promotion, Tax, Rounding และผลก่อน/หลังปัด

Audit เก็บ User, Organization, Branch, Action, occurredAtUtc, Trace ID และ Manual Override พร้อมค่าก่อน/หลังและเหตุผล Snapshot ตอบว่า “คำนวณจากอะไร” ส่วน Audit ตอบว่า “ใครทำอะไรเมื่อใด”

กรณี Unit, Rate หรือ Template ใช้ไม่ได้ต้องคืน Stable Error Code ตาม [Error Contract](../03-contracts/error-contract.md)

## Calibration และ Pilot

```text
Template Calibration → Limited Pilot → Active
```

1. Calibration ใช้ข้อมูลสังเคราะห์/กรณีควบคุมและให้ผู้ประเมินตรวจทุกผลลัพธ์
2. Limited Pilot ใช้งานจริงกลุ่มเล็ก ทุกการแชร์ยังต้อง Review และเปรียบเทียบกับ Official Estimate/Quotation
3. Active อนุญาตให้งานความเสี่ยงต่ำแชร์ได้ทันที แต่งานเสี่ยงยังใช้ Maker–Checker

หากราคาทางการหลุดช่วง ต้องบันทึก Root Cause เช่น Measurement, Rate, Grade, Complexity, Scope Change หรือ Site Condition

## ค่าที่ต้องอนุมัติจาก Pilot

ค่าเหล่านี้เป็น Configurable Business Values ไม่ใช่ค่าคงที่ใน Code:

- Reference Rate และรอบทบทวนต่อ Work Type/Branch
- Grade/Complexity Factor, Add-on และ Minimum Charge
- Base/Max Range, Risk Modifier และ Rounding Step
- Validity Period และ Rate Expiry Rule
- Direct-share Authority Limit, Reviewer Routing, SLA และ Escalation
- Calibration Exit Criteria, Accuracy Target และ Out-of-range Tolerance

ทุกค่าต้องมี Owner, Effective Period, Approval และ Version ก่อน Production
