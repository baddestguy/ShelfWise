using Microsoft.EntityFrameworkCore;
using ShelfWise.Domain.Models;

namespace ShelfWise.Repository.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<BorrowRecord> BorrowRecords { get; set; } = null!;
        public DbSet<HoldRecord> HoldRecords { get; set; } = null!;
    }
}
