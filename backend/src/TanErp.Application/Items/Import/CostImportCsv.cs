using System.Globalization;
using TanErp.Application.Common.Results;
using TanErp.Domain.Items;

namespace TanErp.Application.Items.Import;

/// <summary>
/// Parses the Cost import CSV (same dialect as the Item import) and checks the parts of each row that need no database.
/// Every valid row becomes a Draft Cost Record; nothing here can publish or change a published price.
/// </summary>
public static class CostImportCsv
{
    public static readonly IReadOnlyList<string> Columns = new[]
    {
        "itemCode", "scope", "branchCode", "unitCode", "currency", "amount", "minimumQuantity", "maximumQuantity",
        "effectiveFromUtc", "effectiveToUtc", "costSourceCode", "sourceReference", "reason"
    };

    private const int MaxCodeLength = 50;
    private const int MaxSourceReferenceLength = 128;
    private const int MaxReasonLength = 500;
    private const int CurrencyLength = 3;

    public static Result<IReadOnlyList<CostImportRowDraft>> Parse(string? content) =>
        Parse(content, ItemImportLimits.MaxRows, ItemImportLimits.MaxContentLength);

    public static Result<IReadOnlyList<CostImportRowDraft>> Parse(string? content, int maxRows, int maxContentLength)
    {
        if (string.IsNullOrWhiteSpace(content)) return Fail("The import file is empty.");
        if (content.Length > maxContentLength) return Fail("The import file is too large.");

        var records = ItemImportCsv.ReadRecords(content.TrimStart('﻿'));
        if (records is null) return Fail("The import file has an unterminated quoted value.");
        if (records.Count == 0) return Fail("The import file has no header row.");

        var header = records[0].Select(h => h.Trim()).ToList();
        if (header.Count != Columns.Count
            || !header.Zip(Columns, (actual, expected) => string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase)).All(match => match))
        {
            return Fail($"The header row must be exactly: {string.Join(",", Columns)}.");
        }

        var dataRecords = records.Skip(1).ToList();
        if (dataRecords.Count == 0) return Fail("The import file has no data rows.");
        if (dataRecords.Count > maxRows) return Fail($"The import file has more than {maxRows} rows.");

        var rows = new List<CostImportRowDraft>(dataRecords.Count);
        for (var i = 0; i < dataRecords.Count; i++)
        {
            var cells = dataRecords[i];
            if (cells.Count != Columns.Count) return Fail($"Row {i + 2} has {cells.Count} columns; expected {Columns.Count}.");

            string? Cell(int index) => string.IsNullOrWhiteSpace(cells[index]) ? null : cells[index].Trim();

            rows.Add(new CostImportRowDraft(
                i + 2, Cell(0), Cell(1), Cell(2), Cell(3), Cell(4), Cell(5), Cell(6), Cell(7), Cell(8), Cell(9), Cell(10), Cell(11), Cell(12)));
        }

        return Result<IReadOnlyList<CostImportRowDraft>>.Success(rows);
    }

    /// <summary>Returns the parsed row, or the syntax errors found on it.</summary>
    public static (CostImportRow? Row, IReadOnlyList<ItemImportRowError> Errors) Validate(CostImportRowDraft draft)
    {
        var errors = new List<ItemImportRowError>();

        void Require(string field, string? value)
        {
            if (value is null) errors.Add(new ItemImportRowError(field, ItemImportErrorCodes.Required));
        }

        void Limit(string field, string? value, int max)
        {
            if (value is { } v && v.Length > max) errors.Add(new ItemImportRowError(field, ItemImportErrorCodes.TooLong));
        }

        decimal? ParseDecimal(string field, string? value)
        {
            if (value is null) return null;
            if (decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsed)) return parsed;
            errors.Add(new ItemImportRowError(field, ItemImportErrorCodes.Invalid));
            return null;
        }

        DateTimeOffset? ParseDate(string field, string? value)
        {
            if (value is null) return null;
            if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsed)) return parsed;
            errors.Add(new ItemImportRowError(field, ItemImportErrorCodes.Invalid));
            return null;
        }

        Require("itemCode", draft.ItemCode);
        Require("scope", draft.Scope);
        Require("unitCode", draft.UnitCode);
        Require("currency", draft.Currency);
        Require("amount", draft.Amount);
        Require("effectiveFromUtc", draft.EffectiveFromUtc);
        Require("costSourceCode", draft.CostSourceCode);
        Require("sourceReference", draft.SourceReference);
        Require("reason", draft.Reason);

        Limit("itemCode", draft.ItemCode, MaxCodeLength);
        Limit("sourceReference", draft.SourceReference, MaxSourceReferenceLength);
        Limit("reason", draft.Reason, MaxReasonLength);

        if (draft.Scope is { } scope && !CostScopeType.All.Contains(scope, StringComparer.OrdinalIgnoreCase))
        {
            errors.Add(new ItemImportRowError("scope", ItemImportErrorCodes.Invalid));
        }
        else if (draft.Scope is { } validScope)
        {
            var isBranch = string.Equals(validScope, CostScopeType.Branch, StringComparison.OrdinalIgnoreCase);
            if (isBranch && draft.BranchCode is null) errors.Add(new ItemImportRowError("branchCode", ItemImportErrorCodes.Required));
            if (!isBranch && draft.BranchCode is not null) errors.Add(new ItemImportRowError("branchCode", ItemImportErrorCodes.Invalid));
        }

        if (draft.Currency is { } currency && currency.Length != CurrencyLength)
        {
            errors.Add(new ItemImportRowError("currency", ItemImportErrorCodes.Invalid));
        }

        var amount = ParseDecimal("amount", draft.Amount);
        var minimum = ParseDecimal("minimumQuantity", draft.MinimumQuantity) ?? 0m;
        var maximum = ParseDecimal("maximumQuantity", draft.MaximumQuantity);
        var from = ParseDate("effectiveFromUtc", draft.EffectiveFromUtc);
        var to = ParseDate("effectiveToUtc", draft.EffectiveToUtc);

        if (amount is < 0m) errors.Add(new ItemImportRowError("amount", ItemImportErrorCodes.Invalid));
        if (minimum < 0m) errors.Add(new ItemImportRowError("minimumQuantity", ItemImportErrorCodes.Invalid));
        if (maximum is { } max && max <= minimum) errors.Add(new ItemImportRowError("maximumQuantity", ItemImportErrorCodes.Invalid));
        if (from is { } f && to is { } t && t < f) errors.Add(new ItemImportRowError("effectiveToUtc", ItemImportErrorCodes.Invalid));

        if (errors.Count > 0 || amount is null || from is null)
        {
            return (null, errors);
        }

        return (new CostImportRow(
            draft.RowNumber, draft.ItemCode!, draft.Scope!.ToLowerInvariant(), draft.BranchCode, draft.UnitCode!,
            draft.Currency!.ToUpperInvariant(), amount.Value, minimum, maximum, from.Value, to, draft.CostSourceCode!,
            draft.SourceReference!, draft.Reason!), errors);
    }

    private static Result<IReadOnlyList<CostImportRowDraft>> Fail(string message) =>
        Result<IReadOnlyList<CostImportRowDraft>>.Failure(new Error("ITEM_IMPORT_FILE_INVALID", message));
}
