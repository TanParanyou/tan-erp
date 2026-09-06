# Error Contract and Localization (ข้อผิดพลาดและภาษา)

**สถานะ:** Accepted

Backend ส่ง Error ตาม RFC 9457 Problem Details พร้อมรหัสคงที่ที่ Frontend ใช้ตัดสินใจ ข้อความแสดงผลแปลตาม `Accept-Language` และใช้ภาษาไทยเมื่อภาษาที่ขอไม่รองรับ

## ตัวอย่าง

```json
{
  "type": "https://tan-erp.local/problems/estimate-invalid-state",
  "title": "ไม่สามารถอนุมัติประมาณการได้",
  "status": 422,
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
| `MAKER_CHECKER_VIOLATION` | 403 | ผู้จัดทำหรือผู้แก้ล่าสุดพยายามอนุมัติรายการที่ต้องแยกผู้ตรวจ |
| `IDEMPOTENCY_KEY_REUSED` | 409 | ใช้ Idempotency Key เดิมกับ Payload ต่างจากคำขอแรก; ต้องสร้าง Key ใหม่เมื่อเป็นเจตนาใหม่ |

การ Retry Convert ด้วย **Idempotency Key เดิม** ต้องคืนผล Conversion เดิม ไม่คืน `QUICK_ESTIMATE_ALREADY_CONVERTED` รหัสนี้ใช้เมื่อเป็นคำขอใหม่ที่พยายาม Convert Source Version เดิมอีกครั้งโดยไม่ได้ระบุเจตนาสร้าง Revision ใหม่

## Validation Errors

`errors` เป็น Object ที่ Key ตรงกับ API field และ Value เป็น Array ของข้อความ เช่น `{"customerId":["กรุณาเลือกลูกค้า"]}`
