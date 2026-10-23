using TanErp.Application.Items.Import;
using Xunit;

namespace TanErp.UnitTests.Items;

public class ItemImportCsvTests
{
    private static readonly string Header = string.Join(",", ItemImportCsv.Columns);

    private static string Csv(params string[] rows) => string.Join("\n", new[] { Header }.Concat(rows));

    private const string ValidRow = "ITM-1,material,CAT,BRD,PCS,ไม้อัด,Plywood,,,,false,true,true,true,false";

    [Fact]
    public void Parse_ValidFile_ReturnsDraftsWithRowNumbers()
    {
        var result = ItemImportCsv.Parse(Csv(ValidRow, "ITM-2,service,CAT,,PCS,งานติดตั้ง,,,,,true,true,false,false,false"));

        Assert.True(result.IsSuccess);
        var rows = result.Value!;
        Assert.Equal(2, rows.Count);
        Assert.Equal(2, rows[0].RowNumber);
        Assert.Equal("ITM-1", rows[0].Code);
        Assert.Equal("ไม้อัด", rows[0].NameTh);
        Assert.Null(rows[1].BrandCode);
        Assert.Equal(3, rows[1].RowNumber);
    }

    [Fact]
    public void Parse_HandlesBomCrLfQuotedCommasQuotesAndNewlines()
    {
        var content = "﻿" + Header + "\r\n" +
            "ITM-1,material,CAT,BRD,PCS,\"ไม้, อัด \"\"พิเศษ\"\"\",\"line1\nline2\",,,,false,true,true,true,false\r\n\r\n";

        var result = ItemImportCsv.Parse(content);

        Assert.True(result.IsSuccess);
        var row = Assert.Single(result.Value!);
        Assert.Equal("ไม้, อัด \"พิเศษ\"", row.NameTh);
        Assert.Equal("line1\nline2", row.NameEn);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_EmptyContent_Fails(string content)
    {
        var result = ItemImportCsv.Parse(content);

        Assert.True(result.IsFailure);
        Assert.Equal("ITEM_IMPORT_FILE_INVALID", result.Error.Code);
    }

    [Fact]
    public void Parse_WrongHeaderOrderOrNames_Fails()
    {
        var swapped = "itemType,code" + Header[Header.IndexOf(',', Header.IndexOf(',') + 1)..] + "\nmaterial,X";
        Assert.True(ItemImportCsv.Parse(swapped).IsFailure);
        Assert.True(ItemImportCsv.Parse("code,itemType\nA,material").IsFailure);
    }

    [Fact]
    public void Parse_HeaderOnly_RowWithWrongColumnCount_AndUnterminatedQuote_Fail()
    {
        Assert.True(ItemImportCsv.Parse(Header).IsFailure);
        Assert.True(ItemImportCsv.Parse(Csv("ITM-1,material")).IsFailure);
        Assert.True(ItemImportCsv.Parse(Csv("ITM-1,\"material,CAT,BRD,PCS,x,,,,,false,true,false,false,false")).IsFailure);
    }

    [Fact]
    public void Parse_MoreThanMaxRows_Fails()
    {
        var rows = Enumerable.Range(0, ItemImportLimits.MaxRows + 1)
            .Select(i => $"ITM-{i},material,CAT,,PCS,ชื่อ {i},,,,,false,true,false,false,false")
            .ToArray();

        var result = ItemImportCsv.Parse(Csv(rows));

        Assert.True(result.IsFailure);
        Assert.Equal("ITEM_IMPORT_FILE_INVALID", result.Error.Code);
    }

    [Fact]
    public void Validate_ValidRow_ParsesTypesAndFlags()
    {
        var draft = ItemImportCsv.Parse(Csv(ValidRow)).Value![0];

        var (row, errors) = ItemImportCsv.Validate(draft);

        Assert.Empty(errors);
        Assert.NotNull(row);
        Assert.Equal("material", row.ItemType);
        Assert.False(row.CanSell);
        Assert.True(row.CanCost);
        Assert.True(row.CanStock);
    }

    [Fact]
    public void Validate_ReportsEveryProblemWithFieldAndCode()
    {
        var draft = ItemImportCsv.Parse(Csv(",bogus,,,,,,,,,maybe,,,,")).Value![0];

        var (row, errors) = ItemImportCsv.Validate(draft);

        Assert.Null(row);
        Assert.Contains(new ItemImportRowError("itemType", ItemImportErrorCodes.Invalid), errors);
        Assert.Contains(new ItemImportRowError("categoryCode", ItemImportErrorCodes.Required), errors);
        Assert.Contains(new ItemImportRowError("baseUnitCode", ItemImportErrorCodes.Required), errors);
        Assert.Contains(new ItemImportRowError("nameTh", ItemImportErrorCodes.Required), errors);
        Assert.Contains(new ItemImportRowError("canSell", ItemImportErrorCodes.Invalid), errors);
    }

    [Fact]
    public void Validate_RequiresAtLeastOneCapability_AndLimitsLengths()
    {
        var noCapability = ItemImportCsv.Parse(Csv("ITM-1,material,CAT,,PCS,ชื่อ,,,,,false,false,false,false,false")).Value![0];
        Assert.Contains(new ItemImportRowError("capabilities", ItemImportErrorCodes.Required), ItemImportCsv.Validate(noCapability).Errors);

        var longName = new string('x', 201);
        var tooLong = ItemImportCsv.Parse(Csv($"ITM-1,material,CAT,,PCS,{longName},,,,,false,true,false,false,false")).Value![0];
        Assert.Contains(new ItemImportRowError("nameTh", ItemImportErrorCodes.TooLong), ItemImportCsv.Validate(tooLong).Errors);
    }

    [Fact]
    public void CostImportCsv_ParseAndValidate_EnforcesColumnsScopeBranchCurrencyAndQuantities()
    {
        var costHeader = string.Join(",", CostImportCsv.Columns);
        string CostCsv(params string[] rows) => string.Join("\n", new[] { costHeader }.Concat(rows));

        Assert.True(CostImportCsv.Parse("itemCode,scope\nA,organization").IsFailure);

        var validOrg = "ITM-1,organization,,PCS,thb,1250.50,0,100,2026-10-01T00:00:00Z,2026-12-31T00:00:00Z,MANUAL,QT-001,ราคาตั้งต้น";
        var validBranch = "ITM-2,branch,B01,PCS,THB,990,,50,2026-10-01T00:00:00Z,,MANUAL,QT-002,ราคาสาขา";
        var parsed = CostImportCsv.Parse(CostCsv(validOrg, validBranch));
        Assert.True(parsed.IsSuccess);
        Assert.Equal(2, parsed.Value!.Count);

        var (row1, errors1) = CostImportCsv.Validate(parsed.Value[0]);
        Assert.Empty(errors1);
        Assert.NotNull(row1);
        Assert.Equal(("organization", "THB", 1250.50m, 0m, 100m), (row1.Scope, row1.Currency, row1.Amount, row1.MinimumQuantity, row1.MaximumQuantity));

        var (row2, errors2) = CostImportCsv.Validate(parsed.Value[1]);
        Assert.Empty(errors2);
        Assert.NotNull(row2);
        Assert.Equal(("branch", "B01"), (row2.Scope, row2.BranchCode));

        // Branch scope without branchCode, organization scope with branchCode, negative amount, max <= min, end < start
        var invalidRows = CostImportCsv.Parse(CostCsv(
            "ITM-1,branch,,PCS,THB,100,10,5,2026-10-10T00:00:00Z,2026-10-01T00:00:00Z,MANUAL,REF,เหตุผล",
            "ITM-1,organization,B01,PCS,TH,not-a-number,-1,,bad-date,,MANUAL,,")).Value!;

        var (_, bad1) = CostImportCsv.Validate(invalidRows[0]);
        Assert.Contains(new ItemImportRowError("branchCode", ItemImportErrorCodes.Required), bad1);
        Assert.Contains(new ItemImportRowError("maximumQuantity", ItemImportErrorCodes.Invalid), bad1);
        Assert.Contains(new ItemImportRowError("effectiveToUtc", ItemImportErrorCodes.Invalid), bad1);

        var (_, bad2) = CostImportCsv.Validate(invalidRows[1]);
        Assert.Contains(new ItemImportRowError("branchCode", ItemImportErrorCodes.Invalid), bad2);
        Assert.Contains(new ItemImportRowError("currency", ItemImportErrorCodes.Invalid), bad2);
        Assert.Contains(new ItemImportRowError("amount", ItemImportErrorCodes.Invalid), bad2);
        Assert.Contains(new ItemImportRowError("minimumQuantity", ItemImportErrorCodes.Invalid), bad2);
        Assert.Contains(new ItemImportRowError("effectiveFromUtc", ItemImportErrorCodes.Invalid), bad2);
        Assert.Contains(new ItemImportRowError("sourceReference", ItemImportErrorCodes.Required), bad2);
        Assert.Contains(new ItemImportRowError("reason", ItemImportErrorCodes.Required), bad2);
    }
}
