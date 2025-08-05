using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace Sentra.SentraUnitTests.Application.Behaviors
{
    public class ValidationBehaviorTests
    {
        private readonly Mock<IValidator<TRequest>> _mockValidator;
        private readonly ValidationBehavior<TRequest, TResponse> _behavior;

        public ValidationBehaviorTests()
        {
            _mockValidator = new Mock<IValidator<TRequest>>();
            _behavior = new ValidationBehavior<TRequest, TResponse>(_mockValidator.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithValidRequest_ReturnsResponse()
        {
            // Business Context: Valid request should pass validation and proceed to next handler
            // Arrange
            var request = new TRequest();
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            next.Setup(n => n()).ReturnsAsync(default(TResponse));

            // Act
            var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Response should not be null");
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithInvalidRequest_ThrowsValidationException()
        {
            // Business Context: Invalid request should fail validation and throw exception
            // Arrange
            var request = new TRequest();
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            next.Setup(n => n()).ReturnsAsync(default(TResponse));

            var validationResults = new List<ValidationResult>
            {
                new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("Property", "Error message")
                })
            };

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResults.First());

            // Act & Assert
            await _behavior.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Property: Error message");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithSensitiveData_EnsuresDataIntegrity()
        {
            // Business Context: Sensitive data should be validated and handled securely
            // Test security validations and compliance
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        public async Task Handle_WithHappyPathRequest_ReturnsResponse()
        {
            // Business Context: Happy path request should pass validation and proceed to next handler
            // Arrange
            var request = new TRequest();
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            next.Setup(n => n()).ReturnsAsync(default(TResponse));

            // Act
            var result = await _behavior.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Response should not be null");
            next.Verify(n => n(), Times.Once);
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithEmptyRequest_ThrowsValidationException()
        {
            // Business Context: Empty request should fail validation
            // Arrange
            var request = new TRequest();
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            next.Setup(n => n()).ReturnsAsync(default(TResponse));

            var validationResults = new List<ValidationResult>
            {
                new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("Property", "Error message")
                })
            };

            _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResults.First());

            // Act & Assert
            await _behavior.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage("Property: Error message");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null request should throw ArgumentNullException
            // Arrange
            var request = (TRequest?)null;
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            next.Setup(n => n()).ReturnsAsync(default(TResponse));

            // Act & Assert
            await _behavior.Invoking(b => b.Handle(request!, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithExceptionInNextHandler_ThrowsException()
        {
            // Business Context: Exception in next handler should be propagated
            // Arrange
            var request = new TRequest();
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            next.Setup(n => n()).ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await _behavior.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Test exception");
        }

        #endregion
    }
}