using System.Buffers.Text;
using System.Text;
using System.Text.Json;

namespace TanErp.Application.Crm.Opportunities;

public sealed record OpportunityCursor(
    DateTimeOffset? NextActionAtUtc,
    Guid Id)
{
    public static string Encode(DateTimeOffset? nextActionAtUtc, Guid id)
    {
        var dto = new CursorDto(nextActionAtUtc?.ToUniversalTime().ToString("O"), id);
        var json = JsonSerializer.Serialize(dto);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public static OpportunityCursor? TryDecode(string? cursorStr)
    {
        if (string.IsNullOrWhiteSpace(cursorStr)) return null;

        try
        {
            var bytes = Convert.FromBase64String(cursorStr.Trim());
            var json = Encoding.UTF8.GetString(bytes);
            var dto = JsonSerializer.Deserialize<CursorDto>(json);
            if (dto == null || dto.Id == Guid.Empty) return null;

            DateTimeOffset? nextAction = null;
            if (!string.IsNullOrWhiteSpace(dto.NextActionAtUtc))
            {
                if (DateTimeOffset.TryParse(dto.NextActionAtUtc, out var parsed))
                    nextAction = parsed.ToUniversalTime();
                else
                    return null;
            }

            return new OpportunityCursor(nextAction, dto.Id);
        }
        catch
        {
            return null;
        }
    }

    private sealed record CursorDto(string? NextActionAtUtc, Guid Id);
}
