# Release Readiness (ความพร้อมก่อนปล่อยระบบ)

**สถานะ:** Accepted Checklist

## Product

- [ ] Requirement และ Business Flow ได้รับการยืนยัน
- [ ] UAT ใช้เคสจริงที่ลบข้อมูลอ่อนไหวแล้ว
- [ ] Known Limitations และ Support Owner สื่อสารแล้ว

## Engineering

- [ ] Definition of Done ผ่านทุก Critical Feature
- [ ] Build, Tests, Security Scan และ Migration Rehearsal ผ่าน
- [ ] Version, Change Log และ Artifact ตรวจย้อนกลับได้

## Security and Data

- [ ] RBAC/Scope และ Maker–Checker ทดสอบด้วยหลาย Role/Organization
- [ ] Secret, Personal Data และ File Access ผ่าน Review
- [ ] Backup สำเร็จและ Restore Drill ผ่าน

## Operations

- [ ] Health, Logs, Metrics, Traces และ Alerts พร้อม
- [ ] Runbook, On-call/Contact และ Incident Channel พร้อม
- [ ] Capacity, Rate Limit และ External Dependency failure ถูกทดสอบ
- [ ] Rollout และ Forward-fix/Rollback decision ชัดเจน

## Go/No-go Record

สร้างบันทึกหนึ่งฉบับต่อ release candidate โดยกรอกข้อมูลและลิงก์หลักฐานจริงก่อนให้ผู้มีอำนาจตัดสินใจ. ช่องที่ยังไม่มีผลให้ระบุ `ค้าง` พร้อมเจ้าของงาน; ห้ามถือว่าการมี checklist หรือผล test ของ commit อื่นเป็นหลักฐานผ่าน.

| รายการ | ข้อมูลที่ต้องบันทึก |
| --- | --- |
| Release candidate | วันที่/เวลา, environment, scope ที่รวมและเลื่อน, version, commit SHA, schema/migration version และ immutable artifact ID |
| Product/UAT | เคสและผลของผู้ใช้ตาม role จริง, pilot data/policy ที่อนุมัติ, ข้อจำกัดที่แจ้งผู้ใช้ และลิงก์ verification ของแต่ละ critical slice |
| Engineering/Security | ผล build/test/security/accessibility ของ candidate เดียวกัน, RBAC/organization/file review และรายการ defect ที่ยังเปิด |
| Data/Operations | ผล migration rehearsal บนข้อมูล legacy ที่ลบข้อมูลอ่อนไหวแล้ว หรือเหตุผลที่รับรองการเริ่มด้วยฐานใหม่; backup/restore drill, monitoring, support owner และ staging rehearsal |
| Risk/Decision | Remaining risk/deferral พร้อมผลกระทบและผู้รับผิดชอบ, เงื่อนไขหยุด rollout, rollback/forward-fix, ผู้มีอำนาจตัดสินใจ, เวลา และผล `Go` หรือ `No-go` |

หากหลักฐาน release candidate ยังไม่ครบ ให้คงผลเป็น `Pending decision` และอ้างรายการค้างใน [ERP Completion Plan](../superpowers/plans/2026-09-29-erp-completion-master-plan.md#cp-05--releaseuatoperations). การบันทึก `Go` ต้องเป็นการตัดสินใจของเจ้าของงานที่มีอำนาจ ไม่ใช่การอนุมานจากผล automated tests.
