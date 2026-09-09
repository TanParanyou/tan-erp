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
