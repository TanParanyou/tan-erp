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

บันทึกวันที่ Version ผู้ตัดสินใจ Evidence ที่ตรวจ Remaining Risk และเงื่อนไขหยุด/ย้อน Release ทุกครั้ง
