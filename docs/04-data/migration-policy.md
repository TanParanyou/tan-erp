# Migration Policy (นโยบายเปลี่ยนฐานข้อมูล)

**สถานะ:** Accepted Direction

## หลักการ

- Migration ที่ใช้ร่วมกันแล้วเป็น Append-only; ไม่แก้ไฟล์เก่าย้อนหลัง
- ทุก Migration มีชื่อสื่อความหมายและผ่าน Code Review
- Schema Change ที่เสี่ยงใช้แนวทาง Expand → Migrate → Contract
- Data Migration แยกขั้นตอนและตรวจจำนวน Record ก่อน/หลัง
- Long-running DDL ต้องประเมิน Lock และเวลารันกับข้อมูลขนาดใกล้ Production
- Deploy Application ที่รองรับ Schema เก่าและใหม่ระหว่างช่วงเปลี่ยน

## Release Gate

1. Apply กับฐานข้อมูลใหม่ได้
2. Apply จาก Version Production ปัจจุบันได้
3. Integration Tests ผ่าน
4. Backup พร้อมและ Restore ล่าสุดผ่าน Drill
5. มี Forward-fix/Rollback Procedure ที่ไม่ทำข้อมูลสูญหาย
6. ผู้รับผิดชอบและ Maintenance Window ชัดเจน

ห้ามใช้ `database update` จากเครื่องนักพัฒนาโดยตรงกับ Production; Production Migration ต้องผ่าน Pipeline/Runbook ที่ Audit ได้
