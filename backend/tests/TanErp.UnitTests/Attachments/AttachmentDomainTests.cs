using TanErp.Domain.Attachments;
using Xunit;

namespace TanErp.UnitTests.Attachments;

public class AttachmentDomainTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 5, 0, 0, 0, TimeSpan.Zero);
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Owner = Guid.NewGuid();
    private static readonly Guid Actor = Guid.NewGuid();
    private static readonly string Hash = new('a', 64);

    private static AttachmentLink Link(
        string ownerType = AttachmentOwnerTypes.InstallationJob,
        Guid? owner = null,
        Guid? fileOrg = null,
        Guid? file = null,
        string purpose = AttachmentPurposes.Evidence) =>
        new(Guid.NewGuid(), Org, ownerType, owner ?? Owner, fileOrg ?? Org, file ?? Guid.NewGuid(), purpose, Actor, Now);

    private static string CodeOf(Action action) => Assert.Throws<AttachmentDomainException>(action).Code;

    [Fact]
    public void Link_NormalizesOwnerTypeAndPurpose()
    {
        var link = Link(ownerType: "  Installation-Job ", purpose: " EVIDENCE ");
        Assert.Equal("installation-job", link.OwnerType);
        Assert.Equal("evidence", link.Purpose);
        Assert.True(link.IsActive);
    }

    [Theory]
    [InlineData("customer")]
    [InlineData("installation_job")]
    [InlineData("")]
    [InlineData("   ")]
    public void Link_RejectsUnregisteredOwnerType(string ownerType)
    {
        Assert.Equal("ATTACHMENT_OWNER_TYPE_INVALID", CodeOf(() => Link(ownerType: ownerType)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("misc")]
    public void Link_RejectsUnknownPurpose(string purpose)
    {
        Assert.Equal("ATTACHMENT_PURPOSE_INVALID", CodeOf(() => Link(purpose: purpose)));
    }

    [Fact]
    public void Link_RejectsFileFromAnotherOrganization()
    {
        Assert.Equal("ATTACHMENT_FILE_SCOPE_MISMATCH", CodeOf(() => Link(fileOrg: Guid.NewGuid())));
    }

    [Fact]
    public void Link_RejectsEmptyIds()
    {
        Assert.Equal("ATTACHMENT_FIELD_INVALID", CodeOf(() => Link(owner: Guid.Empty)));
        Assert.Equal("ATTACHMENT_FIELD_INVALID", CodeOf(() => Link(file: Guid.Empty)));
    }

    [Fact]
    public void AssertCanAdd_RejectsTheSameFileOwnerAndPurpose_ButAllowsADifferentPurpose()
    {
        var fileId = Guid.NewGuid();
        var existing = Link(file: fileId, purpose: AttachmentPurposes.Evidence);
        var active = new List<AttachmentLink> { existing };

        Assert.Equal("ATTACHMENT_DUPLICATE", CodeOf(() => AttachmentLink.AssertCanAdd(active, Link(file: fileId, purpose: AttachmentPurposes.Evidence))));
        AttachmentLink.AssertCanAdd(active, Link(file: fileId, purpose: AttachmentPurposes.General));
    }

    [Fact]
    public void AssertCanAdd_IgnoresRemovedLinks()
    {
        var fileId = Guid.NewGuid();
        var removed = Link(file: fileId);
        removed.Remove(Actor, Now);

        AttachmentLink.AssertCanAdd(new List<AttachmentLink> { removed }, Link(file: fileId));
    }

    [Fact]
    public void AssertCanAdd_RejectsMoreThanTheActiveLimit()
    {
        var active = Enumerable.Range(0, AttachmentLink.MaxActiveLinksPerOwner).Select(_ => Link()).ToList();
        Assert.Equal("ATTACHMENT_LIMIT_EXCEEDED", CodeOf(() => AttachmentLink.AssertCanAdd(active, Link())));
    }

    [Fact]
    public void Remove_MarksTheLinkInactive_AndASecondRemoveIsNotFound()
    {
        var link = Link();
        link.Remove(Actor, Now);

        Assert.False(link.IsActive);
        Assert.Equal(Now, link.RemovedAtUtc);
        Assert.Equal(Actor, link.RemovedByUserId);
        Assert.Equal("RESOURCE_NOT_FOUND", CodeOf(() => link.Remove(Actor, Now)));
    }

    private static SignatureCapture Signature(
        string signerName = "คุณสมชาย ใจดี",
        string? role = "เจ้าของบ้าน",
        string purpose = AttachmentPurposes.Handover,
        Guid? imageOrg = null,
        string consent = "handover-2026-10-v1",
        string? hash = null) =>
        new(Guid.NewGuid(), Org, AttachmentOwnerTypes.InstallationJob, Owner, purpose, signerName, role, Now,
            imageOrg ?? Org, Guid.NewGuid(), consent, hash ?? Hash, Actor);

    [Fact]
    public void Signature_StoresTrimmedEvidence()
    {
        var capture = Signature(signerName: "  คุณสมชาย  ", role: "  เจ้าของบ้าน ");
        Assert.Equal("คุณสมชาย", capture.SignerName);
        Assert.Equal("เจ้าของบ้าน", capture.SignerRole);
        Assert.Equal(Now, capture.SignedAtUtc);
        Assert.Equal(Hash, capture.ContentHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ก")]
    public void Signature_RejectsShortSignerName(string name)
    {
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", CodeOf(() => Signature(signerName: name)));
    }

    [Fact]
    public void Signature_RejectsOverlongNameRoleAndConsentVersion()
    {
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", CodeOf(() => Signature(signerName: new string('x', 201))));
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", CodeOf(() => Signature(role: new string('x', 101))));
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", CodeOf(() => Signature(consent: new string('x', 33))));
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", CodeOf(() => Signature(consent: " ")));
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABC")]
    public void Signature_RejectsAMalformedContentHash(string hash)
    {
        Assert.Equal("SIGNATURE_IMAGE_INVALID", CodeOf(() => Signature(hash: hash)));
        Assert.Equal("SIGNATURE_IMAGE_INVALID", CodeOf(() => Signature(hash: new string('A', 64))));
    }

    [Fact]
    public void Signature_RejectsAnImageFromAnotherOrganization_AndNonHandoverPurposes()
    {
        Assert.Equal("ATTACHMENT_FILE_SCOPE_MISMATCH", CodeOf(() => Signature(imageOrg: Guid.NewGuid())));
        Assert.Equal("ATTACHMENT_PURPOSE_INVALID", CodeOf(() => Signature(purpose: AttachmentPurposes.Evidence)));
    }
}
