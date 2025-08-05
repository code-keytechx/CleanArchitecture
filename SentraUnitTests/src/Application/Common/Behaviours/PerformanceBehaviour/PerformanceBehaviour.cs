using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.SentraUnitTests.Application.Common.Behaviours
{
    public class PerformanceBehaviourTests
    {
        private readonly Mock<ILogger<PerformanceBehaviourTests>> _mockLogger;
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly PerformanceBehaviour<PerformanceBehaviourTests, bool> _behaviour;

        public PerformanceBehaviourTests()
        {
            _mockLogger = new Mock<ILogger<PerformanceBehaviourTests>>();
            _mockUser = new Mock<IUser>();
            _mockIdentityService = new Mock<IIdentityService>();
            _behaviour = new PerformanceBehaviour<PerformanceBehaviourTests, bool>(
                _mockLogger.Object,
                _mockUser.Object,
                _mockIdentityService.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithLongRunningRequest_LogsWarning()
        {
            // Business Context: Long-running requests can impact system performance and user experience
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-username");

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 501) { }
            var result = await _behaviour.Handle(this, () => Task.FromResult(true), CancellationToken.None);

            // Assert
            result.Should().BeTrue("the response should be true");
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Once());
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUserId_LogsUserName()
        {
            // Business Context: Ensure user identity is logged for auditing and compliance
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-username");

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 501) { }
            await _behaviour.Handle(this, () => Task.FromResult(true), CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Once(), "CleanArchitecture Long Running Request: PerformanceBehaviourTests (501 milliseconds) test-user-123 test-username PerformanceBehaviourTests");
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithNoUserId_LogsEmptyUserName()
        {
            // Business Context: Ensure system handles cases where user ID is not available
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 501) { }
            await _behaviour.Handle(this, () => Task.FromResult(true), CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Once(), "CleanArchitecture Long Running Request: PerformanceBehaviourTests (501 milliseconds)  PerformanceBehaviourTests");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithShortRunningRequest_DoesNotLogWarning()
        {
            // Business Context: Short-running requests should not trigger logging
            // Arrange

            // Act
            var result = await _behaviour.Handle(this, () => Task.FromResult(true), CancellationToken.None);

            // Assert
            result.Should().BeTrue("the response should be true");
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Never());
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithZeroElapsedMilliseconds_DoesNotLogWarning()
        {
            // Business Context: Edge case where elapsed time is zero
            // Arrange

            // Act
            var result = await _behaviour.Handle(this, () => Task.FromResult(true), CancellationToken.None);

            // Assert
            result.Should().BeTrue("the response should be true");
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Never());
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Ensure system handles null request gracefully
            // Arrange
            PerformanceBehaviourTests? request = null;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request!, () => Task.FromResult(true), CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInNext_ThrowsException()
        {
            // Business Context: Ensure system handles exceptions from next handler
            // Arrange
            var exception = new InvalidOperationException("Test exception");

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(this, () => Task.FromException<bool>(exception), CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>().WithMessage("Test exception");
        }

        #endregion
    }

    public static class MockLoggerExtensions
    {
        public static void VerifyLog(this Mock<ILogger<PerformanceBehaviourTests>> mockLogger, LogLevel logLevel, Times times, string expectedMessage = null)
        {
            mockLogger.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => expectedMessage == null || v.ToString().Contains(expectedMessage)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                times);
        }
    }
}