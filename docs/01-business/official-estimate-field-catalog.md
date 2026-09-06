# Official Estimate Field Catalog (รายการช่องข้อมูลประมาณการทางการ)

**สถานะ:** Accepted Direction — Production Baseline ที่ต้องยืนยันค่า Policy จริงกับเจ้าของงานและฝ่ายบัญชี

เอกสารนี้เป็นแหล่งอ้างอิงหลักว่า Official Estimate ต้องมีข้อมูลอะไร บังคับเมื่อใด ใครแก้ได้ และลูกค้าเห็นหรือไม่ รายละเอียดฐานข้อมูลให้อ่าน [Official Estimate Data Contract](../04-data/official-estimate-data-contract.md)

## วิธีอ่าน

- `D` = ต้องมีเพื่อบันทึก Draft ครั้งแรก
- `C` = ต้องมีก่อน Calculate
- `S` = ต้องมีก่อน Submit
- `Q` = ต้องมีก่อนออก Quotation
- `Conditional` = บังคับเมื่อเข้าเงื่อนไขที่ระบุ
- `Internal` = ใช้ภายใน ห้ามส่งใน Customer-facing Contract
- `Customer` = อนุญาตให้แสดงใน Quotation Snapshot หลังผ่าน Mapping

Frontend อาจซ่อน Field ตาม Permission แต่ Backend ต้องตรวจ Requiredness, Scope และ Policy ทุกครั้ง

## Document Context

| Field | Type/Format | Gate | Default/Validation | ผู้แก้ | Visibility |
| --- | --- | --- | --- | --- | --- |
| `organizationId` | UUID | D | จาก Active Membership; Client เปลี่ยนไม่ได้ | System | Internal |
| `branchId` | UUID | D | ต้องอยู่ Organization Scope | Estimator | Internal |
| `customerId` | UUID | D | Active Customer ใน Scope | Estimator | Customer ผ่าน Snapshot |
| `opportunityId` | UUID | D | ต้องสัมพันธ์กับ Customer/Branch | Estimator | Internal |
| `siteSurveyRevisionId` | UUID/null | S | Ready/Superseded Revision ใน Scope; หากไม่มีต้องมี Scope Evidence ตาม Policy | Estimator/System | Internal |
| `siteSurveySnapshotHash` | String/null | System | Freeze คู่กับ Revision ที่ใช้; null เมื่อไม่มี Survey | System | Internal |
| `ownerUserId` | UUID | D | ค่าเริ่มต้นเป็นผู้สร้าง; ต้องมี `estimates.update` ใน Scope | Estimator/Manager | Internal |
| `currency` | ISO 4217 | D | Phase 1 ใช้ `THB`; Currency เดียวต่อ Revision | System/Policy | Customer |
| `customerReference` | String ≤100 | Optional | เลขอ้างอิงลูกค้า; Trim และ Normalize | Estimator | Customer |
| `estimateDate` | Local Date | S | วันที่ธุรกิจตาม Branch Time Zone | Estimator | Customer |
| `validityDays` | Integer 1–365 | Q | จาก Commercial Policy; Override ต้องมีเหตุผล | Estimator/Authorized | Customer |
| `internalNote` | String ≤2,000 | Optional | ห้าม Map ไป Quotation | Estimator/Reviewer | Internal |

## Section

| Field | Type/Format | Gate | Default/Validation | ผู้แก้ | Visibility |
| --- | --- | --- | --- | --- | --- |
| `code` | String ≤30 | C | ไม่ซ้ำใน Revision | Estimator | Customer |
| `nameTh` | String ≤200 | C | บังคับอย่างน้อยภาษาไทย | Estimator | Customer |
| `nameEn` | String ≤200/null | Optional | ใช้เมื่อออกเอกสารอังกฤษ | Estimator | Customer |
| `sortOrder` | Integer ≥10 | C | เพิ่มทีละ 10; Server Normalize ได้ | Estimator/System | Customer |
| `description` | String ≤1,000/null | Optional | รายละเอียดขอบเขตระดับหมวด | Estimator | Customer |

Section Total เป็นผลคำนวณจาก Server ไม่ใช่ Input

## Work Item

| Field | Type/Format | Gate | Default/Validation | ผู้แก้ | Visibility |
| --- | --- | --- | --- | --- | --- |
| `code` | String ≤30 | C | ไม่ซ้ำใน Revision | Estimator | Customer |
| `itemId` | UUID/null | Optional | อ้าง Active Item Master; null คือ Custom Work Item | Estimator | Internal |
| `descriptionTh` | String ≤500 | C | ห้ามว่าง | Estimator | Customer |
| `descriptionEn` | String ≤500/null | Conditional Q | บังคับเมื่อ Quotation Locale เป็นอังกฤษ | Estimator | Customer |
| `quantity` | Decimal(18,4) | C | มากกว่า 0 | Estimator | Customer |
| `unitCode` | String ≤20 | C | Active Unit และเข้ากับ Item/Measurement | Estimator | Customer |
| `scopeNote` | String ≤1,000/null | Optional | ระบุสิ่งที่รวม/ไม่รวม | Estimator | Customer |
| `sellingRuleType` | Enum | C | `margin`, `markup` หรือ `fixed-price` ตาม Policy | Estimator/Policy | Internal |
| `sellingRuleValue` | Decimal(18,6) | C | อยู่ใน Policy Range; Override ต้องมีเหตุผล | Estimator/Authorized | Internal |
| `sortOrder` | Integer ≥10 | C | ไม่ซ้ำภายใน Section | Estimator/System | Customer |

Custom Work Item ต้องมี `overrideReasonCode`, `overrideReason` และ Approval Trigger จนกว่าจะถูกยืนยันเป็น Item Master

## Cost Component

| Field | Type/Format | Gate | Default/Validation | ผู้แก้ | Visibility |
| --- | --- | --- | --- | --- | --- |
| `type` | Enum | C | `material`, `labor`, `subcontract`, `service`, `other-direct` | Estimator | Internal |
| `itemId` | UUID/null | Optional | Active Item/Service ใน Scope | Estimator | Internal |
| `description` | String ≤500 | C | บังคับเมื่อไม่มี `itemId` | Estimator | Internal |
| `quantity` | Decimal(18,4) | C | มากกว่า 0 | Estimator | Internal |
| `unitCode` | String ≤20 | C | ต้องเข้ากับ Cost Source | Estimator | Internal |
| `unitCost` | Decimal(19,4) | C | ≥0; Client ส่ง Total ไม่ได้ | Estimator/Cost Resolver | Internal |
| `currency` | ISO 4217 | C | ต้องตรง Revision ใน Phase 1 | System | Internal |
| `costRecordId` | UUID/null | S | Published Cost ที่ Resolver เลือก; null ได้เฉพาะ Provisional Cost | Cost Resolver | Internal |
| `costRecordVersion` | Integer/null | S | Freeze คู่กับ Cost Record | Cost Resolver | Internal |
| `costSourceId` | UUID/null | S | Snapshot จาก Cost Record; null ได้เฉพาะ Provisional Cost | Cost Resolver | Internal |
| `costResolutionPolicyVersion` | String/null | S | Version ที่ใช้ Resolve; null ได้เฉพาะ Provisional Cost | Cost Resolver | Internal |
| `conversionSnapshot` | Object/null | Conditional S | บังคับเมื่อ Unit ที่ขอกับ Cost Record ต่างกัน | Cost Resolver | Internal |
| `effectiveAt` | UTC/null | S | ใช้ตรวจ Stale Cost ตาม Policy | System/Estimator | Internal |
| `isProvisional` | Boolean | S | true เมื่อ Source ไม่สมบูรณ์ | System | Internal |
| `provisionalReasonCode` | Stable Code/null | Conditional S | บังคับเมื่อ `isProvisional=true` | Estimator | Internal |
| `provisionalReason` | String ≤500/null | Conditional S | บังคับเมื่อ `isProvisional=true` | Estimator | Internal |

Cost Component ว่างทุกประเภททำให้ Work Item ไม่พร้อม Submit แต่ Zero Cost อนุญาตได้เมื่อมี Reason และ Policy ยอมรับ Resolver/Version/Snapshot อ้าง [Item Master Governance](item-master-governance.md)

## Adjustment, Discount and Tax

| Field | Type/Format | Gate | Default/Validation | ผู้แก้ | Visibility |
| --- | --- | --- | --- | --- | --- |
| `overheadMethod` | Enum | C | `percent-direct-cost` หรือ `fixed-amount` จาก Policy | Policy/Authorized | Internal |
| `overheadValue` | Decimal(19,6) | C | ≥0 | Policy/Authorized | Internal |
| `discountType` | Enum | C | Phase 1: `none`, `percent`, `fixed-amount` ระดับ Document | Estimator/Authorized | Customer |
| `discountValue` | Decimal(19,6) | Conditional C | ≥0 และห้ามทำ Net Before Tax ติดลบ | Estimator/Authorized | Customer |
| `discountReasonCode` | Stable Code/null | Conditional S | บังคับเมื่อ Discount >0 | Estimator | Internal |
| `taxPolicyVersionId` | UUID | C | Resolve จาก Branch/Estimate Date | System | Internal |
| `taxCode` | String ≤30 | C | จาก Published Tax Policy | System | Customer |
| `taxRate` | Decimal(12,6) | C | Server-owned จาก Policy Snapshot | System | Customer |
| `taxDisplayMode` | Enum | Q | `exclusive`, `inclusive` หรือ `exempt` | Policy | Customer |

Line/Section Discount และ Compound Tax หลายชั้นอยู่นอก Phase 1

## Override Fields

ทุกการ Override ต้องใช้ชุดข้อมูลเดียวกัน:

| Field | Rule |
| --- | --- |
| `targetField` | Field ที่ถูกเปลี่ยนและอยู่ใน Allowlist |
| `beforeValue` / `afterValue` | ค่าแบบ Typed; Financial Value ใช้ Decimal String |
| `reasonCode` | Stable Code จาก Catalog |
| `reason` | 10–500 ตัวอักษร |
| `performedBy` / `performedAtUtc` | Server-owned |
| `permissionUsed` | Permission ที่ผ่านการตรวจ |

Override ทำให้ Calculation Outdated และอาจเพิ่ม Approval Step ห้ามใช้ Override เพื่อข้าม Blocking Error

## Server-derived Fields

Client อ่านได้แต่เขียนไม่ได้:

- Direct Cost, Allocated Overhead และ Total Cost
- Selling Price Before Discount, Discount Amount และ Net Before Tax
- Tax Amount, Grand Total, Margin Amount/Rate และ Markup Rate
- Readiness, Reason Codes, Approval Route และ Approval Status
- Calculation Version/Hash, Policy Version และ Row Version/ETag

## Gate Matrix

| Gate | ต้องผ่านขั้นต่ำ |
| --- | --- |
| Save Draft | Document Context ที่ติด `D`; Partial Section/Item เก็บได้พร้อม Draft Warning |
| Calculate | Section/Work Item/Cost/Pricing Field ที่ติด `C`; Unit/Currency ถูกต้อง |
| Submit | Calculation ล่าสุด, ไม่มี Blocking Error, Provisional/Override มีเหตุผล และ Resolve Approval Policy ได้ |
| Approve | Active Approval Step, Permission + Authority + Scope, Maker–Checker และ Snapshot Hash ตรง |
| Quote | Approved Revision, Customer Snapshot Field ครบ, Validity/Locale/Tax Display ถูกต้อง |

## Stable Validation Codes

| Code | ใช้เมื่อ |
| --- | --- |
| `ESTIMATE_FIELD_REQUIRED` | Field ที่ Gate ปัจจุบันบังคับยังว่าง |
| `ESTIMATE_UNIT_INVALID` | Unit ไม่ Active หรือไม่เข้ากับ Item/Cost Source |
| `ESTIMATE_COST_INCOMPLETE` | Work Item ไม่มี Cost Component ที่ครบ |
| `ESTIMATE_PROVISIONAL_COST_REASON_REQUIRED` | Provisional Cost ยังไม่มีเหตุผลที่บังคับ |
| `ESTIMATE_POLICY_UNAVAILABLE` | Resolve Calculation/Tax/Approval Policy ที่ Published ไม่ได้ |
| `ESTIMATE_CALCULATION_OUTDATED` | Financial Input เปลี่ยนหลัง Calculate |

## ตัวอย่างสั้น

`TEST_ONLY`: Work Item “ตู้เสื้อผ้า Built-in” จำนวน `3.0000 m` มี Material และ Labor Cost ผู้ประเมินเลือก Margin Policy แล้วระบบคำนวณ Total เอง หาก Material Cost ไม่มี Source ให้ระบุเป็น Provisional Cost พร้อมเหตุผลและส่ง Checker เพิ่ม ห้ามผู้ใช้กรอก Grand Total โดยตรง

## Field Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-FIELD-EST-001` | Save Draft ขาด Organization จาก Membership | Reject Authorization Boundary |
| `TC-FIELD-EST-002` | Draft มี Work Item บางส่วน | Save ได้พร้อม Draft Warning |
| `TC-FIELD-EST-003` | Calculate เมื่อ Quantity ≤0 | 422 Field Error |
| `TC-FIELD-EST-004` | Unit ไม่เข้ากับ Item/Cost Source | `ESTIMATE_UNIT_INVALID` |
| `TC-FIELD-EST-005` | Provisional Cost ไม่มี Reason | `ESTIMATE_PROVISIONAL_COST_REASON_REQUIRED` |
| `TC-FIELD-EST-006` | Client ส่ง Grand Total/Margin | Reject/Ignore; ใช้ Server-derived Result |
| `TC-FIELD-EST-007` | Quotation ภาษาอังกฤษแต่ Description EN ขาด | Block Q Gate พร้อม Field Pointer |
| `TC-FIELD-EST-008` | Customer Projection | ไม่มี Field ที่ Visibility เป็น Internal |
| `TC-FIELD-EST-009` | Override ไม่มี Permission/Reason | Reject และไม่แก้ Draft |
| `TC-FIELD-EST-010` | แก้ Financial Field หลัง Calculate | `calculationOutdated=true` |

## เอกสารที่เกี่ยวข้อง

- [Calculation Rules](estimation-calculation-rules.md)
- [Approval Matrix](approval-matrix.md)
- [Estimation Flow](estimation-flow.md)
- [Official Estimate API Contract](../03-contracts/official-estimate-api-contract.md)
