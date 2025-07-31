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
        private readonly Mock<IPipelineBehavior<TRequest, TResponse>> _mockNext;
        private readonly AuthorizationBehaviour<TRequest, TResponse> _authorizationBehaviour;

        public AuthorizationBehaviourTests()
        {
            _mockUser = new Mock<IUser>();
            _mockNext = new Mock<IPipelineBehavior<TRequest, TResponse>>();
            _authorizationBehaviour = new AuthorizationBehaviour<TRequest, TResponse>(_mockUser.Object);
        }

        #region Critical Path Tests
        // Tests that protect core business processes critical for revenue and operations

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithNoAuthorizeAttributes_AllowsRequest()
        {
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("user123");

            // Act
            var result = await _authorizationBehaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Request should be allowed if no authorize attributes");
            _mockNext.Verify(x => x.Handle(request, CancellationToken.None), Times.Once);
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthorizeAttributesAndAuthenticatedUser_AllowsRequest()
        {
            // Arrange
            var request = new TestRequest();
            request.GetType().AddCustomAttribute(new AuthorizeAttribute());
            _mockUser.Setup(u => u.Id).Returns("user123");

            // Act
            var result = await _authorizationBehaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Request should be allowed if user is authenticated");
            _mockNext.Verify(x => x.Handle(request, CancellationToken.None), Times.Once);
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthorizeAttributesAndUnauthenticatedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var request = new TestRequest();
            request.GetType().AddCustomAttribute(new AuthorizeAttribute());
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _authorizationBehaviour.Handle(request, _mockNext.Object, CancellationToken.None)
                .Should().ThrowAsync<UnauthorizedAccessException>("Unauthorized access should be thrown if user is not authenticated");
        }

        #endregion

        #region Happy Path Tests
        // Tests for standard, expected user workflows
        // Covers typical business scenarios under normal conditions

        [Fact]
        [Trait("Category", "Happy")]
        public async Task Handle_WithValidRequestAndNoAuthorizeAttributes_AllowsRequest()
        {
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("user123");

            // Act
            var result = await _authorizationBehaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Request should be allowed if no authorize attributes");
            _mockNext.Verify(x => x.Handle(request, CancellationToken.None), Times.Once);
        }

        #endregion

        #region Edge Case Tests
        // Tests for boundary conditions and unusual but valid scenarios
        // Ensures system handles limits and special cases gracefully

        [Fact]
        [Trait("Category", "Edge")]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Arrange
            TRequest? request = null;

            // Act & Assert
            await _authorizationBehaviour.Handle(request, _mockNext.Object, CancellationToken.None)
                .Should().ThrowAsync<ArgumentNullException>("Null request should throw ArgumentNullException");
        }

        #endregion

        #region Negative Tests
        // Tests for invalid inputs and expected failure scenarios
        // Ensures proper validation and error handling

        [Fact]
        [Trait("Category", "Negative")]
        public async Task Handle_WithNullAuthorizeAttributes_AllowsRequest()
        {
            // Arrange
            var request = new TestRequest();
            _mockUser.Setup(u => u.Id).Returns("user123");

            // Act
            var result = await _authorizationBehaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            result.Should().NotBeNull("Request should be allowed if no authorize attributes");
            _mockNext.Verify(x => x.Handle(request, CancellationToken.None), Times.Once);
        }

        #endregion

        #region Exception Handling Tests
        // Tests for system resilience and error recovery
        // Ensures graceful degradation when external systems fail

        [Fact]
        [Trait("Category", "Exception")]
        public async Task Handle_WhenUserRepositoryThrowsException_ThrowsException()
        {
            // Arrange
            var request = new TestRequest();
            request.GetType().AddCustomAttribute(new AuthorizeAttribute());
            _mockUser.Setup(u => u.Id).ThrowsAsync(new Exception("User repository error"));

            // Act & Assert
            await _authorizationBehaviour.Handle(request, _mockNext.Object, CancellationToken.None)
                .Should().ThrowAsync<Exception>("Exception should be thrown if user repository fails");
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // Ensures proper authentication, authorization, and audit trails

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithAuthorizeAttributesAndAuthenticatedUser_LogsAuditEvent()
        {
            // Arrange
            var request = new TestRequest();
            request.GetType().AddCustomAttribute(new AuthorizeAttribute());
            _mockUser.Setup(u => u.Id).Returns("user123");
            var mockAuditService = new Mock<IAuditService>();
            _authorizationBehaviour = new AuthorizationBehaviour<TRequest, TResponse>(_mockUser.Object, mockAuditService.Object);

            // Act
            await _authorizationBehaviour.Handle(request, _mockNext.Object, CancellationToken.None);

            // Assert
            mockAuditService.Verify(x => x.LogAuditEventAsync(It.Is<AuditEvent>(e =>
                e.UserId == "user123" &&
                e.EventType == "AUTHORIZATION_SUCCESS" &&
                !string.IsNullOrEmpty(e.IpAddress)
            )), Times.Once);
        }

        #endregion
    }
}