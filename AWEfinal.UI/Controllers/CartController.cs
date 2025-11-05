using Microsoft.AspNetCore.Mvc;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;
using System.Text.Json;

namespace AWEfinal.UI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ILogger<CartController> _logger;

        public CartController(IProductService productService, IOrderService orderService, ILogger<CartController> logger)
        {
            _productService = productService;
            _orderService = orderService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found";
                return RedirectToAction("Index", "Product");
            }

            var cart = GetCart();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            TempData["SuccessMessage"] = $"{product.Name} added to cart!";
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string fullName, string phone, string address, 
            string city, string postalCode, string country, string paymentMethod)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            try
            {
                var order = new Order
                {
                    UserId = userId.Value,
                    ShippingFullName = fullName,
                    ShippingPhone = phone,
                    ShippingAddress = address,
                    ShippingCity = city,
                    ShippingPostalCode = postalCode,
                    ShippingCountry = country,
                    PaymentMethod = paymentMethod,
                    Status = "pending"
                };

                var orderItems = cart.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    ProductName = c.ProductName,
                    Quantity = c.Quantity,
                    Price = c.Price,
                    Subtotal = c.Price * c.Quantity
                }).ToList();

                var createdOrder = await _orderService.CreateOrderAsync(order, orderItems);
                
                HttpContext.Session.Remove("Cart");
                
                return RedirectToAction("OrderConfirmation", new { id = createdOrder.Id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            // Verify user owns this order
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || order.UserId != userId.Value)
                return Forbid();

            return View(order);
        }

        // Print Customer Receipt (for customers after checkout)
        public async Task<IActionResult> PrintReceipt(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            // Verify user owns this order
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || order.UserId != userId.Value)
                return Forbid();

            return View("PrintReceipt", order);
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
}

