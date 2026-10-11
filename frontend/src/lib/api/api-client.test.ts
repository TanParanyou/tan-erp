import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { ApiClient, type ListCustomersParams, type RequestOptions } from "./api-client";
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

  it("sends GET /api/v1/customers with X-Membership-Id and query params", async () => {
    const mockListResponse = {
      items: [],
      nextCursor: null,
    };

    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => mockListResponse,
    });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");
    const params: ListCustomersParams = { search: "บริษัท", status: "draft", limit: 10 };
    const result = await client.listCustomers(
      {
        token: "sample-token",
        membershipId: "mem-123",
        locale: "th",
      },
      params
    );

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const [url, init] = fetchMock.mock.calls[0];
    expect(url).toBe("http://localhost:5000/api/v1/customers?search=%E0%B8%9A%E0%B8%A3%E0%B8%B4%E0%B8%A9%E0%B8%B1%E0%B8%97&status=draft&limit=10");
    expect(init.method).toBe("GET");
    expect(init.headers["Authorization"]).toBe("Bearer sample-token");
    expect(init.headers["X-Membership-Id"]).toBe("mem-123");
    expect(init.headers["Accept-Language"]).toBe("th");

    expect(result).toEqual(mockListResponse);
  });

  it("sends POST /api/v1/customers with Idempotency-Key and payload", async () => {
    const mockCustomer = {
      id: "019a3cf8-96f0-7c9f-b207-93aa818f4a20",
      code: "CUST-0001",
      displayNameTh: "บริษัท ทดสอบ จำกัด",
    };

    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 201,
      json: async () => mockCustomer,
    });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");
    const payload = {
      customerType: "organization",
      displayNameTh: "บริษัท ทดสอบ จำกัด",
      preferredLocale: "th",
      primaryContact: {
        name: "สมศรี ใจดี",
        phone: "0812345678",
      },
    };

    const result = await client.createCustomer(payload, {
      token: "sample-token",
      membershipId: "mem-123",
      idempotencyKey: "idem-key-abc",
      locale: "th",
    });

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const [url, init] = fetchMock.mock.calls[0];
    expect(url).toBe("http://localhost:5000/api/v1/customers");
    expect(init.method).toBe("POST");
    expect(init.headers["Authorization"]).toBe("Bearer sample-token");
    expect(init.headers["X-Membership-Id"]).toBe("mem-123");
    expect(init.headers["Idempotency-Key"]).toBe("idem-key-abc");
    expect(init.headers["Content-Type"]).toBe("application/json");
    expect(JSON.parse(init.body as string)).toEqual(payload);

    expect(result).toEqual(mockCustomer);
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

  it("sends POST /api/v1/customers/:id/activate with quoted If-Match header and idempotency key", async () => {
    const mockCustomer = {
      id: "cust-1",
      status: "active",
      rowVersion: "version-2",
    };

    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => mockCustomer,
    });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");
    const result = await client.activateCustomer("cust-1", {
      token: "sample-token",
      membershipId: "mem-1",
      idempotencyKey: "idemp-key-1234567890",
      ifMatch: "version-1",
    });

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const [url, init] = fetchMock.mock.calls[0];
    expect(url).toBe("http://localhost:5000/api/v1/customers/cust-1/activate");
    expect(init.method).toBe("POST");
    expect(init.headers["Authorization"]).toBe("Bearer sample-token");
    expect(init.headers["X-Membership-Id"]).toBe("mem-1");
    expect(init.headers["Idempotency-Key"]).toBe("idemp-key-1234567890");
    expect(init.headers["If-Match"]).toBe('"version-1"');
    expect(result).toEqual(mockCustomer);
  });

  it("sends POST /api/v1/customers/:customerId/sites and encodes customerId properly", async () => {
    const mockSite = {
      id: "site-1",
      code: "SITE-01",
      label: "บ้านพัก",
    };

    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 201,
      json: async () => mockSite,
    });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");
    const payload = {
      label: "บ้านพัก",
      addressLine1: "123",
      subdistrict: "ต",
      district: "อ",
      province: "จ",
      postalCode: "10000",
      countryCode: "TH",
    };

    const result = await client.createSite("cust-123", payload, {
      token: "sample-token",
      membershipId: "mem-1",
      idempotencyKey: "site-key-1234567890",
    });

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const [url, init] = fetchMock.mock.calls[0];
    expect(url).toBe("http://localhost:5000/api/v1/customers/cust-123/sites");
    expect(init.method).toBe("POST");
    expect(JSON.parse(init.body)).toEqual(payload);
    expect(result).toEqual(mockSite);
  });

  it("sends GET /api/v1/opportunities with encoded query parameters", async () => {
    const mockOppList = {
      items: [],
      pagination: {
        page: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 1,
        nextCursor: null,
      },
    };

    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => mockOppList,
    });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");
    const result = await client.listOpportunities(
      { token: "sample-token", membershipId: "mem-1" },
      { search: "งาน Built-in", stage: "draft", limit: 10 }
    );

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const [url, init] = fetchMock.mock.calls[0];
    expect(url).toContain("/api/v1/opportunities?");
    expect(url).toContain("search=%E0%B8%87%E0%B8%B2%E0%B8%99+Built-in");
    expect(url).toContain("stage=draft");
    expect(url).toContain("limit=10");
    expect(init.method).toBe("GET");
    expect(result).toEqual(mockOppList);
  });

  it("calls the notification endpoints with membership headers and a bounded query", async () => {
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => ({ items: [], pagination: { page: 1, pageSize: 20, totalCount: 0, totalPages: 0 } }),
    });
    global.fetch = fetchMock;
    const client = new ApiClient("http://localhost:5000");
    const options = { token: "tok", membershipId: "m-1", locale: "th" as const };

    await client.listNotifications(options, { unreadOnly: true, page: 2, pageSize: 20 });
    await client.getUnreadNotificationCount(options);
    await client.markNotificationRead("n 1", options);
    await client.markAllNotificationsRead(options);

    const calls = fetchMock.mock.calls.map(([url, init]) => [url, init.method]);
    expect(calls).toEqual([
      ["http://localhost:5000/api/v1/notifications?unreadOnly=true&page=2&pageSize=20", "GET"],
      ["http://localhost:5000/api/v1/notifications/unread-count", "GET"],
      ["http://localhost:5000/api/v1/notifications/n%201/read", "POST"],
      ["http://localhost:5000/api/v1/notifications/read-all", "POST"],
    ]);
    expect(fetchMock.mock.calls[0][1].headers["X-Membership-Id"]).toBe("m-1");
    // Mark-read is naturally idempotent on the server; no Idempotency-Key is required or sent.
    expect(fetchMock.mock.calls[2][1].headers["Idempotency-Key"]).toBeUndefined();
  });

  it("calls the organization and branch administration endpoints with If-Match and Idempotency-Key where required", async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok: true, status: 200, json: async () => ({}) });
    global.fetch = fetchMock;

    const client = new ApiClient("http://localhost:5000");
    const options: RequestOptions = { token: "tok", membershipId: "m-1", locale: "th" };
    const branch = { name: "n", nameEn: null, taxBranchCode: null, addressTh: null, addressEn: null, phone: null };
    const profile = { name: "บริษัท", nameEn: null, taxIdentifier: null, addressTh: null, addressEn: null, phone: null };

    await client.getOrganizationProfile(options);
    await client.updateOrganizationProfile(profile, { ...options, ifMatch: "v1" });
    await client.listAdminBranches("inactive", options);
    await client.getAdminBranch("b 1", options);
    await client.createAdminBranch({ code: "B2", ...branch }, { ...options, idempotencyKey: "key-0123456789abcdef" });
    await client.updateAdminBranch("b1", branch, { ...options, ifMatch: "v2" });
    await client.getBranchDeactivationCheck("b1", options);
    await client.deactivateAdminBranch("b1", { reason: "ปิด" }, { ...options, ifMatch: "v3" });
    await client.activateAdminBranch("b1", { ...options, ifMatch: "v4" });

    const calls = fetchMock.mock.calls.map(([url, init]) => [url, init.method]);
    expect(calls).toEqual([
      ["http://localhost:5000/api/v1/admin/organization", "GET"],
      ["http://localhost:5000/api/v1/admin/organization", "PUT"],
      ["http://localhost:5000/api/v1/admin/branches?status=inactive", "GET"],
      ["http://localhost:5000/api/v1/admin/branches/b%201", "GET"],
      ["http://localhost:5000/api/v1/admin/branches", "POST"],
      ["http://localhost:5000/api/v1/admin/branches/b1", "PUT"],
      ["http://localhost:5000/api/v1/admin/branches/b1/deactivation-check", "GET"],
      ["http://localhost:5000/api/v1/admin/branches/b1/deactivate", "POST"],
      ["http://localhost:5000/api/v1/admin/branches/b1/activate", "POST"],
    ]);
    expect(fetchMock.mock.calls[1][1].headers["If-Match"]).toBe('"v1"');
    expect(fetchMock.mock.calls[4][1].headers["Idempotency-Key"]).toBe("key-0123456789abcdef");
    expect(fetchMock.mock.calls[8][1].headers["If-Match"]).toBe('"v4"');

    // An undefined status must not add a query filter.
    await client.listAdminBranches(undefined, options);
    expect(fetchMock.mock.calls[9][0]).toBe("http://localhost:5000/api/v1/admin/branches");
  });
});
