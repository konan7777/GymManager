namespace GymManager.Api.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public long GymId { get; set; }

    public string Email { get; set; } = string.Empty;
}