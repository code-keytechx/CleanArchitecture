// MANDATORY using statements:
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using MediatR;
using Microsoft.Extensions.Logging;

/*
REQUIRED NUGET PACKAGES:
- xunit
- xunit.runner.visualstudio  
- FluentAssertions
- Moq
- Microsoft.NET.Test.Sdk
- MediatR
- Microsoft.Extensions.Logging

CRITICAL: Check .csproj for ALL PackageReference elements and include corresponding using statements
*/

namespace CleanArchitecture.Application.Tests.Common.Behaviours
{
    public class UnhandledExceptionBehaviourTests
    {
        private readonly Mock<ILogger<TestRequest>> _mockLogger;
        private readonly Mock<RequestHandlerDelegate<TestResponse>> _mockNext;
        private readonly UnhandledExceptionBehaviour<TestRequest, TestResponse> _behaviour;

        public UnhandledExceptionBehaviourTests()
        {
            _mockLogger = new Mock<ILogger<TestRequest>>();
            _mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            _behaviour = new UnhandledExceptionBehaviour<TestRequest, TestResponse>(_mockLogger.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithSuccessfulRequest_ReturnsResponse()
        {
            // Business Context: This behavior ensures that successful requests are processed without interruption
            // Arrange
            var testRequest = new TestRequest { Id = 1, Name = "Test" };
            var testResponse = new TestResponse { Result = "Success" };
            _mockNext.Setup(n => n()).ReturnsAsync(testResponse);

            // Act
            var result = await _behaviour.Handle(testRequest, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(testResponse);
            _mockNext.Verify(n => n(), Times.Once);
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithException_LogsErrorAndReThrows()
        {
            // Business Context: Proper error logging is critical for system monitoring and debugging
            // Arrange
            var testRequest = new TestRequest { Id = 1, Name = "Test" };
            var exception = new InvalidOperationException("Test exception");
            _mockNext.Setup(n => n()).ThrowsAsync(exception);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(testRequest, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Test exception");

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unhandled Exception for Request")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithValidRequest_ReturnsExpectedResponse()
        {
            // Business Context: Standard execution path must return correct results
            // Arrange
            var testRequest = new TestRequest { Id = 1, Name = "Test" };
            var expectedResponse = new TestResponse { Result = "Processed" };
            _mockNext.Setup(n => n()).ReturnsAsync(expectedResponse);

            // Act
            var result = await _behaviour.Handle(testRequest, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithNullRequest_LogsErrorAndReThrows()
        {
            // Business Context: Null request handling should be robust and logged appropriately
            // Arrange
            TestRequest? testRequest = null;
            var exception = new InvalidOperationException("Test exception");
            _mockNext.Setup(n => n()).ThrowsAsync(exception);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(testRequest!, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Test exception");

            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Unhandled Exception for Request")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithException_ThrowsOriginalException()
        {
            // Business Context: Exception handling must preserve original exception type for proper error handling
            // Arrange
            var testRequest = new TestRequest { Id = 1, Name = "Test" };
            var exception = new ArgumentException("Invalid argument");
            _mockNext.Setup(n => n()).ThrowsAsync(exception);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(testRequest, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("Invalid argument");
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithNullLogger_LogsErrorAndReThrows()
        {
            // Business Context: Behavior must handle null logger gracefully while maintaining error propagation
            // Arrange
            var testRequest = new TestRequest { Id = 1, Name = "Test" };
            var exception = new InvalidOperationException("Test exception");
            _mockNext.Setup(n => n()).ThrowsAsync(exception);
            var behaviourWithNullLogger = new UnhandledExceptionBehaviour<TestRequest, TestResponse>(null!);

            // Act & Assert
            await behaviourWithNullLogger.Invoking(b => b.Handle(testRequest, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Test exception");
        }

        #endregion
    }

    // Test helper classes
    public class TestRequest : IRequest<TestResponse>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class TestResponse
    {
        public string? Result { get; set; }
    }
}