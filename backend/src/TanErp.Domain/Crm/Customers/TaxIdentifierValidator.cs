namespace TanErp.Domain.Crm.Customers;

public static class TaxIdentifierValidator
{
    public static bool IsValid(string value)
    {
        if (value.Length != 13 || value.Any(c => !char.IsDigit(c))) return false;
        var sum = 0;
        for (var index = 0; index < 12; index++) sum += (value[index] - '0') * (13 - index);
        var checkDigit = (11 - sum % 11) % 10;
        return checkDigit == value[12] - '0';
    }
}
