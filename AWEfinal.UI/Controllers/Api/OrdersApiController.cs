using System.Text.Json;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;
using AWEfinal.UI.Models;
using AWEfinal.UI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AWEfinal.UI.Controllers.Api;

[ApiController]
[Route("api/orders")]
public class OrdersApiController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersApiController> _logger;

    public OrdersApiController(IOrderService orderService, ILogger<OrdersApiController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] string scope = "mine")
    {
        var isAdmin = IsAdmin();
        var userId = HttpContext.Session.GetInt32("UserId");

        if (!isAdmin && userId == null)
        {
            return Unauthorized();
        }

        IEnumerable<Order> orders;
        if (isAdmin && scope.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            orders = await _orderService.GetAllOrdersAsync();
        }
        else
        {
            orders = await _orderService.GetOrdersByUserIdAsync(userId!.Value);
        }

        var ordered = orders.OrderByDescending(o => o.CreatedAt).ToList();
        return Ok(ordered.Select(o => o.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (!IsAdmin() && (userId == null || order.UserId != userId.Value))
        {
            return Forbid();
        }

        return Ok(order.ToDto());
    }

    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Unauthorized();
        }

        var cart = GetCartItems();
        if (!cart.Any())
        {
            return BadRequest(new { message = "Cart is empty." });
        }

        var order = new Order
        {
            UserId = userId.Value,
            ShippingFullName = request.FullName,
            ShippingPhone = request.Phone,
            ShippingAddress = request.Address,
            ShippingCity = request.City,
            ShippingPostalCode = request.PostalCode,
            ShippingCountry = request.Country,
            PaymentMethod = request.PaymentMethod
        };

        var orderItems = cart.Select(item => new OrderItem
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            Price = item.Price,
            Subtotal = item.Subtotal
        }).ToList();

        var createdOrder = await _orderService.CreateOrderAsync(order, orderItems);
        HttpContext.Session.Remove("Cart");

        return Ok(createdOrder.ToDto());
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        var updated = await _orderService.UpdateOrderStatusAsync(id, request.Status, request.TrackingNumber);
        return Ok(updated.ToDto());
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("UserRole") == "admin";
    }

    private List<CartItem> GetCartItems()
    {
        var cartJson = HttpContext.Session.GetString("Cart");
        if (string.IsNullOrEmpty(cartJson))
        {
            return new List<CartItem>();
        }

        return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
    }
}
