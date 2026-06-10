using Microsoft.EntityFrameworkCore;
using ShelfWise.Repository.Data;
using ShelfWise.Repository.Repositories;
using ShelfWise.Services.Interfaces;
using ShelfWise.Services.Services;
using ShelfWise.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext (connection string placed in appsettings in later step)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// register repository and service implementations
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

// Ensure the app listens on all container interfaces
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:80");

// Apply migrations / seed database (development-friendly)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var env = services.GetRequiredService<IHostEnvironment>();
        var db = services.GetRequiredService<AppDbContext>();

        // Run migrations and seed only in Development, or when SEED_DB=true is set
        var seedFlag = Environment.GetEnvironmentVariable("SEED_DB");
        if (env.IsDevelopment() || string.Equals(seedFlag, "true", StringComparison.OrdinalIgnoreCase))
        {
            DbInitializer.Initialize(db);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.UseRouting();

app.MapControllers();

app.Run();
