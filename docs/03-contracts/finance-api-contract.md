# Finance Integration API Contract (ข้อตกลง API การวางบิล รับชำระ ภาษีไทย และส่งข้อมูลให้ระบบบัญชี)

**สถานะ:** Implemented 2026-10-04 (`CP-15`) และขยายรองรับภาษีไทยครบวงจร 2026-10-30 (`G-15`). กฎด้านล่างเป็นค่าเริ่มต้น `TEST_ONLY` ที่ทีมพัฒนาเลือก **รอ Finance ยืนยัน** ([Open Validation Questions](../01-business/open-validation-questions.md)). **ไม่ได้เชื่อมระบบบัญชีจริง** — ระบบนี้ออกเลขอ้างอิงที่ตรึงแล้ว คำนวณ VAT/WHT/Retention/Credit-Debit Note ส่งข้อมูลผ่าน outbox และกระทบยอด.

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| ใบแจ้งหนี้ (billing reference) | ต่อ Project (ไม่ใช่ cancelled): `deposit \| milestone \| final \| other`, ยอดฐานภาษีก่อน VAT (`amount`) > 0 ทศนิยมไม่เกิน 2 ตำแหน่ง, รายละเอียด ≤200, วันครบกำหนดไม่บังคับ; เลข `BIL-{YYYY}-{SEQ:4}`; รองรับบรรทัดรายการ (`lines[]` optional), การคิดภาษีมูลค่าเพิ่ม (`applyVat` / `vatRate` ดึงจาก `TaxPolicyVersion` ที่ Active หรือ `0.07` `TEST_ONLY`, ปัดเศษ `AwayFromZero` 2 ตำแหน่ง พร้อมเลขใบกำกับภาษี `TIV-{YYYY}-{SEQ:4}`), และการหักเงินประกันผลงาน (`retentionRate` `0..0.30`, `retentionAmount = round(amount * retentionRate, 2)`, สถานะ `none \| held \| released`) |
| วงเงิน | ยอดรวมฐานภาษี (`amount`) ของใบที่ไม่ถูกยกเลิก ≤ `BaselineContractAmount` ของโครงการ (ตรึงจากใบเสนอราคาที่ยอมรับ) → `BILLING_EXCEEDS_CONTRACT`; ล็อกแถวโครงการ (`FOR UPDATE`) จึงยิงพร้อมกันไม่เกินวงเงิน; การปลด Retention เปลี่ยนสถานะเงินประกันจาก `held → released` บนใบเดิมโดยไม่เพิ่มยอด `Billed` เกินสัญญา |
| Immutable reference | ยอด ประเภท โครงการ เลขที่ และเวลาออก ตรึงด้วย `referenceHash` (SHA-256; รักษา hash แบบ `CP-15` เดิมเมื่อ `vatRate == 0 && retentionRate == 0`); แก้ไขโดย **void แล้วออกใหม่** หรือออก **Credit Note / Debit Note** |
| สถานะ | `issued → partially_paid → paid` ตามยอดรับสุทธิเทียบกับ `EffectiveReceivable` (และเป็น `paid` เมื่อไม่มี Retention ค้าง `held`); `voided` (ปลายทาง) |
| การรับชำระ & WHT (50 ทวิ) | วิธี `transfer\|cheque\|cash\|card`, เลขอ้างอิง (สลิป/เช็ค) ≤100 บังคับ **unique ทั้งองค์กรตลอดไป** (`PAYMENT_DUPLICATE_REFERENCE`), วันที่รับไม่ใช่อนาคต, เลข `PAY-{YYYY}-{SEQ:4}` พร้อมเลขใบเสร็จ `RCT-{YYYY}-{SEQ:4}`; รองรับภาษีหัก ณ ที่จ่าย (`whtRate` `0..0.30`, `whtType`, `whtCertificateNumber` เลข 50 ทวิ บังคับเมื่อ `whtRate > 0` → `PAYMENT_WHT_CERTIFICATE_REQUIRED`), คำนวณ `whtAmount = round(amount * whtRate, 2)` และ `netCashReceived = amount - whtAmount` |
| เงินประกันผลงาน (Retention Release) | `POST /api/v1/billings/{id}/release-retention` (`If-Match`): ปลดเงินประกันจาก `held → released` เมื่อโครงการมีใบรับประกัน (`Warranty` จาก `CP-14`) ที่หมดอายุแล้ว (`EndDate < Today`); หากไม่มีใบรับประกันหรือยังไม่ครบกำหนด คืน `409 RETENTION_WARRANTY_NOT_EXPIRED` |
| ใบลดหนี้ / ใบเพิ่มหนี้ (Credit / Debit Note) | `POST /api/v1/billings/{id}/notes` (`Idempotency-Key`): `noteType` (`credit \| debit`), `reason`, `taxBaseDelta > 0`, คำนวณ `vatDelta = round(taxBaseDelta * billing.vatRate, 2)`, `totalDelta = taxBaseDelta + vatDelta`, เลข `CN-{YYYY}-{SEQ:4}` / `DN-{YYYY}-{SEQ:4}`; Credit Note ห้ามเกินฐานภาษีคงเหลือและห้ามทำให้ยอดสุทธิต่ำกว่ายอดที่รับชำระแล้ว (`422 BILLING_NOTE_EXCEEDS_ORIGINAL`); อนุมัติผ่าน `POST /api/v1/billings/{id}/notes/{noteId}/approve` (`If-Match`) บังคับ Maker ≠ Checker (`409 BILLING_NOTE_SELF_APPROVAL`) และตรวจวงเงินอนุมัติ (`G-03c`) |
| ใบกำกับภาษี/ใบเสร็จ PDF | `GET /api/v1/billings/{id}/tax-document/pdf` สร้างไฟล์ PDF ขนาด A4 ภาษาไทย/อังกฤษผ่าน QuestPDF (`G-04`) |
| การกลับรายการ | กลับรายการรับชำระ (เช็คเด้ง ฯลฯ) พร้อมเหตุผล → ยอดค้างกลับมา, สถานะใบแจ้งหนี้ถอยตามยอด; รายการเดิมไม่ถูกลบ (correction history) |
| Idempotency | สร้างใบแจ้งหนี้/บันทึกรับชำระ/ออกใบลดหนี้-เพิ่มหนี้ต้องมี `Idempotency-Key` (replay คืนผลเดิม, payload ต่าง → 409); void/reverse/release-retention/approve-note ใช้ `If-Match` |
| Accounting outbox | ทุกเหตุการณ์ (`billing.issued\|billing.voided\|payment.recorded\|payment.reversed\|billing-note.approved\|supplier-invoice.posted\|supplier-invoice.voided`) เข้าคิวใน **Transaction เดียวกับธุรกรรมธุรกิจ** พร้อม snapshot ตรึง และ `dedupeKey` unique (`kind:id`) กันซ้ำ |
| การส่ง/ลองใหม่ | `POST /finance/outbox/dispatch` ดึงรายการที่ครบกำหนด (`FOR UPDATE SKIP LOCKED` จึงไม่ซ้ำเมื่อรันพร้อมกัน) ส่งผ่าน `IAccountingConnector`; ล้มเหลว → `failed` ลองใหม่ด้วย backoff 2, 4, 8, 16 นาที และ `dead` เมื่อครบ 5 ครั้ง; เก็บ error แบบสั้น (≤500) และกรณี exception เก็บเฉพาะชื่อชนิด (กันข้อมูลลูกค้ารั่ว); `requeue` นำ failed/dead กลับเข้าคิวหลังแก้สาเหตุ |
| Connector | **ยังไม่มีระบบบัญชีจริง** — ค่าเริ่มต้น `UnconfiguredAccountingConnector` ไม่เคยรายงานสำเร็จ (`CONNECTOR_NOT_CONFIGURED`) ข้อมูลจึงค้างในคิวและไม่ถูกอ้างว่าบันทึกบัญชีแล้ว; connector จริงต้อง idempotent ด้วย `dedupeKey` |
| Callback ยืนยัน | `POST /finance/outbox/{id}/confirm {externalRef, externalAmount}` ซ้ำด้วยข้อมูลเดิม → 200 ไม่เปลี่ยน/ไม่ audit ซ้ำ; ข้อมูลต่าง → `OUTBOX_CONFIRMATION_CONFLICT` 409; ยืนยันแล้วถือว่า sent |
| การกระทบยอด | `GET /finance/reconciliation`: ยอดวางบิล/รับชำระใน ERP เทียบกับยอดที่บัญชียืนยัน; รายการผิดปกติ `unsynced` (pending/failed/dead), `awaiting_confirmation` (ส่งแล้วยังไม่ยืนยัน), `amount_mismatch` (ยอดที่บัญชียืนยันต่างจากยอดที่ส่ง), `missing_message` (มีใบแจ้งหนี้/รับชำระที่ไม่มีรายการในคิว) |
| วงเงินเครดิต & อายุหนี้ลูกหนี้ (`G-16`) | `TotalExposure = OutstandingReceivables + UnbilledAcceptedAmount`; รายงานอายุหนี้ลูกหนี้ `GET /api/v1/finance/ar-aging` แบ่งเป็น 5 ช่วง (`Current <= 0`, `1–30`, `31–60`, `61–90`, `> 90` วันเกินกำหนด); เมื่อ `Customer.CreditLimit` มีค่าและภาระเครดิตหลังออกใบแจ้งหนี้เกินเพดาน (`max(CreditLimit, ApprovedOverrideCeiling)`) จะถูกบล็อกด้วย `409 CREDIT_LIMIT_EXCEEDED` จนกว่าจะขออนุมัติขยายเพดานเครดิต (`POST /api/v1/finance/credit-overrides`) และอนุมัติโดยผู้อื่น (`POST /api/v1/finance/credit-overrides/{id}/approve`, Maker ≠ Checker → `409 CREDIT_OVERRIDE_SELF_APPROVAL` + `IApprovalAuthority`) |
| ไม่อยู่ในขอบเขต | e-Tax Invoice XML (ETDA — แยกเป็น G-15b หลังรู้ provider), อัตราแลกเปลี่ยนหลายสกุลเงิน |

## Endpoints

| Action | Method/Path | Permission |
| --- | --- | --- |
| ออกใบแจ้งหนี้ | `POST /api/v1/billings` `{projectId, kind, description, amount, dueDate?, applyVat?, vatRate?, retentionRate?, lines?}` + `Idempotency-Key` | `billings.manage` |
| ยกเลิก | `POST /api/v1/billings/{id}/void {reason}` + `If-Match` | `billings.manage` |
| ปลดเงินประกันผลงาน | `POST /api/v1/billings/{id}/release-retention` + `If-Match` | `billings.manage` |
| ออกใบลดหนี้/เพิ่มหนี้ | `POST /api/v1/billings/{id}/notes {noteType, reason, taxBaseDelta}` + `Idempotency-Key` | `billings.manage` |
| อนุมัติใบลดหนี้/เพิ่มหนี้ | `POST /api/v1/billings/{id}/notes/{noteId}/approve` + `If-Match` | `billings.manage` |
| รับชำระ (พร้อม WHT) | `POST /api/v1/billings/{id}/payments {amount, method, reference, receivedDate, whtRate?, whtType?, whtCertificateNumber?}` + `Idempotency-Key` | `payments.manage` |
| กลับรายการรับชำระ | `POST /api/v1/billings/{id}/payments/{paymentId}/reverse {reason}` + `If-Match` | `payments.manage` |
| ดาวน์โหลด PDF ใบกำกับภาษี/ใบเสร็จ | `GET /api/v1/billings/{id}/tax-document/pdf` | `billings.read` |
| รายงานอายุหนี้ & วงเงินเครดิต | `GET /api/v1/finance/ar-aging?asOfDate=&customerId=` · `GET /api/v1/customers/{customerId}/credit-exposure` | `billings.read` |
| ขอ/อนุมัติขยายเพดานเครดิต | `GET /api/v1/finance/credit-overrides?customerId=&status=` (`billings.read`) · `POST /api/v1/finance/credit-overrides` + `Idempotency-Key` · `POST /api/v1/finance/credit-overrides/{id}/approve` + `If-Match` | `billings.manage` |
| อ่าน | `GET /api/v1/billings[/{id}]?search=&status=&projectId=` · `GET /api/v1/projects/{id}/billing-summary` | `billings.read` |
| คิวส่งบัญชี | `GET /api/v1/finance/outbox?status=&kind=` · `GET /api/v1/finance/reconciliation` | `finance-sync.read` |
| ส่ง/ยืนยัน/ส่งใหม่ | `POST /api/v1/finance/outbox/dispatch?batchSize=` · `/{id}/confirm` · `/{id}/requeue` | `finance-sync.run` |

## Errors

`409`: `BILLING_VERSION_CONFLICT`, `BILLING_INVALID_STATE`, `BILLING_HAS_PAYMENTS`, `BILLING_PROJECT_NOT_BILLABLE`, `RETENTION_WARRANTY_NOT_EXPIRED`, `BILLING_NOTE_SELF_APPROVAL`, `BILLING_NOTE_INVALID_STATE`, `CREDIT_LIMIT_EXCEEDED`, `CREDIT_OVERRIDE_SELF_APPROVAL`, `CREDIT_OVERRIDE_INVALID_STATE`, `PAYMENT_DUPLICATE_REFERENCE`, `PAYMENT_INVALID_STATE`, `OUTBOX_INVALID_STATE`, `OUTBOX_CONFIRMATION_CONFLICT`, `IDEMPOTENCY_KEY_REUSED`. `422`: `BILLING_EXCEEDS_CONTRACT`, `BILLING_REASON_REQUIRED`, `BILLING_FIELD_REQUIRED/INVALID`, `BILLING_NOTE_EXCEEDS_ORIGINAL`, `CREDIT_OVERRIDE_FIELD_INVALID`, `PAYMENT_EXCEEDS_OUTSTANDING`, `PAYMENT_WHT_CERTIFICATE_REQUIRED`, `PAYMENT_FIELD_INVALID`, `OUTBOX_FIELD_INVALID`. `403`: `APPROVAL_AUTHORITY_NOT_CONFIGURED`, `APPROVAL_LIMIT_EXCEEDED`. `404` นอก Organization.

## Data (migration `AddFinance`, schema `finance`)

`billing_documents` (checks amounts/status/void; `paid_amount ≤ amount`, void ⇒ ไม่มียอดรับ), `payments` (unique `(org, reference)`; check reversal), `accounting_outbox` (unique `(org, dedupe_key)`; payload jsonb; checks status/kind). Audit: `billing.issued|voided`, `payment.recorded|reversed`, `finance.outbox.dispatched|confirmed|requeued` (ไม่มีเหตุผล/เลขอ้างอิงลูกค้า).
