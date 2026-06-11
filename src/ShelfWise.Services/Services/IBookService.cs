using ShelfWise.Domain.Models;

namespace ShelfWise.Services.Services
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<BookInventoryItem>> SearchInventoryAsync(string? search, CancellationToken ct = default);
        Task<BookInventoryItem?> GetInventoryByIdAsync(int id, CancellationToken ct = default);
        Task<Book> CreateAsync(Book book, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, Book updated, CancellationToken ct = default);
        Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> CheckOutAsync(int bookId, int userId, int dueDays = 14, CancellationToken ct = default);
        Task<bool> CheckInAsync(int bookId, int userId, CancellationToken ct = default);
        Task<int> PlaceHoldAsync(int bookId, int userId, CancellationToken ct = default);
    }
}
