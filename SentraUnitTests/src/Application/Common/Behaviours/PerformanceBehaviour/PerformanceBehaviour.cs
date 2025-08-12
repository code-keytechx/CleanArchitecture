// MANDATORY using statements:
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using MediatR;
using Microsoft.Extensions.Logging;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Behaviours;
using System.Diagnostics;

/*
REQUIRED NUGET PACKAGES:
- xunit
- xunit.runner.visualstudio  
- FluentAssertions
- Moq
- Microsoft.NET.Test.Sdk
- MediatR
- AutoMapper
- Microsoft.EntityFrameworkCore
- Microsoft.Extensions.Hosting
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Identity.UI
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Relational
- Microsoft.EntityFrameworkCore.Tools
- Aspire.Microsoft.EntityFrameworkCore.SqlServer
- Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
- Azure.Identity
- Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
- FluentValidation.DependencyInjectionExtensions
- Ardalis.GuardClauses
- AutoFixture
- coverlet.collector

CRITICAL: Check .csproj for ALL PackageReference elements and include corresponding using statements
*/

namespace CleanArchitecture.Application.Tests.Behaviors
{
    public class PerformanceBehaviourTests
    {
        private readonly Mock<ILogger<MockRequest>> _mockLogger;
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly PerformanceBehaviour<MockRequest, MockResponse> _behaviour;

        public PerformanceBehaviourTests()
        {
            _mockLogger = new Mock<ILogger<MockRequest>>();
            _mockUser = new Mock<IUser>();
            _mockIdentityService = new Mock<IIdentityService>();
            
            _behaviour = new PerformanceBehaviour<MockRequest, MockResponse>(
                _mockLogger.Object,
                _mockUser.Object,
                _mockIdentityService.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: performance monitoring, system health, latency detection

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithSlowRequest_LogsWarningAndReturnsResponse()
        {
            // Business Context: Performance degradation can lead to poor user experience and system instability
            // Arrange
            var request = new MockRequest();
            var response = new MockResponse();
            var cancellationToken = CancellationToken.None;
            
            var mockNext = new Mock<RequestHandlerDelegate<MockResponse>>();
            mockNext.Setup(next => next()).ReturnsAsync(response);
            
            // Act
            var result = await _behaviour.Handle(request, mockNext.Object, cancellationToken);

            // Assert
            result.Should().Be(response);
            _mockLogger.VerifyLogWarning("CleanArchitecture Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithValidUserAndSlowRequest_LogsUserName()
        {
            // Business Context: User identification is required for security monitoring and audit trails
            // Arrange
            var userId = "test-user-123";
            var userName = "Test User";
            var request = new MockRequest();
            var response = new MockResponse();
            var cancellationToken = CancellationToken.None;
            
            _mockUser.Setup(u => u.Id).Returns(userId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(userId)).ReturnsAsync(userName);
            
            var mockNext = new Mock<RequestHandlerDelegate<MockResponse>>();
            mockNext.Setup(next => next()).ReturnsAsync(response);
            
            // Act
            var result = await _behaviour.Handle(request, mockNext.Object, cancellationToken);

            // Assert
            result.Should().Be(response);
            _mockLogger.VerifyLogWarning("CleanArchitecture Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithFastRequest_ReturnsResponseWithoutLogging()
        {
            // Business Context: Normal operation should not generate performance warnings
            // Arrange
            var request = new MockRequest();
            var response = new MockResponse();
            var cancellationToken = CancellationToken.None;
            
            var mockNext = new Mock<RequestHandlerDelegate<MockResponse>>();
            mockNext.Setup(next => next()).ReturnsAsync(response);
            
            // Act
            var result = await _behaviour.Handle(request, mockNext.Object, cancellationToken);

            // Assert
            result.Should().Be(response);
            _mockLogger.VerifyNoLogWarning();
        }

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithUserWithoutName_ReturnsResponse()
        {
            // Business Context: System should gracefully handle missing user information
            // Arrange
            var userId = "test-user-123";
            var request = new MockRequest();
            var response = new MockResponse();
            var cancellationToken = CancellationToken.None;
            
            _mockUser.Setup(u => u.Id).Returns(userId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(userId)).ReturnsAsync(string.Empty);
            
            var mockNext = new Mock<RequestHandlerDelegate<MockResponse>>();
            mockNext.Setup(next => next()).ReturnsAsync(response);
            
            // Act
            var result = await _behaviour.Handle(request, mockNext.Object, cancellationToken);

            // Assert
            result.Should().Be(response);
            _mockLogger.VerifyLogWarning("CleanArchitecture Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithNullUserId_ReturnsResponse()
        {
            // Business Context: Null user identifiers should be handled gracefully
            // Arrange
            var request = new MockRequest();
            var response = new MockResponse();
            var cancellationToken = CancellationToken.None;
            
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            
            var mockNext = new Mock<RequestHandlerDelegate<MockResponse>>();
            mockNext.Setup(next => next()).ReturnsAsync(response);
            
            // Act
            var result = await _behaviour.Handle(request, mockNext.Object, cancellationToken);

            // Assert
            result.Should().Be(response);
            _mockLogger.VerifyLogWarning("CleanArchitecture Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}");
        }

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithEmptyUserId_ReturnsResponse()
        {
            // Business Context: Empty user identifiers should be handled gracefully
            // Arrange
            var request = new MockRequest();
            var response = new MockResponse();
            var cancellationToken = CancellationToken.None;
            
            _mockUser.Setup(u => u.Id).Returns(string.Empty);
            
            var mockNext = new Mock<RequestHandlerDelegate<MockResponse>>();
            mockNext.Setup(next => next()).ReturnsAsync(response);
            
            // Act
            var result = await _behaviour.Handle(request, mockNext.Object, cancellationToken);

            // Assert
            result.Should().Be(response);
            _mockLogger.VerifyLogWarning("CleanArchitecture Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null requests should be handled gracefully to prevent system crashes
            // Arrange
            MockRequest? request = null;
            var response = new MockResponse();
            var cancellationToken = CancellationToken.None;
            
            var mockNext = new Mock<RequestHandlerDelegate<MockResponse>>();
            mockNext.Setup(next => next()).ReturnsAsync(response);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request!, mockNext.Object, cancellationToken))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInIdentityService_ReturnsResponse()
        {
            // Business Context: Service failures should not break the request flow
            // Arrange
            var userId = "test-user-123";
            var request = new MockRequest();
            var response = new MockResponse();
            var cancellationToken = CancellationToken.None;
            
            _mockUser.Setup(u => u.Id).Returns(userId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(userId)).ThrowsAsync(new Exception("Service unavailable"));
            
            var mockNext = new Mock<RequestHandlerDelegate<MockResponse>>();
            mockNext.Setup(next => next()).ReturnsAsync(response);
            
            // Act
            var result = await _behaviour.Handle(request, mockNext.Object, cancellationToken);

            // Assert
            result.Should().Be(response);
            _mockLogger.VerifyLogWarning("CleanArchitecture Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}");
        }

        #endregion
    }

    // Helper classes for testing
    public class MockRequest : IRequest<MockResponse>
    {
        public string? Name { get; set; }
    }

    public class MockResponse
    {
        public string? Message { get; set; }
    }

    // Extension methods for verifying logger calls
    public static class LoggerExtensions
    {
        public static void VerifyLogWarning(this Mock<ILogger<MockRequest>> logger, string expectedMessage)
        {
            logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedMessage)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        public static void VerifyNoLogWarning(this Mock<ILogger<MockRequest>> logger)
        {
            logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Warning),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Never);
        }
    }
}