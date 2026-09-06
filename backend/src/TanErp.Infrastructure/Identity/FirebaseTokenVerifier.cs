using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;

namespace TanErp.Infrastructure.Identity;

public interface IFirebaseTokenVerifier
{
    Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default);
}

public class FirebaseTokenVerifier : IFirebaseTokenVerifier
{
    private readonly FirebaseAuth _auth;

    public FirebaseTokenVerifier(IConfiguration configuration)
    {
        var projectId = configuration["Firebase:ProjectId"]
            ?? throw new InvalidOperationException(
                "Required configuration 'Firebase:ProjectId' is missing.");
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        var isProduction = string.Equals(envName, "Production", StringComparison.OrdinalIgnoreCase);

        if (FirebaseApp.DefaultInstance == null)
        {
            var emulatorHost = Environment.GetEnvironmentVariable("FIREBASE_AUTH_EMULATOR_HOST");
            if (!string.IsNullOrEmpty(emulatorHost))
            {
                if (isProduction)
                {
                    throw new InvalidOperationException(
                        "FIREBASE_AUTH_EMULATOR_HOST must not be set in production.");
                }

                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromAccessToken("owner"),
                    ProjectId = projectId
                });
            }
            else
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault(),
                    ProjectId = projectId
                });
            }
        }

        _auth = FirebaseAuth.DefaultInstance;
    }

    public async Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var decoded = await _auth.VerifyIdTokenAsync(idToken, cancellationToken);
            return decoded?.Uid;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (FirebaseAuthException)
        {
            return null;
        }
    }
}
