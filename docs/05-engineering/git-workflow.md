# Git Workflow (แนวทาง Git)

**สถานะ:** Accepted Direction

- Branch มีอายุสั้นและส่งมอบ Vertical Slice ที่ Review ได้
- Commit เล็กและสื่อความหมาย ใช้รูปแบบ `type: intent` เช่น `docs: define estimate flow`
- ห้าม Commit Secret, Environment file, generated build output หรือข้อมูลลูกค้า
- Pull Request อธิบาย Problem, Decision, Risk, Verification และ Migration/Operation impact
- Schema Migration และ Contract Breaking Change ต้องเรียกผู้ตรวจที่เกี่ยวข้อง
- ใช้ Protected Main Branch, Required Checks และ Review ก่อน Merge
- Release Tag และ Changelog ต้องเชื่อมกลับไปยัง Commit/PR ได้

ประเภท Commit ที่ใช้บ่อย: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`, `perf`, `security`
