using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.Common.Validators
{
    public class CreateTodoListCommandValidatorTests
    {
        private readonly CreateTodoListCommandValidator _validator;
        private readonly ITodoListDbContext _context;

        public CreateTodoListCommandValidatorTests()
        {
            _context = Substitute.For<ITodoListDbContext>();
            _validator = new CreateTodoListCommandValidator(_context);
        }

        #region Happy Path Tests

        [Fact]
        public async Task BeUniqueTitle_WithUniqueTitle_ReturnsTrue()
        {
            // Arrange
            var command = new CreateTodoListCommand { Title = "New Todo List" };
            _context.TodoLists.AnyAsync(command.Title, CancellationToken.None).Returns(false);

            // Act
            var result = await _validator.BeUniqueTitle(command.Title, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task BeUniqueTitle_WithEmptyTitle_ReturnsFalse()
        {
            // Arrange
            var command = new CreateTodoListCommand { Title = "" };
            _context.TodoLists.AnyAsync(command.Title, CancellationToken.None).Returns(true);

            // Act
            var result = await _validator.BeUniqueTitle(command.Title, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task BeUniqueTitle_WithWhitespaceTitle_ReturnsFalse()
        {
            // Arrange
            var command = new CreateTodoListCommand { Title = "   " };
            _context.TodoLists.AnyAsync(command.Title, CancellationToken.None).Returns(true);

            // Act
            var result = await _validator.BeUniqueTitle(command.Title, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task BeUniqueTitle_WithExistingTitle_ReturnsFalse()
        {
            // Arrange
            var command = new CreateTodoListCommand { Title = "Existing Todo List" };
            _context.TodoLists.AnyAsync(command.Title, CancellationToken.None).Returns(true);

            // Act
            var result = await _validator.BeUniqueTitle(command.Title, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task BeUniqueTitle_WhenContextThrowsException_ThrowsException()
        {
            // Arrange
            var command = new CreateTodoListCommand { Title = "New Todo List" };
            _context.TodoLists.AnyAsync(command.Title, CancellationToken.None).ThrowsAsync(new InvalidOperationException());

            // Act & Assert
            await FluentActions.Invoking(async () => await _validator.BeUniqueTitle(command.Title, CancellationToken.None))
                .Should()
                .ThrowAsync<InvalidOperationException>();
        }

        #endregion

        #region Helper Methods

        private CreateTodoListCommand CreateValidCommand(string title)
        {
            return new CreateTodoListCommand { Title = title };
        }

        #endregion
    }
}