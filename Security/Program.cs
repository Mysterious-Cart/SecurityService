using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Security;
using Security.Data;
using Security.Data.Model;
using Security.Service;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<SecurityContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddIdentityCore<User>()
    .AddRoles<Role>()
    .AddEntityFrameworkStores<SecurityContext>()
    .AddUserManager<UserManager<User>>()
    .AddSignInManager<SignInManager<User>>()
    .AddRoleManager<RoleManager<Role>>()
    .AddDefaultTokenProviders();

builder.Services
    .AddScoped<ISecurityService, SecurityService>()
    .AddScoped<ServiceTokenService>()
    .AddAuthorization()
    .AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Security.Graphql.Query>()
    .AddMutationType<Security.Graphql.Mutation>()
    .AddAuthorization();

//------ Add HTTP clients for other services
builder.Services.AddHttpClient("ProductService", client =>
{
    client.BaseAddress = new Uri("https://localhost:5002/");
});

builder.Services.AddHttpClient("OrderService", client =>
{
    client.BaseAddress = new Uri("https://localhost:5003/");
});

builder.Services.AddHttpClient("PaymentService", client =>
{
    client.BaseAddress = new Uri("https://localhost:5004/");
});

builder.Services.AddControllers();

var app = builder.Build();

// Custom middleware to forward authentication to downstream services
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/graphql"))
    {
        // Extract user info and add to headers for downstream services
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRoles = string.Join(",", context.User.FindAll(ClaimTypes.Role).Select(c => c.Value));
            
            context.Request.Headers["X-User-Id"] = userId;
            context.Request.Headers["X-User-Roles"] = userRoles;
        }
    }
    
    await next();
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(i => i
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
app.MapGraphQL();
app.MapControllers();

app.Run();
