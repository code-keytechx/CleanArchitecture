using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.SentraUnitTests.Application.Common.Behaviours
{
    public class UnhandledExceptionBehaviourTests
    {
        private readonly Mock<ILogger<TestRequest>> _mockLogger;
        private readonly UnhandledExceptionBehaviour<TestRequest, TestResponse> _behaviour;

        public UnhandledExceptionBehaviourTests()
        {
            _mockLogger = new Mock<ILogger<TestRequest>>();
            _behaviour = new UnhandledExceptionBehaviour<TestRequest, TestResponse>(_mockLogger.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithUnhandledException_LogsErrorAndThrows()
        {
            // Business Context: Unhandled exceptions should be logged and rethrown to prevent silent failures
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            mockNext.Setup(x => x()).ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Test exception");

            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CleanArchitecture Request: Unhandled Exception for Request TestRequest {@Request}")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once());
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithSuccessfulRequest_ReturnsResponse()
        {
            // Business Context: Successful requests should pass through without exceptions
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            mockNext.Setup(x => x()).ReturnsAsync(new TestResponse { Success = true });

            // Act
            var response = await _behaviour.Handle(request, mockNext.Object, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null requests should be handled gracefully
            // Arrange
            TestRequest? request = null;
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request!, mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'request')");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithExceptionInNext_ThrowsOriginalException()
        {
            // Business Context: Exceptions from the next handler should be rethrown
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            mockNext.Setup(x => x()).ThrowsAsync(new ArgumentException("Invalid argument"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid argument");

            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CleanArchitecture Request: Unhandled Exception for Request TestRequest {@Request}")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once());
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithException_LogsCorrectMessage()
        {
            // Business Context: Ensure the correct log message is generated on exception
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            mockNext.Setup(x => x()).ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act
            await _behaviour.Invoking(b => b.Handle(request, mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Test exception");

            // Assert
            _mockLogger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CleanArchitecture Request: Unhandled Exception for Request TestRequest {@Request}")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once());
        }

        #endregion
    }

    public class TestRequest
    {
        public string? UserId { get; set; }
    }

    public class TestResponse
    {
        public bool Success { get; set; }
    }
}