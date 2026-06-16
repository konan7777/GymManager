using BCrypt.Net;
using GymManager.Api.Data;
using GymManager.Api.DTOs.Auth;
using GymManager.Api.Entities;
using GymManager.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GymDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(
        GymDbContext context,
        JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponse>> Register(RegisterRequest request)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email.ToLower() == request.Email.ToLower());

        if (existingUser != null)
        {
            return BadRequest("Email already exists.");
        }

        var gym = new Gym
        {
            Name = request.GymName
        };

        _context.Gyms.Add(gym);

        var user = new User
        {
            Gym = gym,
            Email = request.Email,
            Username = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user);

        return Ok(new LoginResponse
        {
            Token = token,
            GymId = user.GymId,
            Email = user.Email
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email.ToLower() == request.Email.ToLower());

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var passwordValid =
            BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            return Unauthorized("Invalid email or password.");
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new LoginResponse
        {
            Token = token,
            GymId = user.GymId,
            Email = user.Email
        });
    }
}