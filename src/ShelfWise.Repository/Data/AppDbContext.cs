using Microsoft.EntityFrameworkCore;
using ShelfWise.Domain.Models;

namespace ShelfWise.Repository.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; } = null!;
    }
}
