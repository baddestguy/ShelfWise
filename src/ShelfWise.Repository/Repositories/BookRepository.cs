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

        public async Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Books.FindAsync(new object[] { id }, ct);
        }

        public async Task<bool> UpdateAsync(Book book, CancellationToken ct = default)
        {
            _db.Books.Update(book);
            var changed = await _db.SaveChangesAsync(ct);
            return changed > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.Books.FindAsync(new object[] { id }, ct);
            if (entity == null) return false;
            _db.Books.Remove(entity);
            var changed = await _db.SaveChangesAsync(ct);
            return changed > 0;
        }
    }
}
