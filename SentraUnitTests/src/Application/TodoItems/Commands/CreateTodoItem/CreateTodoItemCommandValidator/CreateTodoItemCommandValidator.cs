using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem.Tests
{
    public class CreateTodoItemCommandValidatorTests
    {
        // Test data - varied and realistic
        private readonly CreateTodoItemCommandValidator _validator;

        // Mock declarations

        // Setup/Constructor
        public CreateTodoItemCommandValidatorTests()
        {
            _validator = new CreateTodoItemCommandValidator();
        }

        #region Happy Path Tests

        [Fact]
        public async Task Validate_WithValidTitle_ReturnsSuccess()
        {
            // Arrange
            var command = new CreateTodoItemCommand { Title = "Complete project report" };

            // Act
            ValidationResult result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Validate_WithTitleAtMaxLength_ReturnsSuccess()
        {
            // Arrange
            var title = new string('A', 200);
            var command = new CreateTodoItemCommand { Title = title };

            // Act
            ValidationResult result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WithTitleJustBelowMaxLength_ReturnsSuccess()
        {
            // Arrange
            var title = new string('A', 199);
            var command = new CreateTodoItemCommand { Title = title };

            // Act
            ValidationResult result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Validate_WithEmptyTitle_ReturnsFailure()
        {
            // Arrange
            var command = new CreateTodoItemCommand { Title = "" };

            // Act
            ValidationResult result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should().Be("The Title field is required.");
        }

        [Fact]
        public async Task Validate_WithNullTitle_ReturnsFailure()
        {
            // Arrange
            var command = new CreateTodoItemCommand { Title = null };

            // Act
            ValidationResult result = await _validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should().Be("The Title field is required.");
        }

        #endregion

        #region Exception Tests

        [Fact]
        public async Task Validate_WhenNullCommand_ThrowsArgumentNullException()
        {
            // Arrange
            CreateTodoItemCommand command = null;

            // Act & Assert
            await FluentActions.Invoking(() => _validator.ValidateAsync(command))
                .Should()
                .ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'command')");
        }

        #endregion

        #region Helper Methods

        // Reusable test utilities

        #endregion
    }
}