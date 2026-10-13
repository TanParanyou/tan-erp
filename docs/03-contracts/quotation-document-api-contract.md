# Quotation Document API Contract (ข้อตกลง API ไฟล์ PDF ใบเสนอราคาที่เก็บไว้)

**สถานะ:** Implemented (tests written, not yet run) 2026-10-13 (G-04); รอ Sales/Finance ยืนยัน layout/ข้อมูลหัวเอกสาร และรอ Business ยืนยันเงื่อนไข license ของ QuestPDF ([ADR 0023](../adr/0023-server-side-quotation-pdf-with-questpdf.md)). Preview/Print เดิม (`GET /api/v1/estimates/{id}/quotation/document`) ยังใช้ได้ตาม [ADR 0016](../adr/0016-browser-print-for-quotation-pdf.md).

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| เมื่อไหร่สร้างไฟล์ | ตอนออกใบเสนอราคา (`POST /api/v1/estimates/{id}/quotation`) และตอน amend ใน unit of work เดียวกัน; render พัง → rollback ทั้งหมด (`500 INTERNAL_SERVER_ERROR`) |
| ไฟล์ | `application/pdf` หนึ่งไฟล์ต่อ quotation, สองภาษาในไฟล์เดียว, A4; เก็บครั้งเดียว ไม่ re-render, ไม่มี endpoint แก้/ลบ/สร้างซ้ำ |
| ข้อมูลในไฟล์ | allowlist เท่านั้น: หัวเอกสารองค์กร/สาขา, ลูกค้าจาก billing snapshot ตอนออก, หมวด/รายการ (รหัส, รายละเอียด th/en, จำนวน, หน่วย, ราคาต่อหน่วย, รวม), ยอดสรุป; **ไม่มีต้นทุน/มาร์จิน/ข้อมูลภายใน** (บังคับด้วยชนิด `QuotationPdfModel` + test ด้วย reflection) |
| Hash | SHA-256 ของ bytes ที่เก็บ; ตรวจซ้ำทุกครั้งที่อ่าน; `ETag` = hash |
| ใบเสนอราคาเก่า | ไม่มีไฟล์ → `404 QUOTATION_DOCUMENT_NOT_AVAILABLE`; ใช้ Amend เพื่อออกฉบับใหม่ที่มีไฟล์ |
| Audit | `quotations.document-rendered` (เลขที่เอกสาร, sha256, byteCount, rendererVersion — ไม่มีข้อมูลลูกค้า); การดาวน์โหลดไม่ audit (สอดคล้องการอ่านอื่น) |

## Endpoints

| Action | Method/Path | Permission |
| --- | --- | --- |
| ดาวน์โหลดไฟล์ที่เก็บไว้ | `GET /api/v1/quotations/{id}/document` → `200 application/pdf` | `quotations.read` |
| สรุปเอกสารต่อใบ | `GET /api/v1/estimates/{id}/quotations` → แต่ละ item มี `document: {sha256, byteCount, rendererVersion, renderedAtUtc} \| null` | `quotations.read` |
| ลูกค้าดูไฟล์ | `GET /api/public/v1/quotation-acceptance/{token}/document` → `200 application/pdf` (`no-store`, rate limit เดิม); view ของลูกค้ามี `pdf: {sha256, byteCount} \| null` | token |

ไม่มี permission ใหม่. endpoint ภายในตรวจ Organization **และ Branch** (`access.HasBranchAccess`).

## Errors

`404`: `RESOURCE_NOT_FOUND` (นอก Organization/Branch — ตรวจก่อนตรวจว่ามีไฟล์), **`QUOTATION_DOCUMENT_NOT_AVAILABLE`** (ไม่มีไฟล์; ใหม่), `ACCEPTANCE_LINK_UNAVAILABLE` (public ทุกกรณี). `403`: `PERMISSION_DENIED`. `409`: `ACCEPTANCE_CONFLICT` (hash ที่ลูกค้าส่งไม่ตรงไฟล์ที่เก็บ). `500`: `INTERNAL_SERVER_ERROR` (render ล้มเหลวตอนออก).

## Data (migration `AddQuotationDocuments`)

`commercial.quotation_documents` (unique `quotation_id`, unique `file_id`, FK composite `(file_id, organization_id)` → `files.uploaded_files`; check `sha256 ~ ^[0-9a-f]{64}$`, `byte_count > 0`); `commercial.quotation_acceptance_evidences.document_sha256` (nullable).
