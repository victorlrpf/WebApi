using BackendExample.Models;
using BackendExample.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var created = await _auth.RegisterAsync(req);
        if (created == null) return BadRequest(new { message = "Email já cadastrado" });
        return CreatedAtAction(nameof(Register), new { id = created.Id }, new { created.Id, created.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var token = await _auth.LoginAsync(req);
        if (token == null) return Unauthorized(new { message = "Credenciais inválidas" });
        return Ok(new { token });
    }
}