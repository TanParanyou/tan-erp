using TanErp.Domain.Common;
using TanErp.Domain.IdentityAccess;
using Xunit;

namespace TanErp.UnitTests.IdentityAccess;

public class WaveAClosureDomainTests
{
    [Fact]
    public void AuditRedactor_RedactsDefaultAndConfiguredKeysCaseInsensitivelyAtAnyDepth()
    {
        try
        {
            AuditRedactor.Configure(null);
            var json = """
                {
                  "Email": "secret@example.test",
                  "phone": "0812345678",
                  "nested": {
                    "DisplayName": "สมชาย ใจดี",
                    "taxId": "1234567890123",
                    "safeCode": "KEEP-ME"
                  },
                  "items": [
                    { "Mobile": "0899999999", "amount": 500 },
                    "plain-string"
                  ]
                }
                """;

            var redacted = AuditRedactor.Redact(json);

            Assert.DoesNotContain("secret@example.test", redacted);
            Assert.DoesNotContain("0812345678", redacted);
            Assert.DoesNotContain("สมชาย ใจดี", redacted);
            Assert.DoesNotContain("1234567890123", redacted);
            Assert.DoesNotContain("0899999999", redacted);
            Assert.Contains("\"KEEP-ME\"", redacted);
            Assert.Contains("500", redacted);
            Assert.Contains(AuditRedactor.Placeholder, redacted);

            // Custom deny-list replaces default keys, and empty list restores default
            AuditRedactor.Configure(new[] { "customSecret" });
            var customJson = """{"email":"visible@example.test","CustomSecret":"hidden-val"}""";
            var customRedacted = AuditRedactor.Redact(customJson);
            Assert.Contains("visible@example.test", customRedacted);
            Assert.DoesNotContain("hidden-val", customRedacted);

            // Non-JSON input is returned untouched without throwing
            Assert.Equal("not-json", AuditRedactor.Redact("not-json"));
        }
        finally
        {
            AuditRedactor.Configure(null);
        }
    }

    [Fact]
    public void AuditEvent_Constructor_AutomaticallyRedactsPiiInChangesJson()
    {
        AuditRedactor.Configure(null);
        var evt = new AuditEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "customers.updated",
            "Customer",
            Guid.NewGuid().ToString(),
            DateTimeOffset.UtcNow,
            "trace-1",
            """{"email":"leak@example.test","status":"active"}""");

        Assert.DoesNotContain("leak@example.test", evt.ChangesJson);
        Assert.Contains(AuditRedactor.Placeholder, evt.ChangesJson);
        Assert.Contains("\"active\"", evt.ChangesJson);
    }

    [Fact]
    public void UserOrganizationPolicy_AllowsOnlySameOrganizationOrNewUser()
    {
        var orgA = Guid.NewGuid();
        var orgB = Guid.NewGuid();

        Assert.True(UserOrganizationPolicy.MayHoldMembershipIn(Array.Empty<Guid>(), orgA));
        Assert.True(UserOrganizationPolicy.MayHoldMembershipIn(new[] { orgA, orgA }, orgA));
        Assert.False(UserOrganizationPolicy.MayHoldMembershipIn(new[] { orgB }, orgA));
        Assert.False(UserOrganizationPolicy.MayHoldMembershipIn(new[] { orgA, orgB }, orgA));
    }
}
