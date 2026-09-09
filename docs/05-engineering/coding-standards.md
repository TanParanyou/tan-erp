# Coding Standards (มาตรฐานการพัฒนา)

**สถานะ:** Accepted Direction

เอกสารนี้มีผลเมื่อเริ่ม Application Implementation

## หลักทั่วไป

- **อ่านเอกสารหลักก่อนเริ่มงาน:** อ่าน `AGENTS.md` และ `design.md` ก่อนลงมือทำทุกครั้ง เพื่อเข้าใจสถาปัตยกรรมและข้อกำหนดของ Design System
- **Reuse First & Global Reuse:** ค้นหา Component, Hook, Helper, Utility, DTO หรือ Service ในระบบก่อนสร้างใหม่เสมอ หากไม่มีและมีโอกาสใช้ซ้ำ ให้เสนอแนะในการทำ Global Reuse แก่ผู้ใช้ก่อนลงมือสร้าง
- **Minimal Blast Radius:** แตะเฉพาะไฟล์ที่เกี่ยวกับ Task โดยตรง ห้ามแตะต้องหรือ Reformat ไฟล์นอกขอบเขตงาน
- ชื่อใน Code ตรงกับ `CONTEXT.md` และ Module Boundary
- File หนึ่งมีความรับผิดชอบหลักหนึ่งเรื่อง; เก็บไฟล์ที่เปลี่ยนพร้อมกันไว้ใกล้กัน
- Public Contract เป็น Typed และมี Validation ที่ Boundary
- Business Rule อยู่ Backend/Domain ไม่ซ้ำเป็นกฎตัดสินที่ Frontend
- Comment อธิบาย “เหตุผล” หรือข้อจำกัด ไม่แปล Code ซ้ำ
- Secret, Token และข้อมูลส่วนบุคคลไม่อยู่ใน Source, Test Fixture หรือ Log
- หน้าฟอร์ม ERP ปฏิบัติตาม [Building ERP Forms](../../.agents/skills/building-erp-forms/SKILL.md) (Single Dynamic Route `[id]`, Deferred Upload, Double Submit Lock, Safety Confirmation Modal)

## Backend

- Dependency: Domain ← Application ← Infrastructure/API composition
- Controller บาง; Use Case อยู่ใน Feature Handler
- Async I/O รับ `CancellationToken`
- EF Core เป็น Write Path; Raw SQL ทำตาม Policy
- Exception ที่คาดการณ์ได้แปลงเป็น Result/Error Code ที่กำหนด

## Frontend

- **Strict TypeScript:** ห้ามใช้ type `any`, `as any` หรือ `@ts-ignore` กำหนด Type ให้ชัดเจน หรือใช้ `unknown` แล้ว narrow
- **No Hardcoding & Complete i18n:** ห้าม Hardcode ข้อความบน UI และต้องแปลภาษาให้ครบถ้วนทั้งไทย (`th`) และอังกฤษ (`en`) ใน `messages/` เสมอ
- **Tailwind-first:** ใช้ Tailwind utilities และ semantic `erp-*` tokens เป็นค่าเริ่มต้นสำหรับ Styling; ใช้ CSS แยกหรือ inline style เฉพาะข้อยกเว้นใน [Design System](../../design.md#30-tailwind-first-implementation-standard)
- Route ประกอบ Page; Business UI อยู่ใน Feature
- Central API Client เป็นทางออก HTTP เดียว
- Server State อยู่ TanStack Query; Form State อยู่ใกล้ Form
- UI ทุกสถานะมี Loading, Empty, Error, Success และ Permission behavior
- Text Key มีไทยและอังกฤษก่อน Merge
