using Microsoft.AspNetCore.Mvc;
using OnlineJobs.Application.Services;

namespace OnlineJobs.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        public async Task<IActionResult> Index()
        {
            var root = await _categoryService.BuildCategoryTreeAsync();
            return View(root);
        }
    }
}
