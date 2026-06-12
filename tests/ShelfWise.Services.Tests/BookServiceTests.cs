using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ShelfWise.Domain.Models;
using ShelfWise.Repository.Repositories;
using ShelfWise.Services.Services;
using Xunit;

namespace ShelfWise.Services.Tests
{
    public class BookServiceTests
    {
        private static BookService CreateService(Mock<IBookRepository> repo, IMemoryCache? cache = null)
        {
            return new BookService(repo.Object, cache ?? new MemoryCache(new MemoryCacheOptions()));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsRepositoryBooks()
        {
            var books = new[] { Book(id: 1), Book(id: 2) };
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(books);
            var service = CreateService(repo);

            var result = await service.GetAllAsync();

            Assert.Same(books, result);
            repo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SearchInventoryAsync_CachesResultsByNormalizedSearchTerm()
        {
            var items = new[] { InventoryItem(id: 1, title: "Clean Code") };
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(" clean ", It.IsAny<CancellationToken>())).ReturnsAsync(items);
            var service = CreateService(repo);

            var first = await service.SearchInventoryAsync(" clean ");
            var second = await service.SearchInventoryAsync("CLEAN");

            Assert.Same(first, second);
            repo.Verify(r => r.SearchInventoryAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetInventoryByIdAsync_CachesFoundItem()
        {
            var item = InventoryItem(id: 7, title: "Dune");
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.GetInventoryByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(item);
            var service = CreateService(repo);

            var first = await service.GetInventoryByIdAsync(7);
            var second = await service.GetInventoryByIdAsync(7);

            Assert.Same(first, second);
            repo.Verify(r => r.GetInventoryByIdAsync(7, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetInventoryByIdAsync_DoesNotCacheMissingItem()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.GetInventoryByIdAsync(404, It.IsAny<CancellationToken>())).ReturnsAsync((BookInventoryItem?)null);
            var service = CreateService(repo);

            var first = await service.GetInventoryByIdAsync(404);
            var second = await service.GetInventoryByIdAsync(404);

            Assert.Null(first);
            Assert.Null(second);
            repo.Verify(r => r.GetInventoryByIdAsync(404, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task CreateAsync_ClampsNegativeCopies_AddsBook_AndClearsInventoryCache()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { InventoryItem() });
            repo.Setup(r => r.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book b, CancellationToken _) =>
                {
                    b.Id = 123;
                    return b;
                });
            var service = CreateService(repo);
            await service.SearchInventoryAsync(null);

            var created = await service.CreateAsync(Book(totalCopies: -3));
            await service.SearchInventoryAsync(null);

            Assert.Equal(123, created.Id);
            Assert.Equal(0, created.TotalCopies);
            repo.Verify(r => r.AddAsync(It.Is<Book>(b => b.TotalCopies == 0), It.IsAny<CancellationToken>()), Times.Once);
            repo.Verify(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateAsync_WhenBookMissing_ReturnsFalseAndDoesNotUpdate()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((Book?)null);
            var service = CreateService(repo);

            var result = await service.UpdateAsync(5, Book());

            Assert.False(result);
            repo.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_MapsAllowedFields_ClampsCopies_AndClearsInventoryCache()
        {
            var existing = Book(id: 5, title: "Old", author: "Author", totalCopies: 2);
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { InventoryItem() });
            repo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
            repo.Setup(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var service = CreateService(repo);
            await service.SearchInventoryAsync(null);

            var updated = Book(title: "New", author: "Writer", category: Category.Fiction, genre: "Sci-Fi", totalCopies: -10);
            var result = await service.UpdateAsync(5, updated);
            await service.SearchInventoryAsync(null);

            Assert.True(result);
            repo.Verify(r => r.UpdateAsync(It.Is<Book>(b =>
                b.Id == 5 &&
                b.Title == "New" &&
                b.Author == "Writer" &&
                b.Category == Category.Fiction &&
                b.Genre == "Sci-Fi" &&
                b.TotalCopies == 0), It.IsAny<CancellationToken>()), Times.Once);
            repo.Verify(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateAsync_WhenRepositoryReturnsFalse_DoesNotClearInventoryCache()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { InventoryItem() });
            repo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(Book(id: 5));
            repo.Setup(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var service = CreateService(repo);
            await service.SearchInventoryAsync(null);

            var result = await service.UpdateAsync(5, Book(title: "New"));
            await service.SearchInventoryAsync(null);

            Assert.False(result);
            repo.Verify(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenDeleted_ClearsInventoryCache()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { InventoryItem() });
            repo.Setup(r => r.DeleteAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var service = CreateService(repo);
            await service.SearchInventoryAsync(null);

            var result = await service.DeleteAsync(5);
            await service.SearchInventoryAsync(null);

            Assert.True(result);
            repo.Verify(r => r.DeleteAsync(5, It.IsAny<CancellationToken>()), Times.Once);
            repo.Verify(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task DeleteAsync_WhenNotDeleted_DoesNotClearInventoryCache()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { InventoryItem() });
            repo.Setup(r => r.DeleteAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var service = CreateService(repo);
            await service.SearchInventoryAsync(null);

            var result = await service.DeleteAsync(5);
            await service.SearchInventoryAsync(null);

            Assert.False(result);
            repo.Verify(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CheckOutAsync_WhenUserAlreadyHasBook_ReturnsFalseWithoutCheckingAvailability()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.GetActiveBorrowRecordAsync(3, 9, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BorrowRecord { BookId = 3, UserId = 9 });
            var service = CreateService(repo);

            var result = await service.CheckOutAsync(3, 9);

            Assert.False(result);
            repo.Verify(r => r.GetTotalCopiesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            repo.Verify(r => r.TryCheckoutAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CheckOutAsync_WhenNoCopiesAvailable_ReturnsFalseWithoutCheckout()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.GetActiveBorrowRecordAsync(3, 9, It.IsAny<CancellationToken>())).ReturnsAsync((BorrowRecord?)null);
            repo.Setup(r => r.GetTotalCopiesAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(2);
            repo.Setup(r => r.GetCheckedOutCountAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(2);
            var service = CreateService(repo);

            var result = await service.CheckOutAsync(3, 9);

            Assert.False(result);
            repo.Verify(r => r.TryCheckoutAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CheckOutAsync_WhenCopyAvailable_ChecksOutWithDueDateAndClearsInventoryCache()
        {
            var before = DateTime.UtcNow;
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { InventoryItem() });
            repo.Setup(r => r.GetActiveBorrowRecordAsync(3, 9, It.IsAny<CancellationToken>())).ReturnsAsync((BorrowRecord?)null);
            repo.Setup(r => r.GetTotalCopiesAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(2);
            repo.Setup(r => r.GetCheckedOutCountAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(r => r.TryCheckoutAsync(3, 9, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var service = CreateService(repo);
            await service.SearchInventoryAsync(null);

            var result = await service.CheckOutAsync(3, 9, dueDays: 21);
            await service.SearchInventoryAsync(null);

            Assert.True(result);
            repo.Verify(r => r.TryCheckoutAsync(3, 9,
                It.Is<DateTime>(dueAt => dueAt >= before.AddDays(21) && dueAt <= DateTime.UtcNow.AddDays(21)),
                It.IsAny<CancellationToken>()), Times.Once);
            repo.Verify(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task CheckOutAsync_WhenRepositoryCheckoutFails_DoesNotClearInventoryCache()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { InventoryItem() });
            repo.Setup(r => r.GetActiveBorrowRecordAsync(3, 9, It.IsAny<CancellationToken>())).ReturnsAsync((BorrowRecord?)null);
            repo.Setup(r => r.GetTotalCopiesAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(2);
            repo.Setup(r => r.GetCheckedOutCountAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(r => r.TryCheckoutAsync(3, 9, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var service = CreateService(repo);
            await service.SearchInventoryAsync(null);

            var result = await service.CheckOutAsync(3, 9);
            await service.SearchInventoryAsync(null);

            Assert.False(result);
            repo.Verify(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CheckInAsync_WhenSuccessful_ClearsInventoryCache()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { InventoryItem() });
            repo.Setup(r => r.TryCheckinAsync(3, 9, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var service = CreateService(repo);
            await service.SearchInventoryAsync(null);

            var result = await service.CheckInAsync(3, 9);
            await service.SearchInventoryAsync(null);

            Assert.True(result);
            repo.Verify(r => r.TryCheckinAsync(3, 9, It.IsAny<CancellationToken>()), Times.Once);
            repo.Verify(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async Task CheckInAsync_WhenNotSuccessful_DoesNotClearInventoryCache()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { InventoryItem() });
            repo.Setup(r => r.TryCheckinAsync(3, 9, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var service = CreateService(repo);
            await service.SearchInventoryAsync(null);

            var result = await service.CheckInAsync(3, 9);
            await service.SearchInventoryAsync(null);

            Assert.False(result);
            repo.Verify(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PlaceHoldAsync_CreatesHoldAndClearsInventoryCache()
        {
            var repo = new Mock<IBookRepository>();
            repo.Setup(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { InventoryItem() });
            repo.Setup(r => r.CreateHoldAsync(3, 9, It.IsAny<CancellationToken>())).ReturnsAsync(77);
            var service = CreateService(repo);
            await service.SearchInventoryAsync(null);

            var holdId = await service.PlaceHoldAsync(3, 9);
            await service.SearchInventoryAsync(null);

            Assert.Equal(77, holdId);
            repo.Verify(r => r.CreateHoldAsync(3, 9, It.IsAny<CancellationToken>()), Times.Once);
            repo.Verify(r => r.SearchInventoryAsync(null, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        private static Book Book(
            int id = 0,
            string title = "Title",
            string author = "Author",
            Category category = Category.NonFiction,
            string genre = "Genre",
            int totalCopies = 1)
        {
            return new Book
            {
                Id = id,
                Title = title,
                Author = author,
                Category = category,
                Genre = genre,
                TotalCopies = totalCopies
            };
        }

        private static BookInventoryItem InventoryItem(
            int id = 0,
            string title = "Title",
            string author = "Author",
            Category category = Category.NonFiction,
            string genre = "Genre",
            int totalCopies = 1,
            int checkedOutCopies = 0)
        {
            return new BookInventoryItem
            {
                Id = id,
                Title = title,
                Author = author,
                Category = category,
                Genre = genre,
                TotalCopies = totalCopies,
                CheckedOutCopies = checkedOutCopies,
                AvailableCopies = totalCopies - checkedOutCopies
            };
        }
    }
}
