using CircleApp.Data;
using CircleApp.Data.Helpers;
using CircleApp.Services.Interfaces;
using CircleApp.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.IO;
using Microsoft.AspNetCore.Identity;
using CircleApp.Data.Models;

// Configure Serilog early during startup
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
        .Build())
    .CreateLogger();

try
{
    Log.Information("Starting CircleApp web application...");

    var builder = WebApplication.CreateBuilder(args);

    // Register Serilog as logging provider
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddScoped<IFileService, FileService>();
    builder.Services.AddScoped<IPostService, PostService>();
    builder.Services.AddScoped<IStoryService, StoryService>();
    builder.Services.AddScoped<IHashtagService, HashtagService>();
    builder.Services.AddScoped<IFavoriteService , FavoriteService>();
    builder.Services.AddScoped<IProfileService, ProfileService>();

    // Database Configuration
    var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(dbConnectionString));
    //Identity Configuration

    builder.Services.AddIdentity<User, IdentityRole<int>>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();

    var app = builder.Build();

    // Use Serilog Request Logging
    app.UseSerilogRequestLogging();

    // Seed the database with initial data
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync(); // Apply any pending migrations
        await DbInitializer.SeedAsync(dbContext); // Seed the database with initial data
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        await DbInitializer.SeedUserAndRolesAsync(userManager, roleManager); // Seed users and roles
    }

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CircleApp application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
