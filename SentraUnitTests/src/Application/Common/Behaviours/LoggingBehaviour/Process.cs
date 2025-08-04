using CleanArchitecture.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Tests.UnitTests.Behaviors
{
    public class LoggingBehaviorTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<ILogger<LoggingBehaviour<SampleRequest>>> _mockLogger;
        private readonly LoggingBehaviour<SampleRequest> _loggingBehaviour;

        public LoggingBehaviorTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockLogger = new Mock<ILogger<LoggingBehaviour<SampleRequest>>>();
            _loggingBehaviour = new LoggingBehaviour<SampleRequest>(_mockIdentityService.Object, _mockLogger.Object);
        }

        #region Happy Path Tests

        [Fact]
        public async Task Process_WithValidRequest_LogsInformation()
        {
            // Arrange
            var sampleRequest = new SampleRequest();
            _mockIdentityService.Setup(x => x.GetUserNameAsync(It.IsAny<string>())).ReturnsAsync("JohnDoe");

            // Act
            await _loggingBehaviour.Process(sampleRequest, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                log => log.LogInformation(
                    It.Is<string>(message => message.Contains("CleanArchitecture Request")),
                    It.Is<string>(message => message.Contains("SampleRequest")),
                    It.Is<string>(message => message.Contains("JohnDoe")),
                    It.Is<object>(message => message.Equals(sampleRequest))),
                Times.Once);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Process_WithNullUserId_LogsWithoutUserName()
        {
            // Arrange
            var sampleRequest = new SampleRequest();
            _mockIdentityService.Setup(x => x.GetUserNameAsync(It.IsAny<string>())).ReturnsAsync((string)null);

            // Act
            await _loggingBehaviour.Process(sampleRequest, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                log => log.LogInformation(
                    It.Is<string>(message => message.Contains("CleanArchitecture Request")),
                    It.Is<string>(message => message.Contains("SampleRequest")),
                    It.Is<string>(message => message.Contains("<anonymous>")),
                    It.Is<object>(message => message.Equals(sampleRequest))),
                Times.Once);
        }

        [Fact]
        public async Task Process_WithEmptyUserId_LogsWithoutUserName()
        {
            // Arrange
            var sampleRequest = new SampleRequest();
            _mockIdentityService.Setup(x => x.GetUserNameAsync(string.Empty)).ReturnsAsync((string)null);

            // Act
            await _loggingBehaviour.Process(sampleRequest, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                log => log.LogInformation(
                    It.Is<string>(message => message.Contains("CleanArchitecture Request")),
                    It.Is<string>(message => message.Contains("SampleRequest")),
                    It.Is<string>(message => message.Contains("<anonymous>")),
                    It.Is<object>(message => message.Equals(sampleRequest))),
                Times.Once);
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Process_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            SampleRequest sampleRequest = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _loggingBehaviour.Process(sampleRequest, CancellationToken.None));
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Process_WhenGetUserNameAsyncThrowsException_LogsError()
        {
            // Arrange
            var sampleRequest = new SampleRequest();
            _mockIdentityService.Setup(x => x.GetUserNameAsync(It.IsAny<string>())).ThrowsAsync(new InvalidOperationException("User not found"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _loggingBehaviour.Process(sampleRequest, CancellationToken.None));

            _mockLogger.Verify(
                log => log.LogError(
                    It.IsAny<EventId>(),
                    It.IsAny<Exception>(),
                    It.Is<string>(message => message.Contains("Error occurred while logging user name")),
                    It.Is<string>(message => message.Contains("SampleRequest"))),
                Times.Once);
        }

        #endregion

        #region Helper Methods

        private class SampleRequest : IRequest
        {
        }

        #endregion
    }
}