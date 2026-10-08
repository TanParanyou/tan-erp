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
| `docs/05-engineering/notification-foundation-verification.md` (ใหม่) | หลักฐานการทดสอบ + ข้อจำกัด (Task 12) |
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
| `frontend/src/lib/notifications/notification-view.ts` (+ `.test.tsx`) (ใหม่) | `useNotificationText` (type → ข้อความ, type ไม่รู้จัก → บรรทัดกลาง) + `localizedNotificationHref` ใช้ร่วม dropdown และหน้ารายการ |
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


---

## Task 4: Infrastructure — EF configuration และ migration `AddNotifications`

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/Configurations/NotificationConfiguration.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`
- Generate: `backend/src/TanErp.Infrastructure/Persistence/Migrations/<timestamp>_AddNotifications.cs` (+ `.Designer.cs`, + `AppDbContextModelSnapshot.cs`)
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/NotificationSchemaTests.cs`

`ApplyConfigurationsFromAssembly` (AppDbContext.cs:127) หยิบ configuration อัตโนมัติ จึงไม่ต้องลงทะเบียนเพิ่ม. check constraint ต้อง **เท่ากับหรือหลวมกว่า** invariant ของ `Notification` (Task 2) — ไม่เข้มกว่า: type = รูปแบบเท่านั้น (รายการจริงอยู่ในโค้ด `NotificationTypes`), dedupe key ยาว 1–160 หลัง trim, payload เป็น JSON object. ไม่เพิ่ม check เรื่องความยาว payload (jsonb normalize ข้อความแล้วนับต่างจาก string ต้นฉบับ) และไม่บังคับ `read_at_utc >= created_at_utc` (clock คนละเครื่องทำให้เข้มกว่า domain).

**บทเรียนที่ต้องทำตาม:** ถ้า migration ผิด ให้ลบแล้ว **generate ใหม่** (`dotnet ef migrations remove`) ห้ามซ้อน migration แก้ไขทับ; diff ของ `AppDbContextModelSnapshot.cs` ต้องเป็นการ *เพิ่ม* เท่านั้น (ไม่มีบรรทัดลบ/แก้ entity อื่น).

- [ ] **Step 1: เขียน test ที่ล้มก่อน**

`backend/tests/TanErp.IntegrationTests/Persistence/NotificationSchemaTests.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TanErp.Domain.Notifications;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

/// <summary>Proves the migrated schema: table, indexes, FKs and check constraints that mirror (never exceed) the domain invariants.</summary>
public class NotificationSchemaTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private AppDbContext _db = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(_postgres.GetConnectionString()).Options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private static Notification Make(string key = "estimate.approval-requested:1", string payload = "{\"resourceId\":\"x\"}") =>
        new(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, TestOnlyDataSeeder.TestUserId,
            NotificationTypes.EstimateApprovalRequested, payload, key, DateTimeOffset.UtcNow);

    [Fact]
    public async Task Table_LivesInTheNotificationsSchema_WithTheExpectedIndexes()
    {
        var indexes = await _db.Database
            .SqlQuery<string>($"SELECT indexname AS \"Value\" FROM pg_indexes WHERE schemaname = 'notifications' AND tablename = 'notifications'")
            .ToListAsync();

        Assert.Contains("ux_notifications_recipient_dedupe", indexes);
        Assert.Contains("ix_notifications_recipient_created", indexes);
        Assert.Contains("ix_notifications_unread", indexes);

        var unreadDef = await _db.Database
            .SqlQuery<string>($"SELECT indexdef AS \"Value\" FROM pg_indexes WHERE indexname = 'ix_notifications_unread'")
            .SingleAsync();
        Assert.Contains("read_at_utc IS NULL", unreadDef);
    }

    [Fact]
    public async Task SameRecipientAndDedupeKey_IsRejectedByTheDatabase()
    {
        _db.Set<Notification>().Add(Make());
        await _db.SaveChangesAsync();

        _db.Set<Notification>().Add(Make());
        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => _db.SaveChangesAsync());
        Assert.Equal("ux_notifications_recipient_dedupe", ((PostgresException)ex.InnerException!).ConstraintName);
    }

    [Fact]
    public async Task ARecipientOutsideUsers_IsRejectedByTheForeignKey()
    {
        _db.Set<Notification>().Add(new Notification(
            Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, Guid.NewGuid(),
            NotificationTypes.EstimateApprovalRequested, "{}", "k:1", DateTimeOffset.UtcNow));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => _db.SaveChangesAsync());
        Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, ((PostgresException)ex.InnerException!).SqlState);
    }

    [Theory]
    [InlineData("ck_notifications_type_format", "Bad Type", "k:2", "{}")]
    [InlineData("ck_notifications_dedupe_key_length", "estimate.approval-requested", "   ", "{}")]
    [InlineData("ck_notifications_payload_object", "estimate.approval-requested", "k:3", "[]")]
    public async Task ShapeChecks_RejectRowsTheDomainCouldNeverCreate(string constraint, string type, string key, string payload)
    {
        var sql = "INSERT INTO notifications.notifications (id, organization_id, recipient_user_id, type, payload, dedupe_key, created_at_utc) " +
                  "VALUES ({0}, {1}, {2}, {3}, {4}::jsonb, {5}, now())";
        var ex = await Assert.ThrowsAsync<PostgresException>(() => _db.Database.ExecuteSqlRawAsync(
            sql, Guid.NewGuid(), TestOnlyDataSeeder.TestOrgId, TestOnlyDataSeeder.TestUserId, type, payload, key));
        Assert.Equal(constraint, ex.ConstraintName);
    }
}
```

- [ ] **Step 2: รัน test เพื่อดูว่าล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~NotificationSchemaTests"`
Expected: FAIL (compile error: `Set<Notification>()` ไม่ถูก map / ไม่มีตาราง `notifications.notifications`). ต้องมี Docker.

- [ ] **Step 3: เขียน configuration + DbSet**

`backend/src/TanErp.Infrastructure/Persistence/Configurations/NotificationConfiguration.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Notifications;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications", "notifications", t =>
        {
            // The real type list lives in code (NotificationTypes); the database only guards the shape, never more than the domain does.
            t.HasCheckConstraint("ck_notifications_type_format", "type ~ '^[a-z][a-z0-9.-]{1,59}$'");
            t.HasCheckConstraint("ck_notifications_dedupe_key_length", "char_length(btrim(dedupe_key)) BETWEEN 1 AND 160");
            t.HasCheckConstraint("ck_notifications_payload_object", "jsonb_typeof(payload) = 'object'");
        });

        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.IsRead);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.RecipientUserId).HasColumnName("recipient_user_id").IsRequired();
        builder.Property(x => x.Type).HasColumnName("type").HasMaxLength(60).IsRequired();
        builder.Property(x => x.PayloadJson).HasColumnName("payload").HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.DedupeKey).HasColumnName("dedupe_key").HasMaxLength(Notification.MaxDedupeKeyLength).IsRequired();
        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.ReadAtUtc).HasColumnName("read_at_utc").HasColumnType("timestamptz");

        // One message per (recipient, transition); also closes the race two concurrent submits could open.
        builder.HasIndex(x => new { x.OrganizationId, x.RecipientUserId, x.DedupeKey })
            .IsUnique()
            .HasDatabaseName("ux_notifications_recipient_dedupe");
        // Newest-first listing for one recipient.
        builder.HasIndex(x => new { x.OrganizationId, x.RecipientUserId, x.CreatedAtUtc, x.Id })
            .IsDescending(false, false, true, true)
            .HasDatabaseName("ix_notifications_recipient_created");
        // The bell badge only ever counts unread rows.
        builder.HasIndex(x => new { x.OrganizationId, x.RecipientUserId })
            .HasFilter("read_at_utc IS NULL")
            .HasDatabaseName("ix_notifications_unread");

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.RecipientUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
```

ใน `AppDbContext.cs` ใต้บรรทัด `SignatureCaptures` (:99) เพิ่ม:

```csharp
    public DbSet<TanErp.Domain.Notifications.Notification> Notifications => Set<TanErp.Domain.Notifications.Notification>();
```

(test ข้างบนใช้ `_db.Set<Notification>()` ได้เช่นกัน; เมื่อมี DbSet แล้วไม่ต้องแก้ test.)

- [ ] **Step 4: build, generate migration**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet build backend/TanErp.slnx --nologo -v q` → Expected: Build succeeded.
Run (ConnectionStrings ปลอมพอให้ design-time สร้าง context ได้; ไม่เชื่อมต่อ DB จริง):

```bash
cd /Users/syaco/Documents/development/tan-erp
ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" \
dotnet ef migrations add AddNotifications --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations
```

Expected: `Done.` + ไฟล์ `*_AddNotifications.cs`, `.Designer.cs` และ snapshot ถูกแก้. ถ้า `dotnet ef` ไม่พบ: `dotnet tool install --global dotnet-ef`.

- [ ] **Step 5: ตรวจ migration และ snapshot diff**

Run: `grep -n "EnsureSchema\|CreateTable\|schema: \"notifications\"\|filter:\|CheckConstraint\|IsDescending\|descending" backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddNotifications.cs`
Expected: `EnsureSchema` ของ `notifications`, `CreateTable` ตารางเดียว, `filter: "read_at_utc IS NULL"`, check constraint 3 ตัว, index `ix_notifications_recipient_created` มี `descending`.
Run: `grep -n "Down(" -A6 backend/src/TanErp.Infrastructure/Persistence/Migrations/*_AddNotifications.cs` → Expected: `DropTable` แล้ว `DropSchema`.
Run: `git diff --numstat backend/src/TanErp.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs` → Expected: คอลัมน์ที่สอง (บรรทัดที่ลบ) = `0`. ถ้ามีบรรทัดลบหรือมีตารางอื่นปนมา → `dotnet ef migrations remove ...` (คำสั่งเดียวกัน, ไม่มี `--output-dir`) แล้ว generate ใหม่ ห้ามแก้ไฟล์ที่ generate ด้วยมือ.

- [ ] **Step 6: รัน test ให้ผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~NotificationSchemaTests"`
Expected: PASS ทั้งหมด (6 cases).

- [ ] **Step 7: commit**

```bash
git add backend/src/TanErp.Infrastructure/Persistence/Configurations/NotificationConfiguration.cs backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs backend/src/TanErp.Infrastructure/Persistence/Migrations backend/tests/TanErp.IntegrationTests/Persistence/NotificationSchemaTests.cs
git commit -F - <<'EOF'
feat(notifications): add notifications table and migration

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 5: Infrastructure — publisher, recipient resolver, store, `ResolveMembershipAsync` และ DI

**Files:**
- Modify: `backend/src/TanErp.Application/Common/Abstractions/IRequestAccessResolver.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/RequestAccessResolver.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationRecipientResolver.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationPublisher.cs`
- Create: `backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationStore.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Test: `backend/tests/TanErp.IntegrationTests/Persistence/NotificationInfrastructureTests.cs`, `backend/tests/TanErp.IntegrationTests/Persistence/RequestAccessResolverTests.cs` (เพิ่ม case)

กติกา (ADR 0018): publisher **ไม่เรียก `SaveChangesAsync`** — แค่ `Add` เข้า `AppDbContext` ตัวเดียวกับ store ของโมดูลต้นเหตุ (scoped) แล้ว `SaveChangesAsync`/transaction ของโมดูลนั้น commit พร้อมกัน. publisher/resolver/store ทุกตัวฉีด `AppDbContext` ที่ DI ให้ (ห้ามสร้าง context ใหม่).

- [ ] **Step 1: เขียน test ที่ล้มก่อน**

เพิ่มใน `backend/tests/TanErp.IntegrationTests/Persistence/RequestAccessResolverTests.cs` (class เดิม ใช้ `_resolver`/`_db` ที่มีอยู่):

```csharp
    [Fact]
    public async Task ResolveMembershipAsync_ActiveMembership_ReturnsContextWithoutPermission()
    {
        var result = await _resolver.ResolveMembershipAsync(TestOnlyDataSeeder.TestFirebaseUid, TestOnlyDataSeeder.TestMembershipId);

        Assert.True(result.IsSuccess);
        Assert.Equal(TestOnlyDataSeeder.TestUserId, result.Value!.ActorUserId);
        Assert.Equal(TestOnlyDataSeeder.TestOrgId, result.Value.OrganizationId);
    }

    [Fact]
    public async Task ResolveMembershipAsync_AnotherUsersMembership_IsActiveMembershipRequired()
    {
        var result = await _resolver.ResolveMembershipAsync("someone-else", TestOnlyDataSeeder.TestMembershipId);

        Assert.True(result.IsFailure);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", result.Error.Code);
    }

    [Fact]
    public async Task ResolveMembershipAsync_BlankUid_IsAuthenticationRequired()
    {
        var result = await _resolver.ResolveMembershipAsync(" ", TestOnlyDataSeeder.TestMembershipId);

        Assert.Equal("AUTHENTICATION_REQUIRED", result.Error.Code);
    }
```

`backend/tests/TanErp.IntegrationTests/Persistence/NotificationInfrastructureTests.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Notifications;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Notifications;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.Notifications;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class NotificationInfrastructureTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private AppDbContext _db = null!;
    private NotificationRecipientResolver _resolver = null!;
    private NotificationPublisher _publisher = null!;
    private NotificationStore _store = null!;

    private static readonly Guid Org = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid Maker = TestOnlyDataSeeder.TestUserId;       // Test Admin: holds every *.approve at organization scope

    private sealed class FixedClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(_postgres.GetConnectionString()).Options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);
        var clock = new FixedClock();
        _resolver = new NotificationRecipientResolver(_db);
        _publisher = new NotificationPublisher(_db, _resolver, clock);
        _store = new NotificationStore(_db, clock);
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    /// <summary>A second user whose only grant is <paramref name="permissionKey"/> at organization scope.</summary>
    private async Task<Guid> AddCheckerAsync(string permissionKey, Guid? membershipBranchId = null, bool active = true, DateTimeOffset? expiresAtUtc = null)
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        var permission = await _db.Permissions.SingleAsync(p => p.Key == permissionKey);
        _db.Users.Add(new User(userId, $"uid-{userId:N}", "Checker " + userId.ToString("N")[..6], $"{userId:N}@example.test"));
        _db.Roles.Add(new Role(roleId, Org, "Checker " + userId.ToString("N")[..6]));
        _db.Add(new RolePermission(Guid.NewGuid(), roleId, Org, permission.Id, PermissionScope.Organization, Org));
        _db.Memberships.Add(new Membership(membershipId, Org, membershipBranchId, userId, active, null, expiresAtUtc));
        _db.Add(new MembershipRole(membershipId, roleId, Org));
        await _db.SaveChangesAsync();
        return userId;
    }

    private static NotificationEvent MrpEvent(Guid? transition = null) =>
        NotificationEvents.MrpRunCreated(Org, TestOnlyDataSeeder.TestBranchId, transition ?? Guid.NewGuid(), Maker, "MRP-0001");

    [Fact]
    public async Task Resolver_ReturnsHoldersOfThePermission_AndDropsExcludedInactiveExpiredAndOtherBranchUsers()
    {
        var holder = await AddCheckerAsync("mrp.approve");
        await AddCheckerAsync("mrp.approve", active: false);
        await AddCheckerAsync("mrp.approve", expiresAtUtc: DateTimeOffset.UtcNow.AddDays(-1));
        var otherBranch = new Branch(Guid.NewGuid(), Org, "NOTIF-B2", "Other branch");
        _db.Branches.Add(otherBranch);
        await _db.SaveChangesAsync();
        await AddCheckerAsync("mrp.approve", membershipBranchId: otherBranch.Id);                     // another branch of the same organization: never matches
        await AddCheckerAsync("estimates.read");                                                       // wrong permission

        var ids = await _resolver.ResolveAsync(Org, TestOnlyDataSeeder.TestBranchId, "mrp.approve", [Maker], DateTimeOffset.UtcNow);

        Assert.Equal([holder], ids);
    }

    [Fact]
    public async Task Resolver_WithoutABranch_ReturnsBranchScopedMembersToo()
    {
        var branchHolder = await AddCheckerAsync("roles.assign-approval", membershipBranchId: TestOnlyDataSeeder.TestBranchId);

        var ids = await _resolver.ResolveAsync(Org, null, "roles.assign-approval", [Maker], DateTimeOffset.UtcNow);

        Assert.Contains(branchHolder, ids);
        Assert.DoesNotContain(Maker, ids);
    }

    [Fact]
    public async Task Publish_StagesRowsWithoutSaving_AndTheCallersSaveCommitsThem_ExcludingTheMaker()
    {
        var checker = await AddCheckerAsync("mrp.approve");

        await _publisher.PublishAsync(MrpEvent());

        Assert.Equal(0, await CountRowsAsync());            // nothing is stored until the module saves
        await _db.SaveChangesAsync();
        var rows = await _db.Notifications.AsNoTracking().ToListAsync();
        Assert.Contains(rows, r => r.RecipientUserId == checker && r.Type == NotificationTypes.MrpRunApprovalRequested);
        Assert.DoesNotContain(rows, r => r.RecipientUserId == Maker);
        Assert.Contains("MRP-0001", rows[0].PayloadJson);
    }

    [Fact]
    public async Task Publish_ThenDiscardingTheUnitOfWork_LeavesNoNotification()
    {
        await AddCheckerAsync("mrp.approve");
        await _publisher.PublishAsync(MrpEvent());

        _db.ChangeTracker.Clear();                           // what a failed/rolled-back business change does to the staged rows
        await _db.SaveChangesAsync();

        Assert.Equal(0, await CountRowsAsync());
    }

    [Fact]
    public async Task Publish_TheSameTransitionTwice_NotifiesOnce_ButANewTransitionNotifiesAgain()
    {
        var checker = await AddCheckerAsync("mrp.approve");
        var transition = Guid.NewGuid();

        await _publisher.PublishAsync(MrpEvent(transition));
        await _publisher.PublishAsync(MrpEvent(transition));   // staged but unsaved: still deduped
        await _db.SaveChangesAsync();
        await _publisher.PublishAsync(MrpEvent(transition));   // already stored: deduped
        await _db.SaveChangesAsync();
        Assert.Equal(1, await _db.Notifications.CountAsync(n => n.RecipientUserId == checker));

        await _publisher.PublishAsync(MrpEvent());
        await _db.SaveChangesAsync();
        Assert.Equal(2, await _db.Notifications.CountAsync(n => n.RecipientUserId == checker));
    }

    [Fact]
    public async Task Publish_WithExplicitRecipients_NotifiesOnlyThem()
    {
        var reviewer = TestOnlyDataSeeder.TestEstimateReviewerUserId;
        await AddCheckerAsync("estimates.approve");            // holds the permission but is not on the route

        await _publisher.PublishAsync(NotificationEvents.EstimateSubmitted(
            Org, TestOnlyDataSeeder.TestBranchId, Guid.NewGuid(), Maker, Guid.NewGuid(), "EST-0001", reviewer));
        await _db.SaveChangesAsync();

        Assert.Equal([reviewer], await _db.Notifications.Select(n => n.RecipientUserId).ToListAsync());
    }

    [Fact]
    public async Task Publish_WithNoEligibleRecipient_StagesNothing_AndAnUnknownActorFailsLoudly()
    {
        await _publisher.PublishAsync(MrpEvent());             // seed has only the maker holding mrp.approve
        await _db.SaveChangesAsync();
        Assert.Equal(0, await CountRowsAsync());

        var ex = await Assert.ThrowsAsync<NotificationDomainException>(() => _publisher.PublishAsync(
            NotificationEvents.MrpRunCreated(Org, TestOnlyDataSeeder.TestBranchId, Guid.NewGuid(), Guid.NewGuid(), "MRP-0002")));
        Assert.Equal("NOTIFICATION_FIELD_INVALID", ex.Code);
    }

    [Fact]
    public async Task Store_OnlyEverSeesTheCallersOwnRows_AndMarkReadIsIdempotent()
    {
        var mine = await AddCheckerAsync("mrp.approve");
        var theirs = await AddCheckerAsync("mrp.approve");
        await _publisher.PublishAsync(MrpEvent());
        await _publisher.PublishAsync(MrpEvent());
        await _db.SaveChangesAsync();

        var page = await _store.ListAsync(Org, mine, new NotificationListQuery(false, 1, 20));
        Assert.Equal(2, page.TotalCount);
        Assert.True(page.Items[0].CreatedAtUtc >= page.Items[1].CreatedAtUtc);
        Assert.Equal(2, await _store.CountUnreadAsync(Org, mine));

        var otherId = await _db.Notifications.Where(n => n.RecipientUserId == theirs).Select(n => n.Id).FirstAsync();
        Assert.Null(await _store.MarkReadAsync(Org, mine, otherId));                       // someone else's row
        Assert.Null(await _store.MarkReadAsync(Guid.NewGuid(), mine, page.Items[0].Id));    // another organization
        Assert.Null(await _store.MarkReadAsync(Org, mine, Guid.NewGuid()));                // does not exist

        var first = await _store.MarkReadAsync(Org, mine, page.Items[0].Id);
        var again = await _store.MarkReadAsync(Org, mine, page.Items[0].Id);
        Assert.NotNull(first!.ReadAtUtc);
        Assert.Equal(first.ReadAtUtc, again!.ReadAtUtc);                                   // first read time is kept
        Assert.Equal(1, await _store.CountUnreadAsync(Org, mine));
        Assert.Equal(2, await _store.CountUnreadAsync(Org, theirs));                       // untouched

        var unreadOnly = await _store.ListAsync(Org, mine, new NotificationListQuery(true, 1, 20));
        Assert.Single(unreadOnly.Items);

        Assert.Equal(1, await _store.MarkAllReadAsync(Org, mine));
        Assert.Equal(0, await _store.MarkAllReadAsync(Org, mine));
        Assert.Equal(2, await _store.CountUnreadAsync(Org, theirs));
    }

    private Task<int> CountRowsAsync() => _db.Notifications.AsNoTracking().CountAsync();
}
```

- [ ] **Step 2: รัน test เพื่อดูว่าล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~NotificationInfrastructureTests|FullyQualifiedName~RequestAccessResolverTests"`
Expected: FAIL (compile error: `ResolveMembershipAsync`, `NotificationPublisher`, `NotificationRecipientResolver`, `NotificationStore` ไม่มี).

- [ ] **Step 3: `ResolveMembershipAsync`**

`IRequestAccessResolver.cs` — เพิ่มเป็น default interface method (มี fake ใน unit test 9 ไฟล์ที่ implement interface นี้ จึงต้องไม่บังคับ; default ปิดประตู — fail closed):

```csharp
    /// <summary>
    /// Authenticates the membership only (active user/organization/branch/time window). No permission key: used by features whose
    /// access rule is "this row belongs to me" (notifications). The default fails closed; RequestAccessResolver overrides it.
    /// </summary>
    Task<Result<RequestAccessContext>> ResolveMembershipAsync(
        string firebaseUid,
        Guid membershipId,
        CancellationToken cancellationToken = default)
        => Task.FromResult(Result<RequestAccessContext>.Failure(
            new Error("ACTIVE_MEMBERSHIP_REQUIRED", "Active organization membership is required.")));
```

`RequestAccessResolver.cs` — เพิ่ม method (ใช้เงื่อนไข active membership เดียวกับ `ResolveAsync`; `PermissionKey` ว่างเพราะไม่มี permission):

```csharp
    public async Task<Result<RequestAccessContext>> ResolveMembershipAsync(
        string firebaseUid,
        Guid membershipId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(firebaseUid))
        {
            return Result<RequestAccessContext>.Failure(new Error("AUTHENTICATION_REQUIRED", "Authentication is required."));
        }

        var now = _clock.UtcNow;
        var context = await _db.Memberships
            .AsNoTracking()
            .Where(m => m.Id == membershipId && m.User!.FirebaseUid == firebaseUid)
            .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
            .Where(m => m.BranchId == null || m.Branch!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > now)
            .Select(m => new RequestAccessContext(m.UserId, m.Id, m.OrganizationId, m.BranchId, string.Empty, PermissionScope.Organization))
            .FirstOrDefaultAsync(cancellationToken);

        return context is null
            ? Result<RequestAccessContext>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED", "Active organization membership is required."))
            : Result<RequestAccessContext>.Success(context);
    }
```

- [ ] **Step 4: recipient resolver**

`backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationRecipientResolver.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Notifications;
using TanErp.Domain.IdentityAccess;

namespace TanErp.Infrastructure.Persistence.Notifications;

/// <summary>
/// Same grant rule as RequestAccessResolver.ResolveAsync (organization-scope grant on an active role/permission, active membership in the
/// time window), asked for every membership at once. A branch-limited membership only matches a document of its own branch.
/// Estimate's branch-scoped grants are not consulted: Estimate notifies its route reviewers explicitly.
/// </summary>
public class NotificationRecipientResolver : INotificationRecipientResolver
{
    private readonly AppDbContext _db;

    public NotificationRecipientResolver(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Guid>> ResolveAsync(
        Guid organizationId, Guid? branchId, string permissionKey, IReadOnlyCollection<Guid> excludedUserIds, DateTimeOffset atUtc, CancellationToken ct = default)
    {
        var excluded = excludedUserIds.ToArray();
        return await _db.Memberships
            .AsNoTracking()
            .Where(m => m.OrganizationId == organizationId)
            .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
            .Where(m => m.BranchId == null || m.Branch!.IsActive)
            .Where(m => branchId == null || m.BranchId == null || m.BranchId == branchId)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= atUtc)
            .Where(m => m.ExpiresAtUtc == null || m.ExpiresAtUtc > atUtc)
            .Where(m => !excluded.Contains(m.UserId))
            .Where(m => m.MembershipRoles.Any(mr => mr.Role!.IsActive
                && mr.Role.RolePermissions.Any(rp => rp.Permission!.IsActive
                    && rp.Permission.Key == permissionKey
                    && rp.Scope == PermissionScope.Organization
                    && rp.ScopeId == m.OrganizationId)))
            .Select(m => m.UserId)
            .Distinct()
            .OrderBy(id => id)
            // The planner caps at MaxRecipientsPerEvent after also dropping the actor, hence one spare row.
            .Take(NotificationLimits.MaxRecipientsPerEvent + 1)
            .ToListAsync(ct);
    }
}
```

- [ ] **Step 5: publisher**

`backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationPublisher.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;

namespace TanErp.Infrastructure.Persistence.Notifications;

/// <summary>
/// Stages notification rows in the shared AppDbContext. It never saves: the calling store's SaveChanges/transaction commits them with
/// the business change, and a failed change leaves nothing behind. Programming errors (unknown type, payload outside the allowlist,
/// unknown actor) throw NotificationDomainException so the business transaction fails loudly instead of silently losing a notification.
/// </summary>
public class NotificationPublisher : INotificationPublisher
{
    private readonly AppDbContext _db;
    private readonly INotificationRecipientResolver _recipients;
    private readonly IClock _clock;

    public NotificationPublisher(AppDbContext db, INotificationRecipientResolver recipients, IClock clock)
    {
        _db = db;
        _recipients = recipients;
        _clock = clock;
    }

    public async Task PublishAsync(NotificationEvent evt, CancellationToken ct = default)
    {
        var descriptor = NotificationTypeRegistry.Find(evt.Type)
            ?? throw new NotificationDomainException("NOTIFICATION_TYPE_INVALID", $"Notification type '{evt.Type}' is not registered.");

        var actorName = await _db.Users.AsNoTracking()
            .Where(u => u.Id == evt.ActorUserId)
            .Select(u => u.DisplayName)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotificationDomainException("NOTIFICATION_FIELD_INVALID", "The actor user was not found.");

        var now = _clock.UtcNow;
        IReadOnlyCollection<Guid> candidates = evt.ExplicitRecipientUserIds
            ?? await _recipients.ResolveAsync(evt.OrganizationId, evt.BranchId, descriptor.TargetPermission, evt.ExcludedUserIds, now, ct);

        var plan = NotificationPublishPlanner.Plan(evt, actorName, candidates);
        if (plan.IsFailure) throw new NotificationDomainException(plan.Error.Code, plan.Error.Message);
        if (plan.Value!.Count == 0) return;

        // Idempotent per (recipient, transition): skip rows already stored or already staged in this unit of work.
        var dedupeKey = plan.Value[0].DedupeKey;
        var recipientIds = plan.Value.Select(p => p.RecipientUserId).ToArray();
        var stored = await _db.Notifications.AsNoTracking()
            .Where(n => n.OrganizationId == evt.OrganizationId && n.DedupeKey == dedupeKey && recipientIds.Contains(n.RecipientUserId))
            .Select(n => n.RecipientUserId)
            .ToListAsync(ct);
        var staged = _db.ChangeTracker.Entries<Notification>()
            .Where(e => e.State == EntityState.Added && e.Entity.OrganizationId == evt.OrganizationId && e.Entity.DedupeKey == dedupeKey)
            .Select(e => e.Entity.RecipientUserId);
        var skip = new HashSet<Guid>(stored.Concat(staged));

        foreach (var item in plan.Value.Where(p => !skip.Contains(p.RecipientUserId)))
        {
            _db.Notifications.Add(new Notification(Guid.NewGuid(), evt.OrganizationId, item.RecipientUserId, item.Type, item.PayloadJson, item.DedupeKey, now));
        }
    }
}
```

- [ ] **Step 6: store**

`backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationStore.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Notifications;

namespace TanErp.Infrastructure.Persistence.Notifications;

/// <summary>Every query is keyed by (organization, recipient): another user's or organization's row is simply not found.</summary>
public class NotificationStore : INotificationStore
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public NotificationStore(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    private IQueryable<TanErp.Domain.Notifications.Notification> Own(Guid organizationId, Guid userId) =>
        _db.Notifications.Where(n => n.OrganizationId == organizationId && n.RecipientUserId == userId);

    public async Task<NotificationRowPage> ListAsync(Guid organizationId, Guid userId, NotificationListQuery query, CancellationToken ct = default)
    {
        var rows = Own(organizationId, userId).AsNoTracking();
        if (query.UnreadOnly) rows = rows.Where(n => n.ReadAtUtc == null);

        var total = await rows.CountAsync(ct);
        var items = await rows
            .OrderByDescending(n => n.CreatedAtUtc).ThenByDescending(n => n.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(n => new NotificationRow(n.Id, n.Type, n.PayloadJson, n.CreatedAtUtc, n.ReadAtUtc))
            .ToListAsync(ct);
        return new NotificationRowPage(items, total);
    }

    public Task<int> CountUnreadAsync(Guid organizationId, Guid userId, CancellationToken ct = default) =>
        Own(organizationId, userId).AsNoTracking().CountAsync(n => n.ReadAtUtc == null, ct);

    public async Task<NotificationRow?> MarkReadAsync(Guid organizationId, Guid userId, Guid notificationId, CancellationToken ct = default)
    {
        var now = _clock.UtcNow;
        // Only unread rows are touched, so the first read time survives repeated calls.
        await Own(organizationId, userId)
            .Where(n => n.Id == notificationId && n.ReadAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.ReadAtUtc, now), ct);

        return await Own(organizationId, userId).AsNoTracking()
            .Where(n => n.Id == notificationId)
            .Select(n => new NotificationRow(n.Id, n.Type, n.PayloadJson, n.CreatedAtUtc, n.ReadAtUtc))
            .FirstOrDefaultAsync(ct);
    }

    public Task<int> MarkAllReadAsync(Guid organizationId, Guid userId, CancellationToken ct = default)
    {
        var now = _clock.UtcNow;
        return Own(organizationId, userId)
            .Where(n => n.ReadAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.ReadAtUtc, now), ct);
    }
}
```

- [ ] **Step 7: DI**

Run: `grep -n "Notification" backend/src/TanErp.Api/Program.cs` → Expected: ไม่มีผลลัพธ์ (ถ้ามี ให้ใช้บรรทัดเดิม ห้ามลงทะเบียนซ้ำ).
ใน `Program.cs` ต่อท้ายกลุ่ม attachment (หลังบรรทัด `AddScoped<TanErp.Application.Attachments.IAttachmentStore, ...>` ที่ :183) เพิ่ม:

```csharp
builder.Services.AddScoped<TanErp.Application.Notifications.INotificationRecipientResolver, TanErp.Infrastructure.Persistence.Notifications.NotificationRecipientResolver>();
builder.Services.AddScoped<TanErp.Application.Notifications.INotificationPublisher, TanErp.Infrastructure.Persistence.Notifications.NotificationPublisher>();
builder.Services.AddScoped<TanErp.Application.Notifications.INotificationStore, TanErp.Infrastructure.Persistence.Notifications.NotificationStore>();
```

(`NotificationHandler` ลงทะเบียนใน Task 6.) ทุกตัว scoped เพื่อใช้ `AppDbContext` ตัวเดียวกับ module store ในหนึ่ง request.

- [ ] **Step 8: รัน test ให้ผ่าน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~NotificationInfrastructureTests|FullyQualifiedName~RequestAccessResolverTests"` → Expected: PASS.
Run: `dotnet build backend/TanErp.slnx --nologo -v q` → Expected: Build succeeded (unit test fakes ยังคอมไพล์ได้เพราะเป็น default interface method).
Run: `dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj` → Expected: PASS.

- [ ] **Step 9: commit**

```bash
git add backend/src/TanErp.Application/Common/Abstractions/IRequestAccessResolver.cs backend/src/TanErp.Infrastructure/Persistence/RequestAccessResolver.cs backend/src/TanErp.Infrastructure/Persistence/Notifications backend/src/TanErp.Api/Program.cs backend/tests/TanErp.IntegrationTests/Persistence/NotificationInfrastructureTests.cs backend/tests/TanErp.IntegrationTests/Persistence/RequestAccessResolverTests.cs
git commit -F - <<'EOF'
feat(notifications): add publisher, recipient resolver and own-only store

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 6: Application handler + API — `NOTIFICATION_NOT_FOUND`, contracts, `NotificationsController`, OpenAPI

**Files:**
- Create: `backend/src/TanErp.Application/Notifications/NotificationHandler.cs`
- Create: `backend/src/TanErp.Api/Contracts/Notifications/NotificationContracts.cs`
- Create: `backend/src/TanErp.Api/Controllers/NotificationsController.cs`
- Modify: `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`, `backend/src/TanErp.Api/Resources/Errors.resx`, `backend/src/TanErp.Api/Resources/Errors.en.resx`, `backend/src/TanErp.Api/Program.cs`
- Regenerate: `contracts/openapi/tan-erp.v1.json`
- Test: `backend/tests/TanErp.UnitTests/Notifications/NotificationHandlerTests.cs`, `backend/tests/TanErp.IntegrationTests/Api/NotificationEndpointsTests.cs` (สร้างในขั้นนี้; Task 10 เพิ่ม case ของ publish ต่อโมดูลลงไฟล์เดียวกัน), `backend/tests/TanErp.ArchitectureTests/LayerDependencyTests.cs`

**Idempotency-Key:** repo กำหนด `Idempotency-Key` ให้ POST ที่สร้าง/เปลี่ยนสถานะเอกสาร (73 จุดใน controllers; `RequestContextReader.ReadIdempotentRequest`). Mark read ไม่ใช่ทั้งสองอย่าง — idempotent โดยธรรมชาติ (อ่านซ้ำคืนแถวเดิม, `readAtUtc` ไม่เปลี่ยน) และตรงกับสัญญาใน Task 1 จึงใช้ `ReadAuthenticatedRequest` แบบเดียวกับ GET ของ `AttachmentsController` และมี test ยืนยันว่าเรียกโดยไม่ส่ง header ได้. ไม่ใช้ `If-Match` (ไม่มี concurrency token ที่ผู้ใช้ต้องรู้).

**ลำดับตรวจใน handler (permission ก่อน existence):** authentication/membership (`ResolveMembershipAsync`) → store ที่ค้นด้วย `(org, user, id)` พร้อมกัน → ไม่พบ/ของคนอื่น/ข้าม org = `NOTIFICATION_NOT_FOUND` (404 เดียวกัน). Deep link คำนวณตอนอ่านด้วย `ResolveAsync(target permission)` ของ type นั้น — ไม่มีสิทธิ์แล้ว → `null`.

- [ ] **Step 1: เขียน unit test ที่ล้มก่อน**

`backend/tests/TanErp.UnitTests/Notifications/NotificationHandlerTests.cs`:

```csharp
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationHandlerTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid User = Guid.NewGuid();
    private static readonly Guid ResourceId = Guid.NewGuid();
    private static readonly NotificationCaller Caller = new("uid-1", Guid.NewGuid(), "trace");

    private sealed class FakeAccess : IRequestAccessResolver
    {
        public bool MembershipActive { get; set; } = true;
        public HashSet<string> Granted { get; } = new(StringComparer.Ordinal);
        public List<string> Requested { get; } = new();

        public Task<Result<RequestAccessContext>> ResolveMembershipAsync(string firebaseUid, Guid membershipId, CancellationToken cancellationToken = default) =>
            Task.FromResult(MembershipActive
                ? Result<RequestAccessContext>.Success(new RequestAccessContext(User, membershipId, Org, null, string.Empty, "organization"))
                : Result<RequestAccessContext>.Failure(new Error("ACTIVE_MEMBERSHIP_REQUIRED", "inactive")));

        public Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string permissionKey, CancellationToken cancellationToken = default)
        {
            Requested.Add(permissionKey);
            return Task.FromResult(Granted.Contains(permissionKey)
                ? Result<RequestAccessContext>.Success(new RequestAccessContext(User, membershipId, Org, null, permissionKey, "organization"))
                : Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "denied")));
        }
    }

    private sealed class FakeStore : INotificationStore
    {
        public List<NotificationRow> Rows { get; } = new();
        public int Calls { get; private set; }
        public (Guid Org, Guid User)? LastOwner { get; private set; }
        public NotificationListQuery? LastQuery { get; private set; }

        public Task<NotificationRowPage> ListAsync(Guid organizationId, Guid userId, NotificationListQuery query, CancellationToken ct = default)
        {
            Calls++; LastOwner = (organizationId, userId); LastQuery = query;
            return Task.FromResult(new NotificationRowPage(Rows, Rows.Count + 40));
        }

        public Task<int> CountUnreadAsync(Guid organizationId, Guid userId, CancellationToken ct = default)
        {
            Calls++; LastOwner = (organizationId, userId);
            return Task.FromResult(3);
        }

        public Task<NotificationRow?> MarkReadAsync(Guid organizationId, Guid userId, Guid notificationId, CancellationToken ct = default)
        {
            Calls++; LastOwner = (organizationId, userId);
            return Task.FromResult(Rows.FirstOrDefault(r => r.Id == notificationId));
        }

        public Task<int> MarkAllReadAsync(Guid organizationId, Guid userId, CancellationToken ct = default)
        {
            Calls++; LastOwner = (organizationId, userId);
            return Task.FromResult(7);
        }
    }

    private static NotificationRow Row(string type, string payload) => new(Guid.NewGuid(), type, payload, DateTimeOffset.UtcNow, null);

    private static string PoPayload => $"{{\"resourceId\":\"{ResourceId}\",\"documentNumber\":\"PO-1\",\"actorDisplayName\":\"A\"}}";

    private static (NotificationHandler Handler, FakeAccess Access, FakeStore Store) Build() =>
        BuildWith(new FakeAccess(), new FakeStore());

    private static (NotificationHandler, FakeAccess, FakeStore) BuildWith(FakeAccess access, FakeStore store) => (new NotificationHandler(access, store), access, store);

    [Fact]
    public async Task EveryOperation_RequiresAnActiveMembership_BeforeTouchingTheStore()
    {
        var (handler, access, store) = Build();
        access.MembershipActive = false;

        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", (await handler.ListAsync(Caller, false, 1, 20)).Error.Code);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", (await handler.CountUnreadAsync(Caller)).Error.Code);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", (await handler.MarkReadAsync(Caller, Guid.NewGuid())).Error.Code);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", (await handler.MarkAllReadAsync(Caller)).Error.Code);
        Assert.Equal(0, store.Calls);
    }

    [Fact]
    public async Task List_UsesTheCallersOwnOrganizationAndUser_AndClampsPaging()
    {
        var (handler, _, store) = Build();

        var result = await handler.ListAsync(Caller, true, 0, 500);

        Assert.True(result.IsSuccess);
        Assert.Equal((Org, User), store.LastOwner);
        Assert.Equal(new NotificationListQuery(true, 1, NotificationLimits.MaxPageSize), store.LastQuery);
        Assert.Equal(1, result.Value!.Page);
        Assert.Equal(NotificationLimits.MaxPageSize, result.Value.PageSize);
        Assert.Equal(40, result.Value.TotalCount);
        Assert.Equal(1, result.Value.TotalPages);

        await handler.ListAsync(Caller, false, 3, 0);
        Assert.Equal(new NotificationListQuery(false, 3, NotificationLimits.DefaultPageSize), store.LastQuery);
    }

    [Fact]
    public async Task DeepLink_FollowsTheReadersCurrentPermission()
    {
        var (handler, access, store) = Build();
        store.Rows.Add(Row(NotificationTypes.PurchaseOrderApprovalRequested, PoPayload));
        store.Rows.Add(Row(NotificationTypes.PurchaseOrderApprovalRequested, PoPayload));

        var withoutPermission = await handler.ListAsync(Caller, false, 1, 20);
        Assert.All(withoutPermission.Value!.Items, i => Assert.Null(i.DeepLink));

        access.Granted.Add("purchase-orders.approve");
        access.Requested.Clear();
        var withPermission = await handler.ListAsync(Caller, false, 1, 20);

        Assert.All(withPermission.Value!.Items, i => Assert.Equal($"/procurement/purchase-orders/{ResourceId}", i.DeepLink));
        Assert.Single(access.Requested);                       // one permission lookup per distinct type, not per row
        Assert.Equal("PO-1", withPermission.Value.Items[0].Payload["documentNumber"]);
    }

    [Fact]
    public async Task ARowWhoseTypeIsNoLongerRegistered_IsStillListed_WithoutALink()
    {
        var (handler, _, store) = Build();
        store.Rows.Add(Row("retired.type", "{\"resourceId\":\"x\"}"));

        var result = await handler.ListAsync(Caller, false, 1, 20);

        Assert.Null(Assert.Single(result.Value!.Items).DeepLink);
    }

    [Fact]
    public async Task MarkRead_OfARowThatIsNotTheCallers_IsNotFound()
    {
        var (handler, _, store) = Build();

        var result = await handler.MarkReadAsync(Caller, Guid.NewGuid());

        Assert.Equal("NOTIFICATION_NOT_FOUND", result.Error.Code);
        Assert.Equal((Org, User), store.LastOwner);
    }

    [Fact]
    public async Task MarkRead_ReturnsTheProjection_AndCountsAreScopedToTheCaller()
    {
        var (handler, _, store) = Build();
        var row = Row(NotificationTypes.MrpRunApprovalRequested, $"{{\"resourceId\":\"{ResourceId}\",\"documentNumber\":\"M-1\",\"actorDisplayName\":\"A\"}}");
        store.Rows.Add(row);

        var read = await handler.MarkReadAsync(Caller, row.Id);
        Assert.Equal(row.Id, read.Value!.Id);
        Assert.Equal(3, (await handler.CountUnreadAsync(Caller)).Value);
        Assert.Equal(7, (await handler.MarkAllReadAsync(Caller)).Value);
        Assert.Equal((Org, User), store.LastOwner);
    }
}
```

`backend/tests/TanErp.ArchitectureTests/LayerDependencyTests.cs` — เพิ่ม rule (controller ต้องบาง: ไม่แตะ store/entity/Infrastructure):

```csharp
    [Fact]
    public void NotificationsController_ShouldStayThin()
    {
        var rule = Classes().That().HaveFullName("TanErp.Api.Controllers.NotificationsController")
            .Should().NotDependOnAny(Types().That().ResideInAssembly(InfrastructureAssembly))
            .AndShould().NotDependOnAny(Types().That().HaveFullName("TanErp.Application.Notifications.INotificationStore"))
            .AndShould().NotDependOnAny(Types().That().HaveFullName("TanErp.Domain.Notifications.Notification"));

        rule.Check(Architecture);
    }
```

`backend/tests/TanErp.IntegrationTests/Api/NotificationEndpointsTests.cs` (ใช้โครง factory เดียวกับ `AttachmentEndpointsTests`; ผู้ใช้ A = Test Admin ถือ `*.approve`, ผู้ใช้ B อยู่ Org B):

```csharp
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Notifications;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.Notifications;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class NotificationEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid UserA = TestOnlyDataSeeder.TestUserId;
    private static readonly Guid UserB = TestOnlyDataSeeder.TestUserIdB;
    private static readonly Guid PoId = Guid.NewGuid();
    private Guid[] _mineIds = [];
    private Guid _orgBRowId;

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                "token-org-a" => TestOnlyDataSeeder.TestFirebaseUid,
                "token-org-b" => TestOnlyDataSeeder.TestFirebaseUidB,
                _ => null
            });
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                ["Storage:BasePath"] = Path.Combine(Path.GetTempPath(), $"tan-erp-notif-{Guid.NewGuid():N}"),
                ["SeedTestData"] = "true"
            }));
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedItemCatalogDemoData: true);

        var t = DateTimeOffset.UtcNow;
        string Payload() => $"{{\"resourceId\":\"{PoId}\",\"documentNumber\":\"PO-1\",\"actorDisplayName\":\"Maker\"}}";
        var mine = Enumerable.Range(0, 3).Select(i => new Notification(
            Guid.NewGuid(), OrgId, UserA, NotificationTypes.PurchaseOrderApprovalRequested, Payload(), $"po:{i}", t.AddMinutes(i))).ToArray();
        var theirs = new Notification(Guid.NewGuid(), OrgId, UserB, NotificationTypes.PurchaseOrderApprovalRequested, Payload(), "po:other", t);
        var orgB = new Notification(Guid.NewGuid(), TestOnlyDataSeeder.TestOrgBId, UserB, NotificationTypes.PurchaseOrderApprovalRequested, Payload(), "po:orgb", t);
        db.Notifications.AddRange(mine);
        db.Notifications.AddRange(theirs, orgB);
        await db.SaveChangesAsync();
        _mineIds = mine.OrderByDescending(n => n.CreatedAtUtc).Select(n => n.Id).ToArray();
        _orgBRowId = orgB.Id;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, string? token = "token-org-a", Guid? membership = null)
    {
        var request = new HttpRequestMessage(method, url);
        if (token is not null) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var membershipId = membership ?? (token == "token-org-b" ? TestOnlyDataSeeder.TestMembershipBId : TestOnlyDataSeeder.TestMembershipId);
        request.Headers.Add("X-Membership-Id", membershipId.ToString());
        return await _client.SendAsync(request);
    }

    private static async Task<string> CodeAsync(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<ApiProblemDetails>())!.Code;

    [Fact]
    public async Task Requests_WithoutAuthentication_OrWithAnotherUsersMembership_AreRejected()
    {
        Assert.Equal(HttpStatusCode.Unauthorized, (await SendAsync(HttpMethod.Get, "/api/v1/notifications", token: null)).StatusCode);

        var foreign = await SendAsync(HttpMethod.Get, "/api/v1/notifications", membership: TestOnlyDataSeeder.TestMembershipBId);
        Assert.Equal(HttpStatusCode.Forbidden, foreign.StatusCode);
        Assert.Equal("ACTIVE_MEMBERSHIP_REQUIRED", await CodeAsync(foreign));
    }

    [Fact]
    public async Task List_ReturnsOnlyTheCallersRows_NewestFirst_WithDeepLinkAndNoRawIds()
    {
        var response = await SendAsync(HttpMethod.Get, "/api/v1/notifications?pageSize=500");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = (await response.Content.ReadFromJsonAsync<NotificationListResponse>())!;

        Assert.Equal(_mineIds, body.Items.Select(i => i.Id).ToArray());
        Assert.Equal(3, body.Pagination.TotalCount);
        Assert.Equal(50, body.Pagination.PageSize);
        Assert.All(body.Items, i => Assert.Equal($"/procurement/purchase-orders/{PoId}", i.DeepLink));
        Assert.DoesNotContain("recipient", await (await SendAsync(HttpMethod.Get, "/api/v1/notifications")).Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Paging_AndUnreadFilter_Work()
    {
        var page2 = (await (await SendAsync(HttpMethod.Get, "/api/v1/notifications?page=2&pageSize=2")).Content.ReadFromJsonAsync<NotificationListResponse>())!;
        Assert.Single(page2.Items);
        Assert.Equal(2, page2.Pagination.TotalPages);

        await SendAsync(HttpMethod.Post, $"/api/v1/notifications/{_mineIds[0]}/read");
        var unread = (await (await SendAsync(HttpMethod.Get, "/api/v1/notifications?unreadOnly=true")).Content.ReadFromJsonAsync<NotificationListResponse>())!;
        Assert.Equal(2, unread.Items.Count);
    }

    [Fact]
    public async Task MarkRead_WithoutIdempotencyKey_Works_AndIsIdempotent()
    {
        var first = await SendAsync(HttpMethod.Post, $"/api/v1/notifications/{_mineIds[0]}/read");
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        var a = (await first.Content.ReadFromJsonAsync<NotificationResponse>())!;
        var b = (await (await SendAsync(HttpMethod.Post, $"/api/v1/notifications/{_mineIds[0]}/read")).Content.ReadFromJsonAsync<NotificationResponse>())!;

        Assert.NotNull(a.ReadAtUtc);
        Assert.Equal(a.ReadAtUtc, b.ReadAtUtc);
        Assert.Equal(2, (await (await SendAsync(HttpMethod.Get, "/api/v1/notifications/unread-count")).Content.ReadFromJsonAsync<UnreadCountResponse>())!.UnreadCount);
    }

    [Fact]
    public async Task MarkRead_OfAnotherUsersOrOrganizationsOrUnknownRow_IsTheSame404()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var otherUsersRow = await db.Notifications.Where(n => n.DedupeKey == "po:other").Select(n => n.Id).SingleAsync();

        foreach (var id in new[] { otherUsersRow, _orgBRowId, Guid.NewGuid() })
        {
            var response = await SendAsync(HttpMethod.Post, $"/api/v1/notifications/{id}/read");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("NOTIFICATION_NOT_FOUND", await CodeAsync(response));
        }

        Assert.Null((await db.Notifications.AsNoTracking().SingleAsync(n => n.Id == otherUsersRow)).ReadAtUtc);
    }

    [Fact]
    public async Task ReadAll_OnlyTouchesTheCallersRowsInTheCurrentOrganization()
    {
        var response = await SendAsync(HttpMethod.Post, "/api/v1/notifications/read-all");
        Assert.Equal(3, (await response.Content.ReadFromJsonAsync<MarkAllReadResponse>())!.UpdatedCount);

        var b = await SendAsync(HttpMethod.Get, "/api/v1/notifications/unread-count", "token-org-b");
        Assert.Equal(1, (await b.Content.ReadFromJsonAsync<UnreadCountResponse>())!.UnreadCount);   // user B's Org B row only
        Assert.Equal(0, (await (await SendAsync(HttpMethod.Get, "/api/v1/notifications/unread-count")).Content.ReadFromJsonAsync<UnreadCountResponse>())!.UnreadCount);
    }
}
```

หมายเหตุ: `TestMembershipBId` เป็น membership ของ `TestUserIdB` ใน Org B (ตรวจ `TestOnlyDataSeeder` ก่อนเขียน; ถ้า user B ไม่มี membership ใน Org A การที่แถว `po:other` ของ B ใน Org A ไม่ขัด FK เพราะ FK ผูกกับ users/organizations แยกกัน).

- [ ] **Step 2: รัน test เพื่อดูว่าล้ม**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~NotificationHandlerTests"`
Expected: FAIL (compile error: `NotificationHandler` ไม่มี). Integration/Architecture ล้มด้วยเหตุผลเดียวกัน (ยังไม่มี controller/contracts).

- [ ] **Step 3: implement handler**

`backend/src/TanErp.Application/Notifications/NotificationHandler.cs`:

```csharp
using System.Text.Json;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Notifications;

/// <summary>
/// A user's own notifications. Order of checks: active membership (401/403) → store keyed by (organization, user) → 404 for anything
/// that is not the caller's. There is no permission key: access is "the row is mine". Deep links follow the reader's current permission.
/// </summary>
public class NotificationHandler
{
    private readonly IRequestAccessResolver _access;
    private readonly INotificationStore _store;

    public NotificationHandler(IRequestAccessResolver access, INotificationStore store)
    {
        _access = access;
        _store = store;
    }

    private Task<Result<RequestAccessContext>> OwnerAsync(NotificationCaller caller, CancellationToken ct) =>
        _access.ResolveMembershipAsync(caller.FirebaseUid, caller.MembershipId, ct);

    public async Task<Result<NotificationPage>> ListAsync(NotificationCaller caller, bool unreadOnly, int page, int pageSize, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(caller, ct);
        if (owner.IsFailure) return Result<NotificationPage>.Failure(owner.Error);

        var normalizedPage = Math.Max(page, 1);
        var size = pageSize < 1 ? NotificationLimits.DefaultPageSize : Math.Min(pageSize, NotificationLimits.MaxPageSize);
        var rows = await _store.ListAsync(owner.Value!.OrganizationId, owner.Value.ActorUserId, new NotificationListQuery(unreadOnly, normalizedPage, size), ct);

        var granted = new Dictionary<string, bool>(StringComparer.Ordinal);
        var items = new List<NotificationProjection>(rows.Items.Count);
        foreach (var row in rows.Items) items.Add(await ProjectAsync(caller, row, granted, ct));

        var totalPages = (int)Math.Ceiling(rows.TotalCount / (double)size);
        return Result<NotificationPage>.Success(new NotificationPage(items, normalizedPage, size, rows.TotalCount, totalPages));
    }

    public async Task<Result<int>> CountUnreadAsync(NotificationCaller caller, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(caller, ct);
        if (owner.IsFailure) return Result<int>.Failure(owner.Error);
        return Result<int>.Success(await _store.CountUnreadAsync(owner.Value!.OrganizationId, owner.Value.ActorUserId, ct));
    }

    public async Task<Result<NotificationProjection>> MarkReadAsync(NotificationCaller caller, Guid notificationId, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(caller, ct);
        if (owner.IsFailure) return Result<NotificationProjection>.Failure(owner.Error);

        var row = await _store.MarkReadAsync(owner.Value!.OrganizationId, owner.Value.ActorUserId, notificationId, ct);
        if (row is null) return Result<NotificationProjection>.Failure(new Error("NOTIFICATION_NOT_FOUND", "Notification not found."));
        return Result<NotificationProjection>.Success(await ProjectAsync(caller, row, new Dictionary<string, bool>(StringComparer.Ordinal), ct));
    }

    public async Task<Result<int>> MarkAllReadAsync(NotificationCaller caller, CancellationToken ct = default)
    {
        var owner = await OwnerAsync(caller, ct);
        if (owner.IsFailure) return Result<int>.Failure(owner.Error);
        return Result<int>.Success(await _store.MarkAllReadAsync(owner.Value!.OrganizationId, owner.Value.ActorUserId, ct));
    }

    private async Task<NotificationProjection> ProjectAsync(NotificationCaller caller, NotificationRow row, Dictionary<string, bool> granted, CancellationToken ct)
    {
        // Stored payloads passed the allowlist when written and the column is a jsonb object, so a flat string map is the contract.
        var payload = JsonSerializer.Deserialize<Dictionary<string, string>>(row.PayloadJson)
            ?? throw new InvalidOperationException($"Notification {row.Id} has a null payload.");

        string? deepLink = null;
        var descriptor = NotificationTypeRegistry.Find(row.Type);
        if (descriptor is not null)
        {
            if (!granted.TryGetValue(descriptor.TargetPermission, out var allowed))
            {
                allowed = (await _access.ResolveAsync(caller.FirebaseUid, caller.MembershipId, descriptor.TargetPermission, ct)).IsSuccess;
                granted[descriptor.TargetPermission] = allowed;
            }

            if (allowed) deepLink = NotificationTypeRegistry.RenderDeepLink(descriptor, payload);
        }

        return new NotificationProjection(row.Id, row.Type, payload, deepLink, row.CreatedAtUtc, row.ReadAtUtc);
    }
}
```

- [ ] **Step 4: error code, resx, contracts, controller, DI**

`ProblemDetailsMapper.cs` — ใต้บรรทัด `"ATTACHMENT_OWNER_LOCKED" => StatusCodes.Status409Conflict,` (:288) เพิ่ม:

```csharp
        "NOTIFICATION_NOT_FOUND" => StatusCodes.Status404NotFound,
```

`Errors.en.resx` และ `Errors.resx` — แทรกก่อน `</root>` (ใช้รูปแบบบรรทัดเดียวเหมือน `ATTACHMENT_*`):

```xml
  <data name="NOTIFICATION_NOT_FOUND_TITLE" xml:space="preserve"><value>Notification Not Found</value></data>
  <data name="NOTIFICATION_NOT_FOUND_DETAIL" xml:space="preserve"><value>The notification was not found.</value></data>
```

```xml
  <data name="NOTIFICATION_NOT_FOUND_TITLE" xml:space="preserve"><value>ไม่พบการแจ้งเตือน</value></data>
  <data name="NOTIFICATION_NOT_FOUND_DETAIL" xml:space="preserve"><value>ไม่พบการแจ้งเตือนที่ต้องการ</value></data>
```

(ตัวแรกลง `Errors.en.resx` ตัวหลังลง `Errors.resx`.)

`backend/src/TanErp.Api/Contracts/Notifications/NotificationContracts.cs`:

```csharp
using TanErp.Api.Contracts.Common;

namespace TanErp.Api.Contracts.Notifications;

/// <summary>One notification of the caller. <c>Payload</c> is display-safe strings only (document number, display names); <c>DeepLink</c> is null when the reader no longer holds the target permission.</summary>
public sealed record NotificationResponse(
    Guid Id,
    string Type,
    IReadOnlyDictionary<string, string> Payload,
    string? DeepLink,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ReadAtUtc);

public sealed record NotificationListResponse(IReadOnlyList<NotificationResponse> Items, PaginationMetadataResponse Pagination);

public sealed record UnreadCountResponse(int UnreadCount);

public sealed record MarkAllReadResponse(int UpdatedCount);
```

`backend/src/TanErp.Api/Controllers/NotificationsController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Common;
using TanErp.Api.Contracts.Notifications;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Notifications;

namespace TanErp.Api.Controllers;

/// <summary>The caller's own in-app notifications. No business logic lives here.</summary>
[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationHandler _handler;

    public NotificationsController(NotificationHandler handler)
    {
        _handler = handler;
    }

    private NotificationCaller? ReadCaller(out IActionResult? failure)
    {
        var authenticated = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authenticated.IsFailure)
        {
            failure = ProblemDetailsMapper.CreateProblemResult(authenticated.Error.Code, HttpContext);
            return null;
        }

        failure = null;
        return new NotificationCaller(authenticated.Value!.FirebaseUid, authenticated.Value.MembershipId, HttpContext.TraceIdentifier);
    }

    private IActionResult Problem(string code) => ProblemDetailsMapper.CreateProblemResult(code, HttpContext);

    private static NotificationResponse To(NotificationProjection p) => new(p.Id, p.Type, p.Payload, p.DeepLink, p.CreatedAtUtc, p.ReadAtUtc);

    [HttpGet]
    [ProducesResponseType<NotificationListResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List([FromQuery] bool unreadOnly = false, [FromQuery] int page = 1, [FromQuery] int pageSize = NotificationLimits.DefaultPageSize, CancellationToken ct = default)
    {
        var caller = ReadCaller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.ListAsync(caller, unreadOnly, page, pageSize, ct);
        if (result.IsFailure) return Problem(result.Error.Code);

        var paged = result.Value!;
        return Ok(new NotificationListResponse(
            paged.Items.Select(To).ToList(),
            new PaginationMetadataResponse(paged.Page, paged.PageSize, paged.TotalCount, paged.TotalPages)));
    }

    [HttpGet("unread-count")]
    [ProducesResponseType<UnreadCountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UnreadCount(CancellationToken ct)
    {
        var caller = ReadCaller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.CountUnreadAsync(caller, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new UnreadCountResponse(result.Value));
    }

    [HttpPost("{id:guid}/read")]
    [ProducesResponseType<NotificationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkRead([FromRoute] Guid id, CancellationToken ct)
    {
        var caller = ReadCaller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.MarkReadAsync(caller, id, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(To(result.Value!));
    }

    [HttpPost("read-all")]
    [ProducesResponseType<MarkAllReadResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        var caller = ReadCaller(out var failure);
        if (caller is null) return failure!;
        var result = await _handler.MarkAllReadAsync(caller, ct);
        return result.IsFailure ? Problem(result.Error.Code) : Ok(new MarkAllReadResponse(result.Value));
    }
}
```

`Program.cs` — ใต้สามบรรทัดที่เพิ่มใน Task 5 เพิ่ม (grep `NotificationHandler` ก่อน, คาดว่าไม่มี):

```csharp
builder.Services.AddScoped<TanErp.Application.Notifications.NotificationHandler>();
```

- [ ] **Step 5: รัน test ให้ผ่าน**

Run: `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~NotificationHandlerTests"` → Expected: PASS.
Run: `dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj` → Expected: PASS (4 tests).
Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~NotificationEndpointsTests"` → Expected: PASS ทั้ง 6 (ต้องมี Docker). ถ้า `NotificationEndpointsTests` ล้มตรง seed Org B ให้ตรวจ `TestOnlyDataSeeder` ว่า user B ผูกกับ Org B จริง.

- [ ] **Step 6: regenerate OpenAPI snapshot**

Run: `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"`
Run: `git diff --stat contracts/openapi/tan-erp.v1.json` → Expected: เพิ่มเท่านั้น (paths `/api/v1/notifications`, `/unread-count`, `/{id}/read`, `/read-all` + schemas `Notification*`, `UnreadCountResponse`, `MarkAllReadResponse`); `git diff contracts/openapi/tan-erp.v1.json | grep '^-[^-]' | wc -l` → Expected: `0` (ถ้ามีบรรทัดลบ แปลว่าแก้ contract เดิมโดยไม่ตั้งใจ — ตรวจก่อน).
Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"` (ไม่มี `UPDATE_OPENAPI`) → Expected: PASS.
Run: `dotnet build backend/TanErp.slnx --nologo -v q` → Expected: Build succeeded.

- [ ] **Step 7: commit**

```bash
git add backend/src/TanErp.Application/Notifications/NotificationHandler.cs backend/src/TanErp.Api/Contracts/Notifications backend/src/TanErp.Api/Controllers/NotificationsController.cs backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs backend/src/TanErp.Api/Resources backend/src/TanErp.Api/Program.cs contracts/openapi/tan-erp.v1.json backend/tests/TanErp.UnitTests/Notifications/NotificationHandlerTests.cs backend/tests/TanErp.IntegrationTests/Api/NotificationEndpointsTests.cs backend/tests/TanErp.ArchitectureTests/LayerDependencyTests.cs
git commit -F - <<'EOF'
feat(notifications): add notifications API with own-only read and mark-read

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 7: Event-source hooks — เรียก `INotificationPublisher` ใน transaction เดิมของ 6 แหล่ง

**Files:**
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Items/CostRecordStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Procurement/ProcurementStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Projects/ProjectControlStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Mrp/MrpStore.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/IdentityAdministrationStore.cs`
- Test: ไม่มีไฟล์ test ใหม่ในขั้นนี้ — พิสูจน์พฤติกรรมใน Task 8; ขั้นนี้พิสูจน์ว่าของเดิมไม่พังด้วย regression ต่อ store

**กติการ่วมทุกจุด (ตรวจแล้ว):**
1. เรียก `await _notifications.PublishAsync(NotificationEvents.X(...), ct)` **หลัง** เปลี่ยน state ของ entity และ `Audit`/`AddAudit`, **ก่อน** `SaveChangesAsync` ตัวที่ commit การเปลี่ยนแปลงนั้น — publisher แค่ `Add` เข้า `AppDbContext` เดียวกัน (scoped) จึงถูกบันทึกหรือ rollback พร้อมกัน.
2. ไม่เรียกในสาขาที่ return `Failure` ไปแล้ว และไม่ใส่ใน `catch`.
3. ฉีด `INotificationPublisher notifications` เป็น **พารามิเตอร์สุดท้าย** ของ constructor. ตรวจแล้ว: `grep -rnE 'new (EstimateStore|CostRecordStore|ProcurementStore|ProjectControlStore|MrpStore|IdentityAdministrationStore)\(' backend/src backend/tests` **ไม่พบเลย** — ทุก store สร้างผ่าน DI และ fake ใน unit test (เช่น `FakeEstimateStore` ใน `GetQuotationDocumentHandlerTests.cs`) implement interface ซึ่งไม่เปลี่ยน → **ไม่มี test ที่ต้องแก้ constructor**. `INotificationPublisher` ลงทะเบียน scoped แล้วใน Task 5 (`Program.cs`).
4. Payload ไม่มีตัวเลข/ราคา/ต้นทุน — factory ใน `NotificationEvents` (Task 3) ไม่มี parameter ที่รับตัวเลขได้.

- [ ] **Step 1: ยืนยัน baseline**

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet build backend/TanErp.slnx --nologo -v q` → Expected: Build succeeded (ถ้าแดงอยู่แล้ว ห้ามเริ่มแก้).

### 7.1 Estimate submit — `EstimateStore.SubmitAsync`

Hook point (ตรวจแล้ว): `EstimateStore.cs:648` เริ่ม `SubmitAsync`; transaction เปิดที่ `:655`; `estimate.SubmitCurrentRevision(...)` `~:758`; `_db.EstimateApprovalRequests.Add(request)` / `AddRange(steps)` `~:766–767`; `AddEstimateAudit(... "estimates.submitted" ...)` `~:768`; `_db.IdempotencyRecords.Add(...)` `~:772`; **`SaveChangesAsync` + `CommitAsync` `~:776–777` (SaveChanges ครั้งเดียวในเมธอด)** → วางหลัง `IdempotencyRecords.Add` ทันที.

Same-transaction: จริง. ถ้าชน unique 23505 (idempotency race) บล็อก `catch` เรียก `tx.RollbackAsync()` + `_db.ChangeTracker.Clear()` → notification ที่ stage ไว้หายพร้อมกัน แล้ว replay ผู้ชนะ (ผู้ชนะเป็นผู้ส่ง notification) จึงไม่ซ้ำ.

ผู้รับ = ผู้ตรวจลำดับแรก `steps[0]` (`Sequence = 1`; ตัดผู้ส่ง/ผู้แก้ล่าสุดแล้วโดย `FindIndependentReviewerAsync`). `TransitionId` = `request.Id` (สร้างใหม่ทุกครั้งที่ส่ง → ส่งใหม่หลัง return แจ้งอีกครั้ง).

`EstimateStore.cs`:

```csharp
using TanErp.Application.Notifications;
```

```csharp
    private readonly INotificationPublisher _notifications;

    public EstimateStore(
        AppDbContext db,
        IClock clock,
        IDocumentNumberGenerator documentNumberGenerator,
        ICostResolver costResolver,
        IConfiguration configuration,
        ILogger<EstimateStore> logger,
        INotificationPublisher notifications)
    {
        _notifications = notifications;
        _logger = logger;
```

(ที่เหลือของ ctor คงเดิม) ใน `SubmitAsync` ต่อจาก `_db.IdempotencyRecords.Add(new IdempotencyRecord(... estimate.Id.ToString(), now));` ก่อน `try { await _db.SaveChangesAsync(...)`:

```csharp
            await _notifications.PublishAsync(
                NotificationEvents.EstimateSubmitted(
                    organizationId, estimate.BranchId, request.Id, actorUserId, estimate.Id, estimate.Number, steps[0].ReviewerUserId),
                cancellationToken);
```

Regression: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateEndpointsTests" -m:1` → Expected: PASS เท่าเดิม (ต้องมี Docker).

### 7.2 Cost record submit — `CostRecordStore.SubmitAsync`

Hook point (ตรวจแล้ว): `CostRecordStore.cs:287` เริ่ม `SubmitAsync`; tx เปิด `:295`; `record.Submit(access.ActorUserId, now)` `~:331` (เปลี่ยน `RowVersion = Guid.NewGuid()`); `_db.AuditEvents.Add(audit)` `~:353`; **`SaveChangesAsync` `:355` + `CommitAsync`** → วางระหว่างสองบรรทัดนั้น. ทุก `return Failure` ก่อนหน้าเรียก `tx.RollbackAsync` แล้ว.

ผู้รับ = ผู้ถือ `cost-records.approve` ยกเว้น maker: `record.CreatedByUserId`, `record.LastFinancialEditorId` (domain `Approve` ห้ามสองคนนี้ที่ `CostRecord.cs:170`); planner ตัด actor ให้. `TransitionId` = `record.RowVersion` หลัง `Submit`. `documentNumber` = รหัสสินค้า (`Item.Code`).

`CostRecordStore.cs`:

```csharp
using TanErp.Application.Notifications;
```

```csharp
    private readonly AppDbContext _db;
    private readonly INotificationPublisher _notifications;

    public CostRecordStore(AppDbContext db, INotificationPublisher notifications)
    {
        _db = db;
        _notifications = notifications;
    }
```

ใน `SubmitAsync` หลัง `_db.AuditEvents.Add(audit);` ก่อน `await _db.SaveChangesAsync(ct);`:

```csharp
        var itemCode = await _db.Items.AsNoTracking()
            .Where(i => i.Id == itemId && i.OrganizationId == orgId)
            .Select(i => i.Code)
            .FirstAsync(ct);
        await _notifications.PublishAsync(
            NotificationEvents.CostRecordSubmitted(
                orgId, record.BranchId, record.RowVersion, access.ActorUserId, record.Id, itemCode,
                new[] { record.CreatedByUserId, record.LastFinancialEditorId }.Distinct().ToArray()),
            ct);
```

Regression: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~CostRecordEndpointsTests|FullyQualifiedName~ItemCatalogEstimateFlowTests" -m:1` → Expected: PASS.

### 7.3 Purchase Order submit — `ProcurementStore.PurchaseOrderActionAsync`

Hook point (ตรวจแล้ว): เมธอดเดียวรองรับ Submit/Cancel/Approve/Reject (`ProcurementStore.cs:329`); `case PurchaseOrderAction.Submit: order.Submit(now);` `~:347`; `Audit(...)` `~:372`; `SaveChangesAsync` `:373` + `CommitAsync` ใน `try` ที่จับ `DbUpdateConcurrencyException`. **publish เฉพาะ `action == PurchaseOrderAction.Submit`** — วางหลัง `Audit(...)` ก่อน `try`.

Same-transaction: จริง (SaveChanges ครั้งเดียว). `DbUpdateConcurrencyException` → `return Fail(...)` โดยไม่ commit → `await using tx` dispose = rollback.

ผู้รับ = ผู้ถือ `purchase-orders.approve` ยกเว้น `order.CreatedByUserId`. `TransitionId` = `order.RowVersion` หลัง `Submit`.

```csharp
using TanErp.Application.Notifications;
```

```csharp
    private readonly INotificationPublisher _notifications;

    public ProcurementStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers, INotificationPublisher notifications)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
        _notifications = notifications;
    }
```

```csharp
            if (action == PurchaseOrderAction.Submit)
            {
                await _notifications.PublishAsync(
                    NotificationEvents.PurchaseOrderSubmitted(
                        orgId, order.BranchId, order.RowVersion, access.ActorUserId, order.Id, order.Number, order.CreatedByUserId),
                    ct);
            }
```

Regression: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ProcurementEndpointsTests" -m:1` → Expected: PASS.

### 7.4 Change Order submit — `ProjectControlStore.ChangeOrderActionAsync`

Hook point (ตรวจแล้ว): `ProjectControlStore.cs:401`; `case ChangeOrderAction.Submit: order.Submit(now);` `:423`; `Audit(...)` `~:453`; `SaveChangesAsync` + `CommitAsync` ใน `try` `~:456–457`. วางหลัง `Audit(...)` เฉพาะ `action == ChangeOrderAction.Submit`. `project` ถูกโหลดแล้ว (`AsNoTracking`) → ใช้ `project.BranchId`.

Same-transaction: จริง (SaveChanges ครั้งเดียว; concurrency → return Fail โดยไม่ commit = rollback). ผู้รับ = ผู้ถือ `projects.change-orders.approve` ยกเว้น `order.CreatedByUserId` (domain `ProjectChangeOrder.cs:75`).

```csharp
using TanErp.Application.Notifications;
```

```csharp
    private readonly INotificationPublisher _notifications;

    public ProjectControlStore(AppDbContext db, IClock clock, IDocumentNumberGenerator documentNumberGenerator, INotificationPublisher notifications)
    {
        _db = db;
        _clock = clock;
        _documentNumberGenerator = documentNumberGenerator;
        _notifications = notifications;
    }
```

```csharp
            if (action == ChangeOrderAction.Submit)
            {
                await _notifications.PublishAsync(
                    NotificationEvents.ChangeOrderSubmitted(
                        orgId, project.BranchId, order.RowVersion, access.ActorUserId, order.Id, projectId, order.Number, order.CreatedByUserId),
                    ct);
            }
```

Regression: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ProjectControlEndpointsTests" -m:1` → Expected: PASS.

### 7.5 MRP run — `MrpStore.CreateRunAsync`

Hook point (ตรวจแล้ว): tx `RepeatableRead` เปิด `MrpStore.cs:135`; สร้าง `run` + `AddRecommendation` วน `plan.Orders` `~:169–175`; `_db.MrpRuns.Add(run)` `:177`; `Audit(... "mrp.run.created" ...)` `:178`; `IdempotencyRecords.Add` `:179`; **`SaveChangesAsync` `:180` + `CommitAsync` `:181`** → วางหลัง `IdempotencyRecords.Add` เฉพาะ `plan.Orders.Count > 0` (run ว่างไม่มีอะไรให้อนุมัติ). ผู้รับ = ผู้ถือ `mrp.approve` ยกเว้นผู้สร้าง run (planner ตัด actor). `TransitionId` = `run.Id`.

Same-transaction: จริง. query ของ publisher (`Users`, `Notifications`, `Memberships`) เป็นการอ่านใน snapshot `RepeatableRead` เดียวกัน ไม่เพิ่มความเสี่ยง serialization failure จากการเขียน.

```csharp
using TanErp.Application.Notifications;
```

```csharp
    private readonly INotificationPublisher _notifications;

    public MrpStore(AppDbContext db, IClock clock, IDocumentNumberGenerator numbers, INotificationPublisher notifications)
    {
        _db = db;
        _clock = clock;
        _numbers = numbers;
        _notifications = notifications;
    }
```

```csharp
            if (plan.Orders.Count > 0)
            {
                await _notifications.PublishAsync(
                    NotificationEvents.MrpRunCreated(orgId, access.BranchId.Value, run.Id, access.ActorUserId, number),
                    ct);
            }
```

Regression: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~MrpEndpointsTests" -m:1` → Expected: PASS.

### 7.6 Role assignment request — `IdentityAdministrationStore` (2 จุด)

**ข้อควรระวัง (transaction boundary ไม่ตรงไปตรงมา):** store นี้ไม่เรียก `BeginTransactionAsync` ในเมธอดเอง แต่ทุกเมธอดห่อด้วย `RunAsync` (`IdentityAdministrationStore.cs:463`) ซึ่งเปิด transaction `Serializable`, เรียก `_db.ChangeTracker.Clear()` ก่อนทุก attempt (retry ล้าง notification ที่ stage ค้าง), commit เมื่อ result สำเร็จ และ rollback เมื่อ failure/exception. ดังนั้น "ใน transaction เดียวกัน" เป็นจริง **เฉพาะเมื่อ publish อยู่ภายในแลมบ์ดาที่ส่งให้ `RunAsync`** และก่อน `SaveChangesAsync` ของสาขานั้น — ห้ามย้ายไปไว้หลัง `ReloadUserAsync`/นอกแลมบ์ดา. `CreateUserAsync` มี `SaveChangesAsync` เดียว (`:177`) ใน `try` ที่จับ unique violation → return Fail → `RunAsync` rollback. `AssignRoleAsync` สาขา `RequiresApproval` save ที่ `:337` (สาขา assign ตรง `:352` ไม่แจ้ง).

ผู้รับ = ผู้ถือ `roles.assign-approval` ยกเว้นผู้ขอ (actor) และ user เป้าหมาย. `TransitionId` = `request.Id` (หนึ่ง request = หนึ่ง event; สร้าง user หลาย role ที่ต้องอนุมัติ → หลาย event, dedupe key ต่างกัน).

```csharp
using TanErp.Application.Notifications;
```

```csharp
    private readonly INotificationPublisher _notifications;

    public IdentityAdministrationStore(AppDbContext db, IClock clock, INotificationPublisher notifications)
    {
        _db = db;
        _clock = clock;
        _notifications = notifications;
    }
```

(a) `CreateUserAsync` ในลูป `foreach (var role in roles)` สาขา `role.RequiresApproval` หลัง `_db.RoleAssignmentRequests.Add(request);` (`~:154`):

```csharp
                    await _notifications.PublishAsync(
                        NotificationEvents.RoleAssignmentRequested(
                            organizationId, request.Id, actor.UserId, user.Id, input.DisplayName, role.Name),
                        cancellationToken);
```

(b) `AssignRoleAsync` สาขา `if (role.RequiresApproval)` หลัง `_db.IdempotencyRecords.Add(...)` ก่อน `await _db.SaveChangesAsync(cancellationToken);` (`:337`):

```csharp
                var subjectName = await _db.Users.AsNoTracking()
                    .Where(u => u.Id == membership.UserId)
                    .Select(u => u.DisplayName)
                    .FirstAsync(cancellationToken);
                await _notifications.PublishAsync(
                    NotificationEvents.RoleAssignmentRequested(
                        organizationId, request.Id, actor.UserId, membership.UserId, subjectName, role.Name),
                    cancellationToken);
```

Regression: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~IdentityAdministrationEndpointsTests|FullyQualifiedName~UsersEndpointsTests" -m:1` → Expected: PASS.

- [ ] **Step 2: build + กฎสถาปัตยกรรม**

Run: `dotnet build backend/TanErp.slnx --nologo -v q` → Expected: Build succeeded (ถ้ามี `new Store(` ที่ grep พลาด จะแดงตรงนี้ → เพิ่ม argument).
Run: `dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj` → Expected: PASS.

- [ ] **Step 3: regression ทั้ง 6 กลุ่มพร้อมกัน**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateEndpointsTests|FullyQualifiedName~CostRecordEndpointsTests|FullyQualifiedName~ProcurementEndpointsTests|FullyQualifiedName~ProjectControlEndpointsTests|FullyQualifiedName~MrpEndpointsTests|FullyQualifiedName~IdentityAdministrationEndpointsTests" -m:1` → Expected: PASS ทั้งหมด.

- [ ] **Step 4: commit**

```bash
git add backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs backend/src/TanErp.Infrastructure/Persistence/Items/CostRecordStore.cs backend/src/TanErp.Infrastructure/Persistence/Procurement/ProcurementStore.cs backend/src/TanErp.Infrastructure/Persistence/Projects/ProjectControlStore.cs backend/src/TanErp.Infrastructure/Persistence/Mrp/MrpStore.cs backend/src/TanErp.Infrastructure/Persistence/IdentityAccess/IdentityAdministrationStore.cs
git commit -F - <<'EOF'
feat(notifications): publish approval-requested notifications in the source transaction

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 8: Integration tests — publish ใน transaction เดียวกัน, rollback, scope ผู้รับ, own-only end-to-end

**Files:**
- Modify: `backend/tests/TanErp.IntegrationTests/Api/NotificationEndpointsTests.cs` (**สร้างแล้วใน Task 6 — แก้ด้วย Edit/แทรก ห้ามสร้างใหม่**)
- Modify: `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs` (เพิ่ม 1 test: Estimate ต้องใช้ helper `SetupCalculatedEstimateAsync` ที่เป็น `private` ของคลาสนั้น จึงเขียนในไฟล์นั้นเพื่อไม่ต้องคัดลอก ~300 บรรทัด setup — ความต่างจากข้อกำหนดเดิมที่ให้ทุก test อยู่ใน `NotificationEndpointsTests`)

**แหล่ง event ที่ใช้ใน `NotificationEndpointsTests`: Purchase Order submit** (flow สั้นที่สุดที่ขับผ่าน HTTP ได้: supplier → PO → `POST /purchase-orders/{id}/submit`; ผู้สร้างอนุมัติเองไม่ได้ จึงมี maker–checker จริง). ส่วน Estimate (ผู้รับแบบ explicit reviewer) อยู่ใน Step 4.

**ข้อเท็จจริงจาก seed (ตรวจแล้ว):** `Test Admin` role ถือ `*.approve` ครบ (`TestOnlyDataSeeder.cs:150–225`) → ผู้ใช้ A (`TestUserId`) เป็นทั้ง maker และเป็น "ผู้ถือ permission" ที่ต้องถูกตัดออก; `TestUserIdB` มี membership ใน Org A คือ `TestCostReviewerMembershipId` (ใช้เป็นผู้ตรวจ Estimate ใน `EstimateEndpointsTests`); ผู้อนุมัติ PO ที่ไม่ใช่ maker ต้องสร้างเอง (รูปแบบเดียวกับ `ProcurementEndpointsTests.cs:94–97`).

- [ ] **Step 1: เตรียม fixture ใน `NotificationEndpointsTests`** (แก้ของเดิมจาก Task 6)

1a. เพิ่ม using และ interceptor ท้ายไฟล์ (นอกคลาส, namespace เดียวกัน) — ใช้พิสูจน์ rollback โดยทำให้ `SaveChangesAsync` ล้ม *หลัง* publisher stage แถวแล้ว:

```csharp
using Microsoft.EntityFrameworkCore.Diagnostics;
using TanErp.Api.Contracts.Procurement;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.Procurement;
```

```csharp
/// <summary>Fails the next save that contains a staged notification, so the business change must roll back with it.</summary>
public sealed class FailNextNotificationSaveInterceptor : SaveChangesInterceptor
{
    private static int _armed;

    public static void Arm() => Interlocked.Exchange(ref _armed, 1);

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var staged = eventData.Context!.ChangeTracker.Entries<Notification>().Any(e => e.State == EntityState.Added);
        if (staged && Interlocked.Exchange(ref _armed, 0) == 1)
            throw new DbUpdateConcurrencyException("Forced failure after notifications were staged.");
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

1b. ในคลาส: เพิ่ม field และ token ใหม่ (`token-approver`, `token-noperm`, `token-inactive`):

```csharp
    private const string UidApprover = "uid-notif-approver";
    private const string UidNoPerm = "uid-notif-noperm";
    private const string UidInactive = "uid-notif-inactive";
    private static readonly Guid ApproverUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b01");
    private static readonly Guid ApproverMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b02");
    private static readonly Guid NoPermUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b03");
    private static readonly Guid NoPermMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b04");
    private static readonly Guid InactiveUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b05");
    private static readonly Guid InactiveMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f5b06");
```

แก้ `TestFirebaseTokenVerifier` ให้ switch มี 3 case เพิ่ม:

```csharp
                "token-approver" => UidApprover,
                "token-noperm" => UidNoPerm,
                "token-inactive" => UidInactive,
```

ใน `builder.ConfigureServices(...)` เพิ่มบรรทัด (EF Core หยิบ `IInterceptor` ที่ลงทะเบียนใน application service provider):

```csharp
                services.AddSingleton<Microsoft.EntityFrameworkCore.Diagnostics.IInterceptor, FailNextNotificationSaveInterceptor>();
```

ใน `InitializeAsync` ก่อน `var t = DateTimeOffset.UtcNow;` เพิ่ม:

```csharp
        var adminRole = await db.Roles.SingleAsync(r => r.OrganizationId == OrgId && r.Name == "Test Admin");
        db.Users.Add(new User(ApproverUserId, UidApprover, "notif-approver@example.test", "Notif Approver", true));
        db.Memberships.Add(new Membership(ApproverMembershipId, OrgId, TestOnlyDataSeeder.TestBranchId, ApproverUserId, isActive: true));
        db.MembershipRoles.Add(new MembershipRole(ApproverMembershipId, adminRole.Id, OrgId));
        // Member with no role at all: reachable by the API, holds no approve permission.
        db.Users.Add(new User(NoPermUserId, UidNoPerm, "notif-noperm@example.test", "Notif NoPerm", true));
        db.Memberships.Add(new Membership(NoPermMembershipId, OrgId, TestOnlyDataSeeder.TestBranchId, NoPermUserId, isActive: true));
        // Holds the admin role but the membership is inactive: must never be a recipient.
        db.Users.Add(new User(InactiveUserId, UidInactive, "notif-inactive@example.test", "Notif Inactive", true));
        db.Memberships.Add(new Membership(InactiveMembershipId, OrgId, TestOnlyDataSeeder.TestBranchId, InactiveUserId, isActive: false));
        db.MembershipRoles.Add(new MembershipRole(InactiveMembershipId, adminRole.Id, OrgId));
        await db.SaveChangesAsync();
```

> หมายเหตุ: ผู้ใช้ B/Org B ที่ seed ใน Task 6 ไม่ถูกแตะ. การเพิ่ม `Approver` ไม่กระทบ test เดิมของ Task 6 (แถวที่ seed ไว้ผูก `UserA`/`UserB` โดยตรง ไม่ผ่าน publisher).

- [ ] **Step 2: เขียน test ที่ล้มก่อน** (แทรกต่อจากเมธอด `CodeAsync` ของคลาสเดิม)

```csharp
    private async Task<HttpResponseMessage> SendJsonAsync(
        HttpMethod method, string url, object? body, string token, Guid membership, string? key = null, Guid? ifMatch = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", membership.ToString());
        if (key is not null) request.Headers.Add("Idempotency-Key", key);
        if (ifMatch.HasValue) request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        if (body is not null) request.Content = JsonContent.Create(body);
        return await _client.SendAsync(request);
    }

    private static readonly Guid MembershipA = TestOnlyDataSeeder.TestMembershipId;

    private async Task<PurchaseOrderResponse> CreateDraftPurchaseOrderAsync()
    {
        var supplier = await SendJsonAsync(HttpMethod.Post, "/api/v1/suppliers",
            new SupplierRequest("บริษัท ไม้ดี จำกัด", "Good Wood", "0105500000001", "คุณขาย", "021234567", "sales@example.test", 30),
            "token-org-a", MembershipA, key: Guid.NewGuid().ToString("N"));
        var supplierId = (await supplier.Content.ReadFromJsonAsync<SupplierResponse>())!.Id;
        var created = await SendJsonAsync(HttpMethod.Post, "/api/v1/purchase-orders",
            new PurchaseOrderRequest(supplierId, null, new DateOnly(2026, 11, 15), "ส่งหน้างาน",
                new List<PurchaseOrderLineRequest> { new(TestOnlyDataSeeder.TestItemCatalogPlywoodId, 10m, 1250m) }),
            "token-org-a", MembershipA, key: Guid.NewGuid().ToString("N"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        return (await created.Content.ReadFromJsonAsync<PurchaseOrderResponse>())!;
    }

    private Task<HttpResponseMessage> SubmitAsync(PurchaseOrderResponse order) =>
        SendJsonAsync(HttpMethod.Post, $"/api/v1/purchase-orders/{order.Id}/submit", new PurchaseOrderActionRequest(null),
            "token-org-a", MembershipA, ifMatch: order.RowVersion);

    private async Task<NotificationListResponse> ListAsync(string token, Guid membership, string query = "")
    {
        var response = await SendJsonAsync(HttpMethod.Get, $"/api/v1/notifications{query}", null, token, membership);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<NotificationListResponse>())!;
    }

    [Fact]
    public async Task SubmitPurchaseOrder_NotifiesTheOtherApproverOnce_WithPayloadFreeOfFiguresAndPii_AndNotTheMaker()
    {
        var order = await CreateDraftPurchaseOrderAsync();
        Assert.Equal(HttpStatusCode.OK, (await SubmitAsync(order)).StatusCode);

        var approver = await ListAsync("token-approver", ApproverMembershipId);
        var item = Assert.Single(approver.Items);
        Assert.Equal(NotificationTypes.PurchaseOrderApprovalRequested, item.Type);
        Assert.Equal($"/procurement/purchase-orders/{order.Id}", item.DeepLink);
        Assert.Equal(
            new[] { "actorDisplayName", "documentNumber", "resourceId" },
            item.Payload.Keys.OrderBy(k => k, StringComparer.Ordinal).ToArray());
        Assert.Equal(order.Number, item.Payload["documentNumber"]);
        Assert.Equal(TestOnlyDataSeeder.TestUserDisplayName, item.Payload["actorDisplayName"]);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var stored = await db.Notifications.AsNoTracking()
            .Where(n => n.Type == NotificationTypes.PurchaseOrderApprovalRequested && n.DedupeKey.StartsWith("purchase-order.approval-requested:"))
            .ToListAsync();
        var row = Assert.Single(stored);
        Assert.Equal(ApproverUserId, row.RecipientUserId);
        foreach (var forbidden in new[] { "1250", "12500", "sales@example.test", "0105500000001", "021234567", TestOnlyDataSeeder.TestUserEmail })
        {
            Assert.DoesNotContain(forbidden, row.PayloadJson.Replace(order.Id.ToString(), string.Empty), StringComparison.Ordinal);
        }

        // Maker, a member without the permission and an inactive holder of the permission get nothing.
        Assert.Empty((await ListAsync("token-org-a", MembershipA, "?unreadOnly=true")).Items.Where(i => i.Type == NotificationTypes.PurchaseOrderApprovalRequested && i.Payload["resourceId"] == order.Id.ToString()));
        Assert.Empty((await ListAsync("token-noperm", NoPermMembershipId)).Items);
        Assert.False(await db.Notifications.AnyAsync(n => n.RecipientUserId == InactiveUserId));
    }

    [Fact]
    public async Task SubmitPurchaseOrder_WhenTheSaveFails_LeavesNoNotificationAndNoStatusChange_AndRetryNotifiesOnce()
    {
        var order = await CreateDraftPurchaseOrderAsync();

        FailNextNotificationSaveInterceptor.Arm();
        var failed = await SubmitAsync(order);
        Assert.Equal(HttpStatusCode.Conflict, failed.StatusCode);
        Assert.Equal("PURCHASE_ORDER_VERSION_CONFLICT", await CodeAsync(failed));

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.Equal(PurchaseOrderStatus.Draft, (await db.PurchaseOrders.AsNoTracking().SingleAsync(o => o.Id == order.Id)).Status);
            Assert.False(await db.Notifications.AnyAsync(n => n.RecipientUserId == ApproverUserId));
        }

        Assert.Equal(HttpStatusCode.OK, (await SubmitAsync(order)).StatusCode);
        Assert.Single((await ListAsync("token-approver", ApproverMembershipId)).Items);
    }

    [Fact]
    public async Task OwnOnly_TheMakerNeverSeesTheApproversRow_AndAnotherOrganizationGets404()
    {
        var order = await CreateDraftPurchaseOrderAsync();
        await SubmitAsync(order);
        var approverRowId = Assert.Single((await ListAsync("token-approver", ApproverMembershipId)).Items).Id;

        Assert.DoesNotContain(approverRowId, (await ListAsync("token-org-a", MembershipA, "?pageSize=50")).Items.Select(i => i.Id));

        foreach (var (token, membership) in new[] { ("token-org-a", MembershipA), ("token-org-b", TestOnlyDataSeeder.TestMembershipBId) })
        {
            var response = await SendJsonAsync(HttpMethod.Post, $"/api/v1/notifications/{approverRowId}/read", null, token, membership);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("NOTIFICATION_NOT_FOUND", await CodeAsync(response));
        }

        Assert.Null(Assert.Single((await ListAsync("token-approver", ApproverMembershipId)).Items).ReadAtUtc);
    }

    [Fact]
    public async Task UnreadCount_DecrementsOnMarkRead_AndReadAllClearsOnlyTheCallersRows()
    {
        foreach (var _ in Enumerable.Range(0, 2))
        {
            await SubmitAsync(await CreateDraftPurchaseOrderAsync());
        }

        async Task<int> UnreadAsync(string token, Guid membership) =>
            (await (await SendJsonAsync(HttpMethod.Get, "/api/v1/notifications/unread-count", null, token, membership))
                .Content.ReadFromJsonAsync<UnreadCountResponse>())!.UnreadCount;

        Assert.Equal(2, await UnreadAsync("token-approver", ApproverMembershipId));
        var first = (await ListAsync("token-approver", ApproverMembershipId)).Items[0].Id;

        Assert.Equal(HttpStatusCode.OK, (await SendJsonAsync(HttpMethod.Post, $"/api/v1/notifications/{first}/read", null, "token-approver", ApproverMembershipId)).StatusCode);
        Assert.Equal(1, await UnreadAsync("token-approver", ApproverMembershipId));

        var readAll = await SendJsonAsync(HttpMethod.Post, "/api/v1/notifications/read-all", null, "token-approver", ApproverMembershipId);
        Assert.Equal(1, (await readAll.Content.ReadFromJsonAsync<MarkAllReadResponse>())!.UpdatedCount);
        Assert.Equal(0, await UnreadAsync("token-approver", ApproverMembershipId));
        // UserA's three seeded rows (Task 6 fixture) are untouched by the approver's read-all.
        Assert.Equal(3, await UnreadAsync("token-org-a", MembershipA));
    }
```

- [ ] **Step 3: รันให้ล้มก่อน (ยังไม่มีการ publish ถ้ายังไม่ได้ทำ Task 7)**

ลำดับงานจริง: Task 7 เสร็จก่อนแล้ว จึง **คาดว่า test ผ่านทันที**; เพื่อยืนยันว่า test จับของจริง ให้ทำ mutation ใน Step 6 แทนการ revert Task 7. ถ้ารันที่นี่แล้วล้ม ให้ตรวจ:
- `FailNextNotificationSaveInterceptor` ไม่ทำงาน (rollback test ได้ 200 แทน 409) → EF ไม่หยิบ `IInterceptor` จาก DI; แก้โดยลงทะเบียนใน `ConfigureServices` ด้วย `services.AddDbContext<AppDbContext>` ซ้ำเฉพาะ test ไม่ได้ (จะทับ options) — ให้ใช้ `services.ConfigureDbContext<AppDbContext>(o => o.AddInterceptors(new FailNextNotificationSaveInterceptor()))` แทน.
- `Assert.Equal(3, await UnreadAsync("token-org-a", ...))` ล้ม → ตรวจว่า Task 6 seed UserA ไว้ 3 แถวและไม่มี test ใดใน class เดียวกัน mark read ไว้ (xUnit สร้าง instance ใหม่ + container ใหม่ต่อ test จึงแยกกัน).

Run: `cd /Users/syaco/Documents/development/tan-erp && dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~NotificationEndpointsTests" -m:1`
Expected: PASS ทั้ง 10 (6 จาก Task 6 + 4 ใหม่) — ต้องมี Docker.

- [ ] **Step 4: test Estimate (ผู้รับแบบ explicit reviewer) ใน `EstimateEndpointsTests`**

เพิ่ม `using TanErp.Domain.Notifications;` และ test (ท้ายกลุ่ม `Submit_*`). ผู้ตรวจ default policy คือ `TestUserIdB` (membership `TestCostReviewerMembershipId`) ตามที่ `EstimateEndpointsTests.cs:972–980` ใช้อ่าน review queue:

```csharp
    [Fact]
    public async Task Submit_NotifiesOnlyTheFirstReviewer_WithPayloadFreeOfFigures()
    {
        var (estimate, _, calculatedRevision) = await SetupCalculatedEstimateAsync($"notify-{Guid.NewGuid():N}");
        var current = await GetEstimateAsync(estimate.Id);

        var submitRequest = CreateAuthenticatedRequest(
            HttpMethod.Post, $"/api/v1/estimates/{estimate.Id}/submit", "token-org-a", MembershipAId);
        submitRequest.Headers.Add("If-Match", $"\"{current.RowVersion}\"");
        submitRequest.Headers.Add("Idempotency-Key", $"idemp-notify-{Guid.NewGuid():N}");
        submitRequest.Content = JsonContent.Create(new SubmitEstimateRequest(1, calculatedRevision.CalculationVersion, null));
        var submitted = await _client.SendAsync(submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitted.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var rows = await db.Notifications.AsNoTracking()
            .Where(n => n.Type == NotificationTypes.EstimateApprovalRequested)
            .ToListAsync();
        var row = Assert.Single(rows);
        Assert.Equal(TestOnlyDataSeeder.TestUserIdB, row.RecipientUserId);
        Assert.NotEqual(UserAId, row.RecipientUserId);
        Assert.Contains(current.Number, row.PayloadJson, StringComparison.Ordinal);
        using var payload = System.Text.Json.JsonDocument.Parse(row.PayloadJson);
        Assert.Equal(
            new[] { "actorDisplayName", "documentNumber", "resourceId" },
            payload.RootElement.EnumerateObject().Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray());
        foreach (var forbidden in new[] { "grandTotal", "netCost", "margin", "price", "cost", "amount", "total" })
        {
            Assert.DoesNotContain(forbidden, row.PayloadJson, StringComparison.OrdinalIgnoreCase);
        }

        var reviewerList = await _client.SendAsync(CreateAuthenticatedRequest(
            HttpMethod.Get, "/api/v1/notifications", "token-org-b", TestOnlyDataSeeder.TestCostReviewerMembershipId));
        Assert.Equal(HttpStatusCode.OK, reviewerList.StatusCode);
        Assert.Contains(estimate.Id.ToString(), await reviewerList.Content.ReadAsStringAsync(), StringComparison.Ordinal);
    }
```

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~Submit_NotifiesOnlyTheFirstReviewer" -m:1` → Expected: PASS. (ถ้า `GetEstimateAsync` คืน `RowVersion` คนละชนิดกับ `Guid` ให้ใช้รูปแบบเดียวกับ `EstimateEndpointsTests.cs:963`.)

- [ ] **Step 5: รัน test ทั้งกลุ่ม**

Run: `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~Notification" -m:1` → Expected: PASS ทั้งหมด (รวม `NotificationInfrastructureTests` จาก Task 5).

- [ ] **Step 6: mutation check — พิสูจน์ว่า test จับ regression จริง** (ตามแบบ G-01 Task 8 step 3; BSD `sed -i ''`; เงื่อนไขต้องเป็น runtime-false/true ไม่ใช่ `if (false)` เพราะ CS0162)

6a. **ตัด maker ออก** — maker ถูกตัดสองชั้น (resolver: `ExcludedUserIds`; planner: `excluded` ที่รวม actor) จึงต้องทำ mutation ทั้งสองชั้นพร้อมกัน:

```bash
cd /Users/syaco/Documents/development/tan-erp
sed -i '' 's/\.Where(m => !excluded\.Contains(m\.UserId))/.Where(m => m.UserId != Guid.Empty)/' backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationRecipientResolver.cs
sed -i '' 's/&& !excluded\.Contains(id))/\&\& excluded.Count < 99)/' backend/src/TanErp.Application/Notifications/NotificationPublishPlanner.cs
git diff --stat   # Expected: 2 files changed
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~SubmitPurchaseOrder_NotifiesTheOtherApproverOnce" -m:1
```

Expected: **FAIL** (maker ได้ notification: `Assert.Empty(...)` ของ UserA ล้ม / `Assert.Single(stored)` ได้ 2 แถว).
Revert: `git checkout -- backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationRecipientResolver.cs backend/src/TanErp.Application/Notifications/NotificationPublishPlanner.cs` แล้วรันซ้ำ → PASS.

(ทำเพียงชั้นเดียวแล้ว test ยัง PASS เป็นพฤติกรรมที่ตั้งใจ = defense in depth ไม่ใช่ test อ่อน.)

6b. **own-only filter**:

```bash
sed -i '' 's/&& n\.RecipientUserId == userId)/\&\& (n.RecipientUserId == userId || userId != Guid.Empty))/' backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationStore.cs
git diff --stat   # Expected: 1 file changed
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OwnOnly_|FullyQualifiedName~List_ReturnsOnlyTheCallersRows|FullyQualifiedName~MarkRead_OfAnotherUsers" -m:1
```

Expected: **FAIL** (maker เห็นแถวของผู้อนุมัติ; mark-read ของคนอื่นได้ 200 แทน 404). Revert: `git checkout -- backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationStore.cs` แล้วรันซ้ำ → PASS.

6c. ยืนยันว่าไม่มี mutation ค้าง: `git status --short backend/src` → Expected: ว่าง (ก่อน commit Step 7 จะมีแค่ไฟล์ test).

- [ ] **Step 7: commit**

```bash
git add backend/tests/TanErp.IntegrationTests/Api/NotificationEndpointsTests.cs backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs
git commit -F - <<'EOF'
test(notifications): cover publish, rollback, recipient scope and own-only end to end

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 9: Frontend plumbing — generated API, api-client, type whitelist, `useNotifications`, messages

**Files:**
- Regenerate: `frontend/src/generated/api/tan-erp.v1.ts`
- Modify: `frontend/src/lib/api/api-client.ts`, `frontend/src/lib/api/api-client.test.ts`
- Create: `frontend/src/lib/notifications/notification-types.ts`, `frontend/src/lib/notifications/notification-types.test.ts`
- Create: `frontend/src/hooks/useNotifications.ts`, `frontend/src/hooks/useNotifications.test.tsx`
- Modify: `frontend/src/messages/th.json`, `frontend/src/messages/en.json`

**Reuse check (ตรวจแล้ว):** ใช้ `useApiRequestContext` (`lib/api/use-api-request-context.ts`) และรูปแบบ query key `["business", membershipId, locale, ...]` เหมือน `useAttachments.ts`. **ไม่มี helper polling ในรีโป** (`grep -rln 'refetchInterval\|visibilityState\|visibilitychange' frontend/src` ไม่พบไฟล์) จึงใช้ `refetchInterval` + `refetchIntervalInBackground: false` ของ TanStack Query ในตัว hook (TanStack หยุด interval เมื่อแท็บถูกซ่อน/ไม่โฟกัส และ refetch เมื่อกลับมา) — ไม่เขียน helper กลางใหม่; ถ้ามี feature อื่นต้อง polling เช่นกันภายหลัง ค่อยยก `NOTIFICATION_POLL_INTERVAL_MS` + options เป็น shared helper (ข้อเสนอ Global Reuse).

**Mapping API (จาก Task 6):** `GET /api/v1/notifications?unreadOnly&page&pageSize`, `GET /api/v1/notifications/unread-count`, `POST /api/v1/notifications/{id}/read`, `POST /api/v1/notifications/read-all`. Schema: `NotificationResponse`, `NotificationListResponse`, `UnreadCountResponse`, `MarkAllReadResponse`.

- [ ] **Step 1: regenerate types และตรวจชื่อ schema**

Run: `cd /Users/syaco/Documents/development/tan-erp/frontend && npm run generate:api`
Expected: `src/generated/api/tan-erp.v1.ts` เปลี่ยนเฉพาะส่วน notifications (Task 6 ได้ regenerate `contracts/openapi/tan-erp.v1.json` แล้ว).

Run: `grep -n 'NotificationResponse\|NotificationListResponse\|UnreadCountResponse\|MarkAllReadResponse' src/generated/api/tan-erp.v1.ts | head`
Expected: พบทั้ง 4 schema ใน `components["schemas"]`. ถ้าชื่อต่างจากนี้ ให้ใช้ชื่อที่ generate ได้ในทุกขั้นถัดไป.

Run: `npm run check:api` → Expected: exit 0 (หลัง `git add` ไฟล์ที่ generate แล้ว `git diff --exit-code` ต้องสะอาด; ถ้ารันก่อน add ให้รัน `git add src/generated/api/tan-erp.v1.ts` ก่อน).

- [ ] **Step 2: test api-client ที่ล้มก่อน**

เพิ่มใน `frontend/src/lib/api/api-client.test.ts` (ในบล็อก `describe("ApiClient", ...)`, รูปแบบเดียวกับเคส `/api/v1/me`):

```ts
  it("calls the notification endpoints with membership headers and a bounded query", async () => {
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => ({ items: [], pagination: { page: 1, pageSize: 20, totalCount: 0, totalPages: 0 } }),
    });
    global.fetch = fetchMock;
    const client = new ApiClient("http://localhost:5000");
    const options = { token: "tok", membershipId: "m-1", locale: "th" as const };

    await client.listNotifications(options, { unreadOnly: true, page: 2, pageSize: 20 });
    await client.getUnreadNotificationCount(options);
    await client.markNotificationRead("n 1", options);
    await client.markAllNotificationsRead(options);

    const calls = fetchMock.mock.calls.map(([url, init]) => [url, init.method]);
    expect(calls).toEqual([
      ["http://localhost:5000/api/v1/notifications?unreadOnly=true&page=2&pageSize=20", "GET"],
      ["http://localhost:5000/api/v1/notifications/unread-count", "GET"],
      ["http://localhost:5000/api/v1/notifications/n%201/read", "POST"],
      ["http://localhost:5000/api/v1/notifications/read-all", "POST"],
    ]);
    expect(fetchMock.mock.calls[0][1].headers["X-Membership-Id"]).toBe("m-1");
    // Mark-read is naturally idempotent on the server; no Idempotency-Key is required or sent.
    expect(fetchMock.mock.calls[2][1].headers["Idempotency-Key"]).toBeUndefined();
  });
```

Run: `npx vitest run src/lib/api/api-client.test.ts -t "notification endpoints"` → Expected: FAIL (`client.listNotifications is not a function`).

- [ ] **Step 3: เพิ่ม methods ใน `api-client.ts`**

ต่อจาก `export type CaptureSignatureRequest = ...` (กลุ่ม type alias, ~บรรทัด 234):

```ts
export type NotificationResponse = components["schemas"]["NotificationResponse"];
export type NotificationListResponse = components["schemas"]["NotificationListResponse"];
export type UnreadCountResponse = components["schemas"]["UnreadCountResponse"];
export type MarkAllReadResponse = components["schemas"]["MarkAllReadResponse"];
export interface ListNotificationsParams {
  unreadOnly: boolean;
  page: number;
  pageSize: number;
}
```

ต่อจากเมธอด `captureSignature` (~บรรทัด 1590):

```ts
  async listNotifications(options: RequestOptions, query: ListNotificationsParams): Promise<NotificationListResponse> {
    const params = new URLSearchParams();
    params.set("unreadOnly", String(query.unreadOnly));
    params.set("page", String(query.page));
    params.set("pageSize", String(query.pageSize));
    return this.request<NotificationListResponse>(`/api/v1/notifications?${params.toString()}`, "GET", options);
  }

  async getUnreadNotificationCount(options: RequestOptions): Promise<UnreadCountResponse> {
    return this.request<UnreadCountResponse>("/api/v1/notifications/unread-count", "GET", options);
  }

  async markNotificationRead(notificationId: string, options: RequestOptions): Promise<NotificationResponse> {
    return this.request<NotificationResponse>(`/api/v1/notifications/${encodeURIComponent(notificationId)}/read`, "POST", options);
  }

  async markAllNotificationsRead(options: RequestOptions): Promise<MarkAllReadResponse> {
    return this.request<MarkAllReadResponse>("/api/v1/notifications/read-all", "POST", options);
  }
```

Run: `npx vitest run src/lib/api/api-client.test.ts` → Expected: PASS ทั้งไฟล์. (ถ้า `request` บังคับ `payload` สำหรับ POST ให้ตรวจ signature ที่ `api-client.ts:430` — `attachFiles`/`unlinkAttachment` เรียกโดยไม่มี/มี payload ได้ทั้งคู่.)

- [ ] **Step 4: test whitelist ที่ล้มก่อน**

`frontend/src/lib/notifications/notification-types.test.ts`:

```ts
import { describe, expect, it } from "vitest";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import {
  NOTIFICATION_TYPES,
  isNotificationType,
  notificationMessageKey,
  notificationMessageValues,
} from "./notification-types";

describe("notification types", () => {
  it("mirrors the backend NotificationTypes whitelist", () => {
    expect(Object.keys(NOTIFICATION_TYPES).sort()).toEqual([
      "change-order.approval-requested",
      "cost-record.approval-requested",
      "estimate.approval-requested",
      "mrp-run.approval-requested",
      "purchase-order.approval-requested",
      "role-assignment.approval-requested",
    ]);
  });

  it("narrows only registered types, exactly", () => {
    expect(isNotificationType("estimate.approval-requested")).toBe(true);
    expect(isNotificationType("Estimate.Approval-Requested")).toBe(false);
    expect(isNotificationType("customer.created")).toBe(false);
    expect(notificationMessageKey("customer.created")).toBeNull();
    expect(notificationMessageKey("mrp-run.approval-requested")).toBe("mrpRunApprovalRequested");
  });

  it("passes only the declared fields to the message and shows a dash for a missing one", () => {
    expect(
      notificationMessageValues("estimate.approval-requested", {
        documentNumber: "EST-1",
        actorDisplayName: "สมชาย",
        unexpected: "x",
      }),
    ).toEqual({ documentNumber: "EST-1", actorDisplayName: "สมชาย" });
    expect(notificationMessageValues("estimate.approval-requested", { documentNumber: "EST-1" })).toEqual({
      documentNumber: "EST-1",
      actorDisplayName: "-",
    });
  });

  it.each([
    ["th", thMessages],
    ["en", enMessages],
  ])("has a %s message for every type, using every declared field", (_locale, messages) => {
    for (const descriptor of Object.values(NOTIFICATION_TYPES)) {
      const template: string = messages.notifications.types[descriptor.messageKey];
      expect(template).toBeTruthy();
      for (const field of descriptor.fields) expect(template).toContain(`{${field}}`);
    }
  });

  it("has the same notifications keys in th and en", () => {
    const flat = (value: unknown, prefix = ""): string[] =>
      typeof value === "object" && value !== null
        ? Object.entries(value).flatMap(([key, child]) => flat(child, `${prefix}${key}.`))
        : [prefix];
    expect(flat(enMessages.notifications).sort()).toEqual(flat(thMessages.notifications).sort());
  });
});
```

Run: `npx vitest run src/lib/notifications/notification-types.test.ts` → Expected: FAIL (module ไม่พบ).

- [ ] **Step 5: implement whitelist**

`frontend/src/lib/notifications/notification-types.ts`:

```ts
/**
 * Notification types the UI can render. Mirrors the backend registry (NotificationTypes / NotificationTypeRegistry);
 * register a new type in the backend first, then here and in messages (th + en). A type that is not listed is never
 * rendered from guesses: callers fall back to the generic "unknown" message.
 */
export interface NotificationTypeDescriptor {
  /** Key under `notifications.types` in the messages (JSON keys cannot contain dots). */
  messageKey: NotificationMessageKey;
  /** Payload fields the message uses; all other payload fields are ignored. */
  fields: readonly NotificationPayloadField[];
}

export type NotificationPayloadField = "documentNumber" | "actorDisplayName" | "subjectDisplayName" | "roleName";

export type NotificationMessageKey =
  | "estimateApprovalRequested"
  | "costRecordApprovalRequested"
  | "purchaseOrderApprovalRequested"
  | "changeOrderApprovalRequested"
  | "mrpRunApprovalRequested"
  | "roleAssignmentApprovalRequested";

export const NOTIFICATION_TYPES = {
  "estimate.approval-requested": { messageKey: "estimateApprovalRequested", fields: ["documentNumber", "actorDisplayName"] },
  "cost-record.approval-requested": { messageKey: "costRecordApprovalRequested", fields: ["documentNumber", "actorDisplayName"] },
  "purchase-order.approval-requested": { messageKey: "purchaseOrderApprovalRequested", fields: ["documentNumber", "actorDisplayName"] },
  "change-order.approval-requested": { messageKey: "changeOrderApprovalRequested", fields: ["documentNumber", "actorDisplayName"] },
  "mrp-run.approval-requested": { messageKey: "mrpRunApprovalRequested", fields: ["documentNumber", "actorDisplayName"] },
  "role-assignment.approval-requested": {
    messageKey: "roleAssignmentApprovalRequested",
    fields: ["subjectDisplayName", "roleName", "actorDisplayName"],
  },
} as const satisfies Record<string, NotificationTypeDescriptor>;

export type NotificationType = keyof typeof NOTIFICATION_TYPES;

export function isNotificationType(value: string): value is NotificationType {
  return Object.prototype.hasOwnProperty.call(NOTIFICATION_TYPES, value);
}

export function notificationMessageKey(type: string): NotificationMessageKey | null {
  return isNotificationType(type) ? NOTIFICATION_TYPES[type].messageKey : null;
}

/** Values for the type's message: only declared fields, and "-" for a field the payload does not carry. */
export function notificationMessageValues(type: NotificationType, payload: Readonly<Record<string, string>>): Record<string, string> {
  const values: Record<string, string> = {};
  for (const field of NOTIFICATION_TYPES[type].fields) {
    values[field] = payload[field] ?? "-";
  }
  return values;
}
```

(`payload[field] ?? "-"` เป็นค่าว่างมาตรฐานเดียว ไม่ใช่ chain ของ fallback.)

- [ ] **Step 6: messages `notifications` (th + en) ด้วย script**

เพิ่ม namespace ระดับบนสุด `notifications` (ต่อท้าย `attachments`). Task 10/11 (UI) จะขยายคีย์เฉพาะ UI เพิ่มเอง; ที่นี่ใส่เฉพาะที่ plumbing/ข้อความของ type ต้องใช้.

```bash
cd /Users/syaco/Documents/development/tan-erp/frontend
python3 - <<'PY'
import json

TH = {
    "title": "การแจ้งเตือน",
    "unknownType": "มีการแจ้งเตือนใหม่",
    "loadFailed": "โหลดการแจ้งเตือนไม่สำเร็จ",
    "types": {
        "estimateApprovalRequested": "{actorDisplayName} ส่งใบประมาณราคา {documentNumber} เพื่อรออนุมัติ",
        "costRecordApprovalRequested": "{actorDisplayName} ส่งต้นทุนสินค้า {documentNumber} เพื่อรออนุมัติ",
        "purchaseOrderApprovalRequested": "{actorDisplayName} ส่งใบสั่งซื้อ {documentNumber} เพื่อรออนุมัติ",
        "changeOrderApprovalRequested": "{actorDisplayName} ส่งใบเปลี่ยนแปลงงาน {documentNumber} เพื่อรออนุมัติ",
        "mrpRunApprovalRequested": "{actorDisplayName} สร้างรอบวางแผนวัสดุ {documentNumber} ที่รออนุมัติ",
        "roleAssignmentApprovalRequested": "{actorDisplayName} ขอมอบบทบาท {roleName} ให้ {subjectDisplayName}",
    },
}
EN = {
    "title": "Notifications",
    "unknownType": "You have a new notification",
    "loadFailed": "Could not load notifications",
    "types": {
        "estimateApprovalRequested": "{actorDisplayName} submitted estimate {documentNumber} for approval",
        "costRecordApprovalRequested": "{actorDisplayName} submitted the cost of item {documentNumber} for approval",
        "purchaseOrderApprovalRequested": "{actorDisplayName} submitted purchase order {documentNumber} for approval",
        "changeOrderApprovalRequested": "{actorDisplayName} submitted change order {documentNumber} for approval",
        "mrpRunApprovalRequested": "{actorDisplayName} created planning run {documentNumber} awaiting approval",
        "roleAssignmentApprovalRequested": "{actorDisplayName} requested role {roleName} for {subjectDisplayName}",
    },
}

for path, ns in (("src/messages/th.json", TH), ("src/messages/en.json", EN)):
    raw = open(path, encoding="utf-8").read()
    data = json.loads(raw)
    # Guard: the file must round-trip with this serializer, otherwise the script would reformat unrelated lines.
    assert json.dumps(data, indent=2, ensure_ascii=False) + "\n" == raw, f"{path} does not round-trip; edit by hand"
    assert "notifications" not in data, f"{path} already has notifications"
    data["notifications"] = ns
    open(path, "w", encoding="utf-8").write(json.dumps(data, indent=2, ensure_ascii=False) + "\n")
PY
git diff --stat -- src/messages
```

Expected: ทั้งสองไฟล์มีเฉพาะบรรทัดที่เพิ่ม (insertions เท่านั้น, ไม่มี deletions นอกจากเครื่องหมาย `,` ท้าย `attachments`). ถ้า assert round-trip ล้ม ให้แก้ด้วย Edit ตรง ๆ ท้ายไฟล์แทน.

Run: `npx vitest run src/lib/notifications/notification-types.test.ts` → Expected: PASS ทั้ง 6 (รวมคู่ th/en).

- [ ] **Step 7: test hook ที่ล้มก่อน**

`frontend/src/hooks/useNotifications.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { act, renderHook, waitFor } from "@testing-library/react";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { apiClient } from "@/lib/api/api-client";
import {
  NOTIFICATION_POLL_INTERVAL_MS,
  notificationUnreadKey,
  notificationsKey,
  useMarkAllNotificationsRead,
  useMarkNotificationRead,
  useNotificationList,
  useUnreadNotificationCount,
} from "./useNotifications";

const context = vi.hoisted(() => ({ membershipId: "m-1" as string | undefined }));

vi.mock("@/lib/api/use-api-request-context", () => ({
  useApiRequestContext: () => ({
    membershipId: context.membershipId,
    locale: "th",
    buildOptions: async () => ({ token: "tok", membershipId: context.membershipId ?? "", locale: "th" }),
  }),
}));
vi.mock("@/lib/api/api-client", async (importOriginal) => {
  const original = await importOriginal<typeof import("@/lib/api/api-client")>();
  return {
    ...original,
    apiClient: {
      listNotifications: vi.fn(),
      getUnreadNotificationCount: vi.fn(),
      markNotificationRead: vi.fn(),
      markAllNotificationsRead: vi.fn(),
    },
  };
});

function setup() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } });
  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
  return { queryClient, wrapper };
}

describe("useNotifications", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    context.membershipId = "m-1";
  });

  it("namespaces keys by membership and locale", () => {
    expect(notificationsKey("m-1", "th")).not.toEqual(notificationsKey("m-2", "th"));
    expect(notificationsKey("m-1", "th")).not.toEqual(notificationsKey("m-1", "en"));
    expect(notificationUnreadKey("m-1", "th").slice(0, 4)).toEqual(notificationsKey("m-1", "th"));
  });

  it("polls the unread count only while the tab is visible", async () => {
    vi.mocked(apiClient.getUnreadNotificationCount).mockResolvedValue({ unreadCount: 3 });
    const { queryClient, wrapper } = setup();
    const { result } = renderHook(() => useUnreadNotificationCount(), { wrapper });

    await waitFor(() => expect(result.current.data?.unreadCount).toBe(3));
    const options = queryClient.getQueryCache().find({ queryKey: notificationUnreadKey("m-1", "th") })?.observers[0]?.options;
    expect(options?.refetchInterval).toBe(NOTIFICATION_POLL_INTERVAL_MS);
    expect(options?.refetchIntervalInBackground).toBe(false);
  });

  it("does not call the API without a selected membership", async () => {
    context.membershipId = undefined;
    const { wrapper } = setup();
    renderHook(() => useUnreadNotificationCount(), { wrapper });
    renderHook(() => useNotificationList({ unreadOnly: false, page: 1, pageSize: 20 }), { wrapper });

    await act(async () => {});
    expect(apiClient.getUnreadNotificationCount).not.toHaveBeenCalled();
    expect(apiClient.listNotifications).not.toHaveBeenCalled();
  });

  it("refreshes the list and the count after mark-read and read-all", async () => {
    vi.mocked(apiClient.markNotificationRead).mockResolvedValue({
      id: "n-1", type: "estimate.approval-requested", payload: {}, deepLink: null, createdAtUtc: "2026-10-08T00:00:00Z", readAtUtc: "2026-10-08T00:01:00Z",
    });
    vi.mocked(apiClient.markAllNotificationsRead).mockResolvedValue({ updatedCount: 2 });
    const { queryClient, wrapper } = setup();
    const invalidate = vi.spyOn(queryClient, "invalidateQueries");

    const markRead = renderHook(() => useMarkNotificationRead(), { wrapper });
    await act(async () => {
      await markRead.result.current.mutateAsync("n-1");
    });
    expect(apiClient.markNotificationRead).toHaveBeenCalledWith("n-1", expect.objectContaining({ membershipId: "m-1" }));
    expect(invalidate).toHaveBeenCalledWith({ queryKey: notificationsKey("m-1", "th") });

    invalidate.mockClear();
    const markAll = renderHook(() => useMarkAllNotificationsRead(), { wrapper });
    await act(async () => {
      await markAll.result.current.mutateAsync();
    });
    expect(invalidate).toHaveBeenCalledWith({ queryKey: notificationsKey("m-1", "th") });
  });
});
```

Run: `npx vitest run src/hooks/useNotifications.test.tsx` → Expected: FAIL (module `./useNotifications` ไม่พบ).

- [ ] **Step 8: implement hook**

`frontend/src/hooks/useNotifications.ts`:

```ts
import { useMutation, useQuery, useQueryClient, type UseMutationResult, type UseQueryResult } from "@tanstack/react-query";
import {
  apiClient,
  type ListNotificationsParams,
  type MarkAllReadResponse,
  type NotificationListResponse,
  type NotificationResponse,
  type UnreadCountResponse,
} from "@/lib/api/api-client";
import { useApiRequestContext, type ApiLocale } from "@/lib/api/use-api-request-context";

/** The bell's unread count refreshes this often while the tab is visible; nothing is fetched in a hidden tab. */
export const NOTIFICATION_POLL_INTERVAL_MS = 30_000;
export const NOTIFICATION_PAGE_SIZE = 20;

export function notificationsKey(membershipId: string | undefined, locale: ApiLocale) {
  return ["business", membershipId, locale, "notifications"] as const;
}

export function notificationListKey(membershipId: string | undefined, locale: ApiLocale, params: ListNotificationsParams) {
  return [...notificationsKey(membershipId, locale), "list", params.unreadOnly, params.page, params.pageSize] as const;
}

export function notificationUnreadKey(membershipId: string | undefined, locale: ApiLocale) {
  return [...notificationsKey(membershipId, locale), "unread-count"] as const;
}

export function useUnreadNotificationCount(): UseQueryResult<UnreadCountResponse, Error> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  return useQuery({
    queryKey: notificationUnreadKey(membershipId, locale),
    enabled: Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.getUnreadNotificationCount(await buildOptions({ signal })),
    refetchInterval: NOTIFICATION_POLL_INTERVAL_MS,
    refetchIntervalInBackground: false,
  });
}

/** The list is fetched on demand (dropdown open / full page); it is refreshed by invalidation, not by its own timer. */
export function useNotificationList(params: ListNotificationsParams, enabled = true): UseQueryResult<NotificationListResponse, Error> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  return useQuery({
    queryKey: notificationListKey(membershipId, locale, params),
    enabled: enabled && Boolean(membershipId),
    queryFn: async ({ signal }) => apiClient.listNotifications(await buildOptions({ signal }), params),
  });
}

export function useMarkNotificationRead(): UseMutationResult<NotificationResponse, Error, string> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  const queryClient = useQueryClient();
  return useMutation<NotificationResponse, Error, string>({
    mutationFn: async (notificationId) => apiClient.markNotificationRead(notificationId, await buildOptions()),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: notificationsKey(membershipId, locale) });
    },
  });
}

export function useMarkAllNotificationsRead(): UseMutationResult<MarkAllReadResponse, Error, void> {
  const { membershipId, locale, buildOptions } = useApiRequestContext();
  const queryClient = useQueryClient();
  return useMutation<MarkAllReadResponse, Error, void>({
    mutationFn: async () => apiClient.markAllNotificationsRead(await buildOptions()),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: notificationsKey(membershipId, locale) });
    },
  });
}
```

Run: `npx vitest run src/hooks/useNotifications.test.tsx src/lib/notifications src/lib/api/api-client.test.ts` → Expected: PASS ทั้งหมด. หมายเหตุ: `useApiRequestContext.ts` export `ApiLocale` อยู่แล้ว (บรรทัด 7).

- [ ] **Step 9: verification gates ฝั่ง frontend**

Run: `npm run check:api && npm run lint && npm run typecheck && npx vitest run` → Expected: ผ่านทั้งหมด (ไม่มี `any`/`as any`/`@ts-ignore`; `messages.notifications.types[descriptor.messageKey]` พิมพ์ถูกเพราะ `messageKey` เป็น union ของคีย์ที่มีใน JSON — ถ้า TypeScript บ่นว่า index ไม่ได้ แปลว่าคีย์ใน `NotificationMessageKey` กับ JSON ไม่ตรงกัน ซึ่งคือสิ่งที่ parity test ตั้งใจจับ).

- [ ] **Step 10: commit**

```bash
git add frontend/src/generated/api/tan-erp.v1.ts frontend/src/lib/api/api-client.ts frontend/src/lib/api/api-client.test.ts frontend/src/lib/notifications frontend/src/hooks/useNotifications.ts frontend/src/hooks/useNotifications.test.tsx frontend/src/messages/th.json frontend/src/messages/en.json
git commit -F - <<'EOF'
feat(notifications): add notification api client, type whitelist and polling hooks

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 10: `IconBell`, `NotificationCenter` แบบ controlled, `NotificationBell` และวางใน header

**Files:**
- Modify: `frontend/src/components/common/Icons.tsx`
- Create: `frontend/src/lib/notifications/notification-view.ts`, `frontend/src/lib/notifications/notification-view.test.tsx`
- Modify: `frontend/src/components/layout/NotificationCenter.tsx`
- Create: `frontend/src/components/layout/NotificationCenter.test.tsx`
- Create: `frontend/src/components/layout/NotificationBell.tsx`, `frontend/src/components/layout/NotificationBell.test.tsx`
- Modify: `frontend/src/components/layout/erp-shell.tsx`, `frontend/src/components/layout/erp-shell.test.tsx`
- Modify: `frontend/src/messages/th.json`, `frontend/src/messages/en.json` (เฉพาะ `common.notificationCenter`)

**Reuse decision (ตรวจแล้ว):** `NotificationCenter.tsx` มี markup/สไตล์ที่ถูกต้องตาม design (rounded-none, semantic tokens, dropdown) แต่ถือ state รายการเองและไม่มีผู้ใช้/test → **เก็บ markup เดิม เปลี่ยนเป็น controlled** (รับ `notifications`, `unreadCount`, callbacks) ไม่สร้างกระดิ่งตัวที่สอง. เพราะไม่มีผู้เรียกเดิม การตัด local state ออกไม่กระทบใคร; default ของ prop เดิมคงไว้ (`notifications = []`, `unreadCount` คำนวณจากรายการเมื่อไม่ส่งมา). Container `NotificationBell` แยกจาก presentational เพื่อให้ `NotificationCenter` ทดสอบได้โดยไม่ต้อง mock API. ตัวแปลง notification → ข้อความ/ลิงก์ใช้ร่วมกับหน้ารายการ (Task 11) จึงอยู่ใน `lib/notifications/notification-view.ts` (Global Reuse ตั้งแต่ต้น ไม่ให้สองที่เขียนซ้ำ). Navigation ใช้ `useRouter` ของ `next/navigation` + `/${locale}${path}` เหมือน `erp-shell.tsx` (`router.push(`/${locale}/login`)`) เพราะ deep link จาก API ไม่มี locale prefix. Loading ใช้ `MonoSpinner` (Minimal Mono Loading). Keyboard: Esc ปิดและคืน focus ที่ปุ่ม, คลิกนอกกล่องปิด, รายการเป็น `<button>` จริงจึงใช้ Tab/Enter/Space ได้; **ไม่ทำ arrow-key roving** (dropdown นี้เป็น popover รายการ ไม่ใช่ `role="menu"`; ตัดสินใจเรียบง่ายและบันทึกไว้เป็นข้อจำกัดเรื่อง a11y ใน Task 12).

- [ ] **Step 1: เพิ่ม `IconBell`**

ใน `frontend/src/components/common/Icons.tsx` ต่อจาก `IconInfo`:

```tsx
export function IconBell({ size = 18, strokeWidth = 2, ...props }: IconProps) {
  return (
    <svg width={size} height={size} strokeWidth={strokeWidth} {...baseProps} {...props}>
      <path d="M18 8a6 6 0 0 0-12 0c0 7-3 9-3 9h18s-3-2-3-9" />
      <path d="M13.73 21a2 2 0 0 1-3.46 0" />
    </svg>
  );
}
```

- [ ] **Step 2: messages `common.notificationCenter` (เพิ่ม 4 คีย์)**

```bash
cd /Users/syaco/Documents/development/tan-erp/frontend
python3 - <<'PY'
import json

ADD = {
    "src/messages/th.json": {
        "unreadCount": "{count} รายการที่ยังไม่อ่าน",
        "viewAll": "ดูทั้งหมด",
        "loading": "กำลังโหลดการแจ้งเตือน",
        "loadFailed": "โหลดการแจ้งเตือนไม่สำเร็จ",
    },
    "src/messages/en.json": {
        "unreadCount": "{count} unread",
        "viewAll": "View all",
        "loading": "Loading notifications",
        "loadFailed": "Could not load notifications",
    },
}
for path, keys in ADD.items():
    raw = open(path, encoding="utf-8").read()
    data = json.loads(raw)
    assert json.dumps(data, indent=2, ensure_ascii=False) + "\n" == raw, f"{path} does not round-trip; edit by hand"
    center = data["common"]["notificationCenter"]
    assert not set(keys) & set(center), f"{path} already has some of these keys"
    center.update(keys)
    open(path, "w", encoding="utf-8").write(json.dumps(data, indent=2, ensure_ascii=False) + "\n")
PY
git diff --stat -- src/messages
```

Expected: เพิ่มเฉพาะบรรทัดใน `common.notificationCenter` ของทั้งสองไฟล์ (ถ้า assert round-trip ล้ม ให้แก้ด้วย Edit ตรง ๆ).

- [ ] **Step 3: test ตัวแปลงข้อความ/ลิงก์ที่ล้มก่อน**

`frontend/src/lib/notifications/notification-view.test.tsx`:

```tsx
import React from "react";
import { describe, expect, it } from "vitest";
import { renderHook } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import type { NotificationResponse } from "@/lib/api/api-client";
import { localizedNotificationHref, useNotificationText } from "./notification-view";

const base: NotificationResponse = {
  id: "n-1",
  type: "estimate.approval-requested",
  payload: { documentNumber: "EST-1", actorDisplayName: "สมชาย", costTotal: "999" },
  deepLink: "/estimates/e-1",
  createdAtUtc: "2026-10-08T00:00:00Z",
  readAtUtc: null,
};

function textFor(notification: NotificationResponse): string {
  const wrapper = ({ children }: { children: React.ReactNode }) => (
    <NextIntlClientProvider locale="th" messages={thMessages}>{children}</NextIntlClientProvider>
  );
  return renderHook(() => useNotificationText(), { wrapper }).result.current(notification);
}

describe("notification view helpers", () => {
  it("renders the registered message with only the declared payload fields", () => {
    expect(textFor(base)).toBe("สมชาย ส่งใบประมาณราคา EST-1 เพื่อรออนุมัติ");
  });

  it("renders a neutral line for an unknown type and never throws", () => {
    expect(textFor({ ...base, type: "customer.created" })).toBe(thMessages.notifications.unknownType);
  });

  it("builds a locale-prefixed path only from a same-origin absolute path", () => {
    expect(localizedNotificationHref("th", "/estimates/e-1")).toBe("/th/estimates/e-1");
    expect(localizedNotificationHref("en", null)).toBeNull();
    expect(localizedNotificationHref("th", "//evil.test/x")).toBeNull();
    expect(localizedNotificationHref("th", "https://evil.test/x")).toBeNull();
  });
});
```

Run: `cd /Users/syaco/Documents/development/tan-erp/frontend && npx vitest run src/lib/notifications/notification-view.test.tsx` → Expected: FAIL (module `./notification-view` ไม่พบ).

- [ ] **Step 4: implement `notification-view.ts`**

```ts
import { useCallback } from "react";
import { useTranslations } from "next-intl";
import type { NotificationResponse } from "@/lib/api/api-client";
import { isNotificationType, notificationMessageKey, notificationMessageValues } from "./notification-types";

/** Returns a function that renders a notification as one display line; an unregistered type gets a neutral line. */
export function useNotificationText(): (notification: NotificationResponse) => string {
  const t = useTranslations("notifications");
  return useCallback(
    (notification) => {
      const messageKey = notificationMessageKey(notification.type);
      if (messageKey === null || !isNotificationType(notification.type)) {
        return t("unknownType");
      }
      return t(`types.${messageKey}`, notificationMessageValues(notification.type, notification.payload));
    },
    [t],
  );
}

/** The API returns locale-less paths; accept only an absolute in-app path so a link can never leave the app. */
export function localizedNotificationHref(locale: string, deepLink: string | null): string | null {
  if (deepLink === null || !deepLink.startsWith("/") || deepLink.startsWith("//")) {
    return null;
  }
  return `/${locale}${deepLink}`;
}
```

Run: `npx vitest run src/lib/notifications/notification-view.test.tsx` → Expected: PASS (3 tests). ถ้า TypeScript บ่นว่า `t(`types.${messageKey}`)` ไม่ใช่ key ที่รู้จัก ให้ใช้ `t.rich`-free ทางเดียวกับที่ repo ใช้กับ key แบบ template (ดู `customer-list.tsx`: `tCommon(`status.${key}`)`) — พิมพ์ผ่านเพราะ `messageKey` เป็น union ของคีย์จริง.

- [ ] **Step 5: test `NotificationCenter` (controlled) ที่ล้มก่อน**

`frontend/src/components/layout/NotificationCenter.test.tsx`:

```tsx
import React from "react";
import { describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import { NotificationCenter, type NotificationItem } from "./NotificationCenter";

const items: NotificationItem[] = [
  { id: "n-1", title: "ใบประมาณราคา EST-1 รออนุมัติ", timeText: "8 ต.ค. 69 10:00", isRead: false, href: "/th/estimates/e-1" },
  { id: "n-2", title: "ใบสั่งซื้อ PO-1 รออนุมัติ", timeText: "7 ต.ค. 69 09:00", isRead: true, href: null },
];

function renderCenter(props: Partial<React.ComponentProps<typeof NotificationCenter>> = {}) {
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <NotificationCenter notifications={items} unreadCount={1} {...props} />
    </NextIntlClientProvider>,
  );
}

const bellButton = () => screen.getByRole("button", { name: thMessages.common.notificationCenter.title });

describe("NotificationCenter (controlled)", () => {
  it("shows the unread count from props, capped at 99+, and announces it politely", () => {
    renderCenter({ unreadCount: 150 });
    expect(screen.getByText("99+")).toBeDefined();
    const live = screen.getByRole("status");
    expect(live.getAttribute("aria-live")).toBe("polite");
    expect(live.textContent).toBe("150 รายการที่ยังไม่อ่าน");
  });

  it("falls back to counting unread items only when unreadCount is not provided", () => {
    renderCenter({ unreadCount: undefined });
    expect(screen.getByText("1")).toBeDefined();
  });

  it("renders no badge when nothing is unread", () => {
    renderCenter({ unreadCount: 0 });
    expect(screen.queryByText("0")).toBeNull();
  });

  it("opens, reports it, and calls onItemClick with the clicked item (parent decides what happens next)", () => {
    const onOpenChange = vi.fn();
    const onItemClick = vi.fn();
    renderCenter({ onOpenChange, onItemClick });

    fireEvent.click(bellButton());
    expect(onOpenChange).toHaveBeenLastCalledWith(true);
    expect(bellButton().getAttribute("aria-expanded")).toBe("true");

    fireEvent.click(screen.getByRole("button", { name: /EST-1/ }));
    expect(onItemClick).toHaveBeenCalledWith(items[0]);
    expect(onOpenChange).toHaveBeenLastCalledWith(false);
  });

  it("closes on Escape and returns focus to the bell button", () => {
    renderCenter();
    fireEvent.click(bellButton());
    const item = screen.getByRole("button", { name: /EST-1/ });
    item.focus();

    fireEvent.keyDown(item, { key: "Escape" });
    expect(screen.queryByRole("button", { name: /EST-1/ })).toBeNull();
    expect(document.activeElement).toBe(bellButton());
    expect(bellButton().getAttribute("aria-expanded")).toBe("false");
  });

  it("closes when clicking outside", () => {
    renderCenter();
    fireEvent.click(bellButton());
    fireEvent.mouseDown(document.body);
    expect(screen.queryByRole("button", { name: /EST-1/ })).toBeNull();
  });

  it("offers mark-all only while something is unread, and view-all", () => {
    const onMarkAllRead = vi.fn();
    const onViewAll = vi.fn();
    const { unmount } = renderCenter({ onMarkAllRead, onViewAll });
    fireEvent.click(bellButton());
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.notificationCenter.markAllRead }));
    expect(onMarkAllRead).toHaveBeenCalledTimes(1);
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.notificationCenter.viewAll }));
    expect(onViewAll).toHaveBeenCalledTimes(1);
    unmount();

    renderCenter({ unreadCount: 0, onMarkAllRead });
    fireEvent.click(bellButton());
    expect(screen.queryByRole("button", { name: thMessages.common.notificationCenter.markAllRead })).toBeNull();
  });

  it("shows loading, error and empty states inside the open panel", () => {
    const { unmount } = renderCenter({ isLoading: true, notifications: [] });
    fireEvent.click(bellButton());
    expect(screen.getByText(thMessages.common.notificationCenter.loading)).toBeDefined();
    unmount();

    const errored = renderCenter({ isError: true, notifications: [] });
    fireEvent.click(bellButton());
    expect(screen.getByText(thMessages.common.notificationCenter.loadFailed)).toBeDefined();
    errored.unmount();

    renderCenter({ notifications: [], unreadCount: 0 });
    fireEvent.click(bellButton());
    expect(screen.getByText(thMessages.common.notificationCenter.empty)).toBeDefined();
  });
});
```

Run: `npx vitest run src/components/layout/NotificationCenter.test.tsx` → Expected: FAIL (ไม่มี `isError`/`onItemClick`/ปุ่มที่ชื่อ title ฯลฯ).

- [ ] **Step 6: เขียน `NotificationCenter.tsx` ใหม่เป็น controlled**

แทนที่ทั้งไฟล์ (markup/ class เดิมคงไว้; ส่วนที่เปลี่ยน: ไม่มี local list state, ปุ่มรายการ, Esc/focus return, badge cap, live region, loading/error, view all):

```tsx
"use client";

import React, { useCallback, useEffect, useId, useRef, useState } from "react";
import { IconBell, IconInfo, IconCheckCircle, IconAlertTriangle } from "@/components/common/Icons";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { cn } from "@/lib/utils/cn";
import { useTranslations } from "next-intl";

export interface NotificationItem {
  id: string;
  title: string;
  description?: string;
  timeText: string;
  type?: "info" | "success" | "warning";
  isRead?: boolean;
  /** Locale-prefixed in-app path, or null when the reader has no access to the target. */
  href?: string | null;
}

export interface NotificationCenterProps {
  notifications?: NotificationItem[];
  /** Server-side unread total for the badge; counted from `notifications` only when omitted. */
  unreadCount?: number;
  onItemClick?: (item: NotificationItem) => void;
  onMarkAllRead?: () => void;
  onViewAll?: () => void;
  onOpenChange?: (open: boolean) => void;
  isLoading?: boolean;
  isError?: boolean;
  className?: string;
}

const BADGE_CAP = 99;

export function NotificationCenter({
  notifications = [],
  unreadCount,
  onItemClick,
  onMarkAllRead,
  onViewAll,
  onOpenChange,
  isLoading = false,
  isError = false,
  className,
}: NotificationCenterProps) {
  const t = useTranslations("common.notificationCenter");
  const [isOpen, setIsOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);
  const buttonRef = useRef<HTMLButtonElement>(null);
  const panelId = useId();

  const setOpen = useCallback(
    (next: boolean) => {
      setIsOpen(next);
      onOpenChange?.(next);
    },
    [onOpenChange],
  );

  useEffect(() => {
    if (!isOpen) return;
    function handleClickOutside(event: MouseEvent) {
      if (event.target instanceof Node && menuRef.current && !menuRef.current.contains(event.target)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [isOpen, setOpen]);

  const handleKeyDown = (event: React.KeyboardEvent<HTMLDivElement>) => {
    if (event.key === "Escape" && isOpen) {
      setOpen(false);
      buttonRef.current?.focus();
    }
  };

  const unread = unreadCount ?? notifications.filter((n) => !n.isRead).length;
  const badgeText = unread > BADGE_CAP ? `${BADGE_CAP}+` : String(unread);

  return (
    <div ref={menuRef} onKeyDown={handleKeyDown} className={cn("relative inline-block text-left", className)}>
      <button
        ref={buttonRef}
        type="button"
        onClick={() => setOpen(!isOpen)}
        aria-expanded={isOpen}
        aria-controls={isOpen ? panelId : undefined}
        aria-label={t("title")}
        title={t("title")}
        className="relative flex h-11 w-11 items-center justify-center border border-erp-border bg-erp-surface text-erp-text-main hover:bg-erp-surface-subtle rounded-none"
      >
        <IconBell size={18} />
        {unread > 0 && (
          <span
            aria-hidden="true"
            className="absolute -top-1 -right-1 flex h-4 min-w-4 items-center justify-center bg-erp-navy px-0.5 text-[9px] font-bold text-white rounded-none"
          >
            {badgeText}
          </span>
        )}
      </button>
      {/* Polite live region: screen readers hear the count change when polling brings new items. */}
      <span role="status" aria-live="polite" className="sr-only">
        {unread > 0 ? t("unreadCount", { count: unread }) : ""}
      </span>

      {isOpen && (
        <div
          id={panelId}
          role="region"
          aria-label={t("title")}
          className="absolute right-0 top-full z-50 mt-1 w-80 max-w-[calc(100vw-2rem)] border border-erp-border bg-erp-surface shadow-lg rounded-none text-left"
        >
          <div className="flex items-center justify-between border-b border-erp-border p-3">
            <span className="text-xs font-bold text-erp-text-main uppercase tracking-wider">{t("title")}</span>
            {unread > 0 && onMarkAllRead && (
              <button
                type="button"
                onClick={onMarkAllRead}
                className="min-h-11 px-2 text-[11px] text-erp-navy hover:underline font-medium"
              >
                {t("markAllRead")}
              </button>
            )}
          </div>

          <div className="max-h-72 overflow-y-auto divide-y divide-erp-border">
            {isLoading ? (
              <MonoSpinner size="sm" label={t("loading")} />
            ) : isError ? (
              <p role="alert" className="p-4 text-center text-xs text-erp-text-muted">{t("loadFailed")}</p>
            ) : notifications.length === 0 ? (
              <p className="p-4 text-center text-xs text-erp-text-muted">{t("empty")}</p>
            ) : (
              notifications.map((n) => (
                <button
                  key={n.id}
                  type="button"
                  onClick={() => {
                    setOpen(false);
                    onItemClick?.(n);
                  }}
                  className={cn(
                    "block min-h-11 w-full p-3 text-left transition-colors rounded-none",
                    n.isRead ? "hover:bg-erp-surface-subtle/30" : "bg-erp-surface-subtle/50 hover:bg-erp-surface-subtle",
                  )}
                >
                  <span className="flex items-start gap-2">
                    {n.type === "success" ? (
                      <IconCheckCircle size={14} className="mt-0.5 text-emerald-600 shrink-0" />
                    ) : n.type === "warning" ? (
                      <IconAlertTriangle size={14} className="mt-0.5 text-amber-600 shrink-0" />
                    ) : (
                      <IconInfo size={14} className="mt-0.5 text-erp-navy shrink-0" />
                    )}
                    <span className="flex-1">
                      <span className={cn("block text-xs text-erp-text-main", n.isRead ? "font-normal" : "font-semibold")}>
                        {n.title}
                      </span>
                      {n.description && (
                        <span className="mt-0.5 block text-[11px] leading-relaxed text-erp-text-muted">{n.description}</span>
                      )}
                      <span className="mt-1 block font-mono text-[10px] text-erp-text-muted">{n.timeText}</span>
                    </span>
                  </span>
                </button>
              ))
            )}
          </div>

          {onViewAll && (
            <div className="border-t border-erp-border">
              <button
                type="button"
                onClick={() => {
                  setOpen(false);
                  onViewAll();
                }}
                className="min-h-11 w-full px-3 text-center text-[11px] font-medium text-erp-navy hover:underline"
              >
                {t("viewAll")}
              </button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}

NotificationCenter.displayName = "NotificationCenter";
```

Run: `npx vitest run src/components/layout/NotificationCenter.test.tsx` → Expected: PASS ทั้ง 8. (jsdom: `focus()`/`document.activeElement` และ `fireEvent.keyDown` ทำงานได้; จุดที่ jsdom พิสูจน์ไม่ได้คือ focus ring/ตำแหน่ง dropdown จริง — บันทึกไว้เป็นรายการที่ยังไม่ตรวจในเบราว์เซอร์ใน Task 12.)

- [ ] **Step 7: test container `NotificationBell` ที่ล้มก่อน**

`frontend/src/components/layout/NotificationBell.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import type { NotificationResponse } from "@/lib/api/api-client";
import { NotificationBell } from "./NotificationBell";

const state = vi.hoisted(() => ({
  membership: { id: "m-1" } as { id: string } | null,
  unread: 2,
  items: [] as NotificationResponse[],
  push: vi.fn(),
  markRead: vi.fn(),
  markAll: vi.fn(),
  listCalls: [] as boolean[],
}));

vi.mock("next/navigation", () => ({ useRouter: () => ({ push: state.push }) }));
vi.mock("@/lib/membership/selected-membership-context", () => ({
  useSelectedMembership: () => ({ selectedMembership: state.membership }),
}));
vi.mock("@/hooks/useNotifications", () => ({
  useUnreadNotificationCount: () => ({ data: { unreadCount: state.unread } }),
  useNotificationList: (_params: unknown, enabled: boolean) => {
    state.listCalls.push(enabled);
    return { data: enabled ? { items: state.items } : undefined, isLoading: false, isError: false };
  },
  useMarkNotificationRead: () => ({ mutateAsync: state.markRead }),
  useMarkAllNotificationsRead: () => ({ mutate: state.markAll }),
}));

const notification = (over: Partial<NotificationResponse>): NotificationResponse => ({
  id: "n-1",
  type: "purchase-order.approval-requested",
  payload: { documentNumber: "PO-1", actorDisplayName: "สมชาย" },
  deepLink: "/procurement/purchase-orders/po-1",
  createdAtUtc: "2026-10-08T00:00:00Z",
  readAtUtc: null,
  ...over,
});

const renderBell = () =>
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <NotificationBell />
    </NextIntlClientProvider>,
  );
const open = () => fireEvent.click(screen.getByRole("button", { name: thMessages.common.notificationCenter.title }));

describe("NotificationBell", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    state.membership = { id: "m-1" };
    state.unread = 2;
    state.items = [notification({})];
    state.listCalls = [];
    state.markRead.mockResolvedValue(undefined);
  });

  it("renders nothing without a selected membership", () => {
    state.membership = null;
    const { container } = renderBell();
    expect(container.firstChild).toBeNull();
  });

  it("does not fetch the list until the dropdown is opened", () => {
    renderBell();
    expect(state.listCalls.every((enabled) => enabled === false)).toBe(true);
    open();
    expect(state.listCalls.at(-1)).toBe(true);
  });

  it("marks an unread item read and then navigates to its locale-prefixed deep link", async () => {
    renderBell();
    open();
    fireEvent.click(screen.getByRole("button", { name: /PO-1/ }));

    await waitFor(() => expect(state.push).toHaveBeenCalledWith("/th/procurement/purchase-orders/po-1"));
    expect(state.markRead).toHaveBeenCalledWith("n-1");
    expect(state.markRead.mock.invocationCallOrder[0]).toBeLessThan(state.push.mock.invocationCallOrder[0]);
  });

  it("does not mark an already-read item again, and does not navigate when the link is null", async () => {
    state.items = [notification({ readAtUtc: "2026-10-08T01:00:00Z", deepLink: null })];
    renderBell();
    open();
    fireEvent.click(screen.getByRole("button", { name: /PO-1/ }));

    await waitFor(() => expect(screen.getByRole("button", { name: /PO-1/ })).toBeDefined());
    expect(state.markRead).not.toHaveBeenCalled();
    expect(state.push).not.toHaveBeenCalled();
  });

  it("still navigates when marking read fails (the item just stays unread)", async () => {
    state.markRead.mockRejectedValue(new Error("network"));
    renderBell();
    open();
    fireEvent.click(screen.getByRole("button", { name: /PO-1/ }));
    await waitFor(() => expect(state.push).toHaveBeenCalledTimes(1));
  });

  it("renders a neutral line for an unknown type instead of throwing", () => {
    state.items = [notification({ type: "customer.created", payload: {}, deepLink: null })];
    renderBell();
    open();
    expect(screen.getByText(thMessages.notifications.unknownType)).toBeDefined();
  });

  it("marks everything read and opens the full page from the footer", () => {
    renderBell();
    open();
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.notificationCenter.markAllRead }));
    expect(state.markAll).toHaveBeenCalledTimes(1);
    fireEvent.click(screen.getByRole("button", { name: thMessages.common.notificationCenter.viewAll }));
    expect(state.push).toHaveBeenCalledWith("/th/notifications");
  });
});
```

Run: `npx vitest run src/components/layout/NotificationBell.test.tsx` → Expected: FAIL (module `./NotificationBell` ไม่พบ).

- [ ] **Step 8: implement `NotificationBell.tsx`**

```tsx
"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale } from "next-intl";
import {
  useMarkAllNotificationsRead,
  useMarkNotificationRead,
  useNotificationList,
  useUnreadNotificationCount,
} from "@/hooks/useNotifications";
import { formatDateTime } from "@/lib/formatters/formatters";
import { localizedNotificationHref, useNotificationText } from "@/lib/notifications/notification-view";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { NotificationCenter, type NotificationItem } from "./NotificationCenter";

const DROPDOWN_PAGE_SIZE = 10;

/** Header bell: only mounted (and only polling) while a membership is selected. */
export function NotificationBell() {
  const { selectedMembership } = useSelectedMembership();
  if (!selectedMembership) return null;
  return <NotificationBellContent />;
}

function NotificationBellContent() {
  const router = useRouter();
  const locale = useLocale();
  const textOf = useNotificationText();
  const [isOpen, setIsOpen] = useState(false);

  const unread = useUnreadNotificationCount();
  const list = useNotificationList({ unreadOnly: false, page: 1, pageSize: DROPDOWN_PAGE_SIZE }, isOpen);
  const markRead = useMarkNotificationRead();
  const markAll = useMarkAllNotificationsRead();

  const items: NotificationItem[] = (list.data?.items ?? []).map((n) => ({
    id: n.id,
    title: textOf(n),
    timeText: formatDateTime(n.createdAtUtc, locale),
    isRead: n.readAtUtc !== null,
    href: localizedNotificationHref(locale, n.deepLink),
  }));

  const handleItemClick = async (item: NotificationItem) => {
    try {
      if (item.isRead !== true) {
        await markRead.mutateAsync(item.id);
      }
    } catch {
      // Marking read is best-effort: the item stays unread and the next poll shows it; opening the document must not depend on it.
    }
    if (item.href) {
      router.push(item.href);
    }
  };

  return (
    <NotificationCenter
      notifications={items}
      unreadCount={unread.data?.unreadCount ?? 0}
      isLoading={isOpen && list.isLoading}
      isError={isOpen && list.isError}
      onOpenChange={setIsOpen}
      onItemClick={handleItemClick}
      onMarkAllRead={() => markAll.mutate()}
      onViewAll={() => router.push(`/${locale}/notifications`)}
    />
  );
}
```

หมายเหตุ: `NotificationCenter` เป็นเจ้าของ state เปิด/ปิดเอง และปิดตัวเองเมื่อคลิกรายการหรือ "ดูทั้งหมด"; `isOpen` ใน container เป็นสำเนาจาก `onOpenChange` เพื่อ gate query ของรายการเท่านั้น (ไม่ดึงรายการก่อนเปิด).

Run: `npx vitest run src/components/layout/NotificationCenter.test.tsx src/components/layout/NotificationBell.test.tsx` → Expected: PASS ทั้งสองไฟล์ (8 + 7).

- [ ] **Step 9: test shell ที่ล้มก่อน**

ใน `frontend/src/components/layout/erp-shell.test.tsx` เพิ่มหลัง `vi.mock("@/lib/auth/auth-session", ...)`:

```tsx
vi.mock("./NotificationBell", () => ({
  NotificationBell: () => <div data-testid="notification-bell" />,
}));
```

และเพิ่มเคสท้าย `describe`:

```tsx
  it("places the notification bell in the header, before the theme and language controls", () => {
    renderWithClient(<ErpShell currentUser={mockCurrentUser} />);

    const bell = screen.getByTestId("notification-bell");
    expect(screen.getByRole("banner").contains(bell)).toBe(true);
  });
```

Run: `npx vitest run src/components/layout/erp-shell.test.tsx -t "notification bell"` → Expected: FAIL (ไม่พบ `notification-bell`).

- [ ] **Step 10: วางใน `erp-shell.tsx`**

เพิ่ม import `import { NotificationBell } from "./NotificationBell";` ต่อจาก `import { SidebarNav } from "./SidebarNav";` และใน `erp-header-right` ก่อน `{/* Theme switcher toggle */}`:

```tsx
          {/* In-app notifications (renders nothing without a selected membership) */}
          <NotificationBell />

```

Run: `npx vitest run src/components/layout` → Expected: PASS ทั้งโฟลเดอร์ (รวม `erp-shell.test.tsx` เดิมทุกเคส).

- [ ] **Step 11: gates และ commit**

Run: `npm run lint && npm run typecheck` → Expected: ผ่าน (ไม่มี `any`; `import` ที่ไม่ใช้ถูกลบแล้ว).
Run: `npx vitest run src/lib/notifications src/components/layout src/hooks/useNotifications.test.tsx` → Expected: PASS.

```bash
cd /Users/syaco/Documents/development/tan-erp
git add frontend/src/components/common/Icons.tsx frontend/src/lib/notifications/notification-view.ts frontend/src/lib/notifications/notification-view.test.tsx frontend/src/components/layout/NotificationCenter.tsx frontend/src/components/layout/NotificationCenter.test.tsx frontend/src/components/layout/NotificationBell.tsx frontend/src/components/layout/NotificationBell.test.tsx frontend/src/components/layout/erp-shell.tsx frontend/src/components/layout/erp-shell.test.tsx frontend/src/messages/th.json frontend/src/messages/en.json
git commit -F - <<'EOF'
feat(notifications): add header notification bell with unread badge and dropdown

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 11: หน้ารายการแจ้งเตือนเต็ม `/notifications`

**Files:**
- Create: `frontend/src/features/notifications/components/notification-list-page.tsx`, `frontend/src/features/notifications/components/notification-list-page.test.tsx`
- Create: `frontend/src/app/[locale]/(erp)/notifications/page.tsx`
- Modify: `frontend/src/messages/th.json`, `frontend/src/messages/en.json` (เพิ่ม `notifications.list`)

**Reuse check (ตรวจแล้ว):** โครงรายการมาตรฐานคือ `useListState` (URL-synced) + `PageHeader` + `ListToolbar`/`ListFilterSelect` + `DataTable` (pagination, error/retry, empty ในตัว) + `TableAction` + `StatusBadge` ตามตัวอย่าง `features/customers/components/customer-list.tsx` และ `.agents/skills/building-erp-lists/SKILL.md` → **ใช้ของกลางทั้งหมด ไม่สร้างตารางใหม่**. ข้อความ/ลิงก์ของแต่ละแถวใช้ `useNotificationText` + `localizedNotificationHref` จาก Task 10; data hooks จาก Task 9. Route อยู่ใน `app/[locale]/(erp)/` (ตรวจแล้วว่ามี `layout.tsx` ของ ERP shell ที่ห่อ `ErpShell`). **ไม่ใช้ `PermissionGuard`** — การแจ้งเตือนของตนเองไม่มี permission key (Task 5 `ResolveMembershipAsync`); backend ตรวจ membership และ own-only เอง. เส้นแบ่ง API: `pageSize` ≤ 50 แต่ `useListState` มีตัวเลือก 100 → หน้านี้ส่ง `Math.min(limit, 50)` ไปที่ API และให้ DataTable แสดงค่าเดียวกัน (ไม่เดาค่าอื่น).

**Mark-all ไม่ใช้ ConfirmationModal (ตัดสินใจ):** AGENTS.md บังคับ modal กับ Delete/Void/Cancel และการกระทำเสี่ยงสูง. "อ่านทั้งหมด" เปลี่ยนเฉพาะ flag อ่านของผู้ใช้เอง ไม่ลบ ไม่แตะข้อมูลธุรกิจ และไม่กระทบผู้อื่น → ความเสี่ยงต่ำ จึงไม่มี modal; แต่ปุ่มต้อง disable ระหว่างยิง (Double Submit Protection) และ disable เมื่อไม่มี unread. ถ้าต่อมามี "ลบการแจ้งเตือน" ต้องมี modal ตามกฎ.

- [ ] **Step 1: messages `notifications.list` (th + en)**

```bash
cd /Users/syaco/Documents/development/tan-erp/frontend
python3 - <<'PY'
import json

LIST = {
    "src/messages/th.json": {
        "title": "การแจ้งเตือน",
        "subtitle": "เอกสารที่รอคุณอนุมัติและการแจ้งเตือนอื่นของระบบ",
        "filterLabel": "สถานะการอ่าน",
        "filterUnread": "ยังไม่อ่าน",
        "columnMessage": "ข้อความ",
        "columnReceivedAt": "เวลาที่ได้รับ",
        "columnStatus": "สถานะ",
        "statusUnread": "ยังไม่อ่าน",
        "statusRead": "อ่านแล้ว",
        "markRead": "ทำเครื่องหมายว่าอ่านแล้ว",
        "open": "เปิดเอกสาร",
        "markAllRead": "อ่านทั้งหมด",
        "empty": "ไม่มีการแจ้งเตือน",
    },
    "src/messages/en.json": {
        "title": "Notifications",
        "subtitle": "Documents awaiting your approval and other system notices",
        "filterLabel": "Read status",
        "filterUnread": "Unread",
        "columnMessage": "Message",
        "columnReceivedAt": "Received",
        "columnStatus": "Status",
        "statusUnread": "Unread",
        "statusRead": "Read",
        "markRead": "Mark as read",
        "open": "Open document",
        "markAllRead": "Mark all as read",
        "empty": "No notifications",
    },
}
for path, keys in LIST.items():
    raw = open(path, encoding="utf-8").read()
    data = json.loads(raw)
    assert json.dumps(data, indent=2, ensure_ascii=False) + "\n" == raw, f"{path} does not round-trip; edit by hand"
    assert "list" not in data["notifications"], f"{path} already has notifications.list"
    data["notifications"]["list"] = keys
    open(path, "w", encoding="utf-8").write(json.dumps(data, indent=2, ensure_ascii=False) + "\n")
PY
git diff --stat -- src/messages
```

Expected: เพิ่มเฉพาะบล็อก `notifications.list` (ตัวกรองมีค่าเดียว `unread` เพราะ API รองรับเฉพาะ `unreadOnly`).

- [ ] **Step 2: test หน้ารายการที่ล้มก่อน**

`frontend/src/features/notifications/components/notification-list-page.test.tsx`:

```tsx
import React from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { fireEvent, render, screen, within } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import type { NotificationResponse } from "@/lib/api/api-client";
import { NotificationListPage } from "./notification-list-page";

const state = vi.hoisted(() => ({
  search: "",
  items: [] as NotificationResponse[],
  totalCount: 0,
  isError: false,
  listArgs: [] as unknown[],
  push: vi.fn(),
  replace: vi.fn(),
  markRead: vi.fn(),
  markAll: vi.fn(),
  markAllPending: false,
}));

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: state.push, replace: state.replace }),
  usePathname: () => "/th/notifications",
  useSearchParams: () => new URLSearchParams(state.search),
}));
vi.mock("@/hooks/useNotifications", () => ({
  useNotificationList: (params: unknown) => {
    state.listArgs.push(params);
    return {
      data: { items: state.items, pagination: { page: 1, pageSize: 25, totalCount: state.totalCount, totalPages: 1 } },
      isLoading: false,
      isError: state.isError,
      error: null,
      refetch: vi.fn(),
    };
  },
  useMarkNotificationRead: () => ({ mutate: state.markRead, isPending: false }),
  useMarkAllNotificationsRead: () => ({ mutate: state.markAll, isPending: state.markAllPending }),
}));

const row = (over: Partial<NotificationResponse>): NotificationResponse => ({
  id: "n-1",
  type: "estimate.approval-requested",
  payload: { documentNumber: "EST-1", actorDisplayName: "สมชาย" },
  deepLink: "/estimates/e-1",
  createdAtUtc: "2026-10-08T00:00:00Z",
  readAtUtc: null,
  ...over,
});

const renderPage = () =>
  render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <NotificationListPage />
    </NextIntlClientProvider>,
  );

describe("NotificationListPage", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    state.search = "";
    state.items = [row({}), row({ id: "n-2", type: "customer.created", payload: {}, deepLink: null, readAtUtc: "2026-10-08T01:00:00Z" })];
    state.totalCount = 2;
    state.isError = false;
    state.listArgs = [];
    state.markAllPending = false;
  });

  it("lists registered and unknown types without throwing, with read status text", () => {
    renderPage();
    expect(screen.getByText("สมชาย ส่งใบประมาณราคา EST-1 เพื่อรออนุมัติ")).toBeDefined();
    expect(screen.getByText(thMessages.notifications.unknownType)).toBeDefined();
    expect(screen.getAllByText(thMessages.notifications.list.statusUnread).length).toBeGreaterThanOrEqual(1);
  });

  it("queries all rows by default and unread only when the filter is set in the URL, never above 50 per page", () => {
    renderPage();
    expect(state.listArgs.at(-1)).toEqual({ unreadOnly: false, page: 1, pageSize: 25 });

    state.search = "status=unread&limit=100";
    state.listArgs = [];
    renderPage();
    expect(state.listArgs.at(-1)).toEqual({ unreadOnly: true, page: 1, pageSize: 50 });
  });

  it("marks a single row read from its action, only for unread rows", () => {
    renderPage();
    const markButtons = screen.getAllByRole("button", { name: thMessages.notifications.list.markRead });
    expect(markButtons).toHaveLength(1);
    fireEvent.click(markButtons[0]);
    expect(state.markRead).toHaveBeenCalledWith("n-1");
  });

  it("links a row to its locale-prefixed document, and offers no link when the reader lost access", () => {
    renderPage();
    const links = screen.getAllByRole("link", { name: thMessages.notifications.list.open });
    expect(links).toHaveLength(1);
    expect(links[0].getAttribute("href")).toBe("/th/estimates/e-1");
  });

  it("marks all read without a confirmation modal, and locks the button while the request runs", () => {
    renderPage();
    fireEvent.click(screen.getByRole("button", { name: thMessages.notifications.list.markAllRead }));
    expect(state.markAll).toHaveBeenCalledTimes(1);
    expect(screen.queryByRole("dialog")).toBeNull();
  });

  it("disables mark-all while pending or when nothing is unread", () => {
    state.markAllPending = true;
    const first = renderPage();
    expect((screen.getByRole("button", { name: thMessages.notifications.list.markAllRead }) as HTMLButtonElement).disabled).toBe(true);
    first.unmount();

    state.markAllPending = false;
    state.items = [row({ readAtUtc: "2026-10-08T01:00:00Z" })];
    renderPage();
    expect((screen.getByRole("button", { name: thMessages.notifications.list.markAllRead }) as HTMLButtonElement).disabled).toBe(true);
  });

  it("shows the table error state when loading fails", () => {
    state.isError = true;
    renderPage();
    expect(screen.getByText(thMessages.notifications.loadFailed)).toBeDefined();
  });

  it("shows the empty state when there are no rows", () => {
    state.items = [];
    state.totalCount = 0;
    renderPage();
    expect(within(document.body).getByText(thMessages.notifications.list.empty)).toBeDefined();
  });
});
```

Run: `npx vitest run src/features/notifications` → Expected: FAIL (module `./notification-list-page` ไม่พบ).

- [ ] **Step 3: implement หน้ารายการ**

`frontend/src/features/notifications/components/notification-list-page.tsx`:

```tsx
"use client";

import React from "react";
import { useLocale, useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Button } from "@/components/ui/Button";
import { ListToolbar } from "@/components/ui/ListToolbar";
import { ListFilterSelect } from "@/components/ui/ListFilterSelect";
import { StatusBadge } from "@/components/ui/StatusBadge";
import { TableAction, TableActionGroup } from "@/components/ui/TableAction";
import { IconCheckCircle, IconEye } from "@/components/common/Icons";
import { useListState, type ListFilterRecord, type ListPageSize } from "@/hooks/useListState";
import {
  useMarkAllNotificationsRead,
  useMarkNotificationRead,
  useNotificationList,
} from "@/hooks/useNotifications";
import type { NotificationResponse } from "@/lib/api/api-client";
import { formatDateTime } from "@/lib/formatters/formatters";
import { localizedNotificationHref, useNotificationText } from "@/lib/notifications/notification-view";

/** The API rejects pageSize above this; useListState also offers 100, so the request is clamped (never guessed). */
const MAX_API_PAGE_SIZE = 50;

interface NotificationFilters extends ListFilterRecord {
  status?: string;
}

export function NotificationListPage() {
  const t = useTranslations("notifications");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const textOf = useNotificationText();

  const listState = useListState<NotificationFilters>({
    schema: { single: ["status"] },
  });
  const pageSize = Math.min(listState.params.limit, MAX_API_PAGE_SIZE);

  const { data, isLoading, isError, error, refetch } = useNotificationList({
    unreadOnly: listState.params.filters.status === "unread",
    page: listState.params.page,
    pageSize,
  });
  const markRead = useMarkNotificationRead();
  const markAll = useMarkAllNotificationsRead();

  const items = data?.items ?? [];
  const totalItems = data?.pagination.totalCount ?? 0;
  const totalPages = data?.pagination.totalPages ?? 0;
  const hasUnread = items.some((n) => n.readAtUtc === null);

  const columns: Column<NotificationResponse>[] = [
    {
      id: "message",
      header: t("list.columnMessage"),
      className: "min-w-[280px]",
      cell: (_value, n) => (
        <span className={n.readAtUtc === null ? "text-xs font-semibold text-erp-text-main" : "text-xs text-erp-text-main"}>
          {textOf(n)}
        </span>
      ),
    },
    {
      id: "receivedAt",
      header: t("list.columnReceivedAt"),
      className: "min-w-[160px]",
      cell: (_value, n) => <span className="font-mono text-[11px] text-erp-text-muted">{formatDateTime(n.createdAtUtc, locale)}</span>,
    },
    {
      id: "status",
      header: t("list.columnStatus"),
      className: "min-w-[110px]",
      cell: (_value, n) => (
        <StatusBadge
          label={n.readAtUtc === null ? t("list.statusUnread") : t("list.statusRead")}
          variant={n.readAtUtc === null ? "info" : "neutral"}
        />
      ),
    },
    {
      id: "actions",
      header: tCommon("fields.actions"),
      className: "w-[100px]",
      sticky: "right",
      isAction: true,
      cell: (_value, n) => {
        const href = localizedNotificationHref(locale, n.deepLink);
        return (
          <TableActionGroup>
            {n.readAtUtc === null && (
              <TableAction
                icon={<IconCheckCircle size={14} />}
                label={t("list.markRead")}
                onClick={() => markRead.mutate(n.id)}
              />
            )}
            {href !== null && <TableAction icon={<IconEye size={14} />} label={t("list.open")} href={href} variant="primary" />}
          </TableActionGroup>
        );
      },
    },
  ];

  return (
    <div className="flex flex-col gap-5">
      <PageHeader
        title={t("list.title")}
        subtitle={t("list.subtitle")}
        actions={
          <Button
            type="button"
            variant="outline"
            size="md"
            disabled={markAll.isPending || !hasUnread}
            isLoading={markAll.isPending}
            onClick={() => markAll.mutate()}
          >
            {t("list.markAllRead")}
          </Button>
        }
      />

      <ListToolbar>
        <ListFilterSelect
          id="filter-notification-status"
          label={t("list.filterLabel")}
          value={listState.params.filters.status ?? ""}
          onChange={(value) => listState.actions.setFilter("status", value === "" ? undefined : value)}
          options={[{ value: "unread", label: t("list.filterUnread") }]}
          widthClassName="w-full sm:w-44"
        />
      </ListToolbar>

      <DataTable<NotificationResponse>
        columns={columns}
        data={items}
        isLoading={isLoading}
        isError={isError}
        error={isError ? t("loadFailed") : null}
        onRetry={() => refetch()}
        emptyTitle={t("list.empty")}
        pagination={{ page: listState.params.page, limit: pageSize, totalPages, totalItems }}
        onPageChange={(p) => listState.actions.setPage(p)}
        onLimitChange={(limit) => listState.actions.setLimit(limit as ListPageSize)}
      />
    </div>
  );
}
```

ตรวจแล้ว: `Button` มี `isLoading`, `ListToolbar` รับ `children`, `IconEye`/`IconCheckCircle`/`TableActionGroup` มีอยู่. `setLimit(limit as ListPageSize)` เป็น cast แบบเดียวกับ `customer-list.tsx` (ไม่ใช่ `any`). Mark read ใช้ `mutate` (ไม่ใช่ `mutateAsync`) เพื่อไม่ให้เกิด unhandled rejection.

Run: `npx vitest run src/features/notifications` → Expected: PASS ทั้ง 8.

- [ ] **Step 4: route**

`frontend/src/app/[locale]/(erp)/notifications/page.tsx`:

```tsx
"use client";

import React, { use } from "react";
import { notFound } from "next/navigation";
import { isSupportedLocale } from "@/lib/i18n/locales";
import { NotificationListPage } from "@/features/notifications/components/notification-list-page";

interface NotificationsPageProps {
  params: Promise<{ locale: string }>;
}

export default function NotificationsPage({ params }: NotificationsPageProps) {
  const { locale } = use(params);

  if (!isSupportedLocale(locale)) {
    notFound();
  }

  return <NotificationListPage />;
}
```

Run: `ls "frontend/src/app/[locale]/(erp)/notifications/page.tsx"` → Expected: พบไฟล์. ไม่ใส่ `PermissionGuard` โดยตั้งใจ (ดู Reuse check).

- [ ] **Step 5: gates และ commit**

Run (ใน `frontend/`): `npm run lint && npm run typecheck && npx vitest run src/features/notifications src/components/layout src/lib/notifications src/hooks/useNotifications.test.tsx` → Expected: ผ่านทั้งหมด.
Run: `npm run build` → Expected: สำเร็จ และ route `/[locale]/notifications` ปรากฏในรายการ route.

```bash
cd /Users/syaco/Documents/development/tan-erp
git add frontend/src/features/notifications "frontend/src/app/[locale]/(erp)/notifications" frontend/src/messages/th.json frontend/src/messages/en.json
git commit -F - <<'EOF'
feat(notifications): add full notification list page with unread filter and mark read

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```

---

## Task 12: Verification record และ sync เอกสารสถานะ

**Files:**
- Create: `docs/05-engineering/notification-foundation-verification.md` (ชื่อตามที่ลิงก์ไว้แล้วใน `notification-api-contract.md` Task 1)
- Modify: `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md` (แถว G-02), `docs/00-overview/implementation-roadmap.md` (แถว Foundation), `docs/README.md`, `docs/03-contracts/notification-api-contract.md` (บรรทัดสถานะ)

ขั้นนี้ไม่เขียนผลที่ไม่ได้รัน: ตัวเลขทุกตัวในเอกสารต้องมาจาก output จริงของ Step 1 (รูปแบบเดียวกับ `docs/05-engineering/shared-attachment-signature-verification.md`).

- [ ] **Step 1: รัน focused gate และเก็บ output**

```bash
cd /Users/syaco/Documents/development/tan-erp
dotnet build backend/TanErp.slnx
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~Notification"
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~Notification" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateEndpointsTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~CostRecordEndpointsTests|FullyQualifiedName~ItemCatalogEstimateFlowTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ProcurementEndpointsTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~ProjectControlEndpointsTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~MrpEndpointsTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~IdentityAdministrationEndpointsTests|FullyQualifiedName~UsersEndpointsTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
cd frontend
npm run check:api
npx vitest run src/features/notifications src/components/layout src/lib/notifications src/lib/api/api-client.test.ts src/hooks
npm run lint
npm run typecheck
npm run build
```

Expected: ทุกคำสั่งออกด้วย exit 0 (integration ต้องมี Docker). จดจำนวน passed/failed ของแต่ละบรรทัดไว้ใช้ใน Step 2. ถ้าคำสั่งใดล้ม **หยุดและแก้ที่ task เจ้าของก่อน** ห้ามเขียนเอกสารว่าผ่าน. ตรวจว่าไม่มี Email/outbox หลงเหลือในโค้ด: `grep -rn "IEmailSender\|NotificationOutbox\|EmailOutbox" backend/src frontend/src` → Expected: ไม่พบ.

- [ ] **Step 2: เขียน `docs/05-engineering/notification-foundation-verification.md`**

โครงเอกสาร (ภาษาไทย; แทนตัวเลขในตารางด้วยค่าจาก Step 1 — ห้ามคงข้อความในวงเล็บไว้):

```markdown
# Notification Foundation Verification (G-02)

## 1. สถานะ

Implemented <วันที่ของวันรัน Step 1> ด้วยผล **focused tests เท่านั้น** (in-app เท่านั้น) — ยังไม่ผ่านการตรวจ bell dropdown ในเบราว์เซอร์จริง, full suite, Playwright หรือ UAT ของ Role จริง. สัญญา: [Notification API Contract](../03-contracts/notification-api-contract.md); การตัดสินใจ: [ADR 0018](../adr/0018-in-app-notification-foundation.md); แผน: [แผน G-02](../superpowers/plans/2026-10-08-g02-notification-foundation.md).

## 2. ผลที่รันจริง (<วันที่>)

| คำสั่ง | ผล |
| --- | --- |
| `dotnet build backend/TanErp.slnx` | (Warning/Error จาก output) |
| Unit `FullyQualifiedName~Notification` | (passed/failed จาก output) |
| Integration `FullyQualifiedName~Notification` (`-m:1`) | (passed/failed) |
| Regression Integration: `EstimateEndpointsTests`; `CostRecordEndpointsTests\|ItemCatalogEstimateFlowTests`; `ProcurementEndpointsTests`; `ProjectControlEndpointsTests`; `MrpEndpointsTests`; `IdentityAdministrationEndpointsTests\|UsersEndpointsTests` (`-m:1`, แยกคำสั่ง) | (passed/failed ต่อคำสั่ง) |
| `OpenApiContractTests` / ArchitectureTests | (passed/failed) |
| `npm run check:api` / `lint` / `typecheck` / `build` | (ผล) |
| `npx vitest run src/features/notifications src/components/layout src/lib/notifications src/lib/api/api-client.test.ts src/hooks` | (จำนวนไฟล์/tests) |

สิ่งที่ชุดเหล่านี้พิสูจน์: Notification สร้างใน transaction เดียวกับ business change (rollback = ไม่มี notification; commit = มี); ผู้รับ = ผู้ถือ permission อนุมัติใน Organization/Branch ของเอกสาร โดยตัดผู้ทำ (maker) ทั้งที่ resolver และ planner; อ่าน/mark read ได้เฉพาะของตน, ข้าม Organization = 404 `NOTIFICATION_NOT_FOUND`; payload ผ่าน allowlist (ไม่มีต้นทุน/ราคา/margin); deep link ตามสิทธิ์ปัจจุบัน; registry ครบคู่กับ `NotificationTypes` และ whitelist ฝั่ง UI + ข้อความ th/en; Frontend: badge cap 99+, ไม่ดึงรายการก่อนเปิด, mark read ก่อนนำทาง, type ที่ไม่รู้จักแสดงบรรทัดกลาง, หน้ารายการใช้ DataTable กลาง และ regression ของทั้งหกแหล่ง event ผ่านโดยไม่เปลี่ยนพฤติกรรมเดิม.

### สิ่งที่ไม่ได้รัน

Full backend suite, full vitest suite, Playwright journey, UAT ด้วย Role จริง, **การตรวจ bell dropdown ในเบราว์เซอร์จริง**.

## 3. การตรวจในเบราว์เซอร์

**ยังไม่ได้ตรวจ** ต้องยืนยันด้วยมือ: (1) Esc ปิด dropdown แล้ว focus กลับที่ปุ่มกระดิ่ง, (2) คลิกนอกกล่องปิด, (3) dropdown ไม่ล้นจอที่ความกว้าง 375px, (4) ปุ่มกระดิ่งบนพื้น header สีกรมท่าเห็นชัดทั้งธีมสว่าง/มืด และ focus ring มองเห็น, (5) badge อัปเดตภายใน ~30 วินาทีหลังมี notification ใหม่ และหยุด poll เมื่อสลับแท็บ (ดู Network), (6) สกรีนรีดเดอร์ประกาศจำนวนที่ยังไม่อ่าน.

## 4. ข้อจำกัดและการตัดสินใจ

- **in-app เท่านั้น** — ไม่มี email, LINE, SMS (เลื่อนตามคำสั่งผู้ใช้; ไม่มี `IEmailSender`/outbox; ADR 0018 ระบุเงื่อนไขประเมินใหม่เมื่อมีช่องทางส่งออก).
- Polling ทุก 30 วินาที (หยุดเมื่อแท็บซ่อน) — ไม่มี push/websocket; ผู้ใช้อาจเห็นช้าได้สูงสุดราว 30 วินาที.
- Event source มี 6 แหล่ง "ส่งเข้าสถานะรออนุมัติ" เท่านั้น. Event ประเภท Future ต้องมี scheduler/background job ซึ่งยังไม่มี: ใบประกันใกล้หมด, งานบริการเกิน SLA (G-18), Membership ใกล้หมดอายุ (G-21) — แต่ละเรื่องเพิ่ม type ตามขั้นตอนในสัญญาได้โดยไม่แก้โค้ดกลาง.
- ผู้ทำ (maker) ถูกตัดสองชั้น (resolver และ planner) — ผู้ทำที่ถือสิทธิ์อนุมัติเองจะไม่ได้รับแจ้งเอกสารของตน.
- Estimate แจ้งเฉพาะผู้ตรวจขั้นแรกของ route; ผู้ตรวจขั้นถัดไปยังไม่ได้รับแจ้งเมื่อขั้นก่อนอนุมัติ (Validation Question ข้อ 2 ค้างที่ Sales/Security).
- **ไม่ได้กำหนด retention/purge** ของแถวที่อ่านแล้ว (Validation Question ข้อ 1 ค้างที่ Security + Operations) — ตารางจะโตต่อเนื่อง; มี index `(organization, recipient, createdAt)` รองรับระยะแรก.
- a11y: dropdown ไม่มี arrow-key roving (ใช้ Tab/Enter/Space ของปุ่มจริง); ยังไม่ผ่านการตรวจด้วยสกรีนรีดเดอร์.
- กฎผู้รับเป็นค่าเริ่มต้น TEST_ONLY รอ Security/Operations ยืนยัน.
```

เมื่อเขียนจริง ให้ลบคำอธิบายในวงเล็บของตารางและแทนด้วยตัวเลขจาก Step 1.

- [ ] **Step 3: sync เอกสารสถานะ**

(`DATE=$(date +%F)` ใช้วันที่เดียวกับที่ใส่ในเอกสาร Step 2)

1. `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md` แถว G-02 (บรรทัด `| G-02 | ...`): แทนด้วย
   `| G-02 | Notification Foundation (in-app เท่านั้น; อีเมลเลื่อน) | A | Identity | Security + Operations | [แผน G-02](2026-10-08-g02-notification-foundation.md) — Implemented $DATE (in-app only; focused tests; browser check + full gate pending) |`
   และในส่วน `### G-02 — Notification Foundation` เปลี่ยนรายการ "Email channel ผ่าน `IEmailSender`" เป็น `- [ ] (Future) Email channel — เลื่อนตามคำสั่งผู้ใช้; ไม่อยู่ใน slice G-02 รอบแรก (ดู [ADR 0018](../../adr/0018-in-app-notification-foundation.md))` และ "ADR: เลือก outbox ร่วม..." เป็น `- [x] ADR 0018: ไม่ใช้ outbox, ไม่แตะ Finance` (ขีด `[x]` เฉพาะรายการที่ทำจริง: ADR, Domain, event sources 6 แหล่ง, Frontend, Tests; ใบประกันใกล้หมด/SLA คงเป็น `[ ]` Future).
2. `docs/00-overview/implementation-roadmap.md` แถว Foundation: ต่อท้ายคอลัมน์ "สิ่งที่มี/ข้อจำกัด" ด้วย `; การแจ้งเตือนในระบบ (G-02, $DATE, in-app only, focused tests; ยังไม่ตรวจ bell ในเบราว์เซอร์/full gate; ไม่มี email/push): bell + หน้า /notifications สำหรับเอกสารรออนุมัติ 6 แหล่ง` และเพิ่มลิงก์ [Notification Verification](../05-engineering/notification-foundation-verification.md) ในคอลัมน์หลักฐานของแถวเดียวกัน.
3. `docs/README.md`: เพิ่มแถวถัดจาก Quick Estimate Verification: `| ผลตรวจ Notification Foundation (G-02) | [Notification Verification](05-engineering/notification-foundation-verification.md) |` (แถว API contract ลงทะเบียนแล้วใน Task 1).
4. `docs/03-contracts/notification-api-contract.md`: บรรทัด `**สถานะ:**` เปลี่ยนจาก "Draft → Implemented เมื่อ G-02 เสร็จ" เป็น `Implemented $DATE (in-app only; focused tests)` คงลิงก์ Verification/ADR และประโยค "รอบนี้ไม่มีช่องทางอีเมล (Future)".

- [ ] **Step 4: ตรวจลิงก์สัมพัทธ์ของเอกสารที่แก้**

```bash
cd /Users/syaco/Documents/development/tan-erp
python3 - <<'PY'
import os, re, sys
files = [
    "docs/05-engineering/notification-foundation-verification.md",
    "docs/03-contracts/notification-api-contract.md",
    "docs/adr/0018-in-app-notification-foundation.md",
    "docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md",
    "docs/00-overview/implementation-roadmap.md",
    "docs/README.md",
]
missing = []
for f in files:
    text = open(f, encoding="utf-8").read()
    for target in re.findall(r"\]\(([^)#\s]+)(?:#[^)]*)?\)", text):
        if re.match(r"^[a-z]+:", target):
            continue
        if not os.path.exists(os.path.normpath(os.path.join(os.path.dirname(f), target))):
            missing.append((f, target))
for f, t in missing:
    print(f"BROKEN {f} -> {t}")
sys.exit(1 if missing else 0)
PY
```

Expected: exit 0 และไม่มีบรรทัด `BROKEN`. ถ้ามีลิงก์เสียที่มีอยู่ก่อนงานนี้ในไฟล์ที่ไม่ได้แก้ในส่วนของเรา ให้บันทึกในรายงานแต่ไม่แก้ (Minimal Blast Radius).

- [ ] **Step 5: final focused gate (ย้ำก่อน commit)**

Run: `git status --short` → Expected: เฉพาะไฟล์เอกสารของ Task นี้ (ไม่มี `.env`, `bin/`, `obj/`, `.next/`).
Run: `cd frontend && npx vitest run src/features/notifications src/components/layout src/lib/notifications src/hooks && npm run lint && npm run typecheck` → Expected: ผ่าน.

- [ ] **Step 6: commit**

```bash
cd /Users/syaco/Documents/development/tan-erp
git add docs/05-engineering/notification-foundation-verification.md docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md docs/00-overview/implementation-roadmap.md docs/README.md docs/03-contracts/notification-api-contract.md
git commit -F - <<'EOF'
docs(notifications): add verification record and update roadmap

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>
EOF
```
