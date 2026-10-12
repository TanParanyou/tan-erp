# Membership Expiry Verification (G-21 ข้อ 1)

## 1. สถานะ

**Implemented 2026-10-12 (G-21 ข้อ 1: Membership ใกล้หมดอายุ → แจ้งผู้ถือ `memberships.manage` ผ่าน hosted worker และ notification แบบ system-generated) — tests written, not run.**

- ชุดทดสอบ unit, integration และ frontend ถูก **เขียนแล้ว แต่ยังไม่ได้รันในรอบนี้** เพราะผู้ใช้เป็นผู้รันชุดทดสอบเอง (ดูหัวข้อ 3)
- **ยังไม่ได้ตรวจในเบราว์เซอร์** (bell, หน้า `/notifications`, ลิงก์ deep link ยังไม่เคยเปิดดูจริง)
- **Worker ยังไม่เคยถูกรันกับ PostgreSQL จริง** ทั้งใน local และ Production-like environment
- **ยังไม่รัน full gate** (`dotnet test` ทั้งชุด, `npm run build` ในรอบนี้, `npm run lint` ตาม Definition of Done) และยังไม่ผ่าน UAT หรือพร้อม Production
- ข้อ 2 และ ข้อ 3 ของ G-21 **ยังไม่ implement** และยังไม่ถูกติ๊ก: [G-21 Open Decisions](../superpowers/plans/2026-10-12-g21-open-decisions.md)

เอกสารที่เกี่ยวข้อง: [ADR 0022](../adr/0022-membership-expiry-reminder-and-hosted-worker.md), [แผน G-21](../superpowers/plans/2026-10-12-g21-membership-expiry.md), [Notification Verification](notification-foundation-verification.md)

## 2. ผลที่รันจริง (compile/lint เท่านั้น)

| คำสั่ง | ผล | รันเมื่อ |
| --- | --- | --- |
| `dotnet build backend/TanErp.slnx --nologo -v q` | `Build succeeded.` `0 Warning(s)` `0 Error(s)` | รอบเอกสารนี้ (2026-10-12) |
| `cd frontend && npx tsc --noEmit` | exit 0 | รอบเอกสารนี้ |
| `cd frontend && npm run lint` | exit 0 | รอบเอกสารนี้ |
| `dotnet ef migrations has-pending-model-changes --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api` | ไม่มีการเปลี่ยนแปลงโมเดลค้าง (slice นี้ไม่เพิ่ม entity/column; ไม่มี migration) | ผลจาก Task 1–5 ของแผน (ไม่ได้รันซ้ำในรอบนี้) |
| frontend `npm run build`, key parity `th`/`en` (`messages/`) | ผ่าน | ผลจาก Task 6 ของแผน (ไม่ได้รันซ้ำในรอบนี้) |

ผลข้างต้นเป็นการตรวจว่า **คอมไพล์และ lint ผ่าน** เท่านั้น ไม่ได้หมายความว่า logic ถูกต้องตามชุดทดสอบ

## 3. ชุดทดสอบที่เขียนไว้ (tests written, NOT run)

จำนวนนับจาก grep ของ attribute `[Fact]`/`[Theory]` (backend) และ `it(`/`test(` (frontend) ณ 2026-10-12; `[Theory]` นับเป็น 1 ต่อเมธอด ไม่นับ `InlineData` แต่ละแถว

| ระดับ | ไฟล์ | จำนวนเมธอด | สถานะ |
| --- | --- | --- | --- |
| Unit | `backend/tests/TanErp.UnitTests/Notifications/NotificationRegistryTests.cs` | 10 | เขียนแล้ว ยังไม่รัน |
| Unit | `backend/tests/TanErp.UnitTests/Notifications/NotificationEventsTests.cs` | 10 | เขียนแล้ว ยังไม่รัน |
| Unit | `backend/tests/TanErp.UnitTests/Notifications/NotificationDomainTests.cs` | 9 | เขียนแล้ว ยังไม่รัน |
| Unit | `backend/tests/TanErp.UnitTests/Notifications/NotificationSystemEventTests.cs` | 4 | เขียนแล้ว ยังไม่รัน |
| Unit | `backend/tests/TanErp.UnitTests/Notifications/MembershipExpiryReminderPlannerTests.cs` | 5 | เขียนแล้ว ยังไม่รัน |
| Integration | `backend/tests/TanErp.IntegrationTests/Persistence/MembershipExpiryReminderServiceTests.cs` | 7 | เขียนแล้ว ยังไม่รัน (Testcontainers Postgres) |
| Integration | `backend/tests/TanErp.IntegrationTests/Api/MembershipExpiryReminderWorkerTests.cs` | 4 | เขียนแล้ว ยังไม่รัน |
| Frontend | `frontend/src/lib/notifications/notification-types.test.ts` | 5 | เขียนแล้ว ยังไม่รัน |
| Frontend | `frontend/src/lib/notifications/notification-view.test.tsx` | 4 | เขียนแล้ว ยังไม่รัน |

รวม backend 49 เมธอดใหม่/แก้ใน 7 ไฟล์ และ frontend 9 เมธอดใน 2 ไฟล์ — **ทั้งหมดยังไม่ได้รัน**

## 4. คำสั่งที่ผู้ใช้ควรรันเอง

รันตามลำดับนี้ เริ่มจาก focused แล้วค่อย full gate (ห้ามอ้างว่าผ่านก่อนเห็นผลจริง):

```bash
# Unit: planner, system-generated event, registry, events, domain
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~MembershipExpiryReminderPlannerTests|FullyQualifiedName~NotificationSystemEventTests|FullyQualifiedName~NotificationRegistryTests|FullyQualifiedName~NotificationEventsTests|FullyQualifiedName~NotificationDomainTests"

# Integration: service (advisory lock, dedupe, re-arm) และ worker
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~MembershipExpiryReminderServiceTests|FullyQualifiedName~MembershipExpiryReminderWorkerTests" -m:1

# Regression: แกนกลางแจ้งเตือนเดิมต้องไม่เปลี่ยนพฤติกรรม (ActorUserId กลายเป็น nullable)
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~NotificationPublishPlannerTests|FullyQualifiedName~NotificationPayloadRulesTests|FullyQualifiedName~NotificationHandlerTests"
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~NotificationInfrastructureTests|FullyQualifiedName~NotificationEndpointsTests|FullyQualifiedName~NotificationSchemaTests|FullyQualifiedName~NotificationDedupeConflictTests" -m:1

# Architecture
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj

# Frontend: notification types/view และ component ที่ใช้ bell
cd frontend && npx vitest run src/lib/notifications
```

**Final gate (ก่อนส่งมอบ/merge ตาม AGENTS.md หมวด 8):**

```bash
# Backend
dotnet build backend/TanErp.slnx --nologo -v q
dotnet test backend/TanErp.slnx

# Frontend
cd frontend && npm run build && npm run lint && npx vitest run
```

## 5. สิ่งที่ชุดทดสอบที่เขียนไว้ตั้งใจพิสูจน์ (ยังไม่ได้พิสูจน์จริง)

- **Planner:** เลือก threshold ที่เล็กที่สุดที่เข้าเกณฑ์ (เหลือ ≤ 30 → 30; ≤ 7 → 7; ≤ 1 → 1); Membership ที่หมดอายุแล้วไม่ถูกเตือน; transition id deterministic และต่างกันเมื่อ input ต่าง
- **แกนกลาง:** type `membership.expiring` เป็น system-generated (ไม่มี actor และไม่มี `actorDisplayName`); type ปกติยังบังคับ actor; registry/FE parity; ชื่อ display name ถูก clip ที่ 200 ตัวอักษร
- **Service บน PostgreSQL จริง:** ส่งครั้งเดียวต่อ threshold แม้สแกนซ้ำ; ไล่ 30 → 7 → 1 ตามเวลา; re-arm เมื่อเปลี่ยน `ExpiresAtUtc`; ข้าม inactive/หมดอายุแล้ว/ยังไม่เริ่ม/ไกลเกิน threshold/ไม่มีวันหมดอายุ; ส่งเฉพาะผู้ถือ `memberships.manage`; advisory lock ทำให้ instance ที่ไม่ได้ล็อกข้ามรอบ
- **Worker:** tick เรียก service; exception ไม่ทำให้ schedule หยุด; ไม่ถูกลงทะเบียนใน environment `Test`
- **Frontend:** render ข้อความ th/en ตาม field ที่ประกาศ (`subjectDisplayName`, `daysRemaining`); ลิงก์ deep link ไป `/settings/users/{parentId}`

## 6. การตรวจเชิงปฏิบัติการที่ยังไม่ได้ทำ (ต้องทำก่อนเปิดใช้จริง)

1. เริ่ม API ใน environment ที่ไม่ใช่ `Test` และดู log บรรทัด `Membership expiry reminder run finished` หลังประมาณ 60 วินาที (startup delay)
2. ตั้ง Membership ทดสอบให้หมดอายุภายใน 3 วัน แล้วตรวจว่า bell ของผู้ถือ `memberships.manage` แสดงข้อความ 1 รายการต่อ threshold (ไม่ซ้ำ)
3. ตรวจลิงก์ deep link เปิด `/settings/users/{userId}` ได้เมื่อผู้รับมี `users.read`
4. ตรวจว่าปิดได้จริงด้วย `MembershipExpiryReminder__Enabled=false`
5. ตรวจข้อความ FE ทั้ง `th` และ `en` ในเบราว์เซอร์

## 7. การตัดสินใจที่ต้องให้ Business Owner / System Admin / Security ยืนยัน

| # | เรื่อง | ค่าปัจจุบัน | หมายเหตุ |
| --- | --- | --- | --- |
| 1 | Scheduler | `BackgroundService` + `PeriodicTimer`, ค่าเริ่มต้น 60 นาที, startup delay 60 วินาที | production ต้องยืนยันว่า `MembershipExpiryReminder:Enabled` เป็น `true` และแอปรันเป็น process ที่ทำงานต่อเนื่อง (long-lived) — **จะไม่ทำงานบน serverless/short-lived hosting** |
| 2 | Threshold | 30 / 7 / 1 วัน (**TEST_ONLY**), ตั้งได้ที่ `MembershipExpiryReminder:ThresholdDays` | ค่านี้ยังไม่ใช่นโยบายจริง; แต่ละรอบส่งเฉพาะ threshold ที่เล็กที่สุดที่เข้าเกณฑ์ |
| 3 | ผู้รับ | ผู้ถือ `memberships.manage` ที่ Organization scope (cap 50 รายต่อ notice); **เจ้าของ Membership ไม่ถูกตัดออก**; ไม่ใช้ `users.manage` | Production ต้องมอบ `memberships.manage` ให้ Role ผู้ดูแลจริง และควรมอบ `users.read` คู่กันเพื่อให้ลิงก์เปิดได้ |
| 4 | ไม่มี notice "หมดอายุแล้ว" | ข้าม Membership ที่ `ExpiresAtUtc <= now` | ดู ข้อจำกัดที่ยอมรับ ข้อ 4 |
| 5 | การเปลี่ยนแกนกลาง G-02 | `ActorUserId` เป็น nullable และ descriptor มี `SystemGenerated`; event ปกติไม่เปลี่ยนพฤติกรรม | Security/Owner รับทราบการขยายสัญญา [Notification API Contract](../03-contracts/notification-api-contract.md) |
| 6 | Multi-instance | `pg_try_advisory_xact_lock(2110000021)` + unique dedupe index ของ G-02 | ยืนยันเมื่อ deploy หลาย instance |
| 7 | ข้อความ/payload | ข้อความ en เป็น `day(s)`; payload มี `daysRemaining` เท่านั้น ไม่มีวันที่หมดอายุ; deep link `/settings/users/{userId}` | ข้อความ en ยังไม่ผ่านการ review ของ Owner |

## 8. ข้อจำกัดที่ยอมรับ

1. **ไม่มีหน้าจอ/ตัวบ่งชี้สถานะ job** ผู้ดูแลไม่เห็นว่ารอบล่าสุดรันเมื่อไร ต้องดู log
2. **N+1 query ขนาดเล็กต่อรอบ**: 1 query ผู้รับ + 1 query dedupe ต่อ Membership; จำกัด 200 Membership ต่อรอบ (clamp 1–1000) ที่เหลือรอบถัดไป
3. **ความล่าช้า**: แจ้งเตือนช้าได้สูงสุดหนึ่งรอบ (60 นาที) และการเปลี่ยน `ExpiresAtUtc` จะมีผลในรอบถัดไป
4. **ผู้ดูแลคนสุดท้ายยังหมดอายุตามเวลาได้**: guard `LAST_ADMINISTRATOR_REQUIRED` ตรวจเฉพาะตอนแก้ Membership (`IdentityAdministrationStore`) ไม่ตรวจตอนหมดอายุตามเวลา slice นี้แค่แจ้งล่วงหน้า **ไม่ได้ปิดช่องโหว่นี้** — เป็นช่องว่างที่รู้อยู่แล้ว
5. **เจ้าของ Membership ที่ไม่ใช่ผู้ถือ `memberships.manage` ไม่ได้รับแจ้ง** (ตามการตัดสินใจ: แจ้งผู้ที่ต่ออายุได้)
6. **ค่า `ExpiresAtUtc` ที่ถูกเปลี่ยนแล้วย้อนกลับเป็นค่าเดิม จะไม่ส่งซ้ำ** เพราะ dedupe hash เดิมยังอยู่ — พฤติกรรมตั้งใจ
7. **ผู้ถือ `memberships.manage` ที่ไม่มี `users.read`** จะเห็นข้อความแต่เปิดลิงก์ไม่ได้

## 9. ยังเปิดอยู่ (ไม่ใช่งานของรอบนี้)

- G-21 ข้อ 2 — User หลาย Organization: [Open Decisions, Decision A](../superpowers/plans/2026-10-12-g21-open-decisions.md)
- G-21 ข้อ 3 — PII visibility/retention ใน Audit: [Open Decisions, Decision B](../superpowers/plans/2026-10-12-g21-open-decisions.md)

ทั้งสองข้อยัง **ไม่ได้ implement และยังไม่ถูกติ๊ก** ในแผนปิดช่องว่าง [G-02/G-21 ใน Gap Closure Plan](../superpowers/plans/2026-10-05-erp-gap-closure-plan.md)
