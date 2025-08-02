using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.Tests.Common.Security
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly AuthorizationBehaviour<TestRequest, TestResponse> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _behaviour = new AuthorizationBehaviour<TestRequest, TestResponse>(_mockCurrentUser.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthorizedUser_ReturnsResponse()
        {
            // Business Context: Authorized users should be able to proceed with the request.
            // Arrange
            _mockCurrentUser.Setup(u => u.UserId).Returns("test-user-123");

            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("authorized user should receive a response");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthenticated users should not be able to proceed with the request.
            // Arrange
            _mockCurrentUser.Setup(u => u.UserId).Returns((string?)null);

            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithNoAuthorizeAttributes_ReturnsResponse()
        {
            // Business Context: Requests without authorize attributes should proceed without checks.
            // Arrange
            _mockCurrentUser.Setup(u => u.UserId).Returns((string?)null);

            var request = new TestRequestWithoutAuthorize();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("request without authorize attributes should receive a response");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithMultipleAuthorizeAttributes_ReturnsResponseForAuthorizedUser()
        {
            // Business Context: Requests with multiple authorize attributes should proceed if user is authorized.
            // Arrange
            _mockCurrentUser.Setup(u => u.UserId).Returns("test-user-123");

            var request = new TestRequestWithMultipleAuthorize();
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("authorized user should receive a response");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null requests should be handled gracefully.
            // Arrange
            TestRequest? request = null;
            var next = new RequestHandlerDelegate<TestResponse>(() => Task.FromResult(new TestResponse()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request!, next, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInNext_ThrowsException()
        {
            // Business Context: Exceptions in the next handler should propagate.
            // Arrange
            _mockCurrentUser.Setup(u => u.UserId).Returns("test-user-123");

            var request = new TestRequest();
            var next = new RequestHandlerDelegate<TestResponse>(() => throw new InvalidOperationException("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>();
        }

        #endregion
    }

    // Helper classes for testing
    [Authorize]
    public class TestRequest : IRequest<TestResponse> { }

    [Authorize]
    [Authorize]
    public class TestRequestWithMultipleAuthorize : IRequest<TestResponse> { }

    public class TestRequestWithoutAuthorize : IRequest<TestResponse> { }

    public class TestResponse { }
}