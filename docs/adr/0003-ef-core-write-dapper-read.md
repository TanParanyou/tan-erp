---
status: accepted
---

# EF Core สำหรับ Write และ Dapper สำหรับ Complex Read

EF Core เป็นเจ้าของ Write, Aggregate และ Transaction ส่วน Dapper/Parameterized Raw SQL ใช้เฉพาะ Complex Read/Report ใน Infrastructure เมื่อให้ความชัดเจนหรือประสิทธิภาพดีกว่า LINQ วิธีนี้รักษา Business Invariant และ Developer Productivity โดยยังเปิดทาง Tune Query สำคัญ; ผลที่ตามมาคือ Raw SQL ต้องบังคับ Organization Scope และมี PostgreSQL Integration Test เอง
