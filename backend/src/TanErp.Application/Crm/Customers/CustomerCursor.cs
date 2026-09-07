using System.Text;
using System.Text.Json;

namespace TanErp.Application.Crm.Customers;

public sealed record CustomerCursorData(string Name, Guid Id);

public static class CustomerCursor
{
    public static string Encode(string normalizedDisplayName, Guid id)
    {
        var data = new CustomerCursorData(normalizedDisplayName, id);
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);
        var base64 = Convert.ToBase64String(bytes);
        return base64.Replace("+", "-").Replace("/", "_").TrimEnd('=');
    }

    public static bool TryDecode(string cursor, out CustomerCursorData? data)
    {
        data = null;
        if (string.IsNullOrWhiteSpace(cursor)) return false;

        try
        {
            var base64 = cursor.Replace("-", "+").Replace("_", "/");
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            var bytes = Convert.FromBase64String(base64);
            var json = Encoding.UTF8.GetString(bytes);
            data = JsonSerializer.Deserialize<CustomerCursorData>(json);
            return data != null && data.Id != Guid.Empty;
        }
        catch
        {
            return false;
        }
    }
}
