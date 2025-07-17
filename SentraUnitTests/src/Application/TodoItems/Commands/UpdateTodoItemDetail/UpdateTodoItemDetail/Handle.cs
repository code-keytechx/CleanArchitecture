using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Exceptions;
using Moq;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.Handlers
{
    public class UpdateTodoItemDetailCommandHandlerTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly UpdateTodoItemDetailCommandHandler _handler;

        public UpdateTodoItemDetailCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _handler = new UpdateTodoItemDetailCommandHandler(_mockContext.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithValidRequest_UpdatesEntity()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                DueDate = DateTime.UtcNow,
                Priority = Priority.Normal,
                Note = "Old Note"
            };

            _mockContext.Setup(context => context.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoItem);

            var command = new UpdateTodoItemDetailCommand
            {
                Id = 1,
                ListId = 2,
                Priority = Priority.High,
                Note = "Updated Note"
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            todoItem.ListId.Should().Be(2);
            todoItem.Priority.Should().Be(Priority.High);
            todoItem.Note.Should().Be("Updated Note");
            _mockContext.Verify(context => context.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithNullListId_DoesNotChangeListId()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                DueDate = DateTime.UtcNow,
                Priority = Priority.Normal,
                Note = "Old Note"
            };

            _mockContext.Setup(context => context.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoItem);

            var command = new UpdateTodoItemDetailCommand
            {
                Id = 1,
                ListId = null,
                Priority = Priority.High,
                Note = "Updated Note"
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            todoItem.ListId.Should().Be(1);
            todoItem.Priority.Should().Be(Priority.High);
            todoItem.Note.Should().Be("Updated Note");
            _mockContext.Verify(context => context.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmptyNote_DoesNotChangeNote()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                DueDate = DateTime.UtcNow,
                Priority = Priority.Normal,
                Note = "Old Note"
            };

            _mockContext.Setup(context => context.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoItem);

            var command = new UpdateTodoItemDetailCommand
            {
                Id = 1,
                ListId = 2,
                Priority = Priority.High,
                Note = ""
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            todoItem.ListId.Should().Be(2);
            todoItem.Priority.Should().Be(Priority.High);
            todoItem.Note.Should().Be("Old Note");
            _mockContext.Verify(context => context.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithNonExistentId_ThrowsNotFoundException()
        {
            // Arrange
            _mockContext.Setup(context => context.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TodoItem)null);

            var command = new UpdateTodoItemDetailCommand
            {
                Id = 1,
                ListId = 2,
                Priority = Priority.High,
                Note = "Updated Note"
            };

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithInvalidPriority_ThrowsArgumentException()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                DueDate = DateTime.UtcNow,
                Priority = Priority.Normal,
                Note = "Old Note"
            };

            _mockContext.Setup(context => context.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoItem);

            var command = new UpdateTodoItemDetailCommand
            {
                Id = 1,
                ListId = 2,
                Priority = (Priority)(int.MaxValue + 1), // Invalid priority
                Note = "Updated Note"
            };

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenSaveChangesThrowsException_RethrowsException()
        {
            // Arrange
            var todoItem = new TodoItem
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                DueDate = DateTime.UtcNow,
                Priority = Priority.Normal,
                Note = "Old Note"
            };

            _mockContext.Setup(context => context.TodoItems.FindAsync(new object[] { 1 }, It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoItem);

            _mockContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database save failed"));

            var command = new UpdateTodoItemDetailCommand
            {
                Id = 1,
                ListId = 2,
                Priority = Priority.High,
                Note = "Updated Note"
            };

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        #endregion
    }
}