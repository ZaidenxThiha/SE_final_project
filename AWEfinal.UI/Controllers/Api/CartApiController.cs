using System.Text.Json;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;
using AWEfinal.UI.Models;
using AWEfinal.UI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AWEfinal.UI.Controllers.Api;

[ApiController]
[Route("api/cart")]
public class CartApiController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<CartApiController> _logger;

    public CartApiController(IProductService productService, ILogger<CartApiController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetCart()
    {
        var cart = GetCartItems();
        return Ok(new CartResponse { Items = cart });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] CartMutationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var cart = GetCartItems();
        var product = await _productService.GetProductByIdAsync(request.ProductId);
        if (product == null)
        {
            return NotFound(new { message = "Product not found." });
        }

        var existing = cart.FirstOrDefault(c => c.ProductId == request.ProductId);
        if (existing != null)
        {
            existing.Quantity += Math.Max(1, request.Quantity);
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = Math.Max(1, request.Quantity),
                ImageUrl = GetPrimaryImage(product)
            });
        }

        SaveCart(cart);
        return Ok(new CartResponse { Items = cart });
    }

    [HttpPut("{productId:int}")]
    public IActionResult UpdateQuantity(int productId, [FromBody] UpdateCartQuantityRequest request)
    {
        var cart = GetCartItems();
        var existing = cart.FirstOrDefault(c => c.ProductId == productId);
        if (existing == null)
        {
            return NotFound();
        }

        if (request.Quantity <= 0)
        {
            cart.Remove(existing);
        }
        else
        {
            existing.Quantity = request.Quantity;
        }

        SaveCart(cart);
        return Ok(new CartResponse { Items = cart });
    }

    [HttpDelete("{productId:int}")]
    public IActionResult RemoveItem(int productId)
    {
        var cart = GetCartItems();
        var existing = cart.FirstOrDefault(c => c.ProductId == productId);
        if (existing == null)
        {
            return NotFound();
        }

        cart.Remove(existing);
        SaveCart(cart);
        return Ok(new CartResponse { Items = cart });
    }

    [HttpDelete]
    public IActionResult ClearCart()
    {
        HttpContext.Session.Remove("Cart");
        return NoContent();
    }

    private List<CartItem> GetCartItems()
    {
        var cartJson = HttpContext.Session.GetString("Cart");
        if (string.IsNullOrEmpty(cartJson))
        {
            return new List<CartItem>();
        }

        var items = JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        foreach (var item in items)
        {
            item.ImageUrl = string.IsNullOrEmpty(item.ImageUrl)
                ? "/images/products/placeholder.jpg"
                : item.ImageUrl;
        }

        return items;
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
    }

    private static string GetPrimaryImage(Product product)
    {
        if (!string.IsNullOrEmpty(product.Images))
        {
            try
            {
                var images = JsonSerializer.Deserialize<List<string>>(product.Images);
                if (images is { Count: > 0 })
                {
                    return images.First();
                }
            }
            catch
            {
                // Ignore parse errors
            }
        }

        return "/images/products/placeholder.jpg";
    }
}
