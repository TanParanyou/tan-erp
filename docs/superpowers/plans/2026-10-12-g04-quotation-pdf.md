# G-04 Server-side Quotation PDF + Artifact Hash Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking. Read `AGENTS.md`, `design.md`, `CONTEXT.md` และ `.agents/skills/building-erp-apis/SKILL.md` before Task 2 (`building-erp-forms` อ่านก่อน Task 9). Reply to the user in Thai; code, identifiers and code comments stay in English. macOS: use BSD `sed -i ''` (never GNU `sed -i`). การแก้ไฟล์เดิมทุกจุดใช้ Edit tool ตาม old_string/new_string ที่ให้ไว้ (ต้องตรงทุกตัวอักษรและมีครั้งเดียว) — ถ้า old_string ไม่ตรงให้หยุดและรายงาน ห้ามเดา.

**กฎของผู้ใช้ที่บังคับทุก Task (สำคัญ):** implementer **ห้ามรันชุดทดสอบใด ๆ** — ไม่มี `dotnet test`, ไม่มี `vitest`/`npx vitest`, ไม่มี mutation check, ไม่เปิดเบราว์เซอร์ — แต่ **ต้องเขียน test ทุกตัวที่แผนระบุ**. การตรวจของทุก Task คือ **compile-only**: backend `dotnet build backend/TanErp.slnx --nologo -v q` (ต้อง `0 Warning(s)` `0 Error(s)`; `TreatWarningsAsErrors` เปิดอยู่ และโปรเจกต์ test อยู่ใน solution จึงถูก compile ด้วย — รวมถึง analyzer ของ xUnit); frontend `cd frontend && npx tsc --noEmit`, `npm run lint`, ตรวจ key parity th/en, และ `npm run build` ครั้งเดียวใน Task 9. ไม่มีขั้น RED/GREEN ที่ต้องรัน. `dotnet ef migrations add/has-pending-model-changes`, สคริปต์ regenerate OpenAPI และ `npm run generate:api` เป็นเครื่องมือ build ไม่ใช่การรัน test จึงทำได้; **ห้าม `dotnet ef database update`** (controller เป็นผู้ apply migration).

**Goal:** ให้ใบเสนอราคาที่ออกให้ลูกค้ามี "ไฟล์ที่ส่งจริง" ที่ตรวจย้อนหลังได้ — ตอนนี้ระบบมีแค่หน้า preview ที่ render ใหม่จาก snapshot ทุกครั้งและให้ browser พิมพ์ ([ADR 0016](../../adr/0016-browser-print-for-quotation-pdf.md)) จึงไม่รู้ว่าลูกค้าได้รับ/ยอมรับอะไรเมื่อข้อมูลองค์กรเปลี่ยนภายหลัง. slice นี้สร้าง PDF ครั้งเดียวตอนออก เก็บเป็นไฟล์ที่เปลี่ยนไม่ได้พร้อม SHA-256 ผูกกับ quotation และให้หน้าลูกค้า CP-07 แสดง/ยืนยัน hash ของไฟล์เดียวกัน.

**Architecture:** (1) Application: `IQuotationDocumentRenderer` + `QuotationPdfModel` (allowlist ที่ไม่มีต้นทุน/มาร์จิน) + ports `IQuotationDocumentWriter`/`IQuotationDocumentStore` + `QuotationDocumentHandler`. (2) Infrastructure (`Documents/`): QuestPDF renderer + ฟอนต์ Sarabun ฝังใน assembly, `QuotationPdfModelBuilder` (อ่านเฉพาะคอลัมน์ฝั่งขาย), `QuotationDocumentWriter` (render+เก็บ blob+เพิ่มแถวใน unit of work ของผู้เรียก), `QuotationDocumentStore` (อ่าน/ตรวจ hash/สร้างครั้งเดียว). (3) `EstimateStore.IssueQuotationAsync` และ `QuotationLifecycleStore.AmendAsync` เรียก writer **ก่อน commit** ใน transaction เดียวกัน. (4) Persistence: `commercial.quotation_documents` + `quotation_acceptance_evidences.document_sha256` (1 migration). (5) Api: `QuotationDocumentsController` (GET/POST), endpoint สาธารณะ `GET …/{token}/document`, ตรวจ hash ตอนลูกค้ายอมรับ. (6) Frontend: ปุ่มดาวน์โหลด/สร้าง PDF ในประวัติใบเสนอราคา, หน้าลูกค้าแสดงไฟล์พร้อม SHA-256 และส่ง hash กลับ.

**Tech Stack:** .NET (`net10.0` — `backend/Directory.Build.props`; `AGENTS.md` ที่เขียน ".NET 9" ล้าสมัย), EF Core 10 + Npgsql, PostgreSQL, **QuestPDF 2026.9.1**, xUnit + Testcontainers (ผู้ใช้เป็นคนรัน), Next.js + TypeScript strict, next-intl, Vitest.

## Scope

**ทำในรอบนี้ (ทุก bullet ของ G-04 ยกเว้นโลโก้):** ADR renderer+font; port/implementation; render ครั้งเดียวตอน issue/amend + เก็บ + hash ผูก quotation; pagination/ข้อความยาว/ไทย-อังกฤษ/ข้อมูลองค์กร-สาขา; allowlist + test; cross-scope 404; หน้าลูกค้า CP-07 ที่ hash ตรงกับที่ยอมรับ; การจัดการ render ล้มเหลว; พฤติกรรมของใบเสนอราคาเก่า (404 + code คงที่).

**ไม่ทำโดยตั้งใจ:** โลโก้ (D8); การสร้าง PDF ย้อนหลังให้ใบเก่า (D7); แทนที่/ลบหน้า print เดิม; PDF/A, ลายเซ็นดิจิทัล; permission ใหม่; การกวาด blob กำพร้า; แก้ `GET /estimates/{id}/quotations` ให้ตรวจ Branch (ของเดิม — ดูตาราง Findings).

## Findings จากการสำรวจโค้ด (ตรวจแล้ว)

| หัวข้อ | สิ่งที่พบ | ผลต่อแผน |
| --- | --- | --- |
| ทางออกใบเสนอราคามี **สองทาง** | `EstimateStore.IssueQuotationAsync` (`Infrastructure/Persistence/Estimates/EstimateStore.cs:1256`; เลขที่เอกสาร :1417-1431; audit `quotations.issued` :1493; SaveChanges+Commit :1519-1520) และ `QuotationLifecycleStore.AmendAsync` (`…/Commercial/QuotationLifecycleStore.cs:95-203`; ใบแทนที่ :177; flush :193,197; commit :198). logic ไม่อยู่ใน `Application/Commercial/` ตามที่ gap plan เขียน | Task 5 แก้ทั้งสองจุด; Handler ไม่ต้องแก้ |
| ตัวนับเลขอยู่ใน transaction | `SequenceCounter.cs` ใช้ `_db.Database.CurrentTransaction` (INSERT…ON CONFLICT…RETURNING) | rollback เพราะ render พังจึงไม่เหลือช่องว่างของเลขที่ |
| projection CP-04 (preview) | `GetQuotationDocumentHandler.cs` + `EstimateStore.GetQuotationDocumentAsync` (:1846-2017) อ่าน "ใบล่าสุดของประมาณการ" ตาม `IssuedAtUtc`, เลือกข้อความตาม locale ครั้งละภาษา, ไม่มีข้อมูลองค์กร/สาขา | PDF ใช้ builder ใหม่ที่อ่านตาม `quotationId` (ไม่เดา "ล่าสุด") และมีหัวเอกสาร; projection เดิมไม่แตะ |
| ฟิลด์ต้นทุนอยู่บนแถวเดียวกัน | `EstimateRevision.cs:25,34-36` (`NetCost`, `MarginAmount`, `MarginRate`, `MarkupRate`), `EstimateSection.cs:14` (`SubtotalCost`), `EstimateWorkItem.cs:24-25` (`UnitCost`, `TotalCost`) | ต้องมีชนิด allowlist + test ด้วย reflection; builder อ่านเฉพาะ `*SellingPrice`/ยอดสรุป |
| ความยาวข้อความมีเพดาน | รายละเอียดรายการ 500 ตัวอักษร (`EstimateWorkItemConfiguration.cs:25-26`), ชื่อหมวด 200 (`EstimateSectionConfiguration.cs:21`) | "ข้อความยาว" มีขอบเขต: ตรวจแถวละ 500 ตัวอักษร×2 ภาษาข้ามหน้า (ตรวจแล้วว่า QuestPDF แบ่งแถวข้ามหน้าได้) |
| ข้อมูลลูกค้าถูกตรึงแล้ว | `Quotation.CustomerBillingSnapshotJson` (`Domain/Commercial/Quotation.cs:16`) สร้างโดย `QuotationBillingSnapshotBuilder.cs` | PDF ใช้ snapshot นี้ ไม่ใช้ข้อมูลลูกค้าสด; ย้าย record ที่ `EstimateStore` ซ่อนไว้เป็น `internal` ใช้ร่วม (Task 4) |
| ข้อมูลองค์กร/สาขา (G-03a) | `Organization.cs:8-15` (Name, NameEn, TaxIdentifier, AddressTh/En, Phone) และ `Branch.cs:15-18` (TaxBranchCode, AddressTh/En, Phone); **ไม่มีคอลัมน์โลโก้** | หัวเอกสารพอ; โลโก้ต้องเพิ่ม schema+UI จึงเลื่อน (D8) |
| Files module รองรับ PDF ที่ระบบเขียนเองได้ | `UploadedFile.cs:16` `UploadSessionId` เป็น string ไม่มี FK; allowlist ภาพ `FileConstants.cs:16` บังคับเฉพาะตอนผู้ใช้อัปโหลด (`CreateUploadSessionHandler.cs:103`, `CompleteUploadSessionHandler.cs:151`); เส้นทางอ่านทั่วไป `FileStore.cs:196-` ผูก upload session/parent จึงตอบ 404 สำหรับไฟล์ระบบ; storage `LocalFileStorageProvider.cs` (default `AppContext.BaseDirectory/local-storage`) | ใช้ `UploadedFile`+`IFileStorageProvider` โดยไม่แก้ allowlist และไม่เปิดช่องอ่านทั่วไป |
| กลไก G-01 (`AttachmentLink`) ไม่เหมาะกับ artifact | `AttachmentLink.cs:8` (50 ต่อ owner), `:71` (`Remove`), registry มีแต่ `installation-job` (`AttachmentValues.cs:17-19`) | gap plan บอก "ผ่านกลไก G-01" — ใช้ได้เฉพาะชั้น Files; ไม่ลงทะเบียน owner type `quotation` (D5) |
| error 5xx ไม่มี code สาธารณะ | `ProblemDetailsMapper.cs:402-413` ปล่อยเฉพาะ 400/401/403/404/409/422/428; exception ไม่จับ → `ApiExceptionHandler.cs:26` = `INTERNAL_SERVER_ERROR` | render พังไม่เพิ่ม code ใหม่; code ใหม่เพียง `QUOTATION_DOCUMENT_NOT_AVAILABLE` (404) |
| หน้า CP-07 | `QuotationAcceptanceHandler.cs:72-85` (กฎลิงก์), :87-97 (view; เรียก `GetQuotationDocumentAsync` :92 แล้วเทียบเลขที่ :94), :99-150 (ยอมรับ; `AcceptQuotationAsync` :132); หลักฐาน `QuotationAcceptanceLink.cs:92-124`, `QuotationAcceptanceStore.cs:121-165` | เพิ่ม `pdf` ใน view, endpoint `/document`, ตรวจ hash ก่อน :132, เก็บ `document_sha256` |
| ประวัติใบเสนอราคาไม่ตรวจ Branch | `QuotationLifecycleHandler.cs:51-58` ตรวจ Organization อย่างเดียว | ไม่แก้ (นอกขอบเขต); endpoint PDF ใหม่ตรวจ `HasBranchAccess` และคืน 404 |
| Frontend | print เดิม `quotation-document-page.tsx:80`; `ApiClient.execute` ตอบ `.json()` เท่านั้น (`api-client.ts`); ไม่มี util บันทึก Blob (`export-csv.ts:44`, `export-excel.ts:84` ทำซ้ำกัน) | เพิ่ม `read` ใน `execute` + `saveBlob` ของกลาง (D13) |
| Package | `Directory.Packages.props` ใช้ central versions; ไม่มี QuestPDF. ตรวจ NuGet แล้ว: **2026.9.1 มีจริง** (เผยแพร่ 2026-09-25 = 17 วัน), รองรับ net10.0; compile+รันจริงใน spike ด้วย `TreatWarningsAsErrors` | pin `2026.9.1`. **ข้อควรระวังที่ตรวจพบ:** `FontManager.RegisterFont(Stream)` เป็น obsolete ตั้งแต่ 2026.9 (กลายเป็น error) ต้องใช้ `RegisterFontFromStream` |
| ความ deterministic | spike: input เดียวกัน รันสองโปรเซส ได้ SHA-256 ต่างกัน (`DA6EA52A…` vs `48DC5A4E…`) | hash คงที่เพราะเก็บไฟล์ครั้งเดียว (ADR 0023 ข้อ 4); ห้ามเขียน test ที่เทียบ hash ของการ render สองรอบ |
| OpenAPI formatter | formatter ฝังใน G-03 plan Task 9 ไม่ทำซ้ำ snapshot ปัจจุบัน (`cmp` ต่างที่บรรทัด 33576); เวอร์ชันที่มี float handling ทำซ้ำได้ | Task 8 ฝังเวอร์ชันที่ผ่านและบังคับ `FORMATTER_OK` ก่อนใช้ |

### ข้อที่แผนหลัก (gap plan) คลาดเคลื่อน/ต้องปรับ

1. "เก็บเป็น `UploadedFile` ผ่านกลไก G-01" — ชั้น `AttachmentLink`/owner registry ใช้ไม่ได้ (ผู้ใช้ถอดได้, image-only upload gate, เส้นทางอ่านผูก session); ใช้ Files module + ตารางผูกใหม่แทน.
2. "แตะ `Application/Commercial/`, projection CP-04" — การออกใบเสนอราคาอยู่ใน `EstimateStore` (Estimates) และ `QuotationLifecycleStore` (Amend); projection CP-04 อยู่ใต้ `Application/Estimates/GetQuotationDocument`.
3. "โลโก้จาก G-03" — G-03a ไม่มีโลโก้ และ gap plan เองบอกว่าเลื่อนมา G-04; slice นี้เลื่อนต่ออีก (D8).
4. "hash คงที่ต่อฉบับ" ทดสอบได้เฉพาะในความหมาย "ไฟล์ที่เก็บไม่เปลี่ยน" — render ซ้ำให้ hash ต่างกัน.
5. `AGENTS.md` ระบุ .NET 9 แต่ repo เป็น `net10.0`.

## Decisions ที่ผู้ใช้อาจ veto

| # | ตัดสินใจ | เหตุผล / ทางเลือกที่ไม่เลือก |
| --- | --- | --- |
| D1 | **QuestPDF 2026.9.1**, Community license เป็นค่า default และเป็น **เงื่อนไขที่ Business ต้องยืนยันก่อน production** (รายได้รวม < 1,000,000 USD; ไม่ใช่บริษัทมหาชน/หน่วยงานรัฐ). ตั้งค่าที่ `QuestPdfRuntime.EnsureInitialized` ผ่าน config `Documents:QuestPdf:LicenseType`; `LicenseConfirmed` ไม่ตั้งใน Production = log Warning. Fallback: ซื้อ Professional (เปลี่ยน config) หรือเปลี่ยน implementation หลัง port | ผู้ใช้เลือก QuestPDF แล้ว; รายละเอียดอยู่ใน ADR 0023 |
| D2 | **ฟอนต์ Sarabun Regular+Bold เท่านั้น** (180,024 ไบต์ + `OFL.txt` 4,387 ไบต์ ≈ 176 KiB เทียบ pack ของ repo 12.17 MiB) ฝังเป็น `EmbeddedResource`, `UseSystemFonts=false` | ไม่เอา IBM Plex Sans Thai (ใหญ่กว่า ไม่มีประโยชน์เพิ่ม), ไม่เอา weight อื่น/italic (ใช้ Bold เป็นตัวเน้น) |
| D3 | **PDF เดียวต่อ quotation สองภาษา** (ไทยหลัก, อังกฤษใต้บรรทัดเมื่อมี; ป้ายกำกับเป็นคู่ "ไทย / English") แทนไฟล์ต่อ locale | artifact เดียว = hash เดียว และไม่ติดปัญหา "ข้อความอังกฤษขาด" (`hasIncompleteTranslations`); หน้า preview ยังสลับ th/en ได้. ป้ายกำกับใน PDF เป็นแค็ตตาล็อกคงที่ใน renderer (เอกสารลูกค้าไม่ตาม locale ผู้ใช้) |
| D4 | **Render ก่อน commit ใน unit of work เดียวกับการออก** (เรียก writer ก่อน `SaveChanges`+`Commit` เดิม; ไม่ใช่ commit ก่อนแล้ว render + status flag): พัง → rollback, `500`, ลองใหม่ได้, ไม่มี issued-ไม่มีไฟล์ | ค่าใช้จ่ายคือ transaction ยาวขึ้นเท่าเวลา render (วัด ~2–116 ms/ใบ); ถ้า commit ล้มหลังเขียน blob เหลือ blob กำพร้า (ไม่มีใครอ้างถึง) |
| D5 | **เก็บด้วย `UploadedFile` + ตาราง `quotation_documents` ใหม่** ไม่ใช้ `AttachmentLink`/owner registry; ไฟล์ระบบใช้ `upload_session_id="system:quotation-document:<id>"`; อ่านผ่าน endpoint เฉพาะ ไม่ใช่ `/api/v1/files/{id}/content` | ดู Findings |
| D6 | **ลูกค้าต้องส่ง hash ที่เห็น** ตอนยอมรับ; ไม่ตรง/ขาดขณะมีไฟล์ → `ACCEPTANCE_CONFLICT` (ไม่เพิ่ม code); หลักฐานเก็บ `document_sha256` | ทำให้ "PDF ที่ยอมรับ = PDF ที่เห็น" ตรวจได้ฝั่งเซิร์ฟเวอร์ |
| D7 | **ใบเก่าไม่มีไฟล์ และไม่ backfill:** ดาวน์โหลด → `404 QUOTATION_DOCUMENT_NOT_AVAILABLE`; ต้องการ PDF ให้ใช้ Amend (ออกฉบับใหม่ที่มีไฟล์); หน้า preview/print เดิมยังใช้ได้; ลูกค้ายอมรับใบเก่าได้ (hash=null). Endpoint "สร้างครั้งเดียวตามคำขอ" (`quotations.issue`) **ไม่ทำ** เป็น follow-up | ไฟล์ที่ render ภายหลังใช้ข้อมูลองค์กรวันนี้ ไม่ใช่สิ่งที่ลูกค้าเคยได้รับ จึงไม่สร้างให้เอง; writer ออกแบบให้เรียกซ้ำได้ถ้าต้องเพิ่ม (ต้องมีคอลัมน์บอกว่าเป็นไฟล์ที่ทำย้อนหลัง) |
| D8 | **โลโก้: ไม่ทำ** (เลื่อนเป็น follow-up). ถ้าทำ: คอลัมน์/ไฟล์โลโก้ใน Organization, owner type `organization` ครบสามจุด (whitelist, registry, scope reader) + UI ตั้งค่า + อ่านไฟล์ตอน render | เกินขอบเขต backend ล้วนของ slice นี้; การเพิ่มภายหลังเป็น additive (เพิ่ม field ใน `QuotationPdfIssuer`) และไม่กระทบไฟล์ที่เก็บแล้ว |
| D9 | **หัวเอกสาร:** ชื่อองค์กร th/en, ชื่อสาขา, เลขผู้เสียภาษี, รหัสสาขาภาษี, ที่อยู่ th/en, โทร. สาขาที่มีที่อยู่ (th หรือ en) ใช้ที่อยู่+โทรของสาขาทั้งชุด มิฉะนั้นใช้ของ Organization ทั้งชุด (ไม่ผสมทีละบรรทัด). ช่องว่างไม่ถูกพิมพ์ — ต้องกรอกใน Settings ก่อนใช้จริง | กติกาชัดเจน ไม่ใช่ fallback ลูกโซ่เดาข้อมูล |
| D10 | **ไม่เพิ่ม permission:** ดาวน์โหลดและสรุปเอกสารใช้ `quotations.read` | permission catalog/seeder ไม่เปลี่ยน (แก้เฉพาะคำอธิบายแถว `quotations.read`) |
| D11 | **Audit** `quotations.document-rendered` (เลขที่, sha256, byteCount, rendererVersion) ตอน issue/amend; ไม่ audit การดาวน์โหลด | สอดคล้องการอ่านอื่น; ไม่ใส่ข้อมูลลูกค้า |
| D12 | **วันที่ใน PDF** UTC+7 คงที่ `dd/MM/yyyy` (ค.ศ.); ตัวเลข `#,##0.00`, จำนวน `#,##0.####`; ป้าย "ภาษีมูลค่าเพิ่ม / VAT" ตามป้ายของหน้า preview เดิม | ไทยไม่มี DST จึงไม่พึ่ง tz database; ปีพุทธศักราชไม่ใช้เพื่อไม่ให้ขัดกับหน้า preview |
| D13 | **Frontend:** ปุ่มดาวน์โหลด PDF (+ SHA-256 ย่อ) ในแผงประวัติใบเสนอราคา; หน้าลูกค้าแสดงปุ่มดาวน์โหลด + SHA-256 เต็ม (ไม่ฝัง iframe เพราะ mobile browser รองรับไม่สม่ำเสมอ) คง HTML preview ไว้. **Global Reuse Proposal:** `lib/files/save-blob.ts` (ยังไม่ refactor exporter เดิม) | ผู้ใช้ปฏิเสธข้อเสนอได้ → ย้าย `saveBlob` ไปอยู่ใน hook |
| D14 | **ไม่ใช่ PDF/A และไม่ใช่ลายเซ็นอิเล็กทรอนิกส์** (ระดับหลักฐานเท่า CP-07: ชื่อ+ยินยอม+hash) | ถ้า Legal ต้องการสูงกว่านี้ → slice ใหม่ |

## Task Map

| Task | เรื่อง | commit |
| --- | --- | --- |
| 1 | ADR 0023, สัญญา API, ศัพท์ (docs) | `docs(commercial): add ADR 0023 and quotation document contract` |
| 2 | Renderer foundation: QuestPDF, ฟอนต์ฝัง, allowlist model, port | `feat(commercial): add quotation pdf renderer with embedded Thai fonts` |
| 3 | Persistence: `QuotationDocument`, `document_sha256`, migration | `feat(commercial): persist immutable quotation documents` |
| 4 | Port/handler + builder/writer/store (อ่านและตรวจ hash) + DI | `feat(commercial): add quotation document writer, store and handler` |
| 5 | ต่อสาย issue/amend (render ก่อน commit) + สรุปเอกสารในประวัติ | `feat(commercial): render and store the quotation pdf when issuing or amending` |
| 6 | Endpoint ดาวน์โหลดไฟล์ + error code ใหม่ | `feat(commercial): serve the stored quotation pdf` |
| 7 | หน้าลูกค้า CP-07: ไฟล์ + ผูก hash ในหลักฐาน | `feat(commercial): bind customer acceptance to the stored quotation pdf hash` |
| 8 | OpenAPI snapshot, generated types, API client, `saveBlob` | `feat(commercial): expose quotation pdf endpoints in the api contract and client` |
| 9 | Frontend: ปุ่ม PDF + หน้าลูกค้า + i18n | `feat(commercial): download the stored quotation pdf from history and the customer page` |
| 10 | Verification + สถานะ (ไม่ติ๊ก) | `docs(commercial): record G-04 verification and update status` |

ลำดับคอมไพล์ได้ทีละ commit: Task 2 ไม่มีผู้เรียก; Task 3 เพิ่ม entity/คอลัมน์ที่ยังไม่มีผู้ใช้ (พารามิเตอร์ท้ายของ ctor เป็น optional); Task 4 เพิ่ม port+implementation+DI โดยยังไม่มีใครเรียกใน flow เดิม; Task 5 แก้ ctor ของสอง store พร้อม DI ที่ลงทะเบียนไว้แล้วใน Task 4 (ไม่มี test ที่ `new` store เหล่านี้โดยตรง); Task 6–7 เพิ่ม endpoint/contract; Task 8 ต้องมาหลัง backend ทั้งหมด (snapshot); Task 9 ต้องมาหลัง Task 8 (types).

---

> **รูปแบบ patch (`text`):** ทำตามลำดับจากบนลงล่าง — `>>> หลัง: X` = แทรกบรรทัดที่ตามมา (จนถึง `>>>` ถัดไป) ต่อท้ายบรรทัดเดียวที่มี `X`; `>>> แทนที่:` … `>>> ด้วย:` … = Edit (old_string → new_string). `X` และ old_string ต้องตรงทุกตัวอักษรและพบครั้งเดียวในไฟล์ — ไม่ตรงให้หยุดและรายงาน ห้ามเดา.

## Task 1: ADR 0023, สัญญา API, ศัพท์ธุรกิจ (docs เท่านั้น)

**Files:** Create: `docs/adr/0023-server-side-quotation-pdf-with-questpdf.md`, `docs/03-contracts/quotation-document-api-contract.md`; Modify: `docs/adr/README.md`, `docs/adr/0016-browser-print-for-quotation-pdf.md`, `docs/README.md`, `CONTEXT.md`, `docs/03-contracts/quotation-lifecycle-api-contract.md`, `docs/03-contracts/external-acceptance-api-contract.md`, `docs/03-contracts/permission-catalog.md`

docs เท่านั้น — ไม่มี build. ใน `quotation-document-api-contract.md` ให้แทน `<วันที่จริง>` ด้วยผลของ `date +%F`. ข้อเท็จจริงเรื่อง license/ฟอนต์อยู่ใน ADR เท่านั้น (ไม่ใช่ศัพท์ธุรกิจ จึงไม่ใส่ `CONTEXT.md`); `CONTEXT.md` ได้เฉพาะคำว่า Quotation Document.

- [ ] **Step 1: สร้าง `docs/adr/0023-server-side-quotation-pdf-with-questpdf.md`**
```markdown
---
status: accepted
---

# สร้าง PDF ใบเสนอราคาฝั่งเซิร์ฟเวอร์ด้วย QuestPDF และเก็บเป็นไฟล์ที่เปลี่ยนไม่ได้พร้อม SHA-256

[ADR 0016](0016-browser-print-for-quotation-pdf.md) ให้พิมพ์ PDF ด้วย browser และระบุว่าต้องยกระดับเป็น PDF ฝั่งเซิร์ฟเวอร์ที่เก็บ artifact + hash ก่อนเปิด External Acceptance (CP-07) — CP-07 เปิดแล้ว แต่ระบบยังไม่เก็บไฟล์ที่ลูกค้าเห็นจริง. ADR นี้ **เสริม** 0016 ไม่ยกเลิก: หน้า preview/print เดิมยังอยู่เป็น preview; ไฟล์ที่เป็นหลักฐานคือ PDF ที่สร้างตอนออกใบเสนอราคาและเก็บไว้.

## ตัดสินใจ

**1. Renderer = QuestPDF (in-process .NET)** ผ่าน port `IQuotationDocumentRenderer` (Application) โดย implementation อยู่ใน `TanErp.Infrastructure/Documents` เพียงที่เดียว. ไม่ใช้ headless browser (ต้องดูแล Chromium ใน deployment) และไม่ใช้ HTML-to-PDF. ตรวจแล้วว่า 2026.9.1 มีบน NuGet (เผยแพร่ 2026-09-25), รองรับ net10.0, และเรียงสระ/วรรณยุกต์ไทยถูกต้องด้วย Sarabun.

**2. เงื่อนไข license ที่ Business ต้องยืนยันก่อน production.** ตามข้อกำหนด QuestPDF (License Selection Guide v3.0, มีผล 2026-07-06; ตรวจจาก `LICENSE.md` ต้นทางเมื่อ 2026-10-12): Community License ฟรีเมื่อรายได้รวมของนิติบุคคลและบริษัทในเครือที่ควบคุมร่วมกัน **ต่ำกว่า 1,000,000 USD** ในปีงบประมาณล่าสุด; บริษัทมหาชนและหน่วยงานรัฐใช้ Community ไม่ได้ไม่ว่ารายได้เท่าใด; ที่เกินเงื่อนไขต้องซื้อ Professional/Enterprise ก่อน "production use" (ใช้ใน environment จริงหรือประมวลผลข้อมูลจริง) และมีช่วงเปลี่ยนผ่าน 90 วันเมื่อพ้นเกณฑ์. เอกสารนี้ไม่ใช่คำปรึกษากฎหมาย — ผู้ยืนยันต้องอ่านข้อกำหนดฉบับปัจจุบันที่ questpdf.com/license. **ผู้ยืนยัน/วันที่ยืนยันบันทึกใน [Quotation PDF Verification](../05-engineering/quotation-pdf-verification.md)** (ยังว่างจนกว่า Business ยืนยัน).

- *ตั้งค่าในโค้ด:* `QuestPdfRuntime.EnsureInitialized` (`backend/src/TanErp.Infrastructure/Documents/QuestPdfRuntime.cs`) เป็นที่เดียวที่กำหนด `QuestPDF.Settings.License` ทำงานครั้งเดียวเมื่อ DI สร้าง `QuestPdfQuotationRenderer` (บังคับสร้างตอน startup ใน `Program.cs` เพื่อให้ config ผิดล้มทันที). ค่ามาจาก config `Documents:QuestPdf:LicenseType` = `Community` (default) | `Professional` | `Enterprise`; ค่าอื่น (รวม `Evaluation`) ทำให้แอปไม่ขึ้น. `Documents:QuestPdf:LicenseConfirmed=true` ตั้งเมื่อ Business ยืนยันแล้ว; ใน Production ที่ไม่ตั้ง จะมี log Warning ทุกครั้งที่เริ่ม (ไม่ล้ม เพื่อไม่ให้การออกเอกสารหยุด).
- *Fallback ถ้าไม่ผ่านเงื่อนไข:* (ก) ซื้อ Professional แล้วเปลี่ยน config เป็น `Professional` — ไม่แก้โค้ด และไฟล์ที่เก็บแล้วไม่กระทบ; (ข) ถ้าไม่ซื้อ ให้เปลี่ยน implementation หลัง port (ขอบเขตที่รู้จัก QuestPDF คือโฟลเดอร์ `Documents` ราว 250 บรรทัด) โดยผู้สมัคร (เช่น Chromium ผ่าน Playwright, ซึ่งหนักด้านปฏิบัติการ) ต้องผ่านการตรวจเรียงสระ/วรรณยุกต์ไทยก่อน — เอกสารที่ออกแล้วและ hash ยังใช้ได้เพราะไม่ผูกกับ renderer.

**3. ฟอนต์: Sarabun Regular + Bold (SIL OFL 1.1), repo เป็นเจ้าของไฟล์.** เก็บที่ `backend/src/TanErp.Infrastructure/Documents/Fonts/` (ไฟล์ละ 90,220 และ 89,804 ไบต์ รวม 180,024 ไบต์ + `OFL.txt` 4,387 ไบต์; ดึงจาก `google/fonts` commit `88058c714073e72bb1b32baffc2d4ce9bc73944c` path `ofl/sarabun`, SHA-256 ตรวจใน Task 2). ฝังเป็น `EmbeddedResource` (รูปแบบเดียวกับ `thailand_addresses.json.gz`) จึงไม่ขึ้นกับ path ตอน deploy; ลงทะเบียนด้วย `FontManager.RegisterFontFromStream` ตอน startup พร้อม `UseSystemFonts=false` (ผลเหมือนกันทุกเครื่อง) และ `ThrowOnMissingFontFamilies=true`. ไม่เปิด `ThrowOnMissingTextGlyphs` เพราะอักขระนอกฟอนต์ (เช่น emoji ในรายละเอียด) จะทำให้ออกใบเสนอราคาไม่ได้. เงื่อนไข OFL: ต้องแนบ copyright/license กำกับไฟล์ฟอนต์ที่แจกจ่าย → `OFL.txt` อยู่ข้างไฟล์ฟอนต์ใน repo และฝังใน assembly (test ตรวจ); ห้ามแก้ไฟล์ฟอนต์. ไม่เลือก IBM Plex Sans Thai (ไฟล์ละ ~116 KB ไม่มีประโยชน์เพิ่ม).

**4. Artifact เก็บครั้งเดียว และ hash ผูกกับ quotation.** ตอนออกใบเสนอราคา (และตอน amend) render หนึ่งครั้ง เก็บ bytes ผ่าน `IFileStorageProvider`, บันทึก `UploadedFile` (media type `application/pdf`, `content_sha256`) และแถว `commercial.quotation_documents` (unique ต่อ quotation; sha256, byte_count, renderer_version, เวลา). **ไม่มี mutator** และไม่มี endpoint แก้/ลบ; ใบเสนอราคาที่เปลี่ยน = quotation ใหม่ (amend) ที่มีไฟล์ของตัวเอง. **ความคงที่ของ hash มาจากการเก็บไฟล์ ไม่ใช่จาก renderer:** render input เดียวกันสองครั้งต่าง process ได้ SHA-256 ต่างกัน (ตรวจแล้ว; QuestPDF ฝังเวลา/ID) — จึงห้าม re-render และ test ไม่เทียบ hash ของการ render สองรอบ. ทุกครั้งที่อ่านไฟล์ระบบตรวจ SHA-256 ของ bytes กับค่าที่บันทึกก่อนส่ง.

**5. ใช้ Files module (`UploadedFile` + storage) แต่ไม่ใช้ `AttachmentLink`/owner registry ของ G-01** เพราะ link ถูกออกแบบให้ผู้ใช้ถอด (`Remove`) และจำกัด 50 ต่อ owner, owner registry ปัจจุบันมีแต่ `installation-job`, และเส้นทางอ่านไฟล์ทั่วไป (`/api/v1/files/{id}/content`) ผูกกับ upload session ของผู้ใช้ — ขัดกับ artifact ที่ห้ามแก้. ไฟล์ของระบบใช้ `upload_session_id = "system:quotation-document:<id>"` (ไม่ใช่ GUID) จึงเปิดผ่าน endpoint ทั่วไปไม่ได้ (404) และ allowlist ภาพ (`AllowedMediaTypes`) ที่บังคับเฉพาะตอนผู้ใช้อัปโหลดจึงไม่ถูกแตะ. การอ่านผ่าน endpoint เฉพาะของ quotation ที่ตรวจ Organization+Branch (ไม่เปิดเผย file id).

**6. Render ก่อน commit ใน transaction เดียวกับการออก.** ถ้า render พังทั้ง transaction rollback (ไม่มี quotation, เลขเอกสารและ idempotency key ไม่ถูกใช้, ลองใหม่ได้); ผู้ใช้ได้ `500 INTERNAL_SERVER_ERROR` และมี log (ไม่มี code สาธารณะใหม่เพราะ mapper ส่ง 5xx เป็น code กลาง). ไม่ใช้ "commit ก่อนแล้ว render ทีหลัง + status flag" เพราะเกิดสถานะ issued-แต่ไม่มีไฟล์ ซึ่งลูกค้ายอมรับได้ก่อนมีหลักฐาน. ค่าใช้จ่าย: transaction เปิดนานขึ้นเท่าเวลา render (วัดจริงในเครื่องพัฒนา: ใบเล็กราว 2 ms, ครั้งแรกหลังเริ่มโปรเซสราว 116 ms, ใบ 120 รายการยาว 27 หน้าราว 42 ms).

**7. ใบเสนอราคาที่ออกก่อน G-04 ไม่มีไฟล์ และไม่ backfill** — ไฟล์ที่ render ภายหลังจะใช้ข้อมูลองค์กรวันนี้และไม่ใช่ "สิ่งที่ลูกค้าได้รับ" จึงไม่สร้างให้เอง. ดาวน์โหลด → `404 QUOTATION_DOCUMENT_NOT_AVAILABLE` (code คงที่); หน้า preview/print เดิมยังใช้ได้; ถ้าต้องการ PDF ของใบเก่าให้ **Amend** ออกฉบับใหม่ (ได้ไฟล์ของตัวเอง) — endpoint "สร้างครั้งเดียวตามคำขอ" เป็นทางเลือกใน follow-up (ต้องบันทึกว่าเป็นไฟล์ที่ทำย้อนหลัง). การยอมรับของลูกค้ากับใบเก่ายังทำงาน โดยหลักฐานเก็บ hash เป็น null.

**8. หลักฐานการยอมรับผูก hash.** หน้าลูกค้าแสดงไฟล์ PDF (ดาวน์โหลด) พร้อม SHA-256 และส่ง hash กลับตอนยอมรับ; เซิร์ฟเวอร์ต้องเห็น hash เท่ากับไฟล์ที่เก็บ (null เท่ากับ "ไม่มีไฟล์") มิฉะนั้น `ACCEPTANCE_CONFLICT`; `quotation_acceptance_evidences.document_sha256` เก็บค่าที่ยืนยัน.

**9. ขอบเขต:** เอกสารเดียวสองภาษา (ไทยหลัก อังกฤษใต้บรรทัดเมื่อมี) แทนไฟล์ต่อภาษา; ข้อมูลหัวเอกสารจาก Organization/Branch (G-03a) โดยสาขาที่มีที่อยู่ของตนใช้ที่อยู่+โทรของสาขาทั้งชุด มิฉะนั้นใช้ของ Organization ทั้งชุด; เวลาแสดงเป็น UTC+7 คงที่ (ไทยไม่มี DST) รูปแบบ `dd/MM/yyyy`; **ไม่มีโลโก้** (เลื่อนไป follow-up: Organization ยังไม่มีคอลัมน์โลโก้และต้องมี owner type + UI ตั้งค่า); ไม่ใช่ PDF/A และไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตามกฎหมาย.

## ข้อเสียที่ยอมรับ

เงื่อนไข license ยังเป็นความเสี่ยงทางธุรกิจจนกว่าจะยืนยัน; `QuestPDF.Settings` เป็น static ทั้งโปรเซส; ไฟล์ PDF ใช้พื้นที่เก็บเพิ่มต่อใบเสนอราคา (หลักสิบ KB); blob กำพร้า (ถ้า commit ล้มหลังเขียนไฟล์ เช่นแพ้ idempotency race ของการออกใบเสนอราคา) ไม่มีใครอ้างถึงและไม่มี job กวาด; ข้อมูลองค์กรฝังอยู่ในไฟล์เท่านั้น (ไม่มี snapshot ในฐานข้อมูล).
```
- [ ] **Step 2: สร้าง `docs/03-contracts/quotation-document-api-contract.md`**
```markdown
# Quotation Document API Contract (ข้อตกลง API ไฟล์ PDF ใบเสนอราคาที่เก็บไว้)

**สถานะ:** Implemented <วันที่จริง> (G-04) — tests written, not run; รอ Sales/Finance ยืนยัน layout/ข้อมูลหัวเอกสาร และรอ Business ยืนยันเงื่อนไข license ของ QuestPDF ([ADR 0023](../adr/0023-server-side-quotation-pdf-with-questpdf.md)). Preview/Print เดิม (`GET /api/v1/estimates/{id}/quotation/document`) ยังใช้ได้ตาม [ADR 0016](../adr/0016-browser-print-for-quotation-pdf.md).

## Decisions

| หัวข้อ | ค่าที่ใช้ |
| --- | --- |
| เมื่อไหร่สร้างไฟล์ | ตอนออกใบเสนอราคา (`POST /api/v1/estimates/{id}/quotation`) และตอน amend ใน unit of work เดียวกัน; render พัง → rollback ทั้งหมด (`500 INTERNAL_SERVER_ERROR`) |
| ไฟล์ | `application/pdf` หนึ่งไฟล์ต่อ quotation, สองภาษาในไฟล์เดียว, A4; เก็บครั้งเดียว ไม่ re-render, ไม่มี endpoint แก้/ลบ/สร้างซ้ำ |
| ข้อมูลในไฟล์ | allowlist เท่านั้น: หัวเอกสารองค์กร/สาขา, ลูกค้าจาก billing snapshot ตอนออก, หมวด/รายการ (รหัส, รายละเอียด th/en, จำนวน, หน่วย, ราคาต่อหน่วย, รวม), ยอดสรุป; **ไม่มีต้นทุน/มาร์จิน/ข้อมูลภายใน** (บังคับด้วยชนิด `QuotationPdfModel` + test ด้วย reflection) |
| Hash | SHA-256 ของ bytes ที่เก็บ; ตรวจซ้ำทุกครั้งที่อ่าน; `ETag` = hash |
| ใบเสนอราคาเก่า | ไม่มีไฟล์ → `404 QUOTATION_DOCUMENT_NOT_AVAILABLE`; ใช้ Amend เพื่อออกฉบับใหม่ที่มีไฟล์ |
| Audit | `quotations.document-rendered` (เลขที่เอกสาร, sha256, byteCount, rendererVersion — ไม่มีข้อมูลลูกค้า); การดาวน์โหลดไม่ audit (สอดคล้องการอ่านอื่น) |

## Endpoints

| Action | Method/Path | Permission |
| --- | --- | --- |
| ดาวน์โหลดไฟล์ที่เก็บไว้ | `GET /api/v1/quotations/{id}/document` → `200 application/pdf` | `quotations.read` |
| สรุปเอกสารต่อใบ | `GET /api/v1/estimates/{id}/quotations` → แต่ละ item มี `document: {sha256, byteCount, rendererVersion, renderedAtUtc} \| null` | `quotations.read` |
| ลูกค้าดูไฟล์ | `GET /api/public/v1/quotation-acceptance/{token}/document` → `200 application/pdf` (`no-store`, rate limit เดิม); view ของลูกค้ามี `pdf: {sha256, byteCount} \| null` | token |

ไม่มี permission ใหม่. endpoint ภายในตรวจ Organization **และ Branch** (`access.HasBranchAccess`).

## Errors

`404`: `RESOURCE_NOT_FOUND` (นอก Organization/Branch — ตรวจก่อนตรวจว่ามีไฟล์), **`QUOTATION_DOCUMENT_NOT_AVAILABLE`** (ไม่มีไฟล์; ใหม่), `ACCEPTANCE_LINK_UNAVAILABLE` (public ทุกกรณี). `403`: `PERMISSION_DENIED`. `409`: `ACCEPTANCE_CONFLICT` (hash ที่ลูกค้าส่งไม่ตรงไฟล์ที่เก็บ). `500`: `INTERNAL_SERVER_ERROR` (render ล้มเหลวตอนออก).

## Data (migration `AddQuotationDocuments`)

`commercial.quotation_documents` (unique `quotation_id`, unique `file_id`, FK composite `(file_id, organization_id)` → `files.uploaded_files`; check `sha256 ~ ^[0-9a-f]{64}$`, `byte_count > 0`); `commercial.quotation_acceptance_evidences.document_sha256` (nullable).
```
- [ ] **Step 3: แก้ `docs/adr/README.md`**
```text
>>> แทนที่:
- [0022 — Membership Expiry Reminder และ hosted worker ขั้นต่ำ](0022-membership-expiry-reminder-and-hosted-worker.md)
>>> ด้วย:
- [0022 — Membership Expiry Reminder และ hosted worker ขั้นต่ำ](0022-membership-expiry-reminder-and-hosted-worker.md)
- [0023 — Server-side Quotation PDF ด้วย QuestPDF และ artifact ที่เก็บครั้งเดียวพร้อม SHA-256](0023-server-side-quotation-pdf-with-questpdf.md)
```
- [ ] **Step 4: แก้ `docs/adr/0016-browser-print-for-quotation-pdf.md`**
```text
>>> แทนที่:
หรือเมื่อ Business ต้องการหลักฐานเอกสารที่ส่งออกไป
>>> ด้วย:
หรือเมื่อ Business ต้องการหลักฐานเอกสารที่ส่งออกไป

**อัปเดต:** [ADR 0023](0023-server-side-quotation-pdf-with-questpdf.md) เพิ่ม PDF ฝั่งเซิร์ฟเวอร์ที่เก็บครั้งเดียวพร้อม SHA-256 แล้ว; หน้า preview/print ในข้อนี้ยังใช้เป็น preview ได้ แต่ไม่ใช่ไฟล์ที่เป็นหลักฐาน.
```
- [ ] **Step 5: แก้ `docs/README.md`**
```text
>>> แทนที่:
| API ลูกค้ายอมรับใบเสนอราคาผ่านลิงก์ | [External Acceptance API Contract](03-contracts/external-acceptance-api-contract.md) |
>>> ด้วย:
| API ลูกค้ายอมรับใบเสนอราคาผ่านลิงก์ | [External Acceptance API Contract](03-contracts/external-acceptance-api-contract.md) |
| API ไฟล์ PDF ใบเสนอราคาที่เก็บไว้ | [Quotation Document API Contract](03-contracts/quotation-document-api-contract.md) |
```
- [ ] **Step 6: แก้ `CONTEXT.md`**
```text
>>> แทนที่:
**Quotation (ใบเสนอราคา)**:
ข้อเสนอเชิงพาณิชย์ที่ออกให้ลูกค้า โดยอ้างอิง Estimate ฉบับที่อนุมัติ
_Avoid_: Estimate, Invoice
>>> ด้วย:
**Quotation (ใบเสนอราคา)**:
ข้อเสนอเชิงพาณิชย์ที่ออกให้ลูกค้า โดยอ้างอิง Estimate ฉบับที่อนุมัติ
_Avoid_: Estimate, Invoice

**Quotation Document (เอกสารใบเสนอราคาที่ออกแล้ว)**:
ไฟล์ PDF ของ Quotation หนึ่งฉบับที่สร้างครั้งเดียวตอนออกและเก็บไว้เปลี่ยนไม่ได้ พร้อมค่า SHA-256 ที่ใช้เป็นหลักฐานว่าลูกค้าเห็นและยอมรับอะไร; ถ้าข้อมูลเปลี่ยนต้องออก Quotation ฉบับใหม่ ไม่ใช่สร้างไฟล์ใหม่ของฉบับเดิม
_Avoid_: Preview, Print View, Invoice
```
- [ ] **Step 7: แก้ `docs/03-contracts/quotation-lifecycle-api-contract.md`**
```text
>>> แทนที่:
| ประวัติใบเสนอราคาของประมาณการ | `GET /api/v1/estimates/{id}/quotations` | `quotations.read` |
>>> ด้วย:
| ประวัติใบเสนอราคาของประมาณการ | `GET /api/v1/estimates/{id}/quotations` (แต่ละ item มี `document` = สรุปไฟล์ PDF ที่เก็บไว้ หรือ `null` สำหรับใบที่ออกก่อน G-04 — ดู [Quotation Document API](quotation-document-api-contract.md)) | `quotations.read` |
```
- [ ] **Step 8: แก้ `docs/03-contracts/external-acceptance-api-contract.md`**
```text
>>> แทนที่:
| ดูเอกสาร | `GET /api/public/v1/quotation-acceptance/{token}?locale=th|en` → `{status, expiresAtUtc, signerHint, consentVersion, acceptedAtUtc, document}` |
>>> ด้วย:
| ดูเอกสาร | `GET /api/public/v1/quotation-acceptance/{token}?locale=th|en` → `{status, expiresAtUtc, signerHint, consentVersion, acceptedAtUtc, document, pdf}` โดย `pdf = {sha256, byteCount}` หรือ `null` ถ้าใบเสนอราคาไม่มีไฟล์ที่เก็บไว้ |
| ไฟล์ PDF ที่ใช้ยอมรับ | `GET /api/public/v1/quotation-acceptance/{token}/document` → `200 application/pdf` (ไฟล์ที่เก็บไว้; `no-store`; ใช้กฎลิงก์เดียวกับ view; ไม่มีไฟล์ → `404 ACCEPTANCE_LINK_UNAVAILABLE`) |
>>> แทนที่:
| ยอมรับ | `POST /api/public/v1/quotation-acceptance/{token}/accept` `{signerName, signerRole?, consentAccepted, consentVersion, signatureImage?}` → `200 {status, acceptedAtUtc, quotationNumber, evidence}` |
>>> ด้วย:
| ยอมรับ | `POST /api/public/v1/quotation-acceptance/{token}/accept` `{signerName, signerRole?, consentAccepted, consentVersion, signatureImage?, documentSha256?}` → `200 {status, acceptedAtUtc, quotationNumber, evidence}` — `documentSha256` ต้องเท่ากับ `pdf.sha256` ที่หน้าแสดง (`null` เท่ากับไม่มีไฟล์) มิฉะนั้น `409 ACCEPTANCE_CONFLICT`; หลักฐานใน DB เก็บ `document_sha256` (ค่าที่ยืนยัน) |
```
- [ ] **Step 9: แก้ `docs/03-contracts/permission-catalog.md`**
```text
>>> แทนที่:
| Quotation | `quotations.read` | ดู/พิมพ์เอกสาร Quotation ที่ออกแล้ว (Customer-safe Document) | Branch/Project |
>>> ด้วย:
| Quotation | `quotations.read` | ดู/พิมพ์เอกสาร Quotation ที่ออกแล้ว (Customer-safe Document) และดาวน์โหลดไฟล์ PDF ที่เก็บไว้ | Branch/Project |
```
- [ ] **Step 10: ตรวจลิงก์ภายในของไฟล์ที่แก้ (ADR 0023 ลิงก์ไป `quotation-pdf-verification.md` ซึ่งสร้างใน Task 10 จึงตรวจ ADR ที่ Task 10)**
```bash
cd /Users/syaco/Documents/development/tan-erp && python3 - <<'PY'
import re, pathlib
files = ["docs/03-contracts/quotation-document-api-contract.md", "docs/adr/README.md", "docs/README.md", "docs/03-contracts/quotation-lifecycle-api-contract.md", "docs/03-contracts/external-acceptance-api-contract.md"]
for f in files:
    base = pathlib.Path(f).parent
    for target in re.findall(r"\]\(([^)#]+\.md)(?:#[^)]*)?\)", pathlib.Path(f).read_text(encoding="utf-8")):
        if not target.startswith("http") and not (base / target).resolve().exists():
            print("MISSING", f, target)
print("LINK_CHECK_DONE")
PY
```
- [ ] **Step 11: Commit**
```bash
git add docs CONTEXT.md
git commit -m "docs(commercial): add ADR 0023 and quotation document contract

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 2: Renderer foundation: QuestPDF, ฟอนต์ Sarabun ฝังใน assembly, allowlist model, `IQuotationDocumentRenderer`

**Files:** Modify: `backend/Directory.Packages.props`, `backend/src/TanErp.Infrastructure/TanErp.Infrastructure.csproj`; Create: `backend/src/TanErp.Infrastructure/Documents/Fonts/{Sarabun-Regular.ttf,Sarabun-Bold.ttf,OFL.txt}` (ดาวน์โหลด+ตรวจ SHA-256), `backend/src/TanErp.Application/Commercial/Documents/QuotationPdfModel.cs`, `backend/src/TanErp.Infrastructure/Documents/QuestPdfRuntime.cs`, `backend/src/TanErp.Infrastructure/Documents/QuestPdfQuotationRenderer.cs`; Create (tests): `backend/tests/TanErp.UnitTests/Commercial/QuotationPdfModelAllowlistTests.cs`, `backend/tests/TanErp.IntegrationTests/Documents/QuestPdfQuotationRendererTests.cs`

ยังไม่มีผู้เรียก renderer ใน Task นี้ (ต่อสายใน Task 4) จึง commit และ build ได้โดยไม่กระทบพฤติกรรมเดิม. ไฟล์ฟอนต์เป็นไฟล์ binary ที่ implementer ดาวน์โหลดเอง (ห้าม Write/แก้เนื้อหา) แล้วต้องผ่าน `shasum -c`.

- [ ] **Step 1: ดาวน์โหลดฟอนต์ Sarabun (SIL OFL 1.1) ที่ pin commit และตรวจ SHA-256**
```bash
cd /Users/syaco/Documents/development/tan-erp
F=backend/src/TanErp.Infrastructure/Documents/Fonts && mkdir -p "$F"
C=88058c714073e72bb1b32baffc2d4ce9bc73944c   # google/fonts commit that last touched ofl/sarabun
for f in Sarabun-Regular.ttf Sarabun-Bold.ttf OFL.txt; do curl -fsSL -o "$F/$f" "https://raw.githubusercontent.com/google/fonts/$C/ofl/sarabun/$f"; done
(cd "$F" && shasum -a 256 -c - <<'SUMS'
226d4f368fbc0457990ddef2692679badfd2c1a4e89e5ac4d43c10ba7743b2f1  Sarabun-Regular.ttf
d38308ca27d067b9a1b79006337c1f9a66c29aa016d96a9d88d3801da7fa83b9  Sarabun-Bold.ttf
b26cae1321380296ba8311b632a397d5eac11b47197f9d0aa0b9310f1531ad60  OFL.txt
SUMS
)
ls -l "$F"
```
- [ ] **Step 2: แก้ `backend/Directory.Packages.props`** — ปักเวอร์ชัน QuestPDF (ตรวจแล้วว่ามีจริงบน NuGet: 2026.9.1 เผยแพร่ 2026-09-25, รองรับ net10.0)
```text
>>> หลัง: <PackageVersion Include="FirebaseAdmin" Version="3.6.0" />
    <!-- QuestPDF: Community license is free only below the vendor revenue threshold; see ADR 0023 before production. -->
    <PackageVersion Include="QuestPDF" Version="2026.9.1" />
```
- [ ] **Step 3: แก้ `backend/src/TanErp.Infrastructure/TanErp.Infrastructure.csproj`** — ฝังฟอนต์+ไฟล์ license เป็น EmbeddedResource (รูปแบบเดียวกับ `thailand_addresses.json.gz`)
```text
>>> หลัง: <PackageReference Include="FirebaseAdmin" />
    <PackageReference Include="QuestPDF" />
>>> หลัง: <EmbeddedResource Include="MasterData\Seed\thailand_addresses.json.gz" />
    <EmbeddedResource Include="Documents\Fonts\*.ttf;Documents\Fonts\OFL.txt" LogicalName="TanErp.Infrastructure.Documents.Fonts.%(Filename)%(Extension)" />
```
- [ ] **Step 4: สร้าง `backend/src/TanErp.Application/Commercial/Documents/QuotationPdfModel.cs`**
```csharp
namespace TanErp.Application.Commercial.Documents;

// The renderer sees ONLY these types. They are the allowlist: no cost, margin, markup, supplier or internal id exists here.
// QuotationPdfModelAllowlistTests reflects over this namespace and fails when a property is added without review.

public sealed record QuotationPdfAddress(string AddressLine1, string? Subdistrict, string? District, string? Province, string? PostalCode);

public sealed record QuotationPdfIssuer(
    string NameTh, string? NameEn, string? BranchNameTh, string? BranchNameEn,
    string? TaxIdentifier, string? TaxBranchCode, string? AddressTh, string? AddressEn, string? Phone);

public sealed record QuotationPdfCustomer(
    string DisplayNameTh, string? DisplayNameEn, string? LegalName, string? TaxIdentifier, string? BranchCode, QuotationPdfAddress? Address);

public sealed record QuotationPdfWorkItem(
    string Code, string DescriptionTh, string? DescriptionEn, decimal Quantity, string UnitCode, decimal UnitPrice, decimal LineTotal);

public sealed record QuotationPdfSection(
    string Code, string NameTh, string? NameEn, decimal Subtotal, IReadOnlyList<QuotationPdfWorkItem> WorkItems);

public sealed record QuotationPdfTotals(decimal Subtotal, decimal DiscountAmount, decimal NetBeforeTax, decimal TaxAmount, decimal GrandTotal);

public sealed record QuotationPdfModel(
    string Number,
    DateTimeOffset IssuedAtUtc,
    string Currency,
    string? SupersedesNumber,
    QuotationPdfIssuer Issuer,
    QuotationPdfCustomer Customer,
    IReadOnlyList<QuotationPdfSection> Sections,
    QuotationPdfTotals Totals);

/// <summary>Turns the allowlisted model into PDF bytes. Pure and synchronous: no I/O, no clock, no database.</summary>
public interface IQuotationDocumentRenderer
{
    /// <summary>Recorded with the stored artifact, e.g. "questpdf-2026.9.1/quotation-v1".</summary>
    string Version { get; }

    byte[] Render(QuotationPdfModel model);
}
```
- [ ] **Step 5: สร้าง `backend/src/TanErp.Infrastructure/Documents/QuestPdfRuntime.cs`**
```csharp
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace TanErp.Infrastructure.Documents;

/// <summary>
/// Process-wide QuestPDF setup, run once: the license type, font ownership (embedded Sarabun only, never system fonts) and
/// fail-fast when a font family is missing. QuestPDF settings are static, so they cannot be per-request.
/// </summary>
public static class QuestPdfRuntime
{
    public const string FontFamily = "Sarabun";
    public const string FontResourcePrefix = "TanErp.Infrastructure.Documents.Fonts.";
    public static readonly IReadOnlyList<string> FontFiles = ["Sarabun-Regular.ttf", "Sarabun-Bold.ttf"];

    private static readonly object Gate = new();
    private static bool _initialized;

    /// <summary>Only the three production license types are accepted; "Evaluation" or a typo fails fast.</summary>
    public static LicenseType ParseLicense(string? licenseType) => licenseType switch
    {
        "Community" => LicenseType.Community,
        "Professional" => LicenseType.Professional,
        "Enterprise" => LicenseType.Enterprise,
        _ => throw new InvalidOperationException($"Unsupported Documents:QuestPdf:LicenseType '{licenseType}'. Use Community, Professional or Enterprise.")
    };

    public static void EnsureInitialized(string licenseType)
    {
        lock (Gate)
        {
            if (_initialized) return;

            // The ONLY place the license is chosen. Community is free only below the vendor's revenue threshold (ADR 0023).
            QuestPDF.Settings.License = ParseLicense(licenseType);
            QuestPDF.Settings.UseSystemFonts = false;
            QuestPDF.Settings.ThrowOnMissingFontFamilies = true;

            var assembly = typeof(QuestPdfRuntime).Assembly;
            foreach (var file in FontFiles)
            {
                using var stream = assembly.GetManifestResourceStream(FontResourcePrefix + file)
                    ?? throw new InvalidOperationException($"Embedded font '{file}' is missing from the Infrastructure assembly.");
                FontManager.RegisterFontFromStream(stream);
            }

            _initialized = true;
        }
    }
}
```
- [ ] **Step 6: สร้าง `backend/src/TanErp.Infrastructure/Documents/QuestPdfQuotationRenderer.cs`**
```csharp
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TanErp.Application.Commercial.Documents;

namespace TanErp.Infrastructure.Documents;

public sealed class QuestPdfQuotationRenderer : IQuotationDocumentRenderer
{
    public const string LayoutVersion = "quotation-v1";
    private const string Navy = "#0B3056";
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    public QuestPdfQuotationRenderer(IConfiguration configuration, ILogger<QuestPdfQuotationRenderer> logger)
    {
        var licenseType = configuration["Documents:QuestPdf:LicenseType"] ?? "Community";
        QuestPdfRuntime.EnsureInitialized(licenseType);
        // The business must confirm the license before production (ADR 0023). A warning keeps issuing available.
        if (string.Equals(configuration["ASPNETCORE_ENVIRONMENT"], "Production", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(configuration["Documents:QuestPdf:LicenseConfirmed"], "true", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("QuestPDF license type {LicenseType} is not confirmed by the business (Documents:QuestPdf:LicenseConfirmed). See ADR 0023.", licenseType);
        }
    }

    public string Version => $"questpdf-{typeof(QuestPDF.Settings).Assembly.GetName().Version?.ToString(3)}/{LayoutVersion}";

    public byte[] Render(QuotationPdfModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var document = Document.Create(container => container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(32);
            page.DefaultTextStyle(style => style.FontFamily(QuestPdfRuntime.FontFamily).FontSize(9.5f));
            page.Header().AlignRight().Text(model.Number).FontSize(8).FontColor(Colors.Grey.Darken1);
            page.Content().Element(content => Compose(content, model));
            page.Footer().AlignCenter().Text(text =>
            {
                text.DefaultTextStyle(style => style.FontSize(8).FontColor(Colors.Grey.Darken1));
                text.Span("หน้า / Page ");
                text.CurrentPageNumber();
                text.Span(" / ");
                text.TotalPages();
            });
        }));
        document.WithMetadata(new DocumentMetadata
        {
            Title = model.Number, Author = model.Issuer.NameTh, Creator = "tan-erp", Language = "th-TH",
            CreationDate = model.IssuedAtUtc, ModifiedDate = model.IssuedAtUtc
        });
        return document.GeneratePdf();
    }

    private static string Money(decimal value) => value.ToString("#,##0.00", Invariant);

    private static string? Join(string separator, params string?[] parts)
    {
        var present = parts.Where(part => !string.IsNullOrWhiteSpace(part)).ToList();
        return present.Count == 0 ? null : string.Join(separator, present);
    }

    private static void Lines(ColumnDescriptor column, params string?[] lines)
    {
        foreach (var line in lines.Where(line => !string.IsNullOrWhiteSpace(line))) column.Item().Text(line);
    }

    private static void Compose(IContainer container, QuotationPdfModel model)
    {
        var issued = model.IssuedAtUtc.ToOffset(TimeSpan.FromHours(7)).ToString("dd/MM/yyyy", Invariant); // Thailand has no DST
        var issuer = model.Issuer;
        container.Column(column =>
        {
            column.Spacing(8);
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text(issuer.NameTh).Bold().FontSize(12);
                    Lines(left, issuer.NameEn, Join(" / ", issuer.BranchNameTh, issuer.BranchNameEn),
                        Join("  ", issuer.TaxIdentifier is null ? null : $"เลขประจำตัวผู้เสียภาษี / Tax ID {issuer.TaxIdentifier}",
                            issuer.TaxBranchCode is null ? null : $"สาขา / Branch {issuer.TaxBranchCode}"),
                        issuer.AddressTh, issuer.AddressEn, issuer.Phone is null ? null : $"โทร / Tel {issuer.Phone}");
                });
                row.ConstantItem(190).Column(right =>
                {
                    right.Item().AlignRight().Text("ใบเสนอราคา / Quotation").Bold().FontSize(15).FontColor(Navy);
                    right.Item().AlignRight().Text($"เลขที่ / No. {model.Number}");
                    right.Item().AlignRight().Text($"วันที่ / Date {issued}");
                    right.Item().AlignRight().Text($"สกุลเงิน / Currency {model.Currency}");
                    if (model.SupersedesNumber is not null) right.Item().AlignRight().Text($"แทนที่ฉบับเลขที่ / Replaces {model.SupersedesNumber}").FontSize(8.5f);
                });
            });
            column.Item().LineHorizontal(1.5f).LineColor(Navy);
            column.Item().Border(0.75f).BorderColor(Colors.Grey.Medium).Padding(6).Column(box =>
            {
                var customer = model.Customer;
                box.Item().Text("ลูกค้า / Customer").Bold().FontColor(Navy);
                box.Item().Text(customer.DisplayNameTh).Bold();
                var address = customer.Address is null ? null : Join(" ", customer.Address.AddressLine1, customer.Address.Subdistrict, customer.Address.District, customer.Address.Province, customer.Address.PostalCode);
                Lines(box, customer.DisplayNameEn, customer.LegalName is null ? null : $"ชื่อตามกฎหมาย / Legal name: {customer.LegalName}",
                    Join("  ", customer.TaxIdentifier is null ? null : $"เลขประจำตัวผู้เสียภาษี / Tax ID {customer.TaxIdentifier}", customer.BranchCode is null ? null : $"สาขา / Branch {customer.BranchCode}"),
                    address is null ? null : $"ที่อยู่ / Address: {address}");
            });
            foreach (var section in model.Sections) column.Item().Element(content => ComposeSection(content, section));
            column.Item().ShowEntire().AlignRight().Width(240).Column(totals =>
            {
                void Line(string label, decimal value, bool strong = false) => totals.Item().Row(row =>
                {
                    var name = row.RelativeItem().Text(label);
                    var amount = row.ConstantItem(90).AlignRight().Text(Money(value));
                    if (strong) { name.Bold(); amount.Bold(); }
                });
                Line("รวมเป็นเงิน / Subtotal", model.Totals.Subtotal);
                if (model.Totals.DiscountAmount > 0) Line("ส่วนลด / Discount", model.Totals.DiscountAmount);
                Line("ยอดหลังหักส่วนลด / Net before tax", model.Totals.NetBeforeTax);
                Line("ภาษีมูลค่าเพิ่ม / VAT", model.Totals.TaxAmount);
                totals.Item().PaddingTop(2).LineHorizontal(1.5f).LineColor(Navy);
                Line($"ยอดรวมสุทธิ / Grand total ({model.Currency})", model.Totals.GrandTotal, strong: true);
            });
        });
    }

    private static IContainer Cell(IContainer container) =>
        container.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).PaddingVertical(3).PaddingHorizontal(2);

    private static void ComposeSection(IContainer container, QuotationPdfSection section)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(56); columns.RelativeColumn(); columns.ConstantColumn(44);
                columns.ConstantColumn(40); columns.ConstantColumn(64); columns.ConstantColumn(68);
            });
            table.Header(header =>
            {
                header.Cell().ColumnSpan(6).Background(Colors.Grey.Lighten3).Padding(3).Text(text =>
                {
                    text.Span($"{section.Code}  {section.NameTh}").Bold();
                    if (!string.IsNullOrWhiteSpace(section.NameEn)) text.Span($" / {section.NameEn}");
                });
                foreach (var (label, right) in new[] { ("รหัส / Code", false), ("รายละเอียด / Description", false), ("จำนวน / Qty", true), ("หน่วย / Unit", false), ("ราคา/หน่วย / Unit price", true), ("รวม / Amount", true) })
                {
                    var cell = header.Cell().Element(Cell);
                    (right ? cell.AlignRight() : cell).Text(label).Bold();
                }
            });
            foreach (var item in section.WorkItems)
            {
                table.Cell().Element(Cell).Text(item.Code);
                table.Cell().Element(Cell).Column(description =>
                {
                    description.Item().Text(item.DescriptionTh);
                    if (!string.IsNullOrWhiteSpace(item.DescriptionEn)) description.Item().Text(item.DescriptionEn).FontColor(Colors.Grey.Darken2);
                });
                table.Cell().Element(Cell).AlignRight().Text(item.Quantity.ToString("#,##0.####", Invariant));
                table.Cell().Element(Cell).Text(item.UnitCode);
                table.Cell().Element(Cell).AlignRight().Text(Money(item.UnitPrice));
                table.Cell().Element(Cell).AlignRight().Text(Money(item.LineTotal));
            }
            table.Cell().ColumnSpan(5).Element(Cell).AlignRight().Text("รวมหมวด / Section subtotal").Bold();
            table.Cell().Element(Cell).AlignRight().Text(Money(section.Subtotal)).Bold();
        });
    }
}
```
- [ ] **Step 7: สร้าง `backend/tests/TanErp.UnitTests/Commercial/QuotationPdfModelAllowlistTests.cs`**
```csharp
using System.Reflection;
using TanErp.Application.Commercial.Documents;
using Xunit;

namespace TanErp.UnitTests.Commercial;

/// <summary>The renderer input is a typed allowlist: adding a property is a conscious change here too, and no name may suggest internal pricing data.</summary>
public class QuotationPdfModelAllowlistTests
{
    private static readonly IReadOnlyDictionary<string, string[]> Allowed = new Dictionary<string, string[]>
    {
        [nameof(QuotationPdfModel)] = ["Number", "IssuedAtUtc", "Currency", "SupersedesNumber", "Issuer", "Customer", "Sections", "Totals"],
        [nameof(QuotationPdfIssuer)] = ["NameTh", "NameEn", "BranchNameTh", "BranchNameEn", "TaxIdentifier", "TaxBranchCode", "AddressTh", "AddressEn", "Phone"],
        [nameof(QuotationPdfCustomer)] = ["DisplayNameTh", "DisplayNameEn", "LegalName", "TaxIdentifier", "BranchCode", "Address"],
        [nameof(QuotationPdfAddress)] = ["AddressLine1", "Subdistrict", "District", "Province", "PostalCode"],
        [nameof(QuotationPdfSection)] = ["Code", "NameTh", "NameEn", "Subtotal", "WorkItems"],
        [nameof(QuotationPdfWorkItem)] = ["Code", "DescriptionTh", "DescriptionEn", "Quantity", "UnitCode", "UnitPrice", "LineTotal"],
        [nameof(QuotationPdfTotals)] = ["Subtotal", "DiscountAmount", "NetBeforeTax", "TaxAmount", "GrandTotal"]
    };

    private static readonly string[] Forbidden =
        ["cost", "margin", "markup", "profit", "supplier", "vendor", "rule", "reason", "approval", "userid", "organizationid", "estimateid", "revisionid", "snapshot", "hash", "note"];

    [Fact]
    public void EveryModelType_ExposesExactlyTheReviewedProperties_AndNoInternalPricingNames()
    {
        var types = typeof(QuotationPdfModel).Assembly.GetTypes()
            .Where(type => type.Namespace == typeof(QuotationPdfModel).Namespace && type.Name.StartsWith("QuotationPdf", StringComparison.Ordinal) && type.IsClass).ToList();
        Assert.Equal(Allowed.Keys.OrderBy(name => name), types.Select(type => type.Name).OrderBy(name => name));
        foreach (var type in types)
        {
            var names = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.Name != "EqualityContract").Select(p => p.Name).ToList();
            Assert.Equal(Allowed[type.Name].OrderBy(name => name), names.OrderBy(name => name));
            Assert.DoesNotContain(names, name => Forbidden.Any(fragment => name.Contains(fragment, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
```
- [ ] **Step 8: สร้าง `backend/tests/TanErp.IntegrationTests/Documents/QuestPdfQuotationRendererTests.cs`**
```csharp
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using QuestPDF.Infrastructure;
using TanErp.Application.Commercial.Documents;
using TanErp.Infrastructure.Documents;
using Xunit;

namespace TanErp.IntegrationTests.Documents;

/// <summary>Pure rendering tests: no database, no Docker.</summary>
public class QuestPdfQuotationRendererTests
{
    private static QuotationPdfModel Model(int items, int sections, string descriptionTh) => new(
        "QT-2026-0001", new DateTimeOffset(2026, 10, 12, 3, 0, 0, TimeSpan.Zero), "THB", null,
        new QuotationPdfIssuer("บริษัท ตัวอย่าง จำกัด", "Example Co., Ltd.", "สำนักงานใหญ่", "Head office", "0105536000001", "00000", "99 ถนนสุขุมวิท กรุงเทพมหานคร", "99 Sukhumvit Rd, Bangkok", "02-000-0000"),
        new QuotationPdfCustomer("ลูกค้าทดสอบ", "Test Customer", "บริษัท ลูกค้า จำกัด", "0105536000002", "00000", new QuotationPdfAddress("1 ถนนหนึ่ง", "แขวงหนึ่ง", "เขตหนึ่ง", "กรุงเทพมหานคร", "10110")),
        Enumerable.Range(1, sections).Select(s => new QuotationPdfSection($"S{s}", $"งานระบบ {s}", s % 2 == 0 ? null : $"Systems {s}", 1000m * items,
            Enumerable.Range(1, items).Select(i => new QuotationPdfWorkItem($"W{s}{i:000}", descriptionTh, i % 2 == 0 ? null : "Install air conditioning", 1.5m, "ชุด", 666.67m, 1000m)).ToList())).ToList(),
        new QuotationPdfTotals(1000m * items * sections, 0m, 1000m * items * sections, 70m * items * sections, 1070m * items * sections));

    private static byte[] Render(QuotationPdfModel model) =>
        new QuestPdfQuotationRenderer(new ConfigurationBuilder().Build(), NullLogger<QuestPdfQuotationRenderer>.Instance).Render(model);

    private static int Pages(byte[] pdf) => Regex.Matches(Encoding.Latin1.GetString(pdf), @"/Type\s*/Page[^s]").Count;

    [Fact]
    public void Render_ThaiAndEnglish_ProducesOnePage_AndManyMaximumLengthRowsPaginate()
    {
        var small = Render(Model(3, 1, "ติดตั้งระบบปรับอากาศ"));
        Assert.StartsWith("%PDF-", Encoding.ASCII.GetString(small, 0, 5));
        Assert.Equal(1, Pages(small));

        // EstimateWorkItem.DescriptionTh is at most 500 characters; rows may break across pages.
        Assert.True(Pages(Render(Model(40, 3, new string('ก', 500)))) >= 5);
    }

    [Fact]
    public void Version_AndLicenseAndEmbeddedFonts_AreWhatTheAdrSays()
    {
        Assert.Matches(@"^questpdf-\d+\.\d+\.\d+/quotation-v1$", new QuestPdfQuotationRenderer(new ConfigurationBuilder().Build(), NullLogger<QuestPdfQuotationRenderer>.Instance).Version);
        Assert.Equal(LicenseType.Community, QuestPdfRuntime.ParseLicense("Community"));
        Assert.Equal(LicenseType.Professional, QuestPdfRuntime.ParseLicense("Professional"));
        Assert.Throws<InvalidOperationException>(() => QuestPdfRuntime.ParseLicense("Evaluation"));
        Assert.Throws<InvalidOperationException>(() => QuestPdfRuntime.ParseLicense(null));

        var resources = typeof(QuestPdfRuntime).Assembly.GetManifestResourceNames();
        foreach (var file in QuestPdfRuntime.FontFiles.Append("OFL.txt")) Assert.Contains(QuestPdfRuntime.FontResourcePrefix + file, resources);
    }
}
```
- [ ] **Step 9: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
```
Expected: `0 Warning(s)` `0 Error(s)` (TreatWarningsAsErrors เปิดอยู่; โปรเจกต์ test อยู่ใน solution จึงถูก compile ด้วย)
- [ ] **Step 10: Commit**
```bash
git add backend/Directory.Packages.props backend/src/TanErp.Application/Commercial/Documents backend/src/TanErp.Infrastructure/TanErp.Infrastructure.csproj backend/src/TanErp.Infrastructure/Documents backend/tests/TanErp.UnitTests/Commercial backend/tests/TanErp.IntegrationTests/Documents
git commit -m "feat(commercial): add quotation pdf renderer with embedded Thai fonts

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 3: Persistence: `QuotationDocument` (ไม่เปลี่ยนแปลงได้), hash ที่ลูกค้ายืนยันในหลักฐาน, migration

**Files:** Create: `backend/src/TanErp.Domain/Commercial/QuotationDocument.cs`, `backend/src/TanErp.Infrastructure/Persistence/Configurations/QuotationDocumentConfiguration.cs`, migration `*_AddQuotationDocuments.cs` + `.Designer.cs` (generate); Modify: `backend/src/TanErp.Domain/Commercial/QuotationAcceptanceLink.cs`, `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`, `backend/src/TanErp.Infrastructure/Persistence/Configurations/QuotationAcceptanceConfigurations.cs`, `AppDbContextModelSnapshot.cs` (generate); Create (test): `backend/tests/TanErp.UnitTests/Commercial/QuotationDocumentTests.cs`

**ห้าม apply migration** — controller เป็นผู้ apply. migration ที่ generate ต้องเพิ่มเฉพาะตาราง `commercial.quotation_documents` (unique `quotation_id` และ `file_id`, FK แบบ composite `(file_id, organization_id)` ไป `files.uploaded_files`, check sha256/byte_count) และคอลัมน์ `document_sha256` (nullable) บน `commercial.quotation_acceptance_evidences`. ไม่มีข้อมูลเดิมต้อง backfill (quotation เดิมคือสถานะ "ยังไม่มีเอกสาร" — ดู D7).

- [ ] **Step 1: สร้าง `backend/src/TanErp.Domain/Commercial/QuotationDocument.cs`**
```csharp
using TanErp.Domain.Common;

namespace TanErp.Domain.Commercial;

/// <summary>
/// The immutable PDF artifact of one quotation: which stored file, its SHA-256 and which renderer made it. There is no mutator on purpose;
/// a changed quotation is a new quotation (amendment) with its own document. The file bytes live in the Files module (UploadedFile).
/// </summary>
public class QuotationDocument : Entity
{
    public Guid OrganizationId { get; private set; }
    public Guid QuotationId { get; private set; }
    public Guid FileId { get; private set; }
    public string Sha256 { get; private set; } = string.Empty;
    public long ByteCount { get; private set; }
    public string RendererVersion { get; private set; } = string.Empty;
    public DateTimeOffset RenderedAtUtc { get; private set; }
    public Guid RenderedByUserId { get; private set; }

    protected QuotationDocument() { }

    public QuotationDocument(
        Guid id, Guid organizationId, Guid quotationId, Guid fileId, string sha256, long byteCount,
        string rendererVersion, DateTimeOffset renderedAtUtc, Guid renderedByUserId) : base(id)
    {
        if (id == Guid.Empty || organizationId == Guid.Empty || quotationId == Guid.Empty || fileId == Guid.Empty || renderedByUserId == Guid.Empty)
            throw new ArgumentException("Document, organization, quotation, file and actor identifiers are required.");
        if (sha256 is null || sha256.Length != 64 || !sha256.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f'))
            throw new ArgumentException("SHA-256 must be 64 lowercase hexadecimal characters.", nameof(sha256));
        if (byteCount <= 0) throw new ArgumentOutOfRangeException(nameof(byteCount), "A document cannot be empty.");
        if (string.IsNullOrWhiteSpace(rendererVersion)) throw new ArgumentException("Renderer version is required.", nameof(rendererVersion));

        OrganizationId = organizationId;
        QuotationId = quotationId;
        FileId = fileId;
        Sha256 = sha256;
        ByteCount = byteCount;
        RendererVersion = rendererVersion.Trim();
        RenderedAtUtc = renderedAtUtc.ToUniversalTime();
        RenderedByUserId = renderedByUserId;
    }
}
```
- [ ] **Step 2: แก้ `backend/src/TanErp.Domain/Commercial/QuotationAcceptanceLink.cs`** — เพิ่ม property ใน `QuotationAcceptanceEvidence` (anchor ไม่มี `?` จึงไม่ชน `AcceptedAtUtc` ของ Link); เพิ่มพารามิเตอร์ท้ายแบบ optional ใน ctor ของ Evidence
```text
>>> หลัง: public DateTimeOffset AcceptedAtUtc { get; private set; }

    /// <summary>SHA-256 of the quotation PDF the customer attested to; null when the quotation had no stored PDF (issued before G-04).</summary>
    public string? DocumentSha256 { get; private set; }
>>> แทนที่:
string? userAgent, DateTimeOffset now) : base(id)
>>> ด้วย:
string? userAgent, DateTimeOffset now, string? documentSha256 = null) : base(id)
>>> หลัง: UserAgent = userAgent;
        DocumentSha256 = string.IsNullOrWhiteSpace(documentSha256) ? null : documentSha256.Trim();
```
- [ ] **Step 3: สร้าง `backend/src/TanErp.Infrastructure/Persistence/Configurations/QuotationDocumentConfiguration.cs`**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TanErp.Domain.Commercial;
using TanErp.Domain.Files;
using TanErp.Domain.Organization;

namespace TanErp.Infrastructure.Persistence.Configurations;

public class QuotationDocumentConfiguration : IEntityTypeConfiguration<QuotationDocument>
{
    public void Configure(EntityTypeBuilder<QuotationDocument> builder)
    {
        builder.ToTable("quotation_documents", "commercial", t =>
        {
            t.HasCheckConstraint("ck_quotation_documents_sha256", "sha256 ~ '^[0-9a-f]{64}$'");
            t.HasCheckConstraint("ck_quotation_documents_byte_count", "byte_count > 0");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
        builder.Property(x => x.QuotationId).HasColumnName("quotation_id").IsRequired();
        builder.Property(x => x.FileId).HasColumnName("file_id").IsRequired();
        builder.Property(x => x.Sha256).HasColumnName("sha256").HasMaxLength(64).IsRequired();
        builder.Property(x => x.ByteCount).HasColumnName("byte_count").IsRequired();
        builder.Property(x => x.RendererVersion).HasColumnName("renderer_version").HasMaxLength(64).IsRequired();
        builder.Property(x => x.RenderedAtUtc).HasColumnName("rendered_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.RenderedByUserId).HasColumnName("rendered_by_user_id").IsRequired();

        // One immutable document per quotation (a quotation is bound to exactly one estimate revision); one file backs one document.
        builder.HasIndex(x => x.QuotationId).IsUnique();
        builder.HasIndex(x => x.FileId).IsUnique();
        builder.HasIndex(x => new { x.OrganizationId, x.QuotationId });

        builder.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Quotation>().WithMany().HasForeignKey(x => x.QuotationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(x => new { x.FileId, x.OrganizationId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```
- [ ] **Step 4: แก้ `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs`**
```text
>>> หลัง: public DbSet<TanErp.Domain.Commercial.Quotation> Quotations => Set<TanErp.Domain.Commercial.Quotation>();
    public DbSet<TanErp.Domain.Commercial.QuotationDocument> QuotationDocuments => Set<TanErp.Domain.Commercial.QuotationDocument>();
```
- [ ] **Step 5: แก้ `backend/src/TanErp.Infrastructure/Persistence/Configurations/QuotationAcceptanceConfigurations.cs`**
```text
>>> หลัง: builder.Property(x => x.AcceptedAtUtc).HasColumnName("accepted_at_utc").HasColumnType("timestamptz").IsRequired();
        builder.Property(x => x.DocumentSha256).HasColumnName("document_sha256").HasMaxLength(64);
```
- [ ] **Step 6: สร้าง `backend/tests/TanErp.UnitTests/Commercial/QuotationDocumentTests.cs`**
```csharp
using System.Reflection;
using TanErp.Domain.Commercial;
using Xunit;

namespace TanErp.UnitTests.Commercial;

public class QuotationDocumentTests
{
    private static readonly string Hash = new('a', 64);

    private static QuotationDocument Create(string? sha256 = null, long byteCount = 1024) =>
        new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), sha256 ?? Hash, byteCount, "questpdf-2026.9.1/quotation-v1", DateTimeOffset.UtcNow, Guid.NewGuid());

    [Fact]
    public void Create_RejectsAMalformedHashAndAnEmptyFile()
    {
        Assert.Equal(Hash, Create().Sha256);
        Assert.Throws<ArgumentException>(() => Create("abc"));
        Assert.Throws<ArgumentException>(() => Create(new string('A', 64))); // upper case is not the canonical form
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(byteCount: 0));
    }

    [Fact]
    public void ADocumentIsImmutable_ItHasNoPublicSetterOrMutatingMethod()
    {
        var type = typeof(QuotationDocument);
        Assert.All(type.GetProperties(BindingFlags.Public | BindingFlags.Instance), property =>
            Assert.True(property.SetMethod is null || !property.SetMethod.IsPublic, $"{property.Name} must not have a public setter"));
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(method => !method.IsSpecialName).Select(method => method.Name).ToList();
        Assert.True(methods.Count == 0, $"A document has no mutating method, found: {string.Join(", ", methods)}");
    }
}
```
- [ ] **Step 7: Generate migration (ครั้งเดียว — ถ้าต้องแก้ ให้ `dotnet ef migrations remove` แล้ว generate ใหม่; ห้าม `database update`)**
```bash
cd /Users/syaco/Documents/development/tan-erp && ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" \
dotnet ef migrations add AddQuotationDocuments \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api --output-dir Persistence/Migrations
```
- [ ] **Step 8: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
```
Expected: `0 Warning(s)` `0 Error(s)` (TreatWarningsAsErrors เปิดอยู่; โปรเจกต์ test อยู่ใน solution จึงถูก compile ด้วย); `has-pending-model-changes` ต้องรายงาน `No changes have been made to the model since the last migration.` ถ้าไม่ใช่ ให้ลบ migration แล้ว generate ใหม่
- [ ] **Step 9: Commit**
```bash
git add backend/src/TanErp.Domain/Commercial backend/src/TanErp.Infrastructure/Persistence backend/tests/TanErp.UnitTests/Commercial/QuotationDocumentTests.cs
git commit -m "feat(commercial): persist immutable quotation documents

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 4: Ports, handler และ Infrastructure: model builder, writer (render ครั้งเดียว+เก็บไฟล์), store (อ่านและตรวจ hash), DI

**Files:** Create: `backend/src/TanErp.Application/Commercial/Documents/{QuotationDocumentPorts.cs,QuotationDocumentHandler.cs}`, `backend/src/TanErp.Infrastructure/Documents/{QuotationPdfModelBuilder.cs,QuotationDocumentWriter.cs,QuotationDocumentStore.cs}`; Modify: `backend/src/TanErp.Application/Common/Security/Sha256Hex.cs`, `backend/src/TanErp.Infrastructure/Persistence/Estimates/QuotationBillingSnapshotBuilder.cs`, `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs` (ลบ record ซ้ำ), `backend/src/TanErp.Api/Program.cs`

Reuse First: record ของ billing snapshot ที่เคยเป็น `private` ใน `EstimateStore` (ใช้ใน `GetQuotationDocumentAsync`, EstimateStore.cs:1846) ย้ายไปเป็น `internal` ใน `QuotationBillingSnapshotBuilder.cs` (namespace เดียวกัน) เพื่อให้ PDF model ใช้ชนิดเดียวกันโดยไม่คัดลอก; ผู้ใช้เดิมใน `EstimateStore` compile ต่อได้โดยไม่แก้โค้ดอื่น.

- [ ] **Step 1: แก้ `backend/src/TanErp.Application/Common/Security/Sha256Hex.cs`**
```text
>>> แทนที่:
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
>>> ด้วย:
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static string ComputeBytes(byte[] content)
    {
        ArgumentNullException.ThrowIfNull(content);
        return Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
    }
}
```
- [ ] **Step 2: สร้าง `backend/src/TanErp.Application/Commercial/Documents/QuotationDocumentPorts.cs`**
```csharp
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Domain.Commercial;

namespace TanErp.Application.Commercial.Documents;

/// <summary>What is recorded about a stored quotation PDF. The storage path and file id are never exposed.</summary>
public sealed record QuotationDocumentSummary(string Sha256, long ByteCount, string RendererVersion, DateTimeOffset RenderedAtUtc);

/// <summary>The stored bytes (hash-verified on read) with the name to offer to the client.</summary>
public sealed record QuotationDocumentFile(byte[] Content, string FileName, QuotationDocumentSummary Summary);

/// <summary>Raised when the renderer fails. The caller's transaction rolls back, so nothing is half-issued; surfaces as a 500.</summary>
public sealed class QuotationDocumentRenderException(string message, Exception inner) : Exception(message, inner);

/// <summary>
/// Write side, used inside an issue/amend unit of work. The quotation may still be unsaved (its estimate revision, organization and
/// branch already exist). Renders once, stores the blob and ADDS the file, document and audit rows to the caller's DbContext without
/// saving, so the caller's one SaveChanges commits the quotation and its document together.
/// </summary>
public interface IQuotationDocumentWriter
{
    Task<Result<QuotationDocumentSummary>> StageAsync(Quotation quotation, Guid actorUserId, string traceId, CancellationToken cancellationToken = default);
}

public interface IQuotationDocumentStore
{
    /// <summary>Organization and branch scoped. Out of scope or unknown: RESOURCE_NOT_FOUND. No document: QUOTATION_DOCUMENT_NOT_AVAILABLE.</summary>
    Task<Result<QuotationDocumentFile>> OpenAsync(RequestAccessContext access, Guid quotationId, CancellationToken cancellationToken = default);

    Task<QuotationDocumentSummary?> FindSummaryAsync(Guid organizationId, Guid quotationId, CancellationToken cancellationToken = default);

    /// <summary>For the public acceptance link only (the token already proved access). Null when there is no document.</summary>
    Task<QuotationDocumentFile?> OpenForAcceptanceAsync(Guid organizationId, Guid quotationId, CancellationToken cancellationToken = default);
}
```
- [ ] **Step 3: สร้าง `backend/src/TanErp.Application/Commercial/Documents/QuotationDocumentHandler.cs`**
```csharp
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Commercial.Documents;

/// <summary>Download of the stored quotation PDF needs quotations.read.</summary>
public class QuotationDocumentHandler(IRequestAccessResolver accessResolver, IQuotationDocumentStore store)
{
    public async Task<Result<QuotationDocumentFile>> DownloadAsync(QuotationLifecycleCaller caller, Guid quotationId, CancellationToken ct = default)
    {
        var access = await accessResolver.ResolveAsync(caller.FirebaseUid, caller.MembershipId, "quotations.read", ct);
        return access.IsFailure ? Result<QuotationDocumentFile>.Failure(access.Error) : await store.OpenAsync(access.Value!, quotationId, ct);
    }
}
```
- [ ] **Step 4: แก้ `backend/src/TanErp.Infrastructure/Persistence/Estimates/QuotationBillingSnapshotBuilder.cs`** — ย้ายชนิด snapshot เป็น `internal` ที่ใช้ร่วมกัน
```text
>>> หลัง: internal sealed record QuotationBillingSnapshot(string Json, string Hash);

/// <summary>The stored billing snapshot read back (camelCase JSON written by BuildAsync). Shared by the JSON document and the PDF model.</summary>
internal sealed record QuotationBillingSnapshotData(
    string CustomerType, string DisplayNameTh, string? DisplayNameEn, string? LegalName, string? TaxIdentifier, string? BranchCode,
    QuotationBillingSnapshotAddress? Address);

internal sealed record QuotationBillingSnapshotAddress(
    string? Label, string AddressLine1, string? Subdistrict, string? District, string? Province, string? PostalCode, string? CountryCode);
```
- [ ] **Step 5: แก้ `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`** — ลบ record ที่ซ้ำ (ชื่อเดียวกับที่ย้ายไปแล้ว) ออกจาก `EstimateStore`
```text
>>> แทนที่:
    private sealed record QuotationBillingSnapshotData(
        string CustomerType,
        string DisplayNameTh,
        string? DisplayNameEn,
        string? LegalName,
        string? TaxIdentifier,
        string? BranchCode,
        QuotationBillingSnapshotAddress? Address);

    private sealed record QuotationBillingSnapshotAddress(
        string? Label,
        string AddressLine1,
        string? Subdistrict,
        string? District,
        string? Province,
        string? PostalCode,
        string? CountryCode);
>>> ด้วย:
```
- [ ] **Step 6: สร้าง `backend/src/TanErp.Infrastructure/Documents/QuotationPdfModelBuilder.cs`**
```csharp
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Common.Results;
using TanErp.Domain.Commercial;
using TanErp.Infrastructure.Persistence;
using TanErp.Infrastructure.Persistence.Estimates;

namespace TanErp.Infrastructure.Documents;

/// <summary>
/// Projects one quotation (possibly not saved yet) into the allowlisted PDF model. Only selling-side columns are read:
/// the cost, margin and markup columns of the estimate revision are never touched.
/// </summary>
internal static class QuotationPdfModelBuilder
{
    private static Result<QuotationPdfModel> Fail(string code, string message) => Result<QuotationPdfModel>.Failure(new Error(code, message));

    public static async Task<Result<QuotationPdfModel>> BuildAsync(AppDbContext db, Quotation quotation, CancellationToken ct)
    {
        var organizationId = quotation.OrganizationId;
        var revision = await db.EstimateRevisions.AsNoTracking().Include(r => r.Sections).ThenInclude(s => s.WorkItems)
            .FirstOrDefaultAsync(r => r.OrganizationId == organizationId && r.Id == quotation.EstimateRevisionId, ct);
        var organization = await db.Organizations.AsNoTracking().FirstOrDefaultAsync(o => o.Id == organizationId, ct);
        var branch = await db.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.OrganizationId == organizationId && b.Id == quotation.BranchId, ct);
        if (revision is null || organization is null || branch is null) return Fail("RESOURCE_NOT_FOUND", "Estimate revision, organization or branch not found.");

        QuotationBillingSnapshotData? snapshot = null;
        try
        {
            snapshot = string.IsNullOrWhiteSpace(quotation.CustomerBillingSnapshotJson)
                ? null
                : JsonSerializer.Deserialize<QuotationBillingSnapshotData>(quotation.CustomerBillingSnapshotJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            // Unreadable stored data is reported below without logging its (customer) content.
        }

        if (snapshot is null) return Fail("ESTIMATE_INVALID_STATE", "Quotation billing snapshot is missing or corrupted.");

        string? supersedesNumber = null;
        if (quotation.SupersedesQuotationId is { } supersededId)
            supersedesNumber = await db.Quotations.AsNoTracking().Where(q => q.OrganizationId == organizationId && q.Id == supersededId).Select(q => q.Number).FirstOrDefaultAsync(ct);

        // Business rule (ADR 0023): a branch with its own address (Thai or English) uses that address pair and phone as a whole;
        // otherwise the organization's pair is used. The two are never mixed line by line.
        var branchOwnsAddress = !string.IsNullOrWhiteSpace(branch.AddressTh) || !string.IsNullOrWhiteSpace(branch.AddressEn);
        var issuer = new QuotationPdfIssuer(
            organization.Name, organization.NameEn, branch.Name, branch.NameEn, organization.TaxIdentifier, branch.TaxBranchCode,
            branchOwnsAddress ? branch.AddressTh : organization.AddressTh, branchOwnsAddress ? branch.AddressEn : organization.AddressEn,
            branchOwnsAddress ? branch.Phone : organization.Phone);
        var customer = new QuotationPdfCustomer(
            snapshot.DisplayNameTh, snapshot.DisplayNameEn, snapshot.LegalName, snapshot.TaxIdentifier, snapshot.BranchCode,
            snapshot.Address is null ? null : new QuotationPdfAddress(snapshot.Address.AddressLine1, snapshot.Address.Subdistrict, snapshot.Address.District, snapshot.Address.Province, snapshot.Address.PostalCode));
        var sections = revision.Sections.OrderBy(s => s.SortOrder).Select(section => new QuotationPdfSection(
            section.Code, section.NameTh, section.NameEn, section.SubtotalSellingPrice,
            section.WorkItems.OrderBy(w => w.SortOrder).Select(item => new QuotationPdfWorkItem(
                item.Code, item.DescriptionTh, item.DescriptionEn, item.Quantity, item.UnitCode, item.UnitSellingPrice, item.TotalSellingPrice)).ToList())).ToList();
        var totals = new QuotationPdfTotals(revision.SellingBeforeDiscount, revision.DiscountAmount, revision.NetBeforeTax, revision.TaxAmount, revision.GrandTotal);

        return Result<QuotationPdfModel>.Success(new QuotationPdfModel(
            quotation.Number, quotation.IssuedAtUtc, revision.Currency, supersedesNumber, issuer, customer, sections, totals));
    }
}
```
- [ ] **Step 7: สร้าง `backend/src/TanErp.Infrastructure/Documents/QuotationDocumentWriter.cs`**
```csharp
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Files;
using TanErp.Domain.Commercial;
using TanErp.Domain.Common;
using TanErp.Domain.Files;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Documents;

public sealed partial class QuotationDocumentWriter(
    AppDbContext db, IQuotationDocumentRenderer renderer, IFileStorageProvider storage, IClock clock, ILogger<QuotationDocumentWriter> logger) : IQuotationDocumentWriter
{
    public const string UploadSessionPrefix = "system:quotation-document:";

    [GeneratedRegex("[^A-Za-z0-9._-]")]
    private static partial Regex UnsafeFileNameChars();

    public static string FileNameFor(string quotationNumber) => UnsafeFileNameChars().Replace(quotationNumber, "_") + ".pdf";

    public async Task<Result<QuotationDocumentSummary>> StageAsync(Quotation quotation, Guid actorUserId, string traceId, CancellationToken cancellationToken = default)
    {
        var (organizationId, quotationId) = (quotation.OrganizationId, quotation.Id);
        var model = await QuotationPdfModelBuilder.BuildAsync(db, quotation, cancellationToken);
        if (model.IsFailure) return Result<QuotationDocumentSummary>.Failure(model.Error);

        byte[] bytes;
        try
        {
            bytes = renderer.Render(model.Value!);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Rendering the PDF of quotation {QuotationId} failed.", quotationId); // identifiers only: the model holds customer data
            throw new QuotationDocumentRenderException("The quotation PDF could not be rendered.", ex);
        }

        var now = clock.UtcNow;
        var number = model.Value!.Number;
        var sha256 = Sha256Hex.ComputeBytes(bytes);
        string storagePath;
        using (var content = new MemoryStream(bytes, writable: false))
            storagePath = await storage.SaveAsync(organizationId, $"quotation-document-{quotationId:N}", FileNameFor(number), content, cancellationToken);

        var file = new UploadedFile(Guid.NewGuid(), organizationId, storagePath, FileNameFor(number), "application/pdf", bytes.Length,
            UploadSessionPrefix + quotationId.ToString("N"), actorUserId, now, contentSha256: sha256);
        db.UploadedFiles.Add(file);
        db.QuotationDocuments.Add(new QuotationDocument(Guid.NewGuid(), organizationId, quotationId, file.Id, sha256, bytes.Length, renderer.Version, now, actorUserId));
        db.AddAuditEvent(new AuditEvent(Guid.NewGuid(), organizationId, actorUserId, "quotations.document-rendered", "Quotation", quotationId.ToString(), now, traceId,
            JsonSerializer.Serialize(new { quotationNumber = number, sha256, byteCount = bytes.Length, rendererVersion = renderer.Version })));

        return Result<QuotationDocumentSummary>.Success(new QuotationDocumentSummary(sha256, bytes.Length, renderer.Version, now));
    }
}
```
- [ ] **Step 8: สร้าง `backend/src/TanErp.Infrastructure/Documents/QuotationDocumentStore.cs`**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Files;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Documents;

public sealed class QuotationDocumentStore(AppDbContext db, IFileStorageProvider storage, ILogger<QuotationDocumentStore> logger) : IQuotationDocumentStore
{
    public async Task<Result<QuotationDocumentFile>> OpenAsync(RequestAccessContext access, Guid quotationId, CancellationToken cancellationToken = default)
    {
        var quotation = await db.Quotations.AsNoTracking().FirstOrDefaultAsync(q => q.OrganizationId == access.OrganizationId && q.Id == quotationId, cancellationToken);
        if (quotation is null || !access.HasBranchAccess(quotation.BranchId))
            return Result<QuotationDocumentFile>.Failure(new Error("RESOURCE_NOT_FOUND", "Quotation not found."));

        var file = await LoadAsync(access.OrganizationId, quotationId, cancellationToken);
        return file is null
            ? Result<QuotationDocumentFile>.Failure(new Error("QUOTATION_DOCUMENT_NOT_AVAILABLE", "This quotation has no stored PDF."))
            : Result<QuotationDocumentFile>.Success(file);
    }

    public Task<QuotationDocumentFile?> OpenForAcceptanceAsync(Guid organizationId, Guid quotationId, CancellationToken cancellationToken = default) =>
        LoadAsync(organizationId, quotationId, cancellationToken);

    public async Task<QuotationDocumentSummary?> FindSummaryAsync(Guid organizationId, Guid quotationId, CancellationToken cancellationToken = default) =>
        await db.QuotationDocuments.AsNoTracking()
            .Where(d => d.OrganizationId == organizationId && d.QuotationId == quotationId)
            .Select(d => new QuotationDocumentSummary(d.Sha256, d.ByteCount, d.RendererVersion, d.RenderedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

    private async Task<QuotationDocumentFile?> LoadAsync(Guid organizationId, Guid quotationId, CancellationToken cancellationToken)
    {
        var row = await (from d in db.QuotationDocuments.AsNoTracking()
                         join f in db.UploadedFiles.AsNoTracking() on new { Id = d.FileId, d.OrganizationId } equals new { f.Id, f.OrganizationId }
                         where d.OrganizationId == organizationId && d.QuotationId == quotationId
                         select new { Document = d, f.StoragePath, f.OriginalFilename }).FirstOrDefaultAsync(cancellationToken);
        if (row is null) return null;

        await using var stream = await storage.OpenReadStreamAsync(row.StoragePath, cancellationToken)
            ?? throw new InvalidOperationException($"The stored PDF of quotation {quotationId} is missing from storage.");
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer, cancellationToken);
        var bytes = buffer.ToArray();

        // The recorded hash is the evidence, so never serve bytes that no longer match it.
        if (!string.Equals(Sha256Hex.ComputeBytes(bytes), row.Document.Sha256, StringComparison.Ordinal))
        {
            logger.LogError("The stored PDF of quotation {QuotationId} no longer matches its recorded SHA-256.", quotationId);
            throw new InvalidOperationException($"The stored PDF of quotation {quotationId} failed its integrity check.");
        }

        var summary = new QuotationDocumentSummary(row.Document.Sha256, row.Document.ByteCount, row.Document.RendererVersion, row.Document.RenderedAtUtc);
        return new QuotationDocumentFile(bytes, row.OriginalFilename, summary);
    }
}
```
- [ ] **Step 9: แก้ `backend/src/TanErp.Api/Program.cs`**
```text
>>> หลัง: builder.Services.AddScoped<TanErp.Application.Commercial.QuotationAcceptanceHandler>();
builder.Services.AddSingleton<TanErp.Application.Commercial.Documents.IQuotationDocumentRenderer, TanErp.Infrastructure.Documents.QuestPdfQuotationRenderer>();
builder.Services.AddScoped<TanErp.Application.Commercial.Documents.IQuotationDocumentWriter, TanErp.Infrastructure.Documents.QuotationDocumentWriter>();
builder.Services.AddScoped<TanErp.Application.Commercial.Documents.IQuotationDocumentStore, TanErp.Infrastructure.Documents.QuotationDocumentStore>();
builder.Services.AddScoped<TanErp.Application.Commercial.Documents.QuotationDocumentHandler>();
>>> หลัง: var app = builder.Build();
// Fail fast: resolving the renderer registers the QuestPDF license and the embedded fonts at startup, so a bad configuration
// surfaces here instead of at the first quotation issue (ADR 0023).
_ = app.Services.GetRequiredService<TanErp.Application.Commercial.Documents.IQuotationDocumentRenderer>();
```
- [ ] **Step 10: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
```
Expected: `0 Warning(s)` `0 Error(s)` (TreatWarningsAsErrors เปิดอยู่; โปรเจกต์ test อยู่ใน solution จึงถูก compile ด้วย)
- [ ] **Step 11: Commit**
```bash
git add backend/src/TanErp.Application backend/src/TanErp.Infrastructure backend/src/TanErp.Api/Program.cs
git commit -m "feat(commercial): add quotation document writer, store and handler

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 5: ต่อสาย issue/amend: render ใน unit of work เดียวกับการออก + สรุปเอกสารในประวัติใบเสนอราคา

**Files:** แก้ `EstimateStore.cs`, `QuotationLifecycleStore.cs`, `QuotationLifecycleModels.cs`, `QuotationLifecycleContracts.cs`, `QuotationLifecycleController.cs`, `EstimateEndpointsTests.cs`; สร้าง (test) `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.QuotationDocuments.cs` (partial class ใช้ harness/ฐานข้อมูล/helper เดิม)

**ลำดับ (D4):** `StageAsync` ทำ **ก่อน** `SaveChanges`+`Commit` เดิมของ flow โดยไม่แก้โครง try/catch/transaction: render → เขียน blob → เพิ่มแถว file/document/audit ใน DbContext เดียวกัน แล้ว `SaveChanges` เดิมบันทึก quotation+document+audit พร้อมกัน. render พัง = exception → transaction ถูก dispose = rollback: ไม่มี quotation, เลขเอกสารและ idempotency key ไม่ถูกใช้ (ตัวนับเลขอยู่ใน transaction เดียวกัน: `SequenceCounter.cs` ใช้ `CurrentTransaction`). ถ้า commit ล้มหลังเขียน blob (เช่นแพ้ idempotency race) เหลือ blob กำพร้าที่ไม่มีใครอ้างถึง — ยอมรับได้ (ADR 0023). replay ของ idempotency คืนผลเดิมก่อนถึงขั้นนี้ จึงไม่ render ซ้ำ.

**Test/fixture เดิมที่ออกใบเสนอราคา:** (ก) ผ่าน API จริง (renderer in-process ทำงานจริง เขียนที่ `Storage:BasePath` default `AppContext.BaseDirectory/local-storage` ไม่ต้องตั้งค่าเพิ่ม): `EstimateEndpointsTests.cs` บรรทัด 1094, 1120, 1163, 1835, 2454, 2509, 2931, 2968, 2982, 3023, 3037, 3061 และ helper `IssueQuotedEstimateAsync` (3082-3105) — ยังถูกต้อง: เคสที่ปฏิเสธล้มก่อนถึง render, เคส replay (1163) ไม่ render ซ้ำ. (ข) แถวที่ insert ตรง `new Quotation(...)` — `ServiceEndpointsTests.cs:145`, `ProjectControlEndpointsTests.cs:145`, `AttachmentEndpointsTests.cs:220`, `FinanceEndpointsTests.cs:167`, `ProcurementEndpointsTests.cs:147`, `ProjectEndpointsTests.cs:132`, `EstimateEndpointsTests.cs:1341` — ยังถูกต้องเพราะไม่มี FK จาก quotation ไป document: ไม่มีเอกสาร = สถานะ legacy ที่ออกแบบรองรับ (D7). (ค) `GetQuotationDocumentHandlerTests.cs` เป็น fake ของ `IEstimateStore` ซึ่ง interface ไม่เปลี่ยน; `EstimateEndpointsTests.cs:3203` อาจได้ audit `quotations.document-rendered` แต่ assert เป็น `DoesNotContain` จึงยังผ่าน.

- [ ] **Step 1: แก้ `backend/src/TanErp.Infrastructure/Persistence/Estimates/EstimateStore.cs`** — `IssueQuotationAsync`: เรียก writer ก่อน `SaveChanges` เดิม (anchor นี้มีเฉพาะ event `quotations.issued`; `AcceptQuotationAsync` มีบล็อกคล้ายกันและห้ามแตะ)
```text
>>> หลัง: using TanErp.Application.Common.Abstractions;
using TanErp.Application.Commercial.Documents;
>>> หลัง: private readonly IApprovalAuthority _approvalAuthority;
    private readonly IQuotationDocumentWriter _documents;
>>> แทนที่:
        IApprovalAuthority approvalAuthority)
>>> ด้วย:
        IApprovalAuthority approvalAuthority,
        IQuotationDocumentWriter documents)
>>> หลัง: _approvalAuthority = approvalAuthority;
        _documents = documents;
>>> แทนที่:
                    quotation.TotalAmount,
                    estimateNumber = estimate.Number
                }));
>>> ด้วย:
                    quotation.TotalAmount,
                    estimateNumber = estimate.Number
                }));

            // Render the PDF once and stage it in the same unit of work (ADR 0023): the quotation, its document and the audit trail
            // are saved by the one SaveChanges below, so a failed render commits nothing and the idempotency key stays unused.
            var staged = await _documents.StageAsync(quotation, actorUserId, traceId, cancellationToken);
            if (staged.IsFailure) return Result<QuotationDetailProjection>.Failure(staged.Error);
```
- [ ] **Step 2: แก้ `backend/src/TanErp.Infrastructure/Persistence/Commercial/QuotationLifecycleStore.cs`** — `AmendAsync`: render ใบแทนที่ก่อน `SaveChanges` เดิม (อยู่ใน `try` เดิม)
```text
>>> หลัง: using TanErp.Application.Commercial;
using TanErp.Application.Commercial.Documents;
>>> หลัง: private readonly IDocumentNumberGenerator _numbers;
    private readonly IQuotationDocumentWriter _documents;
>>> แทนที่:
IDocumentNumberGenerator numbers)
>>> ด้วย:
IDocumentNumberGenerator numbers, IQuotationDocumentWriter documents)
>>> หลัง: _numbers = numbers;
        _documents = documents;
>>> หลัง: _db.IdempotencyRecords.Add(new IdempotencyRecord(Guid.NewGuid(), orgId, operation, keyHash, payloadHash, replacement.Id.ToString(), now));
                // The replacement gets its own PDF in the same unit of work (ADR 0023); the superseded quotation keeps its original one.
                var staged = await _documents.StageAsync(replacement, access.ActorUserId, traceId, ct);
                if (staged.IsFailure) return Result<QuotationHistoryProjection>.Failure(staged.Error);
>>> หลัง: var users = await _db.Users.AsNoTracking().Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.DisplayName, ct);
        var quotationIds = rows.Select(q => q.Id).ToList();
        var documents = await _db.QuotationDocuments.AsNoTracking()
            .Where(d => d.OrganizationId == organizationId && quotationIds.Contains(d.QuotationId))
            .ToDictionaryAsync(d => d.QuotationId, ct);
>>> แทนที่:
            q.VoidReason, q.RowVersion)).ToList());
>>> ด้วย:
            q.VoidReason, q.RowVersion,
            documents.TryGetValue(q.Id, out var document) ? new QuotationDocumentSummary(document.Sha256, document.ByteCount, document.RendererVersion, document.RenderedAtUtc) : null)).ToList());
```
- [ ] **Step 3: แก้ `backend/src/TanErp.Application/Commercial/QuotationLifecycleModels.cs`**
```text
>>> แทนที่:
namespace TanErp.Application.Commercial;
>>> ด้วย:
using TanErp.Application.Commercial.Documents;

namespace TanErp.Application.Commercial;
>>> แทนที่:
    string? VoidReason,
    Guid RowVersion);
>>> ด้วย:
    string? VoidReason,
    Guid RowVersion,
    QuotationDocumentSummary? Document);
```
- [ ] **Step 4: แก้ `backend/src/TanErp.Api/Contracts/Commercial/QuotationLifecycleContracts.cs`**
```text
>>> แทนที่:
    string? VoidReason,
    Guid RowVersion);
>>> ด้วย:
    string? VoidReason,
    Guid RowVersion,
    QuotationDocumentSummaryResponse? Document);

/// <summary>The stored PDF of a quotation. The hash is the evidence; the file id and storage path are never exposed.</summary>
public sealed record QuotationDocumentSummaryResponse(string Sha256, long ByteCount, string RendererVersion, DateTimeOffset RenderedAtUtc);
```
- [ ] **Step 5: แก้ `backend/src/TanErp.Api/Controllers/QuotationLifecycleController.cs`**
```text
>>> แทนที่:
q.VoidedAtUtc, q.VoidedBy is null ? null : new QuotationPersonResponse(q.VoidedBy.Id, q.VoidedBy.DisplayName), q.VoidReason, q.RowVersion)).ToList());
>>> ด้วย:
q.VoidedAtUtc, q.VoidedBy is null ? null : new QuotationPersonResponse(q.VoidedBy.Id, q.VoidedBy.DisplayName), q.VoidReason, q.RowVersion,
        q.Document is null ? null : new QuotationDocumentSummaryResponse(q.Document.Sha256, q.Document.ByteCount, q.Document.RendererVersion, q.Document.RenderedAtUtc))).ToList());
```
- [ ] **Step 6: แก้ `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`** — เตรียม harness เดิมให้ไฟล์ test ใหม่ใช้ร่วมได้ (`partial`, token ผู้อ่านใบเสนอราคา, `IssueQuotedEstimateAsync` รับ client/สถานะที่คาด)
```text
>>> แทนที่:
public class EstimateEndpointsTests : IAsyncLifetime
>>> ด้วย:
public partial class EstimateEndpointsTests : IAsyncLifetime
>>> แทนที่:
                "token-branch-outsider" => "test-only-branch-outsider",
>>> ด้วย:
                "token-branch-outsider" => "test-only-branch-outsider",
                "token-quotation-reader" => "test-only-quotation-reader",
>>> แทนที่:
    private async Task<(Guid EstimateId, Guid OpportunityId, QuotationResponse Quotation)> IssueQuotedEstimateAsync(string prefix)
    {
>>> ด้วย:
    private async Task<(Guid EstimateId, Guid OpportunityId, QuotationResponse Quotation)> IssueQuotedEstimateAsync(
        string prefix, HttpClient? issueClient = null, HttpStatusCode expectedIssueStatus = HttpStatusCode.Created)
    {
>>> แทนที่:
        var issueResponse = await _client.SendAsync(issueRequest);
        Assert.Equal(HttpStatusCode.Created, issueResponse.StatusCode);
        return (estimate.Id, opportunity.Id, (await issueResponse.Content.ReadFromJsonAsync<QuotationResponse>())!);
    }
>>> ด้วย:
        var issueResponse = await (issueClient ?? _client).SendAsync(issueRequest);
        Assert.Equal(expectedIssueStatus, issueResponse.StatusCode);
        // A non-success expectation (render failure test) has no quotation to return.
        return (estimate.Id, opportunity.Id, expectedIssueStatus == HttpStatusCode.Created ? (await issueResponse.Content.ReadFromJsonAsync<QuotationResponse>())! : null!);
    }
```
- [ ] **Step 7: สร้าง `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.QuotationDocuments.cs`**
```csharp
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Files;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using Xunit;

namespace TanErp.IntegrationTests.Api;

// G-04: the server-side quotation PDF. Shares the harness (database, tokens, helpers) of the main file.
public partial class EstimateEndpointsTests
{
    private sealed class ThrowingQuotationRenderer : IQuotationDocumentRenderer
    {
        public string Version => "test-throwing/v0";
        public byte[] Render(QuotationPdfModel model) => throw new InvalidOperationException("TEST_ONLY renderer failure");
    }

    private static string Sha256Of(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    // G-04 tests of later tasks are inserted below this line.

    [Fact]
    public async Task IssueQuotation_StoresOneDocumentRow_WhoseHashMatchesTheStoredBytes()
    {
        var (estimateId, _, quotation) = await IssueQuotedEstimateAsync("pdf-issue");
        var item = (await HistoryAsync(estimateId)).Items.Single();
        Assert.NotNull(item.Document);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var document = await db.QuotationDocuments.AsNoTracking().SingleAsync(d => d.QuotationId == quotation.QuotationId);
        var file = await db.UploadedFiles.AsNoTracking().SingleAsync(f => f.Id == document.FileId);
        Assert.Equal(("application/pdf", document.Sha256, document.ByteCount), (file.MediaType, file.ContentSha256, file.FileSizeBytes));
        Assert.Equal(item.Document.Sha256, document.Sha256);

        await using var stream = (await scope.ServiceProvider.GetRequiredService<IFileStorageProvider>().OpenReadStreamAsync(file.StoragePath))!;
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);
        Assert.StartsWith("%PDF-", Encoding.ASCII.GetString(buffer.ToArray(), 0, 5));
        Assert.Equal(document.Sha256, Sha256Of(buffer.ToArray()));

        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "quotations.document-rendered" && a.ResourceId == quotation.QuotationId.ToString());
        Assert.Contains(document.Sha256, audit.ChangesJson);
        Assert.DoesNotContain("ลูกค้า", audit.ChangesJson);
    }

    [Fact]
    public async Task IssueQuotation_WhenRenderingFails_RollsBackTheWholeIssue()
    {
        using var failing = _factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IQuotationDocumentRenderer>();
            services.AddSingleton<IQuotationDocumentRenderer, ThrowingQuotationRenderer>();
        }));
        using var failingClient = failing.CreateClient();

        var (estimateId, oppId, _) = await IssueQuotedEstimateAsync("pdf-fail", failingClient, HttpStatusCode.InternalServerError);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(0, await db.Quotations.CountAsync(q => q.EstimateId == estimateId));
        Assert.Equal(0, await db.QuotationDocuments.CountAsync());
        Assert.Equal(0, await db.IdempotencyRecords.CountAsync(r => r.Operation == "quotations.issue"));
        Assert.Equal(OpportunityStage.Estimating, (await db.Opportunities.AsNoTracking().SingleAsync(o => o.Id == oppId)).Stage);
    }

    [Fact]
    public async Task AmendQuotation_RendersTheReplacementOnce_AndKeepsTheOriginalDocumentUntouched()
    {
        var (estimateId, _, original) = await IssueQuotedEstimateAsync("pdf-amend");
        var before = (await HistoryAsync(estimateId)).Items.Single();
        var amended = await _client.SendAsync(LifecycleRequest($"/api/v1/quotations/{original.QuotationId}/amend", before.RowVersion, "ปรับเงื่อนไข", $"pdf-amend-{Guid.NewGuid():N}"));
        Assert.Equal(HttpStatusCode.Created, amended.StatusCode);

        var items = (await amended.Content.ReadFromJsonAsync<TanErp.Api.Contracts.Commercial.QuotationHistoryResponse>())!.Items;
        Assert.Equal(before.Document!.Sha256, items.Single(i => i.Id == original.QuotationId).Document!.Sha256);
        Assert.NotEqual(before.Document.Sha256, items.Single(i => i.Id != original.QuotationId).Document!.Sha256);
    }

}
```
- [ ] **Step 8: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
```
Expected: `0 Warning(s)` `0 Error(s)` (TreatWarningsAsErrors เปิดอยู่; โปรเจกต์ test อยู่ใน solution จึงถูก compile ด้วย)
- [ ] **Step 9: Commit**
```bash
git add backend/src backend/tests
git commit -m "feat(commercial): render and store the quotation pdf when issuing or amending

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 6: Endpoint ดาวน์โหลดไฟล์ที่เก็บไว้ + error code ใหม่สำหรับใบเสนอราคาที่ไม่มีไฟล์

**Files:** สร้าง `backend/src/TanErp.Api/Controllers/QuotationDocumentsController.cs`; แก้ `ProblemDetailsMapper.cs`, `Errors.resx`, `Errors.en.resx`, `EstimateEndpointsTests.QuotationDocuments.cs`

**Contract:** `GET /api/v1/quotations/{id}/document` (`quotations.read`) → `200 application/pdf` + `ETag: "<sha256>"` + `Cache-Control: private, no-store`. นอก Organization/Branch → `404 RESOURCE_NOT_FOUND` (ตรวจ scope ก่อนตรวจว่ามีเอกสาร จึงไม่รั่วว่ามีใบเสนอราคา); ไม่มีเอกสาร (ใบที่ออกก่อน G-04 — D7) → `404 QUOTATION_DOCUMENT_NOT_AVAILABLE` (**code ใหม่ตัวเดียวของ slice**; `GetKnownCode` ปล่อยเฉพาะ 400/401/403/404/409/422/428, ProblemDetailsMapper.cs:402-413 จึงไม่มี code ใหม่สำหรับ render พัง); ไม่มีสิทธิ์ → `403 PERMISSION_DENIED`. ไม่มี permission ใหม่. `ProducesResponseType(200)` ไม่ระบุ schema เพื่อให้ OpenAPI ไม่อ้างชนิดไฟล์.

- [ ] **Step 1: สร้าง `backend/src/TanErp.Api/Controllers/QuotationDocumentsController.cs`**
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Commercial;
using TanErp.Application.Commercial.Documents;

namespace TanErp.Api.Controllers;

/// <summary>The stored, immutable PDF of a quotation. It is served from storage; it is never rendered again.</summary>
[ApiController]
[Authorize]
public class QuotationDocumentsController(QuotationDocumentHandler handler) : ControllerBase
{
    [HttpGet("api/v1/quotations/{id:guid}/document")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download([FromRoute] Guid id, CancellationToken ct)
    {
        var context = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (context.IsFailure) return ProblemDetailsMapper.CreateProblemResult(context.Error.Code, HttpContext);
        var caller = new QuotationLifecycleCaller(context.Value!.FirebaseUid, context.Value.MembershipId, HttpContext.TraceIdentifier);
        var result = await handler.DownloadAsync(caller, id, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);

        var document = result.Value!;
        Response.Headers.CacheControl = "private, no-store";
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        Response.Headers.ETag = $"\"{document.Summary.Sha256}\"";
        return File(document.Content, "application/pdf", document.FileName);
    }
}
```
- [ ] **Step 2: แก้ `backend/src/TanErp.Api/ErrorHandling/ProblemDetailsMapper.cs`** — เพิ่ม status ของ code ใหม่ใน `GetStatus` (ต้องทำใน Task เดียวกับ resx ทั้งสองไฟล์)
```text
>>> หลัง: "QUOTATION_FIELD_INVALID" => StatusCodes.Status422UnprocessableEntity,
        "QUOTATION_DOCUMENT_NOT_AVAILABLE" => StatusCodes.Status404NotFound,
```
- [ ] **Step 3: แก้ `backend/src/TanErp.Api/Resources/Errors.resx`**
```text
>>> แทนที่:
</root>
>>> ด้วย:
  <data name="QUOTATION_DOCUMENT_NOT_AVAILABLE_TITLE" xml:space="preserve">
    <value>ไม่มีไฟล์ PDF ของใบเสนอราคา</value>
  </data>
  <data name="QUOTATION_DOCUMENT_NOT_AVAILABLE_DETAIL" xml:space="preserve">
    <value>ใบเสนอราคานี้ออกก่อนที่ระบบจะเก็บไฟล์ PDF จึงยังไม่มีไฟล์ที่เก็บไว้ ผู้มีสิทธิ์ออกใบเสนอราคาสร้างไฟล์ได้หนึ่งครั้ง</value>
  </data>
</root>
```
- [ ] **Step 4: แก้ `backend/src/TanErp.Api/Resources/Errors.en.resx`**
```text
>>> แทนที่:
</root>
>>> ด้วย:
  <data name="QUOTATION_DOCUMENT_NOT_AVAILABLE_TITLE" xml:space="preserve">
    <value>Quotation PDF not available</value>
  </data>
  <data name="QUOTATION_DOCUMENT_NOT_AVAILABLE_DETAIL" xml:space="preserve">
    <value>This quotation was issued before PDFs were stored, so no stored PDF exists yet. A user who can issue quotations can create it once.</value>
  </data>
</root>
```
- [ ] **Step 5: แก้ `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.QuotationDocuments.cs`** — เพิ่ม helper และ test ของ endpoint ใหม่ (ใต้บรรทัด marker)
```text
>>> หลัง: // G-04 tests of later tasks are inserted below this line.
    private Task<HttpResponseMessage> GetQuotationPdfAsync(Guid quotationId, string token = "token-org-a", Guid? membership = null) =>
        _client.SendAsync(CreateAuthenticatedRequest(HttpMethod.Get, $"/api/v1/quotations/{quotationId}/document", token, membership ?? MembershipAId));

    // Simulates a quotation issued before G-04: it has no stored document.
    private async Task SimulateLegacyQuotationAsync(Guid quotationId)
    {
        using var scope = _factory.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.ExecuteSqlInterpolatedAsync($"DELETE FROM commercial.quotation_documents WHERE quotation_id = {quotationId}");
    }

    // A user whose quotations.read is scoped to ANOTHER branch of the same organization.
    private static async Task<Guid> SeedOtherBranchReaderAsync(AppDbContext db)
    {
        var (branchId, userId, membershipId, roleId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        db.Branches.Add(new Branch(branchId, OrgAId, $"QR-{branchId:N}"[..12], "Other Branch"));
        db.Users.Add(new User(userId, "test-only-quotation-reader", "Quotation Reader", "quotation-reader@example.test"));
        db.Memberships.Add(new Membership(membershipId, OrgAId, branchId, userId));
        db.Roles.Add(new Role(roleId, OrgAId, "TEST ONLY OTHER BRANCH", "TEST_ONLY other branch"));
        db.MembershipRoles.Add(new MembershipRole(membershipId, roleId, OrgAId));
        var read = await db.Permissions.SingleAsync(row => row.Key == "quotations.read");
        db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), roleId, OrgAId, read.Id, PermissionScope.Branch, branchId, branchId));
        await db.SaveChangesAsync();
        return membershipId;
    }

    [Fact]
    public async Task QuotationDocument_IsServedFromStorage_AndIsNotChangedByLaterOrganizationProfileChanges()
    {
        var (estimateId, _, quotation) = await IssueQuotedEstimateAsync("pdf-download");
        var summary = (await HistoryAsync(estimateId)).Items.Single().Document!;

        var response = await GetQuotationPdfAsync(quotation.QuotationId);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal($"\"{summary.Sha256}\"", response.Headers.ETag?.Tag);
        var before = await response.Content.ReadAsByteArrayAsync();
        Assert.Equal(summary.Sha256, Sha256Of(before));

        // Master data changes after issue: the stored file stays exactly as issued because it is never re-rendered.
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            (await db.Organizations.SingleAsync(o => o.Id == OrgAId)).UpdateProfile("ชื่อองค์กรใหม่หลังออกเอกสาร", "Renamed after issue", null, "ที่อยู่ใหม่", null, null);
            await db.SaveChangesAsync();
        }

        Assert.Equal(before, await (await GetQuotationPdfAsync(quotation.QuotationId)).Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task QuotationDocument_OutsideTheCallersOrganizationOrBranch_IsNotFound_AndALegacyQuotationHasNone()
    {
        var (_, _, quotation) = await IssueQuotedEstimateAsync("pdf-scope");
        Guid otherBranchMembership;
        using (var scope = _factory.Services.CreateScope())
            otherBranchMembership = await SeedOtherBranchReaderAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());

        Assert.Equal(HttpStatusCode.NotFound, (await GetQuotationPdfAsync(quotation.QuotationId, "token-org-b", TestOnlyDataSeeder.TestMembershipBId)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await GetQuotationPdfAsync(quotation.QuotationId, "token-quotation-reader", otherBranchMembership)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await GetQuotationPdfAsync(Guid.NewGuid())).StatusCode);

        // A quotation issued before G-04 has no document: a stable code, and the history says so.
        await SimulateLegacyQuotationAsync(quotation.QuotationId);
        var missing = await GetQuotationPdfAsync(quotation.QuotationId);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal("QUOTATION_DOCUMENT_NOT_AVAILABLE", await ProblemCodeAsync(missing));
    }
```
- [ ] **Step 6: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
```
Expected: `0 Warning(s)` `0 Error(s)` (TreatWarningsAsErrors เปิดอยู่; โปรเจกต์ test อยู่ใน solution จึงถูก compile ด้วย)
- [ ] **Step 7: Commit**
```bash
git add backend/src/TanErp.Api backend/tests
git commit -m "feat(commercial): serve the stored quotation pdf

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 7: หน้าลูกค้า CP-07: แสดง/ส่งไฟล์ PDF ที่เก็บไว้ และบันทึก hash ที่ลูกค้ายืนยันเป็นหลักฐาน

**Files:** แก้ `QuotationAcceptanceModels.cs`, `QuotationAcceptanceHandler.cs`, `QuotationAcceptanceStore.cs`, `QuotationAcceptanceContracts.cs`, `PublicQuotationAcceptanceController.cs`, `EstimateEndpointsTests.cs`, `EstimateEndpointsTests.QuotationDocuments.cs`

**กติกา (D6):** `documentSha256` ที่ลูกค้าส่งต้อง **เท่ากับ** hash ที่เก็บไว้ และ `null` ตรงกับ "ไม่มีไฟล์" เท่านั้น; ไม่ตรง → `409 ACCEPTANCE_CONFLICT` (code เดิม) โดยตรวจ **หลัง** กฎ consent/ชื่อ/ลายเซ็น และ **ก่อน** `AcceptQuotationAsync` จึงไม่มีสถานะยอมรับครึ่งทาง; หลักฐานเก็บค่าที่ลูกค้ายืนยัน. ยอมรับซ้ำบนลิงก์ที่ยอมรับแล้วคืนผลเดิมก่อนถึงการตรวจนี้ (พฤติกรรมเดิม). `GET …/{token}/document` ใช้กฎลิงก์เดียวกับ view (ลิงก์ที่ยอมรับแล้วยังเปิดได้) และคืน `404 ACCEPTANCE_LINK_UNAVAILABLE` เหมือนทุกกรณีใช้ไม่ได้ (รวมใบที่ไม่มีไฟล์), `no-store`, rate limit เดิม; ลูกค้าเห็นเฉพาะ `{sha256, byteCount}`.

**ผลต่อ test เดิม:** `Submission(...)` สร้าง `PublicAcceptRequest` แบบ positional — พารามิเตอร์ท้ายเป็น optional จึง compile ต่อได้; การยอมรับสำเร็จสองจุดเดิม (`ExternalAcceptance_CustomerAcceptsOnce_WithEvidence_AndNoCostData`, `ExternalAcceptance_LinkOfAReplacedQuotation_CannotAcceptTheReplacement`) ต้องส่ง hash เพราะใบที่ออกผ่าน API ตอนนี้มีไฟล์เสมอ จึงแก้สองจุดนั้น; เคสที่คาดว่าล้ม (consent/ชื่อ/ลายเซ็น/ลิงก์ใช้ไม่ได้/rate limit) ล้มก่อนถึงการตรวจ hash จึงไม่ต้องแก้.

- [ ] **Step 1: แก้ `backend/src/TanErp.Application/Commercial/QuotationAcceptanceModels.cs`**
```text
>>> แทนที่:
public sealed record AcceptanceEvidenceSummary(
>>> ด้วย:
/// <summary>What the customer needs to identify the PDF they are accepting. Nothing internal (renderer, storage) is shown.</summary>
public sealed record PublicQuotationPdf(string Sha256, long ByteCount);

public sealed record AcceptanceEvidenceSummary(
>>> แทนที่:
    DateTimeOffset? AcceptedAtUtc,
    QuotationDocumentProjection Document);
>>> ด้วย:
    DateTimeOffset? AcceptedAtUtc,
    QuotationDocumentProjection Document,
    PublicQuotationPdf? Pdf);
>>> แทนที่:
    string? SignatureImage);
>>> ด้วย:
    string? SignatureImage,
    string? DocumentSha256);
```
- [ ] **Step 2: แก้ `backend/src/TanErp.Application/Commercial/QuotationAcceptanceHandler.cs`** — `QuotationAcceptanceHandler`: พารามิเตอร์ ctor
```text
>>> หลัง: using TanErp.Application.Common.Abstractions;
using TanErp.Application.Commercial.Documents;
>>> หลัง: private readonly IClock _clock;
    private readonly IQuotationDocumentStore _documents;
>>> แทนที่:
IEstimateStore estimates, IClock clock)
>>> ด้วย:
IEstimateStore estimates, IClock clock, IQuotationDocumentStore documents)
>>> หลัง: _clock = clock;
        _documents = documents;
>>> แทนที่:
        return Result<PublicAcceptanceView>.Success(new PublicAcceptanceView(context.LinkStatus, context.ExpiresAtUtc, context.SignerHint, AcceptanceConsent.CurrentVersion, context.AcceptedAtUtc, document.Value));
    }
>>> ด้วย:
        var pdf = await _documents.FindSummaryAsync(context.OrganizationId, context.QuotationId, ct);
        return Result<PublicAcceptanceView>.Success(new PublicAcceptanceView(
            context.LinkStatus, context.ExpiresAtUtc, context.SignerHint, AcceptanceConsent.CurrentVersion, context.AcceptedAtUtc, document.Value,
            pdf is null ? null : new PublicQuotationPdf(pdf.Sha256, pdf.ByteCount)));
    }

    /// <summary>The exact stored PDF of the quotation this link is for. Same unusable-link rules (and the same 404) as the view.</summary>
    public async Task<Result<QuotationDocumentFile>> GetPublicDocumentAsync(string? token, CancellationToken ct = default)
    {
        var context = await UsableContextAsync(token, false, ct);
        var file = context is null ? null : await _documents.OpenForAcceptanceAsync(context.OrganizationId, context.QuotationId, ct);
        return file is null ? Fail<QuotationDocumentFile>(UnavailableCode, "This acceptance link is not available.") : Result<QuotationDocumentFile>.Success(file);
    }
>>> แทนที่:
        // The accept transaction is keyed by the link
>>> ด้วย:
        // The customer attests to the exact PDF they were shown: its hash must equal the stored one, and "no PDF" (null) only matches
        // a quotation that has none. A PDF that appeared after the page was loaded is a conflict: reload and review again.
        var storedPdf = await _documents.FindSummaryAsync(context.OrganizationId, context.QuotationId, ct);
        if (!string.Equals(submission.DocumentSha256, storedPdf?.Sha256, StringComparison.Ordinal))
        {
            return Fail<PublicAcceptanceResult>("ACCEPTANCE_CONFLICT", "The document changed while you were reviewing it. Please reload and try again.");
        }

        // The accept transaction is keyed by the link
```
- [ ] **Step 3: แก้ `backend/src/TanErp.Infrastructure/Persistence/Commercial/QuotationAcceptanceStore.cs`**
```text
>>> แทนที่:
clientAddressHash, userAgent, now);
>>> ด้วย:
clientAddressHash, userAgent, now, submission.DocumentSha256);
```
- [ ] **Step 4: แก้ `backend/src/TanErp.Api/Contracts/Commercial/QuotationAcceptanceContracts.cs`**
```text
>>> แทนที่:
public sealed record AcceptanceEvidenceResponse(
>>> ด้วย:
/// <summary>Identifies the quotation PDF offered to the customer; the file itself is served by the public document endpoint.</summary>
public sealed record PublicQuotationPdfResponse(string Sha256, long ByteCount);

public sealed record AcceptanceEvidenceResponse(
>>> แทนที่:
    DateTimeOffset? AcceptedAtUtc,
    QuotationDocumentResponse Document);
>>> ด้วย:
    DateTimeOffset? AcceptedAtUtc,
    QuotationDocumentResponse Document,
    PublicQuotationPdfResponse? Pdf);
>>> แทนที่:
string? ConsentVersion, string? SignatureImage);
>>> ด้วย:
string? ConsentVersion, string? SignatureImage, string? DocumentSha256 = null);
```
- [ ] **Step 5: แก้ `backend/src/TanErp.Api/Controllers/PublicQuotationAcceptanceController.cs`**
```text
>>> แทนที่:
QuotationDocumentResponse.FromProjection(v.Document)));
    }
>>> ด้วย:
QuotationDocumentResponse.FromProjection(v.Document),
            v.Pdf is null ? null : new PublicQuotationPdfResponse(v.Pdf.Sha256, v.Pdf.ByteCount)));
    }

    [HttpGet("{token}/document")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> GetDocument([FromRoute] string token, CancellationToken ct)
    {
        NoStore();
        var result = await _handler.GetPublicDocumentAsync(token, ct);
        if (result.IsFailure) return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        var document = result.Value!;
        Response.Headers["X-Content-Type-Options"] = "nosniff";
        Response.Headers.ETag = $"\"{document.Summary.Sha256}\"";
        return File(document.Content, "application/pdf", document.FileName);
    }
>>> แทนที่:
request.ConsentAccepted, request.ConsentVersion, request.SignatureImage),
>>> ด้วย:
request.ConsentAccepted, request.ConsentVersion, request.SignatureImage, request.DocumentSha256),
```
- [ ] **Step 6: แก้ `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs`** — ปรับ helper และการยอมรับสำเร็จสองจุดของ test เดิมให้ส่ง hash
```text
>>> แทนที่:
string? version = "2026-10-v1") =>
        new(name, "กรรมการผู้จัดการ", consent, version, signature);
>>> ด้วย:
string? version = "2026-10-v1", string? documentSha256 = null) =>
        new(name, "กรรมการผู้จัดการ", consent, version, signature, documentSha256);

    // The customer page sends the hash of the PDF it showed; a test that wants a successful acceptance does the same.
    private async Task<string?> PublicPdfHashAsync(string token) =>
        (await (await PublicGetAsync(token)).Content.ReadFromJsonAsync<TanErp.Api.Contracts.Commercial.PublicAcceptanceViewResponse>())!.Pdf?.Sha256;
>>> แทนที่:
Submission(signature: TinyPng));
>>> ด้วย:
Submission(signature: TinyPng, documentSha256: await PublicPdfHashAsync(token)));
>>> แทนที่:
(await PublicAcceptAsync(newToken, Submission())).StatusCode
>>> ด้วย:
(await PublicAcceptAsync(newToken, Submission(documentSha256: await PublicPdfHashAsync(newToken)))).StatusCode
```
- [ ] **Step 7: แก้ `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.QuotationDocuments.cs`**
```text
>>> หลัง: // G-04 tests of later tasks are inserted below this line.
    [Fact]
    public async Task ExternalAcceptance_ServesTheStoredPdf_AndRecordsTheHashTheCustomerAttested()
    {
        var (_, _, quotation) = await IssueQuotedEstimateAsync("pdf-accept");
        var (linkId, token) = await CreateAcceptanceLinkAsync(quotation.QuotationId);
        var view = (await (await PublicGetAsync(token)).Content.ReadFromJsonAsync<TanErp.Api.Contracts.Commercial.PublicAcceptanceViewResponse>())!;

        var pdf = await _client.GetAsync($"/api/public/v1/quotation-acceptance/{token}/document");
        Assert.Equal(HttpStatusCode.OK, pdf.StatusCode);
        Assert.Equal("no-store", pdf.Headers.CacheControl?.ToString());
        Assert.Equal(view.Pdf!.Sha256, Sha256Of(await pdf.Content.ReadAsByteArrayAsync()));

        // A wrong hash, or no hash while a PDF exists, is a conflict and accepts nothing.
        Assert.Equal("ACCEPTANCE_CONFLICT", await ProblemCodeAsync(await PublicAcceptAsync(token, Submission(documentSha256: new string('0', 64)))));
        Assert.Equal("ACCEPTANCE_CONFLICT", await ProblemCodeAsync(await PublicAcceptAsync(token, Submission())));
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal("issued", (await db.Quotations.AsNoTracking().SingleAsync(q => q.Id == quotation.QuotationId)).Status);

        Assert.Equal(HttpStatusCode.OK, (await PublicAcceptAsync(token, Submission(documentSha256: view.Pdf.Sha256))).StatusCode);
        Assert.Equal(view.Pdf.Sha256, (await db.QuotationAcceptanceEvidences.AsNoTracking().SingleAsync(e => e.LinkId == linkId)).DocumentSha256);

        // The accepted link stays viewable, and an unknown token learns nothing.
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync($"/api/public/v1/quotation-acceptance/{token}/document")).StatusCode);
        var unknown = await _client.GetAsync($"/api/public/v1/quotation-acceptance/{new string('x', 43)}/document");
        Assert.Equal(HttpStatusCode.NotFound, unknown.StatusCode);
        Assert.Equal("ACCEPTANCE_LINK_UNAVAILABLE", await ProblemCodeAsync(unknown));
    }

    [Fact]
    public async Task ExternalAcceptance_OfALegacyQuotationWithoutPdf_StillWorks_WithNoHashRecorded()
    {
        var (_, _, quotation) = await IssueQuotedEstimateAsync("pdf-accept-legacy");
        await SimulateLegacyQuotationAsync(quotation.QuotationId);
        var (linkId, token) = await CreateAcceptanceLinkAsync(quotation.QuotationId);

        Assert.Null((await (await PublicGetAsync(token)).Content.ReadFromJsonAsync<TanErp.Api.Contracts.Commercial.PublicAcceptanceViewResponse>())!.Pdf);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/public/v1/quotation-acceptance/{token}/document")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await PublicAcceptAsync(token, Submission())).StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Null((await db.QuotationAcceptanceEvidences.AsNoTracking().SingleAsync(e => e.LinkId == linkId)).DocumentSha256);
    }
```
- [ ] **Step 8: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
```
Expected: `0 Warning(s)` `0 Error(s)` (TreatWarningsAsErrors เปิดอยู่; โปรเจกต์ test อยู่ใน solution จึงถูก compile ด้วย)
- [ ] **Step 9: Commit**
```bash
git add backend/src backend/tests
git commit -m "feat(commercial): bind customer acceptance to the stored quotation pdf hash

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 8: OpenAPI snapshot (live swagger), generated types, API client และ `saveBlob`

**Files:** Modify: `contracts/openapi/tan-erp.v1.json`, `frontend/src/generated/api/tan-erp.v1.ts`, `frontend/src/lib/api/api-client.ts`, `frontend/src/lib/api/api-client.test.ts`; Create: `frontend/src/lib/files/save-blob.ts`, `frontend/src/lib/files/save-blob.test.ts`

**Global Reuse Proposal (AGENTS.md §2, D13):** repo ยังไม่มี util กลางสำหรับ "บันทึก Blob เป็นไฟล์" (`export-csv.ts:44` และ `export-excel.ts:84` มี anchor+`createObjectURL` ซ้ำกันในแต่ละไฟล์) จึงสร้าง `lib/files/save-blob.ts` เป็นของกลาง — ไม่ refactor ตัว exporter เดิม (minimal blast radius). ถ้าผู้ใช้ไม่เห็นด้วย ให้ย้ายฟังก์ชันไปไว้ใน `quotation-lifecycle-queries.ts` แทน.

**OpenAPI:** ใช้วิธี live swagger + formatter (ไม่ใช้ `UPDATE_OPENAPI=1`). formatter ใน G-03 plan Task 9 **ไม่** ทำซ้ำ snapshot ปัจจุบันแบบ byte-ต่อ-byte (ตรวจแล้ว: `cmp` รายงานต่างที่บรรทัด 33576) — ใช้เวอร์ชันด้านล่าง (มี float handling) และต้องผ่าน `FORMATTER_OK` ก่อน; ถ้าไม่ผ่านให้หยุดและรายงาน.

- [ ] **Step 1: เตรียม formatter ชั่วคราว (ไม่ commit) และพิสูจน์ว่า round-trip snapshot เดิมได้ byte-ต่อ-byte**
```bash
cd /Users/syaco/Documents/development/tan-erp && S=${TMPDIR:-/tmp}/openapi-g04 && mkdir -p "$S" && cat > "$S/fmt.py" <<'PY'
import json, sys
SHORT = {"\\": "\\\\", "\n": "\\n", "\r": "\\r", "\t": "\\t", "\b": "\\b", "\f": "\\f"}
def esc(s):
    o = []
    for ch in s:
        c = ord(ch)
        if ch in SHORT: o.append(SHORT[ch])
        elif c < 0x20 or c >= 0x7F or ch in "\"&'+<>`":
            for u in ([c] if c <= 0xFFFF else [0xD800 + ((c - 0x10000) >> 10), 0xDC00 + ((c - 0x10000) & 0x3FF)]): o.append("\\u%04X" % u)
        else: o.append(ch)
    return '"' + "".join(o) + '"'
def w(v, d):
    p, q = "  " * (d + 1), "  " * d
    if isinstance(v, dict): return "{}" if not v else "{\n" + ",\n".join(p + esc(k) + ": " + w(x, d + 1) for k, x in v.items()) + "\n" + q + "}"
    if isinstance(v, list): return "[]" if not v else "[\n" + ",\n".join(p + w(x, d + 1) for x in v) + "\n" + q + "]"
    if isinstance(v, bool): return "true" if v else "false"
    if v is None: return "null"
    if isinstance(v, str): return esc(v)
    return repr(v).replace("e", "E") if isinstance(v, float) else json.dumps(v)
open(sys.argv[2], "w", encoding="utf-8", newline="").write(w(json.load(open(sys.argv[1], encoding="utf-8")), 0))
PY
python3 "$S/fmt.py" contracts/openapi/tan-erp.v1.json "$S/roundtrip.json" && cmp contracts/openapi/tan-erp.v1.json "$S/roundtrip.json" && echo FORMATTER_OK
```
- [ ] **Step 2: ดึง swagger จาก API ที่รันจริง → format → generate TypeScript types**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
S=${TMPDIR:-/tmp}/openapi-g04
ASPNETCORE_ENVIRONMENT=Test ASPNETCORE_URLS=http://127.0.0.1:5099 \
ConnectionStrings__Database="Host=127.0.0.1;Port=1;Database=openapi;Username=u;Password=p" \
  dotnet run --project backend/src/TanErp.Api --no-build --no-launch-profile > "$S/api.log" 2>&1 &
API_PID=$!
for i in $(seq 1 60); do curl -sf http://127.0.0.1:5099/swagger/v1/swagger.json -o "$S/swagger.json" && break; sleep 1; done
kill $API_PID 2>/dev/null; wait $API_PID 2>/dev/null; lsof -ti :5099 | xargs kill 2>/dev/null
test -s "$S/swagger.json" && python3 "$S/fmt.py" "$S/swagger.json" contracts/openapi/tan-erp.v1.json
git diff --numstat contracts/openapi/tan-erp.v1.json
cd frontend && npm run generate:api && cd .. && git diff --numstat frontend/src/generated/api/tan-erp.v1.ts
```
- [ ] **Step 3: ตรวจผล**
Expected: `numstat` ของ `tan-erp.v1.json` มี **deletions = 0** (เพิ่มเฉพาะ path `/api/v1/quotations/{id}/document` (GET), `/api/public/v1/quotation-acceptance/{token}/document`, schema `QuotationDocumentSummaryResponse`, `PublicQuotationPdfResponse` และ field `document`/`pdf`/`documentSha256`) ~180 บรรทัดเพิ่ม; `tan-erp.v1.ts` เพิ่มเท่านั้น. ถ้ามี deletions > 0 ให้ตรวจ `git diff` ว่าเป็น drift อื่นหรือ formatter ผิดก่อนไปต่อ.

- [ ] **Step 4: แก้ `frontend/src/lib/api/api-client.ts`** — เพิ่ม type, `execute` แบบ `read` ที่กำหนดได้ (default เดิม `response.json()`), และเมธอดดาวน์โหลด PDF
```text
>>> แทนที่:
export type PublicAcceptanceViewResponse = components["schemas"]["PublicAcceptanceViewResponse"];
>>> ด้วย:
export type PublicAcceptanceViewResponse = components["schemas"]["PublicAcceptanceViewResponse"];
export type PublicQuotationPdfResponse = components["schemas"]["PublicQuotationPdfResponse"];
export type QuotationDocumentSummaryResponse = components["schemas"]["QuotationDocumentSummaryResponse"];
>>> แทนที่:
    body: unknown,
    signal?: AbortSignal
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;
>>> ด้วย:
    body: unknown,
    signal?: AbortSignal,
    read: (response: Response) => Promise<T> = (response) => response.json()
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;
>>> แทนที่:
      const data: T = await response.json();
      return data;
>>> ด้วย:
      return await read(response);
>>> แทนที่:
  async getQuotationHistory(estimateId: string, options: RequestOptions)
>>> ด้วย:
  /** Authenticated binary download (the stored quotation PDF). Failures are problem-details errors exactly like JSON calls. */
  private async requestBlob(endpoint: string, options: RequestOptions): Promise<Blob> {
    const { token, membershipId, locale = "th", signal } = options;
    if (!token || token.trim() === "") {
      throw new ApiError({ status: 401, code: "AUTHENTICATION_REQUIRED", message: "Token is required." });
    }

    const headers: Record<string, string> = { Authorization: `Bearer ${token.trim()}`, "Accept-Language": locale, Accept: "application/pdf" };
    if (membershipId) {
      headers["X-Membership-Id"] = membershipId;
    }

    return this.execute<Blob>(endpoint, "GET", headers, undefined, signal, (response) => response.blob());
  }

  async downloadQuotationDocument(quotationId: string, options: RequestOptions): Promise<Blob> {
    return this.requestBlob(`/api/v1/quotations/${encodeURIComponent(quotationId)}/document`, options);
  }

  async getQuotationHistory(estimateId: string, options: RequestOptions)
>>> แทนที่:
  async acceptPublicQuotation(token: string, payload: PublicAcceptRequest, locale: "th" | "en")
>>> ด้วย:
  async downloadPublicQuotationDocument(token: string, signal?: AbortSignal): Promise<Blob> {
    return this.execute<Blob>(
      `/api/public/v1/quotation-acceptance/${encodeURIComponent(token)}/document`,
      "GET",
      { Accept: "application/pdf" },
      undefined,
      signal,
      (response) => response.blob()
    );
  }

  async acceptPublicQuotation(token: string, payload: PublicAcceptRequest, locale: "th" | "en")
```
- [ ] **Step 5: แก้ `frontend/src/lib/api/api-client.test.ts`** — เพิ่ม test ของ client (แทรกก่อน `});` ปิด describe ท้ายไฟล์)
```text
>>> แทนที่:
    expect(fetchMock.mock.calls[4][1].headers["If-Match"]).toBe('"v1"');
  });
});
>>> ด้วย:
    expect(fetchMock.mock.calls[4][1].headers["If-Match"]).toBe('"v1"');
  });

  it("downloads the stored quotation PDF as a blob with the bearer token and membership", async () => {
    const pdf = new Blob(["%PDF-"], { type: "application/pdf" });
    const fetchMock = vi.fn().mockResolvedValue({ ok: true, status: 200, blob: async () => pdf });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");
    expect(await client.downloadQuotationDocument("q 1", { token: "tok", membershipId: "m-1", locale: "th" })).toBe(pdf);

    const [url, init] = fetchMock.mock.calls[0];
    expect([url, init.method]).toEqual(["http://localhost:5000/api/v1/quotations/q%201/document", "GET"]);
    expect(init.headers).toMatchObject({ Authorization: "Bearer tok", Accept: "application/pdf", "X-Membership-Id": "m-1" });
  });

  it("downloads the public quotation PDF without any ERP credentials", async () => {
    const pdf = new Blob(["%PDF-"], { type: "application/pdf" });
    const fetchMock = vi.fn().mockResolvedValue({ ok: true, status: 200, blob: async () => pdf });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");
    expect(await client.downloadPublicQuotationDocument("t".repeat(43))).toBe(pdf);

    const [url, init] = fetchMock.mock.calls[0];
    expect(url).toBe(`http://localhost:5000/api/public/v1/quotation-acceptance/${"t".repeat(43)}/document`);
    expect(init.headers).toEqual({ Accept: "application/pdf" });
  });
});
```
- [ ] **Step 6: สร้าง `frontend/src/lib/files/save-blob.ts`**
```ts
/** Offers an in-memory file to the user as a download. Revokes the object URL right after the click. */
export function saveBlob(blob: Blob, filename: string): void {
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.setAttribute("href", url);
  link.setAttribute("download", filename);
  link.style.visibility = "hidden";
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  URL.revokeObjectURL(url);
}
```
- [ ] **Step 7: สร้าง `frontend/src/lib/files/save-blob.test.ts`**
```ts
import { afterEach, describe, expect, it, vi } from "vitest";
import { saveBlob } from "./save-blob";

describe("saveBlob", () => {
  afterEach(() => vi.restoreAllMocks());

  it("clicks a hidden download link with the file name and releases the object URL", () => {
    const createObjectURL = vi.fn(() => "blob:mock-pdf");
    const revokeObjectURL = vi.fn();
    Object.assign(URL, { createObjectURL, revokeObjectURL });
    const click = vi.spyOn(HTMLAnchorElement.prototype, "click").mockImplementation(() => undefined);

    saveBlob(new Blob(["%PDF-"], { type: "application/pdf" }), "QT-2026-0001.pdf");

    expect(createObjectURL).toHaveBeenCalledTimes(1);
    expect(click).toHaveBeenCalledTimes(1);
    expect(revokeObjectURL).toHaveBeenCalledWith("blob:mock-pdf");
    expect(document.querySelector("a[download]")).toBeNull();
  });
});
```
- [ ] **Step 8: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
git add contracts/openapi/tan-erp.v1.json frontend/src/generated/api/tan-erp.v1.ts   # check:api เทียบกับ index จึงต้อง stage ก่อน
cd frontend && npx tsc --noEmit && npm run lint && npm run check:api
```
Expected: tsc/lint exit 0 และ `check:api` exit 0 (generated file ตรงกับ snapshot)
- [ ] **Step 9: Commit**
```bash
git add contracts/openapi/tan-erp.v1.json frontend/src/generated/api/tan-erp.v1.ts frontend/src/lib/api frontend/src/lib/files
git commit -m "feat(commercial): expose quotation pdf endpoints in the api contract and client

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 9: Frontend: ปุ่มดาวน์โหลด PDF ในประวัติใบเสนอราคา และหน้าลูกค้า (CP-07) ที่แสดงไฟล์พร้อม hash

**Files:** สร้าง `features/estimates/components/quotation-document-actions.tsx` (+ `.test.tsx`); แก้ `quotation-lifecycle-queries.ts`, `quotation-lifecycle-status.ts`, `quotation-lifecycle-panel.tsx`, `public-acceptance-queries.ts`, `public-acceptance-page.tsx`, `public-acceptance-page.test.tsx`, `messages/th.json`, `messages/en.json`

**อ่าน `.agents/skills/building-erp-forms/SKILL.md` ก่อน** (ไม่มีฟอร์มใหม่ แต่ปุ่มต้อง `isLoading`+disable ระหว่างยิง และ error แสดงผ่าน i18n จาก code ที่รู้จักเท่านั้น). หน้า print เดิม (`quotation-document-page.tsx:80`, `window.print()`) **คงไว้** เป็น preview. **ห้ามแก้ `th.json`/`en.json` ด้วย `json.dumps`** (ไฟล์มี object inline หลายจุด จะเกิด diff ทั้งไฟล์) — ใช้ Insert ตาม anchor เท่านั้น.

**test เดิม:** `quotation-lifecycle-panel.test.tsx` ไม่ต้องแก้ — panel เรนเดอร์ `QuotationDocumentActions` เฉพาะเมื่อ `item.document` มีค่า และ fixture เดิมไม่มี `document` จึงไม่เรียก hook ใหม่ (ที่ต้องมี QueryClientProvider). `public-acceptance-page.test.tsx` แก้ 2 จุด เพราะหน้าเรียก `usePublicPdfDownload` ทุกครั้งและ payload ยอมรับมี `documentSha256` (fixture เดิมไม่มี `pdf` จึงเป็น `null`; `toHaveBeenCalledWith` แยก `null` ออกจาก field ที่ไม่มี).

- [ ] **Step 1: แก้ `frontend/src/features/estimates/api/quotation-lifecycle-queries.ts`** — queries: เพิ่ม `useQuotationDocumentDownload` (ก่อน `acceptanceLinksKey`)
```text
>>> หลัง: import { getAuthToken } from "@/lib/auth/auth-session";
import { saveBlob } from "@/lib/files/save-blob";
>>> แทนที่:
export function acceptanceLinksKey(
>>> ด้วย:
/** Downloads the stored quotation PDF (the file is never rendered again). */
export function useQuotationDocumentDownload() {
  const { options } = useRequestContext();
  return useMutation({
    mutationFn: async (input: { quotationId: string; fileName: string }) => {
      const blob = await apiClient.downloadQuotationDocument(input.quotationId, await options());
      saveBlob(blob, input.fileName);
    },
  });
}

export function acceptanceLinksKey(
```
- [ ] **Step 2: แก้ `frontend/src/features/estimates/quotation-lifecycle-status.ts`** — เพิ่ม code ใหม่ใน `KNOWN_ERROR_CODES` (ของ `QuotationLifecycleErrorCode`)
```text
>>> หลัง: "QUOTATION_VERSION_CONFLICT",
  "QUOTATION_DOCUMENT_NOT_AVAILABLE",
```
- [ ] **Step 3: สร้าง `frontend/src/features/estimates/components/quotation-document-actions.tsx`**
```tsx
"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ApiError } from "@/lib/api/api-error";
import type { QuotationHistoryItemResponse } from "@/lib/api/api-client";
import { useQuotationDocumentDownload } from "../api/quotation-lifecycle-queries";
import { quotationLifecycleErrorCode } from "../quotation-lifecycle-status";

const HASH_PREFIX_LENGTH = 16;

/** The stored PDF of one quotation: download it and see the start of its SHA-256. Quotations issued before PDFs were stored have none. */
export function QuotationDocumentActions({ item }: { item: QuotationHistoryItemResponse }) {
  const t = useTranslations("quotationLifecycle.document");
  const tErrors = useTranslations("quotationLifecycle.errors");
  const download = useQuotationDocumentDownload();
  const [message, setMessage] = useState<string | null>(null);

  const document = item.document ?? null;
  if (!item.id || !item.number || !document) return null;
  const quotationId = item.id;
  const fileName = `${item.number}.pdf`;

  async function onDownload(): Promise<void> {
    setMessage(null);
    try {
      await download.mutateAsync({ quotationId, fileName });
    } catch (err: unknown) {
      const code = err instanceof ApiError ? quotationLifecycleErrorCode(err.code) : null;
      setMessage(code ? tErrors(code) : tErrors("failed"));
    }
  }

  return (
    <div className="mt-2 space-y-2">
      {message && <Alert variant="danger" onClose={() => setMessage(null)}>{message}</Alert>}
      <div className="flex flex-wrap items-center gap-3">
        <Button type="button" size="sm" variant="outline" className="min-h-11" isLoading={download.isPending} disabled={download.isPending} onClick={() => void onDownload()}>
          {t("download")}
        </Button>
        <span className="font-mono text-erp-text-muted" title={document.sha256 ?? undefined}>
          {t("hash", { hash: document.sha256 ? document.sha256.slice(0, HASH_PREFIX_LENGTH) : "-" })}
        </span>
      </div>
    </div>
  );
}
```
- [ ] **Step 4: แก้ `frontend/src/features/estimates/components/quotation-lifecycle-panel.tsx`** — panel: แสดง `QuotationDocumentActions` ในใบที่มีไฟล์
```text
>>> หลัง: import { isQuotationStatus, quotationLifecycleErrorCode, quotationStatusVariant } from "../quotation-lifecycle-status";
import { QuotationDocumentActions } from "./quotation-document-actions";
>>> แทนที่:
            {item.status === "issued" && (canAmend || canVoid) && (
>>> ด้วย:
            {item.document && <QuotationDocumentActions item={item} />}
            {item.status === "issued" && (canAmend || canVoid) && (
```
- [ ] **Step 5: สร้าง `frontend/src/features/estimates/components/quotation-document-actions.test.tsx`**
```tsx
import React from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { NextIntlClientProvider } from "next-intl";
import thMessages from "@/messages/th.json";
import enMessages from "@/messages/en.json";
import { ApiError } from "@/lib/api/api-error";
import type { QuotationHistoryItemResponse } from "@/lib/api/api-client";
import { QuotationDocumentActions } from "./quotation-document-actions";
import * as lifecycleQueries from "../api/quotation-lifecycle-queries";

const d = thMessages.quotationLifecycle.document;
const download = vi.fn();
const withDocument = { document: { sha256: "a".repeat(64), byteCount: 100, rendererVersion: "v1", renderedAtUtc: "2026-10-12T00:00:00Z" } };

function renderActions(overrides: Partial<QuotationHistoryItemResponse>) {
  vi.spyOn(lifecycleQueries, "useQuotationDocumentDownload").mockReturnValue({ mutateAsync: download, isPending: false } as unknown as ReturnType<typeof lifecycleQueries.useQuotationDocumentDownload>);
  const item = { id: "q-1", number: "QT-2026-0001", status: "issued", ...overrides } as QuotationHistoryItemResponse;
  return render(
    <NextIntlClientProvider locale="th" messages={thMessages}>
      <QuotationDocumentActions item={item} />
    </NextIntlClientProvider>
  );
}

describe("QuotationDocumentActions", () => {
  beforeEach(() => vi.clearAllMocks());

  it("has Thai and English text for the document keys and the new error", () => {
    for (const messages of [thMessages, enMessages]) {
      expect(messages.quotationLifecycle.document.download).toBeTruthy();
      expect(messages.quotationLifecycle.document.hash).toBeTruthy();
      expect(messages.quotationLifecycle.errors.QUOTATION_DOCUMENT_NOT_AVAILABLE).toBeTruthy();
    }
  });

  it("downloads the stored PDF under the quotation number and shows the start of its hash", async () => {
    download.mockResolvedValue(undefined);
    renderActions(withDocument);
    expect(screen.getByText(d.hash.replace("{hash}", "a".repeat(16)))).toBeDefined();
    fireEvent.click(screen.getByRole("button", { name: d.download }));
    await waitFor(() => expect(download).toHaveBeenCalledWith({ quotationId: "q-1", fileName: "QT-2026-0001.pdf" }));
  });

  it("renders nothing for a quotation issued before PDFs were stored, and shows a translated error on failure", async () => {
    expect(renderActions({}).container.firstChild).toBeNull();
    download.mockRejectedValue(new ApiError({ status: 404, code: "QUOTATION_DOCUMENT_NOT_AVAILABLE", message: "raw" }));
    renderActions(withDocument);
    fireEvent.click(screen.getByRole("button", { name: d.download }));
    expect(await screen.findByText(thMessages.quotationLifecycle.errors.QUOTATION_DOCUMENT_NOT_AVAILABLE)).toBeDefined();
  });
});
```
- [ ] **Step 6: แก้ `frontend/src/features/external-acceptance/api/public-acceptance-queries.ts`**
```text
>>> หลัง: import { apiClient, type PublicAcceptanceViewResponse, type PublicAcceptRequest } from "@/lib/api/api-client";
import { saveBlob } from "@/lib/files/save-blob";
>>> แทนที่:
export function usePublicAcceptMutation(token: string, locale: DocumentLocale) {
>>> ด้วย:
/** The exact stored PDF the customer is asked to accept. Public like the rest of the page: the token in the URL is the credential. */
export function usePublicPdfDownload(token: string) {
  return useMutation({
    mutationFn: async (fileName: string) => {
      const blob = await apiClient.downloadPublicQuotationDocument(token);
      saveBlob(blob, fileName);
    },
  });
}

export function usePublicAcceptMutation(token: string, locale: DocumentLocale) {
```
- [ ] **Step 7: แก้ `frontend/src/features/external-acceptance/components/public-acceptance-page.tsx`**
```text
>>> หลัง: const mutation = usePublicAcceptMutation(token, documentLocale);
  const pdfDownload = usePublicPdfDownload(token);
>>> แทนที่:
import { usePublicAcceptance, usePublicAcceptMutation } from
>>> ด้วย:
import { usePublicAcceptance, usePublicAcceptMutation, usePublicPdfDownload } from
>>> หลัง: signatureImage: signature,
        // The hash of the PDF shown above; null only when the quotation has no stored PDF. The server rejects any other value.
        documentSha256: view.pdf?.sha256 ?? null,
>>> หลัง: <QuotationDocumentView document={document} documentLocale={documentLocale} label={label} />

      {view.pdf?.sha256 && document.number && (
        <section className="erp-card space-y-3 p-5 print:hidden" aria-label={t("pdfTitle")}>
          <h2 className="text-base font-bold text-erp-navy">{t("pdfTitle")}</h2>
          <p className="text-xs text-erp-text-muted">{t("pdfDescription")}</p>
          <div className="flex flex-wrap items-center gap-3">
            <Button type="button" variant="outline" className="min-h-11" isLoading={pdfDownload.isPending} disabled={pdfDownload.isPending}
              onClick={() => void pdfDownload.mutateAsync(`${document.number}.pdf`).catch(() => setError(t("pdfFailed")))}>
              {t("pdfDownload")}
            </Button>
            <span className="break-all font-mono text-xs text-erp-text-muted">{t("pdfHash", { hash: view.pdf.sha256 })}</span>
          </div>
        </section>
      )}
```
- [ ] **Step 8: แก้ `frontend/src/features/external-acceptance/components/public-acceptance-page.test.tsx`** — public page test: mock hook ใหม่, แก้ payload ที่คาด, เพิ่มเคสที่มีไฟล์
```text
>>> แทนที่:
  vi.spyOn(publicQueries, "usePublicAcceptMutation").mockReturnValue(mutation as unknown as ReturnType<typeof publicQueries.usePublicAcceptMutation>);
>>> ด้วย:
  vi.spyOn(publicQueries, "usePublicAcceptMutation").mockReturnValue(mutation as unknown as ReturnType<typeof publicQueries.usePublicAcceptMutation>);
  vi.spyOn(publicQueries, "usePublicPdfDownload").mockReturnValue({ mutateAsync: vi.fn(), isPending: false } as unknown as ReturnType<typeof publicQueries.usePublicPdfDownload>);
>>> แทนที่:
consentVersion: "2026-10-v1", signatureImage: null }));
>>> ด้วย:
consentVersion: "2026-10-v1", signatureImage: null, documentSha256: null }));
>>> แทนที่:
  it("shows the translated error instead of the raw message", async () => {
>>> ด้วย:
  it("sends the hash of the PDF it showed, and shows that PDF with its hash", async () => {
    mutate.mockResolvedValue({});
    const hash = "c".repeat(64);
    renderPage({ data: view({ pdf: { sha256: hash, byteCount: 1234 } }) });

    expect(screen.getByText(p.pdfHash.replace("{hash}", hash))).toBeDefined();
    fireEvent.change(screen.getByLabelText(new RegExp(p.signerName)), { target: { value: "คุณสมชาย ใจดี" } });
    fireEvent.click(screen.getByRole("checkbox"));
    fireEvent.click(screen.getByRole("button", { name: p.accept }));
    await waitFor(() => expect(mutate).toHaveBeenCalledWith(expect.objectContaining({ documentSha256: hash })));
  });

  it("shows the translated error instead of the raw message", async () => {
```
- [ ] **Step 9: แก้ `frontend/src/messages/th.json`**
```text
>>> หลัง: "QUOTATION_ACCEPTED_LOCKED": "ใบเสนอราคาที่ลูกค้ายอมรับแล้วแก้ไขหรือยกเลิกไม่ได้",
      "QUOTATION_DOCUMENT_NOT_AVAILABLE": "ใบเสนอราคานี้ยังไม่มีไฟล์ PDF ที่เก็บไว้",
>>> หลัง: "amended": "ออกใบเสนอราคาฉบับใหม่แล้ว",
    "document": { "download": "ดาวน์โหลด PDF", "hash": "SHA-256 {hash}…" },
>>> หลัง: "acceptedDetail": "บันทึกเมื่อ {date} ขอบคุณที่ใช้บริการ",
      "pdfTitle": "ไฟล์ PDF ฉบับที่ใช้ยอมรับ",
      "pdfDescription": "รายละเอียดและยอดรวมในไฟล์นี้คือฉบับที่คุณยอมรับ ตรวจสอบว่าไฟล์ตรงกันได้ด้วยค่า SHA-256",
      "pdfDownload": "ดาวน์โหลด PDF",
      "pdfHash": "SHA-256: {hash}",
      "pdfFailed": "ดาวน์โหลดไฟล์ไม่สำเร็จ โปรดลองอีกครั้ง",
```
- [ ] **Step 10: แก้ `frontend/src/messages/en.json`**
```text
>>> หลัง: "QUOTATION_ACCEPTED_LOCKED": "An accepted quotation cannot be amended or voided.",
      "QUOTATION_DOCUMENT_NOT_AVAILABLE": "This quotation has no stored PDF yet.",
>>> หลัง: "amended": "Replacement quotation issued.",
    "document": { "download": "Download PDF", "hash": "SHA-256 {hash}…" },
>>> หลัง: "acceptedDetail": "Recorded on {date}. Thank you.",
      "pdfTitle": "The PDF you are accepting",
      "pdfDescription": "The details and totals in this file are the version you accept. Check that a file matches with its SHA-256 value.",
      "pdfDownload": "Download PDF",
      "pdfHash": "SHA-256: {hash}",
      "pdfFailed": "The file could not be downloaded. Please try again.",
```
- [ ] **Step 11: Compile-only gate (ห้ามรันชุดทดสอบ)**
```bash
cd frontend && npx tsc --noEmit && npm run lint
node -e 'const t=require("./src/messages/th.json"),e=require("./src/messages/en.json");const f=(o,p="")=>Object.entries(o).flatMap(([k,v])=>typeof v==="object"&&v!==null?f(v,p+k+"."):[p+k]);const a=f(t).sort().join("\n"),b=f(e).sort().join("\n");console.log(a===b?"KEY_PARITY_OK":"KEY_PARITY_MISMATCH")'
npm run build
```
Expected: tsc/lint exit 0; `KEY_PARITY_OK`; `npm run build` exit 0 (`npm run build` รันครั้งเดียวที่ Task นี้)
- [ ] **Step 12: Commit**
```bash
git add frontend/src
git commit -m "feat(commercial): download the stored quotation pdf from history and the customer page

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## Task 10: Verification และเอกสารสถานะ (ไม่ติ๊กว่าตรวจแล้ว)

**Files:** Create: `docs/05-engineering/quotation-pdf-verification.md`; Modify: `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`, `docs/00-overview/implementation-roadmap.md`

**กฎ:** ไม่รันชุดทดสอบใด ๆ — เอกสารต้องบอกตรง ๆ ว่า **tests written, not run**. **ห้ามติ๊ก `[x]` ข้อใดของ G-04** — checkbox ทุกข้อคง `[ ]` และสถานะอยู่ในบล็อกที่เพิ่ม (tests written, not run); แทน `<วันที่จริง>` ด้วยผล `date +%F` (ทุกจุด). โลโก้ต้องระบุว่ายังไม่ทำ.

- [ ] **Step 1: compile/lint gate สุดท้าย (เท่านั้นที่รันได้)**
```bash
dotnet build backend/TanErp.slnx --nologo -v q
cd frontend && npx tsc --noEmit && npm run lint && npm run check:api
cd .. && ConnectionStrings__Database="Host=localhost;Database=design;Username=x;Password=x" dotnet ef migrations has-pending-model-changes \
  --project backend/src/TanErp.Infrastructure --startup-project backend/src/TanErp.Api
git status --short | grep -v "^??" ; git status --short | grep -E "local-storage|\.ttf" | head
```
- [ ] **Step 2: ตรวจผล**
Expected: `0 Warning(s)` `0 Error(s)`; tsc/lint/`check:api` exit 0; `has-pending-model-changes` รายงานว่าไม่มีการเปลี่ยนโมเดลค้าง (ถ้ามี ห้ามสร้าง migration ใน Task นี้ — หยุดและรายงาน); `git status` ไม่มีไฟล์ใน `local-storage` ติดมา (ถ้ามี ห้าม commit). `npm run build` และ `KEY_PARITY_OK` ผ่านแล้วใน Task 9 — บันทึกเป็น "ผลจาก Task 9". จดผลจริงลงเอกสาร.

- [ ] **Step 3: เขียนเอกสาร verification**
สร้าง `docs/05-engineering/quotation-pdf-verification.md` (ภาษาไทย รูปแบบเดียวกับ `membership-expiry-verification.md`) ประกอบด้วย:

1. **สถานะ:** `Implemented <วันที่จริงจาก date +%F> (G-04: Server-side Quotation PDF + artifact hash) — tests written, not run.` ชุดทดสอบ unit/integration/frontend ถูกเขียนแล้วแต่ **ยังไม่ได้รัน** (ผู้ใช้รันเอง); ยังไม่ตรวจในเบราว์เซอร์; ยังไม่รัน full gate; ยังไม่ผ่าน UAT/Production; **ยังไม่ได้รับการยืนยัน license จาก Business**. ลิงก์ ADR 0023, แผน, [Quotation Document API](../03-contracts/quotation-document-api-contract.md).
2. **ผลที่รันจริง (compile/lint เท่านั้น):** ตารางคำสั่ง → ผลจริงจาก Step 1 ของ Task นี้ และจาก Task ก่อนหน้า (`dotnet build`, `has-pending-model-changes`, `FORMATTER_OK`, `check:api`, `tsc`, `lint`, `KEY_PARITY_OK`, `npm run build`).
3. **คำสั่งที่ผู้ใช้ควรรันเอง** (คัดลอกคำสั่งจากหัวข้อ "คำสั่งสำหรับผู้ใช้" ด้านล่างของแผนนี้ตรง ๆ; ห้ามเขียนว่าผ่าน).
4. **สิ่งที่ชุดทดสอบตั้งใจพิสูจน์:** allowlist ของ model (reflection: property ตรงรายการที่ทบทวนแล้ว, ไม่มีชื่อที่สื่อถึงต้นทุน/มาร์จิน/id ภายใน), renderer (ไทย/อังกฤษ, แบ่งหน้า, ข้อความยาวสุด 500 ตัวอักษร, ฟอนต์+OFL ฝังใน assembly, license parser), ไฟล์ที่เก็บ hash ตรง bytes และ audit ไม่มีข้อมูลลูกค้า, render พังแล้ว rollback ทั้งหมด, amend ได้ไฟล์ใหม่และไฟล์เดิมไม่เปลี่ยน, ไฟล์ไม่เปลี่ยนเมื่อแก้ข้อมูลองค์กรภายหลัง, cross-organization/branch → 404 และขาดสิทธิ์ → 403, ใบเก่า → 404 `QUOTATION_DOCUMENT_NOT_AVAILABLE`, หน้าลูกค้า (ได้ไฟล์ตรง hash, hash ผิด/ขาด → `ACCEPTANCE_CONFLICT`, หลักฐานเก็บ hash, ใบเก่ายอมรับได้ด้วย hash เป็น null), frontend (ปุ่มดาวน์โหลด, ข้อความ error ตาม code, หน้าลูกค้าส่ง hash).
5. **สิ่งที่ยังไม่ได้ทำ/ต้องทำก่อนเปิดใช้จริง:** (ก) **Business ยืนยันเงื่อนไข license QuestPDF** (รายได้รวม < 1,000,000 USD, ไม่ใช่บริษัทมหาชน/หน่วยงานรัฐ) แล้วบันทึกผู้ยืนยัน/วันที่ในหัวข้อนี้และตั้ง `Documents__QuestPdf__LicenseConfirmed=true` ใน Production; ถ้าไม่ผ่านให้ตั้ง `Documents__QuestPdf__LicenseType=Professional` หลังซื้อ; (ข) Sales/Finance ตรวจ layout ของ PDF จริง (เปิดไฟล์ ดูการเรียงสระ/วรรณยุกต์ไทย, แบ่งหน้า, ยอดสรุป) และข้อมูลหัวเอกสาร (ชื่อ/เลขผู้เสียภาษี/ที่อยู่ขององค์กรและสาขาต้องกรอกใน Settings ก่อน — ช่องว่างจะไม่ถูกพิมพ์); (ค) ตรวจเชิงปฏิบัติการ: ออกใบเสนอราคา 1 ใบ → ดาวน์โหลด → `shasum -a 256` ต้องตรงกับ hash ในประวัติ → เปิดหน้าลูกค้าและเทียบ hash; ตั้ง `Storage:BasePath` ให้ชี้ที่เก็บถาวร (ค่า default อยู่ใต้โฟลเดอร์แอป).
6. **การตัดสินใจที่ต้องให้ Sales/Finance/Legal/Business ยืนยัน:** layout และข้อมูลหัวเอกสาร, เอกสารสองภาษาในไฟล์เดียว (D3), ไม่ backfill ใบเก่า (D7: ใช้ Amend), license (D1), ไม่มีโลโก้ (D8), ไม่ใช่ PDF/A และไม่ใช่ลายเซ็นอิเล็กทรอนิกส์ตามกฎหมาย.
7. **ข้อจำกัดที่ยอมรับ:** transaction ออกใบเสนอราคาเปิดนานขึ้นเท่าเวลา render; blob กำพร้าเมื่อ commit ล้มหลังเขียนไฟล์ (ไม่มี job กวาด; ลบอัตโนมัติเฉพาะ flow สร้างไฟล์ใบเก่า); `GET /api/v1/estimates/{id}/quotations` ยังตรวจเฉพาะ Organization ไม่ตรวจ Branch (ของเดิม ไม่แก้ในรอบนี้ — endpoint ใหม่ตรวจ Branch); ไม่ audit การดาวน์โหลด; ข้อมูลองค์กรอยู่ในไฟล์เท่านั้น.
8. **ยังเปิดอยู่:** โลโก้องค์กร (follow-up), ผู้ยืนยัน license.
- [ ] **Step 4: แก้ `docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md`** — เพิ่มบล็อกสถานะใต้หัวข้อ G-04 (checkbox ทุกข้อคง `[ ]`)
```text
>>> แทนที่:
| G-04 | Server-side Quotation PDF + artifact hash | B | CP-04, G-01 | Sales + Finance | — |
>>> ด้วย:
| G-04 | Server-side Quotation PDF + artifact hash | B | CP-04, G-01 | Sales + Finance | Implemented <วันที่จริง> (tests written, not run; browser not checked; ยังไม่ผ่าน UAT; **เงื่อนไข license QuestPDF รอ Business ยืนยันก่อน production**; โลโก้ยังไม่ทำ) — [แผน G-04](2026-10-12-g04-quotation-pdf.md); [ADR 0023](../../adr/0023-server-side-quotation-pdf-with-questpdf.md); [Verification](../../05-engineering/quotation-pdf-verification.md) |
>>> หลัง: projection ของ CP-04, ADR ใหม่ที่ supersede/เสริม

> **สถานะ G-04 (<วันที่จริง>):** Implemented — tests written, not run; browser not checked; ยังไม่ผ่าน UAT. ทุกข้อด้านล่างคง `[ ]` (ยังไม่ตรวจจริง). ทำแล้ว: ADR 0023 ([QuestPDF 2026.9.1 + Sarabun ฝังใน assembly](../../adr/0023-server-side-quotation-pdf-with-questpdf.md)), `IQuotationDocumentRenderer`, render ครั้งเดียวตอน issue/amend ใน unit of work เดียวกัน → `UploadedFile` + `quotation_documents` (sha256 ผูก quotation; ใช้ Files module ไม่ใช่ `AttachmentLink`), pagination/ข้อความยาว/ไทย-อังกฤษ/ข้อมูลองค์กร-สาขา, allowlist + test, cross-scope 404, หน้าลูกค้า CP-07 ดาวน์โหลดไฟล์พร้อม SHA-256 และส่ง hash ตอนยอมรับ (ไม่ตรง → `ACCEPTANCE_CONFLICT`, หลักฐานเก็บ `document_sha256`). **ยังไม่ทำ/ยังเปิด:** โลโก้ (follow-up), PDF ย้อนหลังของใบเก่า (ตอบ `404 QUOTATION_DOCUMENT_NOT_AVAILABLE`; ใช้ Amend), **เงื่อนไข license QuestPDF รอ Business ยืนยันก่อน production**. หมายเหตุ: hash คงที่เพราะเก็บไฟล์ครั้งเดียว ไม่ใช่เพราะ renderer deterministic. [Verification](../../05-engineering/quotation-pdf-verification.md)
```
- [ ] **Step 5: แก้ `docs/00-overview/implementation-roadmap.md`** — อัปเดตแถว Commercial/Document Numbering ใน roadmap (2 จุดในแถวเดียวกัน)
```text
>>> แทนที่:
และยังไม่มี server-side PDF/artifact hash ([ADR 0016](../adr/0016-browser-print-for-quotation-pdf.md));
>>> ด้วย:
และมี server-side PDF ที่เก็บครั้งเดียวพร้อม SHA-256 แล้ว (G-04, <วันที่จริง>; [ADR 0023](../adr/0023-server-side-quotation-pdf-with-questpdf.md) เสริม [ADR 0016](../adr/0016-browser-print-for-quotation-pdf.md); tests written but not run, ยังไม่ตรวจในเบราว์เซอร์, ยังไม่ผ่าน UAT, เงื่อนไข license QuestPDF รอ Business ยืนยัน, ยังไม่มีโลโก้);
>>> แทนที่:
rate limit ([External Acceptance API](../03-contracts/external-acceptance-api-contract.md)) |
>>> ด้วย:
rate limit ([External Acceptance API](../03-contracts/external-acceptance-api-contract.md)); Quotation PDF (G-04): เก็บไฟล์ครั้งเดียวต่อฉบับ, ใบเก่าไม่มีไฟล์ (ใช้ Amend), หน้าลูกค้ายืนยัน hash ([Quotation Document API](../03-contracts/quotation-document-api-contract.md), [Quotation PDF Verification](../05-engineering/quotation-pdf-verification.md)) |
```
- [ ] **Step 6: ตรวจลิงก์ภายใน (ต้องไม่มีบรรทัด MISSING) และว่าไม่เหลือ `<วันที่จริง>` (`grep -c` ทุกไฟล์ต้องลงท้าย `:0`)**
```bash
cd /Users/syaco/Documents/development/tan-erp && python3 - <<'PY'
import re, pathlib
files = ["docs/adr/0023-server-side-quotation-pdf-with-questpdf.md", "docs/05-engineering/quotation-pdf-verification.md", "docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md", "docs/00-overview/implementation-roadmap.md"]
for f in files:
    base = pathlib.Path(f).parent
    for target in re.findall(r"\]\(([^)#]+\.md)(?:#[^)]*)?\)", pathlib.Path(f).read_text(encoding="utf-8")):
        if not target.startswith("http") and not (base / target).resolve().exists():
            print("MISSING", f, target)
print("LINK_CHECK_DONE")
PY
grep -c "<วันที่จริง>" docs/05-engineering/quotation-pdf-verification.md docs/superpowers/plans/2026-10-05-erp-gap-closure-plan.md docs/00-overview/implementation-roadmap.md docs/03-contracts/quotation-document-api-contract.md
```
- [ ] **Step 7: Commit**
```bash
git add docs
git commit -m "docs(commercial): record G-04 verification and update status

Co-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>"
```

---

## คำสั่งสำหรับผู้ใช้ (รันเอง — implementer ห้ามรัน)

```bash
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~QuotationPdfModelAllowlistTests|FullyQualifiedName~QuotationDocumentTests"
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~QuestPdfQuotationRendererTests"
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateEndpointsTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
cd frontend && npx vitest run src/features/estimates src/features/external-acceptance src/lib/files src/lib/api/api-client.test.ts
```

ก่อนใช้จริง: ตั้ง `Storage:BasePath` ให้ชี้ที่เก็บถาวร (ไฟล์ PDF คือหลักฐาน), กรอกชื่อ/เลขผู้เสียภาษี/ที่อยู่ขององค์กรและสาขาใน Settings, ให้ Business ยืนยัน license (ADR 0023) แล้วตั้ง `Documents__QuestPdf__LicenseConfirmed=true`, แล้ว apply migration `AddQuotationDocuments`.

## Self-review (ตรวจแล้ว)

- ทุก path ในแผนมีอยู่จริงหรือทำเครื่องหมาย Create; โค้ดใหม่ทุกไฟล์และทุกการแก้ไฟล์เดิมถูกนำไปใช้กับสำเนาสะอาดของ `main` ตามลำดับ Task แล้ว `dotnet build backend/TanErp.slnx` (0 warning/0 error) และ `tsc --noEmit` ผ่านหลังแต่ละ Task ที่เกี่ยวข้อง; `has-pending-model-changes` สะอาดหลัง Task 3; OpenAPI regenerate ได้ deletions = 0; ไม่มีขั้นใดบอกว่า "เหมือน…" โดยไม่มีโค้ด.
- Error code ใหม่เพียงตัวเดียว (`QUOTATION_DOCUMENT_NOT_AVAILABLE`) และเพิ่มพร้อม `GetStatus` + `Errors.resx` + `Errors.en.resx` ใน Task 6 ที่เดียวกัน.
- ไม่มี permission ใหม่, ไม่มี migration นอก Task 3, ไม่มีขั้นที่สั่งรันชุดทดสอบ; Task 10 ไม่ติ๊กข้อใดว่าตรวจแล้ว.
