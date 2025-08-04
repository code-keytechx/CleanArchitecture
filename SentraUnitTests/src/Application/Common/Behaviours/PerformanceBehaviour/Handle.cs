using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class PerformanceBehaviour_Tests
{
    private readonly Stopwatch _timer;
    private readonly Mock<IIdentityService> _mockIdentityService;
    private readonly Mock<ILogger<PerformanceBehaviour<object, object>>> _mockLogger;
    private readonly PerformanceBehaviour<object, object> _sut;

    public PerformanceBehaviour_Tests()
    {
        _timer = new Stopwatch();
        _mockIdentityService = new Mock<IIdentityService>();
        _mockLogger = new Mock<ILogger<PerformanceBehaviour<object, object>>>();
        _sut = new PerformanceBehaviour<object, object>(_timer, _mockIdentityService.Object, _mockLogger.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task Handle_WithShortExecutionTime_ReturnsResponse()
    {
        // Arrange
        var request = new object();
        var response = new object();
        _timer.Stop(); // Reset timer to simulate short execution time

        // Act
        var result = await _sut.Handle(request, async () => response, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        _mockLogger.VerifyNoOtherCalls();
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task Handle_WithZeroExecutionTime_ReturnsResponse()
    {
        // Arrange
        var request = new object();
        var response = new object();
        _timer.Reset(); // Reset timer to simulate zero execution time

        // Act
        var result = await _sut.Handle(request, async () => response, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        _mockLogger.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithBoundaryExecutionTime_ReturnsResponse()
    {
        // Arrange
        var request = new object();
        var response = new object();
        _timer.Restart();
        _timer.Stop();
        _timer.ElapsedMilliseconds = 500; // Exactly at the boundary

        // Act
        var result = await _sut.Handle(request, async () => response, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        _mockLogger.VerifyNoOtherCalls();
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var request = (object)null;
        var response = new object();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.Handle(request, async () => response, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNullResponse_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new object();
        var response = default(object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Handle(request, async () => response, CancellationToken.None));
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task Handle_WhenNextThrowsException_RethrowsException()
    {
        // Arrange
        var request = new object();
        var exception = new InvalidOperationException("Simulated exception");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.Handle(request, async () => throw exception, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenLoggingThrowsException_DoesNotRethrowException()
    {
        // Arrange
        var request = new object();
        var response = new object();
        _mockLogger.Setup(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>())).Throws(new InvalidOperationException("Simulated logging exception"));

        // Act
        var result = await _sut.Handle(request, async () => response, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
    }

    #endregion

    #region Helper Methods

    private static void SetupTimerToReturnElapsedMilliseconds(int milliseconds)
    {
        _timer.Restart();
        _timer.Stop();
        _timer.ElapsedMilliseconds = milliseconds;
    }

    #endregion
}