using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TanErp.Api.Contracts.Commercial;
using TanErp.Application.Commercial;
using TanErp.Application.Commercial.Documents;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Files;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Infrastructure.Persistence;
using Xunit;

namespace TanErp.IntegrationTests.Api;

// G-04: the server-side quotation PDF. Shares the harness (database, tokens, helpers) of the main file.
public partial class EstimateEndpointsTests
{
    private sealed class CapturingEmailSender : IEmailSender
    {
        public bool IsEnabled => true;
        public List<EmailMessage> Sent { get; } = new();
        public Task SendAsync(EmailMessage message, CancellationToken ct = default)
        {
            Sent.Add(message);
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingQuotationRenderer : IQuotationDocumentRenderer
    {
        public string Version => "test-throwing/v0";
        public byte[] Render(QuotationPdfModel model) => throw new InvalidOperationException("TEST_ONLY renderer failure");
    }

    private static string Sha256Of(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    // G-04 tests of later tasks are inserted below this line.
    [Fact]
    public async Task ExternalAcceptance_ServesTheStoredPdf_AndRecordsTheHashTheCustomerAttested()
    {
        var (_, _, quotation) = await IssueQuotedEstimateAsync("pdf-accept");
        var (linkId, token) = await CreateAcceptanceLinkAsync(quotation.QuotationId);
        var view = (await (await PublicGetAsync(token)).Content.ReadFromJsonAsync<TanErp.Api.Contracts.Commercial.PublicAcceptanceViewResponse>())!;

        var pdf = await _client.GetAsync($"/api/public/v1/quotation-acceptance/{token}/document");
        Assert.Equal(HttpStatusCode.OK, pdf.StatusCode);
        Assert.Equal("no-store", pdf.Headers.CacheControl?.ToString());
        Assert.Equal(view.Pdf!.Sha256, Sha256Of(await pdf.Content.ReadAsByteArrayAsync()));

        // A wrong hash, or no hash while a PDF exists, is a conflict and accepts nothing.
        Assert.Equal("ACCEPTANCE_CONFLICT", await ProblemCodeAsync(await PublicAcceptAsync(token, Submission(documentSha256: new string('0', 64)))));
        Assert.Equal("ACCEPTANCE_CONFLICT", await ProblemCodeAsync(await PublicAcceptAsync(token, Submission())));
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal("issued", (await db.Quotations.AsNoTracking().SingleAsync(q => q.Id == quotation.QuotationId)).Status);

        Assert.Equal(HttpStatusCode.OK, (await PublicAcceptAsync(token, Submission(documentSha256: view.Pdf.Sha256))).StatusCode);
        Assert.Equal(view.Pdf.Sha256, (await db.QuotationAcceptanceEvidences.AsNoTracking().SingleAsync(e => e.LinkId == linkId)).DocumentSha256);

        // The accepted link stays viewable, and an unknown token learns nothing.
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync($"/api/public/v1/quotation-acceptance/{token}/document")).StatusCode);
        var unknown = await _client.GetAsync($"/api/public/v1/quotation-acceptance/{new string('x', 43)}/document");
        Assert.Equal(HttpStatusCode.NotFound, unknown.StatusCode);
        Assert.Equal("ACCEPTANCE_LINK_UNAVAILABLE", await ProblemCodeAsync(unknown));
    }

    [Fact]
    public async Task ExternalAcceptance_OfALegacyQuotationWithoutPdf_StillWorks_WithNoHashRecorded()
    {
        var (_, _, quotation) = await IssueQuotedEstimateAsync("pdf-accept-legacy");
        await SimulateLegacyQuotationAsync(quotation.QuotationId);
        var (linkId, token) = await CreateAcceptanceLinkAsync(quotation.QuotationId);

        Assert.Null((await (await PublicGetAsync(token)).Content.ReadFromJsonAsync<TanErp.Api.Contracts.Commercial.PublicAcceptanceViewResponse>())!.Pdf);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/public/v1/quotation-acceptance/{token}/document")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await PublicAcceptAsync(token, Submission())).StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Null((await db.QuotationAcceptanceEvidences.AsNoTracking().SingleAsync(e => e.LinkId == linkId)).DocumentSha256);
    }

    private Task<HttpResponseMessage> GetQuotationPdfAsync(Guid quotationId, string token = "token-org-a", Guid? membership = null) =>
        _client.SendAsync(CreateAuthenticatedRequest(HttpMethod.Get, $"/api/v1/quotations/{quotationId}/document", token, membership ?? MembershipAId));

    // Simulates a quotation issued before G-04: it has no stored document.
    private async Task SimulateLegacyQuotationAsync(Guid quotationId)
    {
        using var scope = _factory.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.ExecuteSqlInterpolatedAsync($"DELETE FROM commercial.quotation_documents WHERE quotation_id = {quotationId}");
    }

    // A user whose quotations.read is scoped to ANOTHER branch of the same organization.
    private static async Task<Guid> SeedOtherBranchReaderAsync(AppDbContext db)
    {
        var (branchId, userId, membershipId, roleId) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        db.Branches.Add(new Branch(branchId, OrgAId, $"QR-{branchId:N}"[..12], "Other Branch"));
        db.Users.Add(new User(userId, "test-only-quotation-reader", "Quotation Reader", "quotation-reader@example.test"));
        db.Memberships.Add(new Membership(membershipId, OrgAId, branchId, userId));
        db.Roles.Add(new Role(roleId, OrgAId, "TEST ONLY OTHER BRANCH", "TEST_ONLY other branch"));
        db.MembershipRoles.Add(new MembershipRole(membershipId, roleId, OrgAId));
        var read = await db.Permissions.SingleAsync(row => row.Key == "quotations.read");
        // quotations.read is only honoured at organization scope (RequestAccessResolver allows branch-scoped grants for estimates.* only);
        // the membership itself is bound to the other branch, so the reader passes the permission check but not the branch check.
        db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), roleId, OrgAId, read.Id, PermissionScope.Organization, OrgAId));
        await db.SaveChangesAsync();
        return membershipId;
    }

    [Fact]
    public async Task QuotationDocument_IsServedFromStorage_AndIsNotChangedByLaterOrganizationProfileChanges()
    {
        var (estimateId, _, quotation) = await IssueQuotedEstimateAsync("pdf-download");
        var summary = (await HistoryAsync(estimateId)).Items.Single().Document!;

        var response = await GetQuotationPdfAsync(quotation.QuotationId);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal($"\"{summary.Sha256}\"", response.Headers.ETag?.Tag);
        var before = await response.Content.ReadAsByteArrayAsync();
        Assert.Equal(summary.Sha256, Sha256Of(before));

        // Master data changes after issue: the stored file stays exactly as issued because it is never re-rendered.
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            (await db.Organizations.SingleAsync(o => o.Id == OrgAId)).UpdateProfile("ชื่อองค์กรใหม่หลังออกเอกสาร", "Renamed after issue", null, "ที่อยู่ใหม่", null, null);
            await db.SaveChangesAsync();
        }

        Assert.Equal(before, await (await GetQuotationPdfAsync(quotation.QuotationId)).Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task QuotationDocument_OutsideTheCallersOrganizationOrBranch_IsNotFound_AndALegacyQuotationHasNone()
    {
        var (_, _, quotation) = await IssueQuotedEstimateAsync("pdf-scope");
        Guid otherBranchMembership;
        using (var scope = _factory.Services.CreateScope())
            otherBranchMembership = await SeedOtherBranchReaderAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());

        Assert.Equal(HttpStatusCode.NotFound, (await GetQuotationPdfAsync(quotation.QuotationId, "token-org-b", TestOnlyDataSeeder.TestMembershipBId)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await GetQuotationPdfAsync(quotation.QuotationId, "token-quotation-reader", otherBranchMembership)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await GetQuotationPdfAsync(Guid.NewGuid())).StatusCode);

        // A quotation issued before G-04 has no document: a stable code, and the history says so.
        await SimulateLegacyQuotationAsync(quotation.QuotationId);
        var missing = await GetQuotationPdfAsync(quotation.QuotationId);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        Assert.Equal("QUOTATION_DOCUMENT_NOT_AVAILABLE", await ProblemCodeAsync(missing));
    }

    [Fact]
    public async Task IssueQuotation_StoresOneDocumentRow_WhoseHashMatchesTheStoredBytes()
    {
        var (estimateId, _, quotation) = await IssueQuotedEstimateAsync("pdf-issue");
        var item = (await HistoryAsync(estimateId)).Items.Single();
        Assert.NotNull(item.Document);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var document = await db.QuotationDocuments.AsNoTracking().SingleAsync(d => d.QuotationId == quotation.QuotationId);
        var file = await db.UploadedFiles.AsNoTracking().SingleAsync(f => f.Id == document.FileId);
        Assert.Equal(("application/pdf", document.Sha256, document.ByteCount), (file.MediaType, file.ContentSha256, file.FileSizeBytes));
        Assert.Equal(item.Document.Sha256, document.Sha256);

        await using var stream = (await scope.ServiceProvider.GetRequiredService<IFileStorageProvider>().OpenReadStreamAsync(file.StoragePath))!;
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);
        Assert.StartsWith("%PDF-", Encoding.ASCII.GetString(buffer.ToArray(), 0, 5));
        Assert.Equal(document.Sha256, Sha256Of(buffer.ToArray()));

        var audit = await db.AuditEvents.AsNoTracking().SingleAsync(a => a.Action == "quotations.document-rendered" && a.ResourceId == quotation.QuotationId.ToString());
        Assert.Contains(document.Sha256, audit.ChangesJson);
        Assert.DoesNotContain("ลูกค้า", audit.ChangesJson);
    }

    [Fact]
    public async Task IssueQuotation_WhenRenderingFails_RollsBackTheWholeIssue()
    {
        using var failing = _factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<IQuotationDocumentRenderer>();
            services.AddSingleton<IQuotationDocumentRenderer, ThrowingQuotationRenderer>();
        }));
        using var failingClient = failing.CreateClient();

        var (estimateId, oppId, _) = await IssueQuotedEstimateAsync("pdf-fail", failingClient, HttpStatusCode.InternalServerError);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(0, await db.Quotations.CountAsync(q => q.EstimateId == estimateId));
        Assert.Equal(0, await db.QuotationDocuments.CountAsync());
        Assert.Equal(0, await db.IdempotencyRecords.CountAsync(r => r.Operation == "quotations.issue"));
        Assert.Equal(OpportunityStage.Estimating, (await db.Opportunities.AsNoTracking().SingleAsync(o => o.Id == oppId)).Stage);
    }

    [Fact]
    public async Task AmendQuotation_RendersTheReplacementOnce_AndKeepsTheOriginalDocumentUntouched()
    {
        var (estimateId, _, original) = await IssueQuotedEstimateAsync("pdf-amend");
        var before = (await HistoryAsync(estimateId)).Items.Single();
        var amended = await _client.SendAsync(LifecycleRequest($"/api/v1/quotations/{original.QuotationId}/amend", before.RowVersion, "ปรับเงื่อนไข", $"pdf-amend-{Guid.NewGuid():N}"));
        Assert.Equal(HttpStatusCode.Created, amended.StatusCode);

        var items = (await amended.Content.ReadFromJsonAsync<TanErp.Api.Contracts.Commercial.QuotationHistoryResponse>())!.Items;
        Assert.Equal(before.Document!.Sha256, items.Single(i => i.Id == original.QuotationId).Document!.Sha256);
        Assert.NotEqual(before.Document.Sha256, items.Single(i => i.Id != original.QuotationId).Document!.Sha256);
    }

    [Fact]
    public async Task ExternalAcceptance_OtpFlow_EnforcesRequiredVerificationCooldownLockoutExpiryAndSingleUse()
    {
        var (_, _, quotation) = await IssueQuotedEstimateAsync("otp-accept");

        // 1. Default AcceptanceOtp:Required=false -> /otp returns 503 ACCEPTANCE_OTP_UNAVAILABLE and /accept works without OTP.
        var (_, defaultToken) = await CreateAcceptanceLinkAsync(quotation.QuotationId);
        var defaultView = (await (await PublicGetAsync(defaultToken)).Content.ReadFromJsonAsync<PublicAcceptanceViewResponse>())!;
        Assert.False(defaultView.OtpRequired);
        Assert.Empty(defaultView.OtpChannels);
        var unavailableOtp = await _client.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{defaultToken}/otp", new PublicOtpRequest("email"));
        Assert.Equal(HttpStatusCode.ServiceUnavailable, unavailableOtp.StatusCode);
        Assert.Equal("ACCEPTANCE_OTP_UNAVAILABLE", await ProblemCodeAsync(unavailableOtp));

        // 2. Host with AcceptanceOtp:Required=true and an enabled email sender.
        var emailSender = new CapturingEmailSender();
        using var otpFactory = _factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            services.RemoveAll<AcceptanceOtpOptions>();
            services.AddSingleton(new AcceptanceOtpOptions
            {
                Required = true,
                HmacKey = "0123456789abcdef0123456789abcdef",
                CodeLength = 6,
                ExpiryMinutes = 10,
                MaxAttempts = 5,
                ResendCooldownSeconds = 60,
                MaxSendsPerHour = 3,
                VerificationMinutes = 15,
                Channels = new OtpChannelSwitches { Email = true, Sms = false }
            });
            services.RemoveAll<IEmailSender>();
            services.AddSingleton<IEmailSender>(emailSender);
        }));
        using var otpClient = otpFactory.CreateClient();

        // Creating a link when OTP is required demands a recipient email/phone for an enabled channel.
        var missingEmailReq = CreateAuthenticatedRequest(HttpMethod.Post, $"/api/v1/quotations/{quotation.QuotationId}/acceptance-links", "token-org-a", MembershipAId);
        missingEmailReq.Content = JsonContent.Create(new CreateAcceptanceLinkRequest(7, "คุณสมชาย", RecipientEmail: null));
        var missingEmailRes = await otpClient.SendAsync(missingEmailReq);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, missingEmailRes.StatusCode);
        Assert.Equal("ACCEPTANCE_RECIPIENT_EMAIL_REQUIRED", await ProblemCodeAsync(missingEmailRes));

        async Task<(Guid LinkId, string Token)> CreateOtpLinkAsync(string email)
        {
            var req = CreateAuthenticatedRequest(HttpMethod.Post, $"/api/v1/quotations/{quotation.QuotationId}/acceptance-links", "token-org-a", MembershipAId);
            req.Content = JsonContent.Create(new CreateAcceptanceLinkRequest(7, "คุณสมชาย", RecipientEmail: email));
            var res = await otpClient.SendAsync(req);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
            var body = (await res.Content.ReadFromJsonAsync<CreatedAcceptanceLinkResponse>())!;
            return (body.Link.Id, body.Token);
        }

        // Link 1: Test 5 wrong attempts -> 423 ACCEPTANCE_OTP_LOCKED and cooldown -> 429 ACCEPTANCE_OTP_RATE_LIMITED
        var (lockLinkId, lockToken) = await CreateOtpLinkAsync("lock@example.test");
        var otpView = (await (await PublicGetAsync(lockToken, otpClient)).Content.ReadFromJsonAsync<PublicAcceptanceViewResponse>())!;
        Assert.True(otpView.OtpRequired);
        Assert.Equal(new[] { "email" }, otpView.OtpChannels);

        // Verify before requesting any challenge -> 410 ACCEPTANCE_OTP_EXPIRED
        var verifyBeforeReq = await otpClient.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{lockToken}/otp/verify", new PublicOtpVerifyRequest("000000"));
        Assert.Equal(HttpStatusCode.Gone, verifyBeforeReq.StatusCode);
        Assert.Equal("ACCEPTANCE_OTP_EXPIRED", await ProblemCodeAsync(verifyBeforeReq));

        // Request OTP -> 202 Accepted
        var reqOtp1 = await otpClient.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{lockToken}/otp", new PublicOtpRequest("email"));
        Assert.Equal(HttpStatusCode.Accepted, reqOtp1.StatusCode);

        // Request again inside ResendCooldownSeconds -> 429 ACCEPTANCE_OTP_RATE_LIMITED
        var cooldownRes = await otpClient.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{lockToken}/otp", new PublicOtpRequest("email"));
        Assert.Equal(HttpStatusCode.TooManyRequests, cooldownRes.StatusCode);
        Assert.Equal("ACCEPTANCE_OTP_RATE_LIMITED", await ProblemCodeAsync(cooldownRes));

        // 4 wrong attempts -> 422 ACCEPTANCE_OTP_INVALID, 5th wrong attempt -> 423 ACCEPTANCE_OTP_LOCKED
        for (var attempt = 1; attempt <= 4; attempt++)
        {
            var wrong = await otpClient.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{lockToken}/otp/verify", new PublicOtpVerifyRequest("000000"));
            Assert.Equal(HttpStatusCode.UnprocessableEntity, wrong.StatusCode);
            Assert.Equal("ACCEPTANCE_OTP_INVALID", await ProblemCodeAsync(wrong));
        }

        var fifthWrong = await otpClient.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{lockToken}/otp/verify", new PublicOtpVerifyRequest("000000"));
        Assert.Equal(HttpStatusCode.Locked, fifthWrong.StatusCode);
        Assert.Equal("ACCEPTANCE_OTP_LOCKED", await ProblemCodeAsync(fifthWrong));

        // Locked link refuses both /otp and /accept with 423 ACCEPTANCE_OTP_LOCKED
        var lockedAccept = await PublicAcceptAsync(lockToken, Submission(documentSha256: otpView.Pdf!.Sha256), otpClient);
        Assert.Equal(HttpStatusCode.Locked, lockedAccept.StatusCode);
        Assert.Equal("ACCEPTANCE_OTP_LOCKED", await ProblemCodeAsync(lockedAccept));

        // Link 2: Test missing OTP token on /accept (422 ACCEPTANCE_OTP_REQUIRED), expired challenge (410 ACCEPTANCE_OTP_EXPIRED),
        // valid verify + accept recording otp_channel/otp_verified_at_utc/otp_challenge_id, and single-use challenge consumption.
        emailSender.Sent.Clear();
        var (acceptLinkId, acceptToken) = await CreateOtpLinkAsync("customer@example.test");

        var withoutOtp = await PublicAcceptAsync(acceptToken, new PublicAcceptRequest("คุณสมชาย ใจดี", "กรรมการ", true, "2026-10-v1", null, otpView.Pdf.Sha256, null), otpClient);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, withoutOtp.StatusCode);
        Assert.Equal("ACCEPTANCE_OTP_REQUIRED", await ProblemCodeAsync(withoutOtp));

        // Request OTP, expire it in DB, and verify -> 410 ACCEPTANCE_OTP_EXPIRED
        Assert.Equal(HttpStatusCode.Accepted, (await otpClient.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{acceptToken}/otp", new PublicOtpRequest("email"))).StatusCode);
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var pastNow = DateTimeOffset.UtcNow;
            await db.AcceptanceOtpChallenges
                .Where(c => c.LinkId == acceptLinkId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.CreatedAtUtc, pastNow.AddMinutes(-20))
                    .SetProperty(c => c.ExpiresAtUtc, pastNow.AddMinutes(-10)));
        }

        var expiredVerify = await otpClient.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{acceptToken}/otp/verify", new PublicOtpVerifyRequest("123456"));
        Assert.Equal(HttpStatusCode.Gone, expiredVerify.StatusCode);
        Assert.Equal("ACCEPTANCE_OTP_EXPIRED", await ProblemCodeAsync(expiredVerify));

        // Request fresh OTP, extract 6-digit code from captured email, verify -> 200 OK
        emailSender.Sent.Clear();
        Assert.Equal(HttpStatusCode.Accepted, (await otpClient.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{acceptToken}/otp", new PublicOtpRequest("email"))).StatusCode);
        var sentMail = Assert.Single(emailSender.Sent);
        var codeMatch = Regex.Match(sentMail.TextBody, @"\b(\d{6})\b");
        Assert.True(codeMatch.Success);
        var otpCode = codeMatch.Groups[1].Value;

        var verifyOk = await otpClient.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{acceptToken}/otp/verify", new PublicOtpVerifyRequest(otpCode));
        Assert.Equal(HttpStatusCode.OK, verifyOk.StatusCode);
        var verified = (await verifyOk.Content.ReadFromJsonAsync<PublicOtpVerifyResponse>())!;
        Assert.NotEmpty(verified.VerificationToken);

        // Re-verifying the same challenge after it was already verified -> 410 ACCEPTANCE_OTP_EXPIRED (no active unverified challenge)
        var replayVerify = await otpClient.PostAsJsonAsync($"/api/public/v1/quotation-acceptance/{acceptToken}/otp/verify", new PublicOtpVerifyRequest(otpCode));
        Assert.Equal(HttpStatusCode.Gone, replayVerify.StatusCode);
        Assert.Equal("ACCEPTANCE_OTP_EXPIRED", await ProblemCodeAsync(replayVerify));

        // Accept with verificationToken -> 200 OK and records otp evidence in DB
        var acceptOk = await PublicAcceptAsync(
            acceptToken,
            new PublicAcceptRequest("คุณสมชาย ใจดี", "กรรมการ", true, "2026-10-v1", null, otpView.Pdf.Sha256, verified.VerificationToken),
            otpClient);
        Assert.Equal(HttpStatusCode.OK, acceptOk.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var evidence = await db.QuotationAcceptanceEvidences.AsNoTracking().SingleAsync(e => e.LinkId == acceptLinkId);
            Assert.Equal("email", evidence.OtpChannel);
            Assert.NotNull(evidence.OtpVerifiedAtUtc);
            Assert.NotNull(evidence.OtpChallengeId);
            var challenge = await db.AcceptanceOtpChallenges.AsNoTracking().SingleAsync(c => c.Id == evidence.OtpChallengeId);
            Assert.NotNull(challenge.ConsumedAtUtc);
        }
    }

}
