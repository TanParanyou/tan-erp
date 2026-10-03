using System.Globalization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TanErp.Api.Authentication;
using TanErp.Api.ErrorHandling;
using TanErp.Api.OpenApi;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.IdentityAccess.CurrentUser.GetCurrentUser;
using TanErp.Infrastructure.Common;
using TanErp.Infrastructure.Configuration;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Production safety guardrails
if (builder.Environment.IsProduction())
{
    if (builder.Configuration.GetValue<bool>("SeedTestData"))
    {
        throw new InvalidOperationException("SeedTestData must not be enabled in production.");
    }
    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FIREBASE_AUTH_EMULATOR_HOST")))
    {
        throw new InvalidOperationException("FIREBASE_AUTH_EMULATOR_HOST must not be set in production.");
    }
}

// Localization configuration: default 'th', supported 'th', 'en'
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var problem = ProblemDetailsMapper.CreateProblem("REQUEST_VALIDATION_FAILED", actionContext.HttpContext);
        problem.Errors = actionContext.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(_ => problem.Detail ?? "")
                    .ToArray());

        return new ObjectResult(problem)
        {
            StatusCode = problem.Status,
            ContentTypes = { "application/problem+json" }
        };
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(OpenApiConfiguration.ConfigureSwaggerGen);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000", "http://127.0.0.1:3000",
            "http://localhost:3005", "http://127.0.0.1:3005")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Database Context
var connectionString = builder.Configuration.GetConnectionString("Database");
if (!builder.Environment.IsEnvironment("Test"))
{
    RequiredConfiguration.Require(connectionString, "ConnectionStrings:Database");
}

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var resolvedConnectionString = RequiredConfiguration.Require(
        sp.GetRequiredService<IConfiguration>().GetConnectionString("Database") ?? connectionString,
        "ConnectionStrings:Database");

    options.UseNpgsql(resolvedConnectionString);
});

builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddSingleton<IClock, TanErp.Infrastructure.Common.SystemClock>();
builder.Services.AddSingleton<IFirebaseTokenVerifier, FirebaseTokenVerifier>();
builder.Services.AddScoped<ICurrentUserReader, CurrentUserReader>();
builder.Services.AddScoped<IFirebaseIdentityLinker, TanErp.Infrastructure.Persistence.FirebaseIdentityLinker>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.IIdentityAdministrationStore, TanErp.Infrastructure.Persistence.IdentityAccess.IdentityAdministrationStore>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.ListUsers.ListAdminUsersHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.GetUser.GetAdminUserHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.CreateUser.CreateAdminUserHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.RenameUser.RenameAdminUserHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.SetUserActive.SetAdminUserActiveHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.UpdateMembership.UpdateAdminMembershipHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.SetMembershipActive.SetAdminMembershipActiveHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.ListRoles.ListAdminRolesHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.AssignRole.AssignAdminRoleHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.RevokeRole.RevokeAdminRoleHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.ListRoleRequests.ListAdminRoleRequestsHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Administration.DecideRoleRequest.DecideAdminRoleRequestHandler>();
builder.Services.AddScoped<IRequestAccessResolver, RequestAccessResolver>();
builder.Services.AddScoped<GetCurrentUserHandler>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Users.IUserReadStore, TanErp.Infrastructure.Persistence.IdentityAccess.UserReadStore>();
builder.Services.AddScoped<TanErp.Application.IdentityAccess.Users.ListUsers.ListUsersHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.ICustomerCreationStore, TanErp.Infrastructure.Persistence.Crm.CustomerCreationStore>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.CreateCustomer.CreateCustomerHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.ICustomerReadStore, TanErp.Infrastructure.Persistence.Crm.CustomerReadStore>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.ListCustomers.ListCustomersHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.GetCustomer.GetCustomerHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.CheckDuplicates.CheckCustomerDuplicatesHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.ICustomerLifecycleStore, TanErp.Infrastructure.Persistence.Crm.CustomerLifecycleStore>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.ActivateCustomer.ActivateCustomerHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.ICustomerUpdateStore, TanErp.Infrastructure.Persistence.Crm.CustomerUpdateStore>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.UpdateCustomer.UpdateCustomerHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.ICustomerContactStore, TanErp.Infrastructure.Persistence.Crm.CustomerContactStore>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.CustomerContactHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.ChangeCustomerStatus.ChangeCustomerStatusHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.ICustomerAddressStore, TanErp.Infrastructure.Persistence.Crm.CustomerAddressStore>();
builder.Services.AddScoped<TanErp.Application.Crm.Customers.CustomerAddressHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Sites.ISiteStore, TanErp.Infrastructure.Persistence.Crm.SiteStore>();
builder.Services.AddScoped<TanErp.Application.Crm.Sites.CreateSite.CreateSiteHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Sites.ListSites.ListSitesHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Sites.ManageCustomerSiteHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.IOpportunityStore, TanErp.Infrastructure.Persistence.Crm.OpportunityStore>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.CreateOpportunity.CreateOpportunityHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.ListOpportunities.ListOpportunitiesHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.GetOpportunity.GetOpportunityHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.QualifyOpportunity.QualifyOpportunityHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.UpdateDraftQGate.UpdateDraftQGateHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.UpdateOpenOpportunity.UpdateOpenOpportunityHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.ReassignOpportunityOwner.ReassignOpportunityOwnerHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.GetOpportunityStageHistory.GetOpportunityStageHistoryHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.WorkImages.AttachWorkImages.AttachWorkImagesHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.WorkImages.ListWorkImages.ListWorkImagesHandler>();
builder.Services.AddScoped<TanErp.Application.Crm.Opportunities.WorkImages.DetachWorkImage.DetachWorkImageHandler>();
builder.Services.AddScoped<TanErp.Application.Surveys.ISiteSurveyStore, TanErp.Infrastructure.Persistence.Surveys.SiteSurveyStore>();
builder.Services.AddScoped<TanErp.Application.Surveys.CreateSiteSurvey.CreateSiteSurveyHandler>();
builder.Services.AddScoped<TanErp.Application.Surveys.GetSiteSurvey.GetSiteSurveyHandler>();
builder.Services.AddScoped<TanErp.Application.Surveys.UpdateSurveyDraft.UpdateSurveyDraftHandler>();
builder.Services.AddScoped<TanErp.Application.Surveys.MarkSurveyReady.MarkSurveyReadyHandler>();
builder.Services.AddScoped<TanErp.Application.Surveys.CloneSurveyRevision.CloneSurveyRevisionHandler>();
builder.Services.AddScoped<TanErp.Application.Surveys.VoidSurveyRevision.VoidSurveyRevisionHandler>();
builder.Services.AddScoped<TanErp.Application.Surveys.ListSurveyTemplateVersions.ListSurveyTemplateVersionsHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.IEstimateStore, TanErp.Infrastructure.Persistence.Estimates.EstimateStore>();
builder.Services.AddScoped<TanErp.Application.Estimates.CreateEstimateDraft.CreateEstimateDraftHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.GetEstimate.GetEstimateHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.UpdateEstimateDraft.UpdateEstimateDraftHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.CalculateEstimate.CalculateEstimateHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.SubmitEstimate.SubmitEstimateHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.ReviewEstimate.ReviewEstimateHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.CreateEstimateRevision.CreateEstimateRevisionHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.CancelEstimate.CancelEstimateHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.IssueQuotation.IssueQuotationHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.AcceptQuotation.AcceptQuotationHandler>();
builder.Services.AddScoped<TanErp.Application.Estimates.GetQuotationDocument.GetQuotationDocumentHandler>();
builder.Services.AddScoped<TanErp.Application.MasterData.Addresses.SearchAddresses.IAddressLookupCache, TanErp.Infrastructure.MasterData.AddressLookupCache>();

builder.Services.AddScoped<TanErp.Application.MasterData.Addresses.SearchAddresses.SearchAddressesHandler>();

// Item Master Catalog & Taxonomy Store
builder.Services.AddScoped<TanErp.Application.Items.IItemStore, TanErp.Infrastructure.Persistence.Items.ItemStore>();
builder.Services.AddScoped<TanErp.Application.Projects.IProjectStore, TanErp.Infrastructure.Persistence.Projects.ProjectStore>();
builder.Services.AddScoped<TanErp.Application.Projects.CreateProjectFromHandover.CreateProjectFromHandoverHandler>();
builder.Services.AddScoped<TanErp.Application.Projects.GetProject.GetProjectHandler>();
builder.Services.AddScoped<TanErp.Application.Projects.GetHandoverSource.GetHandoverSourceHandler>();
builder.Services.AddScoped<TanErp.Application.Projects.Control.IProjectControlStore, TanErp.Infrastructure.Persistence.Projects.ProjectControlStore>();
builder.Services.AddScoped<TanErp.Application.Projects.Control.ProjectControlHandler>();
builder.Services.AddScoped<TanErp.Application.Procurement.IProcurementStore, TanErp.Infrastructure.Persistence.Procurement.ProcurementStore>();
builder.Services.AddScoped<TanErp.Application.Procurement.ProcurementHandler>();
builder.Services.AddScoped<TanErp.Application.Production.IProductionStore, TanErp.Infrastructure.Persistence.Production.ProductionStore>();
builder.Services.AddScoped<TanErp.Application.Production.ProductionHandler>();
builder.Services.AddScoped<TanErp.Application.Mrp.IMrpStore, TanErp.Infrastructure.Persistence.Mrp.MrpStore>();
builder.Services.AddScoped<TanErp.Application.Mrp.MrpHandler>();
builder.Services.AddScoped<TanErp.Infrastructure.Persistence.Inventory.InventoryStore>();
builder.Services.AddScoped<TanErp.Application.Inventory.IInventoryStore>(sp => sp.GetRequiredService<TanErp.Infrastructure.Persistence.Inventory.InventoryStore>());
builder.Services.AddScoped<TanErp.Application.Inventory.IProductionStockPort>(sp => sp.GetRequiredService<TanErp.Infrastructure.Persistence.Inventory.InventoryStore>());
builder.Services.AddScoped<TanErp.Application.Inventory.InventoryHandler>();
builder.Services.AddScoped<TanErp.Application.Projects.ListProjects.ListProjectsHandler>();
builder.Services.AddScoped<TanErp.Application.Items.IItemImportStore, TanErp.Infrastructure.Persistence.Items.ItemImportStore>();
builder.Services.AddScoped<TanErp.Application.Items.Import.PreviewItemImport.PreviewItemImportHandler>();
builder.Services.AddScoped<TanErp.Application.Items.Import.CommitItemImport.CommitItemImportHandler>();
builder.Services.AddScoped<TanErp.Application.Common.Abstractions.IOrganizationBranchReader, TanErp.Infrastructure.Persistence.OrganizationBranchReader>();
builder.Services.AddScoped<TanErp.Application.Items.IItemImageStore, TanErp.Infrastructure.Persistence.Items.ItemImageStore>();
builder.Services.AddScoped<TanErp.Application.Items.ICostRecordStore, TanErp.Infrastructure.Persistence.Items.CostRecordStore>();
builder.Services.AddScoped<TanErp.Application.Items.ICostSourceStore, TanErp.Infrastructure.Persistence.Items.CostSourceStore>();
builder.Services.AddScoped<TanErp.Application.Items.ICostReviewQueueReader, TanErp.Infrastructure.Persistence.Items.CostReviewQueueReader>();
builder.Services.AddScoped<TanErp.Application.Estimates.IEstimateReviewQueueReader, TanErp.Infrastructure.Persistence.Estimates.EstimateReviewQueueReader>();
builder.Services.AddScoped<TanErp.Application.Items.ICostResolver, TanErp.Infrastructure.Persistence.Items.CostResolver>();
builder.Services.AddScoped<TanErp.Application.Items.Catalog.IEstimateCatalogReader, TanErp.Infrastructure.Persistence.Items.EstimateCatalogReader>();

// File Service (reusable across modules)
builder.Services.AddSingleton<TanErp.Application.Files.IFileStorageProvider, TanErp.Infrastructure.Files.LocalFileStorageProvider>();
builder.Services.AddScoped<TanErp.Application.Files.IFileStore, TanErp.Infrastructure.Files.FileStore>();
builder.Services.AddScoped<TanErp.Application.Files.IFileParentAccessResolver, TanErp.Infrastructure.Files.FileParentAccessResolver>();
builder.Services.AddScoped<TanErp.Application.Files.CreateUploadSession.CreateUploadSessionHandler>();
builder.Services.AddScoped<TanErp.Application.Files.CompleteUploadSession.CompleteUploadSessionHandler>();
builder.Services.AddScoped<TanErp.Application.Files.GetFileContent.GetFileContentHandler>();

// Document Numbering Platform Service
builder.Services.AddScoped<TanErp.Application.Common.Abstractions.ISequenceCounter, TanErp.Infrastructure.Persistence.DocumentNumbering.SequenceCounter>();
builder.Services.AddScoped<TanErp.Application.Common.Abstractions.IDocumentNumberGenerator, TanErp.Infrastructure.Persistence.DocumentNumbering.DocumentNumberGenerator>();
builder.Services.AddScoped<TanErp.Application.DocumentNumbering.ListDocumentSequences.ListDocumentSequencesHandler>();
builder.Services.AddScoped<TanErp.Application.DocumentNumbering.UpdateDocumentSequence.UpdateDocumentSequenceHandler>();
builder.Services.AddScoped<TanErp.Application.DocumentNumbering.PreviewDocumentSequence.PreviewDocumentSequenceHandler>();

// Authentication & Authorization
builder.Services.AddAuthentication(FirebaseAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>(
        FirebaseAuthenticationHandler.SchemeName, null);

builder.Services.AddAuthorization();

// Liveness (process is up) and readiness (dependencies usable); see docs/06-operations/observability.md.
builder.Services.AddHealthChecks()
    .AddCheck<TanErp.Api.Health.DatabaseHealthCheck>("database", tags: new[] { "ready" });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var app = builder.Build();

// Localization Middleware
var supportedCultures = new[] { new CultureInfo("th"), new CultureInfo("en") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("th"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
localizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
{
    new AcceptLanguageHeaderRequestCultureProvider()
};
app.UseRequestLocalization(localizationOptions);
app.UseExceptionHandler();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Test"))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = TanErp.Api.Health.HealthResponseWriter.WriteAsync
}).AllowAnonymous();

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready"),
    ResponseWriter = TanErp.Api.Health.HealthResponseWriter.WriteAsync
}).AllowAnonymous();

if (app.Environment.IsEnvironment("Test") && app.Configuration.GetValue<bool>("SeedTestData"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await TestOnlyDataSeeder.SeedAsync(
        db,
        app.Environment.EnvironmentName,
        true,
        app.Configuration.GetValue<bool>("SeedDedicatedEstimateReviewer"),
        app.Configuration.GetValue<bool>("SeedEstimateDemoData"),
        app.Configuration.GetValue<bool>("SeedItemCatalogDemoData"));
    await TanErp.Infrastructure.MasterData.Seed.AddressMasterDataSeeder.SeedAsync(db);
}

app.Run();

namespace TanErp.Api
{
    public partial class Program { }
}
