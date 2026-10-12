# Role Management API Contract (G-03b)

**สถานะ:** Implemented (tests written, not yet run) — G-03b, 2026-10-11 ([Verification](../05-engineering/role-management-verification.md)). ตัดสินใจเชิงสถาปัตยกรรมใน [ADR 0020](../adr/0020-role-management-and-approval-permission-set.md).

## Endpoints

| Method | Path | Permission | หมายเหตุ |
| --- | --- | --- | --- |
| GET | `/api/v1/admin/permissions` | `roles.manage` | catalog `{ items: [{ key, description, isApprovalPermission }] }` |
| GET | `/api/v1/admin/role-definitions` | `roles.manage` | `search,status,sortBy,sortOrder,page,limit` → `{ items, pagination }` |
| GET | `/api/v1/admin/role-definitions/{id}` | `roles.manage` | รายละเอียด + grants + `ETag`; other-org id → 404 |
| POST | `/api/v1/admin/role-definitions` | `roles.manage` | `Idempotency-Key`; 201 + `ETag` |
| PUT | `/api/v1/admin/role-definitions/{id}` | `roles.manage` | `If-Match`; 200 (มีผลแล้ว) หรือ 202 (สร้างคำขอ) |
| POST | `/api/v1/admin/role-definitions/{id}/deactivate` / `activate` | `roles.manage` | `If-Match` |
| GET | `/api/v1/admin/role-change-requests?status=pending` | `roles.assign-approval` | คิวผู้ตรวจ พร้อม `addedPermissionKeys/removedPermissionKeys` |
| POST | `/api/v1/admin/role-change-requests/{id}/approve` / `reject` | `roles.assign-approval` | `If-Match` (rowVersion ของคำขอ) |
| POST | `/api/v1/admin/role-change-requests/{id}/cancel` | `roles.manage` | `If-Match`; ผู้ขอเท่านั้น |

`GET /api/v1/admin/roles` เดิมคือรายการ Role สำหรับมอบ (`roles.assign`) ไม่เปลี่ยนและไม่ใช้ร่วมกับ endpoint ในหน้านี้.

## Fields และ validation

| Field | กฎ |
| --- | --- |
| `name` | trim, 3–100 ตัวอักษร, ไม่ซ้ำแบบ case-insensitive ในองค์กร (`NormalizedName`), ห้ามขึ้นต้น `system.` (case-insensitive) |
| `description` | optional, trim, ≤ 500, ว่าง = null |
| `permissions[]` | `{ permissionKey, scope, branchId? }`; ≤ 200; key ต้องอยู่ใน catalog และ active; `scope` ∈ `organization`, `branch`, `own`; `branchId` บังคับเมื่อ `branch` และห้ามส่งกับสองแบบที่เหลือ; รายการซ้ำถูกรวม |
| `rowVersion` | response; ค่าเดียวกับ `ETag` / `If-Match` |

## Response

- **รายการ** (`GET /role-definitions`): `{ items, pagination }` ใช้ `AdminPaginationResponse` เดิม; `search` ค้นชื่อ/คำอธิบาย (escape `%` และ `_`), `status=active|inactive`, `sortBy=name|createdAt`, `sortOrder`, `page`, `limit` ≤ 100 (tie-breaker `Id`). ค่า sort/status ผิดคืน `400 REQUEST_VALIDATION_FAILED`.
- **รายละเอียด** (`GET /role-definitions/{id}`): Role + `permissions[]` + `isSystem` + `rowVersion` พร้อม `ETag`.
- **`PUT`**: `200` เมื่อมีผลแล้ว (คืนรายละเอียดใหม่ + `ETag`) หรือ `202` เมื่อสร้างคำขอ `RoleChangeRequest` (Role ยังไม่เปลี่ยนจนกว่าจะอนุมัติ).

## กฎธุรกิจ

1. **System Role** แก้/ปิด/เปิดไม่ได้ (`ROLE_SYSTEM_IMMUTABLE`); API ไม่มีทางตั้ง `is_system` (ADR 0020 การตัดสินใจ 1).
2. **anti-escalation** ตรวจเฉพาะ grant ที่เพิ่ม ด้วย `Covers` เดิม; การถอน grant ทำได้เสมอ; การลบ `users.manage` จากแหล่งผู้ดูแลคนสุดท้ายถูกปฏิเสธด้วย `LAST_ADMINISTRATOR_REQUIRED` (การตัดสินใจ 2).
3. **maker–checker** เมื่อเพิ่ม grant ให้ Role ที่มีผู้ถือ ≥ 1 และ Role หลังแก้ `RequiresApproval`; ผู้ตัดสินต้องเป็นคนละคนกับผู้ขอ และต้องถือ `roles.assign-approval` พร้อม `Covers` grant ที่เพิ่ม (การตัดสินใจ 3).
4. **ปิด Role** ต้องไม่มีผู้ถือ (`ROLE_IN_USE`) และไม่มีคำขอ pending (`ROLE_CHANGE_REQUEST_PENDING`); ไม่มีการถอน Role อัตโนมัติ; เปิดใช้ได้เสมอ (การตัดสินใจ 4).
5. **Approval Permission Set** มี 6 คีย์ตาม ADR 0020 การตัดสินใจ 5 (ชุดนี้ล็อกด้วย test).
6. **ทุก endpoint** จำกัดที่ Organization ของผู้เรียก: id ของต่างองค์กร = `404`; ไม่มีสิทธิ์ = `403` เหมือนกันทุก id.

## Errors

| Code | Status | ที่มา |
| --- | --- | --- |
| `ROLE_NAME_ALREADY_EXISTS` | 409 | ใหม่ |
| `ROLE_NAME_RESERVED` | 422 | ใหม่ |
| `ROLE_PERMISSION_UNKNOWN` | 422 | ใหม่ |
| `ROLE_SYSTEM_IMMUTABLE` | 409 | ใหม่ |
| `ROLE_IN_USE` | 409 | ใหม่ |
| `ROLE_CHANGE_REQUEST_PENDING` | 409 | ใหม่ |
| `ROLE_CHANGE_REQUEST_NOT_PENDING` | 409 | ใหม่ |
| `ROLE_CHANGE_REQUEST_STALE` | 409 | ใหม่ |
| `ROLE_CHANGE_INDEPENDENT_CHECKER_REQUIRED` | 403 | ใหม่ |
| `ROLE_ESCALATION_DENIED` | — | ใช้ซ้ำ |
| `LAST_ADMINISTRATOR_REQUIRED` | — | ใช้ซ้ำ |
| `BRANCH_NOT_FOUND` | — | ใช้ซ้ำ |
| `ADMIN_VERSION_CONFLICT` | — | ใช้ซ้ำ |
| `IDEMPOTENCY_KEY_REUSED` | — | ใช้ซ้ำ |
| `REQUEST_VALIDATION_FAILED` | — | ใช้ซ้ำ |
| `RESOURCE_NOT_FOUND` | — | ใช้ซ้ำ |
| `PERMISSION_DENIED` | — | ใช้ซ้ำ |

รายละเอียดรหัสอยู่ใน [Error Contract](error-contract.md).

## Audit

Action: `roles.created`, `roles.updated`, `roles.change-requested`, `roles.change-approved`, `roles.change-rejected`, `roles.change-cancelled`, `roles.deactivated`, `roles.activated`.

เก็บ id, ชื่อฟิลด์ที่เปลี่ยน, จำนวน และ permission key ที่เพิ่ม/ถอน. ห้ามเก็บชื่อหรือคำอธิบายของ Role ใน audit.
