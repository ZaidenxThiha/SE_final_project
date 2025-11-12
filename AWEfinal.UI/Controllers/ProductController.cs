using Microsoft.AspNetCore.Mvc;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;
using System;
using System.Linq;

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

        public async Task<IActionResult> Index(string? category, string? searchTerm, string? sort)
        {
            IEnumerable<Product> products = (!string.IsNullOrEmpty(category) && !string.Equals(category, "All", StringComparison.OrdinalIgnoreCase))
                ? await _productService.GetProductsByCategoryAsync(category)
                : await _productService.GetAllProductsAsync();

            var productList = products.ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                productList = productList
                    .Where(p =>
                        (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.Category) && p.Category.Contains(term, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            sort = string.IsNullOrWhiteSpace(sort) ? "price-asc" : sort.Trim().ToLowerInvariant();
            productList = sort switch
            {
                "price-desc" => productList.OrderByDescending(p => p.Price).ToList(),
                "name-asc" => productList.OrderBy(p => p.Name).ToList(),
                _ => productList.OrderBy(p => p.Price).ToList()
            };

            ViewBag.Categories = new[] { "All", "Smartphones", "Laptops", "Tablets", "Headphones", 
                "Smart TVs", "Cameras", "Smartwatches", "Gaming Consoles", "Speakers", "Monitors" };
            ViewBag.SelectedCategory = category ?? "All";
            ViewBag.SearchTerm = searchTerm ?? string.Empty;
            ViewBag.Sort = sort;

            return View(productList);
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
