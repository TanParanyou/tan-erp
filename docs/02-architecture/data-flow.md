# Data Flow (การไหลของข้อมูล)

**สถานะ:** Accepted

## Write Flow

```text
User Action
  → Frontend validation
  → API request + Firebase ID token + Accept-Language
  → Authentication
  → RBAC + Scope
  → Application command
  → Domain rules
  → EF Core transaction
  → PostgreSQL + Audit/Outbox
  → Typed response
```

## Read Flow

```text
Screen/Report
  → Typed query criteria
  → RBAC + Scope
  → Application query interface
  → EF Core LINQ OR Dapper/Raw SQL
  → Typed DTO
  → Localized UI
```

## Error Flow

```text
Domain/Validation/Authorization failure
  → Stable error code
  → localized RFC 9457 Problem Details
  → Frontend ApiError
  → field/form/page feedback + traceId
```

## Sensitive Boundaries

- `organization_id` มาจาก Trusted User Context ไม่รับค่าจากหน้าจอมาเชื่อโดยตรง
- Raw SQL ต้องกรอง Organization Scope ด้วยตนเอง เพราะ EF Global Filter ไม่ทำงานกับ Dapper
- File Download ต้องตรวจ Permission และ Resource Scope ก่อนออก URL หรือ Stream
- Log ห้ามเก็บ Token, Password, Full Personal Data หรือ Raw Request ที่อ่อนไหว
