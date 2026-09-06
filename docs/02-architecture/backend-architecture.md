# Backend Architecture (สถาปัตยกรรมหลังบ้าน)

**สถานะ:** Accepted

ใช้ ASP.NET Core + EF Core + PostgreSQL แบบ Clean Architecture สี่ Project และ Feature Folders ภายใน Modular Monolith

```text
TanErp.Api ───────► TanErp.Application ───────► TanErp.Domain
     │                       ▲                       ▲
     └── composition ────────┴── TanErp.Infrastructure
                                  ├─ EF Core
                                  ├─ Dapper / Raw SQL
                                  ├─ Firebase adapter
                                  └─ File/notification adapters
```

## ความรับผิดชอบ

- `Domain`: Entity, Value Object, Aggregate และกฎธุรกิจที่ไม่รู้จัก Framework
- `Application`: Use Case, Command/Query, Validation, DTO และ Interface
- `Infrastructure`: Database, Raw SQL, Identity/File Adapter และ External Integration
- `Api`: HTTP Contract, Authentication/Authorization, Problem Details และ Composition Root

## รูปแบบ Feature

```text
TanErp.Application/Estimation/Estimates/CreateEstimate/
├── CreateEstimateCommand.cs
├── CreateEstimateHandler.cs
├── CreateEstimateValidator.cs
└── CreateEstimateResult.cs
```

Controller ต้องบางและเรียก Use Case โดยตรง ไม่สร้าง Giant Service หรือ Generic Repository ค่าเริ่มต้น EF Core `DbContext` ทำหน้าที่ Unit of Work

## Data Access

- Write และ Transaction: EF Core
- Read ปกติ: EF Core LINQ
- Read/Report ซับซ้อนที่มีเหตุผล: Dapper + Parameterized Raw SQL ใน Infrastructure

รายละเอียดเดิมที่อนุมัติแล้วเก็บใน [Backend Architecture Design](../superpowers/specs/2026-09-05-tan-erp-backend-architecture-design.md)

## โครงสร้างสำหรับเริ่ม Foundation

**สถานะส่วนนี้:** Draft สำหรับตรวจทานก่อนสร้าง Application จริง โฟลเดอร์ต่อไปนี้เป็นแบบแปลน ไม่ใช่ไฟล์ที่มีอยู่แล้ว

ผู้อ่านคือทีม BE/FE ที่ต้องแบ่งงานและระบุว่าโค้ดแต่ละส่วนควรอยู่ที่ใด ใช้ `backend/` เป็นขอบเขต Build ของ BE และ `frontend/` เป็นขอบเขต Build ของ FE ภายใน Repository เดิม

```text
backend/
├── TanErp.slnx
├── global.json                     # ตรึง SDK เมื่อเริ่ม Implementation
├── Directory.Build.props           # กฎ Build ร่วม
├── Directory.Packages.props        # รุ่น Dependency กลาง
├── src/
│   ├── TanErp.Domain/
│   │   ├── Common/                 # ชนิดพื้นฐานที่ใช้ร่วมจริง
│   │   ├── IdentityAccess/         # Role, Permission, Scope
│   │   └── Organization/           # Organization, Branch, Membership
│   ├── TanErp.Application/
│   │   ├── Common/Abstractions/    # Current user, Clock และ Persistence ports
│   │   ├── IdentityAccess/CurrentUser/GetCurrentUser/
│   │   └── Organization/
│   ├── TanErp.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/     # แบ่งโฟลเดอร์ตาม Module
│   │   │   └── Migrations/
│   │   ├── Identity/              # Firebase และ Internal user mapping
│   │   ├── Audit/                 # บันทึกการเปลี่ยนข้อมูลสำคัญ
│   │   └── DependencyInjection.cs
│   └── TanErp.Api/
│       ├── Controllers/
│       ├── Contracts/IdentityAccess/
│       ├── Authorization/         # Permission และ Resource scope policies
│       ├── Middleware/            # Error mapping และ Trace ID
│       ├── Resources/             # ข้อความ Error ภาษาไทย/อังกฤษ
│       ├── DependencyInjection/
│       └── Program.cs
└── tests/
    ├── TanErp.UnitTests/
    ├── TanErp.IntegrationTests/
    └── TanErp.ArchitectureTests/
```

`Api` ประกอบ Dependency จาก `Infrastructure` ที่ Composition Root เท่านั้น Controller เรียก Application Handler ผ่าน Contract ที่ระบุไว้ ส่วน `Application` อ้าง `Domain` และ .NET Base Libraries; Interface ต้องไม่เปิดเผย `DbContext`, `DbSet`, `NpgsqlConnection` หรือชนิดของ Framework ให้ Use Case ผูกติดกับ Infrastructure

เมื่อเพิ่มงานธุรกิจ ให้เพิ่มโฟลเดอร์ตาม [Module Boundaries](module-boundaries.md) เช่น `CRM/Customers/`, `ItemMaster/Items/`, `Estimation/Estimates/` และ `Commercial/Quotations/` ภายในสี่ Project เดิม ตัวอย่างในเอกสาร Design เดิมที่ใช้ `Estimates/` ระดับบนให้ตีความเป็น Feature ภายใต้ `Estimation` ตามตัวอย่างปัจจุบัน ไม่สร้าง Module ซ้ำสองตำแหน่ง

เพิ่ม `Persistence/Queries/<Module>/` พร้อม Typed Query Interface เมื่อมี Read ที่เหมาะกับ Raw SQL ตาม [Raw SQL Policy](../04-data/raw-sql-policy.md) ส่วน Files, Notifications และ Background Processing เพิ่มเมื่อ Slice ที่กำลังทำต้องใช้

## จุดเชื่อมต่อกับ FE

Foundation เริ่มจาก Current User use case: ตรวจ Firebase identity → Resolve ผู้ใช้ภายใน → ตรวจ Membership → คืนบริบทองค์กรและ Effective Permissions ให้ FE ก่อนเข้าหน้าระบบ โดยต้องกำหนด Endpoint และ Payload ใน [API Conventions](../03-contracts/api-conventions.md) ก่อนลงมือทำทั้งสองฝั่ง

OpenAPI ที่ API สร้างเป็นต้นทางของ Type ฝั่ง FE; HTTP Contract อยู่ใน `Api/Contracts` และแปลงจาก Application Result โดยไม่เปิดเผย Domain Entity กฎ Token, Permission และ Scope ใช้ [RBAC](../03-contracts/rbac.md) เป็นแหล่งอ้างอิงหลัก
