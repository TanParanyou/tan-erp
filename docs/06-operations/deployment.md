# Deployment (การนำระบบขึ้นใช้งาน)

**สถานะ:** Ready for Staging Rehearsal — มี Production Docker Artifacts และ Health Probes สมบูรณ์แล้ว

## Pipeline

```text
Commit → Verify → Build immutable artifacts → Security scan
       → Deploy Staging → Smoke/UAT → Approval → Production
       → Health checks (/health/live, /health/ready) → Observe → Complete or Forward-fix
```

## 1. หลักการ

- **Build ครั้งเดียวและ Promote Artifact เดิม:** สร้าง Container Images ที่ Immutable ผ่าน Dockerfile กลาง แล้วส่งต่อ Image Digest เดิมข้าม Environment (Dev → Staging → Production)
- **Zero-Secret Containers:** ไม่มี Hardcoded Secrets ใน Image หรือ Compose file; ค่า Credential และ Key ทั้งหมดส่งผ่าน Environment Variables หรือ Secret Manager
- **Database Migration:** ดำเนินการผ่าน `dotnet ef database update` หรือ Dedicated Migration Runner ก่อนเริ่มหรือระหว่าง Deploy Service ใหม่ พร้อมสำรองข้อมูลทุกครั้ง
- **Health-Driven Rollout & Zero Downtime:** ตรวจสอบความพร้อมของระบบผ่าน `/health/live` และ `/health/ready` ก่อนสลับทราฟฟิก
- **Secure by Default:** Production บังคับรัน Container ด้วย Non-root User, จัดการสิทธิ์โฟลเดอร์ไฟล์แนบเคร่งครัด และใช้งานผ่าน HTTPS Reverse Proxy (Traefik, NGINX หรือ Cloud Load Balancer)

---

## 2. Production Deployment Artifacts (`deploy/`)

ระบบจัดเตรียม Production Artifacts มาตรฐานไว้ในโฟลเดอร์ `deploy/`:

| ไฟล์ | วัตถุประสงค์และรายละเอียดสถาปัตยกรรม |
| --- | --- |
| [`deploy/backend.Dockerfile`](file:///Users/syaco/Documents/development/tan-erp/deploy/backend.Dockerfile) | Multi-stage Dockerfile สำหรับ .NET 10 Web API:<br>• **Build Stage:** ใช้ `mcr.microsoft.com/dotnet/sdk:10.0` ทำ layer caching ของ csproj และ compile Release binaries<br>• **Runtime Stage:** ใช้ `mcr.microsoft.com/dotnet/aspnet:10.0` ติดตั้ง `libfontconfig1` สำหรับ QuestPDF font engine<br>• **Security & Storage:** สร้างโฟลเดอร์ `/var/lib/tan-erp/storage` และรันด้วย non-root user `app` (UID 1654) บนพอร์ต `5179` |
| [`deploy/frontend.Dockerfile`](file:///Users/syaco/Documents/development/tan-erp/deploy/frontend.Dockerfile) | Multi-stage Dockerfile สำหรับ Next.js 16 (React 19):<br>• **Deps Stage:** ใช้ `node:22-alpine` ทำ clean install (`npm ci`)<br>• **Builder Stage:** รับ build arguments (`NEXT_PUBLIC_*`) และ compile `next build`<br>• **Runner Stage:** รัน `npm run start` ด้วย unprivileged user `nextjs` (UID 1001) บนพอร์ต `3000` |
| [`deploy/compose.production.yml`](file:///Users/syaco/Documents/development/tan-erp/deploy/compose.production.yml) | Docker Compose สำหรับ Production & Staging ประกอบด้วย 3 บริการ:<br>1. `postgres`: `postgres:16-alpine` พร้อม volume `tan_erp_pgdata` และ healthcheck `pg_isready`<br>2. `backend`: build จาก `backend.Dockerfile` ผูก volume `tan_erp_storage` และมี healthcheck เรียก `/health/ready`<br>3. `frontend`: build จาก `frontend.Dockerfile` เชื่อมต่อ backend และมี HTTP healthcheck |
| [`deploy/.env.production.example`](file:///Users/syaco/Documents/development/tan-erp/deploy/.env.production.example) | เทมเพลตตัวแปรสภาพแวดล้อมความปลอดภัยสูง (`POSTGRES_PASSWORD`, `JWT_SIGNING_KEY`, `ENCRYPTION_KEY` เป็นค่าบังคับ — compose จะ error ถ้าว่าง; `QUESTPDF_LICENSE_CONFIRMED` default `false` ต้องตั้ง `true` เองหลังธุรกิจยืนยัน license; Postgres bind เฉพาะ `127.0.0.1`; `CORS_ALLOWED_ORIGINS` backend อ่านจริง; ใบรับรอง e-Tax วางที่ `ETAX_CERTIFICATE_HOST_DIR` แล้ว mount อ่านอย่างเดียวที่ `/etc/tan-erp/etax`)  ครอบคลุม Database, JWT/AES Keys (ขั้นต่ำ 32 ตัวอักษร), CORS, URLs, QuestPDF License, Thai SMS Gateway, Accounting Connector และ e-Tax XML Signing |

---

## 3. Health Checks & Observability Probes

สถานะของ Health Check Endpoints ใน Backend (`Program.cs`) **พัฒนาเสร็จสมบูรณ์แล้ว**:

1. **Liveness Probe (`/health/live`):**
   - ตรวจสอบว่า Process ของ Backend API ยังมีชีวิตและสามารถรับคำขอ HTTP ได้
   - Response: `200 OK` พร้อม `{"status":"Healthy"}`
   - ไม่มีการเรียก External Dependencies เพื่อป้องกัน Cascading Failure

2. **Readiness Probe (`/health/ready`):**
   - ตรวจสอบความพร้อมของ Dependencies สำคัญ โดยเฉพาะการเชื่อมต่อ PostgreSQL (`DatabaseHealthCheck`)
   - หากฐานข้อมูลไม่สามารถเข้าถึงได้ จะส่งกลับ `503 Service Unavailable` ทันที
   - ใช้เป็น Gate สกัดการส่ง Traffic ของ Load Balancer และ Orchestrator (Kubernetes / Docker Compose `service_healthy`)

---

## 4. ขั้นตอนการ Deploy บน Staging / Production

### 4.1 เตรียมไฟล์ Environment

```bash
cd deploy
cp .env.production.example .env.production
# ปรับแต่งค่า secret keys และ configuration ใน .env.production
```

### 4.2 เริ่มต้นบริการด้วย Docker Compose

```bash
# สั่ง Build และเริ่มต้น Container ในโหมด Background
docker compose --env-file .env.production -f compose.production.yml up -d --build

# ตรวจสอบสถานะการทำงานและ Health status
docker compose --env-file .env.production -f compose.production.yml ps

# ดู Logs ของ Backend และ Frontend
docker compose --env-file .env.production -f compose.production.yml logs -f backend
```

### 4.3 ตรวจสอบ Health Endpoint ด้วยคำสั่ง Curl

```bash
# ตรวจสอบ Liveness
curl -i http://localhost:5179/health/live

# ตรวจสอบ Readiness (ต้องตอบ Healthy)
curl -i http://localhost:5179/health/ready
```
