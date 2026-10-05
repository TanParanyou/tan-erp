# Shared Attachment & Signature API Contract (ข้อตกลง API ไฟล์แนบและลายเซ็นกลาง)

**สถานะ:** Draft → Implemented เมื่อ G-01 เสร็จ (ดู [Verification](../05-engineering/shared-attachment-signature-verification.md)). กฎเป็นค่าเริ่มต้น TEST_ONLY รอ Security/Legal ยืนยัน. ตัดสินใจเชิงสถาปัตยกรรมใน [ADR 0017](../adr/0017-shared-attachment-owner-registry.md).

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| Owner | `(ownerType, ownerId)` โดย `ownerType` ต้องอยู่ใน registry ในโค้ด (รอบแรก: `installation-job`); ไม่ลงทะเบียน → `422 ATTACHMENT_OWNER_TYPE_INVALID` |
| ลำดับตรวจ | owner type → permission ของ owner (403) → ค้น owner ใน Organization/Branch ของผู้เรียก (ไม่พบ/ข้าม org/ข้ามสาขา = **404** เหมือนกัน) → validation (422) → สถานะ owner (409 `ATTACHMENT_OWNER_LOCKED`) |
| ไฟล์ | ต้องผ่าน upload session ของ Files module โดย `parentType = ownerType`, `parentId = ownerId`; verified, org เดียวกัน, อัปโหลดสำหรับ owner นี้; ผิดเงื่อนไขทั้งหมด → `422 ATTACHMENT_FILE_NOT_READY` (ไม่บอกว่าไฟล์มีอยู่หรือไม่) |
| Purpose | `general`, `evidence`, `handover`, `defect` (ลายเซ็นใช้ได้เฉพาะ `handover`) |
| ความซ้ำ | ไฟล์เดียวกัน + owner เดียวกัน + purpose เดียวกัน (ที่ยัง active) → `409 ATTACHMENT_DUPLICATE`; unique index บางส่วน (`removed_at_utc IS NULL`) ป้องกัน race |
| จำนวน | ≤ 20 ไฟล์ต่อคำขอ, ≤ 50 active ต่อ owner (`409 ATTACHMENT_LIMIT_EXCEEDED`) |
| Atomic | การแนบเป็น transaction เดียวต่อคำขอ: ผิดข้อเดียว = ไม่มี link ใดถูกสร้าง (ไฟล์ที่อัปโหลดแล้วยังอยู่ใน Files module แต่ไม่ถูกผูก) |
| Unlink | soft remove (`removed_at_utc/by`), ไฟล์จริงไม่ถูกลบ; unlink ซ้ำ → 404 |
| Idempotency | `POST` ต้องมี `Idempotency-Key` (replay คืนรายการปัจจุบัน, payload ต่าง → `409 IDEMPOTENCY_KEY_REUSED`); `DELETE` ไม่ต้องมี |
| ลายเซ็น | `signerName` 2–200, `signerRole` ≤100, `consentAccepted=true` และ `consentTextVersion` ตรงเวอร์ชันปัจจุบันของ purpose (`handover` → `handover-2026-10-v1`); ภาพต้องเป็นไฟล์ `image/png` ที่ผ่านการตรวจแล้วและมี `content_sha256`; `signedAtUtc` เป็นเวลาเซิร์ฟเวอร์; `contentHash` = SHA-256 ของ bytes ที่เซิร์ฟเวอร์คำนวณ; ภาพหนึ่งไฟล์เป็นลายเซ็นได้ครั้งเดียว |
| Audit | `attachment.linked`, `attachment.unlinked`, `signature.captured` บันทึก owner type/purpose/จำนวน/id ไฟล์/hash/เวอร์ชันยินยอม **ไม่บันทึกชื่อและตำแหน่งผู้ลงนาม** |
| PDPA | ชื่อ/ตำแหน่งผู้ลงนามเก็บใน `signature_captures` เท่านั้น |

## Permission mapping (ไม่มี permission key ใหม่)

| Owner type | อ่าน (list/serve) | จัดการไฟล์แนบ (attach/unlink/upload) | เซ็น (capture signature / upload ภาพลายเซ็น) | สถานะ owner ที่แก้ไฟล์แนบได้ | สถานะที่เซ็นได้ |
| --- | --- | --- | --- | --- | --- |
| `installation-job` | `installations.read` | `installations.operate` | `installations.handover` | `planned`, `in_progress`, `ready_for_handover` | `ready_for_handover` |

## Endpoints (ต้อง `Authorization` + `X-Membership-Id`)

| Action | Method/Path | Permission |
| --- | --- | --- |
| รายการไฟล์แนบ | `GET /api/v1/attachment-owners/{ownerType}/{ownerId}/attachments` → `{items}` | read |
| แนบไฟล์ | `POST .../attachments` `{purpose, fileIds[1..20]}` → `201 {items}` | manage |
| ถอดไฟล์แนบ | `DELETE .../attachments/{linkId}` → `204` | manage |
| รายการลายเซ็น | `GET .../signatures` → `{items}` | read |
| บันทึกลายเซ็น | `POST .../signatures` `{purpose, signerName, signerRole?, imageFileId, consentAccepted, consentTextVersion}` → `201` | sign |

Response เป็น structured projection: `AttachmentLinkResponse { id, ownerType, ownerId, fileId, purpose, filename, mediaType, fileSizeBytes, servingUrl, createdBy { id, displayName }, createdAtUtc }`; `SignatureCaptureResponse { id, ownerType, ownerId, purpose, signerName, signerRole, signedAtUtc, imageFileId, servingUrl, consentTextVersion, contentHash, capturedBy { id, displayName } }`.

## Errors

`403 PERMISSION_DENIED`; `404 RESOURCE_NOT_FOUND`; `409`: `ATTACHMENT_DUPLICATE`, `ATTACHMENT_LIMIT_EXCEEDED`, `ATTACHMENT_OWNER_LOCKED`, `IDEMPOTENCY_KEY_REUSED`; `422`: `ATTACHMENT_OWNER_TYPE_INVALID`, `ATTACHMENT_PURPOSE_INVALID`, `ATTACHMENT_FIELD_INVALID`, `ATTACHMENT_FILE_NOT_READY`, `ATTACHMENT_FILE_SCOPE_MISMATCH`, `SIGNATURE_SUBMISSION_INVALID`, `SIGNATURE_CONSENT_REQUIRED`, `SIGNATURE_IMAGE_INVALID`.

## Data (migration `AddSharedAttachmentsAndSignatures`, schema `files`)

`attachment_links` (org, owner_type, owner_id, file_id, purpose, created_by/at, removed_by/at; FK `(file_id, organization_id)` → `uploaded_files (id, organization_id)` เพื่อบังคับ org เดียวกันระดับ DB; unique บางส่วน `(organization_id, owner_type, owner_id, file_id, purpose) WHERE removed_at_utc IS NULL`). `signature_captures` (org, owner_type, owner_id, purpose, signer_name, signer_role, signed_at_utc, image_file_id unique, consent_text_version, content_hash, captured_by). `owner_type`/`purpose` มี check เฉพาะรูปแบบ (regex) เพราะรายชื่อจริงอยู่ในโค้ด.

## ขั้นตอนลงทะเบียน owner type ใหม่ (สำหรับ G-09/G-11/G-07/G-18)

1. เพิ่ม constant และใส่ใน `Registered` ของ `AttachmentOwnerTypes` (`backend/src/TanErp.Domain/Attachments/AttachmentValues.cs`).
2. เพิ่ม `AttachmentOwnerDescriptor` ใน `AttachmentOwnerRegistry` (permission อ่าน/จัดการ/เซ็น + สถานะที่แก้ได้/เซ็นได้).
3. เพิ่ม `case` ใน `AttachmentOwnerScopeReader.FindAsync` และใน `SupportedOwnerTypes`.
4. เพิ่มค่าใน `ATTACHMENT_OWNER_TYPES` (`frontend/src/lib/attachments/attachment-owner-types.ts`) แล้วใช้ `<AttachmentList ownerType ownerId />`.
5. ไม่ต้องแก้ controller, handler, store, component หรือ migration. test `AttachmentOwnerScopeReaderParityTests` และ `AttachmentHandlerTests.Registry_*` จะล้มถ้าลืมข้อ 1–3.

## Threat notes

owner type ที่ผู้ใช้ส่งมาไม่เคยถูกใช้เป็นชื่อตาราง/คำสั่ง SQL (ใช้เทียบกับ whitelist เท่านั้น); ไฟล์ที่ผูกแล้วอ่านผ่าน `GET /api/v1/files/{id}/content` ซึ่งตรวจ parent ของ upload session ด้วย permission ของ owner; ไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตามกฎหมาย (ภาพ + hash เท่านั้น); ผู้ที่มีสิทธิ์ `installations.operate` แต่ไม่มี `installations.handover` เซ็นไม่ได้.
```
