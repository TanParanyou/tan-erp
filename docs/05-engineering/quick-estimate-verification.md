# Quick Estimate Verification (CP-16)

**สถานะ:** Implemented 2026-10-04; หลักฐานเป็นชุดทดสอบเฉพาะส่วน ไม่ใช่ UAT กับ Sales/Estimator หรือ Cost Owner. กฎ: [Quick Estimate API Contract](../03-contracts/quick-estimate-api-contract.md), [Pricing Rules](../01-business/quick-estimate-pricing-rules.md).

## ผลที่รันจริง

- Unit `QuickEstimateEngineTests` 13/13: ปริมาณคิดเงินตามกฎ (พื้นที่ กว้าง×สูง×จำนวน, ความยาว, ปริมาตร, จำนวนชิ้น), ตัวคูณเกรด/ความซับซ้อน, add-on ต่อรายการและต่อเอกสาร, ราคาขั้นต่ำ, ช่วงราคา = ฐาน + ตัวปรับ ไม่เกินค่าสูงสุด, ปัดขอบล่างลง/ขอบบนขึ้นตามขั้นปัดเศษ, การแสดงภาษีแบบรวม/ไม่รวม, ผลการแชร์ (pending_review เมื่อวัสดุพิเศษ/แม่แบบปรับเทียบ/ความมั่นใจต่ำ/ช่วงถึงสูงสุด/เกินเพดาน), input hash คงที่และไม่ขึ้นกับวันที่. ตัวอย่าง: 3.0×2.6 ม. เกรดพรีเมียม ×1.3 อัตรา 8,000 + ค่าขนส่ง 5,000 → ยอดสุทธิ 86,120, ช่วงแสดง 75,000–97,000 (±12%); เกรดมาตรฐาน → 67,400.
- Backend Integration `QuickEstimateEndpointsTests` 4/4 + `QuickEstimate_Conversion_...` ใน `EstimateEndpointsTests` บน PostgreSQL: วงจรชีวิตแม่แบบ (maker–checker ห้ามอนุมัติของตัวเอง), calculate ซ้ำ idempotent/เวอร์ชัน, stale/expired, review (ห้ามตรวจของตัวเอง), share ถูกบล็อกเมื่อยังไม่ผ่าน, conversion replay ไม่สร้าง Estimate ซ้ำ, สิทธิ์ 403, error code ตามสัญญา. ชุด Estimate 44/44 ก่อนเพิ่ม CP-16.
- Frontend Vitest `features/quick-estimates` 12/12 (ช่วงราคา/ผลการแชร์/เหตุผล, ส่งตรวจด้วยเวอร์ชันการคำนวณปัจจุบัน, ต้องมี reason code ก่อนอนุมัติ, ซ่อนปุ่มตามสิทธิ์และล็อกหลังแปลง, error แปลแล้ว, แม่แบบส่งอนุมัติพร้อม row version, อนุมัติ/ส่งกลับเฉพาะผู้อนุมัติ, คำเตือนช่วงปรับเทียบ, ข้อความ th-en ครบ), `tsc --noEmit`, `eslint .` ผ่าน.

- Regression รวมรอบสุดท้าย (หลังเพิ่ม CP-16 ทั้งหมด): Integration ที่กรอง OpenApi + DocumentSequence + QuickEstimate + EstimateEndpointsTests **68/68**, Unit **364/364**, Architecture **3/3**.

## ที่ยังไม่ได้ยืนยัน

ไม่ได้รัน full Integration suite ทั้งโปรเจกต์, `next build`, Playwright (ข้ามตามที่ตกลง).

## ข้อจำกัด

ไม่มี Branch override/promotion/manual override/อัปโหลดหลักฐาน; Share เฉพาะ on-screen; Conversion สร้างร่าง Estimate เปล่า (snapshot เก็บเป็นอ้างอิง) และต้องระบุรหัส Site Survey Revision ด้วยตนเองในหน้าเว็บ; ไม่มี mobile/offline; **ไม่มีค่าตั้งต้นของราคา** ทุกอัตรา/ตัวคูณ/ช่วง/เพดานเป็นข้อมูลที่ธุรกิจต้องกรอกและอนุมัติ; กฎการตัดสินใจแชร์และการปัดเศษเป็นกฎที่ทีมพัฒนาเลือก รอ Sales/Cost Owner ยืนยัน; ต้องมอบสิทธิ์ใหม่ (`pricing-templates.read/manage/approve`, `quick-estimates.create/read/update/review/share/convert`) ให้ Role จริง.
