# tan-erp Backend Architecture Design

## 1. สถานะเอกสาร

- วันที่ตัดสินใจ: 5 กันยายน 2026
- สถานะ: อนุมัติแนวทางแล้ว
- ขอบเขต: Backend ของระบบ Project ERP
- Technology หลัก: ASP.NET Core, Entity Framework Core, Dapper และ PostgreSQL

## 2. เป้าหมาย

Backend ต้องเข้าใจง่ายสำหรับทีม .NET ขนาดเล็กถึงกลาง ดูแลระยะยาวได้ และรองรับการเติบโตจากระบบประเมินราคาไปเป็น Project ERP โดยไม่สร้าง Project หรือ Abstraction มากเกินความจำเป็น

ระบบต้องรองรับ Raw SQL อย่างเป็นทางการ แต่ SQL ต้องอยู่ในขอบเขต Infrastructure ไม่กระจายเข้า Controller, Domain หรือ Application

## 3. รูปแบบที่เลือก

ใช้ **Clean Architecture แบบ 4 Projects ร่วมกับ Feature Folders** และ Deploy เป็น Monolith หนึ่ง Application ในระยะแรก

```text
TanErp.Api
    |
    v
TanErp.Application
    |
    v
TanErp.Domain

TanErp.Infrastructure
    |-- implements Application interfaces
    |-- uses Domain entities
    `-- owns EF Core, Dapper and external integrations
```

ไม่เริ่มด้วยการแยก 4–5 Projects ต่อหนึ่ง ERP Module เพราะทำให้จำนวน Project เพิ่มเร็วและสร้างภาระการค้นหา Build และ Dependency Management ก่อนมีทีมแยกดูแลแต่ละ Module

## 4. Solution Structure

```text
backend/
|-- TanErp.slnx
|-- Directory.Build.props
|-- Directory.Packages.props
|
|-- src/
|   |-- TanErp.Api/
|   |   |-- Controllers/
|   |   |-- Middleware/
|   |   |-- Contracts/
|   |   |-- DependencyInjection/
|   |   `-- Program.cs
|   |
|   |-- TanErp.Application/
|   |   |-- Common/
|   |   |   |-- Abstractions/
|   |   |   |-- Behaviors/
|   |   |   `-- Results/
|   |   |-- ItemMaster/
|   |   |-- Opportunities/
|   |   |-- Estimates/
|   |   `-- Quotations/
|   |
|   |-- TanErp.Domain/
|   |   |-- Common/
|   |   |-- ItemMaster/
|   |   |-- Opportunities/
|   |   |-- Estimates/
|   |   `-- Quotations/
|   |
|   `-- TanErp.Infrastructure/
|       |-- Persistence/
|       |   |-- AppDbContext.cs
|       |   |-- Configurations/
|       |   |-- Migrations/
|       |   `-- Queries/
|       |-- Identity/
|       |-- Files/
|       |-- Notifications/
|       `-- DependencyInjection.cs
|
`-- tests/
    |-- TanErp.UnitTests/
    |-- TanErp.IntegrationTests/
    `-- TanErp.ArchitectureTests/
```

## 5. ความรับผิดชอบแต่ละ Project

### TanErp.Domain

เก็บ Business Model และกฎธุรกิจที่ไม่ขึ้นกับ Framework:

- Entity และ Aggregate
- Value Object
- Domain Event
- Domain Service ที่เป็นกฎธุรกิจแท้
- Business Exception

Domain ห้ามอ้างอิง ASP.NET Core, EF Core, Dapper, Firebase หรือระบบจัดเก็บไฟล์

### TanErp.Application

เก็บ Use Case ของระบบและ Interface ที่ Infrastructure ต้องทำให้:

- Command และ Query
- Handler แยกตาม Use Case
- Input Validation
- Result/DTO ภายใน Application
- Interface เช่น `IApplicationDbContext`, `IItemQueries`, `IIdentityProvider` และ `IFileStorage`

Application อ้างอิงได้เฉพาะ Domain และ .NET Base Libraries

### TanErp.Infrastructure

เก็บรายละเอียดทางเทคนิค:

- EF Core และ PostgreSQL
- Dapper และ Raw SQL
- Database Migration
- Firebase Authentication/Notification Adapter
- File Storage Adapter
- Email และ External API
- Outbox และ Background Processing

### TanErp.Api

เป็น HTTP Host และ Composition Root:

- Controller หรือ Endpoint
- Request/Response Contract
- Authentication/Authorization Middleware
- Problem Details
- Dependency Injection Registration
- OpenAPI

Controller ต้องบาง: รับ HTTP input, สร้าง Command/Query, เรียก Handler และแปลงผลเป็น HTTP response

## 6. Feature Folder Convention

ไฟล์ที่เปลี่ยนพร้อมกันต้องอยู่ใกล้กัน ไม่แยกเป็นโฟลเดอร์รวม `Commands`, `Handlers` หรือ `DTOs` ทั้งระบบ

```text
TanErp.Application/
`-- ItemMaster/
    `-- Items/
        |-- CreateItem/
        |   |-- CreateItemCommand.cs
        |   |-- CreateItemHandler.cs
        |   |-- CreateItemValidator.cs
        |   `-- CreateItemResult.cs
        |-- UpdateItem/
        |-- GetItem/
        `-- SearchItems/
            |-- SearchItemsQuery.cs
            |-- SearchItemsHandler.cs
            |-- ItemListItem.cs
            `-- IItemQueries.cs
```

ไม่สร้าง `ItemService` ที่รวม Create, Update, Search, Import, Cost และ Stock ไว้ใน Class เดียว

## 7. Data Access Strategy

ใช้แนวทาง **CQRS-lite**:

```text
Write path
Controller -> Command Handler -> Domain -> EF Core -> PostgreSQL

Read path
Controller -> Query Handler -> Typed Query Interface
           -> Dapper/Raw SQL -> PostgreSQL
```

### Write Path

ใช้ EF Core สำหรับ:

- Create และ Update Aggregate
- Workflow Transition เช่น Approve, Issue และ Accept
- Transaction ที่ต้องรักษา Business Invariant
- Optimistic Concurrency
- Outbox Event ใน Transaction เดียวกัน

EF Core `DbContext` ทำหน้าที่ Unit of Work จึงไม่สร้าง Generic Unit of Work ซ้ำ

### Read Path

ใช้ EF Core LINQ สำหรับ Query ทั่วไปที่อ่านง่าย และใช้ Dapper/Raw SQL เมื่อ:

- Query มีหลาย Join
- เป็น Dashboard หรือ Report
- ต้องใช้ PostgreSQL-specific feature
- LINQ ที่ได้อ่านหรือวิเคราะห์ Query Plan ยาก
- มีหลักฐานว่าต้องควบคุม SQL หรือ Performance โดยตรง

Dapper ใช้สำหรับ Map ผลลัพธ์ไปยัง Typed DTO เท่านั้น ไม่ส่ง `dynamic` ออกจาก Infrastructure

## 8. Raw SQL Structure

```text
TanErp.Application/
`-- ItemMaster/Items/SearchItems/
    |-- IItemQueries.cs
    `-- ItemListItem.cs

TanErp.Infrastructure/
`-- Persistence/Queries/ItemMaster/
    |-- ItemQueries.cs
    `-- Sql/
        `-- SearchItems.sql
```

Application เห็นเพียง Interface ที่มีความหมายทางธุรกิจ:

```csharp
public interface IItemQueries
{
    Task<PagedResult<ItemListItem>> SearchAsync(
        SearchItemsCriteria criteria,
        CancellationToken cancellationToken);
}
```

ห้ามเปิด Interface กลางแบบ `ISqlExecutor.Execute(string sql)` ให้ Application หรือ Controller เรียก SQL อิสระ

## 9. Raw SQL Rules

- SQL ต้องอยู่ใน Infrastructure เท่านั้น
- ทุก Input ต้องส่งผ่าน Parameter ห้ามต่อ String หรือทำ String Interpolation
- Query ทุกตัวต้องรับ `CancellationToken`
- ต้องกำหนด Command Timeout ที่เหมาะสม
- Query ข้อมูลธุรกิจต้องกรอง `organization_id` อย่างชัดเจน
- EF Core Global Query Filter ไม่มีผลกับ Dapper ดังนั้น Raw SQL ต้องบังคับ Organization Scope ด้วยตัวเอง
- SQL สั้นและอ่านง่ายเก็บเป็น Constant ใน Query Class ได้
- SQL ยาว หลาย Join หรือเป็น Report ให้เก็บไฟล์ `.sql` ใกล้ Query Class
- Result ต้อง Map เป็น Typed DTO
- ห้ามใช้ `SELECT *`
- คอลัมน์ที่คืนต้องระบุชื่อและ Alias ชัดเจน
- Pagination ต้องมีลำดับ `ORDER BY` ที่แน่นอน
- Raw SQL Write ต้องใช้ Transaction เดียวกับ EF Core เมื่ออยู่ใน Use Case เดียวกัน
- Raw SQL ทุกตัวต้องมี Integration Test กับ PostgreSQL จริง
- Query สำคัญต้องตรวจ Query Plan และ Index ก่อน Production

## 10. Repository Policy

ไม่สร้าง `GenericRepository<TEntity>` เป็นค่าเริ่มต้น เพราะ EF Core `DbSet<TEntity>` และ `DbContext` มีความสามารถพื้นฐานนั้นอยู่แล้ว

ใช้ `IApplicationDbContext` สำหรับ Use Case ทั่วไป และสร้าง Repository เฉพาะเมื่อ Aggregate ต้องซ่อน Query หรือ Persistence Behavior ที่ซับซ้อน เช่น:

- โหลด Aggregate พร้อม Child ที่จำเป็นต่อ Business Rule
- บังคับ Lock หรือ Concurrency Pattern เฉพาะ
- Persistence มีความหมายทาง Domain ชัดเจน

Repository ต้องทำงานกับ Aggregate Root ไม่ใช่เปิด `IQueryable` ให้ทุก Layer ใช้

## 11. ตัวอย่าง Item Master Flow

### Create Item

```text
POST /api/items
    -> ItemsController
    -> CreateItemHandler
    -> Item.Create(...)
    -> IApplicationDbContext.Items.Add(...)
    -> SaveChangesAsync(...)
    -> PostgreSQL
```

### Search Items

```text
GET /api/items?keyword=ไม้&page=1
    -> ItemsController
    -> SearchItemsHandler
    -> IItemQueries.SearchAsync(...)
    -> ItemQueries using Dapper
    -> SearchItems.sql
    -> PostgreSQL
```

Item Master เป็นเจ้าของรหัส ชื่อ ประเภท หมวดหมู่ หน่วย และสถานะสินค้า แต่ไม่เป็นเจ้าของ Stock Balance, Supplier Price, Price Book, BOM หรือต้นทุนจริงของโครงการ

## 12. Error Handling

- API ใช้ RFC 9457 Problem Details และ `application/problem+json` เป็น Response มาตรฐาน
- Validation Error ตอบ HTTP 400 พร้อม Field Error
- ไม่พบข้อมูลตอบ HTTP 404
- Revision หรือ Concurrency Conflict ตอบ HTTP 409
- Business Rule ที่ประมวลผลไม่ได้ตอบ HTTP 422
- Permission ไม่ผ่านตอบ HTTP 403
- Error Response ต้องมี Stable Error Code, Localized Message และ Trace ID
- ห้ามส่ง Stack Trace, SQL, Connection String หรือ Database Detail ให้ Client

Raw SQL exception ต้องถูกแปลงที่ Infrastructure/Application boundary และ Log ด้วย Trace ID โดยไม่ Log Parameter ที่เป็นข้อมูลลับ

รูปแบบกลาง:

```json
{
  "type": "https://tan-erp/errors/item-code-already-exists",
  "title": "ไม่สามารถบันทึกสินค้าได้",
  "status": 409,
  "code": "ITEM_CODE_ALREADY_EXISTS",
  "detail": "รหัสสินค้า MAT-001 ถูกใช้งานแล้ว",
  "traceId": "00-a12b...",
  "errors": {
    "code": ["รหัสสินค้านี้ถูกใช้งานแล้ว"]
  }
}
```

Frontend ต้องตัดสินใจจาก HTTP Status และ `code` ห้าม Parse หรือเปรียบเทียบ `title` และ `detail`

### Error Localization

- Frontend ส่งภาษาที่ผู้ใช้เลือกผ่าน `Accept-Language`
- Backend ใช้ `.resx` และ `IStringLocalizer` สำหรับข้อความ Error ที่ผู้ใช้เห็น
- รองรับ `th` และ `en` ใน Release แรก โดยมีภาษาไทยเป็นค่าเริ่มต้น
- `code`, `type`, Permission Key และ Field Name ห้ามเปลี่ยนตามภาษา
- ถ้าไม่มีคำแปล ให้ Fallback เป็นภาษาไทย
- Frontend เป็นเจ้าของข้อความ UI-only เช่น Browser Offline
- Backend เป็นเจ้าของข้อความ Validation, Business Rule, Authorization และ Server Error
- ไม่เก็บ System Error Translation ใน PostgreSQL หรือ JSONB
- ไม่รองรับ Organization-specific Error Override ใน Release แรก
- Error ที่ไม่คาดคิดตอบข้อความกลางที่ปลอดภัยและ Trace ID เท่านั้น

รูปแบบนี้ทำให้ Error ยังถูกสร้างได้เมื่อ PostgreSQL ใช้งานไม่ได้ ลดการ Query/Cache และทำให้ตรวจคำแปลที่ขาดจาก Source Code และ Automated Test ได้

## 13. Authentication และ RBAC

Firebase Authentication ยืนยันว่า User เป็นใครเท่านั้น ส่วนสิทธิ์การใช้งานจริงเป็นข้อมูลของ `tan-erp` ใน PostgreSQL

```text
Firebase ID Token
    -> Backend validates identity
    -> Resolve Internal User and Organization Membership
    -> Evaluate Role, Permission and Resource Scope
    -> Allow or deny operation
```

ใช้ **RBAC with Scope**:

- Role รวม Permission หลายรายการ เช่น Estimator, Approver และ Administrator
- Permission ใช้ชื่อคงที่แบบ `resource.action` เช่น `items.read`, `estimates.create`, `estimates.approve` และ `costs.view`
- Scope จำกัดขอบเขต Organization, Branch, Project หรือ Own Record
- ห้ามตรวจสิทธิ์จากชื่อ Role ใน Controller หรือ Business Logic โดยตรง
- Backend Authorization Policy เป็น Authority ตัวจริง
- Frontend รับ Effective Permissions เพื่อซ่อนหรือ Disable Control สำหรับ UX เท่านั้น
- ทุก Query ทั้ง EF Core และ Raw SQL ต้องบังคับ Organization Scope
- การเข้าถึง Resource ต่าง Organization ตอบ HTTP 404 เพื่อไม่เปิดเผยว่าข้อมูลมีอยู่
- User ที่ยังไม่ Authentication ตอบ 401 พร้อม `AUTHENTICATION_REQUIRED`
- User ที่ Authentication แล้วแต่ไม่มี Permission ตอบ 403 พร้อม `PERMISSION_DENIED`
- การเปลี่ยน Role, Permission, Membership และ Approval Authority ต้องสร้าง Audit Trail
- Workflow สำคัญรองรับ Maker–Checker เช่น ผู้สร้าง Estimate ไม่อนุมัติรายการของตนเองเมื่อ Policy บังคับ

Authorization Error ใช้ Error Localization เดียวกับ Error อื่น แต่ `code` และ Permission Key ต้องคงที่ทุกภาษา

## 14. Transaction และ Connection

- EF Core เป็นเจ้าของ Write Transaction หลัก
- เมื่อ Dapper ต้องทำงานใน Transaction เดียวกับ EF Core ให้ใช้ `DbConnection` และ `DbTransaction` จาก `AppDbContext`
- ห้ามเปิด Connection ใหม่ภายใน Transaction เดียวกันโดยไม่ตั้งใจ
- Query ปกติใช้ Connection Factory ที่ Infrastructure เป็นเจ้าของ
- Application และ Domain ห้ามรู้จัก `NpgsqlConnection`

## 15. Testing Strategy

### Unit Tests

ทดสอบ Domain Rule และ Handler ที่ไม่ต้องใช้ฐานข้อมูล เช่น Item Code, Estimate Revision, Margin และ Approval Policy

### Integration Tests

ใช้ PostgreSQL จริงผ่าน Testcontainers เพื่อทดสอบ:

- EF Core Mapping และ Migration
- Dapper/Raw SQL Mapping
- Organization Isolation
- RBAC Policy และ Resource Scope
- Error Contract และภาษาไทย/อังกฤษ
- Transaction ร่วมระหว่าง EF Core และ Dapper
- Pagination และ Sorting
- Concurrency และ Outbox

ไม่ใช้ EF Core InMemory Provider เป็นหลักฐานว่า PostgreSQL Query ใช้งานได้

### Architecture Tests

ตรวจอัตโนมัติว่า:

- Domain ไม่อ้าง Infrastructure, Application หรือ API
- Application ไม่อ้าง Infrastructure หรือ API
- Controller ไม่เรียก EF Core หรือ Dapper โดยตรง
- Raw SQL อยู่ใน Infrastructure เท่านั้น
- Infrastructure-specific type ไม่รั่วผ่าน Application Interface
- Controller ใช้ Authorization Policy และไม่ตรวจชื่อ Role แบบ Hard-coded

## 16. Dependency Direction

```text
TanErp.Domain
    -> no project dependency

TanErp.Application
    -> TanErp.Domain

TanErp.Infrastructure
    -> TanErp.Application
    -> TanErp.Domain

TanErp.Api
    -> TanErp.Application
    -> TanErp.Infrastructure only at Composition Root
```

## 17. แนวทางการขยาย Module

เริ่มด้วย 4 Projects หลักและแบ่ง Module ด้วย Folder/Namespace ก่อน จะแยก Module เป็น Assembly เพิ่มเมื่อมีหลักฐานอย่างน้อยหนึ่งข้อ:

- Module มีทีมเจ้าของแยก
- ต้อง Deploy หรือ Scale แยก
- Build หรือ Change Frequency ต่างจากระบบหลักชัดเจน
- ต้องมี Security/Compliance Boundary แยก
- Dependency Rule ด้วย Folder/Namespace ไม่เพียงพอแล้ว

การมีชื่อ Module อย่างเดียวไม่ใช่เหตุผลให้สร้าง Project ใหม่

## 18. Non-goals ระยะแรก

- ไม่สร้าง Microservices
- ไม่ใช้ Event Sourcing
- ไม่สร้าง Generic Repository Framework
- ไม่สร้าง Service Base Class
- ไม่ใช้ Raw SQL สำหรับทุก Query
- ไม่สร้าง Stored Procedure เป็น Business Logic หลัก
- ไม่แยก Database ต่อ Module
- ไม่เพิ่ม Message Broker ก่อนมี Use Case ที่ต้องใช้
- ไม่เก็บ System Error Translation ใน JSONB
- ไม่สร้างหน้าจอให้แก้ข้อความ System Error
- ไม่ใช้ Frontend Permission แทน Backend Authorization

## 19. ข้อสรุป

Backend ของ `tan-erp` ใช้ Clean Architecture 4 Projects และ Feature Folders เพื่อให้ทีมเข้าใจง่าย ใช้ EF Core เป็นหลักสำหรับ Write และ Query ทั่วไป และใช้ Dapper/Raw SQL สำหรับ Read Model, Report หรือ Query ที่ต้องควบคุม SQL โดยตรง

การรองรับ Raw SQL เป็นความสามารถที่ออกแบบไว้ตั้งแต่ต้น แต่ต้องเข้าผ่าน Typed Query Interface, อยู่ใน Infrastructure, บังคับ Parameter และ Organization Scope และมี Integration Test กับ PostgreSQL จริงทุกครั้ง

Error ใช้ Problem Details พร้อม Stable Code, ข้อความจาก `.resx` ตาม `Accept-Language` และ Trace ID โดยไม่พึ่ง JSONB ส่วน RBAC ถูกบังคับที่ Backend ด้วย Role, Permission และ Resource Scope ที่เก็บใน PostgreSQL
