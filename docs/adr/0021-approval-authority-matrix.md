---
status: accepted
---

# Approval Authority matrix: วงเงินต่อ Role/เอกสาร/สาขา แบบ fail-closed บังคับใน transaction ของการอนุมัติ

Permission บอกว่าเรียก action ใดได้ แต่ไม่จำกัดยอด (`approval-matrix.md`: Permission ≠ Authority). G-03c เพิ่มตาราง `approval_limits` และ `IApprovalAuthority` ที่ Estimate, Purchase Order และ Change Order เรียกตอนอนุมัติ.

**การตัดสินใจ 1 — รูปแบบแถว:** `(role, document_type, branch?, currency, is_unlimited, max_amount?, is_active)` ไม่มีวันที่มีผล (ปิดด้วย `is_active`, ประวัติใน audit). ไม่จำกัดเป็น flag `is_unlimited` ไม่ใช่ `NULL`. `credit_note` สงวนไว้ให้ G-15: DB ยอมรับ แต่ API ปฏิเสธจนกว่า G-15 จะบังคับจริง. THB เท่านั้น.

**การตัดสินใจ 2 — กฎการตัดสิน:** หลาย Role ของ membership เดียว → ใช้เพดานสูงสุด; ต่อ Role แถวเฉพาะสาขาชนะแถวทั้งองค์กร; ไม่มีแถว = `APPROVAL_AUTHORITY_NOT_CONFIGURED`; เกิน = `APPROVAL_LIMIT_EXCEEDED`; เท่ากับเพดานผ่าน.

**การตัดสินใจ 3 — จุดบังคับ:** ใน store ของแต่ละเอกสาร ภายใน transaction เดิม หลังโหลดเอกสาร (ต้องใช้ยอด+สาขา ณ เวลาตัดสิน). Estimate ตรวจที่ step สุดท้ายของ route เมื่อ decision = approved เท่านั้น; PO ตรวจตอน Approve; Change Order ตรวจตอน Approve ด้วย `max(abs(ContractDelta), abs(BudgetDelta))` เป็น THB. Reject/Return ไม่ตรวจ.

**การตัดสินใจ 4 — ไม่มี maker–checker ต่อการแก้วงเงิน:** คุมที่ตัวสิทธิ์ `approval-limits.manage` (เป็น approval permission ตัวที่ 7 → การมอบต้องมี maker–checker) และ audit ก่อน–หลัง. ความเสี่ยงคงเหลือ R1: ผู้ถือสิทธิ์ยกวงเงินของ Role ตนเองได้ทันที.

**การตัดสินใจ 5 — Fail-closed ใน production:** ไม่มี backfill; หลัง deploy การอนุมัติถูกปฏิเสธจนกว่าจะตั้งวงเงินจริง (ต้องผ่าน Business Owner + Finance sign-off ตาม `approval-matrix.md`). Test environment ได้ unlimited จาก `TestOnlyDataSeeder` เท่านั้น.

ข้อเสียที่ยอมรับ: ไม่เก็บหลักฐานวงเงินที่ใช้ลงใน approval snapshot (ไม่แตะ hash เดิม); isolation ของ flow เดิมคงเป็น default จึงการแก้วงเงินพร้อมกันมีผลกับการอนุมัติครั้งถัดไป.
