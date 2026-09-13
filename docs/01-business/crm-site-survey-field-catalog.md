# CRM and Site Survey Field Catalog (รายการช่องข้อมูลลูกค้า งานขาย และสำรวจ)

**สถานะ:** Accepted Direction — Development Baseline; ข้อมูลทั้งหมดเป็น `TEST_ONLY`

เอกสารนี้เป็นแหล่งอ้างอิงหลักของ Field, Type, Required Gate และ Data Classification สำหรับ Customer, Contact, Address, Opportunity, Site และ Site Survey

## Gate Legend

- `D` = บังคับเมื่อสร้าง Draft
- `A` = บังคับก่อน Activate Customer/Site
- `Q` = บังคับก่อน Qualify Opportunity
- `R` = บังคับก่อน Mark Survey Revision Ready
- `Internal` = ข้อมูลการทำงานภายใน
- `Personal` = ข้อมูลบุคคลที่ต้องจำกัดสิทธิ์/Log/Export
- `Customer-safe` = นำไป Snapshot สำหรับเอกสารลูกค้าได้เมื่อผ่าน Allowlist

## Customer

| Field | Type | Gate | Rule | Class |
| --- | --- | --- | --- | --- |
| `organizationId` | UUID | D | จาก Active Membership; Client เปลี่ยนไม่ได้ | Internal |
| `code` | String ≤30 | System | Unique ใน Organization; Server Generate | Internal |
| `customerType` | Enum | D | `person`, `organization` | Internal |
| `displayNameTh` | String ≤250 | A | บังคับ; Trim/Normalize | Personal/Customer-safe |
| `displayNameEn` | String ≤250/null | Optional | ไม่เดาจากภาษาไทย | Personal/Customer-safe |
| `legalName` | String ≤250/null | Conditional | ใช้เมื่อเอกสารทางการต้องการ | Personal |
| `taxIdentifier` | String/null | Conditional | Normalize/Mask; ห้าม Log ค่าจริง | Personal |
| `preferredLocale` | Enum | A | `th`, `en`; default `th` | Internal |
| `status` | Enum | System | `draft`, `active`, `inactive` | Internal |
| `inactiveReason` | String/null | Conditional | บังคับเมื่อ Inactive | Internal |
| `rowVersion` | Token | System | Optimistic concurrency | Internal |

ชื่อ/โทรศัพท์ไม่เป็น Unique Key อัตโนมัติ ระบบแสดง Duplicate Candidate ให้คนตรวจ

## Contact

| Field | Type | Gate | Rule | Class |
| --- | --- | --- | --- | --- |
| `customerId` | UUID | D | Customer ใน Organization เดียวกัน | Internal |
| `name` | String ≤250 | D | ชื่อผู้ติดต่อ | Personal |
| `roleTitle` | String ≤150/null | Optional | ตำแหน่ง/ความสัมพันธ์ | Personal |
| `phone` | String/null | Conditional | อย่างน้อย Phone หรือ Email; Normalize แยก Display | Personal |
| `email` | String/null | Conditional | Validate รูปแบบ แต่ไม่ถือว่ายืนยันตัวตน | Personal |
| `preferredChannel` | Enum/null | Optional | `phone`, `email`, `line`, `other` | Personal |
| `isPrimary` | Boolean | D | Primary ได้หนึ่งรายการต่อ Customer ตาม Policy | Internal |
| `status` | Enum | System | active/inactive | Internal |
| `rowVersion` | Token | System | ทุก Mutation | Internal |

## Customer Address and Site

| Field | Type | Gate | Rule | Class |
| --- | --- | --- | --- | --- |
| `label` | String ≤100 | A | เช่น บ้าน/สำนักงาน/สถานที่ติดตั้ง | Customer-safe |
| `addressLine1`, `subdistrict`, `district`, `province`, `postalCode`, `countryCode` | String | A | Structured fields; Phase 1 country default TH จาก Policy | Personal |
| `latitude`, `longitude` | Decimal/null | Optional | เก็บเมื่อผู้ใช้ยืนยัน; ไม่เดาจากข้อความ | Personal |
| `accessNote` | String ≤1,000/null | Optional | Parking/Lift/เวลานิติฯ; Internal | Personal/Internal |
| `customerId` | UUID | A | Site Owner/Requester ใน Organization | Internal |
| `status` | Enum | System | draft/active/inactive | Internal |
| `rowVersion` | Token | System | Address/Site แต่ละ Resource มี Version ของตน | Internal |

Customer Address เป็นข้อมูลติดต่อ/ออกเอกสาร ส่วน Site เป็นสถานที่ปฏิบัติงาน แม้ข้อความที่อยู่เหมือนกันก็เป็นคนละ Resource

## Opportunity

| Field | Type | Gate | Rule | Class |
| --- | --- | --- | --- | --- |
| `branchId`, `customerId`, `ownerUserId` | UUID | D/Q | Active และอยู่ Scope เดียวกัน | Internal |
| `code` | String ≤30 | System | Unique ใน Organization | Internal |
| `title` | String ≤250 | D | สรุปงาน ไม่ใช้ชื่อลูกค้าอย่างเดียว | Personal/Internal |
| `scopeSummary` | String ≤2,000 | Q | ความต้องการที่ยืนยันเบื้องต้น | Personal/Internal |
| `workTypes` | Enum Set | Q | built-in/interior/curtain/wallpaper/exterior/other | Internal |
| `sourceCode` | String ≤50/null | Optional | Controlled catalog; ไม่เก็บ Free-text เป็น Code | Internal |
| `primarySiteId` | UUID/null | Conditional | บังคับก่อน Surveying | Personal/Internal |
| `expectedBudget` | Money/null | Optional | Internal; ไม่ใช่ Approved Price | Personal/Internal |
| `targetDecisionDate` | Local Date/null | Optional | วันที่คาดการณ์ | Internal |
| `nextActionAtUtc`, `nextActionNote` | UTC/String | Q | ต้องมีเมื่อยัง Open | Personal/Internal |
| `stage` | Enum | System | draft/qualified/surveying/estimating/proposed/won/lost/cancelled | Internal |
| `outcomeReasonCode`, `outcomeNote` | String/null | Conditional | บังคับ Lost/Cancelled/Reopen | Internal |
| `rowVersion` | Token | System | ทุก Mutation | Internal |

### Opportunity Work Images (planned; not yet implemented)

Opportunity หนึ่งรายการแนบภาพงานได้หลายภาพในแต่ละ Open stage (`draft`, `qualified`, `surveying`, `estimating`, `proposed`) และเลือกหลายภาพในครั้งเดียวได้; เพิ่มชุดภาพภายหลังได้. การมีภาพยังเป็น Optional ไม่ใช่ Q gate หรือ stage gate จน Sales Owner ยืนยันเกณฑ์ขั้นต่ำ. Closed stage อ่านย้อนหลังอย่างเดียว. ภาพ Survey/Estimate/Quotation ยังคงเป็นหลักฐานของโมดูลต้นทาง ไม่คัดลอก binary มา CRM.

| Field | Type | Gate | Rule | Class |
| --- | --- | --- | --- | --- |
| `opportunityId`, `fileId` | UUID | On attach | แต่ละภาพอ้าง Opportunity/verified File ใน Organization เดียวกัน | Internal |
| `stageAtAttach` | Enum | System | เก็บ stage ณ เวลาผูกภาพ; ไม่เปลี่ยนย้อนหลัง | Internal |
| `caption` | String/null | Optional | คำอธิบายแยกต่อภาพ; ความยาวรอยืนยัน | Personal/Internal |
| `displayOrder` | Integer | System | รักษาลำดับภาพในชุดที่บันทึก | Internal |
| `createdByUserId`, `createdAtUtc` | UUID/UTC | System | ผู้แนบและเวลาผูกภาพ | Internal |

Binary อยู่ File Service; CRM เก็บเฉพาะ references/metadata และบันทึกทั้งชุดแบบ atomic หลังทุกภาพผ่าน upload/verify. รายละเอียด contract และ verification อยู่ที่ [Opportunity Work Images Plan](../superpowers/plans/2026-09-13-opportunity-work-images-vertical-slice.md).

### แผนการพัฒนา Master Data สู่ Database Table ในอนาคต (Future Master Data Table Roadmap)

ใน Phase ปัจจุบัน (Vertical Slice) ข้อมูลตัวเลือกเชิงจำแนกประเภทถูกควบคุมผ่าน **Canonical Enums และ Controlled Value Objects** เพื่อรักษา Minimal Blast Radius:
1. **Work Types (`workTypes`):** ปัจจุบันเป็น Canonical Enum Set (`built-in`, `interior`, `curtain`, `wallpaper`, `exterior`, `other`) บันทึกแบบ PostgreSQL Array `text[]` เพื่อเป็นรากฐานให้กับโมดูล Estimation/BOM และ MRP ในอนาคต เมื่อระบบขยายตัวเต็มรูปแบบ จะสามารถยกระดับเป็นตาราง `work_type_masters` เพื่อให้แต่ละองค์กรสามารถปรับแต่งรายการประเภทงานและผูก Template งานสำรวจได้
2. **Lead Source (`sourceCode`):** ปัจจุบันเป็น Controlled Code ที่รองรับ Custom Note ผ่านรูปแบบ `other:{detail}` เพื่อความยืดหยุ่น ในอนาคตจะขยายเป็นตาราง `lead_source_masters` เพื่อรองรับการตั้งค่าแคมเปญการตลาดและการวัด Conversion Rate แยกตามสาขา
3. **Currency (`currencyCode`):** ปัจจุบันใช้มาตรฐาน ISO 4217 (THB default, USD, EUR, JPY, SGD, CNY) ในอนาคตเมื่อรองรับ Multi-currency เต็มรูปแบบ จะพัฒนาเป็นตาราง `currency_masters` และ `exchange_rate_histories` เพื่อการแปลงอัตราแลกเปลี่ยนตามช่วงเวลาจริง

## Site Survey Identity and Revision

| Field | Type | Gate | Rule | Class |
| --- | --- | --- | --- | --- |
| `opportunityId`, `siteId`, `branchId` | UUID | D | ต้องสัมพันธ์ Customer/Organization เดียวกัน | Internal |
| `surveyNumber` | String ≤30 | System | Unique ใน Organization | Internal |
| `scheduledStartUtc`, `scheduledEndUtc` | UTC/null | Conditional | End > Start | Personal/Internal |
| `assignedSurveyorId` | UUID | D | Active Membership + Branch Scope | Personal/Internal |
| `revisionNumber` | Integer | System | เริ่ม 1 และเพิ่มแบบ Atomic | Internal |
| `surveyTemplateVersion` | String | R | Published/usable ณ Visit Date | Internal |
| `visitedAtUtc` | UTC | R | เวลาสำรวจจริง | Personal/Internal |
| `scopeSummary` | String ≤2,000 | R | ขอบเขตที่พบหน้างาน | Customer-safe after review |
| `assumptions`, `constraints`, `missingDetails` | String List | R | ระบุแม้เป็น Empty List | Customer-safe after review |
| `readiness` | Enum | System | incomplete/requiresAttention/ready | Internal |
| `status` | Enum | System | draft/ready/superseded/void | Internal |
| `readyAtUtc`, `readyBy`, `snapshotHash` | System | System | มีเมื่อ Ready | Internal |
| `rowVersion` | Token | System | Draft concurrency | Internal |

## Survey Template Version

| Field | Type | Rule |
| --- | --- | --- |
| `code`, `version` | String/Integer | Stable identity; Version เพิ่มเมื่อ Requirement เปลี่ยน |
| `workTypes` | Enum Set | ระบุประเภทงานที่ใช้ Template ได้ |
| `requiredFields`, `requiredMeasurements`, `requiredChecklist`, `requiredEvidence` | Typed/Schema payload | Validate ก่อน Publish และอ่าน Version เก่าได้ |
| `effectiveFromUtc`, `effectiveToUtc` | UTC/null | Period ไม่กลับด้าน/ไม่ซ้อนใน Scope เดียวกัน |
| `status`, `schemaVersion`, `hash` | System | draft/published/superseded/disabled; Published immutable |

Phase แรกใช้ System-owned Baseline Template ที่ติด Version และ Deploy ผ่าน Migration/Seed ที่ตรวจสอบได้ ไม่มี Runtime Form Builder/Admin Editor

## Area, Measurement, Checklist and Evidence

| Entity/Field | Type | Gate | Rule | Class |
| --- | --- | --- | --- | --- |
| Area `code`, `name`, `sortOrder` | String/Integer | R | อย่างน้อยหนึ่ง Area; Code ไม่ซ้ำใน Revision | Customer-safe |
| Measurement `measurementType` | Enum | R | เช่น length/width/height/area/opening/count | Internal |
| Measurement `value`, `unitCode` | Decimal/Code | R | Value >0; Unit เข้ากับ Type | Internal |
| Measurement `captureMethod` | Enum | R | measured/customer-provided/derived | Internal |
| Measurement `sourceMeasurementIds` | UUID List/null | Conditional | บังคับเมื่อ Derived; ห้าม Cycle | Internal |
| Checklist `itemCode`, `result` | Code/Enum | R | pass/fail/not-applicable; Required Item ห้ามว่าง | Internal |
| Checklist `note` | String/null | Conditional | บังคับเมื่อ Fail/NA ตาม Template | Internal |
| Evidence `fileId`, `kind`, `caption` | UUID/Enum/String | R | Upload complete; kind photo/sketch/document/other | Personal/Internal |
| Evidence `areaId`, `capturedAtUtc` | UUID/UTC/null | Conditional | Required ตาม Template/Evidence Type | Personal/Internal |

## Field Contract Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-FIELD-CRM-001` | Active Customer ไม่มีชื่อไทย | `CUSTOMER_FIELD_REQUIRED` |
| `TC-FIELD-CRM-002` | Contact ไม่มี Phone และ Email | Field Error |
| `TC-FIELD-CRM-003` | Opportunity Customer/Branch คนละ Scope | 404 + Security Audit |
| `TC-FIELD-CRM-004` | Qualify โดยไม่มี Next Action | `OPPORTUNITY_FIELD_REQUIRED` |
| `TC-FIELD-SRV-001` | Measurement ≤0/Unit ผิด Type | `SURVEY_MEASUREMENT_INVALID` |
| `TC-FIELD-SRV-002` | Derived Measurement เป็น Cycle | Reject |
| `TC-FIELD-SRV-003` | Ready โดย Evidence Upload ค้าง | `SURVEY_NOT_READY` |
| `TC-FIELD-SRV-004` | Patch Ready Revision | `SURVEY_INVALID_STATE` |
| `TC-FIELD-SRV-005` | Clone Ready Revision | Draft รุ่นใหม่; รุ่นเดิมไม่เปลี่ยน |
| `TC-FIELD-SRV-006` | Log/Export Contact | Mask/Allowlist ตาม Permission |

## เอกสารที่เกี่ยวข้อง

- [Flow](crm-site-survey-flow.md)
- [Governance](crm-site-survey-governance.md)
- [API Contract](../03-contracts/crm-site-survey-api-contract.md)
- [Data Contract](../04-data/crm-site-survey-data-contract.md)
