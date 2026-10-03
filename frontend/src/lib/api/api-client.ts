import type { components, paths } from "@/generated/api/tan-erp.v1";
import { ApiError } from "./api-error";
import type { ProblemDetails } from "./problem-details";

export type CurrentUserResponse = components["schemas"]["CurrentUserResponse"];
export type CustomerListResponse = components["schemas"]["CustomerListResponse"];
export type CustomerListItemResponse = components["schemas"]["CustomerListItemResponse"];
export type CustomerResponse = components["schemas"]["CustomerResponse"];
export type DuplicateCustomerResponse = components["schemas"]["DuplicateCustomerResponse"];
export type CreateCustomerRequest = components["schemas"]["CreateCustomerRequest"];
export type UpdateCustomerRequest = components["schemas"]["UpdateCustomerRequest"];
export type CustomerAddressResponse = components["schemas"]["CustomerAddressResponse"];
export type CustomerAddressListResponse = components["schemas"]["CustomerAddressListResponse"];
export type CreateCustomerAddressRequest = components["schemas"]["CreateCustomerAddressRequest"];
export type UpdateCustomerAddressRequest = components["schemas"]["UpdateCustomerAddressRequest"];
export type CustomerContactDetailResponse = components["schemas"]["CustomerContactDetailResponse"];
export type CustomerContactListResponse = components["schemas"]["CustomerContactListResponse"];
export type CreatePrimaryContactRequest = components["schemas"]["CreatePrimaryContactRequest"];
export type DeactivateCustomerRequest = components["schemas"]["DeactivateCustomerRequest"];

export type SiteResponse = components["schemas"]["SiteResponse"];
export type SiteListResponse = components["schemas"]["SiteListResponse"];
export type CreateSiteRequest = components["schemas"]["CreateSiteRequest"];
export type UpdateSiteRequest = components["schemas"]["UpdateSiteRequest"];
export type SiteImageResponse = components["schemas"]["SiteImageResponse"];
export type CreateSiteImageRequest = components["schemas"]["CreateSiteImageRequest"];

export type OpportunityResponse = components["schemas"]["OpportunityResponse"];
export type OpportunityListResponse = components["schemas"]["OpportunityListResponse"];
export type CustomerSummaryResponse = components["schemas"]["CustomerSummaryResponse"];
export type SiteSummaryResponse = components["schemas"]["SiteSummaryResponse"];
export type ActorSummaryResponse = components["schemas"]["ActorSummaryResponse"];
export type CreateOpportunityRequest = components["schemas"]["CreateOpportunityRequest"];
export type UpdateDraftQGateRequest = components["schemas"]["UpdateDraftQGateRequest"];
export type UpdateOpenOpportunityRequest = components["schemas"]["UpdateOpenOpportunityRequest"];
export type ReassignOpportunityOwnerRequest = components["schemas"]["ReassignOpportunityOwnerRequest"];
export type TransitionOpportunityStageRequest = components["schemas"]["TransitionOpportunityStageRequest"];
export type OpportunityStageHistoryListResponse = components["schemas"]["OpportunityStageHistoryListResponse"];
export type OpportunityStageHistoryItemResponse = components["schemas"]["OpportunityStageHistoryItemResponse"];
export type AttachWorkImagesRequest = components["schemas"]["AttachWorkImagesRequest"];
export type AttachWorkImagesResponse = components["schemas"]["AttachWorkImagesResponse"];
export type OpportunityWorkImageListResponse = components["schemas"]["OpportunityWorkImageListResponse"];
export type OpportunityWorkImageResponse = components["schemas"]["OpportunityWorkImageResponse"];

export type SiteSurveyResponse = components["schemas"]["SiteSurveyResponse"];
export type SiteSurveyRevisionResponse = components["schemas"]["SiteSurveyRevisionResponse"];
export type SiteSurveyAreaResponse = components["schemas"]["SiteSurveyAreaResponse"];
export type SiteSurveyMeasurementResponse = components["schemas"]["SiteSurveyMeasurementResponse"];
export type CreateSiteSurveyRequest = components["schemas"]["CreateSiteSurveyRequest"];
export type UpdateSurveyDraftRequest = components["schemas"]["UpdateSurveyDraftRequest"];
export type UpdateSurveyAreaRequest = components["schemas"]["UpdateSurveyAreaRequest"];
export type UpdateSurveyMeasurementRequest = components["schemas"]["UpdateSurveyMeasurementRequest"];
export type MarkSurveyReadyRequest = components["schemas"]["MarkSurveyReadyRequest"];

export type EstimateDetailResponse = components["schemas"]["EstimateDetailResponse"];
export type EstimateRevisionResponse = components["schemas"]["EstimateRevisionResponse"];
export type CreateEstimateDraftRequest = components["schemas"]["CreateEstimateDraftRequest"];
export type UpdateEstimateDraftRequest = components["schemas"]["UpdateEstimateDraftRequest"];
export type CalculateEstimateRequest = components["schemas"]["CalculateEstimateRequest"];
export type SubmitEstimateRequest = components["schemas"]["SubmitEstimateRequest"];
export type ReviewEstimateRequest = components["schemas"]["ReviewEstimateRequest"];
export type CreateEstimateRevisionRequest = components["schemas"]["CreateEstimateRevisionRequest"];
export type CancelEstimateRequest = components["schemas"]["CancelEstimateRequest"];
export type IssueQuotationRequest = components["schemas"]["IssueQuotationRequest"];
export type QuotationResponse = components["schemas"]["QuotationResponse"];
export type AcceptQuotationRequest = components["schemas"]["AcceptQuotationRequest"];
export type AcceptQuotationResponse = components["schemas"]["AcceptQuotationResponse"];
export type EstimateCatalogResponse = components["schemas"]["EstimateCatalogResponse"];
export type EstimateCatalogItemResponse = components["schemas"]["EstimateCatalogItemResponse"];


export type AddressSearchResponse = components["schemas"]["AddressSearchResponse"];
export type AddressSearchResultItem = components["schemas"]["AddressSearchResultItem"];

export type UserListItemResponse = components["schemas"]["UserListItemResponse"];
export type UserListResponse = components["schemas"]["UserListResponse"];

export type AdminUserResponse = components["schemas"]["AdminUserResponse"];
export type AdminMembershipResponse = components["schemas"]["AdminMembershipResponse"];
export type AdminUserListResponse = components["schemas"]["AdminUserListResponse"];
export type AdminRoleResponse = components["schemas"]["AdminRoleResponse"];
export type AdminRoleListResponse = components["schemas"]["AdminRoleListResponse"];
export type AdminRoleRequestResponse = components["schemas"]["AdminRoleRequestResponse"];
export type AdminRoleRequestListResponse = components["schemas"]["AdminRoleRequestListResponse"];
export type AdminAssignRoleResponse = components["schemas"]["AdminAssignRoleResponse"];
export type CreateAdminUserRequest = components["schemas"]["CreateAdminUserRequest"];
export type RenameAdminUserRequest = components["schemas"]["RenameAdminUserRequest"];
export type UpdateAdminMembershipRequest = components["schemas"]["UpdateAdminMembershipRequest"];
export type AssignAdminRoleRequest = components["schemas"]["AssignAdminRoleRequest"];

export interface AdminUserListParams {
  search?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}

export interface RequestOptions {
  token: string;
  membershipId?: string;
  idempotencyKey?: string;
  ifMatch?: string;
  locale?: "th" | "en";
  signal?: AbortSignal;
}

export interface OrganizationBranchResponse {
  id: string;
  code: string;
  name: string;
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

export type EstimateCatalogParams = NonNullable<
  paths["/api/v1/estimate-catalog/items"]["get"]["parameters"]["query"]
>;

export type ItemResponse = components["schemas"]["ItemResponse"];
export type ItemBranchAvailabilityResponse = components["schemas"]["ItemBranchAvailabilityResponse"];
export type AddAliasRequest = components["schemas"]["AddAliasRequest"];
export type ItemImageDetailResponse = components["schemas"]["ItemImageDetailResponse"];
export type AttachItemImageRequest = components["schemas"]["AttachItemImageRequest"];
export type CreateItemRequest = components["schemas"]["CreateItemRequest"];
export type UpdateItemRequest = components["schemas"]["UpdateItemRequest"];
export type PagedItemsResponse = components["schemas"]["PagedItemsResponse"];
export type CostSourceResponse = components["schemas"]["CostSourceResponse"];
export type CostSourceRequest = components["schemas"]["CostSourceRequest"];
export type UpdateCostSourceRequest = components["schemas"]["UpdateCostSourceRequest"];
export type ItemBarcodeResponse = components["schemas"]["ItemBarcodeResponse"];
export type CreateItemBarcodeRequest = components["schemas"]["CreateItemBarcodeRequest"];
export type ItemUnitConversionResponse = components["schemas"]["ItemUnitConversionResponse"];
export type CreateItemUnitConversionRequest = components["schemas"]["CreateItemUnitConversionRequest"];
export type UnitConversionResponse = components["schemas"]["UnitConversionResponse"];
export type CostReviewQueueResponse = components["schemas"]["CostReviewQueueResponse"];
export type CostRecordResponse = components["schemas"]["CostRecordResponse"];
export type CreateCostRecordRequest = components["schemas"]["CreateCostRecordRequest"];
export type UpdateCostRecordRequest = components["schemas"]["UpdateCostRecordRequest"];
export type ItemCategoryResponse = components["schemas"]["ItemCategoryDetailResponse"];
export type UnitOfMeasureResponse = components["schemas"]["UnitOfMeasureDetailResponse"];
export type ItemBrandResponse = components["schemas"]["ItemBrandDetailResponse"];
export type ItemTaxCategoryResponse = components["schemas"]["ItemTaxCategoryDetailResponse"];
export type CreateItemCategoryRequest = components["schemas"]["CreateItemCategoryRequest"];
export type UpdateItemCategoryRequest = components["schemas"]["UpdateItemCategoryRequest"];
export type CreateItemBrandRequest = components["schemas"]["CreateItemBrandRequest"];
export type UpdateItemBrandRequest = components["schemas"]["UpdateItemBrandRequest"];
export type CreateItemTaxCategoryRequest = components["schemas"]["CreateItemTaxCategoryRequest"];
export type UpdateItemTaxCategoryRequest = components["schemas"]["UpdateItemTaxCategoryRequest"];
export type CreateUnitOfMeasureRequest = components["schemas"]["CreateUnitOfMeasureRequest"];
export type UpdateUnitOfMeasureRequest = components["schemas"]["UpdateUnitOfMeasureRequest"];
export type ListItemsParams = NonNullable<paths["/api/v1/items"]["get"]["parameters"]["query"]>;
export type CostReviewQueueParams = NonNullable<paths["/api/v1/cost-review-queue"]["get"]["parameters"]["query"]>;
export type EstimateReviewQueueResponse = components["schemas"]["EstimateReviewQueueResponse"];
export type EstimateReviewQueueParams = NonNullable<paths["/api/v1/estimates/review-queue"]["get"]["parameters"]["query"]>;

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

      if (response.status === 204) {
        return undefined as T;
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

  async updateCustomer(
    id: string,
    payload: UpdateCustomerRequest,
    options: RequestOptions
  ): Promise<CustomerResponse> {
    return this.request<CustomerResponse>(`/api/v1/customers/${encodeURIComponent(id)}`, "PATCH", options, payload);
  }

  async deactivateCustomer(id: string, payload: DeactivateCustomerRequest, options: RequestOptions): Promise<CustomerResponse> {
    return this.request<CustomerResponse>(`/api/v1/customers/${encodeURIComponent(id)}/deactivate`, "POST", options, payload);
  }

  async reactivateCustomer(id: string, options: RequestOptions): Promise<CustomerResponse> {
    return this.request<CustomerResponse>(`/api/v1/customers/${encodeURIComponent(id)}/reactivate`, "POST", options);
  }

  async listCustomerAddresses(customerId: string, options: RequestOptions): Promise<CustomerAddressListResponse> {
    return this.request<CustomerAddressListResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/addresses`, "GET", options);
  }

  async createCustomerAddress(customerId: string, payload: CreateCustomerAddressRequest, options: RequestOptions): Promise<CustomerAddressResponse> {
    return this.request<CustomerAddressResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/addresses`, "POST", options, payload);
  }

  async updateCustomerAddress(customerId: string, addressId: string, payload: UpdateCustomerAddressRequest, options: RequestOptions): Promise<CustomerAddressResponse> {
    return this.request<CustomerAddressResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/addresses/${encodeURIComponent(addressId)}`, "PATCH", options, payload);
  }

  async setPrimaryCustomerAddress(customerId: string, addressId: string, options: RequestOptions): Promise<CustomerAddressResponse> {
    return this.request<CustomerAddressResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/addresses/${encodeURIComponent(addressId)}/primary`, "POST", options);
  }

  async deactivateCustomerAddress(customerId: string, addressId: string, options: RequestOptions): Promise<CustomerAddressResponse> {
    return this.request<CustomerAddressResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/addresses/${encodeURIComponent(addressId)}/deactivate`, "POST", options);
  }

  async listCustomerContacts(customerId: string, options: RequestOptions): Promise<CustomerContactListResponse> {
    return this.request<CustomerContactListResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/contacts`, "GET", options);
  }

  async createCustomerContact(customerId: string, payload: CreatePrimaryContactRequest, options: RequestOptions): Promise<CustomerContactDetailResponse> {
    return this.request<CustomerContactDetailResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/contacts`, "POST", options, payload);
  }

  async updateCustomerContact(customerId: string, contactId: string, payload: CreatePrimaryContactRequest, options: RequestOptions): Promise<CustomerContactDetailResponse> {
    return this.request<CustomerContactDetailResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/contacts/${encodeURIComponent(contactId)}`, "PATCH", options, payload);
  }

  async setPrimaryCustomerContact(customerId: string, contactId: string, options: RequestOptions): Promise<CustomerContactDetailResponse> {
    return this.request<CustomerContactDetailResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/contacts/${encodeURIComponent(contactId)}/primary`, "POST", options);
  }

  async deactivateCustomerContact(customerId: string, contactId: string, options: RequestOptions): Promise<CustomerContactDetailResponse> {
    return this.request<CustomerContactDetailResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/contacts/${encodeURIComponent(contactId)}/deactivate`, "POST", options);
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

  async updateSite(customerId: string, siteId: string, payload: UpdateSiteRequest, options: RequestOptions): Promise<SiteResponse> {
    return this.request<SiteResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/sites/${encodeURIComponent(siteId)}`, "PATCH", options, payload);
  }

  async deactivateSite(customerId: string, siteId: string, options: RequestOptions): Promise<SiteResponse> {
    return this.request<SiteResponse>(`/api/v1/customers/${encodeURIComponent(customerId)}/sites/${encodeURIComponent(siteId)}/deactivate`, "POST", options);
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

  async attachWorkImages(
    id: string,
    payload: AttachWorkImagesRequest,
    options: RequestOptions
  ): Promise<AttachWorkImagesResponse> {
    return this.request<AttachWorkImagesResponse>(
      `/api/v1/opportunities/${encodeURIComponent(id)}/work-images`,
      "POST",
      options,
      payload
    );
  }

  async listWorkImages(
    id: string,
    options: RequestOptions,
    params?: { stage?: string; limit?: number; cursor?: string }
  ): Promise<OpportunityWorkImageListResponse> {
    const query = new URLSearchParams();
    if (params?.stage) query.set("stage", params.stage);
    if (params?.limit) query.set("limit", params.limit.toString());
    if (params?.cursor) query.set("cursor", params.cursor);

    const queryString = query.toString();
    const endpoint = `/api/v1/opportunities/${encodeURIComponent(id)}/work-images${queryString ? `?${queryString}` : ""}`;
    return this.request<OpportunityWorkImageListResponse>(endpoint, "GET", options);
  }

  async detachWorkImage(
    opportunityId: string,
    imageId: string,
    options: RequestOptions
  ): Promise<void> {
    return this.request<void>(
      `/api/v1/opportunities/${encodeURIComponent(opportunityId)}/work-images/${encodeURIComponent(imageId)}`,
      "DELETE",
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
  ): Promise<SiteSurveyResponse | null> {
    try {
      const res = await this.request<SiteSurveyResponse | undefined>(
        `/api/v1/opportunities/${encodeURIComponent(opportunityId)}/surveys`,
        "GET",
        options
      );
      return res ?? null;
    } catch (err) {
      if (err instanceof ApiError && (err.status === 204 || err.status === 404)) {
        return null;
      }
      throw err;
    }
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

  async getEstimate(
    id: string,
    options: RequestOptions
  ): Promise<EstimateDetailResponse> {
    return this.request<EstimateDetailResponse>(
      `/api/v1/estimates/${encodeURIComponent(id)}`,
      "GET",
      options
    );
  }

  async getOpportunityEstimate(
    opportunityId: string,
    options: RequestOptions
  ): Promise<EstimateDetailResponse | null> {
    try {
      const res = await this.request<EstimateDetailResponse | undefined>(
        `/api/v1/opportunities/${encodeURIComponent(opportunityId)}/estimates`,
        "GET",
        options
      );
      return res ?? null;
    } catch (err) {
      if (err instanceof ApiError && (err.status === 204 || err.status === 404)) {
        return null;
      }
      throw err;
    }
  }

  async createEstimate(
    payload: CreateEstimateDraftRequest,
    options: RequestOptions
  ): Promise<EstimateDetailResponse> {
    return this.request<EstimateDetailResponse>(
      "/api/v1/estimates",
      "POST",
      options,
      payload
    );
  }

  async updateEstimateDraft(
    id: string,
    revisionId: string,
    payload: UpdateEstimateDraftRequest,
    options: RequestOptions
  ): Promise<EstimateRevisionResponse> {
    return this.request<EstimateRevisionResponse>(
      `/api/v1/estimates/${encodeURIComponent(id)}/revisions/${encodeURIComponent(revisionId)}/draft`,
      "PUT",
      options,
      payload
    );
  }

  async calculateEstimate(
    id: string,
    revisionId: string,
    payload: CalculateEstimateRequest,
    options: RequestOptions
  ): Promise<EstimateRevisionResponse> {
    return this.request<EstimateRevisionResponse>(
      `/api/v1/estimates/${encodeURIComponent(id)}/revisions/${encodeURIComponent(revisionId)}/calculate`,
      "POST",
      options,
      payload
    );
  }

  async submitEstimate(
    id: string,
    payload: SubmitEstimateRequest,
    options: RequestOptions
  ): Promise<EstimateDetailResponse> {
    return this.request<EstimateDetailResponse>(
      `/api/v1/estimates/${encodeURIComponent(id)}/submit`,
      "POST",
      options,
      payload
    );
  }

  async reviewEstimate(
    id: string,
    payload: ReviewEstimateRequest,
    options: RequestOptions
  ): Promise<EstimateDetailResponse> {
    return this.request<EstimateDetailResponse>(
      `/api/v1/estimates/${encodeURIComponent(id)}/review-decisions`,
      "POST",
      options,
      payload
    );
  }

  async createEstimateRevision(
    id: string,
    payload: CreateEstimateRevisionRequest,
    options: RequestOptions
  ): Promise<EstimateDetailResponse> {
    return this.request<EstimateDetailResponse>(
      `/api/v1/estimates/${encodeURIComponent(id)}/revisions`,
      "POST",
      options,
      payload
    );
  }

  async cancelEstimate(
    id: string,
    payload: CancelEstimateRequest,
    options: RequestOptions
  ): Promise<EstimateDetailResponse> {
    return this.request<EstimateDetailResponse>(
      `/api/v1/estimates/${encodeURIComponent(id)}/cancel`,
      "POST",
      options,
      payload
    );
  }

  async issueQuotation(
    id: string,
    payload: IssueQuotationRequest,
    options: RequestOptions
  ): Promise<QuotationResponse> {
    return this.request<QuotationResponse>(
      `/api/v1/estimates/${encodeURIComponent(id)}/quotation`,
      "POST",
      options,
      payload
    );
  }

  async acceptQuotation(
    id: string,
    payload: AcceptQuotationRequest,
    options: RequestOptions
  ): Promise<AcceptQuotationResponse> {
    return this.request<AcceptQuotationResponse>(
      `/api/v1/estimates/${encodeURIComponent(id)}/quotation/accept`,
      "POST",
      options,
      payload
    );
  }

  async listAdminUsers(params: AdminUserListParams, options: RequestOptions): Promise<AdminUserListResponse> {
    const query = new URLSearchParams();
    if (params.search) query.set("search", params.search);
    if (params.status) query.set("status", params.status);
    if (params.page) query.set("page", String(params.page));
    if (params.pageSize) query.set("pageSize", String(params.pageSize));
    const suffix = query.toString();
    return this.request<AdminUserListResponse>(`/api/v1/admin/users${suffix ? `?${suffix}` : ""}`, "GET", options);
  }

  async getAdminUser(userId: string, options: RequestOptions): Promise<AdminUserResponse> {
    return this.request<AdminUserResponse>(`/api/v1/admin/users/${encodeURIComponent(userId)}`, "GET", options);
  }

  async createAdminUser(payload: CreateAdminUserRequest, options: RequestOptions): Promise<AdminUserResponse> {
    return this.request<AdminUserResponse>("/api/v1/admin/users", "POST", options, payload);
  }

  async renameAdminUser(userId: string, payload: RenameAdminUserRequest, options: RequestOptions): Promise<AdminUserResponse> {
    return this.request<AdminUserResponse>(`/api/v1/admin/users/${encodeURIComponent(userId)}`, "PATCH", options, payload);
  }

  async setAdminUserActive(userId: string, active: boolean, options: RequestOptions): Promise<AdminUserResponse> {
    return this.request<AdminUserResponse>(
      `/api/v1/admin/users/${encodeURIComponent(userId)}/${active ? "activate" : "deactivate"}`,
      "POST",
      options
    );
  }

  async updateAdminMembership(
    membershipId: string,
    payload: UpdateAdminMembershipRequest,
    options: RequestOptions
  ): Promise<AdminUserResponse> {
    return this.request<AdminUserResponse>(
      `/api/v1/admin/memberships/${encodeURIComponent(membershipId)}`,
      "PATCH",
      options,
      payload
    );
  }

  async setAdminMembershipActive(membershipId: string, active: boolean, options: RequestOptions): Promise<AdminUserResponse> {
    return this.request<AdminUserResponse>(
      `/api/v1/admin/memberships/${encodeURIComponent(membershipId)}/${active ? "activate" : "deactivate"}`,
      "POST",
      options
    );
  }

  async listAdminRoles(options: RequestOptions): Promise<AdminRoleListResponse> {
    return this.request<AdminRoleListResponse>("/api/v1/admin/roles", "GET", options);
  }

  async assignAdminRole(
    membershipId: string,
    payload: AssignAdminRoleRequest,
    options: RequestOptions
  ): Promise<AdminAssignRoleResponse> {
    return this.request<AdminAssignRoleResponse>(
      `/api/v1/admin/memberships/${encodeURIComponent(membershipId)}/roles`,
      "POST",
      options,
      payload
    );
  }

  async revokeAdminRole(membershipId: string, roleId: string, options: RequestOptions): Promise<void> {
    await this.request<unknown>(
      `/api/v1/admin/memberships/${encodeURIComponent(membershipId)}/roles/${encodeURIComponent(roleId)}`,
      "DELETE",
      options
    );
  }

  async listAdminRoleRequests(status: string, options: RequestOptions): Promise<AdminRoleRequestListResponse> {
    return this.request<AdminRoleRequestListResponse>(
      `/api/v1/admin/role-assignment-requests?status=${encodeURIComponent(status)}`,
      "GET",
      options
    );
  }

  async decideAdminRoleRequest(
    requestId: string,
    decision: "approve" | "reject" | "cancel",
    options: RequestOptions
  ): Promise<AdminRoleRequestResponse> {
    return this.request<AdminRoleRequestResponse>(
      `/api/v1/admin/role-assignment-requests/${encodeURIComponent(requestId)}/${decision}`,
      "POST",
      options
    );
  }

  async listDocumentSequences(
    options: RequestOptions
  ): Promise<import("@/features/settings/document-numbering/types").DocumentSequenceItem[]> {
    return this.request<import("@/features/settings/document-numbering/types").DocumentSequenceItem[]>(
      "/api/v1/settings/document-sequences",
      "GET",
      options
    );
  }

  async updateDocumentSequence(
    documentType: string,
    payload: import("@/features/settings/document-numbering/types").UpdateDocumentSequencePayload,
    options: RequestOptions
  ): Promise<import("@/features/settings/document-numbering/types").DocumentSequenceItem> {
    return this.request<import("@/features/settings/document-numbering/types").DocumentSequenceItem>(
      `/api/v1/settings/document-sequences/${encodeURIComponent(documentType)}`,
      "PUT",
      options,
      payload
    );
  }

  async previewDocumentSequence(
    payload: import("@/features/settings/document-numbering/types").PreviewDocumentSequencePayload,
    options: RequestOptions
  ): Promise<{ preview: string }> {
    return this.request<{ preview: string }>(
      "/api/v1/settings/document-sequences/preview",
      "POST",
      options,
      payload
    );
  }

  async getEstimateCatalog(
    query: EstimateCatalogParams,
    options: RequestOptions
  ): Promise<components["schemas"]["EstimateCatalogResponse"]> {
    const params = new URLSearchParams();
    params.set("branchId", query.branchId);
    if (query.search) params.set("search", query.search);
    if (query.itemType) params.set("itemType", query.itemType);
    if (query.categoryId) params.set("categoryId", query.categoryId);
    if (query.brandId) params.set("brandId", query.brandId);
    if (query.attributeKey) params.set("attributeKey", query.attributeKey);
    if (query.attributeValue) params.set("attributeValue", query.attributeValue);
    if (query.hasCost !== undefined) params.set("hasCost", String(query.hasCost));
    if (query.cursor) params.set("cursor", query.cursor);
    if (query.pageSize) params.set("pageSize", String(query.pageSize));

    return this.request<components["schemas"]["EstimateCatalogResponse"]>(
      `/api/v1/estimate-catalog/items?${params.toString()}`,
      "GET",
      options
    );
  }

  async listItems(options: RequestOptions, query: ListItemsParams): Promise<PagedItemsResponse> {
    const params = new URLSearchParams();
    for (const key of ["search", "itemType", "categoryId", "brandId", "status"] as const) {
      const value = query[key];
      if (value) params.set(key, value);
    }
    if (query.sortBy) params.set("sortBy", query.sortBy);
    if (query.sortOrder) params.set("sortOrder", query.sortOrder);
    params.set("pageNumber", String(query.pageNumber ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<PagedItemsResponse>(`/api/v1/items?${params.toString()}`, "GET", options);
  }

  async listOrganizationBranches(options: RequestOptions): Promise<OrganizationBranchResponse[]> {
    return this.request<OrganizationBranchResponse[]>("/api/v1/branches", "GET", options);
  }

  async listItemCategories(options: RequestOptions): Promise<ItemCategoryResponse[]> {
    return this.request<ItemCategoryResponse[]>("/api/v1/item-categories", "GET", options);
  }

  async listUnitsOfMeasure(options: RequestOptions): Promise<UnitOfMeasureResponse[]> {
    return this.request<UnitOfMeasureResponse[]>("/api/v1/units-of-measure", "GET", options);
  }

  async listItemBrands(options: RequestOptions): Promise<ItemBrandResponse[]> {
    return this.request<ItemBrandResponse[]>("/api/v1/item-brands", "GET", options);
  }

  async listItemTaxCategories(options: RequestOptions): Promise<ItemTaxCategoryResponse[]> {
    return this.request<ItemTaxCategoryResponse[]>("/api/v1/item-tax-categories", "GET", options);
  }

  async createItemCategory(payload: CreateItemCategoryRequest, options: RequestOptions): Promise<ItemCategoryResponse> {
    return this.request<ItemCategoryResponse>("/api/v1/item-categories", "POST", options, payload);
  }

  async updateItemCategory(id: string, payload: UpdateItemCategoryRequest, options: RequestOptions): Promise<ItemCategoryResponse> {
    return this.request<ItemCategoryResponse>(`/api/v1/item-categories/${encodeURIComponent(id)}`, "PUT", options, payload);
  }

  async createItemBrand(payload: CreateItemBrandRequest, options: RequestOptions): Promise<ItemBrandResponse> {
    return this.request<ItemBrandResponse>("/api/v1/item-brands", "POST", options, payload);
  }

  async updateItemBrand(id: string, payload: UpdateItemBrandRequest, options: RequestOptions): Promise<ItemBrandResponse> {
    return this.request<ItemBrandResponse>(`/api/v1/item-brands/${encodeURIComponent(id)}`, "PUT", options, payload);
  }

  async createItemTaxCategory(payload: CreateItemTaxCategoryRequest, options: RequestOptions): Promise<ItemTaxCategoryResponse> {
    return this.request<ItemTaxCategoryResponse>("/api/v1/item-tax-categories", "POST", options, payload);
  }

  async updateItemTaxCategory(id: string, payload: UpdateItemTaxCategoryRequest, options: RequestOptions): Promise<ItemTaxCategoryResponse> {
    return this.request<ItemTaxCategoryResponse>(`/api/v1/item-tax-categories/${encodeURIComponent(id)}`, "PUT", options, payload);
  }

  async createUnitOfMeasure(payload: CreateUnitOfMeasureRequest, options: RequestOptions): Promise<UnitOfMeasureResponse> {
    return this.request<UnitOfMeasureResponse>("/api/v1/units-of-measure", "POST", options, payload);
  }

  async updateUnitOfMeasure(id: string, payload: UpdateUnitOfMeasureRequest, options: RequestOptions): Promise<UnitOfMeasureResponse> {
    return this.request<UnitOfMeasureResponse>(`/api/v1/units-of-measure/${encodeURIComponent(id)}`, "PUT", options, payload);
  }

  async getItem(id: string, options: RequestOptions): Promise<ItemResponse> {
    return this.request<ItemResponse>(`/api/v1/items/${encodeURIComponent(id)}`, "GET", options);
  }

  async createItem(payload: CreateItemRequest, options: RequestOptions): Promise<ItemResponse> {
    return this.request<ItemResponse>("/api/v1/items", "POST", options, payload);
  }

  async updateItem(id: string, payload: UpdateItemRequest, options: RequestOptions): Promise<ItemResponse> {
    return this.request<ItemResponse>(`/api/v1/items/${encodeURIComponent(id)}`, "PUT", options, payload);
  }

  async setItemBranchAvailability(id: string, payload: components["schemas"]["SetBranchAvailabilityRequest"], options: RequestOptions): Promise<ItemResponse> {
    return this.request<ItemResponse>(`/api/v1/items/${encodeURIComponent(id)}/branch-availability`, "PUT", options, payload);
  }

  async addItemAlias(id: string, payload: AddAliasRequest, options: RequestOptions): Promise<ItemResponse> {
    return this.request<ItemResponse>(`/api/v1/items/${encodeURIComponent(id)}/aliases`, "POST", options, payload);
  }

  async removeItemAlias(id: string, aliasId: string, options: RequestOptions): Promise<ItemResponse> {
    return this.request<ItemResponse>(`/api/v1/items/${encodeURIComponent(id)}/aliases/${encodeURIComponent(aliasId)}`, "DELETE", options);
  }

  async listItemImages(id: string, options: RequestOptions): Promise<ItemImageDetailResponse[]> {
    return this.request<ItemImageDetailResponse[]>(`/api/v1/items/${encodeURIComponent(id)}/images`, "GET", options);
  }

  async attachItemImage(id: string, payload: AttachItemImageRequest, options: RequestOptions): Promise<ItemImageDetailResponse> {
    return this.request<ItemImageDetailResponse>(`/api/v1/items/${encodeURIComponent(id)}/images`, "POST", options, payload);
  }

  async setPrimaryItemImage(id: string, imageId: string, options: RequestOptions): Promise<ItemImageDetailResponse> {
    return this.request<ItemImageDetailResponse>(`/api/v1/items/${encodeURIComponent(id)}/images/${encodeURIComponent(imageId)}/primary`, "POST", options);
  }

  async detachItemImage(id: string, imageId: string, options: RequestOptions): Promise<void> {
    await this.request<void>(`/api/v1/items/${encodeURIComponent(id)}/images/${encodeURIComponent(imageId)}`, "DELETE", options);
  }

  async activateItem(id: string, options: RequestOptions): Promise<ItemResponse> {
    return this.request<ItemResponse>(`/api/v1/items/${encodeURIComponent(id)}/activate`, "POST", options);
  }

  async deactivateItem(id: string, payload: components["schemas"]["DeactivateItemRequest"], options: RequestOptions): Promise<ItemResponse> {
    return this.request<ItemResponse>(`/api/v1/items/${encodeURIComponent(id)}/deactivate`, "POST", options, payload);
  }

  async listItemBarcodes(itemId: string, options: RequestOptions): Promise<ItemBarcodeResponse[]> {
    return this.request<ItemBarcodeResponse[]>(`/api/v1/items/${encodeURIComponent(itemId)}/barcodes`, "GET", options);
  }

  async createItemBarcode(itemId: string, payload: CreateItemBarcodeRequest, options: RequestOptions): Promise<ItemBarcodeResponse> {
    return this.request<ItemBarcodeResponse>(`/api/v1/items/${encodeURIComponent(itemId)}/barcodes`, "POST", options, payload);
  }

  async setItemBarcodePrimary(itemId: string, barcodeId: string, rowVersion: string, options: RequestOptions): Promise<ItemBarcodeResponse> {
    return this.request<ItemBarcodeResponse>(`/api/v1/items/${encodeURIComponent(itemId)}/barcodes/${encodeURIComponent(barcodeId)}/primary`, "POST", { ...options, ifMatch: rowVersion });
  }

  async deactivateItemBarcode(itemId: string, barcodeId: string, rowVersion: string, options: RequestOptions): Promise<ItemBarcodeResponse> {
    return this.request<ItemBarcodeResponse>(`/api/v1/items/${encodeURIComponent(itemId)}/barcodes/${encodeURIComponent(barcodeId)}/deactivate`, "POST", { ...options, ifMatch: rowVersion });
  }

  async listItemUnitConversions(itemId: string, options: RequestOptions): Promise<ItemUnitConversionResponse[]> {
    return this.request<ItemUnitConversionResponse[]>(`/api/v1/items/${encodeURIComponent(itemId)}/unit-conversions`, "GET", options);
  }

  async createItemUnitConversion(itemId: string, payload: CreateItemUnitConversionRequest, options: RequestOptions): Promise<ItemUnitConversionResponse> {
    return this.request<ItemUnitConversionResponse>(`/api/v1/items/${encodeURIComponent(itemId)}/unit-conversions`, "POST", options, payload);
  }

  async listUnitConversions(options: RequestOptions): Promise<UnitConversionResponse[]> {
    return this.request<UnitConversionResponse[]>("/api/v1/unit-conversions", "GET", options);
  }

  async createUnitConversion(payload: CreateItemUnitConversionRequest, options: RequestOptions): Promise<UnitConversionResponse> {
    return this.request<UnitConversionResponse>("/api/v1/unit-conversions", "POST", options, payload);
  }

  async listCostSources(options: RequestOptions): Promise<CostSourceResponse[]> {
    return this.request<CostSourceResponse[]>("/api/v1/cost-sources", "GET", options);
  }

  async createCostSource(payload: CostSourceRequest, options: RequestOptions): Promise<CostSourceResponse> {
    return this.request<CostSourceResponse>("/api/v1/cost-sources", "POST", options, payload);
  }

  async updateCostSource(id: string, payload: UpdateCostSourceRequest, options: RequestOptions): Promise<CostSourceResponse> {
    return this.request<CostSourceResponse>(`/api/v1/cost-sources/${encodeURIComponent(id)}`, "PUT", options, payload);
  }

  async deactivateCostSource(id: string, options: RequestOptions): Promise<CostSourceResponse> {
    return this.request<CostSourceResponse>(`/api/v1/cost-sources/${encodeURIComponent(id)}/deactivate`, "POST", options);
  }

  async listCostReviewQueue(query: CostReviewQueueParams, options: RequestOptions): Promise<CostReviewQueueResponse> {
    const params = new URLSearchParams();
    if (query.status) params.set("status", query.status);
    if (query.search) params.set("search", query.search);
    params.set("pageNumber", String(query.pageNumber ?? 1));
    params.set("pageSize", String(query.pageSize ?? 20));
    return this.request<CostReviewQueueResponse>(`/api/v1/cost-review-queue?${params.toString()}`, "GET", options);
  }

  async listEstimateReviewQueue(query: EstimateReviewQueueParams, options: RequestOptions): Promise<EstimateReviewQueueResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    params.set("pageNumber", String(query.pageNumber ?? 1));
    params.set("pageSize", String(query.pageSize ?? 20));
    return this.request<EstimateReviewQueueResponse>(`/api/v1/estimates/review-queue?${params.toString()}`, "GET", options);
  }

  async approveCostRecord(itemId: string, costId: string, rowVersion: string, options: RequestOptions): Promise<CostRecordResponse> {
    return this.request<CostRecordResponse>(`/api/v1/items/${encodeURIComponent(itemId)}/costs/${encodeURIComponent(costId)}/approve`, "POST", { ...options, ifMatch: rowVersion });
  }

  async returnCostRecord(itemId: string, costId: string, rowVersion: string, reason: string, options: RequestOptions): Promise<CostRecordResponse> {
    return this.request<CostRecordResponse>(`/api/v1/items/${encodeURIComponent(itemId)}/costs/${encodeURIComponent(costId)}/return`, "POST", { ...options, ifMatch: rowVersion }, { reason });
  }

  async publishCostRecord(itemId: string, costId: string, rowVersion: string, idempotencyKey: string, options: RequestOptions): Promise<CostRecordResponse> {
    return this.request<CostRecordResponse>(`/api/v1/items/${encodeURIComponent(itemId)}/costs/${encodeURIComponent(costId)}/publish`, "POST", { ...options, ifMatch: rowVersion, idempotencyKey });
  }

  async listItemCosts(itemId: string, options: RequestOptions): Promise<CostRecordResponse[]> {
    return this.request<CostRecordResponse[]>(`/api/v1/items/${encodeURIComponent(itemId)}/costs`, "GET", options);
  }

  async createCostRecord(itemId: string, payload: CreateCostRecordRequest, options: RequestOptions): Promise<CostRecordResponse> {
    return this.request<CostRecordResponse>(`/api/v1/items/${encodeURIComponent(itemId)}/costs`, "POST", options, payload);
  }

  async updateCostRecord(itemId: string, costId: string, payload: UpdateCostRecordRequest, options: RequestOptions): Promise<CostRecordResponse> {
    return this.request<CostRecordResponse>(`/api/v1/items/${encodeURIComponent(itemId)}/costs/${encodeURIComponent(costId)}`, "PUT", options, payload);
  }

  async submitCostRecord(itemId: string, costId: string, rowVersion: string, options: RequestOptions): Promise<CostRecordResponse> {
    return this.request<CostRecordResponse>(`/api/v1/items/${encodeURIComponent(itemId)}/costs/${encodeURIComponent(costId)}/submit`, "POST", { ...options, ifMatch: rowVersion });
  }
}

export const apiClient = new ApiClient();
