using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Attachments;
using TanErp.Api.Contracts.Files;
using TanErp.Api.Contracts.Projects;
using TanErp.Api.Contracts.Service;
using TanErp.Api.ErrorHandling;
using TanErp.Domain.Attachments;
using TanErp.Domain.Commercial;
using TanErp.Domain.Files;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Estimates;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class AttachmentEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private static readonly Guid OrgId = TestOnlyDataSeeder.TestOrgId;
    private static readonly Guid BranchId = TestOnlyDataSeeder.TestBranchId;
    private static readonly Guid UserId = TestOnlyDataSeeder.TestUserId;
    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;
    private const string UidB = TestOnlyDataSeeder.TestFirebaseUidB;
    private const string UidNoPerm = "uid-attachments-no-perm";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f6d01");
    private const string Owner = "installation-job";
    private const string ConsentVersion = "handover-2026-10-v1";

    private static readonly DateOnly Start = new(2026, 11, 1);
    private static readonly DateOnly End = new(2027, 1, 31);

    private static readonly byte[] JpegBytes =
    [
        0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00
    ];

    private static readonly byte[] PngBytes =
    [
        0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52
    ];

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(idToken switch
            {
                "token-org-a" => UidA,
                "token-org-b" => UidB,
                "token-no-perm" => UidNoPerm,
                _ => null
            });
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Database"] = _postgres.GetConnectionString(),
                    ["Storage:BasePath"] = Path.Combine(Path.GetTempPath(), $"tan-erp-attach-{Guid.NewGuid():N}"),
                    ["SeedTestData"] = "true"
                });
            });
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true, seedItemCatalogDemoData: true);

        var noPermUser = new TanErp.Domain.IdentityAccess.User(Guid.NewGuid(), UidNoPerm, "noperm-attach@example.com", "No Perm", true);
        db.Users.Add(noPermUser);
        db.Memberships.Add(new TanErp.Domain.Organization.Membership(MembershipNoPermId, OrgId, BranchId, noPermUser.Id, isActive: true));
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    // ----- HTTP helpers ----------------------------------------------------------------------------

    private static HttpRequestMessage Request(HttpMethod method, string url, string token = "token-org-a", string? key = null, Guid? ifMatch = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var membershipId = token switch
        {
            "token-org-b" => TestOnlyDataSeeder.TestMembershipBId,
            "token-no-perm" => MembershipNoPermId,
            _ => TestOnlyDataSeeder.TestMembershipId
        };
        request.Headers.Add("X-Membership-Id", membershipId.ToString());
        if (key is not null) request.Headers.Add("Idempotency-Key", key);
        if (ifMatch.HasValue) request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        return request;
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body = null, string token = "token-org-a", string? key = null, Guid? ifMatch = null)
    {
        var request = Request(method, url, token, key, ifMatch);
        if (body is not null) request.Content = JsonContent.Create(body);
        return await _client.SendAsync(request);
    }

    private static string Key() => Guid.NewGuid().ToString("N");

    private static async Task<string> ErrorCodeAsync(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<ApiProblemDetails>())!.Code;

    private static async Task<T> Ok<T>(HttpResponseMessage response, HttpStatusCode expected = HttpStatusCode.OK)
    {
        Assert.True(response.StatusCode == expected, $"Expected {expected} but got {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static string AttachmentsUrl(Guid ownerId, string ownerType = Owner) => $"/api/v1/attachment-owners/{ownerType}/{ownerId}/attachments";
    private static string SignaturesUrl(Guid ownerId, string ownerType = Owner) => $"/api/v1/attachment-owners/{ownerType}/{ownerId}/signatures";

    private async Task<AttachmentListResponse> ListAsync(Guid ownerId, string token = "token-org-a") =>
        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Get, AttachmentsUrl(ownerId), token: token));

    /// <summary>Uploads one file through the Files module bound to a parent, as the browser does on submit.</summary>
    private async Task<Guid> UploadAsync(string parentType, Guid parentId, string filename, string mediaType, byte[] content, string token = "token-org-a")
    {
        var session = await Ok<CreateUploadSessionResponse>(await SendAsync(
            HttpMethod.Post, "/api/v1/files/upload-sessions",
            new CreateUploadSessionRequest(parentType, parentId, null, new List<FileSlotRequest> { new(filename, mediaType, content.Length) }),
            token: token, key: Key()), HttpStatusCode.Created);

        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        form.Add(fileContent, session.Slots[0].SlotId.ToString(), filename);

        var complete = Request(HttpMethod.Post, $"/api/v1/files/upload-sessions/{session.SessionId}/complete", token);
        complete.Content = form;
        var completed = await Ok<CompleteUploadSessionResponse>(await _client.SendAsync(complete));
        return completed.Files[0].FileId;
    }

    private Task<Guid> UploadJpegAsync(Guid ownerId, string filename = "evidence.jpg", string token = "token-org-a") =>
        UploadAsync(Owner, ownerId, filename, "image/jpeg", JpegBytes.Concat(Guid.NewGuid().ToByteArray()).ToArray(), token);

    private Task<Guid> UploadPngAsync(Guid ownerId, byte[]? content = null) =>
        UploadAsync(Owner, ownerId, "signature.png", "image/png", content ?? PngBytes.Concat(Guid.NewGuid().ToByteArray()).ToArray());

    // ----- scenario seeding (same flow as ServiceEndpointsTests) -----------------------------------

    private sealed record Seeded(Guid QuotationId, Guid QuotationVersion, Guid OpportunityId);

    private async Task<Seeded> SeedAcceptedQuotationAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTimeOffset.UtcNow;

        var customer = Customer.CreateDraft(
            Guid.NewGuid(), OrgId, UserId, CustomerType.Person, "คุณลูกค้า โครงการ TEST_ONLY", null, "th",
            new PrimaryContactInput("คุณสมชาย", null, "0811111111", null, "phone"), now);
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);

        var site = Site.CreateActive(
            Guid.NewGuid(), OrgId, customer.Id, UserId, "บ้านพักอาศัย",
            new SiteAddressInput("123 ถนนสุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH"), 13.7m, 100.5m, null, now);
        db.Sites.Add(site);

        var opp = Opportunity.CreateDraft(
            Guid.NewGuid(), OrgId, BranchId, customer.Id, site.Id, UserId, UserId,
            "งานบิลท์อินห้องนอน", "ขอบเขตงาน", new[] { "built-in" }, null, 350000m, "THB",
            new DateOnly(2026, 12, 31), now.AddDays(2), "นัดเข้าวัดพื้นที่", now);
        opp.Qualify(opp.RowVersion);
        opp.EnterSurveying(opp.RowVersion, site.Id);
        opp.EnterEstimating(opp.RowVersion);
        opp.EnterProposed(opp.RowVersion);
        opp.MarkWon(opp.RowVersion);
        db.Opportunities.Add(opp);

        var estimate = Estimate.CreateDraft(
            Guid.NewGuid(), OrgId, BranchId, customer.Id, opp.Id, $"EST-T-{Guid.NewGuid():N}"[..14],
            siteSurveyRevisionId: null, siteSurveySnapshotHash: null);
        db.Estimates.Add(estimate);

        var quotation = new Quotation(
            Guid.NewGuid(), OrgId, BranchId, customer.Id, opp.Id, estimate.Id, estimate.CurrentRevision!.Id,
            $"QT-T-{Guid.NewGuid():N}"[..14], 125000.50m, "snapshot-hash-abc", now);
        quotation.Accept(now);
        db.Quotations.Add(quotation);

        await db.SaveChangesAsync();
        return new Seeded(quotation.Id, quotation.RowVersion, opp.Id);
    }

    private async Task<Guid> CreateActiveProjectAsync()
    {
        var seeded = await SeedAcceptedQuotationAsync();
        var created = await Ok<ProjectResponse>(await SendAsync(HttpMethod.Post, "/api/v1/projects",
            new CreateProjectFromHandoverRequest(seeded.QuotationId, seeded.QuotationVersion, UserId, null, null), key: Key()), HttpStatusCode.Created);

        var control = await Ok<ProjectControlResponse>(await SendAsync(HttpMethod.Get, $"/api/v1/projects/{created.Id}/control"));
        control = await Ok<ProjectControlResponse>(await SendAsync(HttpMethod.Put, $"/api/v1/projects/{created.Id}/plan", new SetProjectPlanRequest(Start, End), ifMatch: control.RowVersion));
        control = await Ok<ProjectControlResponse>(await SendAsync(HttpMethod.Put, $"/api/v1/projects/{created.Id}/budget",
            new ReplaceProjectBudgetRequest(new List<ProjectBudgetLineRequest> { new("material", "หมวด 1", 100000m) }), ifMatch: control.RowVersion));
        await Ok<ProjectControlResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/projects/{created.Id}/transitions", new TransitionProjectRequest("active", null), ifMatch: control.RowVersion));
        return created.Id;
    }

    private async Task<InstallationResponse> CreateInstallationAsync(Guid projectId) =>
        await Ok<InstallationResponse>(await SendAsync(HttpMethod.Post, "/api/v1/installations",
            new InstallationRequest(projectId, Start, Start.AddDays(5), "ทีมติดตั้ง A", "ติดตั้งห้องนอน",
                new List<ChecklistItemRequest> { new("ตรวจวัดพื้นที่", true), new("ทำความสะอาดหน้างาน", true), new("ถ่ายรูปงาน", false) }), key: Key()), HttpStatusCode.Created);

    private async Task<InstallationResponse> StepAsync(InstallationResponse job, string path, object? body = null) =>
        await Ok<InstallationResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/installations/{job.Id}/{path}", body, ifMatch: job.RowVersion));

    private async Task<InstallationResponse> ReadyJobAsync()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        job = await StepAsync(job, "start");
        foreach (var item in job.Checklist.Where(c => c.Required))
        {
            job = await Ok<InstallationResponse>(await SendAsync(HttpMethod.Put, $"/api/v1/installations/{job.Id}/checklist/{item.Id}", new ChecklistDoneRequest(true), ifMatch: job.RowVersion));
        }

        return await StepAsync(job, "ready");
    }

    // ----- tests -----------------------------------------------------------------------------------

    [Fact]
    public async Task Attach_List_ServeContent_AndUnlink_RoundTrip()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var fileId = await UploadJpegAsync(job.Id, "ห้องนอน.jpg");

        var attached = await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id),
            new AttachFilesRequest("evidence", new[] { fileId }), key: Key()), HttpStatusCode.Created);

        var link = Assert.Single(attached.Items);
        Assert.Equal((Owner, job.Id, fileId, "evidence", "ห้องนอน.jpg"), (link.OwnerType, link.OwnerId, link.FileId, link.Purpose, link.Filename));
        Assert.Equal($"/api/v1/files/{fileId}/content", link.ServingUrl);
        Assert.False(string.IsNullOrWhiteSpace(link.CreatedBy.DisplayName));
        Assert.Single((await ListAsync(job.Id)).Items);

        var content = await SendAsync(HttpMethod.Get, $"/api/v1/files/{fileId}/content");
        Assert.Equal(HttpStatusCode.OK, content.StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await SendAsync(HttpMethod.Delete, $"{AttachmentsUrl(job.Id)}/{link.Id}")).StatusCode);
        Assert.Empty((await ListAsync(job.Id)).Items);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Delete, $"{AttachmentsUrl(job.Id)}/{link.Id}")).StatusCode);

        // A removed link can be attached again (the unique index only covers active links).
        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id),
            new AttachFilesRequest("evidence", new[] { fileId }), key: Key()), HttpStatusCode.Created);
    }

    [Fact]
    public async Task Attach_ReplayWithSameKeyDoesNotDuplicate_AndADifferentPayloadIsRejected()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var first = await UploadJpegAsync(job.Id);
        var second = await UploadJpegAsync(job.Id);
        var key = Key();

        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { first }), key: key), HttpStatusCode.Created);
        var replay = await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { first }), key: key), HttpStatusCode.Created);

        Assert.Single(replay.Items);
        Assert.Single((await ListAsync(job.Id)).Items);
        Assert.Equal("IDEMPOTENCY_KEY_REUSED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { second }), key: key)));
        Assert.Equal(HttpStatusCode.BadRequest, (await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { second }))).StatusCode);
    }

    [Fact]
    public async Task Attach_FailedBatch_LinksNothing_SoUploadedFilesStayOrphanedAndUnlinked()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var linked = await UploadJpegAsync(job.Id);
        var orphan = await UploadJpegAsync(job.Id);
        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { linked }), key: Key()), HttpStatusCode.Created);

        // The second file is already linked for the same purpose: the whole batch fails and the new file is not linked.
        Assert.Equal("ATTACHMENT_DUPLICATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { orphan, linked }), key: Key())));
        // An unknown file id fails before any link is created.
        Assert.Equal("ATTACHMENT_FILE_NOT_READY", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { orphan, Guid.NewGuid() }), key: Key())));

        var items = (await ListAsync(job.Id)).Items;
        Assert.Equal(new[] { linked }, items.Select(i => i.FileId).ToArray());

        // The orphan can still be linked on a later, successful submit.
        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { orphan }), key: Key()), HttpStatusCode.Created);
        Assert.Equal(2, (await ListAsync(job.Id)).Items.Count);
    }

    [Fact]
    public async Task Attach_RejectsFilesUploadedForAnotherParent_AndInvalidInput()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var seeded = await SeedAcceptedQuotationAsync();
        var foreignParentFile = await UploadAsync("opportunity", seeded.OpportunityId, "other.jpg", "image/jpeg", JpegBytes.Concat(Guid.NewGuid().ToByteArray()).ToArray());

        Assert.Equal("ATTACHMENT_FILE_NOT_READY", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { foreignParentFile }), key: Key())));
        Assert.Equal("ATTACHMENT_PURPOSE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("misc", new[] { Guid.NewGuid() }), key: Key())));
        Assert.Equal("ATTACHMENT_FIELD_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", Array.Empty<Guid>()), key: Key())));
        Assert.Empty((await ListAsync(job.Id)).Items);
    }

    [Fact]
    public async Task OtherOrganization_GetsNotFound_ForEveryOperation_AndCannotReadTheFile()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var fileId = await UploadJpegAsync(job.Id);
        var link = (await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }), key: Key()), HttpStatusCode.Created)).Items.Single();

        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Get, AttachmentsUrl(job.Id), token: "token-org-b")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }), token: "token-org-b", key: Key())).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Delete, $"{AttachmentsUrl(job.Id)}/{link.Id}", token: "token-org-b")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Get, SignaturesUrl(job.Id), token: "token-org-b")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Get, $"/api/v1/files/{fileId}/content", token: "token-org-b")).StatusCode);
    }

    [Fact]
    public async Task UnregisteredOwnerType_IsRejected_AndOwnerTypeMatchingIsCaseInsensitive()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());

        Assert.Equal("ATTACHMENT_OWNER_TYPE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Get, AttachmentsUrl(job.Id, "customer"))));
        Assert.Equal("ATTACHMENT_OWNER_TYPE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id, "installation_job"), new AttachFilesRequest("evidence", new[] { Guid.NewGuid() }), key: Key())));
        Assert.Equal(HttpStatusCode.OK, (await SendAsync(HttpMethod.Get, AttachmentsUrl(job.Id, "INSTALLATION-JOB"))).StatusCode);
    }

    [Fact]
    public async Task MissingPermission_IsForbidden_ForReadAndWrite()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());

        Assert.Equal("PERMISSION_DENIED", await ErrorCodeAsync(await SendAsync(HttpMethod.Get, AttachmentsUrl(job.Id), token: "token-no-perm")));
        Assert.Equal("PERMISSION_DENIED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { Guid.NewGuid() }), token: "token-no-perm", key: Key())));
        Assert.Equal("PERMISSION_DENIED", await ErrorCodeAsync(await SendAsync(HttpMethod.Get, SignaturesUrl(job.Id), token: "token-no-perm")));
    }

    [Fact]
    public async Task CancelledInstallation_IsLockedForAttachments()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var fileId = await UploadJpegAsync(job.Id);
        await Ok<InstallationResponse>(await SendAsync(HttpMethod.Post, $"/api/v1/installations/{job.Id}/cancel", new { reason = "ยกเลิกเพื่อทดสอบ" }, ifMatch: job.RowVersion));

        Assert.Equal("ATTACHMENT_OWNER_LOCKED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }), key: Key())));
        Assert.Equal(HttpStatusCode.OK, (await SendAsync(HttpMethod.Get, AttachmentsUrl(job.Id))).StatusCode);
    }

    [Fact]
    public async Task Signature_Capture_StoresHashOfTheImageBytes_AndRejectsInvalidSubmissions()
    {
        var job = await ReadyJobAsync();
        var pngBytes = PngBytes.Concat(Guid.NewGuid().ToByteArray()).ToArray();
        var imageId = await UploadPngAsync(job.Id, pngBytes);
        var expectedHash = Convert.ToHexString(SHA256.HashData(pngBytes)).ToLowerInvariant();
        var request = new CaptureSignatureRequest("handover", "คุณสมชาย ใจดี", "เจ้าของบ้าน", imageId, true, ConsentVersion);

        var before = DateTimeOffset.UtcNow.AddMinutes(-1);
        var capture = await Ok<SignatureCaptureResponse>(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request, key: Key()), HttpStatusCode.Created);

        Assert.Equal(("handover", "คุณสมชาย ใจดี", "เจ้าของบ้าน", ConsentVersion, expectedHash, imageId), (capture.Purpose, capture.SignerName, capture.SignerRole, capture.ConsentTextVersion, capture.ContentHash, capture.ImageFileId));
        Assert.InRange(capture.SignedAtUtc, before, DateTimeOffset.UtcNow.AddMinutes(1));
        Assert.Single((await Ok<SignatureCaptureListResponse>(await SendAsync(HttpMethod.Get, SignaturesUrl(job.Id)))).Items);

        // The same image cannot be used twice, consent must match, and only verified PNG files qualify.
        Assert.Equal("ATTACHMENT_DUPLICATE", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request, key: Key())));
        var second = await UploadPngAsync(job.Id);
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request with { ImageFileId = second, ConsentAccepted = false }, key: Key())));
        Assert.Equal("SIGNATURE_CONSENT_REQUIRED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request with { ImageFileId = second, ConsentTextVersion = "old-v0" }, key: Key())));
        Assert.Equal("SIGNATURE_SUBMISSION_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request with { ImageFileId = second, SignerName = "ก" }, key: Key())));
        var jpeg = await UploadJpegAsync(job.Id);
        Assert.Equal("SIGNATURE_IMAGE_INVALID", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), request with { ImageFileId = jpeg }, key: Key())));
        Assert.Single((await Ok<SignatureCaptureListResponse>(await SendAsync(HttpMethod.Get, SignaturesUrl(job.Id)))).Items);
    }

    [Fact]
    public async Task Signature_IsLockedUntilReadyForHandover_AndStaysOutOfTheAuditTrail()
    {
        var inProgress = await StepAsync(await CreateInstallationAsync(await CreateActiveProjectAsync()), "start");
        var early = await UploadPngAsync(inProgress.Id);
        Assert.Equal("ATTACHMENT_OWNER_LOCKED", await ErrorCodeAsync(await SendAsync(HttpMethod.Post, SignaturesUrl(inProgress.Id),
            new CaptureSignatureRequest("handover", "คุณสมชาย ใจดี", null, early, true, ConsentVersion), key: Key())));

        var ready = await ReadyJobAsync();
        var imageId = await UploadPngAsync(ready.Id);
        await Ok<SignatureCaptureResponse>(await SendAsync(HttpMethod.Post, SignaturesUrl(ready.Id),
            new CaptureSignatureRequest("handover", "ผู้ลงนามเฉพาะกิจ ทดสอบ", "ผู้รับมอบ", imageId, true, ConsentVersion), key: Key()), HttpStatusCode.Created);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var audits = await db.AuditEvents.AsNoTracking().Where(a => a.Action == "signature.captured" && a.ResourceId == ready.Id.ToString()).ToListAsync();
        var audit = Assert.Single(audits);
        Assert.DoesNotContain("ผู้ลงนามเฉพาะกิจ", audit.ChangesJson);
        Assert.DoesNotContain("ผู้รับมอบ", audit.ChangesJson);
        Assert.Contains("contentHash", audit.ChangesJson);
    }

    [Fact]
    public async Task Unlink_WithALinkIdOfAnotherOwnerInTheSameOrganization_IsNotFound()
    {
        var projectId = await CreateActiveProjectAsync();
        var jobA = await CreateInstallationAsync(projectId);
        var jobB = await CreateInstallationAsync(projectId);
        var fileId = await UploadJpegAsync(jobA.Id);
        var link = (await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(jobA.Id), new AttachFilesRequest("evidence", new[] { fileId }), key: Key()), HttpStatusCode.Created)).Items.Single();

        Assert.Equal(HttpStatusCode.NotFound, (await SendAsync(HttpMethod.Delete, $"{AttachmentsUrl(jobB.Id)}/{link.Id}")).StatusCode);
        Assert.Single((await ListAsync(jobA.Id)).Items);
    }

    [Fact]
    public async Task Attach_FileUploadedForJobA_CannotBeAttachedToJobB()
    {
        var projectId = await CreateActiveProjectAsync();
        var jobA = await CreateInstallationAsync(projectId);
        var jobB = await CreateInstallationAsync(projectId);
        var fileId = await UploadJpegAsync(jobA.Id);

        var response = await SendAsync(HttpMethod.Post, AttachmentsUrl(jobB.Id), new AttachFilesRequest("evidence", new[] { fileId }), key: Key());
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("ATTACHMENT_FILE_NOT_READY", await ErrorCodeAsync(response));
        Assert.Empty((await ListAsync(jobB.Id)).Items);
    }

    [Fact]
    public async Task Attach_SameFileAndPurposeTwice_IsConflict()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var fileId = await UploadJpegAsync(job.Id);
        await Ok<AttachmentListResponse>(await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }), key: Key()), HttpStatusCode.Created);

        var response = await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }), key: Key());
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("ATTACHMENT_DUPLICATE", await ErrorCodeAsync(response));
    }

    [Fact]
    public async Task Attach_ParallelRequestsForTheSameFileAndPurpose_YieldOneCreatedAndOneConflict()
    {
        // Smaller proof of the per-owner lock + unique index (seeding 49 verified files is too slow for an integration test).
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var fileId = await UploadJpegAsync(job.Id);

        var responses = await Task.WhenAll(
            SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }), key: Key()),
            SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }), key: Key()));

        Assert.Equal(new[] { HttpStatusCode.Created, HttpStatusCode.Conflict }, responses.Select(r => r.StatusCode).OrderBy(s => (int)s).ToArray());
        Assert.Equal("ATTACHMENT_DUPLICATE", await ErrorCodeAsync(responses.Single(r => r.StatusCode == HttpStatusCode.Conflict)));
        Assert.Single((await ListAsync(job.Id)).Items);
    }

    [Fact]
    public async Task Attach_ParallelRequestsAtFortyNineLinks_YieldOneCreatedAndOneLimitConflict()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var fileA = await UploadJpegAsync(job.Id, "race-a.jpg");
        var fileB = await UploadJpegAsync(job.Id, "race-b.jpg");

        // Seed 49 active links directly (uploading 49 files through the API is too slow). The limit check only counts active links.
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var now = DateTimeOffset.UtcNow;
            for (var i = 0; i < AttachmentLinkLimit - 1; i++)
            {
                var file = new UploadedFile(Guid.NewGuid(), OrgId, $"seed/{Guid.NewGuid():N}.jpg", $"seed-{i}.jpg", "image/jpeg", 10, Guid.NewGuid().ToString(), UserId, now, new string('a', 64));
                db.UploadedFiles.Add(file);
                db.AttachmentLinks.Add(new AttachmentLink(Guid.NewGuid(), OrgId, Owner, job.Id, OrgId, file.Id, "evidence", UserId, now));
            }

            await db.SaveChangesAsync();
        }

        var responses = await Task.WhenAll(
            SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileA }), key: Key()),
            SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileB }), key: Key()));

        Assert.Equal(new[] { HttpStatusCode.Created, HttpStatusCode.Conflict }, responses.Select(r => r.StatusCode).OrderBy(s => (int)s).ToArray());
        Assert.Equal("ATTACHMENT_LIMIT_EXCEEDED", await ErrorCodeAsync(responses.Single(r => r.StatusCode == HttpStatusCode.Conflict)));
        Assert.Equal(AttachmentLinkLimit, (await ListAsync(job.Id)).Items.Count);
    }

    private const int AttachmentLinkLimit = AttachmentLink.MaxActiveLinksPerOwner;

    [Fact]
    public async Task MissingIdempotencyKey_IsRejectedAsClientError_NotServerError()
    {
        var job = await CreateInstallationAsync(await CreateActiveProjectAsync());
        var fileId = await UploadJpegAsync(job.Id);

        var attach = await SendAsync(HttpMethod.Post, AttachmentsUrl(job.Id), new AttachFilesRequest("evidence", new[] { fileId }));
        var sign = await SendAsync(HttpMethod.Post, SignaturesUrl(job.Id), new CaptureSignatureRequest("handover", "คุณสมชาย ใจดี", null, fileId, true, ConsentVersion));

        Assert.InRange((int)attach.StatusCode, 400, 499);
        Assert.InRange((int)sign.StatusCode, 400, 499);
    }
}
