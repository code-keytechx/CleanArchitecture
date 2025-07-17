using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Exceptions;
using Moq;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.Tests.Commands
{
    public class DeleteTodoListCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly DeleteTodoListCommandHandler _handler;

        public DeleteTodoListCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _handler = new DeleteTodoListCommandHandler(_mockContext.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithExistingTodoList_DeletesSuccessfully()
        {
            // Arrange
            var todoListId = 1;
            var todoList = new TodoList { Id = todoListId, Title = "Sample List" };
            _mockContext.Setup(ctx => ctx.TodoLists.FindAsync(todoListId, It.IsAny<CancellationToken>())).ReturnsAsync(todoList);

            // Act
            await _handler.Handle(new DeleteTodoListCommand { Id = todoListId }, CancellationToken.None);

            // Assert
            _mockContext.Verify(ctx => ctx.TodoLists.Remove(todoList), Times.Once);
            _mockContext.Verify(ctx => ctx.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithNonExistentTodoList_ThrowsNotFoundException()
        {
            // Arrange
            var nonExistentId = 999;
            _mockContext.Setup(ctx => ctx.TodoLists.FindAsync(nonExistentId, It.IsAny<CancellationToken>())).ReturnsAsync((TodoList)null);

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() =>
                _handler.Handle(new DeleteTodoListCommand { Id = nonExistentId }, CancellationToken.None));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            DeleteTodoListCommand nullRequest = null;

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(() =>
                _handler.Handle(nullRequest, CancellationToken.None));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenSaveChangesFails_ThrowsDbUpdateException()
        {
            // Arrange
            var todoListId = 1;
            var todoList = new TodoList { Id = todoListId, Title = "Sample List" };
            _mockContext.Setup(ctx => ctx.TodoLists.FindAsync(todoListId, It.IsAny<CancellationToken>())).ReturnsAsync(todoList);
            _mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException());

            // Act & Assert
            await Should.ThrowAsync<DbUpdateException>(() =>
                _handler.Handle(new DeleteTodoListCommand { Id = todoListId }, CancellationToken.None));
        }

        #endregion
    }
}