# คู่มือการใช้งานข้อกำหนดกลางและคอมโพเนนต์สำหรับ ERP Features (tan-erp Shared Foundation Guide)

คู่มือฉบับนี้อธิบายการใช้งานชุดสไตล์ คลาสกลาง คอมโพเนนต์ และ Hooks ที่สร้างขึ้นเพื่อเป็นมาตรฐานร่วมกันสำหรับทุกโมดูลในระบบ **Project ERP (tan-erp)** อิงตามแนวคิดการออกแบบ **Atelier Architectural Navy Sharp**

---

## 🎨 1. Centralized CSS Class System (`.erp-*`)

ทุกฟีเจอร์ในระบบสามารถเรียกใช้คลาสกลางที่รวมศูนย์อยู่ใน `src/styles/erp-theme.css` เพื่อให้สอดคล้องกับธีมและง่ายต่อการปรับเปลี่ยนโทนสี/เลย์เอาต์ของระบบในจุดเดียว:

```css
/* ตัวอย่างคลาสหลัก */
.erp-btn                /* ปุ่มพื้นฐาน 44px min-touch, border-radius: 0 */
.erp-btn-primary        /* Solid Navy #0B3056 */
.erp-btn-secondary      /* Subdued Surface */
.erp-btn-danger         /* Red Action */
.erp-btn-outline        /* White Background with Gray Border */

.erp-input              /* อินพุตขอบคม 44px */
.erp-select             /* ดรอปดาวน์ */
.erp-textarea           /* กล่องข้อความ */

.erp-table-wrapper      /* กรอบตารางพร้อม Horizontal Scroll */
.erp-table              /* ตารางแบบ ERP High-density */
.erp-th                 /* หัวตารางพื้นหลังเทาอ่อน */
.erp-td                 /* ช่องข้อมูลในแถว */
.erp-tr-selected        /* แถวที่ถูก Checkbox เลือก (ไฮไลต์สีฟ้าอ่อน) */

.erp-badge              /* ป้ายสถานะสี่เหลี่ยมมุมคม */
.erp-action-bar         /* แถบลอยด้านล่าง Sticky พร้อมตัวเตือน Unsaved */
```

---

## 🧩 2. การใช้งาน UI Components (`@/components/ui`)

### 2.1 Buttons (`Button`)
```tsx
import { Button } from "@/components/ui";
import { IconPlus } from "@/components/common";

// Primary action
<Button variant="primary" icon={<IconPlus />} onClick={handleCreate}>
  สร้างรายการใหม่
</Button>

// Loading state (ล็อกปุ่มอัตโนมัติ)
<Button variant="primary" isLoading={isSubmitting}>
  บันทึกข้อมูล
</Button>

// Render as Next.js Link
<Button variant="outline" href="/th/items/create">
  เพิ่มสินค้า
</Button>
```

### 2.2 Form Controls (`Input`, `Textarea`, `Select`, `Checkbox`, `Switch`)
```tsx
import { Input, Textarea, Select, Checkbox, Switch } from "@/components/ui";

// Text Input พร้อม Label และ Error Message
<Input
  label="รหัสสินค้า"
  required
  error={errors.code?.message}
  {...register("code")}
/>

// Password Input พร้อมปุ่มเปิด/ปิดตาในตัว
<Input
  type="password"
  label="รหัสผ่าน"
  {...register("password")}
/>

// Textarea พร้อมตัวนับจำนวนอักษร
<Textarea
  label="รายละเอียดโครงการ"
  maxLength={500}
  showCount
  {...register("description")}
/>

// Select Dropdown
<Select
  label="สาขา"
  options={[
    { label: "สำนักงานใหญ่", value: "HQ" },
    { label: "สาขาเชียงใหม่", value: "CNX" },
  ]}
  placeholder="-- เลือกสาขา --"
  {...register("branchId")}
/>

// Checkbox (รองรับ indeterminate สำหรับหัวตาราง)
<Checkbox
  label="เลือกทั้งหมด"
  checked={isAllSelected}
  indeterminate={isPartiallySelected}
  onChange={toggleSelectAll}
/>
```

### 2.3 Status Badges (`StatusBadge`)
```tsx
import { StatusBadge } from "@/components/ui";

// แมปสถานะอัตโนมัติ (เช่น active, approved, draft, pending, voided)
<StatusBadge status="approved" label="อนุมัติแล้ว" />
<StatusBadge status="pending" label="รอดำเนินการ" />
<StatusBadge status="voided" label="ยกเลิกแล้ว" />
```

### 2.4 Sticky Form Action Bar (`FormActionBar`)
```tsx
import { FormActionBar } from "@/components/common";

<FormActionBar
  isDirty={isDirty}
  isLoading={isSubmitting}
  isEditMode={isEdit}
  isReadOnly={isApproved} // โหมด Read-only สำหรับเอกสารที่ผ่านการอนุมัติแล้ว
  cancelHref="/th/items"
/>
```

---

## 🪝 3. การใช้งาน Shared Hooks (`@/hooks`)

### 3.1 Safety Confirmation Modal (`useConfirm`)
ใช้เมื่อต้องลบหรือยกเลิกเอกสารสำคัญ ป้องกันการลบโดยไม่ตั้งใจ:
```tsx
import { useConfirm } from "@/hooks";

function ItemActions({ itemId }: { itemId: string }) {
  const { confirm, ConfirmDialog } = useConfirm();

  const handleDelete = async () => {
    const accepted = await confirm({
      title: "ยืนยันการลบสินค้า",
      message: "คุณแน่ใจหรือไม่ว่าต้องการลบสินค้ารายการนี้? การดำเนินการนี้ไม่สามารถย้อนกลับได้",
      variant: "danger",
      confirmText: "ลบรายการ",
      onConfirm: async () => {
        await deleteItemApi(itemId);
      },
    });

    if (accepted) {
      // ดำเนินการหลังลบสำเร็จ เช่น Invalidate Query
    }
  };

  return (
    <>
      <Button variant="danger" size="sm" onClick={handleDelete}>
        ลบ
      </Button>
      <ConfirmDialog />
    </>
  );
}
```

### 3.2 URL-Synchronized Table State (`useDataTableState`)
จัดการ Search (พร้อม Debounce), Pagination, และ Sorting ซิงค์กับ URL Search Parameters อัตโนมัติ:
```tsx
import { useDataTableState } from "@/hooks";
import { DataTable } from "@/components/ui";

export function ItemList() {
  const { params, actions, draftSearch, isDebouncing } = useDataTableState({
    defaultPageSize: 25,
    defaultSort: { key: "createdAt", order: "desc" },
  });

  const { data, isLoading } = useQuery({
    queryKey: ["items", params],
    queryFn: () => fetchItems(params),
  });

  return (
    <div>
      <Input
        placeholder="ค้นหาสินค้า..."
        value={draftSearch}
        onChange={(e) => actions.setSearch(e.target.value)}
      />

      <DataTable
        columns={columns}
        data={data?.items || []}
        pagination={{
          page: params.page,
          limit: params.pageSize,
          totalPages: data?.totalPages || 0,
          totalItems: data?.totalItems,
        }}
        sorting={params.sort}
        isLoading={isLoading}
        onPageChange={actions.setPage}
        onLimitChange={actions.setPageSize}
        onSort={actions.setSort}
      />
    </div>
  );
}
```

---

## 🛠️ 4. Shared Utilities (`@/lib/utils`)

```tsx
import { formatCurrency, formatNumber, formatDate, formatDateTime, formatFileSize } from "@/lib/utils";

formatCurrency(125000);             // "฿125,000.00"
formatNumber(1500.25, 2, 4);        // "1,500.25"
formatDate("2026-09-06");           // "6 ก.ย. 2569" (th) หรือ "Sep 6, 2026" (en)
formatDateTime("2026-09-06T14:30"); // "6 ก.ย. 2569, 14:30"
formatFileSize(2621440);            // "2.5 MB"
```
