// Copyright (C) TBC Bank. All Rights Reserved.

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.API.Utils;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Models.ProductOffers;

namespace offers.itacademy.ge.API.Controllers;

/// <summary>
/// Controller for managing product offers. 
/// Allows companies to create, update, and delete offers, and customers to browse and purchase them.
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion(2)]
[Authorize]
public class ProductOffersController : ControllerBase
{
    private readonly IProductOfferService _productOffersService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductOffersController"/> class.
    /// </summary>
    /// <param name="productOffersService">Service responsible for handling product offer operations.</param>
    public ProductOffersController(IProductOfferService productOffersService)
    {
        _productOffersService = productOffersService;
    }

    /// <summary>
    /// Admin - Get all offers, paginated
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductOfferResponseModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductOfferResponseModel>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
    {
        var result = await _productOffersService.GetAllOffersAsync(page, pageSize, token).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Get currently active offers, paginated
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(List<ProductOfferResponseModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductOfferResponseModel>>> GetActive([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
    {
        var result = await _productOffersService.GetAllActiveOffersAsync(page, pageSize, token).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Get offer by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductOfferResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductOfferResponseModel>> GetById([FromRoute] int id, CancellationToken token = default)
    {
        var result = await _productOffersService.GetOfferByIdAsync(id, token).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Company - Create offer
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Company")]
    [HttpPost]
    [ProducesResponseType(typeof(ProductOfferResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductOfferResponseModel>> Create([FromBody] ProductOfferRequestModel request, CancellationToken token = default)
    {
        var companyId = AuthUtils.GetUserId(User);
        var result = await _productOffersService.CreateOfferAsync(request, companyId, token).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Company - Update existing offer
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Company")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductOfferResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductOfferResponseModel>> Update([FromRoute] int id, [FromBody] ProductOfferRequestModel request, CancellationToken token = default)
    {
        var companyId = AuthUtils.GetUserId(User);
        var result = await _productOffersService.UpdateOfferAsync(id, request, companyId, token).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Company - Delete offer
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Company")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken token = default)
    {
        var companyId = AuthUtils.GetUserId(User);
        await _productOffersService.DeleteOfferAsync(id, companyId, token).ConfigureAwait(false);
        return NoContent();
    }

    /// <summary>
    /// Company - Cancell offer
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Company")]
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> Cancel([FromRoute] int id, CancellationToken token = default)
    {
        var companyId = AuthUtils.GetUserId(User);
        var result = await _productOffersService.CancelOfferAsync(id, companyId, token).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Customer - Get relevant offers for current customer, paginated
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Customer")]
    [HttpGet("relevant")]
    [ProducesResponseType(typeof(List<ProductOfferResponseModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductOfferResponseModel>>> GetRelevant([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
    {
        var userId = AuthUtils.GetUserId(User);
        var result = await _productOffersService.GetUserRelevantOffersAsync(page, pageSize, userId, token).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Company - Upload image for offer
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Company")]
    [HttpPost("{id}/image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> UploadImage([FromRoute] int id, [FromForm] ImageRequestModel request, CancellationToken token = default)
    {
        var userId = AuthUtils.GetUserId(User);
        var result = await _productOffersService.UploadProductOfferImage(userId, id, request, token).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    /// Company - Get offers by current company, paginated
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Company")]
    [HttpGet("company")]
    [ProducesResponseType(typeof(List<ProductOfferResponseModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductOfferResponseModel>>> GetCompanyOffers([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
    {
        var companyId = AuthUtils.GetUserId(User);
        var result = await _productOffersService.GetCompanyOffersAsync(page, pageSize, companyId, token).ConfigureAwait(false);
        return Ok(result);
    }
}
