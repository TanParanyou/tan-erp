# Official Estimate BOQ & Calculation Vertical Slice Implementation Plan (Slice 5A)

> **Status:** Completed and Verified in commit `28aedbf` (remediated under `2026-09-20-official-estimate-commercial-hardening.md`)
> **Goal:** สร้าง Official Estimate Draft จาก Opportunity ที่อยู่ในขั้นตอน Estimating และ Site Survey Revision ที่ Ready พร้อมหน้าต่าง BOQ Workspace Drawer และระบบคำนวณราคาทางการ (Calculation Engine)

---

## 1. Scope & Capabilities

1. **Official Estimate Draft Creation (`POST /api/v1/estimates`)**:
   - เริ่มต้นจาก Opportunity ในสถานะ `estimating`
   - ผูกกับ Customer, Branch, Opportunity, และ Ready Site Survey Revision Snapshot Hash
   - ป้องกันด้วย Idempotency Key

2. **BOQ Workspace Drawer (`PUT /api/v1/estimates/{id}/revisions/{revisionId}/draft`)**:
   - โครงสร้าง BOQ 3 ระดับ: Sections -> Work Items -> Cost Components (Material, Labor, Subcontract, Service, Other)
   - กฎการตั้งราคาขายต่อ Work Item: Margin % หรือ Markup %
   - Financial HUD แสดงผลแบบ Real-time: ยอดรวมต้นทุน, ราคาขายก่อนลด, กำไรขั้นต้น และอัตรากำไร
   - Concurrency Control ด้วย `If-Match` ETag header และ `expectedRevisionVersion`

3. **Calculation Engine (`POST /api/v1/estimates/{id}/revisions/{revisionId}/calculate`)**:
   - การคำนวณฝั่ง Server อย่างแม่นยำ รองรับส่วนลด (Discount Amount) และภาษีมูลค่าเพิ่ม 7% (VAT 7%)
   - สรุปยอด Net Cost, Selling Before Discount, Net Before Tax, Grand Total
   - เพิ่ม `calculationVersion` และจัดเก็บ `CalculationSnapshotJson` ที่ทำซ้ำได้

4. **Design & Architectural Guardrails**:
   - Atelier Architectural Navy Sharp (`0px` border-radius, Solid Navy `#0B3056`, SVG Icons)
   - Strict TypeScript, Zero `any`, Zero `@ts-ignore`
   - i18n ครบทั้งไทยและอังกฤษ (`messages/th.json`, `messages/en.json`)

---

## 2. Boundaries & Deferred Scope

- **Slice 5A Boundary:** สิ้นสุดที่การจัดทำ BOQ และการคำนวณราคาทางการ (Calculation Snapshot)
- **Deferred to Slice 5B:** การออกใบเสนอราคา (Commercial Quotation Issuance), การตอบรับของลูกค้า (Customer Acceptance), การเปลี่ยนขั้นตอน Opportunity สู่ `proposed` และ `won`, ระบบจัดรูปแบบเลขที่เอกสาร (Document Numbering Engine)
- **Deferred to Future Slices:** การแก้ไข/ยกเลิกใบเสนอราคา (Quotation Amendment/Void), การสร้างไฟล์ PDF ใบเสนอราคาพร้อมลายเซ็น, ระบบอนุมัติ Maker–Checker, การสร้าง Project อัตโนมัติหลัง Won
