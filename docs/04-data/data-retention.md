# Data Retention (การเก็บรักษาข้อมูล)

**สถานะ:** Draft — ต้องยืนยันข้อกฎหมายและนโยบายบริษัท

## กลุ่มข้อมูล

| กลุ่ม | ตัวอย่าง | แนวทาง |
| --- | --- | --- |
| Business Record | Estimate, Quotation, Approval | เก็บตามข้อกำหนดบัญชี/สัญญา |
| Operational | Session, temporary export | อายุสั้นและลบอัตโนมัติ |
| Audit/Security | Permission changes, sign-in events | เก็บตามความเสี่ยงและนโยบายตรวจสอบ |
| Files | รูปหน้างาน แบบ เอกสารลงนาม | ผูก Retention กับ Project/Contract |
| Backup | Database/File backup | หมุนเวียนและทดสอบกู้คืน |

## หลักการ

- กำหนด Purpose, Owner, Retention Period และ Disposal Method ต่อกลุ่มข้อมูล
- การลบต้องครอบคลุม Primary Data, Search Index, Cache และ File Metadata ตามนโยบาย
- Backup อาจลบแบบทันทีไม่ได้ จึงต้องมี Expiry ที่ชัดเจน
- Legal Hold ระงับการลบเฉพาะ Resource ที่เกี่ยวข้องและ Audit ได้
- Personal Data Export/Deletion ต้องผ่านการยืนยันสิทธิ์และผลกระทบทางบัญชี/สัญญา
