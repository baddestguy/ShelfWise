using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ShelfWise.Domain.Models;
using ShelfWise.Repository.Repositories;
using ShelfWise.Services.Services;
using ShelfWise.Api.Models;
using Xunit;

namespace ShelfWise.Services.Tests
{
    public class BookServiceTests
    {
        private static BookService CreateService(Mock<IBookRepository> repo)
        {
            return new BookService(repo.Object, new MemoryCache(new MemoryCacheOptions()));
        }

        [Fact]
        public async Task CreateAsync_MapsDtoAndCallsRepository()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book b, CancellationToken ct) => { b.Id = 123; return b; });

            var svc = CreateService(repo);

            var domain = new Book
            {
                Title = "T",
                Author = "A",
                Category = Category.NonFiction,
                Genre = "G",
                TotalCopies = 2
            };

            var created = await svc.CreateAsync(domain);

            Assert.Equal(123, created.Id);
            Assert.Equal(domain.Title, created.Title);
            Assert.Equal(domain.Author, created.Author);
            Assert.Equal(domain.TotalCopies, created.TotalCopies);
            repo.Verify(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ExistingBook_UpdatesAndReturnsTrue()
        {
            var repo = new Mock<IBookRepository>();
            var existing = new Book { Id = 5, Title = "Old", Author = "A", Category = Category.NonFiction, Genre = "G", TotalCopies = 2 };
            repo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            repo.Setup(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var svc = CreateService(repo);

            var updated = new Book { Title = "New", Author = "B", Category = Category.Fiction, Genre = "NewG", TotalCopies = 3 };
            var result = await svc.UpdateAsync(5, updated);

            Assert.True(result);
            repo.Verify(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()), Times.Once);
            repo.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ExistingBook_DeletesAndReturnsTrue()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.DeleteAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var svc = CreateService(repo);

            var result = await svc.DeleteAsync(5);

            Assert.True(result);
            repo.Verify(r => r.DeleteAsync(5, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
