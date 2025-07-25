using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using Moq;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.Tests.TodoLists.Commands.CreateTodoList
{
    public class CreateTodoListCommandHandlerTests
    {
        // Test data - varied and realistic
        private readonly List<TodoList> _todoLists = new()
        {
            new() { Title = "Grocery Shopping", CreatedBy = "user1" },
            new() { Title = "Project Deadline", CreatedBy = "user2" },
            new() { Title = "Doctor's Appointment", CreatedBy = "user1" }
        };

        // Mock declarations
        private readonly Mock<IApplicationDbContext> _mockContext;

        // System under test
        private readonly CreateTodoListCommandHandler _sut;

        // Constructor with setup
        public CreateTodoListCommandHandlerTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockContext.Setup(context => context.TodoLists).ReturnsDbSet(_todoLists);
            _sut = new CreateTodoListCommandHandler(_mockContext.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithValidTitle_ReturnsNewTodoListId()
        {
            // Arrange
            var command = new CreateTodoListCommand { Title = "Weekly Review" };

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBe(0);
            _todoLists.Last().Title.Should().Be("Weekly Review");
            _todoLists.Last().CreatedBy.Should().Be("user1"); // Assuming user1 is the default creator
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithEmptyTitle_ThrowsArgumentException()
        {
            // Arrange
            var command = new CreateTodoListCommand { Title = "" };

            // Act & Assert
            await Should.ThrowAsync<ArgumentException>(() => _sut.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithNullTitle_ThrowsArgumentNullException()
        {
            // Arrange
            var command = new CreateTodoListCommand { Title = null };

            // Act & Assert
            await Should.ThrowAsync<ArgumentNullException>(() => _sut.Handle(command, CancellationToken.None));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithDuplicateTitle_ReturnsExistingTodoListId()
        {
            // Arrange
            var existingTitle = "Grocery Shopping";
            var command = new CreateTodoListCommand { Title = existingTitle };

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert
            result.ShouldNotBe(0);
            _todoLists.First(tl => tl.Title == existingTitle).Id.Should().Be(result);
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenSaveChangesAsyncFails_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new CreateTodoListCommand { Title = "New List" };
            _mockContext.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException());

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => _sut.Handle(command, CancellationToken.None));
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