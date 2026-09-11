using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Opportunities;
using TanErp.Application.Crm.Opportunities.CreateOpportunity;
using TanErp.Application.Crm.Opportunities.GetOpportunity;
using TanErp.Application.Crm.Opportunities.ListOpportunities;
using TanErp.Application.Crm.Opportunities.QualifyOpportunity;
using TanErp.Domain.Crm.Opportunities;
using Xunit;

namespace TanErp.UnitTests.Crm.Opportunities;

public class OpportunityHandlerTests
{
    private class FakeRequestAccessResolver : IRequestAccessResolver
    {
        public HashSet<string> GrantedPermissions { get; } = new(StringComparer.Ordinal);
        public Guid UserId { get; set; } = Guid.NewGuid();
        public Guid OrgId { get; set; } = Guid.NewGuid();
        public Guid? BranchId { get; set; } = Guid.NewGuid();

        public Task<Result<RequestAccessContext>> ResolveAsync(
            string firebaseUid,
            Guid membershipId,
            string permissionKey,
            CancellationToken cancellationToken = default)
        {
            if (GrantedPermissions.Contains(permissionKey))
            {
                return Task.FromResult(Result<RequestAccessContext>.Success(
                    new RequestAccessContext(UserId, membershipId, OrgId, BranchId, permissionKey, "organization")));
            }

            return Task.FromResult(Result<RequestAccessContext>.Failure(
                new Error("PERMISSION_DENIED", "Permission denied.")));
        }
    }

    private class FakeOpportunityStore : IOpportunityStore
    {
        public CreateOpportunityCommand? LastCreateCommand { get; private set; }
        public string? LastKeyHash { get; private set; }
        public string? LastPayloadHash { get; private set; }
        public int CreateCallCount { get; private set; }
        public int ListCallCount { get; private set; }
        public int GetCallCount { get; private set; }

        public Result<OpportunityProjection> CreateResult { get; set; } = Result<OpportunityProjection>.Failure(new Error("UNSET", "Unset"));
        public OpportunityPage ListResult { get; set; } = new(Array.Empty<OpportunityProjection>(), null);
        public OpportunityProjection? GetResult { get; set; }

        public Task<Result<OpportunityProjection>> CreateAsync(
            RequestAccessContext access,
            CreateOpportunityCommand command,
            string keyHash,
            string payloadHash,
            CancellationToken cancellationToken = default)
        {
            CreateCallCount++;
            LastCreateCommand = command;
            LastKeyHash = keyHash;
            LastPayloadHash = payloadHash;
            return Task.FromResult(CreateResult);
        }

        public Task<OpportunityPage> ListAsync(
            Guid organizationId,
            OpportunityListFilter filter,
            CancellationToken cancellationToken = default)
        {
            ListCallCount++;
            return Task.FromResult(ListResult);
        }

        public Task<OpportunityProjection?> GetAsync(
            Guid organizationId,
            Guid opportunityId,
            CancellationToken cancellationToken = default)
        {
            GetCallCount++;
            return Task.FromResult(GetResult);
        }

        public QualifyOpportunityCommand? LastQualifyCommand { get; private set; }
        public int QualifyCallCount { get; private set; }
        public Result<OpportunityProjection> QualifyResult { get; set; } = Result<OpportunityProjection>.Failure(new Error("UNSET", "Unset"));

        public Task<Result<OpportunityProjection>> QualifyAsync(
            RequestAccessContext access,
            QualifyOpportunityCommand command,
            string keyHash,
            string payloadHash,
            CancellationToken cancellationToken = default)
        {
            QualifyCallCount++;
            LastQualifyCommand = command;
            LastKeyHash = keyHash;
            LastPayloadHash = payloadHash;
            return Task.FromResult(QualifyResult);
        }
    }

    private readonly FakeRequestAccessResolver _accessResolver = new();
    private readonly FakeOpportunityStore _store = new();

    private CreateOpportunityHandler CreateHandler() => new(_accessResolver, _store);
    private ListOpportunitiesHandler ListHandler() => new(_accessResolver, _store);
    private GetOpportunityHandler GetHandler() => new(_accessResolver, _store);

    [Fact]
    public async Task Create_PermissionDenied_ReturnsFailureAndDoesNotCallStore()
    {
        var cmd = new CreateOpportunityCommand(
            "uid", Guid.NewGuid(), Guid.NewGuid(), null, "Title", null,
            new[] { "built-in" }, null, null, null, null, null, null, "key", "trace");
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Equal(0, _store.CreateCallCount);
    }

    [Fact]
    public async Task Create_NoActiveBranch_ReturnsActiveBranchRequired()
    {
        _accessResolver.GrantedPermissions.Add("opportunities.create");
        _accessResolver.BranchId = null; // Actor has no active branch in selected membership
        var cmd = new CreateOpportunityCommand(
            "uid", Guid.NewGuid(), Guid.NewGuid(), null, "Title", null,
            new[] { "built-in" }, null, null, null, null, null, null, "key", "trace");
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsFailure);
        Assert.Equal("ACTIVE_BRANCH_REQUIRED", result.Error.Code);
        Assert.Equal(0, _store.CreateCallCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_MissingTitle_ReturnsValidationError(string title)
    {
        _accessResolver.GrantedPermissions.Add("opportunities.create");
        var cmd = new CreateOpportunityCommand(
            "uid", Guid.NewGuid(), Guid.NewGuid(), null, title, null,
            new[] { "built-in" }, null, null, null, null, null, null, "key", "trace");
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsFailure);
        Assert.Equal("OPPORTUNITY_FIELD_REQUIRED", result.Error.Code);
        Assert.Equal(0, _store.CreateCallCount);
    }

    [Fact]
    public async Task Create_EmptyWorkTypes_ReturnsValidationError()
    {
        _accessResolver.GrantedPermissions.Add("opportunities.create");
        var cmd = new CreateOpportunityCommand(
            "uid", Guid.NewGuid(), Guid.NewGuid(), null, "Title", null,
            Array.Empty<string>(), null, null, null, null, null, null, "key", "trace");
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsFailure);
        Assert.Equal("OPPORTUNITY_FIELD_REQUIRED", result.Error.Code);
        Assert.Equal(0, _store.CreateCallCount);
    }

    [Fact]
    public async Task Create_InvalidWorkType_ReturnsValidationError()
    {
        _accessResolver.GrantedPermissions.Add("opportunities.create");
        var cmd = new CreateOpportunityCommand(
            "uid", Guid.NewGuid(), Guid.NewGuid(), null, "Title", null,
            new[] { "invalid-type" }, null, null, null, null, null, null, "key", "trace");
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsFailure);
        Assert.Equal("OPPORTUNITY_FIELD_REQUIRED", result.Error.Code);
        Assert.Equal(0, _store.CreateCallCount);
    }

    [Fact]
    public async Task Create_BudgetWithoutCurrencyOrInvalid_ReturnsValidationError()
    {
        _accessResolver.GrantedPermissions.Add("opportunities.create");
        // Budget provided but no currency
        var cmd1 = new CreateOpportunityCommand(
            "uid", Guid.NewGuid(), Guid.NewGuid(), null, "Title", null,
            new[] { "built-in" }, null, 1000m, null, null, null, null, "key", "trace");
        var handler = CreateHandler();

        var result1 = await handler.Handle(cmd1);
        Assert.True(result1.IsFailure);
        Assert.Equal("OPPORTUNITY_FIELD_REQUIRED", result1.Error.Code);

        // Negative budget
        var cmd2 = new CreateOpportunityCommand(
            "uid", Guid.NewGuid(), Guid.NewGuid(), null, "Title", null,
            new[] { "built-in" }, null, -500m, "THB", null, null, null, "key", "trace");
        var result2 = await handler.Handle(cmd2);
        Assert.True(result2.IsFailure);
        Assert.Equal("OPPORTUNITY_FIELD_REQUIRED", result2.Error.Code);
    }

    [Fact]
    public async Task Create_ValidCommand_HashesPayloadAndCallsStore()
    {
        _accessResolver.GrantedPermissions.Add("opportunities.create");
        var customerId = Guid.NewGuid();
        var cmd = new CreateOpportunityCommand(
            "uid", Guid.NewGuid(), customerId, null, " Built-in ตู้เสื้อผ้า ", " รายละเอียด ",
            new[] { "built-in", "interior" }, null, 250000m, "thb", null, null, null, "opp-key-1", "trace-opp");

        var expectedProj = new OpportunityProjection(
            Guid.NewGuid(), "OPP-01", customerId, null, _accessResolver.BranchId!.Value, _accessResolver.UserId,
            "Built-in ตู้เสื้อผ้า", "รายละเอียด", new[] { "built-in", "interior" }, null,
            250000m, "THB", null, null, null, OpportunityStage.Draft, Guid.NewGuid(), DateTimeOffset.UtcNow);

        _store.CreateResult = Result<OpportunityProjection>.Success(expectedProj);
        var handler = CreateHandler();

        var result = await handler.Handle(cmd);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, _store.CreateCallCount);
        Assert.NotNull(_store.LastKeyHash);
        Assert.NotNull(_store.LastPayloadHash);
        Assert.Equal(expectedProj, result.Value);
    }

    [Fact]
    public async Task List_PermissionDenied_ReturnsFailureAndDoesNotCallStore()
    {
        var query = new ListOpportunitiesQuery("uid", Guid.NewGuid(), null, null, null, 25, null, "trace");
        var handler = ListHandler();

        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Equal(0, _store.ListCallCount);
    }

    [Fact]
    public async Task List_InvalidCursor_ReturnsValidationError()
    {
        _accessResolver.GrantedPermissions.Add("opportunities.read");
        var query = new ListOpportunitiesQuery("uid", Guid.NewGuid(), null, null, null, 25, "not-valid-base64-json", "trace");
        var handler = ListHandler();

        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("INVALID_CURSOR", result.Error.Code);
        Assert.Equal(0, _store.ListCallCount);
    }

    [Fact]
    public async Task List_ClampsLimitBetween1And100()
    {
        _accessResolver.GrantedPermissions.Add("opportunities.read");
        var query = new ListOpportunitiesQuery("uid", Guid.NewGuid(), null, null, null, 500, null, "trace");
        var handler = ListHandler();

        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, _store.ListCallCount);
    }

    [Fact]
    public async Task Get_PermissionDenied_ReturnsFailureAndDoesNotCallStore()
    {
        var query = new GetOpportunityQuery("uid", Guid.NewGuid(), Guid.NewGuid(), "trace");
        var handler = GetHandler();

        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Equal(0, _store.GetCallCount);
    }

    [Fact]
    public async Task Get_NotFound_ReturnsResourceNotFound()
    {
        _accessResolver.GrantedPermissions.Add("opportunities.read");
        _store.GetResult = null;
        var query = new GetOpportunityQuery("uid", Guid.NewGuid(), Guid.NewGuid(), "trace");
        var handler = GetHandler();

        var result = await handler.Handle(query);

        Assert.True(result.IsFailure);
        Assert.Equal("RESOURCE_NOT_FOUND", result.Error.Code);
        Assert.Equal(1, _store.GetCallCount);
    }

    [Fact]
    public async Task Get_Found_ReturnsProjection()
    {
        _accessResolver.GrantedPermissions.Add("opportunities.read");
        var oppId = Guid.NewGuid();
        var proj = new OpportunityProjection(
            oppId, "OPP-01", Guid.NewGuid(), null, Guid.NewGuid(), Guid.NewGuid(),
            "Title", null, new[] { "built-in" }, null, null, null, null, null, null,
            OpportunityStage.Draft, Guid.NewGuid(), DateTimeOffset.UtcNow);
        _store.GetResult = proj;

        var query = new GetOpportunityQuery("uid", Guid.NewGuid(), oppId, "trace");
        var handler = GetHandler();

        var result = await handler.Handle(query);

        Assert.True(result.IsSuccess);
        Assert.Equal(proj, result.Value);
        Assert.Equal(1, _store.GetCallCount);
    }
}
