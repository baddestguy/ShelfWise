using ShelfWise.Domain.Models;
using ShelfWise.Repository.Repositories;
using ShelfWise.Services.Interfaces;
using System;

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
            // Ensure basic invariants: set available to TotalCopies if not set
            if (book.TotalCopies > 0 && book.Available == 0)
            {
                book.Available = book.TotalCopies;
            }

            return await _repo.AddAsync(book, ct);
        }
    }
}
