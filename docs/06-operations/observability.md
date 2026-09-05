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

## Alert Principles

- Alert เมื่อผู้ใช้ได้รับผลกระทบหรือกำลังจะกระทบ ไม่ Alert จาก Noise
- ทุก Alert มี Owner, Severity, Runbook และช่องทาง Escalation
- Dashboard แยก System Health ออกจาก Business KPI
- Support ค้นปัญหาจาก Trace ID ที่ผู้ใช้เห็นได้
