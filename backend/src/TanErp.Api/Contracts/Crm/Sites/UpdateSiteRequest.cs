using System.ComponentModel.DataAnnotations;

namespace TanErp.Api.Contracts.Crm.Sites;

public sealed record UpdateSiteRequest([Required] string Label, [Required] string AddressLine1, [Required] string Subdistrict, [Required] string District, [Required] string Province, [Required] string PostalCode, [Required] string CountryCode, decimal? Latitude, decimal? Longitude, string? AccessNote);
