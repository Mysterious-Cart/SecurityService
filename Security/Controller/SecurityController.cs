namespace Security.Controller;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Security.Data.Model;

[ApiController]
[Route("api/[controller]")]
public class SecurityController(UserManager<User> userManager) : ControllerBase
{
    private readonly UserManager<User> _userManager = userManager;

    [HttpGet("secure-endpoint")]
    [Authorize]
    public async Task<IActionResult> Register(string Username, string Email, string Password)
    {
        var userProfile = new User { UserName = Username, Email = Email };
        var hashedPassword = _userManager.PasswordHasher.HashPassword(userProfile, Password);
        var user = new User
        {
            UserName = Username,
            Email = Email,
            PasswordHash = hashedPassword
        };
        try
        {
            await _userManager.CreateAsync(user);
            return Ok(new { Message = "User registered successfully." });
        }
        catch (Exception exc)
        {
            return StatusCode(500, new { Message = "An error occurred while registering the user.", Details = exc.Message });
        }
        
    }
}
