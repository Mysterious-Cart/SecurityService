namespace Security;

using Data.Model;

public interface ISecurityService
{
    Task<User> CreateUserAsync(string username, string password);
    Task<User?> GetUserAsync(string userId);
    Task<bool> IsAuthenticatedAsync(string userId);
    Task<User?> Login(string username, string password);
    Task<User?> ValidateUserClaim(string userId, string claimType, string claimValue);
    Task Logout(string userId);
}