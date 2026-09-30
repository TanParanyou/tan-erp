# Official Estimate UAT Scenarios (สถานการณ์ทดสอบกับผู้ใช้)

**สถานะ:** Accepted UAT Baseline — ยังไม่ดำเนิน UAT กับบทบาทธุรกิจที่ได้รับอนุญาต; ต้องบันทึกผลและ Business Sign-off ก่อนเปิดใช้จริง

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

### Seeded Estimate Workspace (TEST_ONLY)

เปิด fixture นี้เฉพาะฐานข้อมูลทดสอบ โดยกำหนด `ASPNETCORE_ENVIRONMENT=Test`, `SeedTestData=true` และ `SeedEstimateDemoData=true` ก่อนเริ่ม API. หากต้องการทดสอบ Item Catalog ให้เปิด `SeedItemCatalogDemoData=true` เพิ่มเติม. Flag สำหรับข้อมูลตัวอย่างจะทำงานได้ภายใน Test environment และเมื่อเปิด `SeedTestData` เท่านั้น; ห้ามใช้กับฐานข้อมูล Production.

Fixture สร้าง Customer, Site, Opportunity ที่อยู่ในขั้น `estimating`, Survey Revision สถานะ Ready และ Estimate Draft `TEST-ONLY-ESTIMATE-0001` พร้อมสอง Work Items, ต้นทุนวัสดุ/ค่าแรงสมมติ, Margin 30%, Calculation Policy และ Tax Policy 7% ที่ติดป้าย `TEST_ONLY`. ต้นทุนเป็น provisional พร้อมเหตุผลทดสอบ จึงควรคาดว่า readiness ต้องให้ผู้ตรวจรับทราบ ไม่ใช่ราคาจริงหรือราคาที่อนุมัติแล้ว. Seed ทำซ้ำได้โดยไม่สร้าง Estimate หรือประวัติ stage ซ้ำ และไม่เขียนทับ fixture ที่ผู้ทดสอบแก้ภายหลัง.

ข้อมูลทดสอบ Item Catalog 75 รายการครอบคลุม item type ทั้งหก สถานะ active/draft/inactive หมวดหมู่ แบรนด์ การค้นหา และ pagination ดูรายละเอียดได้ใน [Item Master verification](item-master-estimate-catalog-verification.md); 27 รายการมีต้นทุน published สำหรับทดสอบการเลือก Cost Source ใน BOQ และทุกแถวยังคงเป็น `TEST_ONLY`.

## วิธีบันทึกผล

แต่ละ Scenario บันทึก:

| Field | ค่า |
| --- | --- |
| Result | Pass / Fail / Accepted with Change |
| Actual Behavior | สิ่งที่ผู้ใช้เห็นและระบบทำ |
| Evidence | Screenshot, Trace ID, Audit Event หรือ Export |
| Business Decision | ยอมรับ/ขอเปลี่ยน พร้อมเหตุผล |
| Sign-off | ชื่อผู้รับผิดชอบ บทบาท วันที่ |

## Execution Register (2026-09-28)

หลักฐานอัตโนมัติด้านล่างยืนยันเฉพาะพฤติกรรมซอฟต์แวร์ตามขอบเขต test ที่ระบุ ไม่ถือเป็นผล UAT จากผู้ใช้ธุรกิจ ผลทดสอบกับผู้ใช้จริงยังเป็น `Not run` ทุกกรณี ช่องสถานะชี้เฉพาะความพร้อมของหลักฐานระบบ

ชื่อ test อ้างอิงจาก [Estimate API integration tests](../../backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs), [Estimate domain tests](../../backend/tests/TanErp.UnitTests/Estimates/EstimateTests.cs), [Document Sequence integration tests](../../backend/tests/TanErp.IntegrationTests/Api/DocumentSequencesEndpointsTests.cs) และ [Playwright journey](../../frontend/e2e/official-estimate.spec.ts).

| Scenario | Automated evidence / readiness | UAT result |
| --- | --- | --- |
| UAT-EST-001 | `CreateEstimate_ReadySurvey_DerivesCustomerBranchAndSnapshot`; two-user Playwright journey | Not run |
| UAT-EST-002 | Estimate domain calculation matrix, PostgreSQL `CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot`, Playwright BOQ/calculation | Not run |
| UAT-EST-003 | `UpdateDraft_WithUnresolvableCatalogCost_ReturnsStructuredConflictWithoutWriting` includes invalid catalog unit and asserts retained Draft; `estimate-workspace-drawer.test.tsx` verifies localized unit error and retained user input; missing Work Item Unit is rejected by request validation. | Focused API case 4/4 and Estimate API class 38/38 passed on disposable PostgreSQL; component test passed; manual UAT not run |
| UAT-EST-004 | `FinancialEdit_MarksCalculationOutdatedAndPreservesPriorSnapshot`; domain invalidation tests | Not run |
| UAT-EST-005 | `EvaluateReadiness_BlocksUnexplainedProvisionalCostAndRequiresAttentionAfterReasonIsProvided`; reviewer queue PostgreSQL journey | Not run |
| UAT-EST-006 | Sequential review lifecycle is implemented. Test environment can opt into `TEST_ONLY-TH-EST-V1`, which combines amount, margin, discount, fixed-price, custom-work-item, and provisional-cost triggers, selects the strongest route, and freezes trigger/threshold snapshots with distinct reviewers. Review Queue projects and displays the frozen triggers. Production stays on the bootstrap checker. | Estimate API class passed 40/40 and Item Catalog Estimate flow passed 1/1 on PostgreSQL, including custom-work-item reason gating, trigger routing, and structured Item Master snapshots. Authorized-user UAT and production policy sign-off remain open |
| UAT-EST-007 | `Approve_RejectsMakerAndAcceptsIndependentChecker`; API approval journey checks maker rejection | Not run |
| UAT-EST-008 | PostgreSQL API approval journey and two-user Playwright Return → edit/recalculate → resubmit | Not run |
| UAT-EST-009 | API approval journey checks writes to Approved revisions are rejected and snapshots stay unchanged | Not run |
| UAT-EST-010 | API New Revision/replay coverage and Playwright New Revision journey | Not run |
| UAT-EST-011 | `CreateEstimate_ReadySurvey_DerivesCustomerBranchAndSnapshot` issues once, then replays the same approved-estimate request with the same idempotency key; verifies same quotation ID/number, one row, unchanged sequence, and one stage-history transition. | Not run; automated replay evidence added |
| UAT-EST-012 | PostgreSQL API matrix verifies cross-organization and other-branch users receive a non-disclosing 404 for Estimate read, draft update, calculate, and approve; denied writes leave revision version, calculation snapshots, approval state, and decisions unchanged. | `EstimateActions_CrossOrganizationAndBranchAccessReturnNotFoundWithoutDisclosure` and Estimate API class passed 40/40 on disposable PostgreSQL; authorized-user UAT remains open |
| UAT-EST-013 | Current API boundary is recorded in [Issue Quotation contract](../03-contracts/official-estimate-api-contract.md#issue-quotation): `QuotationResponse` contains quotation/estimate/opportunity identity, number/status, grand total, issue time, revision/stage, and row versions. `CalculateEstimate_WithDiscount_ProducesAccurateFinancialSnapshot` verifies the serialized property set against the allowlist and retains issue/replay assertions; no Preview/Export/Document endpoint exists in this slice. | API projection boundary verified; customer output and document UAT deferred to CP-04 |
| UAT-EST-014 | `HistoricalCalculationCanBeReproducedFromRevisionInputsAndOriginalPolicies`; PostgreSQL history/reproduction API case | Not run |
| UAT-EST-015 | `UpdateDraft_OutdatedVersion_Returns409Conflict`; revision concurrency token tests | Not run; two-user editing workflow remains |
| UAT-EST-016 | `CancelSubmittedEstimate_RequiresAssignedCheckerAndClosesApprovalRoute`; cancellation replay/concurrency tests | Not run |
| UAT-EST-017 | `CreateEstimate_ReadySurvey_DerivesCustomerBranchAndSnapshot`; forged/unready relationship rejection tests | Not run |
| UAT-EST-018 | PostgreSQL issue quotation journey verifies approved revision and opportunity progression; the same-key replay assertions verify no duplicate quotation, number allocation, or stage transition. | Not run; automated replay evidence added |
| UAT-EST-019 | Two-user Playwright journey verifies customer acceptance and Won progression | Not run |
| UAT-EST-020 | `DocumentSequencesEndpointsTests` covers format, permissions, ETag/version and invalid patterns | Not run |
| UAT-EST-021 | Fixed Price API tests cover missing permission/reason, persisted override reason and reviewer projection | Not run |
| UAT-EST-022 | Calculation replay/history tests plus `HistoricalCalculationCanBeReproducedFromRevisionInputsAndOriginalPolicies` | Not run |

The API/component gates for UAT-EST-003 and the Estimate authorization matrices for UAT-EST-006/012 have automated evidence; authorized-user UAT remains open and Estimate unit conversion snapshots are deferred. UAT-EST-013 retains deferred output scope. Same-key quotation replay now has automated evidence for UAT-EST-011/018, but business UAT remains unrun. See [official estimate plan](../superpowers/plans/2026-09-27-official-estimate-completion.md) for release scope and remaining owners.

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

1. ลองบันทึก Work Item โดยไม่มี Unit
2. เลือก Unit ของ Cost Component ที่ไม่ตรงกับหน่วยฐานของ Catalog Item แล้วบันทึก Draft

**Expected:** ขั้น 1 ถูกปฏิเสธด้วย request validation ก่อนบันทึก. ขั้น 2 API ตอบ `422 ESTIMATE_UNIT_INVALID` และ Draft/Row Version เดิมไม่เปลี่ยน; UI แสดงข้อความหน่วยไม่ถูกต้องและคงค่าที่ผู้ใช้แก้ไว้. Work Item Unit เป็นหน่วยขายของงานและไม่จำเป็นต้องเท่ากับหน่วยของ Cost Component. Estimate ยังไม่รองรับการแปลงหน่วยจาก Item/Shared Unit Conversion; ต้องเลือกหน่วยฐานของ Item จนกว่าจะมี conversion snapshot แบบ immutable.

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

`TEST_ONLY-TH-EST-V1` is enabled only when the host environment is `Test` and `Estimates:ApprovalPolicy` explicitly names the profile. It evaluates combined amount, margin, discount, fixed-price override, custom-work-item, and provisional-cost signals at submission, selects the strongest sequential route, and stores the policy hash, thresholds, trigger values, and reviewer assignments in the immutable route snapshot. The Review Queue exposes the trigger code and actual/threshold values to the assigned reviewer. PostgreSQL integration verifies the four-step strongest route including the custom-work-item trigger and queue projection. Production remains on `SYSTEM_BOOTSTRAP_INDEPENDENT_CHECKER` pending Business/Finance policy approval. Authorized-user UAT remains open; this test-only profile is not production financial policy.

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

**Current implementation boundary:** `POST /api/v1/estimates/{estimateId}/quotation` returns only the typed `QuotationResponse` projection documented in the [API contract](../03-contracts/official-estimate-api-contract.md#issue-quotation). It does not expose line items or provide Preview, Export, PDF, or print output. Automated quotation issuance/replay tests do not certify a customer document.

**Role:** Quotation Issuer/Customer Viewer

**Status:** API projection boundary documented; customer-facing output remains deferred to CP-04 until Business/Finance approve its field allowlist and document rules.

1. Preview และ Export Quotation ภาษาไทย
2. ตรวจ Payload/Document ที่ลูกค้าได้รับ

**Future expected result after CP-04:** มีเฉพาะฟิลด์ที่ Business/Finance อนุมัติ; ไม่มี Unit Cost, Total Cost, Margin/Markup, Internal Note, Trigger, Threshold หรือ Approval Detail. ยังไม่มี allowlist/เอกสารที่อนุมัติให้ใช้ตรวจรับในปัจจุบัน.

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

## UAT-EST-021 — Fixed Price Override Authority

**Role:** Estimator ที่มี/ไม่มี `estimates.override-price`, Independent Reviewer, Auditor

1. ผู้ประเมินที่ไม่มี `estimates.override-price` เลือก Fixed Price และส่ง Draft พร้อม reason code
2. ผู้ประเมินที่มีสิทธิ์แต่เว้น reason code ส่ง Draft
3. ผู้ประเมินที่มี permission ใน Branch ของ Estimate ระบุ reason code แล้ว Save/Calculate/Submit
4. ผู้ตรวจเปิด Review Queue และตรวจ Work Item, Fixed Price, reason code และ readiness trigger
5. ตรวจ Customer-safe Quotation Projection ของ Revision ที่อนุมัติแล้ว

**Expected:** ขั้น 1 คืน 403 `PERMISSION_DENIED` และไม่มีการเปลี่ยน Draft; ขั้น 2 คืน 400 `ESTIMATE_FIXED_PRICE_REASON_REQUIRED`; ขั้น 3 สำเร็จเฉพาะเมื่อ permission scope ตรง Branch, reason ถูกตรึงใน Input/Calculation/Approval Snapshot และ readiness เป็น `requiresAttention`; ขั้น 4 แสดง reason ให้ reviewer; ขั้น 5 ไม่มี `sellingRuleReasonCode`, ต้นทุน, Margin/Markup หรือรายละเอียดอนุมัติในข้อมูลลูกค้า

## UAT-EST-022 — Calculation Replay and Historical Policy

**Role:** Estimator, Finance Reviewer, Auditor

**Preconditions:** มี Calculation Snapshot เดิมภายใต้ cost/policy รุ่น N และเตรียมรุ่น N+1 ที่มี effective date ชัดเจนโดยไม่แก้รุ่น N หรือข้อมูลเดิม; ผู้ทดสอบเก็บ Idempotency-Key เดิมของคำสั่งคำนวณไว้สำหรับ replay

1. คำนวณ Revision ด้วย Idempotency-Key ใหม่ แล้วบันทึก Calculation Version, Input Hash, policy IDs/versions/hashes, cost provenance และยอดรวมจาก Snapshot
2. ส่งคำขอเดิมด้วย Idempotency-Key และ payload เดิม
3. ใช้ Key เดิมแต่เปลี่ยน discount input/reason แล้วส่งซ้ำ
4. เมื่อ N+1 มีผล สร้าง Revision ถัดไปและคำนวณด้วย policy ใหม่; เปิด Calculation History ของ Revision เก่าและ Snapshot ใหม่ควบคู่กัน
5. ให้ Auditor ตรวจว่าประวัติเดิมอ่านได้ครบและเทียบยอด/inputs/policy/cost hashes ของ Revision เก่ากับ Snapshot ที่ตรึงไว้

**Expected:** ขั้น 2 คืนผลเดิมโดยไม่เพิ่ม Calculation Snapshot; ขั้น 3 คืน `ESTIMATE_IDEMPOTENCY_KEY_REUSED`; Revision เก่ายังคง Snapshot, policy/cost version, hashes และ totals เดิม; Revision ใหม่ใช้ N+1 เฉพาะหลัง effective date; Audit/History แสดง calculation version ต่อเนื่องโดยไม่เขียนทับประวัติ

## Exit Criteria

- Scenario Critical `001–014`, `021`, และ `022` ผ่านหรือมี Business Decision ที่อนุมัติการเปลี่ยน
- Finance ลงนาม Calculation/Tax/Precision/Rounding
- Business Owner ลงนาม Field/Status/Approval/Customer Visibility
- Security Owner ยืนยัน Scope, Maker–Checker และ Customer Data Leakage
- Demo Threshold ทุกค่าได้รับการแทนด้วย Published Policy จริง หรือระบบคง Production Bootstrap
- Requirement/Contract/Flow ที่ได้รับผลกระทบถูกอัปเดตก่อนเปลี่ยนสถานะ Documentation Foundation
