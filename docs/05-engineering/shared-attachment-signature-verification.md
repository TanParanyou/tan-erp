# Shared Attachment & Signature Verification (G-01)

## 1. สถานะ

Implemented 2026-10-06 ด้วยผล **focused tests เท่านั้น** — ยังไม่ผ่านการตรวจในเบราว์เซอร์จริง, full suite, Playwright หรือ UAT ของ Role จริง. สัญญา: [Attachment API Contract](../03-contracts/attachment-api-contract.md); การตัดสินใจ: [ADR 0017](../adr/0017-shared-attachment-owner-registry.md); แผน: [แผน G-01](../superpowers/plans/2026-10-05-g01-shared-attachment-signature.md).

## 2. ผลที่รันจริง (2026-10-06)

| คำสั่ง | ผล |
| --- | --- |
| `dotnet build backend/TanErp.slnx` | 0 Warning, 0 Error |
| Unit `FullyQualifiedName~Attachment\|FileGatePermissions\|SignatureEvidenceRules` | 69 passed / 0 failed |
| Integration `FullyQualifiedName~Attachment` (`-m:1`) | 18 passed / 0 failed |
| Regression Integration `FileUploadSessionTests\|ServiceEndpointsTests\|ItemImageEndpointsTests` (`-m:1`) | 23 passed / 0 failed |
| CP-07 Integration `ExternalAcceptance` (`-m:1`) | 4 passed / 0 failed |
| `npm run check:api` | สำเร็จ (generate จาก OpenAPI snapshot; ไม่มีไฟล์เปลี่ยน) |
| `npm run lint` / `npm run typecheck` | ผ่านไม่มี error |
| `npm run build` | สำเร็จ |
| `npx vitest run src/components/forms src/lib/attachments src/lib/media src/hooks src/features/service` | 44 files, 181 tests passed |

สิ่งที่ชุดเหล่านี้พิสูจน์: invariant ของ domain (owner type ต้องลงทะเบียน, ไฟล์ต่าง org ถูกปฏิเสธ, ซ้ำ/เกินจำนวนถูกปฏิเสธ); กฎร่วมลายเซ็นที่ lift จาก CP-07 โดย behavior เดิมไม่เปลี่ยน (regression `ExternalAcceptance` ผ่านหลัง lift); ลำดับตรวจของ handler (owner type → permission → scope → validation → state); Integration: cross-org = 404, owner type ไม่ลงทะเบียน = 422, ไม่มีสิทธิ์ = 403, batch ล้มเหลวไม่ผูกไฟล์ใด, ไฟล์ของ parent อื่น = 422, idempotency replay/คีย์ซ้ำ payload ต่าง, ลายเซ็นเก็บ hash ของ bytes, ล็อกตามสถานะ owner, ขีดจำกัดต่อ owner ภายใต้ attach พร้อมกัน (49 link + 2 คำขอขนาน → 201 หนึ่ง, 409 `ATTACHMENT_LIMIT_EXCEEDED` หนึ่ง); Frontend: อัปโหลดตอนกดบันทึก, ยืนยันก่อนลบ, i18n th/en.

**Mutation checks:** 4 จาก 5 ถูก test จับได้ (killed). การ mutate ตัวกรอง Organization ใน scope reader **รอด** เพราะการตรวจ branch-scope คืน 404 แยกกันอยู่แล้ว (defense in depth) — ไม่ใช่ช่องว่างของ test แต่ตัวกรอง org ไม่ได้ถูกพิสูจน์แยกเดี่ยว.

### สิ่งที่ไม่ได้รัน

Full backend suite, full vitest suite, Playwright journey, UAT ด้วย Role จริง.

## 3. การตรวจในเบราว์เซอร์

**ยังไม่ได้ตรวจ** `SignaturePad` ในเบราว์เซอร์จริง. แก้ไขแล้วใน `6c63994` / `c33b04a` (อ่านค่าผ่าน ref, reset canvas เฉพาะ mount/resize, ล้างเมื่อ value ว่าง) ต้องยืนยันด้วยมือ: (1) เส้นที่วาดคงอยู่ไม่กะพริบ, (2) การ reset จาก parent ล้าง pad, (3) อัปโหลด PNG แล้วได้ 201.

## 4. เกณฑ์จบ "อย่างน้อย 2 โมดูลใช้ component เดียวกัน"

พิสูจน์เฉพาะส่วนที่ทำได้: owner แรก `installation-job` ใช้ `AttachmentList`, `SignatureCapturePanel`, handler, store และ controller กลางโดยไม่มีโค้ดเฉพาะ installation ในของกลาง ยกเว้นจุดลงทะเบียนสามจุด (whitelist, registry descriptor, scope reader; ป้องกันด้วย parity test). ผู้ใช้รายที่สอง (G-09 Quick Estimate / G-18) ต้องยืนยันตอนลงทะเบียน owner ตาม checklist ในสัญญา.

## 5. ข้อจำกัดและการตัดสินใจ

- ไฟล์เป็นรูปภาพเท่านั้น (JPEG/PNG/WebP ≤ 10 MB); PDF ของ G-11 ต้องขยาย Files module.
- ลายเซ็น = ภาพ + SHA-256 ของ bytes ไม่ใช่ e-Signature ตามกฎหมาย (Validation Question ค้างที่ Legal; ADR 0017).
- ส่งมอบยังไม่บังคับลายเซ็น (รอ Business ยืนยัน).
- Replay ของ attach คืนรายการ link active ปัจจุบัน; pre-check ของ handler อาจคืน `ATTACHMENT_LIMIT_EXCEEDED` เมื่อ replay หลังมีการ unlink ภายหลัง.
- สถานะ owner ไม่ถูก lock ระหว่างตรวจกับบันทึก (TEST_ONLY).
- ไม่มี DB foreign key ไป owner (polymorphic); owner type ใหม่ต้องแก้ 3 จุดพร้อมกัน โดย parity test คุม.
- โมดูลเดิม (Work Images/Survey Evidence/Item Images) ยังใช้ตารางของตัวเอง.
- ไม่มี permission key ใหม่ จึงไม่ต้อง seed Role เพิ่ม.
