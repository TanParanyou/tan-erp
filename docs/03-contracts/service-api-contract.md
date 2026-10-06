# Installation, Handover, Warranty and Service API Contract (ข้อตกลง API งานติดตั้ง ส่งมอบ ประกัน และบริการหลังการขาย)

**สถานะ:** Implemented 2026-10-04 (CP-14). กฎด้านล่างเป็นค่าเริ่มต้นที่ทีมพัฒนาเลือก (ผู้ใช้มอบหมายให้ตัดสินใจ) **รอ Installation/Customer Service และ Business ยืนยัน**. **ระยะประกันไม่มีค่าเริ่มต้น** — ผู้ส่งมอบต้องระบุเองทุกครั้ง (0 = ไม่มีประกัน) เพื่อไม่กำหนดเงื่อนไขธุรกิจแทนบริษัท.

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| งานติดตั้ง | ต่อ Project (ไม่ใช่ completed/cancelled): `planned → in_progress → ready_for_handover → handed_over` หรือ `cancelled`; เลข `INS-{YYYY}-{SEQ:4}`; ทีมเป็นข้อความ (ไม่ผูกพนักงาน/ปฏิทินกำลังคน) |
| Checklist | ระบุตอนวางแผน (≤100 รายการ, ติดธง "บังคับ" ได้); ติ๊กได้เฉพาะตอน `in_progress` พร้อมผู้ทำ/เวลา; ไฟล์แนบและลายเซ็นผู้รับมอบใช้ [Attachment API](attachment-api-contract.md) (owner `installation-job`, G-01); ยังไม่บังคับให้ส่งมอบต้องมีลายเซ็น |
| Punch list | ข้อบกพร่อง (เล็กน้อย/ปานกลาง/วิกฤต): `open → resolved → verified`, เปิดซ้ำได้ (`reopened`); **ผู้แก้ตรวจยืนยันเองไม่ได้** (`INSTALLATION_SELF_VERIFICATION` + DB check); แจ้งข้อบกพร่องตอน `ready_for_handover` หรือเปิดซ้ำข้อที่ verified แล้วจะดึงงานกลับเป็น `in_progress` |
| พร้อมส่งมอบ | ต้องทำ checklist ที่บังคับครบ (`INSTALLATION_CHECKLIST_INCOMPLETE`) และทุกข้อบกพร่อง `verified` (`INSTALLATION_DEFECTS_OPEN`) |
| ส่งมอบ | ผู้รับมอบแทนลูกค้า (ชื่อ) เลือก `accepted` หรือ `disputed`: **disputed** ต้องมีเหตุผล → งานกลับ `in_progress` (นับจำนวนครั้งและเก็บเหตุผลล่าสุด) ต้องทำให้พร้อมส่งมอบใหม่; **accepted** ต้องระบุ `warrantyMonths` 0–120 ชัดเจน, วันส่งมอบไม่ใช่อนาคต (ว่าง = วันนี้) → `handed_over` |
| ประกัน | `warrantyMonths > 0` สร้าง Warranty หนึ่งใบต่อการส่งมอบ (`WAR-{YYYY}-{SEQ:4}`, unique ต่องานติดตั้ง): เริ่มวันส่งมอบ, สิ้นสุด = วันก่อนครบรอบเดือน (ถ้าวันเริ่มไม่มีในเดือนสุดท้าย เช่น 31 ม.ค. +1 เดือน → 28/29 ก.พ.); สถานะ active/expired คำนวณจากวันนี้ ไม่เก็บซ้ำ; ไม่มีการต่อ/ยกเลิก/แก้ประกันในรอบนี้ |
| ปิดโครงการ | ย้าย Project เป็น `completed` ไม่ได้ถ้ามีงานติดตั้งที่ไม่ใช่ `handed_over`/`cancelled` (`PROJECT_OPEN_INSTALLATION`); โครงการที่ไม่มีงานติดตั้งไม่ถูกกำกับ |
| งานบริการหลังการขาย | ต่อ Project (ไม่ใช่ cancelled): `open → scheduled → in_progress → resolved → closed`, เปิดซ้ำได้จาก resolved/closed (ต้องมีเหตุผล); เลข `SRV-{YYYY}-{SEQ:4}`; ทุกการเปลี่ยนสถานะเก็บ event (ใคร/เมื่อไร/หมายเหตุ); วันนัดห้ามเป็นอดีต |
| การตรวจประกัน | ตอนรับเรื่อง ระบบหาประกันของโครงการที่ **ครอบคลุมวันที่แจ้ง** → `inWarranty` + ผูกใบประกัน; **ไม่ปฏิเสธ** คำขอนอกประกัน (ธงไว้ให้ธุรกิจตัดสินเรื่องค่าบริการ); ค่าบริการ/การชดเชยไม่อยู่ในรอบนี้ |
| Concurrency/Idempotency | ทุกคำสั่งเปลี่ยนสถานะต้อง `If-Match` (`INSTALLATION_VERSION_CONFLICT`/`SERVICE_VERSION_CONFLICT`); สร้างงานติดตั้ง/งานบริการต้องมี `Idempotency-Key` (replay คืนผลเดิม, payload ต่าง 409) |
| Audit | `installation.*`, `service-request.*` บันทึกเลขเอกสาร/สถานะ **ไม่บันทึกชื่อผู้รับมอบ/เหตุผล/หมายเหตุ** (อยู่ในเรคอร์ดเอง) |

## Endpoints

| Action | Method/Path | Permission |
| --- | --- | --- |
| วางแผน/ยกเลิกงานติดตั้ง | `POST /api/v1/installations` · `POST /{id}/cancel {reason}` | `installations.manage` |
| เริ่ม/ติ๊ก checklist/ข้อบกพร่อง/พร้อมส่งมอบ | `POST /{id}/start` · `PUT /{id}/checklist/{itemId} {done}` · `POST /{id}/defects {description, severity}` · `POST /{id}/defects/{defectId}/resolve|verify|reopen` · `POST /{id}/ready` | `installations.operate` |
| ส่งมอบ | `POST /api/v1/installations/{id}/handover {outcome, signerName, note?, warrantyMonths?, handoverDate?}` | `installations.handover` |
| อ่านงานติดตั้ง | `GET /api/v1/installations[/{id}]?search=&status=&projectId=&page=&pageSize=` | `installations.read` |
| ประกัน | `GET /api/v1/warranties[/{id}]?search=&state=active|expired&projectId=` | `warranties.read` |
| งานบริการ | `POST /api/v1/service-requests` · `POST /{id}/schedule {date}|start|resolve {note}|close|reopen {reason}` | `service-requests.manage` |
| อ่านงานบริการ | `GET /api/v1/service-requests[/{id}]?search=&status=&projectId=&inWarranty=` | `service-requests.read` |
| ไฟล์แนบ/ลายเซ็น | ดู [Attachment API](attachment-api-contract.md) | installations.read / operate / handover |

## Errors

`409`: `INSTALLATION_VERSION_CONFLICT`, `INSTALLATION_INVALID_STATE`, `INSTALLATION_PROJECT_NOT_ACTIVE`, `INSTALLATION_DEFECT_INVALID_STATE`, `SERVICE_VERSION_CONFLICT`, `SERVICE_INVALID_STATE`, `SERVICE_PROJECT_INVALID`, `PROJECT_OPEN_INSTALLATION`, `IDEMPOTENCY_KEY_REUSED`. `403`: `INSTALLATION_SELF_VERIFICATION`. `422`: `INSTALLATION_CHECKLIST_INCOMPLETE`, `INSTALLATION_DEFECTS_OPEN`, `INSTALLATION_REASON_REQUIRED`, `INSTALLATION_FIELD_REQUIRED/INVALID`, `SERVICE_REASON_REQUIRED`, `SERVICE_FIELD_REQUIRED/INVALID`. `404` นอก Organization.

## Data (migration `AddServiceModule`, schema `service`)

`installation_jobs` (checks status/dates/warranty_months, `status='handed_over' ⇔ handover_date`), `installation_checklist_items`, `installation_defects` (check verifier ≠ resolver), `warranties` (unique `installation_job_id`), `service_requests` (check `in_warranty ⇔ warranty_id`), `service_request_events`.

## Hand-offs

รับ Project จาก [Project API](project-api-contract.md) และกำกับการปิดโครงการ; ผลผลิตจาก Production/Work Order ยังไม่ถูกผูกกับงานติดตั้งในรอบนี้ (อ้างผ่านโครงการ).
