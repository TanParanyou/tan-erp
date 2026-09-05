# Approval Matrix (ตารางอนุมัติ)

**สถานะ:** Draft — โครงสร้างพร้อมใช้ แต่ยังไม่มีวงเงินจริงจากลูกค้า

## รูปแบบที่แนะนำ

การอนุมัติพิจารณาจากหลายเงื่อนไขร่วมกัน ไม่ผูกกับยอดรวมเพียงอย่างเดียว:

| เงื่อนไข | ตัวอย่าง Trigger | ผลลัพธ์ |
| --- | --- | --- |
| มูลค่ารวม | เกินวงเงินของผู้จัดทำ | ส่งผู้อนุมัติระดับถัดไป |
| Margin | ต่ำกว่าเกณฑ์องค์กร | บังคับเหตุผลและอนุมัติพิเศษ |
| Discount | สูงกว่าเพดาน | ส่ง Sales Manager |
| Manual Override | แก้ต้นทุน/ราคาเอง | บันทึกเหตุผลและตรวจทาน |
| Stale Cost | ราคาต้นทุนหมดอายุ | ตรวจราคาก่อนส่งอนุมัติ |
| Sensitive Project | โครงการถูกกำหนดความเสี่ยงสูง | เพิ่มผู้อนุมัติหรือเอกสาร |

## Quick Estimate Share Policy

Quick Estimate ใช้การตรวจภายในก่อนแชร์เมื่อเข้าเงื่อนไขต่อไปนี้อย่างน้อยหนึ่งข้อ โดยค่าจริงต้องกำหนดแยกตาม Organization/Branch และยืนยันกับเจ้าของกระบวนการ:

| เงื่อนไข | ตัวอย่าง Trigger | ผลลัพธ์ |
| --- | --- | --- |
| Wide Price Range | ช่วงราคากว้างเกินเกณฑ์ของ Work Type | ส่ง Reviewer ตรวจ Assumptions และ Range |
| High Value | Upper Bound เกินวงเงินของผู้จัดทำ | ส่งผู้ตรวจตามระดับวงเงิน |
| Custom Material | ใช้วัสดุเฉพาะที่ไม่มี Reference Rate มาตรฐาน | บังคับหลักฐานราคาและตรวจภายใน |
| Manual Override | แก้ Rate, Factor หรือผลคำนวณเอง | บันทึกเหตุผลและค่าก่อน/หลัง |
| Stale Template | Template พ้น Effective Period หรือถูกแทนที่ | ห้ามแชร์และให้ Recalculate |
| Missing Evidence | รูปหรือข้อมูลหน้างานไม่ครบตาม Checklist | เก็บ Draft หรือส่งกลับให้เติมข้อมูล |

การมี `quick-estimates.share` ไม่ได้ข้าม Share Policy และเมื่อ Policy บังคับ Maker–Checker ผู้จัดทำต้องไม่เป็นผู้ Review รายการของตนเอง

ระบบต้องคืนผล Share Decision หนึ่งค่าเสมอ:

| ผล | ตัวอย่าง | การดำเนินการ |
| --- | --- | --- |
| `Blocked` | Required Evidence ไม่ครบ, Unit/Rate ผิด, Template ใช้ไม่ได้ หรือไม่มีสิทธิ์ | เก็บ Draft ได้ แต่ห้าม Share/Convert |
| `PendingReview` | Custom/Provisional Rate, Override, เกิน Authority หรือ Template อยู่ใน Calibration | ส่ง Reviewer ที่ไม่ใช่ผู้จัดทำ |
| `Shareable` | Active Template, ข้อมูลครบ, อยู่ใน Direct-share Limit และไม่มี Risk Trigger | สร้าง Preliminary Summary ได้ |

Template ที่อยู่ใน `Calibration` ต้อง Review ทุกการแชร์ ส่วน Template ที่ `Active` ใช้ Risk-based Review เพื่อให้งานทั่วไปเร็วและงานเสี่ยงยังถูกควบคุม

## Decision Record ขั้นต่ำ

- ผู้ขออนุมัติและเวลาส่ง
- ผู้ตัดสินใจและเวลา
- ผล `Approved`, `Returned` หรือ `Rejected`
- เหตุผลและ Comment
- Estimate Revision และค่าที่ใช้ตัดสินใจ ณ ขณะนั้น

## หลัก Maker–Checker

ผู้สร้าง/แก้ไข Estimate ต้องไม่เป็นผู้อนุมัติสุดท้ายในรายการที่ถูกกำหนดให้แยกหน้าที่ ระบบต้องบังคับกฎนี้ที่ Backend
