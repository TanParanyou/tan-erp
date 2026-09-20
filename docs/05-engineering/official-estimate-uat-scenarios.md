# Official Estimate UAT Scenarios (สถานการณ์ทดสอบกับผู้ใช้)

**สถานะ:** Accepted UAT Baseline — ต้องบันทึกผลและ Business Sign-off ก่อนเริ่ม Application Implementation

เอกสารนี้เป็นคู่มือ Workshop/UAT สำหรับยืนยันว่า Official Estimate รองรับงานจริง ตัวเลขทั้งหมดเป็น `TEST_ONLY` และไม่ใช่ Rate, Margin, Tax หรือวงเงินอนุมัติของบริษัท

## ผู้เข้าร่วมที่แนะนำ

- Estimator: ยืนยัน Field และขั้นตอนจัด BOQ
- Sales/Designer/Surveyor: ยืนยันข้อมูลต้นทางและ Customer-facing Description
- Approver/Manager: ยืนยัน Exception และ Approval Route
- Finance/Accounting: ยืนยันสูตร Precision Rounding Discount และ Tax Policy
- Product Owner: ตัดสิน Scope และลงนามผล

## Test Data

| Data | ค่า `TEST_ONLY` |
| --- | --- |
| Customer | บริษัท ตัวอย่าง ดีไซน์ จำกัด |
| Opportunity | ตกแต่งห้องนอนใหญ่ |
| Branch | Bangkok Demo Branch |
| Estimate | ตู้เสื้อผ้า Built-in 3.0000 m |
| Material | ไม้อัด 6.0000 sheet × 1,250.0000 THB |
| Labor | ช่างไม้ 4.0000 day × 1,800.0000 THB |
| Subcontract | งานพ่นสี 1.0000 job × 4,500.0000 THB |
| Pricing | Demo Margin 30% |
| Tax | Demo Policy Rate 7%; ต้องไม่ Activate Production |

Test Data ต้องอยู่ Organization/Branch สำหรับ UAT เท่านั้นและลบหรือ Archive ตาม Data Retention หลังจบ Workshop

## วิธีบันทึกผล

แต่ละ Scenario บันทึก:

| Field | ค่า |
| --- | --- |
| Result | Pass / Fail / Accepted with Change |
| Actual Behavior | สิ่งที่ผู้ใช้เห็นและระบบทำ |
| Evidence | Screenshot, Trace ID, Audit Event หรือ Export |
| Business Decision | ยอมรับ/ขอเปลี่ยน พร้อมเหตุผล |
| Sign-off | ชื่อผู้รับผิดชอบ บทบาท วันที่ |

## UAT-EST-001 — Create Direct Draft

**Role:** Estimator

**Preconditions:** Customer, Opportunity, Branch และ Membership อยู่ Scope เดียวกัน

1. สร้าง Official Estimate โดยไม่เลือก Quick Estimate
2. เลือก Customer, Opportunity, Branch และ Owner
3. บันทึก Draft

**Expected:** ได้ Estimate Number + Revision 1 สถานะ Draft; Organization มาจาก Membership; Audit ระบุผู้สร้าง; Quick Estimate ไม่เป็น Field บังคับ

## UAT-EST-002 — Complete BOQ and Calculate

**Role:** Estimator

**Preconditions:** Published Calculation/Tax Policy และ Test Data พร้อม

1. เพิ่ม Section “งาน Built-in ห้องนอนใหญ่”
2. เพิ่ม Work Item และ Material/Labor/Subcontract Cost
3. เลือก Margin Method แล้ว Calculate

**Expected:** Server คำนวณ Direct Cost, Overhead, Selling, Discount, Tax, Grand Total, Margin/Markup; Snapshot มี Policy/Cost Version; UI แสดง `ready` หรือ Reason ที่ตรง Policy

## UAT-EST-003 — Missing Field and Invalid Unit

**Role:** Estimator

1. ลบ Unit ของ Work Item แล้ว Calculate
2. เลือก Unit ที่ไม่เข้ากับ Cost Source แล้ว Calculate

**Expected:** ไม่คำนวณหรือ Submit; คืน `ESTIMATE_FIELD_REQUIRED`/`ESTIMATE_UNIT_INVALID`; UI Focus Field ที่ผิดและ Draft ไม่หาย

## UAT-EST-004 — Calculation Becomes Outdated

**Role:** Estimator

1. Calculate Draft สำเร็จ
2. เปลี่ยน Quantity หรือ Unit Cost
3. พยายาม Submit โดยไม่ Calculate ใหม่

**Expected:** Banner แสดง Outdated; Submit คืน `ESTIMATE_CALCULATION_OUTDATED`; Snapshot เดิมไม่ถูกแก้; Calculate ใหม่สร้าง Calculation Version ถัดไป

## UAT-EST-005 — Provisional Cost

**Role:** Estimator

1. เพิ่ม Cost ที่ไม่มี Cost Source
2. ลอง Submit โดยไม่มีเหตุผล
3. เพิ่ม Provisional Reason แล้ว Calculate/Submit ใหม่

**Expected:** ขั้น 2 ถูก Block ด้วย `ESTIMATE_PROVISIONAL_COST_REASON_REQUIRED`; ขั้น 3 คืน `requiresAttention`, มี `PROVISIONAL_COST` Trigger และ Route มี Specialist/Checker ตาม Policy

## UAT-EST-006 — Strongest Approval Route

**Role:** Estimator/Manager

**Preconditions:** ใช้ `TEST_ONLY-TH-EST-V1`

1. จัด Estimate ให้เข้า Amount Trigger และ Low Margin Trigger พร้อมกัน
2. Submit

**Expected:** Route เลือกระดับที่เข้มที่สุด, ไม่สร้าง Step ซ้ำ, Freeze Threshold/Policy Snapshot และแสดงเหตุผลครบทุก Trigger

## UAT-EST-007 — Maker–Checker

**Role:** Estimator ผู้สร้าง Estimate

1. Submit Estimate ของตนเอง
2. ใช้ User เดิมพยายาม Approve

**Expected:** ปฏิเสธด้วย `MAKER_CHECKER_VIOLATION`; ไม่มี Approval Decision; มี Security Audit; ผู้ตรวจอิสระยังดำเนินการได้

## UAT-EST-008 — Return, Correct and Resubmit

**Role:** Approver แล้ว Estimator

1. Approver Return พร้อม Reason `MISSING_LABOR_COST` และชี้ Work Item
2. Estimator เพิ่ม Labor Cost
3. Calculate และ Submit ใหม่

**Expected:** Revision เป็น Returned แล้วแก้ได้; การแก้ทำ Calculation Outdated; Submit ใหม่ Freeze Route ใหม่; Decision เดิมยังอยู่แบบ Append-only

## UAT-EST-009 — Approved Revision Is Immutable

**Role:** Estimator/Admin

1. Approve Revision ให้ครบ Route
2. ลอง Patch Quantity, Cost และ Derived Total

**Expected:** ทุก Write ถูกปฏิเสธ `ESTIMATE_INVALID_STATE`; Admin ไม่ข้ามกฎ; Snapshot และ Approved Revision Hash ไม่เปลี่ยน

## UAT-EST-010 — Create New Revision

**Role:** Estimator

1. จาก Approved Revision สร้าง Revision ใหม่
2. ระบุ Reason `CUSTOMER_SCOPE_CHANGED`
3. เพิ่ม Work Item ใหม่

**Expected:** ได้ Draft Revision Number ถัดไปที่อ้าง Parent; ต้นฉบับไม่เปลี่ยน; ต้อง Calculate/Approve ใหม่; Revision Diff แสดงรายการที่เพิ่ม

## UAT-EST-011 — Idempotent Quotation

**Role:** Authorized Sales/Quotation Issuer

1. ออก Quotation จาก Approved Revision ด้วย Idempotency Key
2. Retry Payload เดิมด้วย Key เดิม
3. ลองออกจาก Draft Revision

**Expected:** ขั้น 1–2 คืน Quotation เดิมหนึ่งฉบับ; ขั้น 3 คืน `ESTIMATE_INVALID_STATE`; Quotation Link อ้าง Approved Revision/Hash ตรงกัน

## UAT-EST-012 — Organization and Branch Isolation

**Role:** User จาก Branch/Organization อื่น

1. เปิด URL/ID ของ Estimate ที่อยู่นอก Scope
2. ลองอ่าน Patch Calculate และ Approve

**Expected:** คืน 404 โดยไม่เปิดเผยว่ามี Resource; ไม่มีข้อมูล Business/Financial หลุด; Security Audit มี Trace ID ตาม Policy

## UAT-EST-013 — Customer-safe Output

**Role:** Quotation Issuer/Customer Viewer

1. Preview และ Export Quotation ภาษาไทย
2. ตรวจ Payload/Document ที่ลูกค้าได้รับ

**Expected:** มีเฉพาะ Description, Quantity/Unit, Selling Price, Discount, Tax, Total และเงื่อนไขที่อนุญาต; ไม่มี Unit Cost, Total Cost, Margin/Markup, Internal Note, Trigger, Threshold หรือ Approval Detail

## UAT-EST-014 — Historical Reproducibility

**Role:** Auditor

**Preconditions:** มี Approved Revision จาก Policy Version เก่าและ Published Policy Version ใหม่

1. เปิด Calculation/Approval Snapshot ของ Revision เก่า
2. Reproduce ผลจาก Input + Cost + Policy Version เดิม
3. เปรียบเทียบกับ Revision ที่ Submit หลัง Policy ใหม่มีผล

**Expected:** Revision เก่าได้ Total/Route เดิมและไม่ถูก Policy ใหม่แก้ย้อนหลัง; Revision ใหม่ใช้ Policy ใหม่; Audit แสดง Version/Hash ชัดเจน

## UAT-EST-015 — Concurrent Draft Editing

**Role:** Estimator สอง Session

1. Session A และ B เปิด ETag เดียวกัน
2. A บันทึกก่อน
3. B บันทึกด้วย ETag เก่า

**Expected:** B ได้ `ESTIMATE_VERSION_CONFLICT`; ระบบไม่ Last-write-wins และไม่ Merge Financial Result อัตโนมัติ; ผู้ใช้ Reload/Compare ได้โดย Draft A ไม่เสีย

## UAT-EST-016 — Cancel with Reason

**Role:** Estimator/Authorized Canceller

1. Cancel Draft พร้อม Reason
2. Cancel Submitted Estimate ด้วย User ที่ไม่มี Cancel Authority
3. Cancel Submitted Estimate ด้วยผู้มี Authority

**Expected:** Draft Cancel สำเร็จและแก้ต่อไม่ได้; ขั้น 2 ถูกปฏิเสธ; ขั้น 3 Cancel Route/Open Step อย่าง Atomic และเก็บ Reason/Audit

## UAT-EST-017 — Trusted Create Estimate Draft

**Role:** Estimator

1. สร้าง Estimate Draft จาก Opportunity ที่อยู่ใน stage `estimating` และ Site Survey Revision ที่ Ready
2. Server derive `customerId`, `branchId` และ `siteSurveySnapshotHash` โดยตรงจากฐานข้อมูล
3. ส่ง Request โดยไม่ส่ง fields ที่ derive เหล่านั้น

**Expected:** ได้ Estimate Draft ที่ผูกกับ Customer, Branch, และ Survey Snapshot อย่างถูกต้อง หาก Opportunity ไม่อยู่ใน `estimating` หรือ Survey Revision ยังไม่ Ready หรืออยู่นอก Scope จะถูกปฏิเสธ (404/422/409)

## UAT-EST-018 — Idempotent Quotation Issuance

**Role:** Sales / Commercial

1. ออก Quotation จาก Estimate ที่คำนวณแล้ว
2. ส่ง Idempotency Key และ row versions
3. ส่ง Request เดิมซ้ำ (Replay)

**Expected:** ได้รับเลขที่ Quotation จาก Atomic Sequence Generator; Estimate/Revision กลายเป็น `quoted`; Opportunity เปลี่ยนเป็น `proposed`; มี Stage History 1 รายการ และ Audit 2 รายการ; Replay คืนผลเดิมโดยไม่สร้างเลขที่เอกสารหรือ Audit ซ้ำ

## UAT-EST-019 — Quotation Acceptance and Progression to Won

**Role:** Sales Manager

1. รับการตอบรับ Quotation ที่มีสถานะ `issued`
2. ส่ง expected Opportunity row version
3. ส่ง Request ซ้ำ

**Expected:** Quotation เปลี่ยนสถานะเป็น `accepted`; Opportunity เปลี่ยนเป็น `won`; บันทึก Stage History 1 รายการ และ Audit 2 รายการ (ไม่บันทึก decisionNote); Replay ซ้ำคืนผลเดิม

## UAT-EST-020 — Document Sequence Format and Concurrency

**Role:** System Admin

1. เข้าหน้า Document Numbering Settings และแก้ไข Format Pattern ด้วย If-Match header
2. ทดสอบ Pattern ที่ไม่มี `{SEQ}` หรือ Reset Period ไม่ถูกต้อง

**Expected:** Pattern ที่ไม่ถูกต้องถูกปฏิเสธพร้อม Problem Details ที่มี error code ชัดเจน; การอัปเดตที่ส่ง If-Match ถูกต้องจะหมุน ETag/rowVersion ใหม่

## Exit Criteria

- Scenario Critical `001–014` ผ่านหรือมี Business Decision ที่อนุมัติการเปลี่ยน
- Finance ลงนาม Calculation/Tax/Precision/Rounding
- Business Owner ลงนาม Field/Status/Approval/Customer Visibility
- Security Owner ยืนยัน Scope, Maker–Checker และ Customer Data Leakage
- Demo Threshold ทุกค่าได้รับการแทนด้วย Published Policy จริง หรือระบบคง Production Bootstrap
- Requirement/Contract/Flow ที่ได้รับผลกระทบถูกอัปเดตก่อนเปลี่ยนสถานะ Documentation Foundation
