# CRM and Site Survey UAT Scenarios (สถานการณ์ทดสอบรับรองผู้ใช้)

**สถานะ:** Accepted Direction — Development Baseline

ข้อมูลทั้งหมดเป็น `TEST_ONLY` UAT ใช้บัญชีและข้อมูลอย่างน้อยสอง Organization/Branch และไม่ใช้ข้อมูลลูกค้าจริง

## Critical Scenarios

| ID | Role | Scenario | Expected |
| --- | --- | --- | --- |
| `UAT-CRM-001` | Sales | สร้าง Customer + Primary Contact แล้ว Activate | ได้ Code/ETag; ค้นหาได้ตาม Scope |
| `UAT-CRM-002` | Sales/Data Steward | สร้างชื่อ/เบอร์คล้ายรายการเดิม | แสดง Candidate แบบ Mask; ไม่ Auto-merge |
| `UAT-CRM-003` | Sales | แก้ Customer พร้อมกันสอง Session | Session หลังได้ Version Conflict; ข้อมูลแรกไม่หาย |
| `UAT-CRM-004` | Sales | สร้าง Opportunity จาก Active Customer แล้ว Qualify | Branch/Owner/Scope/Next Action ครบ; มี Stage History |
| `UAT-CRM-005` | Sales | ข้าม Stage หรือ Close โดยไม่มี Reason | Stable Error; State ไม่เปลี่ยน |
| `UAT-CRM-006` | Sales Manager | Reopen Lost Opportunity พร้อมสิทธิ์/Reason | Open Stage + Append-only History/Audit |
| `UAT-SRV-001` | Coordinator | สร้าง Site + Survey Appointment | Survey + Draft Revision 1 แบบ Atomic |
| `UAT-SRV-002` | Surveyor | บันทึกหลาย Area/Measurement/Checklist/Evidence | Draft/ETag/Upload State ถูกต้อง |
| `UAT-SRV-003` | Surveyor | Measurement ผิด Unit, ≤0 หรือ Derived Cycle | Field/Area Error; ไม่บันทึกค่าผิด |
| `UAT-SRV-004` | Surveyor | Mark Ready ขณะ Required Evidence Upload ค้าง | `SURVEY_NOT_READY`; Draft ไม่ถูกล็อก |
| `UAT-SRV-005` | Surveyor | Mark Ready เมื่อ Gate ครบ | Ready immutable + snapshot hash/audit |
| `UAT-SRV-006` | Surveyor | กลับไปวัดใหม่ | Clone เป็น Draft Revision 2; Revision 1 ไม่เปลี่ยน |
| `UAT-SRV-007` | Estimator | สร้าง Official Estimate จาก Ready Revision 1 | Estimate เก็บ Revision ID/hash/source snapshot |
| `UAT-SRV-008` | Survey Reviewer | Ready Revision 2 ภายหลัง | Estimate เดิมยังอ้าง Revision 1; งานใหม่เลือก Revision 2 |
| `UAT-SRV-009` | Authorized User | Void Revision ที่ Estimate อ้าง | เก็บ History/Risk/Audit; ไม่ลบ Estimate/Revision |
| `UAT-SRV-010` | Surveyor | เริ่ม/Ready Survey เมื่อไม่มี Published Template ตรง Work Type/วันที่ | Fail-closed; ไม่เลือก Template อื่นเงียบ ๆ |
| `UAT-SEC-001` | Unauthorized User | Search/Read Customer, Site, Survey อีก Organization | 404; ไม่มี PII/metadata รั่ว |
| `UAT-SEC-002` | Support/Auditor | ตรวจ Log/Export/Error | ไม่มี Phone/Email/Tax ID/Signed URL เกิน Allowlist |
| `UAT-UX-001` | Mobile/Keyboard User | รับ Customer และเก็บ Survey ที่ 320px/Zoom 200% | Focus/Target/Error/Save State ใช้งานได้; ไม่พึ่งสี |
| `UAT-I18N-001` | Thai/English User | Trigger Validation เดียวกัน | Code เดิม; ข้อความตามภาษา; fallback ไทย |

## Exit Criteria

- Scenario `001–020` ผ่านหรือมี Business Decision ที่อนุมัติการเปลี่ยน
- Sales Owner ยืนยัน Stage/Required Field/Outcome และ Duplicate handling
- Survey Owner ยืนยัน Measurement/Unit/Checklist/Evidence/Readiness
- Privacy/Security Owner ยืนยัน Field Classification, Mask, Export, Retention และ Redaction
- Official Estimate Integration ยืนยัน Ready Revision ID/hash และ Historical Reproducibility
- ตัวอย่าง `TEST_ONLY` ไม่ถูกนำไป Seed เป็น Production Data

## Customer + Contact Slice 1 Exit Criteria

สำหรับ Slice 1 (Customer + Contact Vertical Slice) จะถือว่าผ่านเกณฑ์เมื่อ:
1. **`UAT-CRM-001` (Subset):** สร้าง Customer Draft พร้อม Primary Contact และค้นหาได้ตาม Scope (ส่วนการ Activate เลื่อนไปอยู่ใน Slice ถัดไป)
2. **`UAT-CRM-002`:** ตรวจจับชื่อ/เบอร์ตรงกันภายใน Organization เดียวกัน และแสดง Candidate แบบ Mask โดยไม่ Auto-merge
3. **`UAT-SEC-001`:** สมาชิกภาพต่าง Organization ไม่สามารถเปิดดูหรือค้นหา Customer ของอีก Organization ได้ (`404 Not Found`)
4. **`UAT-SEC-002`:** Log, Audit Trail และ Problem Details ปราศจาก Phone, Email, Tax ID หรือ Authorization Bearer Token
5. **`UAT-UX-001`:** ฟอร์มและหน้าจอแสดงผลถูกต้องบนขนาด 320px และ Zoom 200%, Touch target ≥ 44px, Keyboard accessible, ไม่ใช้สีเพียงอย่างเดียวในการสื่อสาร
6. **`UAT-I18N-001`:** ระบบแปลภาษาครบถ้วนทั้ง `th` และ `en` เมื่อเกิดข้อผิดพลาดคืน Error Code เดิมพร้อมคำอธิบายตามภาษา
7. **Idempotency Verification:** การส่ง Request ซ้ำด้วย `Idempotency-Key` เดิมและ Payload เดิม ต้องไม่สร้างข้อมูลซ้ำและคืน Response เดิม; หาก Payload ต่างกันต้องคืน `409 IDEMPOTENCY_KEY_REUSED`

## Customer Activation + Opportunity + Site Slice 2 Exit Criteria

สำหรับ Slice 2 (Customer Activation + Opportunity + Site Vertical Slice) จะถือว่าผ่านเกณฑ์เมื่อ:
1. **`UAT-CRM-001` (Activation Remainder):** ผู้ใช้ที่มีสิทธิ์ `customers.activate` สามารถ Activate Customer Draft ให้เป็น Active ได้สำเร็จโดยส่ง `If-Match` ตรงกับ ETag ปัจจุบันและ `Idempotency-Key`; เมื่อ ETag เก่าคืน `409 CUSTOMER_VERSION_CONFLICT`; เมื่อขาด If-Match คืน `428 IF_MATCH_REQUIRED`
2. **`UAT-SRV-001` (Site Creation Portion):** สามารถสร้าง Active Site ภายใต้ Customer ที่ Active พร้อมที่อยู่แบบมีโครงสร้างครบถ้วน; ห้ามสร้าง Site ภายใต้ Customer ที่ไม่ใช่ Active หรืออยู่นอก Organization Scope
3. **`UAT-CRM-004` (Create-Draft Subset Only):** สามารถสร้าง Opportunity Draft จาก Active Customer โดย Backend Derive Branch และ Owner จาก Active Membership และ Primary Site ต้องเป็นของ Customer เดียวกัน; ไม่รวม Stage Transition/Qualify หรือ Stage History ซึ่งเลื่อนไป Slice ถัดไป
4. **`UAT-SEC-001`:** ผู้ใช้องค์กรอื่นไม่สามารถอ่านหรือใช้ Customer, Site, Opportunity ขององค์กรอื่นได้ (`404 RESOURCE_NOT_FOUND`)
5. **`UAT-SEC-002`:** ที่อยู่ Site, พิกัด และ Access Note ไม่รั่วไหลใน Log, Audit Trail หรือ Problem Details (เก็บเฉพาะ Changed Field Names/IDs)
6. **`UAT-UX-001`:** หน้าจอสร้าง Site, รายการ Opportunity, สร้าง Opportunity และดูรายละเอียด รองรับ Responsive 320px, Zoom 200%, Touch targets ≥ 44px, Keyboard navigation, Atelier Architectural Navy Sharp (0px radius) และ Minimal Mono Loading
7. **`UAT-I18N-001`:** รองรับทั้งภาษาไทยและภาษาอังกฤษสมบูรณ์พร้อม Key parity บน UI messages และ Backend localized Problem Details
8. **Idempotency & Concurrency:** Retry ด้วย Idempotency Key เดิมและ Payload เดิมต้องคืน Resource เดิมเสมอ; Key เดิมกับ Payload เปลี่ยนคืน `409 IDEMPOTENCY_KEY_REUSED`
9. **Explicitly Deferred:** Opportunity Qualify/Stage Transition, Stage History, Owner Reassignment, Survey Appointment, Survey Identity/Revision และ File Upload เลื่อนไป Slice ถัดไป

อ้างอิง [Flow](../01-business/crm-site-survey-flow.md), [API Contract](../03-contracts/crm-site-survey-api-contract.md) และ [Data Contract](../04-data/crm-site-survey-data-contract.md)
