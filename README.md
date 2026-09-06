# Project ERP (tan-erp)

`tan-erp` คือระบบ Project ERP สำหรับธุรกิจออกแบบ ตกแต่งภายใน และผลิตงานบิวต์อิน เริ่มต้นจากงานประเมินราคา (Estimate) แล้วค่อยขยายไปสู่งานขาย โครงการ จัดซื้อ คลัง ผลิต ติดตั้ง การเงิน และบริการหลังการขาย

สถานะปัจจุบันคือ **Application Implementation**: ขอบเขตที่ได้รับอนุมัติคือ Foundation Login และ Current User vertical slice ตาม [Foundation Plan](docs/superpowers/plans/2026-09-06-foundation-login-current-user.md) โดยโมดูลธุรกิจอื่นยังคงถูก Gate ไว้อย่างชัดเจน

## ข้อกำหนดของระบบ (Prerequisites)

- **Backend:** .NET SDK `10.0.400` (`net10.0`)
- **Frontend:** Node.js `24.20.0` LTS (ดู `.nvmrc`)
- **Database:** PostgreSQL 17
- **Containers:** Docker / Docker Compose

## การตรวจสอบคุณภาพ (Verification Commands)

```bash
# ตรวจสอบ Fixture สังเคราะห์
npm run test:fixtures

# ตรวจสอบ Backend (Restore, Build, Tests & Architecture Rules)
npm run verify:backend

# ตรวจสอบ Frontend (Generated types, Lint, Typecheck, Unit tests, Build)
npm run verify:frontend

# ตรวจสอบครบทุกส่วน
npm run verify
```

## การเริ่มต้นระบบอย่างรวดเร็วด้วย Make (Quickstart with Make)

ระบบมี `Makefile` สำหรับบริหารจัดการ Service, Lifecycle และแก้ปัญหาพอร์ตชน (ล็อกพอร์ต Frontend `3005` และ Backend `5005` เพื่อเลี่ยงการชนกับระบบอื่นหรือ macOS AirPlay):

```bash
# ดูรายการคำสั่งทั้งหมดพร้อมคำอธิบายภาษาไทย
make help

# เซ็ตอัปสภาพแวดล้อมครบวงจรในคำสั่งเดียว (Docker Up -> รอ DB พร้อม -> Seed Auth -> EF Migration -> สร้าง .env)
make dev-setup

# รันทั้ง Backend (:5005) และ Frontend (:3005) พร้อมกัน (Auto-kill พอร์ตค้าง + Trap Ctrl+C ปิดทั้งคู่)
make dev
```

### สรุปคำสั่งสำคัญแยกตามหมวดหมู่

| หมวดหมู่ | คำสั่ง | คำอธิบาย |
| :--- | :--- | :--- |
| **Development** | `make dev` | รัน Backend (:5005) และ Frontend (:3005) ขนานกัน พร้อมเคลียร์พอร์ตค้าง |
| | `make dev-setup` | เซ็ตอัปทั้ง Docker, Seed และ Migration ให้อัตโนมัติในคำสั่งเดียว |
| | `make dev-backend` | รันเฉพาะ Backend API บนพอร์ต 5005 (โหมด Test + SeedTestData) |
| | `make dev-frontend` | รันเฉพาะ Frontend Next.js บนพอร์ต 3005 |
| **Port Management** | `make check-ports` | ตรวจสอบสถานะพอร์ต 3005, 5005, 9099, 5432 ว่าว่างหรือถูกใช้งาน |
| | `make kill-ports` | ปิด Process ที่ตกค้างบนพอร์ต 3005 และ 5005 เพื่อไม่ให้พอร์ตชน |
| **Docker Containers** | `make up` / `make down` | สตาร์ต / หยุดและลบคอนเทนเนอร์ PostgreSQL 17 และ Firebase Emulator |
| | `make stop` / `make logs` | หยุดคอนเทนเนอร์ชั่วคราว (เก็บข้อมูลไว้) / ติดตาม Logs แบบสด |
| | `make db-up` | สตาร์ตเฉพาะ PostgreSQL Container (พอร์ต 5432) |
| **Database & Migration**| `make db-migrate` | รัน EF Core Migration ล่าสุดเข้าสู่ PostgreSQL |
| | `make db-rollback` | Rollback โครงสร้างตารางกลับสู่จุดเริ่มต้น (0) |
| | `make db-status` | ดูประวัติและสถานะของ EF Core Migrations |
| | `make db-seed` | เพิ่มข้อมูลผู้ใช้ทดสอบเข้าสู่ Firebase Auth Emulator (พอร์ต 9099) |
| **API Contract** | `make api-gen` | เจนเนอเรต TypeScript API Client จากสเปก OpenAPI ล่าสุด |
| | `make api-check` | ตรวจสอบว่าโค้ด TypeScript API ตรงกับ OpenAPI spec หรือไม่ |
| **Quality & Tests** | `make verify` | รัน Verification Gates ครบทั้งระบบตาม Definition of Done |
| | `make test` | รัน Unit / Integration Tests ทั้งหมด (Fixtures, Backend, Frontend) |
| | `make lint` / `typecheck`| ตรวจสอบ ESLint และ TypeScript Types ฝั่ง Frontend |
| **Build & Cleanup** | `make build` | คอมไพล์และบิลด์ทั้งฝั่ง Backend และ Frontend |
| | `make clean` | ลบไฟล์ Build/Cache ชั่วคราว (`bin/`, `obj/`, `.next/`) |
| | `make clean-all` | ล้างไฟล์ทั้งหมดรวมถึง `node_modules` และ Docker Containers |

## ความปลอดภัยและการถือครองข้อมูล (Safe Configuration and Ownership)

- **Firebase Identity Only:** Firebase Public Web Configuration (`NEXT_PUBLIC_FIREBASE_*`) ทำหน้าที่ยืนยันตัวตน (Identity) เท่านั้น **ไม่ใช่** แหล่งกำหนดสิทธิ์หรือการอนุญาต (Authorization); Backend และ PostgreSQL เป็นเจ้าของ Role, Permission, Scope และ Membership ทั้งหมด
- **Service Account Security:** ไฟล์กุญแจและ Credential ของ Firebase Service Account ต้องอยู่นอก Repository เสมอ ห้าม Commit ไฟล์ Secret หรือ Token ใดๆ ลงใน Git
- **No Production / Customer Data:** PostgreSQL ในเครื่องพัฒนาและการทดสอบต้องไม่มีข้อมูลจริงของลูกค้าหรือ Production ใดๆ ทั้งสิ้น อนุญาตเฉพาะข้อมูลสังเคราะห์ที่ระบุ `TEST_ONLY`

## แหล่งข้อมูลสำคัญ

1. [ภาพรวมเอกสาร](docs/README.md)
2. [วิสัยทัศน์ผลิตภัณฑ์](docs/00-overview/product-vision.md)
3. [ขอบเขตและสิ่งที่ยังไม่ทำ](docs/00-overview/scope-and-non-goals.md)
4. [กระบวนการธุรกิจตั้งแต่ต้นจนจบ](docs/01-business/end-to-end-business-flow.md)
5. [Architecture Decision Records](docs/adr/README.md)
6. [Documentation Portal](docs/portal/README.md)
7. [คู่มือปฏิบัติการ Foundation Login](docs/05-engineering/foundation-login-runbook.md)
8. [บันทึกผลการตรวจสอบ Foundation Login](docs/05-engineering/foundation-login-verification.md)
