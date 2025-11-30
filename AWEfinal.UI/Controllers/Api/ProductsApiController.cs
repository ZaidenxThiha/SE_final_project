using System.Text.Json;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;
using AWEfinal.UI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AWEfinal.UI.Controllers.Api;

[ApiController]
[Route("api/products")]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsApiController> _logger;
    private readonly IWebHostEnvironment _environment;

    public ProductsApiController(IProductService productService, ILogger<ProductsApiController> logger, IWebHostEnvironment environment)
    {
        _productService = productService;
        _logger = logger;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? category, [FromQuery] string? search, [FromQuery] string? sort)
    {
        IEnumerable<Product> products = (!string.IsNullOrEmpty(category) && !string.Equals(category, "All", StringComparison.OrdinalIgnoreCase))
            ? await _productService.GetProductsByCategoryAsync(category)
            : await _productService.GetAllProductsAsync();

        var productList = products.ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
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

        return Ok(productList.Select(p => p.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product.ToDto());
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto request)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var product = ProductMappingExtensions.FromDto(request);
        var created = await _productService.CreateProductAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto request)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        var existing = await _productService.GetProductByIdAsync(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.ApplyFromDto(request);
        var updated = await _productService.UpdateProductAsync(existing);
        return Ok(updated.ToDto());
    }

    [HttpPost("{id:int}/images")]
    public async Task<IActionResult> UploadImage(int id, IFormFile? file)
    {
        if (!IsAdmin())
            return Forbid();

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded." });

        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "products");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/images/products/{fileName}";
        var images = !string.IsNullOrWhiteSpace(product.Images)
            ? JsonSerializer.Deserialize<List<string>>(product.Images) ?? new List<string>()
            : new List<string>();
        images.Add(relativePath);
        product.Images = JsonSerializer.Serialize(images);
        await _productService.UpdateProductAsync(product);

        return Ok(new { images });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        var deleted = await _productService.DeleteProductAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("UserRole") == "admin";
    }
}
