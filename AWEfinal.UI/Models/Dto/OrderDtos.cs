using AWEfinal.DAL.Models;

namespace AWEfinal.UI.Models.Dto;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
    public string? ImageUrl { get; set; }
}

public class ShippingAddressDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public ShippingAddressDto ShippingAddress { get; set; } = new();
    public string? TrackingNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public static class OrderMappingExtensions
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            OrderNumber = order.OrderNumber,
            InvoiceNumber = order.InvoiceNumber,
            Status = order.Status,
            Total = order.Total,
            PaymentMethod = order.PaymentMethod,
            ShippingAddress = new ShippingAddressDto
            {
                FullName = order.ShippingFullName,
                Phone = order.ShippingPhone,
                Address = order.ShippingAddress,
                City = order.ShippingCity,
                PostalCode = order.ShippingPostalCode,
                Country = order.ShippingCountry
            },
            TrackingNumber = order.TrackingNumber,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.OrderItems.Select(oi => new OrderItemDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                Quantity = oi.Quantity,
                Price = oi.Price,
                Subtotal = oi.Subtotal,
                ImageUrl = GetPrimaryImage(oi.Product)
            }).ToList()
        };
    }

    private static string? GetPrimaryImage(Product? product)
    {
        if (product == null || string.IsNullOrWhiteSpace(product.Images))
        {
            return null;
        }

        try
        {
            var images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(product.Images);
            return images?.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }
}
