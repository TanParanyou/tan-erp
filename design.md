# คู่มือแนวทางการออกแบบ UI/UX ระบบ ERP: Atelier Architectural Navy Sharp — tan-erp

เอกสารนี้กำหนดมาตรฐานการออกแบบ UI/UX ขั้นสมบูรณ์ของ **Project ERP (tan-erp)** สำหรับธุรกิจออกแบบ สถาปัตยกรรมภายใน และโรงงานผลิตงานไม้บิวต์อิน (เช่น We Cover Design & Decor) โดยยึดหลัก **“Non-AI Slop Craftsmanship”**, **“สีกรมท่าเข้มสถาปัตย์ (Solid Architectural Navy `#0B3056`)”** และ **“โครงสร้างขอบมุมฉาก 0px (Zero Radius)”**

---

## 1. แนวคิดปฏิเสธ AI Slop สู่ความประณีตระดับ Atelier (Anti-AI Slop Philosophy)

ในการพัฒนาระบบ ERP สำหรับธุรกิจเฉพาะทางอย่างงานตกแต่งภายในและบิวต์อินระดับไฮเอนด์ เราปฏิเสธแพทเทิร์นผิวเผินแบบ AI Slop ที่มักพบในเทมเพลตสำเร็จรูป:

| ลักษณะของ AI Slop (สิ่งที่เราปฏิเสธ) | มาตรฐาน Atelier ERP (สิ่งที่เรานำมาใช้จริง) |
| :--- | :--- |
| **การ์ดสถิติลอยน้ำไร้ประโยชน์** (เช่น Total Revenue +12% ลอยเดี่ยวๆ ไม่มีบริบท) | **Operational Financial HUD**: แสดงต้นทุนสเปกจริง (BOQ Cost), กำไรขั้นต้น (GP%), และยอดเสนอราคารวม พร้อมการผูกโยงกับแต่ละ Work Item |
| **ขอบมนโค้งมนหนาเตอะ (`rounded-2xl`, `rounded-full`)** ทำให้ดูเป็นแอปของเล่น | **ขอบมุมฉาก 0px คมกริบ (`border-radius: 0px !important`)** สะท้อนระเบียบวินัยช่างไม้และการเขียนแบบสถาปัตยกรรม |
| **การใช้ Emoji แทนไอคอนระบบ** (เช่น ☀️, 🌙, 📐, 🦙, 💰) | **Pure SVG Stroke Icons**: ลายเส้นคมชัด 1.6px, ปลายเส้นสี่เหลี่ยม (`square/miter`), ไร้สีฉูดฉาด |
| **ตารางหน้าเดียวแบบแบนราบ (Flat CRUD Table)** แก้ไขอะไรต้องเปิดป๊อปอัปบังจอ | **Split Master-Detail Workspace**: ซีกซ้ายเป็น Spec Sheet ละเอียด ซีกขวาเป็น Live BOM Cutlist และ Audit Trail แบบเรียลไทม์ |
| **การไล่สี Gradient สีม่วง/ฟ้าสว่าง และแก้วมัว (Glassmorphism)** หลอกตาและเปลืองแรงเครื่อง | **Solid Architectural Navy & 1px Hairline Rules**: โทนสีกรมท่าลึก ขาว เทาไอซ์ และเส้นตาราง 1px อ่านตัวเลขได้ทั้งวันไม่ปวดตา |
| **ข้อความสมมติไร้แก่นสาร** (เช่น Item A, Lorem Ipsum) | **ข้อมูลและคำศัพท์เฉพาะทางจริง**: ไม้อัด HMR 18mm กันชื้น, ลามิเนต Formica Oak, รางลิ้นชัก Blum Tandembox, ผ้า Blackout เบลเยียม, Maker-Checker Flow |

---

## 2. สถาปัตยกรรมหน้าจอแบบแยกส่วน (Split Master-Detail Architecture)

ระบบถูกออกแบบให้ทำงานเหมือน **โต๊ะดราฟต์แบบของสถาปนิกและผู้คุมราคา (Cost Estimator)**:

### 2.1 ส่วนบน: 2-Tier Architectural Title & Financial HUD (แถบหัวเรื่องสถาปัตย์และมาตรวัดการเงิน)
เพื่อป้องกันความอึดอัดและข้อความหักบรรทัดผิดธรรมชาติ (Zero-Cramping Architectural Header):
- **Tier 1 (Project Identity & Title Block)**: แสดงชื่อโครงการ (`24px` ตัวหนาคมชัด), รหัสการแก้ไข (`EST-2026-0042 • REV.02`), ป้ายสถานะ (`Pending Checker`) และแถบข้อมูลลูกค้า/ขอบเขตงานแบบแถวเต็มความกว้าง (Full Width) ปราศจากการเบียดอัด
- **Tier 2 (Operational Financial HUD)**: แถบสรุปตัวเลขการเงินระดับบริหารแบบ 3 คอลัมน์เต็มความกว้าง:
  - **ต้นทุนภายใน (BOQ Cost)**: แสดงตัวเลข $19\text{px}$ พร้อมสัดส่วนต้นทุนวัสดุและค่าแรง
  - **กำไรขั้นต้น (Gross Profit)**: แสดงกำไรตัวเลข $19\text{px}$ คู่กับป้ายกำกับสุขภาพกำไร (`▲ GP 38.87% (เป้าหมาย ≥30%)`)
  - **ยอดเสนอราคารวมสุทธิ (Quotation Total)**: โดดเด่นที่สุดที่ขนาด $26\text{px}$ สีกรมท่าเข้มพร้อมกรอบไฮไลต์พิเศษ

### 2.2 ซีกซ้าย: Dense Spec Sheet Table (ตารางสเปกและราคาความหนาแน่นสูง)
- แสดงรายการงานบิวต์อิน (Work Items) พร้อมสเปกวัสดุปิดผิว โครงสร้าง และระบบฮาร์ดแวร์
- จัดการแถบกรองประเภทงานทันที: งานบิวต์อิน, ผนังตกแต่ง & ระบบไฟ, ผ้าม่าน & วอลเปเปอร์, งานติดตั้งหน้างาน
- ช่องค้นหาด่วนตามชื่อวัสดุและรหัส
- ความสูงแถว $44\text{px}$ ตามเกณฑ์ Touch/Click Accessibility โดยไม่เปลืองพื้นที่แนวตั้ง

### 2.3 ซีกขวา: Live Bill-of-Materials (BOM) & Cutlist Inspector
- เมื่อคลิกเลือกรายการงานในตารางฝั่งซ้าย แผงฝั่งขวาจะกางรายละเอียดโครงสร้างย่อยทันที:
  - จำนวนแผ่นไม้อัด HMR และราคาต่อแผ่น
  - แผ่นลามิเนต Formica ลายเฉพาะ
  - ชุดอุปกรณ์ฟิตติ้งแท้ (Blum, Hafele)
  - ค่าแรงทีมช่างไม้ประกอบโรงงานและงานสี
- รวมทั้งแสดง **Governance Audit Trail**: บันทึกประวัติการส่งตรวจตามหลัก Maker-Checker (เช่น ผู้ตรวจส่งกลับเนื่องจาก GP ต่ำกว่าเกณฑ์ $30\%$ และการส่งปรับปรุงใหม่ใน Rev 02)

---

## 3. ระบบชุดสี (Design Tokens)

### 3.0 Tailwind-first Implementation Standard

มาตรฐานนี้ใช้กับ Application UI ภายใต้ `frontend/` โดยให้ **Tailwind CSS เป็นวิธี Styling หลัก**:

- เขียน Layout, spacing, typography, color, border, responsive behavior และ interaction states ด้วย Tailwind utilities ใน `className` เป็นค่าเริ่มต้น
- ใช้ semantic tokens ที่ประกาศใน `frontend/tailwind.config.ts` เช่น `bg-erp-navy`, `text-erp-text-main`, `border-erp-border` ก่อนใช้ arbitrary values (`[...]`) หรือสี Hex โดยตรง
- ใช้ responsive/state variants ของ Tailwind เช่น `sm:`, `md:`, `lg:`, `hover:`, `focus-visible:`, `disabled:` และ `dark:` เพื่อให้พฤติกรรมอยู่ใกล้ markup ที่ใช้งาน
- รวม class แบบมีเงื่อนไขด้วย `cn` จาก `frontend/src/lib/utils` และย้ายชุด utilities ที่เกิดซ้ำเป็น reusable component หรือ variant ของ component กลาง
- เก็บ CSS แยกไว้เฉพาะ Tailwind directives, reset/base styles, CSS custom properties ของ design tokens, browser-specific behavior หรือรูปแบบซับซ้อนที่ Tailwind อธิบายได้ไม่เหมาะสม โดยต้องจำกัด scope และระบุเหตุผลในโค้ด
- Inline `style` ใช้เฉพาะค่าที่คำนวณแบบ runtime และแทนด้วย Tailwind class/token ไม่ได้; ห้ามใช้เพื่อเลี่ยงระบบ tokens

กฎนี้ไม่ครอบคลุม Documentation Portal ใน `docs/portal/` ซึ่งยังคงเป็น semantic HTML, CSS และ JavaScript แบบ dependency-free ตาม `AGENTS.md`

### 3.0.1 Numeric Color Scales (-50 ถึง -950)

เพื่อความยืดหยุ่นในการปรับระดับความเข้ม-อ่อนเฉพาะจุด (เช่น hover backgrounds, subtle borders, zebra stripes) ระบบรองรับ Scale ตัวเลขที่แมปกับ CSS Variables:

- **Atelier Architectural Navy (`erp-navy-{50..950}`)**:
  - `50` (`#F0F5FA`), `100` (`#E1EBF5`), `200` (`#C3D7EC`), `300` (`#98BCDF`), `400` (`#659BD0`)
  - `500` (`#3D7BC0`), `600` (`#255FA3`), `700` (`#194B85`), `800` (`#123968`), `900` (`#0B3056` สีหลัก), `950` (`#071F38` hover)
- **Architectural Ice Slate (`erp-slate-{50..900}`)**:
  - `50` (`#F8FAFC`), `100` (`#F1F5F9`), `200` (`#E2EBF4`), `300` (`#D1DEEC` border), `400` (`#94A3B8`)
  - `500` (`#64748B`), `600` (`#536B88` muted), `700` (`#334155` body), `800` (`#1E293B`), `900` (`#0A192F` text main)

### 3.1 โหมดสว่าง (Light Mode — Solid Architectural Navy)

| โทเค็น (Token) | รหัสสี (Hex) | วัตถุประสงค์และการใช้งาน |
| :--- | :--- | :--- |
| **Canvas** | `#F4F6F9` | พื้นหลังของจอ สีไอซ์เกรย์นุ่มนวล สบายตา |
| **Surface** | `#FFFFFF` | พื้นผิวหน้าโต๊ะทำงาน ตาราง และแผงตรวจการ |
| **Surface Subtle** | `#EBF1F7` | หัวตาราง แถบเครื่องมือ แถบกรอง |
| **Text Primary** | `#0A192F` | ตัวหนังสือหลัก ยอดเงิน สเปก (คอนทราสต์ $>15:1$) |
| **Text Muted** | `#536B88` | รหัสเอกสาร คำอธิบายรอง วันที่ |
| **Text Subtle** | `#7E95B0` | เส้นนำสายตาและไอคอนรอง |
| **Primary Navy** | `#0B3056` | สีกรมท่าเข้มแท้ ปุ่มหลัก ขอบโครงสร้างหลัก (ไม่ใช่ฟ้า) |
| **Primary Hover** | `#071F38` | สีกรมท่าเข้มลึกเมื่อวางเมาส์ |
| **Row Selected** | `#E7EFF7` | สีไฮไลต์แถวที่ถูกเลือกในตาราง |
| **Active Bar** | `#0B3056` | ขีดแสดงตำแหน่งแถวที่เลือก ขนาด 3px |
| **Border Strong** | `#0B3056` | เส้นกรอบแบ่งอาณาเขตโมดูล $1\text{px}$ |
| **Border Rule** | `#D1DEEC` | เส้นแบ่งแถวและส่วนประกอบ |
| **Border Hairline** | `#E2EBF4` | เส้นแบ่งรายการภายใน |

### 3.2 โหมดมืด (Dark Mode — Midnight Architectural Navy)

| โทเค็น (Token) | รหัสสี (Hex) | วัตถุประสงค์และการใช้งาน |
| :--- | :--- | :--- |
| **Canvas** | `#050E1A` | มิดไนท์เนวีเข้มลึก ไร้แสงสะท้อนตาในเวลากลางคืน |
| **Surface** | `#0B1728` | พื้นผิวตารางและแผงควบคุม |
| **Surface Subtle** | `#142742` | หัวตารางและแถบเครื่องมือในโหมดมืด |
| **Text Primary** | `#F0F5FA` | ตัวหนังสือสีขาวไอซ์ คมชัดสูงสุด |
| **Text Muted** | `#9BB0C9` | ข้อความรองและหน่วยนับ |
| **Primary Navy** | `#2563EB` | สีปุ่มและไฮไลต์สำคัญ (Royal Navy คอนทราสต์ชัด) |
| **Row Selected** | `#162C4E` | สีไฮไลต์แถวที่เลือกในโหมดมืด |
| **Active Bar** | `#38BDF8` | ขีดแสดงตำแหน่งสีฟ้าครามสดใส $3\text{px}$ |
| **Border Strong** | `#1F3D68` | เส้นกรอบในโหมดมืด |

### 3.3 รหัสสถานะและระเบียบธรรมาภิบาล (Maker-Checker Semantics)

ทุกสถานะใช้มุมฉาก $0\text{px}$ พร้อมกรอบ $1\text{px}$:
- **อนุมัติแล้ว (Approved)**: พื้น `#EBF7F0`, ตัวหนังสือ `#14532D`, ขอบ `#A7F3D0`
- **รอผู้ตรวจอนุมัติ (Pending Checker)**: พื้น `#FFFBEB`, ตัวหนังสือ `#92400E`, ขอบ `#FDE68A`
- **ส่งกลับแก้ไข / ต่ำกว่าเกณฑ์ (Rejected / Below GP)**: พื้น `#FEF2F2`, ตัวหนังสือ `#991B1B`, ขอบ `#FECACA`

---

## 4. มาตรฐานขนาดตัวอักษรและลำดับความสำคัญ (Typography Scale & Visual Prominence)

ระบบยึดหลักมาตรฐานสรีรศาสตร์สายตา (Ergonomic Readability) โดยกำหนดระดับขนาดตัวอักษรอย่างเป็นระบบ และผลักดันส่วนสำคัญที่สุดทางธุรกิจให้โดดเด่นอย่างชัดเจน:

### 4.1 สเกลขนาดตัวอักษรมาตรฐาน (Type Scale)

| ระดับบทบาท (Role) | ขนาด (Size) | น้ำหนัก (Weight) | การใช้งานในระบบ (Usage) |
| :--- | :--- | :--- | :--- |
| **Hero Financial Number** | $26\text{px}$ | 800 (Boldest) | ยอดเสนอราคารวมสุทธิใน HUD (`hud-num.target`) โดดเด่นที่สุดในหน้าจอ |
| **Document Display Title** | $25\text{px}$ | 800 (Boldest) | หัวข้อเอกสารหลัก (`document-h1`) เช่น ชื่อโครงการคอนโดมิเนียม |
| **Item Hero Cost** | $20\text{px}$ | 800 (Bold) | ตัวเลขต้นทุนชิ้นงานที่เลือกใน Inspector (`hero-cost-value`) |
| **Financial Sub-Metrics** | $18\text{px}$ | 700–800 | ตัวเลขต้นทุน BOQ รวม และกำไรขั้นต้น GP (`hud-num.gp`) |
| **Item Hero Title** | $16\text{px}$ | 800 (Bold) | ชื่องานบิวต์อินในการ์ดหัวเรื่องของ Inspector (`hero-item-title`) |
| **Table Selling Price** | $15\text{px}$ | 800 (Bold) | คอลัมน์ราคาเสนอขายในตาราง (`col-selling-price`) เน้นเด่นชัด |
| **Table Item Title** | $14.5\text{px}$ | 700 (Bold) | ชื่องานในตารางสเปก (`item-title-bold`) สะดุดตา แยกจากสเปกย่อย |
| **BOM Cost** | $14.5\text{px}$ | 700 (Mono) | ตัวเลขต้นทุนชิ้นส่วนย่อยในแผง BOM |
| **Base Body / UI Controls** | $13.5\text{px} - 14\text{px}$ | 500–700 | เนื้อหาทั่วไป, ปุ่มกด Action (`btn-action`), เมนูกรอง, รหัส Code |
| **Secondary Specs / Meta** | $12.5\text{px} - 13.5\text{px}$ | 400–600 | สเปกวัสดุแบบละเอียดในตาราง (`item-specs-quiet`), ข้อมูลลูกค้า |
| **Status / GP Badges** | $11.5\text{px} - 12\text{px}$ | 700–800 | ป้ายสถานะ (`status-badge`), ป้ายกำไร (`table-gp-pill`), ป้าย Benchmark |
| **Labels & Headers** | $11\text{px} - 11.5\text{px}$ | 700 (Uppercase) | หัวตาราง (`th`), เลเบลกำกับตัวเลข HUD (`hud-label`) |

*ข้อกำหนดตายตัว: ห้ามมีตัวหนังสือขนาดต่ำกว่า $11\text{px}$ ในทุกกรณี เพื่อรักษามาตรฐานการเข้าถึงข้อมูล (Accessibility Floor)*

### 4.2 การเน้นส่วนสำคัญให้โดดเด่น (Visual Hierarchy & Prominence Rules)

1. **ยอดเสนอราคารวมสุทธิ (Quotation Selling Total)**:
   - ตกแต่งด้วยกรอบสีเด่นพิเศษ (`.hud-cell.target-cell`) ใช้ฟอนต์ขนาด $26\text{px}$ สีน้ำเงินเข้มสถาปัตย์ สะดุดตาทันทีที่เปิดหน้าจอ
2. **อัตรากำไรขั้นต้น (GP% Health Benchmark)**:
   - แสดงตัวเลข $18\text{px}$ พร้อมป้ายกำกับระดับความปลอดภัยกำไร เช่น `▲ GP 38.87% (เป้าหมาย ≥30%)` เพื่อให้ผู้ประเมินราคาและผู้บริหารเช็กความเสี่ยงได้ทันที
3. **ราคาเสนอขายในตาราง (Selling Total Column)**:
   - ใช้ฟอนต์ตัวหนาพิเศษ $15\text{px}$ พร้อมสีกรมท่าเข้ม โดดเด่นกว่าคอลัมน์ต้นทุนทั่วไป ช่วยให้กวาดสายตาตรวจเช็กยอดได้ในเสี้ยววินาที
4. **ป้ายสถานะกำไรในตาราง (GP% Pills)**:
   - มีการตีกรอบป้ายสีเขียว (`.table-gp-pill.ok`) สำหรับรายการที่กำไรเกินเกณฑ์ และสีแดงส้ม (`.table-gp-pill.alert`) สำหรับรายการที่กำไรต่ำ (เช่น ค่าแรงช่าง 14.3%) ให้เห็นจุดเสี่ยงทันที
5. **แถวงานที่เลือกอยู่ (Active Selected Row)**:
   - ตรึงขอบซ้ายด้วยแท่งสีหนา $4\text{px}$ (`border-left: 4px solid var(--navy-active-bar)`) พร้อมพื้นหลังสีไฮไลต์นวลตา ยกระดับความชัดเจนว่ากำลังตรวจสเปกชิ้นใดอยู่
6. **การ์ดหัวเรื่องในแผงตรวจ BOM (Inspector Hero Card)**:
   - แยกกล่องการ์ดสรุปชิ้นงานที่มีรหัส Code สีทึบ ชื่องานขนาด $16\text{px}$ และยอดต้นทุนรวม $20\text{px}$ แยกชัดเจนจากรายการชิ้นส่วนวัสดุย่อย

### 4.3 มาตรฐานการจัดวางฟอร์มและแถบปุ่มติดหนึบ (Form & Sticky Action Bar Standards)

- **สัดส่วนและการแบ่งคอลัมน์ (Split Screen Grid):** ฟอร์มระดับ Enterprise ใช้ 12-column grid (`lg:grid-cols-12 gap-6 items-start`) เช่น แบ่งฝั่งซ้าย (ข้อมูลหลัก 7-8 ส่วน) และฝั่งขวา (ข้อมูลติดต่อ/ส่วนเสริม 4-5 ส่วน) หลีกเลี่ยงการบีบฟอร์มแคบตรงกลางที่ทำให้เสียสัดส่วน
- **ระยะห่างสม่ำเสมอ (Harmonized Padding & Margin):** คอนเทนเนอร์หลัก (`.erp-main-content`) กำหนด padding สม่ำเสมอรอบด้าน (`p-4 sm:p-6`: 16px บนมือถือ, 24px บนจอใหญ่)
- **แถบ Action Bar ติดหนึบล่างจอ (Flush Sticky Action Bar):** ใช้ `sticky bottom-0 z-40` พร้อม negative margin `-mx-4 -mb-4 mt-8 sm:-mx-6 sm:-mb-6` เพื่อหักล้าง padding ของคอนเทนเนอร์หลักอย่างพอดีเป๊ะ และมี padding ด้านใน `px-4 py-3 sm:px-6 sm:py-4` ทำให้ปุ่มและสถานะตรงแนวกับเนื้อหาฟอร์มด้านบนอย่างนิ่งสนิท ไม่เด้งไปเด้งมา

### 4.3.1 มาตรฐานสถาปัตยกรรมแท็บฟอร์ม (Form Tabs & Zero-Scroll Standards)

สำหรับฟอร์มที่มีข้อมูลหลากหลายหมวดหมู่ เพื่อลดการเลื่อนหน้าจอ (Zero-Scroll Form) ให้ใช้คอมโพเนนต์กลาง `<FormTabs />` และ Hook `useFormTabErrors` จาก `@/components/forms`:
- **สไตล์ Atelier Architectural Navy Sharp:** คม เหลี่ยม ปราศจากความโค้งมน (`border-radius: 0px !important`) แท็บที่แอกทีฟเป็น Solid Navy `#0B3056` ตัวหนังสือขาว และแท็บทั่วไปเป็นพื้นผิว Surface พร้อม Hover Subtle
- **Error Indicator บนหัวแท็บ (Red Dot Badge):** เมื่อมีฟิลด์ในแท็บใดไม่ผ่านการตรวจสอบ (Validation Failure) ให้แสดงจุดเตือนสีแดง (`bg-erp-danger animate-pulse`) บนปุ่มหัวแท็บนั้นทันที
- **Auto-Switch ไปแท็บที่มี Error:** เมื่อผู้ใช้กดปุ่ม Submit จากแท็บอื่น ระบบจะสลับแท็บกลับไปยังแท็บแรกที่มี Error ทันทีโดยอัตโนมัติ พร้อมโฟกัสช่องที่พบปัญหา
- **อิสระในการคลิกสลับแท็บ (Unrestricted Navigation):** ผู้ใช้สามารถคลิกสลับแท็บไปมาเพื่อกรอกข้อมูลส่วนใดก่อนก็ได้ ไม่มีการบล็อกลำดับขั้นตอน
- **การรักษา DOM ของฟอร์ม (CSS `hidden` vs Unmount):** สลับแท็บด้วย Class `hidden` เสมอ ห้ามใช้การ Unmount เพื่อป้องกัน React Hook Form สูญเสียการลงทะเบียนของช่องอินพุต

### 4.3.2 มาตรฐานความสูงของช่องกรอกข้อมูลและปุ่ม Action มุมขวาบน (Input Height Harmony & Top-Right Label Actions)

เพื่อให้การวางช่องข้อมูลใน Grid แนวนอนมีระเบียบวินัยและความสมดุลทางสายตาสูงสุด:
- **Strict Input Height Invariant & Density Scale:** ช่องกรอกข้อมูลทุกประเภท ทั้ง `<input>`, `<select>`, `<textarea>`, `<EntityAutocomplete>` และ Selected Summary Cards ในระดับกริดเดียวกันต้องมีความสูงสม่ำเสมอเท่ากันในทุกเบราว์เซอร์ โดยอ้างอิงตาม Control Height Token (`var(--erp-control-height)` / `size.control.height` ซึ่งในโหมดเริ่มต้น Spacious คือ $44\text{px}$ (`h-11 min-h-[44px]`), โหมด Comfortable คือ $40\text{px}$ (`h-10 min-h-[40px]`), และโหมด Compact คือ $36\text{px}$ (`h-9 min-h-[36px]`)) พร้อมขนาดตัวอักษรมาตรฐาน `text-sm`
- **Top-Right Field Actions (ปุ่ม Action ประจำฟิลด์):** ลิงก์หรือปุ่มสร้างข้อมูลใหม่/ช่วยเหลือประจำฟิลด์ (เช่น `[+ สร้างโอกาสทางการขายใหม่]`, `[เลือกจากลิสต์]`) ต้องจัดวางอยู่ที่ **มุมขวาบนของ Input ในระนาบเดียวกับ Label เสมอ** (`flex items-center justify-between min-h-[20px] mb-1.5`) ห้ามปล่อยให้ห้อยอยู่ใต้ Input ให้เสียสมดุลแนวตั้งของ Grid ในแถวเดียวกัน
- **Icon-First Adjacent Field Actions with Tooltip (`size="icon"` + `<Tooltip />`):** ปุ่ม Action ที่อยู่ระนาบเดียวกับ Input/Select (เช่น ปุ่ม Quick-Add Master Data `+`, ปุ่มสุ่มรหัส, ปุ่มค้นหา):
  1. ต้องใช้ **Icon-only Button (`size="icon"` ขนาด $44\text{px} \times 44\text{px}$)** เสมอ ห้ามเขียนข้อความยาวๆ เช่น `[+ เพิ่ม]` เพื่อประหยัดพื้นที่แนวนอนและป้องกันข้อความตกบรรทัด
  2. ต้องครอบด้วย `<Tooltip content="..." position="top">` พร้อมใส่ `aria-label` ทุกครั้ง เพื่อคงความสามารถในการเข้าถึง (Accessibility)
  3. คอนเทนเนอร์ Input/Select ต้องใส่ `wrapperClassName="!mb-0"` และจัดแนวแถวด้วย `flex items-center gap-2` เพื่อล็อกขอบบนและล่างให้ขนานกับช่องกรอกพอดีเป๊ะ
- **Name-First Dropdown & Autocomplete Hierarchy:** การแสดงผลตัวเลือกและผลลัพธ์การค้นหาต้องนำชื่อรายการที่สื่อความหมาย (Item Name, Title, Customer Name) ขึ้นเป็นหลัก (Primary / Hero) ตัวหนาชัดเจน ควบคู่กับรหัส Code และสถานะเป็นข้อความรอง (Secondary / Meta) เสมอ ห้ามแสดงเฉพาะรหัสตัวเลขเดี่ยวๆ
- **No Duplicate Symbols in i18n:** เมื่อใส่ไอคอน SVG (เช่น `<IconPlus />`) ในปุ่ม Action ข้อความในระบบแปลภาษา (`th.json`, `en.json`) ต้องเป็นตัวอักษรล้วน ห้ามใส่เครื่องหมาย `+ ` ซ้ำซ้อน

### 4.3.3 มาตรฐานความสมดุลของหัวการ์ดแถวเดียวและช่องหลายภาษา (Single-Row Card Header & MultiLangInput Baseline)
- **Single-Row Compact Card Header (Zero-Void Standard):** เมื่อออกแบบหัวการ์ดที่พับได้ (Collapsible Cards หรือ Sections) ให้ทุกองค์ประกอบ (ปุ่มเปิด-ปิด, รหัส, ช่องกรอกชื่อ, เมนูกรอบช่วย, ปุ่มลบ) อยู่ในแนวระนาบเดียวกัน ล็อกความสูงเท่ากันเป๊ะที่ $44\text{px}$ (`h-11 min-h-[44px]`)
- **MultiLangInput ใน Single-Row Header:** ต้องส่ง `hideLabel={true}` เสมอ เพื่อให้ช่องกรอกและปุ่มสลับภาษา `[TH | EN]` วางขนานในระนาบแนวนอนเดียวกัน ความสูง $44\text{px}$ พอดีเป๊ะ ป้องกันการเกิดความเหลื่อมล้ำแนวตั้ง (Zero Vertical Baseline Offset)

### 4.3.4 มาตรฐานความสมบูรณ์ของโครงสร้างตารางและโฟกัสของหน้าต่างป็อปอัป (Table Cell Flow Integrity & Modal Clean Focus)
- **Table Cell Flow Integrity (Zero Table Shifting):** ห้ามใส่คลาส `sr-only` ลงบนแท็ก `<th>`, `<td>`, `<TableHead>` หรือ `<TableCell>` โดยเด็ดขาด เพราะ `position: absolute` จะตัดเซลล์ออกจาก Table Flow Calculation ทำให้จำนวนคอลัมน์ของ `<thead>` และ `<tbody>` ไม่เท่ากัน ส่งผลให้คอลัมน์ทั้งหมดเลื่อนเยื้องไป 1 ช่องและตารางเบี้ยว หากต้องการซ่อนข้อความ ให้ใส่ `<span className="sr-only">...</span>` ภายในเซลล์เสมอ
- **Modal Header Clean Focus (No Hover/Focus Artifacts):** ปุ่ม Action บนส่วนหัวของ Modal หรือ Dialog (เช่น ปุ่มขยายเต็มจอ ปุ่มรีเฟรช) ห้ามครอบด้วย `<Tooltip>` ที่เปิดอัตโนมัติเมื่อได้รับ Focus เพราะ Focus Trap ของ Modal จะดึงโฟกัสไปที่ปุ่มแรกทันที ทำให้ Tooltip เด้งค้าง ให้ใช้แอตทริบิวต์ Native `title="..."` และ `aria-label="..."` แทน เช่นเดียวกับปุ่มปิด Modal

### 4.5 มาตรฐาน 2-Tier Architectural Filter Toolbar ในหน้ารายการ (List Toolbar Standards)

สำหรับหน้าแสดงรายการที่มีตัวกรองมากกว่า 2 มิติขึ้นไป (เช่น มีทั้ง Search, หมวดหมู่/สถานะ, ผู้รับผิดชอบ, ลูกค้า หรือช่วงเวลา) **ห้ามยัดทุกตัวกรองในแถวเดียวแบบบีบอัด (`flex flex-wrap items-end`)**:
1. **Tier 1 (แถวค้นหาหลักและ Actions):** จัดวาง `ListSearchInput` กว้างสมส่วน (`w-full sm:max-w-md lg:max-w-lg`) ร่วมกับปุ่ม Action (เช่น `ExportDropdown` หรือ Export Button) ชิดขวาในแนวระนาบเดียวกัน (`self-start sm:self-end`)
2. **Tier 2 (กริดตัวกรองละเอียด Refined Grid):** คั่นด้วยเส้น `border-t border-erp-border/60 pt-2` แล้วจัดวางตัวกรองด้วย CSS Grid สม่ำเสมอ:
   - Desktop: `lg:grid-cols-4 gap-3` (หรือ 3-4 คอลัมน์เต็มพื้นที่ตามจำนวนตัวกรอง)
   - Tablet: `sm:grid-cols-2 gap-3`
   - Mobile: `grid-cols-1 gap-3`
   - อินพุตและตัวกรองทุกชิ้นใน Grid (ทั้ง `ListFilterSelect`, `CustomerAutocomplete`, `UserAutocomplete`) ต้องกำหนด `widthClassName="w-full"` หรือ `className="w-full"` เพื่อให้กว้างเต็มช่องสม่ำเสมอ ไม่หักฟันปลาหรือเกิดช่องว่างแหว่ง
3. **Tier 3 (Active Filter Chips):** ส่งผ่าน prop `activeFilters` ของ `<ListToolbar />` เพื่อแสดงชิปตัวกรองด้านล่างสุดของ Toolbar เสมอ

### 4.4 มาตรฐานขนาดและบทบาทของปุ่ม (Button Scale & Usage Rules)

เพื่อป้องกันปัญหาปุ่มสูงต่ำไม่เท่ากันหรือไม่สอดคล้องกันข้ามหน้าจอ ระบบกำหนดขนาดปุ่มกลางของ Atelier Architectural Navy Sharp ไว้อย่างเคร่งครัด:

| ขนาด (Size) | ความสูง (Min-Height) | Padding แนวนอน/ตั้ง | ขนาดตัวอักษร | วัตถุประสงค์และบทบาทการใช้งาน (Usage Role) |
| :--- | :--- | :--- | :--- | :--- |
| **`sm`** (Compact / Toolbar) | **$36\text{px}$** (`.erp-btn-sm`) | `py-1 px-3` (`0.25rem 0.75rem`) | $12\text{px} - 13\text{px}$ (`text-xs`) | ใช้ในพื้นที่จำกัด: ListToolbar Actions (เช่น Export CSV), Quick-view drawer footer, Inline card actions |
| **`md`** (Standard / Default) | **$44\text{px}$** (`.erp-btn-md`) | `py-2 px-4` (`0.5rem 1rem`) | $14\text{px}$ (`text-sm`) | **มาตรฐานหลักของระบบ**: PageHeader Primary Action (ปุ่ม Create), Form Action Bar (ปุ่ม Save/Cancel), Confirmation Modal buttons, Error Retry buttons |
| **`lg`** (Hero / Prominent) | **$48\text{px}$** (`.erp-btn-lg`) | `py-3 px-6` (`0.75rem 1.5rem`) | $16\text{px}$ (`text-base`) | จังหวะ Call-to-action ใหญ่, Landing page หรือหน้า Portal ที่ต้องการความโดดเด่นสูงสุด |
| **`icon`** (Square Target) | **$44\text{px} \times 44\text{px}$** | `p-2` (`0.5rem`) | - | ปุ่มไอคอนเดี่ยวตามเกณฑ์ Touch Accessibility Floor ($44\text{px}$) |

#### กฎเหล็กในการใช้งานปุ่ม (Strict Guardrails):
1. **Reuse Central Component:** ทุกปุ่มการทำงานต้องเรียกผ่านคอมโพเนนต์กลาง `<Button />` เสมอ (ซึ่งรองรับ prop `href` เมื่อเป็น Navigation Link) ห้ามใช้แท็ก `<button>` หรือ `<Link>` แต่งสไตล์ปุ่มเองโดยเด็ดขาด
2. **No Arbitrary Height Overrides:** ห้าม override ด้วย `min-h-[40px]`, `min-h-[44px]`, `h-10` หรือ inline style ทับความสูงของปุ่ม ให้ใช้ขนาด `size="sm" | "md" | "lg"` จาก Design System เท่านั้น
3. **PageHeader Actions = `size="md"` ($44\text{px}$):** ทุกหน้าจอ (Customer, Opportunity, Site) ปุ่ม Action หลักบนส่วนหัวต้องมีขนาดเท่ากันคือ $44\text{px}$
4. **Form Actions & Modals = `size="md"` ($44\text{px}$):** ปุ่มใน FormActionBar และ ConfirmationModal ต้องมีความสูงมาตรฐานเท่ากันที่ $44\text{px}$

---

## 5. มาตรฐาน Responsive ทุกขนาดหน้าจอ (Responsive System for All Devices)

ระบบถูกออกแบบให้รองรับตั้งแต่สมาร์ตโฟนหน้างานจนถึงจอมอนิเตอร์ Ultra-wide ในสตูดิโอออกแบบอย่างแม่นยำ โดยไม่บีบอัดข้อมูลจนเสียความหมาย:

| ระดับหน้าจอ (Breakpoint) | พฤติกรรมเลย์เอาต์ (Layout Behavior) | การปรับเปลี่ยนเฉพาะทาง (Adaptation Details) |
| :--- | :--- | :--- |
| **Ultra-wide & 4K**<br>($\ge 1600\text{px}$) | **Centered Architectural Frame** | กำหนด `max-width: 1720px` ตรงกลาง พร้อมเส้นขอบสถาปัตย์ซ้าย-ขวา ป้องกันตารางยืดขยายจนสายตากวาดไม่ถึง |
| **Standard Desktop**<br>($1200\text{px} - 1599\text{px}$) | **Dual Split Workspace** | ตาราง Spec Sheet (ซ้าย) คู่กับ Material BOM Inspector (ขวา) แบบเคียงข้างเต็มสัดส่วน |
| **Tablet Landscape**<br>($992\text{px} - 1199\text{px}$) | **Proportional Split** | ปรับความกว้างแผง BOM ฝั่งขวาเป็น $330\text{px}$ ตารางฝั่งซ้ายเลื่อนแนวนอนได้อย่างลื่นไหล |
| **Tablet Portrait**<br>($769\text{px} - 991px$) | **Stacked / Adaptive Layout** | แถบ Financial HUD และเมนูกรองปรับการเรียงตัว ตารางและแผงตรวจการรองรับทั้งนิ้วแตะและปากกา Stylus |
| **Mobile Screens**<br>($320\text{px} - 768\text{px}$) | **Segmented Mobile Workspace** | • **Mobile Tab Switcher**: มีแท็บสลับ `[ตารางสเปก]` กับ `[ชิ้นส่วน BOM]` ติดหนึบด้านบน<br>• **2-Row Financial HUD**: แบ่งเป็น 2 คอลัมน์ (Cost / GP%) และแถวเต็มสำหรับ Selling Price ตัวเลขคมชัดไม่ล้นจอ<br>• **Horizontal Swipe Filters**: แถบฟิลเตอร์เลื่อนซ้ายขวาได้ด้วยนิ้วโป้ง<br>• **Sticky Code Column**: ล็อกคอลัมน์รหัสสินค้า (Code) ไว้ด้านซ้ายขณะเลื่อนดูราคาในตาราง<br>• **Mobile Backbar**: ปุ่มลัดกระโดดกลับจากหน้าตรวจ BOM ไปยังตารางได้ทันที<br>• **Touch Target Accessibility**: ทุกปุ่มกดและแถวตารางมีความสูงอย่างน้อย $44\text{px} - 48\text{px}$ |

---

## 6. การเปิดทดสอบและสาธิตจริง

ไฟล์ต้นฉบับที่พร้อมใช้งานจริง:
- **หน้าจอขนาดเต็ม (Full Responsive Atelier Workspace)**: [preview.html](file:///Users/syaco/Documents/development/tan-erp/preview.html)
- ทดสอบการย่อ-ขยายหน้าต่างเบราว์เซอร์เพื่อดูการปรับเลย์เอาต์อัตโนมัติ (Desktop $\rightarrow$ Tablet $\rightarrow$ Mobile)
- บนจอมือถือ สามารถทดสอบแตะแท็บสลับระหว่าง **ตารางสเปก** และ **ชิ้นส่วน BOM** ได้ทันที
- สลับโหมดสว่าง/มืดได้ทันทีผ่านปุ่มบน Header

---

## 7. Theming & Cross-platform Design Tokens (เว็บ + แอป Flutter)

> เหตุผลการตัดสินใจ: [ADR 0031](docs/adr/0031-cross-platform-design-tokens-and-theming.md). สถานะ: มาตรฐานใหม่ — ยังอยู่ระหว่างย้ายค่าที่เขียนตายตัวในเว็บ (แผน: [Mobile Foundation Phase 0](docs/superpowers/plans/2026-10-26-mobile-foundation-phase-0.md))

### 7.1 แหล่งความจริงเดียว

- ค่าดีไซน์ทั้งหมด (สี, รูปทรงมุม, เส้นขอบ, ระยะห่าง, ความสูง control, ตัวอักษร) อยู่ใน `design-tokens/` (W3C DTCG JSON) แล้ว build ด้วย Style Dictionary เป็น `frontend/src/styles/tokens.css` และ `mobile/lib/core/theme/tokens.g.dart` — **ห้ามแก้ไฟล์ที่ generate ด้วยมือ**
- ชั้นของ token: **primitive** (ค่าดิบ เช่น `navy.900`) → **semantic** (ความหมาย เช่น `color.action.primary`, `radius.control`, `border.width.hairline`) → **component** (เฉพาะจำเป็น เช่น `button.height.default`)
- ค่าในหมวด 3 (สี light/dark, numeric scales) คือค่าของ primitive/semantic token ชุดเริ่มต้น **Atelier Architectural Navy Sharp**

### 7.2 กฎการใช้ใน Component

- ใช้ได้เฉพาะ semantic/component token: เว็บผ่าน Tailwind semantic classes / `var(--…)`, Flutter ผ่าน `Theme.of(context).extension<ErpTokens>()`
- **ห้าม** hex/rgb ตรง, ห้ามตัวเลข radius/border-width ตรง, ห้าม `!important` เพื่อบังคับรูปทรง — ทั้งหมดต้องมาจาก token
- **รูปทรงคมเป็นค่าเริ่มต้นของ token** (`radius.* = 0`) ไม่ใช่การเขียน `border-radius: 0 !important` ในแต่ละ selector; หน้าตาที่ผู้ใช้เห็นต้องเหมือนเดิมทุกพิกเซล
- ตรวจด้วย lint ทั้งสองฝั่ง (เว็บ: ESLint/Stylelint ห้าม hex และ `rounded-*` ที่ไม่ผ่าน token; Flutter: `custom_lint` ห้าม `Color(0x…)` / `BorderRadius.circular(...)` นอก `core/theme/`)

### 7.3 ใครปรับอะไรได้

| ระดับ | ปรับได้ | ค่าเริ่มต้น | เก็บที่ |
| :--- | :--- | :--- | :--- |
| ผู้ใช้ | โหมด สว่าง/มืด/ตามเครื่อง, ความแน่น ปกติ/กะทัดรัด, ขนาดตัวอักษร 90–130% | ตามเครื่อง / ปกติ / 100% | preference ของผู้ใช้ใน Backend (เว็บและแอปได้ค่าเดียวกัน) |
| Admin องค์กร | โลโก้, สีหลักแบรนด์ 1 สี | Atelier Navy `#0B3056` | ค่าตั้งขององค์กรใน Backend |
| ทีมพัฒนา | รูปทรงมุม, ความหนาเส้นขอบ, ระยะห่าง, ฟอนต์, สเกลสี | Atelier (radius 0, hairline 1px) | token ใน repo |

- ผู้ใช้และ admin **ปรับรูปทรงมุม เส้นขอบ หรือระยะห่างไม่ได้**
- ความแน่นแบบกะทัดรัดลดระยะห่างและความสูงแถวตารางได้ แต่ปุ่มกดบนอุปกรณ์สัมผัสต้องสูง ≥ 44px เสมอ
- ขนาดตัวอักษรต้องปรับได้โดยเลย์เอาต์ไม่ตัดข้อความ (ทดสอบที่ 130%)

### 7.4 สีแบรนด์องค์กร

- ต้องผ่าน WCAG AA ก่อนบันทึก: ข้อความขาวบนสีหลัก ≥ 4.5:1 และสีหลักบนพื้น canvas ≥ 3:1 — ตรวจทั้งที่ Backend และแสดงผลสดในหน้า admin; ไม่ผ่าน = ปุ่มบันทึก disabled พร้อมเหตุผล
- สีโหมดมืดคำนวณจากสีแบรนด์อัตโนมัติและต้องผ่านเกณฑ์เดียวกัน
- มีปุ่มรีเซ็ตกลับ Atelier Navy เสมอ

### 7.5 เปลี่ยนเอกลักษณ์ดีไซน์ในอนาคต

แก้ค่า token จุดเดียว → build → เว็บและแอปเปลี่ยนพร้อมกัน และต้องอัปเดตเอกสารนี้ในรอบเดียวกัน; การเพิ่มแกนให้ผู้ใช้ปรับต้องมี ADR ใหม่

---

## 8. มาตรฐาน UX ของแอปภาคสนาม (Flutter Field App)

> อนุมัติจาก mock เปรียบเทียบเมื่อ 2026-10-26: เลือก **A มุมคม (Atelier)** เหมือนเว็บ. สถาปัตยกรรม: [Mobile Architecture](docs/02-architecture/mobile-architecture.md)

แอปใช้ token สี/ตัวอักษร/รูปทรงชุดเดียวกับเว็บ (หมวด 7) แต่ **จัดข้อมูลต่างจากเว็บ**: เว็บเน้นข้อมูลแน่นสำหรับงานโต๊ะ ส่วนแอปเน้นใช้มือเดียวหน้างาน — กระชับ กดง่าย ไหลลื่น ไม่รก

### 8.1 รูปทรงและสี

- รูปทรงคม 0px ทุก control/card/sheet (ค่าเริ่มต้นของ radius token เหมือนเว็บ)
- สีหลัก Navy `#0B3056` สำหรับปุ่มหลักและเมนูที่เลือก
- **สีเน้นไม้โอ๊ก `#8A5A24`** (token `color.accent.measure`) ใช้ **เฉพาะ** เส้นบอกขนาดของค่าวัด และวิธีวัดที่เลือกอยู่ — ห้ามใช้ตกแต่งที่อื่น
- ไม่มีเงา ยกเว้น bottom sheet และแถบปุ่มหลักที่ติดล่างจอ; ไม่มี gradient

### 8.2 กฎการจัดหน้าจอ

| กฎ | รายละเอียด |
| :--- | :--- |
| 1 จอ 1 งาน | ปุ่มหลักปุ่มเดียวต่อจอ เต็มความกว้าง สูง 52dp ติดล่างจอในระยะนิ้วโป้ง (เหนือ bottom nav ถ้ามี) |
| ข้อมูลกระชับ | รายการแสดงไม่เกิน 3 บรรทัด: ชื่อ, ข้อมูลรอง 1 บรรทัด, สถานะ — รายละเอียดอื่นแตะดูต่อ |
| ไม่มีตาราง | ใช้รายการ/การ์ดแทน |
| ตัวเลขค่าวัดเด่นที่สุด | ค่าวัดหลัก 44sp ตัวหนา ตัวเลข tabular พร้อมเส้นบอกขนาดสีไม้โอ๊ก (`accentMeasure`) — เป็นจุดเด่นจุดเดียวของจอ แตะเพื่อเปิดแป้นตัวเลข 56dp ทันที |
| แถบสลับห้องนิ้วโป้งเดียว | Horizontal Room Thumb Strip ในหน้าสำรวจ เลื่อนและสลับห้อง A-01, A-02 ได้ด้วยนิ้วโป้งเดียวโดยไม่ต้องไถหน้าจอยาว |
| เครื่องมือวัดด่วนในระนาบเดียว | Quick Measure Instrument Bar จัด 4 โหมดหลัก (แป้นตัวเลข 56dp, เลเซอร์ BT, รูปถ่าย A4, สแกน AR) ความสูง ≥ 48dp ในแถวเดียว แตะเข้าโหมดวัดได้ทันที |
| เปิดเผยทีละขั้น | ช่องไม่บังคับอยู่ใต้ "เพิ่มเติม"; ตัวเลือกใช้ bottom sheet แทน dropdown |
| กดง่าย | เป้ากด ≥ 48dp, ห่างกัน ≥ 8dp; แป้นตัวเลข 56dp |
| ป้อนเร็ว | แป้นตัวเลขของแอปเองในจอวัด, แตะช่องแล้วกดแป้นแรกแทนค่าเดิม, ปุ่มวัดด่วน (เลเซอร์/จากรูป) อยู่ในแถววิธีวัด |
| ไหลลื่น | การเปลี่ยนจอ ≤ 200ms, haptic เบาเมื่อกด, บันทึกลงเครื่องทันทีไม่รอเน็ต; เคารพ reduce motion |
| สถานะไม่รบกวน | แถบซิงก์บางด้านบน แสดงเฉพาะเมื่อมีเรื่องต้องรู้ |
| จอว่างมีทางไปต่อ | บอกเหตุผลสั้นๆ และมีปุ่มทำต่อ 1 ปุ่ม |

### 8.3 ตัวอักษรและข้อความ

- ฟอนต์เดียว รองรับไทย; สเกล 13 / 15 / 17 / 22 / 44 sp; line-height ไทย 1.5; ตัวเลขใช้ tabular figures
- ข้อความสั้น ใช้คำกริยาตรงๆ ("บันทึก", "เริ่มงานถัดไป", "ส่งทั้งหมดตอนนี้"); ชื่อปุ่มกับข้อความยืนยันใช้คำเดียวกัน
- ห้ามป้ายตัวพิมพ์ใหญ่เหนือหัวข้อ, ห้ามข้อความคั่นด้วยจุดกลางแบบ "A · B", ห้ามลูกศรต่อท้ายปุ่ม

### 8.4 Navigation

- มือถือ: bottom nav 5 เมนู งานวันนี้ / สำรวจ / สแกน / แจ้งเตือน / ฉัน (ไอคอน SVG stroke + ข้อความ)
- แท็บเล็ต (≥ 768dp): navigation rail + master-detail (หมวด 2) โดยซีกซ้ายเป็น Master Directory (รายการห้องและข้อมูลโครงการ) และซีกขวาเป็น Detail Workspace (ตัวเลข 44sp, เครื่องมือวัดด่วน, รูปถ่าย และสรุปประเมินราคาสด)
