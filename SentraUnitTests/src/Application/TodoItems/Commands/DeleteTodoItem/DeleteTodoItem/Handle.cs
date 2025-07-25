using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;
using CleanArchitecture.Domain.Exceptions;
using Moq;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.Commands
{
    public class DeleteTodoItemCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly DeleteTodoItemCommandHandler _handler;

        public DeleteTodoItemCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _handler = new DeleteTodoItemCommandHandler(_mockContext.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithExistingTodoItem_DeletesIt()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task 1", Done = false };
            _mockContext.Setup(context => context.TodoItems.FindAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(todoItem);

            // Act
            await _handler.Handle(new DeleteTodoItemCommand { Id = 1 }, CancellationToken.None);

            // Assert
            _mockContext.Verify(context => context.TodoItems.Remove(todoItem), Times.Once);
            _mockContext.Verify(context => context.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithNonExistentTodoItemId_ThrowsNotFoundException()
        {
            // Arrange
            _mockContext.Setup(context => context.TodoItems.FindAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((TodoItem)null);

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() =>
                _handler.Handle(new DeleteTodoItemCommand { Id = 1 }, CancellationToken.None));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithNullCommand_ThrowsArgumentNullException()
        {
            // Arrange
            DeleteTodoItemCommand command = null;

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenSaveChangesFails_ThrowsDbUpdateException()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Task 1", Done = false };
            _mockContext.Setup(context => context.TodoItems.FindAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(todoItem);
            _mockContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException());

            // Act & Assert
            await Should.ThrowAsync<DbUpdateException>(() =>
                _handler.Handle(new DeleteTodoItemCommand { Id = 1 }, CancellationToken.None));
        }

        #endregion
    }
}