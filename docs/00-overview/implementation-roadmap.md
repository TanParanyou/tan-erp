# Implementation Roadmap (แผนพัฒนา)

**สถานะ:** Accepted เป็นลำดับการพัฒนา

| ระยะ | เป้าหมาย | ผลส่งมอบหลัก | Gate ก่อนผ่าน |
| --- | --- | --- | --- |
| 0 Documentation | ทำความเข้าใจตรงกัน | Glossary, Flow, Architecture, Contracts, Portal | เจ้าของงานยืนยัน Flow และคำศัพท์ |
| 1 Foundation | ระบบปลอดภัยและวางฐานถูก | Identity, Organization, RBAC, Audit, Localization | Test สิทธิ์และข้ามองค์กรผ่าน |
| 2 Estimation MVP | ประเมินราคาได้ครบวงจร | Customer, Survey, Item, Estimate, Revision, Approval | UAT เคสจริงและเทียบผลคำนวณ |
| 3 Commercial | ส่งข้อเสนอและติดตามผล | Quotation, Acceptance, Project Handover | เอกสารขายย้อนกลับ Estimate ได้ |
| 4 Project Control | คุมแผนและงบ | Project, Budget, Change Order, Progress | ต้นทุนและสถานะตรวจย้อนหลังได้ |
| 5 Supply & Production | จัดซื้อ คลัง ผลิต | Supplier, PO, Inventory, BOM, Production, MRP | Master Data และ Stock Accuracy พร้อม |
| 6 Finance & Service | ปิดวงจรธุรกิจ | Billing Integration, Payment Status, Warranty | Reconciliation และ Support Runbook ผ่าน |

ทุกระยะต้องส่งมอบเป็น Vertical Slice ที่ผู้ใช้ทดลองได้ ไม่เปิดหลายโมดูลพร้อมกันโดยยังไม่มี Flow ใดจบครบวงจร
