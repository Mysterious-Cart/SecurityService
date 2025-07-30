namespace Security;

using Data.Model;

public interface ISecurityService
{
    Task<User> CreateUser(string username, string password);
    //Task<bool> DeleteUser(string userId);
    Task<List<User>> GetAllUsers();
    Task<User?> GetUserById(string userId);
    Task<User?> GetUserByName(string username);
    Task<bool> IsAuthenticated(string userId);
    Task<User?> Login(string username, string password);
    Task<User?> ValidateUserClaim(string userId, string claimType, string claimValue);
    Task Logout();

    // ==== ROLE METHOD ====

    Task<Role> CreateRole(string roleName);
    Task<bool> AssignRole(string userId, string roleId);
    //Task<bool> DeleteRole(string roleId);


}