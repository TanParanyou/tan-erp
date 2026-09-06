# TanErp Frontend

เว็บแอปพลิเคชันส่วนหน้าของระบบ Project ERP (`tan-erp`) พัฒนาด้วย Next.js 16 (App Router), React 19, TypeScript และ TanStack Query

## โครงสร้างไดเรกทอรี (Directory Structure)

```text
src/
├── app/                  # Next.js App Router (Layouts, Pages, Route groups)
├── components/           # UI components พื้นฐานและ Layout shell
├── features/             # Feature modules (auth, ฯลฯ)
├── generated/api/        # TypeScript types ที่สร้างอัตโนมัติจาก OpenAPI (ห้ามแก้ไขด้วยมือ)
├── lib/                  # Utilities, Central API Client, Auth adapter, i18n
├── messages/             # Localization catalogs (th.json, en.json)
└── providers/            # React Context providers (QueryClient, Auth)
```

## คำสั่งสำหรับพัฒนาและตรวจสอบ (Scripts)

```bash
# เริ่มต้นเครื่องพัฒนา
npm run dev

# บิลด์ระบบสำหรับ Production
npm run build

# ตรวจสอบ Lint
npm run lint

# ตรวจสอบ Type
npm run typecheck

# รัน Unit Tests
npm run test

# รัน E2E Tests ด้วย Playwright
npm run test:e2e

# สร้าง TypeScript type จาก OpenAPI ของ Backend
npm run generate:api

# ตรวจสอบว่า Generated type ตรงกับ OpenAPI ล่าสุด
npm run check:api

# รันการตรวจสอบคุณภาพทั้งหมด (Check API, Lint, Typecheck, Test, Build)
npm run verify
```

## การรัน End-to-End (E2E) Acceptance ร่วมกับ Local Stack

```bash
# 1. คัดลอก .env.example เป็น .env.local
cp frontend/.env.example frontend/.env.local

# 2. เริ่มต้นการทำงานของ Frontend Dev Server
npm --prefix frontend run dev

# 3. รัน Playwright Test (เมื่อ Local Stack และ Backend พร้อมแล้ว)
npm --prefix frontend run test:e2e
```

## ตัวแปรสภาพแวดล้อม (Environment Variables)

*หมายเหตุ: ระบุเฉพาะชื่อตัวแปร ห้ามใส่ค่าจริงหรือข้อมูลลับ*

- `NEXT_PUBLIC_API_BASE_URL` — Base URL ของ Backend API
- `NEXT_PUBLIC_FIREBASE_API_KEY` — Firebase Public Web API Key
- `NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN` — Firebase Auth Domain
- `NEXT_PUBLIC_FIREBASE_PROJECT_ID` — Firebase Project ID
- `NEXT_PUBLIC_FIREBASE_APP_ID` — Firebase App ID
- `NEXT_PUBLIC_FIREBASE_AUTH_EMULATOR_URL` — URL ของ Firebase Auth Emulator สำหรับ Non-Production (เช่น `http://127.0.0.1:9099`)
