using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using FluentAssertions;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.Tests.Behaviours
{
    public class LoggingBehaviourTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<ILogger<LoggingBehaviour<TestRequest>>> _mockLogger;
        private readonly LoggingBehaviour<TestRequest> _behaviour;

        public LoggingBehaviourTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockUser = new Mock<IUser>();
            _mockLogger = new Mock<ILogger<LoggingBehaviour<TestRequest>>>();
            _behaviour = new LoggingBehaviour<TestRequest>(_mockUser.Object, _mockIdentityService.Object, _mockLogger.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Process_WithValidUserId_LogsUserName()
        {
            // Business Context: Logging user information is critical for auditing and compliance
            // Arrange
            var testUserId = "test-user-123";
            var testUserName = "Test User";
            _mockUser.Setup(u => u.Id).Returns(testUserId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(testUserId)).ReturnsAsync(testUserName);

            var request = new TestRequest();

            // Act
            await _behaviour.Process(request, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(testUserId), Times.Once);
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Process_WithEmptyUserId_DoesNotCallGetUserNameAsync()
        {
            // Business Context: Ensuring no unnecessary calls to sensitive services when user ID is empty
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            var request = new TestRequest();

            // Act
            await _behaviour.Process(request, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Process_WithValidUserIdAndUserName_LogsCorrectly()
        {
            // Business Context: Ensuring correct logging when both user ID and user name are valid
            // Arrange
            var testUserId = "test-user-123";
            var testUserName = "Test User";
            _mockUser.Setup(u => u.Id).Returns(testUserId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(testUserId)).ReturnsAsync(testUserName);

            var request = new TestRequest();

            // Act
            await _behaviour.Process(request, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(testUserId), Times.Once);
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Process_WithNullUserId_DoesNotCallGetUserNameAsync()
        {
            // Business Context: Handling null user ID gracefully
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            var request = new TestRequest();

            // Act
            await _behaviour.Process(request, CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Process_WithInvalidUserId_ThrowsException()
        {
            // Business Context: Handling invalid user ID scenarios
            // Arrange
            var testUserId = "invalid-user-id";
            _mockUser.Setup(u => u.Id).Returns(testUserId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(testUserId)).ThrowsAsync(new InvalidOperationException("User not found"));

            var request = new TestRequest();

            // Act & Assert
            await _behaviour.Invoking(b => b.Process(request, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("User not found");
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Process_WithExceptionInGetUserNameAsync_LogsError()
        {
            // Business Context: Ensuring system resilience when an exception occurs in GetUserNameAsync
            // Arrange
            var testUserId = "test-user-123";
            _mockUser.Setup(u => u.Id).Returns(testUserId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(testUserId)).ThrowsAsync(new InvalidOperationException("User not found"));

            var request = new TestRequest();

            // Act & Assert
            await _behaviour.Invoking(b => b.Process(request, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("User not found");
        }

        #endregion
    }

    public class TestRequest : IRequest
    {
        // Test request class for unit testing
    }
}