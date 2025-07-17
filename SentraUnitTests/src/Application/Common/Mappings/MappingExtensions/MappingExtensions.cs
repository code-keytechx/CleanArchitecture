using CleanArchitecture.Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Moq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.Common.Mappings.Tests
{
    public class MappingExtensionsTests
    {
        private readonly Mock<IQueryable<MyDto>> _mockQuery;
        private readonly Mock<IMapperConfigurationExpression> _mockConfig;
        private readonly Mock<IMapper> _mockMapper;

        public MappingExtensionsTests()
        {
            _mockQuery = new Mock<IQueryable<MyDto>>();
            _mockConfig = new Mock<IMapperConfigurationExpression>();
            _mockMapper = new Mock<IMapper>();
        }

        #region Happy Path Tests

        [Fact]
        public async Task PaginatedListAsync_WithValidParameters_ReturnsPaginatedList()
        {
            // Arrange
            var queryable = _mockQuery.Object;
            var pageNumber = 1;
            var pageSize = 10;
            var totalCount = 20;
            var items = Enumerable.Range(1, totalCount).Select(i => new MyDto { Id = i }).ToArray();

            queryable.Setup(q => q.Skip(pageSize * (pageNumber - 1))).Returns(queryable);
            queryable.Setup(q => q.Take(pageSize)).Returns(queryable);
            queryable.Setup(q => q.Count()).Returns(totalCount);

            _mockMapper.Setup(m => m.Map<IEnumerable<MyDto>, IEnumerable<MyDto>>(It.IsAny<IEnumerable<MyDto>>())).Returns(items);

            // Act
            var result = await queryable.PaginatedListAsync<MyDto>(pageNumber, pageSize);

            // Assert
            result.PageNumber.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
            result.TotalCount.Should().Be(totalCount);
            result.Items.Should().HaveCount(pageSize);
        }

        [Fact]
        public async Task ProjectToListAsync_WithValidParameters_ReturnsMappedList()
        {
            // Arrange
            var queryable = _mockQuery.Object;
            var configuration = _mockConfig.Object;
            var items = Enumerable.Range(1, 10).Select(i => new MyDto { Id = i }).ToArray();

            queryable.Setup(q => q.ProjectTo<MyDto>(configuration)).Returns(queryable);
            queryable.Setup(q => q.ToListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(items);

            // Act
            var result = await queryable.ProjectToListAsync<MyDto>(configuration);

            // Assert
            result.Should().HaveCount(10);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task PaginatedListAsync_WithPageNumberZero_ReturnsEmptyList()
        {
            // Arrange
            var queryable = _mockQuery.Object;
            var pageNumber = 0;
            var pageSize = 10;

            // Act
            var result = await queryable.PaginatedListAsync<MyDto>(pageNumber, pageSize);

            // Assert
            result.PageNumber.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
            result.TotalCount.Should().Be(0);
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task ProjectToListAsync_WithNullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var queryable = _mockQuery.Object;
            var configuration = null as IMapperConfigurationExpression;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => queryable.ProjectToListAsync<MyDto>(configuration));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task PaginatedListAsync_WithNegativePageSize_ThrowsArgumentException()
        {
            // Arrange
            var queryable = _mockQuery.Object;
            var pageNumber = 1;
            var pageSize = -1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => queryable.PaginatedListAsync<MyDto>(pageNumber, pageSize));
        }

        [Fact]
        public async Task ProjectToListAsync_WithEmptyQuery_ThrowsInvalidOperationException()
        {
            // Arrange
            var queryable = _mockQuery.Object;
            var configuration = _mockConfig.Object;

            queryable.Setup(q => q.ToListAsync(It.IsAny<CancellationToken>())).ReturnsAsync((IEnumerable<MyDto>)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => queryable.ProjectToListAsync<MyDto>(configuration));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task PaginatedListAsync_WhenQueryThrowsException_ThrowsException()
        {
            // Arrange
            var queryable = _mockQuery.Object;
            var pageNumber = 1;
            var pageSize = 10;

            queryable.Setup(q => q.Skip(pageSize * (pageNumber - 1))).ThrowsAsync(new InvalidOperationException());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => queryable.PaginatedListAsync<MyDto>(pageNumber, pageSize));
        }

        [Fact]
        public async Task ProjectToListAsync_WhenMappingThrowsException_ThrowsException()
        {
            // Arrange
            var queryable = _mockQuery.Object;
            var configuration = _mockConfig.Object;
            var items = Enumerable.Range(1, 10).Select(i => new MyDto { Id = i }).ToArray();

            queryable.Setup(q => q.ProjectTo<MyDto>(configuration)).Returns(queryable);
            queryable.Setup(q => q.ToListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(items);

            _mockMapper.Setup(m => m.Map<IEnumerable<MyDto>, IEnumerable<MyDto>>(It.IsAny<IEnumerable<MyDto>>())).ThrowsAsync(new InvalidOperationException());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => queryable.ProjectToListAsync<MyDto>(configuration));
        }

        #endregion

        #region Helper Methods

        private class MyDto
        {
            public int Id { get; set; }
        }

        #endregion
    }
}