# Item Master UAT Scenarios (สถานการณ์ทดสอบรับรองผู้ใช้)

**สถานะ:** Accepted Direction — Production Baseline

ข้อมูล ชื่อ และตัวเลขทั้งหมดเป็น `TEST_ONLY` ไม่ใช่ราคาจริง UAT ใช้บัญชีแยก Maker/Checker และอย่างน้อยสอง Organization/Branch

## Critical Scenarios

| ID | Role | Scenario | Expected |
| --- | --- | --- | --- |
| `UAT-ITEM-001` | Item Master Owner | สร้าง Material Draft แล้ว Activate เมื่อ Field/Capability/Base Unit ครบ | Active; Audit ครบ; ค้นหาได้ |
| `UAT-ITEM-002` | Item Master Owner | สร้าง Code ที่ต่างเพียง Case/Space | `ITEM_CODE_CONFLICT`; ไม่มีรายการซ้ำ |
| `UAT-ITEM-003` | Item Master Owner | Activate โดยไม่มี Base Unit/Capability | Field Error ชี้ตำแหน่ง; ยัง Draft |
| `UAT-ITEM-004` | Data Steward | สร้าง Exact และ Item-specific Conversion | ใช้ได้เฉพาะ Dimension/Item ที่กำหนด; Cycle ถูก Block |
| `UAT-ITEM-005` | Cost Owner/Approver | Create → Submit → Approve → Publish Cost | แยก Maker–Checker; Published อ่านอย่างเดียว |
| `UAT-ITEM-006` | Cost Owner | Maker พยายาม Approve Cost ตนเอง | 403; State/Audit ไม่เสีย |
| `UAT-ITEM-007` | Cost Approver | Publish Cost ที่ Period/Quantity ซ้อน | Atomic Reject พร้อม Stable Code |
| `UAT-ITEM-008` | Estimator | Resolve เมื่อมี Organization Default และ Branch Override | Branch Record ชนะและคืน Evidence/Policy Version |
| `UAT-ITEM-009` | Estimator | Resolve ตาม Quantity Break/Effective Date | เลือก Record ตรงช่วงแบบ Deterministic |
| `UAT-ITEM-010` | Estimator | Resolve ไม่พบ Cost | `ITEM_COST_NOT_FOUND`; ไม่มีราคาคาดเดา |
| `UAT-ITEM-011` | Data Steward | สร้าง Candidate ที่เสมอกันหลังทุก Rule | `ITEM_COST_AMBIGUOUS`; ต้องแก้ข้อมูลก่อนใช้ |
| `UAT-ITEM-012` | Item Master Owner | Deactivate Item ที่เคยอยู่ใน Estimate | งานใหม่ใช้ไม่ได้; Snapshot/ประวัติเดิมอ่านได้ |
| `UAT-ITEM-013` | Import Operator | Preview Import ที่มี Error หนึ่งแถวแล้วกด Commit | Commit ไม่ได้; ชี้ Row/Field; Core Data ไม่เปลี่ยน |
| `UAT-ITEM-014` | Import Checker | Commit Batch พร้อม Retry Key เดิมหลัง Timeout | เกิดผลครั้งเดียวและคืน Result เดิม |
| `UAT-ITEM-015` | Unauthorized User | อ่าน/แก้ Item ของอีก Organization | 404; ไม่มีข้อมูลรั่ว; มี Security Audit ตาม Policy |
| `UAT-ITEM-016` | Estimator/Auditor | Publish Cost รุ่นใหม่หลัง Estimate คำนวณแล้ว | Estimate Snapshot เดิมยัง Reproduce ได้ |
| `UAT-ITEM-017` | Thai/English User | Trigger Validation เดียวกันสองภาษา | Code เดิม; title/detail ตามภาษา; fallback ไทย |
| `UAT-ITEM-018` | Keyboard/Mobile User | ค้นหา เปิดรายละเอียด แก้ Item และอ่าน Cost State ที่ 320px/200% | Focus เห็น, Target ≥44px, ไม่พึ่งสี, ไม่มีข้อมูลสำคัญถูกตัด |
| `UAT-ITEM-019` | Item Master Owner | บันทึก Name/Description สองภาษาเป็น JSONB แล้วค้นหาด้วยชื่อไทย/อังกฤษ | ได้ Item เดิม; Key/Type ที่ไม่อนุญาตถูก Reject |
| `UAT-ITEM-020` | Branch User | เปิด Catalog ที่สาขาซึ่ง Item แบบ Selected Branches ไม่ได้เปิดใช้ | Item ไม่ปรากฏและไม่รั่วข้อมูลข้าม Scope |
| `UAT-ITEM-021` | Estimator | เลือก Item เดียวกันในสองสาขาที่มี Cost Override ต่างกัน | Item ID เดียว; Cost Record/Snapshot ต่างตาม Branch |
| `UAT-ITEM-022` | Item Master Owner | แนบ JPEG/PNG/WebP ที่ Verified และตั้ง Primary Image | Catalog แสดงภาพผ่าน Authorized File API และมี Audit |
| `UAT-ITEM-023` | Malicious User | แนบไฟล์ผิด Signature, ข้าม Parent หรือข้าม Organization | Reject; ไม่มี Relation/File Disclosure |
| `UAT-ITEM-024` | Concurrent Editors | ตั้ง Primary Image หรือแก้ Branch Availability พร้อม ETag เดิม | Request หนึ่งสำเร็จ อีก Request Conflict; ไม่เกิด Primary ซ้ำ |
| `UAT-ITEM-025` | Estimator | เลือก Catalog Item แล้วราคา Published เปลี่ยนก่อน Save Estimate | Backend Reject Version Conflict/ให้เลือกใหม่; ไม่เชื่อราคา Client |
| `UAT-ITEM-026` | Auditor | ตรวจ Item/Branch/Image/Cost หลังแก้หลายครั้ง | Audit ระบุ Actor, Action, Resource, Trace และ Changed Fields ครบโดยไม่มี Secret |
| `UAT-ITEM-027` | Item Master Owner | สร้าง Category แม่/ลูกและพยายามสร้าง Cycle | โครงสร้างปกติบันทึกได้; Cycle ถูก Reject |
| `UAT-ITEM-028` | Estimator | ค้น Item ด้วย Alias “ไม้เขียว” และกรอง Brand | ได้ Item เดิมพร้อม Canonical Name/Brand ที่ถูกต้อง |
| `UAT-ITEM-029` | Cross-organization User | อ้าง Category หรือ Brand ของอีก Organization | 404; ไม่มีข้อมูลรั่ว; State ไม่เปลี่ยน |

## Production Sign-off

- Business Owner ยืนยัน Item Type, Category, Capability และสถานะ
- Data Steward ยืนยัน Unit, Conversion, Code normalization และ Import Template
- Finance/Cost Owner ยืนยัน Source Priority, Effective Period, Staleness, Zero/Manual Cost และ Branch Override
- Security Owner ยืนยัน Permission, Scope, Maker–Checker และข้อมูล Cost ที่แต่ละ Role มองเห็น
- Scenario `001–029` ผ่าน หรือมี Business Decision/Exception ที่บันทึกและอนุมัติ
- ค่า `TEST_ONLY` ถูกแทนด้วย Published Policy/ข้อมูลจริงก่อน Production

อ้างอิง [Governance](../01-business/item-master-governance.md), [API Contract](../03-contracts/item-master-api-contract.md) และ [Data Contract](../04-data/item-master-data-contract.md)
