namespace TanErp.Domain.Items;

public class ItemDomainException : Exception
{
    public string Code { get; }

    public ItemDomainException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public sealed class ItemValidationException : ItemDomainException
{
    public ItemValidationException(string code, string message) : base(code, message) { }
}

public sealed class ItemNotFoundException : ItemDomainException
{
    public ItemNotFoundException(Guid itemId)
        : base("ITEM_NOT_FOUND", $"Item '{itemId}' was not found.") { }
}

public sealed class ItemConflictException : ItemDomainException
{
    public ItemConflictException(string code, string message) : base(code, message) { }
}
