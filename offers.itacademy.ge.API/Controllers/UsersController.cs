// Copyright (C) TBC Bank. All Rights Reserved.

using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Images;
using offers.itacademy.ge.Application.Models.Users;
using offers.itacademy.ge.Application.Models.Users.Admin;
using offers.itacademy.ge.Application.Models.Users.Company;
using offers.itacademy.ge.Application.Models.Users.Customer;
using offers.itacademy.ge.Domain.Users;

namespace offers.itacademy.ge.API.Controllers
{
    /// <summary>
    /// Controller for managing user-related operations such as viewing profiles or managing roles.
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion(2)]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userService">Service for handling user-related operations.</param>
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // ========== GET Endpoints ========== //

        /// <summary>
        /// Admin - Get all companies, paginated
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("companies")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<CompanyResponseModel>), 200)]
        public async Task<ActionResult<List<CompanyResponseModel>>> GetAllCompaniesAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
        {
            var companies = await _userService.GetUsersAsync<Company, CompanyResponseModel>(pageNumber, pageSize, token).ConfigureAwait(false);
            return Ok(companies);
        }

        /// <summary>
        /// Admin - Get all customers, paginated
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("customers")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<CustomerResponseModel>), 200)]
        public async Task<ActionResult<List<CustomerResponseModel>>> GetAllCustomersAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
        {
            var customers = await _userService.GetUsersAsync<Customer, CustomerResponseModel>(pageNumber, pageSize, token).ConfigureAwait(false);
            return Ok(customers);
        }

        /// <summary>
        /// Admin - Get all admins, paginated
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("admins")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<AdminResponseModel>), 200)]
        public async Task<ActionResult<List<AdminResponseModel>>> GetAllAdminsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken token = default)
        {
            var admins = await _userService.GetUsersAsync<Admin, AdminResponseModel>(pageNumber, pageSize, token).ConfigureAwait(false);
            return Ok(admins);
        }

        // ========== POST Endpoints ========== //
        /// <summary>
        /// Login to system
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginRequestModel model, CancellationToken token)
        {
            var result = await _userService.AuthenticateAsync(model, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Admin - Register new admin to system
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("register/admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminResponseModel), 200)]
        public async Task<ActionResult<AdminResponseModel>> RegisterAdmin([FromBody] AdminRequestModel model, CancellationToken token)
        {
            var result = await _userService.RegisterAsync<AdminRequestModel, AdminResponseModel>(model, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Register new company to system
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("register/company")]
        [ProducesResponseType(typeof(CompanyResponseModel), 200)]
        public async Task<ActionResult<CompanyResponseModel>> RegisterCompany([FromBody] CompanyRequestModel model, CancellationToken token)
        {
            var result = await _userService.RegisterAsync<CompanyRequestModel, CompanyResponseModel>(model, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Register new customer to system
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("register/customer")]
        [ProducesResponseType(typeof(CustomerResponseModel), 200)]
        public async Task<ActionResult<CustomerResponseModel>> RegisterCustomer([FromBody] CustomerRequestModel model, CancellationToken token)
        {
            var result = await _userService.RegisterAsync<CustomerRequestModel, CustomerResponseModel>(model, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Company - Upload image for company
        /// </summary>
        /// <param name="fileForm"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost("upload-company-image")]
        [Authorize(Roles = "Company")]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<ActionResult<string>> UploadCompanyImage([FromForm] ImageRequestModel fileForm, CancellationToken token)
        {
            var userId = Utils.AuthUtils.GetUserId(User);
            var result = await _userService.UploadCompanyImage(userId, fileForm, token).ConfigureAwait(false);
            return Ok(result);
        }

        // ========== PUT Endpoints ========== //

        /// <summary>
        /// Admin - Activate company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut("companies/{companyId}/activate")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ActionResult<bool>> ActivateCompany(int companyId, CancellationToken token)
        {
            var result = await _userService.ActivateCompanyAsync(companyId, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Customer - Update preferred categories for current customer
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut("customers/update-categories")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ActionResult<bool>> UpdateCustomerCategories([FromBody] List<int> categoryIds, CancellationToken token = default)
        {
            var customerId = Utils.AuthUtils.GetUserId(User);
            var result = await _userService.UpdateCustomerCategoriesAsync(customerId, categoryIds, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Admin - Update current admin
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut("update/admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminResponseModel), 200)]
        public async Task<ActionResult<AdminResponseModel>> UpdateAdmin([FromBody] AdminRequestUpdateModel model, CancellationToken token)
        {
            var userId = Utils.AuthUtils.GetUserId(User);
            var result = await _userService.UpdateUserAsync<AdminRequestUpdateModel, AdminResponseModel>(userId, model, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Company - Update current company
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut("update/company")]
        [Authorize(Roles = "Company")]
        [ProducesResponseType(typeof(CompanyResponseModel), 200)]
        public async Task<ActionResult<CompanyResponseModel>> UpdateCompany([FromBody] CompanyRequestUpdateModel model, CancellationToken token)
        {
            var userId = Utils.AuthUtils.GetUserId(User);
            var result = await _userService.UpdateUserAsync<CompanyRequestUpdateModel, CompanyResponseModel>(userId, model, token).ConfigureAwait(false);
            return Ok(result);
        }

        /// <summary>
        /// Customer - Update current customer
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut("update/customer")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(CustomerResponseModel), 200)]
        public async Task<ActionResult<CustomerResponseModel>> UpdateCustomer([FromBody] CustomerRequestUpdateModel model, CancellationToken token)
        {
            var userId = Utils.AuthUtils.GetUserId(User);
            var result = await _userService.UpdateUserAsync<CustomerRequestUpdateModel, CustomerResponseModel>(userId, model, token).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
