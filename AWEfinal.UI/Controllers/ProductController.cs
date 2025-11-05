using Microsoft.AspNetCore.Mvc;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;

namespace AWEfinal.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? category)
        {
            IEnumerable<Product> products;
            
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                products = await _productService.GetProductsByCategoryAsync(category);
            }
            else
            {
                products = await _productService.GetAllProductsAsync();
            }

            ViewBag.Categories = new[] { "All", "Smartphones", "Laptops", "Tablets", "Headphones", 
                "Smart TVs", "Cameras", "Smartwatches", "Gaming Consoles", "Speakers", "Monitors" };
            ViewBag.SelectedCategory = category ?? "All";

            return View(products.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}

