using Microsoft.AspNetCore.Mvc;
using AWEfinal.BLL.Services;

namespace AWEfinal.UI.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var orders = await _orderService.GetOrdersByUserIdAsync(userId.Value);
            return View(orders.ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || order.UserId != userId.Value && HttpContext.Session.GetString("UserRole") != "admin")
                return Forbid();

            return View(order);
        }
    }
}

