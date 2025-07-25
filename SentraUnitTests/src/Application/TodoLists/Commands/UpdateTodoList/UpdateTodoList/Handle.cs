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
    public class UpdateTodoListCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly UpdateTodoListCommandHandler _handler;

        public UpdateTodoListCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _handler = new UpdateTodoListCommandHandler(_mockContext.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithExistingTodoList_UpdatesTitle()
        {
            // Arrange
            var todoListId = 1;
            var updatedTitle = "Updated Title";
            var existingTodoList = new TodoList { Id = todoListId, Title = "Original Title" };

            _mockContext.Setup(context => context.TodoLists.FindAsync(todoListId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTodoList);

            // Act
            await _handler.Handle(new UpdateTodoListCommand { Id = todoListId, Title = updatedTitle }, CancellationToken.None);

            // Assert
            existingTodoList.Title.ShouldBe(updatedTitle);
            _mockContext.Verify(context => context.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithNullId_ThrowsArgumentNullException()
        {
            // Arrange
            var command = new UpdateTodoListCommand { Id = default(int?), Title = "New Title" };

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithEmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            var command = new UpdateTodoListCommand { Id = 1, Title = "" };

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithNonExistentTodoList_ThrowsNotFoundException()
        {
            // Arrange
            var nonExistentId = 999;
            _mockContext.Setup(context => context.TodoLists.FindAsync(nonExistentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TodoList)null);

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(new UpdateTodoListCommand { Id = nonExistentId, Title = "New Title" }, CancellationToken.None));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenSaveChangesFails_ThrowsDbUpdateException()
        {
            // Arrange
            var todoListId = 1;
            var updatedTitle = "Updated Title";
            var existingTodoList = new TodoList { Id = todoListId, Title = "Original Title" };

            _mockContext.Setup(context => context.TodoLists.FindAsync(todoListId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTodoList);

            _mockContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            // Act & Assert
            await Should.ThrowAsync<DbUpdateConcurrencyException>(() => _handler.Handle(new UpdateTodoListCommand { Id = todoListId, Title = updatedTitle }, CancellationToken.None));
        }

        #endregion
    }
}