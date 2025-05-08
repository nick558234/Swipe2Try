using System;
using Swipe2Try.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register repositories
builder.Services.AddScoped<Swipe2Try.Core.Interfaces.IDishRepository, Swipe2Try.DAL.Repositories.DishRepository>();
builder.Services.AddScoped<Swipe2Try.Core.Interfaces.IUserRepository, Swipe2Try.DAL.Repositories.UserRepository>();
builder.Services.AddScoped<Swipe2Try.Core.Interfaces.IRoleRepository, Swipe2Try.DAL.Repositories.RoleRepository>();

// Register validators
builder.Services.AddScoped<Swipe2Try.Core.Interfaces.IUserValidator, Swipe2Try.Core.Validation.UserValidator>();

// Register managers
builder.Services.AddScoped<Swipe2Try.Core.Interfaces.IUserManager, Swipe2Try.Core.Managers.UserManager>();
builder.Services.AddScoped<Swipe2Try.Core.Interfaces.IRoleManager, Swipe2Try.Core.Managers.RoleManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Enable session
app.UseSession();

// Add custom authorization middleware
app.UseMiddleware<AuthorizationMiddleware>();

app.MapRazorPages();

app.Run();