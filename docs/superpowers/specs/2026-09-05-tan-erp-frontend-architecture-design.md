# tan-erp Frontend Architecture Design

## 1. สถานะเอกสาร

- วันที่ตัดสินใจ: 5 กันยายน 2026
- สถานะ: อนุมัติแนวทางแล้ว
- ขอบเขต: Frontend ของระบบ Project ERP
- Technology หลัก: Next.js, React, TypeScript และ TanStack Query

## 2. เป้าหมาย

Frontend ต้องจัดโครงสร้างตาม Business Feature เพื่อให้ค้นหาไฟล์ที่เกี่ยวข้องกับ Backend ได้ง่าย รองรับภาษาไทยและอังกฤษ จัดการ Server State, Error และ Permission จากจุดกลาง และไม่ทำหน้าที่แทน Business Rule หรือ Backend Authorization

## 3. รูปแบบที่เลือก

ใช้ Next.js App Router โดย:

- `src/app` เป็น Route และ Page Composition
- `src/features` เป็นเจ้าของ UI และ Client Workflow ของแต่ละ Business Feature
- `src/components` เก็บ UI Primitive และ Layout ที่ใช้ร่วมกัน
- `src/lib` เก็บ Infrastructure ฝั่ง Browser เช่น API Client, Authentication, Permission และ i18n
- `src/generated/api` เก็บ Type ที่สร้างจาก OpenAPI และห้ามแก้ด้วยมือ

## 4. Folder Structure

```text
frontend/
|-- AGENTS.md
|-- package.json
|-- next.config.ts
|-- tsconfig.json
|-- public/
|-- e2e/
|   |-- auth.spec.ts
|   `-- items.spec.ts
`-- src/
    |-- app/
    |   `-- [locale]/
    |       |-- layout.tsx
    |       |-- (auth)/login/page.tsx
    |       `-- (erp)/
    |           |-- layout.tsx
    |           |-- dashboard/page.tsx
    |           |-- items/
    |           |-- estimates/
    |           `-- quotations/
    |-- features/
    |   |-- auth/
    |   |-- item-master/
    |   |   |-- api/
    |   |   |-- components/
    |   |   |-- schemas/
    |   |   `-- types/
    |   |-- opportunities/
    |   |-- estimates/
    |   `-- quotations/
    |-- components/
    |   |-- ui/
    |   `-- layout/
    |-- lib/
    |   |-- api/
    |   |-- auth/
    |   |-- permissions/
    |   |-- query/
    |   `-- i18n/
    |-- generated/api/
    |-- messages/
    |   |-- th.json
    |   `-- en.json
    `-- providers/
```

## 5. Dependency Rules

- Route เรียก Feature Component ได้ แต่ Feature ห้าม Import จาก Route
- Feature ใช้ Shared Component และ `lib` ได้
- Feature ห้าม Import Internal File ของ Feature อื่น; การใช้ข้าม Feature ต้องผ่าน Public Export
- Component ห้ามสร้าง URL, เรียก `fetch` หรือ Axios โดยตรง
- Business API ทุกตัวผ่าน Central API Client
- Generated API Type ห้ามถูกแก้ด้วยมือ
- Server State อยู่ใน TanStack Query ไม่ทำสำเนาเข้า Zustand หรือ React Context
- Local UI State อยู่ใกล้ Component; Global UI State เพิ่มเมื่อมี Use Case ข้าม Route จริงเท่านั้น

## 6. API และ Error Contract

Central API Client ต้อง:

- แนบ Firebase ID Token เมื่อมี Session
- ส่งภาษาที่เลือกผ่าน `Accept-Language`
- รับ RFC 9457 Problem Details
- แปลง Response ผิดพลาดเป็น Typed `ApiError`
- เก็บ `status`, `code`, `traceId` และ Field Errors
- ไม่แสดง Raw Response Body หรือ Internal Error ต่อผู้ใช้

Frontend ตัดสินใจจาก HTTP Status และ Stable Error Code ไม่ Parse ข้อความ `title` หรือ `detail`

การแสดงผล:

- 400: Map Field Error เข้ากับ Form
- 401: Refresh Session หรือกลับหน้า Login
- 403: แสดง Forbidden State
- 404: แสดง Not Found โดยไม่เดาว่า Resource มีอยู่หรือไม่
- 409: แจ้ง Conflict และเสนอให้ Reload
- 422: แสดง Business Rule Message
- 500/503: แสดงข้อความกลางและ Trace ID

## 7. Localization

- ภาษาเริ่มต้นคือ `th`
- รองรับ `th` และ `en` ใน Release แรก
- Frontend Message Catalog เป็นเจ้าของข้อความ UI-only
- Backend เป็นเจ้าของข้อความ Validation, Business Rule และ Authorization
- ทุก Request ส่ง Locale ปัจจุบันผ่าน `Accept-Language`
- Route และ Navigation ต้องรักษา Locale เดิม
- Message Key ที่เพิ่มต้องมีทั้งไทยและอังกฤษก่อน Merge

## 8. Authentication และ Permission

- Firebase Authentication ยืนยันตัวตน แต่ไม่เป็นเจ้าของ Business Permission
- Frontend เรียก Current User endpoint เพื่อรับ Organization และ Effective Permissions
- Permission Guard ใช้ซ่อนหรือ Disable Action เพื่อ UX
- Frontend ห้ามถือว่าปุ่มที่ซ่อนคือ Security Boundary
- Backend ต้องตรวจ Permission และ Resource Scope ทุก Request
- Permission Key ใช้ค่าเดียวกับ Backend เช่น `items.read` และ `estimates.approve`

## 9. Form และ Validation

- ใช้ React Hook Form จัดการ Form State
- ใช้ Zod ตรวจรูปแบบข้อมูลเพื่อ Feedback ทันที
- Backend ตรวจ Validation และ Business Rule ซ้ำเสมอ
- Field Name ใน Frontend Schema ต้องตรงกับ API Contract เพื่อ Map Error ได้
- Form ต้องแสดง Loading, Disabled, Error และ Success State ชัดเจน
- Double Submit ต้องถูกป้องกัน และ Mutation ต้องรองรับ Idempotency เมื่อ Backend Contract กำหนด

## 10. Testing

- Unit Test: API parser, Permission helper, Schema และ Pure Utility
- Component Test: Form state, Field Error และ Permission-aware control
- Integration Test: Feature hook กับ Mock Service Worker หรือ Test API boundary
- End-to-End Test: Login, locale, authorization และ Critical Business Flow
- Production Build และ Type Check เป็น Release Gate

## 11. Accessibility และ UX Baseline

- ใช้ Semantic HTML
- Control ทุกตัวใช้ Keyboard ได้
- Focus State ต้องมองเห็น
- Form Error เชื่อมกับ Field ผ่าน accessible description
- Touch Target อย่างน้อย 44px สำหรับ Action สำคัญ
- รองรับ Reduced Motion
- ตารางต้องมี Loading, Error, Empty และ Pagination State

## 12. Non-goals ของ Foundation

- ไม่สร้าง Dashboard ปลอม
- ไม่สร้างข้อมูลตัวอย่างเป็น Production Fallback
- ไม่สร้าง Global State Store โดยไม่มี Use Case
- ไม่คัดลอก WAT-PROFILE Source Code ทั้งชุด
- ไม่สร้าง Customer Portal หรือ Mobile App
- ไม่ให้ Frontend ติดต่อ PostgreSQL หรือ Firebase Admin SDK

## 13. ข้อสรุป

Frontend ของ `tan-erp` ใช้ Next.js App Router และ Feature Folders โดยชื่อ Feature สอดคล้องกับ Backend มี Central API Client สำหรับ Token, Locale และ Problem Details ใช้ TanStack Query เป็นเจ้าของ Server State และใช้ Permission Guard เพื่อ UX ขณะที่ Backend ยังคงเป็นผู้ตัดสิน Authorization ตัวจริง
