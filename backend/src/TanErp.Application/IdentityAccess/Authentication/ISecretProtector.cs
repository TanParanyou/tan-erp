namespace TanErp.Application.IdentityAccess.Authentication;

public interface ISecretProtector
{
    string Protect(string plaintext);

    string Unprotect(string protectedPayload);
}
