# Estimation Calculation Rules (กฎคำนวณราคา)

**สถานะ:** Accepted Direction — สูตรและ Precision เป็น Production Baseline ส่วนค่า Rate/Threshold จริงต้องผ่าน Business Owner และ Finance Sign-off

เอกสารนี้เป็นกฎคำนวณหลักของ **Official Estimate** รายการ Field อยู่ที่ [Official Estimate Field Catalog](official-estimate-field-catalog.md) และไม่ใช้กฎ Quick Estimate มาคำนวณราคาทางการ

## Policy Model

Calculation Policy ต้องมี `policyCode`, `version`, Organization/Branch Scope, Effective Period, Status และผู้อนุมัติ Published Version แก้ย้อนหลังไม่ได้ การ Calculate ต้อง Resolve Policy ตาม Branch + Estimate Date แล้วบันทึก Version ใน Calculation Snapshot

Lifecycle ใช้ `draft → submitted → approved → published → superseded/disabled` และบังคับ Maker–Checker ก่อน Published เช่นเดียวกับ Approval Policy

หาก Resolve Policy ไม่ได้ ระบบคืน `ESTIMATE_POLICY_UNAVAILABLE` และห้าม Submit แบบ Fail-closed

## ลำดับคำนวณ

```text
Component Cost = Component Quantity × Unit Cost
Work Item Direct Cost = Σ Component Cost
Revision Direct Cost = Σ Work Item Direct Cost
Allocated Overhead = Apply Overhead Policy to Revision Direct Cost
Work Item Overhead = Allocate Overhead by Work Item Direct Cost proportion
Work Item Total Cost = Work Item Direct Cost + Work Item Overhead
Work Item Selling Price = Apply Work Item Pricing Method to Work Item Total Cost
Total Cost = Σ Work Item Total Cost
Selling Before Discount = Σ Work Item Selling Price
Discount Amount = Apply Document Discount
Net Before Tax = Selling Before Discount - Discount Amount
Tax Amount = Apply Tax Policy to Net Before Tax
Grand Total = Net Before Tax + Tax Amount
Margin Amount = Net Before Tax - Total Cost
Margin Rate = Margin Amount ÷ Net Before Tax
Markup Rate = Margin Amount ÷ Total Cost
```

เมื่อฐานหารเป็นศูนย์ ระบบคืน Rate เป็น `null` พร้อม Reason Code ไม่คืน Infinity/NaN

## Pricing Method

Pricing Method อยู่ระดับ Work Item เพื่อรองรับหมวดงานที่ใช้กฎต่างกันใน Revision เดียว Policy เป็นผู้กำหนด Default และ Allowlist ต่อ Work Type

### Margin

```text
Selling Price = Work Item Total Cost ÷ (1 - Target Margin Rate)
```

Target Margin ต้องอยู่ระหว่าง `0` และน้อยกว่า `1` ค่า `1` หรือมากกว่าต้อง Reject

### Markup

```text
Selling Price = Work Item Total Cost × (1 + Markup Rate)
```

Markup Rate ต้องไม่ติดลบ เว้นแต่ Published Policy รองรับกรณีพิเศษพร้อม Approval Trigger

### Fixed Price

Fixed Price ใช้ได้เฉพาะ Permission `estimates.override-price`, บังคับเหตุผล และ Trigger Approval ระบบยังต้องคำนวณ Margin/Markup จริงจาก Fixed Price เพื่อให้ผู้อนุมัติเห็นความเสี่ยง

ตัวอย่าง `TEST_ONLY`: ต้นทุน `100.00` และ Margin `25%` ให้ Selling Price `133.33`; Markup `25%` ให้ `125.00` คำสองคำนี้ห้ามใช้แทนกัน

## Overhead

Phase 1 รองรับสองวิธี:

| Method | สูตร | Rule |
| --- | --- | --- |
| `percent-direct-cost` | Direct Cost × Rate | Rate ≥0 |
| `fixed-amount` | Amount | Amount ≥0 และ Currency ตรง Revision |

หนึ่ง Revision ใช้ Overhead Method เดียวจาก Calculation Policy แล้วจัดสรรเข้า Work Item ตามสัดส่วน Direct Cost โดยให้ Work Item สุดท้ายรับ Rounding Remainder เพื่อให้ผลรวมตรง Document เสมอ หาก Direct Cost รวมเป็นศูนย์แต่มี Fixed Overhead ให้ Block และขอวิธีจัดสรรที่ Policy รองรับ การ Override ต้องมีสิทธิ์ เหตุผล และบันทึกก่อน/หลัง

## Discount

Phase 1 ใช้ Discount ระดับ Document หลังคำนวณ Selling Before Discount และก่อน Tax:

- `none`: Discount = 0
- `percent`: Selling Before Discount × Rate โดย Rate อยู่ระหว่าง 0–1
- `fixed-amount`: จำนวนเงินไม่เกิน Selling Before Discount

Discount >0 บังคับ Reason Code และอาจ Trigger Approval ตาม Policy Line/Section Discount เป็น Future Capability เพื่อหลีกเลี่ยงการแจกจ่ายเศษปัดซับซ้อนใน Release แรก

## Tax

Tax Rate และ Tax Display Mode มาจาก Versioned Tax Policy ไม่ Hard-code ใน Source Code ค่าอัตราที่ปรากฏใน UAT เป็น `TEST_ONLY` จนฝ่ายบัญชียืนยัน

- `exclusive`: Tax = Net Before Tax × Rate; Grand Total = Net Before Tax + Tax
- `inclusive`: แยก Tax จากยอดรวมตาม Policy เพื่อแสดงฐานภาษีอย่างตรวจสอบได้
- `exempt`: Tax = 0 และต้องมี Tax Code/เหตุผลตาม Policy

Withholding Tax, Compound Tax และ Accounting Posting ไม่อยู่ใน Estimation Phase 1

## Precision and Rounding

| Value | Storage/Calculation Precision | Display |
| --- | --- | --- |
| Quantity | Decimal(18,4) | สูงสุด 4 ตำแหน่ง ตัดศูนย์ท้ายได้ |
| Unit Cost/Rate | Decimal(19,4) | ตามสิทธิ์และ Locale |
| Margin/Markup/Tax Rate | Decimal(12,6) | Percent ตาม UI Policy |
| Money | Decimal(19,2) | 2 ตำแหน่ง + Currency |

กฎปัด Phase 1:

1. คูณ Quantity × Unit Cost ด้วย Decimal Precision เต็ม
2. ปัด Component Cost เป็น 2 ตำแหน่งด้วย Midpoint Away From Zero แล้วรวมเป็น Work Item Direct Cost
3. คำนวณ/จัดสรร Overhead ด้วย Precision เต็ม ปัดส่วนแบ่ง Work Item และให้รายการสุดท้ายรับ Rounding Remainder
4. คำนวณ Work Item Selling Price ด้วยกฎของรายการ ปัด 2 ตำแหน่ง แล้วรวมเป็น Section/Revision
5. คำนวณ Document Discount และปัด 2 ตำแหน่ง
6. คำนวณ Tax ที่ระดับ Document และปัด 2 ตำแหน่ง
7. Grand Total ต้องเท่ากับ Net Before Tax + Tax ที่แสดง

ห้ามใช้ Binary Floating Point กับ Quantity, Rate หรือ Money

## Readiness Result

Calculate คืนผลหนึ่งค่า:

| Result | ความหมาย | Submit |
| --- | --- | --- |
| `blocked` | Field/Unit/Cost/Policy ไม่ครบหรือผิด | ไม่ได้ |
| `requiresAttention` | คำนวณได้แต่มี Provisional, Stale, Override หรือ Risk Trigger | ได้เมื่อ Resolve Approval Route สำเร็จ |
| `ready` | ข้อมูลครบและไม่มี Exception | ได้ |

ทุกผลต้องมี Stable Reason Code และ Field/Work Item Pointer ที่ Frontend ใช้ Focus ได้

## Calculation Snapshot

Snapshot อย่างน้อยต้องมี:

- Schema Version, Calculation Version และ Captured Time
- Revision ID/Version และ Input Hash
- Calculation/Tax Policy ID + Version + Snapshot Hash
- Cost Source Version และ Effective Time ของทุก Component
- Intermediate ก่อนปัด, Rounded Boundary และ Final Totals
- Pricing Method, Overhead, Discount, Tax และ Approval Reason Codes
- Actor/Request/Trace ID

Financial Input เปลี่ยนเมื่อใดต้องตั้ง `calculationOutdated=true` และ Submit ไม่ได้จน Calculate ใหม่

## Override

Override Unit Cost, Selling Rule, Fixed Price, Overhead, Discount หรือ Tax ต้องเก็บ Typed Before/After, Reason Code, Reason, Permission, Actor และ Time Override ทำให้ Calculation Outdated และอาจเพิ่ม Approval Step ห้ามแก้ Derived Total โดยตรง

## Deterministic Examples

ตัวเลขทั้งหมดเป็น `TEST_ONLY`

| Case | Input | Expected |
| --- | --- | --- |
| Margin | Cost 100.00, Margin 25% | Selling 133.33 |
| Markup | Cost 100.00, Markup 25% | Selling 125.00 |
| Discount | Selling 1,000.00, Discount 10% | Net Before Tax 900.00 |
| Rounding | Quantity 3.0000 × Unit Cost 33.3350 | Component/Work Item 100.01 |
| Zero denominator | Cost 0.00, Selling 0.00 | Margin/Markup null + Reason Code |

## Calculation Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-CALC-EST-001` | Component Quantity × Unit Cost | ปัด Component ตาม Policy แล้วรวมถูก |
| `TC-CALC-EST-002` | Margin 25% บน Cost 100.00 `TEST_ONLY` | Selling 133.33 |
| `TC-CALC-EST-003` | Markup 25% บน Cost 100.00 `TEST_ONLY` | Selling 125.00 |
| `TC-CALC-EST-004` | Overhead จัดสรรมี Rounding Remainder | ผลรวม Work Item Overhead เท่ากับ Document Overhead |
| `TC-CALC-EST-005` | Discount ทำ Net ติดลบ | Reject Business Rule |
| `TC-CALC-EST-006` | Tax Policy Exclusive/Inclusive/Exempt | Tax Base/Amount/Display ตรง Snapshot |
| `TC-CALC-EST-007` | Margin/Markup หารด้วยศูนย์ | Rate null; ไม่มี Infinity/NaN |
| `TC-CALC-EST-008` | ไม่มี Published Calculation/Tax Policy | `ESTIMATE_POLICY_UNAVAILABLE` |
| `TC-CALC-EST-009` | Recalculate Input/Cost/Policy Version เดิม | Hash และ Result เดิม |
| `TC-CALC-EST-010` | Financial Input เปลี่ยน | Snapshot เดิมคงอยู่และผลเป็น Outdated |
| `TC-CALC-EST-011` | Fixed Price โดยไม่มี Override Permission | Reject |
| `TC-CALC-EST-012` | Client ส่ง Derived Total | ไม่ใช้ค่าจาก Client |

## Production Confirmation

ก่อน Publish Policy จริงต้องยืนยัน Default Margin/Markup ต่อ Work Type, Overhead Method/Rate, Discount Authority, Tax Policy, Cost Staleness และ Customer Rounding กับ Business Owner + Finance การไม่ยืนยันใช้ Production Bootstrap ที่บังคับ Checker ทุก Estimate
