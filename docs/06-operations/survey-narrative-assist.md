# Survey Narrative Assist: การตั้งค่า AI ขัดเกลาข้อความ

เอกสารนี้สำหรับผู้ดูแลระบบที่ต้องการเปิดปุ่ม "ขัดเกลาข้อความด้วย AI" ในหน้า Field Survey (`/th/surveys/field`) ฟีเจอร์ปิดอยู่เป็นค่าเริ่มต้น

## สถานะปัจจุบัน

- ตั้งค่าได้ที่ฝั่ง backend ผ่าน configuration หรือ environment variable เท่านั้น ยังไม่มีหน้าเว็บสำหรับตั้งค่า
- รองรับ provider เดียวคือ `gemini`
- endpoint: `POST /api/v1/surveys/narrative/polish` ต้องมีสิทธิ์ `surveys.update`
- ยังไม่เคยทดสอบการเรียก Gemini จริง

## ค่าที่ต้องตั้ง (section `SurveyNarrative`)

| ค่า | ความหมาย |
| --- | --- |
| `Provider` | ต้องเป็น `gemini` (ค่าเริ่มต้น `none` = ปิด) |
| `ApiKey` | API key จาก Google AI Studio |
| `Model` | ชื่อ model ที่บัญชีของคุณใช้ได้ ดูจาก AI Studio |
| `DataTermsAcknowledged` | `true` เมื่อผู้ดูแลอ่านเงื่อนไขการใช้ข้อมูลของ provider แล้ว |

ต้องตั้งครบทั้ง 4 ค่า ขาดค่าใดค่าหนึ่งฟีเจอร์จะปิดอยู่ ปุ่มในหน้าเว็บจะถูกซ่อน และ endpoint ตอบ 422 `SURVEY_NARRATIVE_DISABLED`

## วิธีตั้งค่า

ใน .NET ใช้ `__` คั่นระดับของ section

```bash
export SurveyNarrative__Provider=gemini
export SurveyNarrative__ApiKey=<API key>
export SurveyNarrative__Model=<ชื่อ model>
export SurveyNarrative__DataTermsAcknowledged=true
```

จากนั้นรัน backend ใหม่

```bash
make dev-backend
```

## ตรวจว่าเปิดแล้ว

1. เปิด `/th/surveys/field` ด้วยบัญชีที่มีสิทธิ์ `surveys.update`
2. ช่องหมายเหตุต้องมีปุ่มขัดเกลาข้อความด้วย AI
3. กดแล้วต้องได้ข้อความที่ขัดเกลาแล้วกลับมา

## ข้อควรระวัง

- **ห้าม commit API key** ห้ามใส่ใน `appsettings.json` หรือไฟล์ใดที่เข้า Git ให้ใช้ environment variable, user-secrets หรือ secret store ของ environment นั้น
- **ข้อมูลที่ส่งออกไปภายนอก:** backend ลบข้อมูลที่ระบุตัวตนบางประเภทก่อนส่ง (redactor) แต่ไม่ครอบคลุมทุกกรณี
- **tier ฟรีของ Google** อาจนำข้อความที่ส่งไปปรับปรุงผลิตภัณฑ์ได้ ใช้กับข้อมูลทดสอบ (`TEST_ONLY`) เท่านั้น ห้ามส่งข้อมูลลูกค้าจริงจนกว่าจะใช้ tier ที่เงื่อนไขอนุญาต
- การตั้ง `DataTermsAcknowledged=true` เป็นการยืนยันของผู้ดูแลว่าอ่านเงื่อนไขแล้ว อย่าตั้งแทนผู้อื่น

## เมื่อเกิดปัญหา

| อาการ | สาเหตุที่เป็นไปได้ |
| --- | --- |
| ไม่เห็นปุ่ม | ตั้งค่าไม่ครบ 4 ค่า หรือไม่ได้รัน backend ใหม่ หรือ reference-data ถูก HTTP cache ไว้ (สูงสุด 1 ชั่วโมง) ให้ refresh แบบ hard reload |
| 422 `SURVEY_NARRATIVE_DISABLED` | ตั้งค่าไม่ครบ หรือ `Provider` ไม่ใช่ `gemini` |
| 502 `SURVEY_NARRATIVE_PROVIDER_FAILED` | key ผิด, ชื่อ model ผิด, เกินโควตา หรือเครือข่ายออกไป Google ไม่ได้ ดู log ของ backend |
| 403 | บัญชีไม่มีสิทธิ์ `surveys.update` |

## ที่มา

แผนและเหตุผลการออกแบบ: [Field Survey Smart Assist](../superpowers/plans/2026-11-02-field-survey-smart-assist.md)
