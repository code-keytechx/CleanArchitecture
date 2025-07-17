using CleanArchitecture.Domain.Events;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Tests
{
    public class TodoItemCompletedEventHandlerTests
    {
        // Test data - varied and realistic
        private readonly TodoItemCompletedEvent _event = new TodoItemCompletedEvent
        {
            TodoItemId = 1,
            UserId = "user123",
            CompletedDate = DateTime.UtcNow
        };

        // Mock declarations
        private readonly Mock<ILogger<TodoItemCompletedEventHandler>> _mockLogger;

        // System under test
        private readonly TodoItemCompletedEventHandler _sut;

        // Constructor with setup
        public TodoItemCompletedEventHandlerTests()
        {
            _mockLogger = new Mock<ILogger<TodoItemCompletedEventHandler>>();
            _sut = new TodoItemCompletedEventHandler(_mockLogger.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithValidEvent_LogsInformation()
        {
            // Arrange
            // No additional setup required

            // Act
            await _sut.Handle(_event, CancellationToken.None);

            // Assert
            _mockLogger.Verify(log => log.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", typeof(TodoItemCompletedEvent).Name), Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithNullEvent_ThrowsArgumentNullException()
        {
            // Arrange
            TodoItemCompletedEvent nullEvent = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Handle(nullEvent, CancellationToken.None));
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithEmptyUserId_LogsWarning()
        {
            // Arrange
            var eventWithEmptyUserId = new TodoItemCompletedEvent
            {
                TodoItemId = 1,
                UserId = "",
                CompletedDate = DateTime.UtcNow
            };

            // Act
            await _sut.Handle(eventWithEmptyUserId, CancellationToken.None);

            // Assert
            _mockLogger.Verify(log => log.LogWarning("TodoItemCompletedEvent has an empty UserId"), Times.Once);
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenLoggingFails_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockLogger.Setup(log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>())).Throws<InvalidOperationException>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Handle(_event, CancellationToken.None));
        }

        #endregion

        #region Helper Methods

        private static TodoItemCompletedEvent CreateValidEvent(int todoItemId, string userId, DateTime completedDate)
        {
            return new TodoItemCompletedEvent
            {
                TodoItemId = todoItemId,
                UserId = userId,
                CompletedDate = completedDate
            };
        }

        #endregion
    }
}