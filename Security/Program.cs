using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Security;
using Security.Data;
using Security.Data.Model;
using Security.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPooledDbContextFactory<SecurityContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<SecurityContext>()
    .AddUserManager<UserManager<User>>()
    .AddDefaultTokenProviders();

builder.Services
    .AddScoped<ISecurityService, SecurityService>()
    .AddAuthorization()
    .AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Hello World!");

app.Run();
