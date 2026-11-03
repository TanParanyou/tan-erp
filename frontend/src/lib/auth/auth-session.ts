import type { QueryClient } from "@tanstack/react-query";
import type { components } from "@/generated/api/tan-erp.v1";
import { ApiError, AuthenticationRequiredError } from "@/lib/api/api-error";
import type { ProblemDetails } from "@/lib/api/problem-details";
import { clearAllDrafts } from "@/lib/drafts/draft-store";

export type AuthResultResponse = components["schemas"]["AuthResultResponse"];

export interface AuthSessionUser {
  uid: string;
  id: string;
  email: string;
  displayName: string;
}

export type AuthFlowOutcome =
  | {
      status: "authenticated";
      user: AuthSessionUser;
      accessToken: string;
      backupCodes?: string[] | null;
    }
  | {
      status: "requires_two_factor";
      challengeToken: string;
    }
  | {
      status: "requires_two_factor_setup";
      challengeToken: string;
      totpSetup: { secretKey: string; otpAuthUri: string };
    };

// In-Memory Defense-in-Depth Session Store
let accessToken: string | null = null;
let expiresAtMs = 0;
let currentUser: AuthSessionUser | null | undefined = undefined;
let refreshPromise: Promise<string | null> | null = null;
let previousUid: string | null = null;

const subscribers = new Set<(user: AuthSessionUser | null) => void>();
const queryClients = new Set<QueryClient>();

// Multi-Tab Sync via BroadcastChannel
let authChannel: BroadcastChannel | null = null;
if (typeof window !== "undefined" && typeof BroadcastChannel !== "undefined") {
  try {
    authChannel = new BroadcastChannel("tan_erp_auth");
    authChannel.onmessage = (event) => {
      if (event.data === "signed_out") {
        accessToken = null;
        expiresAtMs = 0;
        currentUser = null;
        for (const client of queryClients) {
          client.clear();
        }
        notifySubscribers(null);
      } else if (event.data === "signed_in") {
        void refreshAccessToken();
      }
    };
  } catch {
    // Channel initialization ignored in restricted environments
  }
}

function broadcastAuthEvent(event: "signed_in" | "signed_out"): void {
  if (authChannel) {
    try {
      authChannel.postMessage(event);
    } catch {
      // Ignore broadcast error
    }
  }
}

function notifySubscribers(user: AuthSessionUser | null): void {
  if (previousUid && (!user || user.uid !== previousUid)) {
    for (const client of queryClients) {
      client.clear();
    }
  }
  previousUid = user?.uid ?? null;

  for (const callback of subscribers) {
    try {
      callback(user);
    } catch {
      // Ignore individual subscriber callback errors
    }
  }
}

function getApiBaseUrl(): string {
  return (process.env.NEXT_PUBLIC_API_BASE_URL || "").replace(/\/$/, "");
}

async function handleApiError(response: Response): Promise<never> {
  let problem: ProblemDetails | null = null;
  try {
    problem = (await response.json()) as ProblemDetails;
  } catch {
    // Response body not JSON
  }
  if (problem && (problem.code || problem.status || problem.title)) {
    throw ApiError.fromProblemDetails(response.status, problem);
  }
  throw ApiError.fromUnknown(response.status);
}

function processAuthResult(data: AuthResultResponse): AuthFlowOutcome {
  if (data.status === "authenticated" && data.accessToken) {
    accessToken = data.accessToken;
    expiresAtMs = data.expiresAtUtc
      ? new Date(data.expiresAtUtc).getTime()
      : Date.now() + 15 * 60 * 1000;
    currentUser = {
      uid: data.user?.id ?? data.user?.uid ?? "",
      id: data.user?.id ?? "",
      email: data.user?.email ?? "",
      displayName: data.user?.displayName ?? "",
    };
    notifySubscribers(currentUser);
    broadcastAuthEvent("signed_in");
    return {
      status: "authenticated",
      user: currentUser,
      accessToken,
      backupCodes: data.backupCodes,
    };
  }

  if (data.status === "requires_two_factor" && data.challengeToken) {
    return {
      status: "requires_two_factor",
      challengeToken: data.challengeToken,
    };
  }

  if (
    data.status === "requires_two_factor_setup" &&
    data.challengeToken &&
    data.totpSetup
  ) {
    return {
      status: "requires_two_factor_setup",
      challengeToken: data.challengeToken,
      totpSetup: {
        secretKey: data.totpSetup.secretKey ?? "",
        otpAuthUri: data.totpSetup.otpAuthUri ?? "",
      },
    };
  }

  throw ApiError.fromUnknown(500);
}

export async function refreshAccessToken(): Promise<string | null> {
  if (refreshPromise) {
    return refreshPromise;
  }

  refreshPromise = (async () => {
    try {
      const response = await fetch(`${getApiBaseUrl()}/api/v1/auth/refresh`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ clientPlatform: "web" }),
        credentials: "include",
      });

      if (!response.ok) {
        accessToken = null;
        expiresAtMs = 0;
        currentUser = null;
        notifySubscribers(null);
        return null;
      }

      const data = (await response.json()) as AuthResultResponse;
      if (data.status === "authenticated" && data.accessToken) {
        accessToken = data.accessToken;
        expiresAtMs = data.expiresAtUtc
          ? new Date(data.expiresAtUtc).getTime()
          : Date.now() + 15 * 60 * 1000;
        currentUser = {
          uid: data.user?.id ?? data.user?.uid ?? "",
          id: data.user?.id ?? "",
          email: data.user?.email ?? "",
          displayName: data.user?.displayName ?? "",
        };
        notifySubscribers(currentUser);
        return accessToken;
      }

      accessToken = null;
      expiresAtMs = 0;
      currentUser = null;
      notifySubscribers(null);
      return null;
    } catch {
      accessToken = null;
      expiresAtMs = 0;
      currentUser = null;
      notifySubscribers(null);
      return null;
    } finally {
      refreshPromise = null;
    }
  })();

  return refreshPromise;
}

export async function getAuthToken(forceRefresh = false): Promise<string | null> {
  if (accessToken && !forceRefresh && expiresAtMs - Date.now() > 30_000) {
    return accessToken;
  }

  if (currentUser === null && !forceRefresh) {
    return null;
  }

  return refreshAccessToken();
}

export function subscribeToAuthChanges(
  client: QueryClient,
  callback: (user: AuthSessionUser | null) => void
): () => void {
  subscribers.add(callback);
  queryClients.add(client);

  if (currentUser === undefined) {
    void refreshAccessToken();
  } else {
    const immediateUser = currentUser;
    Promise.resolve().then(() => {
      if (subscribers.has(callback)) {
        callback(immediateUser);
      }
    });
  }

  return () => {
    subscribers.delete(callback);
    if (subscribers.size === 0) {
      queryClients.clear();
    }
  };
}

export async function signInWithEmailFlow(
  email: string,
  password: string
): Promise<AuthFlowOutcome> {
  const res = await fetch(`${getApiBaseUrl()}/api/v1/auth/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      email: email.trim(),
      password,
      clientPlatform: "web",
    }),
    credentials: "include",
  });

  if (!res.ok) {
    await handleApiError(res);
  }

  const data = (await res.json()) as AuthResultResponse;
  return processAuthResult(data);
}

export async function signInWithGoogleFlow(idToken: string): Promise<AuthFlowOutcome> {
  const res = await fetch(`${getApiBaseUrl()}/api/v1/auth/google`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      idToken,
      clientPlatform: "web",
    }),
    credentials: "include",
  });

  if (!res.ok) {
    await handleApiError(res);
  }

  const data = (await res.json()) as AuthResultResponse;
  return processAuthResult(data);
}

export async function verifyTwoFactorLoginFlow(
  challengeToken: string,
  code: string
): Promise<AuthFlowOutcome> {
  const res = await fetch(`${getApiBaseUrl()}/api/v1/auth/2fa/verify-login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      challengeToken,
      code: code.trim(),
      clientPlatform: "web",
    }),
    credentials: "include",
  });

  if (!res.ok) {
    await handleApiError(res);
  }

  const data = (await res.json()) as AuthResultResponse;
  return processAuthResult(data);
}

export async function signInWithEmail(
  email: string,
  password: string
): Promise<AuthSessionUser> {
  const outcome = await signInWithEmailFlow(email, password);
  if (outcome.status !== "authenticated") {
    throw new Error(`Authentication requires further steps: ${outcome.status}`);
  }
  return outcome.user;
}

export async function signOutSession(client: QueryClient): Promise<void> {
  await client.cancelQueries();
  client.clear();

  try {
    await clearAllDrafts();
  } catch {
    // Local drafts are a best-effort cache; never block sign-out on storage errors
  }

  try {
    await fetch(`${getApiBaseUrl()}/api/v1/auth/logout`, {
      method: "POST",
      credentials: "include",
    });
  } catch {
    // Ignore network error on logout
  }

  accessToken = null;
  expiresAtMs = 0;
  currentUser = null;
  broadcastAuthEvent("signed_out");
  notifySubscribers(null);
}

export async function changeOwnPassword(
  currentPassword: string | null,
  newPassword: string
): Promise<void> {
  const token = await getAuthToken();
  if (!token) {
    throw new AuthenticationRequiredError();
  }

  const res = await fetch(`${getApiBaseUrl()}/api/v1/me/password`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      currentPassword: currentPassword || null,
      newPassword,
    }),
  });

  if (!res.ok) {
    await handleApiError(res);
  }
}

export function __resetAuthSessionForTesting(): void {
  accessToken = null;
  expiresAtMs = 0;
  currentUser = undefined;
  refreshPromise = null;
  previousUid = null;
  subscribers.clear();
  queryClients.clear();
}
