# Notification Foundation Verification (G-02)

## 1. สถานะ

Implemented 2026-10-10 ด้วยผล **focused tests เท่านั้น** (in-app เท่านั้น) — ยังไม่ผ่านการตรวจ bell dropdown ในเบราว์เซอร์จริง, full suite, Playwright หรือ UAT ของ Role จริง. สัญญา: [Notification API Contract](../03-contracts/notification-api-contract.md); การตัดสินใจ: [ADR 0018](../adr/0018-in-app-notification-foundation.md); แผน: [แผน G-02](../superpowers/plans/2026-10-08-g02-notification-foundation.md).

## 2. ผลที่รันจริง (2026-10-10)

| คำสั่ง | ผล |
| --- | --- |
| `dotnet build backend/TanErp.slnx --nologo -v q` | 0 Warning, 0 Error |
| Unit `FullyQualifiedName~Notification` | 73 passed / 0 failed |
| Architecture `backend/tests/TanErp.ArchitectureTests` | 4 passed / 0 failed |
| Integration `FullyQualifiedName~Notification` (`-m:1`) | 32 passed / 0 failed |
| Regression Integration `FullyQualifiedName~EstimateEndpointsTests` (`-m:1`) | 54 passed / 0 failed |
| Regression Integration `FullyQualifiedName~CostRecordEndpointsTests\|FullyQualifiedName~ItemCatalogEstimateFlowTests` (`-m:1`) | 16 passed / 0 failed |
| Regression Integration `FullyQualifiedName~ProcurementEndpointsTests` (`-m:1`) | 6 passed / 0 failed |
| Regression Integration `FullyQualifiedName~ProjectControlEndpointsTests` (`-m:1`) | 5 passed / 0 failed |
| Regression Integration `FullyQualifiedName~MrpEndpointsTests` (`-m:1`) | 4 passed / 0 failed |
| Regression Integration `FullyQualifiedName~IdentityAdministrationEndpointsTests\|FullyQualifiedName~UsersEndpointsTests` (`-m:1`) | 28 passed / 0 failed |
| Integration `FullyQualifiedName~OpenApiContractTests` | 1 passed / 0 failed |
| `npm run check:api` (frontend) | exit 0; generate จาก OpenAPI snapshot ไม่มีไฟล์เปลี่ยน |
| `npm run lint` (frontend) | exit 0, ไม่มี error |
| `npm run typecheck` (frontend) | exit 0, ไม่มี error |
| `npm run build` (frontend) | exit 0 |
| `npx vitest run src/lib/notifications src/hooks/useNotifications.test.tsx src/components/layout src/features/notifications src/lib/api` | 12 files, 73 tests passed |

สิ่งที่ชุดเหล่านี้พิสูจน์ (ตาม test ที่รัน): การ publish ของ Notification อยู่ใน transaction เดียวกับการเปลี่ยนสถานะเอกสาร (rollback ไม่ทิ้ง notification, commit มี notification); ผู้รับเป็นผู้ถือ permission อนุมัติใน Organization/Branch ของเอกสาร โดยตัดผู้ทำ (maker) ออก; อ่านและ mark read ได้เฉพาะของตน และข้าม Organization ตอบ 404; payload ผ่าน allowlist; registry ของ type ตรงกับ whitelist; regression ของ 6 แหล่ง event และโมดูลเดิม (Estimate, Cost, Procurement, Project Control, MRP, Identity/Users) ยังผ่าน.

### Mutation check (Task 8, commit `704dcf6`)

ผู้ทำ Task 8 รัน mutation เองแล้ว revert ทุกครั้ง (`git status` สะอาดหลังแต่ละรอบ) ผลที่รายงานไว้ในรอบทำงาน (ไม่ได้ถูกบันทึกใน commit message; ผู้ประสานงานตรวจเพียง `git status` สะอาดและรัน `FullyQualifiedName~Notification` ซ้ำได้ 32/32):

| Mutation | ผล |
| --- | --- |
| (a) เอาการตัดผู้ทำ (maker) ออกทั้ง resolver และ planner | 6 test ล้ม (รวม `SubmitPurchaseOrder_NotifiesTheOtherApproverOnce...`, `UnreadCount_DecrementsOnMarkRead...` และ 4 test ใน `NotificationInfrastructureTests`) |
| (a1) เอาออกเฉพาะ planner | 2 test ล้ม (`NotificationInfrastructureTests` เท่านั้น); test ระดับ API ยังเขียว |
| (a2) เอาออกเฉพาะ resolver | 2 test ล้ม (`NotificationInfrastructureTests` เท่านั้น); test ระดับ API ยังเขียว |
| (b) เอา own-only filter (`NotificationStore.Own`) ออก | 10 test ล้ม (store, API own-only, publish flow) |

- การตัดผู้ทำมี 2 ชั้นโดยออกแบบ จึงต้องเอาออกทั้งสองชั้นถึงจะให้ test ระดับ API ล้ม; ชั้นเดียวถูกจับโดย test ระดับ infrastructure.
- ผลนี้เป็นรายงานของผู้ทำ ไม่ใช่การรัน mutation ซ้ำโดยผู้ตรวจ.

### สิ่งที่ไม่ได้รัน

- Full backend suite และ full frontend suite (รันเฉพาะ focused filter ข้างต้น).
- Playwright journey.
- UAT ด้วย Role จริง (authorized-role UAT).
- การตรวจ bell dropdown ในเบราว์เซอร์จริง (ดู section 3).
- การรัน mutation check ซ้ำโดยผู้ตรวจอิสระ (ผลใน section 2 เป็นรายงานของผู้ทำ).

## 3. การตรวจในเบราว์เซอร์

**ยังไม่ได้ตรวจ** ต้องยืนยันด้วยมือ:

1. Bell บนพื้น header สีกรมท่า (navy) มี contrast พอทั้งธีมสว่างและมืด และ focus ring มองเห็นได้.
2. Esc ปิด dropdown แล้ว focus กลับไปที่ปุ่มกระดิ่ง; คลิกนอกกล่องก็ปิดได้.
3. หน้า `/notifications`: ตัวกรอง unread responsive ที่ความกว้าง 375px และคอลัมน์ action (sticky) ไม่ทับข้อมูล.
4. Badge อัปเดตภายในราว 30 วินาทีหลังมี notification ใหม่ และหยุด poll เมื่อสลับแท็บ (ตรวจด้วย Network).
5. สกรีนรีดเดอร์อ่านจำนวนที่ยังไม่อ่านและสถานะ mark read ได้.

## 4. ข้อจำกัดและการตัดสินใจ

- **In-app เท่านั้น** — ไม่มี email, LINE, SMS (เลื่อนตามคำสั่งผู้ใช้; ไม่มี `IEmailSender` และไม่มี outbox; ADR 0018 ระบุเงื่อนไขประเมินใหม่เมื่อมีช่องทางส่งออก).
- Polling ทุก 30 วินาที (หยุดเมื่อแท็บซ่อน) — ไม่มี push/WebSocket; ผู้ใช้อาจเห็นการแจ้งเตือนช้าได้สูงสุดราว 30 วินาที.
- Event source มี 6 แหล่ง เฉพาะ "ส่งเข้าสถานะรออนุมัติ". Event ที่ต้องมี scheduler/background job ซึ่งยังไม่มีใน repo ยังเป็น Future: ใบประกันใกล้หมดอายุ, งานบริการเกิน SLA (G-18), Membership ใกล้หมดอายุ (G-21). แต่ละเรื่องเพิ่ม type ตามขั้นตอนในสัญญาได้โดยไม่แก้โค้ดกลาง.
- ผู้ทำ (maker) ถูกตัดออกสองที่ (resolver และ planner) — ผู้ทำที่ถือสิทธิ์อนุมัติจะไม่ได้รับแจ้งเอกสารของตนเอง.
- Estimate แจ้งเฉพาะผู้ตรวจขั้นแรกของ route; ผู้ตรวจขั้นถัดไปยังไม่ได้รับแจ้งเมื่อขั้นก่อนหน้าอนุมัติ (Validation Question ข้อ 2 ยังค้าง).
- การแจ้งเตือนเกินจำนวนผู้รับสูงสุดต่อ event (`MaxRecipientsPerEvent` = 50) ถูกตัดทอนแบบเงียบ — ยังไม่มี logger บันทึก.
- Dedupe race บน `ux_notifications_recipient_dedupe` ทำได้ยากมากในทางปฏิบัติ (ยังไม่ได้พิสูจน์ด้วย test แบบ concurrent).
- **ไม่ได้กำหนด retention/purge** ของแถวที่อ่านแล้ว (Validation Question ข้อ 1 ค้างที่ Security + Operations) — ตารางจะโตต่อเนื่อง.
- a11y: dropdown ไม่มี arrow-key roving (ใช้ Tab/Enter/Space ของปุ่มจริง); ยังไม่ผ่านการตรวจด้วยสกรีนรีดเดอร์.
- กฎผู้รับเป็นค่าเริ่มต้น TEST_ONLY รอ Security/Operations ยืนยัน.
