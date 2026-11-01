using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.IdentityAccess;
using TanErp.Api.Contracts.IdentityAccess.Administration;
using TanErp.Api.Contracts.IdentityAccess.Authentication;
using TanErp.Api.ErrorHandling;
using TanErp.Application.Common.Results;
using TanErp.Application.IdentityAccess.Authentication;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class AuthEndpointsTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private Guid _adminRoleId;

    private sealed class TestTokenVerifier : IFirebaseTokenVerifier
    {
        private readonly SelfHostedTokenVerifier _inner;

        public TestTokenVerifier(SelfHostedTokenVerifier inner)
        {
            _inner = inner;
        }

        public async Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var identity = await VerifyIdentityAsync(idToken, cancellationToken);
            return identity?.Uid;
        }

        public async Task<FirebaseIdentity?> VerifyIdentityAsync(string idToken, CancellationToken cancellationToken = default)
        {
            if (idToken == "admin-token")
            {
                return new FirebaseIdentity(TestOnlyDataSeeder.TestFirebaseUid, TestOnlyDataSeeder.TestUserEmail, true);
            }

            return await _inner.VerifyIdentityAsync(idToken, cancellationToken);
        }
    }

    private sealed class TestGoogleTokenVerifier : IGoogleTokenVerifier
    {
        public Task<Result<GoogleVerifiedIdentity>> VerifyIdTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(idToken switch
            {
                "stranger-token" => Result<GoogleVerifiedIdentity>.Success(
                    new GoogleVerifiedIdentity("google-stranger-sub", "stranger@example.test", "Stranger")),
                "invited-google-token" => Result<GoogleVerifiedIdentity>.Success(
                    new GoogleVerifiedIdentity("google-invited-sub-1", "invited-google@example.test", "Invited Google User")),
                "invited-google-mismatch-token" => Result<GoogleVerifiedIdentity>.Success(
                    new GoogleVerifiedIdentity("google-invited-sub-2", "invited-google@example.test", "Invited Google User")),
                _ => Result<GoogleVerifiedIdentity>.Failure(new Error("GOOGLE_TOKEN_INVALID", "Invalid Google ID token."))
            });
        }
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
                    ["SeedTestData"] = "true"
                });
            });
            builder.ConfigureServices(services =>
            {
                var tokenDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (tokenDescriptor != null)
                {
                    services.Remove(tokenDescriptor);
                }
                services.AddSingleton<IFirebaseTokenVerifier>(sp =>
                    new TestTokenVerifier(sp.GetRequiredService<SelfHostedTokenVerifier>()));

                var googleDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IGoogleTokenVerifier));
                if (googleDescriptor != null)
                {
                    services.Remove(googleDescriptor);
                }
                services.AddSingleton<IGoogleTokenVerifier, TestGoogleTokenVerifier>();
            });
        });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = false
        });

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        _adminRoleId = await db.Roles
            .Where(r => r.OrganizationId == TestOnlyDataSeeder.TestOrgId && r.Name == "Test Admin")
            .Select(r => r.Id)
            .SingleAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    // =========================================================================
    // 1. Email/Password Login (Web & Mobile)
    // =========================================================================

    [Fact]
    public async Task Login_EmailPassword_WebPlatform_ReturnsAccessTokenAndSetsRefreshCookie()
    {
        var loginRequest = new LoginRequest(
            TestOnlyDataSeeder.TestUserEmail,
            TestOnlyDataSeeder.TestUserPassword,
            "web");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(result);
        Assert.Equal("authenticated", result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.Null(result.RefreshToken);
        Assert.NotNull(result.User);
        Assert.Equal(TestOnlyDataSeeder.TestUserEmail, result.User.Email);
        Assert.Equal(TestOnlyDataSeeder.TestUserDisplayName, result.User.DisplayName);

        // Verify Set-Cookie header for web platform
        Assert.True(response.Headers.TryGetValues("Set-Cookie", out var setCookies));
        var cookieHeader = setCookies.FirstOrDefault(c => c.StartsWith("tan_erp_refresh=", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(cookieHeader);
        Assert.Contains("path=/api/v1/auth", cookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("httponly", cookieHeader, StringComparison.OrdinalIgnoreCase);

        // Calling GET /api/v1/me with accessToken returns 200 OK with profile and memberships
        var meRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        meRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        var meResponse = await _client.SendAsync(meRequest);
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        var me = await meResponse.Content.ReadFromJsonAsync<CurrentUserResponse>(JsonOptions);
        Assert.NotNull(me);
        Assert.Equal(TestOnlyDataSeeder.TestUserEmail, me.User.Email);
        Assert.Equal(TestOnlyDataSeeder.TestUserDisplayName, me.User.DisplayName);
        Assert.NotEmpty(me.Memberships);
        Assert.Contains(me.Memberships, m => m.Id == TestOnlyDataSeeder.TestMembershipId);
    }

    [Fact]
    public async Task Login_EmailPassword_MobilePlatform_ReturnsRefreshTokenInJsonBody()
    {
        var loginRequest = new LoginRequest(
            TestOnlyDataSeeder.TestUserEmail,
            TestOnlyDataSeeder.TestUserPassword,
            "mobile");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(result);
        Assert.Equal("authenticated", result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));

        // For mobile, refresh token must not be set as a cookie
        if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
        {
            Assert.DoesNotContain(setCookies, c => c.StartsWith("tan_erp_refresh=", StringComparison.OrdinalIgnoreCase));
        }
    }

    // =========================================================================
    // 2. Invalid Credentials & Brute-Force Account Lockout
    // =========================================================================

    [Fact]
    public async Task Login_WrongPassword_Returns401_AndFiveFailuresLocksOutAccount()
    {
        var userEmail = "lockout-test@example.test";
        var initialPassword = "CorrectPassword123!";
        await AdminCreateUserAsync(userEmail, initialPassword);

        // Attempts 1 to 4 with wrong password: 401 INVALID_CREDENTIALS
        for (var attempt = 1; attempt <= 4; attempt++)
        {
            var failedResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
                new LoginRequest(userEmail, "WrongPassword123!", "web"));
            Assert.Equal(HttpStatusCode.Unauthorized, failedResponse.StatusCode);

            var problem = await ReadProblemDetailsAsync(failedResponse);
            Assert.Equal("INVALID_CREDENTIALS", problem.Code);
        }

        // 5th failed attempt triggers lockout: 429 ACCOUNT_LOCKED_OUT
        var fifthResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(userEmail, "WrongPassword123!", "web"));
        Assert.Equal(HttpStatusCode.TooManyRequests, fifthResponse.StatusCode);

        var fifthProblem = await ReadProblemDetailsAsync(fifthResponse);
        Assert.Equal("ACCOUNT_LOCKED_OUT", fifthProblem.Code);

        // Immediate 6th attempt with the CORRECT password while locked out: still 429 ACCOUNT_LOCKED_OUT
        var correctWhileLocked = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(userEmail, initialPassword, "web"));
        Assert.Equal(HttpStatusCode.TooManyRequests, correctWhileLocked.StatusCode);

        var lockedProblem = await ReadProblemDetailsAsync(correctWhileLocked);
        Assert.Equal("ACCOUNT_LOCKED_OUT", lockedProblem.Code);
    }

    // =========================================================================
    // 3. Google OAuth Invite-Only Policy
    // =========================================================================

    [Fact]
    public async Task GoogleLogin_InviteOnlyPolicy_EnforcesInviteAndSubjectIdMatching()
    {
        // 1. Uninvited stranger Google login -> 403 USER_NOT_INVITED
        var strangerResponse = await _client.PostAsJsonAsync("/api/v1/auth/google",
            new GoogleLoginRequest("stranger-token", "web"));
        Assert.Equal(HttpStatusCode.Forbidden, strangerResponse.StatusCode);

        var strangerProblem = await ReadProblemDetailsAsync(strangerResponse);
        Assert.Equal("USER_NOT_INVITED", strangerProblem.Code);

        // 2. Admin creates pending user without password -> invited-google@example.test
        var invitedEmail = "invited-google@example.test";
        await AdminCreateUserAsync(invitedEmail, initialPassword: null);

        // First Google login for pending user links account -> 200 OK authenticated
        var firstLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/google",
            new GoogleLoginRequest("invited-google-token", "web"));
        Assert.Equal(HttpStatusCode.OK, firstLoginResponse.StatusCode);

        var loginResult = await firstLoginResponse.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(loginResult);
        Assert.Equal("authenticated", loginResult.Status);
        Assert.False(string.IsNullOrWhiteSpace(loginResult.AccessToken));

        // Authenticated accessToken works on /api/v1/me
        var meRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        meRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.AccessToken);
        var meResponse = await _client.SendAsync(meRequest);
        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);

        // 3. Subsequent Google login attempt with same email but DIFFERENT SubjectId -> 409 GOOGLE_ACCOUNT_MISMATCH
        var mismatchResponse = await _client.PostAsJsonAsync("/api/v1/auth/google",
            new GoogleLoginRequest("invited-google-mismatch-token", "web"));
        Assert.Equal(HttpStatusCode.Conflict, mismatchResponse.StatusCode);

        var mismatchProblem = await ReadProblemDetailsAsync(mismatchResponse);
        Assert.Equal("GOOGLE_ACCOUNT_MISMATCH", mismatchProblem.Code);
    }

    // =========================================================================
    // 4. Self-Service RFC 6238 2FA TOTP & Single-Use Emergency Backup Codes
    // =========================================================================

    [Fact]
    public async Task SelfServiceTwoFactor_TotpAndBackupCodes_WorksAndEnforcesSingleUse()
    {
        var email = "totp-self-test@example.test";
        var password = "UserPassword123!";
        await AdminCreateUserAsync(email, password);

        // Step 1: User logs in to get access token
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, password, "web"));
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(loginResult);
        var accessToken = loginResult.AccessToken!;

        // Step 2: User initiates 2FA setup
        var setupRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/me/2fa/setup");
        setupRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var setupResponse = await _client.SendAsync(setupRequest);
        Assert.Equal(HttpStatusCode.OK, setupResponse.StatusCode);

        var setupPayload = await setupResponse.Content.ReadFromJsonAsync<TotpSetupPayloadDto>(JsonOptions);
        Assert.NotNull(setupPayload);
        Assert.False(string.IsNullOrWhiteSpace(setupPayload.SecretKey));
        Assert.False(string.IsNullOrWhiteSpace(setupPayload.OtpAuthUri));

        // Step 3: Compute valid 6-digit TOTP code and enable 2FA -> returns 8 backup codes
        var totpService = _factory.Services.GetRequiredService<ITotpService>();
        var enableTotpCode = totpService.ComputeCode(setupPayload.SecretKey, DateTimeOffset.UtcNow);

        var enableRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/me/2fa/enable");
        enableRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        enableRequest.Content = JsonContent.Create(new EnableTwoFactorRequest(enableTotpCode));
        var enableResponse = await _client.SendAsync(enableRequest);
        Assert.Equal(HttpStatusCode.OK, enableResponse.StatusCode);

        var backupCodesResponse = await enableResponse.Content.ReadFromJsonAsync<TwoFactorBackupCodesResponse>(JsonOptions);
        Assert.NotNull(backupCodesResponse);
        Assert.Equal(8, backupCodesResponse.BackupCodes.Count);

        // Step 4: Next login requires 2FA challenge
        var twoFactorLogin = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, password, "web"));
        Assert.Equal(HttpStatusCode.OK, twoFactorLogin.StatusCode);

        var twoFactorResult = await twoFactorLogin.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(twoFactorResult);
        Assert.Equal("requires_two_factor", twoFactorResult.Status);
        Assert.False(string.IsNullOrWhiteSpace(twoFactorResult.ChallengeToken));
        Assert.Null(twoFactorResult.AccessToken);
        var challengeToken = twoFactorResult.ChallengeToken!;

        // Step 5: Verify challengeToken CANNOT be used on /api/v1/me (401)
        var blockedMeRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        blockedMeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", challengeToken);
        var blockedMeResponse = await _client.SendAsync(blockedMeRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, blockedMeResponse.StatusCode);

        // Step 6: Verify 2FA login with valid 6-digit TOTP
        var verifyTotpCode = totpService.ComputeCode(setupPayload.SecretKey, DateTimeOffset.UtcNow);
        var verifyLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/2fa/verify-login",
            new VerifyTwoFactorLoginRequest(challengeToken, verifyTotpCode, "web"));
        Assert.Equal(HttpStatusCode.OK, verifyLoginResponse.StatusCode);

        var verifiedResult = await verifyLoginResponse.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(verifiedResult);
        Assert.Equal("authenticated", verifiedResult.Status);
        Assert.False(string.IsNullOrWhiteSpace(verifiedResult.AccessToken));

        // Step 7: Verify login using a single-use backup code
        var backupLogin = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, password, "web"));
        var backupChallenge = (await backupLogin.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions))!.ChallengeToken!;

        var chosenBackupCode = backupCodesResponse.BackupCodes[0];
        var backupVerifyResponse = await _client.PostAsJsonAsync("/api/v1/auth/2fa/verify-login",
            new VerifyTwoFactorLoginRequest(backupChallenge, chosenBackupCode, "web"));
        Assert.Equal(HttpStatusCode.OK, backupVerifyResponse.StatusCode);

        var backupAuthResult = await backupVerifyResponse.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(backupAuthResult);
        Assert.Equal("authenticated", backupAuthResult.Status);

        // Step 8: Reusing that exact same backup code on subsequent login returns 400 TWO_FACTOR_CODE_INVALID
        var thirdLogin = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, password, "web"));
        var thirdChallenge = (await thirdLogin.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions))!.ChallengeToken!;

        var reuseVerifyResponse = await _client.PostAsJsonAsync("/api/v1/auth/2fa/verify-login",
            new VerifyTwoFactorLoginRequest(thirdChallenge, chosenBackupCode, "web"));
        Assert.Equal(HttpStatusCode.BadRequest, reuseVerifyResponse.StatusCode);

        var reuseProblem = await ReadProblemDetailsAsync(reuseVerifyResponse);
        Assert.Equal("TWO_FACTOR_CODE_INVALID", reuseProblem.Code);
    }

    // =========================================================================
    // 5. Admin-Enforced Mandatory 2FA (RequireTwoFactor = true)
    // =========================================================================

    [Fact]
    public async Task AdminEnforcedMandatoryTwoFactor_ForcesSetupOnLogin_AndPreventsDisabling()
    {
        var email = "mandatory-2fa-test@example.test";
        var password = "MandatoryPassword123!";
        await AdminCreateUserAsync(email, password, requireTwoFactor: true);

        // Step 1: User logs in -> status: requires_two_factor_setup with totpSetup
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, password, "web"));
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(loginResult);
        Assert.Equal("requires_two_factor_setup", loginResult.Status);
        Assert.False(string.IsNullOrWhiteSpace(loginResult.ChallengeToken));
        Assert.NotNull(loginResult.TotpSetup);
        Assert.False(string.IsNullOrWhiteSpace(loginResult.TotpSetup.SecretKey));
        Assert.False(string.IsNullOrWhiteSpace(loginResult.TotpSetup.OtpAuthUri));

        // Step 2: User completes setup by verifying valid 6-digit TOTP -> status: authenticated + 8 backup codes
        var totpService = _factory.Services.GetRequiredService<ITotpService>();
        var totpCode = totpService.ComputeCode(loginResult.TotpSetup.SecretKey, DateTimeOffset.UtcNow);

        var verifyResponse = await _client.PostAsJsonAsync("/api/v1/auth/2fa/verify-login",
            new VerifyTwoFactorLoginRequest(loginResult.ChallengeToken, totpCode, "web"));
        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);

        var authResult = await verifyResponse.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(authResult);
        Assert.Equal("authenticated", authResult.Status);
        Assert.False(string.IsNullOrWhiteSpace(authResult.AccessToken));
        Assert.NotNull(authResult.BackupCodes);
        Assert.Equal(8, authResult.BackupCodes.Count);

        var userAccessToken = authResult.AccessToken!;

        // Step 3: User attempts to disable 2FA -> 409 TWO_FACTOR_REQUIRED_BY_ADMIN
        var freshCode = totpService.ComputeCode(loginResult.TotpSetup.SecretKey, DateTimeOffset.UtcNow);
        var disableRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/me/2fa/disable");
        disableRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);
        disableRequest.Content = JsonContent.Create(new DisableTwoFactorRequest(freshCode));

        var disableResponse = await _client.SendAsync(disableRequest);
        Assert.Equal(HttpStatusCode.Conflict, disableResponse.StatusCode);

        var disableProblem = await ReadProblemDetailsAsync(disableResponse);
        Assert.Equal("TWO_FACTOR_REQUIRED_BY_ADMIN", disableProblem.Code);
    }

    // =========================================================================
    // 6. Rotating Refresh Token, 15s Multi-Tab Grace Window & Token Reuse Detection
    // =========================================================================

    [Fact]
    public async Task RefreshToken_RotationGracePeriodAndReuseDetection_RevokesSessionInRealTime()
    {
        var email = "token-rotation-test@example.test";
        var password = "RotationPassword123!";
        await AdminCreateUserAsync(email, password);

        // Step 1: Initial login (mobile platform) -> returns refreshToken1 and accessToken1
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, password, "mobile"));
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(loginResult);
        var refreshToken1 = loginResult.RefreshToken!;
        var accessToken1 = loginResult.AccessToken!;

        // Verify initial access token works on /api/v1/me
        var meRequest1 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        meRequest1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken1);
        Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(meRequest1)).StatusCode);

        // Step 2: Refresh with refreshToken1 -> rotates to refreshToken2
        var rotateResponse1 = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshSessionRequest(refreshToken1, "mobile"));
        Assert.Equal(HttpStatusCode.OK, rotateResponse1.StatusCode);

        var rotateResult1 = await rotateResponse1.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(rotateResult1);
        var refreshToken2 = rotateResult1.RefreshToken!;
        var accessToken2 = rotateResult1.AccessToken!;
        Assert.NotEqual(refreshToken1, refreshToken2);

        // Step 3: Immediate replay of refreshToken1 within 15s grace window -> 200 OK (grace hit)
        var graceHitResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshSessionRequest(refreshToken1, "mobile"));
        Assert.Equal(HttpStatusCode.OK, graceHitResponse.StatusCode);

        var graceResult = await graceHitResponse.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(graceResult);
        Assert.Equal("authenticated", graceResult.Status);
        Assert.False(string.IsNullOrWhiteSpace(graceResult.AccessToken));

        // Step 4: Rotate again with refreshToken2 -> rotates to refreshToken3
        var rotateResponse2 = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshSessionRequest(refreshToken2, "mobile"));
        Assert.Equal(HttpStatusCode.OK, rotateResponse2.StatusCode);

        var rotateResult2 = await rotateResponse2.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(rotateResult2);
        var refreshToken3 = rotateResult2.RefreshToken!;
        var accessToken3 = rotateResult2.AccessToken!;
        Assert.NotEqual(refreshToken2, refreshToken3);

        // Step 5: Token reuse detected! Replaying refreshToken1 (now older than previous) -> 401 AUTHENTICATION_INVALID
        var reuseResponse = await _client.PostAsJsonAsync("/api/v1/auth/refresh",
            new RefreshSessionRequest(refreshToken1, "mobile"));
        Assert.Equal(HttpStatusCode.Unauthorized, reuseResponse.StatusCode);

        var reuseProblem = await ReadProblemDetailsAsync(reuseResponse);
        Assert.Equal("AUTHENTICATION_INVALID", reuseProblem.Code);

        // Step 6: Real-time Session Revocation! Calling /api/v1/me using that session's accessToken immediately fails with 401
        var revokedMeRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        revokedMeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken3);
        var revokedMeResponse = await _client.SendAsync(revokedMeRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, revokedMeResponse.StatusCode);

        var revokedProblem = await ReadProblemDetailsAsync(revokedMeResponse);
        Assert.Equal("AUTHENTICATION_INVALID", revokedProblem.Code);
    }

    // =========================================================================
    // 7. Active Session Management & Password Change Revocation
    // =========================================================================

    [Fact]
    public async Task ActiveSessionManagement_RevokeSessionAndChangePassword_RevokesTargetSessions()
    {
        var email = "session-mgr-test@example.test";
        var password = "SessionPassword123!";
        await AdminCreateUserAsync(email, password);

        // Step 1: Create 2 sessions for the same user (web and mobile)
        var loginWeb = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, password, "web"));
        Assert.Equal(HttpStatusCode.OK, loginWeb.StatusCode);
        var webResult = await loginWeb.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        var accessToken1 = webResult!.AccessToken!;
        var sessionId1 = webResult.SessionId!.Value;

        var loginMobile = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, password, "mobile"));
        Assert.Equal(HttpStatusCode.OK, loginMobile.StatusCode);
        var mobileResult = await loginMobile.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        var accessToken2 = mobileResult!.AccessToken!;
        var sessionId2 = mobileResult.SessionId!.Value;

        // Step 2: GET /api/v1/me/sessions with accessToken1 lists both with accurate isCurrent
        var sessionsRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me/sessions");
        sessionsRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken1);
        var sessionsResponse = await _client.SendAsync(sessionsRequest);
        Assert.Equal(HttpStatusCode.OK, sessionsResponse.StatusCode);

        var sessionList = await sessionsResponse.Content.ReadFromJsonAsync<ActiveSessionListResponse>(JsonOptions);
        Assert.NotNull(sessionList);
        Assert.True(sessionList.Items.Count >= 2);

        var currentItem = sessionList.Items.Single(s => s.Id == sessionId1);
        Assert.True(currentItem.IsCurrent);

        var otherItem = sessionList.Items.Single(s => s.Id == sessionId2);
        Assert.False(otherItem.IsCurrent);

        // Step 3: DELETE /api/v1/me/sessions/{sessionId2} revokes the second session
        var revokeRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/me/sessions/{sessionId2}");
        revokeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken1);
        var revokeResponse = await _client.SendAsync(revokeRequest);
        Assert.Equal(HttpStatusCode.NoContent, revokeResponse.StatusCode);

        // accessToken2 immediately fails /api/v1/me with 401
        var meRequestSession2 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        meRequestSession2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken2);
        Assert.Equal(HttpStatusCode.Unauthorized, (await _client.SendAsync(meRequestSession2)).StatusCode);

        // accessToken1 still works on /api/v1/me with 200 OK
        var meRequestSession1 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        meRequestSession1.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken1);
        Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(meRequestSession1)).StatusCode);

        // Step 4: Create another second session (Session 3)
        var loginSession3 = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, password, "mobile"));
        var session3Result = await loginSession3.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        var accessToken3 = session3Result!.AccessToken!;

        // Change password from Session 1
        var newPassword = "NewSessionPassword456!";
        var changePasswordRequest = new HttpRequestMessage(HttpMethod.Put, "/api/v1/me/password");
        changePasswordRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken1);
        changePasswordRequest.Content = JsonContent.Create(new ChangeMyPasswordRequest(password, newPassword));
        var changePasswordResponse = await _client.SendAsync(changePasswordRequest);
        Assert.Equal(HttpStatusCode.NoContent, changePasswordResponse.StatusCode);

        // Current session (Session 1) still works on /api/v1/me
        var mePostPasswordChange = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        mePostPasswordChange.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken1);
        Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(mePostPasswordChange)).StatusCode);

        // Session 3 was revoked by password change: /api/v1/me fails with 401
        var meSession3Revoked = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        meSession3Revoked.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken3);
        Assert.Equal(HttpStatusCode.Unauthorized, (await _client.SendAsync(meSession3Revoked)).StatusCode);

        // Old password fails login
        var oldPassLogin = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, password, "web"));
        Assert.Equal(HttpStatusCode.Unauthorized, oldPassLogin.StatusCode);

        // New password succeeds login
        var newPassLogin = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, newPassword, "web"));
        Assert.Equal(HttpStatusCode.OK, newPassLogin.StatusCode);
        Assert.Equal("authenticated", (await newPassLogin.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions))!.Status);
    }

    // =========================================================================
    // 8. Admin User Security Endpoints
    // =========================================================================

    [Fact]
    public async Task AdminUserSecurity_ResetPasswordTwoFactorPolicyAndReset_RevokesSessionsAndClearsState()
    {
        var email = "admin-sec-test@example.test";
        var initialPassword = "AdminSecInitialPass123!";
        var createdUser = await AdminCreateUserAsync(email, initialPassword, requireTwoFactor: false);

        // Target user logs in
        var userLogin = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, initialPassword, "web"));
        Assert.Equal(HttpStatusCode.OK, userLogin.StatusCode);
        var userAuth = await userLogin.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        var userAccessToken1 = userAuth!.AccessToken!;

        // 1. Admin resets password with If-Match -> revokes user active sessions
        var latestUser1 = await AdminGetUserAsync(createdUser.Id);
        var adminResetPassRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/admin/users/{createdUser.Id}/password");
        adminResetPassRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "admin-token");
        adminResetPassRequest.Headers.Add("X-Membership-Id", TestOnlyDataSeeder.TestMembershipId.ToString());
        adminResetPassRequest.Headers.Add("If-Match", $"\"{latestUser1.RowVersion}\"");
        var newAdminPassword = "AdminChangedPassword789!";
        adminResetPassRequest.Content = JsonContent.Create(new SetAdminUserPasswordRequest(newAdminPassword));

        var adminResetPassResponse = await _client.SendAsync(adminResetPassRequest);
        Assert.Equal(HttpStatusCode.OK, adminResetPassResponse.StatusCode);

        // Previous user session revoked -> 401 on /api/v1/me
        var meRevokedRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        meRevokedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken1);
        Assert.Equal(HttpStatusCode.Unauthorized, (await _client.SendAsync(meRevokedRequest)).StatusCode);

        // User logs in with new password -> 200 OK
        var newLogin = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, newAdminPassword, "web"));
        Assert.Equal(HttpStatusCode.OK, newLogin.StatusCode);
        var userAccessToken2 = (await newLogin.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions))!.AccessToken!;

        // 2. Admin updates two-factor policy (toggles requireTwoFactor: true)
        var latestUser2 = await AdminGetUserAsync(createdUser.Id);
        var updatePolicyRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/admin/users/{createdUser.Id}/two-factor-policy");
        updatePolicyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "admin-token");
        updatePolicyRequest.Headers.Add("X-Membership-Id", TestOnlyDataSeeder.TestMembershipId.ToString());
        updatePolicyRequest.Headers.Add("If-Match", $"\"{latestUser2.RowVersion}\"");
        updatePolicyRequest.Content = JsonContent.Create(new UpdateAdminUserTwoFactorPolicyRequest(RequireTwoFactor: true));

        var updatePolicyResponse = await _client.SendAsync(updatePolicyRequest);
        Assert.Equal(HttpStatusCode.OK, updatePolicyResponse.StatusCode);
        var policyUser = await updatePolicyResponse.Content.ReadFromJsonAsync<AdminUserResponse>(JsonOptions);
        Assert.True(policyUser!.RequireTwoFactor);

        // Target user sets up and enables 2FA
        var setupRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/me/2fa/setup");
        setupRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken2);
        var setupResponse = await _client.SendAsync(setupRequest);
        var setupPayload = (await setupResponse.Content.ReadFromJsonAsync<TotpSetupPayloadDto>(JsonOptions))!;

        var totpService = _factory.Services.GetRequiredService<ITotpService>();
        var totpCode = totpService.ComputeCode(setupPayload.SecretKey, DateTimeOffset.UtcNow);

        var enableRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/me/2fa/enable");
        enableRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken2);
        enableRequest.Content = JsonContent.Create(new EnableTwoFactorRequest(totpCode));
        Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(enableRequest)).StatusCode);

        // 3. Admin resets 2FA -> clears TOTP & backup codes and revokes active sessions
        var latestUser3 = await AdminGetUserAsync(createdUser.Id);
        var reset2FaRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/admin/users/{createdUser.Id}/two-factor-reset");
        reset2FaRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "admin-token");
        reset2FaRequest.Headers.Add("X-Membership-Id", TestOnlyDataSeeder.TestMembershipId.ToString());
        reset2FaRequest.Headers.Add("If-Match", $"\"{latestUser3.RowVersion}\"");

        var reset2FaResponse = await _client.SendAsync(reset2FaRequest);
        Assert.Equal(HttpStatusCode.OK, reset2FaResponse.StatusCode);

        // Previous user session revoked -> 401 on /api/v1/me
        var meRevoked2Fa = new HttpRequestMessage(HttpMethod.Get, "/api/v1/me");
        meRevoked2Fa.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken2);
        Assert.Equal(HttpStatusCode.Unauthorized, (await _client.SendAsync(meRevoked2Fa)).StatusCode);

        // Direct DB verification: user has 0 backup codes and Totp is disabled
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userInDb = await db.Users.FindAsync(createdUser.Id);
        Assert.NotNull(userInDb);
        Assert.False(userInDb.IsTotpEnabled);
        Assert.Null(userInDb.TotpSecretEncrypted);

        var backupCodesCount = await db.UserBackupCodes.CountAsync(c => c.UserId == createdUser.Id);
        Assert.Equal(0, backupCodesCount);

        // Next login requires setup because policy is still requireTwoFactor: true but 2FA was cleared
        var loginAfterReset = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequest(email, newAdminPassword, "web"));
        Assert.Equal(HttpStatusCode.OK, loginAfterReset.StatusCode);
        var postResetResult = await loginAfterReset.Content.ReadFromJsonAsync<AuthResultResponse>(JsonOptions);
        Assert.NotNull(postResetResult);
        Assert.Equal("requires_two_factor_setup", postResetResult.Status);
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    private async Task<AdminUserResponse> AdminCreateUserAsync(
        string email,
        string? initialPassword = null,
        bool requireTwoFactor = false)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/admin/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "admin-token");
        request.Headers.Add("X-Membership-Id", TestOnlyDataSeeder.TestMembershipId.ToString());
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());
        request.Content = JsonContent.Create(new CreateAdminUserRequest(
            DisplayName: "Test " + email,
            Email: email,
            BranchId: TestOnlyDataSeeder.TestBranchId,
            RoleIds: new[] { _adminRoleId },
            InitialPassword: initialPassword,
            RequireTwoFactor: requireTwoFactor));

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<AdminUserResponse>(JsonOptions);
        Assert.NotNull(created);
        return created;
    }

    private async Task<AdminUserResponse> AdminGetUserAsync(Guid userId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/admin/users/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "admin-token");
        request.Headers.Add("X-Membership-Id", TestOnlyDataSeeder.TestMembershipId.ToString());

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var user = await response.Content.ReadFromJsonAsync<AdminUserResponse>(JsonOptions);
        Assert.NotNull(user);
        return user;
    }

    private static async Task<ApiProblemDetails> ReadProblemDetailsAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        var problem = JsonSerializer.Deserialize<ApiProblemDetails>(body, JsonOptions);
        Assert.NotNull(problem);
        return problem;
    }
}
