using CleanArchitecture.Application.Common.Interfaces;
using FluentValidation.Results;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.Common.Validators
{
    public class UpdateTodoListCommandValidatorTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly UpdateTodoListCommandValidator _validator;

        public UpdateTodoListCommandValidatorTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _validator = new UpdateTodoListCommandValidator(_mockContext.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task BeUniqueTitle_WithUniqueTitle_ReturnsTrue()
        {
            // Arrange
            var command = new UpdateTodoListCommand { Id = 1, Title = "New Title" };
            _mockContext.Setup(c => c.TodoLists.AnyAsync(
                l => l.Id != command.Id && l.Title == command.Title,
                It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _validator.BeUniqueTitle(command, command.Title, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task BeUniqueTitle_WithEmptyTitle_ReturnsFalse()
        {
            // Arrange
            var command = new UpdateTodoListCommand { Id = 1, Title = "" };

            // Act
            var result = await _validator.BeUniqueTitle(command, command.Title, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task BeUniqueTitle_WithWhitespaceTitle_ReturnsFalse()
        {
            // Arrange
            var command = new UpdateTodoListCommand { Id = 1, Title = "   " };

            // Act
            var result = await _validator.BeUniqueTitle(command, command.Title, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task BeUniqueTitle_WithNullTitle_ReturnsFalse()
        {
            // Arrange
            var command = new UpdateTodoListCommand { Id = 1, Title = null };

            // Act
            var result = await _validator.BeUniqueTitle(command, command.Title, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task BeUniqueTitle_WithExistingTitle_ReturnsFalse()
        {
            // Arrange
            var command = new UpdateTodoListCommand { Id = 1, Title = "Existing Title" };
            _mockContext.Setup(c => c.TodoLists.AnyAsync(
                l => l.Id != command.Id && l.Title == command.Title,
                It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _validator.BeUniqueTitle(command, command.Title, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task BeUniqueTitle_WhenDatabaseQueryThrowsException_ThrowsException()
        {
            // Arrange
            var command = new UpdateTodoListCommand { Id = 1, Title = "New Title" };
            _mockContext.Setup(c => c.TodoLists.AnyAsync(
                l => l.Id != command.Id && l.Title == command.Title,
                It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() =>
                _validator.BeUniqueTitle(command, command.Title, CancellationToken.None));
        }

        #endregion
    }
}