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

namespace CleanArchitecture.SentraUnitTests.Application.Behaviors
{
    public class AuthorizationBehaviourTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<IUser> _mockUser;
        private readonly AuthorizationBehaviour<TestRequest, TestResponse> _behaviour;

        public AuthorizationBehaviourTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockUser = new Mock<IUser>();
            _behaviour = new AuthorizationBehaviour<TestRequest, TestResponse>(_mockIdentityService.Object, _mockUser.Object);
        }

        #region Critical Path Tests
        // Tests that protect core revenue-generating business processes
        // REQUIRED for: payments, user data, file uploads, core workflows

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithNoAuthorizeAttributes_ReturnsResponse()
        {
            // Business Context: No authorization attributes means no checks, should proceed
            // Arrange
            var request = new TestRequest();
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().BeOfType<TestResponse>();
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndValidRole_ReturnsResponse()
        {
            // Business Context: Authenticated user with valid role should proceed
            // Arrange
            var request = new TestRequest { UserId = "user123", Roles = "Admin" };
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().BeOfType<TestResponse>();
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndInvalidRole_ThrowsForbiddenAccessException()
        {
            // Business Context: Authenticated user with invalid role should throw ForbiddenAccessException
            // Arrange
            var request = new TestRequest { UserId = "user123", Roles = "Admin" };
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndPolicy_ReturnsResponse()
        {
            // Business Context: Authenticated user with valid policy should proceed
            // Arrange
            var request = new TestRequest { UserId = "user123", Policy = "AdminPolicy" };
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user123", "AdminPolicy")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().BeOfType<TestResponse>();
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        [Trait("Category", "Critical")]
        public async Task Handle_WithAuthenticatedUserAndInvalidPolicy_ThrowsForbiddenAccessException()
        {
            // Business Context: Authenticated user with invalid policy should throw ForbiddenAccessException
            // Arrange
            var request = new TestRequest { UserId = "user123", Policy = "AdminPolicy" };
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user123", "AdminPolicy")).ReturnsAsync(false);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ForbiddenAccessException>();
        }

        #endregion

        #region Security & Compliance Tests
        // Tests for security boundaries and regulatory compliance
        // REQUIRED for: systems handling data/files/user input

        [Fact]
        [Trait("Category", "Security")]
        public async Task Handle_WithNullUserId_ThrowsUnauthorizedAccessException()
        {
            // Business Context: Null user ID should throw UnauthorizedAccessException
            // Arrange
            var request = new TestRequest { UserId = null };
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());
            _mockUser.Setup(u => u.Id).Returns((string?)null);

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion

        #region Happy Path Tests
        // Standard expected workflows under normal conditions

        [Fact]
        public async Task Handle_WithValidRequest_ReturnsResponse()
        {
            // Business Context: Valid request should return response
            // Arrange
            var request = new TestRequest { UserId = "user123", Roles = "Admin" };
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ReturnsAsync(true);

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().BeOfType<TestResponse>();
            next.Verify(n => n(), Times.Once);
        }

        #endregion

        #region Edge Case Tests
        // Boundary conditions and unusual but valid scenarios

        [Fact]
        public async Task Handle_WithEmptyRoles_ReturnsResponse()
        {
            // Business Context: Empty roles should proceed
            // Arrange
            var request = new TestRequest { UserId = "user123", Roles = "" };
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());
            _mockUser.Setup(u => u.Id).Returns("user123");

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().BeOfType<TestResponse>();
            next.Verify(n => n(), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullPolicy_ReturnsResponse()
        {
            // Business Context: Null policy should proceed
            // Arrange
            var request = new TestRequest { UserId = "user123", Policy = null };
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());
            _mockUser.Setup(u => u.Id).Returns("user123");

            // Act
            var result = await _behaviour.Handle(request, next.Object, CancellationToken.None);

            // Assert
            result.Should().BeOfType<TestResponse>();
            next.Verify(n => n(), Times.Once);
        }

        #endregion

        #region Negative Tests
        // Invalid inputs and expected failure scenarios

        [Fact]
        public async Task Handle_WithNullRequest_ThrowsArgumentNullException()
        {
            // Business Context: Null request should throw ArgumentNullException
            // Arrange
            TestRequest? request = null;
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request!, next.Object, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task Handle_WithNullNext_ThrowsArgumentNullException()
        {
            // Business Context: Null next should throw ArgumentNullException
            // Arrange
            var request = new TestRequest { UserId = "user123", Roles = "Admin" };
            RequestHandlerDelegate<TestResponse>? next = null;

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next!, CancellationToken.None))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Exception Handling Tests
        // System resilience and graceful degradation

        [Fact]
        public async Task Handle_WithExceptionInIsInRoleAsync_ThrowsException()
        {
            // Business Context: Exception in IsInRoleAsync should propagate
            // Arrange
            var request = new TestRequest { UserId = "user123", Roles = "Admin" };
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.IsInRoleAsync("user123", "Admin")).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<Exception>("Test exception");
        }

        [Fact]
        public async Task Handle_WithExceptionInAuthorizeAsync_ThrowsException()
        {
            // Business Context: Exception in AuthorizeAsync should propagate
            // Arrange
            var request = new TestRequest { UserId = "user123", Policy = "AdminPolicy" };
            var next = new Mock<RequestHandlerDelegate<TestResponse>>();
            next.Setup(n => n()).ReturnsAsync(new TestResponse());
            _mockUser.Setup(u => u.Id).Returns("user123");
            _mockIdentityService.Setup(i => i.AuthorizeAsync("user123", "AdminPolicy")).ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            await _behaviour.Invoking(b => b.Handle(request, next.Object, CancellationToken.None))
                .Should().ThrowAsync<Exception>("Test exception");
        }

        #endregion
    }

    public class TestRequest : IRequest<TestResponse>
    {
        public string? UserId { get; set; }
        public string? Roles { get; set; }
        public string? Policy { get; set; }
    }

    public class TestResponse
    {
    }
}