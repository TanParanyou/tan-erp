using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TanErp.Application.MasterData.Addresses.SearchAddresses;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.MasterData;

public class AddressLookupCache : IAddressLookupCache
{
    private readonly AppDbContext _db;
    private static IReadOnlyList<SearchAddressItem>? _cachedItems;
    private static readonly SemaphoreSlim _lock = new(1, 1);

    private sealed class RawAddressRecord
    {
        [JsonPropertyName("pTh")] public string PTh { get; set; } = string.Empty;
        [JsonPropertyName("dTh")] public string DTh { get; set; } = string.Empty;
        [JsonPropertyName("sCode")] public string SCode { get; set; } = string.Empty;
        [JsonPropertyName("sTh")] public string STh { get; set; } = string.Empty;
        [JsonPropertyName("zip")] public string Zip { get; set; } = string.Empty;
    }

    public AddressLookupCache(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<SearchAddressItem>> SearchAsync(
        string query,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var items = await EnsureLoadedAsync(cancellationToken);
        if (items.Count == 0 || string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<SearchAddressItem>();
        }

        var isNumeric = query.All(char.IsDigit);

        IEnumerable<SearchAddressItem> matches;
        if (isNumeric)
        {
            matches = items
                .Where(x => x.PostalCode.StartsWith(query, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.PostalCode.Length == query.Length ? 0 : 1)
                .ThenBy(x => x.Subdistrict);
        }
        else
        {
            matches = items
                .Select(x => new
                {
                    Item = x,
                    Score = CalculateScore(x, query)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Item.Subdistrict)
                .Select(x => x.Item);
        }

        return matches.Take(limit).ToList();
    }

    private static int CalculateScore(SearchAddressItem item, string query)
    {
        if (string.Equals(item.Subdistrict, query, StringComparison.OrdinalIgnoreCase)) return 100;
        if (string.Equals(item.District, query, StringComparison.OrdinalIgnoreCase)) return 90;
        if (string.Equals(item.Province, query, StringComparison.OrdinalIgnoreCase)) return 80;

        if (item.Subdistrict.StartsWith(query, StringComparison.OrdinalIgnoreCase)) return 70;
        if (item.District.StartsWith(query, StringComparison.OrdinalIgnoreCase)) return 60;
        if (item.Province.StartsWith(query, StringComparison.OrdinalIgnoreCase)) return 50;

        if (item.Subdistrict.Contains(query, StringComparison.OrdinalIgnoreCase)) return 40;
        if (item.District.Contains(query, StringComparison.OrdinalIgnoreCase)) return 30;
        if (item.Province.Contains(query, StringComparison.OrdinalIgnoreCase)) return 20;

        return 0;
    }

    private async Task<IReadOnlyList<SearchAddressItem>> EnsureLoadedAsync(CancellationToken cancellationToken)
    {
        if (_cachedItems != null)
        {
            return _cachedItems;
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_cachedItems != null)
            {
                return _cachedItems;
            }

            // Try loading from database first
            if (await _db.Subdistricts.AnyAsync(cancellationToken))
            {
                var query = from s in _db.Subdistricts.AsNoTracking()
                            join d in _db.Districts.AsNoTracking() on s.DistrictId equals d.Id
                            join p in _db.Provinces.AsNoTracking() on d.ProvinceId equals p.Id
                            select new SearchAddressItem(
                                s.Code,
                                s.NameTh,
                                d.NameTh,
                                p.NameTh,
                                s.PostalCode,
                                "TH",
                                s.DefaultLatitude,
                                s.DefaultLongitude,
                                $"{s.NameTh} » {d.NameTh} » {p.NameTh} {s.PostalCode}");

                _cachedItems = await query.ToListAsync(cancellationToken);
                return _cachedItems;
            }

            // Fallback: load directly from embedded resource if database is not yet seeded
            var assembly = typeof(AddressLookupCache).Assembly;
            const string resourceName = "TanErp.Infrastructure.MasterData.Seed.thailand_addresses.json.gz";
            await using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                await using var gzip = new GZipStream(stream, CompressionMode.Decompress);
                using var reader = new StreamReader(gzip);
                var json = await reader.ReadToEndAsync(cancellationToken);
                var records = JsonSerializer.Deserialize<List<RawAddressRecord>>(json);

                if (records != null)
                {
                    _cachedItems = records
                        .Select(r => new SearchAddressItem(
                            r.SCode,
                            r.STh,
                            r.DTh,
                            r.PTh,
                            r.Zip,
                            "TH",
                            null,
                            null,
                            $"{r.STh} » {r.DTh} » {r.PTh} {r.Zip}"))
                        .ToList();

                    return _cachedItems;
                }
            }

            _cachedItems = Array.Empty<SearchAddressItem>();
            return _cachedItems;
        }
        finally
        {
            _lock.Release();
        }
    }
}
