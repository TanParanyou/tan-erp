# API Conventions (ข้อตกลง API)

**สถานะ:** Accepted สำหรับอนาคต

## รูปแบบทั่วไป

- Base path: `/api/v1`
- ใช้คำนามพหูพจน์ เช่น `/api/v1/estimates`
- JSON field ใช้ `camelCase`; Identifier ใช้ UUID
- Timestamp ใช้ ISO 8601 UTC เช่น `2026-09-05T08:30:00Z`
- Money ส่ง `amount` และ `currency`; ห้ามใช้ Floating Point
- List endpoint รองรับ deterministic pagination และ explicit sorting โดยจัดกลุ่ม Metadata ไว้ใน nested object `pagination: { page, pageSize, totalCount, totalPages, nextCursor }`
- Mutating request สำคัญรองรับ Idempotency Key ตาม Contract

## ตัวอย่าง Resource

```text
POST   /api/v1/estimates
GET    /api/v1/estimates/{estimateId}
POST   /api/v1/estimates/{estimateId}/submit
POST   /api/v1/estimates/{estimateId}/review-decisions
POST   /api/v1/estimates/{estimateId}/cancel
POST   /api/v1/estimates/{estimateId}/revisions
```

Action endpoint ใช้เมื่อเป็น Business Transition ที่ไม่ควรสื่อเป็น CRUD ธรรมดา

## Current User (`GET /api/v1/me`)

Endpoint สำหรับส่งมอบบริบทผู้ใช้หลังยืนยันตัวตนด้วย Firebase Token โดย Backend จะค้นหาข้อมูลผู้ใช้ภายใน, Membership, Organization, Branch และสิทธิ์ที่มีผล (Effective Permissions) จาก PostgreSQL

### Success Response (`200 OK`)

```json
{
  "user": {
    "id": "019a3cf8-96f0-7c9f-b207-93aa818f4a10",
    "displayName": "ผู้ใช้ TEST_ONLY",
    "email": "foundation-user@example.test"
  },
  "memberships": [
    {
      "id": "019a3cf8-96f0-7c9f-b207-93aa818f4a11",
      "organization": {
        "id": "019a3cf8-96f0-7c9f-b207-93aa818f4a12",
        "name": "TEST_ONLY Project ERP"
      },
      "branch": {
        "id": "019a3cf8-96f0-7c9f-b207-93aa818f4a13",
        "name": "สาขาทดสอบ"
      },
      "permissions": [
        {
          "key": "organizations.read",
          "scope": "organization",
          "scopeId": "019a3cf8-96f0-7c9f-b207-93aa818f4a12"
        }
      ]
    }
  ]
}
```

### Contract Rules ของ `GET /api/v1/me`

- คืนเฉพาะข้อมูลที่ Active เท่านั้น: User ที่ active, Membership ที่ active ในช่วงเวลาปัจจุบัน, Organization ที่ active, Branch ที่ active และ Role/Permission ที่ active
- เรียงลำดับ deterministic: เรียง `memberships` ตามชื่อ Organization แล้วตาม Membership ID; เรียง `permissions` ตาม `key`, `scope` และ `scopeId`
- ยุบรวม (Collapse/Deduplicate) Permission ที่ซ้ำกันจากหลาย Role ด้วย `(key, scope, scopeId)`
- **ห้าม** ส่งคืน Firebase UID, ชื่อ Role หรือ Custom Claims กลับไปยัง Client; สิทธิ์และบริบททั้งหมดต้องมาจาก PostgreSQL


Endpoint และ Payload เฉพาะ Mobile Quick Estimate อยู่ที่ [Quick Estimate API Contract](quick-estimate-api-contract.md)

Endpoint และ Payload ของ Official Estimate/BOQ อยู่ที่ [Official Estimate API Contract](official-estimate-api-contract.md)

Endpoint, Lifecycle และ Cost Resolver ของ Item/Unit/Cost อยู่ที่ [Item Master API Contract](item-master-api-contract.md)

Endpoint, Stage และ Ready Survey Revision ของ Customer/Opportunity/Site Survey อยู่ที่ [CRM and Site Survey API Contract](crm-site-survey-api-contract.md)

## Contract Rules

- OpenAPI เป็น Machine-readable contract สำหรับสร้าง Type ฝั่ง Frontend
- Request/Response Contract อยู่ที่ API Boundary ไม่คืน Domain Entity โดยตรง
- Breaking change ใช้ API Version ใหม่หรือช่วงเปลี่ยนผ่านที่ประกาศชัด
- ทุก Request ส่ง `Accept-Language: th` หรือ `en`
- ทุก Response มี Correlation/Trace ID ที่ค้นในระบบติดตามได้
- List ต้องกำหนด Max Page Size และ Stable Tie-breaker เช่น `id`
