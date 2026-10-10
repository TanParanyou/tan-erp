# Organization Administration API Contract (G-03a)

**สถานะ:** Draft → Implemented เมื่อ G-03a เสร็จ ([Verification](../05-engineering/organization-administration-verification.md)). ตัดสินใจเชิงสถาปัตยกรรมใน [ADR 0019](../adr/0019-organization-branch-administration.md). ค่าที่ระบุ TEST_ONLY รอ Business Owner ยืนยัน.

## Endpoints

| Method | Path | Permission | หมายเหตุ |
| --- | --- | --- | --- |
| GET | `/api/v1/admin/organization` | `organizations.read` | คืน profile + `ETag` |
| PUT | `/api/v1/admin/organization` | `organizations.manage` | ต้องมี `If-Match`; คืน profile ใหม่ + `ETag` |
| GET | `/api/v1/admin/branches` | `branches.manage` | รวมสาขา inactive; `?status=active\|inactive` (ค่าว่าง = ทั้งหมด) |
| POST | `/api/v1/admin/branches` | `branches.manage` | ต้องมี `Idempotency-Key`; 201 + `ETag` |
| GET | `/api/v1/admin/branches/{id}` | `branches.manage` | other-org id → 404 |
| PUT | `/api/v1/admin/branches/{id}` | `branches.manage` | ต้องมี `If-Match`; ไม่รับ `code` |
| GET | `/api/v1/admin/branches/{id}/deactivation-check` | `branches.manage` | คืน `canDeactivate` + `blockers[]` |
| POST | `/api/v1/admin/branches/{id}/deactivate` | `branches.manage` | ต้องมี `If-Match` + body `{ "reason": string }` (1–500) |
| POST | `/api/v1/admin/branches/{id}/activate` | `branches.manage` | ต้องมี `If-Match` |

`BranchesController` เดิม `GET /api/v1/branches` (สาขา active สำหรับ dropdown, `organizations.read`) ไม่เปลี่ยน.

## Fields และ validation

| Field | Organization | Branch | กฎ |
| --- | --- | --- | --- |
| `name` | ต้องมี | ต้องมี | trim, ≤ 255 |
| `nameEn` | optional | optional | trim, ≤ 255, ว่าง = null |
| `code` | — | ต้องมีตอนสร้าง, แก้ไม่ได้ | `[A-Za-z0-9_-]`, ≤ 50, unique ต่อ Organization (case-sensitive ตามดัชนีเดิม `ix_branches_organization_id_branch_code`) |
| `taxIdentifier` | optional | — | 13 หลัก + checksum mod-11 |
| `taxBranchCode` | — | optional | 5 หลัก (`00000` = สำนักงานใหญ่), unique ต่อ Organization |
| `addressTh`, `addressEn` | optional | optional | ≤ 500 |
| `phone` | optional | optional | ≤ 30, ตัวเลข `+ - ( ) ` และช่องว่าง |
| `rowVersion` | response | response | = ค่าใน `ETag` |

## Branch response

`{ id, code, name, nameEn, taxBranchCode, addressTh, addressEn, phone, isActive, rowVersion, createdAtUtc }`

## Deactivation check response

`{ canDeactivate: boolean, blockers: [{ type: string, count: number }] }` โดย `type` เป็นหนึ่งใน
`estimates, quotations, purchase_orders, billings, work_orders, projects, installations, site_surveys, opportunities, quick_estimates, mrp_runs, warehouses, memberships, last_active_branch` (`count` ของ `last_active_branch` = 1).

## Errors

ดู [Error Contract](error-contract.md): `ORGANIZATION_TAX_ID_INVALID` 422, `BRANCH_CODE_INVALID` 422, `BRANCH_TAX_CODE_INVALID` 422, `BRANCH_CODE_ALREADY_EXISTS` 409, `BRANCH_TAX_CODE_ALREADY_EXISTS` 409, `BRANCH_HAS_OPEN_DOCUMENTS` 409, `BRANCH_HAS_ACTIVE_MEMBERSHIPS` 409, `BRANCH_LAST_ACTIVE` 409, `ADMIN_VERSION_CONFLICT` 409, `IDEMPOTENCY_KEY_REUSED` 409, `RESOURCE_NOT_FOUND` 404, `PERMISSION_DENIED` 403.

## Audit

`organization.profile-updated`, `branches.created`, `branches.updated`, `branches.deactivated` (มี `reason`), `branches.activated`. ค่าใน `changes` เป็นรายชื่อฟิลด์ที่เปลี่ยน ไม่ใส่ค่าที่อยู่/โทรศัพท์.
