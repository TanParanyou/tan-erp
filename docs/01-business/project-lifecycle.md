# Project Lifecycle (วงจรโครงการ)

**สถานะ:** Future

## State ที่แนะนำ

```text
Planned → Active → On Hold → Active → Ready for Handover → Completed
                 └──────────────→ Cancelled
```

## จุดควบคุม

- `Planned`: สร้างจาก Quotation ที่ยืนยันแล้ว แต่ยังไม่เริ่มปฏิบัติงาน
- `Active`: มีผู้รับผิดชอบ แผนเวลา และ Baseline Budget
- `On Hold`: หยุดชั่วคราวพร้อมเหตุผลและผู้อนุมัติ
- `Ready for Handover`: งานและรายการตรวจรับสำคัญครบ
- `Completed`: ส่งมอบและบันทึกเงื่อนไขรับประกันแล้ว
- `Cancelled`: เก็บเหตุผล ผลกระทบทางการเงิน และงานค้าง

การเปลี่ยน Scope หลังเริ่มงานต้องผ่าน Change Order ใน Phase Project Control เพื่อแยกจาก Estimate ก่อนขาย
