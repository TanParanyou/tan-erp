# Quick Estimate Flow (ราคาประเมินเบื้องต้นหน้างาน)

**สถานะ:** Accepted — หลักการและ Flow ใช้เป็นมาตรฐาน ส่วนค่าทางธุรกิจที่ระบุใน Validation Questions ยังต้องยืนยัน

## Purpose (เป้าหมาย)

ช่วยเจ้าหน้าที่หน้างานสร้าง **Price Range (ช่วงราคา)** ได้รวดเร็วจาก Pricing Template และข้อมูลขั้นต่ำ เพื่อใช้สนทนากับลูกค้าและติดตามโอกาสการขาย โดยไม่ทำให้ราคานี้กลายเป็น Quotation หรือ Approved Price

Quick Estimate เป็น Business Record แยกจาก [Official Estimate](estimation-flow.md) และเชื่อมกันด้วยการ Convert แบบ Snapshot เพื่อให้ข้อมูลหน้างานนำไปใช้ต่อได้โดยไม่ต้องคีย์ใหม่

## Flow (ลำดับการทำงาน)

```text
Customer/Opportunity → Work Type → Pricing Template → Measurement
    → Price Range → Evidence/Assumptions → Share Policy
        ├─ ต้องตรวจ → Internal Review ─┐
        └─ ผ่าน Policy ────────────────┴→ Preliminary Summary
                                            ↓
                                      Follow-up/Convert
                                            ↓
                                  Official Estimate Draft
```

ลำดับใช้งานจริง:

1. เลือกหรือสร้าง Customer และ Opportunity สำหรับงานที่กำลังสำรวจ
2. เลือกประเภทสถานที่ ประเภทงาน และ Pricing Template Version ที่ยังมีผล
3. กรอก Area/Room, Measurement หรือ Quantity และ Material Grade ตาม Template
4. ระบบคำนวณ Price Range พร้อมแสดง Assumptions และ Exclusions
5. แนบหลักฐานที่จำเป็น เช่น รูปถ่าย หมายเหตุ หรือรายการเงื่อนไขหน้างาน
6. ระบบตรวจ Permission, Scope และ Share Policy
7. หากผ่านจึงแสดงหรือส่ง Preliminary Summary; หากไม่ผ่านให้เก็บ Draft หรือส่งตรวจภายใน
8. บันทึก Customer Interest และ Follow-up หรือ Convert เป็น Official Estimate Draft

## Lifecycle (วงจรสถานะ)

```text
Draft → Calculated → Shareable → Shared → Converted
                    ↘ PendingReview ↗      └─ Read only
                              Shared → Closed
Calculated/Shared → Expired → New Version/Recalculate
```

| สถานะ | ความหมาย | การทำงานถัดไป |
| --- | --- | --- |
| `Draft` | กำลังเก็บข้อมูลและยังไม่พร้อมแสดงราคา | Calculate หรือ Discard |
| `Calculated` | มี Price Range และ Assumptions แล้ว | Edit หรือ Evaluate Share Policy |
| `PendingReview` | เข้าเงื่อนไขที่ต้องตรวจภายใน | Return หรือ Approve for Share |
| `Shareable` | ผ่าน Policy และพร้อมแสดงหรือส่ง | Share หรือ Edit ก่อนแชร์ |
| `Shared` | ส่ง Preliminary Summary แล้ว | Follow-up, Convert, Close หรือ Revise |
| `Converted` | สร้าง Official Estimate Draft แล้ว | อ่านและติดตามเอกสารปลายทาง |
| `Closed` | ลูกค้าไม่ดำเนินการหรือไม่ต้องติดตามต่อ | Reopen เป็น Version ใหม่ตามสิทธิ์ |
| `Expired` | พ้นวัน `validUntil` | Recalculate ด้วย Template ที่ใช้ได้ |

เมื่อ Price Range, Template, Measurement, Assumption หรือ Exclusion เปลี่ยนหลังแชร์ ต้องสร้าง Version ใหม่ ห้ามเขียนทับข้อมูลที่ลูกค้าเคยได้รับ

## Mobile UX (ประสบการณ์ใช้งานบนมือถือ)

หน้าจอเป็น Guided Flow สามช่วง โดยใช้ปุ่มใหญ่ Preset และ Progressive Disclosure เพื่อลดการพิมพ์:

1. **เลือกงาน:** Customer/Opportunity, Property Type, Work Type และ Pricing Template
2. **กรอกข้อมูลเร็ว:** Area/Room, Measurement/Quantity, Material Grade, รูปและ Voice/Text Note
3. **สรุปและดำเนินการ:** ตรวจ Price Range, Assumptions, Exclusions และเลือกบันทึก แชร์ หรือทำ Official Estimate ต่อ

ข้อกำหนด UX ขั้นต่ำ:

- Action หลักในหน้าสรุปไม่เกินสามรายการ
- เปิด Numeric Keyboard สำหรับตัวเลขและแสดงหน่วยติดกับ Field
- Touch Target ไม่น้อยกว่า 44px และใช้งานด้วย Keyboard ได้
- แสดงสถานะ `Saving`, `Saved` หรือ `Failed` อย่างชัดเจน
- หน้าที่ให้ลูกค้าดูต้องซ่อน Cost, Margin, Internal Note และ Approval Detail
- รองรับภาษาไทยเป็นค่าเริ่มต้น และมีคำอธิบายภาษาอังกฤษที่สำคัญ

## Rules (กฎบังคับ)

- Quick Estimate และ Official Estimate เป็นคนละ Record และมี Lifecycle แยกกัน
- Price ต้องแสดงเป็นช่วง พร้อม Currency, Assumptions, Exclusions และ Validity
- Preliminary Summary ต้องระบุชัดว่าเป็นราคาประเมินเบื้องต้น ไม่ใช่ Quotation
- ข้อมูลที่ลูกค้าเห็นต้องผ่าน Customer-safe projection และไม่มีข้อมูลต้นทุนภายใน
- การเปลี่ยนเนื้อหาสำคัญหลังแชร์ต้องสร้าง Quick Estimate Version ใหม่
- การ Convert ต้องทำแบบ Snapshot และ Idempotent: หนึ่ง Source Version สร้าง Official Estimate Draft เดียวสำหรับคำขอเดิม
- Official Estimate ต้องคำนวณต้นทุนและราคาขายตามกฎทางการใหม่ ห้ามใช้ค่ากลางของ Price Range เป็นราคาที่อนุมัติอัตโนมัติ
- Backend ในอนาคตต้องตรวจ Permission, Scope, Share Policy และ Maker–Checker ทุก Action สำคัญ

## Configurable Policy (ค่าที่ปรับได้)

องค์กรปรับรายการต่อไปนี้ได้โดยไม่เปลี่ยน Core Lifecycle:

- Pricing Template, Version และ Effective Period
- Measurement/Quantity Rule ตาม Work Type
- Reference Rate, Material Grade Factor และ Complexity Factor
- Range Percentage และ Validity Period
- Required Fields, Checklist, Default Assumptions และ Exclusions
- Share Threshold, เงื่อนไข Internal Review และผู้ตรวจ
- SLA, Assignment และ Notification
- Customer Summary Template และภาษา

Lifecycle, Audit, Versioning, Organization Scope, Permission และความหมายของเอกสารทางการเป็นข้อบังคับ ไม่ใช่ค่าที่ผู้ใช้ทั่วไปแก้ได้

## Reliability and Recovery (ความน่าเชื่อถือและการกู้คืน)

Release แรกใช้ Online-first พร้อมกลไกต่อไปนี้:

- Autosave หลังแก้ข้อมูล พร้อมสถานะการบันทึกที่มองเห็นได้
- Safe Local Draft เก็บข้อมูล Form ชั่วคราวเมื่อเครือข่ายขาด โดยต้องแจ้งว่ายังไม่ขึ้น Server
- Retry ต้องไม่สร้าง Record, Version, การแชร์ หรือ Conversion ซ้ำ
- การ Convert ใช้ Idempotency Key และคืนผลเดิมเมื่อส่งคำขอเดิมซ้ำ
- รูปที่ Upload ล้มเหลว Retry แยกได้โดยไม่ส่ง Form ใหม่ทั้งหมด
- Version Conflict ต้องให้ Reload/Compare ก่อนบันทึกทับ
- ห้าม Share หรือ Convert จน Server ยืนยัน Template Version และผลคำนวณล่าสุด

## Errors (ข้อผิดพลาด)

API ในอนาคตใช้ RFC 9457 Problem Details, Stable Error Code, Trace ID และข้อความตามภาษาผู้ใช้ โดยไม่ส่งรายละเอียดภายในระบบออกไป

| Error Code | HTTP | การตอบสนองของผู้ใช้ |
| --- | ---: | --- |
| `QUICK_ESTIMATE_INCOMPLETE` | 422 | แสดง Field หรือ Assumption ที่ต้องกรอก โดยรักษาข้อมูลเดิม |
| `QUICK_ESTIMATE_REVIEW_REQUIRED` | 422 | แจ้งว่าต้องส่งตรวจภายในก่อนแชร์ |
| `QUICK_ESTIMATE_TEMPLATE_EXPIRED` | 409 | ให้เลือก Template Version ที่ใช้ได้และ Recalculate |
| `QUICK_ESTIMATE_VERSION_CONFLICT` | 409 | ให้ Reload/Compare ก่อนแก้ต่อ |
| `QUICK_ESTIMATE_ALREADY_CONVERTED` | 409 | เปิด Official Estimate เดิม; Retry ด้วย Idempotency Key เดิมคืนผลเดิม |

รายละเอียดสัญญากลางอยู่ที่ [Error Contract](../03-contracts/error-contract.md)

## Audit and Metrics (ประวัติและตัวชี้วัด)

Audit ต้องบันทึกผู้ใช้ เวลา Resource, Version, Action และ Trace ID สำหรับเหตุการณ์ created, calculated, reviewed, shared, versioned, converted, closed และ expired รวมถึงการเปลี่ยน Pricing Template และ Manual Override พร้อมค่าก่อน/หลังและเหตุผล

Metrics ที่แนะนำ:

- เวลาจากเริ่มกรอกจนเห็น Price Range
- Completion rate และ Field ที่ทำให้หยุดบ่อย
- Share, Follow-up และ Conversion rate
- ความต่างระหว่าง Quick Estimate Range กับ Official Quotation
- Template/Work Type ที่คลาดเคลื่อนบ่อย

Metrics ต้องไม่เก็บ PII เกินความจำเป็นและใช้แทน Audit Trail ไม่ได้

## Acceptance Criteria (เกณฑ์ยอมรับ)

- เจ้าหน้าที่ทำ Flow ขั้นต่ำบนมือถือจนเห็น Price Range ได้ด้วย Preset และข้อมูลที่ Template กำหนด
- Draft Autosave และกู้ข้อมูลหลัง Network Error ได้โดยไม่เกิดรายการซ้ำ
- ลูกค้าเห็นเฉพาะข้อมูลภายนอก พร้อมคำเตือนว่าไม่ใช่ Quotation
- รายการที่เสี่ยงถูกบังคับ Review ตาม Share Policy
- การเปลี่ยนเนื้อหาสำคัญหลังแชร์สร้าง Version ใหม่
- Convert สร้าง Official Estimate Draft ที่อ้าง Source Version และไม่อนุมัติราคาคร่าว ๆ อัตโนมัติ
- Backend ตรวจ Permission, Scope และ Maker–Checker ทุก Action
- ภาษาไทย/อังกฤษ, Keyboard flow, Touch Target และ Error state ผ่านการทดสอบ
- Template Version เก่าคำนวณย้อนหลังได้จาก Snapshot
- Audit เชื่อม User, Action, Resource, Version, Time และ Trace ID ได้

## Out of Scope (นอกขอบเขต Release แรก)

- ออก Quotation ที่อนุมัติแล้ว ณ หน้างาน
- Free-form Workflow Builder ที่สร้าง State/Transition ใหม่ได้อิสระ
- Offline-first synchronization หลายอุปกรณ์
- AI ประเมินราคาจากภาพโดยไม่มี Template และการตรวจของคน
- ลูกค้าแก้ Work Scope หรือราคาผ่าน Public Portal
- แปลง Price Range เป็น Approved Selling Price อัตโนมัติ

## Validation Questions (คำถามที่ต้องยืนยัน)

- Work Type และ Property Type ใดต้องมีใน Release แรก
- Measurement Rule และสูตรของแต่ละ Work Type คืออะไร
- Reference Rate และ Grade/Complexity Factor มาจากใคร และทบทวนบ่อยเพียงใด
- Range Percentage และ Validity Period ของแต่ละประเภทงานเป็นเท่าใด
- หลักฐานขั้นต่ำก่อนแชร์มีอะไรบ้าง เช่น รูป จำนวนจุด หรือหมายเหตุ
- มูลค่า ความกว้างของช่วงราคา Manual Override หรือความเสี่ยงระดับใดต้องส่งตรวจ
- ใครเป็น Reviewer สำรอง และ SLA การตรวจหน้างานควรเป็นเท่าใด
- Preliminary Summary ส่งผ่านช่องทางใด และต้องเก็บ Delivery Status ระดับใด
