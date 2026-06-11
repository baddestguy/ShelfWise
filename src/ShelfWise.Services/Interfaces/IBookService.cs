using ShelfWise.Domain.Models;

namespace ShelfWise.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default);
        Task<Book> CreateAsync(Book book, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, Book updated, CancellationToken ct = default);
        Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
