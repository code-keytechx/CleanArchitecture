using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.SentraUnitTests.Application.Common.Behaviours
{
    public class LoggingBehaviourTests
    {
        private readonly Mock<ILogger<MockRequest>> _loggerMock;
        private readonly Mock<IUser> _userMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly LoggingBehaviour<MockRequest> _loggingBehaviour;

        public LoggingBehaviourTests()
        {
            _loggerMock = new Mock<ILogger<MockRequest>>();
            _userMock = new Mock<IUser>();
            _identityServiceMock = new Mock<IIdentityService>();
            _loggingBehaviour = new LoggingBehaviour<MockRequest>(_loggerMock.Object, _userMock.Object, _identityServiceMock.Object);
        }

        [Fact]
        public async Task Process_WhenUserIdIsNull_ShouldLogRequestWithoutUserName()
        {
            // Arrange
            var request = new MockRequest();
            _userMock.Setup(u => u.Id).Returns((string)null);

            // Act
            await _loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CleanArchitecture Request: MockRequest")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task Process_WhenUserIdIsNotEmpty_ShouldFetchUserNameAndLogRequest()
        {
            // Arrange
            var request = new MockRequest();
            var userId = "test-user-id";
            var userName = "test-user-name";
            _userMock.Setup(u => u.Id).Returns(userId);
            _identityServiceMock.Setup(s => s.GetUserNameAsync(userId)).ReturnsAsync(userName);

            // Act
            await _loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CleanArchitecture Request: MockRequest") && 
 v.ToString().Contains(userId) && 
 v.ToString().Contains(userName)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task Process_WhenUserIdIsEmpty_ShouldLogRequestWithoutUserName()
        {
            // Arrange
            var request = new MockRequest();
            _userMock.Setup(u => u.Id).Returns(string.Empty);

            // Act
            await _loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CleanArchitecture Request: MockRequest")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task Process_WhenUserIdIsNotEmptyButUserNameIsNull_ShouldLogRequestWithEmptyUserName()
        {
            // Arrange
            var request = new MockRequest();
            var userId = "test-user-id";
            _userMock.Setup(u => u.Id).Returns(userId);
            _identityServiceMock.Setup(s => s.GetUserNameAsync(userId)).ReturnsAsync((string)null);

            // Act
            await _loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CleanArchitecture Request: MockRequest") && 
 v.ToString().Contains(userId) && 
 v.ToString().Contains("")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task Process_WhenUserIdIsNotEmptyButUserNameIsEmpty_ShouldLogRequestWithEmptyUserName()
        {
            // Arrange
            var request = new MockRequest();
            var userId = "test-user-id";
            _userMock.Setup(u => u.Id).Returns(userId);
            _identityServiceMock.Setup(s => s.GetUserNameAsync(userId)).ReturnsAsync(string.Empty);

            // Act
            await _loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CleanArchitecture Request: MockRequest") && 
 v.ToString().Contains(userId) && 
 v.ToString().Contains("")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task Process_WithDifferentRequestType_ShouldLogCorrectRequestName()
        {
            // Arrange
            var loggingBehaviour = new LoggingBehaviour<AnotherMockRequest>(_loggerMock.Object, _userMock.Object, _identityServiceMock.Object);
            var request = new AnotherMockRequest();
            _userMock.Setup(u => u.Id).Returns((string)null);

            // Act
            await loggingBehaviour.Process(request, CancellationToken.None);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CleanArchitecture Request: AnotherMockRequest")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
        }

        [Fact]
        public async Task Process_WithNullRequest_ShouldLogNullRequest()
        {
            // Arrange
            MockRequest request = null;
            _userMock.Setup(u => u.Id).Returns((string)null);

            // Act & Assert
            await _loggingBehaviour.Invoking(async x => await x.Process(request, CancellationToken.None))
                .Should().NotThrowAsync();
        }

        [Fact]
        public async Task Process_WithCancellationToken_ShouldCompleteSuccessfully()
        {
            // Arrange
            var request = new MockRequest();
            _userMock.Setup(u => u.Id).Returns((string)null);
            var cancellationToken = new CancellationToken();

            // Act
            await _loggingBehaviour.Process(request, cancellationToken);

            // Assert
            // No exception should be thrown
        }
    }

    // Mock request types for testing
    public class MockRequest
    {
        public string Name { get; set; } = "Test Request";
    }

    public class AnotherMockRequest
    {
        public string Name { get; set; } = "Another Test Request";
    }
}