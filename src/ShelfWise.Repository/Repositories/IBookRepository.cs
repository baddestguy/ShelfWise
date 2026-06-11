using ShelfWise.Domain.Models;

namespace ShelfWise.Repository.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<BookInventoryItem>> SearchInventoryAsync(string? search, CancellationToken ct = default);
        Task<BookInventoryItem?> GetInventoryByIdAsync(int id, CancellationToken ct = default);
        Task<Book> AddAsync(Book book, CancellationToken ct = default);
        Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateAsync(Book book, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> TryCheckoutAsync(int bookId, int userId, DateTime dueAt, CancellationToken ct = default);
        Task<bool> TryCheckinAsync(int bookId, int userId, CancellationToken ct = default);
        Task<int> GetCheckedOutCountAsync(int bookId, CancellationToken ct = default);
        Task<int> GetTotalCopiesAsync(int bookId, CancellationToken ct = default);
        Task<int> CreateHoldAsync(int bookId, int userId, CancellationToken ct = default);
        Task<BorrowRecord?> GetActiveBorrowRecordAsync(int bookId, int userId, CancellationToken ct = default);
    }
}
