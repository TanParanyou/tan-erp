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

## Validation Errors

`errors` เป็น Object ที่ Key ตรงกับ API field และ Value เป็น Array ของข้อความ เช่น `{"customerId":["กรุณาเลือกลูกค้า"]}`
