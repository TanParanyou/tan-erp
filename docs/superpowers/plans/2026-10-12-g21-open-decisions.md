# G-21 Open Decisions: User หลาย Organization และ PII/Retention ใน Audit

> **สถานะ:** รอคำตอบจากผู้รับผิดชอบ — **ยังไม่ implement ใด ๆ** เอกสารนี้เป็นข้อมูลประกอบการตัดสินใจ (options, ข้อแลกเปลี่ยน, ข้อเสนอแนะ, ผลต่อโค้ด/สัญญา) สำหรับ 2 ข้อที่เหลือของ G-21 ใน [แผนปิดช่องว่าง](2026-10-05-erp-gap-closure-plan.md). แผน implement ส่วนที่ทำได้เลย (Membership ใกล้หมดอายุ) อยู่ใน [แผน G-21](2026-10-12-g21-membership-expiry.md). ค่าที่ระบุว่า TEST_ONLY ห้ามถือเป็นนโยบายจริง.

ข้อเท็จจริงทุกข้ออ้างจากโค้ดที่ตรวจแล้ว ณ 2026-10-12 (branch `feat/g21-membership-expiry`, ฐาน `main` ที่มี G-01/G-02/G-03a-c).

---

## Decision A — User หนึ่งคนอยู่ได้หลาย Organization หรือไม่

### บริบท (สิ่งที่โค้ดเป็นอยู่ตอนนี้)

| ข้อเท็จจริง | หลักฐาน |
| --- | --- |
| ระดับข้อมูลรองรับอยู่แล้ว: `Membership` เป็นตัวเชื่อม `(OrganizationId, UserId, BranchId?)`; **ไม่มี unique** ต่อ `(user)` หรือ `(organization, user)` — User เดียวมี Membership ได้หลายแถว ทั้งต่างสาขาและต่าง Organization | `backend/src/TanErp.Infrastructure/Persistence/Configurations/MembershipConfiguration.cs:12-36` (ไม่มี `HasIndex`) |
| ผู้เรียกเลือกบริบทด้วย header `X-Membership-Id` ทุก request; `GET /me` คืนรายการ Membership ที่ active ทั้งหมดของผู้ใช้ | `TanErp.Api/RequestContext/RequestContextReader.cs:22`, `CurrentUserReader.cs:42-48` |
| แต่ **Admin API ปิดทางนี้อยู่**: อีเมลต้อง unique ทั้งระบบ (`NormalizedEmail` unique) และสร้าง/เปลี่ยนอีเมลที่ซ้ำคืน `USER_EMAIL_ALREADY_EXISTS` | `UserConfiguration.cs:29`, `IdentityAdministrationStore.cs:142-143, 187` |
| รายการ/รายละเอียดผู้ใช้ของ Admin เห็นเฉพาะ User ที่มี Membership ใน Organization ของผู้เรียก และโหลดเฉพาะ Membership ของ Organization นั้น | `IdentityAdministrationStore.cs` (`LoadUsersAsync`: `u.Memberships.Any(m => m.OrganizationId == organizationId)`) |
| `User.IsActive` เป็นธงระดับระบบ (ไม่ใช่ต่อ Organization): ปิดผู้ใช้จากหน้า Admin ขององค์กร A จะตัดการเข้าใช้ของ Membership ในองค์กร B ด้วย | `Membership.IsActiveAt` ตรวจ `User.IsActive` (`Membership.cs:52-62`); `RequestAccessResolver.cs` ทุก query กรอง `m.User!.IsActive` |
| `FirebaseUid` ผูก 1:1 กับ `User` (ผูกด้วยอีเมลที่ verified) | `FirebaseIdentityLinker.cs:23-38` |
| แผน G-03 ระบุไว้แล้วว่า "หลาย Organization ต่อ deployment" ยังเป็น Validation Question และไม่มี endpoint สร้าง Organization | [แผน G-03 หัวข้อ Out of scope](2026-10-10-g03-org-role-authority.md), [Organization Admin Verification](../../05-engineering/organization-administration-verification.md) |

สรุป: วันนี้ผู้ใช้ที่มีหลาย Organization **สร้างผ่าน API ไม่ได้** (ทำได้เฉพาะแก้ DB ตรง ๆ) แต่ส่วนอ่าน/เลือกบริบทพร้อมรองรับ ความเสี่ยงคือพฤติกรรมที่ไม่ได้ออกแบบ (เช่น ปิดผู้ใช้ข้ามองค์กร) หากมีคนสร้างข้อมูลลักษณะนี้.

### Options

| Option | แนวคิด | ข้อดี | ข้อเสีย / ความเสี่ยง |
| --- | --- | --- | --- |
| **A1 — 1 User = 1 Organization (ล็อกเป็นกฎ)** | Membership ทั้งหมดของ User ต้องอยู่ Organization เดียว (ยังมีหลายสาขาได้) | ตรงกับพฤติกรรม API ปัจจุบัน; ไม่มีการรั่วข้อมูลข้ามองค์กร; `User.IsActive` ระดับระบบยังถูกต้อง; ไม่ต้องแตะ FE | คนเดียวทำงานสองบริษัทต้องมีสองบัญชี/สองอีเมล; ถ้าอนาคตต้องการหลายองค์กรต้องทำใหม่ |
| **A2 — 1 บุคคล = 1 User, หลาย Organization (global identity)** | เพิ่ม flow "เพิ่มผู้ใช้ที่มีอยู่แล้วเข้า Organization อื่น" | คนเดียวเข้าได้หลายองค์กรด้วย login เดียว; มี `X-Membership-Id` รองรับ | ต้องแยก "ปิดใช้งานทั้งบัญชี" (ระบบ) ออกจาก "ปิดใช้ในองค์กรนี้" (Membership) — ต้องมีผู้ดูแลระดับแพลตฟอร์ม; `USER_EMAIL_ALREADY_EXISTS` เปิดเผยว่าอีเมลมีในอีกองค์กร (info disclosure) ต้องออกแบบ invite/consent; ชื่อ/อีเมลของ User แสดงข้ามองค์กร; FE ต้องมี org switcher; test ข้ามองค์กรเพิ่มมาก |
| **A3 — 1 User ต่อ Organization แม้เป็นคนเดียวกัน (อีเมลเดียวกันได้)** | เปลี่ยน unique อีเมลเป็น `(organization, email)` | แยกองค์กรขาดจากกัน | ชน `FirebaseUid` 1:1 (ต้องมีตาราง identity-link ใหม่); เปลี่ยนการผูกตัวตนทั้งระบบ — **ใหญ่เกินกว่าที่ G-21 ควรแตะ ไม่แนะนำ** |

### ข้อเสนอแนะ: **A1 ตอนนี้** (ปรับเป็น A2 เมื่อมีความต้องการจริงของลูกค้า)

เหตุผล: (1) deployment ปัจจุบันเป็นองค์กรเดียว และ G-03 ตัดการสร้างองค์กรออกแล้ว; (2) A1 ไม่ต้องแก้โค้ดใด ๆ ทางธุรกิจ เพราะ API บังคับอยู่แล้วโดยปริยาย — ต่างเพียงต้อง *ประกาศเป็นกฎ* และ *ล็อกด้วย test* เพื่อไม่ให้ endpoint ในอนาคต (เช่น "เพิ่ม Membership ให้ผู้ใช้เดิม") เปิดช่องโดยไม่ตั้งใจ; (3) A2 เปลี่ยนความหมายของ `users.manage` และการปิดผู้ใช้ ซึ่งเป็นเรื่อง Security.

### ถ้าเลือก A1 โค้ด/สัญญาที่ต้องเปลี่ยน (เล็ก)

- `docs/03-contracts/identity-administration-api-contract.md`: เพิ่มกฎ "User หนึ่งคนมี Membership ได้เฉพาะ Organization เดียว; เพิ่มผู้ใช้ที่อีเมลมีอยู่แล้วคืน `409 USER_EMAIL_ALREADY_EXISTS` เสมอ (ไม่ตอบว่าอยู่ Organization ใด)".
- `docs/04-data/database-standards.md` หัวข้อ Multi-organization: ระบุกฎเดียวกัน.
- test ใหม่ 1 ชุด: สร้าง Membership ผ่าน admin endpoint ให้ User ที่มีอยู่ในอีกองค์กรไม่ได้ (integration, ไม่มีโค้ด production ใหม่ ถ้ายังไม่มี endpoint ดังกล่าว) และ unit/architecture test ที่อ้างกฎว่าไม่มีโค้ด production สร้าง `Membership` ข้าม Organization ของ User (grep-based parity เหมือน `BranchDependencyInspectorCoverageTests`).
- ไม่มี migration, ไม่เปลี่ยน OpenAPI.

### ถ้าเลือก A2 โค้ด/สัญญาที่ต้องเปลี่ยน (ใหญ่ — ต้องเป็น slice ใหม่ ไม่ใช่ G-21)

- Domain: แยก `User.IsActive` (บัญชีระบบ) ออกจากการปิดใช้ต่อ Organization (ใช้ `Membership.IsActive` ที่มีอยู่); role "platform administrator" ที่ปิดบัญชีระบบได้; ปรับ `LAST_ADMINISTRATOR_REQUIRED` ให้นับต่อ Organization.
- API: endpoint ใหม่ "invite existing user to organization" พร้อมกฎ consent; เปลี่ยนความหมาย `USER_EMAIL_ALREADY_EXISTS`; ซ่อนอีเมล/ชื่อของ User จากองค์กรที่ไม่มี Membership; error codes ใหม่ + `Errors.resx`/`Errors.en.resx` + `ProblemDetailsMapper`.
- Audit: บันทึกว่า action ทำในบริบท Organization ใด (มี `OrganizationId` ใน `AuditEvent` อยู่แล้ว).
- FE: org switcher (ส่ง `X-Membership-Id` ที่เลือก), ข้อความ i18n th/en.
- Security review + ชุด test ข้ามองค์กร (อ่าน/เขียน/แจ้งเตือน/ไฟล์).

### ใครต้องตอบ

**Business Owner** (องค์กรเดียวหรือกลุ่มบริษัท? มีคนเดียวทำงานหลายนิติบุคคลหรือไม่) + **Security** (นโยบายตัวตนข้ามองค์กร, การปิดบัญชี) + **System Admin** (ขั้นตอน onboard จริง). คำถามตรง ๆ: "ภายใน 12 เดือนมีลูกค้า/บริษัทในเครือที่ต้องใช้ระบบเดียวกันโดยข้อมูลแยกกันหรือไม่ และพนักงานคนเดียวต้อง login เข้าหลายบริษัทด้วยอีเมลเดียวหรือไม่".

---

## Decision B — การมองเห็น PII และ Retention ใน Audit

### บริบท (สิ่งที่โค้ดเป็นอยู่ตอนนี้)

| ข้อเท็จจริง | หลักฐาน |
| --- | --- |
| `AuditEvent` เก็บ: org, actor user id, actor membership id, action, resource type/id, เวลา, trace id, request id, `ChangesJson` (JSON อิสระ), row version ก่อน/หลัง, `Reason` | `backend/src/TanErp.Domain/Common/AuditEvent.cs:5-17` |
| กฎเดิมของ Identity Admin: "ไม่เก็บอีเมล/ชื่อใน `changes`" และ audit ของ G-03 เก็บ id/ชื่อฟิลด์/permission key เท่านั้น | `docs/03-contracts/identity-administration-api-contract.md:27`; [แผน G-03 D8](2026-10-10-g03-org-role-authority.md) |
| ตรวจเฉพาะ writer ของ Identity/Role/Organization admin แล้ว ที่เหลือ (Customer, Estimate, Quotation ฯลฯ) **ยังไม่ได้ตรวจเนื้อหา `ChangesJson` ในรอบนี้** | — |
| มี permission `audit.read` ในแค่แค็ตตาล็อก แต่ **ไม่มี endpoint ใดอ่าน `AuditEvents`** (มีแต่เขียน) | `docs/03-contracts/permission-catalog.md:140`; grep `AuditEvents` ใน `TanErp.Api` ไม่พบการอ่าน |
| ไม่มี retention/purge/archive ใด ๆ — ตารางโตตลอด (และก่อน G-21 ไม่มี background job ใน repo ให้รันงานนี้) | grep `BackgroundService`/`IHostedService` ก่อน G-21 = 0 |
| ผู้ใช้ลบทิ้งไม่ได้ (FK `Restrict`) การ "ลืม" ข้อมูลส่วนบุคคลจึงต้องเป็นการทำให้ไม่ระบุตัวตน (anonymize) แถว `users` | `NotificationConfiguration.cs` และ `MembershipConfiguration.cs` ใช้ `DeleteBehavior.Restrict` |

PII ใน Audit ปัจจุบันเป็นแบบ *อ้อม* (pseudonymous): เก็บ user id ไม่ใช่ชื่อ/อีเมล แต่ id โยงกลับถึงตัวบุคคลได้ผ่านตาราง `users`.

### คำถามที่ Security/Legal ต้องตอบ

1. ใครอ่าน Audit ได้ (ขอบเขต `audit.read` ต่อ Organization/Branch/Project) และอ่านได้ถึงระดับไหน (เห็นชื่อผู้กระทำหรือเห็นแค่ id)?
2. ถือว่า user id เป็น pseudonymous พอหรือไม่ หรือ `ActorMembershipId`/`TraceId`/`RequestId` ก็ต้องจำกัดสิทธิ์?
3. ระยะเก็บ Audit: เท่าไร แยกตามประเภท (การเงิน/เอกสารภาษี vs การจัดการตัวตน)?
4. คำขอลบข้อมูลส่วนบุคคล (PDPA): ยอมรับการ anonymize `users` (ชื่อ/อีเมล) แล้วคง Audit ที่อ้าง id ไว้หรือไม่?
5. ต้องเก็บ IP/User-Agent หรือไม่ (วันนี้ไม่ได้เก็บ)?

### Options

| Option | แนวคิด | ข้อดี | ข้อเสีย |
| --- | --- | --- | --- |
| **B1 — เก็บถาวร, id เท่านั้น, ชื่อ resolve ตอนอ่าน** | คงของเดิม + endpoint อ่านที่ backend ประกอบ `actor { id, displayName }` ตอนอ่านตามสิทธิ์ | ไม่ต้องมี job; anonymize `users` แล้ว Audit ก็ไม่แสดงชื่อ; ตรง "No FE find" | ตารางโตไม่จำกัด; ต้องตัดสิน retention ภายหลังอยู่ดี |
| **B2 — Retention ตามเวลา (archive/purge รายเดือน)** | ใช้ pattern worker ของ G-21 รันงานลบ/ย้ายแถวเก่า | ควบคุมขนาดและเป็นไปตามนโยบายข้อมูล | ต้องรู้ระยะเก็บจริงก่อน (ผิดแล้วลบแล้วกู้ไม่ได้); ต้องแยกประเภท Audit ที่กฎหมายบังคับเก็บ; ต้อง partition/ดัชนีตามเวลา |
| **B3 — Allowlist ฟิลด์ใน `ChangesJson` ต่อ action** | เหมือน notification payload allowlist: factory เดียวต่อ action + test กัน key ที่มีลักษณะ PII (email/phone/address/name) | ป้องกัน PII รั่วเข้า Audit ที่ต้นทาง ทุกโมดูล | ต้องไล่ทุกจุดเขียน Audit (`AddAudit` หลายที่) — งานกว้างแต่กลไก |

### ข้อเสนอแนะ: **B1 + B3 ก่อน, เลื่อน B2 จนกว่า Security/Legal ระบุ retention**

เหตุผล: B3 ลดความเสี่ยงที่ต้นทางโดยไม่ต้องรู้ระยะเก็บ; B1 ให้ทางอ่านที่ปลอดภัย (id + ชื่อที่ประกอบจาก backend ตามสิทธิ์ ไม่ส่ง id ดิบให้ FE ไป find); B2 ลบข้อมูลถาวรจึงต้องรอคำตอบข้อ 3. ค่า retention ใด ๆ ที่ใส่ก่อนมีคำตอบต้องเป็น TEST_ONLY และไม่ต้องรันงานลบ.

### โค้ด/สัญญาที่ต้องเปลี่ยนตาม option

| ส่วน | B1 | B3 | B2 |
| --- | --- | --- | --- |
| สัญญาใหม่ | `docs/03-contracts/audit-api-contract.md` (`GET /api/v1/audit-events`, filter org/branch/resource/action/ช่วงเวลา, pagination, permission `audit.read`) | เพิ่มตาราง "ฟิลด์ที่อนุญาตต่อ action" ใน `docs/04-data/` | เพิ่มหัวข้อ retention ใน `docs/04-data/database-standards.md` + ADR |
| Backend | endpoint + handler + read store (Dapper/projection) ที่ประกอบชื่อผู้กระทำ | ย้ายการสร้าง `AuditEvent.ChangesJson` ไปผ่าน factory/allowlist; test parity ต่อ action | worker (ใช้ pattern `MembershipExpiryReminderWorker`) + migration ดัชนี/partition |
| Frontend | หน้า Audit (ถ้าต้องการ) + i18n th/en | — | — |
| Error codes | ไม่ต้องมีรหัสใหม่ถ้าใช้ `PERMISSION_DENIED` เดิม | — | — |
| Migration | ไม่มี | ไม่มี | มี (ดัชนี/partition/ตารางที่ archive) |

### ใครต้องตอบ

**Security** (ข้อ 1, 2, 5) + **Legal/Compliance** (ข้อ 3, 4 — ระยะเก็บตามกฎหมายภาษี/PDPA) + **System Admin** (ขนาดข้อมูลที่ยอมรับได้). ผู้ใช้แจ้งผลคำตอบแล้วจึงเปิด slice ใหม่ (ไม่ผูกกับ G-21).

---

## สรุปสำหรับผู้ตัดสินใจ

| Decision | ข้อเสนอแนะ | ต้องการจาก | ผลต่อโค้ดถ้าตามข้อเสนอ |
| --- | --- | --- | --- |
| A หลาย Organization | A1 (1 User = 1 Organization) ประกาศเป็นกฎ + ล็อกด้วย test | Business Owner, Security, System Admin | เอกสาร 2 ไฟล์ + test; ไม่มีโค้ด production/migration/OpenAPI |
| B PII/Retention ใน Audit | B1 + B3 ก่อน; เลื่อน B2 | Security, Legal/Compliance | slice ใหม่: audit read API + allowlist factory; ไม่มี purge จนกว่าจะกำหนด retention |

ใน checklist ของ [แผนปิดช่องว่าง](2026-10-05-erp-gap-closure-plan.md) สองข้อนี้ **ยังไม่ถูกติ๊กว่าเสร็จ** — ติ๊กได้เมื่อมีคำตอบเป็นลายลักษณ์อักษรและงานตามข้อเสนอที่เลือกถูก implement แล้วเท่านั้น.
