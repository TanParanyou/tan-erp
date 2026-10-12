# Approval Limit API Contract (G-03c)

**สถานะ:** Draft → Implemented เมื่อ G-03c เสร็จ ([Verification](../05-engineering/approval-authority-verification.md)). ตัดสินใจเชิงสถาปัตยกรรมใน [ADR 0021](../adr/0021-approval-authority-matrix.md).

## Endpoints

| Method | Path | Permission | หมายเหตุ |
| --- | --- | --- | --- |
| GET | `/api/v1/admin/approval-limits` | `approval-limits.manage` | `roleId,documentType,status=active\|inactive,page,limit` → `{ items, pagination }` |
| GET | `/api/v1/admin/approval-limits/options` | `approval-limits.manage` | `{ roles[], branches[], documentTypes[], currencies[] }` |
| GET | `/api/v1/admin/approval-limits/{id}` | `approval-limits.manage` | + `ETag`; other-org id → 404 |
| POST | `/api/v1/admin/approval-limits` | `approval-limits.manage` | `Idempotency-Key`; 201 + `ETag` |
| PUT | `/api/v1/admin/approval-limits/{id}` | `approval-limits.manage` | `If-Match`; 200 + `ETag` |

ไม่มี endpoint ลบ. การปิดใช้ด้วย `isActive=false` เก็บประวัติไว้ใน audit.

`options` แยกออกเพื่อไม่ผูก `approval-limits.manage` กับ `roles.manage` หรือ `organizations.read`.

## Fields และ validation

| Field | กฎ |
| --- | --- |
| `roleId` | บังคับตอนสร้าง; ต้องเป็น Role active ในองค์กร (ไม่เช่นนั้น `404 APPROVAL_LIMIT_ROLE_NOT_FOUND`); เปลี่ยนไม่ได้หลังสร้าง |
| `documentType` | `estimate`, `purchase_order`, `change_order` (`credit_note` สงวน — `400 REQUEST_VALIDATION_FAILED`); เปลี่ยนไม่ได้ |
| `branchId` | optional; null = ทั้งองค์กร; ต้องเป็นสาขา active ในองค์กร (`404 BRANCH_NOT_FOUND`); เปลี่ยนไม่ได้ |
| `currency` | `THB` เท่านั้น |
| `isUnlimited` / `maxAmount` | `isUnlimited=true` ต้องไม่ส่ง `maxAmount`; มิฉะนั้นต้องส่ง `0 ≤ maxAmount ≤ 9,999,999,999,999.99` ทศนิยมไม่เกิน 2 ตำแหน่ง |
| `isActive` | แก้ได้เฉพาะ PUT; แถวที่ไม่ active ถือเสมือนไม่มีแถว |
| `rowVersion` | response; ค่าเดียวกับ `ETag` / `If-Match` |

ความซ้ำ: หนึ่งแถวต่อ `(organization, role, document_type, branch หรือทั้งองค์กร, currency)` นับทั้ง active และ inactive. สร้างซ้ำคืน `409 APPROVAL_LIMIT_ALREADY_EXISTS` (ให้แก้แถวเดิมแทน).

## Response

```json
{
  "id": "uuid",
  "roleId": "uuid",
  "roleName": "string",
  "documentType": "estimate",
  "branchId": "uuid | null",
  "branchName": "string | null",
  "currency": "THB",
  "isUnlimited": false,
  "maxAmount": "1500000.00 | null",
  "isActive": true,
  "rowVersion": 1
}
```

Response ของ list: `{ items: [...], pagination }` ตามรูปแบบ `AdminPaginationResponse`.

## กฎธุรกิจ

ยึด [ADR 0021](../adr/0021-approval-authority-matrix.md) การตัดสินใจ 1-5 เป็นแหล่งหลัก. ทุก endpoint จำกัดที่ Organization ของผู้เรียก: id ขององค์กรอื่นตอบ `404`; ผู้ไม่มีสิทธิ์ได้ `403 PERMISSION_DENIED` ทุก id เหมือนกัน.

## Error

| รหัส | HTTP | ที่มา |
| --- | --- | --- |
| `APPROVAL_LIMIT_ALREADY_EXISTS` | 409 | สร้างแถวซ้ำกับ key เดิม |
| `APPROVAL_LIMIT_ROLE_NOT_FOUND` | 404 | ไม่พบ Role active ที่ระบุในองค์กร |
| `APPROVAL_LIMIT_EXCEEDED` | 403 | flow อนุมัติ: ยอดเอกสารเกินเพดานสูงสุดของ Role ที่ผู้อนุมัติถือ |
| `APPROVAL_AUTHORITY_NOT_CONFIGURED` | 403 | flow อนุมัติ: ไม่มีวงเงิน active สำหรับ Role/ประเภท/สาขา/สกุลนี้ (fail-closed) |

รหัสที่ใช้ซ้ำ: `BRANCH_NOT_FOUND`, `ADMIN_VERSION_CONFLICT`, `IDEMPOTENCY_KEY_REUSED`, `REQUEST_VALIDATION_FAILED`, `RESOURCE_NOT_FOUND`, `PERMISSION_DENIED`.

## Audit

| Action | เมื่อ |
| --- | --- |
| `approval-limits.created` | สร้างแถววงเงิน |
| `approval-limits.updated` | แก้ `isUnlimited`, `maxAmount` หรือ `isActive` (บันทึกค่าก่อน–หลังและ row version) |

## ผลต่อ flow อนุมัติ

| ประเภทเอกสาร | ยอดที่ตรวจ | สาขา | สกุล | จุดที่ตรวจ |
| --- | --- | --- | --- | --- |
| `estimate` | `revision.GrandTotal` | `estimate.BranchId` | `revision.Currency` | step สุดท้ายของ route เมื่อ decision = `approved` เท่านั้น (ADR 0021 การตัดสินใจ 3) |
| `purchase_order` | `PurchaseOrder.TotalAmount` | `order.BranchId` | `order.Currency` | ตอน Approve (Reject ไม่ตรวจ) |
| `change_order` | `max(abs(ContractDelta), abs(BudgetDelta))` | `project.BranchId` | THB | ตอน Approve (Reject ไม่ตรวจ); โครงการไม่มีคอลัมน์สกุลเงิน |
