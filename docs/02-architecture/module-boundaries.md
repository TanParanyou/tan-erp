# Module Boundaries (ขอบเขตโมดูล)

**สถานะ:** Accepted สำหรับ Boundary; ลำดับเปิดใช้ตาม Roadmap

| Module | เป็นเจ้าของ | รับข้อมูลจาก | ส่งต่อให้ |
| --- | --- | --- | --- |
| Identity & Access | User Mapping, Role, Permission, Scope | Firebase Identity | ทุก Module |
| Organization | Organization, Branch, Membership | Administration | ทุก Module |
| CRM | Customer, Contact, Address, Site, Opportunity และ Stage History | Sales | Site Survey, Estimation, Project |
| Item Master | Item, Category, Capability, Unit/Conversion, Cost Source และ Versioned Cost Record | Item/Data Steward/Cost Owner | Estimation, Procurement, Inventory |
| Site Survey | Survey Identity/Revision, Area, Measurement, Checklist และ Evidence Reference | CRM/Sales/Surveyor | Estimation |
| Estimation | Estimate, Revision, Calculation/Approval Snapshot | CRM, Site Survey, Item Master | Quotation, Project |
| Commercial | Quotation, Customer Acceptance | Estimation | Project, Finance |
| Project Control | Project, Budget, Change Order, Progress | Commercial | Procurement, Production, Installation |
| Procurement | Supplier, Purchase Request, PO, Receipt | Project, Item Master | Inventory, Finance |
| Inventory | Stock, Reservation, Movement | Procurement | Production, Project |
| Production & MRP | BOM, Work Order, Material Plan | Project, Inventory | Installation |
| Installation & Service | Installation, Handover, Warranty | Project/Production | Customer Service |
| Finance Integration | Billing/Payment Reference | Commercial, Procurement | Accounting System |

## Boundary Rules

- Module อ้างอิงข้อมูลของอีก Module ด้วย ID หรือ Public Contract
- Module เจ้าของข้อมูลเป็นผู้บังคับ Invariant และ Lifecycle
- รายงานข้าม Module ใช้ Read Model/Query ที่กำหนดชัด ไม่ย้าย Ownership
- Shared Kernel จำกัดเฉพาะชนิดพื้นฐาน เช่น ID, Money และ Audit metadata
- ห้ามสร้างตารางกลางที่ทุก Module แก้ไขได้โดยไม่มีเจ้าของ
