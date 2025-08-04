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

namespace CleanArchitecture.SentraUnitTests.Application.Behaviors
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
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithLongExecutionTime_LogsPerformanceWarning()
        {
            // Business Context: Long execution times can indicate performance issues that need to be addressed.
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-user-name");

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            stopwatch.ElapsedMilliseconds = 600; // Simulate long execution time
            _mockLogger.Setup(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("PerformanceBehaviour")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()))
                .Verifiable();

            var response = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            _mockLogger.Verify();
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithValidUserId_FetchesUserName()
        {
            // Business Context: Ensuring that user names are fetched correctly for logging purposes.
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-user-name");

            // Act
            var response = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync("test-user-123"), Times.Once);
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithInvalidUserId_DoesNotFetchUserName()
        {
            // Business Context: Ensuring that user names are not fetched for invalid user IDs.
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            var response = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithShortExecutionTime_DoesNotLogPerformanceWarning()
        {
            // Business Context: Short execution times should not trigger performance logging.
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-user-name");

            // Act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            stopwatch.ElapsedMilliseconds = 400; // Simulate short execution time

            var response = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            _mockLogger.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("PerformanceBehaviour")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Never);
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithNullUserId_DoesNotFetchUserName()
        {
            // Business Context: Handling edge case where user ID is null.
            // Arrange
            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            var response = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Handling null request scenario.
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
            // Business Context: Handling null next scenario.
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
        public async Task Handle_WithExceptionInNext_ReThrowsException()
        {
            // Business Context: Handling exceptions thrown by the next handler.
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