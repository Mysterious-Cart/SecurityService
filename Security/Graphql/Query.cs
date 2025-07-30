using Security.Data.Model;
using Security.Service;
using HotChocolate.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace Security.Graphql;

public class Query
{
    /// <summary>
    /// Get current user information
    /// </summary>
    [Authorize]
    public async Task<User?> Me([Service] ISecurityService securityService, ClaimsPrincipal claimsPrincipal)
    {
        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return null;

        return await securityService.GetUserById(userId);
    }

    /// <summary>
    /// Get user
    /// </summary>
    [Authorize(Roles = new[] { "Admin" })]
    public async Task<List<User>> GetUser([Service] ISecurityService securityService)
    {
        return await securityService.GetAllUsers();
    }

    /// <summary>
    /// Check if user has specific permission
    /// </summary>
    [Authorize]
    public async Task<bool> HasPermission(
        [Service] ISecurityService securityService, 
        ClaimsPrincipal claimsPrincipal,
        string permission)
    {
        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return false;

        try
        {
            await securityService.ValidateUserClaim(userId, "permission", permission);
            return true;
        }
        catch
        {
            return false;
        }
    }
}