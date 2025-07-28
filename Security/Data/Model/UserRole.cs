namespace Security.Data.Model;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// A join entity that associates a user with a role.
/// </summary>
public class UserRole : IdentityUserRole<User>
{
    // Additional properties for UserRole can be added here if needed
}