# tan-erp Domain Language

คำศัพท์หลักของ Project ERP สำหรับธุรกิจออกแบบ ตกแต่งภายใน และผลิตงานบิวต์อิน ใช้คำต่อไปนี้ให้เหมือนกันในเอกสาร หน้าจอ API และการสนทนา

## องค์กรและบุคคล

**Organization (องค์กร)**:
นิติบุคคลหรือหน่วยธุรกิจสูงสุดที่เป็นเจ้าของข้อมูลในระบบ
_Avoid_: Tenant, Company Account

**Branch (สาขา)**:
หน่วยปฏิบัติงานภายใต้องค์กร ซึ่งอาจมีทีม เอกสาร และความรับผิดชอบของตนเอง
_Avoid_: Office, Shop

**Customer (ลูกค้า)**:
บุคคลหรือนิติบุคคลที่ขอรับบริการและเป็นเจ้าของสถานที่หรือโครงการ
_Avoid_: User, Account, Buyer

**User (ผู้ใช้งาน)**:
บุคคลที่เข้าสู่ระบบและปฏิบัติงานในนามองค์กร
_Avoid_: Customer, Employee Account

## การขายและการประเมินราคา

**Opportunity (โอกาสการขาย)**:
ความต้องการของลูกค้าที่อยู่ระหว่างการติดตามก่อนเกิดใบเสนอราคาหรือโครงการ
_Avoid_: Lead เมื่อผ่านการคัดกรองแล้ว, Project

**Site Survey (การสำรวจหน้างาน)**:
ชุดข้อมูลการวัดพื้นที่ ภาพถ่าย เงื่อนไขหน้างาน และความต้องการที่ยืนยันแล้ว
_Avoid_: Estimate, Inspection

**Estimate (ประมาณการ)**:
แบบคำนวณต้นทุน ค่าแรง ค่าใช้จ่าย และกำไรที่ใช้ภายในก่อนออกเอกสารขาย
_Avoid_: Quotation, Invoice, Budget

**Quick Estimate (ราคาประเมินเบื้องต้น)**:
ช่วงราคาที่คำนวณอย่างรวดเร็วจากข้อมูลหน้างาน Template และสมมติฐาน เพื่อช่วยสนทนากับลูกค้าโดยยังไม่ใช่ราคาที่อนุมัติหรือผูกพันทางการค้า
_Avoid_: Estimate, Quotation, Approved Price

**Pricing Template (แม่แบบประเมินราคา)**:
ชุดกฎของประเภทงานที่กำหนดข้อมูล หน่วย ราคาอ้างอิง ตัวคูณ ช่วงราคา และหลักฐานสำหรับสร้าง Quick Estimate
_Avoid_: Quotation Template, Free-form Formula

**Pricing Template Version (รุ่นแม่แบบประเมินราคา)**:
ฉบับที่มีผลตามช่วงเวลาและไม่แก้ย้อนหลังหลังเผยแพร่ เพื่อให้ Quick Estimate คำนวณซ้ำจากกฎเดิมได้
_Avoid_: Draft Template, Current Settings

**Reference Rate (ราคาอ้างอิง)**:
ราคาฐานที่มีแหล่งที่มาและช่วงเวลามีผล ใช้เป็นข้อมูลคำนวณ Quick Estimate แต่ไม่ใช่ต้นทุนจริงของ Official Estimate
_Avoid_: Actual Cost, Approved Selling Price

**Billable Quantity (ปริมาณคิดราคา)**:
ปริมาณที่ได้หลังใช้ Measurement Rule ของ Pricing Template กับข้อมูลหน้างาน และนำไปคูณ Reference Rate
_Avoid_: Raw Measurement, Estimate Quantity

**Official Estimate (ประมาณการทางการ)**:
Estimate ที่มีรายการต้นทุนและกฎคำนวณครบตามเกณฑ์ พร้อมเข้าสู่การตรวจทานและอนุมัติก่อนออก Quotation
_Avoid_: Quick Estimate, Quotation

**Estimate Revision (ฉบับแก้ไขประมาณการ)**:
สำเนาที่มีหมายเลขฉบับและคงประวัติการเปลี่ยนแปลงของ Estimate
_Avoid_: Overwrite, Draft Copy

**Calculation Policy (นโยบายคำนวณ)**:
ชุดกฎที่มีรุ่นและช่วงเวลามีผลสำหรับคำนวณต้นทุน ราคาขาย ส่วนลด ภาษี และการปัดเศษของ Official Estimate
_Avoid_: Formula Setting, Pricing Template

**Calculation Snapshot (ภาพบันทึกการคำนวณ)**:
หลักฐานที่ตรึงข้อมูลนำเข้า รุ่นกฎ ขั้นตอนคำนวณ และผลลัพธ์ของ Official Estimate ณ การคำนวณหนึ่งครั้ง
_Avoid_: Current Total, Cache

**Approval Policy (นโยบายอนุมัติ)**:
ชุดกฎที่มีรุ่นและขอบเขตสำหรับตัดสินว่าประมาณการถูกบล็อก ต้องผ่านใครบ้าง หรือพร้อมให้ผู้มีอำนาจอนุมัติ
_Avoid_: Permission, Role

**Approval Route (เส้นทางอนุมัติ)**:
ลำดับผู้ตรวจและเงื่อนไขที่ถูกตรึงให้กับ Estimate Revision เมื่อส่งอนุมัติ
_Avoid_: Role List, Permission Set

**Provisional Cost (ต้นทุนชั่วคราว)**:
ต้นทุนที่ยังไม่มีแหล่งราคาอ้างอิงสมบูรณ์และใช้ได้ชั่วคราวเมื่อมีเหตุผลพร้อมการตรวจอนุมัติ
_Avoid_: Actual Cost, Standard Cost

**Quotation (ใบเสนอราคา)**:
ข้อเสนอเชิงพาณิชย์ที่ออกให้ลูกค้า โดยอ้างอิง Estimate ฉบับที่อนุมัติ
_Avoid_: Estimate, Invoice

## โครงการและสินค้า

**Project (โครงการ)**:
งานที่ลูกค้ายืนยันแล้วและต้องบริหารขอบเขต งบ เวลา การผลิต การติดตั้ง และส่งมอบ
_Avoid_: Opportunity, Job เมื่อหมายถึงโครงการทั้งหมด

**Item Master (ข้อมูลหลักสินค้าและบริการ)**:
รายการมาตรฐานที่นำกลับมาใช้ในประมาณการ จัดซื้อ คลัง หรือการผลิต พร้อมหน่วยและสถานะใช้งาน
_Avoid_: Estimate Line, Free-text Item

**Work Item (รายการงาน)**:
งานหรือผลส่งมอบหนึ่งรายการภายใน Estimate หรือ Project ซึ่งอาจประกอบด้วยวัสดุและแรงงาน
_Avoid_: Item Master, Task

**MRP (การวางแผนความต้องการวัสดุ)**:
โมดูลอนาคตที่คำนวณความต้องการวัสดุจากแผนผลิต คงคลัง และระยะเวลาจัดหา ไม่ใช่ชื่อของระบบ
_Avoid_: ERP

## การกำกับดูแล

**Role (บทบาท)**:
ชุดสิทธิ์ที่มอบให้ผู้ใช้ตามหน้าที่ เช่น ผู้ประเมินราคา หรือผู้อนุมัติ
_Avoid_: Permission, Job Title

**Permission (สิทธิ์)**:
ความสามารถเฉพาะในการทำสิ่งหนึ่งกับทรัพยากรหนึ่ง เช่น `estimates.approve`
_Avoid_: Role, Scope

**Scope (ขอบเขตสิทธิ์)**:
ขอบเขตข้อมูลที่ Permission ใช้ได้ ได้แก่ Organization, Branch, Project หรือ Own
_Avoid_: Permission

**Maker–Checker (ผู้จัดทำ–ผู้ตรวจอนุมัติ)**:
หลักควบคุมที่แยกผู้สร้างหรือแก้ไขรายการออกจากผู้อนุมัติรายการสำคัญ
_Avoid_: Self Approval
