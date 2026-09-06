---
status: accepted
---

# Foundation Application Runtime (.NET 10 LTS, Node 24 LTS, PostgreSQL 17)

## บริบทและการตัดสินใจ (Context and Decisions)

การเริ่มต้น Application Implementation ของ Project ERP (`tan-erp`) จำเป็นต้องตรึงเวอร์ชันของ Runtime, SDK, Framework และ Database ให้สอดคล้องกันทั้ง Local Development, CI และ Production เพื่อป้องกัน Drift ของ Dependency และความเสี่ยงจาก Breaking Changes โดยกำหนดรุ่นทางการดังนี้:

- **Backend SDK:** .NET SDK 10.0.400
- **Backend target:** `net10.0`
- **Runtime/ASP.NET Core/EF Core patch line:** 10.0.11
- **Frontend runtime:** Node.js 24.20.0 LTS
- **Framework:** Next.js 16.3.4 + React 19.2.8
- **Database:** PostgreSQL 17
- **Identity adapters:** Firebase Admin .NET 3.6.0 และ Firebase JS 12.18.0

## เหตุผลและการปฏิเสธทางเลือกอื่น (Rationale and Alternatives Considered)

1. **การปฏิเสธ Node.js 26 Current:** แม้จะมี Node 26 แต่เป็น Current release ซึ่งมีรอบการสนับสนุนสั้นและอาจมีการเปลี่ยนแปลงใน tooling/ecosystem การเลือกระบบ ERP ระดับองค์กรจำเป็นต้องใช้ Active LTS จึงกำหนด Node.js 24.20.0 LTS เป็นมาตรฐาน
2. **การปฏิเสธ .NET 11 Preview:** .NET 11 อยู่ในสถานะ Preview ยังไม่มีเสถียรภาพหรือ SLA ทางการสำหรับ Production จึงเลือก .NET 10 LTS (SDK 10.0.400, net10.0, runtime patch 10.0.11) ซึ่งเป็น Long Term Support ที่มีเสถียรภาพและรอบการสนับสนุนระยะยาว
3. **Database:** PostgreSQL 17 เป็นเวอร์ชันเสถียรหลักที่มีประสิทธิภาพ Query Planner และ JSONB ที่พร้อมสำหรับ Identity/RBAC, Membership และ Audit Event
4. **Identity Adapters:** Firebase Admin .NET 3.6.0 ทำหน้าที่ Verify Identity Token ฝั่ง Backend โดยไม่ดึงสิทธิ์ธุรกิจ ส่วน Firebase JS 12.18.0 ทำหน้าที่ Session Management ฝั่ง Browser

## แหล่งอ้างอิงทางการ (Citations)

- .NET Download and Support Policy (.NET 10 LTS): https://dotnet.microsoft.com/download/dotnet
- Node.js Releases (Node.js 24 LTS): https://nodejs.org/en/about/previous-releases
- Next.js Releases: https://github.com/vercel/next.js/releases
- React Version Documentation: https://react.dev/blog
- Firebase Release Notes: https://firebase.google.com/support/release-notes
