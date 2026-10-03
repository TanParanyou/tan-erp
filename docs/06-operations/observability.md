# Observability (การมองเห็นสถานะระบบ)

**สถานะ:** Accepted Direction

## Signal

- Logs: Structured, มี Trace ID, Organization ID แบบไม่เปิด PII และ Error Code
- Metrics: Request rate, error rate, latency, saturation, queue/background failures
- Traces: Critical flow จาก API ผ่าน Database/External calls
- Audit: Business/Security actions แยกจาก Technical logs

## Health Endpoints

- Liveness: Process ยังทำงาน
- Readiness: พร้อมรับ Traffic และ Dependency สำคัญใช้งานได้
- Diagnostics: จำกัดสิทธิ์และไม่เปิด Secret/PII

### สถานะ implementation

- `GET /health/live` — ตอบ `200` เมื่อ process ทำงาน (ไม่ตรวจ dependency)
- `GET /health/ready` — ตอบ `200` เมื่อ PostgreSQL เชื่อมต่อได้ภายใน 3 วินาที มิฉะนั้น `503`
- ทั้งสองเรียกได้โดยไม่ต้องยืนยันตัวตน ตอบเฉพาะชื่อสถานะ (`{"status","checks"}`) ไม่มีคำอธิบาย exception หรือ connection string และมี `Cache-Control: no-store`
- ยังไม่มี: Diagnostics (version ของ frontend/backend/schema) และการตรวจ dependency อื่น (Firebase, storage) ใน readiness
- Authentication middleware ทำงานกับทุก request จึงต้องตั้งค่า `Firebase:ProjectId` ให้ถูกต้อง ไม่เช่นนั้น endpoint ตอบ 500

## Alert Principles

- Alert เมื่อผู้ใช้ได้รับผลกระทบหรือกำลังจะกระทบ ไม่ Alert จาก Noise
- ทุก Alert มี Owner, Severity, Runbook และช่องทาง Escalation
- Dashboard แยก System Health ออกจาก Business KPI
- Support ค้นปัญหาจาก Trace ID ที่ผู้ใช้เห็นได้
