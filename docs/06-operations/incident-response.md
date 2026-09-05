# Incident Response (การรับมือเหตุขัดข้อง)

**สถานะ:** Accepted Direction

## Severity ตัวอย่าง

- SEV-1: ระบบหลักใช้ไม่ได้ ข้อมูลรั่ว หรือเสี่ยงสูญหาย
- SEV-2: Critical Flow เสียหายแต่มีทางเลี่ยงจำกัด
- SEV-3: กระทบบาง Feature/ผู้ใช้และมีทางเลี่ยง
- SEV-4: ปัญหาเล็กหรือเชิงปรับปรุง

## Flow

```text
Detect → Triage → Contain → Communicate → Recover
       → Verify → Review → Prevent recurrence
```

## บันทึก Incident

- เวลาเริ่ม/ตรวจพบ/แก้ไข
- ผู้บัญชาการเหตุการณ์และผู้รับผิดชอบแต่ละด้าน
- Impact, Scope และข้อมูลที่ได้รับผลกระทบ
- Timeline ของการตัดสินใจ
- Evidence เช่น Trace ID, Metric และ Audit event
- Recovery verification
- Root cause และ Corrective actions ที่มี Owner/กำหนดเวลา

ห้ามแก้ข้อมูล Production โดยตรงโดยไม่มีคำสั่งที่ Review, Backup และ Audit ได้
