# Coding Standards (มาตรฐานการพัฒนา)

**สถานะ:** Accepted Direction

เอกสารนี้มีผลเมื่อเริ่ม Application Implementation

## หลักทั่วไป

- ชื่อใน Code ตรงกับ `CONTEXT.md` และ Module Boundary
- File หนึ่งมีความรับผิดชอบหลักหนึ่งเรื่อง; เก็บไฟล์ที่เปลี่ยนพร้อมกันไว้ใกล้กัน
- Public Contract เป็น Typed และมี Validation ที่ Boundary
- Business Rule อยู่ Backend/Domain ไม่ซ้ำเป็นกฎตัดสินที่ Frontend
- Comment อธิบาย “เหตุผล” หรือข้อจำกัด ไม่แปล Code ซ้ำ
- Secret, Token และข้อมูลส่วนบุคคลไม่อยู่ใน Source, Test Fixture หรือ Log

## Backend

- Dependency: Domain ← Application ← Infrastructure/API composition
- Controller บาง; Use Case อยู่ใน Feature Handler
- Async I/O รับ `CancellationToken`
- EF Core เป็น Write Path; Raw SQL ทำตาม Policy
- Exception ที่คาดการณ์ได้แปลงเป็น Result/Error Code ที่กำหนด

## Frontend

- Route ประกอบ Page; Business UI อยู่ใน Feature
- Central API Client เป็นทางออก HTTP เดียว
- Server State อยู่ TanStack Query; Form State อยู่ใกล้ Form
- UI ทุกสถานะมี Loading, Empty, Error, Success และ Permission behavior
- Text Key มีไทยและอังกฤษก่อน Merge
