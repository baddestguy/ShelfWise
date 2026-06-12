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

        public async Task<IEnumerable<BookInventoryItem>> SearchInventoryAsync(string? search, CancellationToken ct = default)
        {
            var query = _db.Books.AsNoTracking();
            var trimmedSearch = search?.Trim();

            if (!string.IsNullOrWhiteSpace(trimmedSearch))
            {
                var lowered = trimmedSearch.ToLower();
                var matchesCategory = Enum.TryParse<Category>(trimmedSearch, true, out var category);
                query = query.Where(b =>
                    b.Title.ToLower().Contains(lowered) ||
                    b.Author.ToLower().Contains(lowered) ||
                    b.Genre.ToLower().Contains(lowered) ||
                    (matchesCategory && b.Category == category));
            }

            return await query
                .OrderBy(b => b.Title)
                .Select(b => new BookInventoryItem
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Category = b.Category,
                    Genre = b.Genre,
                    TotalCopies = b.TotalCopies,
                    CheckedOutCopies = _db.BorrowRecords.Count(r => r.BookId == b.Id && r.ReturnedAt == null),
                    AvailableCopies = b.TotalCopies - _db.BorrowRecords.Count(r => r.BookId == b.Id && r.ReturnedAt == null)
                })
                .ToListAsync(ct);
        }

        public async Task<BookInventoryItem?> GetInventoryByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Books
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BookInventoryItem
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Category = b.Category,
                    Genre = b.Genre,
                    TotalCopies = b.TotalCopies,
                    CheckedOutCopies = _db.BorrowRecords.Count(r => r.BookId == b.Id && r.ReturnedAt == null),
                    AvailableCopies = b.TotalCopies - _db.BorrowRecords.Count(r => r.BookId == b.Id && r.ReturnedAt == null)
                })
                .FirstOrDefaultAsync(ct);
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

        public async Task<int> GetCheckedOutCountAsync(int bookId, CancellationToken ct = default)
        {
            return await _db.BorrowRecords.CountAsync(b => b.BookId == bookId && b.ReturnedAt == null, ct);
        }

        public async Task<int> GetTotalCopiesAsync(int bookId, CancellationToken ct = default)
        {
            var book = await _db.Books.FindAsync(new object[] { bookId }, ct);
            return book?.TotalCopies ?? 0;
        }

        public async Task<bool> TryCheckoutAsync(int bookId, int userId, DateTime dueAt, CancellationToken ct = default)
        {
            using var tx = await _db.Database.BeginTransactionAsync(ct);
            var checkedOut = await GetCheckedOutCountAsync(bookId, ct);
            var book = await _db.Books.FindAsync(new object[] { bookId }, ct);
            if (book == null) return false;

            var alreadyCheckedOutToUser = await _db.BorrowRecords.AnyAsync(
                b => b.BookId == bookId && b.UserId == userId && b.ReturnedAt == null,
                ct);
            if (alreadyCheckedOutToUser) return false;

            if (checkedOut >= book.TotalCopies)
            {
                // no copies available
                return false;
            }

            var record = new BorrowRecord
            {
                BookId = bookId,
                UserId = userId,
                CheckedOutAt = DateTime.UtcNow,
                DueAt = dueAt
            };
            await _db.BorrowRecords.AddAsync(record, ct);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return true;
        }

        public async Task<BorrowRecord?> GetActiveBorrowRecordAsync(int bookId, int userId, CancellationToken ct = default)
        {
            return await _db.BorrowRecords.FirstOrDefaultAsync(b => b.BookId == bookId && b.UserId == userId && b.ReturnedAt == null, ct);
        }

        public async Task<bool> TryCheckinAsync(int bookId, int userId, CancellationToken ct = default)
        {
            using var tx = await _db.Database.BeginTransactionAsync(ct);
            var record = await GetActiveBorrowRecordAsync(bookId, userId, ct);
            if (record == null) return false;
            record.ReturnedAt = DateTime.UtcNow;
            _db.BorrowRecords.Update(record);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return true;
        }

        public async Task<int> CreateHoldAsync(int bookId, int userId, CancellationToken ct = default)
        {
            var hold = new HoldRecord
            {
                BookId = bookId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            await _db.HoldRecords.AddAsync(hold, ct);
            await _db.SaveChangesAsync(ct);
            return hold.Id;
        }
    }
}
