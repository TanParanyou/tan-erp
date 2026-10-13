using System.Reflection;
using TanErp.Domain.Commercial;
using Xunit;

namespace TanErp.UnitTests.Commercial;

public class QuotationDocumentTests
{
    private static readonly string Hash = new('a', 64);

    private static QuotationDocument Create(string? sha256 = null, long byteCount = 1024) =>
        new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), sha256 ?? Hash, byteCount, "questpdf-2026.9.1/quotation-v1", DateTimeOffset.UtcNow, Guid.NewGuid());

    [Fact]
    public void Create_RejectsAMalformedHashAndAnEmptyFile()
    {
        Assert.Equal(Hash, Create().Sha256);
        Assert.Throws<ArgumentException>(() => Create("abc"));
        Assert.Throws<ArgumentException>(() => Create(new string('A', 64))); // upper case is not the canonical form
        Assert.Throws<ArgumentOutOfRangeException>(() => Create(byteCount: 0));
    }

    [Fact]
    public void ADocumentIsImmutable_ItHasNoPublicSetterOrMutatingMethod()
    {
        var type = typeof(QuotationDocument);
        Assert.All(type.GetProperties(BindingFlags.Public | BindingFlags.Instance), property =>
            Assert.True(property.SetMethod is null || !property.SetMethod.IsPublic, $"{property.Name} must not have a public setter"));
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(method => !method.IsSpecialName).Select(method => method.Name).ToList();
        Assert.True(methods.Count == 0, $"A document has no mutating method, found: {string.Join(", ", methods)}");
    }
}
