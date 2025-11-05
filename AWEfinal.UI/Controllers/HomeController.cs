using Microsoft.AspNetCore.Mvc;
using AWEfinal.BLL.Services;

namespace AWEfinal.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IProductService productService, ILogger<HomeController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products.Take(8).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}

