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
}
