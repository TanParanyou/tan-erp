# Backend Agent Guidelines (คำแนะนำสำหรับ Agent ฝั่ง Backend)

Backend ของ `tan-erp` ยึดหลัก Clean Architecture แบ่งเป็นสี่ Projects หลักภายใต้ `backend/`:
1. `TanErp.Domain` — หัวใจธุรกิจ, Enterprise Entities, Invariants (ห้ามขึ้นกับ Framework ใดๆ)
2. `TanErp.Application` — Use Cases, Feature Folders, Command/Query Handlers, Validation, Interfaces
3. `TanErp.Infrastructure` — Persistence (EF Core / Npgsql), External Services, Identity Verification (Firebase Admin)
4. `TanErp.Api` — HTTP Controllers, Middleware, Auth Handlers, OpenAPI, Problem Details

## กฎเหล็กในการพัฒนา (Strict Guardrails)

1. **Feature Folders:** จัดโครงสร้างใน `Application` ตาม Business Feature เช่น `IdentityAccess/CurrentUser/GetCurrentUser/` แทนการแบ่งตามชนิดไฟล์ทางเทคนิค
2. **Thin Controllers:** Controller ใน `Api` มีหน้าที่เพียงรับ HTTP Request, ตรวจ Model State ขั้นต้น, ส่งต่อให้ Application Handler และแปลง `Result` เป็น HTTP Response
3. **EF Core สำหรับ Writes:** การเขียนและการจัดการ Transaction เป็นกรรมสิทธิ์ของ EF Core ผ่าน `AppDbContext`
4. **ห้ามใช้ Generic Repository:** ห้ามสร้าง Generic Repository หรือ Repository Pattern ครอบ EF Core ซ้ำซ้อน ให้ใช้ DbSet และ Application Abstractions ที่เจาะจงกับ Use Case
5. **ห้ามตรวจชื่อ Role (No Role-name checks):** ระบบอนุญาตให้ตรวจเฉพาะ `Permission` Key และ `Scope` (เช่น `organization`, `branch`) จาก PostgreSQL เท่านั้น ห้ามเขียนโค้ดตรวจสอบชื่อ Role (เช่น `User.IsInRole("Admin")`) หรืออิง Role Claims จาก Token
6. **ห้ามบันทึก Secret ลง Log (No Secret Logging):** ห้ามบันทึก Bearer token, Password, Firebase Service Account JSON, API Key หรือข้อมูลส่วนบุคคลที่มีความอ่อนไหวลงใน Logger เป็นอันขาด
7. **Localized Problem Details (RFC 9457):** ข้อผิดพลาดทั้งหมดต้องส่งกลับในรูปแบบ RFC 9457 Problem Details พร้อม stable `code`, `traceId` และข้อความที่แปลตาม `Accept-Language` (ไทยเป็นค่าเริ่มต้น, อังกฤษเป็นทางเลือก) ผ่านไฟล์ `.resx`
