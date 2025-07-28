namespace Security.Data.Model;
using Microsoft.AspNetCore.Identity;


/// <summary>
/// Represents an Authentication token for a user.
/// </summary>
public class UserToken : IdentityUserToken<User>
{
    // Additional properties for UserToken can be added here if needed
}