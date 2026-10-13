---
status: accepted
---

# ใช้ Browser Print สร้าง PDF ของ Quotation ในรอบแรก

CP-04 ส่ง Quotation ให้ลูกค้าผ่านหน้า Preview ที่ render จาก server projection (`GET /api/v1/estimates/{id}/quotation/document`) แล้วให้ผู้ใช้พิมพ์หรือบันทึกเป็น PDF ด้วย browser (`window.print()` พร้อม print stylesheet A4). เราเลือกทางนี้เพราะ backend ไม่มี PDF library หรือฟอนต์ไทยที่ตรวจสอบกรรมสิทธิ์แล้ว และการเพิ่ม dependency, การเก็บไฟล์ที่ render, version และ hash ของไฟล์ควรตัดสินร่วมกับ Business หลังยืนยัน allowlist, branding และ page size. ข้อเสียคือระบบยังไม่เก็บไฟล์ที่ส่งจริงและ hash ของไฟล์จึงตรวจย้อนหลังไม่ได้ว่าลูกค้าได้รับอะไร แม้เนื้อหา re-render ได้เหมือนเดิมจาก snapshot; ต้องยกระดับเป็น server-side PDF ที่เก็บ artifact + hash ก่อนเปิด External Acceptance (CP-07) หรือเมื่อ Business ต้องการหลักฐานเอกสารที่ส่งออกไป

**อัปเดต:** [ADR 0023](0023-server-side-quotation-pdf-with-questpdf.md) เพิ่ม PDF ฝั่งเซิร์ฟเวอร์ที่เก็บครั้งเดียวพร้อม SHA-256 แล้ว; หน้า preview/print ในข้อนี้ยังใช้เป็น preview ได้ แต่ไม่ใช่ไฟล์ที่เป็นหลักฐาน.
