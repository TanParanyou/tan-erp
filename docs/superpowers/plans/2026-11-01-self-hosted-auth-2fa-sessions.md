# Self-Hosted Authentication, 2FA TOTP & Session Hardening Plan

- **Date:** 2026-11-01
- **ADR:** [ADR 0032](../../adr/0032-self-hosted-authentication-2fa-and-session-hardening.md)
- **Execution Strategy:** Tiered Subagent Orchestration (`/tiered-orchestration`)

## 1. Scope & Business Rules

1. **Zero Firebase Auth Dependency:** ถอด `FirebaseAdmin` (.NET), `firebase` (Next.js) และ `firebase_auth` (Flutter) ออกจากระบบ Login ทั้งหมด
2. **Invite-Only Onboarding:** ไม่มีหน้าสมัครสมาชิกสาธารณะ พนักงานเข้าใช้งานได้เฉพาะบัญชีที่ Admin สร้างไว้ในระบบ โดย Admin ตั้ง/รีเซ็ตรหัสผ่านเริ่มต้นได้ หรือพนักงานกด **Sign in with Google** ด้วยอีเมลที่ตรงกับบัญชีที่ Admin เชิญไว้เพื่อผูกบัญชีอัตโนมัติ
3. **2FA (RFC 6238 TOTP + Backup Codes):**
   - ผู้ใช้ทุกคนสามารถเปิด/ปิด 2FA ผ่านแอป Authenticator (Google/Microsoft Authenticator) และสร้างรหัสสำรองฉุกเฉิน 8 ชุด (Hashed) ได้เองที่หน้าโปรไฟล์ (`Security Tab`)
   - Admin สามารถตั้งค่าบังคับใช้ 2FA (`RequireTwoFactor`) รายบุคคล (เช่น ผู้ดูแลระบบ) รวมถึงรีเซ็ต 2FA กรณีฉุกเฉินได้ หากถูกบังคับใช้ ระบบจะให้ตั้งค่าสแกน QR ตั้งแต่ขั้นตอน Login และไม่อนุญาตให้ผู้ใช้กดปิดเอง
4. **Enterprise Session Management (แนวทางเดียวกับ WAT-PROFILE):**
   - **In-Memory Access Token** อายุสั้น (15 นาที) ห้ามเก็บใน `localStorage` / `sessionStorage`
   - **Rotating Refresh Token** (`HttpOnly` Cookie สำหรับ Web และ `FlutterSecureStorage` สำหรับ Mobile) เก็บเฉพาะ SHA-256 Hash ใน `user_sessions` พร้อม Grace Period 15 วินาทีสำหรับหลายแท็บ และ **Token Reuse Detection** เพิกถอนเซสชันทันทีเมื่อพบการขโมย Token
   - แสดงรายการอุปกรณ์ที่กำลังล็อกอิน (`GET /api/v1/me/sessions`), สั่งเตะออกรายอุปกรณ์ (`DELETE /api/v1/me/sessions/{id}`), สั่งออกจากระบบอุปกรณ์อื่นทั้งหมด (`POST /api/v1/me/sessions/revoke-others`), เพิกถอนอัตโนมัติเมื่อเปลี่ยนรหัสผ่านหรือระงับผู้ใช้ และ **Inactivity Auto-Logout** บนหน้าเว็บ

## 2. Reuse First Inventory

- **Backend:** ต่อยอด `User`, `FirebaseAuthenticationHandler` + `IFirebaseTokenVerifier` seam (ไม่กระทบ Controller/Handler อื่นกว่า 40 โมดูล), `ProblemDetailsMapper` + `Errors.resx`/`Errors.en.resx`, `TestOnlyDataSeeder`
- **Frontend:** นำคอมโพเนนต์กลางที่มีอยู่แล้วมาเชื่อมต่อ API จริงโดยไม่สร้างซ้ำ:
  - `src/lib/auth/auth-session.ts` (คง interface เดิม เพิ่ม Google, 2FA, Single-flight refresh)
  - `src/components/features/security/TwoFactorAuthCard.tsx`
  - `src/components/features/security/ActiveSessionsCard.tsx`
  - `src/components/ui/OtpInput.tsx`, `QRCodeDisplay.tsx`, `ConfirmationModal.tsx`
  - `src/hooks/useInactivityTimeout.ts` + `src/components/common/InactivityTimeoutDialog.tsx`
- **Mobile:** ต่อยอด `AuthRepository`, `SessionController`, `FlutterSecureStorage`, `LoginPage` และชุด `Erp*` UI components

## 3. Tiered Orchestration Phases

1. **Phase 1 — Backend Domain Architect (`Model: "inherit"`, `backend/` only):**
   - Domain (`User`, `UserSession`, `UserBackupCode`), EF Migration, Cryptography (`PBKDF2`, `AES-GCM`, `RFC 6238 TOTP`, `JWT`, `Google OIDC`), `AuthController`, `MeSecurity` & `Sessions` endpoints, Admin User Password/2FA endpoints, Seeders, Unit + Architecture + Integration Tests
2. **Phase 2 — OpenAPI Bridge (Main Orchestrator):**
   - Update `contracts/openapi/tan-erp.v1.json`, `frontend/src/generated/api/tan-erp.v1.ts`, and `mobile/packages/tan_erp_api`
3. **Phase 3 — Parallel Frontend (`Model: "flash"`, `frontend/` only) & Mobile (`Model: "flash"`, `mobile/` only):**
   - Frontend: In-memory session client + HttpOnly refresh, Login + Google + 2FA challenge/mandatory setup, Profile Security Tab (Password + 2FA + Active Sessions), Inactivity Timeout in `ErpShell`, Admin User Password/2FA controls, i18n `th`/`en`, Vitest
   - Mobile: Self-hosted REST `AuthRepository` + `FlutterSecureStorage` rotating refresh + 2FA login step, i18n `app_th.arb`/`app_en.arb`, Flutter tests
4. **Phase 4 — Dev Scripts & Full Verification Gates (Main Orchestrator):**
   - Update `Makefile` dev targets (no Firebase Emulator needed), verify clean `git status`, run full Backend + Frontend + Mobile verification gates.
