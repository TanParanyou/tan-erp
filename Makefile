# =================================================================
# TAN-ERP PROJECT MAKEFILE
# Compatible with macOS GNU Make 3.81+
# =================================================================

SHELL := /bin/bash
.DEFAULT_GOAL := help

# Smart .NET binary resolver (detects local SDK in ~/.dotnet and tools)
DOTNET_ENV := PATH="$(HOME)/.dotnet/tools:$(HOME)/.dotnet:$$PATH" DOTNET_ROOT="$(HOME)/.dotnet" DOTNET_MULTILEVEL_LOOKUP=0
DOTNET     := $(if $(wildcard $(HOME)/.dotnet/dotnet),$(DOTNET_ENV) $(HOME)/.dotnet/dotnet,dotnet)

# -----------------------------------------------------------------
# Configurable Ports and Settings
# -----------------------------------------------------------------
FE_PORT       ?= 3005
BE_PORT       ?= 5005
EMULATOR_PORT ?= 9099
DB_PORT       ?= 5432
DB_HOST       ?= localhost
DB_NAME       ?= tan_erp
DB_USER       ?= postgres
DB_PASS       ?= postgres
DB_CONN       ?= "Host=$(DB_HOST);Database=$(DB_NAME);Username=$(DB_USER);Password=$(DB_PASS);Port=$(DB_PORT)"

COMPOSE_FILE  := deploy/compose.yml
BACKEND_SLN   := backend/TanErp.slnx
BACKEND_API   := backend/src/TanErp.Api
BACKEND_INFRA := backend/src/TanErp.Infrastructure
FRONTEND_DIR  := frontend

.PHONY: help \
        check-ports kill-ports \
        dev dev-setup dev-backend dev-frontend dev-env \
        up down stop restart logs ps db-up db-stop emulator-up \
        db-wait db-migrate db-rollback db-status db-seed seed seed-users db-reset fresh \
        api-gen api-check \
        verify verify-backend verify-frontend \
        test test-backend test-frontend test-e2e e2e test-fixtures \
        lint typecheck \
        build clean clean-all deps install open

# -----------------------------------------------------------------
# 1. แสดงคู่มือและรายการคำสั่งทั้งหมด (Help)
# -----------------------------------------------------------------
help:
	@echo ""
	@echo "================================================================="
	@echo "                   TAN-ERP MAKE COMMANDS                         "
	@echo "================================================================="
	@echo "พอร์ตมาตรฐาน: Frontend=$(FE_PORT) | Backend=$(BE_PORT) | Emulator=$(EMULATOR_PORT) | DB=$(DB_PORT)"
	@echo ""
	@echo "  🛠️  การรันและพัฒนาระบบ (Development):"
	@echo "    make dev            - รันทั้ง Frontend (:$(FE_PORT)) และ Backend (:$(BE_PORT)) พร้อมกัน (เคลียร์พอร์ตค้าง + ดักจับ Ctrl+C เพื่อปิดทั้งคู่)"
	@echo "    make dev-setup      - เซ็ตอัปสภาพแวดล้อมครบวงจรในคำสั่งเดียว (Docker -> รอ DB พร้อม -> Seed ข้อมูล -> Migrate DB -> สร้าง .env)"
	@echo "    make dev-frontend   - รันเฉพาะ Next.js Frontend บนพอร์ต $(FE_PORT)"
	@echo "    make dev-backend    - รันเฉพาะ Backend API บนพอร์ต $(BE_PORT)"
	@echo "    make dev-env        - เตรียมไฟล์ frontend/.env.local จาก .env.example (หากยังไม่มี)"
	@echo "    make deps / install - ติดตั้ง dependencies ทั้งหมดในโปรเจกต์ (npm + dotnet restore)"
	@echo "    make open           - เปิดเบราว์เซอร์ไปยังหน้า Login (http://localhost:$(FE_PORT)/th/login)"
	@echo ""
	@echo "  🔌 การจัดการพอร์ต (Port Management):"
	@echo "    make check-ports    - ตรวจสอบสถานะของพอร์ต $(FE_PORT), $(BE_PORT), $(EMULATOR_PORT), $(DB_PORT)"
	@echo "    make kill-ports     - ปิด Process ที่ตกค้างบนพอร์ต $(FE_PORT) และ $(BE_PORT)"
	@echo ""
	@echo "  🐳 จัดการ Docker Containers (Infrastructure):"
	@echo "    make up             - สตาร์ตคอนเทนเนอร์ PostgreSQL และ Firebase Emulator ในโหมด Background"
	@echo "    make down           - หยุดและลบคอนเทนเนอร์ Docker ทั้งหมด"
	@echo "    make stop           - หยุดคอนเทนเนอร์ชั่วคราว (เก็บรักษาข้อมูลใน Volume ไว้)"
	@echo "    make restart        - รีสตาร์ตคอนเทนเนอร์ Docker ทั้งหมด"
	@echo "    make logs           - แสดงและติดตาม Logs ของคอนเทนเนอร์ Docker แบบเรียลไทม์"
	@echo "    make ps             - แสดงสถานะการทำงานของคอนเทนเนอร์ทั้งหมด"
	@echo "    make db-up          - สตาร์ตเฉพาะคอนเทนเนอร์ PostgreSQL (:$(DB_PORT))"
	@echo "    make db-stop        - หยุดเฉพาะคอนเทนเนอร์ PostgreSQL"
	@echo "    make emulator-up    - สตาร์ตเฉพาะคอนเทนเนอร์ Firebase Auth Emulator (:$(EMULATOR_PORT))"
	@echo ""
	@echo "  🗄️  ฐานข้อมูลและ Migration (Database & Migrations):"
	@echo "    make db-migrate     - Apply EF Core Migration รุ่นล่าสุดเข้าสู่ฐานข้อมูล PostgreSQL"
	@echo "    make db-rollback    - Rollback โครงสร้างตารางกลับสู่จุดเริ่มต้น (0)"
	@echo "    make db-status      - แสดงรายการและประวัติสถานะของ EF Core Migration"
	@echo "    make db-seed / seed - เพิ่มข้อมูลผู้ใช้ทดสอบเข้าสู่ Firebase Auth Emulator"
	@echo "    make db-reset/fresh - รีเซ็ตฐานข้อมูลใหม่หมด (Rollback -> Migrate -> Seed)"
	@echo ""
	@echo "  📜 สัญญาเชื่อมต่อ API (API Contracts):"
	@echo "    make api-gen        - เจนเนอเรต TypeScript API Client จากสเปก OpenAPI ล่าสุด"
	@echo "    make api-check      - ตรวจสอบว่าโค้ด TypeScript API ตรงกับ OpenAPI spec หรือไม่"
	@echo ""
	@echo "  🧪 การทดสอบและตรวจสอบคุณภาพ (Verification & Testing):"
	@echo "    make verify         - รัน Verification Gates ครบทุกส่วนตามเงื่อนไข Definition of Done"
	@echo "    make test           - รัน Unit / Integration Tests ทั้งหมด (Fixtures + Backend + Frontend)"
	@echo "    make test-backend   - รันการทดสอบฝั่ง Backend (.NET Test Suite)"
	@echo "    make test-frontend  - รันการทดสอบฝั่ง Frontend (Vitest)"
	@echo "    make test-e2e / e2e - รันการทดสอบ End-to-End (Playwright) ชี้ไปยังพอร์ต $(FE_PORT)"
	@echo "    make test-fixtures  - รันการทดสอบตรวจสอบ Baseline Fixtures"
	@echo "    make lint           - ตรวจสอบรูปแบบและความสะอาดของโค้ด Frontend ด้วย ESLint"
	@echo "    make typecheck      - ตรวจสอบความถูกต้องของ Type ด้วย TypeScript Compiler"
	@echo ""
	@echo "  🧹 การ Build และทำความสะอาด (Build & Cleanup):"
	@echo "    make build          - คอมไพล์/บิลด์ทั้งฝั่ง Backend และ Frontend"
	@echo "    make clean          - ลบไฟล์ Build และ Cache ชั่วคราว (bin, obj, .next)"
	@echo "    make clean-all      - ล้างไฟล์ทั้งหมดรวมถึง node_modules และคอนเทนเนอร์"
	@echo "================================================================="
	@echo ""

# -----------------------------------------------------------------
# 2. จัดการพอร์ต (Port Management)
# -----------------------------------------------------------------
# ตรวจสอบว่าพอร์ตสำคัญของระบบถูกใช้งานอยู่หรือไม่
check-ports:
	@echo "กำลังตรวจสอบสถานะพอร์ตที่เกี่ยวข้องกับ tan-erp..."
	@for port in $(FE_PORT) $(BE_PORT) $(EMULATOR_PORT) $(DB_PORT); do \
		pid=$$(lsof -ti :$$port 2>/dev/null); \
		if [ -n "$$pid" ]; then \
			echo "  - พอร์ต $$port: ใช้งานอยู่ (PID: $$pid)"; \
		else \
			echo "  - พอร์ต $$port: ว่าง (พร้อมใช้งาน)"; \
		fi; \
	done

# ปิด Process ที่ค้างบนพอร์ต Frontend และ Backend อย่างปลอดภัย
kill-ports:
	@echo "ตรวจสอบและเคลียร์โปรเซสที่ค้างบนพอร์ต $(FE_PORT) และ $(BE_PORT)..."
	@for port in $(FE_PORT) $(BE_PORT); do \
		pid=$$(lsof -ti :$$port 2>/dev/null); \
		if [ -n "$$pid" ]; then \
			echo "กำลังปิด Process บนพอร์ต $$port (PID: $$pid)..."; \
			kill -9 $$pid 2>/dev/null || true; \
		else \
			echo "พอร์ต $$port ว่างเรียบร้อย"; \
		fi; \
	done

# -----------------------------------------------------------------
# 3. โครงสร้างพื้นฐาน Docker (Infrastructure)
# -----------------------------------------------------------------
# สตาร์ต PostgreSQL และ Firebase Emulator ใน Background
up:
	@echo "กำลังสตาร์ตคอนเทนเนอร์ PostgreSQL และ Firebase Emulator..."
	docker compose -f $(COMPOSE_FILE) up -d

# หยุดและนำคอนเทนเนอร์ลง
down:
	@echo "กำลังหยุดและลบคอนเทนเนอร์..."
	docker compose -f $(COMPOSE_FILE) down

# หยุดคอนเทนเนอร์ชั่วคราวโดยไม่ลบข้อมูลใน Volume
stop:
	@echo "กำลังหยุดการทำงานของคอนเทนเนอร์ (รักษาข้อมูลใน Volume)..."
	docker compose -f $(COMPOSE_FILE) stop

# รีสตาร์ตคอนเทนเนอร์ Docker ทั้งหมด
restart:
	@echo "กำลังรีสตาร์ตคอนเทนเนอร์ทั้งหมด..."
	docker compose -f $(COMPOSE_FILE) restart

# ดู Log สดของ Docker
logs:
	docker compose -f $(COMPOSE_FILE) logs -f

# ตรวจสอบสถานะของคอนเทนเนอร์
ps:
	docker compose -f $(COMPOSE_FILE) ps

# สตาร์ตเฉพาะ PostgreSQL Container
db-up:
	@echo "กำลังสตาร์ตคอนเทนเนอร์ PostgreSQL (พอร์ต $(DB_PORT))..."
	docker compose -f $(COMPOSE_FILE) up -d postgres

# หยุดเฉพาะ PostgreSQL Container
db-stop:
	@echo "กำลังหยุดคอนเทนเนอร์ PostgreSQL..."
	docker compose -f $(COMPOSE_FILE) stop postgres

# สตาร์ตเฉพาะ Firebase Auth Emulator Container
emulator-up:
	@echo "กำลังสตาร์ตคอนเทนเนอร์ Firebase Auth Emulator (พอร์ต $(EMULATOR_PORT))..."
	docker compose -f $(COMPOSE_FILE) up -d firebase-emulator

# -----------------------------------------------------------------
# 4. ฐานข้อมูลและ Migration (Database & Migrations)
# -----------------------------------------------------------------
# รอจนกว่า PostgreSQL จะพร้อมรับการเชื่อมต่อ
db-wait:
	@echo "กำลังรอให้ PostgreSQL เริ่มต้นพร้อมใช้งาน..."
	@until docker exec tan-erp-postgres pg_isready -U $(DB_USER) -d $(DB_NAME) >/dev/null 2>&1; do \
		sleep 1; \
	done
	@echo "PostgreSQL พร้อมใช้งานแล้ว"

# อัปเดต Schema ฐานข้อมูลด้วย EF Core Migration
db-migrate:
	@echo "กำลัง Apply EF Core Migrations เข้าสู่ PostgreSQL..."
	$(DOTNET) ef database update --project $(BACKEND_INFRA) --startup-project $(BACKEND_API) --connection $(DB_CONN)

# ย้อนกลับ Schema ฐานข้อมูลสู่จุดเริ่มต้น
db-rollback:
	@echo "กำลังย้อนกลับฐานข้อมูลสู่จุดเริ่มต้น (Migration 0)..."
	$(DOTNET) ef database update 0 --project $(BACKEND_INFRA) --startup-project $(BACKEND_API) --connection $(DB_CONN)

# ดูประวัติรายการ Migration ทั้งหมด
db-status:
	@echo "กำลังตรวจสอบประวัติและสถานะ EF Core Migrations..."
	$(DOTNET) ef migrations list --project $(BACKEND_INFRA) --startup-project $(BACKEND_API) --connection $(DB_CONN)

# เพิ่มข้อมูลผู้ใช้ทดสอบเข้า Firebase Emulator
db-seed:
	@echo "กำลัง Seed ข้อมูลผู้ใช้ทดสอบเข้าสู่ Firebase Auth Emulator..."
	FIREBASE_AUTH_EMULATOR_HOST="127.0.0.1:$(EMULATOR_PORT)" node scripts/seed-emulator-users.mjs

# Alias สำหรับ Seed ผู้ใช้ทดสอบ
seed seed-users: db-seed

# ล้างและตั้งค่าฐานข้อมูลใหม่ทั้งหมด (Rollback -> Migrate -> Seed)
db-reset fresh: db-rollback db-migrate db-seed
	@echo "รีเซ็ตฐานข้อมูลและข้อมูลผู้ใช้ทดสอบเรียบร้อยแล้ว"

# -----------------------------------------------------------------
# 5. สภาพแวดล้อมและรันระบบพัฒนา (Development Runtime)
# -----------------------------------------------------------------
# สร้าง .env.local จาก .env.example หากยังไม่มี
dev-env:
	@if [ ! -f $(FRONTEND_DIR)/.env.local ]; then \
		echo "กำลังสร้าง $(FRONTEND_DIR)/.env.local จาก .env.example..."; \
		cp $(FRONTEND_DIR)/.env.example $(FRONTEND_DIR)/.env.local; \
	fi

# เซ็ตอัปสภาพแวดล้อมทั้งหมดในคำสั่งเดียว
dev-setup: up db-wait db-seed db-migrate dev-env
	@echo ""
	@echo "ติดตั้งสภาพแวดล้อมพร้อมพัฒนาเรียบร้อยแล้ว! สามารถรัน 'make dev' ได้ทันที"

# รันเฉพาะ Backend API ในโหมด Test + SeedTestData
dev-backend:
	@echo "กำลังรัน Backend API บนพอร์ต $(BE_PORT)..."
	ASPNETCORE_ENVIRONMENT=Test \
	SeedTestData=true \
	FIREBASE_AUTH_EMULATOR_HOST=127.0.0.1:$(EMULATOR_PORT) \
	Firebase__ProjectId=tan-erp-test-only \
	ConnectionStrings__Database=$(DB_CONN) \
	$(DOTNET) run --no-launch-profile --project $(BACKEND_API) --urls http://localhost:$(BE_PORT)

# รันเฉพาะ Frontend Next.js Dev Server
dev-frontend: dev-env
	@echo "กำลังรัน Frontend Next.js บนพอร์ต $(FE_PORT)..."
	npm --prefix $(FRONTEND_DIR) run dev -- -p $(FE_PORT)

# รันทั้ง Backend และ Frontend พร้อมกัน (พร้อมเคลียร์พอร์ตและ Trap Ctrl+C)
dev: kill-ports up db-wait dev-env
	@echo "================================================================="
	@echo "กำลังสตาร์ต Backend (:$(BE_PORT)) และ Frontend (:$(FE_PORT)) พร้อมกัน"
	@echo "กด Ctrl+C เพื่อหยุดการทำงานของทั้งสองส่วนอย่างปลอดภัย"
	@echo "================================================================="
	@trap 'echo "\nกำลังปิดระบบ Frontend และ Backend..."; kill 0' SIGINT SIGTERM EXIT; \
	(make dev-backend) & \
	(make dev-frontend) & \
	wait

# ติดตั้ง dependencies ทั้งหมดในโปรเจกต์
deps install:
	@echo "กำลังติดตั้ง dependencies ทั้งหมด..."
	npm ci
	npm --prefix $(FRONTEND_DIR) ci --legacy-peer-deps
	$(DOTNET) restore $(BACKEND_SLN)
	@echo "ติดตั้ง dependencies สำเร็จเรียบร้อย"

# เปิดหน้าจอเว็บในเบราว์เซอร์
open:
	@echo "กำลังเปิดเบราว์เซอร์ไปที่ http://localhost:$(FE_PORT)/th/login ..."
	@open http://localhost:$(FE_PORT)/th/login 2>/dev/null || xdg-open http://localhost:$(FE_PORT)/th/login 2>/dev/null || true

# -----------------------------------------------------------------
# 6. สัญญาเชื่อมต่อ API (API Contracts)
# -----------------------------------------------------------------
# เจนเนอเรต TypeScript Client จาก OpenAPI Spec
api-gen:
	npm --prefix $(FRONTEND_DIR) run generate:api

# ตรวจสอบว่า TypeScript Client ตรงกับ OpenAPI Spec หรือไม่
api-check:
	npm --prefix $(FRONTEND_DIR) run check:api

# -----------------------------------------------------------------
# 7. การตรวจสอบและทดสอบคุณภาพ (Verification & Testing Gates)
# -----------------------------------------------------------------
# รัน Verification Gate ครบถ้วนตามมาตรฐาน Definition of Done (DOD)
verify: test-fixtures verify-backend verify-frontend
	@echo "================================================================="
	@echo "ผ่านการตรวจสอบ Verification Gates ทั้งหมดสมบูรณ์ 100%!"
	@echo "================================================================="

# ตรวจสอบฝั่ง Backend (Restore + Build + Test)
verify-backend:
	$(DOTNET) restore $(BACKEND_SLN)
	$(DOTNET) build $(BACKEND_SLN) --no-restore
	$(DOTNET) test $(BACKEND_SLN) --no-build

# ตรวจสอบฝั่ง Frontend (Contract + Lint + Typecheck + Test + Build)
verify-frontend:
	npm --prefix $(FRONTEND_DIR) run verify

# รัน Test ทุกชุด
test: test-fixtures test-backend test-frontend

# รันเฉพาะ Backend Tests (.NET)
test-backend:
	$(DOTNET) test $(BACKEND_SLN)

# รันเฉพาะ Frontend Tests (Vitest)
test-frontend:
	npm --prefix $(FRONTEND_DIR) run test

# รันเฉพาะ Playwright End-to-End Tests
test-e2e:
	PLAYWRIGHT_TEST_BASE_URL="http://localhost:$(FE_PORT)" npm --prefix $(FRONTEND_DIR) run test:e2e

# Alias สำหรับ Playwright E2E
e2e: test-e2e

# รันการทดสอบ Baseline Schema Fixtures
test-fixtures:
	node --test fixtures/survey-baseline.test.mjs

# ตรวจสอบ ESLint Frontend
lint:
	npm --prefix $(FRONTEND_DIR) run lint

# ตรวจสอบ TypeScript Types Frontend
typecheck:
	npm --prefix $(FRONTEND_DIR) run typecheck

# -----------------------------------------------------------------
# 8. การคอมไพล์และทำความสะอาด (Build & Maintenance)
# -----------------------------------------------------------------
# Build ทั้งโปรเจกต์
build:
	$(DOTNET) build $(BACKEND_SLN)
	npm --prefix $(FRONTEND_DIR) run build

# ล้างไฟล์ Build ชั่วคราว (bin, obj, .next)
clean:
	@echo "กำลังล้างไฟล์ Build และ Cache ชั่วคราว..."
	find backend -type d \( -name bin -o -name obj \) -exec rm -rf {} + 2>/dev/null || true
	rm -rf $(FRONTEND_DIR)/.next $(FRONTEND_DIR)/tsconfig.tsbuildinfo 2>/dev/null || true
	@echo "ล้างไฟล์เรียบร้อย"

# ล้างไฟล์ทั้งหมดรวมถึง node_modules และคอนเทนเนอร์
clean-all: clean down
	@echo "กำลังลบ node_modules และ dependencies ทั้งหมด..."
	rm -rf node_modules $(FRONTEND_DIR)/node_modules 2>/dev/null || true
	@echo "ล้างทั้งหมดสมบูรณ์"
