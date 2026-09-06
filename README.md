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

ระบบมี `Makefile` สำหรับบริหารจัดการ Service และ Lifecycle ต่างๆ (ล็อกพอร์ต Frontend `3005` และ Backend `5005` เพื่อเลี่ยงการชนกับระบบอื่นหรือ macOS AirPlay):

```bash
# 1. ดูคำสั่งทั้งหมดในระบบ
make help

# 2. ติดตั้งและเตรียม Environment ในคำสั่งเดียว (Docker -> Wait DB -> Seed Emulator -> Migrations -> .env)
make dev-setup

# 3. รันทั้ง Backend (:5005) และ Frontend (:3005) พร้อมกัน (มี Auto-kill port ค้าง + Trap Ctrl+C ปิดทั้งคู่)
make dev

# ตรวจสอบ / เคลียร์พอร์ต
make check-ports
make kill-ports

# ตรวจสอบคุณภาพทั้งหมด (Verification Gates)
make verify
```

## ลำดับการเริ่มต้นระบบในเครื่องพัฒนาแบบ Manual (Manual Startup Order)

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
