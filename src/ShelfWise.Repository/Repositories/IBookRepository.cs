using ShelfWise.Domain.Models;

namespace ShelfWise.Repository.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync(CancellationToken ct = default);
        Task<Book> AddAsync(Book book, CancellationToken ct = default);
        Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateAsync(Book book, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
