# External Acceptance API Contract (ข้อตกลง API ลูกค้ายอมรับใบเสนอราคาผ่านลิงก์)

**สถานะ:** Implemented 2026-10-04 (CP-07). กฎด้านล่างเป็นค่าเริ่มต้นที่ทีมพัฒนาเลือก (ผู้ใช้มอบหมายให้ตัดสินใจ) **รอ Sales/Legal ยืนยัน** (ผู้มีอำนาจลงนาม, ถ้อยคำยินยอม, อายุหลักฐาน/retention). **ภาพลายเซ็นเพียงอย่างเดียวไม่ถือเป็นนโยบายการลงนามที่ธุรกิจอนุมัติ** — ระบบเก็บชื่อผู้ลงนาม/ตำแหน่ง/เวอร์ชันถ้อยคำที่ยินยอม/เวลา/ภาพลายเซ็น (ถ้ามี) เป็นหลักฐานประกอบการยอมรับที่ผูกกับใบเสนอราคาเลขที่นั้น.

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| ตัวตนผู้ยอมรับ | ผู้ถือลิงก์ลับ (bearer link) ที่เจ้าหน้าที่สร้างให้ผู้แทนลูกค้า; **ไม่ตรวจยืนยันตัวตนหรืออำนาจของผู้ลงนาม** นอกจากชื่อ/ตำแหน่งที่ผู้ลงนามกรอกและการยืนยันถ้อยคำ "มีอำนาจลงนามแทนลูกค้า" — เจ้าหน้าที่ระบุชื่อผู้ลงนามที่คาดหวังเป็น hint ได้ (ไม่บังคับให้ตรงกัน) |
| Token | สุ่ม 256-bit (base64url) แสดง **ครั้งเดียว** ตอนสร้าง; DB เก็บเฉพาะ SHA-256 (`token_hash` unique); ไม่ลงใน audit/log ของแอป; หน้า/ตอบกลับสาธารณะส่ง `Cache-Control: no-store` และ `Referrer-Policy: no-referrer` |
| อายุ/เพิกถอน | อายุ 1–30 วัน (ค่าเริ่มต้น 7); เจ้าหน้าที่เพิกถอนได้ (`active → revoked`); ลิงก์หมดอายุเมื่อเกิน `expires_at_utc`; **ใช้ไม่ได้อัตโนมัติ** เมื่อใบเสนอราคาไม่ใช่ `issued` อีก (void/superseded/accepted โดยช่องทางอื่น) ตามกฎ CP-06 |
| ตอบกลับผิดปกติ | token ไม่รู้จัก/รูปแบบผิด/หมดอายุ/เพิกถอน/ใบเสนอราคาถูกแทนที่หรือยกเลิก → **404 `ACCEPTANCE_LINK_UNAVAILABLE` แบบเดียวกันทุกกรณี** (กันการเดา/ไล่ตรวจ token) |
| Rate limit | Fixed window ต่อ IP ไคลเอนต์ **30 ครั้ง/นาที** (ค่า `PublicAcceptance:PermitsPerMinute`) สำหรับ endpoint สาธารณะ → `429`; หมายเหตุ: หลัง reverse proxy ต้องตั้ง Forwarded Headers ให้ถูกต้อง มิฉะนั้นทุกคนใช้โควตาร่วมกัน |
| ข้อมูลที่ลูกค้าเห็น | projection เดียวกับใบเสนอราคาที่ลูกค้าเห็น (CP-04): ไม่มีต้นทุน/มาร์จิน/ข้อมูลภายใน; ทดสอบด้วยการตรวจว่า response ไม่มีคำเช่น `unitCost`, `margin`, `supplier`, `estimateId`, `organizationId` |
| ถ้อยคำยินยอม | ต้องส่ง `consentAccepted=true` และ `consentVersion` ตรงเวอร์ชันปัจจุบัน (`2026-10-v1`) มิฉะนั้น `ACCEPTANCE_CONSENT_REQUIRED`; หลักฐานเก็บเวอร์ชันที่ยินยอม |
| ลายเซ็น | ชื่อ 2–200 ตัวอักษร (บังคับ), ตำแหน่ง ≤100, ภาพลายเซ็น PNG (base64/data URL, ≤150,000 ตัวอักษร, ตรวจ magic bytes) ไม่บังคับ; เก็บภาพ + SHA-256 |
| หลักฐาน | `quotation_acceptance_evidences` (หนึ่งต่อลิงก์ — unique): ชื่อ/ตำแหน่ง/เวอร์ชันยินยอม/ภาพ+hash/**hash ของ IP (ผูกกับลิงก์)**/User-Agent (≤200)/เวลา; ไม่เก็บ IP ดิบ; ไม่ลงในข้อความ audit |
| การยอมรับ | ใช้ธุรกรรมยอมรับเดิม (`AcceptQuotationAsync`) ด้วย key ผูกกับลิงก์ จึง replay ได้, เปลี่ยน Opportunity เป็น Won และบันทึกประวัติ (ผู้กระทำ = ผู้สร้างลิงก์, หมายเหตุระบุว่ายอมรับโดยผู้แทนลูกค้าผ่านลิงก์) และส่ง **expectedQuotationId** เพื่อกันยอมรับฉบับใหม่ผิดตัวหากมี amend แทรก; ธุรกรรมยอมรับกับการบันทึกหลักฐานเป็นสองขั้น แต่ retry ปลอดภัย (ขั้นแรก replay แล้วบันทึกหลักฐานต่อ) |
| ยอมรับซ้ำ | ส่งซ้ำ/refresh บนลิงก์ที่ยอมรับแล้วคืนผลเดิม `200` ไม่บันทึกซ้ำ (ไม่ใช้ข้อมูลของคำขอที่สอง) |

## Endpoints

Staff (ต้อง `Authorization` + `X-Membership-Id`):

| Action | Method/Path | Permission |
| --- | --- | --- |
| สร้างลิงก์ (คืน token ครั้งเดียว) | `POST /api/v1/quotations/{id}/acceptance-links` `{lifetimeDays?, signerHint?}` → `201 {link, token, publicPath}` | `quotations.share` |
| รายการลิงก์/หลักฐาน | `GET /api/v1/quotations/{id}/acceptance-links` | `quotations.read` |
| เพิกถอน | `POST /api/v1/acceptance-links/{id}/revoke` | `quotations.share` |

Public (ไม่ต้อง login; rate limited):

| Action | Method/Path |
| --- | --- |
| ดูเอกสาร | `GET /api/public/v1/quotation-acceptance/{token}?locale=th|en` → `{status, expiresAtUtc, signerHint, consentVersion, acceptedAtUtc, document}` |
| ยอมรับ | `POST /api/public/v1/quotation-acceptance/{token}/accept` `{signerName, signerRole?, consentAccepted, consentVersion, signatureImage?}` → `200 {status, acceptedAtUtc, quotationNumber, evidence}` |

หน้าลูกค้า: `/{locale}/accept/{token}` (อยู่นอก ERP layout ไม่มี sign-in gate/เมนู).

## Errors

`404`: `ACCEPTANCE_LINK_UNAVAILABLE` (public ทุกกรณีที่ใช้ไม่ได้). `409`: `ACCEPTANCE_LINK_INVALID_STATE`, `ACCEPTANCE_CONFLICT`, `QUOTATION_INVALID_STATE` (สร้างลิงก์ให้ใบที่ไม่ใช่ issued). `422`: `ACCEPTANCE_LINK_LIFETIME_INVALID`, `ACCEPTANCE_SUBMISSION_INVALID`, `ACCEPTANCE_CONSENT_REQUIRED`. `429` เกินโควตา.

## Data (migration `AddQuotationAcceptance`)

`commercial.quotation_acceptance_links` (unique `token_hash`; check status/lifetime), `commercial.quotation_acceptance_evidences` (unique `link_id`). Audit: `quotation-acceptance-link.created|revoked|accepted` (ไม่มี token/ชื่อผู้ลงนาม).

## Threat notes

ลิงก์รั่ว = ผู้ถือลิงก์ยอมรับได้ → จึงมีอายุสั้น เพิกถอนได้ และใช้ได้ครั้งเดียว; ไม่มี MFA/OTP/ตรวจอีเมลผู้รับในรอบนี้; ภาพลายเซ็นปลอมได้ง่าย — ใช้เป็นหลักฐานเสริมเท่านั้น; ความรับผิดทางกฎหมายของลายเซ็นอิเล็กทรอนิกส์/ระยะเก็บหลักฐานต้องให้ Legal ยืนยัน.
