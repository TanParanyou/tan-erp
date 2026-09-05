# System Context (ภาพรวมระบบและผู้เกี่ยวข้อง)

**สถานะ:** Accepted

```text
Customers / Staff / Managers
            │
            ▼
      tan-erp Web App
            │
            ▼
        tan-erp API
       ┌────┼────────┐
       ▼    ▼        ▼
 PostgreSQL Firebase File Storage
 business   identity documents/images
```

## Trust Boundaries

- Browser เป็น Untrusted Client: ข้อมูลและ Permission จาก Browser ต้องตรวจซ้ำที่ API
- API เป็น Business Authority: ตรวจ Identity, RBAC, Scope, Validation และ Workflow
- PostgreSQL เป็นแหล่งข้อมูลธุรกิจ สิทธิ์ Audit และ Transaction
- Firebase ยืนยันตัวตนเท่านั้น ไม่เก็บ Business Permission
- File Storage เก็บ Binary; Metadata และสิทธิ์การเข้าถึงอยู่ในระบบ

## Deployment Direction

เริ่มเป็น Modular Monolith หนึ่ง Backend และหนึ่ง Frontend เพื่อลดภาระระบบกระจาย Module Boundary ต้องชัดพอให้แยก Service ได้ในอนาคตเมื่อมีทีม ปริมาณงาน หรือการ Scale ที่พิสูจน์ได้
