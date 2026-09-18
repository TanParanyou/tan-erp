---
status: accepted
---

# Production-Grade Document Numbering Engine (Atomic Upsert, Thai Localization & Two-Tier Allocation)

## บริบทและการตัดสินใจ (Context and Decisions)

ระบบ Project ERP (`tan-erp`) มีเอกสารทางธุรกิจจำนวนมากที่ต้องการเลขที่เอกสารต่อเนื่อง เรียงลำดับตามเวลา และมนุษย์อ่านเข้าใจได้ (Human-readable sequential document numbers) เช่น ใบประเมินราคา (Estimate), บันทึกสำรวจหน้างาน (Site Survey), ใบเสนอราคา (Quotation), ใบสั่งซื้อ (PO) และใบแจ้งหนี้/ใบกำกับภาษี (Invoice/Tax Invoice)

การใช้ตรรกะแบบเดิม เช่น `CountAsync() + 1` ใน `ReadCommitted` transaction ก่อให้เกิดปัญหา Concurrency Race Condition อย่างรุนแรงเมื่อมีคำขอสร้างเอกสารเข้ามาพร้อมกันในเสี้ยววินาทีเดียวกัน อีกทั้งไม่รองรับการเริ่มนับใหม่ตามรอบเวลา (Reset Cycles) และไม่รองรับมาตรฐานปี พ.ศ. ของธุรกิจไทย จึงตัดสินใจกำหนดสถาปัตยกรรมกลางของ **Document Numbering Engine** ดังนี้:

1. **Shared Numbering Service (โมดูลกลาง):**
   - ออกแบบเป็น Platform Service ส่วนกลางในระดับ Application (`IDocumentNumberGenerator`) และ Infrastructure (`ISequenceCounter`) ที่ Aggregate ทุกประเภทเอกสารสามารถเรียกใช้ได้
   - แยกตารางตั้งค่าแม่แบบ `common.document_sequence_definitions` (เก็บ Format Pattern, Reset Cycle, Prefix, Padding)
   - แยกตารางตัวนับลำดับ `common.sequence_counters` สำหรับเพิ่มค่าตัวเลขอัตโนมัติ

2. **Atomic Upsert Concurrency Strategy:**
   - ใช้คำสั่ง SQL เดียว:
     ```sql
     INSERT INTO common.sequence_counters (
         organization_id, document_type, branch_id, period_key, current_val, updated_at_utc
     ) VALUES (
         @organizationId, @documentType, @branchId, @periodKey, 1, clock_timestamp()
     )
     ON CONFLICT (organization_id, document_type, branch_id, period_key)
     DO UPDATE SET
         current_val = common.sequence_counters.current_val + 1,
         updated_at_utc = clock_timestamp()
     RETURNING current_val;
     ```
   - คำสั่งนี้แก้ไขทั้งปัญหา Cold-Start (เมื่อขึ้นเดือน/ปีใหม่หรือมีเอกสารใหม่จะเริ่มที่ 1 ทันที) และรันภายใต้ Row Lock ของ PostgreSQL ร่วมกับกลไก EvalPlanQual (EPQ) ใน `ReadCommitted`
   - ป้องกันการเกิด Serialization Failure (`40001`) และป้องกัน Unique Constraint Violation (`23505`) อย่างเด็ดขาด
   - นำคำสั่งเข้าร่วมใน `IDbContextTransaction` เดียวกับตัวเอกสาร เพื่อให้ค่า Counter ถูก Rollback อัตโนมัติหากเอกสารเกิดข้อผิดพลาด

3. **Late Acquisition Pattern:**
   - ดึงเลข Sequence เป็นขั้นตอนสุดท้ายก่อนเรียก `SaveChangesAsync()` และ `CommitAsync()` เพื่อให้ระยะเวลาถือครอง Row Lock บนตารางตัวนับสั้นที่สุด (ระดับ < 2–5 ms)

4. **Thai Enterprise Token Formatting:**
   - รองรับตัวแปรใน Format Pattern:
     - `{PREFIX}`: รหัสประเภทเอกสาร เช่น `EST`, `SRV`, `QT`
     - `{BRANCH}`: รหัสสาขา เช่น `BKK`, `HQ` (หากไม่มีสาขาหรือเป็นเอกสารรวมจะแทนด้วยค่าว่างหรือ default)
     - `{YYYY}` / `{YY}`: ปี ค.ศ. 4 หลัก / 2 หลัก
     - `{BBBB}` / `{BB}`: ปี พ.ศ. 4 หลัก / 2 หลัก (เช่น 2569 / 69) สำหรับธุรกิจในประเทศไทย
     - `{MM}`: เดือน 2 หลัก (01-12)
     - `{DD}`: วันที่ 2 หลัก (01-31)
     - `{SEQ:X}`: ลำดับตัวเลขพร้อม Padding เช่น `{SEQ:4}` -> `0001`
   - รอบการรีเซ็ต (Reset Periods): `Never`, `Yearly`, `Monthly`, `Daily`

5. **Two-Tier Allocation & Gapless Policy:**
   - ฉบับร่าง (Draft): สามารถใช้รหัสร่างชั่วคราวหรือกำหนดเลขเบื้องต้นได้
   - ฉบับทางการ (Official / Mark Ready / Submitted): ดึงเลขทางการจาก Sequence Counter เพื่อรับประกันว่าเลขเอกสารทางการจะเรียงติดกันเสมอ (Gapless) สอดคล้องตามมาตรฐานบัญชีและสรรพากร
   - ห้าม Hard Delete เอกสารทางการ หากยกเลิกให้ปรับสถานะเป็น `Void` เพื่อรักษา Audit Trail

6. **Security & Manual Override Guard:**
   - ระบบตั้งต้นเป็น Auto-generated 100%
   - การกรอกเลขด้วยตนเอง (Manual Override) อนุญาตเฉพาะผู้ใช้ที่มี Permission พิเศษ `document_numbers.manual_override` (เช่น การ Migrate ข้อมูลย้อนหลัง) และต้องผ่าน Unique Validation เสมอ

## เหตุผลและการปฏิเสธทางเลือกอื่น (Rationale and Alternatives Considered)

1. **การปฏิเสธ Native PostgreSQL Sequence (`CREATE SEQUENCE`):** แม้จะทำงานเร็วมาก แต่การจัดการ Sequence นับพันตัวสำหรับ Multi-Tenant ข้าม Organization, การ Reset รายเดือน/ปีแบบ Dynamic และการ Rollback คืนค่าเมื่อทรานแซกชันล้มเหลว (PostgreSQL sequence ไม่ rollback) จะทำให้เกิดเลขกระโดดและยากต่อการจัดการในระยะยาว
2. **การปฏิเสธ `CountAsync() + 1` (Table Scan):** มีความเสี่ยงร้ายแรงต่อ Race Condition ชนกันของ Primary Key / Unique Index ภายใต้ High Concurrency และทำให้เกิดข้อผิดพลาดทันทีหากมีการลบเอกสารในอดีต
3. **การปฏิเสธ Distributed Lock ผ่าน Redis:** การใช้ Row-Level Lock บน PostgreSQL RDBMS ที่มีอยู่แล้วมีความเสถียร ประหยัด Resource และผูกกับ Transaction ฐานข้อมูลได้อย่างสมบูรณ์ โดยไม่ต้องพึ่งพา Infrastructure เสริมภายนอก

## ผลที่ตามมา (Consequences)

- โครงการ `TanErp.Infrastructure` จะมีตาราง `common.document_sequence_definitions` และ `common.sequence_counters`
- `EstimateStore.cs` และ Store เอกสารอื่น ๆ จะถูก Refactor ให้เรียกใช้ `IDocumentNumberGenerator` แทนการนับ `CountAsync()` เดิม
- มี API และหน้าจอ Admin Settings สำหรับปรับแต่ง Format Pattern และพรีวิวแบบเรียลไทม์
