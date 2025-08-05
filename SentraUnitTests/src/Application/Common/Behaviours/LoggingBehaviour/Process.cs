using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR.Pipeline;

namespace CleanArchitecture.Application.Tests
{
    public class LoggingBehaviourTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<ILogger<LoggingBehaviour<Request>>> _mockLogger;
        private readonly LoggingBehaviour<Request> _loggingBehaviour;

        public LoggingBehaviourTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockUser = new Mock<IUser>();
            _mockLogger = new Mock<ILogger<LoggingBehaviour<Request>>>();
            _loggingBehaviour = new LoggingBehaviour<Request>(_mockIdentityService.Object, _mockUser.Object, _mockLogger.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Process_WithValidUserLogsRequest()
        {
            // Business Context: Logging user requests is critical for auditing and monitoring
            // Arrange
            var request = new Request();
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.GetUserNameAsync("user123")).ReturnsAsync("JohnDoe");

            // Act
            await _loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _mockLogger.Verify(l => l.LogInformation("CleanArchitecture Request: {Name} {@UserId} {@UserName} {@Request}",
                typeof(Request).Name, "user123", "JohnDoe", request), Times.Once);
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Process_WithNoUserLogsRequest()
        {
            // Business Context: Logging requests without user information is still valuable for auditing
            // Arrange
            var request = new Request();
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act
            await _loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _mockLogger.Verify(l => l.LogInformation("CleanArchitecture Request: {Name} {@UserId} {@UserName} {@Request}",
                typeof(Request).Name, string.Empty, string.Empty, request), Times.Once);
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Process_WithNullIdentityServiceThrowsArgumentNullException()
        {
            // Business Context: Null dependencies should be handled gracefully
            // Arrange
            var loggingBehaviour = new LoggingBehaviour<Request>(null, _mockUser.Object, _mockLogger.Object);

            // Act & Assert
            await loggingBehaviour.Invoking(b => b.Process(new Request(), CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Process_WithNullUserThrowsArgumentNullException()
        {
            // Business Context: Null dependencies should be handled gracefully
            // Arrange
            var loggingBehaviour = new LoggingBehaviour<Request>(_mockIdentityService.Object, null, _mockLogger.Object);

            // Act & Assert
            await loggingBehaviour.Invoking(b => b.Process(new Request(), CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        [Trait("Category", "Security")]
        public async Task Process_WithNullLoggerThrowsArgumentNullException()
        {
            // Business Context: Null dependencies should be handled gracefully
            // Arrange
            var loggingBehaviour = new LoggingBehaviour<Request>(_mockIdentityService.Object, _mockUser.Object, null);

            // Act & Assert
            await loggingBehaviour.Invoking(b => b.Process(new Request(), CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Process_WithValidRequestLogsRequest()
        {
            // Business Context: Valid requests should be logged correctly
            // Arrange
            var request = new Request();
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.GetUserNameAsync("user123")).ReturnsAsync("JohnDoe");

            // Act
            await _loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _mockLogger.Verify(l => l.LogInformation("CleanArchitecture Request: {Name} {@UserId} {@UserName} {@Request}",
                typeof(Request).Name, "user123", "JohnDoe", request), Times.Once);
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Process_WithEmptyUserNameLogsRequest()
        {
            // Business Context: Empty user names should be handled gracefully
            // Arrange
            var request = new Request();
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.GetUserNameAsync("user123")).ReturnsAsync(string.Empty);

            // Act
            await _loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _mockLogger.Verify(l => l.LogInformation("CleanArchitecture Request: {Name} {@UserId} {@UserName} {@Request}",
                typeof(Request).Name, "user123", string.Empty, request), Times.Once);
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Process_WithNullRequestThrowsArgumentNullException()
        {
            // Business Context: Null requests should be handled gracefully
            // Arrange
            Request? request = null;

            // Act & Assert
            await _loggingBehaviour.Invoking(b => b.Process(request!, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Process_WithExceptionInGetUserNameAsyncLogsError()
        {
            // Business Context: Exceptions in dependency methods should be logged
            // Arrange
            var request = new Request();
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.GetUserNameAsync("user123")).ThrowsAsync(new Exception("Error fetching user name"));

            // Act
            await _loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), "Error fetching user name"), Times.Once);
        }

        #endregion
    }

    public class Request : IRequest
    {
    }
}