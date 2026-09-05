# Frontend Architecture (สถาปัตยกรรมหน้าบ้าน)

**สถานะ:** Accepted สำหรับอนาคต; ยังไม่สร้าง Frontend ใน Documentation Phase

ใช้ Next.js App Router + React + TypeScript และจัดกลุ่มตาม Business Feature

```text
frontend/src/
├── app/[locale]/          # route และ page composition
├── features/              # business UI/workflow
├── components/            # reusable UI/layout
├── lib/                   # API, auth, permissions, query, i18n
├── generated/api/         # types จาก OpenAPI
├── messages/              # th/en UI messages
└── providers/
```

## กฎหลัก

- `app` เรียก Feature ได้ แต่ Feature ห้าม Import จาก Route
- API ทุกตัวผ่าน Central API Client ซึ่งแนบ Token, Locale และ Parse Problem Details
- TanStack Query เป็นเจ้าของ Server State; Local UI State อยู่ใกล้ Component
- Frontend Permission Guard ช่วย UX แต่ Backend เป็น Security Authority
- Form ตรวจรูปแบบฝั่ง Browser เพื่อ Feedback และ Backend ตรวจซ้ำเสมอ
- ภาษาไทยเป็นค่าเริ่มต้น; Message Key ใหม่ต้องมีไทยและอังกฤษ
- ยึดมาตรฐาน UI/UX ตาม **Atelier Architectural Navy Sharp (`border-radius: 0px !important`, Solid Navy `#0B3056`, Pure SVG Stroke Icons)**

## มาตรฐาน UI/UX และหน้าจอต้นแบบ
- มาตรฐานการออกแบบและระบบโทเค็นสี/ขนาดตัวอักษร: [Design System (Atelier Architectural Navy Sharp)](../../design.md)
- หน้าจอต้นแบบระบบ ERP เสมือนจริง (Interactive Master-Detail Prototype): [preview.html](../../preview.html)
- เอกสารสถาปัตยกรรมละเอียดเดิม: [Frontend Architecture Design](../superpowers/specs/2026-09-05-tan-erp-frontend-architecture-design.md)
