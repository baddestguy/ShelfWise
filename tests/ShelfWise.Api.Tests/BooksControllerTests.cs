using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShelfWise.Api.Controllers;
using ShelfWise.Api.Models;
using ShelfWise.Domain.Models;
using ShelfWise.Services.Services;
using Xunit;

namespace ShelfWise.Api.Tests
{
    public class BooksControllerTests
    {
        [Fact]
        public async Task Create_Returns_Created()
        {
                var svc = new Mock<IBookService>();
                svc.Setup(s => s.CreateAsync(It.IsAny<Book>(), default))
                    .ReturnsAsync(new Book { Id = 5, Title = "X" });

            var controller = new BooksController(Mock.Of<Microsoft.Extensions.Logging.ILogger<BooksController>>(), svc.Object);
            var dto = new CreateBookDto { Title = "X", Author = "A", Category = "NonFiction", TotalCopies = 1 };

            var result = await controller.Create(dto, default);

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Patch_NotFound_Returns_404()
        {
            var svc = new Mock<IBookService>();
            svc.Setup(s => s.UpdateAsync(10, It.IsAny<Book>(), default)).ReturnsAsync(false);

            var controller = new BooksController(Mock.Of<Microsoft.Extensions.Logging.ILogger<BooksController>>(), svc.Object);
            var dto = new UpdateBookDto { Title = "T", Author = "A", TotalCopies = 1 };

            var result = await controller.Update(10, dto, default);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_NotFound_Returns_404()
        {
            var svc = new Mock<IBookService>();
            svc.Setup(s => s.DeleteAsync(99, default)).ReturnsAsync(false);

            var controller = new BooksController(Mock.Of<Microsoft.Extensions.Logging.ILogger<BooksController>>(), svc.Object);

            var result = await controller.Delete(99, default);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
