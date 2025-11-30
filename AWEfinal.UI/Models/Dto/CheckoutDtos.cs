using System.ComponentModel.DataAnnotations;

namespace AWEfinal.UI.Models.Dto;

public class CheckoutRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;
    [Required]
    public string Phone { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string PostalCode { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    public string PaymentMethod { get; set; } = string.Empty;
}

public class UpdateOrderStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
}

public class CartMutationRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}

public class UpdateCartQuantityRequest
{
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
}

public class CartResponse
{
    public List<CartItem> Items { get; init; } = new();
    public int TotalItems => Items.Sum(i => i.Quantity);
    public decimal Total => Items.Sum(i => i.Subtotal);
}
