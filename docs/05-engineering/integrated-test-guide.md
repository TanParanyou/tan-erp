# Integrated Test Guide (ทดสอบรวม CP-02, CP-04 และ Health)

**สถานะ:** ใช้กับ branch `integration/test-all` ซึ่งรวม PR #10 (เอกสารใบเสนอราคา), #11 (จัดการผู้ใช้และสิทธิ์) และ #13 (health endpoints). ข้อมูลทั้งหมดเป็น `TEST_ONLY` ห้ามใช้เป็นหลักฐาน UAT หรือ pilot data

## เตรียมระบบ

```bash
git checkout integration/test-all
make up                  # PostgreSQL + Firebase Emulator
make seed-users          # ผู้ใช้ทดสอบใน Emulator
make dev-backend-demo SEED_DEDICATED_ESTIMATE_REVIEWER=true   # เทอร์มินัล 1 (API :5005)
make dev-frontend        # เทอร์มินัล 2 (Web :3005)
make seed-quotation-demo # เทอร์มินัล 3: ออกใบเสนอราคาตัวอย่างผ่าน API จริง
```

ผู้ใช้ทดสอบ (รหัสผ่าน `TestPassword123!` จาก `scripts/seed-emulator-users.mjs`): `foundation-user@example.test` (ผู้ดูแล), `foundation-estimate-reviewer@example.test` (ผู้ตรวจอิสระ)

> `SEED_DEDICATED_ESTIMATE_REVIEWER=true` ตัดสิทธิ์ `estimates.approve` ออกจากผู้ใช้อื่น ใช้เมื่อต้องซ้อมการอนุมัติโดยผู้ตรวจอิสระ

## 1. เอกสารใบเสนอราคา (CP-04)

1. ล็อกอินที่ `http://localhost:3005/th/login` แล้วเปิด `/th/estimates/019a3cf8-96f0-7c9f-b207-93aa818f4c04/quotation`
2. ตรวจ: ข้อมูลลูกค้า ณ วันออก, รายการ, ยอดรวม; สลับ **ไทย/English** (ข้อความอังกฤษที่ไม่มีจะเป็น `-` พร้อมแบนเนอร์เตือน)
3. กด **พิมพ์ / บันทึกเป็น PDF** แล้วดูการแบ่งหน้า A4 และว่าเมนู/แถบด้านข้างไม่ติดไปในเอกสาร
4. ย่อหน้าต่างเหลือความกว้างมือถือ (ตารางต้องเลื่อนแนวนอนได้ หน้าไม่ล้น)
5. ที่การ์ด Estimate ของโอกาสขายที่ออกใบเสนอราคาแล้ว ต้องมีปุ่ม "ดูเอกสารใบเสนอราคา"

## 2. จัดการผู้ใช้และสิทธิ์ (CP-02)

1. ล็อกอินเป็น `foundation-user@example.test` เปิด `/th/settings/users` (เมนู "ผู้ใช้และสิทธิ์")
2. **เพิ่มผู้ใช้:** ใส่ชื่อ + อีเมลใหม่ + Role `Test Read Only` → สถานะ "รอเข้าระบบครั้งแรก"
3. **ผูกบัญชีตอนล็อกอินครั้งแรก:** สร้างบัญชี Emulator ที่ยืนยันอีเมลแล้วด้วยอีเมลเดียวกัน แล้วล็อกอินด้วยบัญชีนั้น

   ```bash
   curl -s -X POST "http://127.0.0.1:9099/identitytoolkit.googleapis.com/v1/projects/tan-erp-test-only/accounts?key=fake-api-key" \
     -H "Content-Type: application/json" -H "Authorization: Bearer owner" \
     -d '{"localId":"my-invitee-1","email":"<อีเมลที่เชิญ>","password":"TestPassword123!","displayName":"Invitee","emailVerified":true}'
   ```

   รีเฟรชหน้ารายละเอียดของผู้ใช้ สถานะต้องเป็น "ใช้งานอยู่"
4. **ถอนสิทธิ์:** กด "ถอนบทบาท" → dialog ยืนยัน → ผู้ใช้ที่ล็อกอินอยู่เสียเมนูทันทีหลังโหลดหน้าใหม่
5. **Role ที่มีสิทธิ์อนุมัติ** (เช่น `Test Estimate Reviewer`) ต้องขึ้น "ต้องมีผู้ตรวจอนุมัติ" และสร้างเป็นคำขอ; ผู้ขอกดอนุมัติเองไม่ได้ (ข้อความอธิบาย) ผู้ตรวจอีกคนอนุมัติได้ที่ `/th/settings/role-requests`
6. **ผู้ดูแลคนสุดท้าย:** ลองปิด Membership หรือถอน Role ของผู้ดูแลที่เหลือคนเดียว ต้องถูกปฏิเสธพร้อมคำอธิบาย
7. เพิ่มผู้ใช้ซ้ำด้วยอีเมลเดิม (ตัวพิมพ์เล็ก/ใหญ่ต่างกัน) ต้องได้ข้อความ "มีผู้ใช้ที่ใช้อีเมลนี้อยู่ในระบบแล้ว"

## 3. Health endpoints

```bash
curl -i http://localhost:5005/health/live    # 200 {"status":"Healthy","checks":{}}
curl -i http://localhost:5005/health/ready   # 200 พร้อม checks.database=Healthy
make stop     # หยุด PostgreSQL และ Emulator (ไม่ลบข้อมูล) แล้วเรียก /health/ready ซ้ำ ต้องได้ 503
make up       # เปิดกลับ แล้ว /health/ready ต้องกลับเป็น 200
```

## ผลอัตโนมัติของ branch นี้

ดู [Commercial Quotation Verification](commercial-quotation-verification.md), [Identity Administration Verification](identity-administration-verification.md) และ [Observability](../06-operations/observability.md). Branch นี้เป็น branch รวมเพื่อทดสอบเท่านั้น ไม่ใช่ตัวแทน PR; แต่ละ PR ยังรีวิวแยก

## เก็บกวาด

```bash
make down    # หยุดและลบ container ของ compose (ข้อมูลใน volume ยังอยู่)
```
