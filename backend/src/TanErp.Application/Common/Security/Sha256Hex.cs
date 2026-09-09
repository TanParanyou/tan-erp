using System.Security.Cryptography;
using System.Text;

namespace TanErp.Application.Common.Security;

public static class Sha256Hex
{
    public static string Compute(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
