using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Interfaces;
using FluentAssertions;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.SentraUnitTests.Application.Common.Behaviours
{
    public class LoggingBehaviourTests
    {
        private readonly Mock<ILogger<TestRequest>> _mockLogger;
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly LoggingBehaviour<TestRequest> _behaviour;

        public LoggingBehaviourTests()
        {
            _mockLogger = new Mock<ILogger<TestRequest>>();
            _mockUser = new Mock<IUser>();
            _mockIdentityService = new Mock<IIdentityService>();
            _behaviour = new LoggingBehaviour<TestRequest>(_mockLogger.Object, _mockUser.Object, _mockIdentityService.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Process_WithValidUser_LogsCorrectInformation()
        {
            // Business Context: Logging is critical for auditing and debugging
            // Arrange
            var testUserId = "test-user-123";
            var testUserName = "test-username";
            var request = new TestRequest();

            _mockUser.Setup(u => u.Id).Returns(testUserId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(testUserId)).ReturnsAsync(testUserName);

            // Act
            await _behaviour.Process(request, CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog($"CleanArchitecture Request: TestRequest {testUserId} {testUserName} {request}", LogLevel.Information, Times.Once());
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Process_WithInvalidUserId_LogsEmptyUserName()
        {
            // Business Context: Ensure that invalid user IDs do not cause errors
            // Arrange
            var request = new TestRequest();

            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            await _behaviour.Process(request, CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog($"CleanArchitecture Request: TestRequest  {string.Empty} {request}", LogLevel.Information, Times.Once());
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Process_WithValidRequest_LogsRequestDetails()
        {
            // Business Context: Ensure that valid requests are logged correctly
            // Arrange
            var testUserId = "test-user-123";
            var testUserName = "test-username";
            var request = new TestRequest();

            _mockUser.Setup(u => u.Id).Returns(testUserId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(testUserId)).ReturnsAsync(testUserName);

            // Act
            await _behaviour.Process(request, CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog($"CleanArchitecture Request: TestRequest {testUserId} {testUserName} {request}", LogLevel.Information, Times.Once());
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Process_WithEmptyUserId_LogsEmptyUserName()
        {
            // Business Context: Ensure that empty user IDs do not cause errors
            // Arrange
            var request = new TestRequest();

            _mockUser.Setup(u => u.Id).Returns(string.Empty);

            // Act
            await _behaviour.Process(request, CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog($"CleanArchitecture Request: TestRequest  {string.Empty} {request}", LogLevel.Information, Times.Once());
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Process_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Ensure that null requests are handled gracefully
            // Arrange
            TestRequest? request = null;

            // Act & Assert
            await _behaviour.Invoking(b => b.Process(request!, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Process_WithIdentityServiceException_LogsError()
        {
            // Business Context: Ensure that exceptions from identity service are logged
            // Arrange
            var testUserId = "test-user-123";
            var request = new TestRequest();

            _mockUser.Setup(u => u.Id).Returns(testUserId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(testUserId)).ThrowsAsync(new InvalidOperationException("Service error"));

            // Act
            await _behaviour.Process(request, CancellationToken.None);

            // Assert
            _mockLogger.VerifyLog($"CleanArchitecture Request: TestRequest {testUserId}  {request}", LogLevel.Information, Times.Once());
        }

        #endregion

        // Helper class for testing
        public class TestRequest { }
    }

    // Extension method to verify log messages
    public static class MockLoggerExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> logger, string message, LogLevel logLevel, Times times)
        {
            logger.Verify(x => x.Log(
                It.Is<LogLevel>(l => l == logLevel),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), times);
        }
    }
}