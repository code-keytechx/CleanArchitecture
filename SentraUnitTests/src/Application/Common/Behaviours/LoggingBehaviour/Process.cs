using CleanArchitecture.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class LoggingBehaviourTests
{
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<ILogger<LoggingBehaviour<SomeRequest>>> _mockLogger;
    private readonly LoggingBehaviour<SomeRequest> _sut;

    public LoggingBehaviourTests()
    {
        _mockIdentityService = new Mock<IIdentityService>();
        _mockLogger = new Mock<ILogger<LoggingBehaviour<SomeRequest>>>();
        _sut = new LoggingBehaviour<SomeRequest>(_mockIdentityService.Object, _mockLogger.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task Process_WithValidRequest_LogsInformation()
    {
        // Arrange
        var request = new SomeRequest();
        var userId = "user123";
        var userName = "JohnDoe";

        _mockIdentityService.Setup(x => x.GetUserNameAsync(userId)).ReturnsAsync(userName);

        // Act
        await _sut.Process(request, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            log => log.LogInformation(
                It.Is<string>(message => message.Contains("CleanArchitecture Request")),
                It.Is<string>(message => message.Contains("SomeRequest")),
                It.Is<string>(message => message.Contains(userId)),
                It.Is<string>(message => message.Contains(userName)),
                It.Is<object>(message => message.Equals(request))),
            Times.Once);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task Process_WithNullUserId_LogsWithoutUserName()
    {
        // Arrange
        var request = new SomeRequest();
        var userId = (string)null;
        var userName = string.Empty;

        _mockIdentityService.Setup(x => x.GetUserNameAsync(userId)).ReturnsAsync(userName);

        // Act
        await _sut.Process(request, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            log => log.LogInformation(
                It.Is<string>(message => message.Contains("CleanArchitecture Request")),
                It.Is<string>(message => message.Contains("SomeRequest")),
                It.Is<string>(message => message.Contains(userId)),
                It.Is<string>(message => message.Contains(userName)),
                It.Is<object>(message => message.Equals(request))),
            Times.Once);
    }

    [Fact]
    public async Task Process_WithEmptyUserName_LogsWithoutUserName()
    {
        // Arrange
        var request = new SomeRequest();
        var userId = "user123";
        var userName = string.Empty;

        _mockIdentityService.Setup(x => x.GetUserNameAsync(userId)).ReturnsAsync(userName);

        // Act
        await _sut.Process(request, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            log => log.LogInformation(
                It.Is<string>(message => message.Contains("CleanArchitecture Request")),
                It.Is<string>(message => message.Contains("SomeRequest")),
                It.Is<string>(message => message.Contains(userId)),
                It.Is<string>(message => message.Contains(userName)),
                It.Is<object>(message => message.Equals(request))),
            Times.Once);
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task Process_WithInvalidRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var request = default(SomeRequest);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Process(request, CancellationToken.None));
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task Process_WhenGetUserNameAsyncThrowsException_LogsError()
    {
        // Arrange
        var request = new SomeRequest();
        var userId = "user123";
        var userName = string.Empty;

        _mockIdentityService.Setup(x => x.GetUserNameAsync(userId)).ThrowsAsync(new InvalidOperationException("User not found"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Process(request, CancellationToken.None));

        _mockLogger.Verify(
            log => log.LogError(
                It.IsAny<EventId>(),
                It.IsAny<Exception>(),
                It.Is<string>(message => message.Contains("CleanArchitecture Request")),
                It.Is<string>(message => message.Contains("SomeRequest")),
                It.Is<string>(message => message.Contains(userId)),
                It.Is<string>(message => message.Contains(userName)),
                It.Is<object>(message => message.Equals(request))),
            Times.Once);
    }

    #endregion

    #region Helper Methods

    private class SomeRequest : IRequest
    {
    }

    #endregion
}