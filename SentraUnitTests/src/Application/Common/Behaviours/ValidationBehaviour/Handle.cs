using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using MediatR;
using FluentValidation;
using FluentValidation.Results;

namespace YourProject.Tests
{
    public class ValidationPipelineBehaviorTests
    {
        private readonly Mock<IEnumerable<IValidator<TestRequest>>> _mockValidators;
        private readonly ValidationPipelineBehavior<TestRequest, TestResponse> _behavior;

        public ValidationPipelineBehaviorTests()
        {
            _mockValidators = new Mock<IEnumerable<IValidator<TestRequest>>>();
            _behavior = new ValidationPipelineBehavior<TestRequest, TestResponse>(_mockValidators.Object);
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
            var request = new TestRequest { UserId = "test-user-123", RequiredRole = "Admin" };
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            mockNext.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(new TestResponse());

            // Act
            var result = await _behavior.Handle(request, mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should not be null");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithInvalidRole_ThrowsValidationException()
        {
            // Business Context: Invalid role should trigger validation failure
            // Arrange
            var request = new TestRequest { UserId = "test-user-123", RequiredRole = "Guest" };
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            var mockValidator = new Mock<IValidator<TestRequest>>();
            mockValidator.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new ValidationFailure("RequiredRole", "Role is not valid") }));
            _mockValidators.Setup(x => x.GetEnumerator()).Returns(new List<IValidator<TestRequest>> { mockValidator.Object }.GetEnumerator());

            // Act & Assert
            await _behavior.Invoking(b => b.Handle(request, mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<ValidationException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithValidRequest_ReturnsResponse()
        {
            // Business Context: Valid request should return a response
            // Arrange
            var request = new TestRequest { UserId = "test-user-123", RequiredRole = "Admin" };
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            mockNext.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(new TestResponse());

            // Act
            var result = await _behavior.Handle(request, mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().BeOfType<TestResponse>("response should be of type TestResponse");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithNoValidators_ReturnsResponse()
        {
            // Business Context: No validators should not affect the response
            // Arrange
            _mockValidators.Setup(x => x.GetEnumerator()).Returns(new List<IValidator<TestRequest>>().GetEnumerator());
            var request = new TestRequest { UserId = "test-user-123", RequiredRole = "Admin" };
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            mockNext.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(new TestResponse());

            // Act
            var result = await _behavior.Handle(request, mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().BeOfType<TestResponse>("response should be of type TestResponse");
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
            TestRequest? request = null;
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();

            // Act & Assert
            await _behavior.Invoking(b => b.Handle(request!, mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithMultipleValidationFailures_ThrowsValidationException()
        {
            // Business Context: Multiple validation failures should throw ValidationException
            // Arrange
            var request = new TestRequest { UserId = "test-user-123", RequiredRole = "Guest" };
            var mockNext = new Mock<RequestHandlerDelegate<TestResponse>>();
            var mockValidator1 = new Mock<IValidator<TestRequest>>();
            mockValidator1.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new ValidationFailure("RequiredRole", "Role is not valid") }));
            var mockValidator2 = new Mock<IValidator<TestRequest>>();
            mockValidator2.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure> { new ValidationFailure("UserId", "User ID is not valid") }));
            _mockValidators.Setup(x => x.GetEnumerator()).Returns(new List<IValidator<TestRequest>> { mockValidator1.Object, mockValidator2.Object }.GetEnumerator());

            // Act & Assert
            await _behavior.Invoking(b => b.Handle(request, mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<ValidationException>();
        }

        #endregion
    }

    public class TestRequest : IRequest<TestResponse>
    {
        public string? UserId { get; set; }
        public string? RequiredRole { get; set; }
    }

    public class TestResponse : IResponse
    {
    }

    public interface IResponse
    {
    }

    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v =>
                        v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .Where(r => r.Errors.Any())
                    .SelectMany(r => r.Errors)
                    .ToList();

                if (failures.Any())
                    throw new ValidationException(failures);
            }

            return await next();
        }
    }
}