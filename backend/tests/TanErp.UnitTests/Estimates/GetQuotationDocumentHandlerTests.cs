using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Common.Security;
using TanErp.Application.Estimates;
using TanErp.Application.Estimates.GetQuotationDocument;
using Xunit;

namespace TanErp.UnitTests.Estimates;

public class GetQuotationDocumentHandlerTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();
    private static readonly Guid OtherBranchId = Guid.NewGuid();
    private static readonly Guid MembershipId = Guid.NewGuid();
    private static readonly Guid ActorUserId = Guid.NewGuid();
    private const string FirebaseUid = "test-uid-123";

    private sealed class FakeAccessResolver : IRequestAccessResolver
    {
        public Result<RequestAccessContext> ResultToReturn { get; set; } =
            Result<RequestAccessContext>.Success(new RequestAccessContext(
                ActorUserId, MembershipId, OrgId, BranchId,
                "quotations.read",
                "Branch"));

        public Task<Result<RequestAccessContext>> ResolveAsync(
            string firebaseUid,
            Guid membershipId,
            string requiredPermission,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(ResultToReturn);
    }

    private sealed class FakeEstimateStore : IEstimateStore
    {
        public Result<QuotationDocumentProjection>? DocumentResultToReturn { get; set; }
        public string? CapturedLocale { get; private set; }

        public Task<Result<QuotationDocumentProjection>> GetQuotationDocumentAsync(
            Guid organizationId,
            Guid estimateId,
            string locale,
            CancellationToken cancellationToken)
        {
            CapturedLocale = locale;
            return Task.FromResult(DocumentResultToReturn ??
                Result<QuotationDocumentProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Not found.")));
        }

        public Task<Result<EstimateDetailProjection>> CreateRevisionAsync(Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, string reason, Guid actorUserId, string keyHash, string payloadHash, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<Result<EstimateDetailProjection>> CancelAsync(Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, string reason, Guid actorUserId, Guid actorMembershipId, string keyHash, string payloadHash, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<Result<EstimateDetailProjection>> CreateDraftAsync(Guid organizationId, Guid opportunityId, Guid siteSurveyRevisionId, string currency, Guid actorUserId, string keyHash, string payloadHash, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<EstimateDetailProjection?> GetByIdAsync(Guid organizationId, Guid estimateId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<EstimateDetailProjection?> GetByOpportunityIdAsync(Guid organizationId, Guid opportunityId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<IReadOnlyList<EstimateCalculationSnapshotProjection>> GetCalculationSnapshotsAsync(Guid organizationId, Guid estimateId, Guid revisionId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<EstimateRevisionProjection> UpdateDraftAsync(Guid organizationId, Guid estimateId, Guid revisionId, Guid expectedRevisionVersion, IReadOnlyList<EstimateSectionDraftDto> sections, Guid actorUserId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<EstimateRevisionProjection> CalculateAsync(Guid organizationId, Guid estimateId, Guid revisionId, Guid expectedRevisionVersion, TanErp.Domain.Estimates.EstimateDiscount discount, Guid actorUserId, string keyHash, string payloadHash, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<Result<EstimateDetailProjection>> SubmitAsync(Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, int revisionNo, int calculationVersion, string? note, Guid actorUserId, string keyHash, string payloadHash, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<Result<EstimateDetailProjection>> ReviewAsync(Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, int revisionNo, string decision, string? reasonCode, string? note, Guid actorUserId, Guid reviewerMembershipId, string keyHash, string payloadHash, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<Result<QuotationDetailProjection>> IssueQuotationAsync(Guid organizationId, Guid estimateId, Guid expectedEstimateVersion, Guid expectedOpportunityVersion, Guid actorUserId, string keyHash, string payloadHash, string traceId, CancellationToken cancellationToken) => throw new NotImplementedException();
        public Task<Result<AcceptQuotationProjection>> AcceptQuotationAsync(Guid organizationId, Guid estimateId, Guid expectedOpportunityVersion, string? decisionNote, Guid actorUserId, string keyHash, string payloadHash, string traceId, CancellationToken cancellationToken) => throw new NotImplementedException();
    }

    [Fact]
    public async Task HandleAsync_WhenAccessResolverFails_ReturnsFailure()
    {
        var fakeResolver = new FakeAccessResolver
        {
            ResultToReturn = Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "Access denied."))
        };
        var fakeStore = new FakeEstimateStore();
        var handler = new GetQuotationDocumentHandler(fakeResolver, fakeStore);

        var query = new GetQuotationDocumentQuery(FirebaseUid, MembershipId, Guid.NewGuid(), "th");
        var result = await handler.HandleAsync(query);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_WhenStoreReturnsFailure_ReturnsFailure()
    {
        var fakeResolver = new FakeAccessResolver();
        var fakeStore = new FakeEstimateStore
        {
            DocumentResultToReturn = Result<QuotationDocumentProjection>.Failure(new Error("RESOURCE_NOT_FOUND", "Quotation not found."))
        };
        var handler = new GetQuotationDocumentHandler(fakeResolver, fakeStore);

        var estimateId = Guid.NewGuid();
        var query = new GetQuotationDocumentQuery(FirebaseUid, MembershipId, estimateId, "th");
        var result = await handler.HandleAsync(query);

        Assert.True(result.IsFailure);
        Assert.Equal("RESOURCE_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_WhenUserLacksBranchAccess_ReturnsResourceNotFound()
    {
        var fakeResolver = new FakeAccessResolver();
        var dummyDoc = CreateDummyDocument(OtherBranchId, "th");
        var fakeStore = new FakeEstimateStore
        {
            DocumentResultToReturn = Result<QuotationDocumentProjection>.Success(dummyDoc)
        };
        var handler = new GetQuotationDocumentHandler(fakeResolver, fakeStore);

        var estimateId = Guid.NewGuid();
        var query = new GetQuotationDocumentQuery(FirebaseUid, MembershipId, estimateId, "th");
        var result = await handler.HandleAsync(query);

        Assert.True(result.IsFailure);
        Assert.Equal("RESOURCE_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task HandleAsync_WhenValid_ReturnsDocumentProjection_AndNormalizesLocale()
    {
        var fakeResolver = new FakeAccessResolver();
        var dummyDoc = CreateDummyDocument(BranchId, "en");
        var fakeStore = new FakeEstimateStore
        {
            DocumentResultToReturn = Result<QuotationDocumentProjection>.Success(dummyDoc)
        };
        var handler = new GetQuotationDocumentHandler(fakeResolver, fakeStore);

        var estimateId = Guid.NewGuid();
        var query = new GetQuotationDocumentQuery(FirebaseUid, MembershipId, estimateId, " EN ");
        var result = await handler.HandleAsync(query);

        Assert.True(result.IsSuccess);
        Assert.Equal("QT-2026-0001", result.Value!.Number);
        Assert.Equal("en", fakeStore.CapturedLocale);
    }

    private static QuotationDocumentProjection CreateDummyDocument(Guid branchId, string locale) =>
        new(
            branchId,
            "QT-2026-0001",
            DateTimeOffset.UtcNow,
            "THB",
            locale,
            false,
            new QuotationCustomerProjection("organization", "บริษัท ตัวอย่าง จำกัด", "บริษัท ตัวอย่าง จำกัด", "Sample Co., Ltd.", "บริษัท ตัวอย่าง จำกัด", "0105550000000", "00000", null),
            new List<QuotationSectionProjection>(),
            new QuotationTotalsProjection(1000m, "none", 0m, 0m, 1000m, 70m, 1070m));
}
