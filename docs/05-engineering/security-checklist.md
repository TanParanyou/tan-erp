# Security Checklist (รายการตรวจความปลอดภัย)

**สถานะ:** Accepted Direction

## ทุก Feature

- [ ] ระบุ Resource Owner และ Organization Scope
- [ ] Authentication และ Permission ถูกตรวจที่ Backend
- [ ] Resource นอก Scope ไม่เปิดเผยการมีอยู่
- [ ] Input ผ่าน Validation และ Output ไม่มีข้อมูลเกิน Contract
- [ ] Query/Raw SQL เป็น Parameterized
- [ ] File Upload ตรวจชนิด ขนาด Malware policy และสิทธิ์
- [ ] Sensitive action มี Audit และ Maker–Checker เมื่อจำเป็น
- [ ] Log ไม่มี Token, Password, Secret หรือ PII เกินจำเป็น
- [ ] Error ภายนอกไม่เผย Stack Trace หรือ Database detail
- [ ] Rate limit/abuse case ถูกพิจารณา
- [ ] Dependency และ Container scan ผ่าน
- [ ] Secret มาจาก Secret Manager/Environment ที่ได้รับอนุญาต

## ก่อน Production

- [ ] Threat Model ของ Critical Flow ได้รับการทบทวน
- [ ] Cross-organization tests ผ่าน
- [ ] Backup encryption และ Restore drill ผ่าน
- [ ] Admin/Break-glass procedure มีเจ้าของและ Audit
- [ ] Security contact และ Incident escalation พร้อม
