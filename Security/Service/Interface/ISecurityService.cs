namespace Security;

using Data.Model;

public interface ISecurityService
{
    Task<User> CreateUserAsync(string username, string password);
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(string userId);
    Task<User?> GetUserByNameAsync(string username);
    Task<bool> IsAuthenticatedAsync(string userId);
    Task<User?> Login(string username, string password);
    Task<User?> ValidateUserClaim(string userId, string claimType, string claimValue);
    Task Logout(string userId);
}