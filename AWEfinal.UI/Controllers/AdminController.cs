using Microsoft.AspNetCore.Mvc;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;

namespace AWEfinal.UI.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IProductService productService, IOrderService orderService, ILogger<AdminController> logger)
        {
            _productService = productService;
            _orderService = orderService;
            _logger = logger;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "admin";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var products = await _productService.GetAllProductsAsync();
            var orders = await _orderService.GetAllOrdersAsync();
            var totalRevenue = orders.Where(o => o.Status != "cancelled").Sum(o => o.Total);
            var totalOrders = orders.Count();
            var avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            ViewBag.TotalProducts = products.Count();
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.AvgOrderValue = avgOrderValue;

            return View();
        }

        // Product Management
        public async Task<IActionResult> Products()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var products = await _productService.GetAllProductsAsync();
            return View(products.ToList());
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.CreateProductAsync(product);
                    return RedirectToAction("Products");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            if (ModelState.IsValid)
            {
                try
                {
                    await _productService.UpdateProductAsync(product);
                    return RedirectToAction("Products");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = ex.Message;
                }
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var result = await _productService.DeleteProductAsync(id);
            return RedirectToAction("Products");
        }

        // Order Management
        public async Task<IActionResult> Orders(int? selectedOrderId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var orders = await _orderService.GetAllOrdersAsync();
            if (selectedOrderId.HasValue)
            {
                ViewBag.SelectedOrderId = selectedOrderId.Value;
            }
            return View(orders.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status, string? trackingNumber)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, status, trackingNumber);
                return RedirectToAction("Orders");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("Orders");
            }
        }

        // Analytics
        public async Task<IActionResult> Analytics(string period = "month")
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Auth");

            var allOrders = (await _orderService.GetAllOrdersAsync()).ToList();
            var now = DateTime.UtcNow;
            DateTime startDate;

            switch (period.ToLower())
            {
                case "day":
                    startDate = now.AddDays(-7);
                    break;
                case "week":
                    startDate = now.AddDays(-28);
                    break;
                case "month":
                    startDate = now.AddMonths(-6);
                    break;
                case "year":
                    startDate = now.AddYears(-2);
                    break;
                case "ytd":
                    startDate = new DateTime(now.Year, 1, 1);
                    break;
                default:
                    startDate = now.AddMonths(-6);
                    break;
            }

            var filteredOrders = allOrders
                .Where(o => o.CreatedAt >= startDate && o.Status != "cancelled")
                .ToList();

            var totalRevenue = filteredOrders.Sum(o => o.Total);
            var totalOrders = filteredOrders.Count;
            var totalProducts = filteredOrders
                .SelectMany(o => o.OrderItems)
                .Sum(oi => oi.Quantity);
            var avgOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;

            // Group by period for charts
            List<object> chartData;
            
            if (filteredOrders.Any())
            {
                chartData = filteredOrders
                    .GroupBy(o =>
                    {
                        var orderDate = o.CreatedAt;
                        return period switch
                        {
                            "day" => orderDate.ToString("MMM dd"),
                            "week" => $"Week of {orderDate.AddDays(-(int)orderDate.DayOfWeek):MMM dd}",
                            "month" or "ytd" => orderDate.ToString("MMM yy"),
                            "year" => orderDate.Year.ToString(),
                            _ => orderDate.ToString("MMM yy")
                        };
                    })
                    .Select(g => new
                    {
                        date = g.Key,  // Use lowercase for JSON serialization
                        revenue = (double)g.Sum(o => o.Total),  // Convert to double for JSON
                        orders = g.Count(),
                        products = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity)
                    })
                    .OrderBy(x => x.date)
                    .Cast<object>()
                    .ToList();
            }
            else
            {
                // If no data, add at least one empty entry to prevent chart errors
                chartData = new List<object>
                {
                    new { date = "No Data", revenue = 0.0, orders = 0, products = 0 }
                };
            }

            // Top selling products
            var topProductsList = filteredOrders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => new { oi.ProductId, oi.ProductName })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    Name = g.Key.ProductName ?? "Unknown Product",
                    Quantity = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Price * oi.Quantity)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(10)
                .ToList();

            ViewBag.Period = period;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.AvgOrderValue = avgOrderValue;
            ViewBag.ChartData = chartData;
            ViewBag.TopProducts = topProductsList;

            return View();
        }
    }
}

