# Backup and Restore (สำรองและกู้คืน)

**สถานะ:** Accepted Principle; ค่าเวลาเป็น Draft

## ขอบเขต

- PostgreSQL database
- File/object storage และ metadata
- Configuration ที่สร้างใหม่ไม่ได้ง่าย
- Encryption keys/secret recovery ตามนโยบายผู้ให้บริการ

## Runbook ขั้นต่ำ

1. ระบุ Incident และจุดเวลาที่ต้องกู้
2. ป้องกันการเขียนเพิ่มเมื่อจำเป็น
3. เลือก Backup ที่ผ่าน Integrity Check
4. Restore ในพื้นที่แยก
5. ตรวจ Schema, Record counts, critical documents และ file links
6. ให้ Business Owner ยืนยัน Critical Flow
7. Cut over หรือ Export ข้อมูลที่กู้
8. บันทึกเวลา ผลกระทบ และบทเรียน

RPO (ข้อมูลที่ยอมเสียได้) และ RTO (เวลาที่กู้ได้) ต้องได้รับการอนุมัติจากธุรกิจก่อนออกแบบ Production Backup Schedule การมี Backup โดยไม่เคย Restore Test ไม่ถือว่าพร้อม
