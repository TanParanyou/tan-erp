# ADR 0032: Self-Hosted Authentication, RFC 6238 TOTP 2FA, and Rotating Session Hardening

- **Status:** Accepted
- **Date:** 2026-11-01
- **Deciders:** Product Owner, Architecture Team

## Context

Previously, **tan-erp** delegated primary user authentication to **Firebase Authentication** (`FirebaseAdmin` on `.NET`, `firebase/auth` on Next.js, and `firebase_auth` on Flutter). While Firebase Spark provides free email/password and Google login up to a monthly active user threshold, enabling **Multi-Factor Authentication (2FA)** introduces significant constraints:
1. SMS-based MFA requires upgrading to the paid Blaze plan and incurs per-SMS telecom charges.
2. TOTP-based MFA requires upgrading to Google Cloud Identity Platform (GCIP) with billing enabled.
3. Relying on external third-party token lifetimes makes immediate per-device session listing, single-session revocation, token-reuse detection, and strict enterprise audit trails harder to control directly within PostgreSQL.

Following the proven **Defense-in-Depth** security model established in `WAT-PROFILE`, **tan-erp** requires a 100% free, zero-vendor-lock-in, enterprise-grade authentication and session management architecture supporting:
- Invite-only user provisioning by Administrators (initial/temporary password or Google OAuth email linking — no public self-registration).
- Email/Password authentication and Google OAuth 2.0 Sign-In.
- Free **RFC 6238 TOTP Two-Factor Authentication** (Google Authenticator, Microsoft Authenticator, Apple Passwords) with single-use hashed Emergency Backup Codes, optional for all staff and enforceable per user/Admin by Administrators.
- Full **Active Session Management** with rotating refresh tokens, reuse detection, real-time revocation, and inactivity auto-logout across Web and Mobile.

## Decision

1. **Remove Firebase Authentication Dependencies:**
   - Remove `FirebaseAdmin` from Backend, `firebase` from Web Frontend, and `firebase_auth` from Mobile Flutter.
   - Retain `User.FirebaseUid` column/property semantics internally as the immutable subject identifier (`usr_<guid>` for newly provisioned users, preserving existing seeded identifiers like `foundation-user-test-only`) to maintain a minimal blast radius across existing CQRS handlers and integration tests.

2. **Self-Hosted Cryptography (Pure .NET BCL):**
   - **Password Hashing:** `PBKDF2-HMAC-SHA512` (`Rfc2898DeriveBytes.Pbkdf2`, ≥ 210,000 iterations, 16-byte CSPRNG salt, 32-byte key, constant-time comparison).
   - **Google OAuth Verification:** Verify Google OIDC ID Tokens against Google's public JWKS (`https://www.googleapis.com/oauth2/v3/certs`), enforcing `email_verified == true` and linking only pre-invited active users in `users`.
   - **2FA TOTP & Backup Codes:** Standard `RFC 6238` (`HMAC-SHA1`, 6 digits, 30-second step, ±1 step clock-skew tolerance) with TOTP secrets encrypted at rest using `AES-256-GCM`, paired with 8 single-use `SHA-256` hashed recovery backup codes.

3. **Session Hardening & Token Lifecycle:**
   - **Access Tokens:** Short-lived (15-minute) `HMAC-SHA256` JWTs with `aud: "tan-erp-api"` and `sid` (Session ID) claim. Kept strictly **in memory** on Web and Mobile; never persisted to `localStorage` or `sessionStorage`.
   - **2FA Challenge / Setup Tokens:** Distinct short-lived (5–10 minute) JWTs with `aud: "tan-erp-2fa-challenge"` / `"tan-erp-2fa-setup"`, rejected by API authentication middleware.
   - **Rotating Refresh Tokens & Reuse Detection:** Stored in PostgreSQL (`user_sessions`) as `SHA-256` hashes (`CurrentRefreshTokenHash` + `PreviousRefreshTokenHash` with a 15-second multi-tab grace window). Delivered via `HttpOnly` cookie (`tan_erp_refresh`, `Path=/api/v1/auth`) for Web and encrypted `FlutterSecureStorage` payload for Mobile. Replaying an invalidated prior token outside the grace window triggers immediate revocation of the compromised session.
   - **Real-Time Session Validation:** API authentication validates `sid` against active, non-revoked `user_sessions` rows, guaranteeing instant logout upon session revocation, password change, or user deactivation.

## Consequences

- **Positive:** 100% free operation without credit-card requirements or SMS fees; complete data sovereignty in PostgreSQL; immediate per-device session revocation; simpler local development (`make dev` no longer requires Firebase Auth Emulator).
- **Negative / Responsibilities:** Backend owns password hashing, secret encryption key management (`Authentication:JwtSigningKey` and `Authentication:EncryptionKey` required in Production), and login rate limiting.
