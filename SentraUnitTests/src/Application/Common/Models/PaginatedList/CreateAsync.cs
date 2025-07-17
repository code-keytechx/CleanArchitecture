using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

namespace PaginatedList.Tests
{
    public class PaginatedListTests
    {
        // Test data - varied and realistic
        private readonly List<int> _items = Enumerable.Range(1, 100).ToList();

        // Mock declarations
        private readonly Mock<IQueryable<int>> _mockQuery;

        // Setup/Constructor
        public PaginatedListTests()
        {
            _mockQuery = new Mock<IQueryable<int>>();
            _mockQuery.Setup(q => q.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_items.Count);
            _mockQuery.Setup(q => q.Skip(It.IsAny<int>()).Take(It.IsAny<int>()).ToListAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(_items.Skip(0).Take(10).ToList()); // Page 1, PageSize 10
        }

        #region Happy Path Tests

        [Fact]
        public async Task CreateAsync_WithValidParameters_ReturnsPaginatedList()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var cancellationToken = CancellationToken.None;

            // Act
            var paginatedList = await PaginatedList<int>.CreateAsync(_mockQuery.Object, pageNumber, pageSize, cancellationToken);

            // Assert
            paginatedList.PageNumber.Should().Be(pageNumber);
            paginatedList.PageSize.Should().Be(pageSize);
            paginatedList.TotalCount.Should().Be(_items.Count);
            paginatedList.Items.Should().HaveCount(pageSize);
            paginatedList.Items.SequenceEqual(_items.Take(pageSize)).Should().BeTrue();
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task CreateAsync_WithPageNumberOne_ReturnsFirstPage()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var cancellationToken = CancellationToken.None;

            // Act
            var paginatedList = await PaginatedList<int>.CreateAsync(_mockQuery.Object, pageNumber, pageSize, cancellationToken);

            // Assert
            paginatedList.PageNumber.Should().Be(pageNumber);
            paginatedList.Items.SequenceEqual(_items.Take(pageSize)).Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_WithLastPage_ReturnsLastPage()
        {
            // Arrange
            var pageNumber = 10;
            var pageSize = 10;
            var cancellationToken = CancellationToken.None;

            // Act
            var paginatedList = await PaginatedList<int>.CreateAsync(_mockQuery.Object, pageNumber, pageSize, cancellationToken);

            // Assert
            paginatedList.PageNumber.Should().Be(pageNumber);
            paginatedList.Items.SequenceEqual(_items.Skip(90).Take(pageSize)).Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_WithEmptySet_ReturnsEmptyList()
        {
            // Arrange
            var items = new List<int>();
            _mockQuery.Setup(q => q.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(items.Count);
            _mockQuery.Setup(q => q.Skip(It.IsAny<int>()).Take(It.IsAny<int>()).ToListAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(items.Skip(0).Take(10).ToList());

            var pageNumber = 1;
            var pageSize = 10;
            var cancellationToken = CancellationToken.None;

            // Act
            var paginatedList = await PaginatedList<int>.CreateAsync(_mockQuery.Object, pageNumber, pageSize, cancellationToken);

            // Assert
            paginatedList.PageNumber.Should().Be(pageNumber);
            paginatedList.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithPageSizeZero_ReturnsEmptyList()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 0;
            var cancellationToken = CancellationToken.None;

            // Act
            var paginatedList = await PaginatedList<int>.CreateAsync(_mockQuery.Object, pageNumber, pageSize, cancellationToken);

            // Assert
            paginatedList.PageNumber.Should().Be(pageNumber);
            paginatedList.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_WithLargePageSize_ReturnsAllItems()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 100;
            var cancellationToken = CancellationToken.None;

            // Act
            var paginatedList = await PaginatedList<int>.CreateAsync(_mockQuery.Object, pageNumber, pageSize, cancellationToken);

            // Assert
            paginatedList.PageNumber.Should().Be(pageNumber);
            paginatedList.Items.SequenceEqual(_items).Should().BeTrue();
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task CreateAsync_WithNegativePageNumber_ThrowsArgumentException()
        {
            // Arrange
            var pageNumber = -1;
            var pageSize = 10;
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                PaginatedList<int>.CreateAsync(_mockQuery.Object, pageNumber, pageSize, cancellationToken));
        }

        [Fact]
        public async Task CreateAsync_WithNegativePageSize_ThrowsArgumentException()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = -1;
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                PaginatedList<int>.CreateAsync(_mockQuery.Object, pageNumber, pageSize, cancellationToken));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task CreateAsync_WhenCountAsyncThrows_ThrowsException()
        {
            // Arrange
            _mockQuery.Setup(q => q.CountAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Database error"));

            var pageNumber = 1;
            var pageSize = 10;
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                PaginatedList<int>.CreateAsync(_mockQuery.Object, pageNumber, pageSize, cancellationToken));
        }

        [Fact]
        public async Task CreateAsync_WhenSkipOrTakeThrows_ThrowsException()
        {
            // Arrange
            _mockQuery.Setup(q => q.Skip(It.IsAny<int>()).Take(It.IsAny<int>()).ToListAsync(It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new InvalidOperationException("Database error"));

            var pageNumber = 1;
            var pageSize = 10;
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                PaginatedList<int>.CreateAsync(_mockQuery.Object, pageNumber, pageSize, cancellationToken));
        }

        #endregion

        #region Helper Methods

        private void SetupMockQuery(List<int> items)
        {
            _mockQuery.Setup(q => q.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(items.Count);
            _mockQuery.Setup(q => q.Skip(It.IsAny<int>()).Take(It.IsAny<int>()).ToListAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(items.Skip(0).Take(10).ToList());
        }

        #endregion
    }
}