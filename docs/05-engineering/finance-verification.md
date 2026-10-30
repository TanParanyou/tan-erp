# Finance Integration Verification (CP-15 + G-15)

**สถานะ:** Implemented 2026-10-04 (`CP-15`) และขยายรองรับภาษีไทยครบวงจร 2026-10-30 (`G-15`); หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่ UAT กับ Finance หรือการทดสอบกับระบบบัญชีจริง (ไม่มี connector จริง). กฎ: [Finance API Contract](../03-contracts/finance-api-contract.md) และ [แผน G-15](../superpowers/plans/2026-10-30-g15-thai-tax.md).

## ผลที่รันจริง

- Unit `FinanceDomainTests` (รวมใน `TanErp.UnitTests` 687/687):
  - `CP-15`: reference hash คงที่และจับการเปลี่ยนยอด, ยอดไม่ถูกต้อง, สถานะตามการรับชำระบางส่วน/เต็ม/กลับรายการและห้ามรับเกิน, void ไม่ได้เมื่อมีเงินรับ, ใบที่ void ไม่มียอดค้างและไม่รับชำระ, validation การรับชำระ, outbox backoff 2/4/8/16 นาทีแล้ว dead ครั้งที่ 5 และ requeue, การยืนยันซ้ำ idempotent/ต่างกัน → conflict, ตัด error ยาว.
  - `G-15`: คำนวณ VAT 7% พร้อมปัดเศษ `AwayFromZero` ทศนิยม 2 ตำแหน่งทั้งรายบรรทัดและระดับเอกสาร, รักษา `ReferenceHash` เดิมเมื่อไม่มีภาษี/เงินประกันและเปลี่ยนค่า Hash เมื่อมีภาษี/เงินประกัน, คำนวณภาษีหัก ณ ที่จ่าย (`WhtRate`, `WhtAmount`, `NetCashReceived`) พร้อมบังคับเลข 50 ทวิ (`PAYMENT_WHT_CERTIFICATE_REQUIRED`), หักเงินประกันผลงาน (`RetentionStatus = "held"`) และปลดเงินประกันเมื่อครบกำหนด (`ReleaseRetention`) โดยไม่เพิ่มยอดวางบิลเกินสัญญา, และออกใบลดหนี้/ใบเพิ่มหนี้ (`BillingAdjustmentNote`) พร้อมตรวจเพดานยอดลดหนี้ (`BILLING_NOTE_EXCEEDS_ORIGINAL`) และ Maker ≠ Checker (`BILLING_NOTE_SELF_APPROVAL`).
- Backend Integration `FinanceEndpointsTests` 6/6 (+ `DocumentSequencesEndpointsTests` 14/14, `ApprovalLimitEndpointsTests` 6/6) บน PostgreSQL:
  - `CP-15` (5 tests): วงเงินสัญญา, ยิงพร้อมกัน 4 คำขอไม่เกินสัญญา, รับชำระบางส่วน/เต็ม/กลับรายการ/ยิงพร้อมกันไม่รับเกิน, และคิวส่งบัญชี `accounting_outbox` + กระทบยอด.
  - `G-15` (`ThaiTaxG15_VatWhtRetentionNotesAndPdf_FullLifecycle`): ออกใบแจ้งหนี้พร้อม `ApplyVat: true` (7%, รันเลข `TIV-`), `RetentionRate: 0.10m` (10% `held`) และรายการบรรทัด; ตรวจว่า `release-retention` คืน `409 RETENTION_WARRANTY_NOT_EXPIRED` ก่อนหมดประกัน และปลดสำเร็จเมื่อใบรับประกัน (`Warranty`) หมดอายุแล้วโดยไม่เพิ่มยอด `Billed` เกินสัญญา; บันทึกรับชำระพร้อม WHT 3% (`50BIS-2026-0001`, รันเลข `RCT-`) และตรวจว่าหากไม่ส่งเลข 50 ทวิ คืน `422 PAYMENT_WHT_CERTIFICATE_REQUIRED`; ออกใบลดหนี้ (`CN-`) ตรวจ `422 BILLING_NOTE_EXCEEDS_ORIGINAL` และ `409 BILLING_NOTE_SELF_APPROVAL`, อนุมัติด้วยผู้อนุมัติคนละคนพร้อมส่ง `billing-note.approved` เข้า Outbox; และดาวน์โหลด PDF ใบกำกับภาษี/ใบเสร็จ (`GET /api/v1/billings/{id}/tax-document/pdf` → `200 OK`, `%PDF-`).
- Frontend Verification (`features/finance`):
  - `npx tsc --noEmit` (0 errors), `npm run lint` (0 errors), `npm run build` (89/89 routes).
  - Vitest `src/features/finance/components/finance-components.test.tsx` 14/14 passed (ครอบคลุม `BillingEditor` คำนวณ VAT 7% + Retention %, `BillingDetail` รับชำระพร้อม WHT 3% & เลข 50 ทวิ, ปุ่มปลดเงินประกันผลงาน, การสร้างและอนุมัติ Credit Note แบบ Maker–Checker, และ Outbox kind `billing-note.approved`).

## ข้อจำกัด

ค่าเริ่มต้น `TEST_ONLY` รอ Finance ยืนยัน ([Open Validation Questions หมวด 13](../01-business/open-validation-questions.md)): การปัดเศษภาษีมูลค่าเพิ่มและจังหวะการออกเลขใบกำกับภาษี (`VQ-TAX-01`), อัตราหัก ณ ที่จ่ายและเลข 50 ทวิ (`VQ-TAX-02`), เงื่อนไขการปลดเงินประกันผลงานและการอนุมัติใบลดหนี้ (`VQ-TAX-03`); ยังไม่มี e-Tax Invoice XML (ETDA — แยกเป็น `G-15b` หลังรู้ provider); ยังไม่บังคับวงเงินเครดิตลูกค้า (`G-16`) และยังไม่มีระบบบัญชีจริง (`G-17`).
