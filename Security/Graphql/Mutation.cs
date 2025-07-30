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
            if(await securityService.GetUserByName(username) is not null)
            {
                return new AuthPayload(false,null, "Username already exists");
            }
            var user = await securityService.CreateUser(username, password);
            return new AuthPayload(true,user, "User created successfully");
        }
        catch (Exception ex)
        {
            return new AuthPayload(false,null, ex.Message);
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    public async Task<AuthPayload> Login(
        [Service] ISecurityService securityService,
        string username,
        string password)
    {
        try
        {
            
            var user = await securityService.Login(username, password);
            if(user == null)
            {
                return new AuthPayload(false,null, "Invalid username or password");
            }
            return new AuthPayload(true,user, "Login successful");
        }
        catch (Exception ex)
        {
            return new AuthPayload(false,null, ex.Message);
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

            await securityService.Logout();
            return true;
        }
        catch
        {
            return false;
        }
    }
    [Authorize]
    public async Task<RolePayload> CreateRole(
        [Service] ISecurityService securityService,
        string roleName
    )
    {
        var role = await securityService.CreateRole(roleName);
        return new RolePayload(true, role, "Role created successfully");
    }
    [Authorize]
    public async Task<bool> AssignRole(
        [Service] ISecurityService securityService,
        string userId,
        string role)
    {
        var user = await securityService.GetUserById(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        if (role == null)
        {
            throw new InvalidOperationException("Invalid role.");
        }
        return true;
        // Assign the role to the user
        // This would need to be implemented in your SecurityService
        // await securityService.AssignRoleToUserAsync(user, role);
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


