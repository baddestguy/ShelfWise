using ShelfWise.Domain.Models;

namespace ShelfWise.Repository.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default);
        Task<Book> AddAsync(Book book, CancellationToken ct = default);
    }
}
