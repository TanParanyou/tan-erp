using TanErp.Application.Commercial;
using TanErp.Domain.Commercial;
using Xunit;

namespace TanErp.UnitTests.Commercial;

public class AcceptanceOtpTests
{
    [Fact]
    public void AcceptanceOtpOptions_Validate_EnforcesKeyLengthCodeLengthAndPositiveLimitsWhenRequired()
    {
        var disabled = new AcceptanceOtpOptions { Required = false, HmacKey = "short" };
        disabled.Validate();

        Assert.Throws<InvalidOperationException>(() =>
            new AcceptanceOtpOptions { Required = true, HmacKey = "too-short" }.Validate());

        Assert.Throws<InvalidOperationException>(() =>
            new AcceptanceOtpOptions { Required = true, HmacKey = new string('k', 32), CodeLength = 3 }.Validate());

        Assert.Throws<InvalidOperationException>(() =>
            new AcceptanceOtpOptions { Required = true, HmacKey = new string('k', 32), ExpiryMinutes = 0 }.Validate());

        var valid = new AcceptanceOtpOptions { Required = true, HmacKey = new string('k', 32), CodeLength = 6 };
        valid.Validate();
    }

    [Fact]
    public void QuotationAcceptanceLink_AndAcceptanceOtpChallenge_EnforceDestinationsLockAndTransitions()
    {
        var now = DateTimeOffset.UtcNow;
        var validHash = new string('a', 64);
        var verifyHash = new string('b', 64);

        Assert.Equal("+66812345678", QuotationAcceptanceLink.NormalizePhone("+66 (81) 234-5678"));
        Assert.Null(QuotationAcceptanceLink.NormalizePhone("   "));

        Assert.Throws<AcceptanceLinkException>(() =>
            new QuotationAcceptanceLink(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), validHash, null, now.AddDays(3), Guid.NewGuid(), now, recipientEmail: "not-an-email"));

        Assert.Throws<AcceptanceLinkException>(() =>
            new QuotationAcceptanceLink(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), validHash, null, now.AddDays(3), Guid.NewGuid(), now, recipientPhone: "0812345678"));

        var link = new QuotationAcceptanceLink(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), validHash, "คุณสมชาย", now.AddDays(3), Guid.NewGuid(), now,
            recipientEmail: "buyer@example.test", recipientPhone: "+66 81-234-5678");
        Assert.Equal("buyer@example.test", link.RecipientEmail);
        Assert.Equal("+66812345678", link.RecipientPhone);
        Assert.Null(link.OtpLockedAtUtc);
        link.LockOtp(now.AddMinutes(2));
        Assert.Equal(now.AddMinutes(2).ToUniversalTime(), link.OtpLockedAtUtc);

        var challenge = new AcceptanceOtpChallenge(
            Guid.NewGuid(), link.OrganizationId, link.Id, OtpChannels.Email, validHash, now, now.AddMinutes(10));

        challenge.MarkVerified(verifyHash, now.AddMinutes(1));
        Assert.Equal(verifyHash, challenge.VerificationTokenHash);
        Assert.Equal(now.AddMinutes(1).ToUniversalTime(), challenge.VerifiedAtUtc);
        Assert.Null(challenge.ConsumedAtUtc);

        challenge.Consume(now.AddMinutes(2));
        Assert.Equal(now.AddMinutes(2).ToUniversalTime(), challenge.ConsumedAtUtc);
    }
}
