using LinkShortener.Controllers;
using LinkShortener.Database;
using LinkShortener.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using LinkShortener.Controllers;
using LinkShortener.Database;
using LinkShortener.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LinkShortener.Tests
{
    public class UrlManagementControllerTests
    {
        private readonly UrlManagementController _controller;
        private readonly DbContextOptions<UrlShortenerContext> _options;

        public UrlManagementControllerTests()
        {
            _options = new DbContextOptionsBuilder<UrlShortenerContext>()
                .UseInMemoryDatabase(databaseName: "UrlShortenerTestDb")
                .Options;

            var context = new UrlShortenerContext(_options);
            _controller = new UrlManagementController(context);
        }

        [Fact]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var model = new UrlRecord
            {
                LongUrl = "https://example.com",
                ShortUrl = string.Empty, // Будет сгенерирован
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0
            };

            // Act
            var result = await _controller.Create(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("UrlDisplay", redirectResult.ControllerName);

            using (var context = new UrlShortenerContext(_options))
            {
                var urlRecord = await context.UrlRecords.FindAsync(1);
                Assert.NotNull(urlRecord);
                Assert.False(string.IsNullOrEmpty(urlRecord.ShortUrl));
            }
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsView()
        {
            // Arrange
            _controller.ModelState.AddModelError("LongUrl", "Required");

            // Act
            var result = await _controller.Create(new UrlRecord());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<UrlRecord>(viewResult.Model);
        }

        [Fact]
        public async Task Edit_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var model = new UrlRecord
            {
                LongUrl = "https://example.com",
                ShortUrl = "short",
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0
            };

            using (var context = new UrlShortenerContext(_options))
            {
                context.UrlRecords.Add(model);
                await context.SaveChangesAsync();
            }

            var updatedModel = new UrlRecord
            {
                Id = model.Id, 
                LongUrl = "https://updatedexample.com",
                ShortUrl = "short",
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0
            };

            // Act
            var result = await _controller.Edit(model.Id, updatedModel); 

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("UrlDisplay", redirectResult.ControllerName);

            using (var context = new UrlShortenerContext(_options))
            {
                var urlRecord = await context.UrlRecords.FindAsync(model.Id);
                Assert.Equal("https://updatedexample.com", urlRecord.LongUrl);
            }
        }


        [Fact]
        public async Task Edit_InvalidModel_ReturnsView()
        {
            // Arrange
            var model = new UrlRecord
            {
                Id = 1,
                LongUrl = "https://example.com",
                ShortUrl = "short",
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0
            };

            using (var context = new UrlShortenerContext(_options))
            {
                context.UrlRecords.Add(model);
                await context.SaveChangesAsync();
            }

            // Добавляем ошибку в модель
            _controller.ModelState.AddModelError("LongUrl", "Required");

            // Act
            var result = await _controller.Edit(model.Id, new UrlRecord()
            {
                Id = 1
            }); 

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<UrlRecord>(viewResult.Model);
        }


        [Fact]
        public async Task Edit_NotFound_ReturnsNotFound()
        {
            // Act
            var result = await _controller.Edit(999, new UrlRecord());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}

