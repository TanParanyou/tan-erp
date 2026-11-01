import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import {
  signInWithEmail,
  signInWithEmailFlow,
  signInWithGoogleFlow,
  verifyTwoFactorLoginFlow,
  signOutSession,
  getAuthToken,
  subscribeToAuthChanges,
  changeOwnPassword,
  __resetAuthSessionForTesting,
} from "./auth-session";
import type { QueryClient } from "@tanstack/react-query";
import { ApiError, AuthenticationRequiredError } from "@/lib/api/api-error";

describe("auth-session (Self-Hosted JWT)", () => {
  beforeEach(() => {
    __resetAuthSessionForTesting();
    vi.restoreAllMocks();
  });

  afterEach(() => {
    __resetAuthSessionForTesting();
    vi.restoreAllMocks();
  });

  it("trims email and calls /api/v1/auth/login via signInWithEmailFlow", async () => {
    const mockAuthResult = {
      status: "authenticated",
      accessToken: "mock-jwt-token-123",
      expiresAtUtc: new Date(Date.now() + 3600_000).toISOString(),
      user: {
        id: "00000000-0000-0000-0000-000000000001",
        email: "user@example.test",
        displayName: "Test User",
      },
    };

    const fetchSpy = vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => mockAuthResult,
    } as Response);

    const outcome = await signInWithEmailFlow("  user@example.test  ", "secret123");

    expect(fetchSpy).toHaveBeenCalledWith(
      expect.stringContaining("/api/v1/auth/login"),
      expect.objectContaining({
        method: "POST",
        credentials: "include",
        body: JSON.stringify({
          email: "user@example.test",
          password: "secret123",
          clientPlatform: "web",
        }),
      })
    );

    expect(outcome.status).toBe("authenticated");
    if (outcome.status === "authenticated") {
      expect(outcome.user.email).toBe("user@example.test");
      expect(outcome.accessToken).toBe("mock-jwt-token-123");
    }

    // Token should now be cached in memory
    const token = await getAuthToken();
    expect(token).toBe("mock-jwt-token-123");
  });

  it("signInWithEmail returns the user object on successful authentication", async () => {
    const mockAuthResult = {
      status: "authenticated",
      accessToken: "mock-jwt-token-456",
      expiresAtUtc: new Date(Date.now() + 3600_000).toISOString(),
      user: {
        id: "00000000-0000-0000-0000-000000000002",
        email: "another@example.test",
        displayName: "Another User",
      },
    };

    vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => mockAuthResult,
    } as Response);

    const user = await signInWithEmail("another@example.test", "secret456");
    expect(user.id).toBe("00000000-0000-0000-0000-000000000002");
    expect(user.displayName).toBe("Another User");
  });

  it("signInWithGoogleFlow calls /api/v1/auth/google with idToken", async () => {
    const mockAuthResult = {
      status: "requires_two_factor",
      challengeToken: "chal_google_999",
    };

    const fetchSpy = vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => mockAuthResult,
    } as Response);

    const outcome = await signInWithGoogleFlow("google-id-token-abc");

    expect(fetchSpy).toHaveBeenCalledWith(
      expect.stringContaining("/api/v1/auth/google"),
      expect.objectContaining({
        method: "POST",
        credentials: "include",
        body: JSON.stringify({
          idToken: "google-id-token-abc",
          clientPlatform: "web",
        }),
      })
    );

    expect(outcome.status).toBe("requires_two_factor");
    if (outcome.status === "requires_two_factor") {
      expect(outcome.challengeToken).toBe("chal_google_999");
    }
  });

  it("verifyTwoFactorLoginFlow calls /api/v1/auth/2fa/verify-login", async () => {
    const mockAuthResult = {
      status: "authenticated",
      accessToken: "mock-jwt-after-2fa",
      expiresAtUtc: new Date(Date.now() + 3600_000).toISOString(),
      user: {
        id: "u-2fa",
        email: "2fa@example.test",
        displayName: "2FA User",
      },
    };

    const fetchSpy = vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => mockAuthResult,
    } as Response);

    const outcome = await verifyTwoFactorLoginFlow("chal_123", " 123456 ");

    expect(fetchSpy).toHaveBeenCalledWith(
      expect.stringContaining("/api/v1/auth/2fa/verify-login"),
      expect.objectContaining({
        method: "POST",
        credentials: "include",
        body: JSON.stringify({
          challengeToken: "chal_123",
          code: "123456",
          clientPlatform: "web",
        }),
      })
    );

    expect(outcome.status).toBe("authenticated");
  });

  it("cancels queries, clears query client and calls logout endpoint on signOutSession", async () => {
    const mockQueryClient = {
      cancelQueries: vi.fn().mockResolvedValue(undefined),
      clear: vi.fn(),
    } as unknown as QueryClient;

    const fetchSpy = vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 204,
    } as Response);

    await signOutSession(mockQueryClient);

    expect(mockQueryClient.cancelQueries).toHaveBeenCalledTimes(1);
    expect(mockQueryClient.clear).toHaveBeenCalledTimes(1);
    expect(fetchSpy).toHaveBeenCalledWith(
      expect.stringContaining("/api/v1/auth/logout"),
      expect.objectContaining({
        method: "POST",
        credentials: "include",
      })
    );

    // After sign out, token should be null without calling refresh
    const token = await getAuthToken();
    expect(token).toBeNull();
  });

  it("refreshes token on bootstrap when subscribeToAuthChanges is first mounted", async () => {
    const mockAuthResult = {
      status: "authenticated",
      accessToken: "bootstrapped-token",
      expiresAtUtc: new Date(Date.now() + 3600_000).toISOString(),
      user: {
        id: "u-boot",
        email: "boot@example.test",
        displayName: "Boot User",
      },
    };

    vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => mockAuthResult,
    } as Response);

    const mockQueryClient = {
      clear: vi.fn(),
    } as unknown as QueryClient;

    const subscriberCallback = vi.fn();
    const unsubscribe = subscribeToAuthChanges(mockQueryClient, subscriberCallback);

    await vi.waitFor(() => {
      expect(subscriberCallback).toHaveBeenCalledWith(
        expect.objectContaining({
          id: "u-boot",
          email: "boot@example.test",
        })
      );
    });

    unsubscribe();
  });

  it("changeOwnPassword calls /api/v1/me/password with Authorization header", async () => {
    // Seed authenticated session first
    vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => ({
        status: "authenticated",
        accessToken: "bearer-token-xyz",
        expiresAtUtc: new Date(Date.now() + 3600_000).toISOString(),
        user: { id: "u-pwd", email: "pwd@example.test", displayName: "Pwd User" },
      }),
    } as Response);
    await signInWithEmail("pwd@example.test", "oldpass");

    // Mock change password
    const fetchSpy = vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 204,
    } as Response);

    await changeOwnPassword("oldpass", "newpass123");

    expect(fetchSpy).toHaveBeenCalledWith(
      expect.stringContaining("/api/v1/me/password"),
      expect.objectContaining({
        method: "PUT",
        headers: expect.objectContaining({
          Authorization: "Bearer bearer-token-xyz",
          "Content-Type": "application/json",
        }),
        body: JSON.stringify({
          currentPassword: "oldpass",
          newPassword: "newpass123",
        }),
      })
    );
  });

  it("throws ApiError when /api/v1/me/password returns ProblemDetails", async () => {
    // Seed authenticated session
    vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: true,
      status: 200,
      json: async () => ({
        status: "authenticated",
        accessToken: "bearer-token-xyz",
        expiresAtUtc: new Date(Date.now() + 3600_000).toISOString(),
        user: { id: "u-pwd", email: "pwd@example.test", displayName: "Pwd User" },
      }),
    } as Response);
    await signInWithEmail("pwd@example.test", "oldpass");

    vi.spyOn(globalThis, "fetch").mockResolvedValueOnce({
      ok: false,
      status: 400,
      json: async () => ({
        code: "CURRENT_PASSWORD_INVALID",
        title: "Wrong password",
      }),
    } as Response);

    await expect(changeOwnPassword("wrongoldpass", "newpass123")).rejects.toThrow(ApiError);
  });
});
