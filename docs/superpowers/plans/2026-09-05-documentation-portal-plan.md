# tan-erp Documentation Portal Implementation Plan

> **For agentic workers:** Execute the documentation tasks in order and verify each deliverable before marking it complete.

**Goal:** สร้าง Documentation Foundation และ JSON-driven HTML Portal โดยไม่สร้าง ERP Application Code

**Architecture:** Markdown เป็น Source of Truth แยกตาม Business, Architecture, Contract, Data, Engineering และ Operations ส่วน Static Portal อ่าน Catalog และ Flow จาก JSON แล้วแสดง Diagram, Text Alternative และลิงก์กลับสู่เอกสาร

**Tech Stack:** Markdown, HTML5, CSS, Vanilla JavaScript, JSON, JSON Schema และ Local SVG

## Global Constraints

- ระบบคือ Project ERP; MRP เป็น Future Module
- ภาษาไทยเป็นค่าเริ่มต้นและรองรับอังกฤษ
- Portal ไม่มี Runtime Dependency หรือ External Network
- JSON เป็นเจ้าของ Node/Edge; HTML ไม่ Hard-code Business Flow
- ทุก Flow มี Text Alternative และใช้งานด้วย Keyboard ได้
- ไม่มี Frontend/Backend ERP, Database, Container หรือ Cloud Resource ในแผนนี้

## Tasks

- [x] สร้าง Governance, Glossary และ Documentation Index
- [x] สร้าง Business, Architecture และ Contract Documents
- [x] สร้าง Data, Engineering และ Operations Documents
- [x] บันทึก ADR และ Templates
- [x] สร้าง JSON Catalog, Flow Files และ JSON Schema
- [x] สร้าง Static Portal และ Local SVG Illustrations
- [x] ตรวจ JSON, Links, Accessibility, Responsive และ Print
- [x] อัปเดต Checklist และ Commit Documentation Foundation
