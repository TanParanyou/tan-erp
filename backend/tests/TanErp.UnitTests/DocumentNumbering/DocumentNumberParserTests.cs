using System;
using TanErp.Application.Common.DocumentNumbering;
using TanErp.Domain.DocumentNumbering;
using Xunit;

namespace TanErp.UnitTests.DocumentNumbering;

public class DocumentNumberParserTests
{
    [Fact]
    public void Format_WithThaiBuddhistEra_ReplacesTokensCorrectly()
    {
        // 2026 AD = 2569 BE, September (09), 18th
        var timestamp = new DateTimeOffset(2026, 9, 18, 10, 0, 0, TimeSpan.Zero);
        var pattern = "{PREFIX}-{BRANCH}-{BB}{MM}-{SEQ:4}";

        var result = DocumentNumberParser.Format(
            pattern,
            prefix: "EST",
            branchCode: "HQ",
            timestamp: timestamp,
            sequenceValue: 42,
            defaultPadding: 4);

        Assert.Equal("EST-HQ-6909-0042", result);
    }

    [Fact]
    public void Format_WithFullYearAndDifferentPaddings_FormatsAccurately()
    {
        var timestamp = new DateTimeOffset(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);
        var pattern = "{PREFIX}-{YYYY}-{BBBB}-{MM}-{DD}-{SEQ:6}";

        var result = DocumentNumberParser.Format(
            pattern,
            prefix: "INV",
            branchCode: null,
            timestamp: timestamp,
            sequenceValue: 123,
            defaultPadding: 4);

        Assert.Equal("INV-2026-2569-01-05-000123", result);
    }

    [Fact]
    public void Format_WhenBranchIsEmpty_ReplacesWithEmpty()
    {
        var timestamp = new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero);
        var pattern = "{PREFIX}-{BRANCH}{YY}-{SEQ:4}";

        var result = DocumentNumberParser.Format(
            pattern,
            prefix: "SRV",
            branchCode: null,
            timestamp: timestamp,
            sequenceValue: 7);

        Assert.Equal("SRV-26-0007", result);
    }

    [Theory]
    [InlineData(ResetPeriod.Never, "ALL")]
    [InlineData(ResetPeriod.Yearly, "2026")]
    [InlineData(ResetPeriod.Monthly, "202609")]
    [InlineData(ResetPeriod.Daily, "20260918")]
    public void ComputePeriodKey_ReturnsExpectedKeys(ResetPeriod period, string expectedKey)
    {
        var timestamp = new DateTimeOffset(2026, 9, 18, 15, 30, 0, TimeSpan.Zero);
        var key = DocumentNumberParser.ComputePeriodKey(period, timestamp);
        Assert.Equal(expectedKey, key);
    }
}
