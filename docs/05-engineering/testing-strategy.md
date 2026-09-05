# Testing Strategy (กลยุทธ์การทดสอบ)

**สถานะ:** Accepted Direction

| ระดับ | พิสูจน์อะไร | ตัวอย่าง |
| --- | --- | --- |
| Unit | กฎคำนวณและ State Transition | Margin, rounding, revision rules |
| Integration | Database/Raw SQL/Transaction/Adapter | Organization filter, concurrency |
| Architecture | Dependency และ Boundary | Domain ไม่อ้าง Infrastructure |
| Contract | API shape, Problem Details, OpenAPI | Error field และ status mapping |
| Component | Form, permission-aware UI, locale | Field errors, disabled actions |
| End-to-End | Critical user journey | Login → Estimate → Approval → Quotation |
| Security | Cross-scope และ privilege escalation | ผู้ใช้สาขา A อ่านสาขา B ไม่ได้ |
| Performance | Query/flow ที่สำคัญกับข้อมูลใกล้จริง | Search estimate, report |
| Recovery | กู้ระบบและข้อมูลได้ | Database + file restore drill |

## Test Data

- สร้างข้อมูลสังเคราะห์ ไม่ใช้ข้อมูลลูกค้าจริง
- ระบุ Organization/Branch/Scope ชัดเพื่อจับข้อมูลรั่วข้ามขอบเขต
- Money/Rounding มี Boundary Cases และค่าที่ตรวจมือได้
- เวลาทดสอบใช้ Clock ที่ควบคุมได้

ทุก Bug สำคัญต้องมี Regression Test ในระดับต่ำที่สุดที่พิสูจน์ปัญหาได้อย่างน่าเชื่อถือ
