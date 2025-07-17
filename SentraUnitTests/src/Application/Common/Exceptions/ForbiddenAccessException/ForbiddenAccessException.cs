using CleanArchitecture.Application.Common.Exceptions;
using Xunit;

namespace CleanArchitecture.Application.Tests.Common.Exceptions;

public class ForbiddenAccessExceptionTests
{
    // Test data - varied and realistic
    private readonly List<string> _messages = new()
    {
        "",
        "Access denied",
        "Unauthorized user",
        "No permissions",
        "Forbidden action"
    };

    // Mock declarations
    // None needed for this simple exception class

    // Setup/Constructor
    public ForbiddenAccessExceptionTests()
    {
        // Initialization can be done here if needed
    }

    #region Happy Path Tests

    [Fact]
    public void ForbiddenAccessException_CreatedWithoutMessage_ReturnsDefaultMessage()
    {
        // Arrange
        var exception = new ForbiddenAccessException();

        // Act & Assert
        exception.Message.Should().Be("Access denied");
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void ForbiddenAccessException_CreatedWithEmptyMessage_ReturnsDefaultMessage()
    {
        // Arrange
        var exception = new ForbiddenAccessException("");

        // Act & Assert
        exception.Message.Should().Be("Access denied");
    }

    [Fact]
    public void ForbiddenAccessException_CreatedWithNullMessage_ReturnsDefaultMessage()
    {
        // Arrange
        var exception = new ForbiddenAccessException(null);

        // Act & Assert
        exception.Message.Should().Be("Access denied");
    }

    #endregion

    #region Negative Tests

    [Fact]
    public void ForbiddenAccessException_CannotBeCreatedWithWhitespaceMessage()
    {
        // Arrange
        var message = "   ";

        // Act & Assert
        var exception = new ForbiddenAccessException(message);
        exception.Message.Should().Be("Access denied");
    }

    [Fact]
    public void ForbiddenAccessException_CannotBeCreatedWithLongMessage()
    {
        // Arrange
        var longMessage = new string('A', 1000);

        // Act & Assert
        var exception = new ForbiddenAccessException(longMessage);
        exception.Message.Should().Be("Access denied");
    }

    #endregion

    #region Exception Tests

    [Fact]
    public void ForbiddenAccessException_WhenThrownInMethod_ThrowsMeaningfulException()
    {
        // Arrange
        Action act = () => throw new ForbiddenAccessException("Custom message");

        // Act & Assert
        var exception = act.Should().Throw<ForbiddenAccessException>().Which;
        exception.Message.Should().Be("Custom message");
    }

    #endregion

    #region Helper Methods

    // None needed for this simple exception class

    #endregion
}