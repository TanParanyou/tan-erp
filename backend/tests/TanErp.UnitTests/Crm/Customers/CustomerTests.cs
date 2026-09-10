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
        Assert.Equal("0812345678", primaryContact.NormalizedPhone);
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

    [Fact]
    public void CreateDraft_WhenLeadSourceAndLineIdProvided_SetsPropertiesCorrectly()
    {
        var contactInput = new PrimaryContactInput(
            "คุณวิชัย",
            "ผู้จัดการทั่วไป",
            "0891234567",
            "wichai@example.test",
            "phone",
            LineId: "wichai_line_01");

        var customer = Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Person,
            "คุณวิชัย ใจมั่น",
            null,
            PreferredLocale.Thai,
            contactInput,
            DateTimeOffset.UtcNow,
            leadSource: CustomerLeadSource.Referral);

        Assert.Equal(CustomerLeadSource.Referral, customer.LeadSource);
        var contact = customer.Contacts.First();
        Assert.Equal("wichai_line_01", contact.LineId);
    }

    [Fact]
    public void CreateDraft_WhenInvalidLeadSource_ThrowsArgumentException()
    {
        var contactInput = new PrimaryContactInput("คุณสมชาย", null, "0812345678", null, "phone");

        Assert.Throws<ArgumentException>(() => Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Person,
            "สมชาย ใจดี",
            null,
            PreferredLocale.Thai,
            contactInput,
            DateTimeOffset.UtcNow,
            leadSource: "invalid_source"));
    }

    [Fact]
    public void CreateDraft_WhenLineIdExceeds100Chars_ThrowsArgumentException()
    {
        var longLineId = new string('x', 101);
        var contactInput = new PrimaryContactInput("คุณสมชาย", null, "0812345678", null, "phone", LineId: longLineId);

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
    public void CustomerLeadSource_IsValid_HandlesNullAndWhitespaceSafely()
    {
        Assert.False(CustomerLeadSource.IsValid(null));
        Assert.False(CustomerLeadSource.IsValid(""));
        Assert.False(CustomerLeadSource.IsValid("   "));
        Assert.False(CustomerLeadSource.IsValid("unknown"));
        Assert.True(CustomerLeadSource.IsValid("walk_in"));
        Assert.True(CustomerLeadSource.IsValid("  referral  "));
    }

    [Fact]
    public void ContactChannel_IsValid_HandlesNullAndWhitespaceSafely()
    {
        Assert.False(ContactChannel.IsValid(null));
        Assert.False(ContactChannel.IsValid(""));
        Assert.False(ContactChannel.IsValid("   "));
        Assert.False(ContactChannel.IsValid("telegram"));
        Assert.True(ContactChannel.IsValid("phone"));
        Assert.True(ContactChannel.IsValid("line"));
    }

    [Fact]
    public void Activate_WhenValidDraftCustomerAndMatchingVersion_ActivatesAndChangesRowVersion()
    {
        var customer = Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Person,
            "คุณลูกค้า ทดสอบ",
            null,
            PreferredLocale.Thai,
            new PrimaryContactInput("คุณผู้ติดต่อ", null, "+66812345678", null, "phone"),
            DateTimeOffset.UtcNow);

        var originalVersion = customer.RowVersion;
        var outcome = customer.Activate(originalVersion);

        Assert.Equal(CustomerActivationOutcome.Activated, outcome);
        Assert.Equal(CustomerStatus.Active, customer.Status);
        Assert.NotEqual(originalVersion, customer.RowVersion);
    }

    [Fact]
    public void Activate_WhenVersionMismatch_ReturnsVersionConflict()
    {
        var customer = Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Person,
            "คุณลูกค้า ทดสอบ",
            null,
            PreferredLocale.Thai,
            new PrimaryContactInput("คุณผู้ติดต่อ", null, "+66812345678", null, "phone"),
            DateTimeOffset.UtcNow);

        var originalVersion = customer.RowVersion;
        var outcome = customer.Activate(Guid.NewGuid());

        Assert.Equal(CustomerActivationOutcome.VersionConflict, outcome);
        Assert.Equal(CustomerStatus.Draft, customer.Status);
        Assert.Equal(originalVersion, customer.RowVersion);
    }

    [Fact]
    public void Activate_WhenAlreadyActive_ReturnsInvalidState()
    {
        var customer = Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Person,
            "คุณลูกค้า ทดสอบ",
            null,
            PreferredLocale.Thai,
            new PrimaryContactInput("คุณผู้ติดต่อ", null, "+66812345678", null, "phone"),
            DateTimeOffset.UtcNow);

        var originalVersion = customer.RowVersion;
        var firstOutcome = customer.Activate(originalVersion);
        Assert.Equal(CustomerActivationOutcome.Activated, firstOutcome);

        var secondOutcome = customer.Activate(customer.RowVersion);
        Assert.Equal(CustomerActivationOutcome.InvalidState, secondOutcome);
    }

    [Fact]
    public void CreateDraft_WhenInternationalPhone_PreservesPlusPrefixInNormalizedPhone()
    {
        var contactInput = new PrimaryContactInput("John Doe", "Director", "+1 (202) 555-0125", "john@example.com", "phone");

        var customer = Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Person,
            "จอห์น โด",
            "John Doe",
            PreferredLocale.English,
            contactInput,
            DateTimeOffset.UtcNow);

        var primaryContact = customer.Contacts.First();
        Assert.Equal("+12025550125", primaryContact.NormalizedPhone);
    }

    [Fact]
    public void CreateDraft_WithLeadSourceOtherAndNote_PersistsBoth()
    {
        var contactInput = new PrimaryContactInput("คุณสมศักดิ์", null, "0812345678", null, "phone");

        var customer = Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Person,
            "สมศักดิ์ มั่งคั่ง",
            null,
            PreferredLocale.Thai,
            contactInput,
            DateTimeOffset.UtcNow,
            leadSource: CustomerLeadSource.Other,
            leadSourceNote: "Exhibition 2026");

        Assert.Equal("other", customer.LeadSource);
        Assert.Equal("Exhibition 2026", customer.LeadSourceNote);
    }

    [Fact]
    public void CreateDraft_WhenLeadSourceNoteExceeds200Chars_ThrowsArgumentException()
    {
        var contactInput = new PrimaryContactInput("คุณสมศักดิ์", null, "0812345678", null, "phone");
        var longNote = new string('A', 201);

        Assert.Throws<ArgumentException>(() => Customer.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CustomerType.Person,
            "สมศักดิ์ มั่งคั่ง",
            null,
            PreferredLocale.Thai,
            contactInput,
            DateTimeOffset.UtcNow,
            leadSource: CustomerLeadSource.Other,
            leadSourceNote: longNote));
    }
}

