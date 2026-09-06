# End-to-End Business Flow (กระบวนการธุรกิจตั้งแต่ต้นจนจบ)

**สถานะ:** Draft — ต้องยืนยันกับเจ้าของกระบวนการ

## ภาพรวม

```text
Customer → Opportunity → Site → Ready Site Survey Revision
    ├─ Quick Estimate → Preliminary Summary → Follow-up
    │                                      └─ Convert
    └───────────────────────────────────────────► Official Estimate
                                                   ↓
                                      Approval → Quotation → Project
                                                   ↓
                           Procurement / Production / Installation
                                                   ↓
                                    Handover → Warranty / After-sales
```

[Quick Estimate](quick-estimate-flow.md) เป็นทางลัดเสริมสำหรับแจ้งช่วงราคาหน้างาน ไม่ใช่ขั้นตอนบังคับและไม่ใช่ Quotation ทุกงานยังต้องมี [Official Estimate](estimation-flow.md) ที่ผ่านการตรวจและอนุมัติก่อนออก Quotation ข้อมูลต้นทางอ้าง [CRM and Site Survey Flow](crm-site-survey-flow.md)

## ขั้นตอน

| ลำดับ | ขั้นตอน | ผลลัพธ์ที่ต้องมี | ผู้รับผิดชอบตัวอย่าง |
| ---: | --- | --- | --- |
| 1 | รับลูกค้าและความต้องการ | Customer และ Opportunity | Sales |
| 2 | นัดและสำรวจหน้างาน | Ready Site Survey Revision พร้อม Measurement/Evidence | Surveyor/Designer |
| 3 | ออกแบบขอบเขตเบื้องต้น | Work Items และสมมติฐาน | Designer |
| 4 | ประเมินราคาด่วน (ถ้าต้องใช้) | Quick Estimate และ Preliminary Summary | Field Estimator |
| 5 | จัดทำประมาณการทางการ | Official Estimate Revision | Estimator |
| 6 | ตรวจและอนุมัติ | Approval Decision | Approver |
| 7 | ออกใบเสนอราคา | Quotation | Sales |
| 8 | ลูกค้ายืนยันหรือขอแก้ไข | Acceptance หรือ Revision ใหม่ | Sales/Customer |
| 9 | เปิดโครงการ | Project และ Baseline Budget | Project Manager |
| 10 | จัดซื้อ ผลิต และติดตั้ง | PO, Production/Installation Progress | Operations |
| 11 | ส่งมอบและรับประกัน | Handover และ Warranty Record | Project/Service Team |

## กฎการส่งต่อ

- Quotation ต้องอ้างอิง Estimate Revision ที่อนุมัติ
- Official Estimate ที่ใช้ Survey ต้องอ้าง Ready Site Survey Revision ID/Hash ที่แน่นอน
- Quick Estimate ที่ Convert ต้องสร้าง Official Estimate Draft ใหม่จาก Snapshot; ห้ามออก Quotation โดยอ้าง Quick Estimate โดยตรง
- การแก้ราคาหลังอนุมัติต้องสร้าง Revision ใหม่
- Project ต้องรับข้อมูลจากเอกสารที่ลูกค้ายืนยัน ไม่คีย์ใหม่โดยไม่มีเหตุผล
- เอกสารและเหตุการณ์สำคัญต้องมีผู้ทำ เวลา และเหตุผล

## คำถามสำหรับ Workshop

- Opportunity เกิดจากช่องทางใดบ้าง และใครเป็นเจ้าของรายแรก
- Site Survey ต้องมีข้อมูลใดจึงถือว่า “พร้อมประเมิน”
- ใครอนุมัติส่วนลดหรือกำไรต่ำกว่าเกณฑ์
- การยืนยันของลูกค้าใช้ลายเซ็น เอกสาร หรือการรับเงินมัดจำ
- จุดใดถือว่าเริ่ม Project และล็อก Baseline Budget
