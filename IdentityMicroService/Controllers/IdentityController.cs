// Controllers/IdentityController.cs
using IdentityMicroService.Models;
using IdentityMicroService.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IJWTService _jwtService;

    public IdentityController(IJWTService jwtService)
    {
        _jwtService = jwtService;
    }

    // Your existing GET endpoint that works
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "Identity service is running" });
    }

    // Add the auth endpoints here
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // TODO: Replace with actual user validation from database
        if (ValidateUser(request.Username, request.Password))
        {
            var token = _jwtService.GenerateToken(request.Username, new[] { "User" });
            var expires = DateTime.UtcNow.AddMinutes(60);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = request.Username,
                Expires = expires
            });
        }

        return Unauthorized(new { message = "Invalid credentials" });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] LoginRequest request)
    {
        // TODO: Implement user registration logic
        return Ok(new { message = "User registered successfully" });
    }

    private bool ValidateUser(string username, string password)
    {
        // TODO: Replace with actual database validation
        return username == "admin" && password == "password";
    }
}

