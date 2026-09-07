# Error Contract and Localization (ข้อผิดพลาดและภาษา)

**สถานะ:** Accepted

Backend ส่ง Error ตาม RFC 9457 Problem Details พร้อมรหัสคงที่ที่ Frontend ใช้ตัดสินใจ ข้อความแสดงผลแปลตาม `Accept-Language` และใช้ภาษาไทยเมื่อภาษาที่ขอไม่รองรับ

## ตัวอย่าง

```json
{
  "type": "https://tan-erp.local/problems/estimate-invalid-state",
  "title": "ไม่สามารถอนุมัติประมาณการได้",
  "status": 409,
  "code": "ESTIMATE_INVALID_STATE",
  "detail": "ประมาณการต้องอยู่ในสถานะรอตรวจสอบก่อนอนุมัติ",
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
  "errors": {}
}
```

## HTTP Mapping

| Status | ใช้เมื่อ | Frontend ทำอะไร |
| ---: | --- | --- |
| 400 | รูปแบบ Request ไม่ถูก | ผูก Error กับ Field |
| 401 | ไม่มี/หมดอายุ Identity | Refresh session หรือ Login |
| 403 | มีตัวตนแต่ไม่มี Permission | แสดง Forbidden |
| 404 | ไม่พบหรือ Resource อยู่นอก Scope | แสดง Not Found |
| 409 | Version/Concurrency conflict | ให้ Reload และตรวจข้อมูล |
| 422 | ผิด Business Rule | แสดงคำอธิบายจาก Backend |
| 429 | เรียกเกินข้อจำกัด | รอแล้วลองใหม่ตาม Header |
| 500/503 | ระบบภายในหรือบริการไม่พร้อม | ข้อความกลางและ Trace ID |

## Localization Ownership

- Backend `.resx`: Validation, Business Rule, Authorization และ System Error
- Frontend message catalog: ปุ่ม หัวข้อ Navigation และข้อความ UI-only
- `code` ไม่เปลี่ยนตามภาษาและห้าม Parse `title/detail`
- System Error Translation ไม่เก็บใน JSONB และไม่มี Runtime Editor ระยะแรก เพื่อลดความไม่ตรงกันและ Bug
- Log เก็บ Error Code และ Trace ID; ไม่จำเป็นต้องเก็บข้อความแปลเป็นตัวตัดสิน

## Authentication and Access Error Codes

| Code | HTTP | Meaning / ความหมาย |
| --- | ---: | --- |
| `AUTHENTICATION_REQUIRED` | 401 | Bearer token is missing / ไม่พบ Authorization Bearer token |
| `AUTHENTICATION_INVALID` | 401 | Token cannot be verified, has expired, or belongs to another Firebase project / Token ไม่ถูกต้อง หมดอายุ หรือมาจาก Firebase Project อื่น |
| `MEMBERSHIP_CONTEXT_REQUIRED` | 400 | Missing X-Membership-Id header or invalid UUID format / ไม่มี X-Membership-Id หรือรูปแบบไม่ใช่ UUID |
| `USER_ACCESS_DISABLED` | 403 | Internal User is disabled / บัญชีผู้ใช้ภายในระบบถูกระงับการใช้งาน |
| `ACTIVE_MEMBERSHIP_REQUIRED` | 403 | Identity is valid but no active Membership is available / ยืนยันตัวตนสำเร็จแต่ไม่มีสมาชิกภาพที่ Active ในองค์กรใด |
| `PERMISSION_DENIED` | 403 | Active Membership exists but the requested permission is absent / มีสมาชิกภาพในองค์กรแต่ไม่มีสิทธิ์สำหรับปฏิบัติการนี้ |
| `RESOURCE_NOT_FOUND` | 404 | Requested resource not found or belongs to another organization scope / ไม่พบ Resource หรืออยู่นอก Organization scope |

### ตัวอย่าง Problem Details ภาษาไทย (Thai Example)

```json
{
  "type": "https://tan-erp.local/problems/active-membership-required",
  "title": "จำเป็นต้องมีสมาชิกภาพที่ใช้งานได้",
  "status": 403,
  "code": "ACTIVE_MEMBERSHIP_REQUIRED",
  "detail": "ยืนยันตัวตนสำเร็จแต่ไม่พบสมาชิกภาพที่ใช้งานอยู่ในองค์กรใด",
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
  "errors": {}
}
```

### ตัวอย่าง Problem Details ภาษาอังกฤษ (English Example)

```json
{
  "type": "https://tan-erp.local/problems/active-membership-required",
  "title": "Active Membership Required",
  "status": 403,
  "code": "ACTIVE_MEMBERSHIP_REQUIRED",
  "detail": "Authentication succeeded but no active organization membership was found.",
  "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
  "errors": {}
}
```

## Quick Estimate Error Codes

| Code | HTTP | ความหมาย/การกู้คืน |
| --- | ---: | --- |
| `QUICK_ESTIMATE_INCOMPLETE` | 422 | ข้อมูลหรือ Assumption ที่ Template บังคับยังไม่ครบ; ระบุ Field ที่ต้องแก้ |
| `QUICK_ESTIMATE_REVIEW_REQUIRED` | 422 | Share Policy บังคับ Internal Review ก่อนแชร์ |
| `QUICK_ESTIMATE_TEMPLATE_EXPIRED` | 409 | Template Version ใช้แชร์ไม่ได้แล้ว; เลือก Version ปัจจุบันและ Recalculate |
| `QUICK_ESTIMATE_VERSION_CONFLICT` | 409 | มีการแก้ Version เดียวกันจากอีก Request; ให้ Reload/Compare |
| `QUICK_ESTIMATE_ALREADY_CONVERTED` | 409 | Source Version นี้เชื่อมกับ Official Estimate อยู่แล้ว; เปิดรายการเดิมแทน |
| `QUICK_ESTIMATE_UNIT_MISMATCH` | 422 | Unit ของ Input ไม่ตรงกับ Measurement Rule/Reference Rate; ให้แก้ Field หรือเลือก Template ใหม่ |
| `QUICK_ESTIMATE_RATE_UNAVAILABLE` | 409 | ไม่มี Reference Rate ที่ใช้ได้ ณ วันที่คำนวณ; เก็บ Draft และให้ผู้ดูแลราคาแก้ไข |
| `PRICING_TEMPLATE_INVALID` | 422 | Formula, Field, Unit หรือ Test Case ไม่ผ่าน; ห้าม Submit/Publish |
| `PRICING_TEMPLATE_NOT_USABLE` | 409 | Template อยู่ใน Draft, Superseded หรือ Disabled จึงเริ่มคำนวณ/แชร์ใหม่ไม่ได้; Calibration ยังใช้ได้โดยบังคับ Review |
| `PRICING_TEMPLATE_INVALID_STATE` | 409 | Action ใช้กับสถานะปัจจุบันไม่ได้ เช่น Publish ก่อน Approve |
| `PRICING_TEMPLATE_VERSION_CONFLICT` | 409 | `rowVersion` ไม่ตรงเพราะมีผู้แก้ข้อมูลใหม่กว่า; ให้ Reload/Compare |
| `PRICING_RATE_PERIOD_OVERLAP` | 409 | Rate/Branch Override ของ Item, Unit และ Scope เดียวกันมี Effective Period ซ้อนกัน |
| `PRICING_TEMPLATE_PILOT_NOT_PASSED` | 422 | Calibration/Pilot ยังไม่ผ่านเกณฑ์ จึง Activate ไม่ได้ |

## Shared Control Error Codes

| Code | HTTP | ความหมาย/การกู้คืน |
| --- | ---: | --- |
| `MAKER_CHECKER_VIOLATION` | 403 | ผู้จัดทำหรือผู้แก้ล่าสุดพยายามอนุมัติรายการที่ต้องแยกผู้ตรวจ |
| `IDEMPOTENCY_KEY_REUSED` | 409 | ใช้ Idempotency Key เดิมกับ Payload ต่างจากคำขอแรก; ต้องสร้าง Key ใหม่เมื่อเป็นเจตนาใหม่ |

## Item Master Error Codes

| Code | HTTP | ความหมาย/การกู้คืน |
| --- | ---: | --- |
| `ITEM_CODE_CONFLICT` | 409 | Code หลัง Normalize ซ้ำใน Organization เดียวกัน |
| `ITEM_VERSION_CONFLICT` | 409 | ETag เก่า; ให้ Reload/Compare ก่อนบันทึก |
| `ITEM_INVALID_STATE` | 409 | Action ไม่รองรับสถานะ เช่นแก้ Code หลังเคย Active |
| `ITEM_FIELD_REQUIRED` | 422 | Field ที่ Activation/Transition Gate บังคับยังไม่ครบ |
| `ITEM_UNIT_INVALID` | 422 | Unit ไม่ Active, คนละ Dimension หรือใช้กับ Item ไม่ได้ |
| `ITEM_CONVERSION_INVALID` | 422 | Factor/Period/Dimension ไม่ถูกหรือทำให้เกิด Cycle |
| `ITEM_COST_NOT_FOUND` | 404 | ไม่มี Published Cost ตรง Scope/Unit/Currency/Quantity/วันที่ |
| `ITEM_COST_AMBIGUOUS` | 409 | Candidate ยังเสมอกันหลัง Cost Resolution Rule ทุกข้อ |
| `ITEM_COST_PERIOD_OVERLAP` | 409 | Published Cost ใน Natural Key เดียวกันมี Period/Quantity Range ซ้อน |
| `ITEM_COST_VERSION_CONFLICT` | 409 | Cost Record ETag เก่า; ให้ Reload/Compare |
| `ITEM_COST_INVALID_STATE` | 409 | Cost Action ไม่ตรง Lifecycle เช่น Publish ก่อน Approve |
| `ITEM_IMPORT_INVALID` | 422 | Template/Row/Field ของ Import ไม่ผ่าน Validation |
| `ITEM_IMPORT_NOT_READY` | 409 | Batch ยัง Parse อยู่ มี Error หรือ Commit ไปแล้ว |

## CRM and Site Survey Error Codes

| Code | HTTP | ความหมาย/การกู้คืน |
| --- | ---: | --- |
| `CUSTOMER_CURSOR_INVALID` | 400 | Cursor ถอดรหัสหรือ validate ไม่ได้ |
| `CUSTOMER_VERSION_CONFLICT` | 409 | Customer ETag เก่า; ให้ Reload/Compare |
| `CUSTOMER_INVALID_STATE` | 409 | Customer Status ไม่รองรับ Action หรือใช้สร้างงานใหม่ไม่ได้ |
| `CUSTOMER_FIELD_REQUIRED` | 422 | ข้อมูลบังคับของ Customer (เช่น customerType/displayNameTh/preferredLocale) ไม่ผ่านกฎ |
| `CONTACT_FIELD_REQUIRED` | 422 | Contact ไม่มีช่องทางติดต่อขั้นต่ำ (ต้องมี phone หรือ email อย่างน้อยหนึ่งค่า) หรือ Field บังคับ |
| `OPPORTUNITY_VERSION_CONFLICT` | 409 | Opportunity ETag เก่า |
| `OPPORTUNITY_FIELD_REQUIRED` | 422 | Field ของ Stage เป้าหมายยังไม่ครบ |
| `OPPORTUNITY_INVALID_TRANSITION` | 409 | Stage Transition ไม่อยู่ใน Allowlist |
| `SITE_VERSION_CONFLICT` | 409 | Site ETag เก่า |
| `SURVEY_VERSION_CONFLICT` | 409 | Draft Revision ETag เก่า |
| `SURVEY_NOT_READY` | 422 | Measurement/Checklist/Evidence/Required Field ยังไม่ผ่าน Gate |
| `SURVEY_INVALID_STATE` | 409 | Action ไม่รองรับสถานะ เช่น Patch Ready Revision |
| `SURVEY_MEASUREMENT_INVALID` | 422 | Value/Unit/Derivation ไม่ถูกหรือมี Cycle |
| `SURVEY_FILE_NOT_READY` | 409 | Evidence File ยัง Upload/Validate ไม่เสร็จหรืออยู่นอก Scope |
| `SURVEY_REVISION_NOT_USABLE` | 422 | Revision ยัง Draft/Void หรือไม่ผ่าน Policy สำหรับ Estimate |
| `SURVEY_TEMPLATE_UNAVAILABLE` | 409 | ไม่มี Published Survey Template Version ที่ตรง Work Type/วันที่ |

## Official Estimate Error Codes

| Code | HTTP | ความหมาย/การกู้คืน |
| --- | ---: | --- |
| `ESTIMATE_VERSION_CONFLICT` | 409 | ETag/Revision ที่ส่งมาเก่ากว่าข้อมูลปัจจุบัน; ให้ Reload/Compare |
| `ESTIMATE_CALCULATION_OUTDATED` | 409 | Draft เปลี่ยนหลังคำนวณ; ต้อง Calculate ใหม่ก่อน Submit |
| `ESTIMATE_INVALID_STATE` | 409 | Action ไม่รองรับสถานะปัจจุบัน เช่น แก้ Approved Revision หรือออก Quotation จาก Draft |
| `ESTIMATE_COST_INCOMPLETE` | 422 | Work Item มี Cost Component หรือ Cost Source ไม่ครบ; ระบุ Field/Item ที่ต้องแก้ |
| `ESTIMATE_FIELD_REQUIRED` | 422 | Field ที่ Gate ปัจจุบันบังคับยังว่าง; คืน Field Pointer ที่แก้ได้ |
| `ESTIMATE_UNIT_INVALID` | 422 | Unit ไม่ Active หรือไม่เข้ากับ Item/Cost Source |
| `ESTIMATE_PROVISIONAL_COST_REASON_REQUIRED` | 422 | ใช้ต้นทุนชั่วคราวแต่ยังไม่มี Reason Code/คำอธิบายที่บังคับ |
| `ESTIMATE_POLICY_UNAVAILABLE` | 409 | Resolve Published Calculation/Tax/Approval Policy หรือ Independent Checker ไม่ได้; ห้าม Calculate/Submit ตาม Gate |

การ Retry Convert ด้วย **Idempotency Key เดิม** ต้องคืนผล Conversion เดิม ไม่คืน `QUICK_ESTIMATE_ALREADY_CONVERTED` รหัสนี้ใช้เมื่อเป็นคำขอใหม่ที่พยายาม Convert Source Version เดิมอีกครั้งโดยไม่ได้ระบุเจตนาสร้าง Revision ใหม่

## Validation Errors

`errors` เป็น Object ที่ Key ตรงกับ API field และ Value เป็น Array ของข้อความ เช่น `{"customerId":["กรุณาเลือกลูกค้า"]}`
