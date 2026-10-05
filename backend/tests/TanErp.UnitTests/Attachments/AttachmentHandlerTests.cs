using TanErp.Application.Attachments;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Domain.Attachments;
using TanErp.Domain.Service;
using Xunit;

namespace TanErp.UnitTests.Attachments;

public class AttachmentHandlerTests
{
    private static readonly Guid Org = Guid.NewGuid();
    private static readonly Guid Branch = Guid.NewGuid();
    private static readonly Guid User = Guid.NewGuid();
    private static readonly Guid Membership = Guid.NewGuid();
    private static readonly Guid OwnerId = Guid.NewGuid();
    private static readonly AttachmentCaller Caller = new("uid", Membership, "trace-1");

    private readonly FakeAccess _access = new();
    private readonly FakeScopes _scopes = new();
    private readonly FakeStore _store = new();
    private readonly AttachmentHandler _handler;

    public AttachmentHandlerTests()
    {
        _handler = new AttachmentHandler(_access, _scopes, _store);
        _access.Granted.UnionWith(["installations.read", "installations.operate", "installations.handover"]);
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, OwnerId, new AttachmentOwnerScope(Org, Branch, InstallationStatus.InProgress));
    }

    private static AttachFilesInput Attach(params Guid[] ids) => new("evidence", ids);

    private static SignatureCaptureInput Sign(string? name = "คุณสมชาย", bool consent = true, string? version = "handover-2026-10-v1", string? purpose = "handover") =>
        new(purpose, name, "เจ้าของบ้าน", Guid.NewGuid(), consent, version);

    [Fact]
    public void Registry_CoversExactlyTheDomainWhitelist()
    {
        Assert.True(AttachmentOwnerRegistry.OwnerTypes.ToHashSet().SetEquals(AttachmentOwnerTypes.All));
    }

    [Fact]
    public void Registry_Find_IsExactMatchOnly()
    {
        Assert.NotNull(AttachmentOwnerRegistry.Find("installation-job"));
        Assert.Null(AttachmentOwnerRegistry.Find("Installation-Job"));
        Assert.Null(AttachmentOwnerRegistry.Find(null));
        Assert.Null(AttachmentOwnerRegistry.Find("customer"));
    }

    [Fact]
    public async Task UnregisteredOwnerType_IsRejectedBeforeAnyAccessOrStoreCall()
    {
        var result = await _handler.ListAsync(Caller, "customer", OwnerId);

        Assert.Equal("ATTACHMENT_OWNER_TYPE_INVALID", result.Error.Code);
        Assert.Empty(_access.Requested);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task MissingPermission_ReturnsPermissionDenied_WithoutLookingUpTheOwner()
    {
        _access.Granted.Clear();

        var result = await _handler.ListAsync(Caller, "installation-job", OwnerId);

        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Equal(0, _scopes.Lookups);
    }

    [Fact]
    public async Task UnknownOwner_AndOwnerOfAnotherOrganization_BothReturnNotFound()
    {
        var unknown = await _handler.ListAsync(Caller, "installation-job", Guid.NewGuid());
        Assert.Equal("RESOURCE_NOT_FOUND", unknown.Error.Code);

        var foreign = Guid.NewGuid();
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, foreign, new AttachmentOwnerScope(Guid.NewGuid(), Branch, InstallationStatus.InProgress));
        var result = await _handler.ListAsync(Caller, "installation-job", foreign);
        Assert.Equal("RESOURCE_NOT_FOUND", result.Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task OwnerOfAnotherBranch_IsNotFound_ForABranchScopedCaller_ButVisibleToAnOrganizationWideCaller()
    {
        _access.BranchId = Guid.NewGuid();
        Assert.Equal("RESOURCE_NOT_FOUND", (await _handler.ListAsync(Caller, "installation-job", OwnerId)).Error.Code);

        _access.BranchId = null;
        Assert.True((await _handler.ListAsync(Caller, "installation-job", OwnerId)).IsSuccess);
    }

    [Fact]
    public async Task OwnerTypeIsNormalizedBeforeLookup()
    {
        var result = await _handler.ListAsync(Caller, "  INSTALLATION-JOB ", OwnerId);
        Assert.True(result.IsSuccess);
        Assert.Equal("installation-job", _store.LastOwnerType);
    }

    [Fact]
    public async Task Attach_RejectsInvalidInput()
    {
        Assert.Equal("ATTACHMENT_FIELD_INVALID", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", null)).Error.Code);
        Assert.Equal("ATTACHMENT_FIELD_INVALID", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", Attach())).Error.Code);
        Assert.Equal("ATTACHMENT_FIELD_INVALID", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", Attach(Guid.Empty))).Error.Code);
        Assert.Equal("ATTACHMENT_FIELD_INVALID", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1",
            Attach(Enumerable.Range(0, AttachmentHandler.MaxFilesPerRequest + 1).Select(_ => Guid.NewGuid()).ToArray()))).Error.Code);
        Assert.Equal("ATTACHMENT_DUPLICATE", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1",
            Attach(Guid.Parse("11111111-1111-1111-1111-111111111111"), Guid.Parse("11111111-1111-1111-1111-111111111111")))).Error.Code);
        Assert.Equal("ATTACHMENT_PURPOSE_INVALID", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", new AttachFilesInput("misc", [Guid.NewGuid()]))).Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task Attach_RequiresTheManagePermission_NotJustRead()
    {
        _access.Granted.Remove("installations.operate");
        var result = await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", Attach(Guid.NewGuid()));
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Theory]
    [InlineData(InstallationStatus.HandedOver)]
    [InlineData(InstallationStatus.Cancelled)]
    public async Task Attach_AndUnlink_AreLockedForClosedOwners(string status)
    {
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, OwnerId, new AttachmentOwnerScope(Org, Branch, status));

        Assert.Equal("ATTACHMENT_OWNER_LOCKED", (await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", Attach(Guid.NewGuid()))).Error.Code);
        Assert.Equal("ATTACHMENT_OWNER_LOCKED", (await _handler.UnlinkAsync(Caller, "installation-job", OwnerId, Guid.NewGuid())).Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task Attach_PassesNormalizedInputAndHashedKeyToTheStore()
    {
        var file = Guid.NewGuid();
        var result = await _handler.AttachAsync(Caller, "installation-job", OwnerId, "key-1", new AttachFilesInput(" EVIDENCE ", [file]));

        Assert.True(result.IsSuccess);
        Assert.Equal("evidence", _store.LastAttach!.Purpose);
        Assert.Equal(new[] { file }, _store.LastAttach.FileIds);
        Assert.Equal(64, _store.LastKeyHash!.Length);
        Assert.NotEqual("key-1", _store.LastKeyHash);
        Assert.Equal("trace-1", _store.LastTraceId);
    }

    [Fact]
    public async Task Attach_LimitCountsOnlyActiveLinksOfThatExactOwner()
    {
        // 50 links on another owner must not block this owner, which only has 1.
        var other = Guid.NewGuid();
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, other, new AttachmentOwnerScope(Org, Branch, InstallationStatus.InProgress));
        _store.Seed("installation-job", other, AttachmentLink.MaxActiveLinksPerOwner);
        _store.Seed("installation-job", OwnerId, 1);

        var result = await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", Attach(Guid.NewGuid()));

        Assert.True(result.IsSuccess);
        Assert.Equal((Org, "installation-job", OwnerId), _store.LastListScope);
    }

    [Fact]
    public async Task Attach_IsRejectedWhenTheOwnerItselfWouldExceedTheLimit()
    {
        _store.Seed("installation-job", OwnerId, AttachmentLink.MaxActiveLinksPerOwner - 1);

        Assert.True((await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k1", Attach(Guid.NewGuid()))).IsSuccess);
        var result = await _handler.AttachAsync(Caller, "installation-job", OwnerId, "k2", Attach(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Equal("ATTACHMENT_LIMIT_EXCEEDED", result.Error.Code);
        Assert.NotEqual(2, _store.LastAttach!.FileIds!.Count);
    }

    [Fact]
    public async Task Attach_DifferentFileListsProduceDifferentPayloadHashes()
    {
        await _handler.AttachAsync(Caller, "installation-job", OwnerId, "key-1", Attach(Guid.NewGuid()));
        var first = _store.LastPayloadHash;
        await _handler.AttachAsync(Caller, "installation-job", OwnerId, "key-1", Attach(Guid.NewGuid()));
        Assert.NotEqual(first, _store.LastPayloadHash);
    }

    [Fact]
    public async Task Signature_RequiresTheSignPermission()
    {
        _access.Granted.Remove("installations.handover");
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, OwnerId, new AttachmentOwnerScope(Org, Branch, InstallationStatus.ReadyForHandover));
        Assert.Equal("PERMISSION_DENIED", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign())).Error.Code);
    }

    [Fact]
    public async Task Signature_IsLockedUnlessTheOwnerIsReadyForHandover()
    {
        Assert.Equal("ATTACHMENT_OWNER_LOCKED", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign())).Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task Signature_ValidatesNameConsentAndPurpose()
    {
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, OwnerId, new AttachmentOwnerScope(Org, Branch, InstallationStatus.ReadyForHandover));

        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign(name: "ก"))).Error.Code);
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", null)).Error.Code);
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign(consent: false))).Error.Code);
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign(version: "old-v0"))).Error.Code);
        Assert.Equal("ATTACHMENT_PURPOSE_INVALID", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k", Sign(purpose: "evidence"))).Error.Code);
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", (await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "k",
            new SignatureCaptureInput("handover", "คุณสมชาย", null, Guid.Empty, true, "handover-2026-10-v1"))).Error.Code);
        Assert.Equal(0, _store.Calls);
    }

    [Fact]
    public async Task Signature_PassesNormalizedEvidenceToTheStore()
    {
        _scopes.Add(AttachmentOwnerTypes.InstallationJob, OwnerId, new AttachmentOwnerScope(Org, Branch, InstallationStatus.ReadyForHandover));

        var result = await _handler.CaptureSignatureAsync(Caller, "installation-job", OwnerId, "key-9",
            new SignatureCaptureInput(" Handover ", "  คุณสมชาย  ", "  เจ้าของบ้าน ", Guid.NewGuid(), true, "handover-2026-10-v1"));

        Assert.True(result.IsSuccess);
        Assert.Equal("handover", _store.LastSignature!.Purpose);
        Assert.Equal("คุณสมชาย", _store.LastSignature.SignerName);
        Assert.Equal("เจ้าของบ้าน", _store.LastSignature.SignerRole);
        Assert.Equal("handover-2026-10-v1", _store.LastSignature.ConsentTextVersion);
    }

    // ----- fakes -----------------------------------------------------------------------------------

    private sealed class FakeAccess : IRequestAccessResolver
    {
        public HashSet<string> Granted { get; } = new(StringComparer.Ordinal);
        public List<string> Requested { get; } = new();
        public Guid? BranchId { get; set; } = Branch;

        public Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string permissionKey, CancellationToken cancellationToken = default)
        {
            Requested.Add(permissionKey);
            return Task.FromResult(Granted.Contains(permissionKey)
                ? Result<RequestAccessContext>.Success(new RequestAccessContext(User, membershipId, Org, BranchId, permissionKey, "organization"))
                : Result<RequestAccessContext>.Failure(new Error("PERMISSION_DENIED", "denied")));
        }
    }

    private sealed class FakeScopes : IAttachmentOwnerScopeReader
    {
        private readonly Dictionary<(string, Guid), AttachmentOwnerScope> _items = new();
        public int Lookups { get; private set; }

        public void Add(string ownerType, Guid ownerId, AttachmentOwnerScope scope) => _items[(ownerType, ownerId)] = scope;

        public Task<AttachmentOwnerScope?> FindAsync(string ownerType, Guid ownerId, Guid organizationId, CancellationToken ct = default)
        {
            Lookups++;
            return Task.FromResult(_items.TryGetValue((ownerType, ownerId), out var scope) ? scope : null);
        }
    }

    private sealed class FakeStore : IAttachmentStore
    {
        public int Calls { get; private set; }
        public string? LastOwnerType { get; private set; }
        public AttachFilesInput? LastAttach { get; private set; }
        public SignatureCaptureCommand? LastSignature { get; private set; }
        public string? LastKeyHash { get; private set; }
        public string? LastPayloadHash { get; private set; }
        public string? LastTraceId { get; private set; }

        public List<AttachmentLinkProjection> ExistingLinks { get; } = new();
        public (Guid Org, string OwnerType, Guid OwnerId)? LastListScope { get; private set; }

        public void Seed(string ownerType, Guid ownerId, int count, string purpose = "evidence")
        {
            for (var i = 0; i < count; i++)
            {
                ExistingLinks.Add(new AttachmentLinkProjection(Guid.NewGuid(), ownerType, ownerId, Guid.NewGuid(), purpose, "f.png", "image/png", 1, "/x",
                    new AttachmentPerson(User, "u"), DateTimeOffset.UtcNow));
            }
        }

        // Contract: returns only the active links of exactly this (organization, ownerType, ownerId).
        public Task<IReadOnlyList<AttachmentLinkProjection>> ListLinksAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default)
        {
            Calls++;
            LastOwnerType = ownerType;
            LastListScope = (organizationId, ownerType, ownerId);
            return Task.FromResult<IReadOnlyList<AttachmentLinkProjection>>(
                ExistingLinks.Where(l => l.OwnerType == ownerType && l.OwnerId == ownerId).ToList());
        }

        public Task<Result<IReadOnlyList<AttachmentLinkProjection>>> AttachAsync(RequestAccessContext access, string ownerType, Guid ownerId, AttachFilesInput input, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
        {
            Calls++;
            LastOwnerType = ownerType;
            LastAttach = input;
            LastKeyHash = keyHash;
            LastPayloadHash = payloadHash;
            LastTraceId = traceId;
            return Task.FromResult(Result<IReadOnlyList<AttachmentLinkProjection>>.Success([]));
        }

        public Task<Result<bool>> UnlinkAsync(RequestAccessContext access, string ownerType, Guid ownerId, Guid linkId, string traceId, CancellationToken ct = default)
        {
            Calls++;
            return Task.FromResult(Result<bool>.Success(true));
        }

        public Task<IReadOnlyList<SignatureCaptureProjection>> ListSignaturesAsync(Guid organizationId, string ownerType, Guid ownerId, CancellationToken ct = default)
        {
            Calls++;
            return Task.FromResult<IReadOnlyList<SignatureCaptureProjection>>([]);
        }

        public Task<Result<SignatureCaptureProjection>> CaptureSignatureAsync(RequestAccessContext access, string ownerType, Guid ownerId, SignatureCaptureCommand command, string keyHash, string payloadHash, string traceId, CancellationToken ct = default)
        {
            Calls++;
            LastSignature = command;
            LastKeyHash = keyHash;
            LastPayloadHash = payloadHash;
            return Task.FromResult(Result<SignatureCaptureProjection>.Success(new SignatureCaptureProjection(
                Guid.NewGuid(), ownerType, ownerId, command.Purpose, command.SignerName, command.SignerRole, DateTimeOffset.UtcNow,
                command.ImageFileId, "/x", command.ConsentTextVersion, new string('a', 64), new AttachmentPerson(User, "u"))));
        }
    }
}
