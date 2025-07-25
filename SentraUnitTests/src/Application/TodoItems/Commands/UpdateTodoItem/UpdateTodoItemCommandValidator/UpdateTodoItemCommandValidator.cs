using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem.Tests
{
    using Application.TodoItems.Commands.UpdateTodoItem;
    using FluentValidation.Results;

    public class UpdateTodoItemCommandValidatorTests
    {
        // Test data - varied and realistic
        private readonly UpdateTodoItemCommand _validCommand = new UpdateTodoItemCommand { Title = "Complete project report" };
        private readonly UpdateTodoItemCommand _titleTooLongCommand = new UpdateTodoItemCommand { Title = new string('a', 201) };
        private readonly UpdateTodoItemCommand _emptyTitleCommand = new UpdateTodoItemCommand { Title = "" };

        // Mock declarations
        private readonly UpdateTodoItemCommandValidator _validator;

        // Setup/Constructor
        public UpdateTodoItemCommandValidatorTests()
        {
            _validator = new UpdateTodoItemCommandValidator();
        }

        #region Happy Path Tests

        [Fact]
        public async Task Validate_WithValidCommand_ReturnsSuccess()
        {
            // Arrange
            var command = _validCommand;

            // Act
            ValidationResult validationResult = await _validator.ValidateAsync(command);

            // Assert
            validationResult.IsValid.Should().BeTrue();
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task Validate_WithTitleOverMaxLength_ReturnsFailure()
        {
            // Arrange
            var command = _titleTooLongCommand;

            // Act
            ValidationResult validationResult = await _validator.ValidateAsync(command);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.First().PropertyName.Should().Be("Title");
            validationResult.Errors.First().ErrorMessage.Should().Be("The Title field must be a maximum length of 200.");
        }

        #endregion

        #region Negative Tests

        [Fact]
        public async Task Validate_WithEmptyTitle_ReturnsFailure()
        {
            // Arrange
            var command = _emptyTitleCommand;

            // Act
            ValidationResult validationResult = await _validator.ValidateAsync(command);

            // Assert
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1);
            validationResult.Errors.First().PropertyName.Should().Be("Title");
            validationResult.Errors.First().ErrorMessage.Should().Be("The Title field is required.");
        }

        #endregion

        #region Exception Tests

        // No exceptions expected in this simple validator

        #endregion

        #region Helper Methods

        // No reusable test utilities needed for this simple validator

        #endregion
    }
}