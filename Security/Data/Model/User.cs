namespace Security.Data.Model;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Represents a user
/// </summary>
public class User : IdentityUser, IEquatable<User>
{   
    public bool Equals(User? other)
    {
        if (other is null) return false;
        return string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is User user && Equals(user);
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }
}