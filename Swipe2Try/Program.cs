using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Validation;
using Swipe2Try.DAL.Repositories;
using Swipe2Try.Core.Managers;

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
builder.Services.AddScoped<IDishRepository, DishRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Register validators
builder.Services.AddScoped<IUserValidator, UserValidator>();
builder.Services.AddScoped<ICategoryValidator, CategoryValidator>();
builder.Services.AddScoped<IDishValidator, DishValidator>();
builder.Services.AddScoped<IRestaurantValidator, RestaurantValidator>();

// Register managers (both concrete classes and interfaces)
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<RoleManager>();
builder.Services.AddScoped<IRoleManager, RoleManager>();
builder.Services.AddScoped<DishManager>();
builder.Services.AddScoped<RestaurantManager>();
builder.Services.AddScoped<CategoryManager>();

// Add built-in cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/Error";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // Set cookie expiration to 30 minutes
        options.SlidingExpiration = true; // Reset expiration time with each request
    });

// Add authorization without custom policies
builder.Services.AddAuthorization();

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

// Enable session
app.UseSession();

// Add built-in authentication/authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();