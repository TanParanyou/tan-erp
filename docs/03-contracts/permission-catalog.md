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
| Estimation | `estimates.read` | ดู Estimate | Organization/Branch/Project/Own |
| Estimation | `estimates.create` | สร้าง Estimate | Branch/Project/Own |
| Estimation | `estimates.update` | แก้ Draft/Returned Estimate | Branch/Project/Own |
| Estimation | `estimates.submit` | ส่งขออนุมัติ | Branch/Project/Own |
| Estimation | `estimates.approve` | อนุมัติตาม Authority Matrix | Organization/Branch/Project |
| Estimation | `estimates.override-price` | Override ราคาตามกฎ | Organization/Branch/Project |
| Quotation | `quotations.issue` | ออกใบเสนอราคาจาก Approved Revision | Branch/Project |
| Audit | `audit.read` | ดู Audit Trail ตามขอบเขต | Organization/Branch/Project |

การตรวจวงเงิน กำไรขั้นต่ำ และ Maker–Checker เป็น Policy เพิ่มจาก Permission; การมี `estimates.approve` ไม่ได้แปลว่าอนุมัติได้ทุกยอดหรืออนุมัติงานตนเองได้
