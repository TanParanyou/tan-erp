import type { components, paths } from "@/generated/api/tan-erp.v1";
import { ApiError } from "./api-error";
import type { ProblemDetails } from "./problem-details";

export type CurrentUserResponse = components["schemas"]["CurrentUserResponse"];
export type CustomerListResponse = components["schemas"]["CustomerListResponse"];
export type CustomerResponse = components["schemas"]["CustomerResponse"];
export type CreateCustomerRequest = components["schemas"]["CreateCustomerRequest"];

export type SiteResponse = components["schemas"]["SiteResponse"];
export type SiteListResponse = components["schemas"]["SiteListResponse"];
export type CreateSiteRequest = components["schemas"]["CreateSiteRequest"];

export type OpportunityResponse = components["schemas"]["OpportunityResponse"];
export type OpportunityListResponse = components["schemas"]["OpportunityListResponse"];
export type CreateOpportunityRequest = components["schemas"]["CreateOpportunityRequest"];

export interface RequestOptions {
  token: string;
  membershipId?: string;
  idempotencyKey?: string;
  ifMatch?: string;
  locale?: "th" | "en";
  signal?: AbortSignal;
}

export type ListCustomersParams = NonNullable<
  paths["/api/v1/customers"]["get"]["parameters"]["query"]
>;

export type ListOpportunitiesParams = NonNullable<
  paths["/api/v1/opportunities"]["get"]["parameters"]["query"]
>;

export class ApiClient {
  private readonly baseUrl: string;

  constructor(baseUrl?: string) {
    this.baseUrl = (baseUrl || process.env.NEXT_PUBLIC_API_BASE_URL || "").replace(/\/$/, "");
  }

  private async request<T>(
    endpoint: string,
    method: "GET" | "POST" | "PUT" | "DELETE" | "PATCH",
    options: RequestOptions,
    body?: unknown
  ): Promise<T> {
    const { token, membershipId, idempotencyKey, ifMatch, locale = "th", signal } = options;

    if (!token || token.trim() === "") {
      throw new ApiError({
        status: 401,
        code: "AUTHENTICATION_REQUIRED",
        message: "Token is required.",
      });
    }

    const headers: Record<string, string> = {
      Authorization: `Bearer ${token.trim()}`,
      "Accept-Language": locale,
      Accept: "application/json",
    };

    if (membershipId) {
      headers["X-Membership-Id"] = membershipId;
    }

    if (idempotencyKey) {
      headers["Idempotency-Key"] = idempotencyKey;
    }

    if (ifMatch) {
      headers["If-Match"] = ifMatch.startsWith('"') && ifMatch.endsWith('"') ? ifMatch : `"${ifMatch}"`;
    }

    if (body !== undefined) {
      headers["Content-Type"] = "application/json";
    }

    const url = `${this.baseUrl}${endpoint}`;

    try {
      const response = await fetch(url, {
        method,
        headers,
        body: body !== undefined ? JSON.stringify(body) : undefined,
        signal,
      });

      if (!response.ok) {
        let problem: ProblemDetails | null = null;
        try {
          problem = await response.json();
        } catch {
          // Response body was not JSON
        }

        if (problem && (problem.code || problem.status || problem.title)) {
          throw ApiError.fromProblemDetails(response.status, problem);
        }

        throw ApiError.fromUnknown(response.status);
      }

      const data: T = await response.json();
      return data;
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        throw err;
      }

      if (err instanceof Error && err.name === "AbortError") {
        throw ApiError.fromAbort();
      }

      throw ApiError.fromUnknown(500);
    }
  }

  async getCurrentUser(
    token: string,
    locale: "th" | "en" = "th",
    signal?: AbortSignal
  ): Promise<CurrentUserResponse> {
    return this.request<CurrentUserResponse>("/api/v1/me", "GET", {
      token,
      locale,
      signal,
    });
  }

  async listCustomers(
    options: RequestOptions,
    params?: ListCustomersParams
  ): Promise<CustomerListResponse> {
    const query = new URLSearchParams();
    if (params?.search) query.set("search", params.search);
    if (params?.status) query.set("status", params.status);
    if (params?.limit) query.set("limit", params.limit.toString());
    if (params?.cursor) query.set("cursor", params.cursor);

    const queryString = query.toString();
    const endpoint = `/api/v1/customers${queryString ? `?${queryString}` : ""}`;
    return this.request<CustomerListResponse>(endpoint, "GET", options);
  }

  async getCustomer(
    id: string,
    options: RequestOptions
  ): Promise<CustomerResponse> {
    return this.request<CustomerResponse>(`/api/v1/customers/${encodeURIComponent(id)}`, "GET", options);
  }

  async createCustomer(
    payload: CreateCustomerRequest,
    options: RequestOptions
  ): Promise<CustomerResponse> {
    return this.request<CustomerResponse>("/api/v1/customers", "POST", options, payload);
  }

  async activateCustomer(
    id: string,
    options: RequestOptions
  ): Promise<CustomerResponse> {
    return this.request<CustomerResponse>(`/api/v1/customers/${encodeURIComponent(id)}/activate`, "POST", options);
  }

  async listCustomerSites(
    customerId: string,
    options: RequestOptions
  ): Promise<SiteListResponse> {
    return this.request<SiteListResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/sites`, "GET", options);
  }

  async createSite(
    customerId: string,
    payload: CreateSiteRequest,
    options: RequestOptions
  ): Promise<SiteResponse> {
    return this.request<SiteResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/sites`, "POST", options, payload);
  }

  async listOpportunities(
    options: RequestOptions,
    params?: ListOpportunitiesParams
  ): Promise<OpportunityListResponse> {
    const query = new URLSearchParams();
    if (params?.search) query.set("search", params.search);
    if (params?.customerId) query.set("customerId", params.customerId);
    if (params?.stage) query.set("stage", params.stage);
    if (params?.limit) query.set("limit", params.limit.toString());
    if (params?.cursor) query.set("cursor", params.cursor);

    const queryString = query.toString();
    const endpoint = `/api/v1/opportunities${queryString ? `?${queryString}` : ""}`;
    return this.request<OpportunityListResponse>(endpoint, "GET", options);
  }

  async getOpportunity(
    id: string,
    options: RequestOptions
  ): Promise<OpportunityResponse> {
    return this.request<OpportunityResponse>(`/api/v1/opportunities/${encodeURIComponent(id)}`, "GET", options);
  }

  async createOpportunity(
    payload: CreateOpportunityRequest,
    options: RequestOptions
  ): Promise<OpportunityResponse> {
    return this.request<OpportunityResponse>("/api/v1/opportunities", "POST", options, payload);
  }
}

export const apiClient = new ApiClient();
