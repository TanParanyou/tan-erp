# Backup and Restore (สำรองและกู้คืน)

**สถานะ:** Accepted Principle; ค่าเวลาเป็น Draft

## ขอบเขต

- PostgreSQL database
- File/object storage และ metadata
- Configuration ที่สร้างใหม่ไม่ได้ง่าย
- Encryption keys/secret recovery ตามนโยบายผู้ให้บริการ

## Runbook ขั้นต่ำ

1. ระบุ Incident และจุดเวลาที่ต้องกู้
2. ป้องกันการเขียนเพิ่มเมื่อจำเป็น
3. เลือก Backup ที่ผ่าน Integrity Check
4. Restore ในพื้นที่แยก
5. ตรวจ Schema, Record counts, critical documents และ file links
6. ให้ Business Owner ยืนยัน Critical Flow
7. Cut over หรือ Export ข้อมูลที่กู้
8. บันทึกเวลา ผลกระทบ และบทเรียน

RPO (ข้อมูลที่ยอมเสียได้) และ RTO (เวลาที่กู้ได้) ต้องได้รับการอนุมัติจากธุรกิจก่อนออกแบบ Production Backup Schedule การมี Backup โดยไม่เคย Restore Test ไม่ถือว่าพร้อม

## Local database restore rehearsal — 2026-09-30

**ผล:** ผ่านการสำรองและกู้คืน PostgreSQL ของ development local ที่ code commit `d77410e`. เป็นหลักฐานด้านฐานข้อมูลเท่านั้น; ยังไม่ปิด Release Readiness ด้านการกู้ระบบทั้งหมด.

ต้นทางคือ `tan-erp-postgres` ของ Compose ซึ่งมีข้อมูล demo/E2E และข้อมูล development เดิม. ไม่มีการรับรองว่าฐานนี้เป็น sanitized legacy copy หรือข้อมูล pilot ที่อนุมัติ. หยุด API/frontend ที่ใช้ทดสอบก่อนซ้อม และตรวจ hash ต้นทางก่อน/หลังให้ตรงกันเพื่อจับการเขียนระหว่างการตรวจ.

### วิธีซ้อมและตรวจผล

1. ใช้ `pg_dump -Fc --no-owner --no-privileges` ผ่าน Unix socket ภายใน container ต้นทาง โดยอ้าง `POSTGRES_USER`/`POSTGRES_DB` จาก environment ของ container; ไม่พิมพ์ credentials หรือ row payload ออก log.
2. สร้าง PostgreSQL 17 Alpine ชั่วคราวใน container แยก, `--network none`, ไม่เปิด port และใช้ tmpfs สำหรับ data directory. Local trust authentication ใช้เฉพาะ container แยกนี้.
3. ส่ง custom-format archive ผ่าน stdin ไปยัง `pg_restore --no-owner --no-privileges --exit-on-error --single-transaction` ในฐานปลายทางว่าง. Archive อยู่ในหน่วยความจำของตัวซ้อม ไม่เก็บหรือ commit database dump.
4. ตรวจ schema dump ทั้งหมดและข้อมูลทุกตารางตาม manifest ด้านล่าง. `pg_dump` สุ่ม `\restrict`/`\unrestrict` token จึงไม่ใช้ token เป็น schema comparison. PostgreSQL parse CHECK expressions ใหม่จาก `(ARRAY['value'::character varying])::text[]` เป็น `ARRAY[('value'::character varying)::text]`; normalize เฉพาะรูปแบบ cast นี้ก่อนเทียบ. ความต่าง 46 CHECK lines เป็นรูปแบบ cast ดังกล่าว; หลัง normalize schema ตรงกันทั้งหมด.
5. ตรวจ migration history, SQL sequences และ `common.sequence_counters`; `pg_restore` ต้องไม่พบข้อผิดพลาดการสร้าง constraints/indexes หรือโหลดข้อมูล. เทียบ manifest ต้นทางอีกครั้งหลัง restore เพื่อยืนยันว่าต้นทางไม่เปลี่ยน.
6. ลบเฉพาะ container ชั่วคราวที่สร้างเพื่อซ้อมและ tmpfs ของมันหลังตรวจ; เก็บ container/volume ต้นทางไว้. ผลรอบนี้ยืนยันว่าลบปลายทางชั่วคราวแล้ว.

SQL สำหรับ manifest ใช้ได้ทั้งต้นทางและฐานที่กู้ ผ่าน `psql -X -At -v ON_ERROR_STOP=1`. เก็บผลไว้ในพื้นที่ local ที่จำกัดสิทธิ์และเปรียบเทียบโดยไม่เปิดเผย payload. คำสั่ง `\gexec` ต้องตาม SELECT โดยไม่มี semicolon ก่อนหน้า เพื่อไม่ให้พิมพ์ generated SQL ซ้ำ:

```sql
SELECT format(
  'SELECT %L, count(*), md5(coalesce(string_agg(row_to_json(t)::text, E''\n'' ORDER BY row_to_json(t)::text), '''')) FROM %I.%I t;',
  schemaname || '.' || tablename, schemaname, tablename
)
FROM pg_tables
WHERE schemaname NOT IN ('pg_catalog', 'information_schema')
ORDER BY 1
\gexec

SELECT schemaname, sequencename, start_value, min_value, max_value,
       increment_by, cycle, cache_size, last_value
FROM pg_sequences
WHERE schemaname NOT IN ('pg_catalog', 'information_schema')
ORDER BY 1, 2;
```

Row hash ใช้ตรวจความเท่ากันของข้อมูลในรอบซ้อมนี้ ไม่ใช่ cryptographic integrity guarantee. SHA-256 ของ archive ใช้ระบุ archive ที่นำมาทดสอบ. วิธี `string_agg` นี้เหมาะกับฐาน local ขนาดเล็ก; ฐาน Production ต้องเลือกการตรวจที่รองรับปริมาณข้อมูลจริง.

### หลักฐานรอบที่ผ่าน

| รายการ | ผล |
| --- | --- |
| Custom archive | 971,223 bytes; SHA-256 `92fa76e056be7754f6f2b3436cbea901eb645d36ecc215042bdea7c3a45235d0` |
| Schema และข้อมูล | 58 ตาราง, 11,084 แถว; schema หลัง normalize และ row counts/hashes ตรงทุกตาราง |
| Migration history | 41 migrations; ล่าสุด `20260929112020_AddEstimateWorkItemCatalogLink` |
| เอกสารสำคัญ | Estimates 23, revisions 25, calculation snapshots 11, approval requests 11, decisions 4, approval snapshots 2, quotations 4; counts/hashes ตรง |
| Catalog/Survey | Items 75, cost records/reviews อย่างละ 27, survey revisions 31; counts/hashes ตรง |
| เลขที่เอกสาร | SQL sequences 0; `common.sequence_counters` 4 แถวตรงกัน |
| File metadata | `files.uploaded_files` 12 แถวตรงกัน; ยังไม่ได้กู้ file bytes หรือพิสูจน์ file links เปิดได้ |
| เวลา | Restore transaction 0.249 วินาที; dump/create/restore/compare รวม 2.408 วินาที ก่อน cleanup; เป็นการวัด local ขนาดเล็ก ไม่ใช่ RTO ที่รับรอง |
| ความปลอดภัยต้นทาง | Manifest ก่อน/หลังตรงกัน; ไม่แก้ข้อมูลต้นทาง; ลบ container ปลายทางหลังตรวจแล้ว |

### ข้อที่ยังต้องทำก่อน Production

- กู้ file/object bytes ควบคู่ metadata และตรวจเปิดไฟล์สำคัญด้วยสิทธิ์ของผู้ใช้จริง.
- ตรวจ identity provider, configuration และ secret/key recovery ตาม environment ที่เลือก; Auth Emulator ไม่ใช่ Firebase Production recovery.
- ซ้อมบน staging/backup storage จริง รวม encryption, retention, backup failure/alerts และสิทธิ์เข้าถึง backup.
- ให้ Business Owner ตรวจ critical flow หลัง restore และอนุมัติ RPO/RTO, cutover, recovery owner และ Go/No-go.
- แยก sanitized legacy upgrade rehearsal จาก restore rehearsal นี้; การกู้ schema ปัจจุบันไม่พิสูจน์การ upgrade ข้อมูลเก่า.
