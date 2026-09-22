using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Api;
using TanErp.Api.Contracts.Items;
using TanErp.Infrastructure.Identity;
using TanErp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class ItemEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    private const string UidA = TestOnlyDataSeeder.TestFirebaseUid;
    private const string UidB = TestOnlyDataSeeder.TestFirebaseUidB;
    private const string UidNoPerm = "uid-no-item-perm";
    private static readonly Guid MembershipNoPermId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b50");

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

        // Seed a user without item permissions in Org A
        var noPermUser = new TanErp.Domain.IdentityAccess.User(Guid.NewGuid(), UidNoPerm, "noperm@example.com", "No Perm User", true);
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
        Guid? ifMatch = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-Membership-Id", (membershipId ?? TestOnlyDataSeeder.TestMembershipId).ToString());

        if (ifMatch.HasValue)
        {
            request.Headers.Add("If-Match", $"\"{ifMatch.Value}\"");
        }

        return request;
    }

    private async Task<(Guid CategoryId, Guid UnitId, Guid? BrandId)> SeedTaxonomyAsync()
    {
        // 1. Create category
        var catReq = CreateRequest(HttpMethod.Post, "/api/v1/item-categories");
        catReq.Content = JsonContent.Create(new CreateItemCategoryRequest
        {
            Code = "CAT-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "หมวดหมู่ทดสอบ", English = "Test Category" },
            AllowedItemTypes = new List<string> { "Standard", "Service" },
            SortOrder = 1
        });
        var catRes = await _client.SendAsync(catReq);
        Assert.Equal(HttpStatusCode.Created, catRes.StatusCode);
        var cat = await catRes.Content.ReadFromJsonAsync<ItemCategoryDetailResponse>();

        // 2. Create unit
        var unitReq = CreateRequest(HttpMethod.Post, "/api/v1/units-of-measure");
        unitReq.Content = JsonContent.Create(new CreateUnitOfMeasureRequest
        {
            Code = "UOM-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "ชิ้น", English = "Piece" },
            Symbol = "pcs",
            Dimension = "Count",
            DecimalScale = 0,
            RoundingMode = "HalfUp"
        });
        var unitRes = await _client.SendAsync(unitReq);
        Assert.Equal(HttpStatusCode.Created, unitRes.StatusCode);
        var unit = await unitRes.Content.ReadFromJsonAsync<UnitOfMeasureDetailResponse>();

        // 3. Create brand
        var brandReq = CreateRequest(HttpMethod.Post, "/api/v1/item-brands");
        brandReq.Content = JsonContent.Create(new CreateItemBrandRequest
        {
            Code = "BRD-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "แบรนด์ทดสอบ", English = "Test Brand" },
            SortOrder = 1
        });
        var brandRes = await _client.SendAsync(brandReq);
        Assert.Equal(HttpStatusCode.Created, brandRes.StatusCode);
        var brand = await brandRes.Content.ReadFromJsonAsync<ItemBrandDetailResponse>();

        return (cat!.Id, unit!.Id, brand!.Id);
    }

    [Fact]
    public async Task CreateItem_WithoutAuth_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/items", new CreateItemRequest());
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateItem_WithoutPermission_Returns403()
    {
        var request = CreateRequest(HttpMethod.Post, "/api/v1/items", "token-no-perm", MembershipNoPermId);
        request.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "NO-PERM-ITEM",
            ItemType = "material",
            CategoryId = Guid.NewGuid(),
            BaseUnitId = Guid.NewGuid(),
            Name = new LocalizedTextInput { Thai = "สินค้า", English = "Item" }
        });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateItem_Valid_Returns201_AndCreatesAuditEvent()
    {
        var (categoryId, unitId, brandId) = await SeedTaxonomyAsync();

        var itemCode = "ITEM-" + Guid.NewGuid().ToString("N")[..6];
        var request = CreateRequest(HttpMethod.Post, "/api/v1/items");
        request.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = itemCode,
            ItemType = "material",
            CategoryId = categoryId,
            BrandId = brandId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "แผงโซลาร์เซลล์ 550W", English = "Solar Panel 550W" },
            Description = new LocalizedTextInput { Thai = "โมโนคริสตัลไลน์ ประสิทธิภาพสูง", English = "Mono-crystalline high efficiency" },
            AvailabilityMode = "all_branches",
            Capabilities = new ItemCapabilitiesInput
            {
                CanSell = true,
                CanCost = true,
                CanPurchase = true,
                CanStock = true,
                CanProduce = false
            },
            Aliases = new List<LocalizedTextInput>
            {
                new() { Thai = "โซลาร์ 550W", English = "Solar 550W" }
            }
        });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var item = await response.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(item);
        Assert.Equal(itemCode, item.Code);
        Assert.Equal("draft", item.Status);
        Assert.Equal("แผงโซลาร์เซลล์ 550W", item.Name.Thai);
        Assert.Equal("Solar Panel 550W", item.Name.English);
        Assert.Equal(categoryId, item.Category.Id);
        Assert.Equal(brandId, item.Brand?.Id);
        Assert.Equal(unitId, item.BaseUnit.Id);
        Assert.Single(item.Aliases);
        Assert.NotNull(response.Headers.ETag);

        // Verify Audit Event
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var audit = await db.AuditEvents.FirstOrDefaultAsync(a => a.ResourceId == item.Id.ToString() && a.Action == "items.create");
        Assert.NotNull(audit);
        Assert.Equal("item", audit.ResourceType);
    }

    [Fact]
    public async Task GetItem_CrossOrg_Returns404()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();

        // Create item in Org A
        var createReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "CROSS-ORG-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้าองค์กร A", English = "Org A Item" }
        });
        var createRes = await _client.SendAsync(createReq);
        Assert.Equal(HttpStatusCode.Created, createRes.StatusCode);
        var item = await createRes.Content.ReadFromJsonAsync<ItemResponse>();

        // Query with Org B credentials
        var getReq = CreateRequest(HttpMethod.Get, $"/api/v1/items/{item!.Id}", "token-org-b", TestOnlyDataSeeder.TestMembershipBId);
        var getRes = await _client.SendAsync(getReq);
        Assert.Equal(HttpStatusCode.NotFound, getRes.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_Concurrency_EnforcedWithIfMatch()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();

        var createReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "CONC-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้าทดสอบ Concurrency", English = "Concurrency Item" }
        });
        var createRes = await _client.SendAsync(createReq);
        var item = await createRes.Content.ReadFromJsonAsync<ItemResponse>();

        // 1. Missing If-Match => 428 Precondition Required
        var noIfMatchReq = CreateRequest(HttpMethod.Put, $"/api/v1/items/{item!.Id}");
        noIfMatchReq.Content = JsonContent.Create(new UpdateItemRequest
        {
            Code = item.Code,
            ItemType = item.ItemType,
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "ชื่อใหม่", English = "New Name" }
        });
        var noIfMatchRes = await _client.SendAsync(noIfMatchReq);
        Assert.Equal(HttpStatusCode.PreconditionRequired, noIfMatchRes.StatusCode);

        // 2. Outdated/Mismatch If-Match => 409 Conflict
        var badIfMatchReq = CreateRequest(HttpMethod.Put, $"/api/v1/items/{item.Id}", ifMatch: Guid.NewGuid());
        badIfMatchReq.Content = JsonContent.Create(new UpdateItemRequest
        {
            Code = item.Code,
            ItemType = item.ItemType,
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "ชื่อใหม่", English = "New Name" }
        });
        var badIfMatchRes = await _client.SendAsync(badIfMatchReq);
        Assert.Equal(HttpStatusCode.Conflict, badIfMatchRes.StatusCode);

        // 3. Valid If-Match => 200 OK
        var validReq = CreateRequest(HttpMethod.Put, $"/api/v1/items/{item.Id}", ifMatch: item.RowVersion);
        validReq.Content = JsonContent.Create(new UpdateItemRequest
        {
            Code = item.Code,
            ItemType = item.ItemType,
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "ชื่อที่แก้ไขแล้ว", English = "Updated Name" }
        });
        var validRes = await _client.SendAsync(validReq);
        Assert.Equal(HttpStatusCode.OK, validRes.StatusCode);
        var updated = await validRes.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.Equal("ชื่อที่แก้ไขแล้ว", updated!.Name.Thai);
        Assert.NotEqual(item.RowVersion, updated.RowVersion);
    }

    [Fact]
    public async Task Item_Lifecycle_ActivateAndDeactivate()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();

        var createReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "LIFE-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้าทดสอบ Lifecycle", English = "Lifecycle Item" }
        });
        var createRes = await _client.SendAsync(createReq);
        var item = await createRes.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.Equal("draft", item!.Status);

        // 1. Activate
        var actReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/activate", ifMatch: item.RowVersion);
        var actRes = await _client.SendAsync(actReq);
        Assert.Equal(HttpStatusCode.OK, actRes.StatusCode);
        var activated = await actRes.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.Equal("active", activated!.Status);
        Assert.True(activated.ActivatedOnce);

        // 2. Deactivate without reason code => 422
        var badDeactReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/deactivate", ifMatch: activated.RowVersion);
        badDeactReq.Content = JsonContent.Create(new DeactivateItemRequest { ReasonCode = "" });
        var badDeactRes = await _client.SendAsync(badDeactReq);
        Assert.Equal(HttpStatusCode.BadRequest, badDeactRes.StatusCode); // Model validation catches empty ReasonCode

        // 3. Deactivate with valid reason code => 200 OK
        var deactReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/deactivate", ifMatch: activated.RowVersion);
        deactReq.Content = JsonContent.Create(new DeactivateItemRequest { ReasonCode = "DISCONTINUED", Reason = "ยกเลิกการผลิตแล้ว" });
        var deactRes = await _client.SendAsync(deactReq);
        Assert.Equal(HttpStatusCode.OK, deactRes.StatusCode);
        var deactivated = await deactRes.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.Equal("inactive", deactivated!.Status);
        Assert.Equal("DISCONTINUED", deactivated.InactiveReasonCode);
    }

    [Fact]
    public async Task Item_BranchAvailability_Update()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();

        var createReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "BR-AVAIL-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้าเฉพาะสาขา", English = "Branch Restricted Item" }
        });
        var createRes = await _client.SendAsync(createReq);
        var item = await createRes.Content.ReadFromJsonAsync<ItemResponse>();

        var branchReq = CreateRequest(HttpMethod.Put, $"/api/v1/items/{item!.Id}/branch-availability", ifMatch: item.RowVersion);
        branchReq.Content = JsonContent.Create(new SetBranchAvailabilityRequest
        {
            Mode = "selected_branches",
            BranchIds = new List<Guid> { TestOnlyDataSeeder.TestBranchId }
        });
        var branchRes = await _client.SendAsync(branchReq);
        Assert.Equal(HttpStatusCode.OK, branchRes.StatusCode);

        var updated = await branchRes.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.Equal("selected_branches", updated!.AvailabilityMode);
        Assert.Single(updated.BranchAvailabilities);
        Assert.Equal(TestOnlyDataSeeder.TestBranchId, updated.BranchAvailabilities[0].BranchId);
    }

    [Fact]
    public async Task Item_Aliases_AddAndRemove()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();

        var createReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "ALIAS-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้าทดสอบ Alias", English = "Alias Test Item" }
        });
        var createRes = await _client.SendAsync(createReq);
        var item = await createRes.Content.ReadFromJsonAsync<ItemResponse>();

        // Add alias
        var addAliasReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item!.Id}/aliases");
        addAliasReq.Content = JsonContent.Create(new AddAliasRequest
        {
            Alias = new LocalizedTextInput { Thai = "ชื่อเล่นสินค้า", English = "Item Nickname" }
        });
        var addAliasRes = await _client.SendAsync(addAliasReq);
        Assert.Equal(HttpStatusCode.OK, addAliasRes.StatusCode);
        var itemWithAlias = await addAliasRes.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.Single(itemWithAlias!.Aliases);
        var aliasId = itemWithAlias.Aliases[0].Id;

        // Remove alias
        var delAliasReq = CreateRequest(HttpMethod.Delete, $"/api/v1/items/{item.Id}/aliases/{aliasId}");
        var delAliasRes = await _client.SendAsync(delAliasReq);
        Assert.Equal(HttpStatusCode.OK, delAliasRes.StatusCode);
        var itemAfterDel = await delAliasRes.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.Empty(itemAfterDel!.Aliases);
    }
}
