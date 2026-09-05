# API Conventions (ข้อตกลง API)

**สถานะ:** Accepted สำหรับอนาคต

## รูปแบบทั่วไป

- Base path: `/api/v1`
- ใช้คำนามพหูพจน์ เช่น `/api/v1/estimates`
- JSON field ใช้ `camelCase`; Identifier ใช้ UUID
- Timestamp ใช้ ISO 8601 UTC เช่น `2026-09-05T08:30:00Z`
- Money ส่ง `amount` และ `currency`; ห้ามใช้ Floating Point
- List endpoint รองรับ deterministic pagination และ explicit sorting
- Mutating request สำคัญรองรับ Idempotency Key ตาม Contract

## ตัวอย่าง Resource

```text
POST   /api/v1/estimates
GET    /api/v1/estimates/{estimateId}
POST   /api/v1/estimates/{estimateId}/submit
POST   /api/v1/estimates/{estimateId}/approve
POST   /api/v1/estimates/{estimateId}/revisions
```

Action endpoint ใช้เมื่อเป็น Business Transition ที่ไม่ควรสื่อเป็น CRUD ธรรมดา

## Contract Rules

- OpenAPI เป็น Machine-readable contract สำหรับสร้าง Type ฝั่ง Frontend
- Request/Response Contract อยู่ที่ API Boundary ไม่คืน Domain Entity โดยตรง
- Breaking change ใช้ API Version ใหม่หรือช่วงเปลี่ยนผ่านที่ประกาศชัด
- ทุก Request ส่ง `Accept-Language: th` หรือ `en`
- ทุก Response มี Correlation/Trace ID ที่ค้นในระบบติดตามได้
- List ต้องกำหนด Max Page Size และ Stable Tie-breaker เช่น `id`
