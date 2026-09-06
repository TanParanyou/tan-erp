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

## Quick Estimate Pricing Test Matrix

Template Version ต้องผ่าน Test Matrix ก่อน Publish:

Business Fixture และ Expected Result กลางอยู่ที่ [Quick Estimate Template Catalog](../01-business/quick-estimate-template-catalog.md) Test อัตโนมัติต้องใช้ค่าเดียวกันเพื่อป้องกันสูตรในเอกสารกับระบบไม่ตรงกัน

Lifecycle, Approval, Effective Period, Branch Override และ Concurrency Case อยู่ที่ [Pricing Template Governance](../01-business/pricing-template-governance.md)

| กลุ่มทดสอบ | กรณีขั้นต่ำ |
| --- | --- |
| Measurement | Built-in, ผ้าม่าน, Wallpaper, หน่วยถูก/ผิด, ค่าต่ำสุด/สูงสุด และค่าศูนย์/ติดลบ |
| Calculation | Grade/Complexity Factor, Add-on, Minimum Charge, Promotion และหลาย Line |
| Price Range | Base/Max Range, Risk Modifier หลายตัว และห้ามผู้ใช้ลดช่วงโดยไม่มีสิทธิ์ |
| Rounding/Tax | Lower ปัดลง, Upper ปัดขึ้น, รวม/ไม่รวม VAT และ Rate Effective Date |
| Template Lifecycle | Draft/Disabled ใช้ไม่ได้, Calibration ต้อง Review, Active ใช้ Risk-based Policy และ Published Version แก้ไม่ได้ |
| Share Policy | Blocked, PendingReview, Shareable พร้อม Reason Code ทุก Trigger |
| Security | Permission/Scope, Maker–Checker, Branch Rate Override และ Cross-organization denial |
| Reproducibility | Calculation Snapshot เดิมคำนวณซ้ำได้ผลเดิมหลัง Template/Rate ปัจจุบันเปลี่ยน |
| Recovery | Autosave, Retry share, Version Conflict และ Idempotent conversion |

Limited Pilot ต้องเปรียบเทียบ Quick Estimate Range กับ Official Estimate/Quotation และเก็บ Root Cause เมื่อราคาทางการหลุดช่วง โดยไม่ใช้ข้อมูลลูกค้าจริงใน Automated Test
