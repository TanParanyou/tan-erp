# Environments (สภาพแวดล้อม)

**สถานะ:** Accepted Direction

| Environment | วัตถุประสงค์ | ข้อมูล |
| --- | --- | --- |
| Local | พัฒนาและทดสอบรายคน | Synthetic only |
| Test/CI | Automated verification | Ephemeral synthetic data |
| Staging | UAT, migration rehearsal, performance sampling | Masked/synthetic |
| Production | งานจริง | Controlled business data |

## Separation Rules

- Database, Firebase Project, Storage Bucket และ Secret แยกตาม Environment
- Production Credential ห้ามใช้ใน Local/CI
- Configuration ผ่าน Environment/Secret Manager ไม่ฝังใน Source
- Staging ใกล้ Production ด้าน Version และ Topology แต่ไม่คัดลอกข้อมูลส่วนบุคคลโดยไม่ Mask
- การเข้าถึง Production ใช้ Least Privilege, MFA และ Audit
