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
        up down stop logs ps db-up db-stop emulator-up \
        db-wait db-migrate db-rollback db-status db-seed \
        api-gen api-check \
        verify verify-backend verify-frontend \
        test test-backend test-frontend test-e2e test-fixtures \
        lint typecheck \
        build clean clean-all

# -----------------------------------------------------------------
# 1. Help
# -----------------------------------------------------------------
help:
	@echo ""
	@echo "================================================================="
	@echo "                   TAN-ERP MAKE COMMANDS                         "
	@echo "================================================================="
	@echo "Default Ports: FE=$(FE_PORT) | BE=$(BE_PORT) | EMULATOR=$(EMULATOR_PORT) | DB=$(DB_PORT)"
	@echo ""
	@echo "  Development:"
	@echo "    make dev            - Run FE (:$(FE_PORT)) & BE (:$(BE_PORT)) together (auto-kills dangling ports)"
	@echo "    make dev-setup      - Full setup (Docker -> Wait DB -> Seed -> Migrate -> .env)"
	@echo "    make dev-frontend   - Run Next.js frontend only on port $(FE_PORT)"
	@echo "    make dev-backend    - Run Backend API only on port $(BE_PORT)"
	@echo "    make dev-env        - Ensure frontend/.env.local exists"
	@echo ""
	@echo "  Port Management:"
	@echo "    make check-ports    - Check status of ports $(FE_PORT), $(BE_PORT), $(EMULATOR_PORT), $(DB_PORT)"
	@echo "    make kill-ports     - Kill dangling processes on ports $(FE_PORT) & $(BE_PORT)"
	@echo ""
	@echo "  Infrastructure (Docker):"
	@echo "    make up             - Start PostgreSQL and Firebase Emulator containers"
	@echo "    make down           - Stop and remove containers"
	@echo "    make stop           - Stop containers without removing volumes"
	@echo "    make logs           - Stream container logs"
	@echo "    make ps             - Show container status"
	@echo "    make db-up          - Start PostgreSQL container only (:$(DB_PORT))"
	@echo "    make db-stop        - Stop PostgreSQL container"
	@echo "    make emulator-up    - Start Firebase Auth Emulator container only (:$(EMULATOR_PORT))"
	@echo ""
	@echo "  Database & Migrations:"
	@echo "    make db-migrate     - Apply latest EF Core migrations to PostgreSQL"
	@echo "    make db-rollback    - Rollback database schema to initial state (0)"
	@echo "    make db-status      - List EF Core migration status"
	@echo "    make db-seed        - Seed test users into Firebase Auth Emulator"
	@echo ""
	@echo "  API Contracts:"
	@echo "    make api-gen        - Generate TypeScript API client from OpenAPI schema"
	@echo "    make api-check      - Verify that generated API client matches schema"
	@echo ""
	@echo "  Verification & Testing:"
	@echo "    make verify         - Run complete verification gates (DOD requirement)"
	@echo "    make test           - Run all tests (fixtures + backend + frontend)"
	@echo "    make test-backend   - Run .NET unit & integration tests"
	@echo "    make test-frontend  - Run frontend Vitest tests"
	@echo "    make test-e2e       - Run Playwright E2E tests against port $(FE_PORT)"
	@echo "    make test-fixtures  - Run baseline fixture schema tests"
	@echo "    make lint           - Run frontend ESLint"
	@echo "    make typecheck      - Run frontend TypeScript type checking"
	@echo ""
	@echo "  Build & Cleanup:"
	@echo "    make build          - Build both backend solution and frontend"
	@echo "    make clean          - Remove build/cache artifacts (bin, obj, .next)"
	@echo "    make clean-all      - Remove build artifacts, node_modules, and containers"
	@echo "================================================================="
	@echo ""

# -----------------------------------------------------------------
# 2. Port Management
# -----------------------------------------------------------------
check-ports:
	@echo "Checking active ports for tan-erp..."
	@for port in $(FE_PORT) $(BE_PORT) $(EMULATOR_PORT) $(DB_PORT); do \
		pid=$$(lsof -ti :$$port 2>/dev/null); \
		if [ -n "$$pid" ]; then \
			echo "  - Port $$port: IN USE (PID: $$pid)"; \
		else \
			echo "  - Port $$port: FREE"; \
		fi; \
	done

kill-ports:
	@echo "Checking and clearing ports $(FE_PORT) and $(BE_PORT)..."
	@for port in $(FE_PORT) $(BE_PORT); do \
		pid=$$(lsof -ti :$$port 2>/dev/null); \
		if [ -n "$$pid" ]; then \
			echo "Killing process on port $$port (PID: $$pid)..."; \
			kill -9 $$pid 2>/dev/null || true; \
		else \
			echo "Port $$port is clear."; \
		fi; \
	done

# -----------------------------------------------------------------
# 3. Infrastructure (Docker)
# -----------------------------------------------------------------
up:
	@echo "Starting PostgreSQL and Firebase Emulator containers..."
	docker compose -f $(COMPOSE_FILE) up -d

down:
	@echo "Stopping and removing containers..."
	docker compose -f $(COMPOSE_FILE) down

stop:
	@echo "Stopping containers (preserving volumes)..."
	docker compose -f $(COMPOSE_FILE) stop

logs:
	docker compose -f $(COMPOSE_FILE) logs -f

ps:
	docker compose -f $(COMPOSE_FILE) ps

db-up:
	@echo "Starting PostgreSQL container..."
	docker compose -f $(COMPOSE_FILE) up -d postgres

db-stop:
	@echo "Stopping PostgreSQL container..."
	docker compose -f $(COMPOSE_FILE) stop postgres

emulator-up:
	@echo "Starting Firebase Auth Emulator container..."
	docker compose -f $(COMPOSE_FILE) up -d firebase-emulator

# -----------------------------------------------------------------
# 4. Database & Migrations
# -----------------------------------------------------------------
db-wait:
	@echo "Waiting for PostgreSQL to be ready..."
	@until docker exec tan-erp-postgres pg_isready -U $(DB_USER) -d $(DB_NAME) >/dev/null 2>&1; do \
		sleep 1; \
	done
	@echo "PostgreSQL is ready."

db-migrate:
	@echo "Applying EF Core migrations to PostgreSQL..."
	$(DOTNET) ef database update --project $(BACKEND_INFRA) --startup-project $(BACKEND_API) --connection $(DB_CONN)

db-rollback:
	@echo "Rolling back database to initial state (0)..."
	$(DOTNET) ef database update 0 --project $(BACKEND_INFRA) --startup-project $(BACKEND_API) --connection $(DB_CONN)

db-status:
	@echo "Listing EF Core migrations..."
	$(DOTNET) ef migrations list --project $(BACKEND_INFRA) --startup-project $(BACKEND_API) --connection $(DB_CONN)

db-seed:
	@echo "Seeding test users into Firebase Auth Emulator..."
	FIREBASE_AUTH_EMULATOR_HOST="127.0.0.1:$(EMULATOR_PORT)" node scripts/seed-emulator-users.mjs

# -----------------------------------------------------------------
# 5. Development Runtime
# -----------------------------------------------------------------
dev-env:
	@if [ ! -f $(FRONTEND_DIR)/.env.local ]; then \
		echo "Creating $(FRONTEND_DIR)/.env.local from .env.example..."; \
		cp $(FRONTEND_DIR)/.env.example $(FRONTEND_DIR)/.env.local; \
	fi

dev-setup: up db-wait db-seed db-migrate dev-env
	@echo ""
	@echo "Development stack setup complete! You can now run 'make dev'."

dev-backend:
	@echo "Starting Backend API on port $(BE_PORT)..."
	ASPNETCORE_ENVIRONMENT=Test \
	SeedTestData=true \
	FIREBASE_AUTH_EMULATOR_HOST=127.0.0.1:$(EMULATOR_PORT) \
	Firebase__ProjectId=tan-erp-test-only \
	ConnectionStrings__Database=$(DB_CONN) \
	$(DOTNET) run --no-launch-profile --project $(BACKEND_API) --urls http://localhost:$(BE_PORT)

dev-frontend: dev-env
	@echo "Starting Frontend Next.js on port $(FE_PORT)..."
	npm --prefix $(FRONTEND_DIR) run dev -- -p $(FE_PORT)

dev: kill-ports up db-wait dev-env
	@echo "================================================================="
	@echo "Starting Backend (:$(BE_PORT)) and Frontend (:$(FE_PORT)) concurrently"
	@echo "Press Ctrl+C to stop both processes cleanly"
	@echo "================================================================="
	@trap 'echo "\nStopping dev processes..."; kill 0' SIGINT SIGTERM EXIT; \
	(make dev-backend) & \
	(make dev-frontend) & \
	wait

# -----------------------------------------------------------------
# 6. API Contracts
# -----------------------------------------------------------------
api-gen:
	npm --prefix $(FRONTEND_DIR) run generate:api

api-check:
	npm --prefix $(FRONTEND_DIR) run check:api

# -----------------------------------------------------------------
# 7. Testing & Verification Gates
# -----------------------------------------------------------------
verify: test-fixtures verify-backend verify-frontend
	@echo "================================================================="
	@echo "All verification gates passed successfully!"
	@echo "================================================================="

verify-backend:
	$(DOTNET) restore $(BACKEND_SLN)
	$(DOTNET) build $(BACKEND_SLN) --no-restore
	$(DOTNET) test $(BACKEND_SLN) --no-build

verify-frontend:
	npm --prefix $(FRONTEND_DIR) run verify

test: test-fixtures test-backend test-frontend

test-backend:
	$(DOTNET) test $(BACKEND_SLN)

test-frontend:
	npm --prefix $(FRONTEND_DIR) run test

test-e2e:
	PLAYWRIGHT_TEST_BASE_URL="http://localhost:$(FE_PORT)" npm --prefix $(FRONTEND_DIR) run test:e2e

test-fixtures:
	node --test fixtures/survey-baseline.test.mjs

lint:
	npm --prefix $(FRONTEND_DIR) run lint

typecheck:
	npm --prefix $(FRONTEND_DIR) run typecheck

# -----------------------------------------------------------------
# 8. Build & Maintenance
# -----------------------------------------------------------------
build:
	$(DOTNET) build $(BACKEND_SLN)
	npm --prefix $(FRONTEND_DIR) run build

clean:
	@echo "Cleaning build artifacts and cache..."
	find backend -type d \( -name bin -o -name obj \) -exec rm -rf {} + 2>/dev/null || true
	rm -rf $(FRONTEND_DIR)/.next $(FRONTEND_DIR)/tsconfig.tsbuildinfo 2>/dev/null || true
	@echo "Clean completed."

clean-all: clean down
	@echo "Cleaning node_modules and all downloaded dependencies..."
	rm -rf node_modules $(FRONTEND_DIR)/node_modules 2>/dev/null || true
	@echo "Full clean completed."
