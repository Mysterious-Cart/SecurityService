namespace Security.Data.Model;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a claim that a user possesses.
/// </summary>
public class UserClaim : IdentityUserClaim<User>
{
    // Additional properties for UserClaim can be added here if needed
}