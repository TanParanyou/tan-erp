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
