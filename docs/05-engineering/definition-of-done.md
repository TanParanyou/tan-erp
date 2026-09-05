# Definition of Done (เกณฑ์ว่างานเสร็จ)

**สถานะ:** Accepted

งาน Feature ถือว่าเสร็จเมื่อทุกข้อที่เกี่ยวข้องผ่าน:

- [ ] Acceptance Criteria และ State/Error cases ชัดเจน
- [ ] ใช้คำศัพท์และ Module Boundary ที่อนุมัติ
- [ ] Authorization, Scope และ Audit ถูกออกแบบและทดสอบ
- [ ] API/OpenAPI และข้อความไทย/อังกฤษตรงกัน
- [ ] Unit/Integration/Contract/E2E tests ตามความเสี่ยงผ่าน
- [ ] Migration ปลอดภัยและมี Runbook เมื่อ Schema เปลี่ยน
- [ ] Loading, Empty, Error, Conflict และ Retry behavior พร้อม
- [ ] Accessibility และ Keyboard flow ผ่าน
- [ ] Logging, Metrics และ Trace ID เพียงพอต่อการ Support
- [ ] เอกสารหลัก Flow และ ADR ที่จำเป็นอัปเดต
- [ ] ไม่มี Secret, Placeholder หรือ Known Critical Vulnerability
- [ ] Build, Static Analysis และ Review ผ่าน
- [ ] Rollout, Rollback/Forward-fix และ Owner ชัดเจน

การ “ทำงานได้บนเครื่องผู้พัฒนา” เพียงอย่างเดียวยังไม่ถือว่า Done
