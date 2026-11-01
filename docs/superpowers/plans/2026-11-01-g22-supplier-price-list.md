# G-22 Supplier Price List + Lead Time Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** เก็บราคาและ lead time ของผู้ขายต่อ Item (ปิดช่องว่าง G-14 "Supplier price list/lead time ต่อ Item") แล้วให้ PO และ MRP ใช้ค่าเหล่านี้

**Architecture:** เพิ่ม aggregate `SupplierItemPrice` ใน `TanErp.Domain/Procurement` (EF Core เป็นเจ้าของ write), CRUD ผ่าน `ProcurementHandler`/`IProcurementStore` เดิม, อ่านราคาที่มีผลตามวันที่ผ่าน query เดียว. MRP อ่าน lead time ของ Supplier ตัวหลักเข้า snapshot (`MrpItemSnapshot.LeadTimeDays`) โดยไม่เปลี่ยน `InputHash` ของ run เก่า.

**Tech Stack:** .NET 10, EF Core + PostgreSQL, Next.js + TanStack Query, i18n th/en.

> ข้อห้ามจาก memory ของผู้ใช้: **ไม่เขียน test ใหม่** และไม่รัน test; ตรวจด้วย `dotnet build`, `npx tsc --noEmit`, `npx eslint <ไฟล์>` เท่านั้น (ผู้ใช้รัน test เอง).

## กฎธุรกิจตั้งต้น (`TEST_ONLY` รอ Procurement/Finance ยืนยัน)

1. หนึ่งแถวคือ (Supplier, Item, หน่วยฐานของ Item) → `UnitPrice` (บาท, ≥ 0, 4 ทศนิยม), `MinOrderQuantity` (≥ 0), `LeadTimeDays` (0–365), `EffectiveFrom`, `EffectiveTo?`.
2. ช่วงวันที่ของ (Supplier, Item) เดียวกัน **ห้ามทับกัน** → `SUPPLIER_PRICE_PERIOD_OVERLAP` (409).
3. `IsPreferred`: ต่อ Item มี Supplier ตัวหลักได้ **หนึ่งรายที่มีผล ณ วันใดวันหนึ่ง**; ตั้งตัวใหม่แล้วถอดตัวเดิมใน Transaction เดียว.
4. PO draft: ถ้าไม่ส่ง `unitPrice` ในบรรทัด ระบบเติมจากราคาที่มีผล ณ วันที่สร้าง; ถ้าไม่มีราคา → `SUPPLIER_PRICE_NOT_FOUND` (422). ถ้าส่งมาเอง ใช้ค่าที่ส่ง (ไม่ override).
5. MRP: lead time ของข้อเสนอ Buy = lead time ใน price list ของ Supplier ตัวหลัก ถ้ามี ไม่งั้นใช้ `MrpItemPlanningParameter.LeadTimeDays` แล้วค่อย default ของรอบ (ลำดับ fallback ตายตัว ไม่เดา field อื่น).
6. Supplier ที่ `inactive` ห้ามเพิ่ม/แก้ราคาใหม่ (`SUPPLIER_INVALID_STATE`).

## Permissions

ใช้ `suppliers.read` / `suppliers.manage` เดิม (ไม่เพิ่ม permission ใหม่ → ไม่ต้อง seed role).

## File Structure

| ไฟล์ | หน้าที่ |
| --- | --- |
| Create `backend/src/TanErp.Domain/Procurement/SupplierItemPrice.cs` | Entity + invariant (ช่วงวันที่, ช่วงค่า) |
| Modify `backend/src/TanErp.Domain/Procurement/ProcurementValues.cs` | เพิ่ม error code ถ้ามีรายการรหัสรวม |
| Modify `backend/src/TanErp.Infrastructure/Persistence/Configurations/ProcurementConfigurations.cs` | `SupplierItemPriceConfiguration` (ตาราง `procurement.supplier_item_prices`, unique/exclusion ช่วงวันที่) |
| Modify `backend/src/TanErp.Infrastructure/Persistence/AppDbContext.cs` | `DbSet<SupplierItemPrice>` |
| Create migration `AddSupplierItemPrices` | สร้างตาราง + index; **ดูข้อควรระวังด้านล่าง** |
| Modify `backend/src/TanErp.Application/Procurement/ProcurementModels.cs` | `SupplierItemPriceInput/Projection`, `PagedSupplierItemPrices` |
| Modify `backend/src/TanErp.Application/Procurement/IProcurementStore.cs` | Create/Update/Delete/List/Resolve methods |
| Modify `backend/src/TanErp.Application/Procurement/ProcurementHandler.cs` | Validation + permission + เติมราคา PO |
| Modify `backend/src/TanErp.Infrastructure/Persistence/Procurement/*Store*.cs` | implement store |
| Create `backend/src/TanErp.Api/Controllers/SupplierItemPricesController.cs` | `/api/v1/suppliers/{supplierId}/item-prices` |
| Modify `backend/src/TanErp.Api/Contracts/Procurement/*` | Request/Response |
| Modify MRP snapshot loader ใน `backend/src/TanErp.Infrastructure/Persistence/Mrp/` | ใส่ lead time จาก price list |
| Modify `docs/03-contracts/procurement-api-contract.md`, `docs/05-engineering/procurement-verification.md`, roadmap | เอกสาร |
| Frontend `frontend/src/features/procurement/**`, `messages/th.json`, `messages/en.json` | แท็บ "ราคา/Lead time" ในหน้า Supplier `[id]` (ใช้ `FormTabs`, ตารางแบบ Table-Preserved) |

## Tasks

### Task 1: Domain + persistence
- [ ] สร้าง `SupplierItemPrice` ตามรูปแบบ `MrpItemPlanningParameter` (constructor ตรวจ guid, `Apply` ตรวจช่วงค่า, `RowVersion`), ใช้ `ProcurementDomainException` กับรหัสด้านบน.
- [ ] เพิ่ม Configuration + DbSet; index `(OrganizationId, SupplierId, ItemId, EffectiveFrom)` และ partial unique `(OrganizationId, ItemId) WHERE is_preferred AND effective_to IS NULL`.
- [ ] `dotnet build backend` → ต้อง 0 error.

### Task 2: Application layer
- [ ] เพิ่ม DTO/Interface/Handler methods (ทุก method ผ่าน `AccessAsync(caller, "suppliers.manage"|"suppliers.read")`, ใช้ `Fail<T>(code, msg)` เดิม).
- [ ] Overlap check และสลับ preferred ทำใน store ภายใน transaction เดียว (ใช้ `SerializableTransactionRunner` เหมือน store อื่น).
- [ ] เติมราคา PO ใน `CreatePurchaseOrderAsync`/`UpdatePurchaseOrderAsync` ตามกฎข้อ 4 (เปลี่ยน `PurchaseOrderLineInput.UnitPrice` เป็น `decimal?` → แก้ทุก caller: `ConvertPurchaseRequests`, MRP convert-to-PO).
- [ ] `dotnet build backend`.

### Task 3: API + migration
- [ ] Controller + contracts + OpenAPI attributes ตามรูปแบบ `SuppliersController`.
- [ ] สร้าง migration: **ก่อนทำต้องตรวจว่า `AppDbContextModelSnapshot.cs` มีการแก้ค้างจากงานอื่น (git status แสดง modified)**; ถ้ามี ให้สร้าง migration หลัง commit/stash งานนั้น เพื่อไม่ให้ migration ลากเอาการเปลี่ยน schema ของงานอื่นเข้ามา.
- [ ] ตรวจ SQL ที่ generate ด้วย `dotnet ef migrations script` ว่ามีแต่ตาราง/ index ของ slice นี้.

### Task 4: MRP lead time
- [ ] ที่ snapshot loader เพิ่ม join `SupplierItemPrice` (preferred, มีผล ณ `AsOfDate`) → `LeadTimeDays` ถ้ามี.
- [ ] ยืนยันว่า `InputHash` ของ run เดิมไม่เปลี่ยน (hash ใช้เฉพาะ payload เดิม; ไม่เพิ่ม field เข้า hash ของ run ที่ไม่มี price list).
- [ ] `dotnet build backend`.

### Task 5: Frontend + i18n
- [ ] Hook/API client ใต้ `frontend/src/features/procurement/` (TanStack Query; ห้าม fetch ใน `useEffect`), generate API types ตามขั้นตอน parity ของโปรเจกต์.
- [ ] แท็บราคาในหน้า Supplier: ตารางใช้ `@/components/ui/Table`, ปุ่ม "+ เพิ่มราคา" `variant="primary"`, input สูง `h-11`, ทุก input มี placeholder, ลบผ่าน Confirmation Modal.
- [ ] เพิ่มคีย์ใน `messages/th.json` และ `en.json` ครบคู่.
- [ ] `npx tsc --noEmit`, `npx eslint <ไฟล์ที่แก้>`.

### Task 6: Docs + final gate
- [ ] อัปเดต API contract, verification doc, roadmap (ตัดข้อค้าง "Supplier price list/lead time", ใส่ `TEST_ONLY` rules).
- [ ] รัน gate เต็มครั้งเดียว: `dotnet build`, `npm run build`, `npm run lint` (ไม่รัน `dotnet test` ตามที่ผู้ใช้กำหนด) แล้วรายงานผลจริง.

## Self-review

- ครอบคลุม: ราคา, lead time, PO auto-fill, MRP, UI, docs. ไม่รวม: multi-currency, tier price ตามปริมาณ, การนำเข้า CSV (ระบุเป็น out of scope).
- จุดเสี่ยง: เปลี่ยน `PurchaseOrderLineInput.UnitPrice` เป็น nullable กระทบ caller หลายจุด; migration ปนกับ snapshot ที่แก้ค้าง.
