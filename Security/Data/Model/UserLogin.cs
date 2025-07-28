namespace Security.Data.Model;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Associates a user with a login.
/// </summary>
public class UserLogin : IdentityUserLogin<User>
{
    // Additional properties for UserLogin can be added here if needed
}