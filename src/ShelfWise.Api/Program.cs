using Microsoft.EntityFrameworkCore;
using ShelfWise.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DbContext (connection string placed in appsettings in later step)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Ensure the app listens on all container interfaces
app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:80");

app.UseRouting();

app.MapControllers();

app.Run();
