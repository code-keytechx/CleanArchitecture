using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.Tests.Common.Security
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<RequestHandlerDelegate<Response>> _mockNext;
        private readonly AuthorizationBehaviour<Request, Response> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockUser = new Mock<IUser>();
            _mockNext = new Mock<RequestHandlerDelegate<Response>>();
            _behaviour = new AuthorizationBehaviour<Request, Response>(_mockUser.Object, _mockNext.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthorizedUser_ReturnsResponse()
        {
            // Business Context: Authorized users should be able to access the resource.
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            var request = new Request { RequiredRole = "Admin" };
            request.GetType().GetCustomAttributes<AuthorizeAttribute>().Should().NotBeEmpty();

            // Act
            var result = await _behaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("authorized user should receive a response");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        [Trait("Category", "HappyPath")]
        public async Task Handle_WithAuthenticatedUser_ReturnsResponse()
        {
            // Business Context: Authenticated users should be able to access the resource.
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("test-user-123");
            var request = new Request { RequiredRole = "Admin" };
            request.GetType().GetCustomAttributes<AuthorizeAttribute>().Should().NotBeEmpty();

            // Act
            var result = await _behaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("authenticated user should receive a response");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Unauthenticated users should not be able to access the resource.
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            var request = new Request { RequiredRole = "Admin" };
            request.GetType().GetCustomAttributes<AuthorizeAttribute>().Should().NotBeEmpty();

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        [Trait("Category", "ExceptionHandling")]
        public async Task Handle_WithNullUserId_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Null user ID should result in an exception.
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string?)null);
            var request = new Request { RequiredRole = "Admin" };
            request.GetType().GetCustomAttributes<AuthorizeAttribute>().Should().NotBeEmpty();

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        // Helper classes
        [Authorize]
        public class Request : IRequest<Response>
        {
            public string? RequiredRole { get; set; }
        }

        public class Response { }
    }
}