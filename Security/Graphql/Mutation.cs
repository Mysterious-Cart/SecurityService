using Security.Data.Model;
using Security.Service;
using HotChocolate.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Security.Graphql;

public class Mutation
{
    /// <summary>
    /// Register a new user
    /// </summary>
    public async Task<AuthPayload> Register(
        [Service] ISecurityService securityService,
        string username,
        string password,
        string? email = null)
    {
        try
        {
            if(await securityService.GetUserByNameAsync(username) is not null)
            {
                return new AuthPayload(null, "Username already exists");
            }
            var user = await securityService.CreateUserAsync(username, password);
            return new AuthPayload(user, "User created successfully");
        }
        catch (Exception ex)
        {
            return new AuthPayload(null, ex.Message);
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    public async Task<AuthPayload> Login(
        [Service] ISecurityService securityService,
        [Service] SignInManager<User> signInManager,
        string username,
        string password)
    {
        try
        {
            
            var user = await securityService.Login(username, password);
            if(user == null)
            {
                return new AuthPayload(null, "Invalid username or password");
            }
            
            await signInManager.SignInAsync(user, false);
            return new AuthPayload(user, "Login successful");
        }
        catch (Exception ex)
        {
            return new AuthPayload(null, ex.Message);
        }
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [Authorize]
    public async Task<bool> Logout(
        [Service] ISecurityService securityService,
        ClaimsPrincipal claimsPrincipal)
    {
        try
        {
            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return false;

            await securityService.Logout(userId);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Grant permission to user (Admin only)
    /// </summary>
    [Authorize(Roles = new[] { "Admin" })]
    public async Task<bool> GrantPermission(
        [Service] ISecurityService securityService,
        string userId,
        string permission)
    {
        try
        {
            // This would need to be implemented in your SecurityService
            // await securityService.GrantPermissionAsync(userId, permission);
            return true;
        }
        catch
        {
            return false;
        }
    }
}


