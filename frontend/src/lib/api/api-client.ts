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
export type CloneSurveyRevisionRequest = components["schemas"]["CloneSurveyRevisionRequest"];
export type VoidSurveyRevisionRequest = components["schemas"]["VoidSurveyRevisionRequest"];
export type SiteSurveyReadyRevisionResponse = components["schemas"]["SiteSurveyReadyRevisionResponse"];
export type SiteSurveyChecklistResultResponse = components["schemas"]["SiteSurveyChecklistResultResponse"];
export type SiteSurveyEvidenceResponse = components["schemas"]["SiteSurveyEvidenceResponse"];
export type UpdateSurveyChecklistRequest = components["schemas"]["UpdateSurveyChecklistRequest"];
export type UpdateSurveyEvidenceRequest = components["schemas"]["UpdateSurveyEvidenceRequest"];
export type SurveyTemplateVersionResponse = components["schemas"]["SurveyTemplateVersionResponse"];
export type SurveyTemplateVersionListResponse = components["schemas"]["SurveyTemplateVersionListResponse"];

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
export type QuotationDocumentResponse = components["schemas"]["QuotationDocumentResponse"];
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

export type AdminUserListParams = NonNullable<paths["/api/v1/admin/users"]["get"]["parameters"]["query"]>;

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
export type ProjectResponse = components["schemas"]["ProjectResponse"];
export type ProjectListResponse = components["schemas"]["ProjectListResponse"];
export type ProjectListItemResponse = components["schemas"]["ProjectListItemResponse"];
export type ProjectHandoverSourceResponse = components["schemas"]["ProjectHandoverSourceResponse"];
export type CreateProjectFromHandoverRequest = components["schemas"]["CreateProjectFromHandoverRequest"];
export type ProjectControlResponse = components["schemas"]["ProjectControlResponse"];
export type ProjectMilestoneResponse = components["schemas"]["ProjectMilestoneResponse"];
export type ProjectChangeOrderResponse = components["schemas"]["ProjectChangeOrderResponse"];
export type ProjectBudgetLineRequest = components["schemas"]["ProjectBudgetLineRequest"];
export type SetProjectPlanRequest = components["schemas"]["SetProjectPlanRequest"];
export type AddProjectMilestoneRequest = components["schemas"]["AddProjectMilestoneRequest"];
export type UpdateProjectMilestoneRequest = components["schemas"]["UpdateProjectMilestoneRequest"];
export type CreateProjectChangeOrderRequest = components["schemas"]["CreateProjectChangeOrderRequest"];
export type ProjectChangeOrderAction = "submit" | "approve" | "reject" | "cancel";
export type SupplierRequest = components["schemas"]["SupplierRequest"];
export type SupplierResponse = components["schemas"]["SupplierResponse"];
export type SupplierListResponse = components["schemas"]["SupplierListResponse"];
export type PurchaseOrderRequest = components["schemas"]["PurchaseOrderRequest"];
export type PurchaseOrderLineRequest = components["schemas"]["PurchaseOrderLineRequest"];
export type PurchaseOrderResponse = components["schemas"]["PurchaseOrderResponse"];
export type PurchaseOrderLineResponse = components["schemas"]["PurchaseOrderLineResponse"];
export type PurchaseOrderListResponse = components["schemas"]["PurchaseOrderListResponse"];
export type PurchaseOrderListItemResponse = components["schemas"]["PurchaseOrderListItemResponse"];
export type GoodsReceiptRequest = components["schemas"]["GoodsReceiptRequest"];
export type GoodsReceiptLineRequest = components["schemas"]["GoodsReceiptLineRequest"];
export type PurchaseOrderAction = "submit" | "approve" | "reject" | "cancel";
export type BomRequest = components["schemas"]["BomRequest"];
export type BomDraftRequest = components["schemas"]["BomDraftRequest"];
export type BomLineRequest = components["schemas"]["BomLineRequest"];
export type BomResponse = components["schemas"]["BomResponse"];
export type BomRevisionResponse = components["schemas"]["BomRevisionResponse"];
export type BomLineResponse = components["schemas"]["BomLineResponse"];
export type BomListResponse = components["schemas"]["BomListResponse"];
export type BomListItemResponse = components["schemas"]["BomListItemResponse"];
export type WorkOrderRequest = components["schemas"]["WorkOrderRequest"];
export type WorkOrderResponse = components["schemas"]["WorkOrderResponse"];
export type WorkOrderMaterialResponse = components["schemas"]["WorkOrderMaterialResponse"];
export type WorkOrderTransactionResponse = components["schemas"]["WorkOrderTransactionResponse"];
export type WorkOrderMaterialsRequest = components["schemas"]["WorkOrderMaterialsRequest"];
export type WorkOrderListResponse = components["schemas"]["WorkOrderListResponse"];
export type WorkOrderListItemResponse = components["schemas"]["WorkOrderListItemResponse"];
export type BomRevisionAction = "approve" | "obsolete";
export type WorkOrderAction = "release" | "cancel";
export type WorkOrderStockOperation = "issues" | "returns";
export interface ListBomsParams {
  search?: string;
  page?: number;
  pageSize?: number;
}
export interface ListWorkOrdersParams {
  search?: string;
  status?: string;
  projectId?: string;
  page?: number;
  pageSize?: number;
}
export type MrpRunRequest = components["schemas"]["MrpRunRequest"];
export type MrpDemandRequest = components["schemas"]["MrpDemandRequest"];
export type MrpConvertRequest = components["schemas"]["MrpConvertRequest"];
export type MrpRunResponse = components["schemas"]["MrpRunResponse"];
export type MrpRecommendationResponse = components["schemas"]["MrpRecommendationResponse"];
export type MrpRunListResponse = components["schemas"]["MrpRunListResponse"];
export type MrpRunListItemResponse = components["schemas"]["MrpRunListItemResponse"];
export type MrpRecommendationDecision = "approve" | "reject";
export interface ListMrpRunsParams {
  search?: string;
  page?: number;
  pageSize?: number;
}
export type QuotationHistoryResponse = components["schemas"]["QuotationHistoryResponse"];
export type QuotationHistoryItemResponse = components["schemas"]["QuotationHistoryItemResponse"];
export type QuotationLifecycleAction = "void" | "amend";
export type CreateAcceptanceLinkRequest = components["schemas"]["CreateAcceptanceLinkRequest"];
export type CreatedAcceptanceLinkResponse = components["schemas"]["CreatedAcceptanceLinkResponse"];
export type AcceptanceLinkResponse = components["schemas"]["AcceptanceLinkResponse"];
export type AcceptanceLinkListResponse = components["schemas"]["AcceptanceLinkListResponse"];
export type PublicAcceptanceViewResponse = components["schemas"]["PublicAcceptanceViewResponse"];
export type PublicAcceptRequest = components["schemas"]["PublicAcceptRequest"];
export type PublicAcceptanceResponse = components["schemas"]["PublicAcceptanceResponse"];
export type InstallationRequest = components["schemas"]["InstallationRequest"];
export type InstallationResponse = components["schemas"]["InstallationResponse"];
export type InstallationsListResponse = components["schemas"]["InstallationsListResponse"];
export type InstallationListItemResponse = components["schemas"]["InstallationListItemResponse"];
export type DefectResponse = components["schemas"]["DefectResponse"];
export type ChecklistItemResponse = components["schemas"]["ChecklistItemResponse"];
export type HandoverRequest = components["schemas"]["HandoverRequest"];
export type WarrantyResponse = components["schemas"]["WarrantyResponse"];
export type AttachmentLinkResponse = components["schemas"]["AttachmentLinkResponse"];
export type AttachmentListResponse = components["schemas"]["AttachmentListResponse"];
export type AttachFilesRequest = components["schemas"]["AttachFilesRequest"];
export type SignatureCaptureResponse = components["schemas"]["SignatureCaptureResponse"];
export type SignatureCaptureListResponse = components["schemas"]["SignatureCaptureListResponse"];
export type CaptureSignatureRequest = components["schemas"]["CaptureSignatureRequest"];
export type NotificationResponse = components["schemas"]["NotificationResponse"];
export type NotificationListResponse = components["schemas"]["NotificationListResponse"];
export type UnreadCountResponse = components["schemas"]["UnreadCountResponse"];
export type MarkAllReadResponse = components["schemas"]["MarkAllReadResponse"];
export interface ListNotificationsParams {
  unreadOnly: boolean;
  page: number;
  pageSize: number;
}
export type WarrantiesListResponse = components["schemas"]["WarrantiesListResponse"];
export type ServiceRequestRequest = components["schemas"]["ServiceRequestRequest"];
export type ServiceRequestResponse = components["schemas"]["ServiceRequestResponse"];
export type ServiceRequestsListResponse = components["schemas"]["ServiceRequestsListResponse"];
export type ServiceRequestListItemResponse = components["schemas"]["ServiceRequestListItemResponse"];
/** One step on an installation; `path` is the endpoint segment after the installation id. */
export type InstallationStep =
  | { kind: "start" }
  | { kind: "ready" }
  | { kind: "cancel"; reason: string }
  | { kind: "handover"; request: HandoverRequest }
  | { kind: "checklist"; itemId: string; done: boolean }
  | { kind: "report-defect"; description: string; severity: string }
  | { kind: "resolve-defect"; defectId: string; note: string }
  | { kind: "verify-defect"; defectId: string }
  | { kind: "reopen-defect"; defectId: string; reason: string };
export type ServiceRequestStep =
  | { kind: "schedule"; date: string }
  | { kind: "start" }
  | { kind: "resolve"; note: string }
  | { kind: "close" }
  | { kind: "reopen"; reason: string };
export interface ListInstallationsParams {
  search?: string;
  status?: string;
  projectId?: string;
  page?: number;
  pageSize?: number;
}
export interface ListWarrantiesParams {
  search?: string;
  state?: string;
  projectId?: string;
  page?: number;
  pageSize?: number;
}
export interface ListServiceRequestsParams {
  search?: string;
  status?: string;
  projectId?: string;
  inWarranty?: boolean;
  page?: number;
  pageSize?: number;
}
export type BillingRequest = components["schemas"]["BillingRequest"];
export type BillingResponse = components["schemas"]["BillingResponse"];
export type BillingListResponse = components["schemas"]["BillingListResponse"];
export type BillingListItemResponse = components["schemas"]["BillingListItemResponse"];
export type PaymentRequest = components["schemas"]["PaymentRequest"];
export type PaymentResponse = components["schemas"]["PaymentResponse"];
export type ProjectBillingSummaryResponse = components["schemas"]["ProjectBillingSummaryResponse"];
export type OutboxListResponse = components["schemas"]["OutboxListResponse"];
export type OutboxMessageResponse = components["schemas"]["OutboxMessageResponse"];
export type DispatchResultResponse = components["schemas"]["DispatchResultResponse"];
export type FinanceReconciliationResponse = components["schemas"]["FinanceReconciliationResponse"];
export interface ListBillingsParams {
  search?: string;
  status?: string;
  projectId?: string;
  page?: number;
  pageSize?: number;
}
export interface ListOutboxParams {
  status?: string;
  kind?: string;
  page?: number;
  pageSize?: number;
}
export type PricingTemplateRequest = components["schemas"]["PricingTemplateRequest"];
export type PricingTemplateResponse = components["schemas"]["PricingTemplateResponse"];
export type PricingTemplateListResponse = components["schemas"]["PricingTemplateListResponse"];
export type PricingTemplateListItemResponse = components["schemas"]["PricingTemplateListItemResponse"];
export type EffectiveTemplateResponse = components["schemas"]["EffectiveTemplateResponse"];
export type QuickEstimateResponse = components["schemas"]["QuickEstimateResponse"];
export type QuickEstimateListResponse = components["schemas"]["QuickEstimateListResponse"];
export type QuickEstimateListItemResponse = components["schemas"]["QuickEstimateListItemResponse"];
export type QuickEstimateDraftRequest = components["schemas"]["QuickEstimateDraftRequest"];
export type TemplateStepName = "submit" | "calibration" | "activate" | "disable";
export interface ListPricingTemplatesParams {
  search?: string;
  status?: string;
  workType?: string;
  page?: number;
  pageSize?: number;
}
export interface ListQuickEstimatesParams {
  search?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}
export type WarehouseRequest = components["schemas"]["WarehouseRequest"];
export type WarehouseResponse = components["schemas"]["WarehouseResponse"];
export type WarehouseListResponse = components["schemas"]["WarehouseListResponse"];
export type StockBalanceResponse = components["schemas"]["StockBalanceResponse"];
export type StockBalanceListResponse = components["schemas"]["StockBalanceListResponse"];
export type StockMovementResponse = components["schemas"]["StockMovementResponse"];
export type StockMovementListResponse = components["schemas"]["StockMovementListResponse"];
export type StockDocumentResponse = components["schemas"]["StockDocumentResponse"];
export type ReservationResponse = components["schemas"]["ReservationResponse"];
export type ReservationListResponse = components["schemas"]["ReservationListResponse"];
export type ReconciliationResponse = components["schemas"]["ReconciliationResponse"];
export type IssueStockRequest = components["schemas"]["IssueStockRequest"];
export type TransferStockRequest = components["schemas"]["TransferStockRequest"];
export type AdjustStockRequest = components["schemas"]["AdjustStockRequest"];
export type ReserveStockRequest = components["schemas"]["ReserveStockRequest"];
export interface ListWarehousesParams {
  search?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}
export interface ListStockBalancesParams {
  warehouseId?: string;
  itemId?: string;
  search?: string;
  inStockOnly?: boolean;
  page?: number;
  pageSize?: number;
}
export interface ListStockMovementsParams {
  warehouseId?: string;
  itemId?: string;
  kind?: string;
  page?: number;
  pageSize?: number;
}
export interface ListSuppliersParams {
  search?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}
export interface ListPurchaseOrdersParams {
  search?: string;
  status?: string;
  supplierId?: string;
  projectId?: string;
  page?: number;
  pageSize?: number;
}
export interface ListProjectsParams {
  search?: string;
  status?: string;
  page?: number;
  pageSize?: number;
}
export type PreviewItemImportRequest = components["schemas"]["PreviewItemImportRequest"];
export type CommitItemImportRequest = components["schemas"]["CommitItemImportRequest"];
export type ItemImportPreviewResponse = components["schemas"]["ItemImportPreviewResponse"];
export type ItemImportRowResponse = components["schemas"]["ItemImportRowResponse"];
export type ItemImportCommitResponse = components["schemas"]["ItemImportCommitResponse"];
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
export type CategoryAttributeTemplateResponse = components["schemas"]["CategoryAttributeTemplateResponse"];
export type CategoryAttributeTemplateDto = components["schemas"]["CategoryAttributeTemplateDto"];
export type CategoryAttributeOptionDto = components["schemas"]["CategoryAttributeOptionDto"];
export type SetCategoryAttributeTemplatesRequest = components["schemas"]["SetCategoryAttributeTemplatesRequest"];
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

    return this.execute<T>(endpoint, method, headers, body, signal);
  }

  /** Sends the request and maps problem-details responses to ApiError; shared by authenticated and public calls. */
  private async execute<T>(
    endpoint: string,
    method: "GET" | "POST" | "PUT" | "DELETE" | "PATCH",
    headers: Record<string, string>,
    body: unknown,
    signal?: AbortSignal
  ): Promise<T> {
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

  async listSurveyTemplateVersions(
    options: RequestOptions
  ): Promise<SurveyTemplateVersionListResponse> {
    return this.request<SurveyTemplateVersionListResponse>(
      "/api/v1/survey-template-versions",
      "GET",
      options
    );
  }

  async cloneSurveyRevision(
    opportunityId: string,
    surveyId: string,
    payload: CloneSurveyRevisionRequest,
    options: RequestOptions
  ): Promise<SiteSurveyRevisionResponse> {
    return this.request<SiteSurveyRevisionResponse>(
      `/api/v1/opportunities/${encodeURIComponent(opportunityId)}/surveys/${encodeURIComponent(surveyId)}/revisions`,
      "POST",
      options,
      payload
    );
  }

  async voidSurveyRevision(
    opportunityId: string,
    surveyId: string,
    revisionId: string,
    payload: VoidSurveyRevisionRequest,
    options: RequestOptions
  ): Promise<SiteSurveyRevisionResponse> {
    return this.request<SiteSurveyRevisionResponse>(
      `/api/v1/opportunities/${encodeURIComponent(opportunityId)}/surveys/${encodeURIComponent(surveyId)}/revisions/${encodeURIComponent(revisionId)}/void`,
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

  async getQuotationHistory(estimateId: string, options: RequestOptions): Promise<QuotationHistoryResponse> {
    return this.request<QuotationHistoryResponse>(`/api/v1/estimates/${encodeURIComponent(estimateId)}/quotations`, "GET", options);
  }

  async quotationLifecycle(quotationId: string, action: QuotationLifecycleAction, rowVersion: string, reason: string, options: RequestOptions): Promise<QuotationHistoryResponse> {
    return this.request<QuotationHistoryResponse>(`/api/v1/quotations/${encodeURIComponent(quotationId)}/${action}`, "POST", { ...options, ifMatch: rowVersion }, { reason });
  }

  async createAcceptanceLink(quotationId: string, payload: CreateAcceptanceLinkRequest, options: RequestOptions): Promise<CreatedAcceptanceLinkResponse> {
    return this.request<CreatedAcceptanceLinkResponse>(`/api/v1/quotations/${encodeURIComponent(quotationId)}/acceptance-links`, "POST", options, payload);
  }

  async listAcceptanceLinks(quotationId: string, options: RequestOptions): Promise<AcceptanceLinkListResponse> {
    return this.request<AcceptanceLinkListResponse>(`/api/v1/quotations/${encodeURIComponent(quotationId)}/acceptance-links`, "GET", options);
  }

  async revokeAcceptanceLink(linkId: string, options: RequestOptions): Promise<AcceptanceLinkResponse> {
    return this.request<AcceptanceLinkResponse>(`/api/v1/acceptance-links/${encodeURIComponent(linkId)}/revoke`, "POST", options);
  }

  /** Customer-facing calls: no ERP login; the token in the path is the credential. */
  async getPublicAcceptance(token: string, locale: "th" | "en", signal?: AbortSignal): Promise<PublicAcceptanceViewResponse> {
    return this.execute<PublicAcceptanceViewResponse>(
      `/api/public/v1/quotation-acceptance/${encodeURIComponent(token)}?locale=${locale}`,
      "GET",
      { Accept: "application/json", "Accept-Language": locale },
      undefined,
      signal
    );
  }

  async acceptPublicQuotation(token: string, payload: PublicAcceptRequest, locale: "th" | "en"): Promise<PublicAcceptanceResponse> {
    return this.execute<PublicAcceptanceResponse>(
      `/api/public/v1/quotation-acceptance/${encodeURIComponent(token)}/accept`,
      "POST",
      { Accept: "application/json", "Accept-Language": locale, "Content-Type": "application/json" },
      payload
    );
  }

  async getQuotationDocument(
    id: string,
    documentLocale: "th" | "en",
    options: RequestOptions
  ): Promise<QuotationDocumentResponse> {
    return this.request<QuotationDocumentResponse>(
      `/api/v1/estimates/${encodeURIComponent(id)}/quotation/document?locale=${documentLocale}`,
      "GET",
      options
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
    if (params.sortBy) query.set("sortBy", params.sortBy);
    if (params.sortOrder) query.set("sortOrder", params.sortOrder);
    if (params.page) query.set("page", String(params.page));
    if (params.limit) query.set("limit", String(params.limit));
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

  async previewItemImport(payload: PreviewItemImportRequest, options: RequestOptions): Promise<ItemImportPreviewResponse> {
    return this.request<ItemImportPreviewResponse>("/api/v1/items/imports/preview", "POST", options, payload);
  }

  async commitItemImport(payload: CommitItemImportRequest, options: RequestOptions): Promise<ItemImportCommitResponse> {
    return this.request<ItemImportCommitResponse>("/api/v1/items/imports/commit", "POST", options, payload);
  }

  async listProjects(options: RequestOptions, query: ListProjectsParams): Promise<ProjectListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.status) params.set("status", query.status);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<ProjectListResponse>(`/api/v1/projects?${params.toString()}`, "GET", options);
  }

  async getProject(id: string, options: RequestOptions): Promise<ProjectResponse> {
    return this.request<ProjectResponse>(`/api/v1/projects/${encodeURIComponent(id)}`, "GET", options);
  }

  async getProjectHandoverSource(opportunityId: string, options: RequestOptions): Promise<ProjectHandoverSourceResponse> {
    return this.request<ProjectHandoverSourceResponse>(
      `/api/v1/projects/handover-source?opportunityId=${encodeURIComponent(opportunityId)}`,
      "GET",
      options
    );
  }

  async createProjectFromHandover(payload: CreateProjectFromHandoverRequest, options: RequestOptions): Promise<ProjectResponse> {
    return this.request<ProjectResponse>("/api/v1/projects", "POST", options, payload);
  }

  async getProjectControl(projectId: string, options: RequestOptions): Promise<ProjectControlResponse> {
    return this.request<ProjectControlResponse>(`/api/v1/projects/${encodeURIComponent(projectId)}/control`, "GET", options);
  }

  async setProjectPlan(projectId: string, rowVersion: string, payload: SetProjectPlanRequest, options: RequestOptions): Promise<ProjectControlResponse> {
    return this.request<ProjectControlResponse>(`/api/v1/projects/${encodeURIComponent(projectId)}/plan`, "PUT", { ...options, ifMatch: rowVersion }, payload);
  }

  async replaceProjectBudget(projectId: string, rowVersion: string, lines: ProjectBudgetLineRequest[], options: RequestOptions): Promise<ProjectControlResponse> {
    return this.request<ProjectControlResponse>(`/api/v1/projects/${encodeURIComponent(projectId)}/budget`, "PUT", { ...options, ifMatch: rowVersion }, { lines });
  }

  async transitionProject(projectId: string, rowVersion: string, targetStatus: string, reason: string | null, options: RequestOptions): Promise<ProjectControlResponse> {
    return this.request<ProjectControlResponse>(`/api/v1/projects/${encodeURIComponent(projectId)}/transitions`, "POST", { ...options, ifMatch: rowVersion }, { targetStatus, reason });
  }

  async addProjectMilestone(projectId: string, payload: AddProjectMilestoneRequest, options: RequestOptions): Promise<ProjectControlResponse> {
    return this.request<ProjectControlResponse>(`/api/v1/projects/${encodeURIComponent(projectId)}/milestones`, "POST", options, payload);
  }

  async updateProjectMilestone(projectId: string, milestoneId: string, payload: UpdateProjectMilestoneRequest, options: RequestOptions): Promise<ProjectControlResponse> {
    return this.request<ProjectControlResponse>(`/api/v1/projects/${encodeURIComponent(projectId)}/milestones/${encodeURIComponent(milestoneId)}`, "PUT", options, payload);
  }

  async completeProjectMilestone(projectId: string, milestoneId: string, expectedVersion: string, options: RequestOptions): Promise<ProjectControlResponse> {
    return this.request<ProjectControlResponse>(`/api/v1/projects/${encodeURIComponent(projectId)}/milestones/${encodeURIComponent(milestoneId)}/complete`, "POST", options, { expectedVersion });
  }

  async deleteProjectMilestone(projectId: string, milestoneId: string, expectedVersion: string, options: RequestOptions): Promise<ProjectControlResponse> {
    return this.request<ProjectControlResponse>(`/api/v1/projects/${encodeURIComponent(projectId)}/milestones/${encodeURIComponent(milestoneId)}/delete`, "POST", options, { expectedVersion });
  }

  async createProjectChangeOrder(projectId: string, payload: CreateProjectChangeOrderRequest, options: RequestOptions): Promise<ProjectControlResponse> {
    return this.request<ProjectControlResponse>(`/api/v1/projects/${encodeURIComponent(projectId)}/change-orders`, "POST", options, payload);
  }

  async projectChangeOrderAction(
    projectId: string,
    changeOrderId: string,
    action: ProjectChangeOrderAction,
    expectedVersion: string,
    note: string | null,
    options: RequestOptions
  ): Promise<ProjectControlResponse> {
    return this.request<ProjectControlResponse>(
      `/api/v1/projects/${encodeURIComponent(projectId)}/change-orders/${encodeURIComponent(changeOrderId)}/${action}`,
      "POST",
      options,
      { expectedVersion, note }
    );
  }

  async listSuppliers(options: RequestOptions, query: ListSuppliersParams): Promise<SupplierListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.status) params.set("status", query.status);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<SupplierListResponse>(`/api/v1/suppliers?${params.toString()}`, "GET", options);
  }

  async getSupplier(id: string, options: RequestOptions): Promise<SupplierResponse> {
    return this.request<SupplierResponse>(`/api/v1/suppliers/${encodeURIComponent(id)}`, "GET", options);
  }

  async createSupplier(payload: SupplierRequest, options: RequestOptions): Promise<SupplierResponse> {
    return this.request<SupplierResponse>("/api/v1/suppliers", "POST", options, payload);
  }

  async updateSupplier(id: string, rowVersion: string, payload: SupplierRequest, options: RequestOptions): Promise<SupplierResponse> {
    return this.request<SupplierResponse>(`/api/v1/suppliers/${encodeURIComponent(id)}`, "PUT", { ...options, ifMatch: rowVersion }, payload);
  }

  async setSupplierActive(id: string, rowVersion: string, active: boolean, options: RequestOptions): Promise<SupplierResponse> {
    return this.request<SupplierResponse>(`/api/v1/suppliers/${encodeURIComponent(id)}/${active ? "activate" : "deactivate"}`, "POST", { ...options, ifMatch: rowVersion });
  }

  async listPurchaseOrders(options: RequestOptions, query: ListPurchaseOrdersParams): Promise<PurchaseOrderListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.status) params.set("status", query.status);
    if (query.supplierId) params.set("supplierId", query.supplierId);
    if (query.projectId) params.set("projectId", query.projectId);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<PurchaseOrderListResponse>(`/api/v1/purchase-orders?${params.toString()}`, "GET", options);
  }

  async getPurchaseOrder(id: string, options: RequestOptions): Promise<PurchaseOrderResponse> {
    return this.request<PurchaseOrderResponse>(`/api/v1/purchase-orders/${encodeURIComponent(id)}`, "GET", options);
  }

  async createPurchaseOrder(payload: PurchaseOrderRequest, options: RequestOptions): Promise<PurchaseOrderResponse> {
    return this.request<PurchaseOrderResponse>("/api/v1/purchase-orders", "POST", options, payload);
  }

  async updatePurchaseOrder(id: string, rowVersion: string, payload: PurchaseOrderRequest, options: RequestOptions): Promise<PurchaseOrderResponse> {
    return this.request<PurchaseOrderResponse>(`/api/v1/purchase-orders/${encodeURIComponent(id)}`, "PUT", { ...options, ifMatch: rowVersion }, payload);
  }

  async purchaseOrderAction(id: string, action: PurchaseOrderAction, rowVersion: string, note: string | null, options: RequestOptions): Promise<PurchaseOrderResponse> {
    return this.request<PurchaseOrderResponse>(`/api/v1/purchase-orders/${encodeURIComponent(id)}/${action}`, "POST", { ...options, ifMatch: rowVersion }, { note });
  }

  async postGoodsReceipt(id: string, payload: GoodsReceiptRequest, options: RequestOptions): Promise<PurchaseOrderResponse> {
    return this.request<PurchaseOrderResponse>(`/api/v1/purchase-orders/${encodeURIComponent(id)}/receipts`, "POST", options, payload);
  }

  async listBoms(options: RequestOptions, query: ListBomsParams): Promise<BomListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<BomListResponse>(`/api/v1/boms?${params.toString()}`, "GET", options);
  }

  async getBom(id: string, options: RequestOptions): Promise<BomResponse> {
    return this.request<BomResponse>(`/api/v1/boms/${encodeURIComponent(id)}`, "GET", options);
  }

  async createBom(payload: BomRequest, options: RequestOptions): Promise<BomResponse> {
    return this.request<BomResponse>("/api/v1/boms", "POST", options, payload);
  }

  async createBomRevision(id: string, payload: BomDraftRequest, options: RequestOptions): Promise<BomResponse> {
    return this.request<BomResponse>(`/api/v1/boms/${encodeURIComponent(id)}/revisions`, "POST", options, payload);
  }

  async updateBomDraft(id: string, revisionId: string, rowVersion: string, payload: BomDraftRequest, options: RequestOptions): Promise<BomResponse> {
    return this.request<BomResponse>(`/api/v1/boms/${encodeURIComponent(id)}/revisions/${encodeURIComponent(revisionId)}`, "PUT", { ...options, ifMatch: rowVersion }, payload);
  }

  async bomRevisionAction(id: string, revisionId: string, action: BomRevisionAction, rowVersion: string, options: RequestOptions): Promise<BomResponse> {
    return this.request<BomResponse>(`/api/v1/boms/${encodeURIComponent(id)}/revisions/${encodeURIComponent(revisionId)}/${action}`, "POST", { ...options, ifMatch: rowVersion });
  }

  async listWorkOrders(options: RequestOptions, query: ListWorkOrdersParams): Promise<WorkOrderListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.status) params.set("status", query.status);
    if (query.projectId) params.set("projectId", query.projectId);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<WorkOrderListResponse>(`/api/v1/work-orders?${params.toString()}`, "GET", options);
  }

  async getWorkOrder(id: string, options: RequestOptions): Promise<WorkOrderResponse> {
    return this.request<WorkOrderResponse>(`/api/v1/work-orders/${encodeURIComponent(id)}`, "GET", options);
  }

  async createWorkOrder(payload: WorkOrderRequest, options: RequestOptions): Promise<WorkOrderResponse> {
    return this.request<WorkOrderResponse>("/api/v1/work-orders", "POST", options, payload);
  }

  async releaseWorkOrder(id: string, rowVersion: string, options: RequestOptions): Promise<WorkOrderResponse> {
    return this.request<WorkOrderResponse>(`/api/v1/work-orders/${encodeURIComponent(id)}/release`, "POST", { ...options, ifMatch: rowVersion });
  }

  async cancelWorkOrder(id: string, rowVersion: string, reason: string, options: RequestOptions): Promise<WorkOrderResponse> {
    return this.request<WorkOrderResponse>(`/api/v1/work-orders/${encodeURIComponent(id)}/cancel`, "POST", { ...options, ifMatch: rowVersion }, { reason });
  }

  async workOrderMaterials(id: string, operation: WorkOrderStockOperation, payload: WorkOrderMaterialsRequest, options: RequestOptions): Promise<WorkOrderResponse> {
    return this.request<WorkOrderResponse>(`/api/v1/work-orders/${encodeURIComponent(id)}/${operation}`, "POST", options, payload);
  }

  async completeWorkOrder(id: string, quantity: number, options: RequestOptions): Promise<WorkOrderResponse> {
    return this.request<WorkOrderResponse>(`/api/v1/work-orders/${encodeURIComponent(id)}/completions`, "POST", options, { quantity });
  }

  async listMrpRuns(options: RequestOptions, query: ListMrpRunsParams): Promise<MrpRunListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<MrpRunListResponse>(`/api/v1/mrp/runs?${params.toString()}`, "GET", options);
  }

  async getMrpRun(id: string, options: RequestOptions): Promise<MrpRunResponse> {
    return this.request<MrpRunResponse>(`/api/v1/mrp/runs/${encodeURIComponent(id)}`, "GET", options);
  }

  async createMrpRun(payload: MrpRunRequest, options: RequestOptions): Promise<MrpRunResponse> {
    return this.request<MrpRunResponse>("/api/v1/mrp/runs", "POST", options, payload);
  }

  async decideMrpRecommendation(runId: string, recommendationId: string, decision: MrpRecommendationDecision, rowVersion: string, options: RequestOptions): Promise<MrpRunResponse> {
    return this.request<MrpRunResponse>(`/api/v1/mrp/runs/${encodeURIComponent(runId)}/recommendations/${encodeURIComponent(recommendationId)}/${decision}`, "POST", { ...options, ifMatch: rowVersion });
  }

  async convertMrpRecommendation(runId: string, recommendationId: string, rowVersion: string, payload: MrpConvertRequest, options: RequestOptions): Promise<MrpRunResponse> {
    return this.request<MrpRunResponse>(`/api/v1/mrp/runs/${encodeURIComponent(runId)}/recommendations/${encodeURIComponent(recommendationId)}/convert`, "POST", { ...options, ifMatch: rowVersion }, payload);
  }

  async listInstallations(options: RequestOptions, query: ListInstallationsParams): Promise<InstallationsListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.status) params.set("status", query.status);
    if (query.projectId) params.set("projectId", query.projectId);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<InstallationsListResponse>(`/api/v1/installations?${params.toString()}`, "GET", options);
  }

  async getInstallation(id: string, options: RequestOptions): Promise<InstallationResponse> {
    return this.request<InstallationResponse>(`/api/v1/installations/${encodeURIComponent(id)}`, "GET", options);
  }

  async createInstallation(payload: InstallationRequest, options: RequestOptions): Promise<InstallationResponse> {
    return this.request<InstallationResponse>("/api/v1/installations", "POST", options, payload);
  }

  async installationStep(id: string, rowVersion: string, step: InstallationStep, options: RequestOptions): Promise<InstallationResponse> {
    const base = `/api/v1/installations/${encodeURIComponent(id)}`;
    const conditional = { ...options, ifMatch: rowVersion };
    switch (step.kind) {
      case "start":
      case "ready":
        return this.request<InstallationResponse>(`${base}/${step.kind}`, "POST", conditional);
      case "cancel":
        return this.request<InstallationResponse>(`${base}/cancel`, "POST", conditional, { reason: step.reason });
      case "handover":
        return this.request<InstallationResponse>(`${base}/handover`, "POST", conditional, step.request);
      case "checklist":
        return this.request<InstallationResponse>(`${base}/checklist/${encodeURIComponent(step.itemId)}`, "PUT", conditional, { done: step.done });
      case "report-defect":
        return this.request<InstallationResponse>(`${base}/defects`, "POST", conditional, { description: step.description, severity: step.severity });
      case "resolve-defect":
        return this.request<InstallationResponse>(`${base}/defects/${encodeURIComponent(step.defectId)}/resolve`, "POST", conditional, { note: step.note });
      case "verify-defect":
        return this.request<InstallationResponse>(`${base}/defects/${encodeURIComponent(step.defectId)}/verify`, "POST", conditional);
      case "reopen-defect":
        return this.request<InstallationResponse>(`${base}/defects/${encodeURIComponent(step.defectId)}/reopen`, "POST", conditional, { reason: step.reason });
    }
  }

  private attachmentOwnerPath(ownerType: string, ownerId: string): string {
    return `/api/v1/attachment-owners/${encodeURIComponent(ownerType)}/${encodeURIComponent(ownerId)}`;
  }

  async listAttachments(ownerType: string, ownerId: string, options: RequestOptions): Promise<AttachmentListResponse> {
    return this.request<AttachmentListResponse>(`${this.attachmentOwnerPath(ownerType, ownerId)}/attachments`, "GET", options);
  }

  async attachFiles(ownerType: string, ownerId: string, payload: AttachFilesRequest, options: RequestOptions): Promise<AttachmentListResponse> {
    return this.request<AttachmentListResponse>(`${this.attachmentOwnerPath(ownerType, ownerId)}/attachments`, "POST", options, payload);
  }

  async unlinkAttachment(ownerType: string, ownerId: string, linkId: string, options: RequestOptions): Promise<void> {
    return this.request<void>(`${this.attachmentOwnerPath(ownerType, ownerId)}/attachments/${encodeURIComponent(linkId)}`, "DELETE", options);
  }

  async listSignatures(ownerType: string, ownerId: string, options: RequestOptions): Promise<SignatureCaptureListResponse> {
    return this.request<SignatureCaptureListResponse>(`${this.attachmentOwnerPath(ownerType, ownerId)}/signatures`, "GET", options);
  }

  async captureSignature(ownerType: string, ownerId: string, payload: CaptureSignatureRequest, options: RequestOptions): Promise<SignatureCaptureResponse> {
    return this.request<SignatureCaptureResponse>(`${this.attachmentOwnerPath(ownerType, ownerId)}/signatures`, "POST", options, payload);
  }

  async listNotifications(options: RequestOptions, query: ListNotificationsParams): Promise<NotificationListResponse> {
    const params = new URLSearchParams();
    params.set("unreadOnly", String(query.unreadOnly));
    params.set("page", String(query.page));
    params.set("pageSize", String(query.pageSize));
    return this.request<NotificationListResponse>(`/api/v1/notifications?${params.toString()}`, "GET", options);
  }

  async getUnreadNotificationCount(options: RequestOptions): Promise<UnreadCountResponse> {
    return this.request<UnreadCountResponse>("/api/v1/notifications/unread-count", "GET", options);
  }

  async markNotificationRead(notificationId: string, options: RequestOptions): Promise<NotificationResponse> {
    return this.request<NotificationResponse>(`/api/v1/notifications/${encodeURIComponent(notificationId)}/read`, "POST", options);
  }

  async markAllNotificationsRead(options: RequestOptions): Promise<MarkAllReadResponse> {
    return this.request<MarkAllReadResponse>("/api/v1/notifications/read-all", "POST", options);
  }

  async listWarranties(options: RequestOptions, query: ListWarrantiesParams): Promise<WarrantiesListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.state) params.set("state", query.state);
    if (query.projectId) params.set("projectId", query.projectId);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<WarrantiesListResponse>(`/api/v1/warranties?${params.toString()}`, "GET", options);
  }

  async listServiceRequests(options: RequestOptions, query: ListServiceRequestsParams): Promise<ServiceRequestsListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.status) params.set("status", query.status);
    if (query.projectId) params.set("projectId", query.projectId);
    if (query.inWarranty !== undefined) params.set("inWarranty", String(query.inWarranty));
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<ServiceRequestsListResponse>(`/api/v1/service-requests?${params.toString()}`, "GET", options);
  }

  async getServiceRequest(id: string, options: RequestOptions): Promise<ServiceRequestResponse> {
    return this.request<ServiceRequestResponse>(`/api/v1/service-requests/${encodeURIComponent(id)}`, "GET", options);
  }

  async createServiceRequest(payload: ServiceRequestRequest, options: RequestOptions): Promise<ServiceRequestResponse> {
    return this.request<ServiceRequestResponse>("/api/v1/service-requests", "POST", options, payload);
  }

  async serviceRequestStep(id: string, rowVersion: string, step: ServiceRequestStep, options: RequestOptions): Promise<ServiceRequestResponse> {
    const url = `/api/v1/service-requests/${encodeURIComponent(id)}/${step.kind}`;
    const conditional = { ...options, ifMatch: rowVersion };
    switch (step.kind) {
      case "schedule":
        return this.request<ServiceRequestResponse>(url, "POST", conditional, { date: step.date });
      case "resolve":
        return this.request<ServiceRequestResponse>(url, "POST", conditional, { note: step.note });
      case "reopen":
        return this.request<ServiceRequestResponse>(url, "POST", conditional, { reason: step.reason });
      case "start":
      case "close":
        return this.request<ServiceRequestResponse>(url, "POST", conditional);
    }
  }

  async listBillings(options: RequestOptions, query: ListBillingsParams): Promise<BillingListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.status) params.set("status", query.status);
    if (query.projectId) params.set("projectId", query.projectId);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<BillingListResponse>(`/api/v1/billings?${params.toString()}`, "GET", options);
  }

  async getBilling(id: string, options: RequestOptions): Promise<BillingResponse> {
    return this.request<BillingResponse>(`/api/v1/billings/${encodeURIComponent(id)}`, "GET", options);
  }

  async getProjectBillingSummary(projectId: string, options: RequestOptions): Promise<ProjectBillingSummaryResponse> {
    return this.request<ProjectBillingSummaryResponse>(`/api/v1/projects/${encodeURIComponent(projectId)}/billing-summary`, "GET", options);
  }

  async createBilling(payload: BillingRequest, options: RequestOptions): Promise<BillingResponse> {
    return this.request<BillingResponse>("/api/v1/billings", "POST", options, payload);
  }

  async voidBilling(id: string, rowVersion: string, reason: string, options: RequestOptions): Promise<BillingResponse> {
    return this.request<BillingResponse>(`/api/v1/billings/${encodeURIComponent(id)}/void`, "POST", { ...options, ifMatch: rowVersion }, { reason });
  }

  async recordPayment(billingId: string, payload: PaymentRequest, options: RequestOptions): Promise<BillingResponse> {
    return this.request<BillingResponse>(`/api/v1/billings/${encodeURIComponent(billingId)}/payments`, "POST", options, payload);
  }

  async reversePayment(billingId: string, paymentId: string, rowVersion: string, reason: string, options: RequestOptions): Promise<BillingResponse> {
    return this.request<BillingResponse>(`/api/v1/billings/${encodeURIComponent(billingId)}/payments/${encodeURIComponent(paymentId)}/reverse`, "POST", { ...options, ifMatch: rowVersion }, { reason });
  }

  async listAccountingOutbox(options: RequestOptions, query: ListOutboxParams): Promise<OutboxListResponse> {
    const params = new URLSearchParams();
    if (query.status) params.set("status", query.status);
    if (query.kind) params.set("kind", query.kind);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<OutboxListResponse>(`/api/v1/finance/outbox?${params.toString()}`, "GET", options);
  }

  async dispatchAccountingOutbox(options: RequestOptions): Promise<DispatchResultResponse> {
    return this.request<DispatchResultResponse>("/api/v1/finance/outbox/dispatch", "POST", options);
  }

  async requeueAccountingMessage(id: string, options: RequestOptions): Promise<OutboxMessageResponse> {
    return this.request<OutboxMessageResponse>(`/api/v1/finance/outbox/${encodeURIComponent(id)}/requeue`, "POST", options);
  }

  async getFinanceReconciliation(options: RequestOptions): Promise<FinanceReconciliationResponse> {
    return this.request<FinanceReconciliationResponse>("/api/v1/finance/reconciliation", "GET", options);
  }

  async listPricingTemplates(options: RequestOptions, query: ListPricingTemplatesParams): Promise<PricingTemplateListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.status) params.set("status", query.status);
    if (query.workType) params.set("workType", query.workType);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<PricingTemplateListResponse>(`/api/v1/pricing-templates?${params.toString()}`, "GET", options);
  }

  async getPricingTemplate(id: string, options: RequestOptions): Promise<PricingTemplateResponse> {
    return this.request<PricingTemplateResponse>(`/api/v1/pricing-templates/${encodeURIComponent(id)}`, "GET", options);
  }

  async listEffectivePricingTemplates(options: RequestOptions): Promise<EffectiveTemplateResponse[]> {
    return this.request<EffectiveTemplateResponse[]>("/api/v1/pricing-templates/effective", "GET", options);
  }

  async createPricingTemplate(payload: PricingTemplateRequest, options: RequestOptions): Promise<PricingTemplateResponse> {
    return this.request<PricingTemplateResponse>("/api/v1/pricing-templates", "POST", options, payload);
  }

  async updatePricingTemplate(id: string, rowVersion: string, payload: PricingTemplateRequest, options: RequestOptions): Promise<PricingTemplateResponse> {
    return this.request<PricingTemplateResponse>(`/api/v1/pricing-templates/${encodeURIComponent(id)}`, "PUT", { ...options, ifMatch: rowVersion }, payload);
  }

  async convertQuickEstimate(id: string, sourceVersion: number, siteSurveyRevisionId: string, options: RequestOptions): Promise<QuickEstimateResponse> {
    return this.request<QuickEstimateResponse>(`/api/v1/quick-estimates/${encodeURIComponent(id)}/conversion`, "POST", options, { sourceVersion, siteSurveyRevisionId });
  }

  async newPricingTemplateVersion(id: string, options: RequestOptions): Promise<PricingTemplateResponse> {
    return this.request<PricingTemplateResponse>(`/api/v1/pricing-templates/${encodeURIComponent(id)}/versions`, "POST", options);
  }

  async pricingTemplateStep(id: string, rowVersion: string, step: TemplateStepName, options: RequestOptions): Promise<PricingTemplateResponse> {
    return this.request<PricingTemplateResponse>(`/api/v1/pricing-templates/${encodeURIComponent(id)}/${step}`, "POST", { ...options, ifMatch: rowVersion });
  }

  async decidePricingTemplate(id: string, rowVersion: string, decision: "approved" | "returned", note: string | null, options: RequestOptions): Promise<PricingTemplateResponse> {
    return this.request<PricingTemplateResponse>(`/api/v1/pricing-templates/${encodeURIComponent(id)}/decision`, "POST", { ...options, ifMatch: rowVersion }, { decision, note });
  }

  async listQuickEstimates(options: RequestOptions, query: ListQuickEstimatesParams): Promise<QuickEstimateListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.status) params.set("status", query.status);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<QuickEstimateListResponse>(`/api/v1/quick-estimates?${params.toString()}`, "GET", options);
  }

  async getQuickEstimate(id: string, options: RequestOptions): Promise<QuickEstimateResponse> {
    return this.request<QuickEstimateResponse>(`/api/v1/quick-estimates/${encodeURIComponent(id)}`, "GET", options);
  }

  async createQuickEstimate(options: RequestOptions): Promise<QuickEstimateResponse> {
    return this.request<QuickEstimateResponse>("/api/v1/quick-estimates", "POST", options, { customerId: null, opportunityId: null });
  }

  async patchQuickEstimateDraft(id: string, rowVersion: string, payload: QuickEstimateDraftRequest, options: RequestOptions): Promise<QuickEstimateResponse> {
    return this.request<QuickEstimateResponse>(`/api/v1/quick-estimates/${encodeURIComponent(id)}/draft`, "PATCH", { ...options, ifMatch: rowVersion }, payload);
  }

  async calculateQuickEstimate(id: string, rowVersion: string, options: RequestOptions): Promise<QuickEstimateResponse> {
    return this.request<QuickEstimateResponse>(`/api/v1/quick-estimates/${encodeURIComponent(id)}/calculate`, "POST", { ...options, ifMatch: rowVersion });
  }

  async submitQuickEstimateReview(id: string, sourceVersion: number, options: RequestOptions): Promise<QuickEstimateResponse> {
    return this.request<QuickEstimateResponse>(`/api/v1/quick-estimates/${encodeURIComponent(id)}/submit-review`, "POST", options, { sourceVersion, note: null });
  }

  async decideQuickEstimateReview(id: string, sourceVersion: number, decision: "approved" | "returned", reasonCode: string, options: RequestOptions): Promise<QuickEstimateResponse> {
    return this.request<QuickEstimateResponse>(`/api/v1/quick-estimates/${encodeURIComponent(id)}/review-decisions`, "POST", options, { sourceVersion, decision, reasonCode, note: null });
  }

  async shareQuickEstimate(id: string, sourceVersion: number, locale: "th" | "en", options: RequestOptions): Promise<QuickEstimateResponse> {
    return this.request<QuickEstimateResponse>(`/api/v1/quick-estimates/${encodeURIComponent(id)}/shares`, "POST", options, { sourceVersion, channel: "onscreen", recipient: null, locale });
  }

  async listWarehouses(options: RequestOptions, query: ListWarehousesParams): Promise<WarehouseListResponse> {
    const params = new URLSearchParams();
    if (query.search) params.set("search", query.search);
    if (query.status) params.set("status", query.status);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<WarehouseListResponse>(`/api/v1/warehouses?${params.toString()}`, "GET", options);
  }

  async createWarehouse(payload: WarehouseRequest, options: RequestOptions): Promise<WarehouseResponse> {
    return this.request<WarehouseResponse>("/api/v1/warehouses", "POST", options, payload);
  }

  async updateWarehouse(id: string, rowVersion: string, payload: WarehouseRequest, options: RequestOptions): Promise<WarehouseResponse> {
    return this.request<WarehouseResponse>(`/api/v1/warehouses/${encodeURIComponent(id)}`, "PUT", { ...options, ifMatch: rowVersion }, payload);
  }

  async setWarehouseActive(id: string, rowVersion: string, active: boolean, options: RequestOptions): Promise<WarehouseResponse> {
    return this.request<WarehouseResponse>(`/api/v1/warehouses/${encodeURIComponent(id)}/${active ? "activate" : "deactivate"}`, "POST", { ...options, ifMatch: rowVersion });
  }

  async listStockBalances(options: RequestOptions, query: ListStockBalancesParams): Promise<StockBalanceListResponse> {
    const params = new URLSearchParams();
    if (query.warehouseId) params.set("warehouseId", query.warehouseId);
    if (query.itemId) params.set("itemId", query.itemId);
    if (query.search) params.set("search", query.search);
    if (query.inStockOnly) params.set("inStockOnly", "true");
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<StockBalanceListResponse>(`/api/v1/inventory/balances?${params.toString()}`, "GET", options);
  }

  async listStockMovements(options: RequestOptions, query: ListStockMovementsParams): Promise<StockMovementListResponse> {
    const params = new URLSearchParams();
    if (query.warehouseId) params.set("warehouseId", query.warehouseId);
    if (query.itemId) params.set("itemId", query.itemId);
    if (query.kind) params.set("kind", query.kind);
    params.set("page", String(query.page ?? 1));
    params.set("pageSize", String(query.pageSize ?? 25));
    return this.request<StockMovementListResponse>(`/api/v1/inventory/movements?${params.toString()}`, "GET", options);
  }

  async getStockReconciliation(options: RequestOptions): Promise<ReconciliationResponse> {
    return this.request<ReconciliationResponse>("/api/v1/inventory/reconciliation", "GET", options);
  }

  async receiveGoodsReceiptIntoStock(goodsReceiptId: string, warehouseId: string, options: RequestOptions): Promise<StockDocumentResponse> {
    return this.request<StockDocumentResponse>("/api/v1/inventory/receipts", "POST", options, { goodsReceiptId, warehouseId });
  }

  async issueStock(payload: IssueStockRequest, options: RequestOptions): Promise<StockDocumentResponse> {
    return this.request<StockDocumentResponse>("/api/v1/inventory/issues", "POST", options, payload);
  }

  async transferStock(payload: TransferStockRequest, options: RequestOptions): Promise<StockDocumentResponse> {
    return this.request<StockDocumentResponse>("/api/v1/inventory/transfers", "POST", options, payload);
  }

  async adjustStock(payload: AdjustStockRequest, options: RequestOptions): Promise<StockDocumentResponse> {
    return this.request<StockDocumentResponse>("/api/v1/inventory/adjustments", "POST", options, payload);
  }

  async reserveStock(payload: ReserveStockRequest, options: RequestOptions): Promise<ReservationResponse> {
    return this.request<ReservationResponse>("/api/v1/inventory/reservations", "POST", options, payload);
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

  async getCategoryAttributeTemplates(id: string, options: RequestOptions): Promise<CategoryAttributeTemplateResponse> {
    return this.request<CategoryAttributeTemplateResponse>(`/api/v1/item-categories/${encodeURIComponent(id)}/attribute-template`, "GET", options);
  }

  async setCategoryAttributeTemplates(id: string, payload: SetCategoryAttributeTemplatesRequest, options: RequestOptions): Promise<CategoryAttributeTemplateResponse> {
    return this.request<CategoryAttributeTemplateResponse>(`/api/v1/item-categories/${encodeURIComponent(id)}/attribute-template`, "PUT", options, payload);
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
