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
            existing.OnHold = updated.OnHold;

            // Adjust Available if TotalCopies changed and Available would exceed it
            if (existing.TotalCopies < existing.Available)
            {
                existing.Available = existing.TotalCopies;
            }

            return await _repo.UpdateAsync(existing, ct);
        }

        public async Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _repo.GetByIdAsync(id, ct);
        }
    }
}
