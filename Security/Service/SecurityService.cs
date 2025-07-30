namespace Security.Service;

using Security.Data.Model;
using Security;
using Security.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class SecurityService(
        IDbContextFactory<SecurityContext> dbContextFactory,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager
    ) : ISecurityService
{
    private readonly SecurityContext _context = dbContextFactory.CreateDbContext();
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly RoleManager<Role> _roleManager = roleManager;

    public async Task<User> CreateUser(string username, string password)
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

    public async Task<User?> GetUserById(string userId) => await _userManager.FindByIdAsync(userId.ToString());
    public async Task<User?> GetUserByName(string username) => await _userManager.FindByNameAsync(username);
    public async Task<List<User>> GetAllUsers() => await _userManager.Users.ToListAsync();

    public async Task<bool> IsAuthenticated(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user != null;
    }

    public async Task<User?> Login(string username, string password)
    {

        var user = await _userManager.FindByNameAsync(username);
        if (user != null && await _userManager.CheckPasswordAsync(user, password))
        {
            // Add login timestamp claim to track when user logged in
            //var loginClaim = new Claim("last_login", DateTimeOffset.UtcNow.ToString());
            //await _userManager.AddClaimAsync(user, loginClaim);
            await _signInManager.SignInAsync(user, isPersistent: false);
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

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }

    // ==== ROLE METHOD ====

    public async Task<Role> CreateRole(string roleName)
    {
        var result = await _roleManager.CreateAsync(new Role() { Name = roleName });
        if (result.Succeeded)
        {
            return await _roleManager.FindByNameAsync(roleName)
            ?? throw new InvalidOperationException("Role creation failed.");
        }
        else
        {
            throw new InvalidOperationException(
                $"Failed to create role: {string.Join(", ", result.Errors.Select(e => e.Description))}"
            );
        }
    }

    public async Task<bool> AssignRole(string userId, string roleId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var role = await _roleManager.FindByIdAsync(roleId);

        if (user == null || role == null)
        {
            throw new InvalidOperationException("User or role not found.");
        }

        var result = await _userManager.AddToRoleAsync(user, role.Name);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to assign role: {string.Join(", ", result.Errors.Select(e => e.Description))}"
            );
        }
        return true;
    }
}