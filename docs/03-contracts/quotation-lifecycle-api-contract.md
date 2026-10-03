# Quotation Lifecycle API Contract (ข้อตกลง API แก้ไข/ยกเลิกใบเสนอราคา)

**สถานะ:** Implemented 2026-10-04 (CP-06). กฎด้านล่างเป็นค่าเริ่มต้นที่ทีมพัฒนาเลือก (ผู้ใช้มอบหมายให้ตัดสินใจ) **รอ Sales/Finance ยืนยัน**. Amendment นี้แยกจาก Estimate Revision: Revision แก้ราคา/BOQ ของประมาณการ ส่วน Amendment ออกใบเสนอราคาฉบับใหม่แทนฉบับที่ออกแล้ว.

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| สถานะ | `issued → accepted` (เดิม) และใหม่ `issued → superseded` (ถูกแทนที่) / `issued → voided` (ยกเลิก); `superseded`/`voided`/`accepted` เป็นสถานะปลายทาง |
| แก้/ยกเลิกได้เมื่อ | เฉพาะ `issued`. **`accepted` ถูกล็อก** (`QUOTATION_ACCEPTED_LOCKED`) เพราะ Opportunity Won และ Project อาจอ้างอิงแล้ว — การยกเลิกหลังยอมรับต้องผ่านขั้นตอนอื่นที่ธุรกิจกำหนดในอนาคต |
| Amend (แทนที่) | ออกใบเสนอราคา **ฉบับใหม่ (เลขใหม่)** จากประมาณการฉบับที่อนุมัติ/เสนอราคาล่าสุด + ตรึงข้อมูลลูกค้าปัจจุบันใหม่ (ตรวจความพร้อมเหมือนตอนออก); ฉบับเดิมเป็น `superseded` พร้อมลิงก์ `supersededBy`/`supersedes` และเหตุผล; **snapshot ของฉบับเดิมไม่ถูกแก้** และยังเปิดดู/พิมพ์ย้อนหลังได้. ถ้ามีประมาณการ revision ใหม่ที่อนุมัติแล้ว (เป็นฉบับปัจจุบัน) จะเสนอราคาจาก revision นั้น (แก้ราคา); ถ้ายังมี revision ที่จัดทำไม่เสร็จ → `QUOTATION_AMEND_REVISION_PENDING` |
| Void | ยกเลิกฉบับที่ออกแล้วและยังไม่ถูกยอมรับ ต้องมีเหตุผล (≤500) บันทึกผู้ยกเลิก/เวลา; **ไม่ย้อน Opportunity** (คง `proposed`) และไม่ย้อนประมาณการ — ผู้ใช้จัดการ Opportunity ต่อด้วยขั้นตอนเดิม (เช่น Lost) |
| ฉบับที่ใช้งาน | ต่อหนึ่งประมาณการมีได้ **หนึ่งใบที่ live** (`issued`/`accepted`) — บังคับด้วย partial unique index `ux_quotations_one_live_per_estimate`; ใบเอกสาร (`/quotation/document`) แสดงฉบับล่าสุดตามเวลาออก; การตอบรับ (`/quotation/accept`) เลือกเฉพาะฉบับ live — ถ้ามีแต่ superseded/voided → `QUOTATION_INVALID_STATE` |
| Concurrency | ทุกการเปลี่ยนต้องส่ง `If-Match` = row version ของใบเสนอราคา (`QUOTATION_VERSION_CONFLICT`); Amend ต้องมี `Idempotency-Key` (replay คืนผลเดิม, payload ต่าง → `IDEMPOTENCY_KEY_REUSED`); ยอมรับพร้อมกับ amend/void ป้องกันด้วย row version ของใบเสนอราคาและ unique index |
| Audit | `quotations.voided`, `quotations.amended` บันทึกเลขเอกสารและยอด **ไม่บันทึกข้อความเหตุผล/ข้อมูลลูกค้า** (เหตุผลอยู่บนตัวใบเสนอราคา) |

## Endpoints

| Action | Method/Path | Permission |
| --- | --- | --- |
| ประวัติใบเสนอราคาของประมาณการ | `GET /api/v1/estimates/{id}/quotations` | `quotations.read` |
| แทนที่ | `POST /api/v1/quotations/{id}/amend` `{reason}` + `If-Match` + `Idempotency-Key` → `201` ประวัติ | `quotations.amend` |
| ยกเลิก | `POST /api/v1/quotations/{id}/void` `{reason}` + `If-Match` → `200` ประวัติ | `quotations.void` |

## Errors

`409`: `QUOTATION_INVALID_STATE`, `QUOTATION_ACCEPTED_LOCKED`, `QUOTATION_AMEND_REVISION_PENDING`, `QUOTATION_VERSION_CONFLICT`, `IDEMPOTENCY_KEY_REUSED`. `422`: `QUOTATION_REASON_REQUIRED`, `QUOTATION_FIELD_INVALID`, `ESTIMATE_INVALID_STATE`, `CUSTOMER_QUOTATION_BILLING_NOT_READY`. `404` นอก Organization.

## Data (migration `AddQuotationLifecycle`)

`commercial.quotations` เพิ่ม `supersedes_quotation_id`, `superseded_by_quotation_id`, `amendment_reason`, `voided_at_utc`, `voided_by_user_id`, `void_reason`; check status ขยายเป็น `superseded|voided` และ check ความสอดคล้อง (`status='voided' ⇔ void_reason`, `status='superseded' ⇔ superseded_by`); unique index ฉบับ live ต่อประมาณการ.

## Hand-offs

Project Handover ใช้เฉพาะฉบับ `accepted` (เดิม). CP-07 (External Acceptance) ต้องปฏิเสธเอกสารที่เป็น `superseded`/`voided` ตามกฎนี้.
