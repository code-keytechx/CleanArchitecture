using System.Collections.Generic;

public class Result
{
    public bool IsSuccess { get; }
    public IEnumerable<string> Errors { get; }

    private Result(bool isSuccess, IEnumerable<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Success()
    {
        return new Result(true, new List<string>());
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }
}

public class ResultTests
{
    // Test data - varied and realistic
    private readonly List<(IEnumerable<string>, bool)> _testCases = new()
    {
        (new List<string>(), true), // Empty list of errors
        (new List<string> { "Error 1", "Error 2" }, false), // Non-empty list of errors
        (null, false) // Null list of errors
    };

    // Mock declarations

    // Setup/Constructor

    #region Happy Path Tests

    [Test]
    public void Success_CreatesResultWithNoErrors()
    {
        // Arrange
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    #endregion

    #region Edge Case Tests

    [Test]
    public void Failure_CreatesResultWithNonEmptyErrors()
    {
        // Arrange
        var errors = new List<string> { "Error 1", "Error 2" };

        // Act
        var result = Result.Failure(errors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain("Error 1");
        result.Errors.Should().Contain("Error 2");
    }

    [Test]
    public void Failure_CreatesResultWithNullErrors()
    {
        // Arrange
        IEnumerable<string> nullErrors = null;

        // Act
        var result = Result.Failure(nullErrors);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    #endregion

    #region Negative Tests

    [Test]
    public void Failure_WithEmptyList_ShouldFail()
    {
        // Arrange
        var emptyList = new List<string>();

        // Act
        var result = Result.Failure(emptyList);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Failure_WithSingleError_ShouldFail()
    {
        // Arrange
        var singleError = new List<string> { "Single Error" };

        // Act
        var result = Result.Failure(singleError);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors.Should().Contain("Single Error");
    }

    #endregion

    #region Exception Tests

    [Test]
    public void Failure_WithNullErrors_ShouldHandleGracefully()
    {
        // Arrange
        IEnumerable<string> nullErrors = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
        {
            var result = Result.Failure(nullErrors);
        });

        exception.ParamName.Should().Be("errors");
    }

    #endregion

    #region Helper Methods

    // Reusable test utilities

    #endregion
}