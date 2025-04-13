using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using offers.itacademy.ge.Application.Interfaces.ServiceInterfaces;
using offers.itacademy.ge.Application.Models.Categories;
using System.Threading;
using System.Threading.Tasks;

namespace offers.itacademy.ge.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /Category/GetAll
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10, CancellationToken token = default)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(page, pageSize, token);
            ViewBag.PageNumber = page;
            ViewBag.PageSize = pageSize;
            return View(categories);
        }

        // GET: /Category/Details/{id}
        public async Task<IActionResult> Details(int id, CancellationToken token = default)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id, token);
            if (category == null)
                return NotFound();
            return View(category);
        }

        // GET: /Category/Create (Admin only)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new CategoryRequestModel());
        }

        // POST: /Category/Create (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryRequestModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _categoryService.CreateCategoryAsync(model, token);
                TempData["Success"] = "Category created successfully.";
                return RedirectToAction(nameof(GetAll));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to create category: {ex.Message}");
                return View(model);
            }
        }

        // GET: /Category/Edit/{id} (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, CancellationToken token = default)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id, token);
            if (category == null)
                return NotFound();
            return View(category);
        }

        // POST: /Category/Edit/{id} (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryRequestModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _categoryService.UpdateCategoryAsync(id, model, token);
                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction(nameof(GetAll));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to update category: {ex.Message}");
                return View(model);
            }
        }

        // GET: /Category/Delete/{id} (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken token = default)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id, token);
            if (category == null)
                return NotFound();
            return View(category);
        }

        // POST: /Category/DeleteConfirmed (Admin only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken token = default)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id, token);
                TempData["Success"] = "Category deleted successfully.";
                return RedirectToAction(nameof(GetAll));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete category: {ex.Message}";
                return RedirectToAction(nameof(GetAll));
            }
        }
    }
}