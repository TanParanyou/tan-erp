using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Estimates.CalculationPolicies;
using TanErp.Domain.Estimates;
using Xunit;

namespace TanErp.UnitTests.Estimates;

public class ManageCalculationPoliciesHandlerTests
{
    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();
    private static readonly Guid MembershipId = Guid.NewGuid();
    private static readonly Guid CreatorUserId = Guid.NewGuid();
    private static readonly Guid CheckerUserId = Guid.NewGuid();
    private static readonly DateTimeOffset FixedNow = new(2026, 10, 29, 4, 0, 0, TimeSpan.Zero);

    private sealed class FakeClock : IClock
    {
        public DateTimeOffset UtcNow => FixedNow;
    }

    private sealed class FakeAccessResolver : IRequestAccessResolver
    {
        public Guid CurrentActorUserId { get; set; } = CreatorUserId;

        public Task<Result<RequestAccessContext>> ResolveAsync(
            string firebaseUid,
            Guid membershipId,
            string requiredPermission,
            CancellationToken cancellationToken = default)
        {
            var context = new RequestAccessContext(
                CurrentActorUserId,
                membershipId,
                OrgId,
                BranchId,
                requiredPermission,
                "Branch");
            return Task.FromResult(Result<RequestAccessContext>.Success(context));
        }
    }

    private sealed class FakeCalculationPolicyStore : ICalculationPolicyStore
    {
        public List<CalculationPolicyVersion> Policies { get; } = new();

        public Task<IReadOnlyList<CalculationPolicyVersion>> ListAsync(Guid organizationId, Guid? branchId, CancellationToken ct = default)
        {
            var query = Policies.Where(p => p.OrganizationId == organizationId);
            if (branchId.HasValue) query = query.Where(p => p.BranchId == branchId.Value);
            return Task.FromResult<IReadOnlyList<CalculationPolicyVersion>>(query.ToList());
        }

        public Task<CalculationPolicyVersion?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct = default)
        {
            return Task.FromResult(Policies.FirstOrDefault(p => p.OrganizationId == organizationId && p.Id == id));
        }

        public Task<Result<CalculationPolicyVersion>> CreateDraftAsync(
            Guid organizationId, Guid? branchId, string policyCode, int version,
            string overheadMethod, decimal overheadValue, string roundingMode,
            string unitConversionMode, DateTimeOffset effectiveFromUtc, DateTimeOffset? effectiveToUtc,
            Guid createdByUserId, CancellationToken ct = default)
        {
            var policy = new CalculationPolicyVersion(
                Guid.NewGuid(), organizationId, branchId, policyCode, version,
                overheadMethod, overheadValue, roundingMode, effectiveFromUtc, effectiveToUtc,
                createdByUserId, unitConversionMode);
            Policies.Add(policy);
            return Task.FromResult(Result<CalculationPolicyVersion>.Success(policy));
        }

        public Task<Result<CalculationPolicyVersion>> PublishAsync(
            Guid organizationId, Guid id, Guid publisherUserId, DateTimeOffset publishedAtUtc, CancellationToken ct = default)
        {
            var policy = Policies.FirstOrDefault(p => p.OrganizationId == organizationId && p.Id == id);
            if (policy is null) return Task.FromResult(Result<CalculationPolicyVersion>.Failure(new Error("RESOURCE_NOT_FOUND", "Policy not found.")));
            policy.Publish(publisherUserId, publishedAtUtc);
            return Task.FromResult(Result<CalculationPolicyVersion>.Success(policy));
        }
    }

    [Fact]
    public async Task CreateDraft_WithSnapshotMode_Succeeds()
    {
        var fakeResolver = new FakeAccessResolver { CurrentActorUserId = CreatorUserId };
        var fakeStore = new FakeCalculationPolicyStore();
        var handler = new ManageCalculationPoliciesHandler(fakeResolver, fakeStore, new FakeClock());

        var command = new CreateCalculationPolicyCommand(
            BranchId: BranchId,
            PolicyCode: "STD-CALC",
            Version: 2,
            OverheadMethod: "none",
            OverheadValue: 0m,
            RoundingMode: "away-from-zero",
            UnitConversionMode: UnitConversionModes.Snapshot,
            EffectiveFromUtc: FixedNow,
            EffectiveToUtc: null);

        var result = await handler.CreateDraftAsync("user-creator", MembershipId, command);

        Assert.True(result.IsSuccess);
        Assert.Equal(UnitConversionModes.Snapshot, result.Value!.UnitConversionMode);
        Assert.Equal(EstimatePolicyStatus.Draft, result.Value.Status);
        Assert.Equal(CreatorUserId, result.Value.CreatedByUserId);
    }

    [Fact]
    public async Task Publish_WhenCreatorAttemptsToPublishSelf_FailsWithMakerCheckerViolation()
    {
        var fakeResolver = new FakeAccessResolver { CurrentActorUserId = CreatorUserId };
        var fakeStore = new FakeCalculationPolicyStore();
        var handler = new ManageCalculationPoliciesHandler(fakeResolver, fakeStore, new FakeClock());

        var policyId = Guid.NewGuid();
        var draft = new CalculationPolicyVersion(
            policyId, OrgId, BranchId, "STD-CALC", 2, "none", 0m, "away-from-zero", FixedNow, null, CreatorUserId, UnitConversionModes.Snapshot);
        fakeStore.Policies.Add(draft);

        // Creator attempts to publish their own policy:
        var result = await handler.PublishAsync("user-creator", MembershipId, policyId);

        Assert.True(result.IsFailure);
        Assert.Equal("MAKER_CHECKER_VIOLATION", result.Error.Code);
    }

    [Fact]
    public async Task Publish_WhenIndependentCheckerPublishes_Succeeds()
    {
        var fakeResolver = new FakeAccessResolver { CurrentActorUserId = CheckerUserId };
        var fakeStore = new FakeCalculationPolicyStore();
        var handler = new ManageCalculationPoliciesHandler(fakeResolver, fakeStore, new FakeClock());

        var policyId = Guid.NewGuid();
        var draft = new CalculationPolicyVersion(
            policyId, OrgId, BranchId, "STD-CALC", 2, "none", 0m, "away-from-zero", FixedNow, null, CreatorUserId, UnitConversionModes.Snapshot);
        fakeStore.Policies.Add(draft);

        // Independent checker publishes:
        var result = await handler.PublishAsync("user-checker", MembershipId, policyId);

        Assert.True(result.IsSuccess);
        Assert.Equal(EstimatePolicyStatus.Published, result.Value!.Status);
        Assert.NotNull(result.Value.ContentHash);
        Assert.Equal(CheckerUserId, result.Value.PublishedByUserId);
        Assert.Equal(UnitConversionModes.Snapshot, result.Value.UnitConversionMode);
    }
}
