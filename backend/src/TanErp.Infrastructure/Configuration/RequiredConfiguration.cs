namespace TanErp.Infrastructure.Configuration;

public static class RequiredConfiguration
{
    public static string Require(string? value, string key)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Required configuration '{key}' is missing.");
        }

        return value;
    }
}
