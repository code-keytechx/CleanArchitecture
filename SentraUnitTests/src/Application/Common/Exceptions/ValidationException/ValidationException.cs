using FluentAssertions;
using FluentValidation.Results;
using System.Collections.Generic;
using Xunit;

namespace CleanArchitecture.Application.Common.Exceptions.Tests
{
    public class ValidationExceptionTests
    {
        // Test data - varied and realistic
        private readonly List<ValidationFailure> _validationFailures = new()
        {
            new ValidationFailure("Email", "The email address is not in a valid format."),
            new ValidationFailure("Password", "The password must be at least 8 characters long.")
        };

        // Mock declarations
        private readonly Mock<IDictionary<string, string[]>> _errorsMock;

        // System under test
        private readonly ValidationException _sut;

        // Constructor with setup
        public ValidationExceptionTests()
        {
            _errorsMock = new Mock<IDictionary<string, string[]>>();
            _errorsMock.SetupGet(e => e.GetEnumerator())
                       .Returns(_validationFailures.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                                                   .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray())
                                                   .GetEnumerator);

            _sut = new ValidationException(_validationFailures);
        }

        #region Happy Path Tests

        [Fact]
        public void ValidationException_Ctor_WithValidationFailures_SetsErrorsProperty()
        {
            // Arrange
            var expectedErrors = new Dictionary<string, string[]>
            {
                { "Email", new[] { "The email address is not in a valid format." } },
                { "Password", new[] { "The password must be at least 8 characters long." } }
            };

            // Act
            var sut = new ValidationException(_validationFailures);

            // Assert
            sut.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void ValidationException_Ctor_WithEmptyValidationFailures_SetsEmptyErrorsProperty()
        {
            // Arrange
            var emptyValidationFailures = new List<ValidationFailure>();

            // Act
            var sut = new ValidationException(emptyValidationFailures);

            // Assert
            sut.Errors.Should().BeEmpty();
        }

        #endregion

        #region Negative Tests

        [Fact]
        public void ValidationException_Ctor_WithNullValidationFailures_ThrowsArgumentNullException()
        {
            // Arrange
            IEnumerable<ValidationFailure> nullValidationFailures = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new ValidationException(nullValidationFailures));

            exception.ParamName.Should().Be("failures");
        }

        #endregion

        #region Exception Tests

        [Fact]
        public void ValidationException_Ctor_WithoutValidationFailures_SetsDefaultMessage()
        {
            // Arrange
            var defaultMessage = "One or more validation failures have occurred.";

            // Act
            var sut = new ValidationException();

            // Assert
            sut.Message.Should().Be(defaultMessage);
        }

        #endregion

        #region Helper Methods

        private static IEnumerable<ValidationFailure> CreateValidationFailures(string propertyName, string errorMessage)
        {
            yield return new ValidationFailure(propertyName, errorMessage);
        }

        #endregion
    }
}