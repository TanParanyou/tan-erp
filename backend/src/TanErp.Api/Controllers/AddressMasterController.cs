using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.MasterData;
using TanErp.Api.ErrorHandling;
using TanErp.Application.MasterData.Addresses.SearchAddresses;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/master-data/addresses")]
[Authorize]
public class AddressMasterController : ControllerBase
{
    private readonly SearchAddressesHandler _handler;

    public AddressMasterController(SearchAddressesHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("search")]
    [ProducesResponseType<AddressSearchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Search(
        [FromQuery] string? q,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Ok(new AddressSearchResponse(Array.Empty<AddressSearchResultItem>()));
        }

        var results = await _handler.Handle(new SearchAddressesQuery(q, limit), cancellationToken);

        Response.Headers.CacheControl = "public, max-age=86400";

        var response = new AddressSearchResponse(
            results.Select(r => new AddressSearchResultItem(
                r.SubdistrictCode,
                r.Subdistrict,
                r.District,
                r.Province,
                r.PostalCode,
                r.CountryCode,
                r.Latitude,
                r.Longitude,
                r.DisplayText)).ToList());

        return Ok(response);
    }
}
