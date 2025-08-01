using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.Tests
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IUser> _mockUser;
        private readonly Mock<IPipelineBehavior<object, object>> _mockNext;
        private readonly AuthorizationBehaviour<object, object> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockUser = new Mock<IUser>();
            _mockNext = new Mock<IPipelineBehavior<object, object>>();
            _behaviour = new AuthorizationBehaviour<object, object>(_mockUser.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUser_ReturnsResponse()
        {
            // Business Context: Ensure authenticated user can proceed
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("authenticated-user-id");
            var request = new object();

            // Act
            var result = await _behaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Response should not be null");
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Ensure unauthenticated user is blocked
            // Arrange
            _mockUser.Setup(u => u.Id).Returns((string)null);
            var request = new object();

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>("UnauthorizedAccessException should be thrown for unauthenticated users");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithAuthorizeAttribute_RequiresAuthentication()
        {
            // Business Context: Ensure authorization attribute requires authentication
            // Arrange
            var request = new object();
            var authorizeAttribute = new AuthorizeAttribute();
            request.GetType().GetCustomAttributes<AuthorizeAttribute>().Add(authorizeAttribute);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>("UnauthorizedAccessException should be thrown for unauthenticated users with Authorize attribute");
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        public async Task Handle_WithValidRequest_ReturnsResponse()
        {
            // Business Context: Ensure valid request returns response
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("authenticated-user-id");
            var request = new object();

            // Act
            var result = await _behaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Response should not be null");
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Ensure null request throws ArgumentNullException
            // Arrange
            var request = (object)null;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>("ArgumentNullException should be thrown for null request");
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithInvalidUser_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Ensure invalid user throws UnauthorizedAccessException
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("invalid-user-id");
            var request = new object();

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>("UnauthorizedAccessException should be thrown for invalid user");
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithExceptionInNext_ThrowsException()
        {
            // Business Context: Ensure exception in next throws exception
            // Arrange
            _mockUser.Setup(u => u.Id).Returns("authenticated-user-id");
            _mockNext.Setup(n => n.Handle(It.IsAny<object>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("Test exception"));

            var request = new object();

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, _mockNext.Object, CancellationToken.None))
                .Should().ThrowAsync<InvalidOperationException>("InvalidOperationException should be thrown from next");
        }

        #endregion
    }
}