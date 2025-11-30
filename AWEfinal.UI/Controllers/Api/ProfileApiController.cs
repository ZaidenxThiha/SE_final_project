using AWEfinal.BLL.Services;
using AWEfinal.UI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AWEfinal.UI.Controllers.Api;

[ApiController]
[Route("api/profile")]
public class ProfileApiController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;

    public ProfileApiController(IUserService userService, IOrderService orderService)
    {
        _userService = userService;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return Unauthorized();

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null)
            return Unauthorized();

        var orders = await _orderService.GetOrdersByUserIdAsync(user.Id);
        var stats = new UserStatsDto
        {
            TotalOrders = orders.Count(),
            DeliveredOrders = orders.Count(o => o.Status == "delivered"),
            InProgressOrders = orders.Count(o => o.Status is "pending" or "paid" or "packaging" or "shipped"),
            TotalSpent = orders.Where(o => o.Status is "paid" or "packaging" or "shipped" or "delivered").Sum(o => o.Total)
        };

        return Ok(new UserProfileResponse
        {
            User = user.ToDto(),
            Stats = stats
        });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return Unauthorized();

        var updated = await _userService.UpdateProfileAsync(
            userId.Value,
            request.Name,
            request.Phone,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.PostalCode,
            request.Country
        );

        return Ok(updated.ToDto());
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return Unauthorized();

        await _userService.ChangePasswordAsync(userId.Value, request.CurrentPassword, request.NewPassword);
        return NoContent();
    }
}
