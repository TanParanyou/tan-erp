# Identity Administration API Contract (CP-02)

**สถานะ:** Implemented (2026-10-03) ตามแผน [CP-02](../superpowers/plans/2026-10-03-identity-organization-administration.md). ชื่อ Permission เป็นค่าเริ่มต้นที่เสนอ รอเจ้าของงานยืนยัน; ชื่อ Role จริงยังไม่กำหนด

ผู้ดูแลที่ได้รับอนุญาตจัดผู้ใช้ Membership และ Role Assignment ภายใน Organization ของตนเท่านั้น. PostgreSQL เป็นเจ้าของสิทธิ์; Firebase เป็น Identity เท่านั้น ตาม [Authentication](authentication.md) และ [RBAC](rbac.md). Resource นอก Organization ตอบ `404`.

## Permission

| Permission | ใช้กับ |
| --- | --- |
| `users.read` | ดูรายการ/รายละเอียดผู้ใช้และ Membership ใน Organization |
| `users.manage` | เพิ่มผู้ใช้ (pending), แก้ชื่อ, เปิด/ปิดใช้ผู้ใช้ |
| `memberships.manage` | แก้สาขา/ช่วงเวลา, เปิด/ปิด Membership |
| `roles.assign` | ดู Role ที่มอบได้, มอบ/ถอน Role, ยกเลิกคำขอของตนเอง |
| `roles.assign-approval` | ดูและอนุมัติ/ปฏิเสธคำขอมอบ Role ที่มี Approval Permission |

สิทธิ์ทั้งหมดต้องเป็น Scope `organization`.

## กฎธุรกิจ

1. **เพิ่มผู้ใช้:** สร้าง User สถานะ pending (ยังไม่มี Firebase UID) พร้อม Membership. อีเมลซ้ำ (normalize trim + lower-case) คืน `409 USER_EMAIL_ALREADY_EXISTS` ทั้งระบบ; รอบแรกไม่รองรับการเชิญอีเมลที่มีอยู่แล้วในองค์กรอื่น
2. **ผูกตัวตน:** `GET /api/v1/me` ผูก UID ให้ pending user ที่ active เมื่อ token มี `email_verified = true` และอีเมลตรงเท่านั้น; ผู้ใช้ที่ผูกแล้วไม่ถูกผูกซ้ำ; ไม่มี email_verified ไม่ผูก; บันทึก Audit `users.identity-linked`
3. **Last administrator:** "ผู้ดูแล" คือผู้ใช้ active ที่มี Membership active ซึ่งมี `users.manage` ระดับ organization. การถอน Role, ปิด Membership หรือปิดผู้ใช้ที่ทำให้ Organization ไม่เหลือผู้ดูแล active คืน `422 LAST_ADMINISTRATOR_REQUIRED` (ตรวจใน transaction แบบ serializable)
4. **Anti-escalation:** มอบ Role ได้เมื่อ permission ทุกรายการของ Role อยู่ในชุดที่ผู้มอบถืออยู่ (key เดียวกัน, Scope เท่ากันหรือกว้างกว่า) มิฉะนั้น `403 ROLE_ESCALATION_DENIED`; มอบ/ถอน Role ของ Membership ตนเองไม่ได้ (`403 SELF_ROLE_CHANGE_FORBIDDEN`)
5. **Maker–checker:** Role ที่มี `estimates.approve`, `cost-records.approve` หรือ `cost-records.publish` ไม่มีผลทันที: การมอบสร้างคำขอ `pending` (`202`). ผู้ตัดสินต้องมี `roles.assign-approval`, เป็นคนละคนกับผู้ขอ และผ่านกฎ anti-escalation. คำขอที่ pending ซ้ำ Membership+Role เดียวกันคืน `409`
6. **มีผลทันที:** ผลการถอนสิทธิ์/ปิดใช้ถูกตรวจจาก PostgreSQL ทุก request (ไม่มี cache ของสิทธิ์)
7. **Audit:** ทุกการเปลี่ยนแปลงบันทึก AuditEvent (ผู้กระทำ, resource, ก่อน/หลัง row version, trace id). ไม่เก็บอีเมล/ชื่อใน `changes`

## Endpoints

| Method | Path | Permission | หมายเหตุ |
| --- | --- | --- | --- |
| GET | `/api/v1/admin/users` | `users.read` | `search`, `status` (`pending|active|inactive`), `page`, `pageSize` |
| GET | `/api/v1/admin/users/{userId}` | `users.read` | ETag = `rowVersion` |
| POST | `/api/v1/admin/users` | `users.manage` + `memberships.manage` + `roles.assign` | 201; body `{ displayName, email, branchId?, roleIds[] }` |
| PATCH | `/api/v1/admin/users/{userId}` | `users.manage` | `If-Match`; body `{ displayName }` |
| POST | `/api/v1/admin/users/{userId}/deactivate` | `users.manage` | `If-Match`; last-admin guard |
| POST | `/api/v1/admin/users/{userId}/activate` | `users.manage` | `If-Match` |
| PATCH | `/api/v1/admin/memberships/{membershipId}` | `memberships.manage` | `If-Match`; `{ branchId?, startsAtUtc?, expiresAtUtc? }` |
| POST | `/api/v1/admin/memberships/{membershipId}/deactivate` | `memberships.manage` | `If-Match`; last-admin guard |
| POST | `/api/v1/admin/memberships/{membershipId}/activate` | `memberships.manage` | `If-Match` |
| GET | `/api/v1/admin/roles` | `roles.assign` | Role ใน Organization พร้อม `assignable`, `requiresApproval` |
| POST | `/api/v1/admin/memberships/{membershipId}/roles` | `roles.assign` | body `{ roleId }`; 201 assigned หรือ 202 pending request |
| DELETE | `/api/v1/admin/memberships/{membershipId}/roles/{roleId}` | `roles.assign` | 204; last-admin guard |
| GET | `/api/v1/admin/role-assignment-requests` | `roles.assign-approval` | `status` default `pending` |
| POST | `/api/v1/admin/role-assignment-requests/{requestId}/approve` | `roles.assign-approval` | `If-Match`; ผู้ขอ approve เองไม่ได้ (`403 ROLE_ASSIGNMENT_INDEPENDENT_CHECKER_REQUIRED`) |
| POST | `/api/v1/admin/role-assignment-requests/{requestId}/reject` | `roles.assign-approval` | `If-Match` |
| POST | `/api/v1/admin/role-assignment-requests/{requestId}/cancel` | `roles.assign` | `If-Match`; เฉพาะผู้ขอ |

Mutating request ไม่ใช้ Idempotency-Key ในรอบนี้: ความซ้ำถูกกันด้วย unique constraint (อีเมล, คำขอ pending) และ `If-Match`.

## User resource (ย่อ)

```json
{
  "id": "<guid>", "displayName": "…", "email": "…", "status": "pending|active|inactive",
  "rowVersion": "<guid>",
  "memberships": [{
    "id": "<guid>", "isActive": true, "rowVersion": "<guid>",
    "branch": { "id": "<guid>", "name": "…" },
    "startsAtUtc": null, "expiresAtUtc": null,
    "roles": [{ "id": "<guid>", "name": "…" }],
    "pendingRoleRequests": [{ "id": "<guid>", "rowVersion": "<guid>", "role": { "id": "<guid>", "name": "…" }, "requestedBy": { "id": "<guid>", "displayName": "…" }, "requestedAtUtc": "…" }]
  }]
}
```

## Error codes ใหม่

`USER_EMAIL_ALREADY_EXISTS` 409, `ADMIN_VERSION_CONFLICT` 409, `LAST_ADMINISTRATOR_REQUIRED` 422, `ROLE_ESCALATION_DENIED` 403, `SELF_ROLE_CHANGE_FORBIDDEN` 403, `ROLE_ALREADY_ASSIGNED` 409, `ROLE_ASSIGNMENT_REQUEST_PENDING` 409, `ROLE_ASSIGNMENT_REQUEST_NOT_PENDING` 409, `ROLE_ASSIGNMENT_INDEPENDENT_CHECKER_REQUIRED` 403, `ROLE_NOT_ASSIGNED` 404 (นอกจากนั้นใช้ `RESOURCE_NOT_FOUND`, `PERMISSION_DENIED`, `IF_MATCH_REQUIRED`, `REQUEST_VALIDATION_FAILED` ที่มีอยู่)

## Data

`identity_access.users.firebase_uid` เป็น nullable (unique เมื่อไม่ null), เพิ่ม `normalized_email` (unique เมื่อไม่ว่าง) และ `row_version`; `organization.memberships.row_version`; ตารางใหม่ `identity_access.role_assignment_requests` (status `pending|approved|rejected|cancelled`, check constraint ผู้ตัดสินต้องไม่ใช่ผู้ขอ ยกเว้น cancelled, unique pending ต่อ membership+role). Migration `AddIdentityAdministration` backfill `normalized_email`/`row_version` และ rollback ได้; rehearsal up/down/up บน DB ที่มีข้อมูลผ่าน 2026-10-03.

## Recovery runbook (ผู้ดูแลคนสุดท้ายหาย)

ขั้นตอนอยู่ใน [Administrator Recovery Runbook](../06-operations/administrator-recovery.md).
