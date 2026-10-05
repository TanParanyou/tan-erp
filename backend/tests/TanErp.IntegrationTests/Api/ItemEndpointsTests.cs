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
using TanErp.Api.Contracts.Items;
using TanErp.Api.ErrorHandling;
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

        if (method == HttpMethod.Post && url is "/api/v1/items" or "/api/v1/item-categories" or "/api/v1/item-brands" or "/api/v1/item-tax-categories" or "/api/v1/units-of-measure")
        {
            request.Headers.Add("Idempotency-Key", idempotencyKey ?? Guid.NewGuid().ToString("N"));
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
        var idempotencyKey = $"item-create-{Guid.NewGuid():N}";
        var payload = new CreateItemRequest
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
        };
        var request = CreateRequest(HttpMethod.Post, "/api/v1/items", idempotencyKey: idempotencyKey);
        request.Content = JsonContent.Create(payload);

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
        Assert.Equal(1, await db.AuditEvents.CountAsync(a => a.ResourceId == item.Id.ToString() && a.Action == "items.create"));

        var replayRequest = CreateRequest(HttpMethod.Post, "/api/v1/items", idempotencyKey: idempotencyKey);
        replayRequest.Content = JsonContent.Create(payload);
        var replayResponse = await _client.SendAsync(replayRequest);
        Assert.Equal(HttpStatusCode.Created, replayResponse.StatusCode);
        var replayed = await replayResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.Equal(item.Id, replayed!.Id);

        var changedPayload = new CreateItemRequest
        {
            Code = itemCode + "-OTHER",
            ItemType = payload.ItemType,
            CategoryId = payload.CategoryId,
            BrandId = payload.BrandId,
            BaseUnitId = payload.BaseUnitId,
            Name = payload.Name,
            Description = payload.Description,
            AvailabilityMode = payload.AvailabilityMode,
            Capabilities = payload.Capabilities,
            Aliases = payload.Aliases,
        };
        var conflictRequest = CreateRequest(HttpMethod.Post, "/api/v1/items", idempotencyKey: idempotencyKey);
        conflictRequest.Content = JsonContent.Create(changedPayload);
        var conflictResponse = await _client.SendAsync(conflictRequest);
        Assert.Equal(HttpStatusCode.Conflict, conflictResponse.StatusCode);
    }

    [Fact]
    public async Task CreateMasterData_WithoutCodes_GeneratesCodesWithinSuccessfulCreateFlow()
    {
        var categoryRequest = CreateRequest(HttpMethod.Post, "/api/v1/item-categories");
        categoryRequest.Content = JsonContent.Create(new CreateItemCategoryRequest { Name = new LocalizedTextInput { Thai = "GEN category" } });
        using var categoryResponse = await _client.SendAsync(categoryRequest);
        Assert.Equal(HttpStatusCode.Created, categoryResponse.StatusCode);
        var category = await categoryResponse.Content.ReadFromJsonAsync<ItemCategoryDetailResponse>();
        Assert.NotNull(category);
        Assert.StartsWith("CAT-", category.Code);

        var brandRequest = CreateRequest(HttpMethod.Post, "/api/v1/item-brands");
        brandRequest.Content = JsonContent.Create(new CreateItemBrandRequest { Name = new LocalizedTextInput { Thai = "GEN brand" } });
        using var brandResponse = await _client.SendAsync(brandRequest);
        Assert.Equal(HttpStatusCode.Created, brandResponse.StatusCode);
        var brand = await brandResponse.Content.ReadFromJsonAsync<ItemBrandDetailResponse>();
        Assert.NotNull(brand);
        Assert.StartsWith("BRD-", brand.Code);

        var unitRequest = CreateRequest(HttpMethod.Post, "/api/v1/units-of-measure");
        unitRequest.Content = JsonContent.Create(new CreateUnitOfMeasureRequest
        {
            Name = new LocalizedTextInput { Thai = "GEN unit" }, Symbol = "gen", Dimension = "count", DecimalScale = 0
        });
        using var unitResponse = await _client.SendAsync(unitRequest);
        Assert.Equal(HttpStatusCode.Created, unitResponse.StatusCode);
        var unit = await unitResponse.Content.ReadFromJsonAsync<UnitOfMeasureDetailResponse>();
        Assert.NotNull(unit);
        Assert.StartsWith("UOM-", unit.Code);

        var taxRequest = CreateRequest(HttpMethod.Post, "/api/v1/item-tax-categories");
        taxRequest.Content = JsonContent.Create(new CreateItemTaxCategoryRequest { Name = new LocalizedTextInput { Thai = "GEN tax" } });
        using var taxResponse = await _client.SendAsync(taxRequest);
        Assert.Equal(HttpStatusCode.Created, taxResponse.StatusCode);
        var taxCategory = await taxResponse.Content.ReadFromJsonAsync<ItemTaxCategoryDetailResponse>();
        Assert.NotNull(taxCategory);
        Assert.StartsWith("TAX-", taxCategory.Code);

        var itemRequest = CreateRequest(HttpMethod.Post, "/api/v1/items");
        itemRequest.Content = JsonContent.Create(new CreateItemRequest
        {
            ItemType = "material", CategoryId = category.Id, BaseUnitId = unit.Id,
            Name = new LocalizedTextInput { Thai = "GEN item" }, AvailabilityMode = "all_branches"
        });
        using var itemResponse = await _client.SendAsync(itemRequest);
        Assert.Equal(HttpStatusCode.Created, itemResponse.StatusCode);
        var item = await itemResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(item);
        Assert.StartsWith("ITM-", item.Code);

        var duplicateItemRequest = CreateRequest(HttpMethod.Post, "/api/v1/items");
        duplicateItemRequest.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = item.Code, ItemType = "material", CategoryId = category.Id, BaseUnitId = unit.Id,
            Name = new LocalizedTextInput { Thai = "รหัสซ้ำ" }, AvailabilityMode = "all_branches"
        });
        using var duplicateItemResponse = await _client.SendAsync(duplicateItemRequest);
        Assert.Equal(HttpStatusCode.Conflict, duplicateItemResponse.StatusCode);
        var duplicateProblem = await duplicateItemResponse.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("ITEM_CODE_CONFLICT", duplicateProblem?.Code);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var audit = await db.AuditEvents.SingleAsync(entry => entry.ResourceType == "item" && entry.ResourceId == item.Id.ToString());
        using var auditJson = JsonDocument.Parse(audit.ChangesJson);
        Assert.True(auditJson.RootElement.GetProperty("codeGenerated").GetBoolean());
    }

    [Fact]
    public async Task ListItems_SortsWithStablePaging_AndRejectsUnknownSortKeys()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();

        async Task CreateItem(string code, string itemType)
        {
            var request = CreateRequest(HttpMethod.Post, "/api/v1/items");
            request.Content = JsonContent.Create(new CreateItemRequest
            {
                Code = code,
                ItemType = itemType,
                CategoryId = categoryId,
                BaseUnitId = unitId,
                Name = new LocalizedTextInput { Thai = code, English = code },
                AvailabilityMode = "all_branches",
                Capabilities = new ItemCapabilitiesInput
                {
                    CanCost = true,
                    CanPurchase = itemType == "material",
                    CanSell = false,
                    CanStock = false,
                    CanProduce = false
                }
            });
            var created = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        }

        await CreateItem("SORT-A", "material");
        await CreateItem("SORT-Z", "product");

        var sortedRequest = CreateRequest(HttpMethod.Get, "/api/v1/items?sortBy=itemType&sortOrder=desc&pageNumber=1&pageSize=1000");
        var sortedResponse = await _client.SendAsync(sortedRequest);
        Assert.Equal(HttpStatusCode.OK, sortedResponse.StatusCode);
        var sorted = await sortedResponse.Content.ReadFromJsonAsync<PagedItemsResponse>();
        Assert.NotNull(sorted);
        Assert.Equal(100, sorted.PageSize);
        Assert.Equal(new[] { "product", "material" }, sorted.Items.Select(item => item.ItemType));

        var invalidSortRequest = CreateRequest(HttpMethod.Get, "/api/v1/items?sortBy=name");
        var invalidSortResponse = await _client.SendAsync(invalidSortRequest);
        Assert.Equal(HttpStatusCode.BadRequest, invalidSortResponse.StatusCode);
        var invalidSort = await invalidSortResponse.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("ITEM_SORT_INVALID", invalidSort?.Code);

        var invalidOrderRequest = CreateRequest(HttpMethod.Get, "/api/v1/items?sortOrder=sideways");
        var invalidOrderResponse = await _client.SendAsync(invalidOrderRequest);
        Assert.Equal(HttpStatusCode.BadRequest, invalidOrderResponse.StatusCode);
        var invalidOrder = await invalidOrderResponse.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.Equal("ITEM_SORT_ORDER_INVALID", invalidOrder?.Code);
    }

    [Fact]
    public async Task Barcode_CreateReplayScanAndDeactivate_RespectsOrganizationAndUnit()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();
        var createItem = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createItem.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "BAR-" + Guid.NewGuid().ToString("N")[..8],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "แผ่นทดสอบ", English = "Test sheet" }
        });
        var createItemResponse = await _client.SendAsync(createItem);
        Assert.Equal(HttpStatusCode.Created, createItemResponse.StatusCode);
        var item = await createItemResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(item);

        var payload = new CreateItemBarcodeRequest
        {
            IdentifierType = "gtin", Value = "6291041500213", UnitId = unitId,
            QuantityInBaseUnit = "12.0000", PackagingLevel = "case", IsPrimary = true
        };
        var key = $"barcode-create-{Guid.NewGuid():N}";
        HttpRequestMessage BarcodeCreate(CreateItemBarcodeRequest body)
        {
            var request = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/barcodes");
            request.Headers.Add("Idempotency-Key", key);
            request.Content = JsonContent.Create(body);
            return request;
        }

        var createdResponse = await _client.SendAsync(BarcodeCreate(payload));
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        var barcode = await createdResponse.Content.ReadFromJsonAsync<ItemBarcodeResponse>();
        Assert.NotNull(barcode);
        Assert.Equal("12.0000", barcode.QuantityInBaseUnit);
        Assert.Equal(item.Id, barcode.Item.Id);
        Assert.Equal(unitId, barcode.Unit.Id);

        var replayResponse = await _client.SendAsync(BarcodeCreate(payload));
        Assert.Equal(HttpStatusCode.Created, replayResponse.StatusCode);
        Assert.Equal(barcode.Id, (await replayResponse.Content.ReadFromJsonAsync<ItemBarcodeResponse>())!.Id);

        var changed = new CreateItemBarcodeRequest
        {
            IdentifierType = "gtin", Value = "6291041500213", UnitId = unitId,
            QuantityInBaseUnit = "1.0000", PackagingLevel = "case", IsPrimary = true
        };
        Assert.Equal(HttpStatusCode.Conflict, (await _client.SendAsync(BarcodeCreate(changed))).StatusCode);

        var duplicate = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/barcodes");
        duplicate.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        duplicate.Content = JsonContent.Create(new CreateItemBarcodeRequest
        {
            IdentifierType = "gtin", Value = "06291041500213", UnitId = unitId,
            QuantityInBaseUnit = "12.0000", PackagingLevel = "case"
        });
        Assert.Equal(HttpStatusCode.Conflict, (await _client.SendAsync(duplicate)).StatusCode);

        var wrongUnit = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/barcodes");
        wrongUnit.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        wrongUnit.Content = JsonContent.Create(new CreateItemBarcodeRequest
        {
            IdentifierType = "internal", Value = "SHEET-CASE", UnitId = Guid.NewGuid(),
            QuantityInBaseUnit = "12.0000", PackagingLevel = "case"
        });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await _client.SendAsync(wrongUnit)).StatusCode);

        var malformedGtin = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/barcodes");
        malformedGtin.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        malformedGtin.Content = JsonContent.Create(new CreateItemBarcodeRequest
        {
            IdentifierType = "gtin", Value = "6291041500214", UnitId = unitId,
            QuantityInBaseUnit = "1.0000", PackagingLevel = "each"
        });
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await _client.SendAsync(malformedGtin)).StatusCode);

        var leadingZero = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/barcodes");
        leadingZero.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        leadingZero.Content = JsonContent.Create(new CreateItemBarcodeRequest
        {
            IdentifierType = "internal", Value = "0000123", UnitId = unitId,
            QuantityInBaseUnit = "1.0000", PackagingLevel = "inner"
        });
        var leadingZeroResponse = await _client.SendAsync(leadingZero);
        Assert.Equal(HttpStatusCode.Created, leadingZeroResponse.StatusCode);
        Assert.Equal("0000123", (await leadingZeroResponse.Content.ReadFromJsonAsync<ItemBarcodeResponse>())!.Value);

        var stalePrimary = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/barcodes/{barcode.Id}/primary", ifMatch: Guid.NewGuid());
        Assert.Equal(HttpStatusCode.Conflict, (await _client.SendAsync(stalePrimary)).StatusCode);

        var crossOrg = CreateRequest(HttpMethod.Get, $"/api/v1/items/{item.Id}/barcodes", "token-org-b", TestOnlyDataSeeder.TestMembershipBId);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.SendAsync(crossOrg)).StatusCode);

        var activate = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/activate", ifMatch: item.RowVersion);
        var activateResponse = await _client.SendAsync(activate);
        Assert.Equal(HttpStatusCode.OK, activateResponse.StatusCode);

        var lookup = CreateRequest(HttpMethod.Get, "/api/v1/items/by-barcode?value=06291041500213");
        var lookupResponse = await _client.SendAsync(lookup);
        Assert.Equal(HttpStatusCode.OK, lookupResponse.StatusCode);
        Assert.Equal(barcode.Id, (await lookupResponse.Content.ReadFromJsonAsync<ItemBarcodeResponse>())!.Id);

        var deactivate = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/barcodes/{barcode.Id}/deactivate", ifMatch: barcode.RowVersion);
        Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(deactivate)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.SendAsync(CreateRequest(HttpMethod.Get, "/api/v1/items/by-barcode?value=6291041500213"))).StatusCode);

        var reuseInactiveValue = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/barcodes");
        reuseInactiveValue.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        reuseInactiveValue.Content = JsonContent.Create(new CreateItemBarcodeRequest
        {
            IdentifierType = "gtin", Value = "6291041500213", UnitId = unitId,
            QuantityInBaseUnit = "12.0000", PackagingLevel = "case"
        });
        Assert.Equal(HttpStatusCode.Conflict, (await _client.SendAsync(reuseInactiveValue)).StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal(1, await db.AuditEvents.CountAsync(a => a.ResourceId == barcode.Id.ToString() && a.Action == "item-barcode.created"));
    }

    [Fact]
    public async Task ItemUnitConversion_RequiresEffectiveConversionAndSnapshotsBarcodeQuantity()
    {
        var (categoryId, baseUnitId, _) = await SeedTaxonomyAsync();
        var itemRequest = CreateRequest(HttpMethod.Post, "/api/v1/items");
        itemRequest.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "CONV-" + Guid.NewGuid().ToString("N")[..8],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = baseUnitId,
            Name = new LocalizedTextInput { Thai = "สินค้าแปลงหน่วย", English = "Converted item" }
        });
        var itemResponse = await _client.SendAsync(itemRequest);
        Assert.Equal(HttpStatusCode.Created, itemResponse.StatusCode);
        var item = (await itemResponse.Content.ReadFromJsonAsync<ItemResponse>())!;

        var alternateUnitRequest = CreateRequest(HttpMethod.Post, "/api/v1/units-of-measure");
        alternateUnitRequest.Content = JsonContent.Create(new CreateUnitOfMeasureRequest
        {
            Code = "BOX-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "กล่อง", English = "Box" },
            Symbol = "box",
            Dimension = "Count",
            DecimalScale = 0,
            RoundingMode = "HalfUp"
        });
        var alternateUnitResponse = await _client.SendAsync(alternateUnitRequest);
        Assert.Equal(HttpStatusCode.Created, alternateUnitResponse.StatusCode);
        var alternateUnit = (await alternateUnitResponse.Content.ReadFromJsonAsync<UnitOfMeasureDetailResponse>())!;

        var barcodeValue = "CONV-" + Guid.NewGuid().ToString("N")[..12];
        HttpRequestMessage CreateBarcode(Guid selectedUnitId, string quantity)
        {
            var request = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/barcodes");
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
            request.Content = JsonContent.Create(new CreateItemBarcodeRequest
            {
                IdentifierType = "internal", Value = barcodeValue, UnitId = selectedUnitId,
                QuantityInBaseUnit = quantity, PackagingLevel = "case"
            });
            return request;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await _client.SendAsync(CreateBarcode(alternateUnit.Id, "12.0000"))).StatusCode);
        var conversionPayload = new CreateItemUnitConversionRequest
        {
            FromUnitId = alternateUnit.Id,
            ToUnitId = baseUnitId,
            Factor = "12",
            EffectiveFrom = today,
            EffectiveTo = today,
            Reason = "Package contains twelve base units"
        };
        HttpRequestMessage CreateConversion(CreateItemUnitConversionRequest body)
        {
            var request = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/unit-conversions");
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
            request.Content = JsonContent.Create(body);
            return request;
        }

        CreateItemUnitConversionRequest InvalidConversion(Guid fromUnitId, string factor) => new()
        {
            FromUnitId = fromUnitId,
            ToUnitId = baseUnitId,
            Factor = factor,
            EffectiveFrom = today,
            Reason = "Invalid conversion test"
        };
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await _client.SendAsync(CreateConversion(InvalidConversion(alternateUnit.Id, "0")))).StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await _client.SendAsync(CreateConversion(InvalidConversion(baseUnitId, "1")))).StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await _client.SendAsync(CreateConversion(InvalidConversion(Guid.NewGuid(), "12")))).StatusCode);

        var conversionResponse = await _client.SendAsync(CreateConversion(conversionPayload));
        Assert.Equal(HttpStatusCode.Created, conversionResponse.StatusCode);
        var conversion = (await conversionResponse.Content.ReadFromJsonAsync<ItemUnitConversionResponse>())!;
        Assert.Equal("12", conversion.Factor);
        Assert.Equal(baseUnitId, conversion.ToUnitId);
        var conversionList = await _client.SendAsync(CreateRequest(HttpMethod.Get, $"/api/v1/items/{item.Id}/unit-conversions"));
        Assert.Equal(HttpStatusCode.OK, conversionList.StatusCode);
        Assert.Contains(await conversionList.Content.ReadFromJsonAsync<List<ItemUnitConversionResponse>>() ?? [], row => row.Id == conversion.Id);

        Assert.Equal(HttpStatusCode.Conflict, (await _client.SendAsync(CreateConversion(conversionPayload))).StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await _client.SendAsync(CreateBarcode(alternateUnit.Id, "24.0000"))).StatusCode);

        var expiredUnitRequest = CreateRequest(HttpMethod.Post, "/api/v1/units-of-measure");
        expiredUnitRequest.Content = JsonContent.Create(new CreateUnitOfMeasureRequest
        {
            Code = "OLD-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "หน่วยเดิม", English = "Old unit" },
            Symbol = "old",
            Dimension = "Count",
            DecimalScale = 0,
            RoundingMode = "HalfUp"
        });
        var expiredUnitResponse = await _client.SendAsync(expiredUnitRequest);
        Assert.Equal(HttpStatusCode.Created, expiredUnitResponse.StatusCode);
        var expiredUnit = (await expiredUnitResponse.Content.ReadFromJsonAsync<UnitOfMeasureDetailResponse>())!;
        var expiredConversion = await _client.SendAsync(CreateConversion(new CreateItemUnitConversionRequest
        {
            FromUnitId = expiredUnit.Id,
            ToUnitId = baseUnitId,
            Factor = "1",
            EffectiveFrom = today.AddDays(-7),
            EffectiveTo = today.AddDays(-1),
            Reason = "Expired package definition"
        }));
        Assert.Equal(HttpStatusCode.Created, expiredConversion.StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await _client.SendAsync(CreateBarcode(expiredUnit.Id, "1.0000"))).StatusCode);

        var nextVersion = await _client.SendAsync(CreateConversion(new CreateItemUnitConversionRequest
        {
            FromUnitId = alternateUnit.Id,
            ToUnitId = baseUnitId,
            Factor = "24",
            EffectiveFrom = today.AddDays(1),
            Reason = "Next package version"
        }));
        Assert.Equal(HttpStatusCode.Created, nextVersion.StatusCode);

        var barcodeResponse = await _client.SendAsync(CreateBarcode(alternateUnit.Id, "12.0000"));
        Assert.Equal(HttpStatusCode.Created, barcodeResponse.StatusCode);
        var barcode = (await barcodeResponse.Content.ReadFromJsonAsync<ItemBarcodeResponse>())!;
        Assert.Equal("12.0000", barcode.QuantityInBaseUnit);
        Assert.Equal(alternateUnit.Id, barcode.Unit.Id);

        var activate = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/activate", ifMatch: item.RowVersion);
        Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(activate)).StatusCode);

        var scanResponse = await _client.SendAsync(CreateRequest(HttpMethod.Get, $"/api/v1/items/by-barcode?value={barcodeValue}"));
        Assert.Equal(HttpStatusCode.OK, scanResponse.StatusCode);
        var scanned = (await scanResponse.Content.ReadFromJsonAsync<ItemBarcodeResponse>())!;
        Assert.Equal("12.0000", scanned.QuantityInBaseUnit);
        Assert.Equal(item.Id, scanned.Item.Id);

        var crossOrg = CreateRequest(HttpMethod.Get, $"/api/v1/items/{item.Id}/unit-conversions", "token-org-b", TestOnlyDataSeeder.TestMembershipBId);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.SendAsync(crossOrg)).StatusCode);
    }

    [Fact]
    public async Task SharedUnitConversion_RejectsCyclesAndScopesByOrganization()
    {
        async Task<Guid> CreateUnit(string suffix)
        {
            var request = CreateRequest(HttpMethod.Post, "/api/v1/units-of-measure");
            request.Content = JsonContent.Create(new CreateUnitOfMeasureRequest
            {
                Code = "SHARED-" + suffix + Guid.NewGuid().ToString("N")[..5],
                Name = new LocalizedTextInput { Thai = "หน่วยทดสอบ", English = "Test unit" },
                Symbol = suffix,
                Dimension = "count",
                DecimalScale = 4,
                RoundingMode = "HalfUp"
            });
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<UnitOfMeasureDetailResponse>())!.Id;
        }

        var unitA = await CreateUnit("a");
        var unitB = await CreateUnit("b");
        var unitC = await CreateUnit("c");
        var effectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow);
        HttpRequestMessage CreateConversion(Guid from, Guid to, string factor)
        {
            var request = CreateRequest(HttpMethod.Post, "/api/v1/unit-conversions");
            request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
            request.Content = JsonContent.Create(new CreateItemUnitConversionRequest
            {
                FromUnitId = from,
                ToUnitId = to,
                Factor = factor,
                EffectiveFrom = effectiveFrom,
                Reason = "Shared exact conversion test"
            });
            return request;
        }

        var first = await _client.SendAsync(CreateConversion(unitA, unitB, "12"));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        var second = await _client.SendAsync(CreateConversion(unitB, unitC, "2"));
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await _client.SendAsync(CreateConversion(unitC, unitA, "0.5"))).StatusCode);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, (await _client.SendAsync(CreateConversion(unitC, unitA, "0"))).StatusCode);

        var list = await _client.SendAsync(CreateRequest(HttpMethod.Get, "/api/v1/unit-conversions"));
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        var rows = await list.Content.ReadFromJsonAsync<List<UnitConversionResponse>>();
        Assert.Contains(rows!, row => row.FromUnitId == unitA && row.ToUnitId == unitB);
        var crossOrg = CreateRequest(HttpMethod.Get, "/api/v1/unit-conversions", "token-org-b", TestOnlyDataSeeder.TestMembershipBId);
        var otherOrganizationRows = await _client.SendAsync(crossOrg);
        Assert.Equal(HttpStatusCode.OK, otherOrganizationRows.StatusCode);
        var otherRows = await otherOrganizationRows.Content.ReadFromJsonAsync<List<UnitConversionResponse>>() ?? [];
        Assert.DoesNotContain(otherRows, row => row.Id == rows!.Single(x => x.FromUnitId == unitA).Id);

        var (categoryId, _, _) = await SeedTaxonomyAsync();
        var itemCreate = CreateRequest(HttpMethod.Post, "/api/v1/items");
        itemCreate.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "SHARED-ITEM-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitB,
            Name = new LocalizedTextInput { Thai = "สินค้าหน่วยกลาง", English = "Shared unit item" }
        });
        var itemCreateResponse = await _client.SendAsync(itemCreate);
        Assert.Equal(HttpStatusCode.Created, itemCreateResponse.StatusCode);
        var item = (await itemCreateResponse.Content.ReadFromJsonAsync<ItemResponse>())!;
        var barcodeValue = "SHARED-" + Guid.NewGuid().ToString("N")[..10];
        var barcodeRequest = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/barcodes");
        barcodeRequest.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString("N"));
        barcodeRequest.Content = JsonContent.Create(new CreateItemBarcodeRequest
        {
            IdentifierType = "internal", Value = barcodeValue, UnitId = unitA,
            QuantityInBaseUnit = "12.0000", PackagingLevel = "case"
        });
        var barcodeResponse = await _client.SendAsync(barcodeRequest);
        Assert.Equal(HttpStatusCode.Created, barcodeResponse.StatusCode);
        var activate = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/activate", ifMatch: item.RowVersion);
        Assert.Equal(HttpStatusCode.OK, (await _client.SendAsync(activate)).StatusCode);
        var scan = await _client.SendAsync(CreateRequest(HttpMethod.Get, $"/api/v1/items/by-barcode?value={barcodeValue}"));
        Assert.Equal(HttpStatusCode.OK, scan.StatusCode);
        Assert.Equal("12.0000", (await scan.Content.ReadFromJsonAsync<ItemBarcodeResponse>())!.QuantityInBaseUnit);
    }

    [Fact]
    public async Task Item_ProductTaxCategoryAndCodeLength_RoundTrip()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();
        var overlongCode = CreateRequest(HttpMethod.Post, "/api/v1/items");
        overlongCode.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = new string('X', 51),
            ItemType = "product",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้า", English = "Product" }
        });
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.SendAsync(overlongCode)).StatusCode);

        var create = CreateRequest(HttpMethod.Post, "/api/v1/items");
        create.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "prd-finished-01",
            ItemType = "product",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "ตู้สำเร็จรูป", English = "Finished cabinet" },
            TaxCategoryCode = " retail "
        });
        var createResponse = await _client.SendAsync(create);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(created);
        Assert.Equal("prd-finished-01", created.Code);
        Assert.Equal("product", created.ItemType);
        Assert.Equal("RETAIL", created.TaxCategoryCode);

        var update = CreateRequest(HttpMethod.Put, $"/api/v1/items/{created.Id}", ifMatch: created.RowVersion);
        update.Content = JsonContent.Create(new UpdateItemRequest
        {
            Code = created.Code,
            ItemType = "product",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "ตู้พร้อมติดตั้ง", English = "Ready-to-install cabinet" },
            TaxCategoryCode = " finished_good "
        });
        var updatedResponse = await _client.SendAsync(update);
        Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);
        var updated = await updatedResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(updated);
        Assert.Equal("product", updated.ItemType);
        Assert.Equal("FINISHED_GOOD", updated.TaxCategoryCode);
        Assert.Equal("prd-finished-01", updated.Code);

        var activate = CreateRequest(HttpMethod.Post, $"/api/v1/items/{created.Id}/activate", ifMatch: updated.RowVersion);
        var activateResponse = await _client.SendAsync(activate);
        Assert.Equal(HttpStatusCode.OK, activateResponse.StatusCode);
        var activated = await activateResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(activated);
        Assert.Equal("active", activated.Status);
        Assert.Equal("FINISHED_GOOD", activated.TaxCategoryCode);
    }

    [Fact]
    public async Task TaxonomyCodes_EnforceFieldCatalogLengths()
    {
        var categoryRequest = CreateRequest(HttpMethod.Post, "/api/v1/item-categories");
        categoryRequest.Content = JsonContent.Create(new CreateItemCategoryRequest
        {
            Code = new string('C', 31),
            Name = new LocalizedTextInput { Thai = "หมวด", English = "Category" }
        });
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.SendAsync(categoryRequest)).StatusCode);

        var brandRequest = CreateRequest(HttpMethod.Post, "/api/v1/item-brands");
        brandRequest.Content = JsonContent.Create(new CreateItemBrandRequest
        {
            Code = new string('B', 31),
            Name = new LocalizedTextInput { Thai = "ยี่ห้อ", English = "Brand" }
        });
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.SendAsync(brandRequest)).StatusCode);

        var unitRequest = CreateRequest(HttpMethod.Post, "/api/v1/units-of-measure");
        unitRequest.Content = JsonContent.Create(new CreateUnitOfMeasureRequest
        {
            Code = new string('U', 21),
            Name = new LocalizedTextInput { Thai = "หน่วย", English = "Unit" },
            Symbol = "u",
            Dimension = "count"
        });
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.SendAsync(unitRequest)).StatusCode);
    }

    [Fact]
    public async Task TaxCategoryMaster_CreatesListsAndUpdatesWithinOrganization()
    {
        var code = "VAT-" + Guid.NewGuid().ToString("N")[..6];
        var create = CreateRequest(HttpMethod.Post, "/api/v1/item-tax-categories");
        create.Content = JsonContent.Create(new CreateItemTaxCategoryRequest
        {
            Code = code,
            Name = new LocalizedTextInput { Thai = "ภาษีมูลค่าเพิ่ม", English = "Value added tax" },
            SortOrder = 2
        });

        var createdResponse = await _client.SendAsync(create);
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        var created = await createdResponse.Content.ReadFromJsonAsync<ItemTaxCategoryDetailResponse>();
        Assert.NotNull(created);
        Assert.Equal(code, created.Code);

        var listResponse = await _client.SendAsync(CreateRequest(HttpMethod.Get, "/api/v1/item-tax-categories"));
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var listed = await listResponse.Content.ReadFromJsonAsync<List<ItemTaxCategoryDetailResponse>>();
        Assert.Contains(listed ?? [], category => category.Id == created.Id);

        var update = CreateRequest(HttpMethod.Put, $"/api/v1/item-tax-categories/{created.Id}", ifMatch: created.RowVersion);
        update.Content = JsonContent.Create(new UpdateItemTaxCategoryRequest
        {
            Code = code,
            Name = new LocalizedTextInput { Thai = "ภาษีขาย", English = "Sales tax" },
            SortOrder = 3
        });
        var updatedResponse = await _client.SendAsync(update);
        Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);
        var updated = await updatedResponse.Content.ReadFromJsonAsync<ItemTaxCategoryDetailResponse>();
        Assert.Equal("ภาษีขาย", updated?.Name.Thai);
        Assert.Equal(3, updated?.SortOrder);
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
    public async Task Item_Update_RoundTripsSelectedBranchesCapabilitiesAttributesAndTaxCategory()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();
        var createRequest = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createRequest.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "ROUNDTRIP-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            AvailabilityMode = "selected_branches",
            SelectedBranchIds = [TestOnlyDataSeeder.TestBranchId],
            TaxCategoryCode = "MATERIAL",
            Capabilities = new ItemCapabilitiesInput { CanSell = false, CanCost = true, CanPurchase = true, CanStock = false, CanProduce = false },
            Attributes = new Dictionary<string, string> { ["thickness"] = "18mm" },
            AttributesSchemaVersion = 1,
            Name = new LocalizedTextInput { Thai = "วัสดุเดิม", English = "Original material" }
        });
        var createdResponse = await _client.SendAsync(createRequest);
        Assert.Equal(HttpStatusCode.Created, createdResponse.StatusCode);
        var created = await createdResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(created);

        var updateRequest = CreateRequest(HttpMethod.Put, $"/api/v1/items/{created.Id}", ifMatch: created.RowVersion);
        updateRequest.Content = JsonContent.Create(new UpdateItemRequest
        {
            Code = created.Code,
            ItemType = created.ItemType,
            CategoryId = categoryId,
            BaseUnitId = unitId,
            AvailabilityMode = "selected_branches",
            SelectedBranchIds = [TestOnlyDataSeeder.TestBranchId],
            TaxCategoryCode = "MATERIAL",
            Capabilities = new ItemCapabilitiesInput { CanSell = false, CanCost = true, CanPurchase = true, CanStock = false, CanProduce = false },
            Attributes = new Dictionary<string, string> { ["thickness"] = "18mm" },
            AttributesSchemaVersion = 1,
            Name = new LocalizedTextInput { Thai = "วัสดุแก้ไขแล้ว", English = "Updated material" }
        });
        var updatedResponse = await _client.SendAsync(updateRequest);
        Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);
        var updated = await updatedResponse.Content.ReadFromJsonAsync<ItemResponse>();
        Assert.NotNull(updated);
        Assert.Equal("selected_branches", updated.AvailabilityMode);
        Assert.Equal("MATERIAL", updated.TaxCategoryCode);
        Assert.Equal("18mm", updated.Attributes!["thickness"]);
        Assert.False(updated.Capabilities.CanSell);
        Assert.True(updated.Capabilities.CanCost);
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

    [Fact]
    public async Task CreateItem_DuplicateCode_Returns409Conflict()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();
        var code = "DUP-" + Guid.NewGuid().ToString("N")[..6];

        var req1 = CreateRequest(HttpMethod.Post, "/api/v1/items");
        req1.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = code,
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้า 1", English = "Item 1" }
        });
        var res1 = await _client.SendAsync(req1);
        Assert.Equal(HttpStatusCode.Created, res1.StatusCode);

        var req2 = CreateRequest(HttpMethod.Post, "/api/v1/items");
        req2.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = code.ToLowerInvariant(), // case-insensitive duplicate
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้า 2", English = "Item 2" }
        });
        var res2 = await _client.SendAsync(req2);
        Assert.Equal(HttpStatusCode.Conflict, res2.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_ActivatedItem_CannotChangeCode_Returns422()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();
        var code = "IMMUT-" + Guid.NewGuid().ToString("N")[..6];

        var createReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = code,
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้าคงรูป", English = "Immutable Item" }
        });
        var createRes = await _client.SendAsync(createReq);
        var item = await createRes.Content.ReadFromJsonAsync<ItemResponse>();

        // Activate item
        var actReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item!.Id}/activate", ifMatch: item.RowVersion);
        var actRes = await _client.SendAsync(actReq);
        Assert.Equal(HttpStatusCode.OK, actRes.StatusCode);
        var activated = await actRes.Content.ReadFromJsonAsync<ItemResponse>();

        // Attempt to change code
        var updateReq = CreateRequest(HttpMethod.Put, $"/api/v1/items/{item.Id}", ifMatch: activated!.RowVersion);
        updateReq.Content = JsonContent.Create(new UpdateItemRequest
        {
            Code = "CHANGED-CODE",
            ItemType = activated.ItemType,
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = activated.Name.Thai, English = activated.Name.English }
        });
        var updateRes = await _client.SendAsync(updateReq);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, updateRes.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_InactiveItem_Returns422()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();

        var createReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "INACT-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้าจะถูกปิด", English = "To be inactive" }
        });
        var createRes = await _client.SendAsync(createReq);
        var item = await createRes.Content.ReadFromJsonAsync<ItemResponse>();

        // Activate then Deactivate
        var actReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item!.Id}/activate", ifMatch: item.RowVersion);
        var actRes = await _client.SendAsync(actReq);
        var activated = await actRes.Content.ReadFromJsonAsync<ItemResponse>();

        var deactReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/deactivate", ifMatch: activated!.RowVersion);
        deactReq.Content = JsonContent.Create(new DeactivateItemRequest { ReasonCode = "OBSOLETE" });
        var deactRes = await _client.SendAsync(deactReq);
        var deactivated = await deactRes.Content.ReadFromJsonAsync<ItemResponse>();

        // Attempt to update inactive item
        var updateReq = CreateRequest(HttpMethod.Put, $"/api/v1/items/{item.Id}", ifMatch: deactivated!.RowVersion);
        updateReq.Content = JsonContent.Create(new UpdateItemRequest
        {
            Code = item.Code,
            ItemType = item.ItemType,
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "แก้ไขชื่อตอน inactive", English = "Edit when inactive" }
        });
        var updateRes = await _client.SendAsync(updateReq);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, updateRes.StatusCode);
    }

    [Fact]
    public async Task ActivateItem_SelectedBranchesWithoutBranches_Returns422()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();

        var createReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "NO-BR-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            AvailabilityMode = "selected_branches",
            SelectedBranchIds = new List<Guid>(), // empty branches
            Name = new LocalizedTextInput { Thai = "ไม่มีสาขา", English = "No branches" }
        });
        var createRes = await _client.SendAsync(createReq);
        var item = await createRes.Content.ReadFromJsonAsync<ItemResponse>();

        var actReq = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item!.Id}/activate", ifMatch: item.RowVersion);
        var actRes = await _client.SendAsync(actReq);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, actRes.StatusCode);
    }

    [Fact]
    public async Task CreateItem_CrossOrgCategory_Returns404()
    {
        var (_, unitId, _) = await SeedTaxonomyAsync();
        var fakeCategoryId = Guid.NewGuid();

        var req = CreateRequest(HttpMethod.Post, "/api/v1/items");
        req.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "CROSS-CAT-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = fakeCategoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "หมวดต่างองค์กร", English = "Cross org cat" }
        });
        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact]
    public async Task Category_CycleDetection_IndirectCycle_Returns422()
    {
        // Create Cat A
        var catAReq = CreateRequest(HttpMethod.Post, "/api/v1/item-categories");
        catAReq.Content = JsonContent.Create(new CreateItemCategoryRequest
        {
            Code = "CYC-A-" + Guid.NewGuid().ToString("N")[..4],
            Name = new LocalizedTextInput { Thai = "หมวด A", English = "Category A" }
        });
        var catARes = await _client.SendAsync(catAReq);
        var catA = await catARes.Content.ReadFromJsonAsync<ItemCategoryDetailResponse>();

        // Create Cat B with Parent = Cat A
        var catBReq = CreateRequest(HttpMethod.Post, "/api/v1/item-categories");
        catBReq.Content = JsonContent.Create(new CreateItemCategoryRequest
        {
            Code = "CYC-B-" + Guid.NewGuid().ToString("N")[..4],
            Name = new LocalizedTextInput { Thai = "หมวด B", English = "Category B" },
            ParentCategoryId = catA!.Id
        });
        var catBRes = await _client.SendAsync(catBReq);
        var catB = await catBRes.Content.ReadFromJsonAsync<ItemCategoryDetailResponse>();

        // Create Cat C with Parent = Cat B
        var catCReq = CreateRequest(HttpMethod.Post, "/api/v1/item-categories");
        catCReq.Content = JsonContent.Create(new CreateItemCategoryRequest
        {
            Code = "CYC-C-" + Guid.NewGuid().ToString("N")[..4],
            Name = new LocalizedTextInput { Thai = "หมวด C", English = "Category C" },
            ParentCategoryId = catB!.Id
        });
        var catCRes = await _client.SendAsync(catCReq);
        var catC = await catCRes.Content.ReadFromJsonAsync<ItemCategoryDetailResponse>();

        // Update Cat A to have Parent = Cat C (forming indirect cycle A -> C -> B -> A)
        var updateCatAReq = CreateRequest(HttpMethod.Put, $"/api/v1/item-categories/{catA.Id}", ifMatch: catA.RowVersion);
        updateCatAReq.Content = JsonContent.Create(new UpdateItemCategoryRequest
        {
            Code = catA.Code,
            Name = new LocalizedTextInput { Thai = catA.Name.Thai, English = catA.Name.English },
            ParentCategoryId = catC!.Id
        });
        var updateCatARes = await _client.SendAsync(updateCatAReq);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, updateCatARes.StatusCode);
    }

    [Fact]
    public async Task ItemAlias_Duplicate_Returns409Conflict()
    {
        var (categoryId, unitId, _) = await SeedTaxonomyAsync();

        var createReq = CreateRequest(HttpMethod.Post, "/api/v1/items");
        createReq.Content = JsonContent.Create(new CreateItemRequest
        {
            Code = "ALIAS-DUP-" + Guid.NewGuid().ToString("N")[..6],
            ItemType = "material",
            CategoryId = categoryId,
            BaseUnitId = unitId,
            Name = new LocalizedTextInput { Thai = "สินค้าซ้ำ Alias", English = "Alias dup item" }
        });
        var createRes = await _client.SendAsync(createReq);
        var item = await createRes.Content.ReadFromJsonAsync<ItemResponse>();

        // Add first alias
        var add1Req = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item!.Id}/aliases");
        add1Req.Content = JsonContent.Create(new AddAliasRequest
        {
            Alias = new LocalizedTextInput { Thai = "ชื่อเล่นเดียว", English = "Same Nickname" }
        });
        var add1Res = await _client.SendAsync(add1Req);
        Assert.Equal(HttpStatusCode.OK, add1Res.StatusCode);

        // Add same alias again
        var add2Req = CreateRequest(HttpMethod.Post, $"/api/v1/items/{item.Id}/aliases");
        add2Req.Content = JsonContent.Create(new AddAliasRequest
        {
            Alias = new LocalizedTextInput { Thai = "ชื่อเล่นเดียว", English = "Same Nickname" }
        });
        var add2Res = await _client.SendAsync(add2Req);
        Assert.Equal(HttpStatusCode.Conflict, add2Res.StatusCode);
    }

    [Fact]
    public async Task CategoryAttributeTemplates_GetAndSet_SupportsInheritanceAndOptions()
    {
        var (parentCatId, _, _) = await SeedTaxonomyAsync();

        // 1. Set template on parent category
        var setParentReq = CreateRequest(HttpMethod.Put, $"/api/v1/item-categories/{parentCatId}/attribute-template");
        setParentReq.Content = JsonContent.Create(new SetCategoryAttributeTemplatesRequest
        {
            Templates = new List<CategoryAttributeTemplateDto>
            {
                new CategoryAttributeTemplateDto
                {
                    Key = "material",
                    Name = new LocalizedTextInput { Thai = "วัสดุหลัก", English = "Primary Material" },
                    DataType = "select",
                    IsRequired = true,
                    Options = new List<CategoryAttributeOptionDto>
                    {
                        new CategoryAttributeOptionDto { Value = "plywood", Label = new LocalizedTextInput { Thai = "ไม้อัดยาง", English = "Plywood" } }
                    }
                }
            }
        });
        var setParentRes = await _client.SendAsync(setParentReq);
        Assert.Equal(HttpStatusCode.OK, setParentRes.StatusCode);

        // 2. Create child category
        var childCatReq = CreateRequest(HttpMethod.Post, "/api/v1/item-categories");
        childCatReq.Content = JsonContent.Create(new CreateItemCategoryRequest
        {
            Code = "CAT-CHILD-" + Guid.NewGuid().ToString("N")[..6],
            Name = new LocalizedTextInput { Thai = "หมวดย่อย", English = "Child Category" },
            AllowedItemTypes = new List<string> { "Standard", "Service" },
            ParentCategoryId = parentCatId
        });
        var childCatRes = await _client.SendAsync(childCatReq);
        Assert.Equal(HttpStatusCode.Created, childCatRes.StatusCode);
        var childCat = await childCatRes.Content.ReadFromJsonAsync<ItemCategoryDetailResponse>();

        // 3. Set template on child category
        var setChildReq = CreateRequest(HttpMethod.Put, $"/api/v1/item-categories/{childCat!.Id}/attribute-template");
        setChildReq.Content = JsonContent.Create(new SetCategoryAttributeTemplatesRequest
        {
            Templates = new List<CategoryAttributeTemplateDto>
            {
                new CategoryAttributeTemplateDto
                {
                    Key = "thickness_mm",
                    Name = new LocalizedTextInput { Thai = "ความหนา", English = "Thickness" },
                    DataType = "number",
                    Unit = "mm",
                    IsRequired = true
                }
            }
        });
        var setChildRes = await _client.SendAsync(setChildReq);
        Assert.Equal(HttpStatusCode.OK, setChildRes.StatusCode);

        // 4. Get child templates -> must contain inherited "material" from parent AND "thickness_mm" from child
        var getTemplatesReq = CreateRequest(HttpMethod.Get, $"/api/v1/item-categories/{childCat.Id}/attribute-template");
        var getTemplatesRes = await _client.SendAsync(getTemplatesReq);
        Assert.Equal(HttpStatusCode.OK, getTemplatesRes.StatusCode);
        var templateResp = await getTemplatesRes.Content.ReadFromJsonAsync<CategoryAttributeTemplateResponse>();

        Assert.NotNull(templateResp);
        Assert.Equal(2, templateResp.Templates.Count);
        Assert.Contains(templateResp.Templates, t => t.Key == "material" && t.DataType == "select");
        Assert.Contains(templateResp.Templates, t => t.Key == "thickness_mm" && t.DataType == "number" && t.Unit == "mm");
    }
}
