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
- Styling ใช้ Tailwind CSS utilities และ semantic `erp-*` tokens เป็นหลัก; ข้อยกเว้นสำหรับ CSS แยกและ inline style ยึด [Tailwind-first Implementation Standard](../../design.md#30-tailwind-first-implementation-standard)
- ยึดมาตรฐาน UI/UX ตาม **Atelier Architectural Navy Sharp (`border-radius: 0px !important`, Solid Navy `#0B3056`, Pure SVG Stroke Icons)**

## มาตรฐาน UI/UX และหน้าจอต้นแบบ
- มาตรฐานการออกแบบและระบบโทเค็นสี/ขนาดตัวอักษร: [Design System (Atelier Architectural Navy Sharp)](../../design.md)
- มาตรฐานหน้าฟอร์ม ERP (Single Route [id], Deferred Upload, Double Submit Lock): [Building ERP Forms](../../.agents/skills/building-erp-forms/SKILL.md)
- หน้าจอต้นแบบระบบ ERP เสมือนจริง (Interactive Master-Detail Prototype): [preview.html](../../preview.html)
- เอกสารสถาปัตยกรรมละเอียดเดิม: [Frontend Architecture Design](../superpowers/specs/2026-09-05-tan-erp-frontend-architecture-design.md)

## โครงสร้างสำหรับเริ่ม Foundation

**สถานะส่วนนี้:** Draft สำหรับตรวจทานก่อนสร้าง Application จริง โฟลเดอร์ต่อไปนี้เป็นแบบแปลน

```text
frontend/
├── package.json
├── next.config.ts
├── tsconfig.json
├── public/
├── e2e/                            # Login, locale และสิทธิ์เข้าถึง
└── src/
    ├── app/[locale]/
    │   ├── layout.tsx
    │   ├── (auth)/login/page.tsx
    │   └── (erp)/
    │       ├── layout.tsx           # ประกอบหน้าระบบกับ Session/Permission state
    │       └── page.tsx             # จุดเข้าระบบหลังตรวจ Current User
    ├── features/
    │   └── auth/
    │       ├── api/                # Current User query และ query keys
    │       ├── components/         # Login และสถานะการเข้าถึง
    │       └── index.ts            # Public exports ของ Feature
    ├── components/
    │   ├── ui/                     # UI พื้นฐานที่ไม่รู้กฎธุรกิจ
    │   └── layout/                 # Navigation และโครงหน้าร่วม
    ├── lib/
    │   ├── api/                    # Central client และ Typed ApiError
    │   ├── auth/                   # Firebase client และ Session lifecycle
    │   ├── permissions/            # Helper สำหรับแสดง Action ตามสิทธิ์
    │   ├── query/                  # QueryClient configuration
    │   └── i18n/                   # Locale และโหลดข้อความ
    ├── generated/api/              # สร้างจาก OpenAPI; ไม่แก้ด้วยมือ
    ├── messages/
    │   ├── th.json
    │   └── en.json
    └── providers/                  # ประกอบ Auth/Query/Locale providers
```

`app` ประกอบหน้า → `features` จัดการ Workflow → `lib/api` ติดต่อ BE โดย Feature ใช้ `components` ร่วมได้ ห้าม Shared UI ย้อนมา Import Business Feature และห้าม Feature เรียก Internal File ของอีก Feature ให้ใช้ Public Export เมื่อต้องใช้ข้ามกัน

เมื่อเพิ่มโมดูล ใช้ชื่อ Feature ตามงานที่ผู้ใช้ทำ เช่น `customers`, `item-master`, `estimates` และ `quotations` โดย Mapping กับ Module ฝั่ง BE ตาม [Module Boundaries](module-boundaries.md) ไม่จำเป็นต้องสร้างหน้าเปล่าสำหรับทุก Module ตั้งแต่ Foundation

## ขอบเขต State และ Session

| ข้อมูล | เจ้าของ | ตำแหน่ง |
| --- | --- | --- |
| Firebase session และ Token lifecycle | Auth adapter | `lib/auth` |
| Current User และข้อมูลธุรกิจจาก BE | TanStack Query | `features/<feature>/api` |
| ค่าฟอร์มและ Error ของฟอร์ม | React Hook Form; Zod ตรวจรูปแบบ | ภายใน Feature |
| เปิด Dialog, เลือก Tab | Component ที่ใช้งาน | Local UI state |
| ข้อความ UI ไทย/อังกฤษ | Message catalog | `messages` |

Query key ของข้อมูลธุรกิจต้องรวมบริบทองค์กรและตัวกรองที่มีผล เมื่อ Logout หรือเปลี่ยนผู้ใช้/องค์กร ต้องยกเลิก Request และล้าง Cache ที่ผูกกับบริบทเดิมก่อนแสดงข้อมูลใหม่ เพื่อไม่ให้ข้อมูลเก่าปรากฏกับผู้ใช้คนถัดไป ทั้งนี้ BE ยังต้องตรวจ Scope ทุก Request

หน้า Login และการเรียก Business API ที่ใช้ Firebase client ทำงานผ่าน Client boundary; การเพิ่ม Server-side fetching ที่ใช้ตัวตนผู้ใช้ต้องออกแบบการส่งต่อ Session ให้ชัดก่อน ห้ามส่ง Token หรือข้อมูลลับผ่าน Props ไปยัง UI ที่ไม่จำเป็นต้องใช้

หน้าเข้าระบบต้องมีสถานะกำลังโหลด, Session หมดอายุ, ไม่มี Membership, ไม่มีสิทธิ์ และเชื่อมต่อ BE ไม่ได้ โดยข้อความที่ผู้ใช้เห็นและการตีความ HTTP error ยึด [Error Contract](../03-contracts/error-contract.md)

## มาตรฐานการจัดการข้อผิดพลาด (Error Handling Standard)

- **ห้ามตรวจสอบข้อผิดพลาดด้วย String Matching ดิบ และห้ามทำ Loose Fallback มั่วเด็ดขาด:**
  - ห้ามเขียน Logic ดักจับหรือเปรียบเทียบข้อความ Error จากสตริง เช่น `error?.message === "..."` โดยเด็ดขาด ทั้งใน UI components, hooks, functions หรือ utility guards
  - **ห้ามทำ Arbitrary/Loose Fallback ใน Functions ทุกประเภท:** ไม่ว่าจะเป็น Guard functions, Resolvers, Transformers, หรือ Mappers ห้ามดักจับข้อความแบบคลุมเครือ ห้ามเดาสุ่ม (No Guesses) และห้าม fallback มั่วโดยไม่มี Type หรือ Contract รองรับอย่างชัดเจน
  - ห้ามแอบใส่ Fallback ดัก `error.message` ซ่อนไว้ใน Guard Functions (เช่น `if (error.message === "...") return true;`) เพื่อหวังแค่ให้เทสต์หรือโค้ดเก่าผ่าน เพราะทำให้เกิดความเปราะบาง (Brittle), ซุกซ่อนปัญหาเชิงโครงสร้าง (Anti-pattern) และขัดต่อมาตรฐาน Production-Grade
- **ใช้ Typed Error และ Machine-Readable Error Codes เท่านั้น (Strict Code Contract):** 
  - การโยน Error ในส่วน Client Preflight (เช่น ก่อนยิง Query หรือ Mutation เมื่อขาด Token หรือ Membership) ให้ใช้ Typed Subclasses ของ `ApiError` เสมอ เช่น `AuthenticationRequiredError` (code: `AUTHENTICATION_REQUIRED`) และ `MembershipRequiredError` (code: `MEMBERSHIP_CONTEXT_REQUIRED`)
  - Error ที่ตอบกลับจาก Backend จะเป็น RFC 7807/9457 Problem Details ซึ่ง `ApiClient` จะแปลงเป็น `ApiError` พร้อม HTTP status code และ machine-readable `code`
- **ใช้ Type Guard Functions และ Instance/Code-driven Narrowing เท่านั้น:**
  - ให้ใช้ Helper Guards กลางจาก `@/lib/api/api-error` เสมอ เช่น `isAuthenticationRequiredError(error)` และ `isMembershipRequiredError(error)`
  - ภายใน Guard Function ต้องตรวจสอบเฉพาะ `instanceof <ErrorClass>` หรือ `error.code === "<STABLE_CODE>"` เท่านั้น ห้ามแตะต้อง `error.message` เด็ดขาด
