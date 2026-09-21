using System.Text;
using System.Text.Json;
using TanErp.Application.Common.Results;

namespace TanErp.Application.Crm.Opportunities.WorkImages;

public static class OpportunityWorkImageCursor
{
    private sealed record CursorPayload(long C, Guid Id);

    public static string Encode(DateTimeOffset createdAtUtc, Guid id)
    {
        var payload = new CursorPayload(createdAtUtc.ToUnixTimeMilliseconds(), id);
        var json = JsonSerializer.Serialize(payload);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    public static Result<(DateTimeOffset CreatedAtUtc, Guid Id)?> TryDecode(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return Result<(DateTimeOffset CreatedAtUtc, Guid Id)?>.Success(null);
        }

        try
        {
            var incoming = cursor.Trim().Replace('-', '+').Replace('_', '/');
            switch (incoming.Length % 4)
            {
                case 2: incoming += "=="; break;
                case 3: incoming += "="; break;
            }

            var bytes = Convert.FromBase64String(incoming);
            var json = Encoding.UTF8.GetString(bytes);
            var payload = JsonSerializer.Deserialize<CursorPayload>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null || payload.Id == Guid.Empty || payload.C <= 0)
            {
                return Result<(DateTimeOffset CreatedAtUtc, Guid Id)?>.Failure(
                    new Error("OPPORTUNITY_WORK_IMAGE_CURSOR_INVALID", "Invalid cursor payload."));
            }

            var createdAt = DateTimeOffset.FromUnixTimeMilliseconds(payload.C);
            return Result<(DateTimeOffset CreatedAtUtc, Guid Id)?>.Success((createdAt, payload.Id));
        }
        catch
        {
            return Result<(DateTimeOffset CreatedAtUtc, Guid Id)?>.Failure(
                new Error("OPPORTUNITY_WORK_IMAGE_CURSOR_INVALID", "Invalid cursor format."));
        }
    }
}
