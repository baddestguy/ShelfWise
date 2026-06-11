using Microsoft.EntityFrameworkCore;
using ShelfWise.Domain.Models;
using ShelfWise.Repository.Data;

namespace ShelfWise.Repository.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _db;

        public BookRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.Books.AsNoTracking().ToListAsync(ct);
        }

        public async Task<Book> AddAsync(Book book, CancellationToken ct = default)
        {
            var entry = await _db.Books.AddAsync(book, ct);
            await _db.SaveChangesAsync(ct);
            return entry.Entity;
        }
    }
}
