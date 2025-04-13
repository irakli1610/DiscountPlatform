// Copyright (C) TBC Bank. All Rights Reserved.

using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Purchases;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.API.Controllers
{
    /// <summary>
    /// Controller for managing purchases. 
    /// Allows customers and companies to view and interact with purchase data.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion(2)]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchasesController"/> class.
        /// </summary>
        /// <param name="purchaseService">Service responsible for handling purchase-related operations.</param>
        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        /// <summary>
        /// Customer - Purchase offer
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="quantity"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("buy")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(PurchaseResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PurchaseResponseModel>> PurchaseOfferAsync(int offerId, int quantity, CancellationToken token = default)
        {
            var userId = Utils.AuthUtils.GetUserId(User);

            var result = await _purchaseService.PurchaseOfferAsync(userId, offerId, quantity, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Customer - Cancell purchase
        /// </summary>
        /// <param name="purchaseId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("{purchaseId}/cancel")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelPurchaseAsync(int purchaseId, CancellationToken token = default)
        {
            var userId = Utils.AuthUtils.GetUserId(User);

            await _purchaseService.CancelPurchaseAsync(purchaseId, userId, token).ConfigureAwait(false);
            return NoContent();
        }

        /// <summary>
        /// Customer,Company - Get purchases of current user, paginated
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("my")]
        [Authorize(Roles = "Customer,Company")]
        [ProducesResponseType(typeof(List<PurchaseResponseModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PurchaseResponseModel>>> GetMyPurchasesAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
        {
            var userId = Utils.AuthUtils.GetUserId(User);
            var result = await _purchaseService.GetUserPurchasesAsync(pageNumber, pageSize, userId, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Admin - Get purchases of user by user id, paginated
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<PurchaseResponseModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PurchaseResponseModel>>> GetUserPurchasesAsync(int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
        {
            var result = await _purchaseService.GetUserPurchasesAsync(pageNumber, pageSize, userId, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Company, Admin - Get purchases of concrete offer, paginated
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("offer/{offerId}")]
        [Authorize(Roles = "Company,Admin")]
        [ProducesResponseType(typeof(List<PurchaseResponseModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<PurchaseResponseModel>>> GetOfferPurchases(int offerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
        {
            var requestingUserId = Utils.AuthUtils.GetUserId(User); //  Get ID from JWT
            var requestingUserRole = Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!); //  Get Role from JWT

            var result = await _purchaseService.GetOfferPurchasesAsync(pageNumber, pageSize, offerId, requestingUserId, requestingUserRole, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Customer - Get purchase by id for current customer user.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(PurchaseResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PurchaseResponseModel>> GetById(int id, CancellationToken token = default)
        {
            var userId = Utils.AuthUtils.GetUserId(User); //  Get ID from JWT   
            var result = await _purchaseService.GetPurchaseByIdAsync(id, userId, token).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
