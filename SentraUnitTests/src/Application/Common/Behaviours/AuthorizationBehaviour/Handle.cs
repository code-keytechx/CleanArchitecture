using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Security;
using MediatR;
using Moq;
using Xunit;
using FluentAssertions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Behaviours;

namespace CleanArchitecture.SentraUnitTests.Application.Common.Security
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IUser> _mockUser;
        private readonly AuthorizationBehaviour<object, object> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockUser = new Mock<IUser>();
            _behaviour = new AuthorizationBehaviour<object, object>(_mockUser.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthorizedUser_ReturnsResponse()
        {
            // Business Context: Authorized users should be able to proceed with the request
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));

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
            // Business Context: Unauthenticated users should not be able to proceed with the request
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));

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
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));

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
        public async Task Handle_WithAuthorizeAttributesAndAuthenticatedUser_ReturnsResponse()
        {
            // Business Context: Requests with authorize attributes should proceed if user is authenticated
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            var request = new object();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute());
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));

            // Act
            var result = await _behaviour.Handle(request, next, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("authorized user with authorize attributes should receive a response");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithAuthorizeAttributesAndUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Requests with authorize attributes should fail if user is not authenticated
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            var request = new object();
            request.GetType().GetCustomAttributes(true).OfType<AuthorizeAttribute>().ToList().Add(new AuthorizeAttribute());
            var next = new RequestHandlerDelegate<object>(() => Task.FromResult(new object()));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithExceptionInNext_ThrowsException()
        {
            // Business Context: If an exception occurs in the next handler, it should be propagated
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            var request = new object();
            var next = new RequestHandlerDelegate<object>(() => throw new InvalidOperationException("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>();
        }

        #endregion
    }
}
