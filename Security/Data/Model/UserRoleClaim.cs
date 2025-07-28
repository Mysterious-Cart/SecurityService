namespace Security.Data.Model;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a claim that's granted to all users within a role.
/// </summary>
public class UserRoleClaim : IdentityRoleClaim<User>
{
    // Additional properties for UserRoleClaim can be added here if needed
}