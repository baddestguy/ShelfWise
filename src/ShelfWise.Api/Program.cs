using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using ShelfWise.Api.Auth;
using ShelfWise.Api.Services;
using ShelfWise.Repository.Data;
using ShelfWise.Repository.Repositories;
using ShelfWise.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services
    .AddAuthentication(DemoRoleAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, DemoRoleAuthenticationHandler>(
        DemoRoleAuthenticationHandler.SchemeName,
        options => { });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("LibrarianOrAdmin", policy => policy.RequireRole("Librarian", "Admin"));
});

// Add DbContext (connection string placed in appsettings in later step)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

// register repository and service implementations
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddHttpClient<IAiBookSearchService, AiBookSearchService>();

var app = builder.Build();

// Apply pending EF Core migrations at startup so Docker/devs don't run manual commands
using (var scope = app.Services.CreateScope())
{
    try
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        logger.LogInformation("Applying database migrations...");
        db.Database.Migrate();
        logger.LogInformation("Database migrations applied.");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or initializing the database.");
        throw;
    }
}

// Ensure the app listens on all container interfaces
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:80");

// Ensure schema exists and seed demo data. This runs in production too so hosted demos
// can start from an empty managed database.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        DbInitializer.Initialize(db);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
