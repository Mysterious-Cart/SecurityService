namespace Security.Service;

using Security.Data.Model;
using Security;
using Security.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class SecurityService(
        IDbContextFactory<SecurityContext> dbContextFactory,
        UserManager<User> userManager
    ) : ISecurityService
{
    private readonly SecurityContext _context = dbContextFactory.CreateDbContext();
    private readonly UserManager<User> _userManager = userManager;

    public async Task<User> CreateUserAsync(string username, string password)
    {
        var user = new User { UserName = username, Email = username };
        try
        {
            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                return user;
            }

            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"An error occurred while creating user: {ex.Message}");
        }
    }

    public async Task<User?> GetUserByIdAsync(string userId) => await _userManager.FindByIdAsync(userId);
    public async Task<User?> GetUserByNameAsync(string username) => await _userManager.FindByNameAsync(username);
    public async Task<List<User>> GetAllUsersAsync() => await _userManager.Users.ToListAsync();

    public async Task<bool> IsAuthenticatedAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user != null;
    }

    public async Task<User?> Login(string username, string password)
    {
        if (username == "admin" && password == "admin")
        {
            // Special case for admin login for development purposes only
            return new User
            {
                UserName = username,
                Email = username,
            };
        }
        var user = await _userManager.FindByNameAsync(username);
        if (user != null && await _userManager.CheckPasswordAsync(user, password))
        {
            // Add login timestamp claim to track when user logged in
            var loginClaim = new Claim("last_login", DateTimeOffset.UtcNow.ToString());
            await _userManager.AddClaimAsync(user, loginClaim);
            return user;
        }
        else
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }
    }
    public async Task<User?> ValidateUserClaim(string userId, string claimType, string claimValue)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }
        var userClaims = await _userManager.GetClaimsAsync(user);
        var hasClaim = userClaims.Any(c => c.Type == claimType && c.Value == claimValue);
        
        if (hasClaim)
        {
            return user;
        }
        else
        {
            throw new UnauthorizedAccessException("User does not have the required claim.");
        }
    }

    public async Task Logout(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            // Remove the last_login claim to indicate logout
            var userClaims = await _userManager.GetClaimsAsync(user);
            var loginClaim = userClaims.FirstOrDefault(c => c.Type == "last_login");
            
            if (loginClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, loginClaim);
            }
        }
        else
        {
            throw new InvalidOperationException("User not found.");
        }
    }
}