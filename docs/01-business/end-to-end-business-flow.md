# End-to-End Business Flow (กระบวนการธุรกิจตั้งแต่ต้นจนจบ)

**สถานะ:** Draft — ต้องยืนยันกับเจ้าของกระบวนการ

## ภาพรวม

```text
Customer → Opportunity → Site Survey → Estimate → Approval
         → Quotation → Customer Acceptance → Project
         → Procurement / Production / Installation
         → Handover → Warranty / After-sales
```

## ขั้นตอน

| ลำดับ | ขั้นตอน | ผลลัพธ์ที่ต้องมี | ผู้รับผิดชอบตัวอย่าง |
| ---: | --- | --- | --- |
| 1 | รับลูกค้าและความต้องการ | Customer และ Opportunity | Sales |
| 2 | นัดและสำรวจหน้างาน | Site Survey พร้อมรูปและขนาด | Surveyor/Designer |
| 3 | ออกแบบขอบเขตเบื้องต้น | Work Items และสมมติฐาน | Designer |
| 4 | ประเมินราคา | Estimate Revision | Estimator |
| 5 | ตรวจและอนุมัติ | Approval Decision | Approver |
| 6 | ออกใบเสนอราคา | Quotation | Sales |
| 7 | ลูกค้ายืนยันหรือขอแก้ไข | Acceptance หรือ Revision ใหม่ | Sales/Customer |
| 8 | เปิดโครงการ | Project และ Baseline Budget | Project Manager |
| 9 | จัดซื้อ ผลิต และติดตั้ง | PO, Production/Installation Progress | Operations |
| 10 | ส่งมอบและรับประกัน | Handover และ Warranty Record | Project/Service Team |

## กฎการส่งต่อ

- Quotation ต้องอ้างอิง Estimate Revision ที่อนุมัติ
- การแก้ราคาหลังอนุมัติต้องสร้าง Revision ใหม่
- Project ต้องรับข้อมูลจากเอกสารที่ลูกค้ายืนยัน ไม่คีย์ใหม่โดยไม่มีเหตุผล
- เอกสารและเหตุการณ์สำคัญต้องมีผู้ทำ เวลา และเหตุผล

## คำถามสำหรับ Workshop

- Opportunity เกิดจากช่องทางใดบ้าง และใครเป็นเจ้าของรายแรก
- Site Survey ต้องมีข้อมูลใดจึงถือว่า “พร้อมประเมิน”
- ใครอนุมัติส่วนลดหรือกำไรต่ำกว่าเกณฑ์
- การยืนยันของลูกค้าใช้ลายเซ็น เอกสาร หรือการรับเงินมัดจำ
- จุดใดถือว่าเริ่ม Project และล็อก Baseline Budget
