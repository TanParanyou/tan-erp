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
