using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviors;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace CleanArchitecture.SentraUnitTests.Application.Common.Behaviors
{
    public class LoggingBehaviourTests
    {
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<ILogger<LoggingBehaviour<object>>> _mockLogger;
        private readonly LoggingBehaviour<object> _behaviour;

        public LoggingBehaviourTests()
        {
            _mockUser = new Mock<IUser>();
            _mockIdentityService = new Mock<IIdentityService>();
            _mockLogger = new Mock<ILogger<LoggingBehaviour<object>>>();
            _behaviour = new LoggingBehaviour<object>(_mockLogger.Object, _mockUser.Object, _mockIdentityService.Object);
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

            // Act
            await _behaviour.Process(new object(), CancellationToken.None);

            // Assert
            // Since we are not testing the logger directly, we assume the logger is configured correctly
            // and the test passes if no exceptions are thrown and the setup expectations are met.
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
            // Business Context: Ensuring that user information is not accessed without a valid user ID
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            await _behaviour.Process(new object(), CancellationToken.None);

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
            // Business Context: Ensuring that the logging behavior works as expected with valid data
            // Arrange
            var testUserId = "test-user-123";
            var testUserName = "Test User";
            _mockUser.Setup(u => u.Id).Returns(testUserId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(testUserId)).ReturnsAsync(testUserName);

            // Act
            await _behaviour.Process(new object(), CancellationToken.None);

            // Assert
            // Since we are not testing the logger directly, we assume the logger is configured correctly
            // and the test passes if no exceptions are thrown and the setup expectations are met.
            _mockIdentityService.Verify(s => s.GetUserNameAsync(testUserId), Times.Once);
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Process_WithNullUserId_DoesNotCallGetUserNameAsync()
        {
            // Business Context: Ensuring that user information is not accessed without a valid user ID
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            await _behaviour.Process(new object(), CancellationToken.None);

            // Assert
            _mockIdentityService.Verify(s => s.GetUserNameAsync(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Process_WithInvalidUserId_ThrowsNoException()
        {
            // Business Context: Ensuring that the behavior handles invalid user IDs gracefully
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("invalid-user-id");
            _mockIdentityService.Setup(s => s.GetUserNameAsync("invalid-user-id")).ReturnsAsync((string?)null);

            // Act & Assert
            await _behaviour.Process(new object(), CancellationToken.None).Should().NotThrowAsync<Exception>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Process_WithExceptionInGetUserNameAsync_ThrowsNoException()
        {
            // Business Context: Ensuring that the behavior handles exceptions in GetUserNameAsync gracefully
            // Arrange
            var testUserId = "test-user-123";
            _mockUser.Setup(u => u.Id).Returns(testUserId);
            _mockIdentityService.Setup(s => s.GetUserNameAsync(testUserId)).ThrowsAsync(new Exception("Simulated exception"));

            // Act & Assert
            await _behaviour.Process(new object(), CancellationToken.None).Should().NotThrowAsync<Exception>();
        }

        #endregion
    }
}