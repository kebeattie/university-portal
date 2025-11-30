using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniversityPortal.Data;

try
{
    Console.WriteLine("Starting application...");
    
    var builder = WebApplication.CreateBuilder(args);

    Console.WriteLine("Configuring services...");
    
    // Configure Kestrel to use PORT environment variable (for Railway deployment)
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5006";
    Console.WriteLine($"Using port: {port}");
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

    // Add services to the container.
    // Use environment-specific database path
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "DataSource=app.db;Cache=Shared";
    Console.WriteLine($"Connection string: {connectionString}");
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
    builder.Services.AddControllersWithViews();

    Console.WriteLine("Building application...");
    var app = builder.Build();

// Seed database with roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Starting database seeding...");
        await DbSeeder.SeedRolesAndAdminAsync(services);
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DbSeeder.SeedSampleDataAsync(context);
        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database. Error: {Message}", ex.Message);
        // Don't throw in production, just log the error
        if (!app.Environment.IsProduction())
        {
            throw;
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    
    // Apply migrations automatically in production
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Applying database migrations...");
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to apply migrations. Error: {Message}", ex.Message);
        throw;
    }
}

// Don't force HTTPS redirection in production (Railway handles SSL)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

Console.WriteLine("Starting web server...");
app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"FATAL ERROR: {ex.GetType().Name}");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"Stack: {ex.StackTrace}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner: {ex.InnerException.Message}");
    }
    throw;
}
