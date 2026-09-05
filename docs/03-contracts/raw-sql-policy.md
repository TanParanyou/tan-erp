# Raw SQL Contract (ข้อตกลงการใช้ Raw SQL)

แหล่งอ้างอิงหลักอยู่ที่ [Raw SQL Policy](../04-data/raw-sql-policy.md) เอกสารนี้สรุป Contract ที่ Layer อื่นมองเห็น

- Application ประกาศ Typed Query Interface ตาม Use Case
- Infrastructure เป็นเจ้าของ SQL และ Mapping
- Controller/Frontend ไม่รับ SQL และไม่รู้ว่าภายในใช้ EF Core หรือ Dapper
- Criteria ต้องมี Trusted Organization/Scope Context
- Result เป็น Typed DTO และไม่คืน `dynamic`
- Error จาก Database ถูกแปลงเป็น Application Result หรือ Problem Details ที่ปลอดภัย

ตัวอย่างเชิงแนวคิด: `SearchEstimates(criteria, userScope)` คืน `PagedResult<EstimateListItem>` ไม่เปิด Interface แบบ `ExecuteSql(string sql)`
