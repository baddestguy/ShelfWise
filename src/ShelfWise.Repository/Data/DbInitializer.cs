using Microsoft.EntityFrameworkCore;
using ShelfWise.Domain.Models;

namespace ShelfWise.Repository.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Apply migrations if any exist; otherwise ensure database/tables are created
            try
            {
                var pending = context.Database.GetPendingMigrations();
                if (pending != null && pending.Any())
                {
                    context.Database.Migrate();
                }
                else
                {
                    context.Database.EnsureCreated();
                }
            }
            catch
            {
                context.Database.EnsureCreated();
            }

            // Seed data if none exists
            if (!context.Books.Any())
            {
                var books = new List<Book>
                {
                    new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Category = Category.NonFiction, Genre = "Technical", Available = 3, OnHold = 0, TotalCopies = 3 },
                    new Book { Title = "Clean Code", Author = "Robert C. Martin", Category = Category.NonFiction, Genre = "Technical", Available = 2, OnHold = 0, TotalCopies = 2 },
                    new Book { Title = "Domain-Driven Design", Author = "Eric Evans", Category = Category.NonFiction, Genre = "Technical", Available = 1, OnHold = 0, TotalCopies = 1 }
                };

                context.Books.AddRange(books);
                context.SaveChanges();
            }
        }
    }
}
