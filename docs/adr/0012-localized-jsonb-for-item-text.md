---
status: accepted
---

# Item-owned Master Data ใช้ JSONB เฉพาะข้อความหลายภาษา

Item, Category, Brand, Alias และ Unit เก็บ `name`/`description` ที่ต้องแสดงหลายภาษาเป็น JSONB รูปทรงคงที่ `{ "th": string, "en"?: string }` ผ่าน Localized Text Contract กลาง พร้อม Database/Application Validation และ Expression Index ตามภาษาที่ค้นหาจริง แทนการเพิ่ม Column ต่อภาษา; Capability, Lifecycle, Scope, Unit identity และข้อมูลที่มีผลต่อการคำนวณยังคงเป็น Relational Typed Fields ตาม ADR 0008 ไม่ใช้ JSONB เป็น Generic Master Document แนวทางนี้ทำให้ master data ใช้รูปแบบภาษาเดียวกันและลด Schema Migration เมื่อเพิ่มภาษา แลกกับการต้องมี Validation และ Index ที่เจาะจงชัดเจน
