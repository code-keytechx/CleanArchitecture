using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using Moq;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.Handlers
{
    public class UpdateTodoItemCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly UpdateTodoItemCommandHandler _handler;

        public UpdateTodoItemCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _handler = new UpdateTodoItemCommandHandler(_mockContext.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithExistingTodoItem_UpdatesEntity()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Old Title", Done = false };
            _mockContext.Setup(ctx => ctx.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>())).ReturnsAsync(todoItem);

            var command = new UpdateTodoItemCommand { Id = 1, Title = "New Title", Done = true };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            todoItem.Title.ShouldBe("New Title");
            todoItem.Done.ShouldBe(true);
            _mockContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithNullTitle_IgnoresChange()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Old Title", Done = false };
            _mockContext.Setup(ctx => ctx.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>())).ReturnsAsync(todoItem);

            var command = new UpdateTodoItemCommand { Id = 1, Title = null, Done = true };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            todoItem.Title.ShouldNotBe(null);
            todoItem.Done.ShouldBe(false);
            _mockContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Handle_WithEmptyTitle_IgnoresChange()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Old Title", Done = false };
            _mockContext.Setup(ctx => ctx.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>())).ReturnsAsync(todoItem);

            var command = new UpdateTodoItemCommand { Id = 1, Title = "", Done = true };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            todoItem.Title.ShouldNotBe("");
            todoItem.Done.ShouldBe(false);
            _mockContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Never);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithNonExistentTodoItem_ThrowsNotFoundException()
        {
            // Arrange
            _mockContext.Setup(ctx => ctx.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>())).ReturnsAsync((TodoItem)null);

            var command = new UpdateTodoItemCommand { Id = 1, Title = "New Title", Done = true };

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithInvalidDoneStatus_ThrowsArgumentException()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Old Title", Done = false };
            _mockContext.Setup(ctx => ctx.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>())).ReturnsAsync(todoItem);

            var command = new UpdateTodoItemCommand { Id = 1, Title = "New Title", Done = null };

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenSaveChangesThrowsException_RethrowsException()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Old Title", Done = false };
            _mockContext.Setup(ctx => ctx.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>())).ReturnsAsync(todoItem);
            _mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Database save failed"));

            var command = new UpdateTodoItemCommand { Id = 1, Title = "New Title", Done = true };

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        #endregion

        #region Helper Methods

        private static UpdateTodoItemCommand CreateValidCommand(int id, string title, bool done)
        {
            return new UpdateTodoItemCommand { Id = id, Title = title, Done = done };
        }

        #endregion
    }
}