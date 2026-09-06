# Item Master Governance (การกำกับดูแลสินค้า หน่วย และต้นทุน)

**สถานะ:** Accepted Direction — Production Bootstrap บังคับ Maker–Checker สำหรับ Cost Record ทุกฉบับ

เอกสารนี้เป็นแหล่งอ้างอิงหลักของ Lifecycle, Ownership, Cost Resolution และ Import Control รายการ Field อยู่ที่ [Item Master Field Catalog](item-master-field-catalog.md)

## Ownership

| Concern | Owner ตัวอย่าง | Checker ตัวอย่าง |
| --- | --- | --- |
| Item/Category | Item Master Owner | Data Steward |
| Unit/Conversion | Data Steward | Estimation/Operations Specialist |
| Cost Source/Record | Cost Owner | Cost Approver/Finance |
| Import Batch | Item Master Owner | ผู้มี `item-imports.commit` |

ชื่อ Role เปลี่ยนได้ ระบบตรวจ Permission + Scope + Authority ไม่ Hard-code ชื่อตำแหน่ง

## Item Lifecycle Rules

- Draft เก็บไม่ครบได้และใช้ในธุรกรรมใหม่ไม่ได้
- Activate ต้องมี Code, Type, Category, Name TH, Base Unit และ Capability ที่สอดคล้อง
- Code immutable หลัง Active ครั้งแรกเพราะเป็น Business Key ที่คนใช้สื่อสาร
- Active เปลี่ยนชื่อ/คำอธิบาย/Category/Capability ได้ด้วย ETag และ Audit
- การถอน Capability ที่มีธุรกรรมค้างต้อง Block หรือกำหนด Effective Change ตาม Module Owner
- Deactivate บังคับเหตุผล; Snapshot เดิมไม่เปลี่ยนและ Item กลับ Active ได้เมื่อผ่าน Validation
- Item ที่เคยถูกอ้างห้าม Hard Delete

## Cost Record Lifecycle Rules

- Draft/Returned แก้ได้ด้วย ETag
- Submitted อ่านอย่างเดียวสำหรับ Maker
- Approver ต้องไม่ใช่ Maker หรือ Last Financial Editor
- Approved ยังใช้ Resolve ไม่ได้จน Publish
- Publish ตรวจ Effective Period Overlap และ Cost Policy อีกครั้งแบบ Atomic
- Published/Superseded/Disabled immutable
- Record ใหม่ที่เริ่มมีผลต้อง Supersede Record เก่าตามช่วงเวลาโดยไม่สร้าง Gap/Overlap ที่ Policy ห้าม
- Disable บังคับ Reason และไม่ลบ Historical Use

## Production Bootstrap

จนกว่าจะมี Cost Authority จริง:

- Cost Record ทุกฉบับต้องผ่าน Independent Checker หนึ่งคน
- Manual/Zero Cost ต้องมีเหตุผลและ Evidence ตาม Policy
- Branch Override ต้องผ่าน Checker ที่มี Branch Scope และ Cost Authority
- Import Commit ต้องแยกจากผู้ Upload เมื่อ Batch มี Cost Update
- ไม่มี Auto-publish
- Resolve Policy/Checker ไม่ได้ให้ Fail-closed

## Cost Source Evidence

| Source Type | Evidence Baseline | Risk |
| --- | --- | --- |
| Supplier Quote | Supplier, Reference Number, Date/File | Expiry/quantity condition |
| Price List | Publisher/File/Effective Date | Version/staleness |
| Contract | Contract Reference/Period | Scope/currency |
| Historical | Transaction Reference/Date | อาจไม่สะท้อนราคาปัจจุบัน |
| Manual | Reason, Owner, Captured Date | บังคับ Checker/อายุสั้นตาม Policy |

Cost Source ไม่มีสิทธิ์หรือราคาด้วยตัวเอง เป็นหลักฐานที่ Cost Record อ้าง

## Deterministic Cost Resolution

Input ต้องมี Organization, Branch, Item, Unit, Currency, Quantity, Effective At และ Cost Policy Version

1. Filter เฉพาะ Published และ Effective Record
2. Filter Item/Unit/Currency/Quantity Range
3. เลือก Branch Scope ก่อน Organization Default
4. เลือก Source Priority จาก Policy
5. เลือก Effective From ล่าสุด
6. หากเสมอกันให้คืน `ITEM_COST_AMBIGUOUS`

ผลต้องคืน Cost Record ID/Version, Source, Original Unit/Amount, Conversion Snapshot, Resolved Amount, Currency, Effective Period และ Staleness/Exception Reason

## Stale and Provisional Cost

- Staleness คำนวณจาก Source Type/Category Policy ไม่ใช้เลขวันเดียวทั้งระบบ
- Stale Cost อาจใช้ได้พร้อม Approval Trigger หรือ Block ตาม Policy
- ไม่พบ Cost สามารถใช้ Provisional Cost ใน Estimate เท่านั้น ไม่สร้าง Published Cost Record อัตโนมัติ
- Provisional Cost ต้องมี Reason/Evidence และไม่ย้อนมาเปลี่ยน Item Master โดยไม่มี Workflow

## Unit Conversion Control

- Exact Conversion กลางอนุญาตเฉพาะ Dimension เดียวกัน
- Item-specific Conversion ใช้กับ Packaging/ขนาดเฉพาะและบังคับ Reason
- Conversion Chain ต้องไม่มี Cycle และต้องให้ผล Deterministic
- Phase 1 จำกัดการ Resolve ไม่เกินหนึ่ง Item-specific Step ต่อคำขอ; Chain ที่ซับซ้อนต้องสร้าง Conversion ตรง
- เปลี่ยน Factor ต้องสร้าง Version/Effective Period ใหม่เมื่อเคยถูก Snapshot

## Import Governance

1. Download Template Version ที่ระบบรองรับ
2. Upload File เข้า File Service
3. Parse เป็น Data-only และเก็บ File Hash
4. Preview Result แบบ Create/Update/Skip/Error
5. แก้ทุก Error ก่อน Commit
6. Commit Atomic ด้วย Idempotency Key
7. เก็บ Audit Summary และ Error Report ตาม Retention

ข้อบังคับ:

- ไม่รัน Macro/Formula/External Link
- ค่า Formula Cell ใช้ไม่ได้หากไม่มี Cached Value ที่ผ่าน Validation
- Header/Template Version ผิดต้อง Reject ทั้งไฟล์
- Duplicate Row/Natural Key ต้องชี้ทุกแถวที่ชน
- Update Existing ต้องมีสิทธิ์และ Current Row Version/Import Match Policy
- Cost Update จาก Import ยังต้องผ่าน Cost Lifecycle; Commit ไม่เท่ากับ Publish

## Audit Events

- `item.created`, `updated`, `activated`, `deactivated`
- `unit.created`, `updated`, `deactivated`
- `unit-conversion.created`, `superseded`, `disabled`
- `cost-record.created`, `submitted`, `returned`, `approved`, `published`, `superseded`, `disabled`
- `item-import.uploaded`, `validated`, `committed`, `failed`
- Cost Resolution ที่ใช้กับ Estimate เก็บใน Estimate Snapshot ไม่สร้าง Audit Event ต่อการ Preview ทุกครั้ง

## Governance Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-GOV-ITEM-001` | Maker Approve Cost ตนเอง | 403 Maker–Checker |
| `TC-GOV-ITEM-002` | Publish Approved Cost ที่ Period ซ้อน | Atomic Reject |
| `TC-GOV-ITEM-003` | Branch Override กับ Organization Default | Branch Record ชนะ |
| `TC-GOV-ITEM-004` | Candidate เท่ากันหลังทุก Rule | `ITEM_COST_AMBIGUOUS` |
| `TC-GOV-ITEM-005` | Deactivate Item ที่เคยใช้ | Snapshot เดิมอ่านได้ |
| `TC-GOV-ITEM-006` | Import มี Error หนึ่งแถว | ไม่มีแถวใด Commit |
| `TC-GOV-ITEM-007` | Retry Import Commit Key เดิม | คืน Batch Result เดิม |
| `TC-GOV-ITEM-008` | Publish Cost รุ่นใหม่ | Estimate Snapshot เดิมไม่เปลี่ยน |
| `TC-GOV-ITEM-009` | Conversion Chain เป็น Cycle | Reject |
| `TC-GOV-ITEM-010` | ผู้ใช้ข้าม Organization | 404 + Security Audit |

## Production Sign-off

Business Owner/Data Steward ต้องยืนยัน Item Type/Category/Capability, Unit/Conversion และ Import Mode ส่วน Finance/Cost Owner ต้องยืนยัน Source Priority, Staleness, Zero/Manual Cost, Branch Override และ Cost Authority ก่อนเปลี่ยนจาก Production Bootstrap
