# CRM and Site Survey Responsive Wireframe (โครงหน้าจอลูกค้า งานขาย และสำรวจ)

**สถานะ:** Accepted Direction — Markdown Text Wireframe

เอกสารนี้กำหนด UX Baseline สำหรับ Desktop, Tablet และ Mobile โดยไม่สร้าง HTML/SVG ตัวอย่างทั้งหมดเป็น `TEST_ONLY`

## Responsive Model

| Context | Layout | งานหลัก |
| --- | --- | --- |
| Desktop ≥1200px | List/Table + Inspector/Workspace | ค้นหา จัดการหลายรายการ ตรวจความพร้อม |
| Tablet 768–1199px | List/Card + Side Sheet | นัดหมายและแก้ทีละรายการ |
| Mobile 320–767px | Search/List → Step Detail | รับลูกค้าด่วนและเก็บ Survey ทีละ Area |

## Customer and Opportunity

```text
ลูกค้าและงานขาย                         [+ ลูกค้า] [+ Opportunity]
[ค้นหา] [สาขา] [Owner] [Stage] [Next action]
┌─────────────────────────────┬──────────────────────────────┐
│ CUS-00042 บริษัทตัวอย่าง     │ Opportunity Inspector        │
│ OP-0018 Built-in ห้องนอน     │ Context | Contact | Site      │
│ Surveying · วันนี้ 14:00      │ Survey | Estimate | Activity │
└─────────────────────────────┴──────────────────────────────┘
```

- Search Result แสดงข้อมูล Contact เท่าที่ Permission อนุญาตและไม่แสดง Tax ID
- Stage Action แสดงเฉพาะ Transition ที่ใช้ได้ พร้อม Required Field ก่อนยืนยัน
- Duplicate Candidate เป็น Review Panel ไม่บังคับผู้ใช้ Merge
- Inspector ไม่ซ้อน Modal หลายชั้น; Save/Conflict/Last saved แสดงชัดเจน

## Survey Workspace

```text
SV-00012 · Revision 2 · Draft                    [บันทึก] [ตรวจความพร้อม]
Customer / Opportunity / Site / Visit / Surveyor

[1 Context] [2 Areas] [3 Checklist] [4 Evidence] [5 Readiness]
┌──────────────────────────────────────────────────────────────┐
│ ห้องนอนใหญ่                                                  │
│ กว้าง [____] m   ยาว [____] m   สูง [____] m                │
│ แหล่งข้อมูล [วัดจริง ▾]  [+ เพิ่มรูป] [+ เพิ่มจุดวัด]       │
└──────────────────────────────────────────────────────────────┘
Incomplete: Required photo 1 · Missing electrical constraint
```

- Readiness Summary ลิงก์ไป Field/Area แรกที่ผิด
- Evidence แสดง Uploading/Failed/Uploaded แยกกัน ห้ามแสดง Saved ก่อน Upload Complete
- Mark Ready แสดงผลที่จะถูกล็อก, Revision Number และรายการ Warning ก่อน Confirm
- Ready Revision เป็น Read-only พร้อม Action “สร้าง Revision ใหม่”

## Tablet and Mobile

- Tablet Portrait ใช้ Section เดียว + Sticky Save/Readiness Bar; Side Sheet ปิดแล้วคืน Focus เดิม
- Mobile Home แสดง Today Appointment, Recent Customer และ Draft ที่ยังไม่ Ready
- Quick Customer Capture ใช้ Name + Phone/Email ขั้นต่ำ แล้วพาไปเติมข้อมูลก่อน Activate
- Survey Mobile เรียง `Context → Area → Measurement → Checklist → Evidence → Readiness`
- ปุ่ม Previous/Next ไม่ข้าม Validation แบบเงียบ และ Draft ที่ Sync ไม่สำเร็จแสดง Retry
- Bulk Export/Duplicate Review จำนวนมากเป็น Desktop/Tablet Flow

## UI States

| State | การแสดง | Action |
| --- | --- | --- |
| Empty | อธิบาย Customer/Opportunity ที่ต้องสร้าง | Create |
| Possible duplicate | Match reason + masked contact | Use existing/Create anyway ตามสิทธิ์ |
| Draft/Incomplete | Missing count + field links | Continue |
| Saving/Failed | Progress/Error + retry | Retry with same intent |
| Conflict | Latest editor/time + reload/compare | ไม่ Last-write-wins |
| Uploading/Failed evidence | Per-file status | Retry/remove Draft reference |
| Ready | Locked badge + snapshot hash/time | Create Estimate/Clone Revision |
| Superseded/Void | Reason + replacement link | Historical read only |
| Forbidden/Out of scope | Generic forbidden/not found | ไม่เปิดเผยข้อมูล |

## Accessibility and Privacy

- Touch Target ≥44×44px, Mobile Input ≥48px, Body Text Mobile ≥16px
- Visible Focus, Semantic Heading/Label/Error Summary และ Keyboard Order ตามลำดับงาน
- Status/Validation/Upload Result ไม่ใช้สีอย่างเดียว; รองรับ Zoom 200% และ Reduced Motion
- Contact/Location ใช้ Mask/Reveal ตาม Permission; Copy/Export เป็น Action ที่ Audit ได้
- ภาษาไทยเป็น Default และ English Label มีคำไทยกำกับในจุดสำคัญ

## Source Documents

- [Flow](crm-site-survey-flow.md)
- [Field Catalog](crm-site-survey-field-catalog.md)
- [Governance](crm-site-survey-governance.md)
- [API Contract](../03-contracts/crm-site-survey-api-contract.md)
- [Data Contract](../04-data/crm-site-survey-data-contract.md)
