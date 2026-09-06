# Implementation Roadmap (แผนพัฒนา)

**สถานะ:** Accepted เป็นลำดับการพัฒนา

| ระยะ | เป้าหมาย | ผลส่งมอบหลัก | Gate ก่อนผ่าน |
| --- | --- | --- | --- |
| 0 Documentation | ทำความเข้าใจตรงกัน | Glossary, Flow, Architecture, Contracts, Portal | เจ้าของงานยืนยัน Flow และคำศัพท์ |
| 1 Foundation | ระบบปลอดภัยและวางฐานถูก | Identity, Organization, RBAC, Audit, Localization (อนุมัติเริ่ม Slice: Foundation Login & Current User ตาม `docs/superpowers/plans/2026-09-06-foundation-login-current-user.md`; โมดูลและงานอื่นยังคงถูก Gate) | Test สิทธิ์และข้ามองค์กรผ่าน |
| 2 Estimation MVP | ประเมินราคาได้ครบวงจร | Customer, Survey, Item, Estimate, Revision, Approval | UAT เคสจริงและเทียบผลคำนวณ |
| 3 Commercial | ส่งข้อเสนอและติดตามผล | Quotation, Acceptance, Project Handover | เอกสารขายย้อนกลับ Estimate ได้ |
| 4 Project Control | คุมแผนและงบ | Project, Budget, Change Order, Progress | ต้นทุนและสถานะตรวจย้อนหลังได้ |
| 5 Supply & Production | จัดซื้อ คลัง ผลิต | Supplier, PO, Inventory, BOM, Production, MRP | Master Data และ Stock Accuracy พร้อม |
| 6 Finance & Service | ปิดวงจรธุรกิจ | Billing Integration, Payment Status, Warranty | Reconciliation และ Support Runbook ผ่าน |

ทุกระยะต้องส่งมอบเป็น Vertical Slice ที่ผู้ใช้ทดลองได้ ไม่เปิดหลายโมดูลพร้อมกันโดยยังไม่มี Flow ใดจบครบวงจร

## งานเริ่มต้น BE และ FE

**สถานะ:** Application Implementation อนุมัติเฉพาะขอบเขต Foundation Login และ Current User ตาม `docs/superpowers/plans/2026-09-06-foundation-login-current-user.md` เท่านั้น งานลำดับถัดไปและโมดูลอื่นยังคงถูก Gate ไว้อย่างเคร่งครัด

แบบแปลนโฟลเดอร์อยู่ที่ [Backend Architecture](../02-architecture/backend-architecture.md#โครงสร้างสำหรับเริ่ม-foundation) และ [Frontend Architecture](../02-architecture/frontend-architecture.md#โครงสร้างสำหรับเริ่ม-foundation) ตารางนี้กำหนดลำดับงาน ไม่ใช่การประกาศว่าโครง Application ถูกสร้างแล้ว

| ลำดับ | ผลลัพธ์ที่ตรวจรับได้ | งาน BE | งาน FE | หลักฐานก่อนผ่าน |
| --- | --- | --- | --- | --- |
| 1. Contract ของการเข้าระบบ | ตกลงข้อมูล Current User และสถานะเข้าถึงตรงกัน | ระบุ Endpoint, Membership, Effective Permissions และ Error ในเอกสาร Contract หลัก | กำหนดการแสดง Login/Loading/ไม่มี Membership/Forbidden ตาม Contract | ตัวอย่าง Response สำหรับสำเร็จ, 401, 403 และผู้ใช้ไม่มี Membership พร้อม Stable Code |
| 2. Login ถึงหน้าระบบ | ผู้ใช้ทดสอบเข้าสู่ระบบและเห็นบริบทที่ BE อนุญาต | สร้างสี่ Project, PostgreSQL migration, Firebase verification, Current User, RBAC และ Error localization | สร้าง Next.js shell, Login, Central API client, Generated types, Query และข้อความ th/en | Build/Type check; ทดสอบ Token ไม่ถูกต้อง, ไม่มี Membership, ข้ามองค์กร, Logout แล้ว Cache ไม่หลงเหลือ และ E2E Login |
| 3. การเปลี่ยนสิทธิ์พร้อม Audit | ผู้มีอำนาจจัดการ Membership/Role ได้ และตรวจย้อนหลังได้ | ระบุ Contract, บังคับ Permission/Scope, บันทึก Audit ใน Transaction และรองรับ Concurrent change | หน้าจัดการสิทธิ์ตาม Contract พร้อม Error/Conflict state | ผู้ไม่มีสิทธิ์แก้ไม่ได้; สิทธิ์ที่ถูกถอนใช้ Request ถัดไปไม่ได้; Audit ระบุผู้กระทำและการเปลี่ยนแปลงได้ |
| 4. ธุรกิจ Slice แรก | เข้าสู่ Estimation MVP ตาม Roadmap | เลือกงาน Customer หรือ Item Master ที่จำเป็นต่อ Estimate แล้วทำ API/Validation/Persistence ครบ | ทำ List/Form ที่ใช้ API จริง พร้อม Loading/Empty/Error | Contract, Scope และ E2E ของ Slice ผ่านตาม Definition of Done |

ก่อนงานลำดับ 2 ให้ตรึงรุ่น SDK, Runtime และ Dependency ที่รองรับร่วมกัน พร้อมคำสั่งเริ่มระบบและค่า Environment ตัวอย่างที่ไม่มี Secret ใน README ของแต่ละฝั่ง ติดตั้ง Library เท่าที่ Slice ใช้จริง และตรวจ [Testing Strategy](../05-engineering/testing-strategy.md) กับ [Definition of Done](../05-engineering/definition-of-done.md) เพื่อเลือกชุดตรวจรับตามความเสี่ยง
