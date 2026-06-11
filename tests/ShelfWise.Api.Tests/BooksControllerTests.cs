using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShelfWise.Api.Controllers;
using ShelfWise.Domain.Models;
using ShelfWise.Services.Interfaces;
using ShelfWise.Api.Models;
using Xunit;

namespace ShelfWise.Api.Tests
{
    public class BooksControllerTests
    {
        [Fact]
        public async void Create_Returns_Created()
        {
                var svc = new Mock<IBookService>();
                svc.Setup(s => s.CreateAsync(It.IsAny<Book>(), default))
                    .ReturnsAsync(new Book { Id = 5, Title = "X" });

            var controller = new BooksController(Mock.Of<Microsoft.Extensions.Logging.ILogger<BooksController>>(), svc.Object);
            var dto = new CreateBookDto { Title = "X", Author = "A", Category = "NonFiction", TotalCopies = 1 };

            var result = await controller.Create(dto, default);

            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}
