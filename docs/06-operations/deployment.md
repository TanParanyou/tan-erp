# Deployment (การนำระบบขึ้นใช้งาน)

**สถานะ:** Future Direction

## Pipeline

```text
Commit → Verify → Build immutable artifacts → Security scan
       → Deploy Staging → Smoke/UAT → Approval → Production
       → Health checks → Observe → Complete or Forward-fix
```

## หลักการ

- Build ครั้งเดียวและ Promote Artifact เดิมข้าม Environment
- Migration เป็นขั้นตอนที่เห็นได้และมีเจ้าของ
- Deploy รองรับ Zero/Low Downtime ตาม Criticality
- Feature Flag ใช้กับ Rollout ที่ต้องแยก Deploy ออกจาก Release
- Version ของ Frontend, Backend และ Schema ตรวจสอบได้จาก Health/Diagnostics
- Production ใช้ HTTPS, Secure Headers และ Reverse Proxy ที่ดูแลได้

เทคโนโลยี Hosting จริงยังไม่ล็อกจนทราบงบ ทีมดูแล SLA และข้อกำหนดข้อมูล
