using System.ComponentModel.DataAnnotations;

namespace AWEfinal.UI.Models.Dto;

public class UserProfileResponse
{
    public UserDto User { get; set; } = new UserDto();
    public UserStatsDto Stats { get; set; } = new UserStatsDto();
}

public class UserStatsDto
{
    public int TotalOrders { get; set; }
    public int DeliveredOrders { get; set; }
    public int InProgressOrders { get; set; }
    public decimal TotalSpent { get; set; }
}

public class UpdateProfileRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Phone { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}
