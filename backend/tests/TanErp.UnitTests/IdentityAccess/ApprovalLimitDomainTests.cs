using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class ApprovalLimitDomainTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 11, 0, 0, 0, TimeSpan.Zero);

    private static ApprovalLimit Make(string type = ApprovalDocumentType.Estimate, bool unlimited = false, decimal? max = 100m, Guid? branch = null) =>
        ApprovalLimit.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), type, branch, "THB", unlimited, max, Now);

    [Theory]
    [InlineData(ApprovalDocumentType.Estimate)]
    [InlineData(ApprovalDocumentType.PurchaseOrder)]
    [InlineData(ApprovalDocumentType.ChangeOrder)]
    public void Create_AcceptsEnforcedTypesAndStartsActive(string type)
    {
        var limit = Make(type, max: 250_000.50m);

        Assert.True(limit.IsActive);
        Assert.Equal(250_000.50m, limit.MaxAmount);
    }

    [Theory]
    [InlineData(ApprovalDocumentType.CreditNote, false, 1.0)]   // reserved for G-15
    [InlineData("invoice", false, 1.0)]
    [InlineData(ApprovalDocumentType.Estimate, true, 10.0)]     // unlimited must not carry an amount
    [InlineData(ApprovalDocumentType.Estimate, false, null)]
    [InlineData(ApprovalDocumentType.Estimate, false, -0.01)]
    [InlineData(ApprovalDocumentType.Estimate, false, 1.005)]   // three decimal places
    public void Create_RejectsInvalidInput(string type, bool unlimited, double? max) =>
        Assert.Equal("REQUEST_VALIDATION_FAILED",
            Assert.Throws<ApprovalLimitDomainException>(() => Make(type, unlimited, max is null ? null : (decimal)max.Value)).Code);

    [Fact]
    public void Create_RejectsOtherCurrenciesAndEmptyIdentifiers()
    {
        Assert.Throws<ApprovalLimitDomainException>(() =>
            ApprovalLimit.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ApprovalDocumentType.Estimate, null, "USD", false, 1m, Now));
        Assert.Throws<ArgumentException>(() => Make(branch: Guid.Empty));
    }

    [Fact]
    public void Update_RejectsBadAmountsWithoutChangingTheRow_ThenAdvancesTheVersion()
    {
        var limit = Make(max: 50m);
        var before = limit.RowVersion;

        Assert.Throws<ApprovalLimitDomainException>(() => limit.Update(false, null, true, Now));
        Assert.Equal(before, limit.RowVersion);

        limit.Update(true, null, false, Now.AddHours(1));
        Assert.True(limit.IsUnlimited);
        Assert.False(limit.IsActive);
        Assert.NotEqual(before, limit.RowVersion);
    }
}
