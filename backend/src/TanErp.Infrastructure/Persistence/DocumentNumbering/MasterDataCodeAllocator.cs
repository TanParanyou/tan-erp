using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Results;

namespace TanErp.Infrastructure.Persistence.DocumentNumbering;

internal static class MasterDataCodeAllocator
{
    public static async Task<Result<string>> ResolveAsync(
        IDocumentNumberGenerator generator,
        Guid organizationId,
        string documentType,
        string? requestedCode,
        int maximumLength,
        string duplicateErrorCode,
        Func<string, CancellationToken, Task<bool>> codeExists,
        CancellationToken cancellationToken)
    {
        if (requestedCode is not null)
        {
            var manualCode = requestedCode.Trim();
            if (manualCode.Length == 0)
            {
                return Result<string>.Failure(new Error("ITEM_CODE_INVALID", "A manually entered code cannot be blank."));
            }

            return await codeExists(manualCode, cancellationToken)
                ? Result<string>.Failure(new Error(duplicateErrorCode, "The code already exists in this organization."))
                : Result<string>.Success(manualCode);
        }

        while (true)
        {
            var generatedCode = await generator.GenerateAsync(
                organizationId,
                documentType,
                cancellationToken: cancellationToken);
            if (generatedCode.Length > maximumLength)
            {
                return Result<string>.Failure(new Error(
                    "MASTER_DATA_CODE_CONFIGURATION_INVALID",
                    "The configured master data code exceeds the resource length limit."));
            }

            if (!await codeExists(generatedCode, cancellationToken))
            {
                return Result<string>.Success(generatedCode);
            }
        }
    }
}
