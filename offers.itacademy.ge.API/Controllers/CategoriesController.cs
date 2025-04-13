// Copyright (C) TBC Bank. All Rights Reserved.

using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Categories;

namespace offers.itacademy.ge.API.Controllers;
/// <summary>
/// Controller for managing product categories. 
/// Provides endpoints for retrieving, creating, updating, and deleting categories.
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion(2)]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoriesController"/> class.
    /// </summary>
    /// <param name="categoryService">The service responsible for handling category operations.</param>
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all categories, paginated
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryResponseModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryResponseModel>>> GetAll([FromQuery][Range(1, int.MaxValue)] int page = 1, [FromQuery][Range(1, 100)] int pageSize = 10, CancellationToken token = default)
    {
        var categories = await _categoryService.GetAllCategoriesAsync(page, pageSize, token).ConfigureAwait(false);
        return Ok(categories);
    }

    /// <summary>
    /// Get category by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseModel>> GetById([FromRoute] int id, CancellationToken token = default)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id, token).ConfigureAwait(false);
        return Ok(category);
    }

    /// <summary>
    /// Admin - Create category
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CategoryResponseModel>> Create([FromBody] CategoryRequestModel request, CancellationToken token = default)
    {
        var createdCategory = await _categoryService.CreateCategoryAsync(request, token).ConfigureAwait(false);
        return Ok(createdCategory);
    }

    /// <summary>
    /// Admin - Update category
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseModel>> Update([FromRoute] int id, [FromBody] CategoryRequestModel request, CancellationToken token = default)
    {
        var updatedCategory = await _categoryService.UpdateCategoryAsync(id, request, token).ConfigureAwait(false);
        return Ok(updatedCategory);
    }

    /// <summary>
    /// Admin - Delete category
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken token = default)
    {
        await _categoryService.DeleteCategoryAsync(id, token).ConfigureAwait(false);
        return NoContent();
    }
}
