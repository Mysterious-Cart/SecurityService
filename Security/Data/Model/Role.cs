namespace Security.Data.Model;

using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents user roles.
/// </summary>

[Table("AspNetRoles")]
public class Role : IdentityRole
{
}