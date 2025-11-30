using System.Text.Json;
using AWEfinal.DAL.Models;

namespace AWEfinal.UI.Models.Dto;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            Description = product.Description,
            Storage = product.Storage,
            Colors = DeserializeList(product.Colors),
            Sizes = DeserializeList(product.Sizes),
            Rating = product.Rating,
            Images = DeserializeList(product.Images),
            Features = DeserializeList(product.Features),
            InStock = product.InStock,
            StockQuantity = product.StockQuantity,
            CreatedAt = product.CreatedAt
        };
    }

    public static Product ApplyFromDto(this Product product, ProductDto dto)
    {
        product.Name = dto.Name.Trim();
        product.Price = dto.Price;
        product.Category = dto.Category.Trim();
        product.Description = dto.Description.Trim();
        product.Storage = dto.Storage;
        product.Colors = SerializeList(dto.Colors);
        product.Sizes = SerializeList(dto.Sizes);
        product.Rating = dto.Rating;
        product.Images = SerializeList(dto.Images);
        product.Features = SerializeList(dto.Features);
        product.StockQuantity = Math.Max(0, dto.StockQuantity);
        product.InStock = product.StockQuantity > 0;
        return product;
    }

    public static Product FromDto(ProductDto dto)
    {
        var product = new Product();
        product.ApplyFromDto(dto);
        return product;
    }

    private static List<string> DeserializeList(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new List<string>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static string SerializeList(IEnumerable<string>? value)
    {
        var list = value?.Where(v => !string.IsNullOrWhiteSpace(v)).ToList() ?? new List<string>();
        return JsonSerializer.Serialize(list);
    }
}
