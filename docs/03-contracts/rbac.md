# RBAC and Scope (บทบาท สิทธิ์ และขอบเขตข้อมูล)

**สถานะ:** Accepted

โมเดลการตัดสินใจคือ **User → Membership → Role → Permission + Scope → Resource** โดย Backend ตรวจทุก Request

## Permission Naming

ใช้รูปแบบ `resource.action` เช่น:

- `estimates.read`
- `estimates.create`
- `estimates.submit`
- `estimates.approve`
- `quotations.issue`
- `roles.manage`

## Scope

| Scope | ความหมาย |
| --- | --- |
| Organization | Resource ทั้งองค์กร |
| Branch | เฉพาะสาขาที่ได้รับมอบหมาย |
| Project | เฉพาะโครงการที่ได้รับมอบหมาย |
| Own | เฉพาะ Resource ที่ตนเป็นเจ้าของตามกฎธุรกิจ |

## Decision Flow

```text
Valid identity?
  No → 401
  Yes → active membership?
    No → 403
    Yes → permission exists?
      No → 403
      Yes → resource in scope?
        No → 404
        Yes → business rule/maker-checker passes?
          No → 422 or 403
          Yes → allow + audit when sensitive
```

## กฎสำคัญ

- Resource นอก Organization Scope ตอบ 404 เพื่อลดการเปิดเผยว่ามีข้อมูลอยู่
- Frontend ซ่อนหรือ Disable Control เพื่อ UX เท่านั้น
- Role เป็นข้อมูลปรับได้ แต่ Permission Key เป็น Contract ที่ควบคุมเวอร์ชัน
- การแก้ Role Assignment และ Approval Permission ต้องมี Audit Trail
- Sensitive Action ต้องประเมิน Maker–Checker เพิ่มจาก Permission ปกติ
