# Finance Integration API Contract (ข้อตกลง API การวางบิล รับชำระ และส่งข้อมูลให้ระบบบัญชี)

**สถานะ:** Implemented 2026-10-04 (CP-15). กฎด้านล่างเป็นค่าเริ่มต้นที่ทีมพัฒนาเลือก (ผู้ใช้มอบหมายให้ตัดสินใจ) **รอ Finance ยืนยัน** (เงินมัดจำ/หัก ณ ที่จ่าย/ภาษี/retention, วงเงินเครดิต, ระบบบัญชีปลายทาง). **ไม่ได้เชื่อมระบบบัญชีจริง** และไม่ใช่ระบบบัญชีเต็มรูปแบบ — ตาม Scope ระบบนี้ออกเลขอ้างอิงที่ตรึงแล้ว ส่งข้อมูลผ่าน outbox และกระทบยอดเท่านั้น.

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| ใบแจ้งหนี้ (billing reference) | ต่อ Project (ไม่ใช่ cancelled): `deposit | milestone | final | other`, ยอดรวม (บาท) > 0 ทศนิยมไม่เกิน 2 ตำแหน่ง, รายละเอียด ≤200, วันครบกำหนดไม่บังคับ; เลข `BIL-{YYYY}-{SEQ:4}`; **ไม่คำนวณภาษี/หัก ณ ที่จ่าย/retention** (ยอดเป็นยอดรวมที่ผู้ออกกำหนด) |
| วงเงิน | ยอดรวมของใบที่ไม่ถูกยกเลิก ≤ `BaselineContractAmount` ของโครงการ (ตรึงจากใบเสนอราคาที่ยอมรับ ไม่คำนวณจาก master ปัจจุบัน) → `BILLING_EXCEEDS_CONTRACT`; ล็อกแถวโครงการ (`FOR UPDATE`) จึงยิงพร้อมกันไม่เกินวงเงิน; **Change Order ที่อนุมัติยังไม่เพิ่มวงเงินวางบิล** |
| Immutable reference | ยอด ประเภท โครงการ เลขที่ และเวลาออก ตรึงด้วย `referenceHash` (SHA-256) และไม่มี endpoint แก้ไข; แก้ไขโดย **void แล้วออกใหม่** (void ได้เฉพาะเมื่อยังไม่มีเงินรับ, ต้องมีเหตุผล) — ยอดที่ void คืนเข้าวงเงิน |
| สถานะ | `issued → partially_paid → paid` ตามยอดรับสุทธิ; `voided` (ปลายทาง) |
| การรับชำระ | วิธี `transfer|cheque|cash|card`, เลขอ้างอิง (สลิป/เช็ค) ≤100 บังคับ **unique ทั้งองค์กรตลอดไป** (`PAYMENT_DUPLICATE_REFERENCE` รวมรายการที่กลับรายการแล้ว), วันที่รับไม่ใช่อนาคต, เลข `PAY-{YYYY}-{SEQ:4}`; **รับบางส่วนได้ แต่เกินยอดค้างไม่ได้** (`PAYMENT_EXCEEDS_OUTSTANDING`, ไม่มีเงินรับเกิน/เครดิตส่วนเกินในรอบนี้); ล็อกแถวใบแจ้งหนี้จึงยิงพร้อมกันไม่รับเกิน |
| การกลับรายการ | กลับรายการรับชำระ (เช็คเด้ง ฯลฯ) พร้อมเหตุผล → ยอดค้างกลับมา, สถานะใบแจ้งหนี้ถอยตามยอด; รายการเดิมไม่ถูกลบ (correction history) |
| Idempotency | สร้างใบแจ้งหนี้/บันทึกรับชำระต้องมี `Idempotency-Key` (replay คืนผลเดิม, payload ต่าง → 409); void/reverse ใช้ `If-Match` |
| Accounting outbox | ทุกเหตุการณ์ (`billing.issued|billing.voided|payment.recorded|payment.reversed`) เข้าคิวใน **Transaction เดียวกับธุรกรรมธุรกิจ** พร้อม snapshot ตรึง (ไม่คำนวณใหม่จากข้อมูลปัจจุบัน) และ `dedupeKey` unique (`kind:id`) กันซ้ำ |
| การส่ง/ลองใหม่ | `POST /finance/outbox/dispatch` ดึงรายการที่ครบกำหนด (`FOR UPDATE SKIP LOCKED` จึงไม่ซ้ำเมื่อรันพร้อมกัน) ส่งผ่าน `IAccountingConnector`; ล้มเหลว → `failed` ลองใหม่ด้วย backoff 2, 4, 8, 16 นาที และ `dead` เมื่อครบ 5 ครั้ง; เก็บ error แบบสั้น (≤500) และกรณี exception เก็บเฉพาะชื่อชนิด (กันข้อมูลลูกค้ารั่ว); `requeue` นำ failed/dead กลับเข้าคิวหลังแก้สาเหตุ |
| Connector | **ยังไม่มีระบบบัญชีจริง** — ค่าเริ่มต้น `UnconfiguredAccountingConnector` ไม่เคยรายงานสำเร็จ (`CONNECTOR_NOT_CONFIGURED`) ข้อมูลจึงค้างในคิวและไม่ถูกอ้างว่าบันทึกบัญชีแล้ว; connector จริงต้อง idempotent ด้วย `dedupeKey` |
| Callback ยืนยัน | `POST /finance/outbox/{id}/confirm {externalRef, externalAmount}` ซ้ำด้วยข้อมูลเดิม → 200 ไม่เปลี่ยน/ไม่ audit ซ้ำ; ข้อมูลต่าง → `OUTBOX_CONFIRMATION_CONFLICT` 409; ยืนยันแล้วถือว่า sent |
| การกระทบยอด | `GET /finance/reconciliation`: ยอดวางบิล/รับชำระใน ERP เทียบกับยอดที่บัญชียืนยัน; รายการผิดปกติ `unsynced` (pending/failed/dead), `awaiting_confirmation` (ส่งแล้วยังไม่ยืนยัน), `amount_mismatch` (ยอดที่บัญชียืนยันต่างจากยอดที่ส่ง), `missing_message` (มีใบแจ้งหนี้/รับชำระที่ไม่มีรายการในคิว) |
| ไม่อยู่ในขอบเขต | วงเงินเครดิตลูกค้า (credit exposure), การตัดชำระหลายใบ/จัดสรรยอด, ภาษีมูลค่าเพิ่ม/ใบกำกับภาษี, หัก ณ ที่จ่าย, retention, ใบลดหนี้/ใบเพิ่มหนี้, ตั้งต้นลูกหนี้, อัตราแลกเปลี่ยน, การรับชำระซื้อ-ขายฝั่ง Supplier |

## Endpoints

| Action | Method/Path | Permission |
| --- | --- | --- |
| ออกใบแจ้งหนี้ | `POST /api/v1/billings` `{projectId, kind, description, amount, dueDate?}` | `billings.manage` |
| ยกเลิก | `POST /api/v1/billings/{id}/void {reason}` + `If-Match` | `billings.manage` |
| รับชำระ | `POST /api/v1/billings/{id}/payments {amount, method, reference, receivedDate}` + `Idempotency-Key` | `payments.manage` |
| กลับรายการรับชำระ | `POST /api/v1/billings/{id}/payments/{paymentId}/reverse {reason}` + `If-Match` | `payments.manage` |
| อ่าน | `GET /api/v1/billings[/{id}]?search=&status=&projectId=` · `GET /api/v1/projects/{id}/billing-summary` | `billings.read` |
| คิวส่งบัญชี | `GET /api/v1/finance/outbox?status=&kind=` · `GET /api/v1/finance/reconciliation` | `finance-sync.read` |
| ส่ง/ยืนยัน/ส่งใหม่ | `POST /api/v1/finance/outbox/dispatch?batchSize=` · `/{id}/confirm` · `/{id}/requeue` | `finance-sync.run` |

## Errors

`409`: `BILLING_VERSION_CONFLICT`, `BILLING_INVALID_STATE`, `BILLING_HAS_PAYMENTS`, `BILLING_PROJECT_NOT_BILLABLE`, `PAYMENT_DUPLICATE_REFERENCE`, `PAYMENT_INVALID_STATE`, `OUTBOX_INVALID_STATE`, `OUTBOX_CONFIRMATION_CONFLICT`, `IDEMPOTENCY_KEY_REUSED`. `422`: `BILLING_EXCEEDS_CONTRACT`, `BILLING_REASON_REQUIRED`, `BILLING_FIELD_REQUIRED/INVALID`, `PAYMENT_EXCEEDS_OUTSTANDING`, `PAYMENT_FIELD_INVALID`, `OUTBOX_FIELD_INVALID`. `404` นอก Organization.

## Data (migration `AddFinance`, schema `finance`)

`billing_documents` (checks amounts/status/void; `paid_amount ≤ amount`, void ⇒ ไม่มียอดรับ), `payments` (unique `(org, reference)`; check reversal), `accounting_outbox` (unique `(org, dedupe_key)`; payload jsonb; checks status/kind). Audit: `billing.issued|voided`, `payment.recorded|reversed`, `finance.outbox.dispatched|confirmed|requeued` (ไม่มีเหตุผล/เลขอ้างอิงลูกค้า).
