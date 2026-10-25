# Quick Estimate Convert: Survey Revision Picker (2026-10-25)

**สถานะ:** Draft — รอเจ้าของงานอนุมัติแผนก่อนเริ่ม implementation

## ปัญหา

Dialog "แปลงเป็นใบประเมินทางการ" ของ Quick Estimate ให้ผู้ใช้พิมพ์ `siteSurveyRevisionId` (UUID) เอง ([quick-estimate-workspace.tsx:310](../../../frontend/src/features/quick-estimates/components/quick-estimate-workspace.tsx)) ทั้งที่ UI ไม่แสดง UUID ที่ไหน และ placeholder `SRV-2026-0001` ทำให้เข้าใจผิด; backend ตอบ 400 และ UI แสดงแค่ "ดำเนินการไม่สำเร็จ".

## ข้อค้นพบที่เปลี่ยนแผน (แก้ข้อสรุปเดิมที่ว่าต้องเพิ่ม endpoint องค์กรทั้งหมด)

- `QuickEstimateStore.ConvertAsync` ([QuickEstimateStore.cs:~526-532](../../../backend/src/TanErp.Infrastructure/Persistence/QuickEstimates/QuickEstimateStore.cs)) **ต้องการ `estimate.OpportunityId`** (มิฉะนั้น `QUICK_ESTIMATE_OPPORTUNITY_REQUIRED`) แล้วเรียก `CreateDraftAsync(orgId, opportunityId, siteSurveyRevisionId)` ซึ่งตรวจว่า revision เป็นของ opportunity เดียวกัน และสถานะ `ready`/`superseded` (`SURVEY_NOT_READY`).
- ดังนั้น revision ที่ใช้ได้มีได้เฉพาะของ Opportunity ที่ผูกกับ Quick Estimate เท่านั้น. API ที่มีอยู่แล้ว `GET /api/v1/opportunities/{id}/surveys` คืน `latestReadyRevision {id, revisionNumber, snapshotHash}` ([contract](../../03-contracts/crm-site-survey-api-contract.md)) และ FE มี hook `useOpportunitySurvey` — **ไม่ต้องเพิ่ม endpoint ระดับองค์กร**.
- `QuickEstimateResponse` มี `opportunityId` อยู่แล้ว และ `CreateQuickEstimateRequest` รับ `OpportunityId?` แต่ **FE ไม่มีที่ผูก Opportunity เลย** (ค้นแล้วไม่พบ `opportunity` ใน `features/quick-estimates/components`). ตัว save draft ยังไม่ยืนยันว่ามีฟิลด์ opportunity ให้แก้หลังสร้าง (ต้องตรวจ `SaveDraft` request ก่อนลงมือ).
- สาเหตุที่ Haiku ลองแปลง QE-2026-0001 ไม่ได้อาจเป็นทั้ง UUID ผิด **และ** QE ไม่มี Opportunity ผูก (ยังไม่ได้ตรวจค่า `opportunityId` ของ QE ตัวอย่าง).

## แผนที่เสนอ (FE เป็นหลัก, ไม่แก้ backend ถ้า SaveDraft รองรับ opportunity อยู่แล้ว)

1. **Dialog แปลง:** แทน Input UUID ด้วยการแสดงผลแบบอ่านอย่างเดียว — ถ้า QE มี `opportunityId` เรียก `useOpportunitySurvey(opportunityId)` แล้วแสดง "ใบสำรวจ {surveyNumber} รุ่น {revisionNumber}" ของ `latestReadyRevision` และส่ง `id` นั้นเอง (ไม่มีช่องกรอก; ไม่มีการเดา/fallback). ถ้าไม่มีรุ่นที่ `ready` แสดง `Alert` ว่ายังไม่มีผลสำรวจที่พร้อม พร้อมลิงก์ไปหน้า Opportunity และปิดปุ่มยืนยัน. ถ้า QE ไม่มี Opportunity แสดง `Alert` ให้ผูก Opportunity ก่อน.
2. **ผูก Opportunity ใน Quick Estimate:** เพิ่ม `OpportunityAutocomplete` (ของเดิม [opportunity-autocomplete.tsx](../../../frontend/src/features/opportunities/components/opportunity-autocomplete.tsx)) ในฟอร์ม Quick Estimate (สร้างและแก้ไข) ตาม Reuse First; ถ้า Save Draft ยังไม่รับ `opportunityId` ต้องเพิ่มที่ backend (Request/Handler/Store + OpenAPI + `make api-gen`) — **ขอความเห็นชอบก่อนแตะ backend**.
3. **ข้อความ error:** map `QUICK_ESTIMATE_OPPORTUNITY_REQUIRED`, `SURVEY_NOT_READY` (และ 400 ของ revision) เป็นข้อความ th/en ผ่านรูปแบบ error code ที่ `quickEstimates.errors` ใช้อยู่ ไม่เทียบสตริงดิบ.
4. **i18n:** เพิ่ม key ที่ใช้ทั้ง `th.json`/`en.json`; ลบ key `surveyRevisionId*` ที่ไม่ใช้แล้ว (ตรวจว่าไม่มีที่อื่นอ้าง).
5. **ตรวจ:** `npx tsc --noEmit`, `npm run lint`, key parity th/en; ดูในเบราว์เซอร์ด้วย QE ที่ผูก Opportunity ที่มีผลสำรวจ `ready` (ต้องเตรียมข้อมูล demo: ทำ Mark Ready ให้ใบสำรวจ demo หรือสร้างใหม่). ไม่เขียน/รัน test ตามที่ตกลง.

## การแบ่งงาน

- **Sonnet:** ตรวจ `SaveDraft` และกติกา `CreateDraftAsync`, ตัดสินว่าต้องแตะ backend หรือไม่, รีวิว diff.
- **Haiku:** แก้ dialog/ฟอร์มและ i18n ตามข้อ 1-4, เตรียมข้อมูล demo ผ่าน UI, ตรวจในเบราว์เซอร์.

## คำถามที่ต้องตัดสินก่อนเริ่ม

1. ผูก Opportunity ตอนสร้างอย่างเดียว หรือแก้ภายหลังได้ด้วย (ถ้าแก้ภายหลังอาจต้องแตะ backend)?
2. ถ้า Opportunity มีหลายใบสำรวจ ให้ใช้ `latestReadyRevision` ของใบล่าสุดเสมอ หรือให้เลือกเอง? (แผนนี้เสนอใช้ค่าที่ API กำหนดโดยไม่ต้องเลือก ตาม contract ที่ใช้เป็น "Source ของ Estimate")
