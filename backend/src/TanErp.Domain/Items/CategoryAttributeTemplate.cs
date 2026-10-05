using TanErp.Domain.Common;

namespace TanErp.Domain.Items;

public sealed class CategoryAttributeOption
{
    public string Value { get; set; } = string.Empty;
    public LocalizedText Label { get; set; } = null!;
}

public sealed class CategoryAttributeTemplate
{
    public string Key { get; set; } = string.Empty;
    public LocalizedText Name { get; set; } = null!;
    public string DataType { get; set; } = "text"; // "number", "select", "text", "boolean"
    public string? Unit { get; set; }
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public List<CategoryAttributeOption>? Options { get; set; }
}
