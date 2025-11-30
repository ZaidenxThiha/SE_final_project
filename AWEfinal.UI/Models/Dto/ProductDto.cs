using System.ComponentModel.DataAnnotations;

namespace AWEfinal.UI.Models.Dto;

public class ProductDto
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    [Required]
    public string Category { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    public string? Storage { get; set; }
    public List<string> Colors { get; set; } = new();
    public List<string>? Sizes { get; set; }
    public decimal Rating { get; set; }
    public List<string> Images { get; set; } = new();
    public List<string> Features { get; set; } = new();
    public bool InStock { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
}
