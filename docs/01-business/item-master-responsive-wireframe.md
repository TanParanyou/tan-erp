# Item Master Responsive Wireframe (โครงหน้าจอข้อมูลสินค้าและต้นทุน)

**สถานะ:** Accepted Direction — UX Baseline สำหรับ Desktop, Tablet และ Mobile

## เป้าหมาย

ให้ผู้ดูแลค้นหา สร้าง แก้ และตรวจ Item/Unit/Cost ได้โดยเห็นสถานะ ความเสี่ยง และผลกระทบชัดเจน Mobile เน้นค้นหา/แก้ทีละรายการ ส่วน Bulk Import เป็นแนวทางสำหรับระยะถัดไปตาม [Estimate Master Data Maintenance Plan](../superpowers/plans/2026-09-23-estimate-master-data-maintenance.md)

เอกสารนี้ใช้ Text Wireframe เพื่อคงขอบเขต Documentation-only; ตัวเลขทั้งหมดเป็น `TEST_ONLY`

## Responsive Model

| Context | Layout | งานหลัก |
| --- | --- | --- |
| Desktop ≥1200px | Item Table + Detail Inspector + Cost Timeline | จัดการหลายรายการและตรวจต้นทุน |
| Tablet 768–1199px | Table/Card + Side Sheet | ตรวจ/แก้ทีละ Item และ Cost |
| Mobile 320–767px | Search/List → Detail | ค้นหา ดู Current Cost และแก้ Field สำคัญ |

## Desktop

```text
Item Master                    [+ Item] [Reference Data]
[Search] [Type] [Category] [Capability] [Status] [Cost State]
┌──────────────────────────────┬─────────────────────────────┐
│ Code / Name / Type / Unit    │ Item Inspector              │
│ Capability / Cost / Status   │ General | Units | Costs     │
│ MAT-PLY-18 ...               │ Current + Future + History  │
└──────────────────────────────┴─────────────────────────────┘
```

- Table ตรึง Code/Name และรองรับ Keyboard Row Navigation
- Inspector แสดง General, Capability, Unit/Conversion, Cost และ Audit เป็น Section ไม่ซ้อน Modal
- General แสดง `Item Code / SKU` ตามชนิด Item; Barcode เป็น Section แยกพร้อมค่า, ชนิด GTIN/Internal, หน่วย, จำนวนต่อบรรจุ, ระดับบรรจุ, Primary และสถานะ. แสดงเลขเต็มและ copy ได้; ห้ามใช้ภาพ Barcode แทนข้อความที่อ่านได้
- Cost Timeline แยก Current/Future/Expired/Disabled และติด Branch Override ชัดเจน
- Import Preview เป็นแบบสำหรับระยะถัดไป; ห้ามแสดงปุ่ม Import ที่ยังใช้งานไม่ได้ใน Slice นี้

## Tablet

- Landscape ใช้ List 55% + Inspector 45%
- Portrait ใช้ List เต็มพื้นที่และ Side Sheet ≤90%
- Cost History ใช้ Card ตาม Effective Period ห้ามบังคับ Horizontal Scroll เพื่ออ่าน Amount/Status
- ปิด Side Sheet แล้วคืน Focus ไป Item เดิม

## Mobile

```text
Item Master                    [+]
[ค้นหา code / ชื่อ]
[Material] [Missing cost] [Inactive]

MAT-PLY-18  ไม้อัด 18 มม.       Active
sheet · Cost 1,250.00 TEST_ONLY · Bangkok override

LAB-CARP    ค่าแรงช่างไม้        Active
day · Cost stale
```

Detail เรียง Summary → Capabilities → Base Unit → Current Cost → Future/History → Audit การเพิ่ม Cost/Import จำนวนมากไม่เป็น Primary Mobile Flow

เมื่อเปิด Barcode UI ให้แสดงรายการแบบอ่านได้ที่ 320px โดยเรียง `ค่ารหัส → หน่วย/จำนวน → ระดับบรรจุ → สถานะ/Action`; ช่องสแกนต้องใช้คีย์บอร์ดได้และแจ้ง Not Found/Inactive/Conflict ด้วยข้อความ `th/en`. ถ้าผู้ใช้มีสิทธิ์เพิ่ม Barcode ให้กรอกค่าเองหรือรับค่าจากสแกนเนอร์ก่อนบันทึก; การสร้าง GTIN จากเลข SKU ภายในอัตโนมัติไม่ได้รับอนุญาต

## Screen and Action Contract

| Screen/Panel | Primary Action | API Intent |
| --- | --- | --- |
| Item List | Search/filter/create | List/Create Item |
| Item Detail | Save/activate/deactivate | Patch/Transition Item |
| Item Barcode (planned) | Add/set primary/deactivate; exact scan lookup | Barcode child resource ตาม [API Contract](../03-contracts/item-master-api-contract.md) |
| Unit & Conversion | Add/disable conversion | Unit/Conversion Resource (Conversion เป็นงานระยะถัดไป) |
| Cost Timeline | Create/submit/review/publish | Cost Lifecycle Commands |
| Cost Resolver Preview | Test branch/date/quantity | Resolve Cost Read-only |
| Import Upload/Preview/Commit | งานระยะถัดไป ไม่แสดง Action ใน Slice นี้ | Import Batch Contract |

## UI States

| State | การแสดง | Action |
| --- | --- | --- |
| Empty | แนะนำสร้าง Item และข้อมูลอ้างอิงที่ต้องใช้ | Create Item/Reference Data |
| Draft | แสดง Missing Required Fields | แก้/Activate |
| Active | แสดง Capability และ Current Cost | ใช้งาน/แก้ Metadata |
| Inactive | Badge + เหตุผล | Reactivate เมื่อผ่าน Gate |
| Missing Cost | Warning ระดับ Item | Add Cost Draft |
| Stale Cost | อายุ/Source/Effective Date | Create Version/Approve Exception |
| Future Cost | วันที่เริ่มใช้ชัดเจน | Review/Supersede |
| Ambiguous Cost | Candidate list + Stable Code | แก้ Scope/Period/Priority |
| Saving/Conflict | Saved time หรือ Compare | Reload; ไม่ Last-write-wins |
| Import Invalid/Committing | งานระยะถัดไป | ไม่แสดงใน Slice นี้ |

## UX Contract สำหรับหน้าดูแล Estimate Master Data รอบแรก

ส่วนนี้กำหนดประสบการณ์ของผู้ดูแล Item, ผู้จัดทำต้นทุน และผู้ตรวจต้นทุนในรอบแรก โดยใช้ [Item Master Governance](item-master-governance.md) เป็นเจ้าของกฎธุรกิจ และใช้ [Design System](../../design.md) เป็นเจ้าของสี ระยะ และ Typography

### Information architecture และงานที่ผู้ใช้ทำ

| ตำแหน่ง | เนื้อหาหลัก | Primary action |
| --- | --- | --- |
| Item Master / Items | ค้นหา Item ตาม Code/ชื่อ, กรอง Type/Category/Status/Cost State, เห็นหน่วยและสถานะราคา | สร้าง Item |
| Item Master / Reference Data | Category, Brand, Unit, Tax Category และ Manual Cost Source แยกเป็นแท็บที่ค้นหาและดูสถานะได้ | เพิ่มข้อมูลอ้างอิงในแท็บปัจจุบัน |
| Item Detail | Summary, ชื่อสองภาษา, Category/Brand/Unit, Capability, Branch Availability, ภาพ, Cost Timeline และ Audit | บันทึก Draft หรือเปลี่ยนสถานะตามสิทธิ์ |
| Cost Review | รายการ Submitted ที่ผู้ใช้มีสิทธิ์ตรวจ พร้อม Item/Amount/Unit/Source/Effective Date/Maker | เปิดตรวจต้นทุนหนึ่งฉบับ |
| Estimate Catalog | Item ที่มี Published Cost ตาม Branch Context และเงื่อนไข Catalog | เลือกเข้า BOQ |

ใช้ List และ Detail เป็นพื้นที่ทำงานหลัก. บน Desktop ให้คงตาราง Item ทางซ้ายและ Inspector ทางขวาตาม Responsive Model; การแก้ Item หลายกลุ่มใช้หน้า `[id]` เดียวสำหรับ Create/Edit. ปุ่มสร้างเร็วข้าง Category, Brand และ Unit เปิด Modal เดียวตามชนิดข้อมูลและเลือกข้อมูลใหม่ให้ฟอร์ม Item หลังบันทึก; ปุ่มแสดงเฉพาะผู้มีสิทธิ์จัดการ Taxonomy. เมนูหลักแบ่งเป็น Workspace, Commercial, Master Data และ Settings พร้อมกลุ่มย่อยที่กรองตามสิทธิ์

### เส้นทางใช้งานที่ต้องจบครบ

1. ผู้ดูแลสร้าง Category/Brand/Unit และ Manual Cost Source ที่จำเป็นจาก Reference Data แล้วกลับมาสร้าง Item โดยไม่เสียข้อมูล Draft ที่กรอกไว้
2. หน้า Item Draft แสดง Required Gate ที่ขาดก่อน Activate; คลิกสถานะหรือคำเตือนแล้วพาไป Field/Tab ที่ต้องแก้. เมื่อ Activate สำเร็จ แสดง Code และสถานะชัดเจน
3. ผู้จัดทำเปิด Cost ของ Item, เห็นราคา Published ปัจจุบัน (ถ้ามี) กับ Draft/Future/History แยกกัน, ใส่ Amount/Unit/Effective Date/Source/เหตุผล/หลักฐาน แล้วส่งตรวจ. หน้าจอห้ามเรียก Draft ว่า “ราคาปัจจุบัน”
4. ผู้ตรวจเปิด Cost Review แล้วเห็น Maker, Last Financial Editor, หลักฐาน, รุ่น, ช่วงมีผล และผลกระทบต่อราคาเดิมก่อน Approve หรือ Return พร้อมเหตุผล. ถ้าเป็นคนเดียวกับ Maker ให้แสดงว่าอนุมัติไม่ได้และ Backend ต้องปฏิเสธด้วย
5. ผู้มีสิทธิ์ Publish เห็นผลลัพธ์ของ effective period และ overlap ก่อนยืนยัน. หลัง Publish ให้แสดงว่าราคาใช้ได้ตั้งแต่เมื่อใด และ Item ปรากฏใน Estimate Catalog ตาม Branch Context; เมื่อ Save/Reload BOQ ต้องเห็น Snapshot ที่เลือกเดิม

### Screen states และ feedback

- List ใช้ DataTable แบบคงโครงตารางไว้เมื่อ Loading/Empty/Error; Search/Filter/Sort/Page อยู่ใน URL. Empty แยก “ยังไม่มี Item” กับ “ไม่พบตามตัวกรอง” พร้อม Action ที่ตรงปัญหา; Error มี Retry
- แสดง Cost State เป็นข้อความและ Badge: `Missing`, `Draft`, `Pending review`, `Approved`, `Published current`, `Future`, `Expired`, `Stale`, `Ambiguous`, `Disabled` ตามข้อมูลจริง. ราคาไม่พร้อมต้องแสดง `-` และเหตุผล; ห้ามเดาราคาหรือใช้ค่า Draft แทน Published
- ฟอร์ม Edit ใช้ Minimal Mono Loading ในคอนเทนเนอร์. ปุ่ม Save/Submit/Approve/Publish/Disable แสดง Loading และปิดการกดซ้ำทันที. ผลสำเร็จแสดงสถานะใหม่และเวลา/ผู้กระทำเมื่อ Contract มีข้อมูล
- Validation ผูกข้อความกับ Field, สรุป Error และสลับไปแท็บแรกที่ผิดเมื่อ Submit ไม่ผ่าน; แท็บอื่นยังคลิกได้. ETag conflict ให้ข้อความว่าข้อมูลเปลี่ยนแล้วพร้อม Reload; ห้ามบันทึกทับเงียบ ๆ หรือทำให้ค่าที่ผู้ใช้เพิ่งกรอกหายโดยไม่เตือน
- การ Deactivate Item/Source และ Disable Cost ใช้ Confirmation Modal ที่บอกผลกระทบและบังคับเหตุผลตามกฎ. Return แสดงช่องเหตุผลชัดเจน; Publish แสดง Source, Amount, Unit และ Effective Date ในขั้นยืนยัน
- หลังเปลี่ยนสถานะหรือแก้ราคา ให้ refresh Item Detail, Cost Timeline, Review Queue และ Catalog เฉพาะข้อมูลที่เกี่ยวข้อง. หาก Catalog ปฏิเสธเพราะราคาหรือรุ่นเปลี่ยน ให้คงบริบท BOQ และเปิดทางเลือก Refresh/เลือกใหม่

### Responsive และ accessibility acceptance

- Desktop ≥1200px: Table + Inspector + Cost Timeline; Tablet landscape 55/45; Tablet portrait เป็น List + Side Sheet; Mobile 320–767px เป็น List → Detail แบบเต็มพื้นที่. ตัวเลข Amount, Unit, Effective Date และ Status ต้องอ่านได้โดยไม่เลื่อนแนวนอนใน Cost Detail
- ปิด Drawer/Side Sheet แล้ว Focus กลับไปยัง Item/Action ที่เปิด; Modal trap focus และ Escape ปิดได้เมื่อไม่มีคำสั่งค้าง. ทุก Action ใช้ได้ด้วย Keyboard และมี visible focus
- Touch target ≥44×44px, Mobile input ≥48px, Mobile body text ≥16px; รองรับ 320px, Zoom 200%, Screen Reader, Reduced Motion และข้อความไทย/อังกฤษ. สถานะ/ความเสี่ยงต้องมีข้อความหรือไอคอนประกอบ ไม่สื่อด้วยสีอย่างเดียว
- UI copy อยู่ใน `frontend/src/messages/th.json` และ `en.json`; ชื่อ Item ภาษาอังกฤษที่ไม่มีข้อมูลแสดงสถานะว่างตาม Contract ไม่คัดลอกชื่อไทยมาแทนโดยพลการ

## Item Editor และ Reference Data Forms

- หน้า `[id]` ใช้ร่วมกันสำหรับ Create/Edit และแบ่งเป็นแท็บข้อมูลสินค้า, รายละเอียดเสริม, การใช้งานและคุณสมบัติ โดยสลับได้อิสระและรักษา DOM ของช่องกรอก
- ข้อมูลสินค้าเริ่มจากชื่อ แล้วเลือกประเภท หมวดหมู่ แบรนด์ และหน่วย; แผงรหัสแยกด้านข้างบน Desktop และเรียงต่อบน Mobile เพื่อแสดงวิธีสร้างรหัสชัดเจน
- รายละเอียดเสริมรวมคำอธิบาย ชื่อเรียกอื่น และรูปภาพสำหรับการสร้างสินค้า; อัปโหลดรูปจริงตอนบันทึกเท่านั้น
- ใช้ FormActionBar กลางสำหรับบันทึก/ยกเลิกและสถานะข้อมูลยังไม่บันทึก; เมื่อ Validate ไม่ผ่านให้เปิดแท็บแรกที่มีข้อผิดพลาด
- ฟอร์มสร้าง/แก้ไข Category, Brand, Unit, Tax Category และ Cost Source เปิดเป็น Drawer จากตารางเดิม โดย Header/Footer คงที่และเลื่อนเฉพาะเนื้อหา
- ปิด Drawer ผ่านปุ่มยกเลิก, ปุ่มปิด, Overlay หรือ Escape ต้องยืนยันเมื่อมีข้อมูลยังไม่บันทึก และปิดไม่ได้ระหว่างบันทึก/เตรียมภาพ
- Drawer และ Confirmation Modal ใช้ keyboard focus containment กลางร่วมกัน; เมื่อเปิด Modal ยืนยันบน Drawer ให้เฉพาะแผงบนสุดจัดการ Escape/Tab และคืน Focus เมื่อปิด

## Accessibility

- Touch Target ≥44×44px, Mobile Input ≥48px และ Body Text Mobile ≥16px
- Filter/Tab/Table/Side Sheet ใช้ Semantic Role และ Visible Focus
- Status/Cost Risk ไม่ใช้สีอย่างเดียว
- รองรับ Keyboard, Screen Reader, Zoom 200%, 320px, ไทย/อังกฤษ และ Reduced Motion
- Import Error Summary Focus ไปแถว/Field แรกได้และ Error Report อ่านได้โดยไม่พึ่งสี

## Source Documents

- [Item Master Flow](item-master-flow.md)
- [Item Master Field Catalog](item-master-field-catalog.md)
- [Item Master Governance](item-master-governance.md)
- [Item Master API Contract](../03-contracts/item-master-api-contract.md)
- [Item Master Data Contract](../04-data/item-master-data-contract.md)
