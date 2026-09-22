using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Items;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Common.Abstractions;
using TanErp.Application.Items;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/items/{itemId:guid}/images")]
[Authorize]
public class ItemImagesController : ControllerBase
{
    private readonly IRequestAccessResolver _accessResolver;
    private readonly IItemImageStore _imageStore;

    public ItemImagesController(IRequestAccessResolver accessResolver, IItemImageStore imageStore)
    {
        _accessResolver = accessResolver;
        _imageStore = imageStore;
    }

    [HttpPost]
    [ProducesResponseType<ItemImageDetailResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Attach(
        [FromRoute] Guid itemId,
        [FromBody] AttachItemImageRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.manage-images",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var data = new AttachItemImageData(
            itemId,
            request.FileId,
            request.Role,
            request.IsPrimary,
            new LocalizedTextDto(request.AltText.Thai, request.AltText.English),
            request.Caption != null ? new LocalizedTextDto(request.Caption.Thai, request.Caption.English) : null);

        var result = await _imageStore.AttachImageAsync(data, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var image = result.Value!;
        Response.Headers.ETag = $"\"{image.RowVersion}\"";
        return CreatedAtAction(nameof(List), new { itemId }, ItemResponseMapper.ToResponse(image));
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ItemImageDetailResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List(
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.read",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var images = await _imageStore.ListImagesAsync(access.OrganizationId, itemId, cancellationToken);
        var response = images.Select(ItemResponseMapper.ToResponse).ToList();
        return Ok(response);
    }

    [HttpPost("{imageId:guid}/primary")]
    [ProducesResponseType<ItemImageDetailResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPrimary(
        [FromRoute] Guid itemId,
        [FromRoute] Guid imageId,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.manage-images",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var result = await _imageStore.SetPrimaryImageAsync(access.OrganizationId, itemId, imageId, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var image = result.Value!;
        Response.Headers.ETag = $"\"{image.RowVersion}\"";
        return Ok(ItemResponseMapper.ToResponse(image));
    }

    [HttpPut("reorder")]
    [ProducesResponseType<IReadOnlyList<ItemImageDetailResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Reorder(
        [FromRoute] Guid itemId,
        [FromBody] ReorderItemImagesRequest request,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.manage-images",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var result = await _imageStore.ReorderImagesAsync(access.OrganizationId, itemId, request.OrderedImageIds, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var response = result.Value!.Select(ItemResponseMapper.ToResponse).ToList();
        return Ok(response);
    }

    [HttpDelete("{imageId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Detach(
        [FromRoute] Guid itemId,
        [FromRoute] Guid imageId,
        CancellationToken cancellationToken)
    {
        var authResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (authResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(authResult.Error.Code, HttpContext);
        }

        var auth = authResult.Value!;
        var accessResult = await _accessResolver.ResolveAsync(
            auth.FirebaseUid,
            auth.MembershipId,
            "items.manage-images",
            cancellationToken);

        if (accessResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(accessResult.Error.Code, HttpContext);
        }

        var access = accessResult.Value!;
        var result = await _imageStore.DetachImageAsync(access.OrganizationId, itemId, imageId, access, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        return NoContent();
    }
}
