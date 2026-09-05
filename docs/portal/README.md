# tan-erp Documentation Portal

Portal นี้เป็นหน้า HTML สำหรับอ่าน Flow จาก JSON ไม่มี Framework, Package Dependency, Analytics หรือการเชื่อม Internet และไม่ใช่ Frontend ของ ERP

Flow ที่มีใน Catalog ได้แก่ ภาพรวมธุรกิจ, Quick Estimate (ราคาประเมินเบื้องต้นหน้างาน), Official Estimation, Architecture, Error และ RBAC โดย Markdown เป็นแหล่งอ้างอิงหลัก ส่วน JSON ใช้จัดแสดงภาพและลำดับอ่าน

Template/Field ของ Node `work-template` และ `measurement` อ้างอิง [Quick Estimate Template Catalog](../01-business/quick-estimate-template-catalog.md), สูตรของ `price-range` อ้างอิง [Quick Estimate Pricing Rules](../01-business/quick-estimate-pricing-rules.md) และการตัดสินก่อนแชร์อ้างอิง [Approval Matrix](../01-business/approval-matrix.md)

## วิธีเปิด

Browser ส่วนใหญ่ไม่อนุญาตให้หน้า `file://` โหลด JSON ด้วย `fetch` จึงต้องเปิดผ่าน Local HTTP Server

จาก Root ของโปรเจกต์ รัน:

```bash
python3 -m http.server 8080 --bind 127.0.0.1 --directory docs/portal
```

จากนั้นเปิด `http://localhost:8080`

## วิธีแก้ Flow เดิม

1. เปิดไฟล์ใน `data/` เช่น `quick-estimate-flow.json` หรือ `estimation-flow.json`
2. เพิ่มหรือแก้ `nodes` และ `edges`
3. ให้ `node.id` ไม่ซ้ำ และ `edge.from/to` ตรงกับ ID ที่มีอยู่
4. ใส่ข้อความทั้ง `th` และ `en`
5. ตรวจ JSON และเปิด Portal ทดสอบภาษา ตัวกรอง และรายละเอียด

ตัวอย่าง Node:

```json
{
  "id": "cost-review",
  "group": "governance",
  "order": 8,
  "title": { "th": "ตรวจต้นทุน", "en": "Review Cost" },
  "summary": { "th": "ตรวจราคาและวันที่มีผล", "en": "Verify price and effective date." },
  "icon": "estimate",
  "phase": "phase-2",
  "status": "draft",
  "document": "../04-data/master-data.md"
}
```

ตัวอย่าง Quick Estimate: Node `share-policy` เชื่อมตรงไป `preliminary-summary` เมื่อผ่าน Policy หรือแยกไป `internal-review` เมื่อเข้าเกณฑ์ตรวจภายใน ข้อความบน Edge ต้องอธิบายเงื่อนไขได้โดยไม่อาศัยสี

## วิธีเพิ่ม Flow ใหม่

1. Copy Flow JSON ที่ใกล้เคียงแล้วเปลี่ยน `id`, metadata, groups, nodes และ edges
2. เพิ่มรายการใน `data/portal.json`
3. ตรวจรูปแบบกับ `schema/flow.schema.json`
4. เพิ่มเอกสาร Markdown หลักและตั้ง `document` ให้ถูก
5. ทดสอบ Desktop, Mobile, Keyboard, ไทย/อังกฤษ และ Print Preview

## กติกา Version

- เพิ่มข้อมูลแบบ Optional และ Portal เก่ายังอ่านได้: คง `version: 1`
- เปลี่ยนชื่อ/ความหมาย Field หรือทำให้ไฟล์เก่าอ่านไม่ได้: เพิ่ม Major Version, สร้าง Schema ใหม่ และเขียน Migration Guide
- JSON ไม่รองรับ Comment ให้เขียนคำอธิบายใน README หรือ Schema `description`

## โครงสร้าง

```text
portal/
├── index.html       # โครงหน้าและ Accessibility landmarks
├── styles.css       # Visual system, responsive และ print
├── app.js           # โหลด JSON, ภาษา, filter และ dialog
├── data/            # Catalog และ Flow content
├── schema/          # JSON Schema
└── assets/          # Local SVG illustrations
```
