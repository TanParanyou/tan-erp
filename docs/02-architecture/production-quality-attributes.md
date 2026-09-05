# Production Quality Attributes (คุณสมบัติระดับ Production)

**สถานะ:** Accepted เป็นเป้าหมายคุณภาพ

| ด้าน | เป้าหมายเริ่มต้น | วิธีพิสูจน์ |
| --- | --- | --- |
| Security | Least privilege และแยก Organization | Authorization/penetration scenarios |
| Reliability | Transaction สำคัญไม่เกิดข้อมูลครึ่งชุด | Integration tests และ failure injection |
| Auditability | การอนุมัติและ Revision ย้อนหลังได้ | Audit report walkthrough |
| Performance | หน้าทั่วไปตอบสนองสม่ำเสมอ | ระบุ SLO หลังมีข้อมูลจริงและ Load Test |
| Recoverability | กู้ข้อมูลได้ตาม RPO/RTO ที่ธุรกิจยอมรับ | Restore drill |
| Maintainability | Feature หาได้ง่ายและ Dependency ถูกทาง | Architecture tests และ review |
| Accessibility | งานหลักใช้ Keyboard และ Screen Reader ได้ | Automated + manual checks |
| Localization | ไทย/อังกฤษไม่หลุด Key หรือรูปแบบข้อมูล | Contract/UI tests |
| Observability | ปัญหาผูกจาก User Report ไป Trace/Log ได้ | Trace ID และ incident drill |

ค่า SLO, RPO และ RTO เชิงตัวเลขต้องกำหนดร่วมกับเจ้าของธุรกิจก่อน Production เพราะมีผลต่อต้นทุนระบบ
