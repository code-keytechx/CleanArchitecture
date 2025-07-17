using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination.Tests
{
    public class GetTodoItemsWithPaginationQueryValidatorTests
    {
        // Test data - varied and realistic
        private readonly List<GetTodoItemsWithPaginationQuery> _testQueries = new()
        {
            new() { ListId = "LIST-001", PageNumber = 1, PageSize = 10 },
            new() { ListId = "LIST-002", PageNumber = 2, PageSize = 20 },
            new() { ListId = "LIST-003", PageNumber = 1, PageSize = 1 },
            new() { ListId = "LIST-004", PageNumber = 10, PageSize = 50 },
            new() { ListId = "", PageNumber = 1, PageSize = 10 } // Empty ListId
        };

        // Mock declarations
        private readonly Mock<IValidator<GetTodoItemsWithPaginationQuery>> _mockValidator;

        // System under test
        private readonly GetTodoItemsWithPaginationQueryValidator _sut;

        // Constructor with setup
        public GetTodoItemsWithPaginationQueryValidatorTests()
        {
            _mockValidator = new Mock<IValidator<GetTodoItemsWithPaginationQuery>>();
            _sut = new GetTodoItemsWithPaginationQueryValidator();
        }

        #region Happy Path Tests

        [Fact]
        public async Task Validate_WithValidInputs_ReturnsSuccess()
        {
            // Arrange - Use realistic, varied data
            var query = new GetTodoItemsWithPaginationQuery { ListId = "LIST-001", PageNumber = 1, PageSize = 10 };

            // Act
            ValidationResult result = await _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Validate_WithMinimumPageNumber_ReturnsSuccess()
        {
            // Arrange - Minimum PageNumber
            var query = new GetTodoItemsWithPaginationQuery { ListId = "LIST-001", PageNumber = 1, PageSize = 10 };

            // Act
            ValidationResult result = await _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Validate_WithMaximumPageSize_ReturnsSuccess()
        {
            // Arrange - Maximum PageSize
            var query = new GetTodoItemsWithPaginationQuery { ListId = "LIST-001", PageNumber = 1, PageSize = int.MaxValue };

            // Act
            ValidationResult result = await _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Validate_WithLargePageNumber_ReturnsSuccess()
        {
            // Arrange - Large PageNumber
            var query = new GetTodoItemsWithPaginationQuery { ListId = "LIST-001", PageNumber = 1000, PageSize = 10 };

            // Act
            ValidationResult result = await _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Validate_WithEmptyListId_ReturnsFailure()
        {
            // Arrange - Empty ListId
            var query = new GetTodoItemsWithPaginationQuery { ListId = "", PageNumber = 1, PageSize = 10 };

            // Act
            ValidationResult result = await _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().ErrorMessage.Should().Be("ListId is required.");
        }

        [Fact]
        public async Task Validate_WithZeroPageNumber_ReturnsFailure()
        {
            // Arrange - Zero PageNumber
            var query = new GetTodoItemsWithPaginationQuery { ListId = "LIST-001", PageNumber = 0, PageSize = 10 };

            // Act
            ValidationResult result = await _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().ErrorMessage.Should().Be("PageNumber at least greater than or equal to 1.");
        }

        [Fact]
        public async Task Validate_WithNegativePageSize_ReturnsFailure()
        {
            // Arrange - Negative PageSize
            var query = new GetTodoItemsWithPaginationQuery { ListId = "LIST-001", PageNumber = 1, PageSize = -10 };

            // Act
            ValidationResult result = await _sut.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().ErrorMessage.Should().Be("PageSize at least greater than or equal to 1.");
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Validate_WhenNullQuery_ThrowsArgumentNullException()
        {
            // Arrange
            GetTodoItemsWithPaginationQuery query = null;

            // Act & Assert
            await FluentActions.Invoking(() => _sut.Validate(query))
                .Should()
                .ThrowAsync<ArgumentNullException>()
                .WithParameterName("query");
        }

        #endregion

        #region Helper Methods

        private GetTodoItemsWithPaginationQuery CreateValidQuery(string listId, int pageNumber, int pageSize)
        {
            return new GetTodoItemsWithPaginationQuery { ListId = listId, PageNumber = pageNumber, PageSize = pageSize };
        }

        #endregion
    }
}