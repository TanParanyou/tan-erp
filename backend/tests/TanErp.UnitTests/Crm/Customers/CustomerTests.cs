using TanErp.Domain.Crm.Customers;
using Xunit;

namespace TanErp.UnitTests.Crm.Customers;

public class CustomerTests
{
    [Fact]
    public void CreateDraft_WhenValidOrganizationCustomer_SetsPropertiesAndGeneratesCode()
    {
        var customerId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b10");
        var orgId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var contactInput = new PrimaryContactInput(
            "คุณตัวอย่าง TEST_ONLY",
            "ผู้จัดการ",
            "+66 81 234 5678",
            "test@example.com",
            "phone");

        var customer = Customer.CreateDraft(
            customerId,
            orgId,
            actorId,
            CustomerType.Organization,
            "  บริษัท ตัวอย่าง จำกัด TEST_ONLY  ",
            "  Sample Company Ltd TEST_ONLY  ",
            PreferredLocale.Thai,
            contactInput,
            now);

        Assert.Equal(customerId, customer.Id);
        Assert.Equal(orgId, customer.OrganizationId);
        Assert.Equal(CustomerType.Organization, customer.CustomerType);
        Assert.Equal("บริษัท ตัวอย่าง จำกัด TEST_ONLY", customer.DisplayNameTh);
        Assert.Equal("Sample Company Ltd TEST_ONLY", customer.DisplayNameEn);
        Assert.Equal("CUS-019A3CF896F0", customer.Code);
        Assert.Equal(CustomerStatus.Draft, customer.Status);
        Assert.Equal(PreferredLocale.Thai, customer.PreferredLocale);
        Assert.NotEqual(Guid.Empty, customer.RowVersion);

        Assert.Single(customer.Contacts);
        var primaryContact = customer.Contacts.First();
        Assert.True(primaryContact.IsPrimary);
        Assert.Equal("active", primaryContact.Status);
        Assert.Equal("คุณตัวอย่าง TEST_ONLY", primaryContact.Name);
        Assert.Equal("+66 81 234 5678", primaryContact.Phone);
        Assert.Equal("66812345678", primaryContact.NormalizedPhone);
        Assert.Equal("test@example.com", primaryContact.Email);
        Assert.Equal("test@example.com", primaryContact.NormalizedEmail);
        Assert.Equal("phone", primaryContact.PreferredChannel);
    }

    [Fact]
    public void CreateDraft_WhenBlankDisplayNameTh_ThrowsArgumentException()
    {
        var contactInput = new PrimaryContactInput("คุณสมชาย", null, "+66812345678", null, "phone");

        Assert.Throws<ArgumentException>(() => Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Person,
            "   ",
            null,
            PreferredLocale.Thai,
            contactInput,
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void CreateDraft_WhenInvalidCustomerType_ThrowsArgumentException()
    {
        var contactInput = new PrimaryContactInput("คุณสมชาย", null, "+66812345678", null, "phone");

        Assert.Throws<ArgumentException>(() => Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "unknown-type",
            "สมชาย ใจดี",
            null,
            PreferredLocale.Thai,
            contactInput,
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void CreateDraft_WhenContactHasNoPhoneAndNoEmail_ThrowsArgumentException()
    {
        var contactInput = new PrimaryContactInput("คุณสมชาย", null, "   ", null, "phone");

        Assert.Throws<ArgumentException>(() => Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Person,
            "สมชาย ใจดี",
            null,
            PreferredLocale.Thai,
            contactInput,
            DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Customer_ToString_DoesNotContainPII()
    {
        var contactInput = new PrimaryContactInput(
            "คุณตัวอย่างความลับ",
            "กรรมการ",
            "+66812345678",
            "secret-email@example.com",
            "phone");

        var customer = Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Organization,
            "บริษัท ความลับ จำกัด",
            null,
            PreferredLocale.Thai,
            contactInput,
            DateTimeOffset.UtcNow);

        var customerString = customer.ToString();
        var contactString = customer.Contacts.First().ToString();

        Assert.DoesNotContain("+66812345678", customerString);
        Assert.DoesNotContain("secret-email@example.com", customerString);
        Assert.DoesNotContain("+66812345678", contactString);
        Assert.DoesNotContain("secret-email@example.com", contactString);
    }
}
