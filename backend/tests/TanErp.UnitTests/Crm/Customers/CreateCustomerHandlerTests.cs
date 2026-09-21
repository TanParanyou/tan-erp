using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Crm.Customers;
using TanErp.Application.Crm.Customers.CreateCustomer;
using TanErp.Application.Files;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.DocumentNumbering;
using Xunit;

namespace TanErp.UnitTests.Crm.Customers;

public class CreateCustomerHandlerTests
{
    private class FakeFileStore : IFileStore
    {
        public Task<Result<TanErp.Domain.Files.UploadedFile>> GetByIdAsync(Guid fileId, Guid organizationId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Result<TanErp.Domain.Files.UploadedFile>.Failure(new Error("NOT_FOUND", "Not found")));

        public Task SaveAsync(TanErp.Domain.Files.UploadedFile file, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<Result<TanErp.Domain.Files.FileUploadSession>> CreateSessionAsync(RequestAccessContext access, string parentType, Guid? parentId, Guid? creationIntentId, IReadOnlyList<TanErp.Application.Files.CreateUploadSession.FileSlotInput> files, string keyHash, string payloadHash, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Result<TanErp.Domain.Files.FileUploadSession>> GetCompletableSessionAsync(Guid organizationId, Guid actorUserId, Guid sessionId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Result<IReadOnlyList<TanErp.Domain.Files.UploadedFile>>> CompleteSessionAsync(Guid organizationId, Guid actorUserId, Guid sessionId, IReadOnlyList<TanErp.Domain.Files.UploadedFile> files, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Result<TanErp.Application.Files.GetFileContent.FileContentResult>> GetAuthorizedFileContentAsync(RequestAccessContext access, Guid fileId, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<Result<bool>> ValidateVerifiedFilesForParentAsync(Guid organizationId, Guid actorUserId, string parentType, Guid? parentId, Guid? creationIntentId, IReadOnlyCollection<Guid> fileIds, CancellationToken cancellationToken = default) =>
            Task.FromResult(Result<bool>.Success(true));

        public Task BindFilesToParentAsync(Guid organizationId, Guid? creationIntentId, Guid actualParentId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private class FakeRequestAccessResolver : IRequestAccessResolver
    {
        public HashSet<string> GrantedPermissions { get; } = new(StringComparer.Ordinal);
        public Guid UserId { get; set; } = Guid.NewGuid();
        public Guid OrgId { get; set; } = Guid.NewGuid();
        public Guid? BranchId { get; set; } = Guid.NewGuid();

        public Task<Result<RequestAccessContext>> ResolveAsync(
            string firebaseUid,
            Guid membershipId,
            string permissionKey,
            CancellationToken cancellationToken = default)
        {
            if (GrantedPermissions.Contains(permissionKey))
            {
                return Task.FromResult(Result<RequestAccessContext>.Success(
                    new RequestAccessContext(UserId, membershipId, OrgId, BranchId, permissionKey, "organization")));
            }

            return Task.FromResult(Result<RequestAccessContext>.Failure(
                new Error("PERMISSION_DENIED", "Permission denied.")));
        }
    }

    private class FakeCustomerCreationStore : ICustomerCreationStore
    {
        public PersistCustomerCreation? LastRequest { get; private set; }

        public Task<Result<PersistCustomerCreationResult>> CreateAsync(
            PersistCustomerCreation request,
            CancellationToken cancellationToken = default)
        {
            LastRequest = request;

            var projection = new CustomerProjection(
                request.Customer.Id,
                request.Customer.Code,
                request.Customer.CustomerType,
                request.Customer.DisplayNameTh,
                request.Customer.DisplayNameEn,
                request.Customer.PreferredLocale,
                request.Customer.Status,
                new CustomerContactProjection("คุณตัวอย่าง", null, "+66812345678", null, "phone", IsMasked: false),
                request.Customer.RowVersion,
                request.Customer.CreatedAtUtc);

            return Task.FromResult(Result<PersistCustomerCreationResult>.Success(
                new PersistCustomerCreationResult(projection, Array.Empty<DuplicateCustomerProjection>(), false)));
        }
    }

    private class FakeDocumentNumberGenerator : IDocumentNumberGenerator
    {
        public string GeneratedNumber { get; set; } = "CUS-00001";

        public Task<string> GenerateAsync(
            Guid organizationId,
            string documentType,
            Guid? branchId = null,
            DateTimeOffset? timestamp = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GeneratedNumber);
        }

        public string Preview(
            string formatPattern,
            string prefix,
            string? branchCode = null,
            DateTimeOffset? timestamp = null,
            long sampleSequence = 1,
            int defaultPadding = 4) =>
            $"{prefix}{sampleSequence.ToString().PadLeft(defaultPadding, '0')}";

        public string ComputePeriodKey(ResetPeriod resetPeriod, DateTimeOffset timestamp) => "GLOBAL";
    }

    private class FakeClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.Parse("2026-09-07T12:00:00Z");
    }

    [Fact]
    public async Task Handle_WhenMissingCustomersCreatePermission_ReturnsPermissionDenied()
    {
        var accessResolver = new FakeRequestAccessResolver();
        var store = new FakeCustomerCreationStore();
        var handler = new CreateCustomerHandler(accessResolver, store, new FakeClock(), new FakeFileStore(), new FakeDocumentNumberGenerator());

        var command = new CreateCustomerCommand(
            "uid-1", Guid.NewGuid(), "key-1234567890123456",
            "organization", "บริษัท ก", null, "th",
            new CreatePrimaryContact("นาย ก", null, "0812345678", null, "phone"),
            "trace-1");

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenMissingContactManagePermission_ReturnsPermissionDeniedWithoutWriting()
    {
        var resolver = new FakeRequestAccessResolver();
        resolver.GrantedPermissions.Add("customers.create");
        var store = new FakeCustomerCreationStore();
        var handler = new CreateCustomerHandler(resolver, store, new FakeClock(), new FakeFileStore(), new FakeDocumentNumberGenerator());

        var result = await handler.Handle(new CreateCustomerCommand(
            "uid-1", Guid.NewGuid(), "key-1234567890123456",
            "organization", "บริษัท ก", null, "th",
            new CreatePrimaryContact("นาย ก", null, "0812345678", null, "phone"),
            "trace-1"));

        Assert.True(result.IsFailure);
        Assert.Equal("PERMISSION_DENIED", result.Error.Code);
        Assert.Null(store.LastRequest);
    }

    [Fact]
    public async Task Handle_WhenBlankDisplayNameTh_ReturnsCustomerFieldRequired()
    {
        var accessResolver = new FakeRequestAccessResolver();
        accessResolver.GrantedPermissions.Add("customers.create");
        accessResolver.GrantedPermissions.Add("customer-contacts.manage");

        var store = new FakeCustomerCreationStore();
        var handler = new CreateCustomerHandler(accessResolver, store, new FakeClock(), new FakeFileStore(), new FakeDocumentNumberGenerator());

        var command = new CreateCustomerCommand(
            "uid-1", Guid.NewGuid(), "key-1234567890123456",
            "organization", "   ", null, "th",
            new CreatePrimaryContact("นาย ก", null, "0812345678", null, "phone"),
            "trace-1");

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal("CUSTOMER_FIELD_REQUIRED", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenContactHasNoPhoneAndNoEmail_ReturnsContactFieldRequired()
    {
        var accessResolver = new FakeRequestAccessResolver();
        accessResolver.GrantedPermissions.Add("customers.create");
        accessResolver.GrantedPermissions.Add("customer-contacts.manage");

        var store = new FakeCustomerCreationStore();
        var handler = new CreateCustomerHandler(accessResolver, store, new FakeClock(), new FakeFileStore(), new FakeDocumentNumberGenerator());

        var command = new CreateCustomerCommand(
            "uid-1", Guid.NewGuid(), "key-1234567890123456",
            "organization", "บริษัท ก", null, "th",
            new CreatePrimaryContact("นาย ก", null, "   ", null, "phone"),
            "trace-1");

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal("CONTACT_FIELD_REQUIRED", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenContactEmailIsMalformed_ReturnsContactFieldRequiredWithoutWriting()
    {
        var accessResolver = new FakeRequestAccessResolver();
        accessResolver.GrantedPermissions.Add("customers.create");
        accessResolver.GrantedPermissions.Add("customer-contacts.manage");
        var store = new FakeCustomerCreationStore();
        var handler = new CreateCustomerHandler(accessResolver, store, new FakeClock(), new FakeFileStore(), new FakeDocumentNumberGenerator());

        var result = await handler.Handle(new CreateCustomerCommand(
            "uid-1", Guid.NewGuid(), "key-1234567890123456",
            "organization", "บริษัท ก", null, "th",
            new CreatePrimaryContact("นาย ก", null, null, "not-an-email", "email"),
            "trace-1"));

        Assert.True(result.IsFailure);
        Assert.Equal("CONTACT_FIELD_REQUIRED", result.Error.Code);
        Assert.Null(store.LastRequest);
    }

    [Fact]
    public async Task Handle_WhenValid_PersistsCustomerWithAuditEventsAndHashes()
    {
        var accessResolver = new FakeRequestAccessResolver();
        accessResolver.GrantedPermissions.Add("customers.create");
        accessResolver.GrantedPermissions.Add("customer-contacts.manage");

        var store = new FakeCustomerCreationStore();
        var handler = new CreateCustomerHandler(accessResolver, store, new FakeClock(), new FakeFileStore(), new FakeDocumentNumberGenerator());

        var command = new CreateCustomerCommand(
            "uid-1", Guid.NewGuid(), "key-1234567890123456",
            "organization", "บริษัท ตัวอย่าง จำกัด", null, "th",
            new CreatePrimaryContact("คุณตัวอย่าง", "ผู้จัดการ", "+66812345678", "test@example.com", "phone"),
            "trace-1");

        var result = await handler.Handle(command);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(store.LastRequest);
        Assert.Equal("CUS-00001", store.LastRequest.Customer.Code);

        // Verify audit events
        var auditEvents = store.LastRequest.AuditEvents;
        Assert.Equal(2, auditEvents.Count);

        var customerAudit = auditEvents.First(a => a.Action == "customer.created");
        Assert.Equal("Customer", customerAudit.ResourceType);
        Assert.Equal(store.LastRequest.Customer.Id.ToString(), customerAudit.ResourceId);
        Assert.Equal("trace-1", customerAudit.TraceId);
        Assert.Equal("{\"changedFields\":[\"customerType\",\"displayNameTh\",\"displayNameEn\",\"preferredLocale\",\"leadSource\",\"leadSourceNote\",\"primaryContact\"]}", customerAudit.ChangesJson);


        var contactAudit = auditEvents.First(a => a.Action == "contact.created");
        Assert.Equal("CustomerContact", contactAudit.ResourceType);
        Assert.Equal("trace-1", contactAudit.TraceId);
        Assert.Equal("{\"changedFields\":[\"name\",\"roleTitle\",\"phone\",\"email\",\"lineId\",\"preferredChannel\"]}", contactAudit.ChangesJson);

        // Verify hashes
        Assert.False(string.IsNullOrWhiteSpace(store.LastRequest.IdempotencyKeyHash));
        Assert.False(string.IsNullOrWhiteSpace(store.LastRequest.PayloadHash));
        Assert.NotEqual("key-1234567890123456", store.LastRequest.IdempotencyKeyHash);
    }

    [Fact]
    public async Task Handle_WhenInvalidLeadSource_ReturnsCustomerFieldRequired()
    {
        var accessResolver = new FakeRequestAccessResolver();
        accessResolver.GrantedPermissions.Add("customers.create");
        accessResolver.GrantedPermissions.Add("customer-contacts.manage");
        var store = new FakeCustomerCreationStore();
        var handler = new CreateCustomerHandler(accessResolver, store, new FakeClock(), new FakeFileStore(), new FakeDocumentNumberGenerator());

        var command = new CreateCustomerCommand(
            "uid-1", Guid.NewGuid(), "key-1234567890123456",
            "organization", "บริษัท ตัวอย่าง จำกัด", null, "th",
            new CreatePrimaryContact("คุณตัวอย่าง", null, "0812345678", null, "phone"),
            "trace-1",
            LeadSource: "invalid_lead_source");

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal("CUSTOMER_FIELD_REQUIRED", result.Error.Code);
    }

    [Fact]
    public async Task Handle_WhenLineIdExceeds100Chars_ReturnsContactFieldRequired()
    {
        var accessResolver = new FakeRequestAccessResolver();
        accessResolver.GrantedPermissions.Add("customers.create");
        accessResolver.GrantedPermissions.Add("customer-contacts.manage");
        var store = new FakeCustomerCreationStore();
        var handler = new CreateCustomerHandler(accessResolver, store, new FakeClock(), new FakeFileStore(), new FakeDocumentNumberGenerator());

        var command = new CreateCustomerCommand(
            "uid-1", Guid.NewGuid(), "key-1234567890123456",
            "organization", "บริษัท ตัวอย่าง จำกัด", null, "th",
            new CreatePrimaryContact("คุณตัวอย่าง", null, "0812345678", null, "phone", LineId: new string('x', 101)),
            "trace-1");

        var result = await handler.Handle(command);

        Assert.True(result.IsFailure);
        Assert.Equal("CONTACT_FIELD_REQUIRED", result.Error.Code);
    }
}
