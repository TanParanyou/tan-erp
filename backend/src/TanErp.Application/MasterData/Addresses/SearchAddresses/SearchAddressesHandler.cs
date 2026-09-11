namespace TanErp.Application.MasterData.Addresses.SearchAddresses;

public class SearchAddressesHandler
{
    private readonly IAddressLookupCache _cache;

    public SearchAddressesHandler(IAddressLookupCache cache)
    {
        _cache = cache;
    }

    public async Task<IReadOnlyList<SearchAddressItem>> Handle(SearchAddressesQuery query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Query))
        {
            return Array.Empty<SearchAddressItem>();
        }

        var limit = Math.Clamp(query.Limit, 1, 50);
        return await _cache.SearchAsync(query.Query.Trim(), limit, cancellationToken);
    }
}
