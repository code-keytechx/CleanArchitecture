using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class UnhandledExceptionBehaviour_Tests
{
    private readonly Mock<ILogger<UnhandledExceptionBehaviour<object, object>>> _mockLogger;
    private readonly UnhandledExceptionBehaviour<object, object> _behaviour;

    public UnhandledExceptionBehaviour_Tests()
    {
        _mockLogger = new Mock<ILogger<UnhandledExceptionBehaviour<object, object>>>();
        _behaviour = new UnhandledExceptionBehaviour<object, object>(_mockLogger.Object);
    }

    #region Happy Path Tests

    [Fact]
    public async Task Handle_WithSuccessfulExecution_ReturnsResponse()
    {
        // Arrange
        var request = new object();
        var response = new object();
        var next = new Mock<RequestHandlerDelegate<object>>();
        next.Setup(n => n()).ReturnsAsync(response);

        // Act
        var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        _mockLogger.VerifyNoOtherCalls();
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        var request = default(object);
        var next = new Mock<RequestHandlerDelegate<object>>();
        next.Setup(n => n()).ReturnsAsync(default(object));

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task Handle_WithInvalidRequestType_ThrowsArgumentException()
    {
        // Arrange
        var request = new { Id = 1 };
        var next = new Mock<RequestHandlerDelegate<object>>();
        next.Setup(n => n()).ReturnsAsync(default(object));

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
    }

    #endregion

    #region Exception Tests

    [Fact]
    public async Task Handle_WhenInnerExceptionOccurs_LogsErrorAndRethrows()
    {
        // Arrange
        var request = new object();
        var innerException = new InvalidOperationException("Something went wrong");
        var next = new Mock<RequestHandlerDelegate<object>>();
        next.Setup(n => n()).ThrowsAsync(innerException);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() => _behaviour.Handle(request, next.Object, CancellationToken.None));
        _mockLogger.Verify(l => l.LogError(innerException, "CleanArchitecture Request: Unhandled Exception for Request {Name} {@Request}", typeof(object).Name, request), Times.Once);
    }

    #endregion
}