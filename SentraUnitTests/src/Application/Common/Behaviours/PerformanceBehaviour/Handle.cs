using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.Tests.Behaviours
{
    public class PerformanceBehaviourTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<ILogger<PerformanceBehaviour<TestRequest, TestResponse>>> _mockLogger;
        private readonly PerformanceBehaviour<TestRequest, TestResponse> _behaviour;

        public PerformanceBehaviourTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockUser = new Mock<IUser>();
            _mockLogger = new Mock<ILogger<PerformanceBehaviour<TestRequest, TestResponse>>>();
            _behaviour = new PerformanceBehaviour<TestRequest, TestResponse>(_mockUser.Object, _mockIdentityService.Object, _mockLogger.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: performance-critical workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithLongExecutionTime_LogsPerformanceWarning()
        {
            // Business Context: Long execution times can impact system performance and user experience
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-user-name");

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await _behaviour.Handle(request, next, CancellationToken.None);
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeGreaterThan(500, "simulated long execution time");
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Once());
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithValidUserId_FetchesUserName()
        {
            // Business Context: Ensure user identity is correctly fetched for logging
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-user-name");

            // Act
            await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync("test-user-123"), Times.Once());
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithInvalidUserId_DoesNotFetchUserName()
        {
            // Business Context: Ensure no user identity fetch if user ID is invalid
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Never());
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithShortExecutionTime_DoesNotLogPerformanceWarning()
        {
            // Business Context: Short execution times should not trigger performance logging
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-user-name");

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await _behaviour.Handle(request, next, CancellationToken.None);
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(500, "simulated short execution time");
            _mockLogger.VerifyLog(LogLevel.Warning, Times.Never());
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithNullUserId_DoesNotFetchUserName()
        {
            // Business Context: Ensure no user identity fetch if user ID is null
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithEmptyUserId_DoesNotFetchUserName()
        {
            // Business Context: Ensure no user identity fetch if user ID is empty
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns(string.Empty);

            // Act
            await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Never());
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Ensure null request handling
            // Arrange
            TestRequest? request = null;
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request!, next, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullNext_ThrowsArgumentNullException()
        {
            // Business Context: Ensure null next delegate handling
            // Arrange
            var request = new TestRequest();
            RequestHandlerDelegate<TestResponse>? next = null;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next!, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInNext_DelegatesException()
        {
            // Business Context: Ensure exceptions in next delegate are propagated
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => throw new InvalidOperationException("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>().WithMessage("Test exception");
        }

        #endregion
    }

    public class TestRequest : IRequest<TestResponse> { }

    public class TestResponse { }
}

// Extension method to verify logger calls
public static class LoggerExtensions
{
    public static void VerifyLog(this Mock<ILogger> logger, LogLevel logLevel, Times times)
    {
        logger.Verify(x => x.Log(
            logLevel,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => true),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            times);
    }
}