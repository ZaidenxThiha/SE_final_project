using AWEfinal.BLL.Services;
using AWEfinal.UI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AWEfinal.UI.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthApiController> _logger;

    public AuthApiController(IUserService userService, ILogger<AuthApiController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Ok(null);
        }

        var user = await _userService.GetUserByIdAsync(userId.Value);
        if (user == null)
        {
            HttpContext.Session.Clear();
            return Ok(null);
        }

        return Ok(user.ToDto());
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var user = await _userService.LoginUserAsync(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        SetSession(user);
        return Ok(user.ToDto());
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var user = await _userService.RegisterUserAsync(request.Email, request.Password, request.Name);
        SetSession(user);
        return Ok(user.ToDto());
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return NoContent();
    }

    private void SetSession(AWEfinal.DAL.Models.User user)
    {
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.Name);
        HttpContext.Session.SetString("UserEmail", user.Email);
        HttpContext.Session.SetString("UserRole", user.Role);
    }
}
