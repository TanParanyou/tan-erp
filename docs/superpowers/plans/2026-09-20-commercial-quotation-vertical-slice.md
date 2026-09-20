# Commercial Quotation Vertical Slice Implementation Plan (Slice 5B)

> **Status:** Authorized Implementation (Hardening under `2026-09-20-official-estimate-commercial-hardening.md`)
> **Goal:** ออกใบเสนอราคาทางการ (Commercial Quotation) จาก Official Estimate ที่คำนวณแล้ว, รับการตอบรับจากลูกค้า (Customer Acceptance), ดำเนินการเปลี่ยนขั้นตอน Opportunity สู่ `proposed` และ `won` พร้อมระบบกำหนดเลขที่เอกสารอัตโนมัติ (Document Numbering Engine)

---

## 1. Authorized Exact Contracts

### 1.1 Issue Commercial Quotation
```http
POST /api/v1/estimates/{estimateId}/quotation
Authorization: Bearer <token>
Idempotency-Key: <16-128 chars>
Content-Type: application/json

{
  "expectedEstimateVersion": "<uuid>",
  "expectedOpportunityVersion": "<uuid>"
}
```
- **Permission:** `quotations.issue`
- **Preconditions:**
  - Estimate อยู่ในสถานะคำนวณแล้ว และ Opportunity อยู่ในขั้นตอน `estimating`
  - Replay check ต้องมาก่อน version/state check
- **Atomic Effects:**
  - จองเลขที่เอกสารอัตโนมัติผ่าน Atomic Sequence Engine (`IDocumentNumberGenerator`)
  - บันทึก Snapshot ของ Revision ที่คำนวณแล้ว
  - ปรับสถานะ Estimate / Revision เป็น `quoted`
  - ดำเนินการเปลี่ยนขั้นตอน Opportunity จาก `estimating` -> `proposed`
  - บันทึก Stage History 1 รายการ
  - บันทึก Privacy-safe Audit 2 รายการ
  - บันทึก Idempotency Record 1 รายการ

### 1.2 Accept Quotation
```http
POST /api/v1/estimates/{estimateId}/quotation/accept
Authorization: Bearer <token>
Idempotency-Key: <16-128 chars>
Content-Type: application/json

{
  "expectedOpportunityVersion": "<uuid>",
  "decisionNote": null
}
```
- **Permission:** `quotations.accept`
- **Preconditions:**
  - Quotation อยู่ในสถานะ `issued` และ Opportunity อยู่ในขั้นตอน `proposed`
  - Replay check ต้องมาก่อน version/state check
- **Atomic Effects:**
  - ปรับสถานะ Quotation เป็น `accepted`
  - ดำเนินการเปลี่ยนขั้นตอน Opportunity จาก `proposed` -> `won`
  - บันทึก Stage History 1 รายการ
  - บันทึก Privacy-safe Audit 2 รายการ (ห้ามบันทึก `decisionNote` ลงใน Audit)
  - บันทึก Idempotency Record 1 รายการ

### 1.3 Document Numbering Settings
```http
GET /api/v1/settings/document-sequences
Permission: document-sequences.read

POST /api/v1/settings/document-sequences/preview
Permission: document-sequences.manage

PUT /api/v1/settings/document-sequences/{documentType}
Permission: document-sequences.manage
If-Match: "<rowVersion>"
```
- Update คืน ETag / rowVersion ใหม่
- ตรวจสอบความถูกต้องของ Token อย่างเข้มงวด: ต้องมี `{SEQ}` เพียง 1 ครั้ง, reset period ต้องถูกต้อง ไม่อนุญาต Default เงียบ

---

## 2. Global Constraints & Invariants

1. **Replay Precedence:** Same key + same payload ส่งผลลัพธ์เดิม; Same key + different payload คืน `409 IDEMPOTENCY_KEY_REUSED`
2. **Atomic Counter:** ห้ามใช้ `COUNT + 1` fallback หากการจองเลขที่ล้มเหลวต้อง Rollback transaction และคืน RFC 9457 Problem Details `DOCUMENT_NUMBER_ALLOCATION_FAILED` (409)
3. **Frontend Row Versions:** Frontend ต้องส่ง row versions ของทั้ง Estimate และ Opportunity และเก็บ Idempotency Key ไว้ต่อ User Intent
4. **Deferred Scope:**
   - การแก้ไขหรือยกเลิกใบเสนอราคา (Quotation Amendment / Void)
   - การ Export PDF พร้อมจัดพิมพ์และระบบลายเซ็นดิจิทัล
   - ระบบ Customer Acceptance Portal ภายนอก
   - การสร้าง Project / Procurement / Production อัตโนมัติหลัง Won
