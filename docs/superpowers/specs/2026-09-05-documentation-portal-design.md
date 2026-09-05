# tan-erp Documentation Portal Design

## เป้าหมาย

สร้างฐานเอกสาร Project ERP ที่คนธุรกิจและนักพัฒนาอ่านร่วมกันได้ พร้อม Static HTML Portal ที่สร้างแผนผังจาก JSON และเชื่อมไปยัง Markdown โดยยังไม่สร้าง Application Code, Database หรือ Infrastructure จริง

## แนวทางที่เลือก

ใช้ Markdown แยกตามความรับผิดชอบเป็นแหล่งอ้างอิงหลัก และใช้ HTML/CSS/JavaScript แบบไม่พึ่ง Framework เป็น Viewer เท่านั้น ข้อมูล Flow อยู่ใน JSON แยกไฟล์และตรวจรูปแบบด้วย JSON Schema ภาพประกอบเป็น Local SVG

## เหตุผล

- แก้เอกสารและ Flow ได้โดยไม่ต้อง Build ระบบ
- ไม่มี Package Dependency หรือ Vendor Lock-in
- เปิดผ่าน Local Static Server ได้ทุกระบบ
- แยก Content ออกจาก Presentation จึงเพิ่ม Flow หรือภาษาในอนาคตได้
- Markdown ยังคงอ่านได้แม้ไม่เปิด Portal

## ประสบการณ์ใช้งาน

หน้าแรกแสดงภาพรวม ระยะโครงการ และตัวเลือกแผนผัง ผู้ใช้สลับไทย/อังกฤษ ค้นหา กรองตาม Phase และเปิดรายละเอียดของแต่ละ Node ได้ Desktop แสดง Flow แบบ Grid ส่วนมือถือแสดงลำดับแนวตั้ง ทุก Diagram มี Text Alternative และลิงก์ไปเอกสารหลัก

## Visual Direction

ใช้รูปแบบ Architectural Blueprint ผสม Swiss editorial grid: พื้นหลังโทนอุ่น ข้อความสีน้ำเงินเข้ม Accent สีเขียวอมฟ้าหนึ่งสี เส้นโครงสร้างและ SVG ลายเส้น สื่อถึงงานออกแบบภายในโดยไม่เลียนแบบหน้าเว็บบริษัท

## ขอบเขตข้อมูล JSON

แต่ละ Flow มี `version`, metadata สองภาษา, groups, nodes และ edges ส่วน `portal.json` เป็น Catalog ของ Flow ทั้งหมด การเปลี่ยนที่ทำให้ข้อมูลเก่าอ่านไม่ได้ต้องเพิ่ม Major Version และอัปเดต Schema พร้อมคู่มือย้ายข้อมูล

## Accessibility และข้อจำกัด

- Semantic landmarks และ Heading ตามลำดับ
- Keyboard focus ชัดเจนและ Touch Target อย่างน้อย 44px
- SVG ที่มีความหมายมีชื่อและคำบรรยาย; ภาพตกแต่งซ่อนจาก Screen Reader
- สีไม่ใช่ตัวบอกสถานะเพียงอย่างเดียว
- รองรับ Reduced Motion และ Print
- ไม่มี External Fonts, CDN, Analytics หรือ Network Request
- การอ่าน JSON ต้องเปิดผ่าน HTTP Local Server เพราะ Browser จำกัด `fetch` บน `file://`
