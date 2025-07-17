using System;
using Xunit;

public class Result
{
    public bool IsSuccess { get; }
    public string[] Errors { get; }

    private Result(bool isSuccess, string[] errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Failure(string[] errors)
    {
        return new Result(false, errors);
    }
}

public class ResultTests
{
    #region Happy Path Tests

    [Fact]
    public void Success_ShouldReturnTrueForIsSuccess()
    {
        // Arrange
        var result = Result.Success();

        // Act & Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Length.Should().Be(0);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Success_ShouldReturnEmptyArrayForErrors()
    {
        // Arrange
        var result = Result.Success();

        // Act & Assert
        result.Errors.Length.Should().Be(0);
    }

    #endregion

    #region Negative Tests

    [Fact]
    public void Failure_ShouldReturnFalseForIsSuccess()
    {
        // Arrange
        var errors = new[] { "Error 1", "Error 2" };
        var result = Result.Failure(errors);

        // Act & Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Length.Should().Be(2);
        result.Errors.Should().ContainInOrder("Error 1", "Error 2");
    }

    #endregion

    #region Exception Tests

    [Fact]
    public void Failure_ShouldAllowMultipleErrors()
    {
        // Arrange
        var errors = new[] { "Error 1", "Error 2", "Error 3" };
        var result = Result.Failure(errors);

        // Act & Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Length.Should().Be(3);
        result.Errors.Should().ContainInOrder("Error 1", "Error 2", "Error 3");
    }

    #endregion
}