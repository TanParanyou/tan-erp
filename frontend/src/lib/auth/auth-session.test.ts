import { describe, it, expect, vi, beforeEach } from "vitest";
import {
  signInWithEmail,
  signOutSession,
  getAuthToken,
  subscribeToAuthChanges,
} from "./auth-session";
import type { QueryClient } from "@tanstack/react-query";
import type { User } from "firebase/auth";

vi.mock("./firebase-client", () => ({
  auth: {
    currentUser: null,
  },
}));

vi.mock("firebase/auth", () => ({
  signInWithEmailAndPassword: vi.fn(),
  signOut: vi.fn(),
  onAuthStateChanged: vi.fn(),
}));

import {
  signInWithEmailAndPassword,
  signOut as firebaseSignOut,
  onAuthStateChanged,
} from "firebase/auth";
import { auth } from "./firebase-client";

describe("auth-session", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    (auth as { currentUser: unknown }).currentUser = null;
  });

  it("trims email and calls signInWithEmailAndPassword", async () => {
    const mockUser = { uid: "user-1", email: "test@example.test" } as User;
    vi.mocked(signInWithEmailAndPassword).mockResolvedValueOnce({
      user: mockUser,
    } as never);

    const user = await signInWithEmail("  user@example.test  ", "secret123");

    expect(signInWithEmailAndPassword).toHaveBeenCalledWith(
      auth,
      "user@example.test",
      "secret123"
    );
    expect(user).toBe(mockUser);
  });

  it("cancels queries, clears query client and signs out of firebase", async () => {
    const mockQueryClient = {
      cancelQueries: vi.fn().mockResolvedValue(undefined),
      clear: vi.fn(),
    } as unknown as QueryClient;

    vi.mocked(firebaseSignOut).mockResolvedValueOnce(undefined);

    await signOutSession(mockQueryClient);

    expect(mockQueryClient.cancelQueries).toHaveBeenCalledTimes(1);
    expect(mockQueryClient.clear).toHaveBeenCalledTimes(1);
    expect(firebaseSignOut).toHaveBeenCalledWith(auth);
  });

  it("returns null if no currentUser in getAuthToken", async () => {
    (auth as { currentUser: unknown }).currentUser = null;

    const token = await getAuthToken();
    expect(token).toBeNull();
  });

  it("returns token from currentUser in getAuthToken", async () => {
    const mockGetIdToken = vi.fn().mockResolvedValue("mock-jwt-token");
    (auth as { currentUser: unknown }).currentUser = {
      getIdToken: mockGetIdToken,
    };

    const token = await getAuthToken(true);
    expect(mockGetIdToken).toHaveBeenCalledWith(true);
    expect(token).toBe("mock-jwt-token");
  });

  it("clears query cache when user changes in subscribeToAuthChanges", () => {
    let authCallback: ((user: User | null) => void) | null = null;
    vi.mocked(onAuthStateChanged).mockImplementationOnce((_auth, cb) => {
      authCallback = cb as (user: User | null) => void;
      return vi.fn();
    });

    const mockQueryClient = {
      clear: vi.fn(),
    } as unknown as QueryClient;

    const outerCallback = vi.fn();
    subscribeToAuthChanges(outerCallback, mockQueryClient);

    expect(authCallback).not.toBeNull();

    // First user login
    const userA = { uid: "user-A" } as User;
    authCallback!(userA);
    expect(outerCallback).toHaveBeenCalledWith(userA);
    expect(mockQueryClient.clear).not.toHaveBeenCalled();

    // User switches to User B -> should clear cache
    const userB = { uid: "user-B" } as User;
    authCallback!(userB);
    expect(outerCallback).toHaveBeenCalledWith(userB);
    expect(mockQueryClient.clear).toHaveBeenCalledTimes(1);

    // User logs out (null) -> should clear cache again
    authCallback!(null);
    expect(outerCallback).toHaveBeenCalledWith(null);
    expect(mockQueryClient.clear).toHaveBeenCalledTimes(2);
  });
});
