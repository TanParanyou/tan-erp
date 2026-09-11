using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TanErp.Domain.MasterData.Geography;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.MasterData.Seed;

public static class AddressMasterDataSeeder
{
    private sealed class RawAddressRecord
    {
        [JsonPropertyName("pCode")]
        public string PCode { get; set; } = string.Empty;

        [JsonPropertyName("pTh")]
        public string PTh { get; set; } = string.Empty;

        [JsonPropertyName("pEn")]
        public string PEn { get; set; } = string.Empty;

        [JsonPropertyName("dCode")]
        public string DCode { get; set; } = string.Empty;

        [JsonPropertyName("dTh")]
        public string DTh { get; set; } = string.Empty;

        [JsonPropertyName("dEn")]
        public string DEn { get; set; } = string.Empty;

        [JsonPropertyName("sCode")]
        public string SCode { get; set; } = string.Empty;

        [JsonPropertyName("sTh")]
        public string STh { get; set; } = string.Empty;

        [JsonPropertyName("sEn")]
        public string SEn { get; set; } = string.Empty;

        [JsonPropertyName("zip")]
        public string Zip { get; set; } = string.Empty;
    }

    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Provinces.AnyAsync(cancellationToken))
        {
            return;
        }

        var assembly = typeof(AddressMasterDataSeeder).Assembly;
        const string resourceName = "TanErp.Infrastructure.MasterData.Seed.thailand_addresses.json.gz";

        await using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
        }

        await using var gzip = new GZipStream(stream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip);
        var json = await reader.ReadToEndAsync(cancellationToken);

        var records = JsonSerializer.Deserialize<List<RawAddressRecord>>(json);
        if (records == null || records.Count == 0)
        {
            return;
        }

        var provinceMap = new Dictionary<string, Province>(StringComparer.OrdinalIgnoreCase);
        var districtMap = new Dictionary<string, District>(StringComparer.OrdinalIgnoreCase);
        var subdistricts = new List<Subdistrict>();

        foreach (var r in records)
        {
            if (!provinceMap.TryGetValue(r.PCode, out var province))
            {
                province = new Province(Guid.NewGuid(), r.PCode, r.PTh, r.PEn);
                provinceMap[r.PCode] = province;
            }

            if (!districtMap.TryGetValue(r.DCode, out var district))
            {
                district = new District(Guid.NewGuid(), province.Id, r.DCode, r.DTh, r.DEn);
                districtMap[r.DCode] = district;
            }

            var subdistrict = new Subdistrict(
                Guid.NewGuid(),
                district.Id,
                r.SCode,
                r.STh,
                r.SEn,
                r.Zip);

            subdistricts.Add(subdistrict);
        }

        await db.Provinces.AddRangeAsync(provinceMap.Values, cancellationToken);
        await db.Districts.AddRangeAsync(districtMap.Values, cancellationToken);
        await db.Subdistricts.AddRangeAsync(subdistricts, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}
