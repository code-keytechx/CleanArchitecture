using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using FluentValidation;
using Xunit;

// MANDATORY: Extract EXACT namespace from source code and add using statement
// Assuming the source code is part of the Sentra.Application.Common.Behaviours namespace
using Sentra.Application.Common.Behaviours;

namespace Sentra.SentraUnitTests.Application.Behaviors
{
    public class ValidationBehaviorTests
    {
        private readonly Mock<IEnumerable<IValidator<TRequest>>> _mockValidators;
        private readonly ValidationBehavior<TRequest, TResponse> _behavior;

        public ValidationBehaviorTests()
        {
            _mockValidators = new Mock<IEnumerable<IValidator<TRequest>>>();
            _behavior = new ValidationBehavior<TRequest, TResponse>(_mockValidators.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithValidRequest_DeliversCoreBusinessValue()
        {
            // Business Context: Valid request should pass validation and proceed to next handler
            // Arrange
            var request = new TRequest();
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TResponse());
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _behavior.Handle(request, next.Object, cancellationToken);

            // Assert
            result.Should().BeOfType<TResponse>("valid request should proceed to next handler");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithInvalidRequest_ThrowsValidationException()
        {
            // Business Context: Invalid request should throw ValidationException
            // Arrange
            var request = new TRequest();
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            var cancellationToken = CancellationToken.None;

            var mockValidator = new Mock<IValidator<TRequest>>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TRequest>>(), cancellationToken))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
                {
                    new FluentValidation.Results.ValidationFailure("PropertyName", "Error message")
                }));

            _mockValidators.Setup(v => v.GetEnumerator()).Returns(new List<IValidator<TRequest>> { mockValidator.Object }.GetEnumerator());

            // Act & Assert
            await _behavior.Invoking(b => b.Handle(request, next.Object, cancellationToken))
                .Should().ThrowAsync<ValidationException>("invalid request should throw ValidationException");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithNoValidators_DeliversCoreBusinessValue()
        {
            // Business Context: No validators should pass validation and proceed to next handler
            // Arrange
            _mockValidators.Setup(v => v.GetEnumerator()).Returns(new List<IValidator<TRequest>>().GetEnumerator());
            var request = new TRequest();
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TResponse());
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _behavior.Handle(request, next.Object, cancellationToken);

            // Assert
            result.Should().BeOfType<TResponse>("no validators should proceed to next handler");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithMultipleValidators_AllValidatorsCalled()
        {
            // Business Context: Multiple validators should all be called
            // Arrange
            var request = new TRequest();
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TResponse());
            var cancellationToken = CancellationToken.None;

            var mockValidator1 = new Mock<IValidator<TRequest>>();
            mockValidator1.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TRequest>>(), cancellationToken))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var mockValidator2 = new Mock<IValidator<TRequest>>();
            mockValidator2.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TRequest>>(), cancellationToken))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _mockValidators.Setup(v => v.GetEnumerator()).Returns(new List<IValidator<TRequest>> { mockValidator1.Object, mockValidator2.Object }.GetEnumerator());

            // Act
            await _behavior.Handle(request, next.Object, cancellationToken);

            // Assert
            mockValidator1.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TRequest>>(), cancellationToken), Times.Once);
            mockValidator2.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<TRequest>>(), cancellationToken), Times.Once);
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null request should throw ArgumentNullException
            // Arrange
            TRequest? request = null;
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            var cancellationToken = CancellationToken.None;

            // Act & Assert
            await _behavior.Invoking(b => b.Handle(request!, next.Object, cancellationToken))
                .Should().ThrowAsync<ArgumentNullException>("null request should throw ArgumentNullException");
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithValidationFailure_ThrowsValidationException()
        {
            // Business Context: Validation failure should throw ValidationException
            // Arrange
            var request = new TRequest();
            var next = new Mock<RequestHandlerDelegate<TResponse>>();
            var cancellationToken = CancellationToken.None;

            var mockValidator = new Mock<IValidator<TRequest>>();
            mockValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TRequest>>(), cancellationToken))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure>
                {
                    new FluentValidation.Results.ValidationFailure("PropertyName", "Error message")
                }));

            _mockValidators.Setup(v => v.GetEnumerator()).Returns(new List<IValidator<TRequest>> { mockValidator.Object }.GetEnumerator());

            // Act & Assert
            await _behavior.Invoking(b => b.Handle(request, next.Object, cancellationToken))
                .Should().ThrowAsync<ValidationException>("validation failure should throw ValidationException");
        }

        #endregion
    }

    // Placeholder classes for generic types
    public class TRequest { }
    public class TResponse { }
}