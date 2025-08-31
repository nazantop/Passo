using Microsoft.AspNetCore.Mvc;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Application.IServices;

namespace PassoCourseApp.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        => Ok(await authService.RegisterAsync(request));


    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        => Ok(await authService.LoginAsync(request));
}