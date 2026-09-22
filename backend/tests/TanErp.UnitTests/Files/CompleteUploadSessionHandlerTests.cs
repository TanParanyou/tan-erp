using System.Security.Cryptography;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Common.Models;
using TanErp.Application.Common.Results;
using TanErp.Application.Files;
using TanErp.Application.Files.CompleteUploadSession;
using TanErp.Application.Files.CreateUploadSession;
using TanErp.Domain.Files;
using Xunit;

namespace TanErp.UnitTests.Files;

public class CompleteUploadSessionHandlerTests
{
    private readonly FakeRequestAccessResolver _accessResolver = new();
    private readonly FakeParentAccessResolver _parentAccessResolver = new();
    private readonly FakeStorageProvider _storageProvider = new();
    private readonly FakeFileStore _fileStore = new();
    private readonly FakeClock _clock = new();

    private readonly CompleteUploadSessionHandler _handler;

    private static readonly Guid OrgId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid MembershipId = Guid.NewGuid();
    private static readonly Guid SessionId = Guid.NewGuid();

    public CompleteUploadSessionHandlerTests()
    {
        _handler = new CompleteUploadSessionHandler(
            _accessResolver,
            _parentAccessResolver,
            _storageProvider,
            _fileStore,
            _clock);

        _accessResolver.Context = new RequestAccessContext(
            UserId,
            MembershipId,
            OrgId,
            null,
            "items.manage-images",
            "organization");
    }

    private static byte[] ValidPngBytes(int totalSize = 64)
    {
        var bytes = new byte[Math.Max(64, totalSize)];
        // PNG magic: 89 50 4E 47 0D 0A 1A 0A
        bytes[0] = 0x89;
        bytes[1] = 0x50;
        bytes[2] = 0x4E;
        bytes[3] = 0x47;
        bytes[4] = 0x0D;
        bytes[5] = 0x0A;
        bytes[6] = 0x1A;
        bytes[7] = 0x0A;
        for (int i = 8; i < bytes.Length; i++)
        {
            bytes[i] = (byte)(i % 256);
        }
        return bytes;
    }

    private CompleteUploadSessionCommand CreateCommand(long declaredSize, byte[] streamBytes, Guid? slotId = null)
    {
        var sId = slotId ?? Guid.NewGuid();
        var session = new FileUploadSession(
            SessionId,
            OrgId,
            FileParentTypes.Item,
            Guid.NewGuid(),
            null,
            UserId,
            _clock.UtcNow,
            _clock.UtcNow.AddMinutes(30),
            "idemp-hash",
            "payload-hash");

        session.AddSlot(new FileUploadSlot(
            sId,
            session.Id,
            0,
            "test.png",
            "image/png",
            declaredSize));

        _fileStore.Session = session;

        return new CompleteUploadSessionCommand(
            "test-uid",
            MembershipId,
            SessionId,
            new[]
            {
                new FileCompletionInput(
                    sId,
                    "test.png",
                    "image/png",
                    declaredSize,
                    new MemoryStream(streamBytes))
            },
            "test-trace-id");
    }

    [Fact]
    public async Task Complete_rejects_content_larger_than_declared_slot_size()
    {
        var declared = ValidPngBytes(64);
        var submitted = declared.Concat(new byte[1024]).ToArray();
        var command = CreateCommand(declaredSize: declared.Length, submitted);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_SIZE_MISMATCH", result.Error.Code);
        Assert.Empty(_storageProvider.SavedPaths);
    }

    [Fact]
    public async Task Complete_rejects_stream_exceeding_10MB_even_if_declared_small()
    {
        var declared = ValidPngBytes(64);
        // Exceed 10 MB (10 * 1024 * 1024 + 100 bytes)
        var submitted = declared.Concat(new byte[10 * 1024 * 1024 + 100]).ToArray();
        var command = CreateCommand(declaredSize: declared.Length, submitted);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.True(result.Error.Code is "FILE_TOO_LARGE" or "FILE_SIZE_MISMATCH");
        Assert.Empty(_storageProvider.SavedPaths);
    }

    [Fact]
    public async Task Complete_rejects_truncated_content_shorter_than_declared()
    {
        var declaredBytes = ValidPngBytes(128);
        var truncated = declaredBytes[..32];
        var command = CreateCommand(declaredSize: declaredBytes.Length, truncated);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_SIZE_MISMATCH", result.Error.Code);
        Assert.Empty(_storageProvider.SavedPaths);
    }

    [Fact]
    public async Task Complete_persists_deterministic_lowercase_sha256()
    {
        var bytes = ValidPngBytes(64);
        var expectedSha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        var command = CreateCommand(declaredSize: bytes.Length, bytes);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(_fileStore.PersistedFiles);
        var persisted = _fileStore.PersistedFiles[0];
        Assert.Equal(expectedSha256, persisted.ContentSha256);
        Assert.Equal("content_verified", persisted.ScanStatus);
        Assert.NotNull(persisted.VerifiedAtUtc);
    }

    [Fact]
    public async Task Complete_rejects_magic_number_mismatch()
    {
        var fakeBytes = new byte[64];
        Array.Fill(fakeBytes, (byte)0x00); // Invalid header
        var command = CreateCommand(declaredSize: fakeBytes.Length, fakeBytes);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_UPLOAD_SESSION_INVALID", result.Error.Code);
        Assert.Empty(_storageProvider.SavedPaths);
    }

    [Fact]
    public async Task Complete_cleans_up_already_saved_paths_on_failure()
    {
        var slot1 = Guid.NewGuid();
        var slot2 = Guid.NewGuid();

        var session = new FileUploadSession(
            SessionId,
            OrgId,
            FileParentTypes.Item,
            Guid.NewGuid(),
            null,
            UserId,
            _clock.UtcNow,
            _clock.UtcNow.AddMinutes(30),
            "idemp-hash",
            "payload-hash");
        session.AddSlot(new FileUploadSlot(slot1, session.Id, 0, "file1.png", "image/png", 64));
        session.AddSlot(new FileUploadSlot(slot2, session.Id, 1, "file2.png", "image/png", 64));
        _fileStore.Session = session;

        var file1Bytes = ValidPngBytes(64);
        var file2Corrupt = new byte[64]; // Corrupt magic

        var command = new CompleteUploadSessionCommand(
            "test-uid",
            MembershipId,
            SessionId,
            new[]
            {
                new FileCompletionInput(slot1, "file1.png", "image/png", 64, new MemoryStream(file1Bytes)),
                new FileCompletionInput(slot2, "file2.png", "image/png", 64, new MemoryStream(file2Corrupt))
            },
            "test-trace-id");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        // All paths that were saved prior to failure must be cleaned up from storage
        Assert.Empty(_storageProvider.SavedPaths);
    }

    // --- Fakes ---

    private sealed class FakeRequestAccessResolver : IRequestAccessResolver
    {
        public RequestAccessContext Context { get; set; } = null!;

        public Task<Result<RequestAccessContext>> ResolveAsync(string firebaseUid, Guid membershipId, string requiredPermission, CancellationToken ct = default)
            => Task.FromResult(Result<RequestAccessContext>.Success(Context));

        public Task<Result<RequestAccessContext>> ResolveAnyAsync(string firebaseUid, Guid membershipId, IReadOnlyCollection<string> requiredPermissions, CancellationToken ct = default)
            => Task.FromResult(Result<RequestAccessContext>.Success(Context));
    }

    private sealed class FakeParentAccessResolver : IFileParentAccessResolver
    {
        public Task<Result<FileParentAccess>> ResolveAsync(RequestAccessContext access, string parentType, Guid? parentId, Guid? creationIntentId, FileAccessOperation operation, CancellationToken ct = default)
            => Task.FromResult(Result<FileParentAccess>.Success(new FileParentAccess(parentType, parentId, creationIntentId, access.OrganizationId)));
    }

    private sealed class FakeStorageProvider : IFileStorageProvider
    {
        public List<string> SavedPaths { get; } = new();

        public Task<string> SaveAsync(Guid organizationId, string sessionId, string filename, Stream content, CancellationToken ct = default)
        {
            var path = $"{organizationId}/{sessionId}/{filename}";
            SavedPaths.Add(path);
            return Task.FromResult(path);
        }

        public Task DeleteAsync(string storagePath, CancellationToken ct = default)
        {
            SavedPaths.Remove(storagePath);
            return Task.CompletedTask;
        }

        public string GetServingUrl(string storagePath) => $"/storage/{storagePath}";

        public Task<Stream?> OpenReadStreamAsync(string storagePath, CancellationToken ct = default)
            => Task.FromResult<Stream?>(new MemoryStream());
    }

    private sealed class FakeFileStore : IFileStore
    {
        public FileUploadSession? Session { get; set; }
        public List<UploadedFile> PersistedFiles { get; } = new();

        public Task<Result<UploadedFile>> GetByIdAsync(Guid fileId, Guid organizationId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task SaveAsync(UploadedFile file, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<Result<FileUploadSession>> CreateSessionAsync(RequestAccessContext access, string parentType, Guid? parentId, Guid? creationIntentId, IReadOnlyList<FileSlotInput> files, string keyHash, string payloadHash, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Result<FileUploadSession>> GetCompletableSessionAsync(Guid organizationId, Guid actorUserId, Guid sessionId, CancellationToken ct = default)
        {
            if (Session is null || Session.Id != sessionId || Session.OrganizationId != organizationId)
                return Task.FromResult(Result<FileUploadSession>.Failure(new Error("FILE_UPLOAD_SESSION_INVALID", "Not found")));
            return Task.FromResult(Result<FileUploadSession>.Success(Session));
        }

        public Task<Result<IReadOnlyList<UploadedFile>>> CompleteSessionAsync(Guid organizationId, Guid actorUserId, Guid sessionId, IReadOnlyList<UploadedFile> files, CancellationToken ct = default)
        {
            PersistedFiles.AddRange(files);
            return Task.FromResult(Result<IReadOnlyList<UploadedFile>>.Success(files));
        }

        public Task<Result<TanErp.Application.Files.GetFileContent.FileContentResult>> GetAuthorizedFileContentAsync(RequestAccessContext access, Guid fileId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Result<bool>> ValidateVerifiedFilesForParentAsync(Guid organizationId, Guid actorUserId, string parentType, Guid? parentId, Guid? creationIntentId, IReadOnlyCollection<Guid> fileIds, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task BindFilesToParentAsync(Guid organizationId, Guid? creationIntentId, Guid actualParentId, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakeClock : IClock
    {
        public DateTimeOffset UtcNow { get; } = DateTimeOffset.UtcNow;
    }
}
