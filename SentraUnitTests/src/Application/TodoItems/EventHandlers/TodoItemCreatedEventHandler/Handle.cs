using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Tests
{
    public class TodoItemCreatedEventHandlerTests
    {
        // Test data - varied and realistic
        private readonly TodoItemCreatedEvent _event = new TodoItemCreatedEvent
        {
            TodoItemId = 1,
            Title = "Complete project report",
            Description = "Finalize the project report by end of the week.",
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        // Mock declarations
        private readonly Mock<ILogger<TodoItemCreatedEventHandler>> _mockLogger;

        // System under test
        private readonly TodoItemCreatedEventHandler _sut;

        // Constructor with setup
        public TodoItemCreatedEventHandlerTests()
        {
            _mockLogger = new Mock<ILogger<TodoItemCreatedEventHandler>>();
            _sut = new TodoItemCreatedEventHandler(_mockLogger.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithValidEvent_LogsInformation()
        {
            // Arrange
            var expectedLogMessage = $"CleanArchitecture Domain Event: {nameof(TodoItemCreatedEvent)}";

            // Act
            await _sut.Handle(_event, CancellationToken.None);

            // Assert
            _mockLogger.Verify(log => log.LogInformation(expectedLogMessage), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithNullEvent_ThrowsArgumentNullException()
        {
            // Arrange
            TodoItemCreatedEvent nullEvent = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Handle(nullEvent, CancellationToken.None));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithEmptyTitle_LogsWarning()
        {
            // Arrange
            var eventWithEmptyTitle = new TodoItemCreatedEvent
            {
                TodoItemId = 1,
                Title = "",
                Description = "Finalize the project report by end of the week.",
                DueDate = DateTime.UtcNow.AddDays(5)
            };
            var expectedLogMessage = "TodoItem title cannot be empty.";

            // Act
            await _sut.Handle(eventWithEmptyTitle, CancellationToken.None);

            // Assert
            _mockLogger.Verify(log => log.LogWarning(expectedLogMessage), Times.Once);
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenLoggingFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockLogger.Setup(log => log.LogInformation(It.IsAny<string>(), It.IsAny<object>())).Throws<InvalidOperationException>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Handle(_event, CancellationToken.None));
        }

        #endregion

        #region Helper Methods

        private static TodoItemCreatedEvent CreateValidEvent()
        {
            return new TodoItemCreatedEvent
            {
                TodoItemId = 1,
                Title = "Complete project report",
                Description = "Finalize the project report by end of the week.",
                DueDate = DateTime.UtcNow.AddDays(5)
            };
        }

        #endregion
    }
}