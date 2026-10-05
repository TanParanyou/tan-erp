using TanErp.Domain.Files;
using Xunit;

namespace TanErp.UnitTests.Files;

public class FileGatePermissionsTests
{
    [Theory]
    [InlineData("installation-job", true)]
    [InlineData("INSTALLATION-JOB", true)]
    [InlineData("installation_job", false)]
    [InlineData("customer", true)]
    [InlineData("unknown", false)]
    public void FileParentTypes_AcceptsRegisteredOwnerTypesAndExistingTypes(string parentType, bool expected)
    {
        Assert.Equal(expected, FileParentTypes.IsValid(parentType));
    }

    [Fact]
    public void UploadGate_KeepsTheExistingPermissionsAndAddsTheRegisteredOwnerManagePermissions()
    {
        Assert.Contains("opportunities.update", TanErp.Application.Files.FileGatePermissions.Upload);
        Assert.Contains("items.manage-images", TanErp.Application.Files.FileGatePermissions.Upload);
        Assert.Contains("installations.operate", TanErp.Application.Files.FileGatePermissions.Upload);
        Assert.Contains("installations.handover", TanErp.Application.Files.FileGatePermissions.Upload);
        Assert.DoesNotContain("installations.read", TanErp.Application.Files.FileGatePermissions.Upload);
    }

    [Fact]
    public void ReadGate_KeepsTheExistingPermissionsAndAddsTheRegisteredOwnerReadAndManagePermissions()
    {
        Assert.Contains("opportunities.read", TanErp.Application.Files.FileGatePermissions.Read);
        Assert.Contains("items.read", TanErp.Application.Files.FileGatePermissions.Read);
        Assert.Contains("installations.read", TanErp.Application.Files.FileGatePermissions.Read);
        Assert.Contains("installations.operate", TanErp.Application.Files.FileGatePermissions.Read);
        Assert.Contains("installations.handover", TanErp.Application.Files.FileGatePermissions.Read);
    }
}
