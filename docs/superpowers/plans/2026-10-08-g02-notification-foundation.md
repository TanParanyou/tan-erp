# G-02 Notification Foundation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. Read `AGENTS.md`, `design.md`, `CONTEXT.md`, `.agents/skills/building-erp-apis/SKILL.md` and `.agents/skills/building-erp-lists/SKILL.md` before Task 1. Reply to the user in Thai; code, identifiers and code comments stay in English. macOS: use BSD `sed -i ''` (never GNU `sed -i`).

**Goal:** ให้ผู้อนุมัติรู้ว่ามีเอกสารรออนุมัติโดยไม่ต้องเปิดดูคิวเอง — **แจ้งเตือนในระบบ (in-app) เท่านั้น**: bell + badge + รายการ + mark read. รอบนี้ต่อ event "ส่งเอกสารเข้าสู่สถานะรออนุมัติ" 6 แหล่ง (Estimate, Cost record, Purchase Order, Change Order, MRP run, Role assignment maker–checker) และวางโครงให้ G-05/G-18/G-21 เพิ่ม notification type ได้โดยไม่แก้โค้ดกลาง. **ช่องทางอีเมลถูกเลื่อน (deferred) ตามคำสั่งผู้ใช้** — ไม่มี `IEmailSender`, email outbox หรือการจัดการภาษาอีเมลใน slice นี้.

**Architecture:** `Notification` เป็นแถวต่อผู้รับ (ต่อ user ต่อ organization) ที่สร้าง **ใน transaction เดียวกับการเปลี่ยนสถานะเอกสาร** โดย store ของแต่ละโมดูลเรียก port `INotificationPublisher` (Application) ซึ่ง implementation ใน Infrastructure *stage* แถวเข้า `AppDbContext` ตัวเดียวกัน (scoped) แล้วให้ `SaveChangesAsync`/transaction ของ business change commit พร้อมกัน — rollback ของ business change จึงไม่ทิ้ง notification ค้าง และ commit แล้วไม่มี event หาย; **ไม่ต้องใช้ outbox** (ไม่มีการส่งออกภายนอก ไม่มีอะไรต้อง retry/backoff/dead/requeue) และ Finance accounting outbox **ไม่ถูกแตะ**. **Notification type เป็น whitelist ในโค้ด** (`NotificationTypes` ใน Domain คู่กับ `NotificationTypeRegistry` ใน Application ที่ระบุ permission ปลายทาง, deep-link template, payload fields ที่อนุญาต; test บังคับให้ครบคู่กัน). ผู้รับ = ผู้ถือ permission อนุมัติของ type นั้นใน Organization/Branch ของเอกสาร ยกเว้นผู้ทำ (maker) — ยกเว้น Estimate ที่ route อนุมัติเลือกผู้ตรวจไว้แล้ว ให้ใช้ผู้ตรวจขั้นแรกของ route. payload ผ่าน **allowlist** (เฉพาะ `resourceId`, `documentNumber`, `actorDisplayName` ฯลฯ; ไม่มีตัวเลขต้นทุน/ราคา/margin/PII). ลิงก์ในรายการคำนวณตามสิทธิ์ **ปัจจุบัน** ของผู้อ่าน. Frontend: bell ใน header + dropdown + หน้า `/notifications`, TanStack Query polling (หยุดเมื่อแท็บถูกซ่อน), ใช้ `NotificationCenter` ที่มีอยู่เป็นฐาน (เปลี่ยนเป็น controlled component).

**Tech Stack:** .NET 9, EF Core 9 + Npgsql (writes/migrations; `ExecuteUpdateAsync` สำหรับ mark-read), PostgreSQL, xUnit + Testcontainers (`postgres:17-alpine`, ต้องมี Docker), Next.js App Router + TypeScript strict, TanStack Query 5, next-intl (`frontend/src/messages/{th,en}.json`), Vitest + Testing Library, openapi-typescript.

**ขอบเขตที่ไม่ทำ (Out of scope / Future):**
- **อีเมล (Future):** `IEmailSender`, provider จริง, email outbox (retry/backoff/dead/requeue), ภาษาอีเมลรายผู้ใช้/องค์กร. เมื่อกลับมาทำ ให้เปิด slice ใหม่และประเมิน ADR 0018 ข้อ "outbox" ใหม่ (ดูหัวข้อ Validation Questions)
- WebSocket/SSE, LINE, SMS
- **event ที่ต้องมี scheduler** (ใบประกันใกล้หมดอายุ, งานบริการเกิน SLA, membership ใกล้หมดอายุ) — ยังไม่มีงานตามเวลาใน repo จึงเป็น Future ของ G-18/G-21
- การตั้งค่าการแจ้งเตือนรายผู้ใช้, การแจ้งผลอนุมัติกลับไปหาผู้ทำ, การแจ้งผู้ตรวจ **ลำดับถัดไป** ของ Estimate เมื่อขั้นก่อนหน้าอนุมัติ (event ของ transition "approve")
- การลบ/archive notification (retention), full backend/frontend suite, CI, Playwright, `next build`; แต่ละ task รันเฉพาะ test ที่เกี่ยวข้อง + `dotnet build` / `npm run lint` / `npm run typecheck`.

**TEST_ONLY defaults (รอยืนยัน):** ไม่ลบ notification ที่อ่านแล้ว; polling ทุก 30 วินาที; จำนวนผู้รับสูงสุด 50 ต่อ event; ผู้ตรวจ Estimate ขั้นแรกเท่านั้นที่ได้รับแจ้ง.

**Validation Questions:**
1. Retention ของ notification ที่อ่านแล้วกี่วัน/เดือน?
2. ผู้ตรวจ Estimate ลำดับถัดไปควรได้รับแจ้งเมื่อขั้นก่อนหน้าอนุมัติหรือไม่?
3. (Email — Future) email provider ใด? ภาษาอีเมลตามผู้ใช้หรือองค์กร? (`User` ยังไม่มีฟิลด์ locale) 
4. (Future) ต้องมี LINE หรือไม่?

---

## Findings จากการสำรวจโค้ด (ที่แผนนี้อ้างอิง)

| หัวข้อ | สิ่งที่พบ (ตรวจแล้ว) | ผลต่อแผน |
| --- | --- | --- |
| Finance outbox | `AccountingOutboxMessage` (`backend/src/TanErp.Domain/Finance/BillingDocument.cs:171`) มี retry/backoff/dead/requeue ในตัว; dispatch ใน `FinanceStore.DispatchAsync` (`FinanceStore.cs`) | เมื่อไม่มีอีเมล ไม่มีอะไรใน G-02 ต้อง retry → **ไม่ extract, ไม่แตะ Finance** (ADR 0018) |
| Access model | `IRequestAccessResolver.ResolveAsync` ต้องมี permission key เสมอ (`RequestAccessResolver.cs`); Membership ผูก Organization + `BranchId?`; permission อยู่ที่ `RolePermission(Scope, ScopeId, BranchId)`; มี branch-scoped grant เฉพาะ `estimates.read/update/approve` | Notification ของตนเองไม่มี permission key → เพิ่ม `ResolveMembershipAsync` (ตรวจ membership active เท่านั้น); recipient resolver ใช้ org-scope grant (ตรงกับ resolver) และ Estimate ใช้ผู้ตรวจจาก route |
| Estimate submit | `EstimateStore.SubmitAsync` (`Infrastructure/Persistence/Estimates/EstimateStore.cs:648`): route อนุมัติสร้าง `EstimateApprovalStep` ต่อผู้ตรวจ (`FindIndependentReviewerAsync` :1091 ตัดผู้ส่ง/ผู้แก้ล่าสุดออกแล้ว), step ที่ active คือ `Sequence` ต่ำสุด | ผู้รับ = `steps[0].ReviewerUserId` (explicit recipients) |
| MRP | ไม่มี "submit": `MrpStore.CreateRunAsync` สร้าง run + recommendation สถานะ `Proposed` ที่รออนุมัติ (`mrp.approve`, ผู้สร้าง run อนุมัติเองไม่ได้) | event = สร้าง run ที่มี recommendation ≥ 1 |
| Role maker–checker | `RoleAssignmentRequest` สร้าง 2 จุดใน `IdentityAdministrationStore` (CreateUser :154, AssignRole :332); ผู้ตัดสินใช้ `roles.assign-approval`, ห้ามเป็นผู้ขอ และห้ามเป็นเจ้าของ membership | ผู้รับ = ผู้ถือ `roles.assign-approval` ยกเว้นผู้ขอและ user เป้าหมาย |
| FE bell | `frontend/src/components/layout/NotificationCenter.tsx` มีอยู่แล้วแต่ไม่มีใครใช้และไม่มี test (state ภายใน, `IconInfo`, key `common.notificationCenter`); ไม่มี `IconBell`; ไม่มี helper polling | reuse markup โดยเปลี่ยนเป็น controlled; เพิ่ม `IconBell`; polling ใช้ `refetchInterval` + `refetchIntervalInBackground:false` |
| API list shape | `{ items, pagination: { page, pageSize, totalCount, totalPages } }` | ใช้รูปเดียวกัน, pageSize ≤ 50 |
| Integration seed | `TestOnlyDataSeeder`: `Test Admin` ถือ `*.approve` ครบ (:150–:225), มี `TestEstimateReviewerUserId`, `TestCostReviewerMembershipId` | test สร้าง checker เพิ่มด้วย `Role`/`RolePermission` ใน test เอง |
| dotnet ef | `--project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations`; migration ล่าสุด `20261005155039_AddSharedAttachmentsAndSignatures` | ใช้คำสั่งเดียวกัน |

---

## File Structure

### Docs (Task 1, Task 12)

| ไฟล์ | หน้าที่ |
| --- | --- |
| `docs/adr/0018-in-app-notification-foundation.md` (ใหม่) | ADR: notification ใน transaction เดียวกัน, type registry ในโค้ด, **ไม่ใช้ outbox / ไม่แตะ Finance** |
| `docs/adr/README.md` (แก้) | ลิงก์ ADR 0018 |
| `docs/03-contracts/notification-api-contract.md` (ใหม่) | สัญญา API/ข้อมูล/error/type registry/payload hygiene/ขั้นตอนเพิ่ม type |
| `docs/03-contracts/permission-catalog.md` (แก้) | หมายเหตุ: ไม่มี permission key ใหม่ |
| `docs/03-contracts/error-contract.md` (แก้) | `NOTIFICATION_NOT_FOUND` |
| `docs/README.md`, `CONTEXT.md` (แก้) | แผนที่เอกสาร + ศัพท์ |
| `docs/05-engineering/notification-foundation-verification.md` (ใหม่) | หลักฐานการทดสอบ + ข้อจำกัด |
| `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/00-overview/implementation-roadmap.md` (แก้) | สถานะ G-02 |

### Backend

| ไฟล์ | หน้าที่ |
| --- | --- |
| `backend/src/TanErp.Domain/Notifications/NotificationValues.cs`, `Notification.cs` (ใหม่) | exception, `NotificationTypes`, entity + invariant + `MarkRead` |
| `backend/src/TanErp.Application/Common/Abstractions/IRequestAccessResolver.cs` (แก้) | `ResolveMembershipAsync` |
| `backend/src/TanErp.Application/Notifications/NotificationModels.cs`, `NotificationTypeRegistry.cs`, `NotificationPayloadRules.cs`, `NotificationPorts.cs`, `NotificationPublishPlanner.cs`, `NotificationEvents.cs`, `NotificationHandler.cs` (ใหม่) | registry, allowlist, planner (ตัด maker), event factories, ports, handler (own-only, deep link ตามสิทธิ์ปัจจุบัน) |
| `backend/src/TanErp.Infrastructure/Persistence/Configurations/NotificationConfigurations.cs` (ใหม่), `AppDbContext.cs` (แก้), `Migrations/*_AddNotifications*.cs` (generate) | ตาราง `notifications.notifications` |
| `backend/src/TanErp.Infrastructure/Persistence/RequestAccessResolver.cs` (แก้) | `ResolveMembershipAsync` |
| `backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationPublisher.cs`, `NotificationRecipientResolver.cs`, `NotificationStore.cs` (ใหม่) | stage ใน unit of work, ผู้ถือ permission, อ่าน/mark-read own-only |
| `backend/src/TanErp.Infrastructure/Persistence/{Estimates/EstimateStore,Items/CostRecordStore,Procurement/ProcurementStore,Projects/ProjectControlStore,Mrp/MrpStore,IdentityAccess/IdentityAdministrationStore}.cs` (แก้) | เรียก `INotificationPublisher` ใน transaction เดิม |
| `backend/src/TanErp.Api/Contracts/Notifications/NotificationContracts.cs`, `Controllers/NotificationsController.cs` (ใหม่) | endpoint บาง ๆ |
| `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `Resources/Errors.resx`, `Resources/Errors.en.resx`, `Program.cs` (แก้) | `NOTIFICATION_NOT_FOUND` th/en + DI |
| `contracts/openapi/tan-erp.v1.json` (regenerate) | snapshot |
| `backend/tests/TanErp.UnitTests/Notifications/*Tests.cs` (ใหม่) | domain, registry parity, payload hygiene, planner (ตัด maker), events, handler |
| `backend/tests/TanErp.IntegrationTests/Api/NotificationEndpointsTests.cs` (ใหม่) | publish ใน transaction เดียวกัน, rollback = ไม่มี notification, own-only, cross-org 404, scope ผู้รับ |

### Frontend

| ไฟล์ | หน้าที่ |
| --- | --- |
| `frontend/src/generated/api/tan-erp.v1.ts` (regenerate), `lib/api/api-client.ts` (แก้) | types + methods |
| `frontend/src/lib/notifications/notification-types.ts` (+ `.test.ts`) (ใหม่) | whitelist ฝั่ง UI + map type → message key + guard |
| `frontend/src/hooks/useNotifications.ts` (+ `.test.tsx`) (ใหม่) | list/unread-count (polling) + mark read/all |
| `frontend/src/components/common/Icons.tsx` (แก้) | `IconBell` |
| `frontend/src/components/layout/NotificationCenter.tsx` (+ `.test.tsx`), `NotificationBell.tsx` (+ `.test.tsx`), `erp-shell.tsx` (+ `.test.tsx`) | dropdown (controlled), container, วางใน header |
| `frontend/src/features/notifications/components/notification-list-page.tsx` (+ `.test.tsx`), `app/[locale]/(erp)/notifications/page.tsx` (ใหม่) | หน้ารายการเต็ม |
| `frontend/src/messages/th.json`, `en.json` (แก้) | namespace `notifications` + ปรับ `common.notificationCenter` |

---

## Task 1: ADR, สัญญา API, permission note, error code, ศัพท์ (docs)

**Files:**
- Create: `docs/adr/0018-in-app-notification-foundation.md`
- Create: `docs/03-contracts/notification-api-contract.md`
- Modify: `docs/adr/README.md`, `docs/03-contracts/permission-catalog.md`, `docs/03-contracts/error-contract.md`, `docs/README.md`, `CONTEXT.md`

ขั้นนี้เป็นเอกสาร จึงไม่มี test ที่รันได้ — ตรวจด้วย link check ใน Step 6.

- [ ] **Step 1: สร้าง ADR 0018**

เขียน `docs/adr/0018-in-app-notification-foundation.md` (ภาษาไทย รูปแบบเดียวกับ ADR 0017):

```markdown
---
status: accepted
---

# Notification Foundation (in-app): สร้างใน Transaction เดียวกับ Business Change โดยไม่ใช้ Outbox

หลาย slice ต้องแจ้งผู้ใช้ (G-02 รออนุมัติ, G-05 OTP, G-18 SLA, G-21 membership หมดอายุ). รอบนี้ทำเฉพาะการแจ้งเตือนในระบบ; อีเมลถูกเลื่อนตามคำสั่งผู้ใช้.

**การตัดสินใจ 1 — แจ้งเตือนต้องไม่หายและไม่ค้างเกิน:** `INotificationPublisher` (Application) ถูกเรียกจาก store ของโมดูลที่เปลี่ยนสถานะ และ implementation *stage* `Notification` เข้า `AppDbContext` ตัวเดียวกัน (scoped) โดยไม่ `SaveChanges` เอง — transaction/`SaveChanges` ของ business change เป็นคน commit. Business change rollback → ไม่มี notification; commit → มี notification แน่นอน. ข้อเสียที่ยอมรับ: publisher ที่ผิดพลาด (payload ผิด type) ทำให้ business action ล้มเหลว จึงล็อกด้วย unit test ของ `NotificationEvents` ทุกแหล่ง และ publisher ตรวจ dedupe key ก่อน stage เพื่อไม่ให้ unique index ของ notification ทำให้ transaction ของเอกสารล้ม.

**การตัดสินใจ 2 — Notification type อยู่ในโค้ด:** `NotificationTypes` (Domain) + `NotificationTypeRegistry` (Application: permission ปลายทาง, deep-link template, payload fields). payload ใช้ allowlist ต่อ type เพื่อไม่ให้ตัวเลขต้นทุน/ราคา/margin หรือ PII หลุด; ลิงก์ในรายการคำนวณตามสิทธิ์ **ปัจจุบัน** ของผู้อ่าน (ไม่มีสิทธิ์แล้ว → ไม่มีลิงก์).

**การตัดสินใจ 3 — ไม่ใช้ outbox และไม่แตะ Finance:** แผนหลักเสนอให้ดึง retry/backoff/dead/requeue ของ Finance accounting outbox ขึ้นเป็นของกลาง (`Common/Outbox`) เพื่อใช้กับอีเมล. เมื่ออีเมลถูกเลื่อน การแจ้งเตือนในระบบเป็นแถวในฐานข้อมูลเดียวกัน ไม่มีการส่งออกภายนอก จึงไม่มีสิ่งใดต้อง retry; การ extract entity ของ Finance ที่ใช้งานจริงโดยไม่มีผู้ใช้ตัวที่สองจะเพิ่มความเสี่ยง (EF map property ของ base class, schema ของ Finance) โดยไม่ได้ประโยชน์ (YAGNI). **จึงยกเลิกงาน extract; Finance ไม่ถูกแก้.** เมื่อมี slice อีเมล (หรือช่องทางส่งออกอื่น) ให้ประเมินใหม่: ถ้ามีช่องทางที่สอง ให้ extract state machine ขึ้น `Domain/Common/Outbox` (ไม่ใช่ `Application/Common` เพราะเป็นพฤติกรรมของ entity และ Domain ห้ามพึ่ง Application) โดยป้องกันด้วย `FinanceDomainTests.Outbox_*`, `FinanceEndpointsTests.AccountingOutbox_*` และ `dotnet ef migrations has-pending-model-changes`.

ทางเลือกที่ไม่เลือก: สร้าง outbox ตารางใหม่เผื่ออนาคต (YAGNI; ผูก schema กับ provider ที่ยังไม่เลือก); เขียน notification หลัง commit ด้วย background job (event หายได้เมื่อ process ตายระหว่างสอง commit).
```

- [ ] **Step 2: เพิ่มลิงก์ใน ADR README**

ต่อท้ายรายการใน `docs/adr/README.md`:

```markdown
- [0018 — Notification Foundation (in-app) โดยไม่ใช้ Outbox](0018-in-app-notification-foundation.md)
```

- [ ] **Step 3: สร้างสัญญา API**

เขียน `docs/03-contracts/notification-api-contract.md` (ภาษาไทย, ตัวอย่างโค้ดภาษาอังกฤษ) ให้มีหัวข้อครบดังนี้:

```markdown
# Notification API Contract (ข้อตกลง API การแจ้งเตือนในระบบ)

**สถานะ:** Draft → Implemented เมื่อ G-02 เสร็จ (ดู [Verification](../05-engineering/notification-foundation-verification.md)). กฎเป็นค่าเริ่มต้น TEST_ONLY รอ Security/Operations ยืนยัน. ตัดสินใจเชิงสถาปัตยกรรมใน [ADR 0018](../adr/0018-in-app-notification-foundation.md). **รอบนี้ไม่มีช่องทางอีเมล** (Future).

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| เจ้าของ | แถวต่อผู้รับ `(organizationId, recipientUserId)`; ผู้ใช้เห็น/แก้ได้เฉพาะของตนใน Organization ของ Membership ที่ส่งมา (`X-Membership-Id`) |
| Permission | **ไม่มี permission key ใหม่** — ต้อง authenticated + Membership active; การเข้าถึง = เป็นเจ้าของแถว. ไม่มี create endpoint (สร้างได้จาก store ของโมดูลต้นเหตุเท่านั้น) |
| ลำดับตรวจ | authentication/membership (401/400/403) → ค้นแถวด้วย `(org, user, id)` พร้อมกัน → ไม่พบ/เป็นของคนอื่น/ข้าม org = **404 เหมือนกัน** (`NOTIFICATION_NOT_FOUND`) |
| Mark read | idempotent โดยธรรมชาติ (อ่านแล้วซ้ำ = 200 คืนแถวเดิม, `readAtUtc` ไม่เปลี่ยน) จึง **ไม่ต้องมี Idempotency-Key/If-Match**; ไม่เขียน audit event |
| Payload | `Record<string,string>` ผ่าน allowlist ต่อ type; ห้ามมีตัวเลขต้นทุน/ราคา/margin/จำนวนเงิน/อีเมล/โทรศัพท์/ที่อยู่ (ตรวจทั้ง allowlist และชื่อ key); ค่า ≤ 200 ตัวอักษร |
| Deep link | `deepLink` คำนวณตอนอ่านจาก template ของ type + payload และ **ตามสิทธิ์ปัจจุบัน** ของผู้อ่าน (ไม่มี target permission แล้ว → `null`); path ไม่มี locale prefix |
| Dedupe | `(organizationId, recipientUserId, dedupeKey)` ไม่ซ้ำ; `dedupeKey = "{type}:{transitionId}"` โดย transitionId เปลี่ยนทุกครั้งที่เอกสารเข้าสถานะรออนุมัติ (ส่งกลับแก้แล้วส่งใหม่ = แจ้งใหม่) |
| ผู้รับ | ผู้ถือ permission อนุมัติของ type ใน Organization (+ Branch ของเอกสารถ้ามี: membership ระดับองค์กรหรือสาขาเดียวกัน) ที่ membership/user/role/permission active และอยู่ในช่วงเวลา; **ตัด maker** ตามกฎ maker–checker ของแต่ละเอกสาร; สูงสุด 50 คนต่อ event. Estimate ใช้ผู้ตรวจขั้นแรกของ route |
| Atomic | สร้างใน transaction เดียวกับ business change (ADR 0018) |
| Polling | UI poll ทุก 30 วินาทีและหยุดเมื่อแท็บถูกซ่อน; ไม่มี WebSocket/SSE |

## Notification type registry (รอบนี้)

| Type | Permission ปลายทาง | Deep link template | Payload fields | แหล่ง event |
| --- | --- | --- | --- | --- |
| `estimate.approval-requested` | `estimates.approve` | `/estimates/review-queue` | `resourceId`, `documentNumber`, `actorDisplayName` | ส่งใบประมาณราคาเพื่ออนุมัติ |
| `cost-record.approval-requested` | `cost-records.approve` | `/item-master/cost-reviews` | `resourceId`, `documentNumber` (รหัสสินค้า), `actorDisplayName` | ส่ง Cost record |
| `purchase-order.approval-requested` | `purchase-orders.approve` | `/procurement/purchase-orders/{resourceId}` | `resourceId`, `documentNumber`, `actorDisplayName` | ส่ง PO |
| `change-order.approval-requested` | `projects.change-orders.approve` | `/projects/{parentId}` | `resourceId`, `parentId` (projectId), `documentNumber`, `actorDisplayName` | ส่ง Change Order |
| `mrp-run.approval-requested` | `mrp.approve` | `/production/mrp/{resourceId}` | `resourceId`, `documentNumber`, `actorDisplayName` | สร้าง MRP run ที่มีข้อเสนอแนะ |
| `role-assignment.approval-requested` | `roles.assign-approval` | `/settings/role-requests` | `resourceId`, `subjectDisplayName`, `roleName`, `actorDisplayName` | คำขอมอบ Role ที่ต้อง maker–checker |

Future (ไม่อยู่ใน G-02): `warranty.expiring`, `service-request.sla-overdue`, `membership.expiring` (ต้องมี scheduler); ช่องทางอีเมล (ต้องเลือก provider, ภาษา, และประเมิน outbox ใหม่).

## Endpoints (ต้อง `Authorization` + `X-Membership-Id`)

| Action | Method/Path | ผลลัพธ์ |
| --- | --- | --- |
| รายการ | `GET /api/v1/notifications?unreadOnly=false&page=1&pageSize=20` | `{ items: NotificationResponse[], pagination: { page, pageSize, totalCount, totalPages } }` เรียง `createdAtUtc desc, id desc`; `pageSize` 1–50 (นอกช่วงถูก clamp) |
| จำนวนที่ยังไม่อ่าน | `GET /api/v1/notifications/unread-count` | `{ unreadCount }` |
| อ่านหนึ่งรายการ | `POST /api/v1/notifications/{id}/read` | `200 NotificationResponse` หรือ `404 NOTIFICATION_NOT_FOUND` |
| อ่านทั้งหมด | `POST /api/v1/notifications/read-all` | `200 { updatedCount }` (เฉพาะของผู้เรียกใน Organization ปัจจุบัน) |

`NotificationResponse { id, type, payload, deepLink, createdAtUtc, readAtUtc }` — ไม่ส่ง recipient/organization id ดิบ.

## Errors

`401 AUTHENTICATION_REQUIRED`, `400 MEMBERSHIP_CONTEXT_REQUIRED`, `403 ACTIVE_MEMBERSHIP_REQUIRED`, `404 NOTIFICATION_NOT_FOUND` (code ใหม่ตัวเดียว; ข้อความ th/en ใน `Errors.resx`/`Errors.en.resx`). ความผิดพลาดภายในของ publisher (`NOTIFICATION_TYPE_INVALID`, `NOTIFICATION_PAYLOAD_INVALID`, `NOTIFICATION_FIELD_INVALID`) เป็นข้อบกพร่องของโปรแกรม ไม่ใช่ API error: โยน exception ให้ business transaction ล้มและถูกล็อกด้วย unit test.

## Data (migration `AddNotifications`)

schema `notifications`, ตาราง `notifications` (id, organization_id, recipient_user_id, type, payload jsonb, dedupe_key, created_at_utc, read_at_utc; unique `ux_notifications_recipient_dedupe (organization_id, recipient_user_id, dedupe_key)`; index `ix_notifications_recipient_created`; partial index `ix_notifications_unread WHERE read_at_utc IS NULL`).

## ขั้นตอนเพิ่ม notification type ใหม่ (สำหรับ G-05/G-18/G-21)

1. เพิ่ม constant และใส่ใน `Registered` ของ `NotificationTypes` (`backend/src/TanErp.Domain/Notifications/NotificationValues.cs`).
2. เพิ่ม `NotificationTypeDescriptor` ใน `NotificationTypeRegistry`.
3. เพิ่ม factory ใน `NotificationEvents`.
4. เพิ่มค่าใน `NOTIFICATION_TYPES` และข้อความ th/en ใน `frontend/src/lib/notifications/notification-types.ts` + `messages/*.json`.
5. เรียก `INotificationPublisher.PublishAsync` ใน store ที่เปลี่ยนสถานะ ก่อน `SaveChangesAsync`.
test `NotificationRegistryTests`/`NotificationEventsTests` ล้มถ้าลืมข้อ 1–3; `notification-types.test.ts` ล้มถ้าลืมข้อ 4.

## Threat notes

payload เป็น allowlist ไม่ใช่ denylist; ผู้อ่านอ่านได้เฉพาะแถวของตน (404 ไม่แยกว่าไม่มีหรือเป็นของคนอื่น); deep link ใช้เฉพาะค่า GUID ที่ผ่านการตรวจรูปแบบ.

## Validation Questions

retention ของ notification ที่อ่านแล้ว; ผู้ตรวจ Estimate ลำดับถัดไปควรได้รับแจ้งเมื่อขั้นก่อนหน้าอนุมัติหรือไม่; (Future) email provider, ภาษาอีเมลตามผู้ใช้หรือองค์กร, ต้องมี LINE หรือไม่.
```

- [ ] **Step 4: permission catalog + error contract**

1. ต่อท้ายตาราง Access ใน `docs/03-contracts/permission-catalog.md` เพิ่มหนึ่งย่อหน้า: `**Notification (G-02):** ไม่มี permission key ใหม่ — ผู้ใช้ที่มี Membership active อ่าน/mark-read การแจ้งเตือนของตนเองได้เท่านั้น. ผู้รับถูกเลือกด้วย permission อนุมัติเดิมของเอกสาร (ตารางใน [Notification API Contract](notification-api-contract.md)) และลิงก์ในรายการจะแสดงเฉพาะเมื่อผู้อ่านยังถือ permission นั้นอยู่.`
2. ใน `docs/03-contracts/error-contract.md` เพิ่มท้ายส่วน Shared Attachment & Signature:

```markdown
## Notification Error Codes

| Code | HTTP | ความหมาย/การกู้คืน |
| --- | ---: | --- |
| `NOTIFICATION_NOT_FOUND` | 404 | ไม่พบการแจ้งเตือน หรือไม่ใช่ของผู้เรียก/ต่าง Organization (ไม่แยกสาเหตุเพื่อไม่เปิดเผยการมีอยู่) |
```

- [ ] **Step 5: README, ศัพท์**

1. `docs/README.md`: ใต้แถว `API ไฟล์แนบและลายเซ็นกลาง (Shared Attachment & Signature)` เพิ่ม `| API การแจ้งเตือนในระบบ (Notification) | [Notification API Contract](03-contracts/notification-api-contract.md) |`.
2. `CONTEXT.md` ต่อท้ายไฟล์:

```markdown

**Notification (การแจ้งเตือน)**:
ข้อความในระบบถึงผู้ใช้หนึ่งคนใน Organization หนึ่ง สร้างพร้อมการเปลี่ยนสถานะเอกสารต้นเหตุ มีประเภทที่กำหนดในโค้ด ผู้รับอ่านได้เฉพาะของตน
_Avoid_: Alert, Message, Push

**Notification Type (ประเภทการแจ้งเตือน)**:
รหัสประเภทที่ลงทะเบียนในโค้ด ระบุ permission ปลายทาง ลิงก์ และ field ที่อนุญาตใน payload (เช่น "ใบสั่งซื้อรออนุมัติ")
_Avoid_: Event name, Topic
```

- [ ] **Step 6: ตรวจลิงก์แล้ว commit**

Run: `cd /Users/syaco/Documents/development/tan-erp && grep -n "notification-api-contract\|0018-in-app" docs/README.md docs/adr/README.md docs/03-contracts/permission-catalog.md && ls docs/adr/0018-in-app-notification-foundation.md docs/03-contracts/notification-api-contract.md`
Expected: บรรทัดที่เพิ่มปรากฏและไฟล์ทั้งสองมีอยู่.

```bash
git add docs/adr/0018-in-app-notification-foundation.md docs/adr/README.md docs/03-contracts/notification-api-contract.md docs/03-contracts/permission-catalog.md docs/03-contracts/error-contract.md docs/README.md CONTEXT.md
git commit -F - <<'EOF'
docs(notifications): add in-app notification contract and ADR 0018

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 2: Domain — `Notification` และ invariant

**Files:**
- Create: `backend/src/TanErp.Domain/Notifications/NotificationValues.cs`
- Create: `backend/src/TanErp.Domain/Notifications/Notification.cs`
- Test: `backend/tests/TanErp.UnitTests/Notifications/NotificationDomainTests.cs`

- [ ] **Step 1: เขียน test ที่ล้มก่อน**

`backend/tests/TanErp.UnitTests/Notifications/NotificationDomainTests.cs`:

```csharp
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationDomainTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 8, 3, 0, 0, TimeSpan.Zero);
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Recipient = Guid.NewGuid();
    private const string Payload = "{\"documentNumber\":\"PO-2026-0001\"}";

    private static Notification Create(string type = NotificationTypes.PurchaseOrderApprovalRequested, string payload = Payload, string key = "purchase-order.approval-requested:abc") =>
        new(Guid.NewGuid(), Org, Recipient, type, payload, key, Now);

    [Fact]
    public void RegisteredTypes_AreExactlyTheSixApprovalRequests_AndMatchTheFrontendList()
    {
        // Keep in step with NOTIFICATION_TYPES in frontend/src/lib/notifications/notification-types.ts (checked there too).
        var expected = new[]
        {
            "change-order.approval-requested",
            "cost-record.approval-requested",
            "estimate.approval-requested",
            "mrp-run.approval-requested",
            "purchase-order.approval-requested",
            "role-assignment.approval-requested"
        };
        Assert.Equal(expected, NotificationTypes.All.OrderBy(t => t, StringComparer.Ordinal).ToArray());
    }

    [Fact]
    public void Normalize_AcceptsOnlyRegisteredTypes()
    {
        Assert.Equal("estimate.approval-requested", NotificationTypes.Normalize("  Estimate.Approval-Requested "));
        Assert.Null(NotificationTypes.Normalize("estimate.deleted"));
        Assert.Null(NotificationTypes.Normalize(null));
        Assert.False(NotificationTypes.IsRegistered("customer"));
    }

    [Fact]
    public void NewNotification_IsUnreadAndKeepsItsFields()
    {
        var notification = Create();
        Assert.Equal((Org, Recipient, NotificationTypes.PurchaseOrderApprovalRequested), (notification.OrganizationId, notification.RecipientUserId, notification.Type));
        Assert.Equal(Now, notification.CreatedAtUtc);
        Assert.False(notification.IsRead);
        Assert.Null(notification.ReadAtUtc);
    }

    [Fact]
    public void UnregisteredType_IsRejected()
    {
        Assert.Equal("NOTIFICATION_TYPE_INVALID", Assert.Throws<NotificationDomainException>(() => Create(type: "estimate.deleted")).Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not json")]
    [InlineData("[1,2]")]
    [InlineData("\"text\"")]
    public void Payload_MustBeAJsonObject(string payload)
    {
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", Assert.Throws<NotificationDomainException>(() => Create(payload: payload)).Code);
    }

    [Fact]
    public void Payload_IsLimitedInLength()
    {
        var tooLong = "{\"documentNumber\":\"" + new string('x', Notification.MaxPayloadLength) + "\"}";
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", Assert.Throws<NotificationDomainException>(() => Create(payload: tooLong)).Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DedupeKey_IsRequired(string key)
    {
        Assert.Equal("NOTIFICATION_FIELD_INVALID", Assert.Throws<NotificationDomainException>(() => Create(key: key)).Code);
    }

    [Fact]
    public void OrganizationAndRecipient_AreRequired()
    {
        Assert.Equal("NOTIFICATION_FIELD_INVALID", Assert.Throws<NotificationDomainException>(() =>
            new Notification(Guid.NewGuid(), Guid.Empty, Recipient, NotificationTypes.EstimateApprovalRequested, Payload, "k", Now)).Code);
        Assert.Equal("NOTIFICATION_FIELD_INVALID", Assert.Throws<NotificationDomainException>(() =>
            new Notification(Guid.NewGuid(), Org, Guid.Empty, NotificationTypes.EstimateApprovalRequested, Payload, "k", Now)).Code);
    }

    [Fact]
    public void MarkRead_IsIdempotent_AndKeepsTheFirstReadTime()
    {
        var notification = Create();
        Assert.True(notification.MarkRead(Now.AddMinutes(5)));
        Assert.False(notification.MarkRead(Now.AddMinutes(10)));
        Assert.Equal(Now.AddMinutes(5), notification.ReadAtUtc);
        Assert.True(notification.IsRead);
    }
}
```

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~NotificationDomainTests"`
Expected: FAIL (compile) — namespace `TanErp.Domain.Notifications` ไม่มี.

- [ ] **Step 2: implement**

`backend/src/TanErp.Domain/Notifications/NotificationValues.cs`:

```csharp
namespace TanErp.Domain.Notifications;

public class NotificationDomainException : Exception
{
    public string Code { get; }

    public NotificationDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

/// <summary>
/// Code-defined whitelist of notification types. A new type also needs a NotificationTypeRegistry descriptor,
/// and a NotificationEvents factory (enforced by tests).
/// </summary>
public static class NotificationTypes
{
    public const string EstimateApprovalRequested = "estimate.approval-requested";
    public const string CostRecordApprovalRequested = "cost-record.approval-requested";
    public const string PurchaseOrderApprovalRequested = "purchase-order.approval-requested";
    public const string ChangeOrderApprovalRequested = "change-order.approval-requested";
    public const string MrpRunApprovalRequested = "mrp-run.approval-requested";
    public const string RoleAssignmentApprovalRequested = "role-assignment.approval-requested";

    private static readonly HashSet<string> Registered = new(StringComparer.Ordinal)
    {
        EstimateApprovalRequested,
        CostRecordApprovalRequested,
        PurchaseOrderApprovalRequested,
        ChangeOrderApprovalRequested,
        MrpRunApprovalRequested,
        RoleAssignmentApprovalRequested
    };

    public static IReadOnlyCollection<string> All => Registered;

    public static bool IsRegistered(string? type) => type is not null && Registered.Contains(type);

    /// <summary>Returns the canonical (trimmed, lower-case) type, or null when it is not registered.</summary>
    public static string? Normalize(string? type)
    {
        var candidate = type?.Trim().ToLowerInvariant();
        return IsRegistered(candidate) ? candidate : null;
    }
}
```

`backend/src/TanErp.Domain/Notifications/Notification.cs`:

```csharp
using System.Text.Json;
using TanErp.Domain.Common;

namespace TanErp.Domain.Notifications;

/// <summary>
/// One in-app message to one user in one organization. Created in the same transaction as the business change that caused it.
/// The payload is a flat JSON object of display-safe strings (document number, display names); the API layer never sends figures.
/// </summary>
public class Notification : Entity
{
    public const int MaxPayloadLength = 2000;
    public const int MaxDedupeKeyLength = 160;

    public Guid OrganizationId { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string PayloadJson { get; private set; } = "{}";
    public string DedupeKey { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? ReadAtUtc { get; private set; }

    public bool IsRead => ReadAtUtc.HasValue;

    protected Notification() { }

    public Notification(Guid id, Guid organizationId, Guid recipientUserId, string type, string payloadJson, string dedupeKey, DateTimeOffset createdAtUtc) : base(id)
    {
        if (organizationId == Guid.Empty || recipientUserId == Guid.Empty)
        {
            throw new NotificationDomainException("NOTIFICATION_FIELD_INVALID", "An organization and a recipient are required.");
        }

        var normalizedType = NotificationTypes.Normalize(type)
            ?? throw new NotificationDomainException("NOTIFICATION_TYPE_INVALID", $"Notification type '{type}' is not registered.");

        var key = dedupeKey?.Trim() ?? string.Empty;
        if (key.Length is 0 or > MaxDedupeKeyLength)
        {
            throw new NotificationDomainException("NOTIFICATION_FIELD_INVALID", $"A dedupe key of 1-{MaxDedupeKeyLength} characters is required.");
        }

        if (!IsJsonObject(payloadJson) || payloadJson.Length > MaxPayloadLength)
        {
            throw new NotificationDomainException("NOTIFICATION_PAYLOAD_INVALID", $"The payload must be a JSON object of at most {MaxPayloadLength} characters.");
        }

        OrganizationId = organizationId;
        RecipientUserId = recipientUserId;
        Type = normalizedType;
        PayloadJson = payloadJson;
        DedupeKey = key;
        CreatedAtUtc = createdAtUtc.ToUniversalTime();
    }

    /// <summary>Marks the notification as read. Returns false (and keeps the first read time) when it was already read.</summary>
    public bool MarkRead(DateTimeOffset now)
    {
        if (ReadAtUtc.HasValue) return false;
        ReadAtUtc = now.ToUniversalTime();
        return true;
    }

    private static bool IsJsonObject(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;
        try
        {
            using var document = JsonDocument.Parse(json);
            return document.RootElement.ValueKind == JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
```

- [ ] **Step 3: รัน test ให้ผ่าน**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~NotificationDomainTests"`
Expected: PASS ทั้งหมด.

- [ ] **Step 4: commit**

```bash
git add backend/src/TanErp.Domain/Notifications backend/tests/TanErp.UnitTests/Notifications/NotificationDomainTests.cs
git commit -F - <<'EOF'
feat(notifications): add notification domain entity

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 3: Application — registry, payload rules, publish planner, event factories, ports

**Files:**
- Create: `backend/src/TanErp.Application/Notifications/NotificationModels.cs`
- Create: `backend/src/TanErp.Application/Notifications/NotificationTypeRegistry.cs`
- Create: `backend/src/TanErp.Application/Notifications/NotificationPayloadRules.cs`
- Create: `backend/src/TanErp.Application/Notifications/NotificationPorts.cs`
- Create: `backend/src/TanErp.Application/Notifications/NotificationPublishPlanner.cs`
- Create: `backend/src/TanErp.Application/Notifications/NotificationEvents.cs`
- Test: `backend/tests/TanErp.UnitTests/Notifications/NotificationRegistryTests.cs`, `NotificationPayloadRulesTests.cs`, `NotificationPublishPlannerTests.cs`, `NotificationEventsTests.cs`

เป็นตรรกะบริสุทธิ์ทั้งหมด (ไม่มี I/O) จึงทดสอบ "ตัด maker", "payload hygiene" และ "registry ครบคู่" ได้ใน unit test โดยไม่ต้องมี DB. การกรอง "นอก scope" ต้องใช้ DB → ทดสอบใน Task 10.

- [ ] **Step 1: เขียน test ที่ล้มก่อน**

`backend/tests/TanErp.UnitTests/Notifications/NotificationRegistryTests.cs`:

```csharp
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationRegistryTests
{
    [Fact]
    public void Registry_CoversExactlyTheDomainWhitelist()
    {
        Assert.True(NotificationTypeRegistry.Types.ToHashSet().SetEquals(NotificationTypes.All));
    }

    [Fact]
    public void Find_IsExactMatchOnly()
    {
        Assert.NotNull(NotificationTypeRegistry.Find("purchase-order.approval-requested"));
        Assert.Null(NotificationTypeRegistry.Find("Purchase-Order.Approval-Requested"));
        Assert.Null(NotificationTypeRegistry.Find(null));
        Assert.Null(NotificationTypeRegistry.Find("customer.created"));
    }

    [Fact]
    public void EveryDescriptor_DeclaresItsContract()
    {
        foreach (var type in NotificationTypeRegistry.Types)
        {
            var d = NotificationTypeRegistry.Find(type)!;
            Assert.False(string.IsNullOrWhiteSpace(d.TargetPermission), type);
            Assert.StartsWith("/", d.DeepLinkTemplate);
            Assert.Contains(NotificationFields.ActorDisplayName, d.RequiredFields);
            Assert.Contains(d.ReferenceField, d.RequiredFields);
            Assert.Contains(NotificationFields.ResourceId, d.RequiredFields);
        }
    }

    [Fact]
    public void DeepLinkTokens_AreAlwaysDeclaredPayloadFields()
    {
        foreach (var type in NotificationTypeRegistry.Types)
        {
            var d = NotificationTypeRegistry.Find(type)!;
            foreach (var token in NotificationTypeRegistry.TemplateTokens(d.DeepLinkTemplate))
            {
                Assert.Contains(token, d.RequiredFields);
            }
        }
    }

    [Fact]
    public void NoRequiredFieldName_LooksLikeAFigureOrContactDetail()
    {
        foreach (var type in NotificationTypeRegistry.Types)
        {
            foreach (var field in NotificationTypeRegistry.Find(type)!.RequiredFields)
            {
                Assert.False(NotificationPayloadRules.IsForbiddenKey(field), $"{type}:{field}");
            }
        }
    }

    [Fact]
    public void TargetPermissions_MatchTheApprovalPermissionsOfEachSource()
    {
        Assert.Equal("estimates.approve", NotificationTypeRegistry.Find(NotificationTypes.EstimateApprovalRequested)!.TargetPermission);
        Assert.Equal("cost-records.approve", NotificationTypeRegistry.Find(NotificationTypes.CostRecordApprovalRequested)!.TargetPermission);
        Assert.Equal("purchase-orders.approve", NotificationTypeRegistry.Find(NotificationTypes.PurchaseOrderApprovalRequested)!.TargetPermission);
        Assert.Equal("projects.change-orders.approve", NotificationTypeRegistry.Find(NotificationTypes.ChangeOrderApprovalRequested)!.TargetPermission);
        Assert.Equal("mrp.approve", NotificationTypeRegistry.Find(NotificationTypes.MrpRunApprovalRequested)!.TargetPermission);
        Assert.Equal("roles.assign-approval", NotificationTypeRegistry.Find(NotificationTypes.RoleAssignmentApprovalRequested)!.TargetPermission);
    }

    [Fact]
    public void RenderDeepLink_FillsGuidTokens_AndRefusesAnythingElse()
    {
        var d = NotificationTypeRegistry.Find(NotificationTypes.PurchaseOrderApprovalRequested)!;
        var id = Guid.NewGuid();
        var payload = new Dictionary<string, string> { [NotificationFields.ResourceId] = id.ToString("D") };
        Assert.Equal($"/procurement/purchase-orders/{id:D}", NotificationTypeRegistry.RenderDeepLink(d, payload));

        payload[NotificationFields.ResourceId] = "../../admin";
        Assert.Null(NotificationTypeRegistry.RenderDeepLink(d, payload));
        Assert.Null(NotificationTypeRegistry.RenderDeepLink(d, new Dictionary<string, string>()));
    }

    [Fact]
    public void RenderDeepLink_ForAStaticTemplate_NeedsNoPayload()
    {
        var d = NotificationTypeRegistry.Find(NotificationTypes.RoleAssignmentApprovalRequested)!;
        Assert.Equal("/settings/role-requests", NotificationTypeRegistry.RenderDeepLink(d, new Dictionary<string, string>()));
    }
}
```

`backend/tests/TanErp.UnitTests/Notifications/NotificationPayloadRulesTests.cs`:

```csharp
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationPayloadRulesTests
{
    private static readonly NotificationTypeDescriptor Po = NotificationTypeRegistry.Find(NotificationTypes.PurchaseOrderApprovalRequested)!;

    private static Dictionary<string, string> Valid() => new()
    {
        [NotificationFields.ResourceId] = Guid.NewGuid().ToString("D"),
        [NotificationFields.DocumentNumber] = "PO-2026-0001",
        [NotificationFields.ActorDisplayName] = "สมชาย ใจดี"
    };

    [Fact]
    public void ExactAllowedFields_Pass_AndAreTrimmed()
    {
        var payload = Valid();
        payload[NotificationFields.DocumentNumber] = "  PO-2026-0001 ";
        var result = NotificationPayloadRules.Validate(Po, payload);
        Assert.True(result.IsSuccess);
        Assert.Equal("PO-2026-0001", result.Value![NotificationFields.DocumentNumber]);
    }

    [Theory]
    [InlineData("totalAmount")]
    [InlineData("grandTotal")]
    [InlineData("unitCost")]
    [InlineData("sellingPrice")]
    [InlineData("marginRate")]
    [InlineData("budgetRemaining")]
    [InlineData("discount")]
    [InlineData("customerEmail")]
    [InlineData("phone")]
    [InlineData("siteAddress")]
    public void AFigureOrContactField_IsRejected_EvenAsAnExtraKey(string key)
    {
        var payload = Valid();
        payload[key] = "1000";
        var result = NotificationPayloadRules.Validate(Po, payload);
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", result.Error.Code);
    }

    [Fact]
    public void AnUnlistedKey_IsRejected_ThatIsNotEvenSuspicious()
    {
        var payload = Valid();
        payload["note"] = "hello";
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", NotificationPayloadRules.Validate(Po, payload).Error.Code);
    }

    [Fact]
    public void AMissingField_IsRejected()
    {
        var payload = Valid();
        payload.Remove(NotificationFields.DocumentNumber);
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", NotificationPayloadRules.Validate(Po, payload).Error.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ABlankValue_IsRejected(string value)
    {
        var payload = Valid();
        payload[NotificationFields.ActorDisplayName] = value;
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", NotificationPayloadRules.Validate(Po, payload).Error.Code);
    }

    [Fact]
    public void ATooLongValue_IsRejected()
    {
        var payload = Valid();
        payload[NotificationFields.DocumentNumber] = new string('x', NotificationPayloadRules.MaxValueLength + 1);
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", NotificationPayloadRules.Validate(Po, payload).Error.Code);
    }

    [Fact]
    public void ANullPayload_IsRejected()
    {
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", NotificationPayloadRules.Validate(Po, null).Error.Code);
    }

    [Fact]
    public void TheErrorMessage_NeverEchoesPayloadValues()
    {
        var payload = Valid();
        payload["totalAmount"] = "987654.32";
        var result = NotificationPayloadRules.Validate(Po, payload);
        Assert.DoesNotContain("987654", result.Error.Message);
    }
}
```

`backend/tests/TanErp.UnitTests/Notifications/NotificationPublishPlannerTests.cs`:

```csharp
using System.Text.Json;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationPublishPlannerTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Maker = Guid.NewGuid();
    private static readonly Guid Transition = Guid.NewGuid();

    private static NotificationEvent PurchaseOrder(IReadOnlyCollection<Guid>? excluded = null, IReadOnlyCollection<Guid>? explicitRecipients = null, Dictionary<string, string>? extra = null)
    {
        var fields = new Dictionary<string, string>
        {
            [NotificationFields.ResourceId] = Guid.NewGuid().ToString("D"),
            [NotificationFields.DocumentNumber] = "PO-2026-0001"
        };
        if (extra is not null) foreach (var pair in extra) fields[pair.Key] = pair.Value;
        return new NotificationEvent(NotificationTypes.PurchaseOrderApprovalRequested, Org, Guid.NewGuid(), Transition, Maker, fields, explicitRecipients, excluded ?? []);
    }

    [Fact]
    public void TheMaker_IsNeverANotifiedRecipient_EvenWhenTheyHoldThePermission()
    {
        var checker = Guid.NewGuid();
        var result = NotificationPublishPlanner.Plan(PurchaseOrder(), "ผู้จัดทำ", [Maker, checker]);
        Assert.True(result.IsSuccess);
        Assert.Equal([checker], result.Value!.Select(p => p.RecipientUserId).ToArray());
    }

    [Fact]
    public void ExplicitlyExcludedUsers_AreDropped_AndDuplicatesCollapse()
    {
        var creator = Guid.NewGuid();
        var checker = Guid.NewGuid();
        var result = NotificationPublishPlanner.Plan(PurchaseOrder(excluded: [creator]), "ผู้ส่ง", [creator, checker, checker, Guid.Empty]);
        Assert.Equal([checker], result.Value!.Select(p => p.RecipientUserId).ToArray());
    }

    [Fact]
    public void NoEligibleRecipient_YieldsAnEmptyPlan_NotAnError()
    {
        var result = NotificationPublishPlanner.Plan(PurchaseOrder(), "ผู้ส่ง", [Maker]);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }

    [Fact]
    public void RecipientsPerEvent_AreCapped_InAStableOrder()
    {
        var candidates = Enumerable.Range(0, NotificationLimits.MaxRecipientsPerEvent + 20).Select(_ => Guid.NewGuid()).ToArray();
        var first = NotificationPublishPlanner.Plan(PurchaseOrder(), "ผู้ส่ง", candidates).Value!;
        var second = NotificationPublishPlanner.Plan(PurchaseOrder(), "ผู้ส่ง", candidates.Reverse().ToArray()).Value!;
        Assert.Equal(NotificationLimits.MaxRecipientsPerEvent, first.Count);
        Assert.Equal(first.Select(p => p.RecipientUserId), second.Select(p => p.RecipientUserId));
    }

    [Fact]
    public void TheDedupeKey_IsTypePlusTransition_SoAResubmissionNotifiesAgain()
    {
        var checker = Guid.NewGuid();
        var one = NotificationPublishPlanner.Plan(PurchaseOrder(), "ผู้ส่ง", [checker]).Value!.Single();
        Assert.Equal($"purchase-order.approval-requested:{Transition:N}", one.DedupeKey);

        var again = PurchaseOrder() with { TransitionId = Guid.NewGuid() };
        var two = NotificationPublishPlanner.Plan(again, "ผู้ส่ง", [checker]).Value!.Single();
        Assert.NotEqual(one.DedupeKey, two.DedupeKey);
    }

    [Fact]
    public void ThePayload_ContainsExactlyTheAllowedFields_AndTheActorDisplayName()
    {
        var plan = NotificationPublishPlanner.Plan(PurchaseOrder(), "สมชาย ใจดี", [Guid.NewGuid()]).Value!.Single();
        using var document = JsonDocument.Parse(plan.PayloadJson);
        var keys = document.RootElement.EnumerateObject().Select(p => p.Name).OrderBy(k => k, StringComparer.Ordinal).ToArray();
        Assert.Equal(["actorDisplayName", "documentNumber", "resourceId"], keys);
        Assert.Equal("สมชาย ใจดี", document.RootElement.GetProperty("actorDisplayName").GetString());
    }

    [Fact]
    public void AFigureInTheEventFields_FailsThePlan_BeforeAnythingIsStored()
    {
        var result = NotificationPublishPlanner.Plan(PurchaseOrder(extra: new() { ["totalAmount"] = "250000.00" }), "ผู้ส่ง", [Guid.NewGuid()]);
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", result.Error.Code);
    }

    [Fact]
    public void ACallerSuppliedActorDisplayName_IsRejected_BecauseThePublisherOwnsIt()
    {
        var result = NotificationPublishPlanner.Plan(PurchaseOrder(extra: new() { [NotificationFields.ActorDisplayName] = "spoof" }), "ผู้ส่ง", [Guid.NewGuid()]);
        Assert.Equal("NOTIFICATION_PAYLOAD_INVALID", result.Error.Code);
    }

    [Fact]
    public void AnUnregisteredType_AndEmptyIds_AreRejected()
    {
        Assert.Equal("NOTIFICATION_TYPE_INVALID", NotificationPublishPlanner.Plan(PurchaseOrder() with { Type = "customer.created" }, "x", [Guid.NewGuid()]).Error.Code);
        Assert.Equal("NOTIFICATION_FIELD_INVALID", NotificationPublishPlanner.Plan(PurchaseOrder() with { TransitionId = Guid.Empty }, "x", [Guid.NewGuid()]).Error.Code);
        Assert.Equal("NOTIFICATION_FIELD_INVALID", NotificationPublishPlanner.Plan(PurchaseOrder() with { OrganizationId = Guid.Empty }, "x", [Guid.NewGuid()]).Error.Code);
    }
}
```

`backend/tests/TanErp.UnitTests/Notifications/NotificationEventsTests.cs`:

```csharp
using System.Text.Json;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationEventsTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Branch = Guid.NewGuid();
    private static readonly Guid Actor = Guid.NewGuid();
    private static readonly Guid Creator = Guid.NewGuid();
    private static readonly Guid Editor = Guid.NewGuid();
    private static readonly Guid Reviewer = Guid.NewGuid();
    private static readonly Guid Subject = Guid.NewGuid();

    public static TheoryData<NotificationEvent> AllEvents() => new()
    {
        NotificationEvents.EstimateSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), "EST-2026-0001", Reviewer),
        NotificationEvents.CostRecordSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), "ITEM-0001", [Creator, Editor]),
        NotificationEvents.PurchaseOrderSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), "PO-2026-0001", Creator),
        NotificationEvents.ChangeOrderSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), Guid.NewGuid(), "CO-2026-0001", Creator),
        NotificationEvents.MrpRunCreated(Org, Branch, Guid.NewGuid(), Actor, "MRP-2026-0001"),
        NotificationEvents.RoleAssignmentRequested(Org, Guid.NewGuid(), Actor, Subject, "สมหญิง ทดสอบ", "Approver"),
    };

    [Theory]
    [MemberData(nameof(AllEvents))]
    public void EveryEvent_BuildsAPlan_WhosePayloadIsExactlyTheRegisteredFields(NotificationEvent evt)
    {
        var plan = NotificationPublishPlanner.Plan(evt, "ผู้ส่ง ทดสอบ", [Reviewer, Guid.NewGuid()]);
        Assert.True(plan.IsSuccess, plan.Error.Message);

        var descriptor = NotificationTypeRegistry.Find(evt.Type)!;
        using var document = JsonDocument.Parse(plan.Value![0].PayloadJson);
        var keys = document.RootElement.EnumerateObject().Select(p => p.Name).ToHashSet(StringComparer.Ordinal);
        Assert.True(keys.SetEquals(descriptor.RequiredFields));
    }

    [Theory]
    [MemberData(nameof(AllEvents))]
    public void NoEventPayload_CarriesFigures_OrContactDetails(NotificationEvent evt)
    {
        var plan = NotificationPublishPlanner.Plan(evt, "ผู้ส่ง ทดสอบ", [Reviewer, Guid.NewGuid()]).Value![0];
        foreach (var fragment in new[] { "amount", "total", "cost", "price", "margin", "budget", "discount", "email", "phone", "address", "@" })
        {
            Assert.DoesNotContain(fragment, plan.PayloadJson, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void Estimate_NotifiesOnlyTheRouteReviewer_AndRecordsTheActor()
    {
        var evt = NotificationEvents.EstimateSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), "EST-1", Reviewer);
        Assert.Equal([Reviewer], evt.ExplicitRecipientUserIds!.ToArray());
        Assert.Equal(Actor, evt.ActorUserId);
    }

    [Fact]
    public void CostRecord_ExcludesTheCreatorAndTheLastFinancialEditor_AndMayBeOrganizationWide()
    {
        var evt = NotificationEvents.CostRecordSubmitted(Org, null, Guid.NewGuid(), Actor, Guid.NewGuid(), "ITEM-1", [Creator, Editor]);
        Assert.Null(evt.ExplicitRecipientUserIds);
        Assert.Null(evt.BranchId);
        Assert.True(evt.ExcludedUserIds.ToHashSet().SetEquals([Creator, Editor]));
    }

    [Fact]
    public void PurchaseOrder_AndChangeOrder_ExcludeTheirCreator()
    {
        var po = NotificationEvents.PurchaseOrderSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), "PO-1", Creator);
        var co = NotificationEvents.ChangeOrderSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), Guid.NewGuid(), "CO-1", Creator);
        Assert.Contains(Creator, po.ExcludedUserIds);
        Assert.Contains(Creator, co.ExcludedUserIds);
    }

    [Fact]
    public void RoleAssignment_ExcludesTheSubjectUser_AndIsOrganizationWide()
    {
        var evt = NotificationEvents.RoleAssignmentRequested(Org, Guid.NewGuid(), Actor, Subject, "สมหญิง", "Approver");
        Assert.Contains(Subject, evt.ExcludedUserIds);
        Assert.Null(evt.BranchId);
    }

    [Fact]
    public void ChangeOrder_PutsTheProjectIdInParentId_ForTheDeepLink()
    {
        var projectId = Guid.NewGuid();
        var evt = NotificationEvents.ChangeOrderSubmitted(Org, Branch, Guid.NewGuid(), Actor, Guid.NewGuid(), projectId, "CO-1", Creator);
        Assert.Equal(projectId.ToString("D"), evt.Fields[NotificationFields.ParentId]);
    }

    [Fact]
    public void TheEventTypes_AreAllDistinctAndCoverTheWholeWhitelist()
    {
        var types = AllEvents().Select(row => ((NotificationEvent)row[0]).Type).ToArray();
        Assert.Equal(types.Length, types.Distinct().Count());
        Assert.True(types.ToHashSet().SetEquals(NotificationTypes.All));
    }
}
```

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~NotificationRegistryTests|FullyQualifiedName~NotificationPayloadRulesTests|FullyQualifiedName~NotificationPublishPlannerTests|FullyQualifiedName~NotificationEventsTests"`
Expected: FAIL (compile) — namespace `TanErp.Application.Notifications` ไม่มี.

- [ ] **Step 2: implement models + ports**

`backend/src/TanErp.Application/Notifications/NotificationModels.cs`:

```csharp
namespace TanErp.Application.Notifications;

public sealed record NotificationCaller(string FirebaseUid, Guid MembershipId, string TraceId);

public static class NotificationLimits
{
    public const int MaxRecipientsPerEvent = 50;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 50;
}

/// <summary>Payload field names. The set each type may carry is declared in <see cref="NotificationTypeRegistry"/>.</summary>
public static class NotificationFields
{
    public const string ResourceId = "resourceId";
    public const string ParentId = "parentId";
    public const string DocumentNumber = "documentNumber";
    public const string ActorDisplayName = "actorDisplayName";
    public const string SubjectDisplayName = "subjectDisplayName";
    public const string RoleName = "roleName";
}

/// <summary>
/// Something that happened to a document and that other users must act on. <c>Fields</c> never contains <c>actorDisplayName</c>
/// (the publisher adds it from the actor's user record). <c>ExplicitRecipientUserIds</c> is used when the module already chose the
/// reviewers (Estimate approval route); otherwise recipients are the holders of the type's target permission in the organization/branch.
/// <c>TransitionId</c> changes every time the document enters the pending-approval state, so resubmission notifies again.
/// </summary>
public sealed record NotificationEvent(
    string Type,
    Guid OrganizationId,
    Guid? BranchId,
    Guid TransitionId,
    Guid ActorUserId,
    IReadOnlyDictionary<string, string> Fields,
    IReadOnlyCollection<Guid>? ExplicitRecipientUserIds,
    IReadOnlyCollection<Guid> ExcludedUserIds);

public sealed record PlannedNotification(Guid RecipientUserId, string Type, string PayloadJson, string DedupeKey);

public sealed record NotificationRow(Guid Id, string Type, string PayloadJson, DateTimeOffset CreatedAtUtc, DateTimeOffset? ReadAtUtc);

public sealed record NotificationRowPage(IReadOnlyList<NotificationRow> Items, int TotalCount);

public sealed record NotificationListQuery(bool UnreadOnly, int Page, int PageSize);

public sealed record NotificationProjection(
    Guid Id, string Type, IReadOnlyDictionary<string, string> Payload, string? DeepLink, DateTimeOffset CreatedAtUtc, DateTimeOffset? ReadAtUtc);

public sealed record NotificationPage(IReadOnlyList<NotificationProjection> Items, int Page, int PageSize, int TotalCount, int TotalPages);
```

`backend/src/TanErp.Application/Notifications/NotificationPorts.cs`:

```csharp
namespace TanErp.Application.Notifications;

/// <summary>
/// Stages notifications in the caller's current unit of work. It never calls SaveChanges:
/// the module's own SaveChanges/transaction commits them together with the business change, so a rolled-back change leaves nothing behind.
/// A bad event (unknown type, payload outside the allowlist) throws NotificationDomainException so the bug is loud, never silent.
/// </summary>
public interface INotificationPublisher
{
    Task PublishAsync(NotificationEvent evt, CancellationToken ct = default);
}

/// <summary>Active users who hold <paramref name="permissionKey"/> in the organization (and the branch when given), minus the excluded ones.</summary>
public interface INotificationRecipientResolver
{
    Task<IReadOnlyList<Guid>> ResolveAsync(
        Guid organizationId, Guid? branchId, string permissionKey, IReadOnlyCollection<Guid> excludedUserIds, DateTimeOffset atUtc, CancellationToken ct = default);
}

/// <summary>Reads and updates a user's own notifications. Every method is keyed by (organization, user): another user's row is never reachable.</summary>
public interface INotificationStore
{
    Task<NotificationRowPage> ListAsync(Guid organizationId, Guid userId, NotificationListQuery query, CancellationToken ct = default);

    Task<int> CountUnreadAsync(Guid organizationId, Guid userId, CancellationToken ct = default);

    /// <summary>Marks one notification read (idempotent) and returns it, or null when it is not this user's in this organization.</summary>
    Task<NotificationRow?> MarkReadAsync(Guid organizationId, Guid userId, Guid notificationId, CancellationToken ct = default);

    Task<int> MarkAllReadAsync(Guid organizationId, Guid userId, CancellationToken ct = default);
}
```

- [ ] **Step 3: implement registry + payload rules**

`backend/src/TanErp.Application/Notifications/NotificationTypeRegistry.cs`:

```csharp
using System.Text.RegularExpressions;
using TanErp.Domain.Notifications;

namespace TanErp.Application.Notifications;

/// <summary>
/// What a registered notification type requires. <c>TargetPermission</c> is the approval permission of the source document: it selects
/// the recipients and decides whether a reader still gets a link. <c>RequiredFields</c> is the exact payload allowlist (no optional fields).
/// <c>ReferenceField</c> is the field shown as the subject line in the UI.
/// </summary>
public sealed record NotificationTypeDescriptor(
    string Type,
    string TargetPermission,
    string DeepLinkTemplate,
    string ReferenceField,
    IReadOnlySet<string> RequiredFields);

/// <summary>Code-defined registry. Must stay in step with NotificationTypes (enforced by tests).</summary>
public static partial class NotificationTypeRegistry
{
    private static IReadOnlySet<string> Fields(params string[] names) => new HashSet<string>(names, StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, NotificationTypeDescriptor> Descriptors = new[]
    {
        new NotificationTypeDescriptor(
            NotificationTypes.EstimateApprovalRequested, "estimates.approve", "/estimates/review-queue", NotificationFields.DocumentNumber,
            Fields(NotificationFields.ResourceId, NotificationFields.DocumentNumber, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.CostRecordApprovalRequested, "cost-records.approve", "/item-master/cost-reviews", NotificationFields.DocumentNumber,
            Fields(NotificationFields.ResourceId, NotificationFields.DocumentNumber, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.PurchaseOrderApprovalRequested, "purchase-orders.approve", "/procurement/purchase-orders/{resourceId}", NotificationFields.DocumentNumber,
            Fields(NotificationFields.ResourceId, NotificationFields.DocumentNumber, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.ChangeOrderApprovalRequested, "projects.change-orders.approve", "/projects/{parentId}", NotificationFields.DocumentNumber,
            Fields(NotificationFields.ResourceId, NotificationFields.ParentId, NotificationFields.DocumentNumber, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.MrpRunApprovalRequested, "mrp.approve", "/production/mrp/{resourceId}", NotificationFields.DocumentNumber,
            Fields(NotificationFields.ResourceId, NotificationFields.DocumentNumber, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.RoleAssignmentApprovalRequested, "roles.assign-approval", "/settings/role-requests", NotificationFields.SubjectDisplayName,
            Fields(NotificationFields.ResourceId, NotificationFields.SubjectDisplayName, NotificationFields.RoleName, NotificationFields.ActorDisplayName))
    }.ToDictionary(d => d.Type, StringComparer.Ordinal);

    public static IReadOnlyCollection<string> Types { get; } = Descriptors.Keys.ToArray();

    /// <summary>Distinct target permissions of every registered type.</summary>
    public static IReadOnlyCollection<string> TargetPermissions { get; } =
        Descriptors.Values.Select(d => d.TargetPermission).Distinct(StringComparer.Ordinal).ToArray();

    /// <summary>Exact (already normalized) type lookup.</summary>
    public static NotificationTypeDescriptor? Find(string? type) =>
        type is not null && Descriptors.TryGetValue(type, out var descriptor) ? descriptor : null;

    [GeneratedRegex(@"\{([A-Za-z]+)\}")]
    private static partial Regex TokenPattern();

    public static IReadOnlyList<string> TemplateTokens(string template) =>
        TokenPattern().Matches(template).Select(m => m.Groups[1].Value).ToArray();

    /// <summary>
    /// Fills the descriptor's deep-link template from the payload. A token is replaced only by a value that parses as a GUID
    /// (re-formatted, never copied verbatim), so a payload can never smuggle a path or query into the link. Returns null otherwise.
    /// </summary>
    public static string? RenderDeepLink(NotificationTypeDescriptor descriptor, IReadOnlyDictionary<string, string> payload)
    {
        var link = descriptor.DeepLinkTemplate;
        foreach (var token in TemplateTokens(link))
        {
            if (!payload.TryGetValue(token, out var raw) || !Guid.TryParse(raw, out var id) || id == Guid.Empty) return null;
            link = link.Replace("{" + token + "}", id.ToString("D"), StringComparison.Ordinal);
        }

        return link;
    }
}
```

`backend/src/TanErp.Application/Notifications/NotificationPayloadRules.cs`:

```csharp
using TanErp.Application.Common.Results;

namespace TanErp.Application.Notifications;

/// <summary>
/// Payload hygiene. The allowlist (exactly the fields the type declares) is the real guard; the forbidden-fragment check is a second
/// net that also stops a future type from declaring a field that looks like a figure or a contact detail.
/// Error messages name keys, never values.
/// </summary>
public static class NotificationPayloadRules
{
    public const int MaxValueLength = 200;

    private static readonly string[] ForbiddenKeyFragments =
    {
        "cost", "price", "margin", "amount", "total", "budget", "discount", "tax", "salary",
        "email", "phone", "address", "token", "password", "secret"
    };

    public static bool IsForbiddenKey(string key) =>
        ForbiddenKeyFragments.Any(fragment => key.Contains(fragment, StringComparison.OrdinalIgnoreCase));

    public static Result<IReadOnlyDictionary<string, string>> Validate(NotificationTypeDescriptor descriptor, IReadOnlyDictionary<string, string>? payload)
    {
        if (payload is null) return Fail("The payload is required.");

        foreach (var key in payload.Keys)
        {
            if (IsForbiddenKey(key)) return Fail($"The payload field '{key}' is not allowed in a notification.");
            if (!descriptor.RequiredFields.Contains(key)) return Fail($"The payload field '{key}' is not declared for '{descriptor.Type}'.");
        }

        var missing = descriptor.RequiredFields.Where(field => !payload.ContainsKey(field)).OrderBy(f => f, StringComparer.Ordinal).ToArray();
        if (missing.Length > 0) return Fail($"The payload is missing: {string.Join(", ", missing)}.");

        var cleaned = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var (key, value) in payload)
        {
            var trimmed = value?.Trim() ?? string.Empty;
            if (trimmed.Length is 0 || trimmed.Length > MaxValueLength) return Fail($"The payload field '{key}' must be 1-{MaxValueLength} characters.");
            cleaned[key] = trimmed;
        }

        return Result<IReadOnlyDictionary<string, string>>.Success(cleaned);
    }

    private static Result<IReadOnlyDictionary<string, string>> Fail(string message) =>
        Result<IReadOnlyDictionary<string, string>>.Failure(new Error("NOTIFICATION_PAYLOAD_INVALID", message));
}
```

- [ ] **Step 4: implement planner + event factories**

`backend/src/TanErp.Application/Notifications/NotificationPublishPlanner.cs`:

```csharp
using System.Text.Json;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Notifications;

/// <summary>
/// Pure part of publishing: validates the event, builds the final payload and picks who gets it. The maker (actor) and any
/// explicitly excluded user never receive it, duplicates collapse, and the list is capped and ordered so retries are stable.
/// Scope filtering (who holds the permission) happens before this, in the recipient resolver.
/// </summary>
public static class NotificationPublishPlanner
{
    public static Result<IReadOnlyList<PlannedNotification>> Plan(NotificationEvent evt, string actorDisplayName, IReadOnlyCollection<Guid> candidateRecipientUserIds)
    {
        var descriptor = NotificationTypeRegistry.Find(evt.Type);
        if (descriptor is null) return Fail("NOTIFICATION_TYPE_INVALID", $"Notification type '{evt.Type}' is not registered.");
        if (evt.OrganizationId == Guid.Empty || evt.TransitionId == Guid.Empty || evt.ActorUserId == Guid.Empty)
        {
            return Fail("NOTIFICATION_FIELD_INVALID", "An organization, a transition and an actor are required.");
        }

        if (evt.Fields.ContainsKey(NotificationFields.ActorDisplayName))
        {
            return Fail("NOTIFICATION_PAYLOAD_INVALID", "The actor display name is added by the publisher and must not be supplied.");
        }

        var fields = new Dictionary<string, string>(evt.Fields, StringComparer.Ordinal)
        {
            [NotificationFields.ActorDisplayName] = actorDisplayName
        };
        var payload = NotificationPayloadRules.Validate(descriptor, fields);
        if (payload.IsFailure) return Result<IReadOnlyList<PlannedNotification>>.Failure(payload.Error);

        var payloadJson = JsonSerializer.Serialize(payload.Value!);
        var dedupeKey = $"{descriptor.Type}:{evt.TransitionId:N}";
        var excluded = new HashSet<Guid>(evt.ExcludedUserIds) { evt.ActorUserId };

        IReadOnlyList<PlannedNotification> planned = candidateRecipientUserIds
            .Where(id => id != Guid.Empty && !excluded.Contains(id))
            .Distinct()
            .OrderBy(id => id)
            .Take(NotificationLimits.MaxRecipientsPerEvent)
            .Select(id => new PlannedNotification(id, descriptor.Type, payloadJson, dedupeKey))
            .ToList();
        return Result<IReadOnlyList<PlannedNotification>>.Success(planned);
    }

    private static Result<IReadOnlyList<PlannedNotification>> Fail(string code, string message) =>
        Result<IReadOnlyList<PlannedNotification>>.Failure(new Error(code, message));
}
```

`backend/src/TanErp.Application/Notifications/NotificationEvents.cs`:

```csharp
using TanErp.Domain.Notifications;

namespace TanErp.Application.Notifications;

/// <summary>
/// The only place that decides which fields each source puts in a notification. Stores pass identifiers and display strings,
/// never amounts: there is no parameter here that could carry a figure.
/// </summary>
public static class NotificationEvents
{
    private static string Id(Guid id) => id.ToString("D");

    private static Dictionary<string, string> Fields(params (string Key, string Value)[] pairs) =>
        pairs.ToDictionary(p => p.Key, p => p.Value, StringComparer.Ordinal);

    /// <summary>Estimate submitted for approval. Notifies the first reviewer of the approval route (already independent of the submitter).</summary>
    public static NotificationEvent EstimateSubmitted(
        Guid organizationId, Guid branchId, Guid approvalRequestId, Guid actorUserId, Guid estimateId, string estimateNumber, Guid firstReviewerUserId) =>
        new(NotificationTypes.EstimateApprovalRequested, organizationId, branchId, approvalRequestId, actorUserId,
            Fields((NotificationFields.ResourceId, Id(estimateId)), (NotificationFields.DocumentNumber, estimateNumber)),
            [firstReviewerUserId], []);

    /// <summary>Cost record submitted. The creator and the last financial editor cannot approve it, so they are not notified.</summary>
    public static NotificationEvent CostRecordSubmitted(
        Guid organizationId, Guid? branchId, Guid newRowVersion, Guid actorUserId, Guid costRecordId, string itemCode, IReadOnlyCollection<Guid> makerUserIds) =>
        new(NotificationTypes.CostRecordApprovalRequested, organizationId, branchId, newRowVersion, actorUserId,
            Fields((NotificationFields.ResourceId, Id(costRecordId)), (NotificationFields.DocumentNumber, itemCode)),
            null, makerUserIds);

    public static NotificationEvent PurchaseOrderSubmitted(
        Guid organizationId, Guid branchId, Guid newRowVersion, Guid actorUserId, Guid purchaseOrderId, string number, Guid creatorUserId) =>
        new(NotificationTypes.PurchaseOrderApprovalRequested, organizationId, branchId, newRowVersion, actorUserId,
            Fields((NotificationFields.ResourceId, Id(purchaseOrderId)), (NotificationFields.DocumentNumber, number)),
            null, [creatorUserId]);

    public static NotificationEvent ChangeOrderSubmitted(
        Guid organizationId, Guid branchId, Guid newRowVersion, Guid actorUserId, Guid changeOrderId, Guid projectId, string number, Guid creatorUserId) =>
        new(NotificationTypes.ChangeOrderApprovalRequested, organizationId, branchId, newRowVersion, actorUserId,
            Fields((NotificationFields.ResourceId, Id(changeOrderId)), (NotificationFields.ParentId, Id(projectId)), (NotificationFields.DocumentNumber, number)),
            null, [creatorUserId]);

    /// <summary>A planning run with at least one proposed recommendation. The run's creator cannot decide it.</summary>
    public static NotificationEvent MrpRunCreated(Guid organizationId, Guid branchId, Guid runId, Guid actorUserId, string number) =>
        new(NotificationTypes.MrpRunApprovalRequested, organizationId, branchId, runId, actorUserId,
            Fields((NotificationFields.ResourceId, Id(runId)), (NotificationFields.DocumentNumber, number)),
            null, []);

    /// <summary>Role assignment that needs a checker. The user receiving the role cannot approve their own change.</summary>
    public static NotificationEvent RoleAssignmentRequested(
        Guid organizationId, Guid requestId, Guid actorUserId, Guid subjectUserId, string subjectDisplayName, string roleName) =>
        new(NotificationTypes.RoleAssignmentApprovalRequested, organizationId, null, requestId, actorUserId,
            Fields((NotificationFields.ResourceId, Id(requestId)), (NotificationFields.SubjectDisplayName, subjectDisplayName), (NotificationFields.RoleName, roleName)),
            null, [subjectUserId]);
}
```

- [ ] **Step 5: รัน test ให้ผ่าน**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~NotificationRegistryTests|FullyQualifiedName~NotificationPayloadRulesTests|FullyQualifiedName~NotificationPublishPlannerTests|FullyQualifiedName~NotificationEventsTests"`
Expected: PASS ทั้งหมด. ถ้า `NoEventPayload_CarriesFigures_OrContactDetails` ล้มเพราะชื่อทดสอบ ("สมหญิง ทดสอบ") ชนคำใน `fragment` ให้เปลี่ยนข้อมูลตัวอย่าง ไม่ใช่ผ่อนปรน fragment.

Run: `dotnet build backend/TanErp.slnx --nologo -v q` → Expected: Build succeeded.
Run: `dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj` → Expected: PASS.

- [ ] **Step 6: commit**

```bash
git add backend/src/TanErp.Application/Notifications backend/tests/TanErp.UnitTests/Notifications
git commit -F - <<'EOF'
feat(notifications): add type registry, payload allowlist, publish planner and event factories

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

