# Item Master Responsive Wireframe (โครงหน้าจอข้อมูลสินค้าและต้นทุน)

**สถานะ:** Accepted Direction — UX Baseline สำหรับ Desktop, Tablet และ Mobile

## เป้าหมาย

ให้ผู้ดูแลค้นหา สร้าง แก้ และตรวจ Item/Unit/Cost ได้โดยเห็นสถานะ ความเสี่ยง และผลกระทบชัดเจน โดย Bulk Import เป็นงาน Desktop/Tablet และ Mobile เน้นค้นหา/แก้ทีละรายการ

เอกสารนี้ใช้ Text Wireframe เพื่อคงขอบเขต Documentation-only; ตัวเลขทั้งหมดเป็น `TEST_ONLY`

## Responsive Model

| Context | Layout | งานหลัก |
| --- | --- | --- |
| Desktop ≥1200px | Item Table + Detail Inspector + Cost Timeline | จัดการหลายรายการและ Import |
| Tablet 768–1199px | Table/Card + Side Sheet | ตรวจ/แก้ทีละ Item และ Cost |
| Mobile 320–767px | Search/List → Detail | ค้นหา ดู Current Cost และแก้ Field สำคัญ |

## Desktop

```text
Item Master                    [+ Item] [Import]
[Search] [Type] [Category] [Capability] [Status] [Cost State]
┌──────────────────────────────┬─────────────────────────────┐
│ Code / Name / Type / Unit    │ Item Inspector              │
│ Capability / Cost / Status   │ General | Units | Costs     │
│ MAT-PLY-18 ...               │ Current + Future + History  │
└──────────────────────────────┴─────────────────────────────┘
```

- Table ตรึง Code/Name และรองรับ Keyboard Row Navigation
- Inspector แสดง General, Capability, Unit/Conversion, Cost และ Audit เป็น Section ไม่ซ้อน Modal
- Cost Timeline แยก Current/Future/Expired/Disabled และติด Branch Override ชัดเจน
- Import เปิด Preview Table ที่กรอง Error และ Download Error Report ได้

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

## Screen and Action Contract

| Screen/Panel | Primary Action | API Intent |
| --- | --- | --- |
| Item List | Search/filter/create | List/Create Item |
| Item Detail | Save/activate/deactivate | Patch/Transition Item |
| Unit & Conversion | Add/disable conversion | Unit/Conversion Resource |
| Cost Timeline | Create/submit/review/publish | Cost Lifecycle Commands |
| Cost Resolver Preview | Test branch/date/quantity | Resolve Cost Read-only |
| Import Upload | Upload template file | Create Import Batch |
| Import Preview | Filter/fix/download errors | Read Validation Result |
| Import Commit | Confirm atomic changes | Commit Import Batch |

## UI States

| State | การแสดง | Action |
| --- | --- | --- |
| Empty | แนะนำสร้าง Item หรือ Download Template | Create/Import |
| Draft | แสดง Missing Required Fields | แก้/Activate |
| Active | แสดง Capability และ Current Cost | ใช้งาน/แก้ Metadata |
| Inactive | Badge + เหตุผล | Reactivate เมื่อผ่าน Gate |
| Missing Cost | Warning ระดับ Item | Add Cost Draft |
| Stale Cost | อายุ/Source/Effective Date | Create Version/Approve Exception |
| Future Cost | วันที่เริ่มใช้ชัดเจน | Review/Supersede |
| Ambiguous Cost | Candidate list + Stable Code | แก้ Scope/Period/Priority |
| Saving/Conflict | Saved time หรือ Compare | Reload; ไม่ Last-write-wins |
| Import Invalid | Summary + Row/Field Errors | Download/Upload corrected file |
| Import Committing | Progress + Idempotent retry | ห้าม Submit ซ้ำด้วย Key ใหม่ |

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
