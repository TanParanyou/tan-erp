# Quick Estimate Workflow Design

## 1. สถานะเอกสาร

- วันที่ออกแบบ: 6 กันยายน 2026
- สถานะ: Accepted
- ขอบเขต: ราคาประเมินเบื้องต้น ณ หน้างาน และการส่งต่อเป็น Official Estimate
- ระบบ: `tan-erp` Project ERP

## 2. เป้าหมาย

ช่วยเจ้าหน้าที่หน้างานแจ้งช่วงราคาคร่าว ๆ ให้ลูกค้าได้รวดเร็วผ่านหน้าจอที่ใช้ง่าย โดยเก็บข้อมูลครั้งเดียวและนำไปทำ Official Estimate ต่อได้ ขณะเดียวกันต้องป้องกันไม่ให้ Quick Estimate ถูกเข้าใจหรือใช้งานแทน Quotation ที่ผ่านการตรวจและอนุมัติแล้ว

## 3. การตัดสินใจหลัก

ใช้ **Quick Estimate เป็น Business Record แยกจาก Official Estimate** และเชื่อมกันด้วยการ Convert แบบ Snapshot

```text
Quick Estimate
    │
    ├─ แสดง/แชร์ Preliminary Summary
    │
    └─ Convert พร้อม Snapshot
             ↓
      Official Estimate Draft
             ↓
      Cost Review → Approval → Quotation
```

เหตุผลที่ไม่ใช้ Record เดียวกันคือ Quick Estimate อนุญาตให้ข้อมูลบางส่วนยังไม่ครบ ใช้ช่วงราคาและสมมติฐาน ขณะที่ Official Estimate ต้องมีต้นทุน กฎคำนวณ Revision และ Approval ที่เข้มงวดกว่า การแยก Record ลดความเสี่ยงที่สถานะหรือ Permission ของเอกสารสองชนิดปะปนกัน

## 4. End-to-End Flow

```text
เริ่มเยี่ยมหน้างาน
    ↓
เลือก/สร้าง Customer และ Opportunity
    ↓
เลือกประเภทสถานที่และประเภทงาน
    ↓
เลือก Pricing Template และ Material Grade
    ↓
กรอก Measurement / Quantity แบบรวดเร็ว
    ↓
ระบบคำนวณ Price Range และแสดง Assumptions
    ↓
เพิ่มรูป หมายเหตุ และ Exclusions
    ↓
ตรวจเงื่อนไขการแชร์
    ├─ ข้อมูลหรือสิทธิ์ไม่ผ่าน → บันทึก Draft / ส่งตรวจภายใน
    └─ ผ่าน → แสดงหรือแชร์ Preliminary Summary
                     ↓
               Customer Interest
                 ├─ Not Interested → Closed พร้อมเหตุผล
                 ├─ Follow-up → กำหนดผู้รับผิดชอบและวันติดตาม
                 └─ Interested → Convert
                                      ↓
                              Official Estimate Draft
```

## 5. Quick Estimate Lifecycle

| สถานะ | ความหมาย | Transition ที่อนุญาต |
| --- | --- | --- |
| `Draft` | กำลังเก็บข้อมูล ยังไม่แสดงเป็นสรุป | Calculate, Discard |
| `Calculated` | มี Price Range และ Assumptions แล้ว | Edit, Evaluate Share Policy |
| `PendingReview` | ต้องให้ผู้มีสิทธิ์ตรวจตาม Policy | Return to Calculated, Approve for Share |
| `Shareable` | ผ่าน Policy และพร้อมแสดง/ส่ง | Share, Edit before sharing |
| `Shared` | ส่ง Preliminary Summary แล้ว | Follow-up, Convert, Close, Revise |
| `Converted` | สร้าง Official Estimate Draft แล้ว | Read only; follow source link |
| `Closed` | ลูกค้าไม่ดำเนินการหรือรายการหมดความจำเป็น | Reopen as New Version ตามสิทธิ์ |
| `Expired` | พ้น `validUntil` | Recalculate as New Version |

การเปลี่ยน Price Range, Template, Measurement, Assumption หรือ Exclusion หลังแชร์ต้องสร้าง Version ใหม่ ห้ามเขียนทับสิ่งที่ลูกค้าเคยได้รับ

เมื่อประเมิน Share Policy จาก `Calculated` แล้ว ระบบเปลี่ยนเป็น `Shareable` อัตโนมัติหากผ่านทุกเงื่อนไข หรือเปลี่ยนเป็น `PendingReview` หากเข้าเกณฑ์ที่ต้องตรวจภายใน

## 6. Mobile-first UX

ใช้ Guided Flow สามช่วงและบันทึกอัตโนมัติ:

### 6.1 เลือกบริบทงาน

- Customer หรือ Quick Customer Capture
- Opportunity หรือการเยี่ยมหน้างานใหม่
- Property Type เช่น บ้าน คอนโด หรือสำนักงาน
- Work Type เช่น Built-in, Curtain, Wallpaper หรือ Electrical
- Pricing Template ที่ตรงกับ Work Type

การเลือกหลักใช้ปุ่มขนาดใหญ่และ Preset แทน Dropdown ยาว ข้อมูลเพิ่มเติมเปิดเมื่อจำเป็นด้วย Progressive Disclosure

### 6.2 กรอกข้อมูลเร็ว

- Area/Room
- Measurement หรือ Quantity ตาม Template
- Material Grade: Standard, Premium หรือ Custom
- รูปถ่ายและ Voice/Text Note
- ตัวเลือกสำคัญที่มีผลต่อราคา

ระบบเปิด Numeric Keyboard สำหรับตัวเลข แสดงหน่วยติดกับ Field รองรับการทำซ้ำรายการ และ Autosave หลังการเปลี่ยนแปลงโดยไม่บล็อกการกรอก

### 6.3 สรุปและดำเนินการ

แสดงช่วงราคา สิ่งที่รวม สิ่งที่ไม่รวม สมมติฐาน ระดับความแม่นยำ และวันหมดอายุ โดยมี Action หลักไม่เกินสามรายการ:

1. บันทึก
2. แชร์ราคาประเมินเบื้องต้น
3. ทำ Official Estimate ต่อ

หน้าที่แสดงให้ลูกค้าดูต้องซ่อนต้นทุน Margin Internal Note และ Approval Detail

## 7. Calculation Model

Quick Estimate ใช้ Pricing Template ที่มี Version และ Effective Period:

```text
Base Quantity = ผลจาก Measurement Rule
Base Price = Base Quantity × Reference Rate
Adjusted Price = Base Price × Grade Factor × Complexity Factor
Lower Bound = Adjusted Price × (1 - Range Percentage)
Upper Bound = Adjusted Price × (1 + Range Percentage)
```

สูตรนี้เป็นโครงสร้างทั่วไป ไม่ใช่สูตรธุรกิจที่อนุมัติแล้ว Template แต่ละประเภทงานเป็นเจ้าของ Measurement Rule, Rate Source, Factor, Range Percentage, Default Assumptions และ Exclusions

ระบบต้องบันทึก Template Version, Input, Factor และผลลัพธ์ที่ใช้คำนวณทุกครั้ง เพื่อให้คำนวณย้อนหลังได้แม้ Template ปัจจุบันเปลี่ยนแล้ว

## 8. Share Policy

การแชร์ Preliminary Summary อนุญาตเมื่อ:

- มี Customer/Contact หรือช่องทางผู้รับที่ตรวจสอบได้
- Work Type, Quantity/Measurement และ Material Grade ครบตาม Template
- Price Range เป็นค่าบวกและ Template ยังมีผล
- Assumptions, Exclusions และ `validUntil` ถูกแสดง
- ผู้ใช้มี `quick-estimates.share`
- มูลค่า ความกว้างของช่วงราคา ความเสี่ยง หรือ Manual Override ไม่เกิน Policy ของตน

หากไม่ผ่าน ระบบยังบันทึก Draft ได้ แต่ต้องส่ง `PendingReview` หรือแจ้งข้อมูลที่ขาดแบบระบุ Field

## 9. Customer-facing Summary

เอกสารหรือลิงก์ที่ส่งให้ลูกค้าต้องมี:

- คำว่า “ราคาประเมินเบื้องต้น / Preliminary Estimate” ชัดเจน
- ช่วงราคา ไม่ใช่ราคาตายตัว
- Work Scope แบบสั้น
- Assumptions และ Exclusions
- วันที่จัดทำและวันหมดอายุ
- ผู้ติดต่อของบริษัท
- Disclaimer ว่าไม่ใช่ Quotation และราคาจริงต้องผ่านการสำรวจรายละเอียดและอนุมัติ
- Version/Reference Number ที่ Support ค้นหาได้

Customer-facing Summary ห้ามมีต้นทุน Margin Internal Comment Permission หรือข้อมูลของลูกค้ารายอื่น

## 10. Convert to Official Estimate

การ Convert ทำงานแบบ Idempotent และสร้าง Official Estimate Draft หนึ่งรายการต่อ Quick Estimate Version เว้นแต่ผู้ใช้เลือกสร้าง Revision ใหม่อย่างชัดเจน

ข้อมูลที่ส่งต่อ:

- Customer, Opportunity และ Site
- Work Type, Area และ Measurement
- Selected Template และ Material Grade
- Photos, Notes, Assumptions และ Exclusions
- Quick Estimate Price Range เป็น Reference เท่านั้น
- `sourceQuickEstimateId` และ `sourceQuickEstimateVersion`

Official Estimate ต้องคำนวณต้นทุนและราคาขายใหม่ตามกฎทางการ ห้ามนำค่ากลางของ Price Range ไปเป็น Approved Selling Price อัตโนมัติ

## 11. Configurable Policy

ปรับได้โดยไม่เปลี่ยน Core Lifecycle:

- Pricing Template และ Version
- Measurement/Quantity Rule ตาม Work Type
- Reference Rate และ Material Grade Factor
- Range Percentage และ Validity Period
- Required Fields, Checklist, Assumptions และ Exclusions
- Share Threshold และผู้ตรวจภายใน
- SLA, Assignment และ Notification
- Customer Summary Template และภาษา

Core Lifecycle, Audit, Versioning, Organization Scope, Permission และความหมายของ Official Document เป็นข้อบังคับที่ผู้ใช้ทั่วไปเปลี่ยนไม่ได้

## 12. Permissions and Scope

- `quick-estimates.read`
- `quick-estimates.create`
- `quick-estimates.update`
- `quick-estimates.review`
- `quick-estimates.share`
- `quick-estimates.convert`
- `quick-estimates.close`
- `pricing-templates.read`
- `pricing-templates.manage`

ทุก Permission ใช้ร่วมกับ Organization, Branch, Project/Opportunity หรือ Own Scope ตาม Membership ผู้สร้างห้าม Review รายการของตนเองเมื่อ Share Policy กำหนด Maker–Checker

## 13. Reliability and Connectivity

Release แรกเป็น **Online-first พร้อม Autosave และ Retry**:

- Draft บันทึกหลังการเปลี่ยนแปลงโดยมีสถานะ Saving/Saved/Failed ชัดเจน
- Request ซ้ำจาก Retry ต้องไม่สร้าง Quick Estimate หรือ Version ซ้ำ
- เมื่อขาดสัญญาณ ให้รักษาข้อมูล Form บนอุปกรณ์ชั่วคราวและแจ้งว่ายังไม่บันทึกขึ้น Server
- ห้ามแชร์หรือ Convert จน Server ยืนยัน Template Version และผลคำนวณล่าสุด
- Offline เต็มรูปแบบและการ Sync หลายอุปกรณ์อยู่นอก Release แรก แต่ Contract ต้องมี Version/Idempotency รองรับการเพิ่มภายหลัง

## 14. Error and Recovery

- Field format ผิด: แสดง Error ใกล้ Field โดยไม่ล้างข้อมูลอื่น
- Template หมดอายุ: บังคับ Recalculate ด้วย Version ปัจจุบัน
- Permission/Scope เปลี่ยน: หยุด Share/Convert และรักษา Draft ที่ปลอดภัย
- Concurrent edit: แสดง Version Conflict และให้ Reload/Compare
- Upload รูปล้มเหลว: Retry แยกรูปได้โดยไม่ส่ง Form ใหม่ทั้งหมด
- Convert ซ้ำ: คืน Official Estimate เดิมจาก Idempotency Record
- Share ล้มเหลว: บันทึกสถานะการส่งและอนุญาต Retry โดยไม่สร้าง Version ใหม่เมื่อเนื้อหาเดิม

Error ใช้ RFC 9457 Problem Details, Stable Code, Trace ID และข้อความตามภาษาผู้ใช้ตามมาตรฐานเดิมของระบบ

## 15. Audit and Metrics

Audit Events สำคัญ:

- Quick Estimate created, calculated, reviewed, shared, versioned, converted, closed และ expired
- Pricing Template created, published, superseded และ disabled
- Manual Override พร้อมค่าก่อน/หลังและเหตุผล

Business Metrics ที่ควรวัด:

- เวลาจากเริ่มกรอกถึงเห็น Price Range
- อัตราการกรอกสำเร็จและ Field ที่ทำให้หยุดบ่อย
- Share rate, follow-up rate และ conversion rate
- ความต่างระหว่าง Quick Estimate Range กับ Official Quotation
- Template/Work Type ที่คลาดเคลื่อนบ่อย

Metrics ห้ามเก็บ PII เกินความจำเป็นและไม่ใช้แทน Audit Trail

## 16. Acceptance Criteria

1. เจ้าหน้าที่สร้าง Price Range จาก Preset และ Measurement ขั้นต่ำได้ด้วยมือถือ
2. Draft Autosave และกู้ข้อมูลหลัง Network Error ได้โดยไม่เกิดรายการซ้ำ
3. ลูกค้าเห็นเฉพาะข้อมูลภายนอกและคำเตือนว่าไม่ใช่ Quotation
4. รายการที่เสี่ยงถูกบังคับ Review ตาม Share Policy
5. การเปลี่ยนข้อมูลสำคัญหลังแชร์สร้าง Version ใหม่
6. Convert สร้าง Official Estimate Draft ที่เชื่อม Source Version และไม่ใช้ราคาคร่าว ๆ เป็นราคาทางการอัตโนมัติ
7. Backend ตรวจ Permission, Scope และ Maker–Checker ทุก Action
8. ไทย/อังกฤษ Keyboard flow Touch Target และ Error state ผ่านการทดสอบ
9. Template Version เก่าคำนวณย้อนหลังได้จาก Snapshot
10. Audit เชื่อม User, Action, Resource, Version, Time และ Trace ID ได้

## 17. Out of Scope for First Release

- ออก Quotation ที่อนุมัติแล้ว ณ หน้างาน
- Workflow Builder ที่สร้าง State/Transition ใหม่ได้อิสระ
- Offline-first synchronization หลายอุปกรณ์
- AI ประเมินราคาจากภาพโดยไม่มี Template/การตรวจคน
- ลูกค้าแก้ Work Scope หรือราคาผ่าน Public Portal
- Auto-convert Price Range เป็น Approved Selling Price
