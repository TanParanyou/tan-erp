# Permission Catalog (รายการสิทธิ์)

**สถานะ:** Draft Baseline — ชื่อ Permission เป็น Contract ที่ต้อง Review ก่อนเริ่มโค้ด

| Module | Permission | ความหมาย | Scope ที่คาดว่าใช้ |
| --- | --- | --- | --- |
| Organization | `organizations.read` | ดูข้อมูลองค์กรที่ตนสังกัด | Organization |
| Organization | `branches.manage` | จัดการสาขา | Organization |
| Access | `users.manage` | เชิญ ปิดใช้งาน และจัด Membership | Organization/Branch |
| Access | `roles.manage` | จัด Role และ Permission | Organization |
| CRM | `customers.read` | ดูลูกค้า | Organization/Branch/Own |
| CRM | `customers.manage` | สร้างและแก้ลูกค้า | Organization/Branch/Own |
| CRM | `opportunities.manage` | จัดการโอกาสการขาย | Organization/Branch/Own |
| Item Master | `items.read` | ดูรายการมาตรฐานและต้นทุนตามสิทธิ์ | Organization |
| Item Master | `items.manage` | สร้าง แก้ และปิดใช้ Item | Organization |
| Survey | `surveys.manage` | สร้างและแก้ Site Survey | Branch/Project/Own |
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
