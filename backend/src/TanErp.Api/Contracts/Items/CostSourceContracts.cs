using System.ComponentModel.DataAnnotations;
using TanErp.Application.Items;

namespace TanErp.Api.Contracts.Items;

public sealed record CostSourceRequest
{
    [StringLength(32)] public string? Code { get; init; }
    [Required] public LocalizedTextInput Name { get; init; } = new();
    public string SourceType { get; init; } = "manual";
}

public sealed record UpdateCostSourceRequest
{
    [Required, StringLength(32, MinimumLength = 1)] public string Code { get; init; } = string.Empty;
    [Required] public LocalizedTextInput Name { get; init; } = new();
    public string SourceType { get; init; } = "manual";
}

public sealed record CostSourceResponse(Guid Id, string Code, LocalizedTextResponse Name, string SourceType, int Priority, bool IsActive, Guid RowVersion);

public static class CostSourceResponseMapper
{
    public static CostSourceResponse ToResponse(CostSourceProjection p) => new(p.Id, p.Code,
        new LocalizedTextResponse { Thai = p.Name.Thai, English = p.Name.English }, p.SourceType, p.Priority, p.IsActive, p.RowVersion);
}
