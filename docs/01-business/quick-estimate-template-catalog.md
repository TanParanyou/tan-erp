# Quick Estimate Template Catalog (รายการแม่แบบและช่องข้อมูล)

**สถานะ:** Accepted Direction — Field/Rule ใช้เป็น Baseline สำหรับ Calibration; ราคาและ Threshold จริงต้องผ่าน Pilot ก่อน Production

## เป้าหมาย

เอกสารนี้เป็นแหล่งอ้างอิงหลักของ Pricing Template, Field, Measurement Rule, ตัวอย่างคำนวณ และ Business Test Case สำหรับ Quick Estimate งาน Built-in, ผ้าม่าน และ Wallpaper

อ่านสูตรกลางและ Lifecycle ที่ [Quick Estimate Pricing Rules](quick-estimate-pricing-rules.md) เอกสารนี้กำหนด “ต้องกรอกอะไรและคาดหวังผลอย่างไร” โดยไม่กำหนดราคาจริงของบริษัท

> **ข้อควรระวัง:** ตัวเลขในหัวข้อ Example Fixtures เป็น `TEST_ONLY` สำหรับพิสูจน์สูตร ห้ามนำไปประเมินหรือเสนอราคาลูกค้า

## Template Set สำหรับ Release แรก

| Template Code | ชื่อ | Pricing Basis | สถานะเริ่มต้น |
| --- | --- | --- | --- |
| `QE-BI-WARDROBE-LM` | ตู้เสื้อผ้า Built-in | Linear Meter | Calibration |
| `QE-BI-CABINET-LM` | ตู้ล่าง/ตู้แขวน/ตู้เก็บของ | Linear Meter + Variant Rate Set | Calibration |
| `QE-CT-FABRIC-DROP` | ผ้าม่านผ้า | Fabric Length + Track + Installation | Calibration |
| `QE-CT-BLIND-AREA` | ม่านม้วน/มู่ลี่ | Opening Area + Minimum Area | Calibration |
| `QE-WP-ROLL` | Wallpaper แบบม้วน | Required Roll + Labor Area | Calibration |

ไม่ใช้ Template เดียวครอบคลุมทุก Work Type เพราะ Unit, Waste และวิธีปัดปริมาณต่างกัน หากพบงานนอก Template ให้เก็บ Draft/Request New Template ไม่อนุญาตให้ดัดแปลงสูตรหน้างานเอง

## ระดับการบังคับกรอก

| Required At | ความหมาย |
| --- | --- |
| `Draft` | ต้องมีเพื่อสร้างและ Autosave Record |
| `Calculate` | ต้องครบก่อน Server คำนวณ Price Range |
| `Share` | ต้องครบก่อนประเมิน Share Policy และสร้าง Preliminary Summary |
| `Optional` | เก็บเมื่อมีข้อมูล; อาจเพิ่มความแม่นยำหรือเปลี่ยน Risk |
| `Derived` | Server/Template คำนวณหรือเติมให้ ผู้ใช้ทั่วไปแก้ไม่ได้ |

## Common Fields (ช่องร่วมทุก Template)

| Field Key | ชื่อไทย | Type | Required At | Source/Validation | ลูกค้าเห็น |
| --- | --- | --- | --- | --- | --- |
| `customerId` | ลูกค้า | ID | Draft | ต้องอยู่ใน Organization Scope | ชื่อที่อนุญาต |
| `opportunityId` | โอกาสการขาย | ID | Draft | ต้องเป็นของ Customer/Branch เดียวกัน | ไม่แสดง ID |
| `contactId` | ผู้ติดต่อ | ID | Share | ต้องมีช่องทางผู้รับที่ตรวจสอบได้ | ชื่อ/ช่องทางตาม Consent |
| `siteSurveyRevisionId` | ฉบับข้อมูลสำรวจหน้างาน | ID | Optional | อ้าง Ready/Superseded Revision ที่อยู่ใน Scope | ไม่แสดง ID |
| `propertyType` | ประเภทสถานที่ | Enum | Calculate | ค่าจาก Catalog เช่น House/Condo/Office | แสดงชื่อ |
| `roomOrArea` | ห้องหรือพื้นที่ | Text | Calculate | 1–120 ตัวอักษรหลัง Trim | แสดง |
| `templateVersionId` | รุ่นแม่แบบ | ID | Calculate | ต้องเป็น Calibration/Active และ Effective | แสดง Reference เท่านั้น |
| `materialGradeId` | ระดับวัสดุ | ID | Calculate | ต้องเป็น Option ของ Template Version | แสดงชื่อ |
| `measurementConfidence` | ความมั่นใจในการวัด | Enum | Calculate | `high`, `medium`, `low`; มีผลต่อ Risk | แสดงคำอธิบายง่าย |
| `assumptions` | สมมติฐาน | Text list | Share | ใช้ Default จาก Template และยืนยันได้ | แสดง |
| `exclusions` | สิ่งที่ไม่รวม | Text list | Share | ต้องไม่ว่าง; ใช้ Default จาก Template ได้ | แสดง |
| `photoIds` | รูปหน้างาน | ID list | Share | จำนวน/ประเภทตาม Evidence Checklist | แสดงเฉพาะรูปที่อนุญาต |
| `fieldNote` | หมายเหตุหน้างาน | Text | Optional | ห้ามมี Secret; เลือก Customer-visible แยก | ตาม Flag |
| `internalNote` | หมายเหตุภายใน | Text | Optional | Internal-only | ไม่แสดง |
| `complexityAnswers` | Checklist ความซับซ้อน | Answer list | Calculate | ต้องตอบข้อบังคับของ Template | ไม่แสดงคะแนนภายใน |
| `currency` | สกุลเงิน | Code | Derived | จาก Organization/Template | แสดง |
| `taxDisplayPolicy` | วิธีแสดงภาษี | Enum | Derived | จาก Organization; ผู้ประเมินเปลี่ยนไม่ได้ | แสดงรวม/ไม่รวม |
| `validUntil` | วันหมดอายุ | Date | Derived | ไม่เกิน Template/Rate Expiry ที่สั้นที่สุด | แสดง |

## Evidence Checklist กลาง

ก่อน Share ทุก Template ต้องมี:

- รูปภาพรวมพื้นที่อย่างน้อยตามจำนวนที่ Template กำหนด
- รูปจุดติดตั้งหรือพื้นผิวที่มีผลต่อราคา
- Measurement พร้อม Unit และผู้วัด
- Material Grade และ Complexity Checklist
- Assumptions และ Exclusions ที่ยืนยันแล้ว

Upload ล้มเหลวเก็บ Draft ได้ แต่ Share/Convert ไม่ได้จน Server ยืนยัน Evidence ครบ

## Built-in Templates

### `QE-BI-WARDROBE-LM`

| Field Key | ชื่อไทย | Type/Unit | Required At | Rule |
| --- | --- | --- | --- | --- |
| `workSubtype` | ชนิดชิ้นงาน | Enum | Calculate | `wardrobe`; Template อื่นห้ามใช้ Code นี้ |
| `widthM` | ความกว้างรวม | Decimal/m | Calculate | มากกว่า 0 และอยู่ใน Template Bounds |
| `heightM` | ความสูง | Decimal/m | Calculate | มากกว่า 0; เลือก Height Band อัตโนมัติ |
| `depthM` | ความลึก | Decimal/m | Calculate | มากกว่า 0; เลือก Depth Band อัตโนมัติ |
| `quantity` | จำนวนชุด | Integer | Calculate | มากกว่า 0 |
| `doorTypeId` | รูปแบบบาน | ID | Calculate | Option ของ Template Version |
| `doorCount` | จำนวนบาน | Integer | Optional | ตั้งแต่ 0 ขึ้นไป |
| `drawerCount` | จำนวนลิ้นชัก | Integer | Optional | ตั้งแต่ 0 ขึ้นไป; สร้าง Add-on ตาม Rule |
| `hardwarePackageId` | ชุดอุปกรณ์ | ID | Calculate | Standard/Premium/Custom Rate Set |
| `demolitionRequired` | ต้องรื้อของเดิม | Boolean | Calculate | เพิ่ม Add-on/Risk ตาม Policy |
| `electricalIntegration` | มีงานไฟซ่อน | Boolean | Calculate | เพิ่ม Complexity/Risk |
| `irregularShape` | งานโค้ง/เข้ามุมพิเศษ | Boolean | Calculate | ต้องมีรูปและ PendingReview ตาม Policy |

```text
Billable Linear Meter = widthM × quantity
Base Amount = Billable Linear Meter × Wardrobe Reference Rate
Adjusted Amount = Base Amount × Grade Factor × Complexity Factor
                  + Drawer/Door/Hardware/Demolition Add-ons
```

Height/Depth ไม่ถูกนำไปคูณตรง ๆ ซ้ำกับ Linear Meter แต่เลือก Band Factor จาก Template เพื่อป้องกันการคิดราคาเกินซ้อนกัน

Evidence ก่อน Share: รูปด้านหน้าชิ้นงาน/ผนังเต็มพื้นที่ 1 รูป, รูปพื้น–ผนัง–ฝ้าบริเวณติดตั้ง 1 รูป และรูปจุดไฟ/ประปาเมื่อเลือกงานระบบ

### `QE-BI-CABINET-LM`

ใช้ Field ร่วมกับ Wardrobe และเพิ่ม `cabinetVariant` ได้แก่ `base`, `wall`, `tall` แต่ละ Variant ต้องมี Rate Set และ Height/Depth Band ของตน ห้ามใช้ Rate ของตู้ล่างกับตู้แขวนโดยอัตโนมัติ

## Curtain Templates

### `QE-CT-FABRIC-DROP`

แต่ละหน้าต่างเป็น Measurement Line แยกเพื่อให้แก้เฉพาะจุดได้

| Field Key | ชื่อไทย | Type/Unit | Required At | Rule |
| --- | --- | --- | --- | --- |
| `openingWidthM` | ความกว้างช่องเปิด | Decimal/m | Calculate | มากกว่า 0 |
| `openingHeightM` | ความสูงช่องเปิด | Decimal/m | Calculate | มากกว่า 0 |
| `openingCount` | จำนวนช่องเหมือนกัน | Integer | Calculate | มากกว่า 0 |
| `curtainStyleId` | รูปแบบม่าน | ID | Calculate | เช่น Pleated/Eyelet ตาม Template |
| `fullnessRatio` | อัตราจีบ | Preset Decimal | Derived | มาจาก Curtain Style; Override ต้อง Review |
| `fabricRateSetId` | ชุดราคาผ้า | ID | Calculate | Effective Rate ต่อเมตรผ้า |
| `usableFabricWidthM` | หน้าผ้าที่ใช้ได้ | Decimal/m | Derived | จาก Fabric Rate Set |
| `topBottomAllowanceM` | ค่าเผื่อหัว/ชาย | Decimal/m | Derived | จาก Template |
| `patternRepeatM` | ระยะต่อลาย | Decimal/m | Optional | ตั้งแต่ 0; ใช้ปัด Cut Drop ขึ้น |
| `liningOptionId` | ซับใน | ID | Optional | เพิ่ม Fabric/Rate ตาม Option |
| `trackTypeId` | ชนิดราง | ID | Calculate | Rate ต่อเมตรและ Minimum Charge ตาม Template |
| `installationHeightBand` | ระดับความสูงติดตั้ง | Enum | Calculate | ระบบเสนอจาก Height; ผู้ใช้ยืนยัน |
| `motorized` | ระบบมอเตอร์ | Boolean | Calculate | ต้องมี Model/Provisional Rate และ Review |

```text
Finished Width = openingWidthM × fullnessRatio
Panel Count per Opening = ceil(Finished Width ÷ usableFabricWidthM)
Raw Cut Drop = openingHeightM + topBottomAllowanceM
Cut Drop = patternRepeatM > 0
           ? ceil(Raw Cut Drop ÷ patternRepeatM) × patternRepeatM
           : Raw Cut Drop
Billable Fabric Length = Panel Count × Cut Drop × openingCount
Track Length = openingWidthM × openingCount
```

Evidence ก่อน Share: รูปช่องเปิดเต็มบาน 1 รูป, รูปหัวช่อง/จุดยึดราง 1 รูป และรูปด้านข้างที่เห็นสิ่งกีดขวาง 1 รูป

### `QE-CT-BLIND-AREA`

| Field Key | ชื่อไทย | Type/Unit | Required At | Rule |
| --- | --- | --- | --- | --- |
| `openingWidthM` | ความกว้างช่องเปิด | Decimal/m | Calculate | มากกว่า 0 |
| `openingHeightM` | ความสูงช่องเปิด | Decimal/m | Calculate | มากกว่า 0 |
| `openingCount` | จำนวนช่องเหมือนกัน | Integer | Calculate | มากกว่า 0 |
| `blindTypeId` | ชนิดม่าน | ID | Calculate | Roller/Venetian/Roman ตาม Catalog |
| `minimumAreaPerOpeningSqM` | พื้นที่ขั้นต่ำต่อช่อง | Decimal/m² | Derived | จาก Template Version |
| `mechanismOptionId` | ชุดกลไก | ID | Calculate | Manual/Motorized; Motorized ต้อง Review |

```text
Billable Area per Opening = max(openingWidthM × openingHeightM,
                               minimumAreaPerOpeningSqM)
Total Billable Area = Billable Area per Opening × openingCount
```

## Wallpaper Template

### `QE-WP-ROLL`

| Field Key | ชื่อไทย | Type/Unit | Required At | Rule |
| --- | --- | --- | --- | --- |
| `grossWallAreaSqM` | พื้นที่ผนังรวม | Decimal/m² | Calculate | กรอกจาก Width×Height หลาย Wall หรือยอดรวมอย่างใดอย่างหนึ่ง |
| `openingAreaSqM` | พื้นที่ช่องเปิด | Decimal/m² | Optional | 0 ถึง Gross Area; หักได้ตาม Template Policy |
| `usableCoveragePerRollSqM` | พื้นที่ใช้งานต่อม้วน | Decimal/m² | Derived | จาก Product/Rate Set หลัง Pattern Allowance |
| `wasteRate` | ค่าเผื่อสูญเสีย | Decimal ratio | Derived | จาก Material/Pattern; Override ต้อง Review |
| `surfaceCondition` | สภาพผนัง | Enum | Calculate | `ready`, `minor-repair`, `major-repair`, `unknown` |
| `removeExisting` | รื้อวัสดุเดิม | Boolean | Calculate | เพิ่ม Labor Add-on และ Risk |
| `patternAlignment` | ต้องต่อลาย | Boolean | Calculate | เลือก Coverage/Waste Rule ที่เหมาะสม |
| `laborRateSetId` | ชุดค่าแรงติดตั้ง | ID | Calculate | Effective Rate ต่อ m² หรือ Minimum Charge |

```text
Net Wall Area = max(0, grossWallAreaSqM - allowedOpeningAreaSqM)
Required Coverage = Net Wall Area × (1 + wasteRate)
Required Rolls = ceil(Required Coverage ÷ usableCoveragePerRollSqM)
Material Amount = Required Rolls × Reference Rate per Roll
Labor Amount = Net Wall Area × Labor Reference Rate
```

Surface Condition `major-repair` หรือ `unknown` ต้องไม่รวมค่าซ่อมจากการเดา ให้เพิ่ม Exclusion หรือ Provisional Add-on และส่ง Review

Evidence ก่อน Share: รูปผนังเต็มพื้นที่ 1 รูป, รูประยะใกล้ของพื้นผิว 1 รูป และรูปความชื้น/รอยแตกร้าวเมื่อพบความเสียหาย

## Complexity Checklist

| Risk Code | คำถาม | ผลเริ่มต้น |
| --- | --- | --- |
| `ACCESS_DIFFICULT` | ทางเข้า ขนส่ง หรือติดตั้งเข้าถึงยากหรือไม่ | Complexity/Risk เพิ่ม |
| `DEMOLITION` | ต้องรื้อของเดิมหรือไม่ | Add-on + Risk |
| `IRREGULAR_SHAPE` | มีงานโค้ง เข้ามุม หรือรูปทรงพิเศษหรือไม่ | Complexity + PendingReview |
| `HIDDEN_SYSTEM` | มีไฟ/ประปา/ระบบซ่อนที่เกี่ยวข้องหรือไม่ | Risk + PendingReview ตาม Policy |
| `HEIGHT_WORK` | ต้องใช้อุปกรณ์ทำงานที่สูงหรือไม่ | Add-on/Complexity |
| `RESTRICTED_HOURS` | อาคารจำกัดเวลาทำงานหรือไม่ | Add-on/Risk |
| `SURFACE_UNCERTAIN` | สภาพพื้นผิวไม่พร้อมหรือยังตรวจไม่ได้หรือไม่ | Risk + PendingReview |
| `CUSTOM_MATERIAL` | ใช้วัสดุ Custom/Provisional Rate หรือไม่ | PendingReview หรือ Blocked หากไม่มี Rate |

Template เป็นผู้ Map Answer ไปยัง Factor/Add-on/Risk Modifier ผู้ใช้เห็นคำถามและผลกระทบเชิงคำอธิบาย แต่ไม่แก้ Mapping ได้

## TEST_ONLY Example Fixtures

ตัวอย่างทั้งหมดใช้ `taxDisplayPolicy = EXCLUSIVE`, Tax ไม่ถูกรวมในตัวเลข และไม่มี Promotion

### EX-BI-001: Wardrobe Premium

```text
Input:
  widthM = 3.0, quantity = 1
  Reference Rate = 12,000 TEST_ONLY THB/m
  Grade Factor = 1.25
  Complexity Factor = 1.10
  Drawer Add-on = 1,500 TEST_ONLY THB
  Minimum Charge = 20,000 TEST_ONLY THB
  Base Range = 15%, Rounding Step = 1,000 THB

Calculation:
  Billable Quantity = 3.0 m
  Base Amount = 3.0 × 12,000 = 36,000
  Adjusted Amount = 36,000 × 1.25 × 1.10 + 1,500 = 51,000
  Raw Range = 43,350–58,650
  Displayed Range = 43,000–59,000 TEST_ONLY THB

Expected Share Decision: Shareable เมื่อ Template Active และ Evidence ครบ
```

### EX-CT-001: Fabric Curtain

```text
Input:
  openingWidthM = 2.4, openingHeightM = 2.6, openingCount = 2
  fullnessRatio = 2.0, usableFabricWidthM = 1.4
  topBottomAllowanceM = 0.3, patternRepeatM = 0
  Fabric Rate = 500 TEST_ONLY THB/m
  Track Rate = 300 TEST_ONLY THB/m
  Installation = 1,000 TEST_ONLY THB/opening
  Base Range = 12%, Rounding Step = 500 THB

Calculation:
  Panel Count = ceil((2.4 × 2.0) ÷ 1.4) = 4 panels/opening
  Cut Drop = 2.6 + 0.3 = 2.9 m
  Billable Fabric Length = 4 × 2.9 × 2 = 23.2 m
  Fabric Amount = 23.2 × 500 = 11,600
  Track Amount = (2.4 × 2) × 300 = 1,440
  Installation = 2 × 1,000 = 2,000
  Adjusted Amount = 15,040
  Raw Range = 13,235.20–16,844.80
  Displayed Range = 13,000–17,000 TEST_ONLY THB

Expected Share Decision: Shareable เมื่อ Template Active และ Evidence ครบ
```

### EX-WP-001: Wallpaper with Surface Risk

```text
Input:
  grossWallAreaSqM = 22.4, openingAreaSqM = 2.0
  wasteRate = 10%, usableCoveragePerRollSqM = 5.3
  Roll Rate = 1,500 TEST_ONLY THB/roll
  Labor Rate = 200 TEST_ONLY THB/m²
  Complexity Factor = 1.10
  Base Range = 12%, SURFACE_UNCERTAIN Risk = 5%
  Rounding Step = 500 THB

Calculation:
  Net Wall Area = 22.4 - 2.0 = 20.4 m²
  Required Coverage = 20.4 × 1.10 = 22.44 m²
  Required Rolls = ceil(22.44 ÷ 5.3) = 5 rolls
  Base Amount = (5 × 1,500) + (20.4 × 200) = 11,580
  Adjusted Amount = 11,580 × 1.10 = 12,738
  Risk Rate = 12% + 5% = 17%
  Raw Range = 10,572.54–14,903.46
  Displayed Range = 10,500–15,000 TEST_ONLY THB

Expected Share Decision: PendingReview เพราะ SURFACE_UNCERTAIN
```

## Business Test Cases

| Test ID | Given / When | Expected Result |
| --- | --- | --- |
| `TC-QE-001` | ใช้ EX-BI-001 กับ Active Template และ Evidence ครบ | ราคา 43,000–59,000 TEST_ONLY THB และ `Shareable` |
| `TC-QE-002` | ใช้ EX-CT-001 | Panel=4, Fabric=23.2m, ราคา 13,000–17,000 TEST_ONLY THB |
| `TC-QE-003` | ใช้ EX-WP-001 | Rolls=5, ราคา 10,500–15,000 TEST_ONLY THB และ `PendingReview` |
| `TC-QE-004` | Quantity/Width เป็น 0 หรือติดลบ | `Blocked` พร้อม Field Error; ไม่ล้างข้อมูลอื่น |
| `TC-QE-005` | Unit ของ Input ไม่ตรงกับ Rate | `Blocked`, `QUICK_ESTIMATE_UNIT_MISMATCH` |
| `TC-QE-006` | ไม่มี Effective Reference Rate | `Blocked`, `QUICK_ESTIMATE_RATE_UNAVAILABLE` |
| `TC-QE-007` | Template เป็น Draft/Superseded/Disabled | `Blocked`, `PRICING_TEMPLATE_NOT_USABLE` |
| `TC-QE-008` | Template เป็น Calibration และข้อมูลครบ | คำนวณได้แต่เป็น `PendingReview` |
| `TC-QE-009` | Active Template, Standard Material, ไม่มี Risk/Override และอยู่ใน Authority | `Shareable` |
| `TC-QE-010` | Custom Material ไม่มี Provisional Rate | `Blocked`, `QUICK_ESTIMATE_RATE_UNAVAILABLE` |
| `TC-QE-011` | Custom Material มี Provisional Rate ที่อนุมัติ | คำนวณได้และ `PendingReview` |
| `TC-QE-012` | ผู้ใช้ Override Rate/Factor/Range | บังคับ Reason, เก็บ Before/After และ `PendingReview` |
| `TC-QE-013` | Risk รวมมากกว่า Max Range | ใช้ Max Range และเก็บ Applied/Capped Modifiers ใน Snapshot |
| `TC-QE-014` | Adjusted Amount ต่ำกว่า Minimum Charge | ใช้ Minimum Charge ก่อนคำนวณ Range |
| `TC-QE-015` | Raw Lower/Upper ไม่ตรง Rounding Step | Lower ปัดลงและ Upper ปัดขึ้น |
| `TC-QE-016` | Upper Bound เกิน Direct-share Authority | `PendingReview`, `QUICK_ESTIMATE_REVIEW_REQUIRED` |
| `TC-QE-017` | Required Photo/Assumption/Exclusion ไม่ครบ | `Blocked`, `QUICK_ESTIMATE_INCOMPLETE` ระบุ Field |
| `TC-QE-018` | Maker พยายาม Review งานตนเองเมื่อ Maker–Checker มีผล | ปฏิเสธ 403 และไม่เปลี่ยนสถานะ |
| `TC-QE-019` | Branch A ใช้ Rate Override ของ Branch B | ปฏิเสธ 404 เพราะ Resource อยู่นอก Scope |
| `TC-QE-020` | Template/Rate ปัจจุบันเปลี่ยนหลังสร้าง Version | คำนวณจาก Snapshot เดิมแล้วได้ผลเดิม |
| `TC-QE-021` | แชร์ Version เดิมซ้ำหลัง Delivery Error | Retry การส่งเดิม ไม่สร้าง Version ใหม่ |
| `TC-QE-022` | Convert ซ้ำด้วย Idempotency Key เดิม | คืน Official Estimate Draft เดิม |
| `TC-QE-023` | เปลี่ยน Measurement/Grade/Assumption หลังแชร์ | สร้าง Quick Estimate Version ใหม่ |
| `TC-QE-024` | Tax Display Policy เป็น Inclusive/Exclusive | เก็บ Net/Tax/Gross แยกและแสดง Disclaimer ตรง Policy |

## เกณฑ์พร้อมเข้า Limited Pilot

- Template ทั้ง 5 ชุดผ่าน Test Case ที่เกี่ยวข้องทั้งหมด
- ผลตัวอย่างคำนวณได้รับการตรวจมือจาก Pricing Template Owner และ Approver
- Field/Unit, Default Assumption, Exclusion และ Evidence Checklist ได้รับอนุมัติ
- Rate/Factor/Range/Validity เป็นค่าของ Pilot ที่มี Owner, Effective Period และ Approval
- Permission/Scope และ Maker–Checker ผ่าน Security Test
- Customer Summary ระบุ TEST/PILOT ชัดเจนและยังต้อง Review ทุกฉบับ
