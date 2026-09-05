# System Principles (หลักการระบบ)

**สถานะ:** Accepted

1. **Business first:** ชื่อและ Workflow ต้องตรงกับภาษาธุรกิจใน `CONTEXT.md`
2. **Traceable:** เอกสารขาย การอนุมัติ และการแก้ไขต้องย้อนหาที่มาได้
3. **Secure by boundary:** Backend ตรวจสิทธิ์และ Scope ทุกครั้ง
4. **One source of truth:** กฎหนึ่งเรื่องมีเอกสารหลักหนึ่งไฟล์
5. **Modular before distributed:** แยกขอบเขต Module ชัดเจนใน Monolith ก่อนแยก Service
6. **Typed contracts:** API, Query และ Error มีโครงสร้างชัดเจน ไม่ส่งข้อมูลแบบเดาโครงสร้าง
7. **Thai-first, bilingual-ready:** ภาษาไทยต้องอ่านเข้าใจง่ายและมีคำอังกฤษเมื่อช่วยการสื่อสาร
8. **Operationally ready:** Backup, Restore, Monitoring และ Audit เป็นส่วนของงาน ไม่ใช่งานเสริมภายหลัง
9. **Evidence before optimization:** ใช้ Raw SQL หรือแยกระบบเมื่อมีเหตุผลและหลักฐาน
10. **Change without erasing history:** ใช้ Revision และ Audit แทนการเขียนทับข้อมูลสำคัญ
