# Requirements Catalog (รายการความต้องการระบบ)

**สถานะ:** Baseline Draft — ใช้ควบคุมขอบเขตและยืนยันใน Workshop

## Functional Requirements

| ID | Requirement | Phase | สถานะ |
| --- | --- | --- | --- |
| FR-IAM-001 | ผู้ใช้เข้าสู่ระบบผ่าน Firebase และต้องมี Active Membership | 1 | Accepted direction |
| FR-RBAC-001 | Backend ตรวจ Permission และ Scope ทุก Request | 1 | Accepted direction |
| FR-RBAC-002 | Sensitive approval รองรับ Maker–Checker | 1–2 | Draft rule |
| FR-ORG-001 | ข้อมูลธุรกิจแยกตาม Organization และรองรับ Branch | 1 | Accepted direction |
| FR-I18N-001 | UI และ Error รองรับไทย/อังกฤษ โดยไทยเป็นค่าเริ่มต้น | 1 | Accepted |
| FR-AUD-001 | การแก้สิทธิ์ อนุมัติ Override และเปลี่ยนสถานะสำคัญมี Audit | 1 | Accepted |
| FR-CRM-001 | จัดเก็บ Customer, Contact และ Opportunity ขั้นพื้นฐาน | 2 | Draft |
| FR-SRV-001 | Site Survey เก็บขนาด รูป และเงื่อนไขหน้างาน | 2 | Draft |
| FR-ITEM-001 | Item Master มี Code, Type, Unit, Cost, Effective Date และ Status | 2 | Draft |
| FR-QEST-001 | สร้าง Quick Estimate จาก Pricing Template ที่มี Version | 2 | Accepted principle; Template จริงเป็น Draft |
| FR-QEST-002 | คำนวณและแสดง Price Range จากข้อมูลขั้นต่ำตาม Template | 2 | Accepted principle; สูตรและช่วงเป็น Draft |
| FR-QEST-003 | บังคับ Share Policy ก่อนแสดงหรือส่งข้อมูลให้ลูกค้า | 2 | Accepted principle; Threshold เป็น Draft |
| FR-QEST-004 | แชร์ Preliminary Summary ที่มีเฉพาะข้อมูล Customer-safe | 2 | Accepted principle; รูปแบบเอกสารเป็น Draft |
| FR-QEST-005 | การเปลี่ยนเนื้อหาสำคัญหลังแชร์ต้องสร้าง Quick Estimate Version ใหม่ | 2 | Accepted principle |
| FR-QEST-006 | Convert Quick Estimate Version เป็น Official Estimate Draft แบบ Idempotent | 2 | Accepted principle |
| FR-QEST-007 | Autosave และกู้ Draft หน้างานที่ยัง Sync ไม่สำเร็จได้อย่างปลอดภัย | 2 | Accepted principle |
| FR-EST-001 | Estimate แบ่งพื้นที่/หมวดงาน/Work Item ได้ | 2 | Draft |
| FR-EST-002 | คำนวณต้นทุน ราคาขาย ส่วนลด ภาษี และกำไรอย่างตรวจสอบได้ | 2 | Draft |
| FR-EST-003 | การเปลี่ยนสาระสำคัญสร้าง Estimate Revision ใหม่ | 2 | Accepted principle |
| FR-EST-004 | ตรวจ Missing Data, Stale Cost, Low Margin และ Override ก่อนอนุมัติ | 2 | Draft |
| FR-APP-001 | Routing การอนุมัติพิจารณาวงเงิน Margin Discount และ Risk | 2 | Draft |
| FR-QUO-001 | Quotation อ้างอิง Estimate Revision ที่อนุมัติ | 2–3 | Accepted principle |
| FR-PRJ-001 | Quotation ที่ยืนยันแล้วส่งต่อเป็น Project/Baseline ได้ | 3–4 | Future |
| FR-MRP-001 | MRP คำนวณความต้องการวัสดุจาก BOM, Inventory และ Production Plan | 5 | Future |

## Non-functional Requirements

| ID | Requirement | หลักฐานก่อน Production |
| --- | --- | --- |
| NFR-SEC-001 | ไม่มีการอ่าน/เขียนข้อมูลข้าม Organization โดยไม่ได้รับสิทธิ์ | Security integration tests |
| NFR-SEC-002 | Secret และ Token ไม่อยู่ใน Source/Log | Secret scan และ log review |
| NFR-REL-001 | Critical write เป็น Atomic Transaction | Failure-path integration tests |
| NFR-REC-001 | Backup กู้คืน Database และ File relationship ได้ | Restore drill report |
| NFR-OBS-001 | Error ผู้ใช้เชื่อมไป Trace/Log ด้วย Trace ID ได้ | Incident walkthrough |
| NFR-ACC-001 | Critical flow ใช้ Keyboard และ Screen Reader ได้ | Manual accessibility test |
| NFR-I18N-001 | ทุก Message Key สำคัญมีไทยและอังกฤษ | Automated catalog check |
| NFR-PERF-001 | Critical queries ผ่าน SLO ที่ธุรกิจอนุมัติ | Load test ด้วยข้อมูลใกล้จริง |
| NFR-MNT-001 | Dependency Direction และ Module Boundary ตรวจอัตโนมัติ | Architecture tests |
| NFR-QEST-001 | Flow ขั้นต่ำของ Quick Estimate ใช้งานบนมือถือได้ มีสถานะบันทึกชัดเจน และ Touch Target ไม่น้อยกว่า 44px | Mobile viewport, keyboard และ accessibility test |

## การเปลี่ยน Requirement

Requirement ที่อนุมัติแล้วต้องเปลี่ยนสถานะ เหตุผล ผู้อนุมัติ และเอกสาร/Flow ที่ได้รับผลกระทบ ห้ามเปลี่ยนความหมายของ ID เดิมแบบเงียบ ๆ; หากความหมายเปลี่ยนสาระสำคัญให้สร้าง ID ใหม่และระบุว่าแทนที่รายการใด
