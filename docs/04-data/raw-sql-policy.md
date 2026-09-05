# Raw SQL Policy (นโยบาย Raw SQL)

**สถานะ:** Accepted

ใช้ EF Core สำหรับ Write/Transaction และใช้ Dapper + Raw SQL สำหรับ Complex Read/Report เมื่อ SQL ทำให้ความถูกต้องหรือประสิทธิภาพชัดเจนกว่า LINQ

## ใช้ Raw SQL เมื่อใด

- Report หรือ Dashboard ที่ Join/Aggregate ซับซ้อน
- ต้องใช้ PostgreSQL-specific feature
- Generated SQL วิเคราะห์หรือ Tune ยาก
- มี Query Plan/Measurement สนับสนุนความจำเป็น

## กฎบังคับ

- SQL อยู่ใน Infrastructure ใกล้ Query Class
- Input ทุกตัวเป็น Parameter; ห้ามต่อ String จาก User Input
- ระบุ Column ห้าม `SELECT *`
- Result Map เป็น Typed DTO ห้ามปล่อย `dynamic`
- Business Query บังคับ `organization_id` จาก Trusted Context
- Pagination มี deterministic `ORDER BY` และ tie-breaker
- รับ `CancellationToken` และกำหนด Timeout
- Raw SQL Write ใน Use Case เดียวกับ EF ต้องใช้ Connection/Transaction เดียวกัน
- ทุก Query มี Integration Test กับ PostgreSQL จริง
- Query สำคัญตรวจ `EXPLAIN (ANALYZE, BUFFERS)` บนข้อมูลปลอดภัยขนาดใกล้จริงก่อน Production

## โครงสร้างอนาคต

```text
Application/Estimation/Estimates/SearchEstimates/
├── IEstimateQueries.cs
└── EstimateListItem.cs

Infrastructure/Persistence/Queries/Estimation/
├── EstimateQueries.cs
└── Sql/SearchEstimates.sql
```

ห้ามสร้าง `ISqlExecutor` ให้ Controller/Application ส่ง SQL อิสระ เพราะทำลาย Boundary, Type Safety และการควบคุม Scope
