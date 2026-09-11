using TanErp.Application.MasterData.Addresses.SearchAddresses;
using Xunit;

namespace TanErp.UnitTests.MasterData;

public class SearchAddressesHandlerTests
{
    private class FakeAddressLookupCache : IAddressLookupCache
    {
        public string? LastQuery { get; private set; }
        public int LastLimit { get; private set; }
        public bool WasCalled { get; private set; }

        public Task<IReadOnlyList<SearchAddressItem>> SearchAsync(string query, int limit, CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            LastQuery = query;
            LastLimit = limit;

            IReadOnlyList<SearchAddressItem> results = new List<SearchAddressItem>
            {
                new(
                    "103901",
                    "คลองตันเหนือ",
                    "วัฒนา",
                    "กรุงเทพมหานคร",
                    "10110",
                    "TH",
                    13.7386m,
                    100.5847m,
                    "คลองตันเหนือ » วัฒนา » กรุงเทพมหานคร 10110")
            };

            return Task.FromResult(results);
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Handle_EmptyOrWhitespaceQuery_ReturnsEmptyWithoutCallingCache(string? query)
    {
        var cache = new FakeAddressLookupCache();
        var handler = new SearchAddressesHandler(cache);

        var result = await handler.Handle(new SearchAddressesQuery(query!), CancellationToken.None);

        Assert.Empty(result);
        Assert.False(cache.WasCalled);
    }

    [Fact]
    public async Task Handle_ValidQuery_TrimsQueryAndClampsLimit()
    {
        var cache = new FakeAddressLookupCache();
        var handler = new SearchAddressesHandler(cache);

        var result = await handler.Handle(new SearchAddressesQuery("  10110  ", Limit: 100), CancellationToken.None);

        Assert.Single(result);
        Assert.True(cache.WasCalled);
        Assert.Equal("10110", cache.LastQuery);
        Assert.Equal(50, cache.LastLimit); // clamped to 50
    }

    [Fact]
    public async Task Handle_LimitBelow1_ClampsToOne()
    {
        var cache = new FakeAddressLookupCache();
        var handler = new SearchAddressesHandler(cache);

        await handler.Handle(new SearchAddressesQuery("กรุงเทพ", Limit: -5), CancellationToken.None);

        Assert.Equal(1, cache.LastLimit);
    }
}
