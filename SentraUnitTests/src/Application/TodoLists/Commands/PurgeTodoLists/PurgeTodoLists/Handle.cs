using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Domain.Constants;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Persistence;
using Moq;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.Tests.Commands
{
    public class PurgeTodoListsCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly PurgeTodoListsCommandHandler _handler;

        public PurgeTodoListsCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _handler = new PurgeTodoListsCommandHandler(_mockContext.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithExistingTodoLists_RemovesAllTodoLists()
        {
            // Arrange
            var todoList1 = new TodoList { Id = 1, Title = "Grocery Shopping" };
            var todoList2 = new TodoList { Id = 2, Title = "Work Tasks" };
            _mockContext.Object.TodoLists.AddRange(todoList1, todoList2);
            await _mockContext.Object.SaveChangesAsync();

            // Act
            await _handler.Handle(new PurgeTodoListsCommand(), CancellationToken.None);

            // Assert
            _mockContext.Object.TodoLists.Should().BeEmpty();
            _mockContext.Object.SaveChangesAsync().Wait().ShouldNotThrow();
        }

        [Fact]
        public async Task Handle_WithNoTodoLists_DoesNothing()
        {
            // Arrange
            await _mockContext.Object.SaveChangesAsync();

            // Act
            await _handler.Handle(new PurgeTodoListsCommand(), CancellationToken.None);

            // Assert
            _mockContext.Object.TodoLists.Should().BeEmpty();
            _mockContext.Object.SaveChangesAsync().Wait().ShouldNotThrow();
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithLargeNumberOfTodoLists_PerformsEfficiently()
        {
            // Arrange
            var todoLists = Enumerable.Range(1, 1000).Select(i => new TodoList { Id = i, Title = $"Task {i}" }).ToList();
            _mockContext.Object.TodoLists.AddRange(todoLists);
            await _mockContext.Object.SaveChangesAsync();

            // Act
            await _handler.Handle(new PurgeTodoListsCommand(), CancellationToken.None);

            // Assert
            _mockContext.Object.TodoLists.Should().BeEmpty();
            _mockContext.Object.SaveChangesAsync().Wait().ShouldNotThrow();
        }

        [Fact]
        public async Task Handle_WithNullContext_ThrowsArgumentNullException()
        {
            // Arrange
            var handler = new PurgeTodoListsCommandHandler(null!);

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(() => handler.Handle(new PurgeTodoListsCommand(), CancellationToken.None));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithInvalidRequest_ThrowsArgumentException()
        {
            // Arrange
            var handler = new PurgeTodoListsCommandHandler(_mockContext.Object);

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>(() => handler.Handle(null!, CancellationToken.None));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenSaveChangesFails_ThrowsDbUpdateException()
        {
            // Arrange
            _mockContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException());

            // Act & Assert
            await Should.ThrowAsync<DbUpdateException>(() => _handler.Handle(new PurgeTodoListsCommand(), CancellationToken.None));
        }

        #endregion

        #region Helper Methods

        private static PurgeTodoListsCommand CreateValidCommand() => new PurgeTodoListsCommand();

        #endregion
    }
}