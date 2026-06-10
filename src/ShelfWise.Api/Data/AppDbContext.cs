using Microsoft.EntityFrameworkCore;
using ShelfWise.Api.Models;

namespace ShelfWise.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; } = null!;
    }
}
