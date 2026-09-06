import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { ApiClient } from "./api-client";
import { ApiError } from "./api-error";

describe("ApiClient", () => {
  const originalFetch = global.fetch;

  beforeEach(() => {
    vi.restoreAllMocks();
  });

  afterEach(() => {
    global.fetch = originalFetch;
  });

  it("throws ApiError if token is missing or empty before making network request", async () => {
    const fetchMock = vi.fn();
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");

    await expect(client.getCurrentUser("")).rejects.toThrow(ApiError);
    await expect(client.getCurrentUser("   ")).rejects.toMatchObject({
      status: 401,
      code: "AUTHENTICATION_REQUIRED",
    });

    expect(fetchMock).not.toHaveBeenCalled();
  });

  it("sends GET /api/v1/me with Authorization Bearer and Accept-Language headers", async () => {
    const mockUserResponse = {
      user: {
        id: "019a3cf8-96f0-7c9f-b207-93aa818f4a10",
        displayName: "ผู้ใช้ TEST_ONLY",
        email: "foundation-user@example.test",
      },
      memberships: [
        {
          id: "019a3cf8-96f0-7c9f-b207-93aa818f4a11",
          organization: {
            id: "019a3cf8-96f0-7c9f-b207-93aa818f4a12",
            name: "TEST_ONLY Project ERP",
          },
          branch: {
            id: "019a3cf8-96f0-7c9f-b207-93aa818f4a13",
            name: "สาขาทดสอบ",
          },
          permissions: [
            {
              key: "organizations.read",
              scope: "organization",
              scopeId: "019a3cf8-96f0-7c9f-b207-93aa818f4a12",
            },
          ],
        },
      ],
    };

    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => mockUserResponse,
    });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");
    const result = await client.getCurrentUser("sample-token", "th");

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const [url, init] = fetchMock.mock.calls[0];
    expect(url).toBe("http://localhost:5000/api/v1/me");
    expect(init.method).toBe("GET");
    expect(init.headers["Authorization"]).toBe("Bearer sample-token");
    expect(init.headers["Accept-Language"]).toBe("th");

    expect(result).toEqual(mockUserResponse);
  });

  it("converts RFC 9457 Problem Details into typed ApiError", async () => {
    const problemDetails = {
      type: "https://tan-erp.local/problems/active-membership-required",
      title: "จำเป็นต้องมีสมาชิกภาพที่ใช้งานได้",
      status: 403,
      code: "ACTIVE_MEMBERSHIP_REQUIRED",
      detail: "ยืนยันตัวตนสำเร็จแต่ไม่พบสมาชิกภาพที่ใช้งานอยู่ในองค์กรใด",
      traceId: "trace-12345",
      errors: {
        membership: ["No active membership found"],
      },
    };

    const fetchMock = vi.fn().mockResolvedValue({
      ok: false,
      status: 403,
      json: async () => problemDetails,
    });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");

    try {
      await client.getCurrentUser("sample-token");
      expect.unreachable("Should have thrown ApiError");
    } catch (err) {
      expect(err).toBeInstanceOf(ApiError);
      const apiError = err as ApiError;
      expect(apiError.status).toBe(403);
      expect(apiError.code).toBe("ACTIVE_MEMBERSHIP_REQUIRED");
      expect(apiError.traceId).toBe("trace-12345");
      expect(apiError.errors?.membership).toEqual(["No active membership found"]);
    }
  });

  it("converts non-JSON 500 error into safe UNKNOWN_ERROR without leaking raw response", async () => {
    const fetchMock = vi.fn().mockResolvedValue({
      ok: false,
      status: 500,
      json: async () => {
        throw new Error("SyntaxError: Unexpected token < in JSON");
      },
    });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");

    try {
      await client.getCurrentUser("sample-token");
      expect.unreachable("Should have thrown ApiError");
    } catch (err) {
      expect(err).toBeInstanceOf(ApiError);
      const apiError = err as ApiError;
      expect(apiError.status).toBe(500);
      expect(apiError.code).toBe("UNKNOWN_ERROR");
      expect(apiError.message).not.toContain("SyntaxError");
    }
  });

  it("distinguishes AbortSignal cancellation from server failure", async () => {
    const abortError = new Error("The operation was aborted");
    abortError.name = "AbortError";

    const fetchMock = vi.fn().mockRejectedValue(abortError);
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");
    const controller = new AbortController();
    controller.abort();

    try {
      await client.getCurrentUser("sample-token", "th", controller.signal);
      expect.unreachable("Should have thrown ApiError");
    } catch (err) {
      expect(err).toBeInstanceOf(ApiError);
      const apiError = err as ApiError;
      expect(apiError.isAbort).toBe(true);
      expect(apiError.code).toBe("REQUEST_ABORTED");
    }
  });
});
