# design-tokens

แหล่งความจริงเดียวของค่าดีไซน์ (สี, radius, เส้นขอบ, ระยะห่าง, ความสูง control, ตัวอักษร) ทั้งเว็บและแอป Flutter — รูปแบบ W3C DTCG (`$value`, `$type`, `$description`). เหตุผล: [ADR 0031](../docs/adr/0031-cross-platform-design-tokens-and-theming.md), มาตรฐาน: `design.md` หมวด 3, 7, 8.

## ชั้นของ token

| ชั้น | โฟลเดอร์ | ตัวอย่าง |
| :--- | :--- | :--- |
| primitive (ค่าดิบ) | `primitive/` | `color.navy.900`, `color.slate.300`, `space.4` |
| semantic (ความหมาย) | `semantic/` | `color.action.primary`, `color.surface.default`, `color.text.muted`, `radius.control` |
| theme override (dark) | `themes/dark.json` | เฉพาะ token ที่ค่าต่างจาก light (path เดียวกับ semantic) |

แต่ละ token มี `$extensions.tanerp.css` = ชื่อตัวแปร CSS (`--erp-*`) ที่เว็บใช้อยู่เดิม — ห้ามเปลี่ยนชื่อโดยไม่แก้เว็บ

## กฎ

- Component ใช้ได้เฉพาะ **semantic token** (ห้าม hex, ห้ามตัวเลข radius/border ตรง); primitive ใช้อ้างอิงใน semantic เท่านั้น
- รูปทรงคมคือค่าของ token (`radius.control = 0px`) ไม่ใช่ `!important`

## เพิ่ม/แก้ token

1. แก้หรือเพิ่มใน `primitive/` หรือ `semantic/` (ใส่ `$type`, `$value`, `$description` ภาษาอังกฤษสั้นๆ และ `$extensions.tanerp.css`)
2. ถ้าโหมดมืดต่างออกไป เพิ่ม path เดียวกันใน `themes/dark.json`
3. `npm run tokens:build` แล้ว commit ไฟล์ที่ generate; อัปเดต `design.md` ในรอบเดียวกันถ้าเปลี่ยนค่ามาตรฐาน

## Build และผลลัพธ์

`npm run tokens:build` (Style Dictionary, config: `style-dictionary.config.mjs`; output ซ้ำได้เหมือนเดิม ไม่มี timestamp)

- `frontend/src/styles/tokens.css` — `:root { … }` (light) และ `[data-theme="dark"] { … }` (เฉพาะค่าที่ต่าง)
- `mobile/lib/core/theme/tokens.g.dart` — `ErpTokensLight`, `ErpTokensDark` (สี), `ErpTokensScale` (ค่าตัวเลขเป็น double)

ไฟล์ที่ generate ห้ามแก้มือ
