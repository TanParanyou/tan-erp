# คู่มือการใช้งานคอมโพเนนต์กลางของ Foundation Slice (tan-erp Shared Foundation Guide)

คู่มือฉบับนี้อธิบายชุดคอมโพเนนต์ Primitives และสไตล์กลางสำหรับ Foundation Slice ในระบบ **Project ERP (tan-erp)** อิงตามแนวคิดการออกแบบ **Atelier Architectural Navy Sharp**

> ⚠️ **หมายเหตุขอบเขต (Scope Notice):**
> คอมโพเนนต์และยูทิลิตีขั้นสูงสำหรับโมดูลธุรกิจในอนาคต (เช่น `DataTable`, `Drawer`, `ConfirmModal`, `useConfirm`, `useDataTableState`, `deferred-upload`, `formatters`, `line-items` เป็นต้น) ถูกถอนออกจาก Foundation Runtime ตามแผนงาน remediation ใน [`2026-09-06-foundation-login-current-user-remediation.md`](../superpowers/plans/2026-09-06-foundation-login-current-user-remediation.md) เพื่อรักษา Minimal Blast Radius และความถูกต้องของ Vertical Slice

---

## 🎨 1. Centralized CSS Class System (`.erp-*`)

ฟีเจอร์ในระดับ Foundation เรียกใช้คลาสกลางจาก `src/styles/erp-theme.css`:

```css
/* คลาสหลักของ Foundation */
.erp-btn                /* ปุ่มพื้นฐาน 44px min-touch, border-radius: 0 */
.erp-btn-primary        /* Solid Navy #0B3056 */
.erp-btn-secondary      /* Subdued Surface */
.erp-btn-danger         /* Red Action */
.erp-btn-outline        /* White Background with Gray Border */

.erp-input              /* อินพุตขอบคม 44px */
.erp-form-group         /* กรอบฟอร์มและป้ายกำกับ */
.erp-label              /* ป้ายกำกับช่องกรอกข้อมูล */
.erp-error-text         /* ข้อความแจ้งเตือนข้อผิดพลาด */
```

---

## 🧩 2. Foundation UI Primitives (`@/components/ui`)

### 2.1 Buttons (`Button`)
```tsx
import { Button } from "@/components/ui/Button";
import { useTranslations } from "next-intl";

const t = useTranslations("auth");

// ปุ่มหลักพร้อมสถานะ Loading (ปิดการคลิกซ้ำอัตโนมัติ)
<Button variant="primary" isLoading={isSubmitting} type="submit">
  {t("submitButton")}
</Button>

// ปุ่มขนาดสัมผัส 44px
<Button variant="outline" onClick={handleAction}>
  {t("logout")}
</Button>
```

### 2.2 Inputs (`Input`)
```tsx
import { Input } from "@/components/ui/Input";
import { useTranslations } from "next-intl";

const t = useTranslations("auth");

// อินพุตข้อความทั่วไป
<Input
  id="email"
  label={t("emailLabel")}
  required
  error={fieldErrors.email}
  value={email}
  onChange={(e) => setEmail(e.target.value)}
/>

// อินพุตรหัสผ่านพร้อมปุ่มสลับการแสดงผล (ขนาดสัมผัส 44x44px และรองรับแป้นพิมพ์)
<Input
  id="password"
  type="password"
  label={t("passwordLabel")}
  required
  value={password}
  onChange={(e) => setPassword(e.target.value)}
/>
```

### 2.3 Loading Spinners (`MonoSpinner`)
```tsx
import { MonoSpinner } from "@/components/ui/MonoSpinner";

<MonoSpinner size="sm" />
```

---

## 🖼️ 3. Icons (`@/components/common/Icons`)

ไอคอนแบบ Pure SVG ตามแนวทาง Atelier Architectural Navy Sharp:
```tsx
import { IconEye, IconEyeOff, IconClose } from "@/components/common/Icons";
```

---

## 🛠️ 4. Shared Utilities (`@/lib/utils`)

```tsx
import { cn } from "@/lib/utils/cn";

// รวม ClassName อย่างปลอดภัย
const className = cn("erp-input", hasError && "erp-input-error");
```
