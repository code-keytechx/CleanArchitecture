using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;
using Moq;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.Tests.Commands
{
    public class CreateTodoItemCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly CreateTodoItemCommandHandler _handler;

        public CreateTodoItemCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _handler = new CreateTodoItemCommandHandler(_mockContext.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithValidRequest_AddsTodoItemToDatabase()
        {
            // Arrange
            var command = new CreateTodoItemCommand
            {
                ListId = 1,
                Title = "Buy groceries"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBe(0);
            _mockContext.Verify(ctx => ctx.TodoItems.Add(It.IsAny<TodoItem>()), Times.Once);
            _mockContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithEmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            var command = new CreateTodoItemCommand
            {
                ListId = 1,
                Title = ""
            };

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithNullTitle_ThrowsArgumentNullException()
        {
            // Arrange
            var command = new CreateTodoItemCommand
            {
                ListId = 1,
                Title = null
            };

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(() => _handler.Handle(command, CancellationToken.None));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithNonExistentListId_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new CreateTodoItemCommand
            {
                ListId = 999,
                Title = "Fix bug"
            };

            _mockContext.Setup(ctx => ctx.Lists.FindAsync(command.ListId, It.IsAny<CancellationToken>())).ReturnsAsync((List)null);

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenSaveChangesFails_ThrowsDbUpdateException()
        {
            // Arrange
            var command = new CreateTodoItemCommand
            {
                ListId = 1,
                Title = "Write tests"
            };

            _mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException());

            // Act & Assert
            await Should.ThrowAsync<DbUpdateException>(() => _handler.Handle(command, CancellationToken.None));
        }

        #endregion

        #region Helper Methods

        private static CreateTodoItemCommand CreateValidCommand(int listId, string title)
        {
            return new CreateTodoItemCommand
            {
                ListId = listId,
                Title = title
            };
        }

        #endregion
    }
}