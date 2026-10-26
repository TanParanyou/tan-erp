//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

import 'package:dio/dio.dart';
import 'package:built_value/serializer.dart';
import 'package:tan_erp_api/src/serializers.dart';
import 'package:tan_erp_api/src/auth/api_key_auth.dart';
import 'package:tan_erp_api/src/auth/basic_auth.dart';
import 'package:tan_erp_api/src/auth/bearer_auth.dart';
import 'package:tan_erp_api/src/auth/oauth.dart';
import 'package:tan_erp_api/src/api/address_master_api.dart';
import 'package:tan_erp_api/src/api/admin_approval_limits_api.dart';
import 'package:tan_erp_api/src/api/admin_organization_api.dart';
import 'package:tan_erp_api/src/api/admin_roles_api.dart';
import 'package:tan_erp_api/src/api/admin_users_api.dart';
import 'package:tan_erp_api/src/api/app_version_api.dart';
import 'package:tan_erp_api/src/api/attachments_api.dart';
import 'package:tan_erp_api/src/api/audit_events_api.dart';
import 'package:tan_erp_api/src/api/billings_api.dart';
import 'package:tan_erp_api/src/api/boms_api.dart';
import 'package:tan_erp_api/src/api/branch_rate_overrides_api.dart';
import 'package:tan_erp_api/src/api/branches_api.dart';
import 'package:tan_erp_api/src/api/cost_records_api.dart';
import 'package:tan_erp_api/src/api/cost_review_queue_api.dart';
import 'package:tan_erp_api/src/api/cost_sources_api.dart';
import 'package:tan_erp_api/src/api/current_user_api.dart';
import 'package:tan_erp_api/src/api/customers_api.dart';
import 'package:tan_erp_api/src/api/departments_api.dart';
import 'package:tan_erp_api/src/api/document_sequences_api.dart';
import 'package:tan_erp_api/src/api/document_templates_api.dart';
import 'package:tan_erp_api/src/api/estimate_catalog_api.dart';
import 'package:tan_erp_api/src/api/estimate_review_queue_api.dart';
import 'package:tan_erp_api/src/api/estimates_api.dart';
import 'package:tan_erp_api/src/api/files_api.dart';
import 'package:tan_erp_api/src/api/finance_sync_api.dart';
import 'package:tan_erp_api/src/api/installations_api.dart';
import 'package:tan_erp_api/src/api/inventory_api.dart';
import 'package:tan_erp_api/src/api/item_barcodes_api.dart';
import 'package:tan_erp_api/src/api/item_images_api.dart';
import 'package:tan_erp_api/src/api/item_imports_api.dart';
import 'package:tan_erp_api/src/api/item_tax_categories_api.dart';
import 'package:tan_erp_api/src/api/item_taxonomy_api.dart';
import 'package:tan_erp_api/src/api/item_unit_conversions_api.dart';
import 'package:tan_erp_api/src/api/items_api.dart';
import 'package:tan_erp_api/src/api/job_titles_api.dart';
import 'package:tan_erp_api/src/api/mrp_runs_api.dart';
import 'package:tan_erp_api/src/api/notifications_api.dart';
import 'package:tan_erp_api/src/api/opportunities_api.dart';
import 'package:tan_erp_api/src/api/pricing_templates_api.dart';
import 'package:tan_erp_api/src/api/project_control_api.dart';
import 'package:tan_erp_api/src/api/projects_api.dart';
import 'package:tan_erp_api/src/api/public_quotation_acceptance_api.dart';
import 'package:tan_erp_api/src/api/purchase_orders_api.dart';
import 'package:tan_erp_api/src/api/quick_estimates_api.dart';
import 'package:tan_erp_api/src/api/quotation_acceptance_links_api.dart';
import 'package:tan_erp_api/src/api/quotation_documents_api.dart';
import 'package:tan_erp_api/src/api/quotation_lifecycle_api.dart';
import 'package:tan_erp_api/src/api/service_requests_api.dart';
import 'package:tan_erp_api/src/api/site_surveys_api.dart';
import 'package:tan_erp_api/src/api/sites_api.dart';
import 'package:tan_erp_api/src/api/suppliers_api.dart';
import 'package:tan_erp_api/src/api/survey_master_data_api.dart';
import 'package:tan_erp_api/src/api/survey_template_versions_api.dart';
import 'package:tan_erp_api/src/api/unit_conversions_api.dart';
import 'package:tan_erp_api/src/api/users_api.dart';
import 'package:tan_erp_api/src/api/warehouses_api.dart';
import 'package:tan_erp_api/src/api/warranties_api.dart';
import 'package:tan_erp_api/src/api/work_orders_api.dart';

class TanErpApi {
  static const String basePath = r'http://localhost';

  final Dio dio;
  final Serializers serializers;

  TanErpApi({
    Dio? dio,
    Serializers? serializers,
    String? basePathOverride,
    List<Interceptor>? interceptors,
  })  : this.serializers = serializers ?? standardSerializers,
        this.dio = dio ??
            Dio(BaseOptions(
              baseUrl: basePathOverride ?? basePath,
              connectTimeout: const Duration(milliseconds: 5000),
              receiveTimeout: const Duration(milliseconds: 3000),
            )) {
    if (interceptors == null) {
      this.dio.interceptors.addAll([
        OAuthInterceptor(),
        BasicAuthInterceptor(),
        BearerAuthInterceptor(),
        ApiKeyAuthInterceptor(),
      ]);
    } else {
      this.dio.interceptors.addAll(interceptors);
    }
  }

  void setOAuthToken(String name, String token) {
    if (this.dio.interceptors.any((i) => i is OAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is OAuthInterceptor) as OAuthInterceptor).tokens[name] = token;
    }
  }

  /// Removes the OAuth token associated with the given [name].
  ///
  /// If no [OAuthInterceptor] is registered or no token exists for the given
  /// [name], this method has no effect.
  void removeOAuthToken(String name) {
    if (this.dio.interceptors.any((i) => i is OAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is OAuthInterceptor) as OAuthInterceptor).tokens.remove(name);
    }
  }

  void setBearerAuth(String name, String token) {
    if (this.dio.interceptors.any((i) => i is BearerAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is BearerAuthInterceptor) as BearerAuthInterceptor).tokens[name] = token;
    }
  }

  /// Removes the bearer authentication token associated with the given [name].
  ///
  /// If no [BearerAuthInterceptor] is registered or no token exists for the
  /// given [name], this method has no effect.
  void removeBearerAuth(String name) {
    if (this.dio.interceptors.any((i) => i is BearerAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is BearerAuthInterceptor) as BearerAuthInterceptor).tokens.remove(name);
    }
  }

  void setBasicAuth(String name, String username, String password) {
    if (this.dio.interceptors.any((i) => i is BasicAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is BasicAuthInterceptor) as BasicAuthInterceptor).authInfo[name] = BasicAuthInfo(username, password);
    }
  }

  /// Removes the basic authentication credentials associated with the given [name].
  ///
  /// If no [BasicAuthInterceptor] is registered or no credentials exist for the
  /// given [name], this method has no effect.
  void removeBasicAuth(String name) {
    if (this.dio.interceptors.any((i) => i is BasicAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((i) => i is BasicAuthInterceptor) as BasicAuthInterceptor).authInfo.remove(name);
    }
  }

  void setApiKey(String name, String apiKey) {
    if (this.dio.interceptors.any((i) => i is ApiKeyAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((element) => element is ApiKeyAuthInterceptor) as ApiKeyAuthInterceptor).apiKeys[name] = apiKey;
    }
  }

  /// Removes the API key associated with the given [name].
  ///
  /// If no [ApiKeyAuthInterceptor] is registered or no API key exists for the
  /// given [name], this method has no effect.
  void removeApiKey(String name) {
    if (this.dio.interceptors.any((i) => i is ApiKeyAuthInterceptor)) {
      (this.dio.interceptors.firstWhere((element) => element is ApiKeyAuthInterceptor) as ApiKeyAuthInterceptor).apiKeys.remove(name);
    }
  }

  /// Get AddressMasterApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  AddressMasterApi getAddressMasterApi() {
    return AddressMasterApi(dio, serializers);
  }

  /// Get AdminApprovalLimitsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  AdminApprovalLimitsApi getAdminApprovalLimitsApi() {
    return AdminApprovalLimitsApi(dio, serializers);
  }

  /// Get AdminOrganizationApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  AdminOrganizationApi getAdminOrganizationApi() {
    return AdminOrganizationApi(dio, serializers);
  }

  /// Get AdminRolesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  AdminRolesApi getAdminRolesApi() {
    return AdminRolesApi(dio, serializers);
  }

  /// Get AdminUsersApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  AdminUsersApi getAdminUsersApi() {
    return AdminUsersApi(dio, serializers);
  }

  /// Get AppVersionApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  AppVersionApi getAppVersionApi() {
    return AppVersionApi(dio, serializers);
  }

  /// Get AttachmentsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  AttachmentsApi getAttachmentsApi() {
    return AttachmentsApi(dio, serializers);
  }

  /// Get AuditEventsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  AuditEventsApi getAuditEventsApi() {
    return AuditEventsApi(dio, serializers);
  }

  /// Get BillingsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  BillingsApi getBillingsApi() {
    return BillingsApi(dio, serializers);
  }

  /// Get BomsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  BomsApi getBomsApi() {
    return BomsApi(dio, serializers);
  }

  /// Get BranchRateOverridesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  BranchRateOverridesApi getBranchRateOverridesApi() {
    return BranchRateOverridesApi(dio, serializers);
  }

  /// Get BranchesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  BranchesApi getBranchesApi() {
    return BranchesApi(dio, serializers);
  }

  /// Get CostRecordsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  CostRecordsApi getCostRecordsApi() {
    return CostRecordsApi(dio, serializers);
  }

  /// Get CostReviewQueueApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  CostReviewQueueApi getCostReviewQueueApi() {
    return CostReviewQueueApi(dio, serializers);
  }

  /// Get CostSourcesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  CostSourcesApi getCostSourcesApi() {
    return CostSourcesApi(dio, serializers);
  }

  /// Get CurrentUserApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  CurrentUserApi getCurrentUserApi() {
    return CurrentUserApi(dio, serializers);
  }

  /// Get CustomersApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  CustomersApi getCustomersApi() {
    return CustomersApi(dio, serializers);
  }

  /// Get DepartmentsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  DepartmentsApi getDepartmentsApi() {
    return DepartmentsApi(dio, serializers);
  }

  /// Get DocumentSequencesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  DocumentSequencesApi getDocumentSequencesApi() {
    return DocumentSequencesApi(dio, serializers);
  }

  /// Get DocumentTemplatesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  DocumentTemplatesApi getDocumentTemplatesApi() {
    return DocumentTemplatesApi(dio, serializers);
  }

  /// Get EstimateCatalogApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  EstimateCatalogApi getEstimateCatalogApi() {
    return EstimateCatalogApi(dio, serializers);
  }

  /// Get EstimateReviewQueueApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  EstimateReviewQueueApi getEstimateReviewQueueApi() {
    return EstimateReviewQueueApi(dio, serializers);
  }

  /// Get EstimatesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  EstimatesApi getEstimatesApi() {
    return EstimatesApi(dio, serializers);
  }

  /// Get FilesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  FilesApi getFilesApi() {
    return FilesApi(dio, serializers);
  }

  /// Get FinanceSyncApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  FinanceSyncApi getFinanceSyncApi() {
    return FinanceSyncApi(dio, serializers);
  }

  /// Get InstallationsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  InstallationsApi getInstallationsApi() {
    return InstallationsApi(dio, serializers);
  }

  /// Get InventoryApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  InventoryApi getInventoryApi() {
    return InventoryApi(dio, serializers);
  }

  /// Get ItemBarcodesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ItemBarcodesApi getItemBarcodesApi() {
    return ItemBarcodesApi(dio, serializers);
  }

  /// Get ItemImagesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ItemImagesApi getItemImagesApi() {
    return ItemImagesApi(dio, serializers);
  }

  /// Get ItemImportsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ItemImportsApi getItemImportsApi() {
    return ItemImportsApi(dio, serializers);
  }

  /// Get ItemTaxCategoriesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ItemTaxCategoriesApi getItemTaxCategoriesApi() {
    return ItemTaxCategoriesApi(dio, serializers);
  }

  /// Get ItemTaxonomyApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ItemTaxonomyApi getItemTaxonomyApi() {
    return ItemTaxonomyApi(dio, serializers);
  }

  /// Get ItemUnitConversionsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ItemUnitConversionsApi getItemUnitConversionsApi() {
    return ItemUnitConversionsApi(dio, serializers);
  }

  /// Get ItemsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ItemsApi getItemsApi() {
    return ItemsApi(dio, serializers);
  }

  /// Get JobTitlesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  JobTitlesApi getJobTitlesApi() {
    return JobTitlesApi(dio, serializers);
  }

  /// Get MrpRunsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  MrpRunsApi getMrpRunsApi() {
    return MrpRunsApi(dio, serializers);
  }

  /// Get NotificationsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  NotificationsApi getNotificationsApi() {
    return NotificationsApi(dio, serializers);
  }

  /// Get OpportunitiesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  OpportunitiesApi getOpportunitiesApi() {
    return OpportunitiesApi(dio, serializers);
  }

  /// Get PricingTemplatesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  PricingTemplatesApi getPricingTemplatesApi() {
    return PricingTemplatesApi(dio, serializers);
  }

  /// Get ProjectControlApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ProjectControlApi getProjectControlApi() {
    return ProjectControlApi(dio, serializers);
  }

  /// Get ProjectsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ProjectsApi getProjectsApi() {
    return ProjectsApi(dio, serializers);
  }

  /// Get PublicQuotationAcceptanceApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  PublicQuotationAcceptanceApi getPublicQuotationAcceptanceApi() {
    return PublicQuotationAcceptanceApi(dio, serializers);
  }

  /// Get PurchaseOrdersApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  PurchaseOrdersApi getPurchaseOrdersApi() {
    return PurchaseOrdersApi(dio, serializers);
  }

  /// Get QuickEstimatesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  QuickEstimatesApi getQuickEstimatesApi() {
    return QuickEstimatesApi(dio, serializers);
  }

  /// Get QuotationAcceptanceLinksApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  QuotationAcceptanceLinksApi getQuotationAcceptanceLinksApi() {
    return QuotationAcceptanceLinksApi(dio, serializers);
  }

  /// Get QuotationDocumentsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  QuotationDocumentsApi getQuotationDocumentsApi() {
    return QuotationDocumentsApi(dio, serializers);
  }

  /// Get QuotationLifecycleApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  QuotationLifecycleApi getQuotationLifecycleApi() {
    return QuotationLifecycleApi(dio, serializers);
  }

  /// Get ServiceRequestsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  ServiceRequestsApi getServiceRequestsApi() {
    return ServiceRequestsApi(dio, serializers);
  }

  /// Get SiteSurveysApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  SiteSurveysApi getSiteSurveysApi() {
    return SiteSurveysApi(dio, serializers);
  }

  /// Get SitesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  SitesApi getSitesApi() {
    return SitesApi(dio, serializers);
  }

  /// Get SuppliersApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  SuppliersApi getSuppliersApi() {
    return SuppliersApi(dio, serializers);
  }

  /// Get SurveyMasterDataApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  SurveyMasterDataApi getSurveyMasterDataApi() {
    return SurveyMasterDataApi(dio, serializers);
  }

  /// Get SurveyTemplateVersionsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  SurveyTemplateVersionsApi getSurveyTemplateVersionsApi() {
    return SurveyTemplateVersionsApi(dio, serializers);
  }

  /// Get UnitConversionsApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  UnitConversionsApi getUnitConversionsApi() {
    return UnitConversionsApi(dio, serializers);
  }

  /// Get UsersApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  UsersApi getUsersApi() {
    return UsersApi(dio, serializers);
  }

  /// Get WarehousesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  WarehousesApi getWarehousesApi() {
    return WarehousesApi(dio, serializers);
  }

  /// Get WarrantiesApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  WarrantiesApi getWarrantiesApi() {
    return WarrantiesApi(dio, serializers);
  }

  /// Get WorkOrdersApi instance, base route and serializer can be overridden by a given but be careful,
  /// by doing that all interceptors will not be executed
  WorkOrdersApi getWorkOrdersApi() {
    return WorkOrdersApi(dio, serializers);
  }
}
