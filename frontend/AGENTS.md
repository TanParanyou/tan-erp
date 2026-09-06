# Frontend Agent Guidelines (คำแนะนำสำหรับ Agent ฝั่ง Frontend)

Frontend ของ `tan-erp` พัฒนาด้วย Next.js App Router, React 19, TypeScript และ TanStack Query โดยยึดหลักการออกแบบและข้อกำหนดความปลอดภัยดังนี้:

## กฎเหล็กในการพัฒนา (Strict Guardrails)

1. **Generated Contracts จาก OpenAPI:** ห้ามเขียน Type หรือ Interface สำหรับ Request/Response ของ API เองด้วยมือ ให้ใช้ Type จาก `src/generated/api/tan-erp.v1.ts` ซึ่งถูกสร้างจาก OpenAPI ของ Backend เสมอ
2. **Central API Calls เท่านั้น:** ห้ามเรียก `fetch` หรือประกอบ API URL โดยตรงใน React Component หรือ Custom Hook ใดๆ การติดต่อเครือข่ายทั้งหมดต้องผ่าน `ApiClient` ใน `src/lib/api/`
3. **TanStack Query ถือครอง Server State:** ข้อมูลทางธุรกิจและ Server State เป็นกรรมสิทธิ์ของ TanStack Query เท่านั้น ห้ามนำไปเก็บซ้ำซ้อนใน React State
4. **ห้ามใช้ useEffect สำหรับ Derived State (No Effect for Derived State):** ห้ามใช้ `useEffect` เพื่อ Sync State หรือคำนวณค่าจาก State อื่น ให้คำนวณระหว่าง Render หรือใช้ Memoization เท่าที่จำเป็น
5. **Thai/English Key Parity:** ข้อความ UI ทั้งหมดต้องเก็บใน `messages/th.json` และ `messages/en.json` โดยทุก Key ในไฟล์ภาษาไทยต้องมี Key คู่ขนานในไฟล์ภาษาอังกฤษเสมอ (ห้ามมี Key ตกหล่น)
6. **Semantic Controls และ Accessibility:**
   - ใช้ Semantic HTML (`<button>`, `<a>`, `<input>`, `<label>`) เสมอ
   - Target Size ของ Control ที่กดได้ต้องสูงอย่างน้อย 44px
   - มี Visible Keyboard Focus สำหรับผู้ใช้คีย์บอร์ด
   - มีการเชื่อมโยง Error เข้ากับ Input Field ผ่าน `aria-invalid` และ `aria-describedby`
   - สถานะสำคัญและการแจ้งเตือนต้องใช้ `aria-live="polite"`
   - รองรับ `prefers-reduced-motion` ใน CSS
