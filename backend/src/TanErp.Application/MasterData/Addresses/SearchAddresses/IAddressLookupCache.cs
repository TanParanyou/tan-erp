namespace TanErp.Application.MasterData.Addresses.SearchAddresses;

public interface IAddressLookupCache
{
    Task<IReadOnlyList<SearchAddressItem>> SearchAsync(string query, int limit, CancellationToken cancellationToken = default);
}
