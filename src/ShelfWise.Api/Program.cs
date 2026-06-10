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

app.UseRouting();

// register repository and services
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // DbContext and repositories registered in DI below
}

app.MapControllers();

app.Run();
