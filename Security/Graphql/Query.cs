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

        return await securityService.GetUserByIdAsync(userId);
    }

    /// <summary>
    /// Get user
    /// </summary>
    [Authorize(Roles = new[] { "Admin" })]
    public async Task<List<User>> GetUser([Service] ISecurityService securityService)
    {
        return await securityService.GetAllUsersAsync();
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

    // ===== GATEWAY METHODS - Proxy to other services =====

    /// <summary>
    /// Get products (proxied to ProductService)
    /// </summary>
    [Authorize]
    public async Task<List<Product>> GetProducts(
        [Service] IHttpClientFactory httpClientFactory,
        [Service] ServiceTokenService tokenService,
        ClaimsPrincipal claimsPrincipal)
    {
        var httpClient = httpClientFactory.CreateClient("ProductService");
        
        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        var userRoles = claimsPrincipal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        
        // Generate service-to-service JWT token
        var serviceToken = tokenService.GenerateServiceToken("ProductService", userId, userRoles);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serviceToken);
        
        var query = @"
            query {
                products {
                    id
                    name
                    price
                    farmerId
                }
            }";
        
        var response = await httpClient.PostAsJsonAsync("graphql", new { query });
        var content = await response.Content.ReadAsStringAsync();
        
        // Parse response and return products
        return new List<Product>(); // Placeholder
    }

    /// <summary>
    /// Get orders for current user (proxied to OrderService)
    /// </summary>
    [Authorize]
    public async Task<List<Order>> GetMyOrders(
        [Service] IHttpClientFactory httpClientFactory,
        ClaimsPrincipal claimsPrincipal)
    {
        var httpClient = httpClientFactory.CreateClient("OrderService");
        var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        
        var query = $@"
            query {{
                ordersByUser(userId: ""{userId}"") {{
                    id
                    total
                    status
                    createdAt
                }}
            }}";
        
        var response = await httpClient.PostAsJsonAsync("graphql", new { query });
        // Process response...
        
        return new List<Order>(); // Placeholder
    }
}

// ===== PLACEHOLDER TYPES FOR GATEWAY =====
// These would be defined properly based on your other services

public record Product(string Id, string Name, decimal Price, string FarmerId);
public record Order(string Id, decimal Total, string Status, DateTime CreatedAt);
