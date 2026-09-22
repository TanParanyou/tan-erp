using TanErp.Domain.Items;
using Xunit;

namespace TanErp.UnitTests.Items;

public class CostRecordTests
{
    private readonly Guid _orgId = Guid.NewGuid();
    private readonly Guid _branchId = Guid.NewGuid();
    private readonly Guid _itemId = Guid.NewGuid();
    private readonly Guid _unitId = Guid.NewGuid();
    private readonly Guid _makerId = Guid.NewGuid();
    private readonly Guid _checkerId = Guid.NewGuid();

    [Fact]
    public void CreateDraft_ValidInputs_InitializesDraftCostRecord()
    {
        var costId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var record = CostRecord.CreateDraft(
            costId,
            _orgId,
            _itemId,
            CostScopeType.Branch,
            _branchId,
            _unitId,
            "THB",
            250.50m,
            0m,
            null,
            now,
            null,
            version: 1,
            costSourceId: null,
            sourceReference: "QUOT-2026-001",
            reason: "Initial purchase quotation",
            evidenceFileId: null,
            _makerId,
            now);

        Assert.Equal(costId, record.Id);
        Assert.Equal(_orgId, record.OrganizationId);
        Assert.Equal(_itemId, record.ItemId);
        Assert.Equal(CostScopeType.Branch, record.Scope);
        Assert.Equal(_branchId, record.BranchId);
        Assert.Equal(_unitId, record.UnitId);
        Assert.Equal("THB", record.Currency);
        Assert.Equal(250.50m, record.Amount);
        Assert.Equal(CostRecordStatus.Draft, record.Status);
        Assert.Equal(_makerId, record.CreatedByUserId);
        Assert.Equal(_makerId, record.LastFinancialEditorId);
        Assert.Null(record.ApprovedByUserId);
        Assert.Null(record.PublishedByUserId);
        Assert.Equal(1, record.Version);
    }

    [Fact]
    public void CreateDraft_BranchScopeWithoutBranchId_ThrowsValidationException()
    {
        var now = DateTimeOffset.UtcNow;
        var ex = Assert.Throws<ItemValidationException>(() =>
            CostRecord.CreateDraft(
                Guid.NewGuid(),
                _orgId,
                _itemId,
                CostScopeType.Branch,
                branchId: null,
                _unitId,
                "THB",
                100m,
                0m,
                null,
                now,
                null,
                1,
                null,
                null,
                null,
                null,
                _makerId,
                now));

        Assert.Equal("COST_BRANCH_REQUIRED", ex.Code);
    }

    [Fact]
    public void CreateDraft_NegativeAmount_ThrowsValidationException()
    {
        var now = DateTimeOffset.UtcNow;
        var ex = Assert.Throws<ItemValidationException>(() =>
            CostRecord.CreateDraft(
                Guid.NewGuid(),
                _orgId,
                _itemId,
                CostScopeType.Organization,
                null,
                _unitId,
                "THB",
                -10m,
                0m,
                null,
                now,
                null,
                1,
                null,
                null,
                null,
                null,
                _makerId,
                now));

        Assert.Equal("COST_AMOUNT_INVALID", ex.Code);
    }

    [Fact]
    public void CreateDraft_MaxQuantityLessThanMinQuantity_ThrowsValidationException()
    {
        var now = DateTimeOffset.UtcNow;
        var ex = Assert.Throws<ItemValidationException>(() =>
            CostRecord.CreateDraft(
                Guid.NewGuid(),
                _orgId,
                _itemId,
                CostScopeType.Organization,
                null,
                _unitId,
                "THB",
                100m,
                minimumQuantity: 50m,
                maximumQuantity: 20m,
                now,
                null,
                1,
                null,
                null,
                null,
                null,
                _makerId,
                now));

        Assert.Equal("COST_QUANTITY_RANGE_INVALID", ex.Code);
    }

    [Fact]
    public void Submit_FromDraft_TransitionsToSubmitted()
    {
        var record = CreateStandardDraft();
        var now = DateTimeOffset.UtcNow;

        record.Submit(_makerId, now);

        Assert.Equal(CostRecordStatus.Submitted, record.Status);
    }

    [Fact]
    public void Approve_WhenApproverIsCreator_ThrowsMakerCheckerViolation()
    {
        var record = CreateStandardDraft();
        var now = DateTimeOffset.UtcNow;
        record.Submit(_makerId, now);

        var ex = Assert.Throws<ItemDomainException>(() => record.Approve(_makerId, now));

        Assert.Equal("MAKER_CHECKER_VIOLATION", ex.Code);
    }

    [Fact]
    public void Approve_WhenApproverIsLastFinancialEditor_ThrowsMakerCheckerViolation()
    {
        var record = CreateStandardDraft();
        var editorId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        record.UpdateFinancials(300m, "THB", _unitId, 0m, null, now, null, editorId, now);
        record.Submit(editorId, now);

        var ex = Assert.Throws<ItemDomainException>(() => record.Approve(editorId, now));

        Assert.Equal("MAKER_CHECKER_VIOLATION", ex.Code);
    }

    [Fact]
    public void Approve_WhenApproverIsDifferentUser_TransitionsToApproved()
    {
        var record = CreateStandardDraft();
        var now = DateTimeOffset.UtcNow;
        record.Submit(_makerId, now);

        record.Approve(_checkerId, now);

        Assert.Equal(CostRecordStatus.Approved, record.Status);
        Assert.Equal(_checkerId, record.ApprovedByUserId);
    }

    [Fact]
    public void Return_WhenReviewerIsCreator_ThrowsMakerCheckerViolation()
    {
        var record = CreateStandardDraft();
        var now = DateTimeOffset.UtcNow;
        record.Submit(_makerId, now);

        var ex = Assert.Throws<ItemDomainException>(() => record.Return(_makerId, "Need more info", now));

        Assert.Equal("MAKER_CHECKER_VIOLATION", ex.Code);
    }

    [Fact]
    public void Return_WhenReviewerIsDifferentUser_TransitionsToReturned()
    {
        var record = CreateStandardDraft();
        var now = DateTimeOffset.UtcNow;
        record.Submit(_makerId, now);

        record.Return(_checkerId, "Price exceeds benchmark", now);

        Assert.Equal(CostRecordStatus.Returned, record.Status);
        Assert.Equal("Price exceeds benchmark", record.Reason);
    }

    [Fact]
    public void Publish_WhenApproved_TransitionsToPublished()
    {
        var record = CreateStandardDraft();
        var now = DateTimeOffset.UtcNow;
        record.Submit(_makerId, now);
        record.Approve(_checkerId, now);

        var publisherId = Guid.NewGuid();
        record.Publish(publisherId, now);

        Assert.Equal(CostRecordStatus.Published, record.Status);
        Assert.Equal(publisherId, record.PublishedByUserId);
    }

    [Fact]
    public void UpdateFinancials_WhenPublished_ThrowsImmutableCostException()
    {
        var record = CreateStandardDraft();
        var now = DateTimeOffset.UtcNow;
        record.Submit(_makerId, now);
        record.Approve(_checkerId, now);
        record.Publish(_checkerId, now);

        var ex = Assert.Throws<ItemDomainException>(() =>
            record.UpdateFinancials(999m, "THB", _unitId, 0m, null, now, null, _makerId, now));

        Assert.Equal("ITEM_COST_IMMUTABLE", ex.Code);
    }

    [Fact]
    public void Disable_WhenPublished_TransitionsToDisabled()
    {
        var record = CreateStandardDraft();
        var now = DateTimeOffset.UtcNow;
        record.Submit(_makerId, now);
        record.Approve(_checkerId, now);
        record.Publish(_checkerId, now);

        record.Disable(_checkerId, "Vendor terminated agreement", now);

        Assert.Equal(CostRecordStatus.Disabled, record.Status);
        Assert.Equal("Vendor terminated agreement", record.Reason);
    }

    private CostRecord CreateStandardDraft()
    {
        var now = DateTimeOffset.UtcNow;
        return CostRecord.CreateDraft(
            Guid.NewGuid(),
            _orgId,
            _itemId,
            CostScopeType.Organization,
            null,
            _unitId,
            "THB",
            200m,
            0m,
            null,
            now,
            null,
            1,
            null,
            null,
            null,
            null,
            _makerId,
            now);
    }
}
