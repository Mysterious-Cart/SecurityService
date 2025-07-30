namespace Security.Data;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model;

public class SecurityContext : IdentityDbContext<User, Role, string>
{
    public SecurityContext(DbContextOptions<SecurityContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("security");
        base.OnModelCreating(builder);
        // Additional model configuration can go here
    }
}