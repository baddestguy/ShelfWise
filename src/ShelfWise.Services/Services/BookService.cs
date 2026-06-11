using ShelfWise.Domain.Models;
using ShelfWise.Repository.Repositories;

namespace ShelfWise.Services.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repo;

        public BookService(IBookRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default)
        {
            return await _repo.GetAllAsync(ct);
        }

        public async Task<Book> CreateAsync(Book book, CancellationToken ct = default)
        {
            // Ensure basic invariants: TotalCopies must be non-negative
            if (book.TotalCopies < 0) book.TotalCopies = 0;

            return await _repo.AddAsync(book, ct);
        }

        public async Task<bool> UpdateAsync(int id, Book updated, CancellationToken ct = default)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return false;

            // map allowed updatable fields
            existing.Title = updated.Title;
            existing.Author = updated.Author;
            existing.Genre = updated.Genre;
            existing.Category = updated.Category;
            existing.TotalCopies = updated.TotalCopies;

            // If TotalCopies decreased below zero, clamp
            if (existing.TotalCopies < 0) existing.TotalCopies = 0;

            return await _repo.UpdateAsync(existing, ct);
        }

        public async Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(id, ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            return await _repo.DeleteAsync(id, ct);
        }

        public async Task<bool> CheckOutAsync(int bookId, int userId, int dueDays = 14, CancellationToken ct = default)
        {
            var total = await _repo.GetTotalCopiesAsync(bookId, ct);
            var checkedOut = await _repo.GetCheckedOutCountAsync(bookId, ct);
            if (checkedOut >= total)
            {
                // no copies available
                return false;
            }

            var dueAt = DateTime.UtcNow.AddDays(dueDays);
            return await _repo.TryCheckoutAsync(bookId, userId, dueAt, ct);
        }

        public async Task<bool> CheckInAsync(int bookId, int userId, CancellationToken ct = default)
        {
            return await _repo.TryCheckinAsync(bookId, userId, ct);
        }

        public async Task<int> PlaceHoldAsync(int bookId, int userId, CancellationToken ct = default)
        {
            return await _repo.CreateHoldAsync(bookId, userId, ct);
        }
    }
}
