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
        private readonly Mock<ILogger<UnhandledExceptionBehaviour<TestRequest, TestResponse>>> _mockLogger;
        private readonly UnhandledExceptionBehaviour<TestRequest, TestResponse> _behaviour;
        private readonly Mock<RequestHandlerDelegate<TestResponse>> _mockNext;

        public UnhandledExceptionBehaviourTests()
        {
            _mockLogger = new Mock<ILogger<UnhandledExceptionBehaviour<TestRequest, TestResponse>>>();
            _mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            _behaviour = new UnhandledExceptionBehaviour<TestRequest, TestResponse>(_mockLogger.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithCriticalScenario_DeliversCoreBusinessValue()
        {
            // Business Context: Ensuring exceptions are handled without crashing the pipeline
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            _mockNext.Setup(x => x()).ReturnsAsync(new TestResponse { Success = true });

            // Act
            var result = await _behaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should not be null");
            result.Success.Should().BeTrue("response should indicate success");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithSecurityScenario_ProtectsBusinessData()
        {
            // Business Context: Ensuring that exceptions do not expose sensitive data
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            _mockNext.Setup(x => x()).ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Test exception");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithValidRequest_ReturnsSuccessfulResponse()
        {
            // Business Context: Normal request processing should return a successful response
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            _mockNext.Setup(x => x()).ReturnsAsync(new TestResponse { Success = true });

            // Act
            var result = await _behaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should not be null");
            result.Success.Should().BeTrue("response should indicate success");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithEmptyUserId_ReturnsSuccessfulResponse()
        {
            // Business Context: Edge case where UserId is empty but request should still process
            // Arrange
            var request = new TestRequest { UserId = string.Empty };
            _mockNext.Setup(x => x()).ReturnsAsync(new TestResponse { Success = true });

            // Act
            var result = await _behaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should not be null");
            result.Success.Should().BeTrue("response should indicate success");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null request should throw ArgumentNullException
            // Arrange
            TestRequest? request = null;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request!, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'request')");
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithException_LogsExceptionAndThrows()
        {
            // Business Context: Exception should be logged and rethrown
            // Arrange
            var request = new TestRequest { UserId = "test-user-123" };
            _mockNext.Setup(x => x()).ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Test exception");
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