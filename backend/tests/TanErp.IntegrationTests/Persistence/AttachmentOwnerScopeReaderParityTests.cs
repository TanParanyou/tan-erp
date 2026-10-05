using TanErp.Application.Attachments;
using TanErp.Domain.Attachments;
using TanErp.Infrastructure.Persistence.Attachments;
using Xunit;

namespace TanErp.IntegrationTests.Persistence;

public class AttachmentOwnerScopeReaderParityTests
{
    [Fact]
    public void EveryRegisteredOwnerType_HasAScopeReaderBranch_AndViceVersa()
    {
        var registry = AttachmentOwnerRegistry.OwnerTypes.ToHashSet(StringComparer.Ordinal);

        Assert.True(registry.SetEquals(AttachmentOwnerScopeReader.SupportedOwnerTypes),
            "AttachmentOwnerRegistry and AttachmentOwnerScopeReader.SupportedOwnerTypes must list the same owner types.");
        Assert.True(registry.SetEquals(AttachmentOwnerTypes.All),
            "AttachmentOwnerRegistry and AttachmentOwnerTypes must list the same owner types.");
    }
}
