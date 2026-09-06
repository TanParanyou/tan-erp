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
| FR-QEST-001 | สร้าง Quick Estimate จาก Pricing Template ที่มี Version | Future | Deferred optional module |
| FR-QEST-002 | คำนวณและแสดง Price Range จากข้อมูลขั้นต่ำตาม Template | Future | Deferred optional module |
| FR-QEST-003 | บังคับ Share Policy ก่อนแสดงหรือส่งข้อมูลให้ลูกค้า | Future | Deferred optional module |
| FR-QEST-004 | แชร์ Preliminary Summary ที่มีเฉพาะข้อมูล Customer-safe | Future | Deferred optional module |
| FR-QEST-005 | การเปลี่ยนเนื้อหาสำคัญหลังแชร์ต้องสร้าง Quick Estimate Version ใหม่ | Future | Deferred optional module |
| FR-QEST-006 | Convert Quick Estimate Version เป็น Official Estimate Draft แบบ Idempotent | Future | Deferred optional module |
| FR-QEST-007 | Autosave และกู้ Draft หน้างานที่ยัง Sync ไม่สำเร็จได้อย่างปลอดภัย | Future | Deferred optional module |
| FR-QEST-008 | คำนวณ Amount/Price Range แบบ Deterministic จาก Template และ Calculation Snapshot พร้อม Outward Rounding/Tax Display Policy | Future | Deferred optional module |
| FR-QEST-009 | Pricing Template Version ใช้ Lifecycle ที่อนุมัติและ Published Version แก้ย้อนหลังไม่ได้ | Future | Deferred optional module |
| FR-QEST-010 | Measurement Rule รองรับ Built-in, ผ้าม่าน และ Wallpaper ด้วย Field/Unit ตาม Template | Future | Deferred optional module |
| FR-QEST-011 | Share Policy คืนผล Blocked, PendingReview หรือ Shareable พร้อม Stable Reason | Future | Deferred optional module |
| FR-EST-001 | Estimate แบ่งพื้นที่/หมวดงาน/Work Item ได้ | 2 | Draft |
| FR-EST-002 | คำนวณต้นทุน ราคาขาย ส่วนลด ภาษี และกำไรอย่างตรวจสอบได้ | 2 | Draft |
| FR-EST-003 | การเปลี่ยนสาระสำคัญสร้าง Estimate Revision ใหม่ | 2 | Accepted principle |
| FR-EST-004 | ตรวจ Missing Data, Stale Cost, Low Margin และ Override ก่อนอนุมัติ | 2 | Draft |
| FR-EST-005 | Field Catalog กำหนด Required Gate, Validation, Permission และ Customer Visibility | 2 | Accepted direction |
| FR-EST-006 | Calculation Policy มี Version/Effective Period และทุกผลมี Calculation Snapshot ที่ทำซ้ำได้ | 2 | Accepted direction |
| FR-EST-007 | Client เขียน Derived Total, Margin, Tax หรือ Approval State โดยตรงไม่ได้ | 2 | Accepted principle |
| FR-EST-008 | Approved, Quoted และ Cancelled Revision แก้ย้อนหลังไม่ได้ | 2 | Accepted principle |
| FR-APP-001 | Routing การอนุมัติพิจารณาวงเงิน Margin Discount และ Risk | 2 | Accepted direction |
| FR-APP-002 | ไม่มี Policy/Checker ที่ใช้ได้ต้อง Fail-closed และทุก Estimate ใช้ Maker–Checker จนยืนยันวงเงินจริง | 2 | Accepted direction |
| FR-APP-003 | Approval Route และ Threshold ถูก Freeze ตอน Submit และ Policy ใหม่ไม่แก้ Route เดิม | 2 | Accepted direction |
| FR-APP-004 | Permission, Resource Scope และ Approval Authority ต้องผ่านพร้อมกัน | 2 | Accepted direction |
| FR-QUO-001 | Quotation อ้างอิง Estimate Revision ที่อนุมัติ | 2–3 | Accepted principle |
| FR-QUO-002 | Customer-facing Output ใช้ Allowlist และไม่เผย Cost, Margin, Internal Note หรือ Approval Detail | 2–3 | Accepted direction |
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
| NFR-EST-001 | Calculation เดิมให้ผลซ้ำจาก Input/Cost/Policy Snapshot เดิม | Deterministic calculation contract tests |
| NFR-EST-002 | State, Field Gate และ Reason Code ตรงกันระหว่าง Business, API, Data และ UI | Contract/document consistency check |
| NFR-EST-003 | Critical Official Estimate flow ผ่าน UAT พร้อม Business/Finance/Security Sign-off | Official Estimate UAT evidence |
| NFR-EST-004 | Customer-facing Projection ไม่มีข้อมูลต้นทุนหรือกฎอนุมัติภายใน | Projection allowlist security tests |
| NFR-QEST-001 | Flow ขั้นต่ำของ Quick Estimate ใช้งานบนมือถือได้ มีสถานะบันทึกชัดเจน และ Touch Target ไม่น้อยกว่า 44px | Mobile viewport, keyboard และ accessibility test |
| NFR-QEST-002 | Calculation Snapshot เดิมต้องคำนวณซ้ำได้ผลเดิม และ Template Publish ไม่ได้จนกว่า Test Matrix ผ่าน | Deterministic calculation และ template contract tests |

## การเปลี่ยน Requirement

Requirement ที่อนุมัติแล้วต้องเปลี่ยนสถานะ เหตุผล ผู้อนุมัติ และเอกสาร/Flow ที่ได้รับผลกระทบ ห้ามเปลี่ยนความหมายของ ID เดิมแบบเงียบ ๆ; หากความหมายเปลี่ยนสาระสำคัญให้สร้าง ID ใหม่และระบุว่าแทนที่รายการใด
