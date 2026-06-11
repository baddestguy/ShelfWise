using ShelfWise.Domain.Models;

namespace ShelfWise.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default);
        Task<Book> CreateAsync(Book book, CancellationToken ct = default);
    }
}
