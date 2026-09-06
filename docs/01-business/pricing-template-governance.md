# Pricing Template Governance (การควบคุมแม่แบบและราคา)

**สถานะ:** Accepted Direction — Workflow และ Control ใช้เป็นมาตรฐาน ส่วนผู้ดำรงบทบาทจริง, SLA และเกณฑ์ Pilot ต้องยืนยันก่อน Production

## เป้าหมาย

เอกสารนี้เป็นแหล่งอ้างอิงหลักสำหรับการสร้าง ตรวจ อนุมัติ ทดลอง เปิดใช้ เปลี่ยนรุ่น และหยุดใช้ Pricing Template/Reference Rate ของ Quick Estimate

ผู้อ่านหลักคือ Pricing Template Owner, Pricing Template Approver, ผู้ดูแลราคา, Auditor และทีมพัฒนา เอกสารนี้ไม่กำหนดราคาจริงของบริษัทและไม่ใช้แทนสูตรคำนวณใน [Quick Estimate Pricing Rules](quick-estimate-pricing-rules.md)

## สิ่งที่อยู่ภายใต้การควบคุม

| Object | หน้าที่ | กฎ Version |
| --- | --- | --- |
| Template Family | ตัวตนถาวรของแม่แบบ เช่น `QE-BI-WARDROBE-LM` | Code ไม่เปลี่ยนเมื่อออก Version ใหม่ |
| Template Version | Field, Measurement Rule, Formula, Factor, Risk, Evidence และ Customer Disclaimer | Published แล้วแก้เนื้อหาย้อนหลังไม่ได้ |
| Rate Set Version | Reference Rate, Unit, Material/Service Option, Source และ Effective Period | เปลี่ยน Rate/Unit/Source ต้องสร้าง Version ใหม่ |
| Branch Rate Override | ราคาเฉพาะสาขาที่ทับ Standard Rate ชั่วคราว | ต้องมีเหตุผล ช่วงเวลา และ Approval แยก |
| Calibration Record | ผล Test Fixture, Controlled Case และ Pilot Comparison | Append-only ต่อ Template Version |

Template Version และ Rate Set Version แยกจากกันเพื่อปรับราคาได้โดยไม่คัดลอกสูตรทั้งหมด แต่ Calculation Snapshot ต้องบันทึก Version ของทั้งคู่เสมอ

## Lifecycle (วงจรสถานะ)

```text
Draft ──submit──> Submitted ──approve──> Approved ──publish──> Calibration
  ↑                    │                                      │
  └──────return────────┘                                      └──activate──> Active
                                                                                 │
                                                               New Active Version ──> Superseded

Draft / Submitted / Approved / Calibration / Active ──disable──> Disabled
```

| สถานะ | ใช้ทำอะไร | ใช้คำนวณ Quick Estimate ใหม่ได้หรือไม่ |
| --- | --- | --- |
| `Draft` | Owner กำลังจัด Field, Formula, Rate และ Test Case | ไม่ได้ |
| `Submitted` | รอ Approver ตรวจ; Owner แก้ไม่ได้จนถูก Return | ไม่ได้ |
| `Returned` | แสดงผลการตรวจที่ต้องแก้ แล้วกลับเข้าสู่ Draft Work Queue | ไม่ได้ |
| `Approved` | เนื้อหาผ่าน Maker–Checker แต่ยังไม่เผยแพร่ | ไม่ได้ |
| `Calibration` | ทดสอบกรณีควบคุม/Limited Pilot; ทุกการแชร์ต้อง Review | ได้แบบควบคุม |
| `Active` | ใช้งานตาม Risk-based Share Policy | ได้ |
| `Superseded` | มี Active Version ใหม่แทน; เก็บอ่าน Snapshot เดิม | ไม่ได้สำหรับรายการใหม่ |
| `Disabled` | หยุดใช้ฉุกเฉินหรือพบปัญหาร้ายแรง | ไม่ได้ |

`Returned` เป็นผลของการตรวจ ไม่ใช่ Published State ระบบอาจจัดเก็บเป็น Status หรือ Review Outcome ได้ แต่หน้าจอต้องแสดงว่าแก้ Draft ต่อได้อย่างชัดเจน

## Separation of Duties (การแยกหน้าที่)

| Action | ผู้รับผิดชอบหลัก | ผู้ที่ห้ามทำ Action เดียวกันกับ Version ที่ตนแก้ |
| --- | --- | --- |
| Create/Update/Submit | Pricing Template Owner | — |
| Approve/Return | Pricing Template Approver | Creator หรือผู้แก้เนื้อหาสาระสำคัญล่าสุด |
| Publish to Calibration | ผู้มี `pricing-templates.publish` | ต้องไม่ข้าม Approved State |
| Activate | Business Owner/ผู้มี `pricing-templates.activate` | ต้องไม่ข้าม Pilot Gate |
| Disable | ผู้มี `pricing-templates.disable` | ทำได้เมื่อมี Incident Reason และ Audit |
| Branch Override | ผู้มี `pricing-templates.override-rate` + Approver | ผู้สร้าง Override ห้ามอนุมัติรายการตนเอง |

Backend ต้องตรวจ Permission, Organization/Branch Scope, State และ Maker–Checker ทุกครั้ง ชื่อ Role เป็นเพียงชุด Permission และไม่ใช้เป็น Business Rule โดยตรง

## Template Version Fields (ช่องข้อมูลรุ่นแม่แบบ)

### Identity และ Ownership

| Field Key | Type | Required At | Rule |
| --- | --- | --- | --- |
| `templateCode` | String | Draft | ไม่ซ้ำใน Organization และแก้ไม่ได้หลัง Version แรก Published |
| `versionNumber` | Integer | Derived | เพิ่มแบบเรียงลำดับภายใน Template Family |
| `nameTh` / `nameEn` | String | Submit | ไทยบังคับ; อังกฤษแนะนำ |
| `workTypeCode` | String | Draft | ต้องอ้าง Work Type ที่ Active |
| `ownerUserId` | ID | Draft | ต้องอยู่ใน Organization Scope |
| `organizationId` | ID | Derived | จาก Scope ของคำสั่ง ไม่รับจาก Client โดยเชื่อถือทันที |
| `branchScopeIds` | ID list | Submit | ว่างหมายถึงมาตรฐานระดับ Organization; ต้องไม่ข้าม Scope |
| `changeReason` | String | Submit | บังคับเมื่อ Version มากกว่า 1 |

### Formula และ Guardrails

| Field Key | Type | Required At | Rule |
| --- | --- | --- | --- |
| `measurementRuleVersion` | String | Submit | อ้าง Rule ที่รองรับและมี Test Case |
| `inputFieldDefinitions` | Object list | Submit | Field Key ไม่ซ้ำ; Type/Unit/Required At ชัดเจน |
| `rateSetVersionIds` | ID list | Submit | Unit ต้องเข้ากับ Measurement Output |
| `gradeFactors` | Object list | Submit | Code ไม่ซ้ำและ Factor อยู่ใน Allowed Bounds |
| `complexityRules` | Object list | Submit | Checklist Answer Map ไป Factor/Add-on/Risk อย่างชัดเจน |
| `addOns` | Object list | Optional | ระบุ Basis, Unit, Amount/Factor และเงื่อนไข |
| `minimumCharge` | Money | Submit | ตั้งแต่ 0; Currency เดียวกับ Rate Set |
| `baseRangeRate` | Decimal ratio | Submit | ตั้งแต่ 0 ถึง `maxRangeRate` |
| `maxRangeRate` | Decimal ratio | Submit | ไม่เกิน Organization Safety Limit |
| `roundingStep` | Money | Submit | มากกว่า 0 และอยู่ในชุดค่าที่องค์กรอนุญาต |
| `validityDays` | Integer | Submit | มากกว่า 0 และไม่เกิน Organization Maximum |
| `requiredEvidence` | Object list | Submit | ระบุ Evidence Type, Minimum Count และ Trigger |
| `defaultAssumptions` / `defaultExclusions` | Text list | Submit | ต้องมีข้อความภาษาไทยสำหรับ Customer Summary |

### Effective Period และ Approval

| Field Key | Type | Required At | Rule |
| --- | --- | --- | --- |
| `effectiveFromUtc` | Date-time | Publish | ต้องไม่ย้อนหลังเกิน Policy |
| `effectiveToUtc` | Date-time/null | Optional | ต้องมากกว่า `effectiveFromUtc` |
| `submittedBy` / `submittedAtUtc` | Derived | Submit | Server บันทึกจากผู้กระทำ |
| `approvedBy` / `approvedAtUtc` | Derived | Approve | ห้ามเป็น Maker เมื่อ Policy บังคับ |
| `publishedBy` / `publishedAtUtc` | Derived | Publish | บันทึกทุกครั้งใน Audit |
| `activatedBy` / `activatedAtUtc` | Derived | Activate | มีได้หลัง Pilot Gate ผ่านเท่านั้น |
| `rowVersion` | Concurrency Token | Every write | Request เก่าห้ามเขียนทับข้อมูลใหม่ |

ข้อมูล Field เฉพาะแต่ละงานอยู่ที่ [Quick Estimate Template Catalog](quick-estimate-template-catalog.md)

## Rate Set Version Fields (ช่องข้อมูลราคาอ้างอิง)

| Field Key | Type | กฎบังคับ |
| --- | --- | --- |
| `rateSetCode` | String | ไม่ซ้ำใน Organization/Scope |
| `versionNumber` | Integer | Server เพิ่มตามลำดับ |
| `rateType` | Enum | `material`, `labor`, `service`, `installation`, `addon` |
| `itemOrOptionCode` | String | อ้าง Item/Option ที่ Active |
| `unitCode` | String | ต้องเข้ากับ Template Measurement Rule |
| `amount` | Decimal Money | ตั้งแต่ 0; ห้ามใช้ Floating Point |
| `currency` | ISO Code | Release แรกใช้ค่า Organization Default |
| `sourceType` | Enum | `supplier-quote`, `price-list`, `historical`, `approved-provisional` |
| `sourceReference` | String | อ้างเลขเอกสาร/ที่มาโดยไม่เก็บ Secret |
| `effectiveFromUtc` / `effectiveToUtc` | Date-time | ห้ามมี Effective Period ซ้อนกันใน Scope เดียวกัน |
| `reviewDueAtUtc` | Date-time | ต้องไม่เกิน Policy ของ Rate Type |
| `status` | Enum | Draft/Submitted/Approved/Active/Superseded/Disabled |
| `ownerUserId` | ID | ผู้รับผิดชอบทบทวนราคา |
| `rowVersion` | Concurrency Token | ป้องกัน Lost Update |

## Branch Rate Override

Branch Override ใช้เมื่อสาขามีต้นทุนจริงต่างจาก Standard Rate และต้องมีทุก Field ต่อไปนี้:

- `branchId`, `standardRateSetVersionId` และ Rate/Factor ที่แทนค่า
- `reasonCode` และ `reasonDetail`
- หลักฐาน Source, Effective Period, Review Due Date และผู้รับผิดชอบ
- Maker, Checker และ Audit Event

กฎบังคับ:

- Override มีผลเฉพาะ Branch ที่อนุมัติและไม่แก้ Standard Rate
- Period ห้ามซ้อนกับ Override ของ Item/Option/Unit เดียวกันใน Branch เดียวกัน
- หมดอายุแล้วกลับไปใช้ Standard Rate ที่ Effective โดยอัตโนมัติ
- ถ้า Standard Rate ที่อ้างถูก Disabled ให้ Recalculate/Review ก่อนแชร์ ห้ามเดาราคาทดแทน

## Validation Gates (ด่านตรวจ)

### Save Draft

ต้องมี `templateCode`, Work Type, Owner และ Scope ที่ถูกต้อง อนุญาตให้สูตรหรือ Test Case ยังไม่ครบได้

### Submit for Approval

ต้องผ่านทั้งหมด:

- Field Key/Type/Unit ไม่ซ้ำและสัมพันธ์กับ Measurement Rule
- Rate Set ทุกชุดอยู่ใน Scope, Currency/Unit ตรง และครอบคลุม Effective Period
- Factor, Range, Minimum Charge, Rounding และ Validity อยู่ใน Guardrail
- Evidence, Assumption, Exclusion และ Customer Disclaimer ครบ
- มี Test Fixture อย่างน้อยหนึ่ง Happy Path ต่อ Material Grade และ Boundary/Risk Case ตาม Test Matrix
- ไม่มี unresolved validation error

### Approve

Approver ต้องเห็น Diff จาก Version ก่อนหน้า, Formula Preview, Source ของ Rate, Test Result และผลกระทบต่อ Active Quick Estimate ต้องระบุ Decision Note และห้ามอนุมัติงานตนเอง

### Publish to Calibration

Version ต้อง Approved, Effective Period ไม่ชน Version อื่น และ Test Matrix ผ่าน ระบบ Freeze เนื้อหา Version แล้วเปิดใช้เฉพาะ Calibration Policy

### Activate

ต้องมี Calibration/Pilot Report, Sample Size ตาม Policy, ไม่มี Critical Defect, Accuracy อยู่ใน Threshold, Out-of-range ทุกกรณีมี Root Cause และ Business Owner อนุมัติ Activation

## Change Classification (จำแนกการเปลี่ยน)

| การเปลี่ยน | วิธีจัดการ |
| --- | --- |
| แก้คำอธิบายภายในที่ไม่แสดงลูกค้าและไม่กระทบผล | Metadata update พร้อม Audit ตาม Policy |
| Field, Unit, Formula, Rate, Factor, Range, Evidence หรือ Disclaimer | สร้าง Template/Rate Version ใหม่ |
| แก้ Effective Date ก่อน Publish | แก้ Draft/Returned แล้ว Submit ใหม่ตาม Policy |
| พบราคาผิดร้ายแรงใน Active Version | Disable ทันทีพร้อม Incident แล้วสร้าง Version ใหม่ |
| ต้องใช้ราคาเฉพาะสาขาชั่วคราว | Branch Rate Override ที่มี Approval/Expiry |

ห้ามแก้ Published Version ในฐานข้อมูลโดยตรง แม้ยังไม่มี Quick Estimate ใช้งาน ให้สร้าง Version ใหม่เพื่อรักษาประวัติที่ตรวจสอบได้

## TEST_ONLY Examples (ตัวอย่าง)

### EX-PT-001: เปลี่ยน Reference Rate

```text
Given: QE-BI-WARDROBE-LM v1 เป็น Active และใช้ RATE-BI-WARDROBE v3
When: Owner ต้องเปลี่ยน Rate จาก 12,000 เป็น 12,800 TEST_ONLY THB/m
Then: สร้าง RATE-BI-WARDROBE v4 และ Template v2 ที่อ้าง Rate v4
      Submit → Approve → Calibration → Activate
      Template v1 เปลี่ยนเป็น Superseded แต่ Snapshot เดิมยังคำนวณซ้ำได้
```

### EX-PT-002: Branch Override ชั่วคราว

```text
Given: Standard Rate = 500 TEST_ONLY THB/m และ Branch CNX มีค่าขนส่งเพิ่ม
When: สร้าง Override = 550 TEST_ONLY THB/m ช่วง 1–30 พฤศจิกายน
Then: Quick Estimate ของ Branch CNX ในช่วงเวลานั้นใช้ 550
      Branch อื่นใช้ 500 และวันที่ 1 ธันวาคม CNX กลับไปใช้ Standard Rate ที่ Effective
```

### EX-PT-003: Emergency Disable

```text
Given: พบ Formula ผิดใน Active Template
When: ผู้มีสิทธิ์ Disable พร้อม Incident Reason
Then: ห้าม Calculate/Share รายการใหม่ทันที
      Draft เดิมยังอ่านได้แต่ต้องเลือก Version ใหม่และ Recalculate
      Shared Snapshot เดิมไม่ถูกเขียนทับ และ Auditor เห็นผู้สั่งหยุด/เวลา/เหตุผล
```

ตัวเลขทั้งหมดในตัวอย่างเป็น `TEST_ONLY` ห้ามใช้เป็นราคาจริง

## Business Test Cases

| Test ID | Given / When | Expected Result |
| --- | --- | --- |
| `TC-PT-001` | Draft มีเพียง Code, Work Type, Owner และ Scope | Save Draft ได้ |
| `TC-PT-002` | Submit โดยไม่มี Rate Set/Test Fixture | 422 `PRICING_TEMPLATE_INVALID` พร้อม Field Error |
| `TC-PT-003` | Input Unit ไม่ตรง Rate Unit | 422 `PRICING_TEMPLATE_INVALID` |
| `TC-PT-004` | Base Range มากกว่า Max Range | 422 และไม่เปลี่ยน State |
| `TC-PT-005` | Effective Period ของ Rate ซ้อนกันใน Scope เดียวกัน | 409 `PRICING_RATE_PERIOD_OVERLAP` |
| `TC-PT-006` | Owner Submit Version ที่ครบ | State เป็น `Submitted`, เก็บ Actor/Time |
| `TC-PT-007` | Owner แก้ Submitted Version โดยยังไม่ Return | 409 `PRICING_TEMPLATE_INVALID_STATE` |
| `TC-PT-008` | Approver Return พร้อมเหตุผล | กลับ Draft Work Queue และเก็บ Review Outcome |
| `TC-PT-009` | Maker พยายาม Approve Version ของตนเอง | 403 `MAKER_CHECKER_VIOLATION` |
| `TC-PT-010` | Approver อนุมัติ Version ที่ Test ผ่าน | State เป็น `Approved` และ Freeze Approval Snapshot |
| `TC-PT-011` | Publish Version ที่ยังไม่ Approved | 409 `PRICING_TEMPLATE_INVALID_STATE` |
| `TC-PT-012` | Publish Approved Version | State เป็น `Calibration`; เนื้อหาสาระสำคัญแก้ไม่ได้ |
| `TC-PT-013` | Share Quick Estimate ที่ใช้ Calibration Version | `PendingReview` เสมอ |
| `TC-PT-014` | Activate โดยไม่มี Pilot Report | 422 `PRICING_TEMPLATE_PILOT_NOT_PASSED` |
| `TC-PT-015` | Activate Version ที่ผ่าน Pilot | Version ใหม่เป็น Active และ Active เดิมเป็น Superseded ใน Transaction เดียวกัน |
| `TC-PT-016` | ผู้ใช้ Branch A อ่าน/ใช้ Version เฉพาะ Branch B | 404 เพื่อไม่เปิดเผย Resource นอก Scope |
| `TC-PT-017` | สร้าง Branch Override โดยไม่มี Checker | ยังไม่ Active และใช้คำนวณไม่ได้ |
| `TC-PT-018` | Override Period หมดอายุ | ใช้ Standard Rate ที่ Effective โดยอัตโนมัติ |
| `TC-PT-019` | Request เก่าส่ง `rowVersion` ที่ไม่ตรง | 409 `PRICING_TEMPLATE_VERSION_CONFLICT` พร้อมให้ Reload/Compare |
| `TC-PT-020` | Disable Active Version พร้อมเหตุผล | Calculate/Share ใหม่ถูก Block แต่ Snapshot เดิมยังอ่านได้ |
| `TC-PT-021` | เปลี่ยน Rate หลังมี Shared Quick Estimate | Shared Version เดิมไม่เปลี่ยน; รายการใหม่ใช้ Effective Version ใหม่ |
| `TC-PT-022` | Retry Submit/Publish ด้วย Idempotency Key เดิม | คืนผล Action เดิม ไม่สร้าง Version/Audit ซ้ำ |

## Audit Events ขั้นต่ำ

- `pricing-template.created`, `updated`, `submitted`, `returned`, `approved`, `published`, `activated`, `superseded`, `disabled`
- `pricing-rate.created`, `submitted`, `approved`, `activated`, `superseded`, `disabled`
- `pricing-rate-override.created`, `submitted`, `approved`, `activated`, `expired`, `disabled`
- ทุก Event เก็บ Actor, Organization/Branch Scope, Resource/Version, UTC Time, Trace ID และ Reason/Diff เมื่อเกี่ยวข้อง

Audit เก็บเหตุการณ์ ส่วน Published Version/Approval Snapshot เก็บเนื้อหาทางธุรกิจ ห้ามใช้ Audit Payload แทน Business Snapshot

## เกณฑ์พร้อม Production

- ระบุตัวจริงของ Owner, Approver, Business Owner และผู้มีอำนาจ Disable
- Guardrail, SLA, Pilot Sample Size, Accuracy Threshold และ Review Cycle ได้รับอนุมัติ
- Permission/Scope/Maker–Checker และ Test Case ในเอกสารนี้ผ่านทั้งหมด
- มี Dashboard แจ้ง Rate ใกล้หมดอายุ, Template ค้าง Review และ Calibration ที่ยังไม่ผ่าน
- ซ้อม Emergency Disable, Recalculate และ Audit Retrieval สำเร็จ
