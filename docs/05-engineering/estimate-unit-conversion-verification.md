# Estimate Unit Conversion Verification (G-08: Estimate Unit Conversion Snapshot)

## 1. สถานะ

**Implemented 2026-10-15 (G-08: Estimate Unit Conversion Snapshot) — no tests written, none run (per user).**

- ตามคำสั่งผู้ใช้ รอบนี้ **ไม่ได้เขียน test ใหม่** และ **ไม่ได้รัน test ใด ๆ** (`dotnet test`, `vitest`, Playwright, mutation check) มีเพียงการตรวจแบบ compile/lint/build และเครื่องมือ build (ดูหัวข้อ 2)
- **ไม่ได้ตรวจในเบราว์เซอร์**
- **ความสามารถนี้ปิดอยู่ตามค่าเริ่มต้น** จนกว่าจะมี Calculation Policy แบบ `snapshot` ที่ publish แล้ว ซึ่งยังไม่มี API/UI สร้างได้ (ดูหัวข้อ 4)
- ยังไม่ผ่าน UAT และยังไม่พร้อม Production

เอกสารที่เกี่ยวข้อง: [ADR 0024](../adr/0024-estimate-unit-conversion-snapshot.md), [แผน G-08](../superpowers/plans/2026-10-15-g08-estimate-unit-conversion.md), [Official Estimate API](../03-contracts/official-estimate-api-contract.md), [Error Contract](../03-contracts/error-contract.md)

## 2. ผลที่รันจริง (compile/lint/build เท่านั้น)

| คำสั่ง | ผล | รันเมื่อ |
| --- | --- | --- |
| `dotnet build backend/TanErp.slnx --nologo -v q` | `Build succeeded.` `0 Warning(s)` `0 Error(s)` | รอบเอกสารนี้ (2026-10-15) |
| `cd frontend && npx tsc --noEmit` | exit 0 | รอบเอกสารนี้ |
| `cd frontend && npm run lint` | exit 0 | รอบเอกสารนี้ |
| `dotnet ef migrations has-pending-model-changes` | ไม่มีการเปลี่ยนแปลงโมเดลค้าง | ผลจาก Task 2 ของแผน (ไม่ได้รันซ้ำในรอบนี้) |
| migration `AddEstimateUnitConversionSnapshot` apply กับ local dev database ผ่าน `make db-migrate` | สำเร็จ | ผลจาก Task ก่อนหน้า (เฉพาะ DB ใน local) |
| OpenAPI snapshot regenerate ด้วย formatter ที่พิสูจน์แล้วว่าได้ไบต์เดียวกับไฟล์เดิม (`FORMATTER_OK`); diff เป็นการเพิ่มเท่านั้น | ผ่าน | ผลจาก Task 5 ของแผน |
| frontend `npm run check:api`, `npm run build`, key parity `th`/`en` | ผ่าน | ผลจาก Task 5–6 ของแผน (ไม่ได้รันซ้ำในรอบนี้) |

ผลข้างต้นยืนยันเพียงว่า **คอมไพล์ lint และ build ผ่าน** ไม่ได้ยืนยันว่า logic ถูกต้อง ไม่มีการพิสูจน์พฤติกรรมด้วย test

## 3. Test/fixture เดิมที่เสี่ยง (วิเคราะห์จากโค้ดอย่างเดียว — ไม่ได้รัน)

ผู้ implement วิเคราะห์ว่าไฟล์เหล่านี้ยังควรถูกต้อง โดยอ่านโค้ดเท่านั้น (read-only) ไม่ใช่ผลการรัน

| ไฟล์ | จุดเสี่ยง | ข้อสรุปจากการวิเคราะห์ |
| --- | --- | --- |
| `backend/tests/TanErp.UnitTests/Estimates/EstimateTests.cs` | สร้าง `CalculationPolicyVersion` แบบ positional; `Calculate`; assert `CalculationSnapshotJson` ด้วย `Contains` | parameter ใหม่ `unitConversionMode` เป็น optional ท้ายสุด (default `none`); แถวไม่มี snapshot ไม่เข้าเงื่อนไขใหม่ |
| `backend/tests/TanErp.UnitTests/Estimates/EstimateCostComponentSnapshotTests.cs` | `SetCatalogCostSnapshot`/`ClearCatalogCostSnapshot` | `SetCatalogCostSnapshot` ไม่เปลี่ยน; `Clear…` ล้างชุด conversion เพิ่ม (เดิมเป็น null) |
| `backend/tests/TanErp.IntegrationTests/Persistence/ItemSchemaConstraintTests.cs` | insert component ผ่าน EF | คอลัมน์ใหม่ nullable และ check ผ่านเมื่อเป็น null ทั้งชุด |
| `backend/tests/TanErp.IntegrationTests/Api/EstimateEndpointsTests.cs` | policy ctor; theory `unit-mismatch` | policy seeded เป็น `none` จึงโยน `ESTIMATE_UNIT_INVALID` ก่อนแตะ resolver |
| `backend/tests/TanErp.IntegrationTests/Scenarios/ItemCatalogEstimateFlowTests.cs` | policy ctor | ไม่เปลี่ยนสตริง/พฤติกรรม |
| `backend/tests/TanErp.IntegrationTests/Api/OpenApiContractTests.cs` | เทียบ swagger สดกับ `contracts/openapi/tan-erp.v1.json` | snapshot ถูก regenerate แล้ว แต่ **ยังไม่เคยรันเทียบ** |
| `backend/tests/TanErp.ArchitectureTests/LayerDependencyTests.cs` | ทิศทาง dependency | port อยู่ Application, implementation อยู่ Infrastructure |
| ชุด vitest ใน `frontend/src/features/estimates` (`estimate-workspace-mapper.test.ts`, `estimate-workspace-layout.test.tsx`, `estimate-workspace-drawer.test.tsx`, `estimate-review-queue.test.tsx`, `estimate-card.test.tsx`, `estimate-work-item-card.test.tsx`) | fixture พิมพ์ตาม response type | field ใหม่ใน type เป็น optional; dropdown คืน `null` เมื่อ policy ไม่เปิด |
| `frontend/e2e/estimate-item-catalog.spec.ts`, `frontend/e2e/official-estimate.spec.ts` | DOM ของตารางต้นทุน | **ไม่ได้ประเมิน** (E2E specs สองไฟล์นี้ไม่ได้ถูกวิเคราะห์ในรอบนี้) |

## 4. ข้อกำหนดก่อนเปิดใช้ (สำคัญ — switch-on gap)

1. ความสามารถนี้ **ปิดอยู่** จนกว่าจะมี Calculation Policy ที่ effective ของสาขาเป็น `unit_conversion_mode='snapshot'` ตราบนั้นหน่วยที่ไม่ใช่หน่วยฐานยังคืน `ESTIMATE_UNIT_INVALID` เหมือนเดิม
2. **ไม่มี API หรือ UI สร้าง/publish Calculation Policy** ops ต้องเพิ่มแถว v2 เอง (`version` = v1 + 1, overhead/rounding เท่า v1, `EffectiveFromUtc` = ปลายช่วงของ v1, ปิดช่วง v1 ด้วย `EffectiveTo` เดียวกัน, `unit_conversion_mode='snapshot'`) แล้ว publish ด้วย checker อิสระผ่านโค้ด `CalculationPolicyVersion.Publish` (ต้องคำนวณ `ContentHash`; ห้ามใส่ค่า hash ด้วยมือ)
3. ต้องไม่มี policy effective ซ้อนกันสองแถวต่อสาขา (จะได้ `ESTIMATE_POLICY_UNAVAILABLE`)
4. ต้องมี Item Master conversion ที่ active สำหรับ Item ที่จะลอง
5. apply migration `AddEstimateUnitConversionSnapshot` ในแต่ละ environment (apply กับ local dev database แล้วเท่านั้น)
6. Follow-up ที่เสนอ: Calculation Policy admin API (maker–checker) — **ต้องรอผู้ใช้อนุมัติขอบเขตก่อน**

## 5. คำสั่งที่ผู้ใช้ควรรันเอง

รันตามลำดับนี้ เริ่มจาก focused แล้วค่อย full gate (ห้ามอ้างว่าผ่านก่อนเห็นผลจริง):

```bash
dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --filter "FullyQualifiedName~Estimates"
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~EstimateEndpointsTests|FullyQualifiedName~ItemCatalogEstimateFlowTests|FullyQualifiedName~ItemSchemaConstraintTests" -m:1
dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter "FullyQualifiedName~OpenApiContractTests"
dotnet test backend/tests/TanErp.ArchitectureTests/TanErp.ArchitectureTests.csproj
cd frontend && npx vitest run src/features/estimates
```

**Final gate (ตาม AGENTS.md หมวด 8):**

```bash
# Backend
dotnet build
dotnet test

# Frontend
cd frontend && npm run build && npm run lint
```

E2E (`estimate-item-catalog`, `official-estimate`) ควรรันเองด้วย เพราะยังไม่ได้ประเมินผลกระทบ

## 6. สิ่งที่ควรตรวจด้วยมือหลังรัน test

- Estimate เดิมที่อนุมัติ/คำนวณแล้ว เปิดดูแล้วตัวเลขและ `CalculationPolicyVersion` ไม่เปลี่ยน
- เพิ่ม conversion รุ่นใหม่ใน Item Master แล้วเปิด Estimate ที่บันทึกไว้: factor ใน `conversionSnapshot` ยังเดิม และบันทึก draft ซ้ำแล้วยังเดิม
- เลือกหน่วยต่างมิติ: 422 `ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH`; ไม่มี conversion: 422 `ESTIMATE_UNIT_CONVERSION_NOT_FOUND`; policy ไม่ใช่ `snapshot`: 422 `ESTIMATE_UNIT_INVALID` เหมือนเดิม
- `New revision` แล้ว snapshot ถูกคัดลอก (ต้องมี policy `snapshot` effective จึงคำนวณใหม่ได้)
- Review Queue แสดงหลักฐานการแปลงหน่วยต่อแถวต้นทุน
- Quotation/PDF ของ Estimate ที่มีแถวแปลงหน่วยแสดงเฉพาะหน่วยและราคาระดับ Work Item (ไม่เห็นต้นทุนหรือ conversion)

## 7. การตัดสินใจที่ implement แล้ว / ที่ Business Owner ต้องตัดสินหรือรับทราบ

| # | เรื่อง | ค่าที่ implement | หมายเหตุ |
| --- | --- | --- | --- |
| 1 | แหล่งข้อมูล conversion | ตาราง conversion เดิม: Item-specific ก่อน แล้ว Shared; แปลงตรงไปหน่วยฐานเท่านั้น ไม่รองรับหลายขั้น | |
| 2 | ที่เก็บ snapshot | 5 คอลัมน์ nullable บน `estimate_cost_components` ระดับ cost component เท่านั้น | Quotation/PDF ไม่เปลี่ยน |
| 3 | การปัดเศษ | ต้นทุนแถว = `round(qty × factor × base cost, 2)` ปัดครั้งเดียว; ต้นทุนต่อหน่วยที่แสดง = `round(base cost × factor, 4)`; เลือกขั้นราคาด้วย qty × factor ที่ไม่ปัด | Cost Owner ยืนยันกติกาปัด |
| 4 | เวลาตรึง | ตรึงตอนบันทึก draft; `conversionId` เดิมคง snapshot ที่เก็บไว้; ไม่ตรง = 409 `ITEM_COST_VERSION_CONFLICT`; `unitCost` ที่ client ส่งสำหรับแถวที่แปลงหน่วยถูกละเว้น | |
| 5 | โหมด Calculation Policy | คอลัมน์ใหม่ `calculation_policy_versions.unit_conversion_mode` = `none` หรือ `snapshot`; แถวเดิมเป็น `none` และ hash ของ policy เดิมไม่เปลี่ยน | **ช่องว่างการเปิดใช้:** ไม่มี API/UI สร้าง/publish policy จึงต้องให้ ops เพิ่มแถว `snapshot` (หัวข้อ 4) จนกว่าจะทำ ฟีเจอร์ปิดอยู่ |
| 6 | Estimate เดิม | ไม่ backfill; ทุกแถวเดิมเป็นหน่วยฐาน และคง `CalculationPolicyVersion` เดิม | `InputHash` ของแถวที่ไม่มี snapshot ต้องเหมือนเดิมทุกไบต์: แก้ในคอมมิต `74c871b` ให้ serialize ฟิลด์ conversion เฉพาะแถวที่มี snapshot (ยังไม่พิสูจน์ด้วย test) |
| 7 | Revision | `New revision` คัดลอก snapshot แต่ต้องมี policy `snapshot` effective จึงคำนวณใหม่ได้ | |
| 8 | การแก้ conversion | API มีเฉพาะ GET/POST: "แก้ conversion" = เพิ่มรุ่นใหม่ที่มีผลช่วงใหม่; มิติของหน่วยแก้ภายหลังได้ resolver จึงตรวจมิติซ้ำ ณ วันตรึง | |
| 9 | Test seeder | ห้ามเพิ่ม calculation policy ที่ effective ตัวที่สองใน `TestOnlyDataSeeder` เพราะ policy resolution จะคืน null และทำให้ทุก test ล้ม | |
| 10 | Error code ใหม่ | `ESTIMATE_UNIT_CONVERSION_NOT_FOUND`, `ESTIMATE_UNIT_CONVERSION_DIMENSION_MISMATCH` (422) | ดู [Error Contract](../03-contracts/error-contract.md) |

## 8. ข้อจำกัดที่ยอมรับ

- ไม่มี test ใหม่และไม่ได้รัน test; ไม่ได้ตรวจในเบราว์เซอร์
- ไม่รองรับ conversion หลายขั้น; Item Master ยังไม่มี API ปิด/แก้ conversion
- พรีวิวบรรทัดฝั่ง UI ใช้ `baseUnitCost × factor` (ปัด 4 ตำแหน่ง) ผลจริงมาจาก server
- `ITEM_COST_VERSION_CONFLICT` ใช้ข้อความ UI เดิมสำหรับกรณี conversion เปลี่ยน
- G-09 ต้องสร้างแถวจาก template ผ่าน `IUnitConversionResolver` และกติกาตรึง/คงค่าเดียวกัน

## 9. ยังเปิดอยู่

- Calculation Policy admin API (รอผู้ใช้อนุมัติขอบเขต)
- การรัน test ทั้งหมด รวม E2E ที่ยังไม่ได้ประเมิน
- การตรวจในเบราว์เซอร์ และ UAT
