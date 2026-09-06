# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Stack

Documentation Foundation ใช้ Markdown, JSON, semantic HTML, CSS, JavaScript และ local SVG แบบไม่มี Dependency ภายนอก ส่วน Application ที่วางทิศทางไว้ใช้ .NET, EF Core และ Firebase Identity โดยยังไม่เริ่ม Implementation

## Users

- Field Estimator ใช้โทรศัพท์ที่หน้างานเพื่อเก็บข้อมูลและแจ้งช่วงราคาเบื้องต้นอย่างรวดเร็ว
- Pricing Template Owner/Approver ดูแลสูตร ราคาอ้างอิง และการอนุมัติ Version
- Estimator/Approver จัดทำประมาณการทางการและควบคุมความเสี่ยง
- ผู้บริหารและ Auditor ต้องตรวจสอบที่มา การอนุมัติ และประวัติย้อนหลังได้

## Product Purpose

tan-erp เป็น Project ERP สำหรับธุรกิจออกแบบ ตกแต่งภายใน และผลิตงานบิวต์อิน ช่วยเชื่อมข้อมูลจากความต้องการลูกค้า การสำรวจ ประเมินราคา อนุมัติ และใบเสนอราคา ไปยังโครงการและกระบวนการหลังการขายโดยไม่คีย์ซ้ำ

ความสำเร็จหมายถึงทำราคาได้เร็วขึ้น ใช้มาตรฐานเดียวกัน ตรวจสอบย้อนกลับได้ และไม่มีการเข้าถึงข้อมูลผิดขอบเขต

## Positioning

เริ่มจาก Official Estimate ที่แตก BOQ/ต้นทุน ตรวจ Margin และอนุมัติเป็น Revision ก่อนออก Quotation ส่วน Quick Estimate เป็น Optional Module ที่ต่อเพิ่มภายหลังได้

## Operating Context

- ใช้งานระหว่างสำรวจบ้าน คอนโด และสำนักงาน รวมถึงพื้นที่ที่สัญญาณเครือข่ายไม่เสถียร
- งานเริ่มต้นคือ Built-in, ผ้าม่าน และ Wallpaper
- ผู้ใช้ต้องวัดพื้นที่ เลือกวัสดุ ตอบ Checklist ความซับซ้อน ถ่ายรูป และอธิบาย Assumption/Exclusion
- Official Estimate สร้างตรงจาก Customer/Opportunity/Site Survey ได้โดยไม่ต้องผ่าน Quick Estimate
- Quick Estimate เป็น Optional Source และไม่ใช่ Quotation หรือ Approved Price

## Capabilities and Constraints

- ไทยเป็นภาษาหลักและรองรับอังกฤษ
- รองรับหลาย Organization/Branch และ RBAC ตาม Resource Scope
- Firebase ให้ Identity เท่านั้น; PostgreSQL เป็นเจ้าของ Membership, Permission และ Scope
- Backend เป็นเจ้าของ Calculation, State Transition, Share Policy และ Maker–Checker
- Pricing Template, Rate และ Calculation Snapshot มี Version และ Effective Period
- Release แรกไม่ใช่ Offline-first; Offline เก็บ Draft ชั่วคราวได้แต่ Calculate/Share ต้องผ่าน Server
- MRP เป็นโมดูลอนาคตภายใน ERP ไม่ใช่ชื่อผลิตภัณฑ์
- ราคา, Threshold, VAT และ Authority จริงยังต้องยืนยันจาก Pilot/Business Workshop

## Brand Commitments

- ชื่อผลิตภัณฑ์คือ Project ERP หรือ tan-erp
- น้ำเสียงตรงไปตรงมา เป็นมืออาชีพ และอธิบายศัพท์อังกฤษด้วยภาษาไทยสั้น ๆ
- ระบบภาพปัจจุบันใช้ Atelier Architectural Navy Sharp และโครงสร้างมุมฉากตาม `design.md`

## Evidence on Hand

- Product Vision และ Scope อยู่ใน `docs/00-overview/`
- ภาษาธุรกิจอยู่ใน `CONTEXT.md`
- Quick Estimate Flow, Pricing Rules, Template Catalog และ Governance อยู่ใน `docs/01-business/`
- ยังไม่มีข้อมูลราคาจริง ผล Pilot, Testimonial หรือ Benchmark ที่อนุญาตให้นำมาอ้างเป็นข้อเท็จจริง

## Product Principles

1. เร็วที่หน้างาน แต่ไม่ข้ามการควบคุมราคา
2. Configurable by Version ไม่ Hard-code กฎธุรกิจ
3. Backend ตรวจสิทธิ์ Scope และ State ทุกครั้ง
4. ข้อมูลที่เผยแพร่แล้วทำซ้ำและตรวจย้อนกลับได้
5. เริ่มจาก Estimation Foundation และต่อยอด ERP โดยไม่สร้าง MRP ก่อนข้อมูลพื้นฐานพร้อม

## Accessibility & Inclusion

- Touch Target ขั้นต่ำ 44px
- รองรับ Keyboard, Screen Reader, Visible Focus และ Reduced Motion
- Error ต้องชี้จุดแก้และใช้ภาษาที่ผู้ใช้เลือก โดยมีภาษาไทยเป็นค่าเริ่มต้น
