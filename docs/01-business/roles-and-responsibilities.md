# Roles and Responsibilities (บทบาทและหน้าที่)

**สถานะ:** Draft — ชื่อบทบาทจริงต้องยืนยัน

| บทบาท | หน้าที่หลัก | สิ่งที่ไม่ควรทำคนเดียว |
| --- | --- | --- |
| Sales | ดูแล Customer, Contact, Opportunity, Site และ Quotation | อ่านข้อมูลข้าม Scope หรือปิดงานโดยไม่มีเหตุผล |
| Designer | จัด Scope และข้อมูลออกแบบ | เปลี่ยนราคาที่อนุมัติแล้วโดยไม่สร้าง Revision |
| Surveyor | เก็บข้อมูล Site Survey | ประกาศว่าราคาพร้อมเสนอโดยไม่มี Estimate |
| Field Estimator | สร้าง คำนวณ และแชร์ Quick Estimate ภายใน Permission, Scope และ Share Policy | แชร์รายการที่ต้อง Review หรือออก Quotation จาก Quick Estimate โดยตรง |
| Pricing Template Owner | จัด Measurement Rule, Reference Rate, Factor, Checklist และ Test Case | อนุมัติหรือ Publish Version ที่ตนแก้เมื่อ Maker–Checker มีผล |
| Pricing Template Approver | ตรวจสูตร Unit, Rate Source, Range, Test Case และผล Calibration | แก้ Template แทน Owner แล้วอนุมัติเอง |
| Item Master Owner | ดูแล Item, Category, Capability และ Lifecycle | เปลี่ยน Business Key หลัง Active หรือลบรายการที่เคยใช้ |
| Data Steward | ดูแลคุณภาพ Code, Unit, Conversion และ Import | Commit Batch ที่ผิดหรือสร้าง Conversion ที่ตรวจสอบไม่ได้ |
| Cost Owner | สร้าง Cost Source/Cost Record และหลักฐาน | อนุมัติ Cost Version ที่ตนสร้างหรือแก้ล่าสุด |
| Cost Approver | ตรวจ Source, Amount, Scope, Effective Period และ Exception | แก้ Financial Field แล้วอนุมัติ Version เดียวกัน |
| Estimator | จัดทำ Estimate และตรวจต้นทุน | อนุมัติ Estimate ของตนเองเมื่อ Maker–Checker บังคับ |
| Approver | ตรวจราคา กำไร ส่วนลด และความเสี่ยงภายใน Permission, Scope และ Approval Authority | แก้ข้อมูลแทนผู้จัดทำหรืออนุมัติงานตนเอง |
| Project Manager | รับมอบขอบเขตและบริหาร Project | เปลี่ยน Baseline โดยไม่มี Change Order |
| Procurement | จัดหาและออกคำสั่งซื้อ | อนุมัติ Supplier/PO ของตนเองเกินวงเงิน |
| Administrator | จัด User, Role และ Scope | อ่านข้อมูลธุรกิจนอก Scope โดยอัตโนมัติ |
| Auditor | อ่าน Audit Trail และรายงานควบคุม | แก้ Business Record |

Role เป็นชุด Permission ไม่ควรผูก Business Logic กับชื่อ Role โดยตรง เพราะองค์กรอาจตั้งชื่อหรือจัดหน้าที่ใหม่ในอนาคต

Approval Policy ต้อง Resolve ผู้อนุมัติจาก Permission + Scope + Authority ไม่ใช้ชื่อ Role เป็นเงื่อนไข Hard-code และ Production Bootstrap ต้องมี Independent Checker อย่างน้อยหนึ่งคน

Workflow และข้อห้ามของ Pricing Template Owner/Approver อยู่ที่ [Pricing Template Governance](pricing-template-governance.md)

Workflow ของ Item/Data Steward/Cost Owner/Approver อยู่ที่ [Item Master Governance](item-master-governance.md)

Lifecycle และข้อควบคุมของ Sales/Surveyor อยู่ที่ [CRM and Site Survey Governance](crm-site-survey-governance.md)
