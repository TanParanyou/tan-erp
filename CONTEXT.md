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

**Site (สถานที่หน้างาน)**:
สถานที่จริงของลูกค้าที่ใช้สำรวจ ออกแบบ ติดตั้ง หรือให้บริการ และอาจถูกใช้กับหลายกิจกรรม
_Avoid_: Customer Address, Site Survey, Project

**Site Survey (การสำรวจหน้างาน)**:
กิจกรรมและชุด Revision สำหรับบันทึกการวัดพื้นที่ ภาพถ่าย เงื่อนไข และความต้องการ ณ Site หนึ่งแห่ง
_Avoid_: Estimate, Inspection

**Site Survey Revision (ฉบับข้อมูลสำรวจหน้างาน)**:
ฉบับข้อมูลสำรวจที่แก้ได้ระหว่าง Draft และถูกล็อกเมื่อ Ready เพื่อให้ Estimate อ้างหลักฐานชุดเดิมได้
_Avoid_: Current Survey Data, Estimate Revision

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

**Item Capability (ความสามารถของรายการ)**:
ขอบเขตการนำ Item ไปใช้ เช่น ขาย คิดต้นทุน จัดซื้อ เก็บคลัง หรือผลิต โดยไม่เปลี่ยนความหมายของประเภท Item
_Avoid_: Item Type, Permission

**Item Category (หมวดรายการ)**:
โครงสร้างจัดกลุ่ม Item แบบลำดับชั้น โดย Category ลูกใช้ Parent Category เดียวกันแทนการสร้างชนิด Subcategory แยก
_Avoid_: Tag, Separate Subcategory Entity

**Item Brand (ยี่ห้อรายการ)**:
ข้อมูลหลักระดับองค์กรสำหรับระบุยี่ห้อของ Item โดย Item ที่ไม่มีแนวคิดเรื่องยี่ห้อสามารถเว้นว่างได้
_Avoid_: Supplier, Manufacturer Contract

**Item Alias (ชื่อเรียกอื่นของรายการ)**:
คำค้นหลายภาษาที่ชี้กลับไปยัง Item เดิมโดยไม่แทน Canonical Item Name และไม่สร้าง Item ใหม่
_Avoid_: Duplicate Item, Display Name, Item Code

**Item Branch Availability (การเปิดใช้รายการในสาขา)**:
กติกาที่ระบุว่า Item ระดับองค์กรใช้ได้ทุกสาขาหรือเฉพาะสาขาที่เลือก โดยไม่สร้าง Item ซ้ำและไม่ใช้แทนราคาประจำสาขา
_Avoid_: Branch Item, Branch Cost, Item Copy

**Item Image (ภาพประกอบรายการ)**:
ความสัมพันธ์ระหว่าง Item กับ Verified File สำหรับภาพหลัก ภาพเพิ่มเติม หรือภาพข้อมูลทางเทคนิค โดยไฟล์ยังอยู่ภายใต้ File Service
_Avoid_: Image URL, Embedded Image, Public File

**Unit of Measure (หน่วยนับ)**:
หน่วยมาตรฐานที่ใช้บอกปริมาณของ Item, Work Item หรือต้นทุน
_Avoid_: Package Size, Conversion Factor

**Unit Conversion (การแปลงหน่วย)**:
ความสัมพันธ์ที่ยืนยันแล้วสำหรับแปลงปริมาณระหว่างสองหน่วย โดยอาจใช้ร่วมกันหรือเฉพาะ Item
_Avoid_: Estimate Formula, Assumed Factor

**Cost Source (แหล่งต้นทุน)**:
หลักฐานหรือที่มาของต้นทุน เช่น ใบเสนอราคาผู้ขาย รายการราคา สัญญา หรือข้อมูลที่บันทึกด้วยเหตุผล
_Avoid_: Cost Record, Reference Rate

**Cost Record (รายการต้นทุนอ้างอิง)**:
ต้นทุนภายในที่มีหน่วย สกุลเงิน ขอบเขต และช่วงเวลามีผลสำหรับใช้สร้าง Official Estimate
_Avoid_: Actual Purchase Cost, Reference Rate, Selling Price

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

## การจัดการเอกสารและเลขที่เอกสาร

**Document Number (เลขที่เอกสาร)**:
รหัสที่มนุษย์อ่านเข้าใจได้และไม่ซ้ำกันสำหรับระบุเอกสารทางธุรกิจ เช่น ใบประเมินราคา ใบสำรวจ หรือใบเสนอราคา
_Avoid_: Document ID, UUID, Primary Key

**Document Sequence Definition (ข้อกำหนดรูปแบบเลขที่เอกสาร)**:
กฎและแม่แบบ (Format Pattern) ที่กำหนดโครงสร้างของเลขที่เอกสารแต่ละประเภท พร้อมรอบการเริ่มนับใหม่
_Avoid_: Numbering Setting, Prefix Config

**Format Pattern (แม่แบบเลขที่เอกสาร)**:
ข้อความระบุโครงสร้างรหัสเอกสารที่ประกอบด้วยข้อความคงที่และโทเคน เช่น `{PREFIX}-{BRANCH}-{BB}{MM}-{SEQ:4}`
_Avoid_: Regex, Custom Code

**Sequence Counter (ตัวนับลำดับเลขที่เอกสาร)**:
ตัวนับค่าล่าสุดในฐานข้อมูลที่เพิ่มขึ้นแบบ Atomic เพื่อรับประกันความต่อเนื่องและป้องกันเลขซ้ำ
_Avoid_: Row Count, Max ID

**Period Key (รหัสรอบเวลา)**:
ข้อความระบุช่วงเวลาสำหรับแยกชุดการนับเลขตามรอบการรีเซ็ต เช่น `2569` (รายปี), `256909` (รายเดือน) หรือ `ALL` (ไม่รีเซ็ต)
_Avoid_: Date String, Timestamp

**Two-tier Allocation (การจัดสรรเลขสองระดับ)**:
นโยบายแยกเลขเอกสารระหว่างฉบับร่าง (Draft) กับฉบับทางการ (Official) เพื่อรับประกันความต่อเนื่องของเลขทางการ (Gapless)
_Avoid_: Single Running Number

## การจัดการไฟล์และภาพถ่าย (Files & Media Assets)

**File Upload Session (รอบการอัปโหลดไฟล์)**:
วงจรเตรียมอัปโหลดไฟล์ที่มีอายุจำกัด ผูกติดกับ Parent Entity (เช่น Opportunity, Customer, Site) เพื่อยืนยันขอบเขตสิทธิ์ ป้องกันการอัปโหลดไฟล์ลอย และป้องกันการนำไฟล์ไปใช้ข้ามเอกสารหรือข้ามองค์กร
_Avoid_: Direct Upload, Free Upload, Temp Upload

**File Upload Slot (ช่องรับไฟล์)**:
ข้อกำหนดช่องรับไฟล์แต่ละรายการภายใน Upload Session ซึ่งระบุชื่อไฟล์ ขนาดไฟล์ และ MIME Type ที่ผ่านการตรวจสอบความถูกต้องก่อนรับข้อมูล
_Avoid_: Form Field, Multi-part Param

**Verified File (ไฟล์ที่ผ่านการตรวจสอบแล้ว)**:
ไฟล์ที่ถูกอัปโหลด ตรวจสอบความถูกต้องของ Magic Numbers และ Header แล้ว บันทึกในระบบจัดเก็บไฟล์พร้อมระบุเจ้าขององค์กร และพร้อมสำหรับนำไปผูกติดกับ Parent Entity
_Avoid_: Unchecked File, Raw File

**File Parent Invariant (เงื่อนไขความสัมพันธ์ไฟล์กับข้อมูลหลัก)**:
ข้อกำหนดความปลอดภัยระดับสถาปัตยกรรมที่รับประกันว่าไฟล์ที่อัปโหลดต้องถูกผูกและใช้งานได้เฉพาะกับ Parent Entity (และ Parent Type) ที่ประกาศไว้ใน Upload Session เท่านั้น ห้ามนำ `fileId` ข้าม Tenant หรือข้าม Parent มาผูกเด็ดขาด
_Avoid_: Loose File Reference, Global File Pool

**Work Images (ภาพถ่ายหน้างานตามขั้นตอน)**:
ภาพถ่ายจริงที่แนบไว้กับ Opportunity เพื่อบันทึกสภาพหน้างานหรือหลักฐานประกอบในแต่ละขั้นตอนการขาย (Stage) โดยอ้างอิงไฟล์ที่ Verified แล้ว และรองรับการดึงข้อมูลแบบ Keyset Cursor Pagination
_Avoid_: Site Survey Image เมื่อหมายถึงภาพประกอบ Opportunity ทั่วไป
