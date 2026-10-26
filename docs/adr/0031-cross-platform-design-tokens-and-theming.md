---
status: accepted
---

> สถานะ implementation: Not started — เอกสารตัดสินใจเท่านั้น (2026-10-26). มาตรฐานการใช้งานอยู่ที่ [design.md หมวด 7](../../design.md)

# Design Tokens ชุดเดียวสำหรับเว็บและแอป และขอบเขตการปรับธีม

เว็บมี CSS variables และโหมดมืด (`data-theme="dark"`) อยู่แล้ว แต่ค่าบางส่วนถูกเขียนตายตัว (`border-radius: 0 !important` หลายจุดใน `erp-theme.css`, hex ใน component เช่น `erp-shell.tsx`). เมื่อมีแอป Flutter ([ADR 0030](0030-flutter-field-companion-app.md)) ต้องมีแหล่งความจริงเดียว ไม่เช่นนั้นสี/ขอบ/ขนาดจะเพี้ยนระหว่างสองแพลตฟอร์ม.

## ตัดสินใจ

1. **แหล่งความจริงเดียว `design-tokens/`** ที่ root ในรูปแบบ W3C Design Tokens (DTCG JSON) แบ่ง 3 ชั้น:
   - **primitive** — ค่าดิบ (`navy.900 = #0B3056`, `space.4 = 16`)
   - **semantic** — ความหมาย (`color.action.primary`, `color.surface.default`, `border.width.hairline`, `radius.control`) แยกค่า light/dark
   - **component** — เฉพาะเมื่อจำเป็น (`button.height.default`)
2. **Build ด้วย Style Dictionary** (OSS) เป็น `frontend/src/styles/tokens.css` (CSS variables ที่ Tailwind อ้างถึง) และ `mobile/lib/core/theme/tokens.g.dart` (ใช้สร้าง `ThemeData` + `ThemeExtension`). ไฟล์ที่ generate ห้ามแก้มือ.
3. **Component ใช้ได้เฉพาะ semantic/component token** — ห้าม hex, ห้ามตัวเลข radius/border ตรง. บังคับด้วย lint: เว็บ (ESLint/Stylelint rule ห้าม hex และ `rounded-*` ที่ไม่ใช่ token), Flutter (`custom_lint` ห้าม `Color(0x…)`/`BorderRadius.circular(...)` นอกโฟลเดอร์ theme).
4. **ขอบเขตการปรับ (ตามแนวปฏิบัติ SaaS ระดับ enterprise):**

   | ใคร | ปรับได้ | เก็บที่ |
   |---|---|---|
   | ผู้ใช้ | โหมด สว่าง/มืด/ตามเครื่อง, ความแน่น ปกติ/กะทัดรัด, ขนาดตัวอักษร 90–130% | preference ของ User ใน Backend (ซิงก์เว็บ+แอป) |
   | Admin องค์กร | โลโก้, สีหลักแบรนด์ 1 สี | ค่าตั้งขององค์กรใน Backend |
   | ทีมพัฒนา | รูปทรงมุม, ความหนาเส้นขอบ, ระยะห่าง, ฟอนต์, สเกลสี | token ใน repo เท่านั้น |

   ผู้ใช้และ admin **ไม่สามารถ** เปลี่ยนมุม/เส้นขอบ/ระยะห่าง เพื่อไม่ให้เกิดชุดค่าผสมที่ไม่ได้ทดสอบ.
5. **สีแบรนด์ต้องผ่าน WCAG AA ก่อนบันทึก:** ข้อความขาวบนสีหลัก ≥ 4.5:1 และสีหลักบนพื้น canvas ≥ 3:1 — ตรวจที่ Backend (validation) และแสดงผลสดใน UI admin; สีโหมดมืดคำนวณจากสีแบรนด์และต้องผ่านเกณฑ์เดียวกัน.
6. **ค่าเริ่มต้นคือ Atelier Architectural Navy Sharp** (radius 0, navy `#0B3056`, hairline 1px) — หน้าตาเดิมไม่เปลี่ยน; สิ่งที่เปลี่ยนคือวิธีอ้างค่า.
7. **ปุ่มกดบนมือถือสูง ≥ 44px เสมอ** แม้โหมดกะทัดรัด.

## ทางเลือกที่ไม่เลือก

- **ให้ผู้ใช้ปรับรูปทรง/เส้นขอบเอง:** จำนวนชุดค่าผสมโตเร็ว ทดสอบไม่ครบ หน้าจอพังง่าย และเสียเอกลักษณ์ Atelier.
- **ดูแลค่า token แยกเว็บกับแอป:** เพี้ยนแน่นอนเมื่อเวลาผ่านไป.
- **Theme เป็น JSON ที่ admin แก้ได้ทุกค่า (white-label เต็มรูปแบบ):** เกินความต้องการของธุรกิจ และ validate ยาก.

## ข้อเสียที่ยอมรับ

- ต้องรื้อค่าที่เขียนตายตัวในเว็บ (radius `!important`, hex ใน component, inline style) ก่อนจะเปลี่ยน token ได้ผลทั่วระบบ — ทำเป็นงานแยกในเฟส 0.
- มีขั้นตอน build token เพิ่มใน dev workflow และ CI.
- Admin เลือกสีแบรนด์ได้จำกัด (สีที่ไม่ผ่าน contrast บันทึกไม่ได้).

## วิธีเปลี่ยนในภายหลัง

- เปลี่ยนเอกลักษณ์ดีไซน์ (เช่น radius 0 → 4px): แก้ token ค่าเดียว แล้ว build ใหม่ — เว็บและแอปเปลี่ยนพร้อมกัน; ต้องอัปเดต design.md ในรอบเดียวกัน.
- เพิ่มแกนให้ผู้ใช้ปรับ: ต้องมี ADR ใหม่พร้อมแผนทดสอบชุดค่าผสม.
