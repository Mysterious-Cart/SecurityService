namespace Security.Data.Model;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents user roles.
/// </summary>
public class Role : IdentityRole
{
    public Role() : base()
    {
    }

    public Role(string roleName) : base(roleName)
    {
    }
}