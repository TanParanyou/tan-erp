# Permission Catalog (รายการสิทธิ์)

**สถานะ:** Draft Baseline — ชื่อ Permission เป็น Contract ที่ต้อง Review ก่อนเริ่มโค้ด

| Module | Permission | ความหมาย | Scope ที่คาดว่าใช้ |
| --- | --- | --- | --- |
| Organization | `organizations.read` | ดูข้อมูลองค์กรที่ตนสังกัด | Organization |
| Organization | `branches.manage` | จัดการสาขา | Organization |
| Access | `users.manage` | เชิญ ปิดใช้งาน และจัด Membership | Organization/Branch |
| Access | `roles.manage` | จัด Role และ Permission | Organization |
| CRM | `customers.read` | ดู Customer และข้อมูลที่ Allowlist อนุญาต | Organization/Branch/Own |
| CRM | `customers.create` | สร้าง Customer Draft | Organization/Branch |
| CRM | `customers.update` | แก้ Customer ตาม Scope/State | Organization/Branch/Own |
| CRM | `customers.activate` | เปิดใช้ Customer ที่ผ่าน Gate | Organization/Branch |
| CRM | `customers.deactivate` | ปิดใช้ Customer พร้อมเหตุผล | Organization/Branch |
| CRM | `customer-contacts.manage` | จัด Contact/Address และข้อมูลส่วนบุคคล | Organization/Branch/Own |
| CRM | `sites.read` | ดู Site ตาม Data Allowlist | Organization/Branch/Own |
| CRM | `sites.manage` | สร้าง/แก้/ปิดใช้ Site | Organization/Branch/Own |
| CRM | `opportunities.read` | ดู Opportunity ตาม Scope | Organization/Branch/Own |
| CRM | `opportunities.create` | สร้าง Opportunity | Organization/Branch/Own |
| CRM | `opportunities.update` | แก้ Open Opportunity | Organization/Branch/Own |
| CRM | `opportunities.transition` | เปลี่ยน Stage/Close/Reopen ตาม Policy | Organization/Branch/Own |
| Item Master | `items.read` | ดู Item ตาม Scope และ Cost ตามสิทธิ์แยก | Organization/Branch |
| Item Master | `items.create` | สร้าง Item Draft | Organization |
| Item Master | `items.update` | แก้ Item Draft/Active ที่อนุญาต | Organization |
| Item Master | `items.activate` | เปิดใช้ Item ที่ผ่าน Gate | Organization |
| Item Master | `items.deactivate` | ปิดใช้ Item พร้อมเหตุผล | Organization |
| Item Cost | `cost-records.read` | ดูต้นทุน/Evidence ตาม Scope | Organization/Branch |
| Item Cost | `cost-records.create` | สร้าง Cost Record Draft | Organization/Branch |
| Item Cost | `cost-records.submit` | ส่ง Cost Record ให้ตรวจ | Organization/Branch |
| Item Cost | `cost-records.approve` | Approve/Return ตาม Cost Authority | Organization/Branch |
| Item Cost | `cost-records.publish` | Publish Cost ที่อนุมัติแล้ว | Organization/Branch |
| Item Cost | `cost-records.disable` | ปิดใช้ Published Cost พร้อมเหตุผล | Organization/Branch |
| Unit | `units.read` | ดู Unit/Conversion ที่ใช้ได้ | Organization/Shared |
| Unit | `units.manage` | จัดการ Unit/Conversion ตาม Governance | Organization/Shared |
| Item Import | `item-imports.create` | Upload/Preview/Validate Import Batch | Organization |
| Item Import | `item-imports.commit` | Commit Batch ที่พร้อมแบบ Atomic | Organization |
| Survey | `surveys.read` | ดู Survey/Revision/Evidence Metadata | Organization/Branch/Opportunity/Own |
| Survey | `surveys.create` | สร้าง Survey + Draft Revision | Branch/Opportunity/Own |
| Survey | `surveys.update` | แก้ Draft Revision | Branch/Opportunity/Own |
| Survey | `surveys.mark-ready` | ตรวจและล็อก Ready Revision | Branch/Opportunity/Own |
| Survey | `surveys.create-revision` | Clone เป็น Draft Revision ใหม่ | Branch/Opportunity/Own |
| Survey | `surveys.void` | Void Revision พร้อมเหตุผล | Organization/Branch/Opportunity |
| Survey | `surveys.export` | Export Survey/Evidence ตาม Allowlist | Organization/Branch/Opportunity |
| Quick Estimate | `quick-estimates.read` | ดู Quick Estimate ตามขอบเขตที่ได้รับ | Organization/Branch/Opportunity/Own |
| Quick Estimate | `quick-estimates.create` | สร้าง Quick Estimate | Branch/Opportunity/Own |
| Quick Estimate | `quick-estimates.update` | แก้ Draft/Calculated หรือสร้าง Version ใหม่ตามกฎ | Branch/Opportunity/Own |
| Quick Estimate | `quick-estimates.review` | ตรวจรายการที่เข้า Share Policy | Organization/Branch/Opportunity |
| Quick Estimate | `quick-estimates.share` | แสดงหรือส่ง Preliminary Summary ที่ผ่าน Policy | Branch/Opportunity/Own |
| Quick Estimate | `quick-estimates.convert` | Convert Source Version เป็น Official Estimate Draft | Branch/Opportunity/Own |
| Quick Estimate | `quick-estimates.close` | ปิดรายการพร้อมเหตุผล | Branch/Opportunity/Own |
| Pricing Template | `pricing-templates.read` | ดู Template Version ที่เผยแพร่และอยู่ใน Scope | Organization/Branch |
| Pricing Template | `pricing-templates.create` | สร้าง Template Draft | Organization/Branch |
| Pricing Template | `pricing-templates.update` | แก้ Template ที่ยังเป็น Draft/Returned | Organization/Branch |
| Pricing Template | `pricing-templates.submit` | ส่ง Template และ Test Case ให้ตรวจ | Organization/Branch |
| Pricing Template | `pricing-templates.approve` | อนุมัติสูตร Unit, Rate Source และ Test Matrix ตาม Maker–Checker | Organization/Branch |
| Pricing Template | `pricing-templates.publish` | เผยแพร่ Version ที่อนุมัติเข้าสู่ Calibration | Organization/Branch |
| Pricing Template | `pricing-templates.activate` | เปิดใช้ Version หลังเจ้าของธุรกิจอนุมัติผล Pilot | Organization/Branch |
| Pricing Template | `pricing-templates.disable` | หยุดใช้ Version ใหม่ทันทีโดยเก็บเหตุผล | Organization/Branch |
| Pricing Template | `pricing-templates.override-rate` | กำหนด Branch Rate Override พร้อม Effective Period และเหตุผล | Organization/Branch |
| Estimation | `estimates.read` | ดู Estimate | Organization/Branch/Project/Own |
| Estimation | `estimates.create` | สร้าง Estimate | Branch/Project/Own |
| Estimation | `estimates.update` | แก้ Draft/Returned Estimate | Branch/Project/Own |
| Estimation | `estimates.submit` | ส่งขออนุมัติ | Branch/Project/Own |
| Estimation | `estimates.approve` | อนุมัติตาม Authority Matrix | Organization/Branch/Project |
| Estimation | `estimates.override-price` | Override ราคาตามกฎ | Organization/Branch/Project |
| Estimation | `estimates.cancel` | ยกเลิก Draft/Returned; Submitted ต้องผ่าน Cancel Authority | Branch/Project/Own |
| Quotation | `quotations.issue` | ออกใบเสนอราคาจาก Approved Revision | Branch/Project |
| Audit | `audit.read` | ดู Audit Trail ตามขอบเขต | Organization/Branch/Project |

การตรวจวงเงิน กำไรขั้นต่ำ Discount, Exception และ Maker–Checker เป็น Approval Policy เพิ่มจาก Permission; การมี `estimates.approve` ไม่ได้แปลว่าอนุมัติได้ทุกยอดหรืออนุมัติงานตนเองได้ หากยังไม่มี Published Policy/Independent Checker ระบบต้อง Fail-closed ตาม [Approval Matrix](../01-business/approval-matrix.md)

เช่นเดียวกัน การมี `quick-estimates.share` ไม่ได้ข้าม Share Policy และการมี `quick-estimates.review` ไม่ได้อนุญาตให้ตรวจงานของตนเองเมื่อ Maker–Checker มีผล Backend ต้องตรวจ Permission พร้อม Organization, Branch, Opportunity และ Own Scope ทุกครั้ง

การมี `pricing-templates.publish` ไม่ข้ามสถานะ Approval และการมี `pricing-templates.activate` ไม่ข้ามเกณฑ์ผ่าน Pilot ผู้แก้ Template ห้ามอนุมัติ Version เดียวกันเมื่อ Maker–Checker มีผล การ Override Rate ต้องอยู่ในขอบเขต Branch และไม่แก้ Standard Rate ของ Organization

การมี `cost-records.approve` ไม่อนุญาตให้อนุมัติ Cost ที่ตนสร้างหรือแก้ Financial Field ล่าสุด และ `cost-records.publish` ไม่ข้าม Approved State/Overlap/Effective Policy การ Commit Import ที่มี Cost Update ไม่เท่ากับ Publish Cost รายละเอียดอยู่ที่ [Item Master Governance](../01-business/item-master-governance.md)

การมี `opportunities.transition` ไม่อนุญาตให้ข้าม Stage/Required Field หรือ Patch Won โดยตรง และ `surveys.mark-ready` ไม่ข้าม Readiness/File/Scope Check การ Export/Reveal Personal Data ต้องตรวจ Permission แยกและ Audit ตาม [CRM and Site Survey Governance](../01-business/crm-site-survey-governance.md)
