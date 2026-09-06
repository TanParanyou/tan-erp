# Database Standards (มาตรฐานฐานข้อมูล)

**สถานะ:** Accepted Direction

ใช้ PostgreSQL เป็น Transactional Database และ EF Core Migrations จัดการ Schema

## Data Rules

- Business table มี `id`, `organization_id` เมื่อเป็นข้อมูลองค์กร และ audit timestamps
- Timestamp เก็บ UTC; แปลง Time Zone ตอนแสดงผล
- Money ใช้ `numeric/decimal` พร้อม Currency
- Quantity/Rate กำหนด Precision ตาม Domain ไม่ใช้ Floating Point
- ชื่อ Table/Column ใช้รูปแบบเดียวทั้งระบบและหลีกเลี่ยงคำสงวน
- Foreign Key, Unique Constraint และ Check Constraint ใช้ป้องกันข้อมูลผิดที่ Database Boundary
- Soft Delete ใช้เฉพาะเมื่อมีความต้องการกู้คืนหรืออ้างอิงประวัติ ไม่ใช้เป็นค่าเริ่มต้นทุกตาราง
- JSONB ใช้กับข้อมูลธุรกิจที่ Schema ยืดหยุ่นและมี Ownership ชัด ไม่ใช้เก็บ Error Translation, Permission หรือโครงสร้างหลักที่ต้อง Join/Constraint

Relational/JSONB Boundary และ Logical Schema ของราคาหน้างานอยู่ที่ [Quick Estimate Data Contract](quick-estimate-data-contract.md)

Relational BOQ, Revision Snapshot และ Approval/Quotation Integrity อยู่ที่ [Official Estimate Data Contract](official-estimate-data-contract.md)

Relational Item/Unit/Cost Record, Import Staging JSONB และ Cost Resolve Index อยู่ที่ [Item Master Data Contract](item-master-data-contract.md)

Precision ของ Quantity, Rate และ Money รวมถึง Required Gate อ้าง [Official Estimate Field Catalog](../01-business/official-estimate-field-catalog.md) และ [Calculation Rules](../01-business/estimation-calculation-rules.md) ห้ามกำหนดค่าคนละชุดซ้ำใน Migration โดยไม่มี Contract Change

## Multi-organization

- Query ข้อมูลธุรกิจต้องมี Organization Scope
- Unique business key ที่ซ้ำข้ามองค์กรได้ต้องรวม `organization_id`
- Cross-organization access tests เป็น Release Gate
- Super-admin access ต้องเป็น Use Case ที่กำหนดและ Audit ได้ ไม่ใช่การข้าม Filter แบบทั่วไป

## Migration

อ่าน [Migration Policy](migration-policy.md) ก่อนเปลี่ยน Schema และ [Backup and Restore](../06-operations/backup-and-restore.md) ก่อน Production Migration
