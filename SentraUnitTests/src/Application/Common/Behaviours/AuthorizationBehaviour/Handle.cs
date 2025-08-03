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

namespace CleanArchitecture.SentraUnitTests.Application.Common.Security
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IUser> _mockUser;
        private readonly AuthorizationBehaviour<MockRequest, MockResponse> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockUser = new Mock<IUser>();
            _behaviour = new AuthorizationBehaviour<MockRequest, MockResponse>(_mockUser.Object, null);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthorizedUser_ReturnsResponse()
        {
            // Business Context: Authorized users should be able to proceed with the request
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            var request = new MockRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should be returned for authorized users");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithUnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthorized users should not be able to proceed with the request
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            var request = new MockRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));

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
            // Business Context: Requests without authorize attributes should proceed without checks
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            var request = new MockRequestWithoutAuthorize { SomeProperty = "test" };
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("response should be returned for requests without authorize attributes");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        [Trait("Category", "EdgeCase")]
        public async Task Handle_WithEmptyUserId_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Requests with empty user ID should be treated as unauthorized
            // Arrange
            _mockUser.Setup(u => u.Id).Returns(string.Empty);
            var request = new MockRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null requests should be handled gracefully
            // Arrange
            MockRequest? request = null;
            var next = new RequestHandlerDelegate<MockResponse>(() => Task.FromResult(new MockResponse()));

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
            // Business Context: Exceptions in the next handler should propagate
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            var request = new MockRequest { RequiredRole = "Admin" };
            var next = new RequestHandlerDelegate<MockResponse>(() => throw new InvalidOperationException("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>();
        }

        #endregion

        // Helper classes for testing
        [Authorize(Roles = "Admin")]
        public class MockRequest : IRequest<MockResponse>
        {
            public string? RequiredRole { get; set; }
        }

        public class MockRequestWithoutAuthorize : IRequest<MockResponse>
        {
            public string? SomeProperty { get; set; }
        }

        public class MockResponse
        {
        }
    }
}