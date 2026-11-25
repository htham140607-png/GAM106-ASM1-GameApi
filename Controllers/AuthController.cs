using GameApi.DTOs;
using GameApi.Services;
using Microsoft.AspNetCore.Mvc;
namespace GameApi.Controllers;
[ApiController, Route("api/auth")]
public class AuthController(AuthService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await service.Register(request);
        return result is null ? Conflict(new { message = "Tên đăng nhập đã tồn tại" }) : StatusCode(201, result);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await service.Login(request);
        return result is null ? Unauthorized(new { message = "Sai tên đăng nhập hoặc mật khẩu" }) : Ok(result);
    }
}
