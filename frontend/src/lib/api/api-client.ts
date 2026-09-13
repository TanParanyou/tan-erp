import type { components, paths } from "@/generated/api/tan-erp.v1";
import { ApiError } from "./api-error";
import type { ProblemDetails } from "./problem-details";

export type CurrentUserResponse = components["schemas"]["CurrentUserResponse"];
export type CustomerListResponse = components["schemas"]["CustomerListResponse"];
export type CustomerListItemResponse = components["schemas"]["CustomerListItemResponse"];
export type CustomerResponse = components["schemas"]["CustomerResponse"];
export type DuplicateCustomerResponse = components["schemas"]["DuplicateCustomerResponse"];
export type CreateCustomerRequest = components["schemas"]["CreateCustomerRequest"];

export type SiteResponse = components["schemas"]["SiteResponse"];
export type SiteListResponse = components["schemas"]["SiteListResponse"];
export type CreateSiteRequest = components["schemas"]["CreateSiteRequest"];

export type OpportunityResponse = components["schemas"]["OpportunityResponse"];
export type OpportunityListResponse = components["schemas"]["OpportunityListResponse"];
export type CreateOpportunityRequest = components["schemas"]["CreateOpportunityRequest"];
export type UpdateDraftQGateRequest = components["schemas"]["UpdateDraftQGateRequest"];
export type UpdateOpenOpportunityRequest = components["schemas"]["UpdateOpenOpportunityRequest"];
export type ReassignOpportunityOwnerRequest = components["schemas"]["ReassignOpportunityOwnerRequest"];
export type TransitionOpportunityStageRequest = components["schemas"]["TransitionOpportunityStageRequest"];
export type OpportunityStageHistoryListResponse = components["schemas"]["OpportunityStageHistoryListResponse"];
export type OpportunityStageHistoryItemResponse = components["schemas"]["OpportunityStageHistoryItemResponse"];

export type SiteSurveyResponse = components["schemas"]["SiteSurveyResponse"];
export type SiteSurveyRevisionResponse = components["schemas"]["SiteSurveyRevisionResponse"];
export type SiteSurveyAreaResponse = components["schemas"]["SiteSurveyAreaResponse"];
export type SiteSurveyMeasurementResponse = components["schemas"]["SiteSurveyMeasurementResponse"];
export type CreateSiteSurveyRequest = components["schemas"]["CreateSiteSurveyRequest"];
export type UpdateSurveyDraftRequest = components["schemas"]["UpdateSurveyDraftRequest"];
export type UpdateSurveyAreaRequest = components["schemas"]["UpdateSurveyAreaRequest"];
export type UpdateSurveyMeasurementRequest = components["schemas"]["UpdateSurveyMeasurementRequest"];
export type MarkSurveyReadyRequest = components["schemas"]["MarkSurveyReadyRequest"];


export type AddressSearchResponse = components["schemas"]["AddressSearchResponse"];
export type AddressSearchResultItem = components["schemas"]["AddressSearchResultItem"];

export type UserListItemResponse = components["schemas"]["UserListItemResponse"];
export type UserListResponse = components["schemas"]["UserListResponse"];

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

export type ListUsersParams = NonNullable<
  paths["/api/v1/users"]["get"]["parameters"]["query"]
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
    if (params?.customerType) query.set("customerType", params.customerType);
    if (params?.sortBy) query.set("sortBy", params.sortBy);
    if (params?.sortOrder) query.set("sortOrder", params.sortOrder);
    if (params?.page !== undefined && params?.page !== null) query.set("page", params.page.toString());
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

  async checkCustomerDuplicates(
    params: { name?: string | null; phone?: string | null; email?: string | null },
    options: RequestOptions
  ): Promise<DuplicateCustomerResponse[]> {
    const query = new URLSearchParams();
    if (params.name) query.set("name", params.name);
    if (params.phone) query.set("phone", params.phone);
    if (params.email) query.set("email", params.email);

    const queryString = query.toString();
    const endpoint = `/api/v1/customers/check-duplicates${queryString ? `?${queryString}` : ""}`;
    return this.request<DuplicateCustomerResponse[]>(endpoint, "GET", options);
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
    if (params?.sortBy) query.set("sortBy", params.sortBy);
    if (params?.sortOrder) query.set("sortOrder", params.sortOrder);
    if (params?.page !== undefined && params?.page !== null) query.set("page", params.page.toString());
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

  async updateDraftQGate(
    id: string,
    payload: UpdateDraftQGateRequest,
    options: RequestOptions
  ): Promise<OpportunityResponse> {
    return this.request<OpportunityResponse>(
      `/api/v1/opportunities/${encodeURIComponent(id)}`,
      "PATCH",
      options,
      payload
    );
  }

  async updateOpenOpportunity(
    id: string,
    payload: UpdateOpenOpportunityRequest,
    options: RequestOptions
  ): Promise<OpportunityResponse> {
    return this.request<OpportunityResponse>(
      `/api/v1/opportunities/${encodeURIComponent(id)}`,
      "PUT",
      options,
      payload
    );
  }

  async reassignOpportunityOwner(
    id: string,
    payload: ReassignOpportunityOwnerRequest,
    options: RequestOptions
  ): Promise<OpportunityResponse> {
    return this.request<OpportunityResponse>(
      `/api/v1/opportunities/${encodeURIComponent(id)}/owner-changes`,
      "POST",
      options,
      payload
    );
  }

  async transitionOpportunityStage(
    id: string,
    payload: TransitionOpportunityStageRequest,
    options: RequestOptions
  ): Promise<OpportunityResponse> {
    return this.request<OpportunityResponse>(
      `/api/v1/opportunities/${encodeURIComponent(id)}/stage-transitions`,
      "POST",
      options,
      payload
    );
  }

  async getOpportunityStageHistory(
    id: string,
    options: RequestOptions
  ): Promise<OpportunityStageHistoryListResponse> {
    return this.request<OpportunityStageHistoryListResponse>(
      `/api/v1/opportunities/${encodeURIComponent(id)}/stage-history`,
      "GET",
      options
    );
  }

  async searchAddresses(
    query: string,
    options: RequestOptions,
    limit: number = 20
  ): Promise<AddressSearchResponse> {
    const encoded = encodeURIComponent(query);
    return this.request<AddressSearchResponse>(
      `/api/v1/master-data/addresses/search?q=${encoded}&limit=${limit}`,
      "GET",
      options
    );
  }

  async listUsers(
    options: RequestOptions,
    params?: ListUsersParams
  ): Promise<UserListResponse> {
    const searchParams = new URLSearchParams();
    if (params?.branchId) searchParams.set("branchId", params.branchId);
    if (params?.search) searchParams.set("search", params.search);
    if (params?.limit) searchParams.set("limit", params.limit.toString());

    const qs = searchParams.toString();
    const endpoint = `/api/v1/users${qs ? `?${qs}` : ""}`;

    return this.request<UserListResponse>(endpoint, "GET", options);
  }

  async createSiteSurvey(
    opportunityId: string,
    payload: CreateSiteSurveyRequest,
    options: RequestOptions
  ): Promise<SiteSurveyResponse> {
    return this.request<SiteSurveyResponse>(
      `/api/v1/opportunities/${encodeURIComponent(opportunityId)}/surveys`,
      "POST",
      options,
      payload
    );
  }

  async getOpportunitySurvey(
    opportunityId: string,
    options: RequestOptions
  ): Promise<SiteSurveyResponse> {
    return this.request<SiteSurveyResponse>(
      `/api/v1/opportunities/${encodeURIComponent(opportunityId)}/surveys`,
      "GET",
      options
    );
  }

  async updateSurveyDraft(
    opportunityId: string,
    surveyId: string,
    revisionId: string,
    payload: UpdateSurveyDraftRequest,
    options: RequestOptions
  ): Promise<SiteSurveyRevisionResponse> {
    return this.request<SiteSurveyRevisionResponse>(
      `/api/v1/opportunities/${encodeURIComponent(opportunityId)}/surveys/${encodeURIComponent(surveyId)}/revisions/${encodeURIComponent(revisionId)}/draft`,
      "PUT",
      options,
      payload
    );
  }

  async markSurveyReady(
    opportunityId: string,
    surveyId: string,
    revisionId: string,
    payload: MarkSurveyReadyRequest,
    options: RequestOptions
  ): Promise<SiteSurveyRevisionResponse> {
    return this.request<SiteSurveyRevisionResponse>(
      `/api/v1/opportunities/${encodeURIComponent(opportunityId)}/surveys/${encodeURIComponent(surveyId)}/revisions/${encodeURIComponent(revisionId)}/mark-ready`,
      "POST",
      options,
      payload
    );
  }
}

export const apiClient = new ApiClient();

