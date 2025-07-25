using System.Diagnostics;
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.Tests.Behaviors
{
    public class PerformanceBehaviourTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<ILogger<PerformanceBehaviour<object, object>>> _mockLogger;
        private readonly PerformanceBehaviour<object, object> _behaviour;
        private readonly Stopwatch _timer;

        public PerformanceBehaviourTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockLogger = new Mock<ILogger<PerformanceBehaviour<object, object>>>();
            _behaviour = new PerformanceBehaviour<object, object>(_mockIdentityService.Object, _mockLogger.Object);
            _timer = new Stopwatch();
        }

        #region Happy Path Tests

        [Fact]
        public async Task Handle_WithShortExecutionTime_ReturnsResponse()
        {
            // Arrange
            _timer.Start();
            _timer.Stop();
            _timer.ElapsedMilliseconds = 100;

            // Act
            var response = await _behaviour.Handle(new object(), () => Task.FromResult(new object()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Handle_WithBoundaryExecutionTime_ReturnsResponse()
        {
            // Arrange
            _timer.Start();
            _timer.Stop();
            _timer.ElapsedMilliseconds = 500;

            // Act
            var response = await _behaviour.Handle(new object(), () => Task.FromResult(new object()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithLongExecutionTime_ReturnsResponse()
        {
            // Arrange
            _timer.Start();
            _timer.Stop();
            _timer.ElapsedMilliseconds = 600;

            // Act
            var response = await _behaviour.Handle(new object(), () => Task.FromResult(new object()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            Func<Task> act = () => _behaviour.Handle(null!, () => Task.FromResult(new object()), CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentNullException>("request");
        }

        [Fact]
        public async Task Handle_WithNullNext_ThrowsArgumentNullException()
        {
            // Arrange
            Func<Task> act = () => _behaviour.Handle(new object(), null!, CancellationToken.None);

            // Act & Assert
            await act.Should().ThrowAsync<ArgumentNullException>("next");
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Handle_WhenIdentityServiceThrowsException_LogsWarning()
        {
            // Arrange
            _mockIdentityService.Setup(s => s.GetUserNameAsync(It.IsAny<string>())).ThrowsAsync(new InvalidOperationException("User not found"));

            _timer.Start();
            _timer.Stop();
            _timer.ElapsedMilliseconds = 500;

            // Act
            var response = await _behaviour.Handle(new object(), () => Task.FromResult(new object()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenLoggingThrowsException_DoesNotCrash()
        {
            // Arrange
            _mockLogger.Setup(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>())).ThrowsAsync(new InvalidOperationException("Log failed"));

            _timer.Start();
            _timer.Stop();
            _timer.ElapsedMilliseconds = 500;

            // Act
            var response = await _behaviour.Handle(new object(), () => Task.FromResult(new object()), CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        #endregion

        #region Helper Methods

        private static void SetupUser(Mock<IIdentityService> mockIdentityService, string userId, string userName)
        {
            mockIdentityService.Setup(s => s.GetUserNameAsync(userId)).ReturnsAsync(userName);
        }

        #endregion
    }
}