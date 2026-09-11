# คู่มือการใช้งานคอมโพเนนต์กลางและ Hooks ของ tan-erp (Shared Components & Hooks Guide)

คู่มือฉบับนี้อธิบายชุดคอมโพเนนต์กลาง (Reusable Components), Custom Hooks, และ Utilities ของระบบ **Project ERP (tan-erp)** โดยยึดตามมาตรฐานการออกแบบ **Atelier Architectural Navy Sharp** (`border-radius: 0px !important`, Solid Navy `#0B3056`, Pure SVG Stroke Icons) และผสานการทำงานร่วมกับ Tailwind CSS อย่างสมบูรณ์

---

## 🎨 1. Centralized Styling & Design Tokens

ระบบใช้ **Tailwind CSS เป็น Styling API หลัก** ร่วมกับโทเค็นใน `tailwind.config.ts`; `src/styles/erp-theme.css` ทำหน้าที่เก็บ CSS custom properties และ global/shared exceptions เท่านั้น โค้ดคอมโพเนนต์ต้องเลือก semantic utilities เช่น `bg-erp-navy`, `text-erp-text-main`, `border-erp-border` ก่อน arbitrary values หรือสี Hex โดยตรง และใช้ `cn` จาก `@/lib/utils` เมื่อต้องรวม class แบบมีเงื่อนไข:

```css
/* สีระบบหลัก (ERP Semantic Tokens) */
--erp-navy: #0B3056;             /* bg-erp-navy, text-erp-navy */
--erp-canvas: #F4F6F9;           /* bg-erp-canvas */
--erp-surface: #FFFFFF;          /* bg-erp-surface */
--erp-surface-muted: #F8FAFC;    /* bg-erp-surface-muted */
--erp-border: #D1DEEC;           /* border-erp-border */
--erp-text-main: #0A192F;        /* text-erp-text-main */
--erp-text-muted: #536B88;       /* text-erp-text-muted */

/* สถานะธรรมาภิบาล (Maker-Checker Semantics) */
--erp-success / --erp-success-bg / --erp-success-border  /* อนุมัติแล้ว (Approved) */
--erp-warning / --erp-warning-bg / --erp-warning-border  /* รอผู้ตรวจอนุมัติ (Pending Checker) */
--erp-danger / --erp-danger-bg / --erp-danger-border     /* ส่งกลับแก้ไข / ต่ำกว่าเกณฑ์ (Rejected / Below GP) */
--erp-info / --erp-info-bg       /* ข้อมูลทั่วไป (Info) */
```

*หมายเหตุ: ทุกคอมโพเนนต์ถูกกำหนด `border-radius: 0px` (Zero Radius) โดยเด็ดขาด ห้ามใช้ขอบมนโค้ง*

---

## 🧩 2. Core UI Primitives (`@/components/ui`)

| คอมโพเนนต์ | หน้าที่และการใช้งาน |
| :--- | :--- |
| **`Button`** | ปุ่มมาตรฐานระบบ รองรับ Variant (`primary`, `secondary`, `danger`, `outline`, `ghost`), ขนาด (`sm`, `md`, `lg`, `icon`), และ `isLoading` (ล็อกปุ่มอัตโนมัติ) |
| **`Input`** | ช่องกรอกข้อความ/รหัสผ่าน มีปุ่มสลับการมองเห็นรหัสผ่าน, รองรับ `leftIcon`, `rightIcon`, `error`, `helperText` |
| **`Select`** | Dropdown เมนูขอบฉาก 0px รองรับตัวเลือก Placeholder, Option groups, ข้อความ Error และการเชื่อมต่อผ่าน Controller |
| **`Textarea`** | กล่องข้อความหลายบรรทัด รองรับ `rows`, `resize-y` ขอบมุมฉาก 0px |
| **`Checkbox`** | ช่องทำเครื่องหมายสี่เหลี่ยมมุมฉาก 0px พร้อมป้ายกำกับและคำอธิบาย |
| **`Switch`** | สวิตช์เปิด/ปิดสไตล์สถาปัตยกรรม (Architectural Sharp Rectangular Toggle) ปราศจากความโค้งมน |
| **`Modal` / `ConfirmModal` / `FormModal`** | หน้าต่างป๊อปอัปขอบคมฉาก แสดงผลผ่าน React Portal พร้อม Focus trap และการกด Escape ปิด |
| **`ConfirmationModal`** | กล่องยืนยันการกระทำความเสี่ยงสูง (เช่น การลบ, การยกเลิกเอกสาร) บังคับสถานะ `isLoading` ล็อกปุ่มเสมอ |
| **`Drawer`** | แผงเลื่อนด้านข้างขวา (Slide-over) ขอบคมฉาก พร้อม Backdrop blur และการล็อก Scroll |
| **`StatusBadge`** | ป้ายแสดงสถานะตาม Maker-Checker Semantics (เขียว/เหลือง/แดง/น้ำเงิน/เทา) ขอบมุมฉาก |
| **`Tooltip`** | กล่องคำอธิบายลอยตัว สีกรมท่ามุมฉาก แสดงเมื่อวางเมาส์หรือ Focus |
| **`Tabs`** | แท็บสลับหน้าย่อยขอบล่างสีกรมท่าหนา 3px สำหรับฟอร์มและเอกสารยาว |
| **`EmptyState`** | หน้าจอว่างเปล่าแบบระเบียบวินัยช่าง พร้อมไอคอน Pure SVG และปุ่มกระตุ้น Action |
| **`BulkActionToolbar`** | แถบเครื่องมือลอยตัวสำหรับจัดการแถวข้อมูลที่ถูกเลือกพร้อมกันในตาราง |
| **`DataTable`** | ตารางข้อมูล ERP ความหนาแน่นสูง รองรับ Pagination, Sorting, Row Selection (Checkbox), และ Sticky Column |
| **`MapPreview`** | กล่องแสดงตัวอย่างพิกัดแผนที่ (ละติจูด, ลองจิจูด) พร้อมลิงก์เปิดแผนที่ภายนอก (Google Maps / Apple Maps) |
| **`Alert`** | แถบแจ้งเตือนสถานะ/ข้อผิดพลาดระดับอินไลน์หรือ Banner ขอบมุมฉาก 0px รองรับ variant (`danger`, `warning`, `success`, `info`), Pure SVG icon, และปุ่ม Dismiss (`onClose`) |
| **`PageLoading` / `MonoSpinner`** | Minimal Mono Loading แสดงสถานะกำลังโหลดแบบเรียบง่าย ไม่ใช้ Skeleton หลอกตา |
| **`Toast` / `ToastContainer`** | กล่องแจ้งเตือนมุมฉาก 0px พร้อมแถบสีสถานะหนา 5px ที่ขอบซ้าย |

---

## 📝 3. ERP Form Workflows (`@/components/forms`)

### 3.1 `FormActionBar`
แถบเครื่องมือบันทึกฟอร์มแบบติดตรึงด้านล่างเต็มจอ (`fixed bottom-0 left-0 right-0 z-40 w-full`)
- **Full-Width Solid Docking:** ใช้สีพื้นทึบแสง 100% (`bg-erp-surface`) พร้อมเส้นขอบบน (`border-t border-erp-border`) กว้างพาดเต็มหน้าจอซ้าย-ขวาอย่างมั่นคง ปราศจากความโปร่งแสง
- **Harmonized Inner Padding:** กำหนด padding ด้านในเป็น `px-4 py-3 sm:px-6 sm:py-4` ทำให้ป้ายสถานะทางซ้ายและปุ่มคำสั่งทางขวาตรงแนว (align) กับขอบเนื้อหาฟอร์มด้านบนอย่างเป็นระเบียบ
- **Unsaved Changes Pulse:** แสดงตัวเตือน `isDirty` เมื่อมีข้อมูลค้างยังไม่ได้บันทึก
- **Double Submit Protection:** ปุ่มบันทึกมีสถานะ `isLoading` ปิดการกดซ้ำขณะยิง API
- **Extra Actions:** ช่องเสียบปุ่มเพิ่มเติม เช่น ปุ่มลบ (Delete), ยกเลิกเอกสาร (Void)

### 3.2 `MultiLangInput`
อินพุตข้อความสองภาษา (`th` เป็นค่าเริ่มต้น และ `en`) พร้อมแท็บสลับภาษา และปุ่ม Copy Source สำหรับคัดลอกข้อความภาษาไทยไปยังภาษาอังกฤษได้อย่างรวดเร็ว

### 3.3 `ImageUpload` (Deferred Upload Flow)
คอมโพเนนต์อัปโหลดรูปภาพและเอกสารแนบตามมาตรฐาน ERP:
- **Deferred Upload:** เลือกไฟล์แล้วเก็บ `File` ไว้ใน Local/Form State และแสดง Local Preview ทันที โดยยังไม่อัปโหลดจริงเพื่อป้องกันไฟล์ขยะ
- **Memory Safe:** มีการจัดการ `URL.revokeObjectURL()` อัตโนมัติเมื่อเปลี่ยนรูปหรือ unmount คอมโพเนนต์
- **Image Preview:** มี Modal แสดงตัวอย่างภาพขนาดใหญ่

### 3.4 `FormContainer` (Unified Layout Wrapper)
คอมโพเนนต์ห่อหุ้มฟอร์มมาตรฐานของระบบ เพื่อความเป็นเอกภาพของโครงสร้างหน้าจอ Layout:
- จัดสรรพื้นที่แบ่งเป็นส่วนชัดเจน: `header` (PageHeader), `errorBanner`, `topAlert` (คำเตือนเพิ่มเติม เช่น รายการซ้ำ), `children` (ฟิลด์ข้อมูลในฟอร์ม), และ `actionBar` (FormActionBar ติดตรึงด้านล่าง)
- รองรับการทำงานแบบ Native Form (`asForm={true}` หรือส่ง `onSubmit`, `onChange`) ทำให้ปุ่มบันทึกใน `FormActionBar` ทำงานสอดคล้องกับ Form Validation และการ Submit ได้ทันที
- ใช้ `min-h-[calc(100vh-7rem)] flex flex-col justify-between` และ `flex-1 mb-8 pb-24` ป้องกันไม่ให้ Fixed Action Bar บังฟิลด์อินพุตแถวล่างสุดตอนเลื่อนจอ
- กำหนดความกว้างมาตรฐานด้วย `maxWidth` (`full`: w-full [ค่าเริ่มต้น เพื่อความ Fluid สอดคล้องกับหน้า List/Detail ไม่เกิด Layout Shift], `sm`: max-w-xl, `md`: max-w-3xl, `lg`: max-w-5xl, `xl`: max-w-7xl)

### 3.5 `AddressAreaField` & `AddressAutocomplete` (Centralized Geographic Area Selector)
คอมโพเนนต์เลือกพื้นที่และที่อยู่สำหรับฟอร์ม ERP ทั้งหมด (เช่น สถานที่ตั้งหน้างาน, ที่อยู่ลูกค้า, สาขา):
- **Seamless Dual Mode:**
  - **ขณะยังไม่เลือกพื้นที่:** แสดงช่องค้นหา `AddressAutocomplete` สไตล์ WAI-ARIA Combobox ดึงข้อมูลจากฐานข้อมูลภูมิศาสตร์ไทย (77 จังหวัด, 929 อำเภอ, 7,451 ตำบล) ผ่าน In-memory Cache ค้นหาได้ทั้งรหัสไปรษณีย์ 5 หลัก และชื่อตำบล/อำเภอ
  - **เมื่อเลือกพื้นที่แล้ว:** แปลงสภาพเป็น **Selected Location Summary Card** คม เหลี่ยม ไร้ขอบมน ความสูงมาตรฐาน 44px (`min-h-[44px]`) แสดง `ตำบล » อำเภอ » จังหวัด รหัสไปรษณีย์` พร้อมปุ่ม "เปลี่ยนที่อยู่" เพื่อเคลียร์และค้นหาใหม่
- **Zero Redundant Inputs:** ยกเลิกการให้ผู้ใช้กรอกข้อความแยก ตำบล อำเภอ จังหวัด รหัสไปรษณีย์ แบบ Free-text ซึ่งเสี่ยงต่อการสะกดผิดและทำให้เกิดช่องอินพุตมากเกินความจำเป็น ผู้ใช้กรอกเพียงเลขที่/ซอย/ถนน และเลือกตำบลเพียงครั้งเดียว ระบบจะเติมข้อมูลทั้งหมดให้สมบูรณ์

### 3.6 `QuickNoteChips` (Rapid Template Injection)
ชิปสำหรับเลือกข้อความเทมเพลตที่ใช้บ่อย เพื่อนำข้อความไปต่อท้ายใน `Textarea` หรือช่องบันทึกโดยอัตโนมัติ:
- สไตล์ขอบคม 0px ทึบและชัดเจน พร้อมไอคอน `+`
- ป้องกันข้อความซ้ำซ้อน (ฉลาดพอที่จะไม่เติมข้อความซ้ำหากมีอยู่ในช่องแล้ว)
- รองรับคีย์เทมเพลตมาตรฐาน เช่น เงื่อนไขการเข้าหน้างาน (`siteAccess` — แลกบัตร, ประตูปิด 18:00, ติดต่อ รปภ.)

### 3.7 `FormSection` (Unified Architectural Form Card)
การ์ดแบ่งส่วนฟอร์มมาตรฐานของระบบ ERP:
- โครงสร้าง `.erp-card` ขอบคมฉาก 0px (`border-radius: 0px`)
- ส่วนหัวสไตล์ Architectural Solid Navy (`text-erp-navy`) ตัวพิมพ์ใหญ่ tracking-wide uppercase พร้อมเส้นขอบแบ่งหมวดหมู่บาง
- รองรับ `title`, `description` (คำอธิบายย่อยใต้หัวข้อ), และ `headerAction` (สำหรับใส่ Badge หรือปุ่มคำสั่งขนาดกะทัดรัดประจำการ์ด)
- ยุติปัญหาการเขียน custom card markup ซ้ำซ้อนข้าม Feature Form ต่างๆ

---

## 🌐 4. Shared Utilities & Global Widgets (`@/components/common` & `@/components/layout`)

- **`PageHeader` (`@/components/layout`):** แถบหัวเรื่องสถาปัตย์ 2-Tier แสดงชื่อเอกสาร, รหัส, ป้ายสถานะ และปุ่มคำสั่ง
- **`Icons` (`@/components/common`):** Pure SVG Stroke Icons (`strokeWidth={2}` หรือ `1.6`, `square/miter`) เช่น `IconCopy`, `IconUpload`, `IconSearch`, `IconEdit`, `IconTrash`, `IconCalendar`, `IconClock`
- **`CurrencyDisplay` (`@/components/common`):** จัดรูปแบบสกุลเงิน (ค่าเริ่มต้น THB) ด้วยฟอนต์โมโน (`font-mono` / tabular numbers)
- **`CopyButton` (`@/components/common`):** ปุ่มคลิกคัดลอกรหัสเอกสารหรือเลขประจำตัวผู้เสียภาษี พร้อมสลับไอคอนและแสดง Toast แจ้งเตือน
- **`PageBreadcrumbs` (`@/components/common`):** แถบนำทางเส้นทางเมนูสถาปัตย์
- **`DetailNavigation` (`@/components/common`):** ปุ่มเลื่อนดูรายการก่อนหน้า/ถัดไป
- **`InactivityTimeoutDialog` (`@/components/common`):** กล่องเตือนหมดเวลาการเชื่อมต่อเมื่อไม่มีการใช้งาน

---

## 🪝 5. Central Custom Hooks (`@/hooks`)

| Hook | วัตถุประสงค์และการใช้งาน |
| :--- | :--- |
| **`useToast`** | แจ้งเตือนข้อความระบบ: `toast.success()`, `toast.error()`, `toast.warning()`, `toast.info()`, `toast.loading()` |
| **`useConfirm`** | เรียกกล่องยืนยันแบบ Imperative: `const confirmed = await confirm({ title, message, variant })` |
| **`useDisclosure`** | จัดการสถานะเปิด/ปิด Modal, Drawer, Dropdown: `{ isOpen, onOpen, onClose, onToggle }` |
| **`useScrollLock`** | ล็อกการเลื่อนหน้าจอของ Body ขณะเปิด Modal หรือ Drawer |
| **`useDataTable`** | จัดการ Pagination, Sorting, Search query สำหรับตาราง |
| **`useRowSelection`** | จัดการ Checkbox Selection ในตาราง: `selectedIds`, `toggleRow`, `selectAll`, `clearSelection` |
| **`useDebounce`** | หน่วงเวลาอินพุตค้นหาเพื่อลดภาระการยิง API ถี่เกินไป |
| **`useClipboard`** | คัดลอกข้อความสู่คลิปบอร์ด พร้อมเรียก Toast แจ้งเตือนอัตโนมัติ |
| **`useInactivityTimeout`** | ตรวจจับระยะเวลาที่ผู้ใช้ไม่ได้ขยับเมาส์หรือคีย์บอร์ดเพื่อความปลอดภัย |
| **`useKeyboardShortcut`** | ดักจับคีย์ลัดแป้นพิมพ์ เช่น `Ctrl+S` / `Cmd+S` สำหรับบันทึก และ `Escape` สำหรับปิด |
| **`useLocalStorage`** | Type-safe LocalStorage Hook สำหรับบันทึกความชอบของผู้ใช้ (เช่น Page size ของตาราง) |
| **`useMediaQuery`** | ตรวจสอบ Responsive Breakpoints (`isMobile`, `isTablet`, `isDesktop`) |
| **`useOnlineStatus`** | ตรวจสอบสถานะการเชื่อมต่ออินเทอร์เน็ตของผู้ใช้ |
| **`useCurrentLocation`** | ดึงพิกัดภูมิศาสตร์ (GPS ละติจูด, ลองจิจูด) จาก Browser Geolocation API พร้อมการตรวจจับข้อผิดพลาดตาม W3C (Permission Denied, Position Unavailable, Timeout) และแจ้งเตือนผ่าน Toast |
| **`useImagePreview`** | จัดการสร้างและคืนหน่วยความจำ Object URL สำหรับภาพพรีวิว |

---

## 📚 6. Libraries & Helpers (`@/lib`)

- **`@/lib/export/export-csv.ts`**: ฟังก์ชันส่งออกตารางเป็นไฟล์ CSV พร้อมใส่ UTF-8 BOM เพื่อให้อ่านภาษาไทยใน Microsoft Excel ได้ถูกต้อง ไม่เป็นภาษาต่างดาว
- **`@/lib/formatters/formatters.ts`**: ฟังก์ชันจัดรูปแบบตัวเลข, สกุลเงิน (THB), ขนาดไฟล์ (Bytes, KB, MB) และวันที่ตาม Locale
- **`@/lib/media/image-optimization.ts`**: ปรับขนาดและบีบอัดภาพเป็น WebP ฝั่งเบราว์เซอร์ก่อนส่งขึ้น Storage

---

## 🧪 7. การทดสอบและการรับรองคุณภาพ (Quality Gates)

ทุกคอมโพเนนต์และ Hook ได้รับการเขียน Unit Test ด้วย **Vitest** และ **Testing Library** อย่างครบถ้วน:
```bash
# ตรวจสอบ Type ความเข้มงวด (ห้ามมี any)
npm run typecheck

# ตรวจสอบ Linting
npm run lint

# รันชุดทดสอบทั้งหมด
npm run test

# ทดสอบ Production Build
npm run build
```
