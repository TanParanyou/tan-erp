# G-21 Membership ใกล้หมดอายุ (CP-02 ที่ค้าง) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. Read `AGENTS.md`, `design.md`, `CONTEXT.md` and `.agents/skills/building-erp-apis/SKILL.md` before Task 1 (`building-erp-forms` ไม่จำเป็น: slice นี้ไม่มีฟอร์ม). Reply to the user in Thai; code, identifiers and code comments stay in English. macOS: use BSD `sed -i ''` (never GNU `sed -i`).

**กฎของผู้ใช้ที่บังคับทุก Task (สำคัญ):** implementer **ห้ามรันชุดทดสอบใด ๆ** — ไม่มี `dotnet test`, ไม่มี `vitest`/`npx vitest`, ไม่มี mutation check, ไม่เปิดเบราว์เซอร์ — แต่ **ต้องเขียน test ทุกตัวที่แผนระบุ**. การตรวจของทุก Task คือ **compile-only**: backend `dotnet build backend/TanErp.slnx --nologo -v q` (ต้อง `0 Warning(s)` `0 Error(s)`; `TreatWarningsAsErrors` เปิดอยู่ และโปรเจกต์ test อยู่ใน solution จึงถูก compile ด้วย); frontend `cd frontend && npx tsc --noEmit`, `npm run lint`, ตรวจ key parity th/en, และ `npm run build` ครั้งเดียวใน Task 6. ไม่มีขั้น RED/GREEN ที่ต้องรัน.

**Goal:** ให้ผู้ดูแลระบบรู้ล่วงหน้าเมื่อ Membership ของใครใกล้หมดอายุ (`Membership.ExpiresAtUtc`) ผ่านการแจ้งเตือนในระบบของ G-02 โดยไม่ต้องเปิดหน้าผู้ใช้ตรวจเอง — ตอนนี้ Membership ที่หมดอายุทำให้ผู้ใช้เข้าระบบไม่ได้ทันทีโดยไม่มีใครรู้ล่วงหน้า (และ guard `LAST_ADMINISTRATOR_REQUIRED` ตรวจเฉพาะตอนแก้ ไม่ตรวจตอนหมดอายุตามเวลา). นี่คือ **scheduler ตัวแรกของ repo** จึงวาง pattern เล็ก ๆ ที่ G-18 (SLA) และใบประกันนำไปใช้ซ้ำได้.

**Architecture:** (1) Application: `MembershipExpiryReminderPlanner` (pure: เลือก threshold, สร้าง transition id แบบ deterministic), options, port `IMembershipExpiryReminderService`. (2) Infrastructure: `MembershipExpiryReminderService` สแกน Membership ที่จะหมดอายุใน transaction เดียว ล็อกด้วย `pg_try_advisory_xact_lock` แล้วเรียก `INotificationPublisher` เดิม — ใช้ dedupe unique index เดิมของ G-02 เป็นตัวกันซ้ำ. (3) Api: `MembershipExpiryReminderWorker : BackgroundService` เรียก service เป็นรอบ ๆ ด้วย `PeriodicTimer`; **ไม่ลงทะเบียนใน environment `Test`**. (4) Notification type ใหม่ `membership.expiring` เป็น **system-generated** (ไม่มี actor) — ต้องขยายแกนกลาง G-02 เล็กน้อย (`ActorUserId` เป็น nullable + descriptor flag). (5) Frontend: เพิ่ม type + ข้อความ th/en เท่านั้น.

**Tech Stack:** .NET 9, EF Core 9 + Npgsql, PostgreSQL, xUnit + Testcontainers (`postgres:17-alpine`; ผู้ใช้เป็นคนรัน), Next.js + TypeScript strict, next-intl, Vitest.

## Scope

**ทำในรอบนี้ (1 จาก 3 ข้อของ G-21):** แจ้งเตือน Membership ใกล้หมดอายุถึงผู้ดูแล + scheduler ขั้นต่ำ.

**ไม่ทำ (เป็น Decision รอผู้ใช้/Security — ห้าม implement):** (2) User หลาย Organization, (3) PII visibility/retention ใน Audit → ดู [G-21 Open Decisions](2026-10-12-g21-open-decisions.md). Task 7 **ห้ามติ๊ก** สองข้อนี้ว่าเสร็จ.

**ไม่ทำโดยตั้งใจ:** ตัวบ่งชี้ในหน้ารายการผู้ใช้ (หน้า `/settings/users/[id]` แสดง `expiresAtUtc` ของแต่ละ Membership อยู่แล้ว — ไม่จำเป็นต่อเป้าหมายและจะขยาย contract ของ list); อีเมล/ช่องทางอื่น (เลื่อนตาม [ADR 0018](../../adr/0018-in-app-notification-foundation.md)); แจ้งผู้ใช้เจ้าของ Membership เอง; แจ้ง "หมดอายุแล้ว"; ตั้งค่าผ่าน UI; endpoint/OpenAPI/error code/migration ใหม่ (ไม่มี — ดู Decisions D11).

## Findings จากการสำรวจโค้ด (ตรวจแล้ว)

| หัวข้อ | สิ่งที่พบ | ผลต่อแผน |
| --- | --- | --- |
| ไม่มี background job เลย | ค้น `BackgroundService`/`IHostedService`/`Quartz` ทั้ง `backend/` ไม่พบ (มีแต่ G-02 ที่ระบุว่า "ยังไม่มี job ตามเวลา": gap plan :99, `notification-foundation-verification.md:66`). `Program.cs` ไม่มี `AddHostedService`; มีการแยก environment `Test` อยู่แล้ว (`Program.cs:73, 268, 299`) | สร้าง worker เล็กเองโดยไม่เพิ่ม package; ข้ามการลงทะเบียนใน `Test` ตาม pattern เดิม |
| Clock | `IClock { UtcNow }` (`Application/Common/Abstractions/IClock.cs`), ลงทะเบียน singleton `SystemClock` (`Program.cs:88`) | service รับ `IClock` → test ใช้ fake clock เรียก service ตรง ๆ ได้ |
| Membership entity | `StartsAtUtc?`, `ExpiresAtUtc?`, `IsActive`, `RowVersion` (`Domain/Organization/Membership.cs:15-21`); `IsActiveAt` ถือว่าหมดอายุเมื่อ `instant >= ExpiresAtUtc` (:52-60); `UpdateAssignment` เปลี่ยน `ExpiresAtUtc` พร้อม bump `RowVersion` (:67-76) | การเปลี่ยน `ExpiresAtUtc` เกิดผ่านจุดเดียว (PATCH `/admin/memberships/{id}`, `IdentityAdministrationStore.cs:242-271`) → ใช้ `ExpiresAtUtc` ใน seed ของ dedupe เพื่อ re-arm |
| การบังคับหมดอายุทำที่ไหนบ้าง (ตรวจแล้ว **บังคับทุกจุดอ่านสิทธิ์**) | `RequestAccessResolver.cs:40, 69, 98, 125, 178, 216, 243` (`ExpiresAtUtc == null \|\| > now`), `CurrentUserReader.cs:46`, `UserReadStore.cs:35`, `EstimateStore.cs:1136, 1172`, `FileParentAccessResolver.cs:365`, `IdentityAdministrationSupport.cs:29` (รายชื่อ administrator), `NotificationRecipientResolver.cs:22`, และ `IsActiveAt` ใน `OpportunityStore.cs:239, 766`, `SiteSurveyStore.cs:120` | ไม่ต้องเพิ่มการบังคับ — slice นี้เพียง *แจ้งล่วงหน้า*. ไม่พบจุดที่ปล่อยให้ Membership หมดอายุแล้วยังใช้งานได้ |
| ช่องโหว่ที่ทำให้ต้องมีการแจ้ง | guard `LAST_ADMINISTRATOR_REQUIRED` ตรวจตอนแก้ Membership เท่านั้น (`IdentityAdministrationStore.cs:260-263, 285-286`); หากผู้ดูแลคนสุดท้ายหมดอายุตามเวลา องค์กรสูญเสียผู้ดูแลโดยไม่มี guard | เหตุผลหลักของ slice นี้ (ผู้ดูแลที่หมดอายุก็ถูกแจ้งให้ผู้ดูแลอื่นเห็น) |
| นิยาม "ผู้ดูแล" | `AdministrationPermissions.UsersManage` = ผู้ดูแลตาม guard (`AdministrationModels.cs:8`, `IdentityAdministrationSupport.cs:21-37`); แต่ **การแก้ `ExpiresAtUtc` ต้องใช้ `memberships.manage`** (`AdministrationModels.cs:9`; contract `identity-administration-api-contract.md:39`; permission catalog :12) | ผู้รับ = ผู้ถือ `memberships.manage` (ผู้ที่ลงมือต่ออายุได้จริง) — Decision D2 |
| ความสามารถของ recipient resolver (G-02) | `INotificationRecipientResolver.ResolveAsync(org, branch?, permissionKey, excluded, atUtc)` คืนผู้ใช้ที่ active ถือ permission ที่ **Organization scope** และ Membership ยังอยู่ในช่วงเวลา; Membership ผูกสาขาตรงเฉพาะเอกสารสาขาตน (`NotificationRecipientResolver.cs:26-58`); จำกัด 50 ราย/event | ใช้ resolver เดิมได้เลย ไม่ต้องเขียน query ผู้รับใหม่ |
| Dedupe ของ G-02 | `dedupeKey = "{type}:{transitionId:N}"` (`NotificationPublishPlanner.cs:35`); publisher ข้ามผู้รับที่มีแถว dedupe เดิมแล้ว (`NotificationPublisher.cs:46-60`); unique index `ux_notifications_recipient_dedupe (organization_id, recipient_user_id, dedupe_key)` (`NotificationConfiguration.cs:34-36`); `NotificationDedupeConflict.Is` ตรวจ race (`NotificationDedupeConflict.cs`) | transitionId แบบ deterministic = idempotency โดยไม่ต้องมีตารางใหม่/migration |
| ข้อจำกัดของแกนกลางที่ต้องขยาย | event ต้องมี actor เสมอ: `NotificationEvent.ActorUserId` เป็น `Guid` (`NotificationModels.cs:34`), planner ปฏิเสธ `Guid.Empty` และเพิ่ม `actorDisplayName` (`NotificationPublishPlanner.cs:17, 26-30`), publisher โหลดชื่อ actor จากตาราง users (`NotificationPublisher.cs:31-35`); registry test บังคับให้ทุก type มี `actorDisplayName` (`NotificationRegistryTests.cs` `EveryDescriptor_DeclaresItsContract`) | Task 2 เพิ่ม `SystemGenerated` ใน descriptor + ทำ `ActorUserId` เป็น `Guid?` (การเรียกเดิมทั้งหมดยัง compile) |
| ข้อความและลิงก์ | payload เป็น flat string map ตาม allowlist (`NotificationTypeRegistry.cs`), FE render จาก `notifications.types.<messageKey>` + `NOTIFICATION_TYPES` (`frontend/src/lib/notifications/notification-types.ts`, `notification-view.ts`); ลิงก์ render จากแม่แบบ + GUID ใน payload และแสดงเฉพาะผู้อ่านที่ยังถือ `TargetPermission` (`NotificationHandler.cs:67-80`). ชื่อ field ที่ห้าม: `cost price margin amount total budget discount tax salary email phone address token password secret` (`NotificationPayloadRules.cs:14-18`); ค่าต้องยาว 1–200 ตัว (:12, :40) | field ใหม่ `daysRemaining` ผ่านกฎ; `User.DisplayName` ยาวได้ 255 (`UserConfiguration.cs:16`) → ต้อง clip เป็น 200 ที่ factory |
| หน้า deep link | `/settings/users/[id]` ใช้ `[id]` = **user id** และต้องมี `users.read` (`frontend/src/app/[locale]/(erp)/settings/users/[id]/page.tsx`) | ลิงก์ = `/settings/users/{parentId}` โดย `parentId` = user id |
| Test harness ที่ reuse | `NotificationInfrastructureTests` สร้าง Postgres container + `TestOnlyDataSeeder.SeedAsync` + helper `AddCheckerAsync(permissionKey, ...)` (`backend/tests/TanErp.IntegrationTests/Persistence/NotificationInfrastructureTests.cs:13-65`); Test Admin ถือทุก permission รวม `memberships.manage` (`TestOnlyDataSeeder.cs:150, 263-270`) แต่ Membership ผูก `TestBranchId` | event ที่ผูก `branchId = null` จะถึงผู้ดูแลทุกสาขา — test ใช้สมาชิกที่ไม่ผูกสาขา |
| Migration / OpenAPI / error codes | slice นี้ไม่เพิ่ม entity/column/endpoint/error code | ไม่มี migration, ไม่ regenerate OpenAPI/`tan-erp.v1.ts`; Task 7 ตรวจ `has-pending-model-changes` ว่าไม่มีอะไรค้าง |
| ADR/doc ล่าสุด | ADR ล่าสุด `0021-approval-authority-matrix.md` → ใหม่คือ **0022** | |

### ข้อที่แผนหลัก (gap plan) คลาดเคลื่อน/ต้องปรับ

1. gap plan เขียน "แจ้งเตือนผู้ดูแล" โดยไม่นิยามว่าใคร: ในโค้ด "ผู้ดูแล" (`users.manage`) ไม่ใช่ผู้ที่แก้วันหมดอายุได้ (`memberships.manage`) — แผนนี้ใช้ `memberships.manage` (D2).
2. G-02 เขียนว่า type ใหม่ "เพิ่มได้โดยไม่แก้โค้ดกลาง" (`notification-api-contract.md` หัวข้อขั้นตอนเพิ่ม type) — จริงสำหรับ event ที่มี actor เท่านั้น; type แบบ system-generated ต้องขยายแกนกลางเล็กน้อย (Task 2) และอัปเดตเอกสารขั้นตอน.
3. gap plan ระบุว่ายังไม่มี job ตามเวลา — ถูกต้อง; หลัง slice นี้มี worker หนึ่งตัว (ยังไม่ผ่านการรัน test จริงในรอบนี้).

## Decisions ที่ผู้ใช้อาจ veto

| # | ตัดสินใจ | เหตุผล |
| --- | --- | --- |
| D1 | **Scheduler = `BackgroundService` ใน Api + `PeriodicTimer`**, เรียก service ที่ scoped; ไม่เพิ่ม Quartz/Hangfire; ไม่ลงทะเบียนเมื่อ environment = `Test` หรือ `Enabled=false`. รอบแรกหลังเริ่มแอป +60 วินาที แล้วทุก 60 นาที (clamp 5–1440 นาที). Logic ทั้งหมดอยู่ใน service ที่เรียกตรงจาก test ด้วย fake clock — worker บางจนไม่มี logic ให้ทดสอบ | ขั้นต่ำที่ทำงานได้, ไม่มี dependency/ตารางใหม่, reuse ได้กับ G-18. ข้อเสียที่ยอมรับ: ถ้า deploy หลาย instance ต้องพึ่ง advisory lock (D6); ไม่มีหน้าจอดูสถานะ job |
| D2 | **ผู้รับ = ผู้ถือ `memberships.manage` ที่ Organization scope** (resolver เดิม). event ผูกสาขาของ Membership ที่จะหมดอายุ: Membership ไม่ผูกสาขา → ผู้ดูแลทุกสาขา; ผูกสาขา B → ผู้ถือสิทธิ์ระดับองค์กร + ผู้ถือสิทธิ์ที่ผูกสาขา B. **ไม่ตัดเจ้าของ Membership ออก** (ถ้าเจ้าของเป็นผู้ดูแลก็ได้รับ) | ผู้รับต้องเป็นคนที่ต่ออายุได้จริง; ทางเลือก `users.manage` (นิยามตาม last-administrator) ไม่ครอบคลุมผู้ที่แก้ได้. ลิงก์ไปหน้าผู้ใช้ต้องมี `users.read` — ผู้ถือ `memberships.manage` โดยไม่มี `users.read` จะเห็นข้อความแต่เปิดหน้าไม่ได้ (ยอมรับ; Role จริงควรให้คู่กัน) |
| D3 | **Threshold ค่าเริ่มต้น 30 / 7 / 1 วัน (TEST_ONLY)** ตั้งได้ที่ `MembershipExpiryReminder:ThresholdDays` (ค่า 1–365, ไม่ซ้ำ, เรียงเอง). แต่ละรอบสแกนส่ง **เฉพาะ threshold ที่เล็กที่สุดที่เวลาที่เหลือเข้าเกณฑ์** (เหลือ ≤ 30 วัน → 30; ≤ 7 → 7; ≤ 1 → 1) | กันการส่ง 3 ข้อความพร้อมกันเมื่อ Membership ถูกสร้างให้หมดอายุใน 3 วัน หรือเมื่อ job หยุดไปหลายวัน |
| D4 | **Idempotency = transitionId แบบ deterministic**: `SHA256("membership-expiring\|{membershipId:N}\|{thresholdDays}\|{expiresAtUtc.UtcTicks}")` 16 ไบต์แรกเป็น GUID → dedupe key เดิมของ G-02 จึงส่งได้ครั้งเดียวต่อ (Membership, threshold, วันหมดอายุ, ผู้รับ). เปลี่ยน `ExpiresAtUtc` → ค่า hash เปลี่ยน → **re-arm อัตโนมัติ**; ผู้ถือสิทธิ์รายใหม่ที่เพิ่งได้รับสิทธิ์ภายหลังจะได้รับข้อความของ threshold ปัจจุบันในรอบถัดไป (dedupe ต่อผู้รับ) | ไม่ต้องมีตารางสถานะ/migration; ใช้ unique index ที่พิสูจน์แล้ว |
| D5 | **ไม่ส่ง "หมดอายุแล้ว"**: Membership ที่ `ExpiresAtUtc <= now` ถูกข้าม | threshold 1 วันครอบคลุมช่วงสุดท้ายแล้ว; ส่งย้อนหลังจะท่วมกล่องขาเข้าตอนเปิดใช้ครั้งแรก (มี Membership เก่าที่หมดอายุสะสม). ถ้าต้องการ — เพิ่มได้ภายหลังเป็น type แยกโดยไม่แตะของเดิม |
| D6 | **Multi-instance:** service เปิด transaction แล้วเรียก `pg_try_advisory_xact_lock(2110000021)`; ไม่ได้ล็อก → ข้ามรอบ (ไม่รอ). unique index ของ G-02 เป็นตาข่ายชั้นสอง: ถ้าชนจริง (`NotificationDedupeConflict.Is`) rollback และรอรอบถัดไป | lock แบบ transaction-scoped ปล่อยเองเมื่อ commit/rollback/connection หลุด จึงไม่มีล็อกค้าง |
| D7 | **Notification type `membership.expiring` เป็น system-generated:** `ActorUserId = null`, descriptor `SystemGenerated = true`, payload **ไม่มี** `actorDisplayName`. Planner ปฏิเสธ system type ที่ส่ง actor มา และปฏิเสธ type ปกติที่ไม่มี actor | ไม่มีผู้กระทำจริง; ไม่เดา/สร้าง "ผู้ใช้ระบบ" ปลอม |
| D8 | **Payload (allowlist เท่านั้น):** `resourceId` = membership id, `parentId` = user id, `subjectDisplayName` (clip 200), `daysRemaining` = threshold ที่ถึง (เช่น "7"). **ไม่ใส่วันที่หมดอายุ** — หลีกเลี่ยงปัญหา time zone/locale; ผู้ดูแลเห็นเวลาจริงในหน้าผู้ใช้. ข้อความ FE: th `สิทธิ์เข้าใช้งานของ {subjectDisplayName} จะหมดอายุภายใน {daysRemaining} วัน` / en `Access for {subjectDisplayName} expires within {daysRemaining} day(s)` | แกนกลางส่งเฉพาะ string; ไม่ต้องเพิ่ม formatter วันที่ใน FE |
| D9 | **ลิงก์** = `/settings/users/{parentId}` ผ่านระบบ deep link เดิม (แสดงเฉพาะผู้ที่ยังถือ `memberships.manage`) | reuse ทั้งหมด |
| D10 | **Cap:** สแกนสูงสุด 200 Membership/รอบ เรียงตามที่จะหมดอายุเร็วสุดก่อน (clamp 1–1000); ผู้รับต่อ event ตามเพดานเดิม 50 | จำกัดเวลา transaction/ล็อก; ที่เหลือถูกประมวลผลรอบถัดไป. ข้อจำกัดที่ยอมรับ: ต่อ Membership 1 query ผู้รับ + 1 query dedupe (N+1 ขนาดเล็กต่อรอบ) |
| D11 | **ไม่มี migration/endpoint/OpenAPI/error code ใหม่.** ความผิดพลาดภายในของ publisher ยังเป็น exception ตามสัญญา G-02 (ไม่ใช่ API error); worker จับ exception ทุกรอบ log แล้วรอรอบถัดไป ไม่ปล่อยให้ host ล้ม | ลด blast radius |
| D12 | **Decision (2) และ (3) ไม่ implement** — เขียนเป็น options + ข้อเสนอแนะใน [Open Decisions](2026-10-12-g21-open-decisions.md) | ตามคำสั่ง: เป็นเรื่องธุรกิจ/Security |

## Task Map

| Task | เรื่อง | commit |
| --- | --- | --- |
| 1 | ADR 0022, ปรับสัญญา Notification/Identity, ศัพท์ (docs) | `docs(membership): add expiry reminder ADR and contract notes` |
| 2 | แกนกลางแจ้งเตือน: system-generated type + `membership.expiring` (Domain/Application/publisher + test) | `feat(notifications): add system-generated membership.expiring type` |
| 3 | Application: options, planner, port `IMembershipExpiryReminderService` (+ unit test) | `feat(membership): add expiry reminder planner and service port` |
| 4 | Infrastructure: `MembershipExpiryReminderService` + advisory lock + DI (+ integration test) | `feat(membership): scan expiring memberships with advisory lock` |
| 5 | Api: `MembershipExpiryReminderWorker`, config, ลงทะเบียน (ข้ามใน Test) (+ test) | `feat(membership): run expiry reminders from a hosted worker` |
| 6 | Frontend: type + ข้อความ th/en + test | `feat(notifications): render membership expiring notifications` |
| 7 | Verification, เอกสารสถานะ (G-21 ข้อ 1 เท่านั้น) | `docs(membership): record G-21 verification and update status` |

---

## Task 1: ADR 0022, สัญญา, ศัพท์ (docs)

**Files:**
- Create: `docs/adr/0022-membership-expiry-reminder-and-hosted-worker.md`
- Modify: `docs/adr/README.md`, `docs/03-contracts/notification-api-contract.md`, `docs/03-contracts/identity-administration-api-contract.md`, `CONTEXT.md`

- [ ] **Step 1: สร้าง ADR**

สร้าง `docs/adr/0022-membership-expiry-reminder-and-hosted-worker.md`:

````markdown
---
status: accepted
---

# แจ้งเตือน Membership ใกล้หมดอายุด้วย hosted worker ขั้นต่ำ และ notification แบบ system-generated

G-21 ต้องแจ้งผู้ดูแลก่อน Membership หมดอายุ ซึ่งเป็น event ที่ไม่ได้เกิดจากคำขอของผู้ใช้ จึงต้องมี scheduler — repo ยังไม่มีงานตามเวลาใด ๆ.

**การตัดสินใจ 1 — Scheduler:** `BackgroundService` ใน `TanErp.Api` ใช้ `PeriodicTimer` เรียก `IMembershipExpiryReminderService` (scoped) ทุกรอบ (ค่าเริ่มต้น 60 นาที, เริ่มหลังแอปขึ้น 60 วินาที). ไม่ใช้ Quartz/Hangfire (ไม่มี dependency/ตารางใหม่). ไม่ลงทะเบียนเมื่อ environment = `Test` หรือ `MembershipExpiryReminder:Enabled=false`. Logic ทั้งหมดอยู่ใน service เพื่อให้ทดสอบด้วย fake clock โดยไม่ต้องรอเวลา.

**การตัดสินใจ 2 — ความถูกต้องเมื่อมีหลาย instance:** service ทำงานใน transaction เดียวและเรียก `pg_try_advisory_xact_lock`; instance ที่ไม่ได้ล็อกข้ามรอบ. unique index `ux_notifications_recipient_dedupe` ของ G-02 เป็นตาข่ายชั้นสอง (ชนแล้ว rollback รอรอบถัดไป).

**การตัดสินใจ 3 — Idempotency โดยไม่มีตารางสถานะ:** transition id ของ notification = hash แบบ deterministic ของ `(membershipId, thresholdDays, expiresAtUtc)`. ส่งได้ครั้งเดียวต่อค่าชุดนี้ต่อผู้รับ; แก้ `ExpiresAtUtc` ทำให้ hash เปลี่ยนและแจ้งใหม่ได้ (re-arm). ต่อรอบส่งเฉพาะ threshold ที่เล็กที่สุดที่เข้าเกณฑ์ (30/7/1 วัน เป็นค่า TEST_ONLY ที่ตั้งได้).

**การตัดสินใจ 4 — system-generated notification:** descriptor มีธง `SystemGenerated`; event ประเภทนี้ไม่มี actor และ payload ไม่มี `actorDisplayName`. type ปกติยังต้องมี actor ทุกกรณี (planner บังคับทั้งสองทิศทาง).

**การตัดสินใจ 5 — ผู้รับ:** ผู้ถือ `memberships.manage` ที่ Organization scope (ผู้ที่ต่ออายุได้จริง) ผ่าน resolver เดิม; ไม่ส่ง "หมดอายุแล้ว" ย้อนหลัง.

ข้อเสียที่ยอมรับ: ไม่มีหน้าจอสถานะ job; รอบสแกนละ N+1 query ขนาดเล็ก (จำกัด 200 รายการ/รอบ); ผู้ถือ `memberships.manage` ที่ไม่มี `users.read` เห็นข้อความแต่เปิดลิงก์ไม่ได้.
````

- [ ] **Step 2: ลิงก์ ADR**

ใน `docs/adr/README.md` เพิ่มบรรทัดนี้ต่อท้ายรายการ ADR (หลังบรรทัด 0021 ล่าสุดในรายการ — ใช้ Read หาตำแหน่ง ถ้ามี 0021 ซ้ำสองที่ให้ต่อท้ายที่ท้ายไฟล์สุด):

```markdown
- [0022 — Membership Expiry Reminder และ hosted worker ขั้นต่ำ](0022-membership-expiry-reminder-and-hosted-worker.md)
```

- [ ] **Step 3: ปรับสัญญา Notification**

ใน `docs/03-contracts/notification-api-contract.md` ใช้ Edit แทรก section ใหม่ **ก่อน** บรรทัด `## ขั้นตอนเพิ่ม notification type ใหม่ (สำหรับ G-05/G-18/G-21)` (old_string = บรรทัดหัวข้อนั้นทั้งบรรทัด; new_string = section ใหม่ + บรรทัดหัวข้อเดิม):

````markdown
## Type ที่ระบบสร้างเอง (system-generated) — G-21

| Type | เมื่อไร | ผู้รับ (`TargetPermission`) | Payload (allowlist) | ลิงก์ |
| --- | --- | --- | --- | --- |
| `membership.expiring` | Membership ที่ active จะหมดอายุภายใน threshold (ค่าเริ่มต้น 30/7/1 วัน — TEST_ONLY, ตั้งที่ `MembershipExpiryReminder:ThresholdDays`) | `memberships.manage` (Organization scope; Membership ผูกสาขาแจ้งเฉพาะผู้ถือสิทธิ์องค์กร + สาขาเดียวกัน) | `resourceId` (membership id), `parentId` (user id), `subjectDisplayName`, `daysRemaining` | `/settings/users/{parentId}` |

กฎ: event ประเภทนี้ **ไม่มี actor** (`ActorUserId = null`) และ payload **ไม่มี** `actorDisplayName`; type ที่ไม่ใช่ system-generated ต้องมี actor เสมอ. Dedupe = `(type, transition id)` โดย transition id เป็น hash ของ `(membership id, threshold, expiresAtUtc)` จึงส่งครั้งเดียวต่อผู้รับ และแจ้งใหม่เมื่อ `expiresAtUtc` เปลี่ยน. ไม่ส่ง "หมดอายุแล้ว". สร้างโดย `MembershipExpiryReminderWorker` (ไม่ทำงานใน environment `Test`) — ดู [ADR 0022](../adr/0022-membership-expiry-reminder-and-hosted-worker.md).

````

และในหัวข้อ "ขั้นตอนเพิ่ม notification type ใหม่" ต่อท้ายข้อ 5 (หลังบรรทัดที่ขึ้นต้น `5. เรียก`) ด้วยบรรทัด — ใช้ Edit กับ old_string = ข้อความต้นบรรทัดถัดไปที่ขึ้นต้นว่า `test` ตามด้วยชื่อ test registry (ตรวจด้วย Read ก่อนว่าตรงและพบจุดเดียว) แล้วเติมข้อ 6 ไว้ข้างหน้าบรรทัดนั้น:

```markdown
6. Type ที่เกิดจาก scheduler (ไม่มีผู้กระทำ): ตั้ง `SystemGenerated: true` ใน descriptor, ส่ง `ActorUserId = null`, อย่าใส่ `actorDisplayName` ใน `RequiredFields`, และสร้าง transition id แบบ deterministic เพื่อ idempotency.

```

- [ ] **Step 4: ปรับสัญญา Identity Administration**

ใน `docs/03-contracts/identity-administration-api-contract.md` ใช้ Edit: old_string = `\n\n## Endpoints` (ต้องพบ 1 จุด) → new_string = ข้อ 8 แล้วตามด้วย `\n\n## Endpoints`:

```markdown

8. **Membership ใกล้หมดอายุ (G-21):** ผู้ถือ `memberships.manage` ได้รับการแจ้งเตือนในระบบเมื่อ Membership ที่ active ใกล้ `expiresAtUtc` (30/7/1 วัน TEST_ONLY; ดู [Notification Contract](notification-api-contract.md)). การแจ้งเป็นข้อมูลเสริม ไม่เปลี่ยนการบังคับหมดอายุ (Backend ตรวจ `expiresAtUtc` ทุก request ตามข้อ 6) และ **ไม่ใช่ guard** — ผู้ดูแลคนสุดท้ายยังหมดอายุตามเวลาได้; ต่ออายุด้วย `PATCH /api/v1/admin/memberships/{membershipId}`.
```

- [ ] **Step 5: ศัพท์**

ใน `CONTEXT.md` ใช้ Edit: old_string = `**Notification Type (ประเภทการแจ้งเตือน)**:` → new_string = ข้อความใหม่นำหน้า:

```markdown
**Membership Expiry Reminder (การเตือน Membership ใกล้หมดอายุ)**:
Notification แบบ system-generated ถึงผู้ถือ `memberships.manage` เมื่อ Membership ที่ active จะหมดอายุภายใน threshold (30/7/1 วัน TEST_ONLY) ส่งครั้งเดียวต่อ (Membership, threshold, วันหมดอายุ, ผู้รับ) และไม่ส่งหลังหมดอายุแล้ว
_Avoid_: Expiry alert, Membership warning

**Notification Type (ประเภทการแจ้งเตือน)**:
```

- [ ] **Step 6: ตรวจลิงก์ภายใน**

```bash
for f in docs/adr/0022-membership-expiry-reminder-and-hosted-worker.md docs/03-contracts/notification-api-contract.md docs/03-contracts/identity-administration-api-contract.md docs/adr/README.md; do
  d=$(dirname "$f"); grep -o '](\([^)#]*\)\(#[^)]*\)\?)' "$f" | sed 's/^](//; s/)$//; s/#.*//' | grep -v '^http' | grep -v '^$' | while read -r l; do [ -e "$d/$l" ] || echo "MISSING in $f: $l"; done
done
```
Expected: ไม่มีบรรทัด `MISSING`.

- [ ] **Step 7: Commit**

```bash
git add docs/adr/0022-membership-expiry-reminder-and-hosted-worker.md docs/adr/README.md docs/03-contracts/notification-api-contract.md docs/03-contracts/identity-administration-api-contract.md CONTEXT.md
git commit -m "docs(membership): add expiry reminder ADR and contract notes

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 2: แกนกลางแจ้งเตือน — system-generated type และ `membership.expiring`

**Files:**
- Modify: `backend/src/TanErp.Domain/Notifications/NotificationValues.cs`
- Modify: `backend/src/TanErp.Application/Notifications/NotificationModels.cs`, `NotificationTypeRegistry.cs`, `NotificationEvents.cs`
- Replace: `backend/src/TanErp.Application/Notifications/NotificationPublishPlanner.cs`
- Modify: `backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationPublisher.cs`
- Modify tests: `backend/tests/TanErp.UnitTests/Notifications/NotificationRegistryTests.cs`, `NotificationEventsTests.cs`, `NotificationDomainTests.cs`
- Create test: `backend/tests/TanErp.UnitTests/Notifications/NotificationSystemEventTests.cs`

คอมไพล์ได้ทุกขั้น: ทำตามลำดับ Step แล้ว build ครั้งเดียวตอนท้าย (ไม่ commit ระหว่างกลาง).

- [ ] **Step 1: Domain — type ใหม่**

`NotificationValues.cs` (Edit 2 จุด):

```csharp
// old
    public const string RoleAssignmentApprovalRequested = "role-assignment.approval-requested";
// new
    public const string RoleAssignmentApprovalRequested = "role-assignment.approval-requested";
    /// <summary>System-generated (no actor): a membership is about to expire. See ADR 0022.</summary>
    public const string MembershipExpiring = "membership.expiring";
```

```csharp
// old
        RoleAssignmentApprovalRequested
    };
// new
        RoleAssignmentApprovalRequested,
        MembershipExpiring
    };
```

- [ ] **Step 2: Application — field, event record, descriptor**

`NotificationModels.cs`: เพิ่ม constant ใน `NotificationFields` (หลัง `RoleName`):

```csharp
    public const string RoleName = "roleName";
    public const string DaysRemaining = "daysRemaining";
```

และแก้ record `NotificationEvent` (Edit: old `    Guid ActorUserId,` → new `    Guid? ActorUserId,`) และแก้ summary ของ record โดยเติมประโยคท้ายก่อน `</summary>`:

```csharp
/// <c>ActorUserId</c> is null only for a system-generated type (descriptor <c>SystemGenerated</c>), which has no actor and no <c>actorDisplayName</c>.
```
(Edit: old `/// <c>TransitionId</c> changes every time the document enters the pending-approval state, so resubmission notifies again.` → new = บรรทัดเดิม + ขึ้นบรรทัดใหม่ด้วยประโยคด้านบน.)

`NotificationTypeRegistry.cs`:

```csharp
// old (end of the record declaration)
    IReadOnlySet<string> RequiredFields);
// new
    IReadOnlySet<string> RequiredFields,
    bool SystemGenerated = false);
```

เติม `/// <c>SystemGenerated</c> types come from a scheduler: no actor, and <c>actorDisplayName</c> is not a payload field.` ในเอกสารของ record (บรรทัดใหม่ก่อน `/// </summary>` ของ record นั้น). จากนั้นเพิ่ม descriptor (Edit):

```csharp
// old
            Fields(NotificationFields.ResourceId, NotificationFields.SubjectDisplayName, NotificationFields.RoleName, NotificationFields.ActorDisplayName))
    }.ToDictionary(d => d.Type, StringComparer.Ordinal);
// new
            Fields(NotificationFields.ResourceId, NotificationFields.SubjectDisplayName, NotificationFields.RoleName, NotificationFields.ActorDisplayName)),
        new NotificationTypeDescriptor(
            NotificationTypes.MembershipExpiring, "memberships.manage", "/settings/users/{parentId}", NotificationFields.SubjectDisplayName,
            Fields(NotificationFields.ResourceId, NotificationFields.ParentId, NotificationFields.SubjectDisplayName, NotificationFields.DaysRemaining),
            SystemGenerated: true)
    }.ToDictionary(d => d.Type, StringComparer.Ordinal);
```

- [ ] **Step 3: Factory event**

`NotificationEvents.cs`: เพิ่ม `using System.Globalization;` บรรทัดแรกของไฟล์ แล้วเพิ่มท้ายคลาส (ก่อน `}` ปิด — Edit anchored ที่เมธอด `RoleAssignmentRequested` ตัวสุดท้าย: old `            null, [subjectUserId]);\n}` → new ใส่เมธอดใหม่ก่อน `}`):

```csharp
            null, [subjectUserId]);

    /// <summary>
    /// System-generated (no actor): a membership's remaining validity fell under a reminder threshold. Recipients are the holders of the
    /// type's target permission (memberships.manage). The transition id must be the deterministic hash of (membership, threshold, expiry)
    /// so the same reminder is never stored twice and a changed expiry re-arms it.
    /// </summary>
    public static NotificationEvent MembershipExpiring(
        Guid organizationId, Guid? branchId, Guid transitionId, Guid membershipId, Guid subjectUserId, string subjectDisplayName, int daysRemaining) =>
        new(NotificationTypes.MembershipExpiring, organizationId, branchId, transitionId, null,
            Fields((NotificationFields.ResourceId, Id(membershipId)), (NotificationFields.ParentId, Id(subjectUserId)),
                (NotificationFields.SubjectDisplayName, Clip(subjectDisplayName)),
                (NotificationFields.DaysRemaining, daysRemaining.ToString(CultureInfo.InvariantCulture))),
            null, []);

    /// <summary>A display name can be up to 255 characters; a payload value is limited to 200.</summary>
    private static string Clip(string value)
    {
        var trimmed = value.Trim();
        return trimmed.Length > NotificationPayloadRules.MaxValueLength ? trimmed[..NotificationPayloadRules.MaxValueLength] : trimmed;
    }
}
```
(ปิดคลาสด้วย `}` ครั้งเดียว — ตรวจว่าไม่มี `}` ซ้ำ.)

- [ ] **Step 4: Planner (แทนทั้งไฟล์)**

เขียน `backend/src/TanErp.Application/Notifications/NotificationPublishPlanner.cs` ใหม่ทั้งไฟล์:

```csharp
using System.Text.Json;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Notifications;

/// <summary>
/// Pure part of publishing: validates the event, builds the final payload and picks who gets it. The maker (actor) and any
/// explicitly excluded user never receive it, duplicates collapse, and the list is capped and ordered so retries are stable.
/// A system-generated type has no actor: it must not carry one and gets no actorDisplayName. Every other type requires an actor.
/// Scope filtering (who holds the permission) happens before this, in the recipient resolver.
/// </summary>
public static class NotificationPublishPlanner
{
    public static Result<IReadOnlyList<PlannedNotification>> Plan(NotificationEvent evt, string? actorDisplayName, IReadOnlyCollection<Guid> candidateRecipientUserIds)
    {
        var descriptor = NotificationTypeRegistry.Find(evt.Type);
        if (descriptor is null) return Fail("NOTIFICATION_TYPE_INVALID", $"Notification type '{evt.Type}' is not registered.");

        var system = descriptor.SystemGenerated;
        if (evt.OrganizationId == Guid.Empty || evt.TransitionId == Guid.Empty || (!system && (evt.ActorUserId is null || evt.ActorUserId == Guid.Empty)))
        {
            return Fail("NOTIFICATION_FIELD_INVALID", "An organization, a transition and an actor are required.");
        }

        if (system && evt.ActorUserId is not null)
        {
            return Fail("NOTIFICATION_FIELD_INVALID", "A system-generated notification has no actor.");
        }

        if (evt.Fields.ContainsKey(NotificationFields.ActorDisplayName))
        {
            return Fail("NOTIFICATION_PAYLOAD_INVALID", "The actor display name is added by the publisher and must not be supplied.");
        }

        var fields = new Dictionary<string, string>(evt.Fields, StringComparer.Ordinal);
        if (!system) fields[NotificationFields.ActorDisplayName] = actorDisplayName ?? string.Empty;

        var payload = NotificationPayloadRules.Validate(descriptor, fields);
        if (payload.IsFailure) return Result<IReadOnlyList<PlannedNotification>>.Failure(payload.Error);

        var payloadJson = JsonSerializer.Serialize(payload.Value!);
        var dedupeKey = $"{descriptor.Type}:{evt.TransitionId:N}";
        var excluded = new HashSet<Guid>(evt.ExcludedUserIds);
        if (evt.ActorUserId is { } actorId) excluded.Add(actorId);

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

- [ ] **Step 5: Publisher**

`NotificationPublisher.cs` Edit: old

```csharp
        var actorName = await _db.Users.AsNoTracking()
            .Where(u => u.Id == evt.ActorUserId)
            .Select(u => u.DisplayName)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotificationDomainException("NOTIFICATION_FIELD_INVALID", "The actor user was not found.");
```
new

```csharp
        string? actorName = null;
        if (!descriptor.SystemGenerated)
        {
            var actorId = evt.ActorUserId
                ?? throw new NotificationDomainException("NOTIFICATION_FIELD_INVALID", "The actor user is required.");
            actorName = await _db.Users.AsNoTracking()
                .Where(u => u.Id == actorId)
                .Select(u => u.DisplayName)
                .FirstOrDefaultAsync(ct)
                ?? throw new NotificationDomainException("NOTIFICATION_FIELD_INVALID", "The actor user was not found.");
        }
```

- [ ] **Step 6: อัปเดต test เดิม (ยังไม่รัน)**

`NotificationRegistryTests.cs`:
1. ใน `EveryDescriptor_DeclaresItsContract` แทนบรรทัด `Assert.Contains(NotificationFields.ActorDisplayName, d.RequiredFields);` ด้วย

```csharp
            if (d.SystemGenerated) Assert.DoesNotContain(NotificationFields.ActorDisplayName, d.RequiredFields);
            else Assert.Contains(NotificationFields.ActorDisplayName, d.RequiredFields);
```
2. ใน `TargetPermissions_MatchTheApprovalPermissionsOfEachSource` เติมท้ายเมธอด (ก่อน `}`):

```csharp
        Assert.Equal("memberships.manage", NotificationTypeRegistry.Find(NotificationTypes.MembershipExpiring)!.TargetPermission);
```
3. เพิ่ม test ใหม่ท้ายคลาส:

```csharp
    [Fact]
    public void OnlyMembershipExpiring_IsSystemGenerated()
    {
        var system = NotificationTypeRegistry.Types.Where(t => NotificationTypeRegistry.Find(t)!.SystemGenerated).ToArray();
        Assert.Equal([NotificationTypes.MembershipExpiring], system);
    }

    [Fact]
    public void MembershipExpiring_LinksToTheUserPage_ByUserId()
    {
        var d = NotificationTypeRegistry.Find(NotificationTypes.MembershipExpiring)!;
        var userId = Guid.NewGuid();
        var payload = new Dictionary<string, string> { [NotificationFields.ParentId] = userId.ToString("D") };
        Assert.Equal($"/settings/users/{userId:D}", NotificationTypeRegistry.RenderDeepLink(d, payload));
    }
```

`NotificationEventsTests.cs`:
- ใน `AllEvents()` เพิ่มบรรทัดท้าย collection:

```csharp
        NotificationEvents.MembershipExpiring(Org, Branch, Guid.NewGuid(), Guid.NewGuid(), Subject, "สมหญิง ทดสอบ", 7),
```
- เพิ่ม test:

```csharp
    [Fact]
    public void MembershipExpiring_HasNoActor_NoExplicitRecipients_AndCarriesTheThreshold()
    {
        var membershipId = Guid.NewGuid();
        var evt = NotificationEvents.MembershipExpiring(Org, null, Guid.NewGuid(), membershipId, Subject, "สมหญิง", 30);
        Assert.Null(evt.ActorUserId);
        Assert.Null(evt.ExplicitRecipientUserIds);
        Assert.Empty(evt.ExcludedUserIds);
        Assert.Null(evt.BranchId);
        Assert.Equal("30", evt.Fields[NotificationFields.DaysRemaining]);
        Assert.Equal(membershipId.ToString("D"), evt.Fields[NotificationFields.ResourceId]);
        Assert.Equal(Subject.ToString("D"), evt.Fields[NotificationFields.ParentId]);
    }

    [Fact]
    public void MembershipExpiring_ClipsALongDisplayNameToThePayloadLimit()
    {
        var evt = NotificationEvents.MembershipExpiring(Org, null, Guid.NewGuid(), Guid.NewGuid(), Subject, new string('ก', 255), 7);
        Assert.Equal(NotificationPayloadRules.MaxValueLength, evt.Fields[NotificationFields.SubjectDisplayName].Length);
        Assert.True(NotificationPublishPlanner.Plan(evt, null, [Reviewer]).IsSuccess);
    }
```

`NotificationDomainTests.cs`: ชื่อ test `RegisteredTypes_AreExactlyTheSixApprovalRequests_AndMatchTheFrontendList` → เปลี่ยนเป็น `RegisteredTypes_AreTheApprovalRequestsAndMembershipExpiring_AndMatchTheFrontendList` และแทรก `"membership.expiring",` ระหว่าง `"estimate.approval-requested",` กับ `"mrp-run.approval-requested",` (ลำดับ ordinal).

- [ ] **Step 7: test ใหม่ของ planner แบบ system-generated**

สร้าง `backend/tests/TanErp.UnitTests/Notifications/NotificationSystemEventTests.cs`:

```csharp
using System.Text.Json;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class NotificationSystemEventTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Subject = Guid.NewGuid();

    private static NotificationEvent Expiring(int days = 7) =>
        NotificationEvents.MembershipExpiring(Org, null, Guid.NewGuid(), Guid.NewGuid(), Subject, "สมหญิง", days);

    [Fact]
    public void ASystemEvent_PlansWithoutAnActorName_AndItsPayloadHasNoActorField()
    {
        var admin = Guid.NewGuid();
        var plan = NotificationPublishPlanner.Plan(Expiring(), null, [admin]);
        Assert.True(plan.IsSuccess, plan.Error.Message);
        using var document = JsonDocument.Parse(plan.Value![0].PayloadJson);
        Assert.False(document.RootElement.TryGetProperty(NotificationFields.ActorDisplayName, out _));
        Assert.Equal("7", document.RootElement.GetProperty(NotificationFields.DaysRemaining).GetString());
    }

    [Fact]
    public void ASystemEvent_IgnoresASuppliedActorName_ButRejectsASuppliedActorId()
    {
        var admin = Guid.NewGuid();
        Assert.True(NotificationPublishPlanner.Plan(Expiring(), "ignored", [admin]).IsSuccess);

        var withActor = Expiring() with { ActorUserId = Guid.NewGuid() };
        var result = NotificationPublishPlanner.Plan(withActor, null, [admin]);
        Assert.True(result.IsFailure);
        Assert.Equal("NOTIFICATION_FIELD_INVALID", result.Error.Code);
    }

    [Fact]
    public void AnOrdinaryType_StillRequiresAnActor()
    {
        var fields = new Dictionary<string, string>
        {
            [NotificationFields.ResourceId] = Guid.NewGuid().ToString("D"),
            [NotificationFields.DocumentNumber] = "PO-1"
        };
        var evt = new NotificationEvent(NotificationTypes.PurchaseOrderApprovalRequested, Org, null, Guid.NewGuid(), null, fields, null, []);
        var result = NotificationPublishPlanner.Plan(evt, "ผู้ส่ง", [Guid.NewGuid()]);
        Assert.True(result.IsFailure);
        Assert.Equal("NOTIFICATION_FIELD_INVALID", result.Error.Code);
    }

    [Fact]
    public void TheDedupeKey_IsTypeAndTransition_SoTheSameTransitionNeverStoresTwice()
    {
        var evt = Expiring();
        var a = NotificationPublishPlanner.Plan(evt, null, [Guid.NewGuid()]).Value![0];
        var b = NotificationPublishPlanner.Plan(evt, null, [Guid.NewGuid()]).Value![0];
        Assert.Equal(a.DedupeKey, b.DedupeKey);
        Assert.Equal($"membership.expiring:{evt.TransitionId:N}", a.DedupeKey);
    }
}
```

- [ ] **Step 8: Build (compile-only — ห้ามรัน test)**

Run: `dotnet build backend/TanErp.slnx --nologo -v q`
Expected: `0 Warning(s)` `0 Error(s)`. ถ้ามี error จาก `ActorUserId` เป็น nullable ที่อื่น ให้ grep `ActorUserId` ใน `backend/src` กับ `backend/tests` เฉพาะที่เป็นของ `NotificationEvent` (ไม่ใช่ `RequestAccessContext.ActorUserId`) และแก้ให้ compile โดยไม่เปลี่ยนพฤติกรรม.

- [ ] **Step 9: Commit**

```bash
git add backend/src/TanErp.Domain/Notifications backend/src/TanErp.Application/Notifications backend/src/TanErp.Infrastructure/Persistence/Notifications/NotificationPublisher.cs backend/tests/TanErp.UnitTests/Notifications
git commit -m "feat(notifications): add system-generated membership.expiring type

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 3: Application — options, planner, port

**Files:**
- Create: `backend/src/TanErp.Application/Notifications/MembershipExpiryReminder.cs`
- Create test: `backend/tests/TanErp.UnitTests/Notifications/MembershipExpiryReminderPlannerTests.cs`

- [ ] **Step 1: เขียนโค้ด**

สร้าง `backend/src/TanErp.Application/Notifications/MembershipExpiryReminder.cs`:

```csharp
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace TanErp.Application.Notifications;

/// <summary>
/// Settings under <c>MembershipExpiryReminder</c>. The thresholds are TEST_ONLY defaults pending the System Admin's real policy.
/// Out-of-range values are clamped, never rejected, so a typo cannot stop the application from starting.
/// </summary>
public sealed class MembershipExpiryReminderOptions
{
    public const string SectionName = "MembershipExpiryReminder";

    /// <summary>Ascending. Used when <see cref="ThresholdDays"/> is missing or has no valid value.</summary>
    public static IReadOnlyList<int> DefaultThresholdDays { get; } = [1, 7, 30];

    public bool Enabled { get; set; } = true;
    public int IntervalMinutes { get; set; } = 60;
    public int StartupDelaySeconds { get; set; } = 60;
    public int MaxMembershipsPerRun { get; set; } = 200;

    /// <summary>Null (not an initialized array) so configuration binding replaces rather than appends.</summary>
    public int[]? ThresholdDays { get; set; }

    public TimeSpan EffectiveInterval => TimeSpan.FromMinutes(Math.Clamp(IntervalMinutes, 5, 1440));
    public TimeSpan EffectiveStartupDelay => TimeSpan.FromSeconds(Math.Clamp(StartupDelaySeconds, 0, 3600));
    public int EffectiveMaxMemberships => Math.Clamp(MaxMembershipsPerRun, 1, 1000);
}

/// <summary>Pure rules of the reminder: which threshold applies and which transition id makes a reminder unique.</summary>
public static class MembershipExpiryReminderPlanner
{
    public const int MaxThresholdDays = 365;

    /// <summary>Valid values (1-365) de-duplicated and ascending; the default list when none is valid.</summary>
    public static IReadOnlyList<int> NormalizeThresholds(IEnumerable<int>? configured)
    {
        var values = configured?.Where(d => d is >= 1 and <= MaxThresholdDays).Distinct().OrderBy(d => d).ToArray() ?? [];
        return values.Length == 0 ? MembershipExpiryReminderOptions.DefaultThresholdDays : values;
    }

    /// <summary>
    /// The smallest threshold the remaining time fits in (so 20 days left with 30/7/1 is "30", 6 days is "7", 12 hours is "1"),
    /// or null when the membership is already expired or still beyond the largest threshold. Expired memberships are never reminded.
    /// </summary>
    public static int? SelectThreshold(DateTimeOffset now, DateTimeOffset expiresAtUtc, IReadOnlyList<int> thresholdsAscending)
    {
        var remaining = expiresAtUtc - now;
        if (remaining <= TimeSpan.Zero) return null;
        foreach (var days in thresholdsAscending)
        {
            if (remaining <= TimeSpan.FromDays(days)) return days;
        }

        return null;
    }

    /// <summary>
    /// Deterministic id of one reminder: the same (membership, threshold, expiry) always gives the same value, so the notification
    /// dedupe key stores it once; a changed expiry gives a new value and re-arms the reminder.
    /// </summary>
    public static Guid TransitionId(Guid membershipId, int thresholdDays, DateTimeOffset expiresAtUtc)
    {
        var text = string.Create(CultureInfo.InvariantCulture, $"membership-expiring|{membershipId:N}|{thresholdDays}|{expiresAtUtc.UtcTicks}");
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return new Guid(hash.AsSpan(0, 16));
    }
}

/// <summary>What one scan did. <c>SkippedBecauseLocked</c> means another instance holds the advisory lock.</summary>
public sealed record MembershipExpiryReminderRunResult(int Scanned, int Created, int Failed, bool SkippedBecauseLocked);

/// <summary>Runs one scan. Everything time-based is read from <see cref="Common.Abstractions.IClock"/>; the schedule lives in the host.</summary>
public interface IMembershipExpiryReminderService
{
    Task<MembershipExpiryReminderRunResult> RunOnceAsync(CancellationToken ct = default);
}
```

- [ ] **Step 2: เขียน test**

สร้าง `backend/tests/TanErp.UnitTests/Notifications/MembershipExpiryReminderPlannerTests.cs`:

```csharp
using TanErp.Application.Notifications;
using Xunit;

namespace TanErp.UnitTests.Notifications;

public class MembershipExpiryReminderPlannerTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 12, 8, 0, 0, TimeSpan.Zero);
    private static readonly IReadOnlyList<int> Default = MembershipExpiryReminderPlanner.NormalizeThresholds(null);

    [Fact]
    public void Defaults_AreThirtySevenAndOneDays_Ascending()
    {
        Assert.Equal([1, 7, 30], Default);
    }

    [Theory]
    [InlineData(new[] { 30, 7, 1, 7 }, new[] { 1, 7, 30 })]
    [InlineData(new[] { 0, -3, 366, 14 }, new[] { 14 })]
    [InlineData(new int[0], new[] { 1, 7, 30 })]
    [InlineData(new[] { 0, 999 }, new[] { 1, 7, 30 })]
    public void NormalizeThresholds_KeepsValidDistinctValuesAscending_OrFallsBackToTheDefaults(int[] configured, int[] expected)
    {
        Assert.Equal(expected, MembershipExpiryReminderPlanner.NormalizeThresholds(configured));
    }

    [Theory]
    [InlineData(45 * 24, null)]       // beyond the largest threshold
    [InlineData(30 * 24, 30)]         // exactly on the boundary belongs to that threshold
    [InlineData(20 * 24, 30)]
    [InlineData(7 * 24, 7)]
    [InlineData(6 * 24, 7)]
    [InlineData(24, 1)]
    [InlineData(12, 1)]
    [InlineData(0, null)]             // already expired: never reminded
    [InlineData(-5, null)]
    public void SelectThreshold_PicksTheSmallestThresholdTheRemainingTimeFitsIn(int hoursRemaining, int? expected)
    {
        Assert.Equal(expected, MembershipExpiryReminderPlanner.SelectThreshold(Now, Now.AddHours(hoursRemaining), Default));
    }

    [Fact]
    public void TransitionId_IsDeterministic_AndChangesWithEachInput()
    {
        var membership = Guid.NewGuid();
        var expiry = Now.AddDays(10);
        var id = MembershipExpiryReminderPlanner.TransitionId(membership, 30, expiry);

        Assert.NotEqual(Guid.Empty, id);
        Assert.Equal(id, MembershipExpiryReminderPlanner.TransitionId(membership, 30, expiry));
        Assert.Equal(id, MembershipExpiryReminderPlanner.TransitionId(membership, 30, expiry.ToOffset(TimeSpan.FromHours(7))));
        Assert.NotEqual(id, MembershipExpiryReminderPlanner.TransitionId(membership, 7, expiry));
        Assert.NotEqual(id, MembershipExpiryReminderPlanner.TransitionId(membership, 30, expiry.AddDays(1)));
        Assert.NotEqual(id, MembershipExpiryReminderPlanner.TransitionId(Guid.NewGuid(), 30, expiry));
    }

    [Fact]
    public void Options_ClampOutOfRangeValues()
    {
        var options = new MembershipExpiryReminderOptions { IntervalMinutes = 0, StartupDelaySeconds = -5, MaxMembershipsPerRun = 99999 };
        Assert.Equal(TimeSpan.FromMinutes(5), options.EffectiveInterval);
        Assert.Equal(TimeSpan.Zero, options.EffectiveStartupDelay);
        Assert.Equal(1000, options.EffectiveMaxMemberships);
        Assert.True(new MembershipExpiryReminderOptions().Enabled);
    }
}
```

หมายเหตุ: `[InlineData(new int[0], ...)]` และ `[InlineData(new[] {...}, new[] {...})]` ใช้ได้กับ `int[]` ทั้งสอง parameter.

- [ ] **Step 3: Build (compile-only — ห้ามรัน test)**

Run: `dotnet build backend/TanErp.slnx --nologo -v q` → `0 Warning(s)` `0 Error(s)`.

- [ ] **Step 4: Commit**

```bash
git add backend/src/TanErp.Application/Notifications/MembershipExpiryReminder.cs backend/tests/TanErp.UnitTests/Notifications/MembershipExpiryReminderPlannerTests.cs
git commit -m "feat(membership): add expiry reminder planner and service port

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 4: Infrastructure — service, advisory lock และ DI

**Files:**
- Create: `backend/src/TanErp.Infrastructure/Persistence/Notifications/MembershipExpiryReminderService.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`
- Create test: `backend/tests/TanErp.IntegrationTests/Persistence/MembershipExpiryReminderServiceTests.cs`

pitfall: ใน namespace `TanErp.Infrastructure.Persistence.Notifications` ห้ามอ้างชนิดชื่อ `Organization` เปล่า ๆ (ชนกับ namespace) — โค้ดนี้ใช้เฉพาะ `Membership` และ navigation `m.Organization!` (member access ปลอดภัย).

- [ ] **Step 1: เขียน service**

สร้าง `backend/src/TanErp.Infrastructure/Persistence/Notifications/MembershipExpiryReminderService.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Notifications;
using TanErp.Domain.Notifications;

namespace TanErp.Infrastructure.Persistence.Notifications;

/// <summary>
/// One scan for memberships that will expire within the largest reminder threshold. Everything happens in one transaction that first
/// takes a PostgreSQL transaction-scoped advisory lock: an instance that cannot get it skips the run, and the lock is released by commit,
/// rollback or a dropped connection. Reminders go through the shared INotificationPublisher, whose dedupe key (type + deterministic
/// transition id) and unique index make a repeated scan, a restart or a second instance harmless. A single bad row is logged and skipped.
/// </summary>
public class MembershipExpiryReminderService : IMembershipExpiryReminderService
{
    /// <summary>Arbitrary but fixed: every instance must use the same number for the lock to mean anything.</summary>
    public const long AdvisoryLockKey = 2_110_000_021L;

    private readonly AppDbContext _db;
    private readonly INotificationPublisher _publisher;
    private readonly IClock _clock;
    private readonly MembershipExpiryReminderOptions _options;
    private readonly ILogger<MembershipExpiryReminderService> _logger;

    public MembershipExpiryReminderService(
        AppDbContext db, INotificationPublisher publisher, IClock clock, MembershipExpiryReminderOptions options,
        ILogger<MembershipExpiryReminderService> logger)
    {
        _db = db;
        _publisher = publisher;
        _clock = clock;
        _options = options;
        _logger = logger;
    }

    private sealed record Candidate(Guid Id, Guid OrganizationId, Guid? BranchId, Guid UserId, string DisplayName, DateTimeOffset ExpiresAtUtc);

    public async Task<MembershipExpiryReminderRunResult> RunOnceAsync(CancellationToken ct = default)
    {
        var thresholds = MembershipExpiryReminderPlanner.NormalizeThresholds(_options.ThresholdDays);
        var now = _clock.UtcNow;
        var horizon = now.AddDays(thresholds[^1]);

        await using var transaction = await _db.Database.BeginTransactionAsync(ct);
        var locked = await _db.Database
            .SqlQuery<bool>($"SELECT pg_try_advisory_xact_lock({AdvisoryLockKey}) AS \"Value\"")
            .SingleAsync(ct);
        if (!locked) return new MembershipExpiryReminderRunResult(0, 0, 0, true);

        // Same "usable membership" rule as NotificationMembershipFilter / RequestAccessResolver, plus: it expires inside the horizon.
        var candidates = await _db.Memberships.AsNoTracking()
            .Where(m => m.IsActive && m.User!.IsActive && m.Organization!.IsActive)
            .Where(m => m.BranchId == null || m.Branch!.IsActive)
            .Where(m => m.StartsAtUtc == null || m.StartsAtUtc <= now)
            .Where(m => m.ExpiresAtUtc != null && m.ExpiresAtUtc > now && m.ExpiresAtUtc <= horizon)
            .OrderBy(m => m.ExpiresAtUtc).ThenBy(m => m.Id)
            .Take(_options.EffectiveMaxMemberships)
            .Select(m => new Candidate(m.Id, m.OrganizationId, m.BranchId, m.UserId, m.User!.DisplayName, m.ExpiresAtUtc!.Value))
            .ToListAsync(ct);

        var failed = 0;
        foreach (var candidate in candidates)
        {
            var threshold = MembershipExpiryReminderPlanner.SelectThreshold(now, candidate.ExpiresAtUtc, thresholds);
            if (threshold is null) continue;

            var evt = NotificationEvents.MembershipExpiring(
                candidate.OrganizationId, candidate.BranchId,
                MembershipExpiryReminderPlanner.TransitionId(candidate.Id, threshold.Value, candidate.ExpiresAtUtc),
                candidate.Id, candidate.UserId, candidate.DisplayName, threshold.Value);
            try
            {
                await _publisher.PublishAsync(evt, ct);
            }
            catch (NotificationDomainException ex)
            {
                // A programming/data error for one membership must not block everyone else's reminder. No names or emails are logged.
                failed++;
                _logger.LogWarning(ex, "Membership expiry reminder skipped for membership {MembershipId}: {Code}", candidate.Id, ex.Code);
            }
        }

        var created = _db.ChangeTracker.Entries<Notification>().Count(e => e.State == EntityState.Added);
        try
        {
            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (NotificationDedupeConflict.Is(ex))
        {
            // Another writer stored the same (recipient, reminder) first. Nothing is lost: the next scan sees it as already sent.
            await transaction.RollbackAsync(ct);
            _db.ChangeTracker.Clear();
            return new MembershipExpiryReminderRunResult(candidates.Count, 0, failed, false);
        }

        return new MembershipExpiryReminderRunResult(candidates.Count, created, failed, false);
    }
}
```

- [ ] **Step 2: DI**

`backend/src/TanErp.Api/Program.cs` Edit: old (บรรทัดเดียว, ต้องพบ 1 จุด)

```csharp
builder.Services.AddScoped<TanErp.Application.Notifications.NotificationHandler>();
```
new

```csharp
builder.Services.AddScoped<TanErp.Application.Notifications.NotificationHandler>();
var membershipExpiryOptions = builder.Configuration
    .GetSection(TanErp.Application.Notifications.MembershipExpiryReminderOptions.SectionName)
    .Get<TanErp.Application.Notifications.MembershipExpiryReminderOptions>()
    ?? new TanErp.Application.Notifications.MembershipExpiryReminderOptions();
builder.Services.AddSingleton(membershipExpiryOptions);
builder.Services.AddScoped<TanErp.Application.Notifications.IMembershipExpiryReminderService, TanErp.Infrastructure.Persistence.Notifications.MembershipExpiryReminderService>();
```
(ลงทะเบียน service ทุก environment เพื่อให้ test resolve ได้; **worker** จะลงทะเบียนใน Task 5.)

- [ ] **Step 3: เขียน integration test (ผู้ใช้เป็นคนรัน)**

สร้าง `backend/tests/TanErp.IntegrationTests/Persistence/MembershipExpiryReminderServiceTests.cs`:

```csharp
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
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

public class MembershipExpiryReminderServiceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();
    private AppDbContext _db = null!;
    private MembershipExpiryReminderService _service = null!;

    private static readonly Guid Org = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid Admin = TestOnlyDataSeeder.TestUserId;   // Test Admin: holds memberships.manage at organization scope
    private static readonly DateTimeOffset T0 = new(2026, 10, 12, 8, 0, 0, TimeSpan.Zero);

    private sealed class FakeClock : IClock
    {
        public DateTimeOffset UtcNow { get; set; } = T0;
    }

    private readonly FakeClock _clock = new();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(_postgres.GetConnectionString()).Options);
        await _db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(_db, "Test", true);
        var publisher = new NotificationPublisher(_db, new NotificationRecipientResolver(_db), _clock);
        _service = new MembershipExpiryReminderService(
            _db, publisher, _clock, new MembershipExpiryReminderOptions(), NullLogger<MembershipExpiryReminderService>.Instance);
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private async Task<(Guid UserId, Guid MembershipId)> AddMemberAsync(
        DateTimeOffset? expiresAt, bool membershipActive = true, bool userActive = true, DateTimeOffset? startsAt = null, string? name = null)
    {
        var userId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        _db.Users.Add(new User(userId, $"uid-{userId:N}", name ?? "Expiring " + userId.ToString("N")[..6], $"{userId:N}@example.test", userActive));
        _db.Memberships.Add(new Membership(membershipId, Org, null, userId, membershipActive, startsAt, expiresAt));
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return (userId, membershipId);
    }

    /// <summary>A second recipient whose only grant is <paramref name="permissionKey"/> at organization scope.</summary>
    private async Task<Guid> AddHolderAsync(string permissionKey)
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        var permission = await _db.Permissions.SingleAsync(p => p.Key == permissionKey);
        _db.Users.Add(new User(userId, $"uid-{userId:N}", "Holder " + userId.ToString("N")[..6], $"{userId:N}@example.test"));
        _db.Roles.Add(new Role(roleId, Org, "Holder " + userId.ToString("N")[..6]));
        _db.Add(new RolePermission(Guid.NewGuid(), roleId, Org, permission.Id, PermissionScope.Organization, Org));
        _db.Memberships.Add(new Membership(membershipId, Org, null, userId));
        _db.Add(new MembershipRole(membershipId, roleId, Org));
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return userId;
    }

    private Task<List<Notification>> ReminderRowsAsync(Guid recipient) =>
        _db.Notifications.AsNoTracking()
            .Where(n => n.RecipientUserId == recipient && n.Type == NotificationTypes.MembershipExpiring)
            .OrderBy(n => n.CreatedAtUtc).ThenBy(n => n.Id)
            .ToListAsync();

    private static string Field(Notification row, string name)
    {
        using var document = JsonDocument.Parse(row.PayloadJson);
        return document.RootElement.GetProperty(name).GetString()!;
    }

    [Fact]
    public async Task FirstScan_NotifiesTheAdministratorOnce_WithTheSmallestMatchingThreshold()
    {
        var subject = await AddMemberAsync(T0.AddDays(20), name: "สมหญิง ใจดี");

        var first = await _service.RunOnceAsync();
        var second = await _service.RunOnceAsync();

        var rows = await ReminderRowsAsync(Admin);
        var row = Assert.Single(rows);
        Assert.Equal("30", Field(row, NotificationFields.DaysRemaining));
        Assert.Equal(subject.UserId.ToString("D"), Field(row, NotificationFields.ParentId));
        Assert.Equal(subject.MembershipId.ToString("D"), Field(row, NotificationFields.ResourceId));
        Assert.Equal("สมหญิง ใจดี", Field(row, NotificationFields.SubjectDisplayName));
        Assert.Equal(1, first.Created);
        Assert.Equal(0, second.Created);
        Assert.False(first.SkippedBecauseLocked);
        Assert.Empty(await ReminderRowsAsync(subject.UserId));   // the subject holds no memberships.manage
    }

    [Fact]
    public async Task AsTimePasses_EachThresholdFiresExactlyOnce()
    {
        await AddMemberAsync(T0.AddDays(20));

        await _service.RunOnceAsync();
        _clock.UtcNow = T0.AddDays(14);                       // 6 days left -> 7
        await _service.RunOnceAsync();
        await _service.RunOnceAsync();
        _clock.UtcNow = T0.AddDays(19).AddHours(12);          // 12 hours left -> 1
        await _service.RunOnceAsync();
        await _service.RunOnceAsync();

        var rows = await ReminderRowsAsync(Admin);
        Assert.Equal(["30", "7", "1"], rows.Select(r => Field(r, NotificationFields.DaysRemaining)).ToArray());
    }

    [Fact]
    public async Task ChangingTheExpiry_ReArmsTheReminder()
    {
        var subject = await AddMemberAsync(T0.AddDays(20));
        await _service.RunOnceAsync();

        var membership = await _db.Memberships.SingleAsync(m => m.Id == subject.MembershipId);
        membership.UpdateAssignment(null, null, T0.AddDays(25));
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        await _service.RunOnceAsync();

        Assert.Equal(2, (await ReminderRowsAsync(Admin)).Count);
    }

    [Fact]
    public async Task OnlyUsableMembershipsInsideTheHorizonAreReminded()
    {
        await AddMemberAsync(T0.AddDays(45));                                   // beyond the largest threshold
        await AddMemberAsync(T0.AddHours(-1));                                  // already expired: no "expired" notice
        await AddMemberAsync(null);                                             // never expires
        await AddMemberAsync(T0.AddDays(5), membershipActive: false);
        await AddMemberAsync(T0.AddDays(5), userActive: false);
        await AddMemberAsync(T0.AddDays(5), startsAt: T0.AddDays(1));           // not started yet

        var result = await _service.RunOnceAsync();

        Assert.Equal(0, result.Created);
        Assert.Empty(await ReminderRowsAsync(Admin));
    }

    [Fact]
    public async Task OnlyHoldersOfMembershipsManage_AreNotified()
    {
        var holder = await AddHolderAsync("memberships.manage");
        var other = await AddHolderAsync("estimates.read");
        await AddMemberAsync(T0.AddDays(3));

        await _service.RunOnceAsync();

        Assert.Single(await ReminderRowsAsync(holder));
        Assert.Empty(await ReminderRowsAsync(other));
    }

    [Fact]
    public async Task ALongDisplayName_DoesNotBreakTheScan()
    {
        await AddMemberAsync(T0.AddDays(3), name: new string('ก', 255));

        var result = await _service.RunOnceAsync();

        Assert.Equal(0, result.Failed);
        Assert.Single(await ReminderRowsAsync(Admin));
    }

    [Fact]
    public async Task WhenAnotherInstanceHoldsTheLock_TheRunIsSkipped()
    {
        await AddMemberAsync(T0.AddDays(3));

        await using var other = new NpgsqlConnection(_postgres.GetConnectionString());
        await other.OpenAsync();
        await using var transaction = await other.BeginTransactionAsync();
        await using (var command = new NpgsqlCommand($"SELECT pg_advisory_xact_lock({MembershipExpiryReminderService.AdvisoryLockKey})", other, transaction))
        {
            await command.ExecuteScalarAsync();
        }

        var result = await _service.RunOnceAsync();

        Assert.True(result.SkippedBecauseLocked);
        Assert.Empty(await ReminderRowsAsync(Admin));
    }
}
```

- [ ] **Step 4: Build (compile-only — ห้ามรัน test)**

Run: `dotnet build backend/TanErp.slnx --nologo -v q` → `0 Warning(s)` `0 Error(s)`.
ถ้า `User`/`Role`/`MembershipRole` constructor signature ต่างจากที่ใช้ใน `NotificationInfrastructureTests.AddCheckerAsync` ให้ลอก signature จากไฟล์นั้น (เป็นแหล่งอ้างอิงที่ compile อยู่แล้ว).

- [ ] **Step 5: Commit**

```bash
git add backend/src/TanErp.Infrastructure/Persistence/Notifications/MembershipExpiryReminderService.cs backend/src/TanErp.Api/Program.cs backend/tests/TanErp.IntegrationTests/Persistence/MembershipExpiryReminderServiceTests.cs
git commit -m "feat(membership): scan expiring memberships with advisory lock

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 5: Api — hosted worker, config และการลงทะเบียน

**Files:**
- Create: `backend/src/TanErp.Api/Background/MembershipExpiryReminderWorker.cs`
- Modify: `backend/src/TanErp.Api/Program.cs`, `backend/src/TanErp.Api/appsettings.json`
- Create test: `backend/tests/TanErp.IntegrationTests/Api/MembershipExpiryReminderWorkerTests.cs`

- [ ] **Step 1: เขียน worker**

สร้าง `backend/src/TanErp.Api/Background/MembershipExpiryReminderWorker.cs`:

```csharp
using TanErp.Application.Notifications;

namespace TanErp.Api.Background;

/// <summary>
/// The schedule only: waits a startup delay, then runs one scan per interval. All rules live in IMembershipExpiryReminderService
/// (which tests call directly with a fake clock). A failed run is logged and retried on the next tick; it never stops the host.
/// Not registered in the Test environment or when MembershipExpiryReminder:Enabled is false.
/// </summary>
public sealed class MembershipExpiryReminderWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly MembershipExpiryReminderOptions _options;
    private readonly ILogger<MembershipExpiryReminderWorker> _logger;

    public MembershipExpiryReminderWorker(
        IServiceScopeFactory scopes, MembershipExpiryReminderOptions options, ILogger<MembershipExpiryReminderWorker> logger)
    {
        _scopes = scopes;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(_options.EffectiveStartupDelay, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        using var timer = new PeriodicTimer(_options.EffectiveInterval);
        do
        {
            await RunTickAsync(stoppingToken);
        }
        while (await WaitForNextTickAsync(timer, stoppingToken));
    }

    private static async Task<bool> WaitForNextTickAsync(PeriodicTimer timer, CancellationToken ct)
    {
        try
        {
            return await timer.WaitForNextTickAsync(ct);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    /// <summary>One scan in its own DI scope. Public so a test can drive a tick without waiting for the timer.</summary>
    public async Task RunTickAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopes.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMembershipExpiryReminderService>();
            var result = await service.RunOnceAsync(ct);
            _logger.LogInformation(
                "Membership expiry reminder run finished: scanned {Scanned}, created {Created}, failed {Failed}, skipped (locked) {Skipped}",
                result.Scanned, result.Created, result.Failed, result.SkippedBecauseLocked);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            // Shutting down.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Membership expiry reminder run failed; it will be retried on the next tick.");
        }
    }
}
```

- [ ] **Step 2: ลงทะเบียน (ข้ามใน Test)**

`Program.cs` Edit: old บรรทัดสุดท้ายที่เพิ่มใน Task 4

```csharp
builder.Services.AddScoped<TanErp.Application.Notifications.IMembershipExpiryReminderService, TanErp.Infrastructure.Persistence.Notifications.MembershipExpiryReminderService>();
```
new

```csharp
builder.Services.AddScoped<TanErp.Application.Notifications.IMembershipExpiryReminderService, TanErp.Infrastructure.Persistence.Notifications.MembershipExpiryReminderService>();
// The only scheduler in the app (ADR 0022). Never runs in the Test environment, so tests stay deterministic and need no clock waiting.
if (!builder.Environment.IsEnvironment("Test") && membershipExpiryOptions.Enabled)
{
    builder.Services.AddHostedService<TanErp.Api.Background.MembershipExpiryReminderWorker>();
}
```

- [ ] **Step 3: config**

`backend/src/TanErp.Api/appsettings.json` Edit: old `  "AllowedHosts": "*"` → new

```json
  "AllowedHosts": "*",
  "MembershipExpiryReminder": {
    "Enabled": true,
    "IntervalMinutes": 60,
    "StartupDelaySeconds": 60,
    "MaxMembershipsPerRun": 200,
    "ThresholdDays": [ 30, 7, 1 ]
  }
```
(ตรวจว่า JSON ยังถูกต้อง: `AllowedHosts` ต้องไม่ใช่สมาชิกสุดท้ายอีกต่อไป จึงมีคอมมา.) ค่า `ThresholdDays` เป็น TEST_ONLY — อธิบายใน Task 7.

- [ ] **Step 4: เขียน test**

สร้าง `backend/tests/TanErp.IntegrationTests/Api/MembershipExpiryReminderWorkerTests.cs`:

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using TanErp.Api;
using TanErp.Api.Background;
using TanErp.Application.Notifications;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class MembershipExpiryReminderWorkerTests
{
    private sealed class FakeService : IMembershipExpiryReminderService
    {
        public int Calls { get; private set; }
        public bool Throw { get; set; }

        public Task<MembershipExpiryReminderRunResult> RunOnceAsync(CancellationToken ct = default)
        {
            Calls++;
            if (Throw) throw new InvalidOperationException("boom");
            return Task.FromResult(new MembershipExpiryReminderRunResult(1, 1, 0, false));
        }
    }

    private static (MembershipExpiryReminderWorker Worker, FakeService Service) Create()
    {
        var service = new FakeService();
        var provider = new ServiceCollection().AddScoped<IMembershipExpiryReminderService>(_ => service).BuildServiceProvider();
        var worker = new MembershipExpiryReminderWorker(
            provider.GetRequiredService<IServiceScopeFactory>(), new MembershipExpiryReminderOptions(), NullLogger<MembershipExpiryReminderWorker>.Instance);
        return (worker, service);
    }

    [Fact]
    public async Task ATick_RunsTheServiceOnce()
    {
        var (worker, service) = Create();
        await worker.RunTickAsync(CancellationToken.None);
        Assert.Equal(1, service.Calls);
    }

    [Fact]
    public async Task AFailingRun_IsSwallowed_SoTheScheduleKeepsGoing()
    {
        var (worker, service) = Create();
        service.Throw = true;
        await worker.RunTickAsync(CancellationToken.None);
        service.Throw = false;
        await worker.RunTickAsync(CancellationToken.None);
        Assert.Equal(2, service.Calls);
    }

    [Fact]
    public async Task ACancelledTick_DoesNotThrow()
    {
        var (worker, _) = Create();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await worker.RunTickAsync(cts.Token);
    }

    [Fact]
    public void TheTestEnvironment_NeverRegistersTheWorker_ButStillResolvesTheService()
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = "Host=localhost;Database=unused;Username=unused;Password=unused"
            }));
        });

        Assert.DoesNotContain(factory.Services.GetServices<IHostedService>(), s => s is MembershipExpiryReminderWorker);
        using var scope = factory.Services.CreateScope();
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IMembershipExpiryReminderService>());
    }
}
```
หมายเหตุ: การ resolve `IMembershipExpiryReminderService` สร้าง `AppDbContext` (ไม่เชื่อมต่อจนกว่าจะ query) จึงไม่ต้องมี DB.

- [ ] **Step 5: Build (compile-only — ห้ามรัน test)**

Run: `dotnet build backend/TanErp.slnx --nologo -v q` → `0 Warning(s)` `0 Error(s)`.

- [ ] **Step 6: Commit**

```bash
git add backend/src/TanErp.Api/Background backend/src/TanErp.Api/Program.cs backend/src/TanErp.Api/appsettings.json backend/tests/TanErp.IntegrationTests/Api/MembershipExpiryReminderWorkerTests.cs
git commit -m "feat(membership): run expiry reminders from a hosted worker

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 6: Frontend — type และข้อความ th/en

**Files:**
- Modify: `frontend/src/lib/notifications/notification-types.ts`, `frontend/src/lib/notifications/notification-types.test.ts`, `frontend/src/lib/notifications/notification-view.test.tsx`
- Modify: `frontend/src/messages/th.json`, `frontend/src/messages/en.json`

ไม่มี UI ใหม่ (ใช้ bell + หน้า `/notifications` เดิม), ไม่มี endpoint/type ที่ generate ใหม่ → ไม่รัน `generate:api`.

- [ ] **Step 1: registry ฝั่ง FE**

`notification-types.ts`:
1. `export type NotificationPayloadField = "documentNumber" | "actorDisplayName" | "subjectDisplayName" | "roleName";` → เพิ่ม `| "daysRemaining"` ต่อท้าย.
2. ใน `NotificationMessageKey` เพิ่ม `| "membershipExpiring"` ต่อท้าย union (หลัง `"roleAssignmentApprovalRequested"` — ย้าย `;` ไปท้ายสุด).
3. ใน `NOTIFICATION_TYPES` เพิ่มรายการท้าย (ก่อน `} as const satisfies ...`):

```ts
  "membership.expiring": {
    messageKey: "membershipExpiring",
    fields: ["subjectDisplayName", "daysRemaining"],
  },
```

- [ ] **Step 2: ข้อความ th/en**

`frontend/src/messages/th.json` Edit: old

```json
      "roleAssignmentApprovalRequested": "{actorDisplayName} ขอมอบบทบาท {roleName} ให้ {subjectDisplayName}"
```
new

```json
      "roleAssignmentApprovalRequested": "{actorDisplayName} ขอมอบบทบาท {roleName} ให้ {subjectDisplayName}",
      "membershipExpiring": "สิทธิ์เข้าใช้งานของ {subjectDisplayName} จะหมดอายุภายใน {daysRemaining} วัน"
```

`frontend/src/messages/en.json` Edit: old

```json
      "roleAssignmentApprovalRequested": "{actorDisplayName} requested role {roleName} for {subjectDisplayName}"
```
new

```json
      "roleAssignmentApprovalRequested": "{actorDisplayName} requested role {roleName} for {subjectDisplayName}",
      "membershipExpiring": "Access for {subjectDisplayName} expires within {daysRemaining} day(s)"
```

- [ ] **Step 3: test**

`notification-types.test.ts`: ในรายการที่คาดหวังของ `"mirrors the backend NotificationTypes whitelist"` แทรก `"membership.expiring",` ระหว่าง `"estimate.approval-requested",` กับ `"mrp-run.approval-requested",`. เพิ่มเคสท้าย `describe`:

```ts
  it("declares exactly the payload fields the membership expiring message uses", () => {
    expect(NOTIFICATION_TYPES["membership.expiring"].fields).toEqual(["subjectDisplayName", "daysRemaining"]);
    expect(
      notificationMessageValues("membership.expiring", {
        subjectDisplayName: "สมหญิง",
        daysRemaining: "7",
        resourceId: "ignored",
        parentId: "ignored",
      }),
    ).toEqual({ subjectDisplayName: "สมหญิง", daysRemaining: "7" });
  });
```

`notification-view.test.tsx`: เพิ่มเคสใน `describe`:

```tsx
  it("renders the membership expiring message from the declared fields only", () => {
    expect(
      textFor({
        ...base,
        type: "membership.expiring",
        payload: { subjectDisplayName: "สมหญิง", daysRemaining: "7", resourceId: "m-1", parentId: "u-1" },
        deepLink: "/settings/users/u-1",
      }),
    ).toBe("สิทธิ์เข้าใช้งานของ สมหญิง จะหมดอายุภายใน 7 วัน");
  });
```

- [ ] **Step 4: Compile/lint (ห้ามรัน vitest)**

```bash
cd frontend && npx tsc --noEmit && npm run lint
node -e 'const t=require("./src/messages/th.json"),e=require("./src/messages/en.json");const f=(o,p="")=>Object.entries(o).flatMap(([k,v])=>typeof v==="object"&&v!==null?f(v,p+k+"."):[p+k]);const a=f(t).sort().join("\n"),b=f(e).sort().join("\n");console.log(a===b?"KEY_PARITY_OK":"KEY_PARITY_MISMATCH")'
npm run build
```
Expected: tsc/lint exit 0; `KEY_PARITY_OK`; build exit 0. (`npm run build` รันครั้งเดียวที่ Task นี้.)

- [ ] **Step 5: Commit**

```bash
git add frontend/src/lib/notifications frontend/src/messages/th.json frontend/src/messages/en.json
git commit -m "feat(notifications): render membership expiring notifications

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 7: Verification, เอกสารสถานะ (G-21 ข้อ 1 เท่านั้น)

**Files:**
- Create: `docs/05-engineering/membership-expiry-verification.md`
- Modify: `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/00-overview/implementation-roadmap.md`, `docs/05-engineering/notification-foundation-verification.md`

**กฎ:** ไม่รันชุดทดสอบใด ๆ — เอกสารต้องบอกตรง ๆ ว่า **tests written, not run**. **ห้ามติ๊กข้อ "ตัดสินใจ User หลาย Organization" และ "PII visibility/retention ใน Audit"** — สองข้อนี้ยังเป็น Decision รอผู้ใช้/Security.

- [ ] **Step 1: compile/lint gate สุดท้าย (เท่านั้นที่รันได้)**

```bash
dotnet build backend/TanErp.slnx --nologo -v q
cd frontend && npx tsc --noEmit && npm run lint
cd .. && dotnet ef migrations has-pending-model-changes --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
```
Expected: `0 Warning(s)` `0 Error(s)`; tsc/lint exit 0; `has-pending-model-changes` ต้องรายงานว่า **ไม่มี** การเปลี่ยนแปลงโมเดลค้าง (slice นี้ไม่แตะ entity). ถ้ามี แปลว่ามีใครแก้ model — หยุดและรายงาน ห้ามสร้าง migration ใน Task นี้. จดผลจริงลงเอกสาร (`npm run build` และ key parity ผ่านแล้วใน Task 6 — บันทึกเป็น "ผลจาก Task 6").

- [ ] **Step 2: เอกสาร verification**

สร้าง `docs/05-engineering/membership-expiry-verification.md` (ภาษาไทย รูปแบบเดียวกับ `approval-authority-verification.md`) ประกอบด้วย:

1. **สถานะ:** `Implemented <วันที่จริงจาก date +%F> (G-21 ข้อ 1: Membership ใกล้หมดอายุ → แจ้งผู้ดูแล) — tests written, not run.` ชุดทดสอบ unit/integration/frontend ถูกเขียนแล้วแต่ **ยังไม่ได้รัน** (ผู้ใช้รันเอง); ยังไม่ตรวจในเบราว์เซอร์; ยังไม่รัน full gate; ยังไม่เคยรัน worker กับ PostgreSQL จริง; ยังไม่ผ่าน UAT/Production. ลิงก์ ADR 0022, แผน, Open Decisions.
2. **ผลที่รันจริง (compile/lint เท่านั้น):** ตารางคำสั่ง→ผลจาก Step 1 และผลจาก Task 6 (`tsc`, `lint`, `KEY_PARITY_OK`, `npm run build`).
3. **คำสั่งที่ผู้ใช้ควรรันเอง** (คัดลอกตรง ๆ; ห้ามเขียนว่าผ่าน):

```bash
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~MembershipExpiryReminderPlannerTests|FullyQualifiedName~NotificationSystemEventTests|FullyQualifiedName~NotificationRegistryTests|FullyQualifiedName~NotificationEventsTests|FullyQualifiedName~NotificationDomainTests|FullyQualifiedName~NotificationPublishPlannerTests"
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~MembershipExpiryReminderServiceTests|FullyQualifiedName~MembershipExpiryReminderWorkerTests" -m:1
# regression: แกนกลางแจ้งเตือนเดิมต้องไม่เปลี่ยนพฤติกรรม (ActorUserId กลายเป็น nullable)
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~NotificationInfrastructureTests|FullyQualifiedName~NotificationEndpointsTests|FullyQualifiedName~NotificationSchemaTests|FullyQualifiedName~NotificationDedupeConflictTests" -m:1
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
cd frontend && npx vitest run src/lib/notifications src/components/layout src/features/notifications src/hooks/useNotifications.test.tsx
```

4. **สิ่งที่ชุดทดสอบที่เขียนไว้ตั้งใจพิสูจน์:** planner (เลือก threshold เล็กสุด, ขอบเขตเท่ากันเป็นของ threshold นั้น, หมดอายุแล้วไม่ถูกเตือน, transition id deterministic/ต่างเมื่อ input ต่าง), แกนกลาง (system type ไม่มี actor/`actorDisplayName`, type ปกติยังบังคับ actor, registry/FE parity, clip ชื่อ 255→200), service บน PostgreSQL จริง (ส่งครั้งเดียวต่อ threshold แม้สแกนซ้ำ, ครบ 30→7→1, re-arm เมื่อเปลี่ยน `ExpiresAtUtc`, ข้าม inactive/expired/ไม่เริ่ม/ไกลเกิน/ไม่มีวันหมดอายุ, เฉพาะผู้ถือ `memberships.manage`, advisory lock ทำให้ข้ามรอบ), worker (tick เรียก service, exception ไม่ทำให้ schedule หยุด, ไม่ถูกลงทะเบียนใน Test), FE (render ข้อความ th/พารามิเตอร์ตาม field ที่ประกาศ).
5. **การตรวจเชิงปฏิบัติการที่ยังไม่ได้ทำ (ต้องทำก่อนเปิดใช้จริง):** เริ่ม API ใน environment ที่ไม่ใช่ Test ดู log บรรทัด `Membership expiry reminder run finished` หลัง ~60 วินาที; ตั้ง Membership ทดสอบให้หมดอายุใน 3 วันแล้วดู bell ของผู้ถือ `memberships.manage`; ปิดด้วย `MembershipExpiryReminder__Enabled=false`.
6. **การตัดสินใจที่ต้องให้ System Admin/Security ยืนยัน:** threshold 30/7/1 วัน (TEST_ONLY), ผู้รับ = `memberships.manage` (D2; ผู้ถือ `users.manage` อย่างเดียวไม่ได้รับ), ไม่ส่ง "หมดอายุแล้ว" (D5), ช่วงสแกนทุก 60 นาที, **Production ต้องมอบ `memberships.manage` ให้ Role ผู้ดูแลจริง (ควรมี `users.read` คู่กันเพื่อให้ลิงก์เปิดได้)**.
7. **ข้อจำกัดที่ยอมรับ:** ไม่มีหน้าจอ/ตัวบ่งชี้สถานะ job; N+1 query ขนาดเล็กต่อ Membership ต่อรอบ (จำกัด 200); แจ้งหลังเที่ยงคืนช้าได้สูงสุดหนึ่งช่วงรอบ (60 นาที); เจ้าของ Membership ไม่ได้รับแจ้ง; ผู้ดูแลคนสุดท้ายยังหมดอายุตามเวลาได้ (ไม่มี guard — มีแต่การแจ้ง); `ExpiresAtUtc` ที่เปลี่ยนแล้วย้อนกลับเป็นค่าเดิมจะไม่ส่งซ้ำ (hash เดิมถูก dedupe) — พฤติกรรมตั้งใจ.
8. **ยังเปิดอยู่ (ไม่ใช่งานของรอบนี้):** [Open Decisions](../superpowers/plans/2026-10-12-g21-open-decisions.md) — A หลาย Organization, B PII/Retention ใน Audit.

- [ ] **Step 3: อัปเดตสถานะ (G-21 ข้อ 1 เท่านั้น)**

- `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`:
  - แถวตาราง G-21 (บรรทัดที่ขึ้นต้น `| G-21 |`): เปลี่ยนคอลัมน์สุดท้าย `—` เป็น `ข้อ 1 (Membership ใกล้หมดอายุ) Implemented <วันที่จริง> (tests written, not run; browser not checked; ยังไม่ผ่าน UAT); ข้อ 2–3 รอคำตอบ — [แผน G-21](2026-10-12-g21-membership-expiry.md); [Open Decisions](2026-10-12-g21-open-decisions.md); [Verification](../../05-engineering/membership-expiry-verification.md)`.
  - checkbox `- [ ] Membership ใกล้หมดอายุ → แจ้งเตือนผู้ดูแล (G-02)` → `- [x] Membership ใกล้หมดอายุ → แจ้งเตือนผู้ดูแล (G-02) — Implemented <วันที่จริง> (hosted worker + system-generated notification; tests written, not run; browser not checked) — [Verification](../../05-engineering/membership-expiry-verification.md)`.
  - สอง checkbox ถัดไป (`ตัดสินใจ User หลาย Organization…` และ `PII visibility/retention…`) **คง `[ ]`** และต่อท้ายบรรทัดแต่ละข้อด้วย ` — รอคำตอบ: [Open Decisions](2026-10-12-g21-open-decisions.md) (options + ข้อเสนอแนะ; ยังไม่ implement)`.
  - บรรทัด `(Future) Event ที่ต้องมี scheduler: …` ใน G-02 (คง `[ ]`): แทนวลี `Membership ใกล้หมดอายุ (G-21) — ยังไม่มี job ตามเวลาใน repo` ด้วย `Membership ใกล้หมดอายุ (G-21: ทำแล้ว <วันที่จริง> ด้วย MembershipExpiryReminderWorker — scheduler ตัวแรกของ repo, ADR 0022); ที่เหลือ (ใบประกัน, SLA G-18) ยังไม่มี job`.
- `docs/00-overview/implementation-roadmap.md` แถว Foundation: ต่อท้ายวลี `ครอบคลุมเอกสารรออนุมัติ 6 แหล่ง` ด้วย ` และแจ้งผู้ถือ memberships.manage เมื่อ Membership ใกล้หมดอายุ 30/7/1 วัน (G-21 ข้อ 1, <วันที่จริง>, hosted worker + tests written but not run; ยังไม่ตรวจในเบราว์เซอร์และยังไม่ผ่าน UAT; การตัดสินใจเรื่อง User หลาย Organization และ PII/Retention ใน Audit ยังรอคำตอบ)` และเพิ่มลิงก์ `[Membership Expiry Verification](../05-engineering/membership-expiry-verification.md)` ท้ายคอลัมน์หลักฐานของแถวเดียวกัน.
- `docs/05-engineering/notification-foundation-verification.md`: บรรทัดที่ขึ้นต้น `- Event source มี 6 แหล่ง` เพิ่มประโยคท้ายบรรทัด: ` ต่อมา G-21 เพิ่ม type system-generated \`membership.expiring\` (scheduler ตัวแรก; ดู [Membership Expiry Verification](membership-expiry-verification.md)) — ที่เหลือยังเป็น Future.`
- ตรวจลิงก์ภายในด้วยคำสั่งใน Task 1 Step 6 กับไฟล์ที่แก้ → ต้องไม่มี `MISSING`.

- [ ] **Step 4: Commit**

```bash
git add docs/05-engineering/membership-expiry-verification.md docs/00-overview/implementation-roadmap.md docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md docs/05-engineering/notification-foundation-verification.md
git commit -m "docs(membership): record G-21 verification and update status

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Self-review (ตรวจแล้ว)

- ทุก path ในแผนมีอยู่จริงหรือทำเครื่องหมาย Create; ลำดับ Task คอมไพล์ได้ทีละ commit: Task 2 ขยายแกนกลางโดยการเรียกเดิมทั้งหมดยัง compile (`Guid` → `Guid?`, descriptor พารามิเตอร์ default); Task 3 เพิ่มชนิดใหม่โดยไม่มีผู้เรียก; Task 4 ใช้ชนิดจาก Task 2–3; Task 5 ใช้ service จาก Task 4; Task 6 อิสระจาก backend.
- ไม่มีขั้นที่สั่งให้รัน test; ทุก Task ปิดด้วย build/tsc/lint เท่านั้น; Verification ระบุตรง ๆ ว่า tests written, not run พร้อมคำสั่งที่ผู้ใช้ควรรัน.
- ไม่มีรหัส error ใหม่ → ไม่ต้องแก้ `ProblemDetailsMapper.GetStatus`/`Errors.resx`; ไม่มี endpoint → ไม่ regenerate OpenAPI; ไม่มี entity/column → ไม่มี migration (Task 7 ตรวจ `has-pending-model-changes`).
- ข้อ (2) และ (3) ของ G-21 ไม่ถูก implement และไม่ถูกติ๊ก.
