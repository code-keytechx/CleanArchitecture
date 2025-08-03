using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.SentraUnitTests.Application.Common.Behaviours
{
    public class PerformanceBehaviourTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<ILogger<PerformanceBehaviour<object, object>>> _mockLogger;
        private readonly PerformanceBehaviour<object, object> _behaviour;

        public PerformanceBehaviourTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockUser = new Mock<IUser>();
            _mockLogger = new Mock<ILogger<PerformanceBehaviour<object, object>>>();
            _behaviour = new PerformanceBehaviour<object, object>(_mockUser.Object, _mockIdentityService.Object, _mockLogger.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithLongExecutionTime_LogsPerformanceWarning()
        {
            // Business Context: Long execution times can impact system performance and user experience
            // Arrange
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));
            var cancellationToken = CancellationToken.None;

            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-user-name");

            // Simulate long execution time
            var timer = new Stopwatch();
            timer.Start();
            await Task.Delay(600); // Delay to exceed 500ms threshold
            timer.Stop();

            // Act
            await _behaviour.Handle(request, next, cancellationToken);

            // Assert
            // Verify that the logger was called with the expected message
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("PerformanceBehaviour") && v.ToString().Contains("test-user-name")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithValidUserId_RetrievesUserName()
        {
            // Business Context: Ensuring user data is correctly retrieved and handled
            // Arrange
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));
            var cancellationToken = CancellationToken.None;

            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-user-name");

            // Act
            await _behaviour.Handle(request, next, cancellationToken);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync("test-user-123"), Times.Once);
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithInvalidUserId_DoesNotRetrieveUserName()
        {
            // Business Context: Ensuring user data is not retrieved for invalid user IDs
            // Arrange
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));
            var cancellationToken = CancellationToken.None;

            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            await _behaviour.Handle(request, next, cancellationToken);

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
            // Business Context: Short execution times should not trigger performance logging
            // Arrange
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));
            var cancellationToken = CancellationToken.None;

            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("test-user-123")).ReturnsAsync("test-user-name");

            // Simulate short execution time
            var timer = new Stopwatch();
            timer.Start();
            await Task.Delay(400); // Delay to stay below 500ms threshold
            timer.Stop();

            // Act
            await _behaviour.Handle(request, next, cancellationToken);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("PerformanceBehaviour")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Never);
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithNullUserId_DoesNotRetrieveUserName()
        {
            // Business Context: Handling edge case where user ID is null
            // Arrange
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));
            var cancellationToken = CancellationToken.None;

            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            await _behaviour.Handle(request, next, cancellationToken);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithEmptyUserId_DoesNotRetrieveUserName()
        {
            // Business Context: Handling edge case where user ID is empty
            // Arrange
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));
            var cancellationToken = CancellationToken.None;

            _mockUser.Setup(u => u.Id).Returns(string.Empty);

            // Act
            await _behaviour.Handle(request, next, cancellationToken);

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
            // Business Context: Handling null request input
            // Arrange
            object? request = null;
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request!, next, cancellationToken))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullNext_ThrowsArgumentNullException()
        {
            // Business Context: Handling null next delegate input
            // Arrange
            var request = new object();
            RequestHandlerDelegate<object>? next = null;
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next!, cancellationToken))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInNext_DelegatesException()
        {
            // Business Context: Handling exceptions thrown by the next delegate
            // Arrange
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => throw new InvalidOperationException("Test exception"));
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, cancellationToken))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Test exception");
        }

        #endregion
    }
}