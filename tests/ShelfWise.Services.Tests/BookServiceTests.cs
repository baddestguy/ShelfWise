using System.Threading;
using System.Threading.Tasks;
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
        [Fact]
        public async Task CreateAsync_MapsDtoAndCallsRepository()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book b, CancellationToken ct) => { b.Id = 123; return b; });

            var svc = new BookService(repo.Object);

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
    }
}
