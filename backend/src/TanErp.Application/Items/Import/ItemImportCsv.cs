using System.Text;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Items.Import;

/// <summary>
/// Parses the Item import CSV (UTF-8, comma separated, RFC 4180 quoting, header row required) and checks the
/// parts of each row that need no database: required cells, lengths, enum values and booleans.
/// </summary>
public static class ItemImportCsv
{
    public static readonly IReadOnlyList<string> Columns = new[]
    {
        "code", "itemType", "categoryCode", "brandCode", "baseUnitCode", "nameTh", "nameEn",
        "descriptionTh", "descriptionEn", "taxCategoryCode", "canSell", "canCost", "canPurchase", "canStock", "canProduce"
    };

    private const int MaxCodeLength = 50;
    private const int MaxNameLength = 200;
    private const int MaxDescriptionLength = 1000;

    public static Result<IReadOnlyList<ItemImportRowDraft>> Parse(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Fail("The import file is empty.");
        }

        if (content.Length > ItemImportLimits.MaxContentLength)
        {
            return Fail("The import file is too large.");
        }

        var records = ReadRecords(content.TrimStart('﻿'));
        if (records is null)
        {
            return Fail("The import file has an unterminated quoted value.");
        }

        if (records.Count == 0)
        {
            return Fail("The import file has no header row.");
        }

        var header = records[0].Select(h => h.Trim()).ToList();
        if (header.Count != Columns.Count
            || !header.Zip(Columns, (actual, expected) => string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase)).All(match => match))
        {
            return Fail($"The header row must be exactly: {string.Join(",", Columns)}.");
        }

        var dataRecords = records.Skip(1).ToList();
        if (dataRecords.Count == 0)
        {
            return Fail("The import file has no data rows.");
        }

        if (dataRecords.Count > ItemImportLimits.MaxRows)
        {
            return Fail($"The import file has more than {ItemImportLimits.MaxRows} rows.");
        }

        var rows = new List<ItemImportRowDraft>(dataRecords.Count);
        for (var i = 0; i < dataRecords.Count; i++)
        {
            var cells = dataRecords[i];
            if (cells.Count != Columns.Count)
            {
                return Fail($"Row {i + 2} has {cells.Count} columns; expected {Columns.Count}.");
            }

            string? Cell(int index) => string.IsNullOrWhiteSpace(cells[index]) ? null : cells[index].Trim();

            rows.Add(new ItemImportRowDraft(
                RowNumber: i + 2,
                Code: Cell(0), ItemType: Cell(1), CategoryCode: Cell(2), BrandCode: Cell(3), BaseUnitCode: Cell(4),
                NameTh: Cell(5), NameEn: Cell(6), DescriptionTh: Cell(7), DescriptionEn: Cell(8), TaxCategoryCode: Cell(9),
                CanSell: Cell(10), CanCost: Cell(11), CanPurchase: Cell(12), CanStock: Cell(13), CanProduce: Cell(14)));
        }

        return Result<IReadOnlyList<ItemImportRowDraft>>.Success(rows);
    }

    /// <summary>Returns the parsed row, or the syntax errors found on it.</summary>
    public static (ItemImportRow? Row, IReadOnlyList<ItemImportRowError> Errors) Validate(ItemImportRowDraft draft)
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

        Require("itemType", draft.ItemType);
        Require("categoryCode", draft.CategoryCode);
        Require("baseUnitCode", draft.BaseUnitCode);
        Require("nameTh", draft.NameTh);

        if (draft.ItemType is { } type && !Domain.Items.ItemType.IsValid(type))
        {
            errors.Add(new ItemImportRowError("itemType", ItemImportErrorCodes.Invalid));
        }

        Limit("code", draft.Code, MaxCodeLength);
        Limit("nameTh", draft.NameTh, MaxNameLength);
        Limit("nameEn", draft.NameEn, MaxNameLength);
        Limit("descriptionTh", draft.DescriptionTh, MaxDescriptionLength);
        Limit("descriptionEn", draft.DescriptionEn, MaxDescriptionLength);

        bool ParseFlag(string field, string? value)
        {
            if (value is null) return false;
            if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || value == "1") return true;
            if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase) || value == "0") return false;
            errors.Add(new ItemImportRowError(field, ItemImportErrorCodes.Invalid));
            return false;
        }

        var canSell = ParseFlag("canSell", draft.CanSell);
        var canCost = ParseFlag("canCost", draft.CanCost);
        var canPurchase = ParseFlag("canPurchase", draft.CanPurchase);
        var canStock = ParseFlag("canStock", draft.CanStock);
        var canProduce = ParseFlag("canProduce", draft.CanProduce);

        if (errors.All(e => !e.Field.StartsWith("can", StringComparison.Ordinal))
            && !(canSell || canCost || canPurchase || canStock || canProduce))
        {
            errors.Add(new ItemImportRowError("capabilities", ItemImportErrorCodes.Required));
        }

        if (errors.Count > 0)
        {
            return (null, errors);
        }

        return (new ItemImportRow(
            draft.RowNumber, draft.Code, draft.ItemType!.ToLowerInvariant(), draft.CategoryCode!, draft.BrandCode,
            draft.BaseUnitCode!, draft.NameTh!, draft.NameEn, draft.DescriptionTh, draft.DescriptionEn, draft.TaxCategoryCode,
            canSell, canCost, canPurchase, canStock, canProduce), errors);
    }

    private static Result<IReadOnlyList<ItemImportRowDraft>> Fail(string message) =>
        Result<IReadOnlyList<ItemImportRowDraft>>.Failure(new Error("ITEM_IMPORT_FILE_INVALID", message));

    /// <summary>Splits CSV text into records of cells; returns null when a quoted value is never closed.</summary>
    private static List<List<string>>? ReadRecords(string text)
    {
        var records = new List<List<string>>();
        var cells = new List<string>();
        var cell = new StringBuilder();
        var inQuotes = false;
        var cellWasQuoted = false;

        void EndCell()
        {
            cells.Add(cell.ToString());
            cell.Clear();
            cellWasQuoted = false;
        }

        void EndRecord()
        {
            EndCell();
            // Blank lines (a single empty, unquoted cell) are ignored.
            if (!(cells.Count == 1 && cells[0].Length == 0 && !cellWasQuoted))
            {
                records.Add(cells);
            }

            cells = new List<string>();
        }

        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < text.Length && text[i + 1] == '"')
                    {
                        cell.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    cell.Append(c);
                }

                continue;
            }

            switch (c)
            {
                case '"' when cell.Length == 0:
                    inQuotes = true;
                    cellWasQuoted = true;
                    break;
                case ',':
                    EndCell();
                    break;
                case '\r':
                    if (i + 1 < text.Length && text[i + 1] == '\n') i++;
                    EndRecord();
                    break;
                case '\n':
                    EndRecord();
                    break;
                default:
                    cell.Append(c);
                    break;
            }
        }

        if (inQuotes) return null;

        if (cell.Length > 0 || cells.Count > 0)
        {
            EndRecord();
        }

        return records;
    }
}
