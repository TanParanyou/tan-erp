# Approval Matrix (ตารางและนโยบายอนุมัติ)

**สถานะ:** Accepted Direction — ใช้ Production Bootstrap แบบ Fail-closed จนกว่าวงเงินจริงจะผ่าน Business Owner และ Finance Sign-off

เอกสารนี้เป็นแหล่งอ้างอิงหลักของ Approval Policy สำหรับ Official Estimate ส่วน Permission Name อยู่ที่ [Permission Catalog](../03-contracts/permission-catalog.md)

## Permission ไม่เท่ากับ Authority

- Permission ระบุว่า User เรียก Action ประเภทใดได้ เช่น `estimates.approve`
- Scope จำกัด Resource ที่ User เข้าถึงได้ เช่น Organization หรือ Branch
- Approval Authority จำกัดวงเงิน Margin Discount และ Exception ที่ User ตัดสินได้

Backend ต้องตรวจครบทั้งสามส่วน การมี Permission อย่างเดียวไม่อนุญาตให้อนุมัติทุก Estimate

## Policy Lifecycle

```text
Draft → Submitted → Approved → Published → Superseded
   └──── Returned ────┘          └──────→ Disabled
```

Published Policy ต้องมี Version, Effective Period, Organization/Branch Scope และ Maker–Checker แก้ย้อนหลังไม่ได้ Policy ใหม่มีผลเฉพาะการ Submit ใหม่; Approval Route ที่ Freeze แล้วใช้ Snapshot เดิม

## Production Bootstrap

เมื่อยังไม่มี Threshold จริง:

- ทุก Estimate ต้องผ่าน Independent Checker อย่างน้อยหนึ่งคน
- Maker และ Last Financial Editor ห้ามเป็น Final Approver
- ไม่มี Auto-approval
- Manual Override, Provisional Cost และ Custom Work Item ต้องแสดงให้ Checker เห็น
- Resolve ผู้อนุมัติไม่ได้ให้คืน `ESTIMATE_POLICY_UNAVAILABLE` และห้าม Submit
- Administrator ไม่ได้รับ Approval Authority โดยอัตโนมัติ

นี่เป็น Safe Default ที่ใช้ได้ก่อน Workshop โดยไม่สมมติวงเงินจริง

## Trigger Catalog

| Trigger | Input | Baseline Result |
| --- | --- | --- |
| `AMOUNT_AUTHORITY_EXCEEDED` | Grand Total > Authority Limit | เพิ่ม Approver ระดับถัดไป |
| `MARGIN_BELOW_MINIMUM` | Margin Rate ต่ำกว่า Policy | เพิ่ม Financial Approver; อาจ Block ตาม Floor |
| `DISCOUNT_ABOVE_LIMIT` | Discount เกิน Authority | เพิ่ม Commercial Approver |
| `MANUAL_OVERRIDE` | Cost/Price/Tax/Overhead Override | บังคับเหตุผลและ Independent Checker |
| `PROVISIONAL_COST` | Cost Source ยังไม่สมบูรณ์ | เพิ่ม Reviewer ที่มี Cost Authority |
| `STALE_COST` | Cost เกิน Effective/Staleness Policy | Reconfirm หรือ Approve Exception |
| `CUSTOM_WORK_ITEM` | Work Item ไม่มี Item Master | ตรวจ Scope, Unit และ Cost Evidence |
| `MISSING_EVIDENCE` | เอกสาร/Survey ที่ Policy บังคับไม่ครบ | Block Submit |
| `SENSITIVE_PROJECT` | Project Risk Flag | เพิ่ม Approver ตาม Risk Policy |

## Policy Decision

Policy คืนผลหนึ่งค่าเสมอ:

| Decision | ความหมาย | Action |
| --- | --- | --- |
| `blocked` | ข้อมูลหรือ Control ขั้นต่ำไม่ผ่าน | แก้ข้อมูลก่อน Submit |
| `requiresApproval` | คำนวณได้และสร้าง Route ได้ | Freeze Route แล้ว Submit |
| `eligibleForApproval` | ผู้ตรวจปัจจุบันมี Permission/Authority/Scope ครบ | ตัดสิน Approve/Return ได้ |

เมื่อมีหลาย Trigger ระบบรวม Required Step แบบไม่ซ้ำและใช้ Route ที่เข้มที่สุด ห้าม Trigger หนึ่งลดระดับ Trigger อื่น

## Route Resolution

ลำดับ Resolve:

1. โหลด Published Policy ตาม Organization, Branch และ Estimate Date
2. ตรวจ Blocking Condition
3. ประเมิน Amount, Margin, Discount และ Exception จาก Calculation Snapshot ล่าสุด
4. Resolve Candidate ตาม Permission + Scope + Authority
5. ตัด Maker, Last Financial Editor และผู้มี Conflict of Interest ออก
6. รวม Step ตาม Severity/Sequence
7. Freeze Policy/Threshold/Candidate Rule Snapshot ตอน Submit

Assignment อาจเปลี่ยนผู้รับงานภายใน Candidate Rule ได้ แต่ห้ามลด Requirement ของ Route

## Approval Step Rules

- Step เป็น Sequential โดยค่าเริ่มต้น; Parallel Approval เป็น Future Capability
- Reviewer เลือก `approved` หรือ `returned`
- Return บังคับ Reason Code, Note และ Target Field/Work Item เมื่อระบุได้
- Approval Decision เป็น Append-only
- Approved Revision เป็น Immutable
- Financial Input เปลี่ยนหลัง Return ต้อง Calculate และ Resolve Route ใหม่ก่อน Submit
- ผู้อนุมัติที่ไม่มี Authority ในขณะตัดสินต้องถูกปฏิเสธ แม้เคยถูก Assign ไว้

## Demo/UAT Profile

Profile `TEST_ONLY-TH-EST-V1` ใช้สาธิตเท่านั้นและห้าม Publish อัตโนมัติ:

| เงื่อนไข `TEST_ONLY` | Route ตัวอย่าง |
| --- | --- |
| ≤100,000 บาท, Margin ≥30%, ไม่มี Exception | Checker 1 คน |
| >100,000 ถึง 500,000 บาท หรือ Margin 20–29.9999% | Manager 1 คน |
| >500,000 ถึง 1,500,000 บาท หรือ Discount >10% | Manager → Financial Approver |
| >1,500,000 บาท หรือ Margin <20% | Manager → Financial Approver → Director |
| Manual Override/Provisional Cost/Custom Work Item | เพิ่ม Specialist Checker ตาม Trigger |

ตัวเลขทั้งหมดเป็น `TEST_ONLY` ไม่ใช่วงเงินจริงของบริษัท

## Decision Record

ต้องเก็บ Estimate/Revision/Calculation Hash, Approval Policy Version, Route/Step, Trigger/Threshold Snapshot, Requester, Reviewer, Permission/Scope/Authority ที่ใช้, Decision, Reason/Note และ UTC Time

## Test Cases

| ID | Case | Expected |
| --- | --- | --- |
| `TC-APP-001` | ไม่มี Published Policy | Block `ESTIMATE_POLICY_UNAVAILABLE` |
| `TC-APP-002` | Bootstrap Policy | Independent Checker ทุก Estimate |
| `TC-APP-003` | Maker เป็น Candidate คนเดียว | Block; ไม่ Self-approve |
| `TC-APP-004` | เข้า Amount + Low Margin Trigger | Route ที่เข้มที่สุดและไม่ซ้ำ Step |
| `TC-APP-005` | Provisional Cost ไม่มีเหตุผล | Block Submit |
| `TC-APP-006` | Override มีเหตุผล | เพิ่ม Trigger/Checker และ Audit |
| `TC-APP-007` | Policy ใหม่ Published หลัง Submit | Route เดิมใช้ Frozen Snapshot |
| `TC-APP-008` | Return แล้วแก้ Financial Input | Calculation Outdated; Resolve Route ใหม่ |
| `TC-APP-009` | Approver ถูกถอน Authority ก่อนตัดสิน | Reject และ Reassign ตาม Policy |
| `TC-APP-010` | Approved Revision ถูก Patch | Reject Immutable |

กฎ Quick Estimate เป็น Future/Optional Module และแยกอยู่ที่ [Quick Estimate Pricing Rules](quick-estimate-pricing-rules.md) เพื่อไม่ให้ Share Policy ปะปนกับ Official Estimate Approval
