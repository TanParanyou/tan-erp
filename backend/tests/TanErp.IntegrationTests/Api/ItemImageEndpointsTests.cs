using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Files;
using TanErp.Api.Contracts.Items;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class ItemImageEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;
    private const string UidB = TestOnlyDataSeeder.TestFirebaseUidB;
    private const string UidNoPerm = "uid-no-image-perm";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f6d01");

    private class TestFirebaseTokenVerifier : IFirebaseTokenVerifier
    {
        public Task<string?> VerifyTokenAsync(string idToken, CancellationToken cancellationToken = default)
        {
            var uid = idToken switch
            {
                "token-org-a" => UidA,
                "token-org-b" => UidB,
                "token-no-perm" => UidNoPerm,
                _ => null
            };

            return Task.FromResult(uid);
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
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFirebaseTokenVerifier));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }
                services.AddSingleton<IFirebaseTokenVerifier, TestFirebaseTokenVerifier>();
            });
        });

        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await TestOnlyDataSeeder.SeedAsync(db, "Test", true);

        var noPermUser = new TanErp.Domain.IdentityAccess.User(Guid.NewGuid(), UidNoPerm, "noimgperm@example.com", "No Img Perm User", true);
        db.Users.Add(noPermUser);
        var noPermMembership = new TanErp.Domain.Organization.Membership(
            MembershipNoPermId,
            TestOnlyDataSeeder.TestOrgId,
            TestOnlyDataSeeder.TestBranchId,
            noPermUser.Id,
            isActive: true);
        db.Memberships.Add(noPermMembership);
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    private HttpRequestMessage CreateRequest(
        HttpMethod method,
        string url,
        string token = "token-org-a",
        Guid? membershipId = null,
        Guid? ifMatch = null,
        string? idempotencyKey = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membershipId ?? TestOnlyDataSeeder.TestMembershipId).ToString());

        if (ifMatch.HasValue)
        {
            request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        }

        if (idempotencyKey != null)
        {
            request.Headers.Add("Idempotency-Key", idempotencyKey);
        }

        return request;
    }

    private async Task<Guid> CreateTestItemAsync()
    {
        // 1. Create Category
        var catReq = CreateRequest(HttpMethod.Post, "/api/v1/item-categories");
        catReq.Content = JsonContent.Create(new CreateItemCategoryRequest
        {
            Code = "CAT-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "หมวดหมู่ทดสอบรูปภาพ", English = "Image Test Cat" },
            AllowedItemTypes = new List<string> { "material" }
        });
        var catRes = await _client.SendAsync(catReq);
        var cat = await catRes.Content.ReadFromJsonAsync<ItemCategoryDetailResponse>();

        // 2. Create Unit
        var unitReq = CreateRequest(HttpMethod.Post, "/api/v1/units-of-measure");
        unitReq.Content = JsonContent.Create(new CreateUnitOfMeasureRequest
        {
            Code = "UOM-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "ชิ้น", English = "Piece" },
            Symbol = "pcs"
        });
        var unitRes = await _client.SendAsync(unitReq);
        var unit = await unitRes.Content.ReadFromJsonAsync<UnitOfMeasureDetailResponse>();

        // 3. Create Item
        var itemReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        itemReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "IMG-ITEM-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = cat!.Id,
            BaseUnitId = unit!.Id,
            Name = new LocalizedTextInput { Thai = "สินค้าทดสอบรูปภาพ", English = "Image Test Item" }
        });
        var itemRes = await _client.SendAsync(itemReq);
        var item = await itemRes.Content.ReadFromJsonAsync<ItemResponse>();
        return item!.Id;
    }

    private static readonly byte[] ValidJpegBytes =
    [
        0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00
    ];

    private async Task<Guid> UploadFileForItemAsync(Guid itemId, string filename, byte[]? extraData = null)
    {
        var content = extraData == null
            ? ValidJpegBytes
            : ValidJpegBytes.Concat(extraData).ToArray();

        var sessionReq = new CreateUploadSessionRequest(
            "item",
            itemId,
            null,
            new List<FileSlotRequest> { new(filename, "image/jpeg", content.Length) });

        var sessionMsg = CreateRequest(HttpMethod.Post, "/api/v1/files/upload-sessions", idempotencyKey: $"key-{Guid.NewGuid():N}");
        sessionMsg.Content = JsonContent.Create(sessionReq);
        var sessionRes = await _client.SendAsync(sessionMsg);
        Assert.Equal(HttpStatusCode.Created, sessionRes.StatusCode);
        var session = await sessionRes.Content.ReadFromJsonAsync<CreateUploadSessionResponse>();

        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        form.Add(fileContent, session!.Slots[0].SlotId.ToString(), filename);

        var compMsg = CreateRequest(HttpMethod.Post, $"/api/v1/files/upload-sessions/{session.SessionId}/complete");
        compMsg.Content = form;
        var compRes = await _client.SendAsync(compMsg);
        var compBody = await compRes.Content.ReadAsStringAsync();
        Assert.True(compRes.StatusCode == HttpStatusCode.OK, $"Complete failed with {compRes.StatusCode}: {compBody}");
        var compData = await compRes.Content.ReadFromJsonAsync<CompleteUploadSessionResponse>();
        return compData!.Files[0].FileId;
    }

    [Fact]
    public async Task AttachImage_Valid_SetsFirstImageAsPrimary()
    {
        var itemId = await CreateTestItemAsync();
        var fileId = await UploadFileForItemAsync(itemId, "panel1.jpg");

        var attachReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images");
        attachReq.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId,
            Role = "gallery",
            IsPrimary = false, // First image should automatically become primary
            AltText = new LocalizedTextInput { Thai = "รูปแผงโซลาร์ 1", English = "Solar Panel 1" }
        });

        var attachRes = await _client.SendAsync(attachReq);
        Assert.Equal(HttpStatusCode.Created, attachRes.StatusCode);

        var img = await attachRes.Content.ReadFromJsonAsync<ItemImageDetailResponse>();
        Assert.NotNull(img);
        Assert.True(img.IsPrimary);
        Assert.Equal("primary", img.Role);
        Assert.Equal(0, img.DisplayOrder);
        Assert.Equal("panel1.jpg", img.FileName);

        // Verify Get Item reflects primary image
        var getReq = CreateRequest(HttpMethod.Get, $"/api/v1/items/{itemId}");
        var getRes = await _client.SendAsync(getReq);
        var item = await getRes.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(item!.PrimaryImage);
        Assert.Equal(img.Id, item.PrimaryImage.Id);
    }

    [Fact]
    public async Task SetPrimaryImage_UpdatesPrimaryStatus_AtomicallyUnsetsPrevious()
    {
        var itemId = await CreateTestItemAsync();
        var fileId1 = await UploadFileForItemAsync(itemId, "photo1.jpg");
        var fileId2 = await UploadFileForItemAsync(itemId, "photo2.jpg");

        // Attach image 1 (becomes primary)
        var attachReq1 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images");
        attachReq1.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId1,
            Role = "primary",
            IsPrimary = true,
            AltText = new LocalizedTextInput { Thai = "รูป 1", English = "Photo 1" }
        });
        var attachRes1 = await _client.SendAsync(attachReq1);
        var img1 = await attachRes1.Content.ReadFromJsonAsync<ItemImageDetailResponse>();

        // Attach image 2 (gallery)
        var attachReq2 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images");
        attachReq2.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId2,
            Role = "gallery",
            IsPrimary = false,
            AltText = new LocalizedTextInput { Thai = "รูป 2", English = "Photo 2" }
        });
        var attachRes2 = await _client.SendAsync(attachReq2);
        var img2 = await attachRes2.Content.ReadFromJsonAsync<ItemImageDetailResponse>();
        Assert.False(img2!.IsPrimary);

        // Set image 2 as primary
        var setPrimaryReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images/{img2.Id}/primary");
        var setPrimaryRes = await _client.SendAsync(setPrimaryReq);
        Assert.Equal(HttpStatusCode.OK, setPrimaryRes.StatusCode);
        var updatedImg2 = await setPrimaryRes.Content.ReadFromJsonAsync<ItemImageDetailResponse>();
        Assert.True(updatedImg2!.IsPrimary);

        // List images and verify image 1 is no longer primary
        var listReq = CreateRequest(HttpMethod.Get, $"/api/v1/items/{itemId}/images");
        var listRes = await _client.SendAsync(listReq);
        var images = await listRes.Content.ReadFromJsonAsync<List<ItemImageDetailResponse>>();
        Assert.Equal(2, images!.Count);

        var listImg1 = images.First(i => i.Id == img1!.Id);
        var listImg2 = images.First(i => i.Id == img2.Id);
        Assert.False(listImg1.IsPrimary);
        Assert.True(listImg2.IsPrimary);
    }

    [Fact]
    public async Task DetachImage_DeactivatesAndPromotesNextActiveIfPrimary()
    {
        var itemId = await CreateTestItemAsync();
        var fileId1 = await UploadFileForItemAsync(itemId, "img1.jpg");
        var fileId2 = await UploadFileForItemAsync(itemId, "img2.jpg");

        // Attach 1 and 2
        var req1 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images");
        req1.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId1,
            Role = "primary",
            IsPrimary = true,
            AltText = new LocalizedTextInput { Thai = "1" }
        });
        var res1 = await _client.SendAsync(req1);
        var img1 = await res1.Content.ReadFromJsonAsync<ItemImageDetailResponse>();

        var req2 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images");
        req2.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId2,
            Role = "gallery",
            IsPrimary = false,
            AltText = new LocalizedTextInput { Thai = "2" }
        });
        var res2 = await _client.SendAsync(req2);
        var img2 = await res2.Content.ReadFromJsonAsync<ItemImageDetailResponse>();

        // Detach image 1 (which was primary)
        var detachReq = CreateRequest(HttpMethod.Delete, $"/api/v1/items/{itemId}/images/{img1!.Id}");
        var detachRes = await _client.SendAsync(detachReq);
        Assert.Equal(HttpStatusCode.NoContent, detachRes.StatusCode);

        // List should now only contain img2 and it should be promoted to primary
        var listReq = CreateRequest(HttpMethod.Get, $"/api/v1/items/{itemId}/images");
        var listRes = await _client.SendAsync(listReq);
        var images = await listRes.Content.ReadFromJsonAsync<List<ItemImageDetailResponse>>();
        Assert.NotNull(images);
        Assert.Single(images);
        Assert.Equal(img2!.Id, images[0].Id);
        Assert.True(images[0].IsPrimary);
    }

    [Fact]
    public async Task ReorderImages_UpdatesDisplayOrder()
    {
        var itemId = await CreateTestItemAsync();
        var fileId1 = await UploadFileForItemAsync(itemId, "order1.jpg");
        var fileId2 = await UploadFileForItemAsync(itemId, "order2.jpg");

        var req1 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images");
        req1.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId1,
            AltText = new LocalizedTextInput { Thai = "1" }
        });
        var res1 = await _client.SendAsync(req1);
        var img1 = await res1.Content.ReadFromJsonAsync<ItemImageDetailResponse>();

        var req2 = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images");
        req2.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId2,
            AltText = new LocalizedTextInput { Thai = "2" }
        });
        var res2 = await _client.SendAsync(req2);
        var img2 = await res2.Content.ReadFromJsonAsync<ItemImageDetailResponse>();

        // Reorder to [img2, img1]
        var reorderReq = CreateRequest(HttpMethod.Put, $"/api/v1/items/{itemId}/images/reorder");
        reorderReq.Content = JsonContent.Create(new ReorderItemImagesRequest
        {
            OrderedImageIds = new List<Guid> { img2!.Id, img1!.Id }
        });
        var reorderRes = await _client.SendAsync(reorderReq);
        Assert.Equal(HttpStatusCode.OK, reorderRes.StatusCode);

        var list = await reorderRes.Content.ReadFromJsonAsync<List<ItemImageDetailResponse>>();
        Assert.NotNull(list);
        Assert.Equal(2, list.Count);
        Assert.Equal(img2!.Id, list[0].Id);
        Assert.Equal(0, list[0].DisplayOrder);
        Assert.Equal(img1!.Id, list[1].Id);
        Assert.Equal(1, list[1].DisplayOrder);
    }

    [Fact]
    public async Task AttachImage_WrongOrganization_Returns404()
    {
        var itemId = await CreateTestItemAsync();
        var fileId = await UploadFileForItemAsync(itemId, "cross-org.jpg");

        // Request from Org B credentials to Org A's item
        var attachReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images", "token-org-b", TestOnlyDataSeeder.TestMembershipBId);
        attachReq.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId,
            Role = "gallery",
            AltText = new LocalizedTextInput { Thai = "รูปข้ามองค์กร" }
        });

        var attachRes = await _client.SendAsync(attachReq);
        Assert.Equal(HttpStatusCode.NotFound, attachRes.StatusCode);
    }

    [Fact]
    public async Task AttachImage_WithoutPermission_Returns403()
    {
        var itemId = await CreateTestItemAsync();
        var fileId = await UploadFileForItemAsync(itemId, "no-perm.jpg");

        // Request without items.manage-images permission
        var attachReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images", "token-no-perm", MembershipNoPermId);
        attachReq.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId,
            Role = "gallery",
            AltText = new LocalizedTextInput { Thai = "ไม่มีสิทธิ์" }
        });

        var attachRes = await _client.SendAsync(attachReq);
        Assert.Equal(HttpStatusCode.Forbidden, attachRes.StatusCode);
    }

    [Fact]
    public async Task AttachImage_WrongParentId_Returns409Conflict()
    {
        var item1Id = await CreateTestItemAsync();
        var item2Id = await CreateTestItemAsync();

        // Upload file for Item 1
        var fileId = await UploadFileForItemAsync(item1Id, "item1-file.jpg");

        // Try to attach this file to Item 2 (parent mismatch)
        var attachReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item2Id}/images");
        attachReq.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId,
            Role = "gallery",
            AltText = new LocalizedTextInput { Thai = "ไฟล์ผิด item" }
        });

        var attachRes = await _client.SendAsync(attachReq);
        Assert.Equal(HttpStatusCode.Conflict, attachRes.StatusCode);
    }

    [Fact]
    public async Task DetachImage_PreservesUploadedFileBinaryRetention()
    {
        var itemId = await CreateTestItemAsync();
        var fileId = await UploadFileForItemAsync(itemId, "retain.jpg");

        var attachReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{itemId}/images");
        attachReq.Content = JsonContent.Create(new AttachItemImageRequest
        {
            FileId = fileId,
            Role = "primary",
            IsPrimary = true,
            AltText = new LocalizedTextInput { Thai = "รูปเก็บไฟล์" }
        });
        var attachRes = await _client.SendAsync(attachReq);
        var img = await attachRes.Content.ReadFromJsonAsync<ItemImageDetailResponse>();

        // Detach image
        var detachReq = CreateRequest(HttpMethod.Delete, $"/api/v1/items/{itemId}/images/{img!.Id}");
        var detachRes = await _client.SendAsync(detachReq);
        Assert.Equal(HttpStatusCode.NoContent, detachRes.StatusCode);

        // Verify UploadedFile is still present in database (not deleted)
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var fileStillExists = await db.UploadedFiles.AnyAsync(f => f.Id == fileId);
        Assert.True(fileStillExists, "UploadedFile must be retained after detaching image");
    }
}
