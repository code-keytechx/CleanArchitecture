using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Sentra.SentraUnitTests.Application.Behaviors
{
    public class UnhandledExceptionBehaviourTests
    {
        private readonly Mock<ILogger<UnhandledExceptionBehaviour<object, object>>> _mockLogger;
        private readonly UnhandledExceptionBehaviour<object, object> _behaviour;

        public UnhandledExceptionBehaviourTests()
        {
            _mockLogger = new Mock<ILogger<UnhandledExceptionBehaviour<object, object>>>();
            _behaviour = new UnhandledExceptionBehaviour<object, object>(_mockLogger.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: systems handling critical workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithUnhandledException_LogsError()
        {
            // Business Context: Unhandled exceptions should be logged for system resilience
            // Arrange
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => throw new InvalidOperationException("Test exception"));

            // Act
            Func<Task<object>> act = () => _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _mockLogger.VerifyLog(LogLevel.Error, Times.Once());
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithSuccessfulExecution_ReturnsResponse()
        {
            // Business Context: Successful execution should return the response without logging
            // Arrange
            var request = new object();
            var expectedResponse = new object();
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(expectedResponse));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResponse);
            _mockLogger.VerifyLog(LogLevel.Error, Times.Never());
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null request should throw ArgumentNullException
            // Arrange
            RequestHandlerDelegate<object> next = () => Task.FromResult(new object());

            // Act
            Func<Task<object>> act = () => _behaviour.Handle(null!, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
            _mockLogger.VerifyLog(LogLevel.Error, Times.Once());
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithExceptionInNext_ThrowsException()
        {
            // Business Context: Exception in next should be thrown and logged
            // Arrange
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => throw new InvalidOperationException("Test exception"));

            // Act
            Func<Task<object>> act = () => _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _mockLogger.VerifyLog(LogLevel.Error, Times.Once());
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithException_LogsExceptionDetails()
        {
            // Business Context: Exception details should be logged for debugging
            // Arrange
            var request = new object();
            var exception = new InvalidOperationException("Test exception");
            var next = new RequestHandlerDelegate<object>(() => throw exception);

            // Act
            Func<Task<object>> act = () => _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
            _mockLogger.VerifyLog(LogLevel.Error, Times.Once(), exception);
        }

        #endregion
    }

    // Extension method to verify logger calls
    public static class MockLoggerExtensions
    {
        public static void VerifyLog(this Mock<ILogger<T>> logger, LogLevel logLevel, Times times, Exception? exception = null) where T : class
        {
            logger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == logLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.Is<Exception>(ex => ex == exception),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                times);
        }
    }
}
