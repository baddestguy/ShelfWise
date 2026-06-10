using ShelfWise.Domain.Models;
using ShelfWise.Repository.Repositories;
using ShelfWise.Services.Interfaces;

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
    }
}
